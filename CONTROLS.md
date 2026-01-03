# Controls & Keybinds Reference

## Default Controls

### Movement
| Action | Primary Key | Alternative Key |
|--------|------------|-----------------|
| Move Up | W | ↑ (Up Arrow) |
| Move Down | S | ↓ (Down Arrow) |
| Move Left | A | ← (Left Arrow) |
| Move Right | D | → (Right Arrow) |
| Run/Sprint | Left Shift (Hold) | Right Shift (Hold) |

**Running Mechanics:**
- Running drains energy (2 energy/second)
- Cannot run when energy is below 1
- Running accelerates hunger and thirst decay (2x multiplier)
- Movement speed: Walk = 120 units/sec, Run = 200 units/sec
- Low hunger reduces all movement speed by 30%

### Actions
| Action | Primary Key | Alternative Key |
|--------|------------|-----------------|
| Use Tool / Place Item | C | Left Mouse Button |
| Interact / Talk | X | Right Mouse Button |
| **Throw Grenade** ⭐ **AUTO-COMBAT** | **Space** | - |
| Open Menu / Inventory | E | Escape |
| Open Journal / Quests | F | - |
| **Open Map Menu** ⭐ **UPDATED** | **M** | - |
| **Open Crafting Menu** ⭐ | **K** | - |
| **Open Shop Menu** ⭐ | **B** | - |
| **Gift to NPC** ⭐ | **G** | - |
| **Toggle Auto-Fire** ⭐ **PHASE 8** | **N** | - |
| **Sort Inventory** ⭐ **PHASE 8** | **I** | - |
| **Switch Weapon Loadout** ⭐ **AUTO-COMBAT** | **Tab** | - |
| **Quick Save** ⭐ | **F5** | - |
| **Quick Load** ⭐ | **F9** | - |

**Note on Combat**: Your weapons fire automatically when in caves (auto-fire enabled by default). Press N to toggle auto-fire on/off. Press Space to manually throw grenades for AoE damage/effects.

**Note on Map Menu**: Press M to open the Map Menu, which contains tabs for:
- **World Map**: View the minimap and world overview
- **Waypoints**: See all discovered locations
- **Fast Travel**: Travel to discovered waypoints (costs gold and advances time)

**IMPORTANT**: WASD keys are RESERVED for movement only and cannot be used for other actions.

### Hotbar (Quick Access)
| Slot | Key |
|------|-----|
| Slot 1 | 1 |
| Slot 2 | 2 |
| Slot 3 | 3 |
| Slot 4 | 4 |
| Slot 5 | 5 |
| Slot 6 | 6 |
| Slot 7 | 7 |
| Slot 8 | 8 |
| Slot 9 | 9 |
| Slot 10 | 0 |
| Slot 11 | - (Minus) |
| Slot 12 | = (Equals) |

## Game Mechanics

### Survival Stats

#### Hunger
- **Decay Rate**: 0.05% per second (3% per minute)
- **Activity Multipliers**:
  - Idle: 0.5x (1.5% per minute)
  - Walking: 1.0x (3% per minute)
  - Running: 2.0x (6% per minute)
  - Using Tools: 1.5x (4.5% per minute)
  - Mining: 1.8x (5.4% per minute)
  - Fishing: 0.75x (2.25% per minute)

#### Thirst
- **Decay Rate**: 0.08% per second (4.8% per minute)
- **Activity Multipliers**: Same as hunger
- **Note**: Thirst depletes faster than hunger

#### Consequences
- **Low Stat (< 20%)**:
  - Hunger: -30% movement speed
  - Thirst: -2 energy drain per second
- **Critical (< 5%)**:
  - Visual warning appears
  - More severe debuffs
- **Zero**:
  - Health drain: 0.5 HP/sec (hunger), 0.75 HP/sec (thirst)
  - Player will blackout if health reaches zero

#### Blackout
When player blacks out from starvation/dehydration:
- Teleported to bed
- Health restored to 50%
- Energy restored to 25%
- Hunger restored to 50%
- Thirst restored to 50%
- Lose 10% of money
- Time advanced to next day

### Energy Management
- **Max Energy**: 100
- **Running**: -2 energy/second
- **Tool Use**: -2 energy × efficiency multiplier
- **Low Thirst Penalty**: -2 energy/second when thirst < 20%
- **Recovery**: Sleep fully restores energy

