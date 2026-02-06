using MoonBrookRidge.Core.Scenes;
using MoonBrookRidge.World.Interiors;
using MoonBrookRidge.World.Maps;
using MoonBrookRidge.Dungeons;
using System.Collections.Generic;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Initializes all scenes in the game and registers them with the SceneManager
/// Handles creation of farm, farmhouse, villages, dungeons, and their connections
/// </summary>
public class SceneInitializer
{
    private SceneManager _sceneManager;
    private Dictionary<string, DungeonFloorGenerator> _dungeonGenerators;
    
    public SceneInitializer(SceneManager sceneManager)
    {
        _sceneManager = sceneManager;
        _dungeonGenerators = new Dictionary<string, DungeonFloorGenerator>();
    }
    
    /// <summary>
    /// Initialize all starting scenes (farm, farmhouse, and first dungeon overworlds)
    /// </summary>
    public void InitializeStartingScenes()
    {
        // Create and register farmhouse interior
        var farmhouse = new FarmhouseInterior();
        _sceneManager.RegisterScene(farmhouse);
        
        // Create and register farm exterior
        var farm = new FarmExteriorScene();
        _sceneManager.RegisterScene(farm);
        
        // Set parent-child relationship
        farmhouse.ParentScene = farm;
        
        // Initialize first dungeon overworld (example: Slime Cave)
        InitializeDungeon("SlimeCave");
    }
    
    /// <summary>
    /// Initialize a dungeon with its overworld and floor generator
    /// </summary>
    public void InitializeDungeon(string dungeonName)
    {
        // Create dungeon overworld
        var dungeonOverworld = new DungeonOverworldScene(dungeonName);
        _sceneManager.RegisterScene(dungeonOverworld);
        
        // Create floor generator for this dungeon
        var floorGenerator = new DungeonFloorGenerator(dungeonName);
        _dungeonGenerators[dungeonName] = floorGenerator;
        
        // Pre-generate first 3 floors for smooth start
        for (int floor = 1; floor <= 3; floor++)
        {
            var dungeonFloor = floorGenerator.GetFloor(floor);
            _sceneManager.RegisterScene(dungeonFloor);
            dungeonFloor.ParentScene = dungeonOverworld;
        }
    }
    
    /// <summary>
    /// Get or generate a dungeon floor (lazy loading)
    /// </summary>
    public DungeonScene GetDungeonFloor(string dungeonName, int floorNumber)
    {
        if (!_dungeonGenerators.TryGetValue(dungeonName, out var generator))
        {
            // Initialize dungeon if not yet created
            InitializeDungeon(dungeonName);
            generator = _dungeonGenerators[dungeonName];
        }
        
        var floor = generator.GetFloor(floorNumber);
        
        // Register with scene manager if not already registered
        if (!_sceneManager.HasScene(floor.SceneId))
        {
            _sceneManager.RegisterScene(floor);
            
            // Set parent to dungeon overworld
            var overworldId = $"dungeon_overworld_{dungeonName}";
            var overworld = _sceneManager.GetScene(overworldId);
            if (overworld != null)
            {
                floor.ParentScene = overworld;
            }
        }
        
        return floor;
    }
    
    /// <summary>
    /// Initialize all 8 dungeons (can be called later for performance)
    /// </summary>
    public void InitializeAllDungeons()
    {
        string[] dungeonNames = {
            "SlimeCave",
            "SkeletonCrypt",
            "SpiderNest",
            "GoblinWarrens",
            "HauntedManor",
            "DragonLair",
            "DemonRealm",
            "AncientRuins"
        };
        
        foreach (var dungeonName in dungeonNames)
        {
            if (!_dungeonGenerators.ContainsKey(dungeonName))
            {
                InitializeDungeon(dungeonName);
            }
        }
    }
    
    /// <summary>
    /// Initialize village scenes (can be added later)
    /// </summary>
    public void InitializeVillages()
    {
        // TODO: Create village scenes
        // Each village will be an ExteriorScene with shops/houses as InteriorScenes
        
        string[] villages = {
            "MoonBrook Valley",
            "Pinewood Village",
            "Stonehelm Village",
            "Sandshore Village",
            "Frostpeak Village",
            "Marshwood Village",
            "Crystalgrove Village",
            "Ruinwatch Village"
        };
        
        // Villages to be implemented
    }
    
    /// <summary>
    /// Clear distant dungeon floors to save memory
    /// </summary>
    public void OptimizeDungeonMemory(string dungeonName, int currentFloor)
    {
        if (_dungeonGenerators.TryGetValue(dungeonName, out var generator))
        {
            generator.ClearDistantFloors(currentFloor, keepRange: 3);
        }
    }
    
    /// <summary>
    /// Get dungeon floor generator for a dungeon
    /// </summary>
    public DungeonFloorGenerator? GetDungeonGenerator(string dungeonName)
    {
        return _dungeonGenerators.TryGetValue(dungeonName, out var generator) ? generator : null;
    }
}
