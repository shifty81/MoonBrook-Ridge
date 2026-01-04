using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.World.Mining;

/// <summary>
/// Ore types following Core Keeper progression
/// </summary>
public enum OreType
{
    // Basic
    Stone,
    
    // Tier 1 - Copper Age
    Copper,
    
    // Tier 2 - Tin Age
    Tin,
    
    // Tier 3 - Iron Age
    Iron,
    Coal,
    
    // Tier 4 - Scarlet Era (unlocks automation)
    Scarlet,
    
    // Tier 5 - Mystical Age
    Octarine,
    Crystal,
    
    // Tier 6 - Galactic Age
    Galaxite,
    
    // Tier 7 - Solar Age
    Solarite,
    Pandorium
}

/// <summary>
/// Underground biome types (Core Keeper inspired)
/// </summary>
public enum UndergroundBiomeType
{
    DirtCavern,         // Starting area - Copper
    ClayCaves,          // Tin and clay
    ForgottenRuins,     // Iron and ancient artifacts
    ScarletWilderness,  // Scarlet ore, automation unlocked
    CrystalCaverns,     // Octarine and crystals
    SunkenSea,          // Water-filled caverns
    DesertCaves,        // Sandy underground, Galaxite
    VolcanicCore,       // Solarite and Pandorium
}

/// <summary>
/// Ore node in the underground
/// </summary>
public class OreNode
{
    public OreType Type { get; set; }
    public Vector2 Position { get; set; }
    public int RemainingHits { get; set; }
    public int MaxHits { get; set; }
    public int YieldAmount { get; set; } // How many ore items drop
    public UndergroundBiomeType Biome { get; set; }
    
    public OreNode(OreType type, Vector2 position, UndergroundBiomeType biome)
    {
        Type = type;
        Position = position;
        Biome = biome;
        
        // Set hits and yield based on ore type
        (MaxHits, YieldAmount) = GetOreStats(type);
        RemainingHits = MaxHits;
    }
    
    private (int hits, int yield) GetOreStats(OreType type)
    {
        return type switch
        {
            OreType.Stone => (2, 1),
            OreType.Copper => (3, 2),
            OreType.Tin => (4, 2),
            OreType.Iron => (5, 3),
            OreType.Coal => (3, 2),
            OreType.Scarlet => (6, 3),
            OreType.Octarine => (7, 4),
            OreType.Crystal => (5, 2),
            OreType.Galaxite => (8, 4),
            OreType.Solarite => (10, 5),
            OreType.Pandorium => (12, 3),
            _ => (3, 1)
        };
    }
}

/// <summary>
/// Expanded ore and underground biome system
/// Core Keeper-inspired progression
/// </summary>
public class ExpandedOreSystem
{
    private Dictionary<UndergroundBiomeType, List<OreType>> _biomeOres;
    private Dictionary<OreType, string> _oreDescriptions;
    private Dictionary<OreType, int> _tierLevels;
    
    public ExpandedOreSystem()
    {
        InitializeBiomeOres();
        InitializeOreDescriptions();
        InitializeTierLevels();
    }
    
    /// <summary>
    /// Map ores to underground biomes
    /// </summary>
    private void InitializeBiomeOres()
    {
        _biomeOres = new Dictionary<UndergroundBiomeType, List<OreType>>
        {
            [UndergroundBiomeType.DirtCavern] = new List<OreType>
            {
                OreType.Stone, OreType.Copper, OreType.Coal
            },
            
            [UndergroundBiomeType.ClayCaves] = new List<OreType>
            {
                OreType.Stone, OreType.Copper, OreType.Tin, OreType.Coal
            },
            
            [UndergroundBiomeType.ForgottenRuins] = new List<OreType>
            {
                OreType.Stone, OreType.Iron, OreType.Tin, OreType.Coal
            },
            
            [UndergroundBiomeType.ScarletWilderness] = new List<OreType>
            {
                OreType.Iron, OreType.Scarlet, OreType.Coal
            },
            
            [UndergroundBiomeType.CrystalCaverns] = new List<OreType>
            {
                OreType.Octarine, OreType.Crystal, OreType.Iron
            },
            
            [UndergroundBiomeType.SunkenSea] = new List<OreType>
            {
                OreType.Octarine, OreType.Crystal, OreType.Scarlet
            },
            
            [UndergroundBiomeType.DesertCaves] = new List<OreType>
            {
                OreType.Galaxite, OreType.Iron, OreType.Scarlet
            },
            
            [UndergroundBiomeType.VolcanicCore] = new List<OreType>
            {
                OreType.Solarite, OreType.Pandorium, OreType.Galaxite
            }
        };
    }
    
    /// <summary>
    /// Ore descriptions for UI
    /// </summary>
    private void InitializeOreDescriptions()
    {
        _oreDescriptions = new Dictionary<OreType, string>
        {
            [OreType.Stone] = "Basic stone for construction",
            [OreType.Copper] = "First metal ore, used for basic tools",
            [OreType.Tin] = "Stronger than copper, found in clay caves",
            [OreType.Iron] = "Strong metal for advanced equipment",
            [OreType.Coal] = "Fuel for furnaces and smelting",
            [OreType.Scarlet] = "Mystical ore that unlocks automation",
            [OreType.Octarine] = "Magical ore from crystal caverns",
            [OreType.Crystal] = "Pure crystal for enchantments",
            [OreType.Galaxite] = "Cosmic ore from deep deserts",
            [OreType.Solarite] = "Radiant ore from volcanic depths",
            [OreType.Pandorium] = "Legendary endgame material"
        };
    }
    
