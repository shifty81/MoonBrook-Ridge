using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.World.Villages;

/// <summary>
/// Represents a village in the game world with its own culture, NPCs, and resources
/// </summary>
public class Village
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public BiomeType Biome { get; set; }
    public Vector2 CenterPosition { get; set; }
    public int Reputation { get; set; }
    public List<string> NPCIds { get; set; }
    public List<string> ShopItems { get; set; }
    public string Culture { get; set; }
    
    /// <summary>
    /// Fast travel cost to this village (in gold)
    /// </summary>
    public int TravelCost { get; set; }
    
    /// <summary>
    /// Whether player has discovered this village
    /// </summary>
    public bool IsDiscovered { get; set; }
    
    public Village(string id, string name, string description, BiomeType biome, Vector2 centerPosition, string culture)
    {
        Id = id;
        Name = name;
        Description = description;
        Biome = biome;
        CenterPosition = centerPosition;
        Culture = culture;
        Reputation = 0;
        NPCIds = new List<string>();
        ShopItems = new List<string>();
        TravelCost = 50; // Default travel cost
        IsDiscovered = false;
    }
}

/// <summary>
/// Biome types for villages
/// </summary>
public enum BiomeType
{
    Grassland,      // MoonBrook Valley - home farm area
    Forest,         // Pinewood Village - lumberjacks and hunters
    Mountain,       // Stonehelm Village - miners and blacksmiths
    Desert,         // Sandshore Village - traders and fire mages
    Frozen,         // Frostpeak Village - ice fishing and survival
    Swamp,          // Marshwood Village - alchemists and healers
    CrystalCave,    // Crystalgrove Village - magical academy
    Ruins           // Ruinwatch Village - archaeologists and lore
}
