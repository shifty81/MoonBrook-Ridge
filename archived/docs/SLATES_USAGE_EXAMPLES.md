# Slates Tileset - Example Usage

This document provides code examples for using the Slates tileset in MoonBrook Ridge.

## Basic Usage

### 1. Loading the Tileset

In your game state (e.g., `GameplayState.cs`):

```csharp
using MoonBrookRidge.World.Tiles;

public class GameplayState : GameState
{
    private Texture2D _slatesTileset;
    private SlatesTilesetHelper _slatesHelper;

    public override void LoadContent()
    {
        base.LoadContent();
        
        // Load the Slates tileset
        _slatesTileset = Game.Content.Load<Texture2D>("Textures/Tiles/Slates_32x32_v2");
        
        // Create helper instance
        _slatesHelper = new SlatesTilesetHelper(_slatesTileset);
        
        Console.WriteLine($"Slates tileset loaded: {_slatesHelper.TotalTiles} tiles available");
    }
}
```

### 2. Drawing a Single Tile

Draw a tile at a specific position:

```csharp
protected override void Draw(GameTime gameTime)
{
    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    
    // Draw tile ID 0 at position (100, 100) at full 32x32 size
    _slatesHelper.DrawTile(_spriteBatch, 0, new Vector2(100, 100), scale: 1.0f);
    
    // Draw tile ID 5 at position (150, 100) scaled down to 16x16
    _slatesHelper.DrawTile(_spriteBatch, 5, new Vector2(150, 100), scale: 0.5f);
    
    _spriteBatch.End();
}
```

### 3. Drawing to a Grid

Draw tiles to fit the existing 16x16 game grid:

```csharp
protected override void Draw(GameTime gameTime)
{
    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    
    // Draw a 5x5 grid of tiles starting at tile ID 0
    for (int y = 0; y < 5; y++)
    {
        for (int x = 0; x < 5; x++)
        {
            int tileId = x + (y * 56); // Slates has 56 columns
            Rectangle destRect = new Rectangle(
                x * 16,  // Game uses 16x16 tiles
                y * 16,
                16,
                16
            );
            
            _slatesHelper.DrawTile(_spriteBatch, tileId, destRect);
        }
    }
    
    _spriteBatch.End();
}
```

## Integration with WorldMap

### Option A: Add Slates Tiles to Existing Tile System

Extend the `TileType` enum and load specific tiles:

```csharp
// In Tile.cs
public enum TileType
{
    // Existing types...
    
    // New Slates tiles
    SlatesGrass,
    SlatesStonePath,
    SlatesWater,
    SlatesWall,
    SlatesDoor
}
```

In `WorldMap.cs`:

```csharp
public class WorldMap
{
    private SlatesTilesetHelper _slatesHelper;
    private Dictionary<TileType, int> _slateTileIds;
    
    public void LoadContent(ContentManager content)
    {
        // Load Slates tileset
        Texture2D slatesTileset = content.Load<Texture2D>("Textures/Tiles/Slates_32x32_v2");
        _slatesHelper = new SlatesTilesetHelper(slatesTileset);
        
        // Map TileType to Slates tile IDs
        _slateTileIds = new Dictionary<TileType, int>
        {
            { TileType.SlatesGrass, 0 },        // Example IDs
            { TileType.SlatesStonePath, 100 },
            { TileType.SlatesWater, 200 },
            { TileType.SlatesWall, 300 },
            { TileType.SlatesDoor, 400 }
        };
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Tile tile = _tiles[x, y];
                
                // Check if this is a Slates tile
                if (_slateTileIds.ContainsKey(tile.Type))
                {
                    int tileId = _slateTileIds[tile.Type];
                    Rectangle destRect = new Rectangle(x * 16, y * 16, 16, 16);
                    _slatesHelper.DrawTile(spriteBatch, tileId, destRect);
                }
                else
                {
                    // Draw using existing tile textures
                    // ... existing code ...
                }
            }
        }
    }
}
```

### Option B: Extract Tiles to 16x16

Convert Slates tiles to 16x16 textures that work with the existing system:

```csharp
public class WorldMap
{
    public void LoadContent(ContentManager content, GraphicsDevice graphicsDevice)
    {
        // Load Slates tileset
        Texture2D slatesTileset = content.Load<Texture2D>("Textures/Tiles/Slates_32x32_v2");
        SlatesTilesetHelper slatesHelper = new SlatesTilesetHelper(slatesTileset);
        
        // Extract specific tiles and scale to 16x16
        _tileTextures[TileType.SlatesGrass] = slatesHelper.ExtractTile(graphicsDevice, 0, 16);
        _tileTextures[TileType.SlatesStonePath] = slatesHelper.ExtractTile(graphicsDevice, 100, 16);
        _tileTextures[TileType.SlatesWater] = slatesHelper.ExtractTile(graphicsDevice, 200, 16);
        
        // Now these tiles can be used just like any other 16x16 tile
    }
}
```

## Practical Examples

### Example 1: Create a Stone Path

