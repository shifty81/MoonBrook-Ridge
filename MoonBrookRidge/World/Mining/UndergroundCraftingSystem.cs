using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MoonBrookRidge.World.Mining;

/// <summary>
/// Workbench tiers following Core Keeper progression
/// Each tier unlocks new recipes and crafting capabilities
/// </summary>
public enum WorkbenchTier
{
    Basic,      // Starting workbench
    Copper,     // First ore tier
    Tin,        // Clay caves
    Iron,       // Forgotten ruins
    Scarlet,    // Mid-game, unlocks automation
    Octarine,   // Late game mystical tier
    Galaxite,   // Desert endgame
    Solarite    // Final tier
}

/// <summary>
/// Types of crafting stations in underground
/// </summary>
public enum StationType
{
    Workbench,      // Main crafting hub
    Anvil,          // Weapons and armor
    Smelter,        // Ore refining
    Furnace,        // Advanced smelting
    AutomationTable,// Drills, conveyors, automation
    AlchemyTable,   // Potions and chemicals
    Electronics,    // Advanced circuits
    CookingStation  // Food preparation
}

/// <summary>
/// Represents a crafting station in the underground
/// Core Keeper-inspired tiered crafting system
/// </summary>
public class CraftingStation
{
    public string Id { get; set; }
    public string Name { get; set; }
    public StationType Type { get; set; }
    public WorkbenchTier Tier { get; set; }
    public Vector2 Position { get; set; }
    public bool IsPlaced { get; set; }
    public List<string> UnlockedRecipes { get; set; }
    
    /// <summary>
    /// Resources required to build this station
    /// </summary>
    public Dictionary<string, int> BuildCost { get; set; }
    
    public CraftingStation(string id, string name, StationType type, WorkbenchTier tier)
    {
        Id = id;
        Name = name;
        Type = type;
        Tier = tier;
        Position = Vector2.Zero;
        IsPlaced = false;
        UnlockedRecipes = new List<string>();
        BuildCost = new Dictionary<string, int>();
    }
}

/// <summary>
/// Manages Core Keeper-inspired tiered crafting stations
/// Progression: Basic → Copper → Tin → Iron → Scarlet → Octarine → Galaxite → Solarite
/// </summary>
public class UndergroundCraftingSystem
{
    private Dictionary<string, CraftingStation> _stations;
    private Dictionary<WorkbenchTier, List<string>> _tierRecipes;
    private WorkbenchTier _currentMaxTier;
    
    public event Action<CraftingStation>? OnStationPlaced;
    public event Action<WorkbenchTier>? OnTierUnlocked;
    
    public WorkbenchTier CurrentMaxTier => _currentMaxTier;
    
    public UndergroundCraftingSystem()
    {
        _stations = new Dictionary<string, CraftingStation>();
        _tierRecipes = new Dictionary<WorkbenchTier, List<string>>();
        _currentMaxTier = WorkbenchTier.Basic;
        
        InitializeStationTemplates();
        InitializeTierRecipes();
    }
    
