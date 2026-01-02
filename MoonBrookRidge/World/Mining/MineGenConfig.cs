using System.Collections.Generic;
using MoonBrookRidge.World.Tiles;

namespace MoonBrookRidge.World.Mining;

/// <summary>
/// Configuration for mine generation loaded from JSON
/// </summary>
public class MineGenConfig
{
    public int Width { get; set; } = 40;
    public int Height { get; set; } = 40;
    public int RandomSeedBase { get; set; } = 100;
    public RoomGenerationConfig Rooms { get; set; } = new();
    public TunnelConfig Tunnels { get; set; } = new();
    public EntranceConfig Entrance { get; set; } = new();
    public ExitConfig Exit { get; set; } = new();
    public TileTypeConfig TileTypes { get; set; } = new();
}

public class RoomGenerationConfig
{
    public int BaseCount { get; set; } = 3;
    public int CountPerLevel { get; set; } = 1; // Additional rooms per level
    public int MinWidth { get; set; } = 4;
    public int MaxWidth { get; set; } = 8;
    public int MinHeight { get; set; } = 4;
    public int MaxHeight { get; set; } = 8;
    public int MarginFromEdge { get; set; } = 5;
}

public class TunnelConfig
{
    public int Width { get; set; } = 2; // Tunnel width in tiles
    public int RandomTunnelsPerLevel { get; set; } = 1;
}

public class EntranceConfig
{
    public string Position { get; set; } = "top-center"; // top-center, top-left, etc.
    public int Width { get; set; } = 5;
    public int Height { get; set; } = 4;
}

public class ExitConfig
{
    public string Position { get; set; } = "bottom-center";
    public int Width { get; set; } = 5;
    public int Height { get; set; } = 4;
}

public class TileTypeConfig
{
    public string WallTile { get; set; } = "Stone";
    public string FloorTile { get; set; } = "Dirt";
}

/// <summary>
/// Helper class to apply mine generation configuration
/// </summary>
public static class MineGenConfigApplier
{
    /// <summary>
    /// Generate a mine level using configuration
    /// </summary>
    public static Tile[,] GenerateMineLevel(MineGenConfig config, int level)
    {
        var tiles = new Tile[config.Width, config.Height];
        var random = new System.Random(config.RandomSeedBase + level);
        
        // Parse tile types
        System.Enum.TryParse<TileType>(config.TileTypes.WallTile, true, out TileType wallType);
        System.Enum.TryParse<TileType>(config.TileTypes.FloorTile, true, out TileType floorType);
        
        // Initialize all tiles as walls
        for (int x = 0; x < config.Width; x++)
        {
            for (int y = 0; y < config.Height; y++)
            {
                tiles[x, y] = new Tile(wallType, new Microsoft.Xna.Framework.Vector2(x, y));
            }
        }
        
        // Create entrance area
        var entrance = CreateAreaFromPosition(config.Entrance.Position, config.Width, config.Height, 
                                             config.Entrance.Width, config.Entrance.Height);
        CreateRoom(tiles, entrance.X, entrance.Y, entrance.Width, entrance.Height, floorType);
        
        // Create exit area
        var exit = CreateAreaFromPosition(config.Exit.Position, config.Width, config.Height,
                                         config.Exit.Width, config.Exit.Height);
        CreateRoom(tiles, exit.X, exit.Y, exit.Width, exit.Height, floorType);
        
        // Create rooms
        int numRooms = config.Rooms.BaseCount + (level * config.Rooms.CountPerLevel);
        var roomCenters = new List<(int X, int Y)>
        {
            (entrance.X + entrance.Width / 2, entrance.Y + entrance.Height / 2),
            (exit.X + exit.Width / 2, exit.Y + exit.Height / 2)
        };
        
        for (int i = 0; i < numRooms; i++)
        {
            int roomX = random.Next(config.Rooms.MarginFromEdge, config.Width - config.Rooms.MaxWidth - config.Rooms.MarginFromEdge);
            int roomY = random.Next(config.Rooms.MarginFromEdge, config.Height - config.Rooms.MaxHeight - config.Rooms.MarginFromEdge);
            int roomWidth = random.Next(config.Rooms.MinWidth, config.Rooms.MaxWidth);
            int roomHeight = random.Next(config.Rooms.MinHeight, config.Rooms.MaxHeight);
            
            CreateRoom(tiles, roomX, roomY, roomWidth, roomHeight, floorType);
            roomCenters.Add((roomX + roomWidth / 2, roomY + roomHeight / 2));
        }
        
        // Connect rooms with tunnels
        for (int i = 0; i < roomCenters.Count - 1; i++)
        {
            CreateTunnel(tiles, roomCenters[i].X, roomCenters[i].Y, 
                        roomCenters[i + 1].X, roomCenters[i + 1].Y, 
                        config.Tunnels.Width, floorType);
        }
        
        // Add random tunnels for variety
        int numRandomTunnels = config.Tunnels.RandomTunnelsPerLevel * level;
        for (int i = 0; i < numRandomTunnels; i++)
        {
            int startIdx = random.Next(roomCenters.Count);
            int endIdx = random.Next(roomCenters.Count);
            if (startIdx != endIdx)
            {
                CreateTunnel(tiles, roomCenters[startIdx].X, roomCenters[startIdx].Y,
                           roomCenters[endIdx].X, roomCenters[endIdx].Y,
                           config.Tunnels.Width, floorType);
            }
        }
        
        return tiles;
    }
    
