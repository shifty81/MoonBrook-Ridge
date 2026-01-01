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
| Open Menu / Inventory | E | Escape |
| Open Journal / Quests | F | - |
| Open Map | M | - |
| **Open Crafting Menu** ⭐ **NEW** | **K** | - |
| **Open Shop Menu** ⭐ **NEW** | **B** | - |
| Switch Toolbar Row | Tab | - |
| **Quick Save** ⭐ | **F5** | - |
| **Quick Load** ⭐ | **F9** | - |

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
