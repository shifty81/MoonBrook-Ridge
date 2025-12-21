# Asset Loading Guide

## Overview

This guide explains how sprite assets are integrated into MoonBrook Ridge using the MonoGame Content Pipeline.

## Content Pipeline Structure

The game uses MonoGame's Content Pipeline to process and load assets. All game assets are stored in the `MoonBrookRidge/Content/` directory and managed through the `Content.mgcb` file.

### Directory Structure

```
Content/
├── Fonts/
│   └── Default.spritefont          # DejaVu Sans font for UI text
├── Textures/
│   ├── Characters/
│   │   ├── player.png              # Static player sprite
│   │   └── Animations/
│   │       ├── base_walk_strip8.png      # Walking animation (8 frames)
│   │       ├── base_run_strip8.png       # Running animation (8 frames)
│   │       ├── base_waiting_strip9.png   # Idle animation (9 frames)
│   │       ├── base_hamering_strip23.png # Tool use animation (23 frames)
│   │       └── base_dig_strip13.png      # Digging animation (13 frames)
│   ├── Tiles/
│   │   ├── grass.png               # Grass tile texture
│   │   └── plains.png              # Plains tile texture
│   ├── Crops/
│   │   ├── wheat_00.png to wheat_04.png   # Wheat growth stages
│   │   └── potato_00.png to potato_04.png # Potato growth stages
│   └── Buildings/
│       ├── House1.png              # Basic house sprite
│       └── House2.png              # Alternative house sprite
└── Content.mgcb                    # Content Pipeline configuration
```

## Adding New Assets

### Step 1: Copy Asset Files

Copy your PNG sprite files to the appropriate directory within `Content/Textures/`:

```bash
cp /path/to/sprite.png MoonBrookRidge/Content/Textures/[Category]/
```

### Step 2: Add to Content.mgcb

Add an entry to `Content.mgcb` for each asset:

```mgcb
#begin Textures/[Category]/sprite.png
/importer:TextureImporter
/processor:TextureProcessor
/processorParam:ColorKeyColor=255,0,255,255
/processorParam:ColorKeyEnabled=True
/processorParam:GenerateMipmaps=False
/processorParam:PremultiplyAlpha=True
/processorParam:ResizeToPowerOfTwo=False
/processorParam:MakeSquare=False
/processorParam:TextureFormat=Color
/build:Textures/[Category]/sprite.png
```

**Important Settings:**
- `ColorKeyEnabled=True` with `ColorKeyColor=255,0,255,255` - Makes magenta transparent
- `GenerateMipmaps=False` - Essential for pixel art to avoid blurriness
- `ResizeToPowerOfTwo=False` - Preserves original sprite dimensions
- `PremultiplyAlpha=True` - Proper alpha blending

### Step 3: Build the Project

Build the project to process the assets through the Content Pipeline:

```bash
cd MoonBrookRidge
dotnet build
```

### Step 4: Load in Code

Load the texture in your game code using the ContentManager:

```csharp
// In LoadContent method or similar
Texture2D myTexture = Content.Load<Texture2D>("Textures/[Category]/sprite");
```

**Note:** Do NOT include the file extension (.png) when loading assets.

## Loading Assets in Different Components

### In GameState Classes

GameState classes have access to `Game.Content`:

```csharp
public override void LoadContent()
{
    base.LoadContent();
    
    Texture2D playerTexture = Game.Content.Load<Texture2D>("Textures/Characters/player");
    Texture2D grassTexture = Game.Content.Load<Texture2D>("Textures/Tiles/grass");
}
```

### In Game Components

Pass textures to components through their constructors or LoadContent methods:

```csharp
// In component class
public void LoadContent(Texture2D texture)
{
    _texture = texture;
}

// In GameState
_player.LoadContent(playerTexture);
```

## Working with Sprite Sheets

Animation sprite sheets contain multiple frames in a single image file.

### Format

Animation files are named with the format: `[animation]_strip[framecount].png`

Examples:
- `base_walk_strip8.png` - 8 frames of walking animation
- `base_run_strip8.png` - 8 frames of running animation
- `base_waiting_strip9.png` - 9 frames of idle animation

### Loading and Using

```csharp
Texture2D walkAnimation = Content.Load<Texture2D>("Textures/Characters/Animations/base_walk_strip8");

// Calculate frame dimensions
int frameCount = 8;
int frameWidth = walkAnimation.Width / frameCount;
int frameHeight = walkAnimation.Height;

// Get source rectangle for specific frame
int currentFrame = 3;
Rectangle sourceRect = new Rectangle(
    currentFrame * frameWidth,
    0,
    frameWidth,
    frameHeight
);

// Draw the specific frame
spriteBatch.Draw(walkAnimation, position, sourceRect, Color.White);
```

## Rendering Settings

### Pixel Art Configuration

For crisp pixel art rendering, always use `SamplerState.PointClamp`:

```csharp
spriteBatch.Begin(
    transformMatrix: camera.GetTransform(),
    samplerState: SamplerState.PointClamp  // Critical for pixel art!
);
```

### Camera Scaling

The game uses a 2x camera zoom for comfortable viewing:

```csharp
_camera.Zoom = 2.0f;  // In Camera2D initialization
```

Individual sprites can be scaled further during rendering:

```csharp
spriteBatch.Draw(
    texture,
    position,
    null,
    Color.White,
    0f,
    origin,
    2.0f,  // Scale multiplier
    SpriteEffects.None,
    0f
);
```

## Font Assets

### Creating a SpriteFont

Font files use `.spritefont` XML descriptor format:

```xml
<?xml version="1.0" encoding="utf-8"?>
<XnaContent xmlns:Graphics="Microsoft.Xna.Framework.Content.Pipeline.Graphics">
  <Asset Type="Graphics:FontDescription">
    <FontName>DejaVu Sans</FontName>  <!-- System font name -->
    <Size>14</Size>
    <Spacing>0</Spacing>
    <UseKerning>true</UseKerning>
    <Style>Regular</Style>
    <CharacterRegions>
      <CharacterRegion>
        <Start>&#32;</Start>  <!-- Space -->
        <End>&#126;</End>     <!-- Tilde -->
      </CharacterRegion>
    </CharacterRegions>
  </Asset>
</XnaContent>
```

### Loading Fonts

```csharp
SpriteFont font = Content.Load<SpriteFont>("Fonts/Default");

// Drawing text
spriteBatch.DrawString(font, "Hello World", position, Color.White);
```

## Asset Source

All sprite assets are from the **Sunnyside World** asset pack, located in the `/sprites` directory at the repository root. This directory contains:

- 10,000+ PNG sprite files
- Character parts (base, hair, tools)
- Animation strips for various actions
- Tilesets for terrain
- Crops with growth stages
- Buildings and decorations
- Particle effects

## Best Practices

### 1. Organize by Category

Keep assets organized in subdirectories:
- Characters - Player and NPC sprites
- Tiles - Ground and terrain textures
- Crops - Farming plants with growth stages
- Buildings - Structures and constructions
- UI - Interface elements

### 2. Naming Conventions

- Use lowercase with underscores: `base_walk_strip8.png`
- Include frame count in animation names: `_strip8`, `_strip13`
- Include growth stage for crops: `wheat_00`, `wheat_01`, etc.

### 3. Performance Optimization

- **Texture Atlases**: Combine related small sprites into larger textures
- **Reduce Texture Switches**: Load and draw sprites from the same texture consecutively
- **Cache Loaded Assets**: Load textures once, reuse throughout the game
- **Unload Unused Assets**: Call `Content.Unload()` when changing states

### 4. Pixel Art Settings

Always use these settings for pixel art:
- `SamplerState.PointClamp` - No smoothing/filtering
- `GenerateMipmaps=False` - No mipmaps
- Integer positions - Avoid subpixel rendering

## Troubleshooting

### Asset Not Found

**Error:** `ContentLoadException: Could not load Textures/...`

**Solutions:**
1. Check the path in Content.Load() matches the path in Content.mgcb
2. Ensure the asset was built (check bin/Content directory)
3. Don't include .png extension when loading
4. Path is case-sensitive on Linux

### Blurry Sprites

**Problem:** Sprites appear blurred or fuzzy

**Solution:** Use `SamplerState.PointClamp` in SpriteBatch.Begin()

### Font Not Found

**Error:** `Could not find "FontName" font file`

**Solutions:**
1. Use a system font available on the target platform
2. On Linux, use: DejaVu Sans, Liberation Sans
3. On Windows, use: Arial, Segoe UI
4. Alternatively, include a TTF font file in the project

### Build Failures

**Error:** Content Pipeline fails to build

**Solutions:**
1. Check Content.mgcb syntax is correct
2. Ensure PNG files are valid and not corrupted
3. Run `dotnet restore` to ensure tools are installed
4. Check build output for specific error messages

## Platform Considerations

### Cross-Platform Fonts

Different platforms have different fonts available:

- **Linux**: DejaVu Sans, Liberation Sans, FreeSans
- **Windows**: Arial, Segoe UI, Tahoma
- **macOS**: Arial, Helvetica, SF Pro

Use fonts available on all target platforms or include custom TTF files.

### File Paths

Always use forward slashes `/` in Content.Load() paths for cross-platform compatibility:

```csharp
// Good
Content.Load<Texture2D>("Textures/Characters/player");

// Bad (Windows-only)
Content.Load<Texture2D>("Textures\\Characters\\player");
```

## Next Steps

Now that assets are integrated:

1. **Implement Animation System** - Create an animation controller using the loaded sprite sheets
2. **Add More Assets** - Continue adding NPCs, UI elements, effects
3. **Create Texture Atlas** - Combine small sprites for better performance
4. **Add Sound Effects** - Integrate audio assets
5. **Implement Save System** - Save and load asset states

## References

- [MonoGame Documentation](https://docs.monogame.net/)
- [Content Pipeline Guide](https://docs.monogame.net/articles/content_pipeline/index.html)
- [Sprite Batching](https://docs.monogame.net/articles/getting_started/5_adding_basic_code.html)

---

*Last Updated: December 2024*
*Project: MoonBrook Ridge*
