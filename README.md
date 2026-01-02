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
- **Building Construction**: Construct and place buildings on your farm ⭐ **NEW**
- **Tool System**: Upgrade tools (hoe, watering can, axe, pickaxe, fishing rod, scythe)
- **Time & Season System**: Dynamic day/night cycle with 4 seasons (28 days each)
- **Weather System**: Dynamic weather with rain, snow, storms, and fog affecting gameplay ⭐ **NEW**
- **Events & Festivals**: 8 seasonal festivals throughout the year ⭐ **NEW**
- **Particle Effects**: Visual feedback for all actions (farming, mining, fishing, etc.) ⭐ **NEW**

### Building System ⭐ **NEW**
- **8 Building Types Available**:
  - **Barn**: Houses livestock (12 animal capacity) - 6,000g + 200 Wood + 100 Stone
  - **Coop**: Houses chickens and ducks (8 bird capacity) - 4,000g + 150 Wood + 50 Stone
  - **Shed**: Storage building (120 inventory slots) - 3,000g + 100 Wood + 50 Stone
  - **Silo**: Stores hay for animals (240 hay capacity) - 1,000g + 50 Wood + 50 Stone + 10 Copper
  - **Well**: Provides water (refills every 2 minutes) - 500g + 75 Stone
  - **Greenhouse**: Year-round crop growth (48 tillable tiles) - 15,000g + 300 Wood + 100 Stone + 50 Iron + 20 Gold
  - **Mill**: Process crops into products - 5,000g + 150 Wood + 100 Stone + 25 Iron
  - **Workshop**: Advanced crafting and ore smelting - 4,000g + 100 Wood + 100 Stone + 50 Iron
- **Placement System**: Visual preview with green/red validation
- **Resource Requirements**: Buildings require gold, wood, stone, and ores
- **Smart Validation**: Only place on valid terrain, prevents overlapping

### Enhanced NPC Interactions ⭐ **IMPLEMENTED**
- **Chat Bubble Conversations**: NPCs communicate through floating chat bubbles ✅
- **Radial Dialogue Wheel**: Sims 4-inspired dialogue selection system ✅
- **Relationship System**: Build friendships with NPCs (10 heart levels) ✅
- **Gift System**: Give gifts to NPCs to increase friendship ✅ **NEW**
- **NPC Schedules**: NPCs follow daily routines and move around the world ✅
- **Branching Dialogues**: Multiple conversation paths based on friendship level ✅
- **Pathfinding**: NPCs automatically navigate to scheduled locations ✅
- **Unique NPCs**: 7 NPCs with distinct personalities and gift preferences ✅ **EXPANDED!**
  - **Emma** (Farmer): Loves crops and flowers
  - **Marcus** (Blacksmith): Loves minerals and ores
  - **Lily** (Merchant): Loves gems and valuable items
  - **Oliver** (Fisherman): Loves fish and seafood
  - **Sarah** (Doctor): Loves medicinal items and herbs ⭐ **NEW!**
  - **Jack** (Carpenter): Loves wood and crafted materials ⭐ **NEW!**
  - **Maya** (Artist): Loves flowers and beautiful things ⭐ **NEW!**

### Events & Festivals System ⭐ **NEW**
- **8 Seasonal Festivals**:
  - **Spring**: Egg Festival (Day 13), Flower Dance (Day 24)
  - **Summer**: Luau (Day 11), Moonlight Jellies (Day 28)
  - **Fall**: Harvest Festival (Day 15), Spirit's Eve (Day 27)
  - **Winter**: Festival of Ice (Day 8), Feast of the Winter Star (Day 25)
- **Event Notifications**: Visual notifications appear when festivals begin
- **Calendar Integration**: Events are tied to specific days and seasons
- **Festival Variety**: Different types including egg hunts, dances, feasts, and viewing events

### Weather System ⭐ **NEW**
- **8 Weather Types**: Clear, Sunny, Cloudy, Rainy, Stormy, Snowy, Windy, Foggy
- **Seasonal Patterns**: Weather probability varies by season
  - Spring: More rainy and cloudy days
  - Summer: Predominantly sunny with occasional storms
  - Fall: Mix of rain, wind, and fog
  - Winter: Frequent snow with foggy conditions