### Health System
- **Max Health**: 100
- **Damage Sources**:
  - Starvation (0 hunger)
  - Dehydration (0 thirst)
  - Combat (planned)
  - Environmental hazards (planned)

### Time System
- **In-Game Time**: 1 real second ≈ 24 game minutes
- **Full Day**: ~15 real minutes
- **Day Start**: 6:00 AM
- **Exhaustion Time**: 2:00 AM (26:00)
- **Forced Sleep**: If awake past 2 AM with < 10 energy
- **Season Length**: 28 days
- **Year Progression**: 4 seasons per year

## Consumables

### Food Items
Restores hunger and sometimes energy/health:

| Item | Hunger | Energy | Health | Price |
|------|--------|--------|--------|-------|
| Wild Berry | +10% | +3 | - | $15 |
| Carrot | +15% | +5 | - | $25 |
| Potato | +20% | +8 | - | $30 |
| Corn | +25% | +10 | - | $35 |
| Wheat Bread | +40% | +15 | - | $50 |
| Pumpkin Pie | +45% | +35 | - | $100 |
| Vegetable Stew | +60% | +25 | +10 | $120 |
| Baked Fish | +55% | +30 | +15 | $150 |
| Farmer's Breakfast | +70% | +40 | +20 | $180 |

### Drink Items
Restores thirst and sometimes energy/health:

| Item | Thirst | Energy | Health | Price |
|------|--------|--------|--------|-------|
| Water | +40% | - | - | $5 |
| Spring Water | +50% | +5 | - | $15 |
| Milk | +35% | +10 | +5 | $40 |
| Berry Juice | +45% | +15 | - | $50 |
| Tea | +35% | +20 | - | $50 |
| Coffee | +25% | +40 | - | $80 |
| Stamina Tonic | +50% | +50 | +15 | $180 |
| Energy Elixir | +60% | +60 | +10 | $200 |

### Collecting Water
- **Wells**: Primary water source
- **Refill Time**: 2 minutes real-time
- **Springs**: Always available but location-dependent
- **Crafting**: Can craft water containers to carry more

## UI Information

### HUD Display (Top-Left)
1. **Health Bar** (Red): Current/Max HP
2. **Energy Bar** (Gold): Current/Max Energy
3. **Hunger Bar** (Green/Orange/Red): 
   - Green: > 20%
   - Orange: 5-20%
   - Red: < 5%
4. **Thirst Bar** (Cyan/Orange/Red):
   - Cyan: > 20%
   - Orange: 5-20%
   - Red: < 5%
5. **Time**: Current in-game time
6. **Date**: Season, Day, Year
7. **Money**: Current funds

### Warning Messages
Appear on left side when stats are critical:
- "⚠ Hungry" (hunger < 20%)
- "⚠ STARVING!" (hunger < 5%)
- "⚠ Thirsty" (thirst < 20%)
- "⚠ DEHYDRATED!" (thirst < 5%)
- "⚠ Exhausted" (energy < 20)

### Pause Menu
- Press **E** or **Esc** to pause
- Game logic stops (time, stat decay, NPCs)
- Semi-transparent overlay
- Resume with same keys

### Crafting Menu ⭐ **NEW**
- Press **K** to open crafting menu
- Navigate with **↑/↓ arrow keys**
- Press **Enter** to craft selected recipe
- Press **Esc** to close menu
- Shows required ingredients and owned quantities
- Recipes are color-coded:
  - **Green**: Can craft (all ingredients available)
  - **Red**: Cannot craft (missing ingredients)
  - **Gray**: Item name when locked

**Available Recipes:**
- **Wood Fence**: 2 Wood → 1 Fence
- **Chest**: 50 Wood → 1 Chest
- **Fertilizer**: 1 Wood + 1 Stone → 5 Fertilizer
- **Scarecrow**: 10 Wood → 1 Scarecrow
- **Stone Path**: 3 Stone → 1 Path

### Shop Menu ⭐ **NEW**
- Press **B** to open shop menu
- Navigate with **↑/↓ arrow keys**
- Press **Enter** to buy/sell selected item
- Press **Tab** to switch between Buy and Sell modes
- Press **Esc** to close menu

**Buy Mode:**
- Shows available items with prices
- Displays how many you already own
- Cannot buy if insufficient funds
- Cannot buy if inventory is full

