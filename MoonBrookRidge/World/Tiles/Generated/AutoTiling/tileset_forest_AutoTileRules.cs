using System.Collections.Generic;

namespace MoonBrookRidge.World.Tiles.AutoTiling;

/// <summary>
/// Auto-generated autotiling rules from GameMaker tileset: tileset_forest
/// This file is auto-generated. Do not edit manually.
/// </summary>
public static class ForestAutoTileRules
{
    /// <summary>
    /// Tile dimensions
    /// </summary>
    public const int TileWidth = 32;
    public const int TileHeight = 32;
    public const int TotalTiles = 180;
    
    /// <summary>
    /// Autotile set definitions
    /// Each set contains 16 tiles arranged for blob/wang tiling:
    /// Tiles are indexed for corners and edges to create seamless terrain
    /// </summary>
    public static class AutoTileSets
    {
        /// <summary>
        /// forest_01 autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] forest_01 = new int[]
        {
            11, 12, 13, 14, 15, 16, 17, 18,
            21, 22, 23, 24, 25, 26, 27, 0
        };

    }
    
    /// <summary>
    /// Get autotile ID based on neighboring tiles
    /// Uses the blob/wang tiling pattern
    /// </summary>
    /// <param name="north">True if tile to the north is same type</param>
    /// <param name="south">True if tile to the south is same type</param>
    /// <param name="east">True if tile to the east is same type</param>
    /// <param name="west">True if tile to the west is same type</param>
    /// <param name="autoTileSet">The autotile set to use</param>
    /// <returns>Index into the autotile set (0-15)</returns>
    public static int GetAutoTileIndex(bool north, bool south, bool east, bool west, int[] autoTileSet)
    {
        // Blob tiling pattern (16 tiles):
        // 0  = isolated
        // 1-4 = single edges
        // 5-12 = corners and combinations
        // 13-15 = multiple edges
        
        int index = 0;
        if (north) index |= 1;
        if (east) index |= 2;
        if (south) index |= 4;
        if (west) index |= 8;
        
        return autoTileSet[index];
    }
}