    /// <summary>
    /// Initialize all crafting station templates
    /// </summary>
    private void InitializeStationTemplates()
    {
        // Basic Tier
        AddStationTemplate(new CraftingStation("basic_workbench", "Basic Workbench", StationType.Workbench, WorkbenchTier.Basic)
        {
            BuildCost = new Dictionary<string, int> { { "Wood", 10 } }
        });
        
        // Copper Tier
        AddStationTemplate(new CraftingStation("copper_workbench", "Copper Workbench", StationType.Workbench, WorkbenchTier.Copper)
        {
            BuildCost = new Dictionary<string, int> { { "Wood", 15 }, { "CopperBar", 5 } }
        });
        
        AddStationTemplate(new CraftingStation("copper_anvil", "Copper Anvil", StationType.Anvil, WorkbenchTier.Copper)
        {
            BuildCost = new Dictionary<string, int> { { "Stone", 20 }, { "CopperBar", 8 } }
        });
        
        AddStationTemplate(new CraftingStation("smelter", "Smelter", StationType.Smelter, WorkbenchTier.Copper)
        {
            BuildCost = new Dictionary<string, int> { { "Stone", 25 }, { "CopperBar", 5 } }
        });
        
        // Tin Tier
        AddStationTemplate(new CraftingStation("tin_workbench", "Tin Workbench", StationType.Workbench, WorkbenchTier.Tin)
        {
            BuildCost = new Dictionary<string, int> { { "Wood", 20 }, { "TinBar", 8 } }
        });
        
        AddStationTemplate(new CraftingStation("tin_anvil", "Tin Anvil", StationType.Anvil, WorkbenchTier.Tin)
        {
            BuildCost = new Dictionary<string, int> { { "Stone", 25 }, { "TinBar", 10 } }
        });
        
        // Iron Tier
        AddStationTemplate(new CraftingStation("iron_workbench", "Iron Workbench", StationType.Workbench, WorkbenchTier.Iron)
        {
            BuildCost = new Dictionary<string, int> { { "Wood", 25 }, { "IronBar", 10 } }
        });
        
        AddStationTemplate(new CraftingStation("iron_anvil", "Iron Anvil", StationType.Anvil, WorkbenchTier.Iron)
        {
            BuildCost = new Dictionary<string, int> { { "Stone", 30 }, { "IronBar", 15 } }
        });
        
        AddStationTemplate(new CraftingStation("furnace", "Furnace", StationType.Furnace, WorkbenchTier.Iron)
        {
            BuildCost = new Dictionary<string, int> { { "Stone", 40 }, { "IronBar", 12 } }
        });
        
        // Scarlet Tier - Unlocks Automation!
        AddStationTemplate(new CraftingStation("scarlet_workbench", "Scarlet Workbench", StationType.Workbench, WorkbenchTier.Scarlet)
        {
            BuildCost = new Dictionary<string, int> { { "Wood", 30 }, { "ScarletBar", 15 } }
        });
        
        AddStationTemplate(new CraftingStation("scarlet_anvil", "Scarlet Anvil", StationType.Anvil, WorkbenchTier.Scarlet)
        {
            BuildCost = new Dictionary<string, int> { { "Stone", 35 }, { "ScarletBar", 20 } }
        });
        
        AddStationTemplate(new CraftingStation("automation_table", "Automation Table", StationType.AutomationTable, WorkbenchTier.Scarlet)
        {
            BuildCost = new Dictionary<string, int> { { "Wood", 25 }, { "ScarletBar", 15 }, { "IronBar", 10 } }
        });
        
        // Octarine Tier - Mystical
        AddStationTemplate(new CraftingStation("octarine_workbench", "Octarine Workbench", StationType.Workbench, WorkbenchTier.Octarine)
        {
            BuildCost = new Dictionary<string, int> { { "Wood", 35 }, { "OctarineBar", 20 } }
        });
        
        AddStationTemplate(new CraftingStation("alchemy_table", "Alchemy Table", StationType.AlchemyTable, WorkbenchTier.Octarine)
        {
            BuildCost = new Dictionary<string, int> { { "Wood", 30 }, { "OctarineBar", 15 }, { "Crystal", 10 } }
        });
        
        // Galaxite Tier - Desert/Endgame
        AddStationTemplate(new CraftingStation("galaxite_workbench", "Galaxite Workbench", StationType.Workbench, WorkbenchTier.Galaxite)
        {
            BuildCost = new Dictionary<string, int> { { "Wood", 40 }, { "GalaxiteBar", 25 } }
        });
        
        // Solarite Tier - Final
        AddStationTemplate(new CraftingStation("solarite_workbench", "Solarite Workbench", StationType.Workbench, WorkbenchTier.Solarite)
        {
            BuildCost = new Dictionary<string, int> { { "Wood", 50 }, { "SolariteBar", 30 }, { "Pandorium", 5 } }
        });
    }
    
    private void AddStationTemplate(CraftingStation station)
    {
        _stations[station.Id] = station;
    }
    
