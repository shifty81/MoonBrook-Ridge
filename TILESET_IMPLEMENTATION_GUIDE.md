# Tileset Implementation Guide - MoonBrook Ridge

## Overview

This guide provides comprehensive documentation for implementing tileset dimensions and rendering systems in MoonBrook Ridge, following best practices from Stardew Valley-like games.

## Tileset Dimensions and Details

### World/Terrain Tiles (Ground, Water, Paths, Biomes)

**Recommended Dimensions**: **16×16 pixels**

This is the foundational grid size for the map. These tiles are typically arranged in layers for terrain, water, and paths.

#### Current Implementation

```csharp
// MoonBrookRidge/Core/GameConstants.cs
public const int TILE_SIZE = 16;

// MoonBrookRidge/World/Maps/WorldMap.cs
private const int TILE_SIZE = 16;
```

#### Tile Types Available
- **Grass variants**: Multiple grass textures for natural variation (Tiles 0-15 in ground_tileset.png)
- **Dirt & Paths**: Dirt paths and transitional terrain (Tiles 16-31)
- **Tilled Soil & Farmland**: Agricultural tiles for farming (Tiles 32-47)
- **Stone & Rock**: Stone paths, rock formations (Tiles 48-63)
- **Water**: Water tiles with various patterns (Tiles 64-79)
- **Sand & Beach**: Sandy terrain for beaches and deserts (Tiles 80-95)

#### Usage Example

```csharp
// Calculate destination rectangle for a tile at grid position (x, y)
Rectangle tileRect = new Rectangle(
    x * TILE_SIZE,  // 16 pixels per tile
    y * TILE_SIZE,  // 16 pixels per tile
    TILE_SIZE,      // 16 pixel width
    TILE_SIZE       // 16 pixel height
);

spriteBatch.Draw(tileTexture, tileRect, Color.White);
```

### Characters (Player, NPCs)

**Recommended Dimensions**: **16×32 pixels (sprite size)**

Characters occupy one 16×16 tile in the game grid but have a taller sprite to give a top-down, slightly angled view. The "feet" or collision point is what interacts with the grid for movement and sorting.

#### Implementation Details

```csharp
// Character base occupies 1 tile but sprite is taller
public const float PLAYER_RADIUS = 16f; // Half the player collision size

// Character position determines grid placement
int tileX = (int)(worldPosition.X / GameConstants.TILE_SIZE);
int tileY = (int)(worldPosition.Y / GameConstants.TILE_SIZE);
```

#### Character Sprite System

The game uses a modular character system with separate layers:

1. **Base Layer**: Body/skin tone (base_*_strip*.png)
2. **Hair Layer**: Various hairstyles (longhair_*, shorthair_*, etc.)
3. **Tool Layer**: Shows tools in character's hands (tools_*_strip*.png)

Animation strips contain multiple frames (e.g., `base_walk_strip8.png` has 8 walking frames).

```csharp
// Example: Layered character rendering
Texture2D baseSprite = Content.Load<Texture2D>("characters/base_walk_strip8");
Texture2D hairSprite = Content.Load<Texture2D>("characters/longhair_walk_strip8");
Texture2D toolSprite = Content.Load<Texture2D>("characters/tools_walk_strip8");

// Draw layered (in order):
spriteBatch.Draw(baseSprite, position, sourceRect, Color.White);
spriteBatch.Draw(hairSprite, position, sourceRect, hairColor);
spriteBatch.Draw(toolSprite, position, sourceRect, Color.White);
```

### Buildings

**Recommended Dimensions**: **Varied (multiple of 16×16)**

Buildings are large objects that occupy multiple tiles (e.g., a farmhouse might be 9×6 tiles). They are often treated as single, large sprites or objects rather than being built from many individual 16×16 tiles.

#### Current Building Types

