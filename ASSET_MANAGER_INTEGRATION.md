# Asset Manager Integration Example

## Overview

This document shows how to integrate the new `AssetManager` class into MoonBrook Ridge to utilize the 11,000+ Sunnyside World assets.

## Step 1: Add AssetManager to Game Class

Update `Game1.cs` to include the AssetManager:

```csharp
public class Game1 : Game
{
    // Add this field
    private AssetManager _assetManager;
    
    protected override void LoadContent()
    {
        // Initialize AssetManager
        _assetManager = new AssetManager(Content, GraphicsDevice);
        
        // Optional: Preload essential categories
        _assetManager.PreloadCategory("Tiles");
        _assetManager.PreloadCategory("Characters");
        
        // Pass to game states
        _stateManager.Initialize(_assetManager);
    }
    
    protected override void UnloadContent()
    {
        // Clean up
        _assetManager.ClearCache();
    }
}
```

## Step 2: Use AssetManager in Game States

Update `GameplayState.cs`:

```csharp
public class GameplayState : GameState
{
    private AssetManager _assetManager;
    
    public override void Initialize(AssetManager assetManager)
    {
        _assetManager = assetManager;
        
        // Load only what's needed for gameplay
        LoadGameplayAssets();
    }
    
    private void LoadGameplayAssets()
    {
        // These will be cached automatically
        var grassTile = _assetManager.GetTexture("Tiles/grass_01");
        var dirtTile = _assetManager.GetTexture("Tiles/dirt_01");
        
        // Character animations
        var walkAnim = _assetManager.GetCharacterAnimation("WALKING", "base");
        
        // Buildings
        var barn = _assetManager.GetBuilding("House1", "Black");
        
        Console.WriteLine($"Assets loaded: {_assetManager.GetStats()}");
    }
}
```

## Step 3: Use in WorldMap

Update `WorldMap.cs` to use AssetManager for tiles:

```csharp
public class WorldMap
{
    private AssetManager _assetManager;
    
    public void Initialize(AssetManager assetManager)
    {
        _assetManager = assetManager;
    }
    
    public void DrawTile(SpriteBatch spriteBatch, int x, int y, TileType tileType)
    {
        Texture2D tileTexture = GetTileTexture(tileType);
        Rectangle destRect = new Rectangle(x * 16, y * 16, 16, 16);
        spriteBatch.Draw(tileTexture, destRect, Color.White);
    }
    
    private Texture2D GetTileTexture(TileType tileType)
    {
        // Use AssetManager instead of loading directly
        return tileType switch
        {
            TileType.Grass => _assetManager.GetTexture("Tiles/grass_01"),
            TileType.Dirt => _assetManager.GetTexture("Tiles/dirt_01"),
            TileType.Water => _assetManager.GetTexture("Tiles/water_01"),
            TileType.Sand => _assetManager.GetTexture("Tiles/sand_01"),
            _ => _assetManager.GetTexture("Tiles/grass_01")
        };
    }
}
```

## Step 4: Use with Crops

Update crop rendering to use AssetManager:

```csharp
public class Crop
{
    private AssetManager _assetManager;
    private string _cropType;
    private int _growthStage;
    
    public Crop(AssetManager assetManager, string cropType)
    {
        _assetManager = assetManager;
        _cropType = cropType;
        _growthStage = 0;
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        // Get the appropriate growth stage texture
        var texture = _assetManager.GetCropTexture(_cropType, _growthStage);
        spriteBatch.Draw(texture, position, Color.White);
    }
}
```

## Step 5: Use with Buildings

Update building system:

```csharp
public class Building
{
    private Texture2D _sprite;
    
    public Building(AssetManager assetManager, string buildingType, string color)
    {
        // Load building sprite
        _sprite = assetManager.GetBuilding(buildingType, color);
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.Draw(_sprite, position, Color.White);
    }
}
```

## Step 6: Memory Management

Manage memory by loading/unloading categories:

```csharp
public class GameplayState : GameState
{
    public override void OnEnter()
    {
        // Entering overworld - load surface assets
        _assetManager.PreloadCategory("Tiles");
        _assetManager.PreloadCategory("Buildings");
        _assetManager.PreloadCategory("Decorations");
        
        // Unload underground assets
        _assetManager.UnloadCategory("Enemies");
    }
}

public class CaveState : GameState
{
    public override void OnEnter()
    {
        // Entering cave - load cave assets
        _assetManager.PreloadCategory("Enemies");
        _assetManager.PreloadCategory("Effects");
        
        // Unload surface assets we don't need
        _assetManager.UnloadCategory("Buildings");
        _assetManager.UnloadCategory("Decorations");
    }
}
```

## Step 7: Character System Integration

For modular character system:

```csharp
public class Character
{
    private AssetManager _assetManager;
    private Texture2D _baseSprite;
    private Texture2D _hairSprite;
    private Texture2D _toolSprite;
    
    private string _currentAction = "WALKING";
    private string _hairStyle = "longhair";
    
    public void LoadAnimations(AssetManager assetManager)
    {
        _assetManager = assetManager;
        UpdateSprites();
    }
    
    private void UpdateSprites()
    {
        _baseSprite = _assetManager.GetCharacterAnimation(_currentAction, "base");
        _hairSprite = _assetManager.GetCharacterAnimation(_currentAction, _hairStyle);
        _toolSprite = _assetManager.GetCharacterAnimation(_currentAction, "tools");
    }
    
    public void SetAction(string action)
    {
        if (_currentAction != action)
        {
            _currentAction = action;
            UpdateSprites();
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position, int frame)
    {
        // Calculate source rectangle for current frame
        int frameWidth = _baseSprite.Width / 8; // Assuming 8 frames
        Rectangle sourceRect = new Rectangle(frame * frameWidth, 0, frameWidth, _baseSprite.Height);
        
        // Draw layered
        spriteBatch.Draw(_baseSprite, position, sourceRect, Color.White);
        spriteBatch.Draw(_hairSprite, position, sourceRect, Color.Brown); // Hair color
        spriteBatch.Draw(_toolSprite, position, sourceRect, Color.White);
    }
}
```

