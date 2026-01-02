# Example Integration of Asset-Driven World Generation

## Quick Start

This guide shows how to integrate the new asset-driven world generation system into your game.

## Option 1: Automatic Loading (Recommended)

Modify `GameplayState.Initialize()` to automatically load world configs:

```csharp
public override void Initialize()
{
    base.Initialize();
    
    // Initialize core systems
    _timeSystem = new TimeSystem();
    _eventSystem = new EventSystem(_timeSystem);
    // ... other system initialization
    
    // Initialize world with config support
    _worldMap = new WorldMap();
    
    // Try to load world from config, fallback to hardcoded if it fails
    WorldGenConfigHelper.TryLoadWorldConfig(_worldMap, "default_world.json");
    
    // Rest of initialization...
    _player = new PlayerCharacter(new Vector2(27.5f * GameConstants.TILE_SIZE, 27.5f * GameConstants.TILE_SIZE));
    // ...
}
```

## Option 2: Manual Config Loading

For more control, load and apply configs manually:

```csharp
using System;
using System.IO;
using MoonBrookRidge.World.Maps;

public void InitializeWorldFromConfig()
{
    // Build path to config file
    string configPath = Path.Combine(
        AppDomain.CurrentDomain.BaseDirectory,
        "Content", "WorldGen", "default_world.json"
    );
    
    // Load configuration
    var config = WorldGenConfigLoader.LoadWorldConfig(configPath);
    
    if (config != null)
    {
        // Initialize world from config
        _worldMap.InitializeFromConfig(config);
        Console.WriteLine("World generated from configuration");
    }
    else
    {
        // Fallback: world will use hardcoded InitializeMap()
        Console.WriteLine("Using default hardcoded world generation");
    }
}
```

## Option 3: Programmatic Configuration

Create configs in code without JSON files:

```csharp
using MoonBrookRidge.World.Maps;

public void CreateCustomWorld()
{
    var config = new WorldGenConfig
    {
        Width = 50,
        Height = 50,
        RandomSeed = 42,
        DefaultTerrain = new DefaultTerrainConfig
        {
            TileWeights = new List<TileWeightConfig>
            {
                new() { TileType = "Grass", Weight = 60 },
                new() { TileType = "Grass01", Weight = 40 }
            }
        },
        Biomes = new List<BiomeRegion>
        {
            new()
            {
                Name = "Farm Area",
                X = 20,
                Y = 20,
                Width = 15,
                Height = 15,
                Shape = "rectangle",
                TileWeights = new List<TileWeightConfig>
                {
                    new() { TileType = "Grass", Weight = 100 }
                }
            }
        }
    };
    
    // Apply to world
    _worldMap.InitializeFromConfig(config);
}
```

## Mine Generation Integration

### MiningManager Integration

Update your `MiningManager` to support config-based mine generation:

```csharp
public class MiningManager
{
    private MineGenConfig _mineConfig;
    
    public void Initialize()
    {
        // Try to load mine config
        _mineConfig = WorldGenConfigHelper.TryLoadMineConfig("default_mine.json");
    }
    
    public void CreateMineLevel(int level)
    {
        MineLevel mineLevel;
        
        if (_mineConfig != null)
        {
            // Use config-based generation
            mineLevel = new MineLevel(level, _mineConfig, _rockTexture, _rockSprites);
        }
        else
        {
            // Fallback to hardcoded generation
            mineLevel = new MineLevel(level, 40, 40, _rockTexture, _rockSprites);
        }
        
        _mineLevels[level] = mineLevel;
    }
}
```

## Testing Your Configuration

### 1. Verify JSON Syntax

Use a JSON validator (online or in IDE) to check for syntax errors.

### 2. Test World Generation

Run the game and check console output:
```
Successfully loaded world config from: Content/WorldGen/default_world.json
Initializing world from config: default_world.json
```

### 3. Visual Verification

- Look for your configured biomes (ponds, beaches, etc.)
- Check that paths appear where configured
- Verify mine entrance is at the specified location
- Test mine generation on different levels

### 4. Debug Output

Add logging to see what's happening:

```csharp
var config = WorldGenConfigLoader.LoadWorldConfig(configPath);
if (config != null)
{
    Console.WriteLine($"Loaded config: {config.Width}x{config.Height}");
    Console.WriteLine($"Biomes: {config.Biomes.Count}");
    Console.WriteLine($"Paths: {config.Paths.Count}");
}
```

## Creating Variants

### Different World Types

Create multiple config files for variety:

```
Content/WorldGen/
├── default_world.json       # Standard farm world
├── desert_world.json        # Sandy desert world
├── forest_world.json        # Dense forest world
├── island_world.json        # Surrounded by water
└── snow_world.json          # Winter/tundra world
```

Load different configs based on save file or player choice:

```csharp
string worldType = saveData.WorldType ?? "default";
string configFile = $"{worldType}_world.json";
WorldGenConfigHelper.TryLoadWorldConfig(_worldMap, configFile);
```

## Troubleshooting

### Config Not Found
**Error:** `World config file not found: Content/WorldGen/default_world.json`

**Solution:** Ensure the JSON files are copied to the output directory. Check your project file or copy them manually.

### Invalid JSON
**Error:** `Error loading world config: Unexpected character...`

**Solution:** Validate JSON syntax. Common issues:
- Missing commas between objects
- Trailing commas (some parsers don't allow them)
- Unquoted property names
- Mismatched brackets

### Wrong Tile Types
**Error:** Silent failure, default grass everywhere

**Solution:** Check that tile type names match exactly (case-sensitive):
- Correct: `"Grass"`, `"Water"`, `"Sand"`
- Wrong: `"grass"`, `"WATER"`, `"sand"`

See `Tile.cs` TileType enum for exact names.

### Performance Issues
**Problem:** Game runs slowly with custom world

**Solution:**
- Reduce world size (Width/Height)
- Limit number of biomes
- Use rectangles instead of circles/ellipses
- Reduce path complexity

## Best Practices

1. **Start Small** - Begin with the default configs and make small changes
2. **Test Frequently** - Run the game after each config change
3. **Version Control** - Keep backups of working configs
4. **Comment Your JSON** - Add `// comments` explaining complex setups (if your JSON parser supports it)
5. **Use Fallbacks** - Always keep the hardcoded generation working as backup

## Performance Tips

- **Large Worlds**: Keep under 100x100 tiles for good performance
- **Biome Count**: Aim for fewer than 20 biomes
- **Shape Complexity**: Rectangles are fastest, circles/ellipses are slower
- **Path Count**: More paths = more processing time

## Next Steps

1. Copy this integration code into your `GameplayState.cs`
2. Run the game to test default configs
3. Modify `default_world.json` to experiment
4. Create custom world variants
5. Share your configs with the community!

## See Also

- [WORLDGEN_ASSET_GUIDE.md](WORLDGEN_ASSET_GUIDE.md) - Complete configuration reference
- `WorldGenConfig.cs` - Configuration class definitions
- `WorldGenConfigHelper.cs` - Helper utilities
- `default_world.json` - Sample world configuration
- `default_mine.json` - Sample mine configuration
