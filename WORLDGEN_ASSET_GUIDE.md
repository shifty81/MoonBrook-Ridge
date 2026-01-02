# Asset-Driven World Generation Guide

## Overview

MoonBrook Ridge now supports asset-driven world generation, allowing designers to easily modify terrain layouts, biomes, and mine structures by editing JSON configuration files instead of changing C# code.

## Architecture

### Components

1. **WorldGenConfig** - Configuration class for overworld generation
2. **MineGenConfig** - Configuration class for mine generation
3. **WorldGenConfigApplier** - Applies world configuration to generate tiles
4. **MineGenConfigApplier** - Applies mine configuration to generate tiles
5. **WorldGenConfigLoader** - Loads world configs from JSON files
6. **MineGenConfigLoader** - Loads mine configs from JSON files

### Configuration Files Location

Configuration files are stored in:
```
MoonBrookRidge/Content/WorldGen/
├── default_world.json     # Overworld configuration
└── default_mine.json      # Mine generation configuration
```

## World Generation Configuration

### Structure

```json
{
  "Width": 50,
  "Height": 50,
  "RandomSeed": 42,
  "MineEntrance": {
    "X": 10,
    "Y": 40
  },
  "DefaultTerrain": {
    "TileWeights": [
      { "TileType": "Grass", "Weight": 40 },
      { "TileType": "Grass01", "Weight": 25 }
    ]
  },
  "Biomes": [
    {
      "Name": "Central Farm Area",
      "X": 20,
      "Y": 20,
      "Width": 15,
      "Height": 15,
      "Shape": "rectangle",
      "TileWeights": [
        { "TileType": "Grass", "Weight": 50 }
      ]
    }
  ],
  "Paths": [
    {
      "Name": "North Path",
      "StartX": 27,
      "StartY": 15,
      "EndX": 27,
      "EndY": 20,
      "Width": 2,
      "TileWeights": [
        { "TileType": "Dirt", "Weight": 100 }
      ]
    }
  ]
}
```

### Configuration Options

#### World Properties
- **Width** (int) - World width in tiles (default: 50)
- **Height** (int) - World height in tiles (default: 50)
- **RandomSeed** (int) - Seed for random generation (default: 42)

#### Mine Entrance
- **X** (int) - X coordinate for mine entrance
- **Y** (int) - Y coordinate for mine entrance

#### Default Terrain
- **TileWeights** (array) - Tile types and their spawn weights for areas not covered by biomes

#### Biomes
Biomes are regions with specific terrain characteristics:
- **Name** (string) - Descriptive name for the biome
- **X**, **Y** (int) - Top-left corner position
- **Width**, **Height** (int) - Size of the biome
- **Shape** (string) - Shape type: "rectangle", "circle", or "ellipse"
- **TileWeights** (array) - Tile types and weights for this biome

#### Paths
Paths connect different areas:
- **Name** (string) - Descriptive name
- **StartX**, **StartY** (int) - Starting position
- **EndX**, **EndY** (int) - Ending position
- **Width** (int) - Path width in tiles
- **TileWeights** (array) - Tile types for the path

### Tile Types

Available tile types include:
- Grass variants: `Grass`, `Grass01`, `Grass02`, `Grass03`
- Dirt variants: `Dirt`, `Dirt01`, `Dirt02`
- Water variants: `Water`, `Water01`
- Sand variants: `Sand`, `Sand01`
- Stone variants: `Stone`, `Stone01`, `Rock`
- Farmland: `Tilled`, `TilledDry`, `TilledWatered`
- Special: `MineEntrance`

See `Tile.cs` for the complete list of available tile types.

### Tile Weights

Weights determine the probability of a tile type being selected:
- Higher weight = more likely to appear
- Weights are relative to each other
- Example: `{"TileType": "Grass", "Weight": 70}` + `{"TileType": "Grass01", "Weight": 30}` = 70% Grass, 30% Grass01

## Mine Generation Configuration

### Structure

```json
{
  "Width": 40,
  "Height": 40,
  "RandomSeedBase": 100,
  "Rooms": {
    "BaseCount": 3,
    "CountPerLevel": 1,
    "MinWidth": 4,
    "MaxWidth": 8,
    "MinHeight": 4,
    "MaxHeight": 8,
    "MarginFromEdge": 5
  },
  "Tunnels": {
    "Width": 2,
    "RandomTunnelsPerLevel": 1
  },
  "Entrance": {
    "Position": "top-center",
    "Width": 5,
    "Height": 4
  },
  "Exit": {
    "Position": "bottom-center",
    "Width": 5,
    "Height": 4
  },
  "TileTypes": {
    "WallTile": "Stone",
    "FloorTile": "Dirt"
  }
}
```

### Configuration Options

#### Mine Properties
- **Width** (int) - Mine level width in tiles
- **Height** (int) - Mine level height in tiles
- **RandomSeedBase** (int) - Base seed (each level adds to this)

