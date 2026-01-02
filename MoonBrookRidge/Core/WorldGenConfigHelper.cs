using System;
using System.IO;
using MoonBrookRidge.World.Maps;
using MoonBrookRidge.World.Mining;

namespace MoonBrookRidge.Core;

/// <summary>
/// Helper class to demonstrate loading and using world generation configurations
/// </summary>
public static class WorldGenConfigHelper
{
    /// <summary>
    /// Try to load world configuration from file, falling back to hardcoded generation if it fails
    /// </summary>
    /// <param name="worldMap">The WorldMap instance to initialize</param>
    /// <param name="configFileName">Optional config file name (defaults to "default_world.json")</param>
    /// <returns>True if config was loaded, false if using fallback</returns>
    public static bool TryLoadWorldConfig(WorldMap worldMap, string configFileName = "default_world.json")
    {
        try
        {
            // Build path to config file
            string configPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Content", "WorldGen", configFileName
            );
            
            // Try to load the config
            var config = WorldGenConfigLoader.LoadWorldConfig(configPath);
            
            if (config != null)
            {
                Console.WriteLine($"Initializing world from config: {configFileName}");
                worldMap.InitializeFromConfig(config);
                return true;
            }
            else
            {
                Console.WriteLine($"Failed to load world config, using default generation");
                return false;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading world config: {ex.Message}");
            Console.WriteLine("Using default world generation");
            return false;
        }
    }
    
    /// <summary>
    /// Try to load mine configuration from file
    /// </summary>
    /// <param name="configFileName">Optional config file name (defaults to "default_mine.json")</param>
    /// <returns>MineGenConfig if successful, null otherwise</returns>
    public static MineGenConfig TryLoadMineConfig(string configFileName = "default_mine.json")
    {
        try
        {
            // Build path to config file
            string configPath = Path.Combine(
                AppDomain.CurrentDomain.BaseDirectory,
                "Content", "WorldGen", configFileName
            );
            
            // Try to load the config
            var config = MineGenConfigLoader.LoadMineConfig(configPath);
            
            if (config != null)
            {
                Console.WriteLine($"Loaded mine config: {configFileName}");
                return config;
            }
            else
            {
                Console.WriteLine($"Failed to load mine config, using default generation");
                return null;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading mine config: {ex.Message}");
            Console.WriteLine("Using default mine generation");
            return null;
        }
    }
    
    /// <summary>
    /// Example of creating a custom world config programmatically
    /// </summary>
    public static WorldGenConfig CreateExampleDesertWorld()
    {
        return new WorldGenConfig
        {
            Width = 50,
            Height = 50,
            RandomSeed = 999,
            MineEntrance = new MineEntranceConfig { X = 25, Y = 45 },
            DefaultTerrain = new DefaultTerrainConfig
            {
                TileWeights = new System.Collections.Generic.List<TileWeightConfig>
                {
                    new() { TileType = "Sand", Weight = 70 },
                    new() { TileType = "Sand01", Weight = 30 }
                }
            },
            Biomes = new System.Collections.Generic.List<BiomeRegion>
            {
                new()
                {
                    Name = "Oasis",
                    X = 22,
                    Y = 22,
                    Width = 10,
                    Height = 10,
                    Shape = "circle",
                    TileWeights = new System.Collections.Generic.List<TileWeightConfig>
                    {
                        new() { TileType = "Water", Weight = 100 }
                    }
                }
            }
        };
    }
    
    /// <summary>
    /// Example of creating a custom mine config programmatically
    /// </summary>
    public static MineGenConfig CreateExampleDeepMine()
    {
        return new MineGenConfig
        {
            Width = 60,
            Height = 60,
            RandomSeedBase = 500,
            Rooms = new RoomGenerationConfig
            {
                BaseCount = 5,
                CountPerLevel = 2,
                MinWidth = 6,
                MaxWidth = 12,
                MinHeight = 6,
                MaxHeight = 12,
                MarginFromEdge = 3
            },
            Tunnels = new TunnelConfig
            {
                Width = 3,
                RandomTunnelsPerLevel = 2
            },
            Entrance = new EntranceConfig
            {
                Position = "top-center",
                Width = 6,
                Height = 5
            },
            Exit = new ExitConfig
            {
                Position = "bottom-center",
                Width = 6,
                Height = 5
            },
            TileTypes = new TileTypeConfig
            {
                WallTile = "Stone",
                FloorTile = "Dirt"
            }
        };
    }
}
