using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Represents a waypoint location that can be fast-traveled to
/// </summary>
public class Waypoint
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Vector2 Position { get; set; }
    public bool IsUnlocked { get; set; }
    public WaypointType Type { get; set; }
    
    public Waypoint(string id, string name, string description, Vector2 position, WaypointType type)
    {
        Id = id;
        Name = name;
        Description = description;
        Position = position;
        Type = type;
        IsUnlocked = false;
    }
}

/// <summary>
/// Types of waypoints
/// </summary>
public enum WaypointType
{
    Farm,           // Player's home farm
    Village,        // Town or village
    DungeonEntrance,// Dungeon entrance
    MineshaftEntrance, // Mine entrance
    Landmark,       // Notable location
    ShopDistrict,   // Shopping area
    Custom          // Player-placed waypoint
}

/// <summary>
/// Fast travel and waypoint management system
/// Quality of life feature for navigating large worlds
/// </summary>
public class WaypointSystem
{
    private Dictionary<string, Waypoint> _waypoints;
    private List<Waypoint> _unlockedWaypoints;
    
    // Fast travel costs
    private const int BASE_TRAVEL_COST = 50; // Gold cost
    private const float TIME_COST_HOURS = 1f; // Game time hours
    
    public event Action<Waypoint> OnWaypointUnlocked;
    public event Action<Waypoint, int> OnFastTravel; // Waypoint, cost
    
    public WaypointSystem()
    {
        _waypoints = new Dictionary<string, Waypoint>();
        _unlockedWaypoints = new List<Waypoint>();
        
        InitializeDefaultWaypoints();
    }
    
    private void InitializeDefaultWaypoints()
    {
        // Updated for 250x250 world (5X larger)
        // Center of world is at (125, 125)
        const float TILE_SIZE = 16f;
        
        // Farm waypoint (always unlocked) - center of 250x250 world
        var farmWaypoint = new Waypoint(
            "farm_home",
            "Home Farm",
            "Your cozy farm in MoonBrook Ridge",
            new Vector2(125f * TILE_SIZE, 125f * TILE_SIZE), // Center of farm area
            WaypointType.Farm
        );
        farmWaypoint.IsUnlocked = true;
        AddWaypoint(farmWaypoint);
        
        // ===== 8 Village Waypoints (Phase 10) =====
        
        // 1. MoonBrook Valley (Grassland) - Main village
        AddWaypoint(new Waypoint(
            "moonbrook_village",
            "MoonBrook Valley",
            "Your home village, a peaceful farming community",
            new Vector2(75f * TILE_SIZE, 75f * TILE_SIZE),
            WaypointType.Village
        ));
        
        // 2. Pinewood Village (Forest) - North
        AddWaypoint(new Waypoint(
            "pinewood_village",
            "Pinewood Village",
            "Forest settlement of lumberjacks and hunters",
            new Vector2(125f * TILE_SIZE, 30f * TILE_SIZE),
            WaypointType.Village
        ));
        
        // 3. Stonehelm Village (Mountain) - Northeast
        AddWaypoint(new Waypoint(
            "stonehelm_village",
            "Stonehelm Village",
            "Dwarven mining town with master blacksmiths",
            new Vector2(200f * TILE_SIZE, 50f * TILE_SIZE),
            WaypointType.Village
        ));
        
        // 4. Sandshore Village (Desert) - East
        AddWaypoint(new Waypoint(
            "sandshore_village",
            "Sandshore Village",
            "Desert trading outpost with fire mages",
            new Vector2(220f * TILE_SIZE, 125f * TILE_SIZE),
            WaypointType.Village
        ));
        
        // 5. Frostpeak Village (Frozen) - Northwest
        AddWaypoint(new Waypoint(
            "frostpeak_village",
            "Frostpeak Village",
            "Icy settlement specializing in ice fishing",
            new Vector2(30f * TILE_SIZE, 30f * TILE_SIZE),
            WaypointType.Village
        ));
        
        // 6. Marshwood Village (Swamp) - Southwest
        AddWaypoint(new Waypoint(
            "marshwood_village",
            "Marshwood Village",
            "Swamp settlement of alchemists and healers",
            new Vector2(50f * TILE_SIZE, 200f * TILE_SIZE),
            WaypointType.Village
        ));
        
        // 7. Crystalgrove Village (Crystal Cave) - Southeast
        AddWaypoint(new Waypoint(
            "crystalgrove_village",
            "Crystalgrove Village",
            "Magical academy within glowing crystal caverns",
            new Vector2(200f * TILE_SIZE, 200f * TILE_SIZE),
            WaypointType.Village
        ));
        
        // 8. Ruinwatch Village (Ruins) - South
        AddWaypoint(new Waypoint(
            "ruinwatch_village",
            "Ruinwatch Village",
            "Archaeological expedition studying ancient ruins",
            new Vector2(125f * TILE_SIZE, 220f * TILE_SIZE),
            WaypointType.Village
        ));
        
        // Mine entrance - southwest (kept for backwards compatibility)
        AddWaypoint(new Waypoint(
            "mine_entrance",
            "Mine Entrance",
            "The entrance to the dangerous mines",
            new Vector2(50f * TILE_SIZE, 200f * TILE_SIZE),
            WaypointType.MineshaftEntrance
        ));
        
        // Shop district - north of farm (kept for backwards compatibility)
        AddWaypoint(new Waypoint(
            "shop_district",
            "Shopping District",
            "Various shops and merchants",
            new Vector2(125f * TILE_SIZE, 50f * TILE_SIZE),
            WaypointType.ShopDistrict
        ));
    }
    
