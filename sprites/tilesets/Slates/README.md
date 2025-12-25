# Slates Tileset by Ivan Voirol

## Overview

The Slates tileset is a comprehensive 32x32px orthogonal tileset created by Ivan Voirol. This is version 2 of the tileset, featuring an improved color palette and expanded tile collection.

## File Information

- **Filename**: `Slates_32x32_v2.png`
- **Dimensions**: 1792 x 736 pixels
- **Tile Size**: 32x32 pixels
- **Grid Layout**: 56 columns x 23 rows
- **Total Tiles**: 1,288 tiles
- **Format**: PNG with alpha channel (RGBA)

## License

This tileset is released under the **CC-BY 4.0** (Creative Commons Attribution 4.0) license.

**Attribution Required**: Ivan Voirol

You are free to:
- Use in commercial and non-commercial projects
- Modify and adapt the tileset
- Redistribute

As long as you provide appropriate credit to Ivan Voirol.

## Source

- **Original Source**: [OpenGameArt.org](https://opengameart.org/content/slates-32x32px-orthogonal-tileset-by-ivan-voirol)
- **Artist**: Ivan Voirol
- **Artist Profile**: [Ivan Voirol on OpenGameArt](https://opengameart.org/users/ivan-voirol)

## Tileset Contents

The Slates tileset includes a comprehensive collection of tiles for building top-down game environments:

### Terrain
- Grass variations (different colors and patterns)
- Dirt and paths
- Stone floors and rocky terrain
- Sand and beach tiles
- Water tiles (rivers, ponds, oceans)
- Snow and ice tiles
- Lava tiles

### Structures
- Walls (various materials: stone, brick, wood, etc.)
- Floors and interior tiles
- Roofs and building tops
- Doors and windows
- Stairs and ladders
- Fences and boundaries
- Pontoons and bridges

### Natural Elements
- Trees (various types and sizes)
- Bushes and shrubs
- Rocks and boulders
- Flowers and plants
- Cliffs and elevation changes

### Objects and Details
- Signs and posts
- Columns and pillars
- Chests and containers
- Decorative elements
- Water features (waterfalls, fountains)
- Animated elements support

## Usage Notes

### Compatibility with MoonBrook Ridge

**Important**: MoonBrook Ridge currently uses **16x16 pixel tiles**, while this tileset uses **32x32 pixel tiles**.

To use this tileset in the game, you have several options:

1. **Scale Down to 16x16**: Use image editing tools to scale individual tiles down to 16x16 pixels
2. **Scale at Runtime**: Load 32x32 tiles and scale them during rendering (may look pixelated)
3. **Update Game Tile Size**: Modify the game's `TILE_SIZE` constant to 32 (requires code changes)
4. **Use for Larger Objects**: Use these tiles for buildings, large decorations, or special areas

### Recommended Approach

For best results with MoonBrook Ridge's current 16x16 tile system, consider:
- Using this tileset for buildings and large structures (which can span multiple 16x16 tiles)
- Extracting specific tiles and scaling them to 16x16 for terrain use
- Using as reference/inspiration for creating compatible 16x16 variants

### Animation Support

Some tiles in this set are designed to work together for animation sequences (e.g., water, lava, torches). Tiles are typically arranged horizontally in animation strips.

### Layering and Modular Design

The tileset is designed to be layered and modular:
- Background tiles (floors, terrain)
- Mid-layer tiles (furniture, objects)
- Foreground tiles (walls, roofs, tall objects)
- Overlay tiles (shadows, effects)

## Character Compatibility

According to the artist's notes, this 32x32px tileset works well with:
- **16x24 pixel characters** (recommended size for proper scale)
- Zelda-like sprites
- PixElthen character sprites
- Smaller 16x16 sprites can be used for children or distant characters

MoonBrook Ridge's current character sprites are larger (Sunnyside World assets), so scaling considerations apply.

## Integration Steps

To add tiles from this set to MoonBrook Ridge:

1. **Extract Desired Tiles**: Use a tileset editor or image editing tool to extract specific tiles
2. **Scale if Needed**: Resize to 16x16 pixels if using with current game tile system
3. **Copy to Content**: Place in `MoonBrookRidge/Content/Textures/Tiles/` or appropriate subdirectory
4. **Add to Content.mgcb**: Register each tile in the Content Pipeline configuration
5. **Update Code**: Add tile types to `TileType` enum and update `WorldMap` texture loading
6. **Build and Test**: Rebuild the project and verify tiles render correctly

## Tools and Resources

### Tileset Editors
- **Tiled Map Editor**: Industry-standard free tilemap editor
- **Aseprite**: Excellent for working with pixel art and tilesets
- **GIMP/Photoshop**: For extracting and editing individual tiles

### File Format Support
The tileset works with:
- Tiled (.tmx, .tsx formats)
- Godot Engine
- GameMaker Studio
- Unity (with sprite slicing)
- MonoGame/XNA (used by MoonBrook Ridge)

## Credits

When using this tileset in your project, please include credit:

```
Tileset: Slates v.2 [32x32px orthogonal tileset]
Artist: Ivan Voirol
Source: OpenGameArt.org
License: CC-BY 4.0
```

## Additional Resources

- **Other Voirol Tilesets**: Ivan Voirol has created other compatible tilesets including tinySLATES (16x16 version)
- **Community Variations**: The community has created palette swaps and variations
- **Tutorial Videos**: Search for "Slates tileset tutorial" for usage guides

## Version History

- **v1**: Initial release
- **v2** (this version): 
  - Improved color palette
  - Rearranged tiles for better organization
  - Added new tile variations
  - Enhanced visual consistency

---

**Downloaded**: December 2024  
**Added to MoonBrook Ridge**: December 2024  
**Source Repository**: https://github.com/shionn/whisp
