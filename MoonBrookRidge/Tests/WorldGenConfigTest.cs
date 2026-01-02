using System;
using System.IO;
using MoonBrookRidge.World.Maps;
using MoonBrookRidge.World.Mining;
using MoonBrookRidge.World.Tiles;

namespace MoonBrookRidge.Tests;

/// <summary>
/// Test class for world generation configuration loading
/// </summary>
public static class WorldGenConfigTest
{
    /// <summary>
    /// Run all world gen config tests
    /// </summary>
    public static void RunTests()
    {
        Console.WriteLine("=== World Generation Config Tests ===\n");
        
        TestWorldConfigLoading();
        TestMineConfigLoading();
        TestWorldGeneration();
        TestMineGeneration();
        TestConfigHelper();
        
        Console.WriteLine("\n=== All Tests Complete ===");
    }
    
    private static void TestWorldConfigLoading()
    {
        Console.WriteLine("Test 1: Load World Configuration");
        
        string configPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Content", "WorldGen", "default_world.json"
        );
        
        var config = WorldGenConfigLoader.LoadWorldConfig(configPath);
        
        if (config != null)
        {
            Console.WriteLine($"✓ World config loaded successfully");
            Console.WriteLine($"  - Size: {config.Width}x{config.Height}");
            Console.WriteLine($"  - Seed: {config.RandomSeed}");
            Console.WriteLine($"  - Biomes: {config.Biomes.Count}");
            Console.WriteLine($"  - Paths: {config.Paths.Count}");
            Console.WriteLine($"  - Mine entrance: ({config.MineEntrance.X}, {config.MineEntrance.Y})");
        }
        else
        {
            Console.WriteLine("✗ Failed to load world config");
        }
        