**Sell Mode:**
- Shows your inventory items that can be sold
- Displays sell price per item
- Sells items one at a time
- Empty slots are not shown

**Shop Inventory:**
- Seeds: Wheat, Carrot, Potato, Cabbage, Pumpkin
- Food: Apple, Carrot
- Drinks: Water, Spring Water
- Materials: Wood, Stone

### Gift Menu ⭐ **NEW**
- Press **G** when near an NPC to open gift menu
- Navigate with **↑/↓ arrow keys**
- Press **Enter** or **X** to give selected gift
- Press **Esc** or **G** to close menu

**Gift System:**
- Most items can be gifted (except tools)
- NPCs have individual gift preferences
- **Loved gifts**: +80 friendship points
- **Liked gifts**: +45 friendship points
- **Neutral gifts**: +20 friendship points
- **Disliked gifts**: -20 friendship points
- **Hated gifts**: -40 friendship points
- Friendship is measured in hearts (10 max)
- Each heart = 250 friendship points
- Menu shows NPC's current heart level
- Feedback message shows NPC's reaction

**Emma's Preferences (Example NPC):**
- ❤️ Loves: Sunflower, Pumpkin, Cauliflower
- 👍 Likes: Wheat, Carrot, Potato, Cabbage
- 👎 Dislikes: Stone, Wood
- 💔 Hates: Coal, Copper Ore

**Marcus's Preferences (Blacksmith):**
- ❤️ Loves: Gold Ore, Diamond, Emerald
- 👍 Likes: Copper Ore, Iron Ore, Coal, Stone
- 👎 Dislikes: Wheat, Carrot, Cabbage
- 💔 Hates: Sunflower

**Lily's Preferences (Merchant):**
- ❤️ Loves: Diamond, Emerald, Gold Ore
- 👍 Likes: Copper Ore, Fish, Wood, Stone
- 👎 Dislikes: Coal
- 💔 Hates: Trash

**Oliver's Preferences (Fisherman):**
- ❤️ Loves: Salmon, Tuna, Lobster
- 👍 Likes: Fish, Seaweed, Crab
- 👎 Dislikes: Coal, Stone, Copper Ore
- 💔 Hates: Iron Ore

### Quest Journal ⭐ **NEW**
- Press **F** to open quest journal
- Navigate with **↑/↓ arrow keys**
- Switch tabs with **Tab** or **1-3 keys**:
  - **1**: Active Quests
  - **2**: Available Quests
  - **3**: Completed Quests
- Press **Enter** to accept an available quest
- Press **Esc** or **F** to close journal

**Quest System Features:**
- Track multiple quests simultaneously
- View detailed objectives and progress
- See rewards before accepting
- Quests given by NPCs or town notices
- Objectives update automatically as you play
- Quest completion awards money, items, and friendship

**Starter Quests:**
1. **Welcome to Town**: Meet all 4 NPCs
2. **First Harvest** (Emma): Harvest 5 Wheat
3. **Mining for Marcus** (Marcus): Collect 10 Copper Ore
4. **Lily's Supply Run** (Lily): Collect 20 Wood & 15 Stone
5. **The Big Catch** (Oliver): Catch 15 Fish

## Auto-Combat System ⭐ **AUTO-SHOOTER ROGUELITE**

### How Auto-Combat Works
**MoonBrook Valley features an auto-shooter combat system where your weapons fire automatically!**

- **Automatic Firing**: Your equipped weapons shoot at nearby enemies without pressing attack buttons
- **Automatic Reloading**: Weapons reload themselves when out of ammo
- **No Manual Aiming**: Weapons target enemies based on their firing pattern
- **Your Role**: Focus on positioning, dodging, and strategic build choices

### Player Controls in Combat
| Action | Key | Description |
|--------|-----|-------------|
| **Movement** | WASD | Dodge enemies, position for optimal weapon coverage |
| **Throw Grenade** | Space | Manually trigger AoE grenade (30-60 second cooldown) |
| **Switch Loadout** | Tab | Cycle between weapon loadouts |
| **Quick Heal** | Q | Use health potion from inventory |
| **Call Pet** | T | Command pet to specific location |

### Combat Zones
**Important**: Combat only occurs in caves! The overworld (villages, farms) is peaceful.

