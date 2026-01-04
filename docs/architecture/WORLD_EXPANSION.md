# World Expansion System

## Overview
MoonBrook Ridge features a massively expanded world that is **5X larger** than the original design, with room for future expansion through upgrades and purchasable plots.

## World Size

### Current Dimensions
- **World Size**: 250x250 tiles (62,500 total tiles)
- **Previous Size**: 50x50 tiles (2,500 total tiles)
- **Expansion Factor**: 5X larger (25X more tiles!)
- **Tile Size**: 16x16 pixels per tile
- **Actual Size**: 4,000 x 4,000 pixels

### World Structure

#### Central Farm (Starting Area)
- **Size**: 100x100 tiles (10,000 tiles)
- **Location**: Centered at world coordinates (125, 125)
- **Terrain**: Flat, farmable grassland
- **Purpose**: Player's initial farming and building area
- **Features**: 
  - Mostly grass tiles for farming
  - Clear, obstacle-free space
  - Ideal for crop plots and buildings

#### Near-Farm Expansion Zone
- **Size**: ~50 tile radius beyond central farm
- **Distance**: 75-150 tiles from world center
- **Terrain**: Mixed grass with some variety
- **Purpose**: Early game expansion areas
- **Features**:
  - Slightly more varied terrain
  - Still very farmable
  - Room for animal pens, barns, etc.

#### Outer Wilderness
- **Size**: Remaining ~75 tiles to world edges
- **Distance**: Beyond 150 tiles from center
- **Terrain**: Varied grassland with features
- **Purpose**: Future expansion via plot purchases
- **Features**:
  - Natural water features (ponds)
  - Diverse terrain types
  - Dungeon entrances
  - Village locations
  - Mine entrance

## Player Spawn Location

### Spawn Coordinates
- **Tile Coordinates**: (125, 125)
- **Pixel Coordinates**: (2000, 2000)
- **Relative Position**: Dead center of 250x250 world

### Why Center Spawn?
1. **Equal Access**: Player starts with equal distance to all directions
2. **Symmetric Expansion**: Can expand farm evenly in all directions
3. **Natural Progression**: Explore outward from safe center
4. **Clear Starting Point**: No confusion about "where's my farm?"

## Points of Interest

### Updated Locations (250x250 World)

#### Home and Villages
| Location | Coordinates | Type | Distance from Home |
|----------|-------------|------|-------------------|
| Home Farm | (125, 125) | Farm (unlocked) | 0 tiles |
| MoonBrook Village | (75, 75) | Village | ~71 tiles NW |
| Shopping District | (125, 50) | Shop District | 75 tiles N |

#### Resource Locations
| Location | Coordinates | Type | Distance from Home |
|----------|-------------|------|-------------------|
| Mine Entrance | (50, 200) | Mine | ~97 tiles SW |
| Northeast Pond | (195, 45) | Water Feature | ~114 tiles NE |

#### Dungeons (Outer Areas)
| Dungeon | Coordinates | Direction | Distance from Home |
|---------|-------------|-----------|-------------------|
| Slime Cave | (195, 55) | NE | ~100 tiles |
| Skeleton Crypt | (35, 35) | NW | ~127 tiles |
| Spider Nest | (25, 125) | W | 100 tiles |
| Goblin Warrens | (45, 205) | SW | ~113 tiles |
| Haunted Manor | (125, 25) | N | 100 tiles |
| Dragon Lair | (225, 125) | E | 100 tiles |

## Terrain Generation

### Terrain Types
1. **Grass** (60% of central farm)
   - Primary farming surface
   - Most common tile type
   - Flat, easy to work

2. **Grass Variants** (01, 02, 03)
   - Visual variety
   - Same functionality as base grass
   - Distributed for natural look

3. **Water Features**
   - Ponds in outer areas
   - Not in central farm
   - Decorative and resource

4. **Sand**
   - Around water features
   - Beach areas
   - Transition zones

### Generation Algorithm
```
For each tile in 250x250 world:
  Calculate distance from center (125, 125)
  
  If distance <= 50 (Central Farm):
    Generate flat farmable grass (60/30/10 distribution)
  
  Else if distance <= 75 (Near Farm):
    Generate varied grass (more diversity)
  
  Else (Outer Wilderness):
    Generate diverse terrain with features
```

## Future Expansion System

### Plot Purchase System (Planned)
Players will be able to expand their usable land through purchases:

#### Expansion Tiers
1. **Tier 1: Near Farm** (Already Accessible)
   - 0-75 tiles from center
   - Free with initial game
   - 17,500 tiles available

2. **Tier 2: Mid Range** (Purchasable)
   - 75-100 tiles from center
   - Cost: TBD (gold or resources)
   - Unlocks: ~13,000 additional tiles

3. **Tier 3: Far Range** (Purchasable)
   - 100-125 tiles from center
   - Higher cost
   - Unlocks: ~15,000 additional tiles

4. **Tier 4: Wilderness** (Purchasable)
   - 125+ tiles from center
   - Premium cost
   - Unlocks: ~17,000 tiles
   - Includes special resource areas

### Skill-Based Unlocks (Planned)
Certain skills could unlock expansion options:
- **Farming Level 5**: Unlock fertile plot (5x5 tiles)
- **Mining Level 5**: Access to ore-rich areas
- **Building Level 3**: Unlock construction in expansion zones
- **Exploration Level 7**: Discover hidden plots

## Hexagonal Sector System (Planned)

### Sector Organization
The world will be divided into hexagonal sectors for:
- Easy navigation
- Clear boundaries for plot purchases
- Visual organization on map
- Exploration tracking

