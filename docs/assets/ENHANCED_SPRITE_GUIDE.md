# Enhanced Sprite and Tileset Utilization Guide

## Overview

This guide documents the enhanced sprite and tileset system in MoonBrook Ridge, which now fully utilizes the extensive Sunnyside World asset collection and GameMaker template data.

## What's New

### 1. **1,081 New Asset Files Organized**
   - **Cooking Items**: 320 sprites in `Content/Textures/Objects/Cooking/`
   - **Forage Items**: 152 sprites in `Content/Textures/Objects/Forage/`
   - **Minerals**: 171 sprites in `Content/Textures/Objects/Minerals/`
   - **Artifacts**: 123 sprites in `Content/Textures/Objects/Artifacts/`
   - **Additional Crops**: 172 sprites in `Content/Textures/Crops/Additional/`
   - **Artisan Goods**: 60 sprites in `Content/Textures/Objects/ArtisanGoods/`
   - **Fruits & Vegetables**: 70 sprites in `Content/Textures/Objects/FruitsVegetables/`
   - **Tools**: 8 sprites in `Content/Textures/Objects/Tools/`
   - **Farm Animals**: 5 sprite sheets in `Content/Textures/Objects/Farm/`

### 2. **GameMaker Autotiling Support**
   - Extracted autotiling rules from GameMaker templates
   - 10+ predefined terrain autotile sets (Land, Paths, River, Buildings, Clouds)
   - Seamless terrain transitions using blob/wang tiling
   - Located in `World/Tiles/Generated/AutoTiling/`

### 3. **Tile Animation System**
   - 25+ animation sequences extracted from GameMaker
   - Water animations (3 variants)
   - Flame/torch effects (2 variants)
   - Flag and banner animations
   - Plant swaying effects
   - Special effects (sparkles, portals)
   - Located in `World/Tiles/Generated/Animations/`

### 4. **New Helper Classes**
   - `AutoTileRenderer`: Seamless terrain transitions
   - `TileAnimationManager`: Animated tile effects
   - Extended `Tile` class with animation and sprite ID support

## Using Autotiling

### Basic Autotiling

```csharp
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.World.Tiles.AutoTiling;

// Create autotile renderer
var autoTileRenderer = new AutoTileRenderer(sunnysideTilesetHelper);

// Define which tile types use autotiling
var tileTypeMapping = new Dictionary<TileType, int[]>
{
    { TileType.Grass, SunnysideworldAutoTileRules.AutoTileSets.Land },
    { TileType.Dirt, SunnysideworldAutoTileRules.AutoTileSets.Path01 },
    { TileType.Water, SunnysideworldAutoTileRules.AutoTileSets.River }
};

// Apply autotiling to a region
autoTileRenderer.ApplyAutoTiling(
    tiles,           // Your tile map
    0, 0,           // Start position
    mapWidth, mapHeight,  // Size
    tileTypeMapping // Tile type to autotile set mapping
);
```

### Available Autotile Sets

From `SunnysideworldAutoTileRules.AutoTileSets`:
- **Land**: Grass/ground terrain
- **Path01**: Dirt paths
- **Path02**: Stone paths
- **Path03**: Cobblestone paths
- **River**: Water transitions
- **Building01**: Building walls type 1
- **Building02**: Building walls type 2
- **InnerWalls**: Indoor walls
- **Clouds01**: Cloud layer 1
- **Clouds02**: Cloud layer 2
- **CloudShadow**: Cloud shadows

### Helper Method

```csharp
// Get autotile set by terrain name
int[] autoTileSet = AutoTileRenderer.GetAutoTileSetForTerrainType("grass");
int[] pathSet = AutoTileRenderer.GetAutoTileSetForTerrainType("dirt");
```

## Using Tile Animations

### Setup

```csharp
using MoonBrookRidge.World.Tiles;

// Create animation manager
var animationManager = new TileAnimationManager();

// Update in your game loop
public override void Update(GameTime gameTime)
{
    animationManager.Update(gameTime);
    animationManager.UpdateAnimatedTiles(tiles); // Updates sprite IDs
    
    base.Update(gameTime);
}
```

### Applying Animations to Tiles

```csharp
// During map generation
if (tiles[x, y].Type == TileType.Water)
{
    // Apply water animation
    animationManager.ApplyAnimationToTile(tiles[x, y], "water_1");
}
```

### Available Animations

- **Water**: `water_1`, `water_2`, `water_3`
- **Fire**: `flame_1`, `flame_2`
- **Decorations**: `flag`, `banner`
- **Plants**: `plant_1`, `plant_2`, `plant_3`
- **Effects**: `sparkle`, `portal`

### Custom Animation

```csharp
// Check if animation exists
if (animationManager.HasAnimation("water_1"))
{
    // Get current frame ID for rendering
    int frameId = animationManager.GetCurrentFrame("water_1");
}

// List all available animations
foreach (string animName in animationManager.GetAvailableAnimations())
{
    Console.WriteLine($"Animation: {animName}");
}
```

## Extended Tile Properties

The `Tile` class now includes:

