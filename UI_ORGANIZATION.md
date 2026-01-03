# UI Organization & Keybinding Guide

## Overview
MoonBrook Ridge has undergone a major UI reorganization to fix keybinding conflicts and improve accessibility. This document outlines the new menu system and keybinding philosophy.

## Core Gameplay Keybinds (Protected)
These keys are **RESERVED** for core gameplay and cannot be used for other functions:

### Movement (WASD + Arrow Keys)
- **W / ↑ Arrow**: Move Up
- **S / ↓ Arrow**: Move Down
- **A / ← Arrow**: Move Left
- **D / → Arrow**: Move Right
- **Left Shift**: Run/Sprint

**CRITICAL**: WASD keys are exclusively for movement. Any new features must NOT use these keys to prevent interference with player movement.

## Primary Menu System

### Map Menu (M Key) ⭐ **NEW**
The M key now opens a comprehensive tabbed Map Menu with three sections:

#### Tab 1: World Map
- View the game world
- Toggle minimap visibility (Space key within menu)
- Overview of discovered areas

#### Tab 2: Waypoints
- List of all discovered locations
- View waypoint details (name, description, type)
- Navigate with ↑↓ arrow keys
- Select waypoints with Enter

#### Tab 3: Fast Travel
- Travel to discovered waypoints
- Shows travel cost (gold) and time cost (1 hour)
- Can only travel to unlocked waypoints
- Costs vary based on distance from current location

**Navigation**: 
- Use Tab key or number keys (1-3) to switch between tabs
- Use ↑↓ to navigate within lists
- Enter to select/confirm
- Escape to close menu

### Player Menu (E Key)
Opens the unified player menu with all character-related information:
- Inventory
- Crafting
- Skills & Magic
- Quests & Achievements
- And more...

### Context-Specific Menus
These menus are available in specific situations:

| Key | Menu | When Available |
|-----|------|----------------|
| **B** | Shop Menu | Anywhere (for testing) |
| **G** | Gift Menu | When near an NPC |
| **T** | Tame Pet | When near a wild pet |
| **V** | Marriage Proposal | When near NPC with 10 hearts |
| **H** | Building Menu | Anywhere |
| **O** | Settings | Anywhere |

## Quick Action Keys

| Key | Action |
|-----|--------|
| **C** | Use Tool / Place Item |
| **X** | Interact / Talk |
| **Space** | Throw Grenade (combat) |
| **N** | Toggle Auto-Fire |
| **I** | Sort Inventory |
| **F** | Open Quest Journal |
| **Tab** | Switch Weapon Loadout |
| **F5** | Quick Save |
| **F9** | Quick Load |
| **F3** | Toggle Performance Monitor |

## Hotbar (Numbers & Symbols)
Quick access to equipped items:
- **1-9**: Hotbar slots 1-9
- **0**: Hotbar slot 10
- **- (Minus)**: Hotbar slot 11
- **= (Equals)**: Hotbar slot 12

## Design Philosophy

### 1. Core Gameplay First
Movement keys (WASD) are sacred and never overlap with other functions.

### 2. Contextual Menus
Menus are grouped by context:
- **M**: Map & world navigation
- **E**: Player character & stats
- **Context keys**: Specific actions (G for Gift, etc.)

### 3. Intuitive Organization
Related features are grouped in tabbed interfaces:
- Map menu contains waypoints AND fast travel
- Player menu contains inventory, crafting, skills
- No more searching through multiple keybinds for related features

### 4. Consistent Navigation
All tabbed menus follow the same pattern:
- **Tab** or **Number keys**: Switch tabs
- **↑↓**: Navigate lists
- **Enter**: Select/Confirm
- **Escape**: Close/Cancel

## Migration from Old System

### What Changed?

| Old Binding | New Binding | Reason |
|-------------|-------------|--------|
| W - Fast Travel | M (Map Menu > Fast Travel tab) | W conflicts with movement |
| M - Toggle Minimap | M (Map Menu > World Map tab) | Consolidated with other map features |
| K - Crafting | E (Player Menu > Crafting tab) | Consolidated player features |

### What Stayed the Same?
- All movement keys (WASD, Arrow keys, Shift)
- All action keys (C, X, Space)
- All quick actions (F5, F9, I, N)
- Hotbar numbers (1-0, -, =)

## Future Improvements

### Planned Features
1. **Tab Key Reorganization**: Make Tab open Inventory/Crafting instead of weapon switching
2. **C Key Menu**: Create comprehensive character menu with sub-tabs
3. **Waypoint Improvements**: Show waypoints as compass icons on minimap
4. **Click-to-Travel**: Allow clicking waypoints on map for fast travel

### Expandability
The new tabbed menu system is designed to be expandable:
- Easy to add new tabs to existing menus
- Consistent user experience across all menus
- Scalable for future features without cluttering keybinds

## Tips for Players

1. **Press M first**: When you want to navigate or fast travel, think "M for Map"
2. **E for Everything about YOU**: Inventory, crafting, skills - it's all in the E menu
3. **Context is key**: Special action keys (G, T, V) only work when appropriate
4. **Learn the Tab flow**: Within any menu, Tab cycles through sections

## Tips for Developers

1. **Never use WASD**: These keys are off-limits for any new features
2. **Use tabbed menus**: Group related features instead of adding new keybinds
3. **Follow patterns**: Use existing navigation patterns (↑↓ for lists, Enter to confirm)
4. **Context over clutter**: Make context-specific keys rather than global hotkeys
5. **Test movement**: Always verify WASD movement works after UI changes

## Troubleshooting

### "I press W and nothing moves!"
- Check if a menu is open (Map, Player, Shop, etc.)
- Press Escape to close all menus
- Ensure game window has focus (click on it)

### "I can't find the fast travel menu!"
- Press **M** to open Map Menu
- Use **Tab** or press **3** to go to Fast Travel tab
- Or use arrow keys to navigate to Fast Travel section

### "Where did the minimap toggle go?"
- Press **M** to open Map Menu
- The World Map tab shows minimap status
- Press **Space** while in Map Menu to toggle minimap

## Summary

The new UI system prioritizes:
1. ✅ **Conflict-free movement** - WASD always works
2. ✅ **Logical grouping** - Related features together
3. ✅ **Easy discovery** - Fewer top-level keys to remember
4. ✅ **Consistent navigation** - Same patterns across all menus
5. ✅ **Room to grow** - Easy to add new features

For the complete keybind reference, see [CONTROLS.md](CONTROLS.md).