        Console.WriteLine();
    }
    
    private static void TestMineConfigLoading()
    {
        Console.WriteLine("Test 2: Load Mine Configuration");
        
        string configPath = Path.Combine(
            AppDomain.CurrentDomain.BaseDirectory,
            "Content", "WorldGen", "default_mine.json"
        );
        
        var config = MineGenConfigLoader.LoadMineConfig(configPath);
        
        if (config != null)
        {
            Console.WriteLine($"✓ Mine config loaded successfully");
            Console.WriteLine($"  - Size: {config.Width}x{config.Height}");
            Console.WriteLine($"  - Seed base: {config.RandomSeedBase}");
            Console.WriteLine($"  - Base rooms: {config.Rooms.BaseCount}");
            Console.WriteLine($"  - Tunnel width: {config.Tunnels.Width}");
            Console.WriteLine($"  - Floor tile: {config.TileTypes.FloorTile}");
        }
        else
        {
            Console.WriteLine("✗ Failed to load mine config");
        }
        
        Console.WriteLine();
    }
    
    private static void TestWorldGeneration()
    {
        Console.WriteLine("Test 3: Generate World from Config");
        
        try
        {
            var config = new WorldGenConfig
            {
                Width = 20,
                Height = 20,
                RandomSeed = 123,
                DefaultTerrain = new DefaultTerrainConfig
                {
                    TileWeights = new System.Collections.Generic.List<TileWeightConfig>
                    {
                        new() { TileType = "Grass", Weight = 100 }
                    }
                },
                Biomes = new System.Collections.Generic.List<BiomeRegion>
                {
                    new()
                    {
                        Name = "Test Pond",
                        X = 8,
                        Y = 8,
                        Width = 4,
                        Height = 4,
                        Shape = "circle",
                        TileWeights = new System.Collections.Generic.List<TileWeightConfig>
                        {
                            new() { TileType = "Water", Weight = 100 }
                        }
                    }
                },
                MineEntrance = new MineEntranceConfig { X = 10, Y = 10 }
            };
            
            var tiles = WorldGenConfigApplier.GenerateWorld(config);
            
            if (tiles != null && tiles.GetLength(0) == 20 && tiles.GetLength(1) == 20)
            {
                Console.WriteLine("✓ World generated successfully");
                
                // Count tile types
                int waterCount = 0;
                int grassCount = 0;
                int mineEntranceCount = 0;
                
                for (int x = 0; x < 20; x++)
                {
                    for (int y = 0; y < 20; y++)
                    {
                        var tile = tiles[x, y];
                        if (tile.Type == TileType.Water) waterCount++;
                        else if (tile.Type == TileType.Grass) grassCount++;
                        else if (tile.Type == TileType.MineEntrance) mineEntranceCount++;
                    }
                }
                
                Console.WriteLine($"  - Water tiles: {waterCount}");
                Console.WriteLine($"  - Grass tiles: {grassCount}");
                Console.WriteLine($"  - Mine entrance: {mineEntranceCount}");
            }
            else
            {
                Console.WriteLine("✗ World generation failed");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Exception during world generation: {ex.Message}");
        }
        
        Console.WriteLine();
    }
    
    private static void TestMineGeneration()
    {
        Console.WriteLine("Test 4: Generate Mine from Config");
        
        try
        {
            var config = new MineGenConfig
            {
                Width = 30,
                Height = 30,
                RandomSeedBase = 200,
                Rooms = new RoomGenerationConfig
                {
                    BaseCount = 2,
                    CountPerLevel = 1,
                    MinWidth = 4,
                    MaxWidth = 6,
                    MinHeight = 4,
                    MaxHeight = 6,
                    MarginFromEdge = 3
                },
                Tunnels = new TunnelConfig
                {
                    Width = 2,
                    RandomTunnelsPerLevel = 1
                },
                Entrance = new EntranceConfig
                {
                    Position = "top-center",
                    Width = 5,
                    Height = 3
                },
                Exit = new ExitConfig
                {
                    Position = "bottom-center",
                    Width = 5,
                    Height = 3
                },
                TileTypes = new TileTypeConfig
                {
                    WallTile = "Stone",
                    FloorTile = "Dirt"
                }
            };
            
            var tiles = MineGenConfigApplier.GenerateMineLevel(config, 1);
            
            if (tiles != null && tiles.GetLength(0) == 30 && tiles.GetLength(1) == 30)
            {
                Console.WriteLine("✓ Mine generated successfully");
                
                // Count tile types
                int wallCount = 0;
                int floorCount = 0;
                
                for (int x = 0; x < 30; x++)
                {
                    for (int y = 0; y < 30; y++)
                    {
                        var tile = tiles[x, y];
                        if (tile.Type == TileType.Stone) wallCount++;
                        else if (tile.Type == TileType.Dirt) floorCount++;
                    }
                }
                
                Console.WriteLine($"  - Wall tiles: {wallCount}");
                Console.WriteLine($"  - Floor tiles: {floorCount}");
                Console.WriteLine($"  - Floor percentage: {(floorCount * 100.0 / (30 * 30)):F1}%");
            }
            else
            {
                Console.WriteLine("✗ Mine generation failed");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Exception during mine generation: {ex.Message}");
        }
        
        Console.WriteLine();
    }
    
    private static void TestConfigHelper()
    {
        Console.WriteLine("Test 5: WorldGenConfigHelper");
        
        try
        {
            // Test example configs
            var desertWorld = Core.WorldGenConfigHelper.CreateExampleDesertWorld();
            var deepMine = Core.WorldGenConfigHelper.CreateExampleDeepMine();
            
            if (desertWorld != null && deepMine != null)
            {
                Console.WriteLine("✓ Example configs created successfully");
                Console.WriteLine($"  - Desert world: {desertWorld.Width}x{desertWorld.Height}");
                Console.WriteLine($"  - Desert biomes: {desertWorld.Biomes.Count}");
                Console.WriteLine($"  - Deep mine: {deepMine.Width}x{deepMine.Height}");
                Console.WriteLine($"  - Deep mine base rooms: {deepMine.Rooms.BaseCount}");
            }
            else
            {
                Console.WriteLine("✗ Failed to create example configs");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"✗ Exception testing config helper: {ex.Message}");
        }
        
        Console.WriteLine();
    }
}
