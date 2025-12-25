# The World Below - Design Document

## Overview

"The World Below" is a planned expansion for MoonBrook Ridge that will add cavernous cave systems, dungeons, and secret areas to discover beneath the overworld. This document outlines the design goals and technical implementation plan for this feature.

## Design Goals

### Core Pillars
1. **Exploration** - Reward players for venturing into dangerous underground areas
2. **Challenge** - Provide combat and puzzle-solving opportunities
3. **Progression** - Unlock deeper areas as player skills and equipment improve
4. **Mystery** - Hide secrets and lore throughout the underground

### Gameplay Features

#### Cave Systems
- **Natural Caves**: Randomly generated cavern networks
- **Multiple Levels**: Increasing difficulty as player descends
- **Resources**: Unique minerals, ores, and materials found only underground
- **Hazards**: Environmental dangers (lava, water, cave-ins, toxic gas)

#### Dungeons
- **Structured Layouts**: Hand-crafted puzzle and combat encounters
- **Boss Encounters**: Unique enemies with special mechanics
- **Treasures**: Rare items and equipment rewards
- **Lore Elements**: Story pieces that reveal the world's history

#### Secret Areas
- **Hidden Passages**: Secret doors and breakable walls
- **Ancient Ruins**: Mysterious structures predating the town
- **Treasure Vaults**: High-value loot behind challenges
- **Easter Eggs**: Fun surprises for thorough explorers

## Technical Architecture

### Tile System Integration

The World Below will utilize the Slates tileset along with future dedicated underground tilesets:

#### Current Implementation (Slates Tileset)
The game now uses Slates v.2 32x32px tileset by Ivan Voirol for overworld generation. The system is designed to be extensible:

```csharp
// Current tile types in Tile.cs enum
TileType.SlatesGrassBasic    // Overworld grass
TileType.SlatesDirtBasic     // Overworld dirt
TileType.SlatesStoneFloor    // Can be used for cave floors
// ... etc
```

#### Future Underground Tile Types
When implementing The World Below, add these tile types to the `TileType` enum:

```csharp
// Underground cave tiles
TileType.CaveFloorRough,
TileType.CaveFloorSmooth,
TileType.CaveWallStone,
TileType.CaveWallCrystal,
TileType.CaveCeiling,
TileType.Stalactite,
TileType.Stalagmite,

// Dungeon tiles
TileType.DungeonFloorBrick,
TileType.DungeonWallStone,
TileType.DungeonDoor,
TileType.DungeonTrap,
TileType.DungeonChest,

// Special underground features
TileType.UndergroundWater,
TileType.Lava,
TileType.CrystalFormation,
TileType.Mushrooms,
TileType.Ruins
```

### World Generation System

#### Multi-Level Map System

Create a new `UndergroundMap` class that extends or parallels `WorldMap`:

```csharp
public class UndergroundMap
{
    private Tile[,] _tiles;
    private int _width;
    private int _height;
    private int _depth; // Which underground level (1, 2, 3...)
    private SlatesTilesetHelper _slatesTileset;
    private UndergroundTilesetHelper _undergroundTileset; // Future tileset
    
    // Entrance/exit points
    private List<Vector2> _entrances;
    private List<Vector2> _exits;
    
    public UndergroundMap(int depth)
    {
        _depth = depth;
        GenerateCaveSystem();
    }
    
    private void GenerateCaveSystem()
    {
        // Use cellular automata or perlin noise for organic caves
        // Deeper levels = more complex, dangerous layouts
    }
}
```

#### Transition System

Create seamless transitions between overworld and underground:

```csharp
public class MapTransitionSystem
{
    private WorldMap _overworld;
    private Dictionary<int, UndergroundMap> _undergroundLevels;
    
    public void TransitionToUnderground(Vector2 entrancePosition, int targetDepth)
    {
        // Fade out
        // Load underground map
        // Position player at entrance
        // Fade in
    }
    
    public void TransitionToOverworld(Vector2 exitPosition)
    {
        // Fade out
        // Return to overworld
        // Position player at exit
        // Fade in
    }
}
```

### Asset Requirements

#### Tileset Needs
1. **Cave Tileset** (32x32 to match Slates)
   - Natural rock formations
   - Various stone textures
   - Water and lava tiles
   - Crystal formations
   - Organic cave features

2. **Dungeon Tileset** (32x32)
   - Brick and stone walls
   - Floor patterns
   - Doors and gates
   - Traps and mechanisms
   - Decorative elements

3. **Underground Objects**
   - Treasure chests
   - Crystals and ores
   - Mushrooms and underground plants
   - Ancient artifacts
   - Light sources (torches, crystals)

