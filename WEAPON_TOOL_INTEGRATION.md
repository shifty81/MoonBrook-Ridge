# Weapon and Tool System Integration

## Overview
This document outlines the planned integration of weapon loadout and tool management into the character UI, including upgrade systems for both weapons and tools.

## Current State

### Current Keybindings
- **Tab**: Switches weapon loadout (in combat)
- **Tools**: Selected via hotbar (1-0, -, =)
- **No Upgrade System**: Weapons and tools are static

### Current Issues
1. Weapon loadout switching uses Tab key (conflicts with planned Inventory UI)
2. No centralized location to view/manage weapons
3. No upgrade system for weapons or tools
4. Tool management is limited to hotbar selection

## Proposed Integration

### Character UI Reorganization

#### Tab Key Menu (Primary Character Interface)
```
┌─────────────────────────────────────────┐
│  TAB: Character Menu                    │
├─────────────────────────────────────────┤
│ [Inventory] [Loadout] [Tools] [Stats]  │
├─────────────────────────────────────────┤
│                                         │
│  Current Tab Content Here               │
│                                         │
└─────────────────────────────────────────┘
```

#### Tab 1: Inventory
- Grid view of all items
- Drag-and-drop organization
- Quick sort options
- Item details on hover

#### Tab 2: Weapon Loadout ⭐ **NEW**
```
┌─────────────────────────────────────────┐
│  Weapon Loadout Management              │
├─────────────────────────────────────────┤
│  Active Loadout: [Slot 1] [Slot 2]     │
│                  [Slot 3]               │
│                                         │
│  Loadout Presets:                       │
│  ● [Loadout A] - Melee + Ranged        │
│  ○ [Loadout B] - Magic + Support        │
│  ○ [Loadout C] - Custom                 │
│                                         │
│  Weapon Details:                        │
│  ┌─────────────────────────┐           │
│  │ Iron Sword             │           │
│  │ Damage: 25            │           │
│  │ Speed: 1.2/sec        │           │
│  │ Range: Melee          │           │
│  │                       │           │
│  │ [Upgrade] Level 2/5   │           │
│  │ Cost: 500g + 10 Iron  │           │
│  └─────────────────────────┘           │
└─────────────────────────────────────────┘
```

#### Tab 3: Tools & Equipment ⭐ **NEW**
```
┌─────────────────────────────────────────┐
│  Tools & Equipment                      │
├─────────────────────────────────────────┤
│  Farming Tools:                         │
│  [Hoe] [Watering Can] [Sickle]        │
│                                         │
│  Harvesting Tools:                      │
│  [Axe] [Pickaxe] [Fishing Rod]         │
│                                         │
│  Selected: Copper Pickaxe               │
│  ┌─────────────────────────┐           │
│  │ Efficiency: 1.5x       │           │
│  │ Durability: 85/100     │           │
│  │ Mining Speed: Fast     │           │
│  │                       │           │
│  │ [Upgrade to Iron]     │           │
│  │ Cost: 1000g + 20 Iron │           │
│  │ Next: 2.0x efficiency │           │
│  └─────────────────────────┘           │
└─────────────────────────────────────────┘
```

#### Tab 4: Stats
- Character statistics
- Skill levels
- Achievements progress

### Weapon Loadout System

#### Loadout Slots
- **3 Active Weapons**: Can be equipped simultaneously
- **Multiple Loadouts**: Save 3-5 preset loadouts
- **Quick Switch**: Number keys or Tab to cycle through loadouts
- **Auto-Switch**: Optional auto-switch based on context (combat vs farming)

#### Weapon Types
1. **Melee Weapons**: Sword, Axe, Spear, Dagger
2. **Ranged Weapons**: Bow, Crossbow, Gun
3. **Magic Weapons**: Staff, Wand, Tome
4. **Support Items**: Shield, Lantern, Healing Items

