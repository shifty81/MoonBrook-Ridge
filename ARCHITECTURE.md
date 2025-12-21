# MoonBrook Ridge - Architecture Documentation

## Overview

MoonBrook Ridge is built using a modular, component-based architecture leveraging MonoGame's game loop and C#'s object-oriented features.

## Design Patterns

### 1. State Pattern
The game uses a state machine to manage different game states:
- **StateManager**: Central controller for state transitions
- **GameState**: Abstract base class
- **GameplayState**: Main game state
- **MenuState**: For menus (to be implemented)
- **PauseState**: For pause screen (to be implemented)

### 2. Entity-Component System (Lightweight)
While not a full ECS, entities like Player and NPC have component-like behavior:
- Position, velocity, stats
- Animation states
- Rendering components

### 3. Observer Pattern
Used for:
- Dialogue option selection events
- NPC interaction events
- Time system events (day change, season change)

### 4. Singleton-like Services
- TimeSystem: Global time management
- InventorySystem: Per-player inventory
- CraftingSystem: Global recipe database

## Core Systems

### Time System
```
Real Time → Game Time Conversion
- 1 real second ≈ 24 game minutes
- 1 game day ≈ 15 real minutes
- Player can sleep to advance to next day
```

**Flow:**
1. `TimeSystem.Update()` called each frame
2. Converts elapsed real time to game hours
3. Triggers day advance at 2 AM
4. Season changes every 28 days
5. Year advances after Winter

### Dialogue System Architecture

```
DialogueTree (Root Node)
    ├─ DialogueNode (Greeting)
    │   ├─ Option 1 → DialogueNode (Response A)
    │   │   └─ Option 1 → DialogueNode (End)
    │   └─ Option 2 → DialogueNode (Response B)
    │       └─ Option 1 → DialogueNode (End)
    └─ (Player selects via Radial Wheel)
```

**Radial Wheel Interaction:**
1. Player clicks NPC
2. `RadialDialogueWheel.Show()` displays options in circle
3. Mouse hover highlights option
4. Click selects option
5. Tree advances to next node
6. Repeat until conversation ends

### Farming System Flow

```
1. Player equips Hoe
2. Uses hoe on Grass tile
3. Tile changes to Tilled
4. Player equips Seeds
5. Plants seed on Tilled tile
6. Crop object created
7. Each day, crop grows if watered
8. Crop reaches max stage
9. Player harvests with Scythe
10. Crop item added to inventory
11. Tile returns to Tilled state
```

### NPC AI & Schedules

```
NPCSchedule
    6:00 AM → Home (Wake up)
    8:00 AM → Town Square (Walk)
    12:00 PM → Restaurant (Lunch)
    3:00 PM → Park (Socializing)
    7:00 PM → Home (Dinner)
    10:00 PM → Bed (Sleep)
```

**Pathfinding** (To be implemented):
- A* algorithm for navigation
- Collision avoidance
- Animation based on movement direction

## Data Flow

### Game Loop
```
Initialize()
    ↓
LoadContent()
    ↓
┌──────────────────┐
│  Update(gameTime)│ ←─┐
│   ├─ Input       │    │
│   ├─ State Update│    │
│   ├─ Physics     │    │
│   └─ AI          │    │
├──────────────────┤    │
│  Draw(gameTime)  │    │
│   ├─ World       │    │
│   ├─ Entities    │    │
│   └─ UI          │    │
└──────────────────┘    │
         │              │
         └──────────────┘
         (Loop continues)
```

### Player Input → Action
```
Keyboard.GetState()
    ↓
PlayerCharacter.HandleInput()
    ↓
Calculate movement vector
    ↓
Apply velocity
    ↓
Update position
    ↓
Update animation state
    ↓
Camera.Follow(player.Position)
```

### NPC Interaction
```
Player near NPC + Press E
    ↓
Get NPC's current dialogue tree
    ↓
Show RadialDialogueWheel
    ↓
Display options in circle
    ↓
Player selects option (mouse)
    ↓
Advance to next dialogue node
    ↓
Update friendship points if applicable
    ↓
Show ChatBubble with response
    ↓
Continue or end conversation
```

## Memory Management

### Texture Loading
- Load sprites once in LoadContent()
- Store in Content Pipeline
- Reuse throughout game
- Dispose in UnloadContent()

### Object Pooling (Planned)
For frequently created/destroyed objects:
- Particle effects
- Chat bubbles
- Crop growth indicators

## Performance Considerations

### Rendering Optimization
1. **Culling**: Only render tiles/entities in camera view
2. **Batching**: Group draw calls by texture
3. **Sprite Sheets**: Reduce texture switches
4. **Point Sampling**: Use `SamplerState.PointClamp` for pixel art

### Update Optimization
1. **Spatial Partitioning**: Grid-based entity lookup
2. **Update Frequency**: NPCs update less frequently when off-screen
3. **Event-Driven**: Use events instead of polling where possible

## Save System Architecture (Planned)

```json
{
  "player": {
    "position": {"x": 400, "y": 300},
    "health": 100,
    "energy": 85,
    "money": 1500,
    "inventory": [...]
  },
  "world": {
    "season": "Spring",
    "day": 15,
    "year": 1,
    "tiles": [...]
  },
  "npcs": [
    {
      "name": "Emma",
      "friendship": 500,
      "position": {"x": 200, "y": 150}
    }
  ]
}
```

Serialization: JSON format using System.Text.Json

## Extending the Game

### Adding a New Tool
1. Create class inheriting from `Tool`
2. Implement `Use(Vector2 position)` method
3. Add tool to initial inventory
4. Create sprite and animations

### Adding a New Crop
1. Define crop in data file (JSON)
2. Specify growth stages and times
3. Add sprites for each growth stage
4. Add harvest item to item database

### Adding a New NPC
1. Create NPC instance with name and schedule
2. Define dialogue trees
3. Set gift preferences
4. Add sprite and animations
5. Place in world

### Adding a New Recipe
1. Define ingredients and quantities
2. Specify output item
3. Add to CraftingSystem
4. Create UI for recipe display

## Testing Strategy

### Unit Testing
- Item stacking logic
- Recipe validation
- Time calculations
- Pathfinding algorithms

### Integration Testing
- State transitions
- NPC schedules
- Crop growth cycles
- Dialogue tree traversal

### Playtesting
- Balance (energy costs, crop prices)
- Progression pacing
- UI/UX feedback
- Bug discovery

## Future Enhancements

### Multiplayer (Long-term)
- Client-server architecture
- Synchronize world state
- Shared farm or separate farms
- Co-op tasks and events

### Modding Support
- Lua scripting for custom events
- JSON data files for content
- Custom sprite loading
- Plugin system for new features

### Advanced AI
- Personality traits affecting NPC behavior
- Memory system (NPCs remember past interactions)
- Dynamic relationship web (NPCs have opinions of each other)
- Emergent storylines

## Performance Targets

- **60 FPS** on modern desktop hardware
- **< 100ms** frame time during normal gameplay
- **< 1 second** state transitions
- **< 500MB** memory usage
- **< 5 seconds** save/load time

## Code Style Guidelines

- Use C# naming conventions (PascalCase for public, _camelCase for private)
- XML documentation for public APIs
- Keep classes focused (Single Responsibility Principle)
- Prefer composition over inheritance
- Use properties instead of public fields
- Handle null cases gracefully

---

*Last Updated: December 2025*