#### Rooms
- **BaseCount** (int) - Initial number of rooms
- **CountPerLevel** (int) - Additional rooms per deeper level
- **MinWidth**, **MaxWidth** (int) - Room width range
- **MinHeight**, **MaxHeight** (int) - Room height range
- **MarginFromEdge** (int) - Minimum distance from map edge

#### Tunnels
- **Width** (int) - Tunnel width in tiles
- **RandomTunnelsPerLevel** (int) - Additional random connections per level

#### Entrance/Exit
- **Position** (string) - Position: "top-center", "top-left", "bottom-center", etc.
- **Width** (int) - Area width
- **Height** (int) - Area height

#### Tile Types
- **WallTile** (string) - Tile type for walls
- **FloorTile** (string) - Tile type for floors

## Using Configuration Files

### In Code (WorldMap)

```csharp
// Load configuration
string configPath = Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory, 
    "Content/WorldGen/default_world.json"
);
var config = WorldGenConfigLoader.LoadWorldConfig(configPath);

// Apply to world
if (config != null)
{
    worldMap.InitializeFromConfig(config);
}
```

### In Code (MineLevel)

```csharp
// Load mine configuration
string configPath = Path.Combine(
    AppDomain.CurrentDomain.BaseDirectory,
    "Content/WorldGen/default_mine.json"
);
var config = MineGenConfigLoader.LoadMineConfig(configPath);

// Create mine level with config
if (config != null)
{
    var mineLevel = new MineLevel(level, config, rockTexture, rockSprites);
}
```

### Fallback Behavior

If configuration files are not found or fail to load:
- WorldMap will use hardcoded `InitializeMap()` method
- MineLevel will use hardcoded generation parameters
- Console warnings will be printed

## Creating Custom Worlds

### Example: Desert World

Create `desert_world.json`:

```json
{
  "Width": 50,
  "Height": 50,
  "RandomSeed": 123,
  "DefaultTerrain": {
    "TileWeights": [
      { "TileType": "Sand", "Weight": 80 },
      { "TileType": "Sand01", "Weight": 20 }
    ]
  },
  "Biomes": [
    {
      "Name": "Oasis",
      "X": 25,
      "Y": 25,
      "Width": 8,
      "Height": 8,
      "Shape": "circle",
      "TileWeights": [
        { "TileType": "Water", "Weight": 100 }
      ]
    }
  ]
}
```

### Example: Cave Mine

Create `cave_mine.json`:

```json
{
  "Width": 60,
  "Height": 60,
  "RandomSeedBase": 200,
  "Rooms": {
    "BaseCount": 5,
    "CountPerLevel": 2,
    "MinWidth": 6,
    "MaxWidth": 12,
    "MinHeight": 6,
    "MaxHeight": 12,
    "MarginFromEdge": 3
  },
  "Tunnels": {
    "Width": 3,
    "RandomTunnelsPerLevel": 2
  },
  "TileTypes": {
    "WallTile": "Stone",
    "FloorTile": "Stone01"
  }
}
```

## Best Practices

### Design Guidelines

1. **Balance Biome Sizes** - Don't make biomes too large relative to world size
2. **Use Appropriate Shapes** - Circles for ponds, rectangles for farms
3. **Path Connectivity** - Ensure paths connect important areas
4. **Tile Variety** - Use multiple tile types with varied weights for visual interest
5. **Random Seeds** - Change seeds for different world variations

### Performance Considerations

1. **World Size** - Larger worlds (>100x100) may impact performance
2. **Biome Count** - Keep biomes under 20 for optimal performance
3. **Shape Complexity** - Circles/ellipses are slightly slower than rectangles

### Testing

1. **Validate JSON** - Use a JSON validator before loading
2. **Test Generation** - Run the game to see visual results
3. **Iterate** - Adjust weights and positions based on results
4. **Version Control** - Keep backup configs when experimenting

## Troubleshooting

### Config Not Loading

**Problem:** "World config file not found" message

**Solution:**
1. Verify file exists in `Content/WorldGen/`
2. Check file path is correct
3. Ensure JSON is valid (no syntax errors)

### Unexpected Terrain

**Problem:** Terrain doesn't match expectations

**Solution:**
1. Check tile type names match exactly (case-sensitive)
2. Verify weight calculations (higher weight = more common)
3. Review biome overlap (later biomes override earlier ones)
4. Test with different random seeds

### Mine Generation Issues

**Problem:** Rooms not connecting properly

**Solution:**
1. Increase tunnel width
2. Add more random tunnels per level
3. Increase room count
4. Adjust margin from edge

## Future Enhancements

Potential additions to the system:
- Tile transition rules (smooth biome edges)
- Decorative object placement configs
- NPC spawn point definitions
- Quest location markers
- Seasonal terrain variations
- Procedural structure placement

## References

- **WorldGenConfig.cs** - Configuration class definitions
- **WorldGenConfigApplier.cs** - Application logic
- **Tile.cs** - Available tile types
- **WorldMap.cs** - World map implementation

---

*Last Updated: January 2026*
*Project: MoonBrook Ridge*
