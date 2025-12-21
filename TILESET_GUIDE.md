# Ground Tileset Guide - MoonBrook Ridge

## Overview

This document describes the custom ground tileset generated for MoonBrook Ridge. The tileset combines elements from multiple asset packs to create a cohesive visual style.

## Asset Pack Sources

The project combines assets from three sources:

1. **Sunnyside World** - Larger varied-size assets for characters, buildings, and objects
2. **16x16 Tilemap Assets** (in sprites/tilesets/) - Ground tiles, terrain
3. **Custom Generated Tileset** - `ground_tileset.png` - Combines best tiles from all sources

## Generated Tileset

### File Information
- **Location**: `Content/Textures/Tiles/ground_tileset.png`
- **Dimensions**: 256x192 pixels (16x12 tiles)
- **Tile Size**: 16x16 pixels
- **Total Tiles**: 192 tiles
- **Format**: PNG with alpha channel

### Tileset Layout

The tileset is organized in 12 rows of 16 tiles each:

#### Row 0: Grass Variants (Tiles 0-15)
16 different grass tile variations for natural-looking terrain
- Tiles 0-7: From Tilemap_color1
- Tiles 8-15: From Tilemap_color2

#### Row 1: Dirt & Paths (Tiles 16-31)
Dirt paths and transitional terrain
- Various dirt textures for pathways and bare ground

#### Row 2: Tilled Soil & Farmland (Tiles 32-47)
Agricultural tiles for farming
- Tiles 32-39: Tilled soil variants
- Tiles 40-47: Prepared farmland

#### Row 3: Stone & Rock (Tiles 48-63)
Stone paths, rock formations, and hard terrain
- 16 stone/rock tile variants

#### Row 4: Water (Tiles 64-79)
Water tiles with various patterns
- Use for rivers, ponds, lakes

#### Row 5: Sand & Beach (Tiles 80-95)
Sandy terrain for beaches and deserts
- 16 sand tile variants

#### Rows 6-7: Sunnyside World Tiles (Tiles 96-127)
Mixed terrain from the Sunnyside 16px tileset
- Additional ground varieties
- Forest terrain, decorative tiles

#### Rows 8-9: Extended Variants (Tiles 128-159)
More terrain options from Tilemap_color2
- Additional textures for variety

#### Rows 10-11: Sunnyside Extended (Tiles 160-191)
More Sunnyside World tiles
- Additional ground patterns and special tiles

## Individual Tile Files

For convenience, common tiles are also available as individual files:

| File | Description | Tile ID |
|------|-------------|---------|
| `grass_01.png` | Basic grass | 0 |
| `grass_02.png` | Grass variant 2 | 1 |
| `grass_03.png` | Grass variant 3 | 2 |
| `dirt_01.png` | Basic dirt | 16 |
| `dirt_02.png` | Dirt variant 2 | 17 |
| `tilled_01.png` | Tilled soil | 32 |
| `tilled_02.png` | Tilled soil variant | 33 |
| `stone_01.png` | Stone path | 48 |
| `water_01.png` | Water tile | 64 |
| `sand_01.png` | Sand tile | 80 |

## Usage in Code

### Loading the Tileset

```csharp
// In LoadContent() method
private Texture2D groundTileset;

protected override void LoadContent()
{
    groundTileset = Content.Load<Texture2D>("Textures/Tiles/ground_tileset");
}
```

### Drawing a Single Tile

```csharp
// Calculate source rectangle for a specific tile
int tileId = 5; // Grass variant 6
int tilesPerRow = 16;
int tileSize = 16;

int tileX = (tileId % tilesPerRow) * tileSize;
int tileY = (tileId / tilesPerRow) * tileSize;

Rectangle sourceRect = new Rectangle(tileX, tileY, tileSize, tileSize);
Rectangle destRect = new Rectangle(x * tileSize, y * tileSize, tileSize, tileSize);

spriteBatch.Draw(groundTileset, destRect, sourceRect, Color.White);
```

### Drawing a Tile Map

```csharp
// Example: Draw a simple grass field
int[,] tileMap = new int[10, 10];

// Fill with random grass tiles (0-15)
Random rand = new Random();
for (int y = 0; y < 10; y++)
{
    for (int x = 0; x < 10; x++)
    {
        tileMap[x, y] = rand.Next(0, 16);
    }
}

// Render the tile map
for (int y = 0; y < 10; y++)
{
    for (int x = 0; x < 10; x++)
    {
        int tileId = tileMap[x, y];
        int tileX = (tileId % 16) * 16;
        int tileY = (tileId / 16) * 16;
        
        Rectangle sourceRect = new Rectangle(tileX, tileY, 16, 16);
        Rectangle destRect = new Rectangle(x * 16, y * 16, 16, 16);
        
        spriteBatch.Draw(groundTileset, destRect, sourceRect, Color.White);
    }
}
```

## Tile Selection Guidelines

### For Natural Terrain
- Use tiles 0-15 (grass variants) with random selection for natural variation
- Avoid repeating patterns - mix different grass tiles

### For Pathways
- Use tiles 16-31 (dirt/paths) for walking trails
- Can blend with grass tiles at edges

### For Farmland
- Use tiles 32-47 for agricultural areas
- Different tiles represent different soil conditions

### For Stone Areas
- Use tiles 48-63 for rocky terrain or stone paths
- Good for dungeons, caves, or paved areas

### For Water Features
- Use tiles 64-79 for rivers, lakes, ponds
- Consider animating by cycling through consecutive tiles

### For Beaches
- Use tiles 80-95 for sandy areas
- Transition from grass/dirt to sand at water edges

## Autotiling Recommendations

For seamless tile transitions, consider implementing an autotiling system:

1. **Corner Matching**: Check adjacent tiles and select appropriate border/corner sprites
2. **Blob Tiling**: Use Wang tiles or blob tiling patterns
3. **Weighted Random**: For grass/dirt, use weighted random selection from appropriate ranges

## Performance Tips

1. **Batch Rendering**: Draw all tiles in a single SpriteBatch call
2. **Culling**: Only render tiles visible in the camera view
3. **Caching**: Pre-calculate source rectangles for frequently used tiles
4. **Texture Atlas**: The tileset is already an atlas - minimize texture switches

## Regenerating the Tileset

If you need to modify or regenerate the tileset:

1. The generator script is at `/tmp/generate_tileset.py`
2. Edit the script to adjust tile selections
3. Run: `python3 /tmp/generate_tileset.py`
4. The tileset will be regenerated in `Content/Textures/Tiles/`
5. Rebuild the project: `dotnet build`

## Visual Style Notes

The tileset combines 16x16 pixel art from multiple sources:
- **Bright, colorful** aesthetic from the 16x16 tilemaps
- **Detailed terrain** from Sunnyside World 16px tileset
- **Cohesive palette** - colors selected to work well together

## Tile Index Reference

See `ground_tileset_index.txt` for a complete list of all 192 tiles with their IDs and descriptions.

## Future Enhancements

Potential additions to the tileset:
- Animated water tiles (separate texture)
- Seasonal variations (spring, summer, fall, winter)
- Weather effects (wet tiles, snow-covered)
- Transition tiles (grass-to-dirt, dirt-to-stone, etc.)
- Interior floor tiles
- Dungeon/cave tiles

---

**Generated**: December 2024  
**Version**: 1.0  
**Project**: MoonBrook Ridge
