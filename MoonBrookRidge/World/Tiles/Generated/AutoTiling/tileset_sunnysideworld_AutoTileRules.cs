using System.Collections.Generic;

namespace MoonBrookRidge.World.Tiles.AutoTiling;

/// <summary>
/// Auto-generated autotiling rules from GameMaker tileset: tileset_sunnysideworld
/// This file is auto-generated. Do not edit manually.
/// </summary>
public static class SunnysideworldAutoTileRules
{
    /// <summary>
    /// Tile dimensions
    /// </summary>
    public const int TileWidth = 16;
    public const int TileHeight = 16;
    public const int TotalTiles = 4096;
    
    /// <summary>
    /// Autotile set definitions
    /// Each set contains 16 tiles arranged for blob/wang tiling:
    /// Tiles are indexed for corners and edges to create seamless terrain
    /// </summary>
    public static class AutoTileSets
    {
        /// <summary>
        /// Land autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] Land = new int[]
        {
            193, 194, 195, 196, 197, 198, 199, 200,
            257, 258, 259, 260, 261, 262, 263, 264
        };

        /// <summary>
        /// Building 01 autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] Building01 = new int[]
        {
            577, 578, 579, 580, 581, 582, 583, 584,
            641, 642, 643, 644, 645, 646, 647, 648
        };

        /// <summary>
        /// Building 02 autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] Building02 = new int[]
        {
            961, 962, 963, 964, 965, 966, 967, 968,
            1025, 1026, 1027, 1028, 1029, 1030, 1031, 0
        };

        /// <summary>
        /// Inner Walls autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] InnerWalls = new int[]
        {
            769, 770, 771, 772, 773, 774, 775, 776,
            833, 834, 835, 836, 837, 838, 839, 840
        };

        /// <summary>
        /// Path 01 autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] Path01 = new int[]
        {
            449, 450, 451, 452, 453, 454, 455, 456,
            513, 514, 515, 516, 517, 518, 519, 0
        };

        /// <summary>
        /// Path 02 autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] Path02 = new int[]
        {
            460, 461, 462, 463, 464, 465, 466, 467,
            524, 525, 526, 527, 528, 529, 530, 0
        };

        /// <summary>
        /// Path 03 autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] Path03 = new int[]
        {
            482, 483, 484, 485, 486, 487, 488, 489,
            546, 547, 548, 549, 550, 551, 552, 0
        };

        /// <summary>
        /// River autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] River = new int[]
        {
            470, 471, 472, 473, 474, 475, 476, 477,
            534, 535, 536, 537, 538, 539, 540, 0
        };

        /// <summary>
        /// Clouds 01 autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] Clouds01 = new int[]
        {
            1153, 1154, 1155, 1156, 1157, 1158, 1159, 1160,
            1217, 1218, 1219, 1220, 1221, 1222, 1223, 0
        };

        /// <summary>
        /// Clouds 02 autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] Clouds02 = new int[]
        {
            1345, 1346, 1347, 1348, 1349, 1350, 1351, 1352,
            1409, 1410, 1411, 1412, 1413, 1414, 1415, 0
        };

        /// <summary>
        /// Cloud Shadow autotile set
        /// Closed edge: False
        /// </summary>
        public static readonly int[] CloudShadow = new int[]
        {
            1537, 1538, 1539, 1540, 1541, 1542, 1543, 1544,
            1601, 1602, 1603, 1604, 1605, 1606, 1607, 0
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
