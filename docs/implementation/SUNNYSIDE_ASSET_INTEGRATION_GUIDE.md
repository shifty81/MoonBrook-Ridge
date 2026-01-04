# Sunnyside World Asset Integration Guide

## Overview

MoonBrook Ridge has access to the comprehensive **Sunnyside World** asset pack, containing over **11,000 high-quality pixel art assets** perfect for a farming simulation game. This guide explains how to organize, import, and use these assets in the game.

## Asset Inventory

### Total Assets Available: 11,122 PNG files

| Category | Count | Description |
|----------|-------|-------------|
| **Tiles** | 9,304 | Ground tiles, terrain, tilesets for world building |
| **Characters** | 261 | Modular character parts with animations (walking, mining, fishing, etc.) |
| **Objects** | 253 | Interactive items, furniture, tools, containers |
| **Crops** | 78 | Crop growth stages for various plants (wheat, carrots, etc.) |
| **Buildings** | 42 | Houses, castles, barns, towers in multiple color variants |
| **Resources** | 31 | Harvestable resources (rocks, trees, gold, animals) |
| **Decorations** | 21 | Environmental decorations (bushes, rocks, clouds) |
| **Enemies** | 20 | Goblin sprites and animations |
| **Effects** | 14 | Particle effects, chimney smoke, animations |
| **Uncategorized** | 1,098 | Additional assets to be sorted |

## Asset Organization System

### Source Location
All assets are currently in: `/sprites/`

### Target Location
Organized assets will be copied to: `/MoonBrookRidge/Content/Textures/`

### Directory Structure

```
Content/Textures/
├── Buildings/          # Building sprites
│   ├── Black Buildings/
│   ├── Purple Buildings/
│   ├── Yellow Buildings/
│   └── ...
├── Characters/         # Character animation parts
│   ├── WALKING/
│   ├── MINING/
│   ├── FISHING/
│   ├── IDLE/
│   └── ...
├── Crops/              # Crop growth stages
│   ├── wheat/
│   ├── carrot/
│   ├── potato/
│   └── ...
├── Resources/          # Harvestable resources
│   ├── Gold/
│   ├── Tools/
│   ├── Meat/
│   └── ...
├── Decorations/        # Environmental objects
│   ├── Rocks/
│   ├── Bushes/
│   ├── Clouds/
│   └── ...
├── Effects/            # Particle effects
│   └── smoke/
├── Enemies/            # Enemy sprites
│   └── Goblins/
├── Objects/            # Interactive items
│   └── ...
└── Tiles/              # Ground tiles and tilesets
    └── ...
```

## Using the Asset Organizer

### Step 1: Dry Run (Preview)

First, run the organizer in dry-run mode to see what will be organized:

```bash
cd /home/runner/work/MoonBrook-Ridge/MoonBrook-Ridge
python3 tools/organize_sunnyside_assets.py
```

This will:
- Scan all 11,000+ PNG files in the sprites folder
- Categorize them by type
- Show statistics without copying files
- Generate a preview of the organization

### Step 2: Execute Organization

Once you're satisfied with the preview, run with `--execute` to actually copy files:

```bash
python3 tools/organize_sunnyside_assets.py --execute
```

This will:
- Copy all assets to `Content/Textures/` in organized folders
- Skip duplicate files
- Skip non-essential files (__MACOSX, .DS_Store, etc.)
- Generate an asset catalog JSON file

### Step 3: Review Asset Catalog

After execution, review the generated catalog:

```bash
cat MoonBrookRidge/Content/Textures/asset_catalog.json
```

This JSON file contains:
- Total asset count
- Assets per category
- Sample file listings
- Category descriptions

## Asset Categories in Detail

### 1. Tiles (9,304 assets)

**Usage**: Ground terrain, world building, biomes

**Available tilesets**:
- Sunnyside World main tileset (1280×1280 pixels, 80×80 tiles at 16×16)
- Color variant tilemaps (576×384 pixels each)
- Slates tileset (1792×736 pixels, 32×32 tiles)
- Individual tile sprites