#### Weapon Upgrade System
```
Weapon Progression:
Wooden → Stone → Copper → Iron → Steel → Mithril → Legendary

Each Upgrade:
- Increased damage
- Better attack speed
- Enhanced effects
- New abilities (at certain tiers)

Upgrade Requirements:
- Gold cost (increases per tier)
- Materials (specific to weapon type)
- Player level requirement
- Skill level requirement (optional)
```

### Tool System

#### Tool Categories
1. **Farming Tools**
   - Hoe (tilling soil)
   - Watering Can (watering crops)
   - Sickle (harvesting)
   - Scythe (clearing grass)

2. **Harvesting Tools**
   - Axe (chopping trees)
   - Pickaxe (mining rocks)
   - Fishing Rod (fishing)

3. **Utility Tools**
   - Lantern (lighting)
   - Magnifying Glass (finding items)
   - Backpack (inventory space)

#### Tool Upgrade Tiers
```
Basic → Copper → Iron → Gold → Iridium

Upgrade Benefits:
- Efficiency multiplier (faster work)
- Durability (more uses before breaking)
- Area of effect (water/till multiple tiles)
- Energy cost reduction
- Special abilities
```

#### Tool Progression Example: Pickaxe
```
Tier 1: Basic Pickaxe
- Efficiency: 1.0x
- Durability: 40 uses
- Energy: 2 per use
- Range: 1 tile

Tier 2: Copper Pickaxe
- Efficiency: 1.5x (faster mining)
- Durability: 80 uses
- Energy: 1.5 per use
- Range: 1 tile
- Cost: 200g + 10 Copper Ore

Tier 3: Iron Pickaxe
- Efficiency: 2.0x
- Durability: 120 uses
- Energy: 1 per use
- Range: 1 tile
- Ability: Can break large rocks
- Cost: 1000g + 20 Iron Ore

Tier 4: Gold Pickaxe
- Efficiency: 2.5x
- Durability: 200 uses
- Energy: 0.5 per use
- Range: 3x3 tiles
- Ability: Chance for gems
- Cost: 5000g + 10 Gold Ore

Tier 5: Iridium Pickaxe
- Efficiency: 3.0x
- Durability: Infinite
- Energy: 0 per use
- Range: 5x5 tiles
- Ability: Always find gems, rare ore bonus
- Cost: 50000g + 5 Iridium Bars
```

### UI Navigation Flow

#### Opening Character Menu
```
Press Tab → Character Menu Opens
Default: Inventory Tab

Within Menu:
- Tab or Number Keys (1-4): Switch tabs
- ↑↓: Navigate items/weapons/tools
- Enter: Select/Use/Equip
- E or Esc: Close menu
```

#### Weapon Loadout Switching (In-Game)
```
Option 1: Via Character Menu
Tab → Loadout Tab → Select Preset → Enter

Option 2: Quick Switch (Combat)
Hold Shift + 1/2/3 → Switch to Loadout A/B/C
(Preserves Tab key for menu)

Option 3: Auto-Switch
Context-aware switching:
- Enter cave → Combat loadout
- Exit cave → Farming loadout
- Near water → Fishing loadout
```

### Upgrade System Details

#### Upgrade UI
```
When Tool/Weapon Selected:
┌─────────────────────────────────┐
│  Iron Sword (Level 2)           │
├─────────────────────────────────┤
│  Current Stats:                 │
│  • Damage: 25                   │
│  • Speed: 1.2 attacks/sec       │
│  • Range: Melee                 │
│                                 │
│  Next Level (3):                │
│  • Damage: 35 (+10)             │
│  • Speed: 1.4 (+0.2)            │
│  • NEW: Critical Hit Chance 10% │
│                                 │
│  Upgrade Cost:                  │
│  ✓ 500 Gold                     │
│  ✓ 10 Iron Bars                 │
│  ✗ 5 Gems (need 2 more)         │
│                                 │
│  [Upgrade] [Cancel]             │
└─────────────────────────────────┘
```

