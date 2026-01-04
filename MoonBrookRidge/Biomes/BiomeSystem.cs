using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Biomes;

/// <summary>
/// Biome system for managing different environmental zones
/// </summary>
public class BiomeSystem
{
    private Dictionary<BiomeType, BiomeDefinition> _biomes;
    private BiomeType _currentBiome;
    
    public BiomeType CurrentBiome => _currentBiome;
    
    public event Action<BiomeType, BiomeType> OnBiomeChanged;
    
    public BiomeSystem()
    {
        _biomes = new Dictionary<BiomeType, BiomeDefinition>();
        _currentBiome = BiomeType.Farm;
        
        InitializeBiomes();
    }
    
    private void InitializeBiomes()
    {
        // Farm biome (default/starting area)
        _biomes[BiomeType.Farm] = new BiomeDefinition(
            BiomeType.Farm,
            "Farmland",
            "Peaceful farmland perfect for growing crops",
            new Color(144, 238, 144), // Light green tint
            new[] { "Grass", "Dirt", "Water", "Crop" },
            new[] { "Chicken", "Cow", "Sheep" },
            1.0f // Normal speed
        );
        
        // Forest biome
        _biomes[BiomeType.Forest] = new BiomeDefinition(
            BiomeType.Forest,
            "Forest",
            "Dense forest filled with trees and wildlife",
            new Color(34, 139, 34), // Forest green
            new[] { "Oak Tree", "Pine Tree", "Berry Bush", "Mushroom" },
            new[] { "Deer", "Rabbit", "Wolf", "Bear" },
            0.9f // Slightly slower
        );
        
        // Haunted Forest biome
        _biomes[BiomeType.HauntedForest] = new BiomeDefinition(
            BiomeType.HauntedForest,
            "Haunted Forest",
            "A dark, spooky forest inhabited by undead creatures",
            new Color(64, 64, 96), // Dark purple tint
            new[] { "Dead Tree", "Gravestone", "Tombstone", "Ghost Mushroom" },
            new[] { "Ghost", "Skeleton", "Zombie", "Phantom" },
            0.8f // Slow movement
        );
        
        // Cave/Underground biome
        _biomes[BiomeType.Cave] = new BiomeDefinition(
            BiomeType.Cave,
            "Underground Cave",
            "Dark underground caves rich with minerals",
            new Color(96, 96, 96), // Dark gray
            new[] { "Stone", "Iron Ore", "Gold Ore", "Crystal", "Stalactite" },
            new[] { "Bat", "Spider", "Slime", "Rock Golem" },
            0.85f
        );
        
        // Deep Cave biome
        _biomes[BiomeType.DeepCave] = new BiomeDefinition(
            BiomeType.DeepCave,
            "Deep Caverns",
            "Dangerous depths with rare treasures",
            new Color(64, 64, 64), // Very dark gray
            new[] { "Dark Stone", "Diamond", "Mithril", "Glowing Crystal" },
            new[] { "Fire Elemental", "Demon", "Dragon", "Ancient Guardian" },
            0.75f
        );
        
        // Floating Islands biome
        _biomes[BiomeType.FloatingIslands] = new BiomeDefinition(
            BiomeType.FloatingIslands,
            "Sky Islands",
            "Magical floating islands high in the sky",
            new Color(173, 216, 230), // Light blue
            new[] { "Cloud Block", "Sky Crystal", "Celestial Flower", "Star Fragment" },
            new[] { "Sky Serpent", "Cloud Elemental", "Harpy", "Phoenix" },
            1.2f // Faster movement
        );
        
        // Underwater biome
        _biomes[BiomeType.Underwater] = new BiomeDefinition(
            BiomeType.Underwater,
            "Ocean Depths",
            "Mysterious underwater realm",
            new Color(0, 105, 148), // Deep blue
            new[] { "Coral", "Seaweed", "Pearl Oyster", "Sea Crystal" },
            new[] { "Shark", "Octopus", "Mermaid", "Sea Dragon" },
            0.6f // Very slow without water breathing
        );
        
        // Desert biome
        _biomes[BiomeType.Desert] = new BiomeDefinition(
            BiomeType.Desert,
            "Desert Wasteland",
            "Hot, arid desert with scorching sands",
            new Color(237, 201, 175), // Sandy color
            new[] { "Cactus", "Dead Bush", "Sand Dune", "Pyramid" },
            new[] { "Scorpion", "Snake", "Vulture", "Sand Elemental" },
            0.9f
        );
        
        // Snow/Tundra biome
        _biomes[BiomeType.Tundra] = new BiomeDefinition(
            BiomeType.Tundra,
            "Frozen Tundra",
            "Icy wasteland with freezing temperatures",
            new Color(240, 248, 255), // Alice blue (icy)
            new[] { "Ice Block", "Snow Pile", "Frozen Tree", "Ice Crystal" },
            new[] { "Ice Elemental", "Yeti", "Frost Wolf", "Penguin" },
            0.85f
        );
        
        // Volcanic biome
        _biomes[BiomeType.Volcanic] = new BiomeDefinition(
            BiomeType.Volcanic,
            "Volcanic Wasteland",
            "Dangerous volcanic region with rivers of lava",
            new Color(255, 69, 0), // Red-orange
            new[] { "Obsidian", "Lava Rock", "Sulfur", "Fire Crystal" },
            new[] { "Fire Elemental", "Lava Slime", "Salamander", "Magma Dragon" },
            0.9f
        );
        
        // Swamp biome
        _biomes[BiomeType.Swamp] = new BiomeDefinition(
            BiomeType.Swamp,
            "Murky Swamp",
            "Dank swampland with poisonous creatures",
            new Color(85, 107, 47), // Dark olive green
            new[] { "Swamp Tree", "Lily Pad", "Poison Mushroom", "Bog Iron" },
            new[] { "Swamp Zombie", "Giant Frog", "Alligator", "Poison Spider" },
            0.7f // Slow in mud
        );
        
        // Magical Meadow biome
        _biomes[BiomeType.MagicalMeadow] = new BiomeDefinition(
            BiomeType.MagicalMeadow,
            "Enchanted Meadow",
            "Beautiful meadow infused with magic",
            new Color(255, 182, 193), // Light pink
            new[] { "Magic Flower", "Fairy Ring", "Rainbow Crystal", "Enchanted Tree" },
            new[] { "Fairy", "Unicorn", "Sprite", "Pixie" },
            1.1f
        );
    }
    
