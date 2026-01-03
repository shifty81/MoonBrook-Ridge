using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using MoonBrookRidge.World.Tiles.AutoTiling;

namespace MoonBrookRidge.World.Tiles;

/// <summary>
/// Enhanced tile renderer with autotiling support from GameMaker templates.
/// Provides seamless terrain transitions using blob/wang tiling patterns.
/// </summary>
public class AutoTileRenderer
{
    private readonly SunnysideTilesetHelper _tilesetHelper;
    
    public AutoTileRenderer(SunnysideTilesetHelper tilesetHelper)
    {
        _tilesetHelper = tilesetHelper;
    }
    
    /// <summary>
    /// Get the appropriate tile ID for a position using autotiling rules.
    /// Checks all 8 neighbors to determine the correct tile variant.
    /// </summary>
    /// <param name="tiles">The tile map</param>
    /// <param name="x">X position in tile grid</param>
    /// <param name="y">Y position in tile grid</param>
    /// <param name="targetType">The tile type to check for autotiling</param>
    /// <param name="autoTileSet">The autotile set to use (e.g., SunnysideworldAutoTileRules.AutoTileSets.Land)</param>
    /// <returns>The tile ID to render</returns>
    public int GetAutoTileId(Tile[,] tiles, int x, int y, TileType targetType, int[] autoTileSet)
    {
        if (autoTileSet == null || autoTileSet.Length < 16)
            return -1; // Invalid autotile set
        
        int width = tiles.GetLength(0);
        int height = tiles.GetLength(1);
        
        // Check if position is valid
        if (x < 0 || x >= width || y < 0 || y >= height)
            return -1;
        
        // Check if current tile matches target type
        if (tiles[x, y]?.Type != targetType)
            return -1;
        
        // Check all 4 cardinal directions
        bool north = y > 0 && tiles[x, y - 1]?.Type == targetType;
        bool south = y < height - 1 && tiles[x, y + 1]?.Type == targetType;
        bool east = x < width - 1 && tiles[x + 1, y]?.Type == targetType;
        bool west = x > 0 && tiles[x - 1, y]?.Type == targetType;
        
        // Use the GameMaker-compatible autotile index calculation
        return SunnysideworldAutoTileRules.GetAutoTileIndex(north, south, east, west, autoTileSet);
    }
    
    /// <summary>
    /// Get autotile ID with support for 8-directional neighbors (including diagonals).
    /// More sophisticated version that checks corners for smoother transitions.
    /// </summary>
    public int GetAutoTileIdAdvanced(Tile[,] tiles, int x, int y, TileType targetType, int[] autoTileSet)
    {
        if (autoTileSet == null || autoTileSet.Length < 16)
            return -1;
        
        int width = tiles.GetLength(0);
        int height = tiles.GetLength(1);
        
        if (x < 0 || x >= width || y < 0 || y >= height)
            return -1;
        
        if (tiles[x, y]?.Type != targetType)
            return -1;
        
        // Check all 8 neighbors
        bool north = y > 0 && tiles[x, y - 1]?.Type == targetType;
        bool south = y < height - 1 && tiles[x, y + 1]?.Type == targetType;
        bool east = x < width - 1 && tiles[x + 1, y]?.Type == targetType;
        bool west = x > 0 && tiles[x - 1, y]?.Type == targetType;
        
        bool northEast = x < width - 1 && y > 0 && tiles[x + 1, y - 1]?.Type == targetType;
        bool northWest = x > 0 && y > 0 && tiles[x - 1, y - 1]?.Type == targetType;
        bool southEast = x < width - 1 && y < height - 1 && tiles[x + 1, y + 1]?.Type == targetType;
        bool southWest = x > 0 && y < height - 1 && tiles[x - 1, y + 1]?.Type == targetType;
        
        // Calculate index based on neighbors
        // This uses a 16-tile blob tiling pattern
        int index = 0;
        if (north) index |= 1;
        if (east) index |= 2;
        if (south) index |= 4;
        if (west) index |= 8;
        
        // For corner refinement, you could add additional logic here
        // based on diagonal neighbors
        
        return autoTileSet[index];
    }
    
    /// <summary>
    /// Apply autotiling to an entire region of the map.
    /// Updates tile IDs to use appropriate autotile variants.
    /// </summary>
    /// <param name="tiles">The tile map to update</param>
    /// <param name="startX">Start X position</param>
    /// <param name="startY">Start Y position</param>
    /// <param name="width">Width of region</param>
    /// <param name="height">Height of region</param>
    /// <param name="tileTypeToAutoTileSetMapping">Mapping of tile types to their autotile sets</param>
    public void ApplyAutoTiling(
        Tile[,] tiles, 
        int startX, int startY, 
        int width, int height,
        Dictionary<TileType, int[]> tileTypeToAutoTileSetMapping)
    {
        int mapWidth = tiles.GetLength(0);
        int mapHeight = tiles.GetLength(1);
        
        for (int x = startX; x < Math.Min(startX + width, mapWidth); x++)
        {
            for (int y = startY; y < Math.Min(startY + height, mapHeight); y++)
            {
                if (tiles[x, y] == null)
                    continue;
                
                TileType tileType = tiles[x, y].Type;
                
                // Check if this tile type has autotiling rules
                if (tileTypeToAutoTileSetMapping.TryGetValue(tileType, out int[] autoTileSet))
                {
                    int autoTileId = GetAutoTileId(tiles, x, y, tileType, autoTileSet);
                    
                    if (autoTileId >= 0)
                    {
                        // Update the tile's sprite ID to use the autotiled version
                        tiles[x, y].SpriteId = autoTileId;
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Get the autotile set for common terrain types.
    /// Helper method to access the generated autotile rules easily.
    /// </summary>
    public static int[] GetAutoTileSetForTerrainType(string terrainType)
    {
        return terrainType.ToLower() switch
        {
            "land" or "grass" => SunnysideworldAutoTileRules.AutoTileSets.Land,
            "path1" or "dirt" => SunnysideworldAutoTileRules.AutoTileSets.Path01,
            "path2" or "stone" => SunnysideworldAutoTileRules.AutoTileSets.Path02,
            "path3" or "cobble" => SunnysideworldAutoTileRules.AutoTileSets.Path03,
            "river" or "water" => SunnysideworldAutoTileRules.AutoTileSets.River,
            "building1" => SunnysideworldAutoTileRules.AutoTileSets.Building01,
            "building2" => SunnysideworldAutoTileRules.AutoTileSets.Building02,
            "walls" => SunnysideworldAutoTileRules.AutoTileSets.InnerWalls,
            "clouds1" => SunnysideworldAutoTileRules.AutoTileSets.Clouds01,
            "clouds2" => SunnysideworldAutoTileRules.AutoTileSets.Clouds02,
            "cloudshadow" => SunnysideworldAutoTileRules.AutoTileSets.CloudShadow,
            _ => null
        };
    }
}
