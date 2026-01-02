# World Generation Configurations

This directory contains JSON configuration files for world generation.

## Available Configurations

### Overworld Maps

- **default_world.json** - Standard farm world with pond, beaches, and paths
- **desert_world.json** - Sandy desert world with central oasis

### Mine Configurations

- **default_mine.json** - Standard mine with moderate room sizes
- **deep_mine.json** - Larger, more complex mine with bigger rooms

## Usage

To use a specific world configuration, load it in your game code:

```csharp
using MoonBrookRidge.Core;

// Load specific world config
WorldGenConfigHelper.TryLoadWorldConfig(_worldMap, "desert_world.json");

// Load specific mine config
var mineConfig = WorldGenConfigHelper.TryLoadMineConfig("deep_mine.json");
```

## Creating Custom Configurations

1. Copy an existing JSON file as a template
2. Modify the values (see WORLDGEN_ASSET_GUIDE.md for details)
3. Save with a descriptive name (e.g., `forest_world.json`)
4. Load in game using the filename

## Configuration Files

### World Configuration Format

```json
{
  "Width": 50,
  "Height": 50,
  "RandomSeed": 42,
  "MineEntrance": { "X": 10, "Y": 40 },
  "DefaultTerrain": { "TileWeights": [...] },
  "Biomes": [...],
  "Paths": [...]
}
```

### Mine Configuration Format

```json
{
  "Width": 40,
  "Height": 40,
  "RandomSeedBase": 100,
  "Rooms": {...},
  "Tunnels": {...},
  "Entrance": {...},
  "Exit": {...},
  "TileTypes": {...}
}
```

## Tips

- Start with default configs and make small changes
- Test changes by running the game
- Keep backups of working configurations
- Use meaningful RandomSeed values for reproducible worlds

## Documentation

For complete documentation, see:
- [WORLDGEN_ASSET_GUIDE.md](../../../WORLDGEN_ASSET_GUIDE.md) - Full configuration reference
- [WORLDGEN_INTEGRATION_EXAMPLE.md](../../../WORLDGEN_INTEGRATION_EXAMPLE.md) - Integration guide

## Examples

The provided configs demonstrate different world types:
- **default_world.json** - Balanced farm with water features
- **desert_world.json** - Arid environment with central oasis
- **default_mine.json** - Standard procedural cave system
- **deep_mine.json** - Expansive underground complex
