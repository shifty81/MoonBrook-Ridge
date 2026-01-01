# MoonBrook Ridge 🌾

A farming and life simulation game inspired by Stardew Valley with enhanced NPC interaction systems similar to The Sims 4, **plus survival mechanics** (hunger and thirst). Built with MonoGame (C#).

## 🎮 Game Features

### Survival Mechanics ⭐ NEW
- **Hunger System**: Depletes over time, faster during strenuous activities
- **Thirst System**: Depletes faster than hunger, critical for survival
- **Activity-Based Decay**: Running, mining, and tool use drain stats faster
- **Debuff System**: Low hunger reduces movement speed; low thirst drains extra energy
- **Critical State**: When hunger/thirst reach zero, health starts draining
- **Blackout Mechanic**: Player blacks out if health drops to zero from starvation/dehydration
- **Visual Warnings**: HUD shows critical warnings when stats are dangerously low
- **Consumables**: Food items restore hunger, drinks restore thirst

### Core Gameplay
- **Farming System**: Plant, water, and harvest crops with seasonal variations
- **Mining & Fishing**: Explore caves for resources and catch various fish species
- **Crafting System**: Create items from collected resources using recipes ⭐ **NEW UI**
- **Shop System**: Buy seeds, food, and materials; sell your harvested goods ⭐ **NEW**
- **Tool System**: Upgrade tools (hoe, watering can, axe, pickaxe, fishing rod, scythe)
- **Time & Season System**: Dynamic day/night cycle with 4 seasons (28 days each)
- **Weather System**: Different weather patterns affecting gameplay

### Enhanced NPC Interactions ⭐ **IMPLEMENTED**
- **Chat Bubble Conversations**: NPCs communicate through floating chat bubbles ✅
- **Radial Dialogue Wheel**: Sims 4-inspired dialogue selection system ✅
- **Relationship System**: Build friendships with NPCs (10 heart levels) ✅
- **Gift System**: Give gifts to NPCs to increase friendship ✅ **NEW**
- **NPC Schedules**: NPCs follow daily routines and move around the world ✅
- **Branching Dialogues**: Multiple conversation paths based on friendship level ✅
- **Pathfinding**: NPCs automatically navigate to scheduled locations ✅
- **Unique NPCs**: 4 NPCs with distinct personalities and gift preferences ✅ **NEW**
  - **Emma** (Farmer): Loves crops and flowers
  - **Marcus** (Blacksmith): Loves minerals and ores
  - **Lily** (Merchant): Loves gems and valuable items
  - **Oliver** (Fisherman): Loves fish and seafood

### Character Systems
- **Player Character**: Fully customizable with stats (health, energy, money)
- **Animation System**: Multiple animation states (idle, walking, running, using tools)
- **Movement**: WASD or arrow keys for movement, Shift to run
- **Inventory System**: 36-slot inventory with item stacking

### World & Environment
- **Tile-Based Map**: 50x50 grid world with multiple tile types
- **Dynamic Camera**: Smooth camera following with zoom support
- **Building System**: Construct buildings and place furniture

### User Interface ⭐ **ENHANCED**
- **HUD**: Displays health, energy, time, date, season, and money
- **Inventory Menu**: Manage items and tools
- **Crafting Menu**: Browse and craft items from recipes (K key) ⭐ **NEW**
- **Shop Menu**: Buy and sell items with dynamic pricing (B key) ⭐ **NEW**
- **Dialogue System**: Interactive conversation interface with radial wheel

## 🎨 Art Assets

The game combines assets from multiple high-quality pixel art packs for visual variety:

### Primary Asset Packs
1. **Sunnyside World** ⭐ **MAIN TILESET** - 16x16px tileset and asset pack for characters, buildings, and terrain (4,096 tiles)
2. **16x16 Tilemap Collection** - Ground tiles and terrain (legacy/fallback)
3. **Custom Generated Tileset** - Combined ground tileset (legacy/fallback)

### Asset Categories
- **Terrain**: Sunnyside World tileset with grass, dirt, stone, water, sand, and varied tiles
- **Characters**: Sunnyside World sprites with multiple hairstyles and animations
- **Buildings**: Detailed structures from Sunnyside World
- **Crops**: Farming items with growth stages
- **Resources**: Trees, rocks, and harvestable objects
- **Particle Effects**: Visual feedback and animations
- **UI Elements**: Icons, buttons, and interface graphics

### Asset Integration ⭐ SUNNYSIDE WORLD ACTIVE

Assets are now extensively integrated through MonoGame's Content Pipeline:

**Currently Loaded (~200+ files):**
- ✅ **Sunnyside World Tileset (4,096 tiles)** - Primary tileset for world generation ⭐ **ACTIVE**
- ✅ Arial font for UI text
- ✅ **ALL 20 character animation sprites** (walk, run, idle, dig, mine, axe, fish, water, attack, etc.)
- ✅ **ALL 20 tool overlay sprites** (for layered rendering showing tools in use)
- ✅ **Custom ground tileset (192 16x16 tiles)** combining multiple packs (fallback):
  - 16 grass variants, 16 dirt/path variants, 16 tilled soil variants
  - 16 stone/rock variants, 16 water variants, 16 sand/beach variants
  - 96 additional terrain variants
- ✅ **Individual tile textures** (grass, dirt, stone, water, sand, tilled soil)
- ✅ **Structural elements** (fences, floors, walls, doors, decorations)
- ✅ **11 crop types** with full growth stages (wheat, potato, carrot, cabbage, pumpkin, sunflower, beetroot, cauliflower, kale, parsnip, radish)
- ✅ **20+ building sprites** (houses, towers, castles, barracks, monasteries, archery ranges)
- ✅ **Resource sprites** (4 tree types, 3 rock types)

**Sunnyside World Integration Status:**
- ✅ Tileset loaded and rendering system integrated
- ✅ World generation using Sunnyside World tiles by default
- ✅ Legacy tile types mapped to Sunnyside tile IDs
- ✅ 16x16 tile size matches game grid perfectly
- ✅ Extensible system ready for future expansion

**Archived Assets:**
- Slates Tileset v.2 integration has been archived. See [archived/README.md](archived/README.md) for details.

See [ASSET_STATUS_SUMMARY.md](ASSET_STATUS_SUMMARY.md) for complete current status.  
See [TILESET_GUIDE.md](TILESET_GUIDE.md) for details on the custom ground tileset.  
See [ASSET_LOADING_GUIDE.md](ASSET_LOADING_GUIDE.md) for details on how to add more assets.  
See [ASSET_WORK_STATUS.md](ASSET_WORK_STATUS.md) for comprehensive status of what's loaded and what's available.

### Sprite Categories
```
sprites/
├── Buildings/              # Farm buildings and structures
├── Characters/             # NPC sprites
├── Crops/                  # Crop growth stages
├── Decorations/            # Decorative items
├── Particle FX/            # Visual effects
├── Resources/              # Harvestable resources
├── Tilesets/              # Ground tiles and terrain
├── Units/                  # Character units
└── SUNNYSIDE_WORLD_*/     # Full asset packs
```

## 🏗️ Technical Architecture

### Project Structure
```
MoonBrookRidge/
├── Core/
│   ├── Components/         # Reusable game components
│   ├── Entities/          # Base entity classes
│   ├── States/            # Game state management
│   └── Systems/           # Core game systems (Time, Camera)
├── Characters/
│   ├── Player/            # Player character
│   └── NPCs/              # NPC system and dialogue
├── World/
│   ├── Maps/              # World map system
│   └── Tiles/             # Tile and crop systems
├── Farming/
│   ├── Crops/             # Crop definitions
│   └── Tools/             # Farming tools
├── Items/
│   ├── Inventory/         # Inventory system
│   └── Crafting/          # Crafting recipes
├── UI/
│   ├── HUD/               # Heads-up display
│   ├── Menus/             # Game menus
│   └── Dialogue/          # Dialogue UI (chat bubbles, radial wheel)
└── Content/               # Game assets (sprites, fonts, sounds)
```

### Key Systems

#### State Management
- `StateManager`: Handles game state transitions
- `GameState`: Abstract base class for all game states
- `GameplayState`: Main game loop state

#### Time System
- Real-time to game-time conversion
- Season progression (Spring → Summer → Fall → Winter)
- Day/night cycle
- 28 days per season

#### Camera System
- 2D camera with smooth following
- Configurable zoom levels (0.5x to 4x)
- Perfect for pixel art rendering

#### Dialogue System
- `DialogueTree`: Branching conversation system
- `DialogueNode`: Individual dialogue entries with options
- `RadialDialogueWheel`: Interactive circular menu for dialogue choices
- `ChatBubble`: Floating speech bubbles above characters

#### Inventory & Items
- Stack-based inventory (items stack up to 99)
- Multiple item types (tools, seeds, crops, fish, minerals, etc.)
- Buy/sell pricing system

#### Crafting
- Recipe-based crafting
- Ingredient checking
- Automatic resource consumption

## 🚀 Getting Started

### Prerequisites
- [.NET 9.0 SDK](https://dotnet.microsoft.com/download/dotnet/9.0) or later
- MonoGame 3.8.4 or later
- **Fonts**: The Liberation Sans font is now bundled with the project - no installation required!

### Quick Play (Recommended)

Use the convenient play script:
```bash
git clone https://github.com/shifty81/MoonBrook-Ridge.git
cd MoonBrook-Ridge
./play.sh
```

### Manual Building

1. **Clone the repository**
   ```bash
   git clone https://github.com/shifty81/MoonBrook-Ridge.git
   cd MoonBrook-Ridge
   ```

2. **Restore dependencies**
   ```bash
   cd MoonBrookRidge
   dotnet restore
   ```

3. **Build the project**
   ```bash
   dotnet build
   ```

4. **Run the game**
   ```bash
   dotnet run
   ```

### Playtest Guide

**New!** See [PLAYTEST_GUIDE.md](PLAYTEST_GUIDE.md) for:
- How to test the complete farming loop (till → plant → water → harvest)
- Currently implemented features and controls
- Tips for effective playtesting
- Known limitations and troubleshooting

### Development

The project uses MonoGame's Content Pipeline for asset management. To add new sprites:

1. Place sprite files in the `Content/` directory
2. Add them to `Content.mgcb` using the MGCB Editor
3. Load them in code using `Content.Load<Texture2D>("filename")`

## 🎮 Controls

| Action | Key(s) |
|--------|--------|
| Move Up | W or ↑ |
| Move Down | S or ↓ |
| Move Left | A or ← |
| Move Right | D or → |
| Run | Hold Shift |
| Use Tool | C |
| Plant Seed / Interact | X |
| Open Menu/Inventory | E or Esc |
| Open Journal/Quests | F |
| Open Map | M |
| **Open Crafting Menu** ⭐ **NEW** | **K** |
| **Open Shop Menu** ⭐ **NEW** | **B** |
| **Gift to NPC** ⭐ **NEW** | **G** |
| Switch Tool | Tab |
| Hotbar Slots (Consume) | 1-9, 0, -, = |
| **Quick Save** ⭐ | **F5** |
| **Quick Load** ⭐ | **F9** |

See [CONTROLS.md](CONTROLS.md) for complete control documentation.

## 📚 Documentation

Comprehensive guides and references:

### For Players & Testers
- **[PLAYTEST_GUIDE.md](PLAYTEST_GUIDE.md)** ⭐ **NEW!** - How to playtest the game and test features
- **[DEV_SETUP.md](DEV_SETUP.md)** ⭐ **NEW!** - Development environment and debugging setup
- **[CONTROLS.md](CONTROLS.md)** - Complete control reference

### Game Development
- **[TILE_SIZE_GUIDE.md](TILE_SIZE_GUIDE.md)** ⭐ **NEW!** - Why 16×16 is the ideal tile size for the game
- **[ASSET_WORK_STATUS.md](ASSET_WORK_STATUS.md)** - What asset work is complete and what's still needed
- **[ASSET_LOADING_GUIDE.md](ASSET_LOADING_GUIDE.md)** - How to add new sprites through Content Pipeline
- **[TILESET_GUIDE.md](TILESET_GUIDE.md)** - Guide to using the custom ground tileset
- **[SPRITE_GUIDE.md](SPRITE_GUIDE.md)** - Guide to using Sunnyside World assets
- **[ARCHITECTURE.md](ARCHITECTURE.md)** - Technical architecture and design patterns
- **[DEVELOPMENT.md](DEVELOPMENT.md)** - Development guide and best practices
- **[IMPLEMENTATION_SUMMARY.md](IMPLEMENTATION_SUMMARY.md)** - Overall project status and features

### Contributing & Maintenance
- **[CONTRIBUTING.md](CONTRIBUTING.md)** - Guidelines for contributing to the project
- **[COPILOT_ERROR_INVESTIGATION.md](COPILOT_ERROR_INVESTIGATION.md)** - Investigation of PR #7 Copilot errors and lessons learned

## 🗺️ Roadmap

### Phase 1: Core Foundation ✅
- [x] MonoGame project setup
- [x] State management system
- [x] Player character with movement
- [x] Camera system
- [x] Time and season system
- [x] Basic HUD
- [x] **Hunger and Thirst mechanics** ⭐
- [x] **Input management system with configurable keybinds** ⭐
- [x] **Animation controller with state machine** ⭐
- [x] **Z-ordering rendering system** ⭐
- [x] **PlayerStats system with survival mechanics** ⭐
- [x] **Consumable items (food and drinks)** ⭐
- [x] **Pause menu functionality** ⭐

### Phase 2: World & Farming ✅
- [x] Load and render Sunnyside World sprites ⭐
- [x] Add fonts for text rendering ⭐
- [x] Tile-based world rendering with actual sprites ⭐
- [x] Integrate character animations with movement ⭐
- [x] Farming mechanics (planting, watering, harvesting) ⭐
- [x] Tool usage system ⭐
- [x] Crop growth with time system ⭐
- [x] Save/load system (basic) ⭐

### Phase 3: NPC & Social ✅ **COMPLETE!**
- [x] **NPC spawning and movement** ⭐
- [x] **Chat bubble system implementation** ⭐
- [x] **Radial dialogue wheel with mouse interaction** ⭐
- [x] **Dialogue content and branching paths** ⭐
- [x] **NPC schedules and pathfinding** ⭐
- [x] **Gift-giving mechanics with UI** ⭐ **IMPLEMENTED**
- [x] **Multiple NPCs with personalities** ⭐ **IMPLEMENTED** (Emma, Marcus, Lily, Oliver)

### Phase 4: Advanced Features 🚧 **IN PROGRESS**
- [x] **Crafting UI and recipes** ⭐ **IMPLEMENTED**
- [x] **Shop system** ⭐ **IMPLEMENTED**
- [x] **Mining system with caves** ⭐ **IMPLEMENTED**
- [x] **Fishing minigame** ⭐ **IMPLEMENTED**
- [ ] Building construction
- [ ] Quest/task system
- [ ] Events and festivals

### Phase 5: Polish & Content 📋
- [ ] Sound effects and music
- [ ] Particle effects
- [ ] Weather effects
- [ ] More crops, items, and recipes
- [ ] Multiple NPCs with unique personalities
- [ ] Marriage and family system
- [ ] Achievements

## 🤝 Contributing

We welcome contributions to MoonBrook Ridge! Please read [CONTRIBUTING.md](CONTRIBUTING.md) for guidelines on:
- Pull request best practices
- Code and asset contribution guidelines
- How to handle large asset additions
- Branch naming conventions
- Commit message format

Feel free to:
- Open issues for bugs or feature requests
- Submit pull requests for improvements (following our guidelines)
- Share feedback on game mechanics

## 📝 License

This project is for educational and personal use. 

**Sprite Assets**: 
- The Sunnyside World asset pack has its own license
- The Slates tileset is licensed under CC-BY 4.0 (attribution required)

Please ensure you have proper rights to use these assets.

## 🙏 Credits

### Game Framework & Tools
- **Game Framework**: [MonoGame](https://www.monogame.net/)
- **Development**: C# with .NET 9.0

### Art Assets
- **Slates Tileset v.2**: [Ivan Voirol](https://opengameart.org/users/ivan-voirol) (CC-BY 4.0)
  - 32x32px orthogonal tileset (1,288 tiles)
  - Source: [OpenGameArt.org](https://opengameart.org/content/slates-32x32px-orthogonal-tileset-by-ivan-voirol)
- **Sunnyside World**: Sprite collection for characters and objects
- **Custom Tilesets**: Combined and enhanced terrain tiles

### Inspiration
- **Stardew Valley**: Core farming and life simulation mechanics
- **The Sims 4**: Enhanced NPC interaction systems
- **Harvest Moon**: Classic farming game inspiration

## 📧 Contact

Project maintained by [shifty81](https://github.com/shifty81)

---

**Note**: This project is in active development. Features and systems are subject to change.
