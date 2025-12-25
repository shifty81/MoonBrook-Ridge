# Slates Tileset Integration Guide - MoonBrook Ridge

## Overview

This guide explains how to integrate the Slates 32x32px tileset by Ivan Voirol into MoonBrook Ridge. The tileset has been downloaded and added to the project, but requires additional steps for full integration.

## Current Status

✅ **Completed Steps:**
- Downloaded Slates v.2 tileset (1792x736px, 1,288 tiles)
- Created documentation in `sprites/tilesets/Slates/README.md`
- Copied tileset to `MoonBrookRidge/Content/Textures/Tiles/Slates_32x32_v2.png`
- Added to Content Pipeline (`Content.mgcb`)

⏳ **Pending Steps:**
- Load tileset in game code
- Extract and integrate specific tiles
- Update tile rendering system (if using 32x32 tiles directly)
- Create tile mapping configuration

## Integration Options

You have three main options for integrating this tileset:

### Option 1: Extract and Scale Individual Tiles (Recommended for Current Setup)

Since MoonBrook Ridge uses 16x16 tiles, extract individual tiles from the Slates tileset and scale them down.

**Advantages:**
- Compatible with existing tile system
- No code changes required
- Can cherry-pick best tiles

**Steps:**
1. Use an image editor (GIMP, Aseprite, Photoshop) to extract tiles
2. Scale from 32x32 to 16x16 pixels
3. Save as individual PNG files
4. Add to Content.mgcb
5. Load in `WorldMap.cs`

**Example Tiles to Extract:**
- Grass variations
- Path/dirt tiles
- Stone walls
- Water tiles
- Interior floors

### Option 2: Use as a Tilemap (32x32 Grid)

Load the entire tileset as a texture atlas and reference tiles by their grid coordinates.

**Advantages:**
- Access to all 1,288 tiles
- Efficient memory usage
- Easy to swap tiles

**Implementation:**

```csharp
// In WorldMap.cs LoadContent
private Texture2D _slatesTileset;
private const int SLATE_TILE_SIZE = 32;
private const int SLATE_COLUMNS = 56;

public void LoadContent(ContentManager content)
{
    _slatesTileset = content.Load<Texture2D>("Textures/Tiles/Slates_32x32_v2");
}

// Helper method to get a specific tile's source rectangle
private Rectangle GetSlateTileRect(int tileId)
{
    int x = (tileId % SLATE_COLUMNS) * SLATE_TILE_SIZE;
    int y = (tileId / SLATE_COLUMNS) * SLATE_TILE_SIZE;
    return new Rectangle(x, y, SLATE_TILE_SIZE, SLATE_TILE_SIZE);
}

// Drawing a tile (scaled to fit 16x16 game tiles)
public void DrawSlateTile(SpriteBatch spriteBatch, int tileId, Vector2 position)
{
    Rectangle sourceRect = GetSlateTileRect(tileId);
    Rectangle destRect = new Rectangle(
        (int)position.X * 16,  // Game uses 16x16
        (int)position.Y * 16,
        16,  // Scale down to 16x16
        16
    );
    spriteBatch.Draw(_slatesTileset, destRect, sourceRect, Color.White);
}
```

### Option 3: Update Game to Use 32x32 Tiles

Modify the game's tile system to support 32x32 tiles natively.

**Advantages:**
- Better visual quality (larger tiles)
- Native tileset support
- More detail possible

**Required Changes:**

1. **Update Tile Size Constant:**
```csharp
// In WorldMap.cs
private const int TILE_SIZE = 32;  // Changed from 16
```

2. **Update Camera Zoom:**
```csharp
// In GameplayState.cs or Camera2D.cs
_camera.Zoom = 1.5f;  // Adjust for larger tiles
```

3. **Update Collision System:**
```csharp
// Adjust collision detection for new tile size
```

4. **Update Player Movement:**
```csharp
// Movement grid may need adjustment
```

## Loading the Tileset in Code

### Basic Loading

Add to your `GameplayState.cs` or wherever tiles are loaded:

```csharp
// In LoadContent method
private Texture2D _slatesTileset;

public override void LoadContent()
{
    base.LoadContent();
    
    // Load the Slates tileset
    _slatesTileset = Game.Content.Load<Texture2D>("Textures/Tiles/Slates_32x32_v2");
    
    // Pass to WorldMap if needed
    _worldMap.LoadSlatesTileset(_slatesTileset);
}
```

