using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.World;

namespace MoonBrookRidge.Biomes;

/// <summary>
/// Manages spawning of biome-specific resources (trees, rocks, plants)
/// </summary>
public class BiomeResourceSpawner
{
    private Random _random;
    private Dictionary<BiomeType, BiomeResourceConfig> _biomeConfigs;
    
    public BiomeResourceSpawner()
    {
        _random = new Random();
        _biomeConfigs = new Dictionary<BiomeType, BiomeResourceConfig>();
        InitializeBiomeConfigs();
    }
    
    private void InitializeBiomeConfigs()
    {
        // Farm biome - basic resources
        _biomeConfigs[BiomeType.Farm] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "oak", "apple", "birch" },
            TreeSpawnChance = 0.05f,
            RockTypes = new[] { "stone", "limestone" },
            RockSpawnChance = 0.03f,
            MinResourcesPerChunk = 3,
            MaxResourcesPerChunk = 8
        };
        
        // Forest - many trees, few rocks
        _biomeConfigs[BiomeType.Forest] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "oak", "pine", "birch", "maple", "cedar" },
            TreeSpawnChance = 0.15f,
            RockTypes = new[] { "stone", "mossy_stone" },
            RockSpawnChance = 0.02f,
            MinResourcesPerChunk = 8,
            MaxResourcesPerChunk = 15
        };
        
        // Haunted Forest - dead trees, creepy resources
        _biomeConfigs[BiomeType.HauntedForest] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "dead_tree", "withered_oak", "cursed_pine" },
            TreeSpawnChance = 0.10f,
            RockTypes = new[] { "gravestone", "dark_stone" },
            RockSpawnChance = 0.05f,
            MinResourcesPerChunk = 5,
            MaxResourcesPerChunk = 12
        };
        
        // Cave - mostly rocks, rare mushroom trees
        _biomeConfigs[BiomeType.Cave] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "mushroom_tree" },
            TreeSpawnChance = 0.02f,
            RockTypes = new[] { "stone", "iron_ore", "coal_ore" },
            RockSpawnChance = 0.20f,
            MinResourcesPerChunk = 10,
            MaxResourcesPerChunk = 20
        };
        
        // Deep Cave - rare ores
        _biomeConfigs[BiomeType.DeepCave] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "crystal_formation" },
            TreeSpawnChance = 0.01f,
            RockTypes = new[] { "dark_stone", "gold_ore", "diamond_ore", "mithril_ore" },
            RockSpawnChance = 0.25f,
            MinResourcesPerChunk = 12,
            MaxResourcesPerChunk = 25
        };
        
        // Floating Islands - sky crystals, light trees
        _biomeConfigs[BiomeType.FloatingIslands] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "cloud_tree", "sky_pine", "celestial_oak" },
            TreeSpawnChance = 0.08f,
            RockTypes = new[] { "sky_crystal", "cloud_stone" },
            RockSpawnChance = 0.10f,
            MinResourcesPerChunk = 6,
            MaxResourcesPerChunk = 12
        };
        
        // Underwater - coral and sea rocks
        _biomeConfigs[BiomeType.Underwater] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "kelp", "coral_tree", "sea_plant" },
            TreeSpawnChance = 0.12f,
            RockTypes = new[] { "coral_rock", "pearl_oyster", "sea_stone" },
            RockSpawnChance = 0.15f,
            MinResourcesPerChunk = 8,
            MaxResourcesPerChunk = 16
        };
        
        // Desert - cacti and sandstone
        _biomeConfigs[BiomeType.Desert] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "cactus", "palm_tree", "dead_bush" },
            TreeSpawnChance = 0.06f,
            RockTypes = new[] { "sandstone", "desert_rock" },
            RockSpawnChance = 0.08f,
            MinResourcesPerChunk = 4,
            MaxResourcesPerChunk = 10
        };
        
        // Tundra - frozen resources
        _biomeConfigs[BiomeType.Tundra] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "frozen_pine", "ice_tree", "snow_covered_oak" },
            TreeSpawnChance = 0.05f,
            RockTypes = new[] { "ice_block", "frozen_stone", "snow_pile" },
            RockSpawnChance = 0.12f,
            MinResourcesPerChunk = 5,
            MaxResourcesPerChunk = 12
        };
        
        // Volcanic - obsidian and lava rocks
        _biomeConfigs[BiomeType.Volcanic] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "charred_tree", "lava_mushroom" },
            TreeSpawnChance = 0.02f,
            RockTypes = new[] { "obsidian", "lava_rock", "sulfur_deposit", "basalt" },
            RockSpawnChance = 0.18f,
            MinResourcesPerChunk = 10,
            MaxResourcesPerChunk = 18
        };
        
        // Swamp - murky resources
        _biomeConfigs[BiomeType.Swamp] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "swamp_tree", "willow", "mangrove" },
            TreeSpawnChance = 0.10f,
            RockTypes = new[] { "bog_iron", "muddy_stone" },
            RockSpawnChance = 0.08f,
            MinResourcesPerChunk = 7,
            MaxResourcesPerChunk = 14
        };
        
        // Magical Meadow - enchanted resources
        _biomeConfigs[BiomeType.MagicalMeadow] = new BiomeResourceConfig
        {
            TreeTypes = new[] { "enchanted_tree", "rainbow_tree", "fairy_oak" },
            TreeSpawnChance = 0.09f,
            RockTypes = new[] { "magic_crystal", "rainbow_stone", "fairy_rock" },
            RockSpawnChance = 0.10f,
            MinResourcesPerChunk = 8,
            MaxResourcesPerChunk = 15
        };
    }
    
    /// <summary>
    /// Spawns biome-appropriate resources in a given area
    /// </summary>
    /// <param name="biome">The biome type</param>
    /// <param name="chunkPosition">Top-left position of the spawn chunk</param>
    /// <param name="chunkSize">Size of the chunk (in tiles)</param>
    /// <param name="existingObjects">Existing objects to avoid overlap</param>
    /// <returns>List of spawned world objects</returns>
    public List<WorldObject> SpawnResourcesInChunk(BiomeType biome, Vector2 chunkPosition, 
        int chunkSize, List<WorldObject> existingObjects)
    {
        List<WorldObject> spawnedObjects = new List<WorldObject>();
        
        if (!_biomeConfigs.TryGetValue(biome, out BiomeResourceConfig config))
        {
            return spawnedObjects; // No config for this biome
        }
        
        int resourceCount = _random.Next(config.MinResourcesPerChunk, config.MaxResourcesPerChunk + 1);
        int attempts = 0;
        int maxAttempts = resourceCount * 3; // Try 3 times per resource
        
        while (spawnedObjects.Count < resourceCount && attempts < maxAttempts)
        {
            attempts++;
            
            // Random position within chunk
            Vector2 position = new Vector2(
                chunkPosition.X + _random.Next(0, chunkSize) * 16, // 16 = tile size
                chunkPosition.Y + _random.Next(0, chunkSize) * 16
            );
            
            // Check if position overlaps with existing objects
            if (IsPositionOccupied(position, existingObjects) || 
                IsPositionOccupied(position, spawnedObjects))
            {
                continue; // Try another position
            }
            
            // Decide whether to spawn tree or rock
            bool spawnTree = _random.NextDouble() < (config.TreeSpawnChance / (config.TreeSpawnChance + config.RockSpawnChance));
            
            WorldObject newObject;
            if (spawnTree && config.TreeTypes.Length > 0)
            {
                string treeType = config.TreeTypes[_random.Next(config.TreeTypes.Length)];
                newObject = CreateTree(position, treeType);
            }
            else if (config.RockTypes.Length > 0)
            {
                string rockType = config.RockTypes[_random.Next(config.RockTypes.Length)];
                newObject = CreateRock(position, rockType);
            }
            else
            {
                continue;
            }
            
            spawnedObjects.Add(newObject);
        }
        
        return spawnedObjects;
    }
    
    private bool IsPositionOccupied(Vector2 position, List<WorldObject> objects)
    {
        const float MIN_DISTANCE = 32f; // Minimum distance between resources
        
        foreach (var obj in objects)
        {
            float distance = Vector2.Distance(position, obj.Position);
            if (distance < MIN_DISTANCE)
            {
                return true;
            }
        }
        
        return false;
    }
    
    private WorldObject CreateTree(Vector2 position, string treeType)
    {
        // Create a ChoppableTree with biome-specific properties
        // Note: Texture will need to be set separately when integrating with the game
        // The null texture is intentional - requires game integration to set actual textures
        var tree = new ChoppableTree(treeType, position, (Texture2D)null, treeType, _random);
        return tree;
    }
    
    private WorldObject CreateRock(Vector2 position, string rockType)
    {
        // Create a BreakableRock with biome-specific properties
        // Note: Texture will need to be set separately when integrating with the game
        // The null texture is intentional - requires game integration to set actual textures
        var rock = new BreakableRock(rockType, position, (Texture2D)null, _random);
        return rock;
    }
}

/// <summary>
/// Configuration for biome-specific resource spawning
/// </summary>
public class BiomeResourceConfig
{
    public string[] TreeTypes { get; set; } = Array.Empty<string>();
    public float TreeSpawnChance { get; set; }
    public string[] RockTypes { get; set; } = Array.Empty<string>();
    public float RockSpawnChance { get; set; }
    public int MinResourcesPerChunk { get; set; }
    public int MaxResourcesPerChunk { get; set; }
}