| Building | Tile Size | Description |
|----------|-----------|-------------|
| Barn | 6×8 tiles (96×128px) | Houses livestock (12 animal capacity) |
| Coop | 5×6 tiles (80×96px) | Houses chickens and ducks (8 bird capacity) |
| Shed | 4×5 tiles (64×80px) | Storage building (120 inventory slots) |
| Silo | 3×4 tiles (48×64px) | Stores hay for animals (240 hay capacity) |
| Well | 2×2 tiles (32×32px) | Provides water source |
| Greenhouse | 8×10 tiles (128×160px) | Year-round crop growth |
| Mill | 5×6 tiles (80×96px) | Process crops into products |
| Workshop | 5×5 tiles (80×80px) | Advanced crafting and ore smelting |

#### Building Placement System

```csharp
// Buildings occupy multiple tiles
public class Building
{
    public int WidthInTiles { get; set; }  // e.g., 6 tiles
    public int HeightInTiles { get; set; } // e.g., 8 tiles
    public Vector2 GridPosition { get; set; }
    
    // Check if area is valid for placement
    public bool CanPlaceAt(WorldMap map, int x, int y)
    {
        for (int dx = 0; dx < WidthInTiles; dx++)
        {
            for (int dy = 0; dy < HeightInTiles; dy++)
            {
                int checkX = x + dx;
                int checkY = y + dy;
                
                // Verify tile is in bounds and valid for building
                if (!map.IsTileValidForBuilding(checkX, checkY))
                    return false;
            }
        }
        return true;
    }
}
```

### Placeable/Interactive Items (Crafting stations, Storage, Crops)

**Recommended Dimensions**: **Varied (e.g., 16×16, 16×32, 32×32)**

Small items like chairs might be 1×1 tiles (16×16), while a large table could be 2×2 tiles (32×32). These are typically handled as dynamic objects that the player places onto the grid during gameplay.

#### Crop System

Crops occupy a single tile (16×16) but display growth stages:

```csharp
public class Crop
{
    private string _cropType;
    private int _growthStage;        // 0-4 (seed to harvest)
    private int _maxGrowthStage;
    private float _hoursPerStage;    // Game hours per stage
    
    // Each stage has its own sprite
    public Texture2D GetCurrentStageSprite()
    {
        return _stageSprites[_growthStage];
    }
}
```

#### Interactive Objects

```csharp
public class WorldObject
{
    public string Type { get; set; }      // "Tree", "Rock", "Chest", etc.
    public Vector2 GridPosition { get; set; }
    public int TileWidth { get; set; }    // Width in tiles
    public int TileHeight { get; set; }   // Height in tiles
    public Texture2D Sprite { get; set; }
    
    // Objects can span multiple tiles but are placed at a grid position
    public Rectangle GetBoundingBox()
    {
        return new Rectangle(
            (int)(GridPosition.X * TILE_SIZE),
            (int)(GridPosition.Y * TILE_SIZE),
            TileWidth * TILE_SIZE,
            TileHeight * TILE_SIZE
        );
    }
}
```

## Implementation Details in Game Engine

### Use a Tile Map Editor

