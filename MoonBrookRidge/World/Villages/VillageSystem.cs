using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MoonBrookRidge.World.Villages;

/// <summary>
/// Manages all villages in the game world
/// Handles village discovery, reputation, and relationships
/// </summary>
public class VillageSystem
{
    private Dictionary<string, Village> _villages;
    private const float DISCOVERY_RANGE = 64f; // ~4 tiles
    
    public event Action<Village>? OnVillageDiscovered;
    public event Action<Village, int>? OnReputationChanged; // Village, new reputation
    
    public VillageSystem()
    {
        _villages = new Dictionary<string, Village>();
        InitializeVillages();
    }
    
    /// <summary>
    /// Initialize all 8 villages in the game world
    /// Positioned across the 250x250 tile world
    /// </summary>
    private void InitializeVillages()
    {
        const float TILE_SIZE = 16f;
        
        // 1. MoonBrook Valley (Grassland) - Center/Home area - Always discovered
        var moonbrook = new Village(
            "moonbrook_valley",
            "MoonBrook Valley",
            "Your home village, a peaceful farming community surrounded by rolling grasslands.",
            BiomeType.Grassland,
            new Vector2(75f * TILE_SIZE, 75f * TILE_SIZE),
            "Farming community with strong ties to the land. Known for harvest festivals."
        );
        moonbrook.IsDiscovered = true;
        moonbrook.TravelCost = 25;
        AddVillage(moonbrook);
        
        // 2. Pinewood Village (Forest) - North
        AddVillage(new Village(
            "pinewood_village",
            "Pinewood Village",
            "A forest settlement of lumberjacks, hunters, and nature mages. Surrounded by towering pine trees.",
            BiomeType.Forest,
            new Vector2(125f * TILE_SIZE, 30f * TILE_SIZE),
            "Forest dwellers who respect nature. Expert woodworkers and rangers."
        )
        {
            TravelCost = 40
        });
        
        // 3. Stonehelm Village (Mountain) - Northeast
        AddVillage(new Village(
            "stonehelm_village",
            "Stonehelm Village",
            "A dwarven mining town carved into the mountainside. Master blacksmiths and miners.",
            BiomeType.Mountain,
            new Vector2(200f * TILE_SIZE, 50f * TILE_SIZE),
            "Dwarven stronghold known for metalwork and mining expertise."
        )
        {
            TravelCost = 60
        });
        
        // 4. Sandshore Village (Desert) - East
        AddVillage(new Village(
            "sandshore_village",
            "Sandshore Village",
            "A desert trading outpost with fire mages and treasure hunters. Gateway to ancient ruins.",
            BiomeType.Desert,
            new Vector2(220f * TILE_SIZE, 125f * TILE_SIZE),
            "Trading hub with exotic goods. Fire magic practitioners and adventurers."
        )
        {
            TravelCost = 55
        });
        
        // 5. Frostpeak Village (Frozen) - Northwest
        AddVillage(new Village(
            "frostpeak_village",
            "Frostpeak Village",
            "An icy settlement specializing in ice fishing and cold-weather survival. Hardy folk.",
            BiomeType.Frozen,
            new Vector2(30f * TILE_SIZE, 30f * TILE_SIZE),
            "Resilient ice dwellers. Expert fishermen and cold magic users."
        )
        {
            TravelCost = 70
        });
        
        // 6. Marshwood Village (Swamp) - Southwest
        AddVillage(new Village(
            "marshwood_village",
            "Marshwood Village",
            "A swamp settlement of alchemists, poison experts, and healers. Mysterious and humid.",
            BiomeType.Swamp,
            new Vector2(50f * TILE_SIZE, 200f * TILE_SIZE),
            "Alchemical center with rare herbs. Masters of potions and antidotes."
        )
        {
            TravelCost = 45
        });
        
        // 7. Crystalgrove Village (Crystal Cave) - Southeast
        AddVillage(new Village(
            "crystalgrove_village",
            "Crystalgrove Village",
            "A magical academy built within glowing crystal caverns. Enchanters and scholars.",
            BiomeType.CrystalCave,
            new Vector2(200f * TILE_SIZE, 200f * TILE_SIZE),
            "Magical academy where enchanters study arcane arts among crystals."
        )
        {
            TravelCost = 80
        });
        
        // 8. Ruinwatch Village (Ruins) - South
        AddVillage(new Village(
            "ruinwatch_village",
            "Ruinwatch Village",
            "An archaeological expedition camp studying ancient ruins. Lore keepers and historians.",
            BiomeType.Ruins,
            new Vector2(125f * TILE_SIZE, 220f * TILE_SIZE),
            "Archaeological outpost preserving ancient knowledge and artifacts."
        )
        {
            TravelCost = 50
        });
    }
    
    private void AddVillage(Village village)
    {
        _villages[village.Id] = village;
    }
    
    /// <summary>
    /// Get a village by its ID
    /// </summary>
    public Village? GetVillage(string villageId)
    {
        return _villages.TryGetValue(villageId, out var village) ? village : null;
    }
    
    /// <summary>
    /// Get all villages in the game
    /// </summary>
    public List<Village> GetAllVillages()
    {
        return _villages.Values.ToList();
    }
    
    /// <summary>
    /// Get all discovered villages
    /// </summary>
    public List<Village> GetDiscoveredVillages()
    {
        return _villages.Values.Where(v => v.IsDiscovered).ToList();
    }
    
    /// <summary>
    /// Check if player is near any undiscovered villages and discover them
    /// Call this in Update() with player position
    /// </summary>
    public void CheckForDiscovery(Vector2 playerPosition)
    {
        foreach (var village in _villages.Values)
        {
            if (!village.IsDiscovered)
            {
                float distance = Vector2.Distance(playerPosition, village.CenterPosition);
                if (distance <= DISCOVERY_RANGE)
                {
                    DiscoverVillage(village);
                }
            }
        }
    }
    
    private void DiscoverVillage(Village village)
    {
        village.IsDiscovered = true;
        OnVillageDiscovered?.Invoke(village);
    }
    
    /// <summary>
    /// Add reputation to a village
    /// </summary>
    public void AddReputation(string villageId, int amount)
    {
        if (_villages.TryGetValue(villageId, out var village))
        {
            village.Reputation += amount;
            OnReputationChanged?.Invoke(village, village.Reputation);
        }
    }
    
    /// <summary>
    /// Get reputation level name
    /// </summary>
    public string GetReputationLevel(int reputation)
    {
        return reputation switch
        {
            < -1000 => "Hated",
            < -500 => "Hostile",
            < -100 => "Unfriendly",
            < 100 => "Neutral",
            < 500 => "Friendly",
            < 1000 => "Honored",
            _ => "Exalted"
        };
    }
    
    /// <summary>
    /// Get village discovery statistics
    /// </summary>
    public (int discovered, int total) GetDiscoveryStats()
    {
        int discovered = _villages.Values.Count(v => v.IsDiscovered);
        int total = _villages.Count;
        return (discovered, total);
    }
}
