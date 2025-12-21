# Sprite Asset Guide

## Overview

MoonBrook Ridge uses the **Sunnyside World** sprite collection, a comprehensive pixel art asset pack designed for farming and life simulation games. This guide explains how to use these assets in the game.

## Asset Organization

### Directory Structure

```
sprites/
├── Buildings/                          # Structures and buildings
├── Characters/                         # NPC character sprites
├── Color Pallet/                       # Color reference
├── Decorations/                        # Decorative objects
├── Particle FX/                        # Visual effects
├── Resources/                          # Trees, rocks, plants
├── Sunnyside World - Tileset/          # Ground tiles
├── Sunnyside_World_ASSET_PACK_V2.1/   # Main asset pack
├── SUNNYSIDE_WORLD_ASSETS_V0.2/       # Additional assets
├── SUNNYSIDE_WORLD_BUILDINGS_V0.01/   # Building sprites
├── SUNNYSIDE_WORLD_CHARACTERS_PARTS_V0.3.1/  # Character parts
├── SUNNYSIDE_WORLD_CHARACTERS_V0.3.1/  # Full character sprites
├── SUNNYSIDE_WORLD_CHIMNEYSMOKE_v1.0/ # Smoke animations
├── SUNNYSIDE_WORLD_CROPS_V0.01/       # Crop growth stages
├── SUNNYSIDE_WORLD_GOBLIN_V0.1/       # Enemy sprites
├── Units/                              # Sprite units
├── characters/                         # Character animations
├── objects/                            # Interactive objects
├── particles/                          # Particle effects
└── tilesets/                          # Terrain tilesets
```

## Character Sprites

### Character Parts System
Located in: `SUNNYSIDE_WORLD_CHARACTERS_PARTS_V0.3.1/`

The character system uses a modular approach with separate layers:

#### Base Layers
- `base_*_strip*.png` - Body base (skin tone)
- Different actions: walking, mining, fishing, doing, caught, watering, carrying, etc.

#### Hair Styles
- `bowlhair_*_strip*.png`
- `curlyhair_*_strip*.png`
- `longhair_*_strip*.png`
- `shorthair_*_strip*.png`
- `spikeyhair_*_strip*.png`
- `mophair_*_strip*.png`

#### Tools Overlay
- `tools_*_strip*.png` - Shows tools in character's hands

### Animation Strips
Format: `[part]_[action]_strip[framecount].png`

Examples:
- `base_walk_strip8.png` - 8 frames of walking animation
- `longhair_mining_strip10.png` - 10 frames of mining with long hair
- `tools_fishing_strip10.png` - 10 frames of fishing tool animation

### Usage Example
```csharp
// To create a character:
// 1. Load base sprite
// 2. Load hair sprite (optional)
// 3. Load tool sprite (if using tool)
// 4. Layer them on top of each other during rendering
// 5. Animate by cycling through strip frames

Texture2D baseSprite = Content.Load<Texture2D>("characters/base_walk_strip8");
Texture2D hairSprite = Content.Load<Texture2D>("characters/longhair_walk_strip8");
Texture2D toolSprite = Content.Load<Texture2D>("characters/tools_walk_strip8");

// Draw layered:
spriteBatch.Draw(baseSprite, position, sourceRect, Color.White);
spriteBatch.Draw(hairSprite, position, sourceRect, hairColor);
spriteBatch.Draw(toolSprite, position, sourceRect, Color.White);
```

## Tileset Sprites

### Ground Tiles
Located in: `Sunnyside World - Tileset/` and `tilesets/`

Tile types:
- Grass (various types)
- Dirt paths
- Stone paths
- Water (animated)
- Sand
- Tilled soil
- And more...

### Tile Size
Most tiles are **16x16 pixels**

### Auto-tiling
Many tilesets support auto-tiling (connecting tiles automatically):
- Use tile IDs to determine which variant to display
- Check neighboring tiles to select correct border/corner sprites

### Usage Example
```csharp
Texture2D tileset = Content.Load<Texture2D>("tilesets/ground");

// For a 16x16 tile at grid position (0, 0) using tile ID 5:
int tilesPerRow = tileset.Width / 16;
int tileX = (tileId % tilesPerRow) * 16;
int tileY = (tileId / tilesPerRow) * 16;
Rectangle sourceRect = new Rectangle(tileX, tileY, 16, 16);

spriteBatch.Draw(tileset, destinationRect, sourceRect, Color.White);
```

## Crop Sprites

Located in: `SUNNYSIDE_WORLD_CROPS_V0.01/`

### Growth Stages
Each crop has multiple growth stages:
1. Planted/Seeds
2. Sprout
3. Young plant
4. Mature plant
5. Ready to harvest

Example crops:
- Wheat
- Corn
- Tomatoes
- Carrots
- Pumpkins
- And more...

### Implementation
```csharp
// Track growth stage in Crop class
public class Crop
{
    private int _currentStage;
    private int _maxStages = 5;
    private Texture2D[] _stageSprites;
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.Draw(_stageSprites[_currentStage], position, Color.White);
    }
}
```