## Step 8: Resource System

For harvestable resources with highlights:

```csharp
public class HarvestableRock
{
    private Texture2D _normalSprite;
    private Texture2D _highlightSprite;
    private bool _isHovered;
    
    public HarvestableRock(AssetManager assetManager, string rockType)
    {
        _normalSprite = assetManager.GetResource($"Gold/Gold Stones/Gold Stone {rockType}", false);
        _highlightSprite = assetManager.GetResource($"Gold/Gold Stones/Gold Stone {rockType}", true);
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        var sprite = _isHovered ? _highlightSprite : _normalSprite;
        spriteBatch.Draw(sprite, position, Color.White);
    }
}
```

## Step 9: Decoration System

Random decorations placement:

```csharp
public class DecorationManager
{
    private AssetManager _assetManager;
    private List<Decoration> _decorations = new();
    
    public void PlaceRandomDecorations(int count)
    {
        Random rand = new Random();
        
        for (int i = 0; i < count; i++)
        {
            // Random rock decoration
            int rockType = rand.Next(1, 5);
            var rockSprite = _assetManager.GetDecoration("Rocks/Rock", rockType);
            
            Vector2 position = new Vector2(
                rand.Next(0, worldWidth * 16),
                rand.Next(0, worldHeight * 16)
            );
            
            _decorations.Add(new Decoration(rockSprite, position));
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        foreach (var decoration in _decorations)
        {
            decoration.Draw(spriteBatch);
        }
    }
}
```

## Step 10: Monitoring Asset Usage

Add debug display:

```csharp
public class DebugOverlay
{
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, AssetManager assetManager)
    {
        var stats = assetManager.GetStats();
        string text = $"Assets: {stats}";
        
        spriteBatch.DrawString(font, text, new Vector2(10, 10), Color.White);
    }
}
```

## Complete Integration Example

Here's a complete minimal example:

```csharp
public class Game1 : Game
{
    private AssetManager _assetManager;
    private WorldMap _worldMap;
    private Character _player;
    
    protected override void LoadContent()
    {
        // Initialize asset manager
        _assetManager = new AssetManager(Content, GraphicsDevice);
        
        // Preload essential assets
        Console.WriteLine("Loading assets...");
        _assetManager.PreloadCategory("Tiles");
        
        // Initialize systems with asset manager
        _worldMap = new WorldMap();
        _worldMap.Initialize(_assetManager);
        
        _player = new Character();
        _player.LoadAnimations(_assetManager);
        
        Console.WriteLine($"Assets ready: {_assetManager.GetStats()}");
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        _spriteBatch.Begin(
            samplerState: SamplerState.PointClamp,
            transformMatrix: _camera.GetTransform()
        );
        
        _worldMap.Draw(_spriteBatch);
        _player.Draw(_spriteBatch, playerPosition, currentFrame);
        
        _spriteBatch.End();
    }
    
    protected override void UnloadContent()
    {
        _assetManager.ClearCache();
    }
}
```

## Performance Tips

### 1. Lazy Loading (Default)

AssetManager loads on-demand by default. Assets are only loaded when first requested.

### 2. Preloading Critical Assets

Preload assets you know you'll need immediately:

```csharp
// Load all tiles at game start
_assetManager.PreloadCategory("Tiles");

// Load character animations when entering game
_assetManager.PreloadCategory("Characters");
```

### 3. Unloading Unused Assets

Free memory by unloading categories you don't need:

```csharp
// When leaving surface world
_assetManager.UnloadCategory("Buildings");
_assetManager.UnloadCategory("Decorations");

// When leaving caves
_assetManager.UnloadCategory("Enemies");
```

### 4. Check Cache Stats

Monitor memory usage:

```csharp
var stats = _assetManager.GetStats();
Console.WriteLine($"Currently caching {stats.TotalCachedTextures} textures");

// If too many, selectively unload
if (stats.TotalCachedTextures > 1000)
{
    _assetManager.UnloadCategory("Effects");
}
```

## Next Steps

1. **Execute Asset Organization**:
   ```bash
   python3 tools/organize_sunnyside_assets.py --execute
   ```

2. **Integrate AssetManager**: Add to Game1.cs as shown above

3. **Update Existing Systems**: Modify WorldMap, Character, etc. to use AssetManager

4. **Test Loading**: Verify assets load correctly

5. **Optimize**: Profile memory usage and adjust preloading strategy

## Troubleshooting

**Issue**: "Texture not found"
- Check asset was organized by running the Python script
- Verify path matches organized structure
- Check Content/Textures/ for the file

**Issue**: High memory usage
- Use `UnloadCategory()` for unused assets
- Don't preload all categories at once
- Check for memory leaks with `GetStats()`

**Issue**: Slow loading
- Preload assets during loading screens
- Use async loading for large batches
- Reduce number of different textures used simultaneously

---

**See Also**:
- `SUNNYSIDE_ASSET_INTEGRATION_GUIDE.md` - Complete asset guide
- `Core/Systems/AssetManager.cs` - AssetManager source code
- `tools/organize_sunnyside_assets.py` - Asset organization script
