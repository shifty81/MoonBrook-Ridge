# Modern Interiors Asset Pack - Integration Guide

## Overview

This document describes the Modern Interiors asset pack integration for MoonBrook Ridge. These assets from LimeZu (https://limezu.itch.io/moderninteriors) provide comprehensive 16x16 pixel interior tiles for houses, businesses, and town POIs.

**Asset Pack Version:** Modern Interiors Free v2.2  
**License:** Free for personal and commercial use (see LICENSE.txt)  
**Artist:** LimeZu (https://limezu.itch.io/)  
**Integration Date:** January 2026

## Asset Compatibility ✅

The Modern Interiors pack is fully compatible with MoonBrook Ridge:

- **Tile Size**: 16x16 pixels (matches `GameConstants.TILE_SIZE = 16`) ✅
- **Format**: PNG with RGBA alpha channel ✅
- **Style**: Top-down pixel art ✅
- **Color Palette**: Cohesive modern aesthetic ✅

## Asset Location

All Modern Interiors assets are located in:

```
MoonBrookRidge/Content/Textures/Interiors/ModernInteriors/
├── Tilesets/
│   ├── Interiors_free_16x16.png       # Main interior objects tileset (256x1424, 16x89 tiles)
│   ├── Room_Builder_free_16x16.png    # Walls and floors tileset (272x368, 17x23 tiles)
│   └── (Future: organized individual assets)
├── Floors/           # (Reserved for extracted floor tiles)
├── Walls/            # (Reserved for extracted wall tiles)
├── Furniture/        # (Reserved for extracted furniture)
├── Kitchen/          # (Reserved for extracted kitchen items)
├── Bedroom/          # (Reserved for extracted bedroom items)
├── Bathroom/         # (Reserved for extracted bathroom items)
├── Office/           # (Reserved for extracted office items)
├── Decorations/      # (Reserved for extracted decorations)
└── free_overview.png # Visual reference showing all available tiles
```

## Tileset Files

### 1. Interiors_free_16x16.png

**Dimensions:** 256x1424 pixels (16 columns × 89 rows = 1,424 tiles)  
**Content:** Comprehensive interior objects and furniture

This is the main tileset containing:
- **Furniture**: Beds, sofas, chairs, tables, desks
- **Kitchen**: Refrigerators, stoves, sinks, counters, cabinets
- **Bathroom**: Toilets, sinks, bathtubs, showers
- **Office**: Desks, computers, bookshelves, filing cabinets
- **Decorations**: Plants, lamps, paintings, rugs, curtains
- **Electronics**: TVs, computers, microwaves, washing machines
- **Storage**: Dressers, wardrobes, shelves, boxes
- **Misc**: Doors, windows, stairs, railings

**Tile Layout:** 16 tiles per row, 89 rows total

```
Tiles 0-15:    Row 0  - [Kitchen appliances and counters]
Tiles 16-31:   Row 1  - [Furniture and storage]
Tiles 32-47:   Row 2  - [Bedroom furniture]
Tiles 48-63:   Row 3  - [Bathroom fixtures]
...
(See free_overview.png for complete visual reference)
```

### 2. Room_Builder_free_16x16.png

**Dimensions:** 272x368 pixels (17 columns × 23 rows = 391 tiles)  
**Content:** Walls, floors, and room construction elements

This tileset contains:
- **Floor Tiles**: Wood, carpet, tile, stone variations
- **Wall Tiles**: Various wall styles with windows and doors
- **Corner Pieces**: Inside/outside corners for room construction
- **Trim**: Baseboards, molding, edge pieces
- **Doors**: Various door styles and states (open/closed)
- **Windows**: Different window styles and frames

**Tile Layout:** 17 tiles per row, 23 rows total

This tileset is designed for autotiling and includes all necessary variations for seamless room construction.

## Loading Assets in Code

### Basic Asset Loading

```csharp
// In your LoadContent() method or asset manager
private Texture2D interiorsAtlas;
private Texture2D roomBuilderAtlas;

protected override void LoadContent()
{
    // Load the main interior objects tileset
    interiorsAtlas = Content.Load<Texture2D>("Textures/Interiors/ModernInteriors/Tilesets/Interiors_free_16x16");
    
    // Load the room builder (floors/walls) tileset
    roomBuilderAtlas = Content.Load<Texture2D>("Textures/Interiors/ModernInteriors/Tilesets/Room_Builder_free_16x16");
}
```

### Drawing a Single Tile

```csharp
// Drawing a specific tile from the Interiors tileset
// Tile ID is calculated as: (row * columns) + column

public void DrawInteriorTile(SpriteBatch spriteBatch, int tileId, Vector2 position)
{
    const int TILE_SIZE = 16;
    const int COLUMNS = 16; // Interiors tileset has 16 columns
    
    // Calculate source position in the tileset
    int sourceX = (tileId % COLUMNS) * TILE_SIZE;
    int sourceY = (tileId / COLUMNS) * TILE_SIZE;
    
    Rectangle sourceRect = new Rectangle(sourceX, sourceY, TILE_SIZE, TILE_SIZE);
    Rectangle destRect = new Rectangle(
        (int)position.X * TILE_SIZE,
        (int)position.Y * TILE_SIZE,
        TILE_SIZE,
        TILE_SIZE
    );
    
    spriteBatch.Draw(interiorsAtlas, destRect, sourceRect, Color.White);
}
```

### Drawing Room Builder Tiles

```csharp
public void DrawRoomTile(SpriteBatch spriteBatch, int tileId, Vector2 position)
{
    const int TILE_SIZE = 16;
    const int COLUMNS = 17; // Room Builder has 17 columns
    
    int sourceX = (tileId % COLUMNS) * TILE_SIZE;
    int sourceY = (tileId / COLUMNS) * TILE_SIZE;
    
    Rectangle sourceRect = new Rectangle(sourceX, sourceY, TILE_SIZE, TILE_SIZE);
    Rectangle destRect = new Rectangle(
        (int)position.X * TILE_SIZE,
        (int)position.Y * TILE_SIZE,
        TILE_SIZE,
        TILE_SIZE
    );
    
    spriteBatch.Draw(roomBuilderAtlas, destRect, sourceRect, Color.White);
}
```

## Creating Interior Rooms

### Example: Simple Kitchen

```csharp
public class InteriorRoom
{
    private int[,] floorTiles;
    private int[,] wallTiles;
    private List<InteriorObject> objects;
    
    public void CreateKitchen()
    {
        // Create 10x8 kitchen floor with wood tiles (example tile IDs)
        floorTiles = new int[10, 8];
        for (int y = 0; y < 8; y++)
        {
            for (int x = 0; x < 10; x++)
            {
                floorTiles[x, y] = 5; // Wood floor tile ID from Room Builder
            }
        }
        
        // Place furniture from Interiors tileset
        objects = new List<InteriorObject>
        {
            new InteriorObject(35, new Vector2(2, 2)),  // Refrigerator
            new InteriorObject(36, new Vector2(4, 2)),  // Stove
            new InteriorObject(37, new Vector2(6, 2)),  // Sink
            new InteriorObject(50, new Vector2(5, 5)),  // Kitchen table
            new InteriorObject(51, new Vector2(4, 6)),  // Chair
            new InteriorObject(52, new Vector2(6, 6)),  // Chair
        };
    }
    
    public void Draw(SpriteBatch spriteBatch, Texture2D roomAtlas, Texture2D objectAtlas)
    {
        // Draw floor
        for (int y = 0; y < floorTiles.GetLength(1); y++)
        {
            for (int x = 0; x < floorTiles.GetLength(0); x++)
            {
                DrawRoomTile(spriteBatch, floorTiles[x, y], new Vector2(x, y));
            }
        }
        
        // Draw objects
        foreach (var obj in objects)
        {
            DrawInteriorTile(spriteBatch, obj.TileId, obj.Position);
        }
    }
}

public class InteriorObject
{
    public int TileId { get; set; }
    public Vector2 Position { get; set; }
    
    public InteriorObject(int tileId, Vector2 position)
    {
        TileId = tileId;
        Position = position;
    }
}
```

## Tile Categories and ID Ranges

**Note:** Exact tile IDs should be verified using `free_overview.png`. The ranges below are approximate based on typical tileset organization.

### Interiors_free_16x16.png (Main Objects)

| Category | Approx. Row Range | Description |
|----------|-------------------|-------------|
| Kitchen Appliances | 0-10 | Fridges, stoves, microwaves, sinks |
| Kitchen Furniture | 10-20 | Counters, cabinets, tables, chairs |
| Living Room | 20-35 | Sofas, TV stands, coffee tables, lamps |
| Bedroom | 35-50 | Beds, dressers, nightstands, wardrobes |
| Bathroom | 50-60 | Toilets, sinks, bathtubs, showers |
| Office | 60-70 | Desks, computers, bookshelves, chairs |
| Decorations | 70-85 | Plants, paintings, rugs, curtains, misc |
| Special Items | 85-89 | Doors, windows, stairs, unique objects |

### Room_Builder_free_16x16.png (Floors/Walls)

| Category | Approx. Row Range | Description |
|----------|-------------------|-------------|
| Floor Variations | 0-8 | Wood, tile, carpet, stone floors |
| Wall Bases | 8-12 | Basic wall tiles in different colors |
| Wall Details | 12-18 | Walls with windows, doors, trim |
| Corners & Edges | 18-23 | Corner pieces and edge transitions |

## Using with Camera System

The game's default 2x camera zoom works perfectly with 16x16 tiles:

```csharp
// In your camera setup
_camera.Zoom = 2.0f; // Each 16x16 tile renders as 32x32 screen pixels

// Always use PointClamp for crisp pixel art
spriteBatch.Begin(
    transformMatrix: camera.GetTransform(),
    samplerState: SamplerState.PointClamp
);
```

## Integration Tips

### 1. Autotiling for Walls and Floors

Consider implementing an autotiling system for seamless room construction:

```csharp
public int GetAutoTile(int x, int y, int[,] tileMap)
{
    // Check adjacent tiles and select appropriate corner/edge tile
    bool hasTop = (y > 0 && tileMap[x, y - 1] != 0);
    bool hasBottom = (y < tileMap.GetLength(1) - 1 && tileMap[x, y + 1] != 0);
    bool hasLeft = (x > 0 && tileMap[x - 1, y] != 0);
    bool hasRight = (x < tileMap.GetLength(0) - 1 && tileMap[x + 1, y] != 0);
    
    // Use bitmasking to determine correct tile variant
    int mask = (hasTop ? 1 : 0) | (hasRight ? 2 : 0) | 
               (hasBottom ? 4 : 0) | (hasLeft ? 8 : 0);
    
    return GetTileForMask(mask);
}
```

### 2. Layered Rendering

Render interior scenes in layers for proper depth:

```csharp
public void DrawInteriorScene(SpriteBatch spriteBatch)
{
    // Layer 1: Floor tiles
    DrawFloorLayer(spriteBatch);
    
    // Layer 2: Floor decorations (rugs, floor items)
    DrawFloorDecorations(spriteBatch);
    
    // Layer 3: Furniture and objects
    DrawFurniture(spriteBatch);
    
    // Layer 4: Character/NPCs
    DrawCharacters(spriteBatch);
    
    // Layer 5: Overhead elements (ceiling lamps, etc)
    DrawOverheadLayer(spriteBatch);
}
```

### 3. Collision Detection

Add collision data for furniture:

```csharp
public class InteriorObject
{
    public int TileId { get; set; }
    public Vector2 Position { get; set; }
    public bool IsWalkable { get; set; }
    public Rectangle CollisionBox { get; set; }
    
    public InteriorObject(int tileId, Vector2 position, bool walkable = false)
    {
        TileId = tileId;
        Position = position;
        IsWalkable = walkable;
        
        // Most furniture is not walkable
        if (!walkable)
        {
            CollisionBox = new Rectangle(
                (int)position.X * 16,
                (int)position.Y * 16,
                16, 16
            );
        }
    }
}
```

### 4. Interaction System

Add interactive elements:

```csharp
public class InteractableObject : InteriorObject
{
    public Action OnInteract { get; set; }
    public string InteractionPrompt { get; set; }
    
    public InteractableObject(int tileId, Vector2 position, Action onInteract)
        : base(tileId, position, false)
    {
        OnInteract = onInteract;
        InteractionPrompt = "Press E to interact";
    }
}

// Example: Bed that restores health
var bed = new InteractableObject(
    tileId: 128,
    position: new Vector2(5, 3),
    onInteract: () => {
        // Restore player health
        player.RestoreHealth(100);
        ShowMessage("You slept and restored your health!");
    }
);
```

## Intended Use Cases

These assets are designed for:

### 1. Player Home Interior
- Bedroom with bed, dresser, wardrobe
- Kitchen with appliances and dining area
- Living room with furniture and entertainment
- Bathroom with fixtures
- Upgradable/customizable layout

### 2. NPC Houses
- Unique layouts for each NPC reflecting their personality
- Interactive furniture (beds, chairs, chests)
- Decorative items that tell a story

### 3. Town Businesses
- **General Store**: Shelves, counters, cash register
- **Cafe/Restaurant**: Kitchen, dining tables, bar
- **Clinic**: Medical equipment, waiting area
- **Library**: Bookshelves, reading tables
- **Town Hall**: Office desks, meeting rooms

### 4. Community Buildings
- **Community Center**: Multi-purpose rooms with various furniture
- **Museum**: Display cases, exhibits
- **School**: Desks, chalkboards, supplies

## Performance Considerations

### Texture Atlas Usage

Both tilesets are texture atlases designed for efficient rendering:

```csharp
// Load once, use throughout the game
public class InteriorAssetManager
{
    private static Texture2D _interiorsAtlas;
    private static Texture2D _roomBuilderAtlas;
    private static bool _loaded = false;
    
    public static void LoadContent(ContentManager content)
    {
        if (_loaded) return;
        
        _interiorsAtlas = content.Load<Texture2D>(
            "Textures/Interiors/ModernInteriors/Tilesets/Interiors_free_16x16"
        );
        _roomBuilderAtlas = content.Load<Texture2D>(
            "Textures/Interiors/ModernInteriors/Tilesets/Room_Builder_free_16x16"
        );
        
        _loaded = true;
    }
    
    public static Texture2D InteriorObjects => _interiorsAtlas;
    public static Texture2D RoomBuilder => _roomBuilderAtlas;
}
```

### Culling Off-Screen Tiles

Only render visible interior tiles:

```csharp
public void DrawInteriorWithCulling(SpriteBatch spriteBatch, Camera2D camera)
{
    // Calculate visible tile range
    int minTileX = Math.Max(0, (int)(camera.Position.X / 16) - 1);
    int maxTileX = Math.Min(width, (int)((camera.Position.X + camera.ViewportWidth) / 16) + 1);
    int minTileY = Math.Max(0, (int)(camera.Position.Y / 16) - 1);
    int maxTileY = Math.Min(height, (int)((camera.Position.Y + camera.ViewportHeight) / 16) + 1);
    
    // Only draw visible tiles
    for (int y = minTileY; y < maxTileY; y++)
    {
        for (int x = minTileX; x < maxTileX; x++)
        {
            DrawTile(spriteBatch, x, y);
        }
    }
}
```

## Future Enhancements

Potential expansions for this asset integration:

- [ ] **Extract Individual Tiles**: Separate commonly used furniture into individual files
- [ ] **Create Prefab Rooms**: Pre-designed room templates (kitchen, bedroom, etc.)
- [ ] **Animation Support**: Animated tiles (TVs, computers, water in sinks)
- [ ] **Seasonal Variations**: Holiday decorations, seasonal items
- [ ] **Furniture Catalog System**: In-game furniture placement and customization
- [ ] **Save/Load Room Layouts**: Persistent interior customization
- [ ] **Lighting System**: Lamps and windows that affect ambient lighting
- [ ] **Interactive Appliances**: Functional kitchen appliances, computers, etc.

## Asset License

**Modern Interiors Free v2.2**  
**Artist:** LimeZu (https://limezu.itch.io/)  
**License:** Free for personal and commercial use

See `Modern_Interiors_Free_v2.2/LICENSE.txt` for full license details.

**Attribution:** While not required, crediting LimeZu in your game is appreciated.

## Additional Resources

- **Asset Source**: https://limezu.itch.io/moderninteriors
- **Visual Reference**: `MoonBrookRidge/Content/Textures/Interiors/ModernInteriors/free_overview.png`
- **Existing Tileset Guide**: `TILESET_GUIDE.md` (ground tiles)
- **Asset Loading Guide**: `ASSET_LOADING_GUIDE.md` (MonoGame Content Pipeline)
- **Tile Size Reference**: `TILE_SIZE_GUIDE.md` (16x16 standard)

## Example Projects

### Quick Test: Loading and Displaying a Tile

```csharp
// In Game1.cs or a test scene
private Texture2D interiorsAtlas;

protected override void LoadContent()
{
    interiorsAtlas = Content.Load<Texture2D>(
        "Textures/Interiors/ModernInteriors/Tilesets/Interiors_free_16x16"
    );
}

protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(Color.CornflowerBlue);
    
    _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
    
    // Draw a refrigerator tile at screen position (100, 100)
    // Assuming tile ID 35 is a refrigerator (verify with free_overview.png)
    int tileId = 35;
    int sourceX = (tileId % 16) * 16;
    int sourceY = (tileId / 16) * 16;
    
    _spriteBatch.Draw(
        interiorsAtlas,
        new Rectangle(100, 100, 64, 64), // 4x scaled for visibility
        new Rectangle(sourceX, sourceY, 16, 16),
        Color.White
    );
    
    _spriteBatch.End();
    
    base.Draw(gameTime);
}
```

---

**Document Version:** 1.0  
**Last Updated:** January 3, 2026  
**Project:** MoonBrook Ridge