### Sector Features
- **Size**: ~30-40 tiles per hexagon
- **Total Sectors**: Approximately 60-80 hexagons
- **Display**: On map menu as overlay
- **Interaction**: Click to zoom, view details
- **States**: Unexplored, Explored, Owned, Purchasable

### Sector Map UI
```
Map Menu > World Map Tab
┌─────────────────────────┐
│   Hexagonal Sector Map  │
│                         │
│    🔷 🔷 🔷 🔷 🔷      │
│   🔷 🔶 🔶 🔶 🔷 🔷    │
│  🔷 🔶 🟢 🟢 🔶 🔷 🔷  │  
│   🔷 🔶 🔶 🔶 🔷 🔷    │
│    🔷 🔷 🔷 🔷 🔷      │
│                         │
│ 🟢 = Owned/Home         │
│ 🔶 = Explored           │
│ 🔷 = Unexplored         │
└─────────────────────────┘
```

## Performance Considerations

### Optimizations for Large World

#### 1. Frustum Culling
- Only renders tiles visible on screen
- Adds 2-tile buffer to prevent pop-in
- Dramatically reduces rendering load

#### 2. Spatial Partitioning
- World divided into chunks
- Only active chunks are updated
- Inactive chunks are dormant

#### 3. Entity Culling
- Trees, rocks, NPCs only drawn when near player
- 32-pixel buffer beyond viewport
- Sorted by Y-position for proper depth

#### 4. Lazy Loading
- Tiles generated on-demand (future)
- Unused sectors can be unloaded
- Memory-efficient for large worlds

### Expected Performance
- **Target FPS**: 60 FPS
- **Tile Draw Calls**: ~500-1000 per frame (viewport only)
- **Object Draw Calls**: ~20-100 per frame
- **Memory Usage**: ~50-100MB for world data

## Exploration & Discovery

### Waypoint System
As players explore the 5X larger world:
- **Auto-Discovery**: Waypoints unlock when player gets close (~3 tiles)
- **Fast Travel**: Unlocked waypoints can be traveled to via Map Menu
- **Cost**: Gold + Time (varies by distance)
- **Strategy**: Explore to unlock, fast travel for convenience

### Recommended Exploration Path
1. **Start**: Center farm (125, 125)
2. **First Exploration**: Village (75, 75) - 10 min walk
3. **Resource Gathering**: Mine (50, 200) - 15 min walk
4. **Shopping**: Shopping District (125, 50) - 12 min walk
5. **Adventure**: Dungeons in outer ring - 15-20 min walk each

### Travel Times
Without fast travel:
- **Across Full World**: ~8-10 minutes real time
- **Center to Edge**: ~4-5 minutes real time
- **Between Opposite Edges**: ~16-20 minutes real time

With fast travel:
- **Any Unlocked Location**: Instant (costs gold + 1 game hour)

## Comparison: Old vs New

### Old World (50x50)
- Total Tiles: 2,500
- Walk Across: ~2 minutes
- Farm Area: 15x15 (225 tiles)
- Expansion Potential: Very limited

### New World (250x250)
- Total Tiles: 62,500 (25X more!)
- Walk Across: ~10 minutes
- Farm Area: 100x100 (10,000 tiles)
- Expansion Potential: Massive!

### Benefits of 5X Expansion
1. ✅ **More Building Space**: 44X more central farm area
2. ✅ **Natural Progression**: Room to grow and expand
3. ✅ **Exploration**: Actually feels like a world to explore
4. ✅ **Endgame Content**: Plenty of room for late-game features
5. ✅ **Community**: Multiple villages/towns can exist
6. ✅ **Replayability**: Different build strategies in different areas

## Tips for Players

### Early Game
- Focus on central 50x50 area (like old world size)
- Establish farm in safe center area
- Explore nearby villages for quests and shops

### Mid Game
- Unlock waypoints for fast travel
- Start expanding beyond initial farm
- Explore dungeon entrances in outer areas

### Late Game
- Purchase expansion plots
- Build mega-farms in expansion zones
- Complete all dungeon challenges
- Unlock entire sector map

## Technical Details

### World Coordinates
- **Origin**: (0, 0) at top-left
- **Center**: (125, 125)
- **Maximum**: (249, 249) at bottom-right

### Distance Calculations
```csharp
// Distance from center
float distanceFromCenter = Vector2.Distance(
    new Vector2(125, 125), 
    new Vector2(tileX, tileY)
);

// In circular zones (farmRadius = 50)
bool isCentralFarm = distanceFromCenter <= 50;
bool isNearFarm = distanceFromCenter <= 75;
bool isWilderness = distanceFromCenter > 75;
```

### Memory Footprint
- **Tile Array**: 250 x 250 = 62,500 tiles
- **Per Tile**: ~32 bytes (type, position, crop data)
- **Total**: ~2 MB for base tiles
- **With Objects**: +10-20 MB (trees, rocks, buildings)
- **Grand Total**: ~12-22 MB for entire world

## Future Vision

The 5X world expansion sets the foundation for:
- **Multiple biomes** in different sectors
- **Player-owned territory** through plot system
- **Community features** with other players' farms visible
- **Seasonal events** in specific world zones
- **Dynamic world** with weather and time-of-day effects
- **Procedural content** in outer wilderness areas

---

*For navigation and fast travel information, see [UI_ORGANIZATION.md](UI_ORGANIZATION.md)*
*For complete controls, see [CONTROLS.md](CONTROLS.md)*