- **Overworld**: Safe zones for farming, fishing, trading, and village activities
- **Caves**: Procedurally generated combat zones with endless enemy waves
- **Cave Entrances**: Found near each biome's village
- **Floor Progression**: Descend deeper for harder enemies and better loot

### Weapon System

#### Weapon Types
**Melee Weapons** (Forward firing, high damage, short range)
- Rusty Sword, Wooden Club, Iron Sword, Steel Sword, Golden Sword

**Ranged Weapons** (Forward firing, medium damage, long range)
- Wooden Bow, Crossbow, Longbow

**Magic Weapons** (Various patterns, uses mana instead of energy)
- Magic Staff (360° firing), Fire Wand (forward), Arcane Staff (360°)

#### Firing Patterns
- **Forward Firing**: Shoots in direction you're facing (rifles, bows, swords)
- **Backward Firing**: Shoots behind you (protects your back)
- **360° Firing**: Shoots in all directions (magic weapons, area weapons)
- **Targeted Firing**: Prioritizes high-value targets (sniper weapons)

#### Weapon Stats
- **Damage**: Base damage per shot
- **Fire Rate**: Shots per second (automatic)
- **Range**: Maximum target distance
- **Magazine Size**: Shots before reload
- **Reload Time**: Seconds to reload
- **Energy/Mana Cost**: Resource cost per shot

### Damage Types
Mix damage types for effective crowd control:

**Kinetic (Physical)**: Pure damage, no special effects
- Best against: Unarmored enemies
- Weapons: Most melee and ranged weapons

**Fire (DoT)**: Burning damage over time
- Best against: Groups, organic enemies
- Weapons: Flamethrower, fire wand, incendiary grenades

**Cryo (Ice)**: Slows and freezes enemies
- Best against: Fast enemies, creating defensive zones
- Weapons: Ice staff, cryo grenades, frost bow

**Electric (Lightning)**: Critical buff, chains to nearby enemies
- Best against: Groups, metal enemies
- Weapons: Lightning gun, shock wand, tesla coils

**Acid (Corrosive)**: Armor reduction, damage amplification
- Best against: Armored enemies, bosses
- Weapons: Acid sprayer, poison dagger, toxin grenades

### Grenade System
Manually triggered AoE abilities:

| Grenade Type | Effect | Cooldown | Best Use |
|--------------|--------|----------|----------|
| **Explosive** | High AoE damage | 30s | Dense enemy groups |
| **Cryo** | Freezes area | 45s | Crowd control, breathing room |
| **H-Grenade** | Destroys walls, mines resources | 60s | Mining, secret passages |
| **Neurotoxin** | DoT cloud, reduces enemy damage | 40s | Area denial, boss fights |
| **Electric** | Chain lightning, stuns | 35s | Mixed groups, stopping charges |

### Pet Companion System

#### Pet Types
**Combat Pets**: Attack enemies automatically
- **Wolf**: High damage melee, aggro draw
- **Hawk**: Fast ranged dive attacks
- **Bear**: Tank, high HP, knockback

**Support Pets**: Provide buffs and healing
- **Fairy**: HP regeneration, mana restoration
- **Spirit**: Damage buffs, elemental synergies
- **Phoenix**: Revive ability (once per run), fire immunity

**Utility Pets**: Enhance loot and detection
- **Dog**: Finds hidden items, loot bonus
- **Cat**: Enemy detection, crit chance boost
- **Owl**: Reveals map, enemy health bars

#### Pet Management
- **Summon/Dismiss**: Press **P** to open pet menu
- **Pet Actions**: Pets auto-attack and support automatically
- **Pet Leveling**: Pets gain XP alongside you
- **Skill Trees**: Allocate points to Offensive/Defensive/Utility branches
- **Pet Charms**: Equip charms to enhance abilities

### Build Strategy

#### Example Builds
**"Front and Back"** (Beginner-friendly)
- Weapon 1: Rifle (forward, kinetic)
- Weapon 2: Shotgun (backward, kinetic)
- Pet: Dog (utility, loot bonus)
- Strategy: Cover both directions, focus on positioning

**"Elemental Master"** (Advanced)
- Weapon 1: Fire Wand (360°, fire DoT)
- Weapon 2: Ice Staff (forward, cryo slow)
- Weapon 3: Lightning Gun (targeted, electric chain)
- Pet: Spirit (support, elemental synergy)
- Strategy: Apply multiple status effects, combo damage