- **Visual Effects**: Weather particles (rain drops, snowflakes) and screen tinting
- **Gameplay Impact**: Weather affects movement speed and natural crop watering
- **Dynamic Duration**: Each weather pattern lasts 30 minutes to 2 hours of game time

### Particle Effects System ⭐ **NEW**
- **Tool Action Effects**:
  - **Hoe**: Dust particles when tilling soil
  - **Watering Can**: Water droplets when watering
  - **Pickaxe**: Rock fragments when mining
  - **Axe**: Wood chips when chopping
  - **Scythe**: Sparkles when harvesting
  - **Fishing Rod**: Water splashes when casting
- **Visual Feedback**: Immediate particle effects for all player actions
- **Performance Optimized**: Particle pooling system for efficient rendering

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
- **Crafting Menu**: Browse and craft items from recipes (K key) ⭐
- **Shop Menu**: Buy and sell items with dynamic pricing (B key) ⭐
- **Building Menu**: Construct and place buildings on your farm (H key) ⭐ **NEW**
- **Gift Menu**: Give gifts to NPCs to build relationships (G key) ⭐ **NEW**
- **Quest Journal**: Track objectives and manage quests (F key) ⭐ **NEW**
- **Achievement Menu**: View all 30 achievements and progress (A key) ⭐ **NEW!**
- **Settings Menu**: Configure audio and game options (O key) ⭐ **NEW!**
- **Event Notifications**: Visual notifications for festivals and special events ⭐ **NEW**
- **Achievement Notifications**: Toast-style notifications for unlocked achievements ⭐ **NEW!**
- **Dialogue System**: Interactive conversation interface with radial wheel

### Achievement System ⭐ **NEW!**
- **30 Achievements** across 8 categories: Farming, Fishing, Mining, Social, Crafting, Wealth, Exploration, Survival
- **Progress Tracking**: Incremental achievements with visible progress bars
- **Category Filtering**: View achievements by category or see all at once
- **Toast Notifications**: Non-intrusive pop-ups when achievements are unlocked
- **Completion Tracking**: See your overall achievement completion percentage

### Audio System ⭐ **NEW!**
- **Full Audio Management**: Complete system for music and sound effects
- **Volume Control**: Separate volume sliders for music and SFX (0-100%)
- **Audio Toggle**: Enable/disable music and sound effects independently
- **22+ Sound Effects**: Predefined sounds for tools, UI, actions, world, and NPCs
- **15+ Music Tracks**: Seasonal music, location themes, and event music
- **Ready for Content**: Audio infrastructure complete, awaiting audio file integration

## 🎨 Art Assets

The game combines assets from multiple high-quality pixel art packs for visual variety:

### Primary Asset Packs
1. **Sunnyside World** ⭐ **MAIN TILESET** - 16x16px tileset and asset pack for characters, buildings, and terrain (4,096 tiles)
2. **16x16 Tilemap Collection** - Ground tiles and terrain (legacy/fallback)
3. **Custom Generated Tileset** - Combined ground tileset (legacy/fallback)
4. **Textured Placeholder Tiles** ⭐ **NEW!** - Procedurally generated textured tiles (grass, dirt, water, etc.) - See [VISUAL_IMPROVEMENTS.md](VISUAL_IMPROVEMENTS.md)

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
See [VISUAL_IMPROVEMENTS.md](VISUAL_IMPROVEMENTS.md) ⭐ **NEW!** for details on textured placeholder tiles.

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

#### Event System ⭐ **NEW**
- `EventSystem`: Manages festivals and special events
- `Festival`: Defines festival types and properties
- `EventNotification`: Displays event announcements
- Seasonal event triggering based on date
- 8 unique festivals across all seasons

#### Weather System ⭐ **NEW**
- `WeatherSystem`: Dynamic weather patterns
- Seasonal weather probability
- Visual weather effects (rain, snow, fog particles)
- Screen tinting based on weather
- Movement speed modifiers
- Natural crop watering during rain