    public BiomeDefinition GetBiome(BiomeType type)
    {
        return _biomes.ContainsKey(type) ? _biomes[type] : _biomes[BiomeType.Farm];
    }
    
    public BiomeDefinition GetCurrentBiomeDefinition()
    {
        return GetBiome(_currentBiome);
    }
    
    public void ChangeBiome(BiomeType newBiome)
    {
        var oldBiome = _currentBiome;
        _currentBiome = newBiome;
        OnBiomeChanged?.Invoke(oldBiome, newBiome);
    }
    
    public bool IsHostileBiome(BiomeType biome)
    {
        return biome switch
        {
            BiomeType.HauntedForest => true,
            BiomeType.Cave => true,
            BiomeType.DeepCave => true,
            BiomeType.Volcanic => true,
            BiomeType.Swamp => true,
            _ => false
        };
    }
    
    public List<BiomeType> GetAllBiomes()
    {
        return new List<BiomeType>(_biomes.Keys);
    }
    
    /// <summary>
    /// Detect biome at a given world position
    /// For now, returns the current biome. Can be enhanced with spatial biome detection.
    /// </summary>
    public BiomeType DetectBiomeAtPosition(Vector2 worldPosition)
    {
        // Simple implementation: return current biome
        // In the future, this could use world coordinates to determine biome
        // based on spatial zones, temperature, altitude, etc.
        return _currentBiome;
    }
    
    /// <summary>
    /// Get movement speed modifier for a specific biome
    /// </summary>
    public float GetMovementModifier(BiomeType biome)
    {
        if (_biomes.TryGetValue(biome, out BiomeDefinition definition))
        {
            return definition.MovementModifier;
        }
        return 1.0f; // Default no modifier
    }
}

/// <summary>
/// Biome definition with properties
/// </summary>
public class BiomeDefinition
{
    public BiomeType Type { get; }
    public string Name { get; }
    public string Description { get; }
    public Color TintColor { get; }
    public string[] Resources { get; }
    public string[] Creatures { get; }
    public float MovementModifier { get; } // Speed multiplier in this biome
    
    public BiomeDefinition(BiomeType type, string name, string description, 
        Color tintColor, string[] resources, string[] creatures, float movementModifier)
    {
        Type = type;
        Name = name;
        Description = description;
        TintColor = tintColor;
        Resources = resources;
        Creatures = creatures;
        MovementModifier = movementModifier;
    }
}

public enum BiomeType
{
    Farm,              // Starting area (safe)
    Forest,            // Regular forest (peaceful)
    HauntedForest,     // Spooky forest with undead
    Cave,              // Underground caves
    DeepCave,          // Deeper caves with rare resources
    FloatingIslands,   // Sky islands
    Underwater,        // Ocean depths
    Desert,            // Hot desert
    Tundra,            // Frozen wasteland
    Volcanic,          // Lava and fire
    Swamp,             // Murky swamp
    MagicalMeadow      // Enchanted area
}