**"Tank Brawler"** (Aggressive)
- Weapon 1: Flamethrower (forward cone, fire)
- Weapon 2: Shotgun (forward, high damage)
- Pet: Bear (tank, high HP, aggro)
- Strategy: Face-tank enemies, close-range destruction

### Combat Tips
- **Positioning is Key**: Auto-weapons fire automatically, so your positioning determines effectiveness
- **Mix Damage Types**: Elemental combos create powerful synergies
- **Watch Resources**: Monitor energy/mana for your weapons
- **Use Grenades Wisely**: 30-60 second cooldowns make timing crucial
- **Pet Synergy**: Choose pets that complement your weapon loadout
- **Upgrade Often**: Visit villages to upgrade weapons and buy better gear
- **Learn Enemy Patterns**: Each biome cave has unique enemy types

### Enemy Types (By Floor Depth)

**Floors 1-2 (Easy)**
- Green Slime (20 HP, 5 dmg)
- Cave Bat (15 HP, 8 dmg)
- Goblin (35 HP, 10 dmg)

**Floors 3-5 (Medium)**
- Skeleton (40 HP, 12 dmg)
- Giant Spider (30 HP, 14 dmg)
- Wild Wolf (45 HP, 16 dmg)

**Floors 6-8 (Hard)**
- Phantom (50 HP, 20 dmg)
- Zombie (60 HP, 18 dmg)
- Orc Warrior (80 HP, 25 dmg)

**Floors 9+ (Very Hard)**
- Fire Elemental (100 HP, 35 dmg)
- Lesser Demon (120 HP, 40 dmg)

**Bosses**: Larger, more HP, special abilities, guaranteed loot

## Tips & Strategies

### Survival Tips
1. **Always carry food and water** - Keep at least 3 of each
2. **Monitor stats constantly** - Don't let them drop below 40%
3. **Plan activities** - Mining and running drain stats quickly
4. **Use coffee strategically** - Great before long mining sessions
5. **Build a well early** - Reliable water source near your farm
6. **Cook meals** - Cooked food restores more than raw ingredients
7. **Sleep regularly** - Don't push past midnight unless necessary
8. **Eat before bed** - Wake up with full energy but stats still decay

### Energy Management
- Run only when necessary
- Use lower-tier tools for simple tasks
- Rest (stand still) to slow stat decay
- Coffee/tea for energy boosts during long work sessions

### Time Management
- Most productive hours: 6 AM - 6 PM
- Shop hours (planned): 9 AM - 5 PM
- NPC availability varies by schedule
- Don't waste daylight hours

### Economic Tips
- Sell crops at peak ripeness for maximum profit
- Buy seeds from shop when you run out
- Gather Wood and Stone to sell for quick money
- Use crafting to create valuable items
- Shop prices are fixed (buy and sell prices differ)
- Keep emergency food/water supply
- Don't hoard consumables - use them!

### Crafting Tips ⭐ **NEW**
- Gather Wood and Stone before crafting
- Craft Fertilizer to improve crop growth (planned feature)
- Build Scarecrows to protect crops (planned feature)
- Save expensive recipes (like Chest) for when you have surplus materials
- Check crafting menu regularly for new recipes

## Accessibility

### Customizing Controls
Controls can be customized by editing the `InputManager` class:
- File location: `Core/Systems/InputManager.cs`
- Modify key bindings in the constructor
- Recompile the game

### Difficulty Adjustments
To adjust survival difficulty, edit `PlayerStats.cs`:
- `HUNGER_DECAY_RATE`: Default 0.05 (lower = easier)
- `THIRST_DECAY_RATE`: Default 0.08 (lower = easier)
- Activity multipliers can be reduced
- Threshold values can be adjusted

## Troubleshooting

### "Controls not working"
1. Ensure game window has focus (click on it)
2. Check if paused (press Esc to unpause)
3. Verify NumLock state for number keys

### "Character moves too slow"
- Check hunger level (< 20% reduces speed by 30%)
- Verify you're not trying to run with < 1 energy
- Clear any obstacles in the way

### "Stats draining too fast"
- This is intentional game design
- Carry more food/water
- Avoid constant running
- Take breaks from strenuous activities

---

*For more information, see ARCHITECTURE.md and DEVELOPMENT.md*
