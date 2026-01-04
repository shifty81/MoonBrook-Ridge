using System.Numerics;
using System.Collections.Generic;
using MoonBrookRidge.World.Tiles;

namespace MoonBrookRidge.World.Maps;

/// <summary>
/// Configuration for world generation loaded from JSON
/// </summary>
public class WorldGenConfig
{
    public int Width { get; set; } = 50;
    public int Height { get; set; } = 50;
    public int RandomSeed { get; set; } = 42;
    public List<BiomeRegion> Biomes { get; set; } = new();
    public List<PathDefinition> Paths { get; set; } = new();
    public MineEntranceConfig MineEntrance { get; set; } = new();
    public DefaultTerrainConfig DefaultTerrain { get; set; } = new();
}

/// <summary>
/// Defines a biome region in the world
/// </summary>
public class BiomeRegion
{
    public string Name { get; set; } = "";
    public int X { get; set; }
    public int Y { get; set; }
    public int Width { get; set; }
    public int Height { get; set; }
    public string Shape { get; set; } = "rectangle"; // rectangle, circle, ellipse
    public List<TileWeightConfig> TileWeights { get; set; } = new();
}

/// <summary>
/// Defines tile types and their probability weights
/// </summary>
public class TileWeightConfig
{
    public string TileType { get; set; } = "";
    public int Weight { get; set; } = 1;
}

/// <summary>
/// Defines a path between regions
/// </summary>
public class PathDefinition
{
    public string Name { get; set; } = "";
    public int StartX { get; set; }
    public int StartY { get; set; }
    public int EndX { get; set; }
    public int EndY { get; set; }
    public int Width { get; set; } = 1;
    public List<TileWeightConfig> TileWeights { get; set; } = new();
}

/// <summary>
/// Configuration for mine entrance placement
/// </summary>
public class MineEntranceConfig
{
    public int X { get; set; } = 10;
    public int Y { get; set; } = 40;
}

/// <summary>
/// Default terrain configuration for areas not covered by biomes
/// </summary>
public class DefaultTerrainConfig
{
    public List<TileWeightConfig> TileWeights { get; set; } = new();
}

/// <summary>
/// Helper class to apply world generation configuration
/// </summary>
public static class WorldGenConfigApplier
{
    /// <summary>
    /// Apply configuration to generate world tiles
    /// </summary>
    public static Tile[,] GenerateWorld(WorldGenConfig config)
    {
        var tiles = new Tile[config.Width, config.Height];
        var random = new System.Random(config.RandomSeed);
        
        // Initialize all tiles with default terrain
        for (int x = 0; x < config.Width; x++)
        {
            for (int y = 0; y < config.Height; y++)
            {
                TileType tileType = SelectTileType(config.DefaultTerrain.TileWeights, random);
                tiles[x, y] = new Tile(tileType, new Vector2(x, y));
            }
        }
        
        // Apply biomes
        foreach (var biome in config.Biomes)
        {
            ApplyBiome(tiles, biome, random);
        }
        
        // Apply paths
        foreach (var path in config.Paths)
        {
            ApplyPath(tiles, path, random);
        }
        
        // Place mine entrance
        if (config.MineEntrance.X >= 0 && config.MineEntrance.X < config.Width &&
            config.MineEntrance.Y >= 0 && config.MineEntrance.Y < config.Height)
        {
            tiles[config.MineEntrance.X, config.MineEntrance.Y] = 
                new Tile(TileType.MineEntrance, new Vector2(config.MineEntrance.X, config.MineEntrance.Y));
        }
        
        return tiles;
    }
    
    private static void ApplyBiome(Tile[,] tiles, BiomeRegion biome, System.Random random)
    {
        int width = tiles.GetLength(0);
        int height = tiles.GetLength(1);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                if (IsInBiome(x, y, biome))
                {
                    TileType tileType = SelectTileType(biome.TileWeights, random);
                    tiles[x, y] = new Tile(tileType, new Vector2(x, y));
                }
            }
        }
    }
    
    private static bool IsInBiome(int x, int y, BiomeRegion biome)
    {
        switch (biome.Shape.ToLower())
        {
            case "circle":
                {
                    int centerX = biome.X + biome.Width / 2;
                    int centerY = biome.Y + biome.Height / 2;
                    int radius = System.Math.Min(biome.Width, biome.Height) / 2;
                    int dx = x - centerX;
                    int dy = y - centerY;
                    return (dx * dx + dy * dy) <= (radius * radius);
                }
            case "ellipse":
                {
                    int centerX = biome.X + biome.Width / 2;
                    int centerY = biome.Y + biome.Height / 2;
                    float radiusX = biome.Width / 2.0f;
                    float radiusY = biome.Height / 2.0f;
                    float dx = (x - centerX) / radiusX;
                    float dy = (y - centerY) / radiusY;
                    return (dx * dx + dy * dy) <= 1.0f;
                }
            case "rectangle":
            default:
                return x >= biome.X && x < biome.X + biome.Width &&
                       y >= biome.Y && y < biome.Y + biome.Height;
        }
    }
    
    private static void ApplyPath(Tile[,] tiles, PathDefinition path, System.Random random)
    {
        int width = tiles.GetLength(0);
        int height = tiles.GetLength(1);
        
        // Draw path using Bresenham's line algorithm with width
        int x0 = path.StartX;
        int y0 = path.StartY;
        int x1 = path.EndX;
        int y1 = path.EndY;
        
        int dx = System.Math.Abs(x1 - x0);
        int dy = System.Math.Abs(y1 - y0);
        int sx = x0 < x1 ? 1 : -1;
        int sy = y0 < y1 ? 1 : -1;
        int err = dx - dy;
        int halfWidth = path.Width / 2;
        
        while (true)
        {
            // Draw path with width
            for (int wx = -halfWidth; wx <= halfWidth; wx++)
            {
                for (int wy = -halfWidth; wy <= halfWidth; wy++)
                {
                    int px = x0 + wx;
                    int py = y0 + wy;
                    
                    if (px >= 0 && px < width && py >= 0 && py < height)
                    {
                        TileType tileType = SelectTileType(path.TileWeights, random);
                        tiles[px, py] = new Tile(tileType, new Vector2(px, py));
                    }
                }
            }
            
            if (x0 == x1 && y0 == y1) break;
            
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x0 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y0 += sy;
            }
        }
    }
    
    private static TileType SelectTileType(List<TileWeightConfig> weights, System.Random random)
    {
        if (weights == null || weights.Count == 0)
        {
            return TileType.Grass;
        }
        
        // Calculate total weight
        int totalWeight = 0;
        foreach (var weight in weights)
        {
            totalWeight += weight.Weight;
        }
        
        // Select random tile based on weights
        int randomValue = random.Next(totalWeight);
        int currentWeight = 0;
        
        foreach (var weight in weights)
        {
            currentWeight += weight.Weight;
            if (randomValue < currentWeight)
            {
                // Parse tile type from string
                if (System.Enum.TryParse<TileType>(weight.TileType, true, out TileType tileType))
                {
                    return tileType;
                }
            }
        }
        
        return TileType.Grass;
    }
}