```csharp
// In GameplayState or similar
public void CreateStonePath()
{
    // Assuming tile ID 250 is a nice stone path tile
    for (int x = 10; x < 20; x++)
    {
        _worldMap.GetTile(x, 15).Type = TileType.SlatesStonePath;
    }
}
```

### Example 2: Build a House Wall

```csharp
// Draw a wall using Slates wall tiles
public void DrawHouseWall(SpriteBatch spriteBatch, int startX, int startY, int width, int height)
{
    int wallTileId = 500; // Example wall tile ID from Slates
    
    for (int x = 0; x < width; x++)
    {
        for (int y = 0; y < height; y++)
        {
            Rectangle destRect = new Rectangle(
                (startX + x) * 16,
                (startY + y) * 16,
                16,
                16
            );
            _slatesHelper.DrawTile(spriteBatch, wallTileId, destRect);
        }
    }
}
```

### Example 3: Animated Water Tiles

```csharp
public class AnimatedSlatesTile
{
    private SlatesTilesetHelper _helper;
    private int[] _animationFrames;
    private int _currentFrame;
    private float _frameTime;
    private float _frameDuration = 0.2f; // 200ms per frame
    
    public AnimatedSlatesTile(SlatesTilesetHelper helper, int[] tileIds)
    {
        _helper = helper;
        _animationFrames = tileIds;
        _currentFrame = 0;
    }
    
    public void Update(GameTime gameTime)
    {
        _frameTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_frameTime >= _frameDuration)
        {
            _frameTime = 0;
            _currentFrame = (_currentFrame + 1) % _animationFrames.Length;
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, Rectangle destRect)
    {
        int tileId = _animationFrames[_currentFrame];
        _helper.DrawTile(spriteBatch, tileId, destRect);
    }
}

// Usage:
// Water tiles might be IDs 200, 201, 202, 203
var waterAnimation = new AnimatedSlatesTile(_slatesHelper, new[] { 200, 201, 202, 203 });
```

## Debugging and Tile Identification

### Display Tile Information

```csharp
// Show information about a tile when hovering
public void ShowTileInfo(int tileId)
{
    string info = _slatesHelper.GetTileInfo(tileId);
    Console.WriteLine(info);
    
    // Or display on screen
    _spriteBatch.DrawString(_font, info, new Vector2(10, 10), Color.White);
}
```

### Visual Tile Browser

```csharp
// Create a simple tile browser to see all tiles
public void DrawTileBrowser(SpriteBatch spriteBatch)
{
    int tilesPerRow = 20;
    int tileSize = 32;
    int startTileId = 0; // You can change this to browse different sections
    
    for (int i = 0; i < 200; i++) // Show 200 tiles at a time
    {
        int tileId = startTileId + i;
        if (tileId >= _slatesHelper.TotalTiles) break;
        
        int x = (i % tilesPerRow) * tileSize;
        int y = (i / tilesPerRow) * tileSize;
        
        _slatesHelper.DrawTile(spriteBatch, tileId, new Vector2(x, y), 1.0f);
        
        // Draw tile ID
        _spriteBatch.DrawString(_smallFont, tileId.ToString(), 
            new Vector2(x + 2, y + 2), Color.Yellow);
    }
}
```

## Performance Considerations

### Batch Drawing

Always use a single SpriteBatch call for all Slates tiles:

```csharp
// Good: Single batch
_spriteBatch.Begin(samplerState: SamplerState.PointClamp);
for (int i = 0; i < 100; i++)
{
    _slatesHelper.DrawTile(_spriteBatch, i, new Vector2(i * 32, 0), 1.0f);
}
_spriteBatch.End();

// Bad: Multiple batches
for (int i = 0; i < 100; i++)
{
    _spriteBatch.Begin();
    _slatesHelper.DrawTile(_spriteBatch, i, new Vector2(i * 32, 0), 1.0f);
    _spriteBatch.End();
}
```

### Tile Extraction Caching

If extracting many tiles, cache the results:

```csharp
private Dictionary<int, Texture2D> _extractedTiles = new Dictionary<int, Texture2D>();

public Texture2D GetOrExtractTile(int tileId, int size = 16)
{
    if (!_extractedTiles.ContainsKey(tileId))
    {
        _extractedTiles[tileId] = _slatesHelper.ExtractTile(_graphicsDevice, tileId, size);
    }
    return _extractedTiles[tileId];
}
```

## Attribution

When using the Slates tileset in your game, include this attribution:

```csharp
// In your credits screen or About section:
string slatesCredit = "Slates Tileset by Ivan Voirol (CC-BY 4.0)\n" +
                      "Source: OpenGameArt.org";
```

## Additional Resources

- **SlatesTilesetHelper.cs**: Helper class for working with the tileset
- **SLATES_INTEGRATION_GUIDE.md**: Comprehensive integration guide
- **sprites/tilesets/Slates/README.md**: Tileset documentation
- **Tileset File**: Content/Textures/Tiles/Slates_32x32_v2.png

---

For more detailed integration instructions, see: `SLATES_INTEGRATION_GUIDE.md`