```csharp
public class Tile
{
    // ... existing properties ...
    
    /// <summary>
    /// Animation name for animated tiles (empty = no animation)
    /// </summary>
    public string AnimationName { get; set; }
    
    /// <summary>
    /// Sprite ID from tileset (-1 = use default for tile type)
    /// Set by autotiling or animation systems
    /// </summary>
    public int SpriteId { get; set; }
}
```

## Integration Example: Complete Scene

```csharp
public class EnhancedWorldMap : WorldMap
{
    private AutoTileRenderer _autoTileRenderer;
    private TileAnimationManager _animationManager;
    
    public void Initialize(SunnysideTilesetHelper tilesetHelper)
    {
        _autoTileRenderer = new AutoTileRenderer(tilesetHelper);
        _animationManager = new TileAnimationManager();
        
        // Generate base terrain
        GenerateBaseTerrain();
        
        // Apply autotiling for smooth transitions
        ApplyAutoTiling();
        
        // Add animated elements
        AddAnimatedTiles();
    }
    
    private void ApplyAutoTiling()
    {
        var mapping = new Dictionary<TileType, int[]>
        {
            { TileType.Grass, AutoTileRenderer.GetAutoTileSetForTerrainType("grass") },
            { TileType.Dirt, AutoTileRenderer.GetAutoTileSetForTerrainType("path1") },
            { TileType.Water, AutoTileRenderer.GetAutoTileSetForTerrainType("water") }
        };
        
        _autoTileRenderer.ApplyAutoTiling(
            _tiles, 0, 0, 
            _width, _height, 
            mapping
        );
    }
    
    private void AddAnimatedTiles()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                if (_tiles[x, y].Type == TileType.Water)
                {
                    // Vary water animation for visual interest
                    string waterAnim = (x + y) % 3 == 0 ? "water_1" : 
                                     (x + y) % 3 == 1 ? "water_2" : "water_3";
                    _animationManager.ApplyAnimationToTile(_tiles[x, y], waterAnim);
                }
            }
        }
    }
    
    public override void Update(GameTime gameTime)
    {
        _animationManager.Update(gameTime);
        _animationManager.UpdateAnimatedTiles(_tiles);
        
        base.Update(gameTime);
    }
}
```

## Asset Organization Scripts

### organize_sunnyside_assets.py

Organizes the main Sunnyside World assets from `sprites/` into `Content/Textures/`.

```bash
# Dry run (preview only)
python3 tools/organize_sunnyside_assets.py

# Execute with confirmation
python3 tools/organize_sunnyside_assets.py --execute

# Execute without confirmation
python3 tools/organize_sunnyside_assets.py --execute --force

# Show help
python3 tools/organize_sunnyside_assets.py --help
```

### organize_needs_sorted.py

Organizes additional assets from `sprites/needs sorted/`.

```bash
# Dry run
python3 tools/organize_needs_sorted.py

# Execute
python3 tools/organize_needs_sorted.py --execute

# Generate asset catalog
python3 tools/organize_needs_sorted.py --catalog
```

### parse_gamemaker_tilesets.py

Extracts autotiling and animation data from GameMaker .yy files.

```bash
# List available tilesets
python3 tools/parse_gamemaker_tilesets.py --list

# Generate C# code
python3 tools/parse_gamemaker_tilesets.py
```

## Best Practices

### 1. **Autotiling**
   - Apply autotiling after initial terrain generation
   - Use appropriate autotile sets for each terrain type
   - Consider 8-directional checking for smoother corners
   - Reapply autotiling when terrain changes

### 2. **Animations**
   - Update animation manager in your game loop
   - Apply animations during map initialization
   - Vary animation types for visual interest
   - Consider frame rate impact on performance

### 3. **Performance**
   - Batch sprite draws by texture
   - Use frustum culling for off-screen tiles
   - Cache autotile calculations when possible
   - Limit animated tiles to visible areas

### 4. **Asset Loading**
   - Use lazy loading for large asset collections
   - Preload frequently used assets
   - Consider texture atlases for small sprites
   - Use Content.mgcb for asset management

## Scene Examples

Reference images showing proper asset usage:
- `sprites/` directory contains example scenes
- `sprites/needs sorted/ASSET_CATALOG.md` lists available assets
- Study the autotile patterns in GameMaker templates

## Troubleshooting

### Autotiling Not Working
- Ensure tile types are correctly set in mapping
- Check that autotile sets have 16+ tiles
- Verify neighbor detection logic
- Test with simpler 4-directional first

### Animations Not Showing
- Verify `Update()` calls animation manager
- Check `UpdateAnimatedTiles()` is called before rendering
- Ensure animation name matches exactly
- Confirm tile has AnimationName set

### Missing Sprites
- Verify files are in Content/Textures/
- Check Content.mgcb includes the files
- Ensure Build Action is set to "Build"
- Rebuild content project

## Next Steps

### Phase 5: Testing
- Create test scenes for each feature
- Add dev mode for asset preview
- Implement hot-reload

### Phase 6: Cleanup
- Archive unused assets
- Update documentation
- Create asset usage guide

## Support

For questions or issues:
1. Check the documentation files in the root directory
2. Review the generated code in `World/Tiles/Generated/`
3. See example usage in `WorldMap.cs`
4. Consult `SPRITE_GUIDE.md` and `TILESET_GUIDE.md`

---

**Last Updated**: January 2026
**Version**: 1.0