### Extending WorldMap

Add support in `WorldMap.cs`:

```csharp
public class WorldMap
{
    private Texture2D _slatesTileset;
    private Dictionary<string, int> _slateTileMap;
    
    public void LoadSlatesTileset(Texture2D tileset)
    {
        _slatesTileset = tileset;
        InitializeSlateTileMapping();
    }
    
    private void InitializeSlateTileMapping()
    {
        // Map descriptive names to tile IDs
        _slateTileMap = new Dictionary<string, int>
        {
            // Grass tiles (example - actual IDs need to be determined)
            { "grass_01", 0 },
            { "grass_02", 1 },
            { "grass_03", 2 },
            
            // Path tiles
            { "path_stone", 56 },
            { "path_dirt", 57 },
            
            // Add more mappings...
        };
    }
}
```

## Tile Mapping Reference

To use specific tiles, you need to identify them in the tileset. The tileset is arranged as:

- **Grid**: 56 columns × 23 rows
- **Tile ID Formula**: `tileId = (row * 56) + column`
- **Zero-indexed**: Top-left tile is ID 0

### Sample Tile Locations

*Note: These are estimates - visual inspection needed for exact IDs*

| Tile Type | Approximate Row | Approximate Column Range | Tile ID Range |
|-----------|----------------|--------------------------|---------------|
| Grass     | 0-2            | 0-55                     | 0-167         |
| Paths     | 3-5            | 0-55                     | 168-335       |
| Stone     | 6-8            | 0-55                     | 336-503       |
| Water     | 9-11           | 0-55                     | 504-671       |
| Walls     | 12-15          | 0-55                     | 672-895       |
| Objects   | 16-22          | 0-55                     | 896-1287      |

**Important**: Open `Slates_32x32_v2.png` in an image viewer with grid overlay to identify exact tile positions.

## Practical Example: Adding Stone Path Tiles

Here's a complete example of adding stone path tiles from the Slates tileset:

### Step 1: Identify Tiles
Open the tileset and note the tile IDs for stone paths (e.g., tiles 200-210)

### Step 2: Add to Tile Enum
```csharp
// In Tile.cs
public enum TileType
{
    // Existing types...
    SlatesStonePath01,
    SlatesStonePath02,
    SlatesStonePath03,
}
```

### Step 3: Update WorldMap Loading
```csharp
// In WorldMap.cs LoadContent
_tileTextures[TileType.SlatesStonePath01] = ExtractSlateTile(200);
_tileTextures[TileType.SlatesStonePath02] = ExtractSlateTile(201);
_tileTextures[TileType.SlatesStonePath03] = ExtractSlateTile(202);
```

### Step 4: Helper Method to Extract Tile
```csharp
private Texture2D ExtractSlateTile(int tileId)
{
    // Calculate source position
    int x = (tileId % 56) * 32;
    int y = (tileId / 56) * 32;
    
    // Create render target for extraction
    RenderTarget2D renderTarget = new RenderTarget2D(
        _graphicsDevice,
        32, 32  // Or 16x16 if scaling down
    );
    
    GraphicsDevice.SetRenderTarget(renderTarget);
    GraphicsDevice.Clear(Color.Transparent);
    
    SpriteBatch batch = new SpriteBatch(GraphicsDevice);
    batch.Begin();
    batch.Draw(
        _slatesTileset,
        new Rectangle(0, 0, 32, 32),  // Destination
        new Rectangle(x, y, 32, 32),  // Source
        Color.White
    );
    batch.End();
    
    GraphicsDevice.SetRenderTarget(null);
    
    return renderTarget;
}
```

## Testing the Integration

### Step 1: Build the Project
```bash
cd MoonBrookRidge
dotnet build
```

Check for any errors related to content loading.

### Step 2: Verify Tileset Loads
Add debug code to verify the tileset is accessible:

```csharp
// In LoadContent
_slatesTileset = Game.Content.Load<Texture2D>("Textures/Tiles/Slates_32x32_v2");
Console.WriteLine($"Slates tileset loaded: {_slatesTileset.Width}x{_slatesTileset.Height}");
```

### Step 3: Test Rendering
Try rendering a test tile from the tileset:

```csharp
// In Draw method
spriteBatch.Begin(samplerState: SamplerState.PointClamp);

// Draw top-left tile of Slates as a test
spriteBatch.Draw(
    _slatesTileset,
    new Rectangle(100, 100, 32, 32),  // Screen position
    new Rectangle(0, 0, 32, 32),      // First tile from tileset
    Color.White
);

spriteBatch.End();
```

## Using with Tiled Map Editor (Optional)

For advanced map design, you can use the Tiled Map Editor:

1. **Import Tileset to Tiled**:
   - Open Tiled Map Editor
   - New Tileset → Load `Slates_32x32_v2.png`
   - Set tile size to 32x32

2. **Create Map**:
   - Design your map visually
   - Export as TMX or JSON

3. **Load in MonoGame**:
   - Use a TMX loader library (e.g., MonoGame.Extended)
   - Or parse the TMX/JSON manually

## Attribution Requirements

**Important**: When using the Slates tileset, you must provide attribution:

```
Tileset: Slates v.2 [32x32px orthogonal tileset]
Artist: Ivan Voirol
Source: OpenGameArt.org
License: CC-BY 4.0
```

Add this to your game's credits screen or README file.

## Performance Considerations

### Memory Usage
- Full tileset is ~450KB (compressed PNG)
- Loaded in memory as texture: ~5MB (RGBA)
- Extracting individual tiles creates additional textures

### Optimization Tips
1. **Use Texture Atlas**: Keep as single tileset, render by source rectangles
2. **Lazy Loading**: Only load tiles actually used in game
3. **Combine with Existing Tilesets**: Merge best tiles into existing ground_tileset.png
4. **Mipmap Disabled**: Already disabled in Content.mgcb (correct for pixel art)

## Troubleshooting

### Tileset Not Found Error
```
ContentLoadException: Could not load Textures/Tiles/Slates_32x32_v2
```

**Solutions:**
1. Verify file exists: `MoonBrookRidge/Content/Textures/Tiles/Slates_32x32_v2.png`
2. Check Content.mgcb entry is correct
3. Rebuild project: `dotnet build`
4. Check for typos in Content.Load() call

### Blurry Tiles
If tiles appear blurry or smoothed:

```csharp
// Ensure using PointClamp sampler
spriteBatch.Begin(samplerState: SamplerState.PointClamp);
```

### Tiles Too Large/Small
Adjust destination rectangle size when drawing:

```csharp
// Scale 32x32 tiles to fit 16x16 grid
Rectangle destRect = new Rectangle(x * 16, y * 16, 16, 16);
```

## Next Steps

1. **Identify Useful Tiles**: Browse the tileset and note which tiles would enhance the game
2. **Extract or Map**: Choose integration method and implement
3. **Update Existing Areas**: Replace or supplement existing tiles
4. **Create New Content**: Design new areas using Slates tiles
5. **Test Thoroughly**: Ensure rendering performance is acceptable

## Resources

- **Tileset File**: `sprites/tilesets/Slates/Slates_32x32_v2.png`
- **README**: `sprites/tilesets/Slates/README.md`
- **Tiled Map Editor**: https://www.mapeditor.org/
- **OpenGameArt**: https://opengameart.org/content/slates-32x32px-orthogonal-tileset-by-ivan-voirol

## Example: Quick Start Integration

Here's a minimal example to get started immediately:

```csharp
// 1. In GameplayState.cs LoadContent:
private Texture2D _slatesTileset;

public override void LoadContent()
{
    base.LoadContent();
    _slatesTileset = Game.Content.Load<Texture2D>("Textures/Tiles/Slates_32x32_v2");
}

// 2. In GameplayState.cs Draw:
protected override void Draw(GameTime gameTime)
{
    // Draw a sample tile from the Slates tileset
    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    
    // Draw tile ID 0 (top-left tile) at screen position (200, 200)
    _spriteBatch.Draw(
        _slatesTileset,
        new Rectangle(200, 200, 32, 32),
        new Rectangle(0, 0, 32, 32),
        Color.White
    );
    
    _spriteBatch.End();
}
```

This will render one tile from the tileset as a test.

---

**Document Created**: December 2024  
**Project**: MoonBrook Ridge  
**Tileset**: Slates v.2 by Ivan Voirol  
**License**: CC-BY 4.0