    /// <summary>
    /// Adds a waypoint to the system
    /// </summary>
    public void AddWaypoint(Waypoint waypoint)
    {
        if (!_waypoints.ContainsKey(waypoint.Id))
        {
            _waypoints[waypoint.Id] = waypoint;
            if (waypoint.IsUnlocked)
            {
                _unlockedWaypoints.Add(waypoint);
            }
        }
    }
    
    /// <summary>
    /// Unlocks a waypoint by discovering it
    /// </summary>
    public bool UnlockWaypoint(string waypointId)
    {
        if (_waypoints.TryGetValue(waypointId, out var waypoint))
        {
            if (!waypoint.IsUnlocked)
            {
                waypoint.IsUnlocked = true;
                _unlockedWaypoints.Add(waypoint);
                OnWaypointUnlocked?.Invoke(waypoint);
                return true;
            }
        }
        return false;
    }
    
    /// <summary>
    /// Checks if player is near a waypoint to unlock it
    /// </summary>
    public Waypoint CheckForNearbyWaypoint(Vector2 playerPosition, float discoverRadius = 64f)
    {
        foreach (var waypoint in _waypoints.Values)
        {
            if (!waypoint.IsUnlocked)
            {
                float distance = Vector2.Distance(playerPosition, waypoint.Position);
                if (distance <= discoverRadius)
                {
                    UnlockWaypoint(waypoint.Id);
                    return waypoint;
                }
            }
        }
        return null;
    }
    
    /// <summary>
    /// Fast travels to a waypoint
    /// </summary>
    public bool TravelToWaypoint(string waypointId, int playerGold, out Vector2 destination, out int cost)
    {
        destination = Vector2.Zero;
        cost = 0;
        
        if (!_waypoints.TryGetValue(waypointId, out var waypoint))
        {
            return false;
        }
        
        if (!waypoint.IsUnlocked)
        {
            return false;
        }
        
        // Calculate cost
        cost = CalculateTravelCost(waypoint);
        
        // Check if player can afford
        if (playerGold < cost)
        {
            return false;
        }
        
        destination = waypoint.Position;
        OnFastTravel?.Invoke(waypoint, cost);
        return true;
    }
    
    /// <summary>
    /// Calculates the gold cost to travel to a waypoint
    /// </summary>
    private int CalculateTravelCost(Waypoint waypoint)
    {
        // Farm is free
        if (waypoint.Type == WaypointType.Farm)
        {
            return 0;
        }
        
        // Base cost varies by waypoint type
        int cost = BASE_TRAVEL_COST;
        
        switch (waypoint.Type)
        {
            case WaypointType.Village:
                cost = 25;
                break;
            case WaypointType.DungeonEntrance:
                cost = 100;
                break;
            case WaypointType.MineshaftEntrance:
                cost = 50;
                break;
            case WaypointType.ShopDistrict:
                cost = 20;
                break;
            case WaypointType.Landmark:
                cost = 30;
                break;
        }
        
        return cost;
    }
    
    /// <summary>
    /// Gets all unlocked waypoints
    /// </summary>
    public List<Waypoint> GetUnlockedWaypoints()
    {
        return new List<Waypoint>(_unlockedWaypoints);
    }
    
    /// <summary>
    /// Gets all waypoints (including locked)
    /// </summary>
    public List<Waypoint> GetAllWaypoints()
    {
        return new List<Waypoint>(_waypoints.Values);
    }
    
    /// <summary>
    /// Gets a waypoint by ID
    /// </summary>
    public Waypoint GetWaypoint(string waypointId)
    {
        _waypoints.TryGetValue(waypointId, out var waypoint);
        return waypoint;
    }
    
    /// <summary>
    /// Checks if a waypoint is unlocked
    /// </summary>
    public bool IsWaypointUnlocked(string waypointId)
    {
        return _waypoints.TryGetValue(waypointId, out var waypoint) && waypoint.IsUnlocked;
    }
    
    /// <summary>
    /// Gets statistics about waypoints
    /// </summary>
    public WaypointStats GetStats()
    {
        int total = _waypoints.Count;
        int unlocked = _unlockedWaypoints.Count;
        
        return new WaypointStats
        {
            TotalWaypoints = total,
            UnlockedWaypoints = unlocked,
            LockedWaypoints = total - unlocked,
            UnlockPercentage = total > 0 ? (unlocked / (float)total) * 100f : 0f
        };
    }
    
    /// <summary>
    /// Gets the time cost for fast travel in game hours
    /// </summary>
    public float GetTimeCost() => TIME_COST_HOURS;
    
    /// <summary>
    /// Gets the gold cost to travel to a specific waypoint without attempting travel
    /// </summary>
    /// <param name="waypointId">ID of the waypoint to check</param>
    /// <returns>Gold cost, or -1 if waypoint not found</returns>
    public int GetTravelCost(string waypointId)
    {
        if (_waypoints.TryGetValue(waypointId, out var waypoint))
        {
            return CalculateTravelCost(waypoint);
        }
        return -1;
    }
}

/// <summary>
/// Statistics about waypoint system
/// </summary>
public struct WaypointStats
{
    public int TotalWaypoints;
    public int UnlockedWaypoints;
    public int LockedWaypoints;
    public float UnlockPercentage;
}
