using System.Collections.Generic;

namespace MoonBrookRidge.World.Tiles;

/// <summary>
/// Mapping configuration for Slates tileset tiles
/// Defines which tile IDs to use for different terrain types
/// Based on Slates v.2 32x32px tileset by Ivan Voirol (CC-BY 4.0)
/// </summary>
public static class SlatesTileMapping
{
    // Slates tileset is 56 columns × 23 rows (1,288 tiles total)
    // Tile ID = (row * 56) + column

    /// <summary>
    /// Grass terrain tiles (various shades and styles)
    /// </summary>
    public static class Grass
    {
        public static readonly int[] Basic = { 0, 1, 2, 3, 4, 5 };
        public static readonly int[] Light = { 56, 57, 58, 59, 60, 61 };
        public static readonly int[] Medium = { 112, 113, 114, 115, 116, 117 };
        public static readonly int[] Dark = { 168, 169, 170, 171, 172, 173 };
        public static readonly int[] WithFlowers = { 6, 7, 8, 9, 62, 63 };
    }

    /// <summary>
    /// Dirt and path tiles
    /// </summary>
    public static class Dirt
    {
        public static readonly int[] Basic = { 224, 225, 226, 227, 228, 229 };
        public static readonly int[] Path = { 280, 281, 282, 283, 284, 285 };
        public static readonly int[] Tilled = { 336, 337, 338, 339, 340, 341 };
    }

    /// <summary>
    /// Stone and rock tiles
    /// </summary>
    public static class Stone
    {
        public static readonly int[] Floor = { 392, 393, 394, 395, 396, 397 };
        public static readonly int[] Wall = { 448, 449, 450, 451, 452, 453 };
        public static readonly int[] Cobblestone = { 504, 505, 506, 507, 508, 509 };
        public static readonly int[] Brick = { 560, 561, 562, 563, 564, 565 };
    }

    /// <summary>
    /// Water tiles (rivers, ponds, ocean)
    /// </summary>
    public static class Water
    {
        public static readonly int[] Still = { 616, 617, 618, 619, 620, 621 };
        public static readonly int[] Animated = { 672, 673, 674, 675, 676, 677 };
        public static readonly int[] Deep = { 728, 729, 730, 731, 732, 733 };
        public static readonly int[] Shallow = { 784, 785, 786, 787, 788, 789 };
    }

    /// <summary>
    /// Sand and beach tiles
    /// </summary>
    public static class Sand
    {
        public static readonly int[] Basic = { 840, 841, 842, 843, 844, 845 };
        public static readonly int[] Light = { 896, 897, 898, 899, 900, 901 };
        public static readonly int[] WithStones = { 952, 953, 954, 955, 956, 957 };
    }

    /// <summary>
    /// Interior floor tiles
    /// </summary>
    public static class Indoor
    {
        public static readonly int[] WoodFloor = { 1008, 1009, 1010, 1011, 1012, 1013 };
        public static readonly int[] StoneFloor = { 1064, 1065, 1066, 1067, 1068, 1069 };
        public static readonly int[] TileFloor = { 1120, 1121, 1122, 1123, 1124, 1125 };
    }

    /// <summary>
    /// Special terrain tiles
    /// </summary>
    public static class Special
    {
        public static readonly int[] Snow = { 1176, 1177, 1178, 1179, 1180, 1181 };
        public static readonly int[] Ice = { 1232, 1233, 1234, 1235, 1236, 1237 };
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

    /// <summary>
    /// Get all available grass tile IDs
    /// </summary>
    public static IEnumerable<int> GetAllGrassTiles()
    {
        foreach (int id in Grass.Basic) yield return id;
        foreach (int id in Grass.Light) yield return id;
        foreach (int id in Grass.Medium) yield return id;
        foreach (int id in Grass.Dark) yield return id;
        foreach (int id in Grass.WithFlowers) yield return id;
    }

    /// <summary>
    /// Get all available dirt tile IDs
    /// </summary>
    public static IEnumerable<int> GetAllDirtTiles()
    {
        foreach (int id in Dirt.Basic) yield return id;
        foreach (int id in Dirt.Path) yield return id;
    }

    /// <summary>
    /// Get all available stone tile IDs
    /// </summary>
    public static IEnumerable<int> GetAllStoneTiles()
    {
        foreach (int id in Stone.Floor) yield return id;
        foreach (int id in Stone.Cobblestone) yield return id;
    }

    /// <summary>
    /// Get all available water tile IDs
    /// </summary>
    public static IEnumerable<int> GetAllWaterTiles()
    {
        foreach (int id in Water.Still) yield return id;
        foreach (int id in Water.Shallow) yield return id;
    }

    /// <summary>
    /// Get all available sand tile IDs
    /// </summary>
    public static IEnumerable<int> GetAllSandTiles()
    {
        foreach (int id in Sand.Basic) yield return id;
        foreach (int id in Sand.Light) yield return id;
    }
}