**Code Example**:
```csharp
// Load tileset
Texture2D groundTileset = Content.Load<Texture2D>("Textures/Tiles/ground_tileset");

// Draw tile at grid position (x, y) with tile ID
int tileId = 42;
int tilesPerRow = 16;
Rectangle sourceRect = new Rectangle(
    (tileId % tilesPerRow) * 16,
    (tileId / tilesPerRow) * 16,
    16, 16
);
spriteBatch.Draw(groundTileset, destRect, sourceRect, Color.White);
```

### 2. Characters (261 assets)

**Usage**: Player and NPC sprites with modular customization

**Modular System**:
- **Base**: Body/skin tone (8 frames per animation)
- **Hair**: Multiple styles (longhair, shorthair, bowlhair, curlyhair, spikeyhair, mophair)
- **Tools**: Shows tools in hands

**Actions Available**:
- WALKING, RUNNING
- MINING, DIG, AXE
- FISHING (CASTING, WAITING, REELING, CAUGHT)
- WATERING
- IDLE
- ATTACK, HURT, DEATH
- CARRY, DOING, HAMMERING
- JUMP, ROLL, SWIMMING

**Code Example**:
```csharp
// Load character layers
Texture2D baseWalk = Content.Load<Texture2D>("Textures/Characters/WALKING/base_walk_strip8");
Texture2D hairWalk = Content.Load<Texture2D>("Textures/Characters/WALKING/longhair_walk_strip8");
Texture2D toolWalk = Content.Load<Texture2D>("Textures/Characters/WALKING/tools_walk_strip8");

// Draw layered (during render)
int frameWidth = baseWalk.Width / 8; // 8 frames
int currentFrame = 0;
Rectangle sourceRect = new Rectangle(currentFrame * frameWidth, 0, frameWidth, baseWalk.Height);

spriteBatch.Draw(baseWalk, position, sourceRect, Color.White);
spriteBatch.Draw(hairWalk, position, sourceRect, hairColor);
spriteBatch.Draw(toolWalk, position, sourceRect, Color.White);
```

### 3. Crops (78 assets)

**Usage**: Farming system with growth stages

**Available Crops**:
- Wheat, Kale, Parsnip, Beetroot
- Potato, Carrot, Sunflower
- Cabbage, Cauliflower
- Each with 5-6 growth stages (00 to 05)

**Code Example**:
```csharp
public class Crop
{
    private Texture2D[] _growthStages;
    private int _currentStage = 0;
    
    public void Load(ContentManager content, string cropType)
    {
        _growthStages = new Texture2D[6];
        for (int i = 0; i < 6; i++)
        {
            _growthStages[i] = content.Load<Texture2D>(
                $"Textures/Crops/{cropType}_{i:D2}"
            );
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position)
    {
        spriteBatch.Draw(_growthStages[_currentStage], position, Color.White);
    }
}
```

### 4. Buildings (42 assets)

**Usage**: Structures for villages and farms

**Available Buildings**:
- Castles, Houses (3 types), Towers
- Barracks, Monastery, Archery
- Color variants: Black, Purple, Yellow, Blue, Red, Green

**Typical Sizes**: Buildings are multi-tile (e.g., 6×8 tiles for a barn)

**Code Example**:
```csharp
public class Building
{
    public Texture2D Sprite { get; set; }
    public Vector2 GridPosition { get; set; }
    public int WidthInTiles { get; set; }
    public int HeightInTiles { get; set; }
    
    public void Load(ContentManager content, string buildingType, string color)
    {
        Sprite = content.Load<Texture2D>(
            $"Textures/Buildings/{color} Buildings/{buildingType}"
        );
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        Vector2 worldPos = GridPosition * 16; // 16 pixels per tile
        spriteBatch.Draw(Sprite, worldPos, Color.White);
    }
}
```

### 5. Resources (31 assets)

**Usage**: Harvestable resources in the world

**Types**:
- **Gold**: Gold stones (6 variants) with highlight versions
- **Tools**: Tool sprites (4 types)
- **Animals**: Sheep (idle, grass), other livestock

**Code Example**:
```csharp
public class HarvestableResource
{
    public Texture2D NormalSprite { get; set; }
    public Texture2D HighlightSprite { get; set; }
    public int RemainingHits { get; set; }
    
    public void Draw(SpriteBatch spriteBatch, Vector2 position, bool isHovered)
    {
        var sprite = isHovered ? HighlightSprite : NormalSprite;
        spriteBatch.Draw(sprite, position, Color.White);
    }
}
```

