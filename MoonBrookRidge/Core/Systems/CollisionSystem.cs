using Microsoft.Xna.Framework;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.World.Maps;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Handles collision detection for the player and world objects
/// </summary>
public class CollisionSystem
{
    private WorldMap _worldMap;
    
    public CollisionSystem(WorldMap worldMap)
    {
        _worldMap = worldMap;
    }
    
    /// <summary>
    /// Check if a position is valid (within bounds and walkable)
    /// </summary>
    public bool IsPositionValid(Vector2 position)
    {
        // Check world boundaries
        if (!IsWithinWorldBounds(position))
        {
            return false;
        }
        
        // Check tile walkability
        Vector2 gridPos = WorldToGridPosition(position);
        Tile tile = _worldMap.GetTile((int)gridPos.X, (int)gridPos.Y);
        
        if (tile == null)
        {
            return false;
        }
        
        return IsTileWalkable(tile);
    }
    
    /// <summary>
    /// Check if position is within world boundaries
    /// </summary>
    public bool IsWithinWorldBounds(Vector2 position)
    {
        float minX = GameConstants.PLAYER_RADIUS;
        float maxX = (_worldMap.Width * GameConstants.TILE_SIZE) - GameConstants.PLAYER_RADIUS;
        float minY = GameConstants.PLAYER_RADIUS;
        float maxY = (_worldMap.Height * GameConstants.TILE_SIZE) - GameConstants.PLAYER_RADIUS;
        
        return position.X >= minX && position.X <= maxX &&
               position.Y >= minY && position.Y <= maxY;
    }
    
    /// <summary>
    /// Check if a tile is walkable
    /// </summary>
    private bool IsTileWalkable(Tile tile)
    {
        // Water tiles are not walkable
        if (tile.Type == TileType.Water || tile.Type == TileType.Water01)
        {
            return false;
        }
        
        // All other tile types are walkable
        // TODO: Add checks for buildings, rocks, trees when implemented
        return true;
    }
    
    /// <summary>
    /// Constrain position to valid area
    /// </summary>
    public Vector2 ClampToValid(Vector2 position)
    {
        float minX = GameConstants.PLAYER_RADIUS;
        float maxX = (_worldMap.Width * GameConstants.TILE_SIZE) - GameConstants.PLAYER_RADIUS;
        float minY = GameConstants.PLAYER_RADIUS;
        float maxY = (_worldMap.Height * GameConstants.TILE_SIZE) - GameConstants.PLAYER_RADIUS;
        
        return new Vector2(
            MathHelper.Clamp(position.X, minX, maxX),
            MathHelper.Clamp(position.Y, minY, maxY)
        );
    }
    
    /// <summary>
    /// Get corrected position that avoids collisions
    /// </summary>
    public Vector2 ResolveCollision(Vector2 currentPosition, Vector2 desiredPosition)
    {
        // If desired position is valid, use it
        if (IsPositionValid(desiredPosition))
        {
            return desiredPosition;
        }
        
        // Try sliding along X axis
        Vector2 slideX = new Vector2(desiredPosition.X, currentPosition.Y);
        if (IsPositionValid(slideX))
        {
            return slideX;
        }
        
        // Try sliding along Y axis
        Vector2 slideY = new Vector2(currentPosition.X, desiredPosition.Y);
        if (IsPositionValid(slideY))
        {
            return slideY;
        }
        
        // Can't move, stay at current position
        return currentPosition;
    }
    
    /// <summary>
    /// Convert world position to grid position
    /// </summary>
    private Vector2 WorldToGridPosition(Vector2 worldPosition)
    {
        return new Vector2(
            (int)(worldPosition.X / GameConstants.TILE_SIZE),
            (int)(worldPosition.Y / GameConstants.TILE_SIZE)
        );
    }
    
    /// <summary>
    /// Check collision between two rectangles
    /// </summary>
    public bool CheckRectangleCollision(Rectangle rect1, Rectangle rect2)
    {
        return rect1.Intersects(rect2);
    }
    
    /// <summary>
    /// Get the player's collision bounds
    /// </summary>
    public Rectangle GetPlayerBounds(Vector2 position)
    {
        const int PLAYER_WIDTH = 24;
        const int PLAYER_HEIGHT = 24;
        
        return new Rectangle(
            (int)(position.X - PLAYER_WIDTH / 2),
            (int)(position.Y - PLAYER_HEIGHT / 2),
            PLAYER_WIDTH,
            PLAYER_HEIGHT
        );
    }
}