    private static (int X, int Y, int Width, int Height) CreateAreaFromPosition(
        string position, int worldWidth, int worldHeight, int areaWidth, int areaHeight)
    {
        int x = 0, y = 0;
        
        switch (position.ToLower())
        {
            case "top-left":
                x = 1;
                y = 1;
                break;
            case "top-center":
                x = worldWidth / 2 - areaWidth / 2;
                y = 2;
                break;
            case "top-right":
                x = worldWidth - areaWidth - 1;
                y = 1;
                break;
            case "bottom-left":
                x = 1;
                y = worldHeight - areaHeight - 1;
                break;
            case "bottom-center":
                x = worldWidth / 2 - areaWidth / 2;
                y = worldHeight - areaHeight - 3;
                break;
            case "bottom-right":
                x = worldWidth - areaWidth - 1;
                y = worldHeight - areaHeight - 1;
                break;
            case "center":
                x = worldWidth / 2 - areaWidth / 2;
                y = worldHeight / 2 - areaHeight / 2;
                break;
            default: // top-center
                x = worldWidth / 2 - areaWidth / 2;
                y = 2;
                break;
        }
        
        return (x, y, areaWidth, areaHeight);
    }
    
    private static void CreateRoom(Tile[,] tiles, int startX, int startY, int width, int height, TileType floorType)
    {
        int maxX = System.Math.Min(startX + width, tiles.GetLength(0));
        int maxY = System.Math.Min(startY + height, tiles.GetLength(1));
        
        for (int x = System.Math.Max(0, startX); x < maxX; x++)
        {
            for (int y = System.Math.Max(0, startY); y < maxY; y++)
            {
                tiles[x, y] = new Tile(floorType, new Microsoft.Xna.Framework.Vector2(x, y));
            }
        }
    }
    
    private static void CreateTunnel(Tile[,] tiles, int x1, int y1, int x2, int y2, int width, TileType floorType)
    {
        int x = x1;
        int y = y1;
        
        // Horizontal first
        while (x != x2)
        {
            for (int w = 0; w < width; w++)
            {
                if (x >= 0 && x < tiles.GetLength(0) && y + w >= 0 && y + w < tiles.GetLength(1))
                {
                    tiles[x, y + w] = new Tile(floorType, new Microsoft.Xna.Framework.Vector2(x, y + w));
                }
            }
            x += (x < x2) ? 1 : -1;
        }
        
        // Then vertical
        while (y != y2)
        {
            for (int w = 0; w < width; w++)
            {
                if (x + w >= 0 && x + w < tiles.GetLength(0) && y >= 0 && y < tiles.GetLength(1))
                {
                    tiles[x + w, y] = new Tile(floorType, new Microsoft.Xna.Framework.Vector2(x + w, y));
                }
            }
            y += (y < y2) ? 1 : -1;
        }
    }
}