### 6. Decorations (21 assets)

**Usage**: Environmental aesthetics

**Types**:
- Rocks (4 variants)
- Rocks in water (4 variants)
- Bushes (3 variants)
- Clouds (8 variants)
- Rubber duck (why not!)

**Code Example**:
```csharp
// Place decorative rocks randomly
Random rand = new Random();
for (int i = 0; i < 10; i++)
{
    int rockType = rand.Next(1, 5); // Rock1 to Rock4
    var rockSprite = Content.Load<Texture2D>(
        $"Textures/Decorations/Rocks/Rock{rockType}"
    );
    
    Vector2 pos = new Vector2(rand.Next(width), rand.Next(height));
    decorations.Add(new Decoration(rockSprite, pos));
}
```

### 7. Effects (14 assets)

**Usage**: Visual feedback and ambiance

**Types**:
- Chimney smoke animations
- Particle effects

**Code Example**:
```csharp
public class SmokeEffect : ParticleEffect
{
    private Texture2D[] _frames;
    private int _currentFrame;
    
    public void Load(ContentManager content)
    {
        // Load smoke animation frames
        _frames = new Texture2D[4];
        for (int i = 0; i < 4; i++)
        {
            _frames[i] = content.Load<Texture2D>(
                $"Textures/Effects/smoke/smoke_frame_{i}"
            );
        }
    }
}
```

### 8. Enemies (20 assets)

**Usage**: Combat encounters

**Available**:
- Goblin sprites with animations
- Multiple poses and actions

**Code Example**:
```csharp
public class Goblin : Enemy
{
    public void Load(ContentManager content)
    {
        IdleSprite = content.Load<Texture2D>("Textures/Enemies/Goblins/goblin_idle");
        AttackSprite = content.Load<Texture2D>("Textures/Enemies/Goblins/goblin_attack");
        WalkSprite = content.Load<Texture2D>("Textures/Enemies/Goblins/goblin_walk");
    }
}
```

### 9. Objects (253 assets)

**Usage**: Interactive items, furniture, tools

**Types**: Various interactive objects from the asset packs

**Code Example**:
```csharp
public class InteractiveObject
{
    public Texture2D Sprite { get; set; }
    public string ObjectType { get; set; }
    public Action OnInteract { get; set; }
    
    public void Load(ContentManager content, string objectPath)
    {
        Sprite = content.Load<Texture2D>($"Textures/Objects/{objectPath}");
    }
}
```

## Updating Content.mgcb

After organizing assets, they need to be added to the Content Pipeline:

### Option 1: Manual Addition (Small batches)

1. Open `Content.mgcb` in the MonoGame Content Pipeline Tool
2. Right-click on Textures folder
3. Add → Existing Item
4. Select the new assets
5. Set Build Action to "Build"

### Option 2: Bulk Addition (Script)

Create a script to auto-generate Content.mgcb entries:

```python
# tools/update_content_mgcb.py
def generate_mgcb_entries(texture_dir):
    entries = []
    for png_file in texture_dir.rglob("*.png"):
        relative_path = png_file.relative_to(texture_dir.parent)
        entry = f"""
#begin {relative_path}
/importer:TextureImporter
/processor:TextureProcessor
/processorParam:ColorKeyColor=255,0,255,255
/processorParam:ColorKeyEnabled=False
/processorParam:GenerateMipmaps=False
/processorParam:PremultiplyAlpha=True
/processorParam:ResizeToPowerOfTwo=False
/processorParam:MakeSquare=False
/processorParam:TextureFormat=Color
/build:{relative_path}
"""
        entries.append(entry)
    return entries
```

### Option 3: Dynamic Loading (Runtime)

Load textures dynamically without Content Pipeline:

```csharp
public class DynamicAssetLoader
{
    public Texture2D LoadTextureFromFile(GraphicsDevice graphics, string path)
    {
        using FileStream stream = new FileStream(path, FileMode.Open);
        return Texture2D.FromStream(graphics, stream);
    }
}
```

## Asset Loading Best Practices

### 1. Lazy Loading

Don't load all 11,000 assets at startup. Load on-demand:

