using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.World.Tiles;
using System.Collections.Generic;

namespace MoonBrookRidge.World.Interiors;

/// <summary>
/// Represents an interior scene (farmhouse, shop, etc.)
/// </summary>
public class InteriorScene
{
    protected Tile[,] _tiles;
    protected int _width;
    protected int _height;
    protected List<InteriorObject> _objects;
    protected Vector2 _doorPosition; // Position where player exits to exterior
    protected Vector2 _playerSpawnPosition; // Where player spawns when entering
    
    public string Name { get; protected set; }
    public int Width => _width;
    public int Height => _height;
    public Vector2 DoorPosition => _doorPosition;
    public Vector2 PlayerSpawnPosition => _playerSpawnPosition;
    
    public InteriorScene(string name, int width, int height)
    {
        Name = name;
        _width = width;
        _height = height;
        _tiles = new Tile[width, height];
        _objects = new List<InteriorObject>();
    }
    
    /// <summary>
    /// Get tile at grid position
    /// </summary>
    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            return _tiles[x, y];
        }
        return null;
    }
    
    /// <summary>
    /// Set tile at grid position
    /// </summary>
    protected void SetTile(int x, int y, Tile tile)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            _tiles[x, y] = tile;
        }
    }
    
    /// <summary>
    /// Add an interior object (furniture, decoration, etc.)
    /// </summary>
    public void AddObject(InteriorObject obj)
    {
        _objects.Add(obj);
    }
    
    /// <summary>
    /// Get all interior objects
    /// </summary>
    public List<InteriorObject> GetObjects()
    {
        return _objects;
    }
    
    /// <summary>
    /// Check if position is the door (exit point)
    /// </summary>
    public bool IsAtDoor(Vector2 position)
    {
        Vector2 gridPos = new Vector2(
            (int)(position.X / GameConstants.TILE_SIZE),
            (int)(position.Y / GameConstants.TILE_SIZE)
        );
        
        float distance = Vector2.Distance(gridPos, _doorPosition);
        return distance < 1.5f; // Within 1.5 tiles of door
    }
    
    /// <summary>
    /// Draw the interior scene
    /// </summary>
    public virtual void Draw(SpriteBatch spriteBatch, Texture2D floorAtlas, Texture2D objectAtlas)
    {
        // Draw floor tiles
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_tiles[x, y] != null)
                {
                    _tiles[x, y].Draw(spriteBatch);
                }
            }
        }
        
        // Draw objects
        foreach (var obj in _objects)
        {
            obj.Draw(spriteBatch, objectAtlas);
        }
    }
}