## Building Sprites

Located in: `Buildings/` and `SUNNYSIDE_WORLD_BUILDINGS_V0.01/`

Building types:
- Houses (various sizes)
- Barns
- Coops
- Silos
- Sheds
- Wells
- Fences
- And more...

### Multi-tile Buildings
Large buildings span multiple tiles:
- Store building dimensions (e.g., 3x4 tiles)
- Calculate collision based on building footprint
- Draw as single sprite at base position

## Resource Sprites

Located in: `Resources/`

Harvestable resources:
- Trees (fruit trees, normal trees)
- Rocks (various sizes)
- Forageable plants
- Bushes
- Flowers

### Animated Resources
Some resources have multiple states:
- Tree with fruit vs. without fruit
- Tree stump after cutting
- Broken rock pieces

## Decorative Objects

Located in: `Decorations/`

Decorative items:
- Furniture
- Signs
- Lamps
- Paths
- Garden decorations
- Seasonal decorations

## Particle Effects

Located in: `Particle FX/` and `particles/`

Effects:
- Dust clouds
- Water splashes
- Sparkles
- Impact effects
- Magic effects

### Animation
Particles are typically:
- Short-lived (< 1 second)
- Play once and disappear
- Use sprite sheets for animation

## UI Elements

UI sprites (if included):
- Buttons
- Frames
- Icons
- Cursors
- Dialogue boxes

## Best Practices

### Loading Assets
```csharp
// Load sprites in LoadContent() method
protected override void LoadContent()
{
    _characterBase = Content.Load<Texture2D>("sprites/characters/base_walk");
    _tileset = Content.Load<Texture2D>("sprites/tilesets/ground");
    // ... load other assets
}
```

### Organizing Content
```
Content/
├── Textures/
│   ├── Characters/
│   ├── Tiles/
│   ├── Buildings/
│   └── UI/
├── Fonts/
├── Sounds/
└── Music/
```

### Performance Tips
1. **Use Texture Atlases**: Combine small sprites into larger textures
2. **Reduce Texture Switches**: Group draw calls by texture
3. **Use Point Sampling**: `SamplerState.PointClamp` for crisp pixel art
4. **Cache Rectangles**: Pre-calculate source rectangles for sprite sheets

### Sprite Batching
```csharp
// Begin batch with appropriate settings
spriteBatch.Begin(
    SpriteSortMode.Deferred,
    BlendState.AlphaBlend,
    SamplerState.PointClamp,  // Important for pixel art!
    null,
    null,
    null,
    camera.GetTransform()
);

// Draw all sprites
DrawWorld(spriteBatch);
DrawEntities(spriteBatch);

// End batch
spriteBatch.End();
```

## Animation System

### Frame-based Animation
```csharp
public class Animation
{
    private Texture2D _spriteSheet;
    private int _frameCount;
    private int _currentFrame;
    private float _frameTime;
    private float _timer;
    
    public void Update(GameTime gameTime)
    {
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_timer >= _frameTime)
        {
            _currentFrame = (_currentFrame + 1) % _frameCount;
            _timer = 0;
        }
    }
    
    public Rectangle GetSourceRectangle()
    {
        int frameWidth = _spriteSheet.Width / _frameCount;
        return new Rectangle(_currentFrame * frameWidth, 0, frameWidth, _spriteSheet.Height);
    }
}
```

## Color Customization

### Using Color Palette
Located in: `Color Pallet/`

Apply colors to sprites:
```csharp
// Tint a sprite
Color hairColor = new Color(139, 69, 19); // Brown hair
spriteBatch.Draw(hairSprite, position, sourceRect, hairColor);

// Character skin tones
Color skinTone = new Color(255, 220, 177); // Light skin
spriteBatch.Draw(baseSprite, position, sourceRect, skinTone);
```

## Common Issues

### Blurry Sprites
**Solution**: Use `SamplerState.PointClamp` instead of default linear filtering

### Gaps Between Tiles
**Solution**: 
- Ensure pixel-perfect positioning
- Use integer positions for tile coordinates
- Check for floating-point rounding errors

### Animation Jitter
**Solution**:
- Use consistent frame times
- Base animation on game time, not frame rate
- Consider deltaTime for smooth animation

### Missing Sprites
**Solution**:
- Verify file paths match Content.Load() calls
- Check that files are added to Content.mgcb
- Ensure Build Action is set to "Build"

## Asset License

**Important**: Verify you have appropriate licensing for the Sunnyside World assets before commercial use. These assets may have specific usage terms.

## Additional Resources

- MonoGame Documentation: https://docs.monogame.net/
- Sprite Packing Tools: TexturePacker, Aseprite
- Animation Tools: Aseprite, GraphicsGale
- Tilemap Editors: Tiled, LDtk

---

*This guide is for the MoonBrook Ridge development team*