    /// <summary>
    /// Initialize recipes unlocked at each tier
    /// </summary>
    private void InitializeTierRecipes()
    {
        // Basic Tier
        _tierRecipes[WorkbenchTier.Basic] = new List<string>
        {
            "WoodenPickaxe", "WoodenAxe", "BasicTorch", "WoodenWall", "WoodenFloor"
        };
        
        // Copper Tier
        _tierRecipes[WorkbenchTier.Copper] = new List<string>
        {
            "CopperPickaxe", "CopperAxe", "CopperSword", "CopperArmor", "CopperWall"
        };
        
        // Tin Tier
        _tierRecipes[WorkbenchTier.Tin] = new List<string>
        {
            "TinPickaxe", "TinAxe", "TinSword", "TinArmor", "ImprovedTorch"
        };
        
        // Iron Tier
        _tierRecipes[WorkbenchTier.Iron] = new List<string>
        {
            "IronPickaxe", "IronAxe", "IronSword", "IronArmor", "IronChest"
        };
        
        // Scarlet Tier - Automation unlocked!
        _tierRecipes[WorkbenchTier.Scarlet] = new List<string>
        {
            "ScarletPickaxe", "ScarletSword", "ScarletArmor", "Drill", "ConveyorBelt", "RoboticArm"
        };
        
        // Octarine Tier
        _tierRecipes[WorkbenchTier.Octarine] = new List<string>
        {
            "OctarinePickaxe", "OctarineSword", "MagicalArmor", "TeleportationPad"
        };
        
        // Galaxite Tier
        _tierRecipes[WorkbenchTier.Galaxite] = new List<string>
        {
            "GalaxitePickaxe", "GalaxiteSword", "SpaceArmor", "AdvancedDrill"
        };
        
        // Solarite Tier
        _tierRecipes[WorkbenchTier.Solarite] = new List<string>
        {
            "SolaritePickaxe", "SolariteSword", "LegendaryArmor", "MasterAutomation"
        };
    }
    
    /// <summary>
    /// Unlock a new crafting tier
    /// Usually triggered by finding new ores or defeating bosses
    /// </summary>
    public void UnlockTier(WorkbenchTier tier)
    {
        if (tier > _currentMaxTier)
        {
            _currentMaxTier = tier;
            OnTierUnlocked?.Invoke(tier);
        }
    }
    
    /// <summary>
    /// Place a crafting station in the world
    /// </summary>
    public bool PlaceStation(string stationId, Vector2 position)
    {
        if (!_stations.TryGetValue(stationId, out var station))
            return false;
        
        // Check if player has unlocked this tier
        if (station.Tier > _currentMaxTier)
            return false;
        
        // Create a new instance
        var placedStation = new CraftingStation(station.Id, station.Name, station.Type, station.Tier)
        {
            Position = position,
            IsPlaced = true,
            BuildCost = station.BuildCost,
            UnlockedRecipes = GetRecipesForTier(station.Tier)
        };
        
        OnStationPlaced?.Invoke(placedStation);
        return true;
    }
    
    /// <summary>
    /// Get all recipes available at a tier
    /// </summary>
    public List<string> GetRecipesForTier(WorkbenchTier tier)
    {
        var recipes = new List<string>();
        
        // Include recipes from current tier and all previous tiers
        foreach (WorkbenchTier t in Enum.GetValues(typeof(WorkbenchTier)))
        {
            if (t <= tier && _tierRecipes.ContainsKey(t))
            {
                recipes.AddRange(_tierRecipes[t]);
            }
        }
        
        return recipes;
    }
    
    /// <summary>
    /// Get all station templates for a tier
    /// </summary>
    public List<CraftingStation> GetStationsForTier(WorkbenchTier tier)
    {
        var stations = new List<CraftingStation>();
        
        foreach (var station in _stations.Values)
        {
            if (station.Tier == tier)
            {
                stations.Add(station);
            }
        }
        
        return stations;
    }
    
    /// <summary>
    /// Check if automation is unlocked (Scarlet tier or higher)
    /// </summary>
    public bool IsAutomationUnlocked()
    {
        return _currentMaxTier >= WorkbenchTier.Scarlet;
    }
    
    /// <summary>
    /// Get tier name for display
    /// </summary>
    public string GetTierDisplayName(WorkbenchTier tier)
    {
        return tier switch
        {
            WorkbenchTier.Basic => "Basic Tools",
            WorkbenchTier.Copper => "Copper Age",
            WorkbenchTier.Tin => "Tin Age",
            WorkbenchTier.Iron => "Iron Age",
            WorkbenchTier.Scarlet => "Scarlet Era (Automation Unlocked!)",
            WorkbenchTier.Octarine => "Mystical Age",
            WorkbenchTier.Galaxite => "Galactic Age",
            WorkbenchTier.Solarite => "Solar Age",
            _ => "Unknown"
        };
    }
}
