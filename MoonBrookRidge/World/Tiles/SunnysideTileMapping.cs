using System;
using System.Collections.Generic;

namespace MoonBrookRidge.World.Tiles;

/// <summary>
/// Mapping configuration for Sunnyside World tileset tiles
/// Defines which tile IDs to use for different terrain types
/// Based on Sunnyside World 16x16px tileset (1024x1024, 64x64 tiles)
/// </summary>
public static class SunnysideTileMapping
{
    // Sunnyside tileset is 64 columns × 64 rows (4,096 tiles total)
    // Tile ID = (row * 64) + column
    
    // Note: These are approximate mappings. The actual tile positions
    // in the Sunnyside World tileset may vary. These should be adjusted
    // based on visual inspection of the tileset.

    /// <summary>
    /// Grass terrain tiles
    /// </summary>
    public static class Grass
    {
        // Using first rows for grass (common in tilesets)
        public static readonly int[] Basic = { 0, 1, 2, 3, 4, 5, 64, 65, 66, 67 };
        public static readonly int[] Light = { 6, 7, 8, 9, 68, 69, 70, 71 };
        public static readonly int[] Medium = { 10, 11, 12, 13, 72, 73, 74, 75 };
        public static readonly int[] Dark = { 14, 15, 16, 17, 76, 77, 78, 79 };
    }

    /// <summary>
    /// Dirt and path tiles
    /// </summary>
    public static class Dirt
    {
        // Typically placed after grass in tilesets
        public static readonly int[] Basic = { 128, 129, 130, 131, 132, 133 };
        public static readonly int[] Path = { 192, 193, 194, 195, 196, 197 };
        public static readonly int[] Tilled = { 256, 257, 258, 259, 260, 261 };
    }

    /// <summary>
    /// Stone and rock tiles
    /// </summary>
    public static class Stone
    {
        public static readonly int[] Floor = { 320, 321, 322, 323, 324, 325 };
        public static readonly int[] Wall = { 384, 385, 386, 387, 388, 389 };
    }

    /// <summary>
    /// Water tiles
    /// </summary>
    public static class Water
    {
        public static readonly int[] Basic = { 448, 449, 450, 451, 452, 453 };
        public static readonly int[] Shallow = { 454, 455, 456, 457, 458, 459 };
    }

    /// <summary>
    /// Sand and beach tiles
    /// </summary>
    public static class Sand
    {
        public static readonly int[] Basic = { 512, 513, 514, 515, 516, 517 };
        public static readonly int[] Light = { 518, 519, 520, 521, 522, 523 };
    }

    /// <summary>
    /// Get a random tile ID from a given array
    /// </summary>
    public static int GetRandomTile(int[] tileIds, System.Random random)
    {
        if (tileIds == null || tileIds.Length == 0)
            return 0;
        
        return tileIds[random.Next(tileIds.Length)];
    }
}