    /// <summary>
    /// Tier levels for progression
    /// </summary>
    private void InitializeTierLevels()
    {
        _tierLevels = new Dictionary<OreType, int>
        {
            [OreType.Stone] = 0,
            [OreType.Copper] = 1,
            [OreType.Tin] = 2,
            [OreType.Iron] = 3,
            [OreType.Coal] = 3,
            [OreType.Scarlet] = 4,
            [OreType.Octarine] = 5,
            [OreType.Crystal] = 5,
            [OreType.Galaxite] = 6,
            [OreType.Solarite] = 7,
            [OreType.Pandorium] = 7
        };
    }
    
    /// <summary>
    /// Get ores available in a biome
    /// </summary>
    public List<OreType> GetOresInBiome(UndergroundBiomeType biome)
    {
        return _biomeOres.TryGetValue(biome, out var ores) 
            ? new List<OreType>(ores) 
            : new List<OreType>();
    }
    
    /// <summary>
    /// Get random ore from biome
    /// </summary>
    public OreType GetRandomOreForBiome(UndergroundBiomeType biome, System.Random random)
    {
        var ores = GetOresInBiome(biome);
        if (ores.Count == 0) return OreType.Stone;
        
        return ores[random.Next(ores.Count)];
    }
    
    /// <summary>
    /// Get ore description
    /// </summary>
    public string GetOreDescription(OreType ore)
    {
        return _oreDescriptions.TryGetValue(ore, out var desc) ? desc : "Unknown ore";
    }
    
    /// <summary>
    /// Get ore tier level
    /// </summary>
    public int GetOreTier(OreType ore)
    {
        return _tierLevels.TryGetValue(ore, out var tier) ? tier : 0;
    }
    
    /// <summary>
    /// Check if ore requires specific pickaxe tier
    /// </summary>
    public bool CanMineOre(OreType ore, int pickaxeTier)
    {
        return pickaxeTier >= GetOreTier(ore);
    }
    
    /// <summary>
    /// Get biome description
    /// </summary>
    public string GetBiomeDescription(UndergroundBiomeType biome)
    {
        return biome switch
        {
            UndergroundBiomeType.DirtCavern => "Starting underground area with copper deposits",
            UndergroundBiomeType.ClayCaves => "Clay-rich caves with tin veins",
            UndergroundBiomeType.ForgottenRuins => "Ancient ruins containing iron and artifacts",
            UndergroundBiomeType.ScarletWilderness => "Mystical wilderness where automation begins",
            UndergroundBiomeType.CrystalCaverns => "Glowing caverns filled with magical crystals",
            UndergroundBiomeType.SunkenSea => "Flooded underground sea with rare resources",
            UndergroundBiomeType.DesertCaves => "Sandy underground tunnels with cosmic ores",
            UndergroundBiomeType.VolcanicCore => "Volcanic depths with legendary materials",
            _ => "Unknown biome"
        };
    }
    
    /// <summary>
    /// Get recommended player level for biome
    /// </summary>
    public int GetBiomeRecommendedLevel(UndergroundBiomeType biome)
    {
        return biome switch
        {
            UndergroundBiomeType.DirtCavern => 1,
            UndergroundBiomeType.ClayCaves => 5,
            UndergroundBiomeType.ForgottenRuins => 10,
            UndergroundBiomeType.ScarletWilderness => 15,
            UndergroundBiomeType.CrystalCaverns => 20,
            UndergroundBiomeType.SunkenSea => 25,
            UndergroundBiomeType.DesertCaves => 30,
            UndergroundBiomeType.VolcanicCore => 40,
            _ => 1
        };
    }
    
    /// <summary>
    /// Get ore spawn weight for biome (higher = more common)
    /// </summary>
    public Dictionary<OreType, int> GetOreSpawnWeights(UndergroundBiomeType biome)
    {
        var weights = new Dictionary<OreType, int>();
        
        switch (biome)
        {
            case UndergroundBiomeType.DirtCavern:
                weights[OreType.Stone] = 50;
                weights[OreType.Copper] = 30;
                weights[OreType.Coal] = 20;
                break;
                
            case UndergroundBiomeType.ClayCaves:
                weights[OreType.Stone] = 40;
                weights[OreType.Tin] = 30;
                weights[OreType.Copper] = 20;
                weights[OreType.Coal] = 10;
                break;
                
            case UndergroundBiomeType.ForgottenRuins:
                weights[OreType.Stone] = 30;
                weights[OreType.Iron] = 40;
                weights[OreType.Tin] = 20;
                weights[OreType.Coal] = 10;
                break;
                
            case UndergroundBiomeType.ScarletWilderness:
                weights[OreType.Scarlet] = 40;
                weights[OreType.Iron] = 30;
                weights[OreType.Coal] = 30;
                break;
                
            case UndergroundBiomeType.CrystalCaverns:
                weights[OreType.Octarine] = 40;
                weights[OreType.Crystal] = 35;
                weights[OreType.Iron] = 25;
                break;
                
            case UndergroundBiomeType.SunkenSea:
                weights[OreType.Octarine] = 45;
                weights[OreType.Crystal] = 30;
                weights[OreType.Scarlet] = 25;
                break;
                
            case UndergroundBiomeType.DesertCaves:
                weights[OreType.Galaxite] = 50;
                weights[OreType.Scarlet] = 30;
                weights[OreType.Iron] = 20;
                break;
                
            case UndergroundBiomeType.VolcanicCore:
                weights[OreType.Solarite] = 45;
                weights[OreType.Pandorium] = 15;
                weights[OreType.Galaxite] = 40;
                break;
        }
        
        return weights;
    }
}
