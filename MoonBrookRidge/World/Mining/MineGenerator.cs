using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.World.Tiles;

namespace MoonBrookRidge.World.Mining;

/// <summary>
/// Generates procedural mine levels with rocks and tunnels
/// </summary>
public class MineGenerator
{
    private Random _random;
    
    public MineGenerator(int seed)
    {
        _random = new Random(seed);
    }
    
    /// <summary>
    /// Generate a mine level with walls, floors, and mineable rocks
    /// </summary>
    public Tile[,] GenerateMineLevel(int level, int width, int height)
    {
        Tile[,] tiles = new Tile[width, height];
        
        // Initialize all tiles as walls
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                tiles[x, y] = new Tile(TileType.Stone, new Vector2(x, y));
            }
        }
        
        // Create entrance area (top-center)
        int entranceX = width / 2;
        int entranceY = 2;
        CreateRoom(tiles, entranceX - 2, entranceY - 1, 5, 4);
        
        // Create exit area (bottom-center) 
        int exitX = width / 2;
        int exitY = height - 5;
        CreateRoom(tiles, exitX - 2, exitY - 1, 5, 4);
        
        // Carve tunnels and rooms
        int numRooms = 3 + level; // More rooms on deeper levels
        for (int i = 0; i < numRooms; i++)
        {
            int roomX = _random.Next(5, width - 10);
            int roomY = _random.Next(5, height - 10);
            int roomWidth = _random.Next(4, 8);
            int roomHeight = _random.Next(4, 8);
            
            CreateRoom(tiles, roomX, roomY, roomWidth, roomHeight);
        }
        
        // Connect rooms with tunnels
        CreateTunnel(tiles, entranceX, entranceY + 3, width / 2, height / 2);
        CreateTunnel(tiles, width / 2, height / 2, exitX, exitY);
        
        // Add random tunnels
        for (int i = 0; i < numRooms; i++)
        {
            int startX = _random.Next(5, width - 5);
            int startY = _random.Next(5, height - 5);
            int endX = _random.Next(5, width - 5);
            int endY = _random.Next(5, height - 5);
            
            CreateTunnel(tiles, startX, startY, endX, endY);
        }
        
        return tiles;
    }
    
    /// <summary>
    /// Generate a mine level using configuration
    /// </summary>
    public Tile[,] GenerateMineLevelFromConfig(MineGenConfig config, int level)
    {
        return MineGenConfigApplier.GenerateMineLevel(config, level);
    }
    
    /// <summary>
    /// Create a rectangular room
    /// </summary>
    private void CreateRoom(Tile[,] tiles, int startX, int startY, int width, int height)
    {
        int maxX = Math.Min(startX + width, tiles.GetLength(0));
        int maxY = Math.Min(startY + height, tiles.GetLength(1));
        
        for (int x = Math.Max(0, startX); x < maxX; x++)
        {
            for (int y = Math.Max(0, startY); y < maxY; y++)
            {
                tiles[x, y] = new Tile(TileType.Dirt, new Vector2(x, y));
            }
        }
    }
    
    /// <summary>
    /// Create a tunnel connecting two points
    /// </summary>
    private void CreateTunnel(Tile[,] tiles, int x1, int y1, int x2, int y2)
    {
        int x = x1;
        int y = y1;
        
        // Horizontal first, then vertical
        while (x != x2)
        {
            if (x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1))
            {
                tiles[x, y] = new Tile(TileType.Dirt, new Vector2(x, y));
                
                // Make tunnel 2-wide for easier navigation
                if (y + 1 < tiles.GetLength(1))
                    tiles[x, y + 1] = new Tile(TileType.Dirt, new Vector2(x, y + 1));
            }
            
            x += (x < x2) ? 1 : -1;
        }
        
        while (y != y2)
        {
            if (x >= 0 && x < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1))
            {
                tiles[x, y] = new Tile(TileType.Dirt, new Vector2(x, y));
                
                // Make tunnel 2-wide
                if (x + 1 < tiles.GetLength(0))
                    tiles[x + 1, y] = new Tile(TileType.Dirt, new Vector2(x + 1, y));
            }
            
            y += (y < y2) ? 1 : -1;
        }
    }
    
    /// <summary>
    /// Get spawn positions for mineable rocks
    /// Returns positions where rocks should be placed (next to tunnels)
    /// </summary>
    public List<Vector2> GetRockSpawnPositions(Tile[,] tiles, int numRocks)
    {
        List<Vector2> positions = new List<Vector2>();
        List<Vector2> candidates = new List<Vector2>();
        
        // Find wall tiles adjacent to floor tiles (good rock positions)
        for (int x = 1; x < tiles.GetLength(0) - 1; x++)
        {
            for (int y = 1; y < tiles.GetLength(1) - 1; y++)
            {
                if (tiles[x, y].Type == TileType.Stone)
                {
                    // Check if adjacent to a floor tile
                    bool adjacentToFloor = 
                        (tiles[x - 1, y].Type == TileType.Dirt) ||
                        (tiles[x + 1, y].Type == TileType.Dirt) ||
                        (tiles[x, y - 1].Type == TileType.Dirt) ||
                        (tiles[x, y + 1].Type == TileType.Dirt);
                    
                    if (adjacentToFloor)
                    {
                        candidates.Add(new Vector2(x, y));
                    }
                }
            }
        }
        
        // Randomly select positions from candidates
        for (int i = 0; i < Math.Min(numRocks, candidates.Count); i++)
        {
            int index = _random.Next(candidates.Count);
            positions.Add(candidates[index]);
            candidates.RemoveAt(index);
        }
        
        return positions;
    }
}