#### Recommended Asset Sources
- **OpenGameArt.org**: Search for "cave tileset", "dungeon tileset"
- **Itch.io**: Many pixel art packs available
- **Custom Creation**: Commission or create tiles matching Slates style

### Lighting System

Underground areas will require a lighting system:

```csharp
public class LightingSystem
{
    private float _ambientLight = 0.2f; // Dark underground
    
    public void AddLightSource(Vector2 position, float radius, Color color)
    {
        // Add dynamic light (torch, crystal, lava)
    }
    
    public float GetLightLevel(Vector2 position)
    {
        // Calculate light at position
        // Used to determine visibility and atmosphere
    }
}
```

### Enemy System Integration

Underground areas will feature unique enemies:

```csharp
public enum EnemyType
{
    // Cave creatures
    CaveBat,
    GiantSpider,
    SlimeMold,
    CaveWorm,
    
    // Dungeon enemies
    SkeletonWarrior,
    GhostlyApparition,
    MimicChest,
    
    // Boss enemies
    CaveTroll,
    AncientGuardian,
    CrystalBehemoth
}
```

## Implementation Phases

### Phase 1: Foundation (2-3 weeks)
- [ ] Add underground tile types to `TileType` enum
- [ ] Create `UndergroundMap` class
- [ ] Implement basic cave generation algorithm
- [ ] Add cave entrance/exit tiles to overworld
- [ ] Create `MapTransitionSystem`

### Phase 2: Core Features (3-4 weeks)
- [ ] Implement lighting system
- [ ] Add underground resource nodes (ores, crystals)
- [ ] Create basic enemy AI for underground creatures
- [ ] Add hazards (lava, water, falling rocks)
- [ ] Implement multiple depth levels

### Phase 3: Dungeons (2-3 weeks)
- [ ] Design and implement structured dungeon layouts
- [ ] Create puzzle mechanics (pressure plates, levers, keys)
- [ ] Add treasure chests and loot system
- [ ] Implement boss encounters

### Phase 4: Polish (2-3 weeks)
- [ ] Add particle effects (dripping water, smoke, sparkles)
- [ ] Implement ambient sounds (echoes, drips, wind)
- [ ] Create minimap system for underground navigation
- [ ] Add lore elements and story integration
- [ ] Secret areas and easter eggs

## Integration with Slates Tileset

The Slates tileset provides an excellent foundation for The World Below:

### Tiles Already Usable
From `SlatesTileMapping.cs`:
- **Stone.Floor** - Can serve as cave floors
- **Stone.Wall** - Can be cave walls
- **Stone.Brick** - Perfect for dungeons
- **Water.Still** - Underground pools
- **Water.Deep** - Underground lakes

### Expansion Strategy
1. Use Slates stone tiles for initial underground implementation
2. Gradually add specialized cave/dungeon tiles as needed
3. Maintain visual consistency by matching Slates' 32x32 format
4. Credit all asset sources appropriately

## Code Integration Points

### Current Codebase Hooks

#### 1. WorldMap.cs
Add underground map reference:
```csharp
public class WorldMap
{
    // Existing code...
    private UndergroundMap _undergroundMap;
    
    public void AddCaveEntrance(int x, int y, int targetDepth)
    {
        // Mark this tile as a cave entrance
        // Link to underground level
    }
}
```

#### 2. GameplayState.cs
Add underground state management:
```csharp
public class GameplayState
{
    // Existing code...
    private bool _isUnderground;
    private UndergroundMap _currentUndergroundMap;
    
    private void HandleMapTransition()
    {
        // Check for entrance/exit tiles
        // Trigger transitions
    }
}
```

#### 3. Tile.cs
Already prepared with extensible tile type system - just add new types as needed.

## Future Considerations

### Save System Integration
Underground progress must be saved:
- Current underground level
- Explored cave areas (fog of war)
- Defeated bosses
- Collected treasures
- Unlocked shortcuts

### Performance Optimization
Large underground areas may require:
- Chunk-based loading
- Culling of off-screen tiles
- Efficient lighting calculations
- Enemy pooling system

### Content Expansion
Possibilities for future updates:
- Multiple underground biomes (ice caves, lava caves, crystal caves)
- Underground villages (dwarf settlements, mushroom folk)
- Ancient civilizations with their own stories
- Rare crops that only grow underground
- Mining mechanics for resource gathering

## Attribution

This design builds on the Slates tileset foundation:
```
Tileset: Slates v.2 [32x32px orthogonal tileset]
Artist: Ivan Voirol
Source: OpenGameArt.org
License: CC-BY 4.0
```

Future underground tilesets will require similar attribution based on their licenses.

---

**Document Status**: Design Phase  
**Target Release**: Post-launch expansion  
**Priority**: Medium (after core overworld features complete)  
**Owner**: MoonBrook Ridge Development Team
