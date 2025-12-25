# Slates Tileset - Quick Reference Card

## Load and Use in 3 Steps

### Step 1: Load
```csharp
var tileset = Content.Load<Texture2D>("Textures/Tiles/Slates_32x32_v2");
var helper = new SlatesTilesetHelper(tileset);
```

### Step 2: Draw
```csharp
spriteBatch.Begin(samplerState: SamplerState.PointClamp);
helper.DrawTile(spriteBatch, tileId, position, scale);
spriteBatch.End();
```

### Step 3: Integrate
```csharp
// Extract to 16x16 for game
var texture16 = helper.ExtractTile(graphicsDevice, tileId, 16);
```

## Tileset Specs
- **Size**: 1792x736 pixels
- **Tiles**: 56x23 grid = 1,288 tiles
- **Tile Size**: 32x32 pixels
- **Format**: PNG RGBA

## Tile ID Formula
```
tileId = (row * 56) + column
```

## Helper Methods
| Method | Description |
|--------|-------------|
| `GetTileSourceRectangle(id)` | Get source rect for tile |
| `DrawTile(batch, id, pos, scale)` | Draw tile at position |
| `DrawTile(batch, id, rect)` | Draw tile to rectangle |
| `ExtractTile(device, id, size)` | Extract tile as texture |
| `GetTileInfo(id)` | Get debug information |

## Properties
| Property | Value |
|----------|-------|
| `TileSize` | 32 |
| `Columns` | 56 |
| `Rows` | 23 |
| `TotalTiles` | 1288 |
| `Texture` | Texture2D |

## Integration Patterns

### Pattern 1: Direct Drawing
```csharp
_slatesHelper.DrawTile(_spriteBatch, 0, new Vector2(100, 100), 1.0f);
```

### Pattern 2: Scale to 16x16
```csharp
Rectangle dest = new Rectangle(x * 16, y * 16, 16, 16);
_slatesHelper.DrawTile(_spriteBatch, tileId, dest);
```

### Pattern 3: Extract and Cache
```csharp
Texture2D tile = _slatesHelper.ExtractTile(_graphicsDevice, 0, 16);
_tileTextures[TileType.SlatesGrass] = tile;
```

## Sample Tile IDs (Approximate)
| Type | ID Range | Row Range |
|------|----------|-----------|
| Grass | 0-167 | 0-2 |
| Paths | 168-335 | 3-5 |
| Stone | 336-503 | 6-8 |
| Water | 504-671 | 9-11 |
| Walls | 672-895 | 12-15 |
| Objects | 896-1287 | 16-22 |

*Note: Visual inspection needed for exact IDs*

## Files
- **Asset**: `MoonBrookRidge/Content/Textures/Tiles/Slates_32x32_v2.png`
- **Helper**: `MoonBrookRidge/World/Tiles/SlatesTilesetHelper.cs`
- **Guide**: `SLATES_INTEGRATION_GUIDE.md`
- **Examples**: `SLATES_USAGE_EXAMPLES.md`

## License
**CC-BY 4.0** - Attribution Required

**Credit:**
```
Tileset: Slates v.2 by Ivan Voirol
Source: OpenGameArt.org
```

## Common Tasks

### Browse Tiles Visually
```csharp
for (int i = 0; i < 100; i++)
{
    int x = (i % 10) * 32;
    int y = (i / 10) * 32;
    _slatesHelper.DrawTile(_spriteBatch, i, new Vector2(x, y), 1.0f);
}
```

### Create Animated Tiles
```csharp
int[] waterFrames = { 200, 201, 202, 203 };
int currentFrame = (int)(gameTime.TotalGameTime.TotalSeconds * 5) % waterFrames.Length;
_slatesHelper.DrawTile(_spriteBatch, waterFrames[currentFrame], position, 1.0f);
```

### Build a Wall
```csharp
int wallTileId = 500;
for (int i = 0; i < 10; i++)
{
    _slatesHelper.DrawTile(_spriteBatch, wallTileId, 
        new Rectangle(i * 16, 100, 16, 16));
}
```

## Troubleshooting

| Issue | Solution |
|-------|----------|
| Tileset not found | Verify path: `Textures/Tiles/Slates_32x32_v2` |
| Blurry tiles | Use `SamplerState.PointClamp` |
| Tile ID error | Check range: 0-1287 |
| Build fails | Run `dotnet build` in MoonBrookRidge/ |

## Performance Tips
✅ Use single SpriteBatch.Begin/End  
✅ Cache extracted tiles  
✅ Use texture atlas (don't extract all tiles)  
✅ PointClamp sampler for pixel art  

## Documentation
📖 Full Guide: `SLATES_INTEGRATION_GUIDE.md`  
💡 Examples: `SLATES_USAGE_EXAMPLES.md`  
📋 Summary: `SLATES_IMPLEMENTATION_SUMMARY.md`  
📚 Tileset Info: `sprites/tilesets/Slates/README.md`

---
Quick Ref v1.0 | December 2024