#### Upgrade Sources
1. **Blacksmith NPC**: Weapon upgrades
2. **Toolsmith NPC**: Tool upgrades
3. **Self-Crafting**: Basic upgrades at workbench
4. **Quest Rewards**: Special upgrade materials
5. **Dungeon Loot**: Rare upgrade components

#### Upgrade Materials
```
Common Materials:
- Wood, Stone, Copper, Iron
- Found everywhere

Uncommon Materials:
- Steel, Silver, Gold
- Found in deeper mines

Rare Materials:
- Mithril, Adamantite, Orichalcum
- Found in dungeons, boss drops

Legendary Materials:
- Dragon Scales, Phoenix Feathers
- Boss-only drops, rare events
```

### Durability System (Optional)

#### Tool Durability
- **Uses Before Break**: Each tool has max uses
- **Visual Indicator**: Color-coded (green → yellow → red)
- **Repair System**: Visit blacksmith or use repair kit
- **Higher Tiers**: More durable

#### Repair Options
1. **Blacksmith Repair**: Pay gold, instant repair
2. **Repair Kit**: Craftable item, portable repair
3. **Auto-Repair**: Expensive upgrade, tool never breaks

### Integration with Existing Systems

#### Combat System
- Weapon loadout affects auto-combat behavior
- Different loadouts = different combat strategies
- Upgrade weapons for more damage/effects

#### Farming System
- Tool upgrades make farming more efficient
- Higher tier tools = less energy use
- Area upgrades = faster large-scale farming

#### Skill System
- Weapon/Tool proficiency levels
- Higher skill = better efficiency
- Unlock special abilities at skill milestones

#### Economy System
- Upgrades are major gold sinks
- Material gathering becomes important
- Trading/selling surplus materials

## Implementation Plan

### Phase 1: UI Framework
1. Create TabbedCharacterMenu base
2. Add Inventory tab (already exists in UnifiedPlayerMenu)
3. Add Loadout tab structure
4. Add Tools tab structure

### Phase 2: Weapon Loadout
1. Create LoadoutSystem class
2. Add loadout presets (save/load)
3. Implement quick-switch keybinds
4. Integrate with combat system

### Phase 3: Upgrade System
1. Create UpgradeManager class
2. Define upgrade tiers for all weapons
3. Define upgrade tiers for all tools
4. Create upgrade UI components
5. Add upgrade cost checking
6. Implement upgrade application

### Phase 4: Tool Management
1. Create ToolManager enhancements
2. Add durability tracking
3. Add efficiency modifiers
4. Integrate with farming/harvesting

### Phase 5: Polish
1. Add visual feedback for upgrades
2. Add sound effects
3. Add particle effects for upgraded tools
4. Balance costs and progression
5. Tutorial/help system

## Benefits

### For Players
- ✅ Centralized character management
- ✅ Clear upgrade paths
- ✅ Meaningful progression system
- ✅ Strategic loadout customization
- ✅ Improved tool efficiency over time

### For Gameplay
- ✅ Extended endgame content
- ✅ Gold sink for economy balance
- ✅ Incentive for material gathering
- ✅ Rewards for dungeon exploration
- ✅ Skill-based progression

### For UI/UX
- ✅ Less keybind clutter
- ✅ Logical organization
- ✅ Consistent navigation
- ✅ Scalable for future features

## Keybinding Changes

### Current (Combat)
- Tab: Switch weapon loadout

### Proposed
- Tab: Open Character Menu
- Shift + 1/2/3: Quick-switch loadout (in combat)
- Within Menu: Number keys switch tabs

This allows:
- Tab for main character interface (consistent with inventory)
- Quick combat switching preserved with modifier
- Better organization of character features

## Future Enhancements

1. **Weapon Enchantments**: Add elemental effects to weapons
2. **Tool Modifications**: Attachments for tools (auto-harvest, etc.)
3. **Set Bonuses**: Matching weapon/tool sets for bonuses
4. **Transmog System**: Change appearance while keeping stats
5. **Trading System**: Trade upgraded items with other players

---

*This feature will be implemented in a future update. Current priority: Fix keybinds and expand world.*