#### Particle System ⭐ **NEW**
- `ParticleSystem`: Manages visual effects
- Particle pooling for performance
- Tool-specific particle effects
- Physics-based particle movement
- Fade and gravity effects

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
| **Attack (Combat)** ⭐ **NEW!** | **Space** |
| Plant Seed / Interact | X |
| Open Menu/Inventory | E or Esc |
| Open Journal/Quests | F |
| **Open Magic Spell Book** ⭐ **NEW!** | **M** |
| **Open Alchemy Lab** ⭐ **NEW!** | **L** |
| **Open Skills Menu** ⭐ **NEW!** | **J** |
| **Open Pet Menu** ⭐ **NEW!** | **P** |
| **Open Crafting Menu** ⭐ | **K** |
| **Open Shop Menu** ⭐ **NEW** | **B** |
| **Open Building Menu** ⭐ **NEW** | **H** |
| **Gift to NPC** ⭐ **NEW** | **G** |
| **Open Achievement Menu** ⭐ **NEW!** | **A** |
| **Open Settings Menu** ⭐ **NEW!** | **O** |
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
- **[WORLDGEN_ASSET_GUIDE.md](WORLDGEN_ASSET_GUIDE.md)** ⭐ **NEW!** - Asset-driven world generation configuration guide
- **[WORLDGEN_INTEGRATION_EXAMPLE.md](WORLDGEN_INTEGRATION_EXAMPLE.md)** ⭐ **NEW!** - How to integrate JSON world configs
- **[TILE_SIZE_GUIDE.md](TILE_SIZE_GUIDE.md)** - Why 16×16 is the ideal tile size for the game
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

### Phase 4: Advanced Features ✅ **COMPLETE!**
- [x] **Crafting UI and recipes** ⭐ **IMPLEMENTED**
- [x] **Shop system** ⭐ **IMPLEMENTED**
- [x] **Mining system with caves** ⭐ **IMPLEMENTED**
- [x] **Fishing minigame** ⭐ **IMPLEMENTED**
- [x] **Quest/task system** ⭐ **IMPLEMENTED** - 5 starter quests available
- [x] **Building construction** ⭐ **IMPLEMENTED** - 8 building types available
- [x] **Events and festivals** ⭐ **IMPLEMENTED** - 8 seasonal festivals with notifications

### Phase 5: Polish & Content ✅ **COMPLETE!**
- [x] **Audio system** ⭐ **IMPLEMENTED** - Complete audio management for music and SFX (ready for audio files)
- [x] **Particle effects** ⭐ **IMPLEMENTED** - Visual effects for all tool actions
- [x] **Weather effects** ⭐ **IMPLEMENTED** - Dynamic weather system with seasonal patterns
- [x] **More crops, items, and recipes** ⭐ **IMPLEMENTED** - 7 new crops, 17 new recipes, 12 new food items
- [x] **Multiple NPCs with unique personalities** ⭐ **IMPLEMENTED** - 7 total NPCs (Emma, Marcus, Lily, Oliver, Sarah, Jack, Maya)
- [x] **Achievements** ⭐ **IMPLEMENTED** - 30 achievements across 8 categories with notification system
- [ ] Marriage and family system (deferred to future phase)

### Phase 6: Advanced Game Systems ✅ **COMPLETE!**
- [x] **Magic System** ⭐ **IMPLEMENTED** - 8 spells, mana resource, spell casting mechanics
- [x] **Alchemy System** ⭐ **IMPLEMENTED** - 10 potion recipes with ingredient-based brewing
- [x] **Skill Tree System** ⭐ **IMPLEMENTED** - 6 skill categories, 30+ skills, XP and leveling
- [x] **Combat System** ⭐ **IMPLEMENTED** - 12 weapons, 16 enemy types, boss battles, loot system
- [x] **Pet/Companion System** ⭐ **IMPLEMENTED** - 10 pet types with taming, abilities, and management
- [x] **Dungeon System** ⭐ **IMPLEMENTED** - Procedural generation, multi-floor dungeons, 8 dungeon types
- [x] **Biome System** ⭐ **IMPLEMENTED** - 12 unique biomes with resources and creatures
- [x] UI integration for new systems (magic, alchemy, skills, pets)
- [x] Combat integration into game loop ⭐ **INTEGRATED** - Enemies spawn in mines, real-time combat with Space key
- [ ] Dungeon integration (separate from mines with themed enemies)
- [ ] Advanced quest system with moral choices and branching
- [ ] Faction system

See [PHASE_6_COMPLETION_SUMMARY.md](PHASE_6_COMPLETION_SUMMARY.md) for complete Phase 6 documentation.

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