**Tool**: Tiled Map Editor (https://www.mapeditor.org/)

Tiled is the standard tool for creating maps in this style. It allows you to:
- Define multiple layers
- Place tiles from tilesets
- Add custom properties (like "passable" or "blocked") to specific tiles
- Export to JSON or other formats

#### Integration with MoonBrook Ridge

The game supports loading worlds from JSON configuration files:

```csharp
// Load world configuration
WorldGenConfig config = JsonSerializer.Deserialize<WorldGenConfig>(jsonContent);

// Initialize world from config
worldMap.InitializeFromConfig(config);
```

Configuration files are located in `MoonBrookRidge/Content/WorldGen/`:
- `default_world.json` - Main overworld
- `default_mine.json` - Cave/mine levels
- `desert_world.json` - Desert biome
- `deep_mine.json` - Deep mining areas

### Layer Management

**Implementation**: Multiple layers for correct visual sorting

#### Standard Layers

The rendering system uses layer-based sorting with Y-ordering:

```csharp
public abstract class Renderable
{
    public int Layer { get; set; } // Layer determines base sorting order
    public Vector2 Position { get; set; }
    
    // Layer values:
    // 0 = Back (Ground/Terrain)
    // 1 = Buildings (Large structures)
    // 2 = Paths (Flooring, removable debris)
    // 3 = Objects (Trees, rocks, placed items)
    // 4 = Front (Object tops that appear above player)
    // 5 = Characters (Player, NPCs, enemies)
    // 6 = Effects (Particles, animations)
    // 7 = AlwaysFront (UI overlays)
}
```

#### Rendering with Layers

```csharp
public class RenderingSystem
{
    public void Draw(Camera2D camera)
    {
        // Sort by Layer first, then by Y position within each layer
        var sorted = _renderables
            .OrderBy(r => r.Layer)
            .ThenBy(r => r.Position.Y)
            .ToList();
        
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Deferred,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointClamp,  // Critical for pixel art!
            transformMatrix: camera.GetTransform()
        );
        
        foreach (var renderable in sorted)
        {
            if (renderable.IsVisible)
            {
                renderable.Draw(_spriteBatch);
            }
        }
        
        _spriteBatch.End();
    }
}
```

### Character Movement and Z-Sorting

**Implementation**: Grid-based movement system with Y-sorting

#### Grid-Based Movement

Characters move in smooth increments based on the 16×16 pixel grid:

```csharp
// Character movement
public class CharacterController
{
    private Vector2 _position;
    private float _baseSpeed = 100f;      // pixels per second
    private float _runningSpeed = 175f;   // pixels per second when running
    
    public void Update(GameTime gameTime, Vector2 direction)
    {
        float speed = IsRunning ? _runningSpeed : _baseSpeed;
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Move character smoothly
        _position += direction * speed * deltaTime;
        
        // Clamp to world boundaries
        _position.X = MathHelper.Clamp(_position.X, 0, 
            worldMap.Width * GameConstants.TILE_SIZE - GameConstants.PLAYER_RADIUS);
        _position.Y = MathHelper.Clamp(_position.Y, 0,
            worldMap.Height * GameConstants.TILE_SIZE - GameConstants.PLAYER_RADIUS);
    }
    
    // Get current tile position
    public Point GetGridPosition()
    {
        return new Point(
            (int)(_position.X / GameConstants.TILE_SIZE),
            (int)(_position.Y / GameConstants.TILE_SIZE)
        );
    }
}
```

#### Sprite Sorting (Y-Sorting)

The rendering system automatically sorts sprites by their Y position to ensure correct depth ordering. Objects lower on the screen (higher Y value) are drawn later, appearing in front of objects higher on the screen.

```csharp
// Y-sorting ensures correct visual depth
// Example: A character at Y=200 will be drawn in front of a tree at Y=150
// This creates the illusion that the character is "behind" the tree when north of it
// and "in front" of the tree when south of it
```

#### Collision with Grid

```csharp
public class CollisionSystem
{
    // Convert world position to tile coordinates
    public Point WorldToTile(Vector2 worldPosition)
    {
        return new Point(
            (int)(worldPosition.X / GameConstants.TILE_SIZE),
            (int)(worldPosition.Y / GameConstants.TILE_SIZE)
        );
    }
    
    // Check if tile is passable
    public bool IsTilePassable(WorldMap map, int tileX, int tileY)
    {
        if (tileX < 0 || tileX >= map.Width || tileY < 0 || tileY >= map.Height)
            return false;
        
        var tile = map.GetTile(tileX, tileY);
        
        // Water, rocks, buildings are not passable
        return tile.Type switch
        {
            TileType.Water or TileType.Water01 => false,
            TileType.Rock or TileType.Stone => false,
            _ => true
        };
    }
}
```

### Interactive Objects

**Implementation**: Handle placeable/craftable items as separate game objects

#### Tile Objects vs Tiles

Interactive objects are handled differently from static tiles:

```csharp
// Static tiles are part of the WorldMap
public class WorldMap
{
    private Tile[,] _tiles;  // 2D array of tiles
}

// Dynamic objects are separate entities
public class WorldObject
{
    public Vector2 GridPosition { get; set; }
    public string ObjectType { get; set; }
    public bool IsInteractable { get; set; }
    public bool IsPassable { get; set; }
    
    // Called when player interacts
    public void OnInteract(Player player)
    {
        // Handle interaction (open chest, harvest tree, etc.)
    }
}
```

#### Placement Validation

```csharp
public class ObjectPlacementSystem
{
    // Check if area is valid for placement
    public bool CanPlaceObject(WorldMap map, WorldObject obj, int gridX, int gridY)
    {
        // Check all tiles that object will occupy
        for (int dx = 0; dx < obj.TileWidth; dx++)
        {
            for (int dy = 0; dy < obj.TileHeight; dy++)
            {
                int checkX = gridX + dx;
                int checkY = gridY + dy;
                
                // Check bounds
                if (checkX < 0 || checkX >= map.Width || 
                    checkY < 0 || checkY >= map.Height)
                    return false;
                
                // Check if tile is clear
                var tile = map.GetTile(checkX, checkY);
                if (!IsTileClearForPlacement(tile))
                    return false;
                
                // Check if another object is already there
                if (IsObjectAtPosition(checkX, checkY))
                    return false;
            }
        }
        
        return true;
    }
    
    private bool IsTileClearForPlacement(Tile tile)
    {
        // Can only place on grass, dirt, or tilled soil
        return tile.Type switch
        {
            TileType.Grass or TileType.Grass01 or TileType.Grass02 or TileType.Grass03 => true,
            TileType.Dirt or TileType.Dirt01 or TileType.Dirt02 => true,
            TileType.Tilled or TileType.TilledDry => true,
            _ => false
        };
    }
}
```

### Autotiling

**Implementation**: Use autotiling logic to blend different terrain types seamlessly

#### Basic Autotiling Concept

Autotiling automatically selects the correct tile variant based on neighboring tiles to create smooth transitions.

```csharp
public class AutotilingSystem
{
    // Get appropriate tile based on neighbors
    public int GetAutoTileIndex(WorldMap map, int x, int y, TileType targetType)
    {
        // Check 8 neighbors (N, NE, E, SE, S, SW, W, NW)
        bool n  = IsSameType(map, x, y-1, targetType);
        bool ne = IsSameType(map, x+1, y-1, targetType);
        bool e  = IsSameType(map, x+1, y, targetType);
        bool se = IsSameType(map, x+1, y+1, targetType);
        bool s  = IsSameType(map, x, y+1, targetType);
        bool sw = IsSameType(map, x-1, y+1, targetType);
        bool w  = IsSameType(map, x-1, y, targetType);
        bool nw = IsSameType(map, x-1, y-1, targetType);
        
        // Use bitmasking to determine tile variant
        // This is simplified - actual implementation would use Wang tiles or blob tiles
        int mask = (n ? 1 : 0) | (e ? 2 : 0) | (s ? 4 : 0) | (w ? 8 : 0);
        
        return GetTileVariantForMask(mask);
    }
    
    private bool IsSameType(WorldMap map, int x, int y, TileType type)
    {
        if (x < 0 || x >= map.Width || y < 0 || y >= map.Height)
            return false;
        
        return map.GetTile(x, y).Type == type;
    }
    
    // Map bit mask to tile variant
    private int GetTileVariantForMask(int mask)
    {
        // 0 = isolated
        // 1 = connects north
        // 2 = connects east
        // 3 = connects north and east (inner corner)
        // ... etc for all 16 combinations
        return mask;
    }
}
```

#### Weighted Random Tiling

For natural-looking terrain without explicit autotiling rules:

```csharp
public class TerrainGenerator
{
    private Random _random = new Random();
    
    // Get a random grass variant for natural variation
    public TileType GetRandomGrassTile()
    {
        int variant = _random.Next(4);
        return variant switch
        {
            0 => TileType.Grass,
            1 => TileType.Grass01,
            2 => TileType.Grass02,
            _ => TileType.Grass03
        };
    }
    
    // Create natural terrain with smooth transitions
    public void GenerateNaturalTerrain(WorldMap map, int x, int y)
    {
        // Use noise or distance functions for smooth biome transitions
        float grassValue = GetPerlinNoise(x, y, scale: 0.1f);
        
        if (grassValue > 0.7f)
            map.SetTile(x, y, GetRandomGrassTile());
        else if (grassValue > 0.4f)
            map.SetTile(x, y, GetRandomDirtTile());
        else
            map.SetTile(x, y, TileType.Stone);
    }
}
```

## Camera System

### Camera Scaling and Pixel-Perfect Rendering

```csharp
public class Camera2D
{
    public float Zoom { get; set; } = 2.0f;  // Default 2x zoom
    public Vector2 Position { get; set; }
    
    public Matrix GetTransform()
    {
        return Matrix.CreateTranslation(-Position.X, -Position.Y, 0) *
               Matrix.CreateScale(Zoom, Zoom, 1) *
               Matrix.CreateTranslation(
                   _viewport.Width / 2f,
                   _viewport.Height / 2f,
                   0);
    }
    
    // Keep camera centered on player
    public void Follow(Vector2 targetPosition)
    {
        Position = targetPosition;
    }
}
```

### Rendering with Point Sampling

**Critical**: Always use `SamplerState.PointClamp` for pixel art to avoid blurring:

```csharp
spriteBatch.Begin(
    sortMode: SpriteSortMode.Deferred,
    blendState: BlendState.AlphaBlend,
    samplerState: SamplerState.PointClamp,  // Prevents pixel art from being blurred
    transformMatrix: camera.GetTransform()
);
```

## Performance Optimization

### Tile Rendering

```csharp
// Only render visible tiles (frustum culling)
public void DrawTiles(SpriteBatch spriteBatch, Camera2D camera)
{
    // Calculate visible tile range
    Rectangle visibleArea = camera.GetVisibleArea();
    
    int startX = Math.Max(0, visibleArea.Left / TILE_SIZE);
    int startY = Math.Max(0, visibleArea.Top / TILE_SIZE);
    int endX = Math.Min(_width, (visibleArea.Right / TILE_SIZE) + 1);
    int endY = Math.Min(_height, (visibleArea.Bottom / TILE_SIZE) + 1);
    
    // Only draw visible tiles
    for (int y = startY; y < endY; y++)
    {
        for (int x = startX; x < endX; x++)
        {
            DrawTile(spriteBatch, x, y);
        }
    }
}
```

### Texture Atlases

The game uses texture atlases to minimize texture switches:

```csharp
// ground_tileset.png contains 192 tiles (16x12 layout)
// All ground tiles can be drawn with a single texture
Texture2D groundTileset = Content.Load<Texture2D>("Textures/Tiles/ground_tileset");

// Draw from atlas
int tileId = 5;
int tilesPerRow = 16;
Rectangle sourceRect = new Rectangle(
    (tileId % tilesPerRow) * TILE_SIZE,
    (tileId / tilesPerRow) * TILE_SIZE,
    TILE_SIZE,
    TILE_SIZE
);
```

## Summary

MoonBrook Ridge implements industry-standard practices for tileset-based games:

✅ **16×16 base tile size** - Perfect for pixel art style and performance  
✅ **Character sprites (16×32)** - Occupy 1 tile, rendered with taller sprites  
✅ **Multi-tile buildings** - Large structures span multiple grid cells  
✅ **Layer-based rendering** - Proper visual sorting with 8 defined layers  
✅ **Y-sorting within layers** - Automatic depth sorting by position  
✅ **Grid-based movement** - Smooth character movement on tile grid  
✅ **Interactive objects** - Separate from tiles, with placement validation  
✅ **Autotiling support** - Smooth terrain transitions  
✅ **Pixel-perfect rendering** - PointClamp sampling for crisp visuals  
✅ **Performance optimized** - Frustum culling, texture atlases  

## References

- **GameConstants.cs** - Core game constants including TILE_SIZE
- **WorldMap.cs** - Tile-based world system
- **RenderingSystem.cs** - Layer and Y-sort rendering
- **TILE_SIZE_GUIDE.md** - Detailed tile size rationale
- **SPRITE_GUIDE.md** - Sprite asset organization
- **TILESET_GUIDE.md** - Tileset integration guide

## External Resources

- **Tiled Map Editor**: https://www.mapeditor.org/
- **Stardew Valley Wiki** (for reference): https://stardewvalleywiki.com/
- **MonoGame Documentation**: https://docs.monogame.net/
- **Pixel Art Tutorials**: Various online resources for 16×16 tile creation

---

**Document Version**: 1.0  
**Last Updated**: January 2026  
**Project**: MoonBrook Ridge