```csharp
public class AssetManager
{
    private Dictionary<string, Texture2D> _cache = new();
    private ContentManager _content;
    
    public Texture2D GetTexture(string path)
    {
        if (!_cache.ContainsKey(path))
        {
            _cache[path] = _content.Load<Texture2D>(path);
        }
        return _cache[path];
    }
    
    public void UnloadCategory(string category)
    {
        var toRemove = _cache.Keys.Where(k => k.Contains(category)).ToList();
        foreach (var key in toRemove)
        {
            _cache[key]?.Dispose();
            _cache.Remove(key);
        }
    }
}
```

### 2. Texture Atlases

For frequently used small sprites, combine into atlases:

```csharp
public class TextureAtlas
{
    public Texture2D Texture { get; set; }
    public Dictionary<string, Rectangle> Regions { get; set; }
    
    public void Draw(SpriteBatch sb, string region, Vector2 position)
    {
        if (Regions.TryGetValue(region, out var rect))
        {
            sb.Draw(Texture, position, rect, Color.White);
        }
    }
}
```

### 3. Asset Preloading

Preload essential assets for each game state:

```csharp
public class GameplayState : GameState
{
    public override void LoadContent(ContentManager content)
    {
        // Load only what's needed for gameplay
        _assetManager.PreloadCategory("Tiles");
        _assetManager.PreloadCategory("Characters/WALKING");
        _assetManager.PreloadCategory("Crops");
        // Don't load buildings, effects, etc. until needed
    }
}
```

## Integration Roadmap

### Phase 1: Core Assets (Immediate)
- [x] Organize tile assets
- [ ] Import character walking/idle animations
- [ ] Import crop growth stages
- [ ] Update WorldMap to use organized tiles

### Phase 2: Gameplay Assets (Short-term)
- [ ] Import building sprites
- [ ] Import resource sprites (rocks, trees, gold)
- [ ] Import decoration sprites
- [ ] Add to world generation system

### Phase 3: Advanced Assets (Medium-term)
- [ ] Import all character animations (mining, fishing, etc.)
- [ ] Import enemy sprites
- [ ] Import particle effects
- [ ] Integrate with animation system

### Phase 4: Polish (Long-term)
- [ ] Create texture atlases for performance
- [ ] Implement dynamic asset loading
- [ ] Add asset hot-reloading for development
- [ ] Optimize memory usage

## Performance Considerations

### Memory Usage

11,000 assets × ~10KB average = ~110MB of textures

**Strategies**:
1. **Lazy Loading**: Load only what's visible/needed
2. **Unloading**: Unload unused categories (e.g., unload mine assets when on surface)
3. **Texture Compression**: Use DXT compression for non-pixel-art assets
4. **Mipmaps**: Disabled for pixel art (sharp rendering)

### Loading Time

**Initial Load Optimization**:
- Only load essential startup assets (~50-100 textures)
- Load additional assets during gameplay transitions
- Show loading screen for heavy asset loads

### Rendering Performance

**Batching**:
- Group draws by texture (minimize texture switches)
- Use sprite atlases for small frequently-drawn sprites
- Implement frustum culling (only draw visible objects)

## Troubleshooting

### Issue: Assets not appearing in game

**Solution**:
1. Check Content.mgcb includes the asset
2. Verify Build Action is "Build"
3. Check the content path is correct
4. Rebuild Content project

### Issue: Blurry sprites

**Solution**:
```csharp
// Use PointClamp for pixel art
spriteBatch.Begin(
    samplerState: SamplerState.PointClamp
);
```

### Issue: Memory overflow

**Solution**:
- Implement asset unloading
- Use texture atlases
- Reduce loaded asset count
- Check for memory leaks (dispose textures)

### Issue: Slow loading

**Solution**:
- Implement async loading
- Show loading screen
- Preload during transitions
- Use texture compression

## References

- **Asset Organizer Script**: `tools/organize_sunnyside_assets.py`
- **Asset Catalog**: `Content/Textures/asset_catalog.json` (after organization)
- **Content Pipeline**: `Content/Content.mgcb`
- **Sprite Guide**: `SPRITE_GUIDE.md`
- **Tileset Guide**: `TILESET_GUIDE.md`

## License and Attribution

**Sunnyside World Assets**:
- Created by various artists
- Check individual asset pack licenses
- Provide attribution as required

---

**Document Version**: 1.0  
**Last Updated**: January 2026  
**Project**: MoonBrook Ridge
