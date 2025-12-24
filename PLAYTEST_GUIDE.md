# Playtest Guide

This guide explains how to quickly playtest MoonBrook Ridge and what features are currently implemented.

## 🚀 Quick Start

### Method 1: Using the Play Script (Recommended)
```bash
./play.sh
```

### Method 2: Manual Build and Run
```bash
cd MoonBrookRidge
dotnet build
dotnet run
```

### Method 3: Using Visual Studio or Rider
1. Open `MoonBrookRidge.sln` in your IDE
2. Press F5 or click the "Run" button

## ✅ Quick Build Test (Without Running)
To verify the game compiles without launching it:
```bash
./test-build.sh
```

## 🎮 Currently Implemented Features

### ✅ Core Systems
- **Player Movement**: WASD or Arrow keys to move, Shift to run
- **Time System**: Watch the in-game clock progress (1 game hour = 2.5 real seconds)
- **Camera**: Smooth following camera with zoom
- **Survival Stats**: Hunger and thirst that deplete over time
- **Save/Load** ⭐ **NEW!**: Press `F5` to quick save, `F9` to quick load

### ✅ Farming Mechanics (NEW!)
1. **Till Soil**: Press `Tab` to select the Hoe, then `C` to till grass/dirt tiles
2. **Plant Seeds**: Stand on tilled soil and press `X` to plant seeds
   - You start with: 20 Wheat Seeds, 10 Carrot Seeds, 10 Potato Seeds
3. **Water Crops**: Press `Tab` to select Watering Can, then `C` on tilled soil
4. **Harvest Crops**: Press `Tab` to select Scythe, then `C` on fully grown crops
   - **Harvested crops now add to inventory!** ⭐
   - Watered crops yield 2x harvest (vs 1x for unwatered)
5. **Watch Crops Grow**: Crops grow based on in-game time
   - Wheat: 1 hour per stage (6 hours total)
   - Carrots: 1.2 hours per stage (7.2 hours total)
   - Potatoes: 1.5 hours per stage (9 hours total)

### ✅ Inventory System
- **Starting Items**:
  - 5 Carrots (food)
  - 3 Apples (food)
  - 2 Wheat Bread (food)
  - 10 Water (drink)
  - 3 Spring Water (drink)
  - 20 Wheat Seeds
  - 10 Carrot Seeds
  - 10 Potato Seeds
- **Harvested crops** ⭐ are automatically added to your inventory

### ✅ Consumables
- **Hotbar Keys**: Press `1-9`, `0`, `-`, `=` to consume items
- **Food**: Restores hunger
- **Drinks**: Restores thirst
- Watch your hunger/thirst bars in the HUD

### ✅ Tool System
- **Tool Cycling**: Press `Tab` to cycle through tools
  - Hoe → Watering Can → Scythe → Pickaxe → Axe → (back to Hoe)
- **Use Tool**: Press `C` to use the equipped tool
- Tools consume energy when used

### ✅ User Interface
- **HUD**: Top-left shows health, energy, hunger, thirst
- **Time Display**: Shows current time, day, season, year
- **Pause**: Press `E` or `Esc` to pause

## 🧪 Things to Test

### Complete Farming Loop
1. **Start the game** and observe the HUD
2. **Find a grassy area** near spawn
3. **Equip the Hoe** (press `Tab` if needed)
4. **Till 3x3 area**: Press `C` to till 9 grass tiles into tilled soil
5. **Plant seeds**: Stand on tilled soil and press `X` to plant
6. **Water crops**: Switch to Watering Can (`Tab`) and press `C` on crops
7. **Wait for growth**: Let in-game time pass (or speed up by standing idle)
8. **Harvest**: Switch to Scythe (`Tab`) and press `C` on fully grown crops

### Survival Mechanics
1. **Run around** (hold `Shift`) and watch hunger/thirst deplete faster
2. **Eat food**: Press `1` (or other hotbar key) when hunger is low
3. **Drink water**: Press hotbar keys for drinks when thirst is low
4. **Watch for warnings**: Low stats show warnings in the HUD
5. **Energy consumption**: Use tools and watch energy decrease

### Time and Day Cycle
1. **Watch time progress**: Each game hour is 2.5 real seconds
2. **Day advancement**: Time goes until 2 AM (26:00), then resets to next day
3. **Seasons**: 28 days per season (Spring → Summer → Fall → Winter)

### Save/Load System ⭐ **NEW!**
1. **Quick Save**: Press `F5` at any time to save your game
   - Saves to `quicksave.json` in your local app data
2. **Quick Load**: Press `F9` to load your last quick save
   - Restores player position, stats, and inventory
3. **Save Location**: 
   - Windows: `%LocalAppData%\MoonBrookRidge\Saves\`
   - Mac: `~/Library/Application Support/MoonBrookRidge/Saves/`
   - Linux: `~/.local/share/MoonBrookRidge/Saves/`
4. **What's Saved**:
   - Player position, health, energy, hunger, thirst, money
   - Time, day, season, year
   - Inventory contents
   - *(Crops in world not yet saved - coming soon)*

### Tool Usage
1. **Hoe**: Converts grass/dirt to tilled soil (costs 2 energy)
2. **Watering Can**: Waters tilled soil (costs 2 energy)
3. **Scythe**: Harvests fully grown crops (costs 3 energy)
4. **Pickaxe**: Not yet implemented for mining
5. **Axe**: Not yet implemented for tree chopping

## 🐛 Known Limitations
- Pickaxe and Axe don't have functional targets yet
- No visual feedback when planting seeds
- No NPC interactions yet
- Crops in world not saved yet (only player/inventory/time)
- Save/load doesn't have UI menu yet (F5/F9 only)

## 💡 Tips for Testing
- **Plant multiple crops**: Test different growth rates
- **Time observation**: A full day cycle takes about 3-4 real minutes
- **Resource management**: Balance tool use with energy/hunger/thirst
- **Collision**: Try walking into water tiles (blocks movement)
- **Camera zoom**: See the world from different perspectives
- **Test save/load**: Try saving mid-game, making changes, then loading

## 🔧 Troubleshooting

### Game Won't Start
```bash
# Try cleaning and rebuilding
cd MoonBrookRidge
dotnet clean
dotnet build
dotnet run
```

### Build Errors
```bash
# Restore dependencies
dotnet restore
```

### Performance Issues
- The game runs at 60 FPS by default
- Monitor CPU/GPU usage if experiencing lag

## 📝 Reporting Issues
When testing, note:
1. What you were doing when the issue occurred
2. What you expected to happen
3. What actually happened
4. Any error messages shown

## 🎯 Next Features Coming
- [ ] Harvested crops add to inventory
- [ ] Save/load game system
- [ ] Pause menu with save/load options
- [ ] Visual feedback for seed planting
- [ ] Mining with pickaxe
- [ ] Tree chopping with axe
- [ ] NPC interactions
