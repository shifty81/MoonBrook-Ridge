# Runtime Testing Guide

This guide provides comprehensive instructions for testing the MoonBrook Ridge game after the custom engine migration.

## Quick Start

To run the game for testing:

```bash
cd MoonBrookRidge
dotnet run
```

Or use the convenience script:

```bash
./play.sh
```

## Testing Phases

### Phase 1: Basic Initialization & Launch ✅

**Objective**: Verify the game starts without crashes and initializes all core systems.

#### What to Test:
1. **Game Launch**
   - [ ] Game window opens successfully
   - [ ] Window title shows "MoonBrook Ridge"
   - [ ] Window size is correct (1280x720 default)
   - [ ] No immediate crashes or exceptions

2. **Graphics Initialization**
   - [ ] Green background color displays (bright grass green)
   - [ ] Window is resizable
   - [ ] Fullscreen toggle works (if implemented)

3. **Content Loading**
   - [ ] Default font loads successfully
   - [ ] No "Content not found" errors in console
   - [ ] Loading screen appears (if implemented)

4. **Main Menu**
   - [ ] Main menu state loads
   - [ ] Menu options are visible and clickable
   - [ ] Menu navigation works

#### Expected Results:
- Game launches within 2-3 seconds
- No crashes or exceptions
- Main menu is fully interactive

#### Common Issues:
- **Content not found**: Check Content directory exists and files are built
- **Black screen**: Check GraphicsDevice initialization
- **Crash on startup**: Check engine initialization in Game.cs

---

### Phase 2: Core Systems Testing

**Objective**: Verify fundamental game systems work correctly.

#### 2.1 Player Movement

**What to Test:**
- [ ] WASD keys move player in correct directions
- [ ] Arrow keys work as alternative controls
- [ ] Shift key enables running (faster movement)
- [ ] Player cannot walk through walls/obstacles
- [ ] Player animation changes with movement direction
- [ ] Player animation plays smoothly

**How to Test:**
1. Start a new game or load existing save
2. Press W - player should move up
3. Press S - player should move down
4. Press A - player should move left
5. Press D - player should move right
6. Hold Shift while moving - player should move faster
7. Try walking into buildings/trees - player should stop

**Expected Results:**
- Movement is smooth (60 FPS)
- No jittering or stuttering
- Collision detection works correctly

#### 2.2 Camera System

**What to Test:**
- [ ] Camera follows player smoothly
- [ ] Camera doesn't go outside world bounds
- [ ] Zoom functionality works (if implemented)
- [ ] Camera centers on player

**How to Test:**
1. Move player around the map
2. Observe camera tracking
3. Try moving to map edges
4. Test zoom in/out if available

**Expected Results:**
- Camera smoothly follows player with slight lag (cinematic feel)
- Camera stays within world boundaries
- No camera jittering

#### 2.3 HUD & UI

**What to Test:**
- [ ] Health bar displays correctly
- [ ] Energy bar displays correctly  
- [ ] Time displays (hour and minute)
- [ ] Date displays (day, season, year)
- [ ] Money/gold displays
- [ ] Weather icon displays
- [ ] All UI elements scale properly

**How to Test:**
1. Check all HUD elements are visible
2. Perform actions that change stats (use tools = lose energy)
3. Wait for time to pass
4. Verify all values update correctly

**Expected Results:**
- All HUD elements visible in corners
- Values update in real-time
- No overlapping UI elements

#### 2.4 Time System

**What to Test:**
- [ ] In-game time progresses
- [ ] Day/night cycle works
- [ ] Season changes after 28 days
- [ ] Time affects lighting (if implemented)
- [ ] Time can be paused (in menus)

**How to Test:**
1. Watch time progress on HUD
2. Wait for day to change
3. Note current season
4. Open inventory - time should pause
5. Wait multiple in-game days to see season change

**Expected Results:**
- Time flows smoothly
- 1 real second = ~1 minute game time (adjustable)
- Season transitions work correctly

---

### Phase 3: Gameplay Systems Testing

#### 3.1 Farming Mechanics

**What to Test:**
- [ ] Hoe tool can till soil
- [ ] Seeds can be planted on tilled soil
- [ ] Watering can waters crops
- [ ] Crops show growth stages
- [ ] Mature crops can be harvested
- [ ] Scythe harvests crops correctly
- [ ] Harvested items go to inventory

**How to Test:**
1. Equip hoe (press 1 or select from inventory)
2. Use tool (C key) on grass tile
3. Select seeds from inventory
4. Plant seeds (X key) on tilled soil
5. Equip watering can (press 2)
6. Water planted crops (C key)
7. Wait several in-game days for growth
8. Harvest with scythe when mature

**Expected Results:**
- Each action produces visual feedback
- Crop sprites change as they grow
- Particles appear for each action
- Crops appear in inventory after harvest

#### 3.2 Tool System

**What to Test:**
- [ ] All 6 tools can be equipped
- [ ] Tool hotkeys work (1-6)
- [ ] Tool overlay shows when using tools
- [ ] Tool durability decreases (if implemented)
- [ ] Tool animations play correctly
- [ ] Energy depletes when using tools

**Tools to Test:**
1. Hoe (1) - till soil
2. Watering Can (2) - water crops
3. Scythe (3) - harvest crops, cut grass
4. Pickaxe (4) - mine rocks
5. Axe (5) - chop trees
6. Fishing Rod (6) - catch fish

**Expected Results:**
- Tools change player animation
- Tool overlay appears above player
- Each use costs energy (1-2 points)
- Appropriate particles appear

#### 3.3 Inventory System

**What to Test:**
- [ ] Inventory opens (E or Esc key)
- [ ] Items display correctly
- [ ] Item stacking works (up to 99)
- [ ] Items can be moved/dragged
- [ ] Items can be dropped
- [ ] Items can be consumed
- [ ] Hotbar items work (7-9, 0, -, =)
- [ ] Inventory sorting works (I key)

**How to Test:**
1. Press E to open inventory
2. Pick up multiple of same item
3. Try dragging items to different slots
4. Drop an item outside inventory
5. Place food item in hotbar
6. Press hotbar key to consume
7. Press I to sort inventory

**Expected Results:**
- Inventory grid displays properly
- Items stack automatically
- Drag and drop is smooth
- Item counts are accurate

#### 3.4 Crafting System

**What to Test:**
- [ ] Crafting menu opens (K key)
- [ ] Recipes display correctly
- [ ] Can craft items with ingredients
- [ ] Ingredients are consumed
- [ ] Crafted items appear in inventory
- [ ] Can't craft without ingredients
- [ ] Recipe categories work

**How to Test:**
1. Press K to open crafting menu
2. Select a recipe (e.g., Basic Fence)
3. Check ingredient requirements
4. Click craft button
5. Verify ingredients removed
6. Verify crafted item in inventory

**Expected Results:**
- Recipes show ingredients needed
- Grayed out if missing ingredients
- Crafting plays sound effect
- Item quantity updates

#### 3.5 Shop System

**What to Test:**
- [ ] Shop menu opens (B key)
- [ ] Can buy items
- [ ] Can sell items
- [ ] Money updates correctly
- [ ] Can't buy if insufficient funds
- [ ] Item prices display correctly

**How to Test:**
1. Press B to open shop
2. Try buying seeds
3. Try selling harvested crops
4. Check money before/after
5. Try buying expensive item with no money

**Expected Results:**
- Transactions work correctly
- Money deducts/adds properly
- Error message if can't afford

#### 3.6 Building System

**What to Test:**
- [ ] Building menu opens (H key)
- [ ] Building preview shows
- [ ] Green/red validation works
- [ ] Can place buildings on valid tiles
- [ ] Can't place on invalid tiles
- [ ] Resources are consumed
- [ ] Buildings are functional

**How to Test:**
1. Press H for building menu
2. Select a building (e.g., Shed)
3. Move mouse to see preview
4. Try placing on grass (valid)
5. Try placing on water (invalid)
6. Place building
7. Verify resources deducted

**Expected Results:**
- Preview follows mouse
- Color indicates valid/invalid placement
- Building appears after placement
- Can interact with building

---

### Phase 4: Advanced Features Testing

#### 4.1 NPC Interactions

**What to Test:**
- [ ] NPCs spawn in world
- [ ] NPCs follow schedules
- [ ] NPCs pathfind correctly
- [ ] Chat bubbles appear
- [ ] Dialogue system works
- [ ] Radial dialogue wheel works
- [ ] Gift giving works (G key)
- [ ] Friendship levels increase

**How to Test:**
1. Find an NPC (Emma, Marcus, Lily, etc.)
2. Walk near NPC to see chat bubble
3. Interact with NPC (X key)
4. Select dialogue options
5. Give gift (G key near NPC)
6. Check relationship level

**Expected Results:**
- NPCs move naturally
- Dialogue appears correctly
- Gifts affect friendship
- NPCs react to time/location

#### 4.2 Quest System

**What to Test:**
- [ ] Quest journal opens (F key)
- [ ] Active quests display
- [ ] Quest objectives track progress
- [ ] Quest completion triggers
- [ ] Quest rewards given
- [ ] Notification toasts appear

**How to Test:**
1. Press F for quest journal
2. Accept a quest from NPC
3. Complete quest objectives
4. Return to quest giver
5. Receive rewards

**Expected Results:**
- Quest log updates
- Progress tracked accurately
- Rewards added to inventory

#### 4.3 Combat System

**What to Test:**
- [ ] Combat works in caves/dungeons
- [ ] Enemies spawn correctly
- [ ] Can attack enemies (Space key)
- [ ] Auto-fire works (` key toggle)
- [ ] Weapons fire automatically
- [ ] Enemies take damage
- [ ] Player takes damage
- [ ] Enemy loot drops
- [ ] Combat stats update

**How to Test:**
1. Enter a cave/mine
2. Wait for enemy spawn
3. Press Space to attack
4. Toggle auto-fire with `
5. Defeat enemy
6. Collect loot

**Expected Results:**
- Smooth combat mechanics
- Projectiles fire correctly
- Damage numbers appear
- Loot appears after death

#### 4.4 Pet System

**What to Test:**
- [ ] Pet menu opens (P key)
- [ ] Can tame wild pets (T key)
- [ ] Pet follows player
- [ ] Pet attacks enemies
- [ ] Pet levels up
- [ ] Pet skills unlock

**How to Test:**
1. Find wild pet in world
2. Press T near pet to tame
3. Press P for pet menu
4. Assign pet to active slot
5. Enter combat area
6. Watch pet assist in battle

**Expected Results:**
- Pet AI works correctly
- Pet deals damage
- Pet gains experience
- Skills activate

#### 4.5 Dungeon System

**What to Test:**
- [ ] Dungeon entrances spawn
- [ ] Can enter dungeons
- [ ] Dungeon map shows (D key)
- [ ] Rooms generate correctly
- [ ] Boss encounters work
- [ ] Treasure chests spawn
- [ ] Can exit dungeons

**How to Test:**
1. Find dungeon entrance
2. Enter dungeon (X key)
3. Press D for dungeon map
4. Explore rooms
5. Fight enemies
6. Find boss room
7. Defeat boss
8. Exit dungeon

**Expected Results:**
- Procedural generation works
- Rooms connect properly
- Enemies spawn in combat rooms
- Treasure rooms have loot

---

### Phase 5: Save/Load & Performance

#### 5.1 Save System

**What to Test:**
- [ ] Manual save works (F5)
- [ ] Auto-save triggers (every 5 min)
- [ ] Save notification appears
- [ ] All game state saves correctly
- [ ] Multiple save slots work

**What Should Save:**
- Player position, stats, inventory
- World state (tilled soil, crops, buildings)
- NPC friendship levels
- Quest progress
- Time, date, season
- Skill levels and experience
- Pet data
- Dungeon progress

**How to Test:**
1. Play for a while, make changes
2. Press F5 to quick save
3. Note current game state
4. Exit game
5. Reload game (F9)
6. Verify all state restored

**Expected Results:**
- Save completes quickly (<1 second)
- Toast notification appears
- No data loss

#### 5.2 Load System

**What to Test:**
- [ ] Quick load works (F9)
- [ ] Load from menu works
- [ ] All saved data restores
- [ ] No corruption

**How to Test:**
1. Load existing save
2. Check player position
3. Check inventory contents
4. Check farm state
5. Check NPC positions
6. Verify time/date

**Expected Results:**
- Load completes quickly (<2 seconds)
- All systems restore correctly
- No missing data

#### 5.3 Performance Testing

**What to Test:**
- [ ] Frame rate stable at 60 FPS
- [ ] No stuttering during gameplay
- [ ] No memory leaks
- [ ] Load times acceptable
- [ ] Large farms don't lag

**How to Test:**
1. Press F3 to toggle performance monitor (implemented in Phase 7)
2. Play for 30+ minutes
3. Monitor FPS displayed in performance overlay
4. Monitor memory usage in performance overlay
5. Check frame times in performance overlay

**Performance Targets:**
- FPS: 60 (constant)
- Frame time: ~16.6ms
- Memory: <500 MB
- Load time: <5 seconds

**Expected Results:**
- Smooth 60 FPS gameplay
- No frame drops
- Memory usage stable
- Quick transitions

---

## Regression Testing

### Visual Regressions

Check that nothing visually broke:

- [ ] Sprites render at correct size
- [ ] No missing textures (pink squares)
- [ ] Colors are correct
- [ ] Animations play smoothly
- [ ] UI elements align properly
- [ ] Text is readable
- [ ] Particles appear correctly

### Gameplay Regressions

Check that mechanics still work:

- [ ] All tools work as before
- [ ] Farming loop complete (till → plant → water → harvest)
- [ ] Shop transactions work
- [ ] Crafting works
- [ ] NPCs interact correctly
- [ ] Quests complete
- [ ] Combat functions
- [ ] Save/load preserves state

---

## Known Limitations

Based on ENGINE_MIGRATION_STATUS.md, these items need runtime verification:

1. **Content Loading**: Verify all sprites, fonts, and audio load correctly
2. **Input Handling**: Ensure keyboard/mouse work with custom engine
3. **Rendering**: Check that custom SpriteBatch matches MonoGame behavior
4. **Audio**: Test that sound effects and music play (if audio files present)
5. **Performance**: Verify frame rate matches or exceeds MonoGame version

---

## Reporting Issues

If you find bugs during testing:

1. **Capture details**:
   - What you were doing
   - What you expected
   - What actually happened
   - Any error messages

2. **Check console output**:
   - Look for exceptions
   - Note any warnings
   - Check error stack traces

3. **Reproduce the bug**:
   - Try to trigger it again
   - Note exact steps
   - Check if it's consistent

4. **Document and report**:
   - Open GitHub issue
   - Include reproduction steps
   - Attach screenshots if visual
   - Include log files

---

## Testing Checklist Summary

### Critical Path (Must Work):
- [x] Game launches successfully
- [ ] Player can move
- [ ] HUD displays correctly
- [ ] Can till, plant, water, harvest
- [ ] Inventory works
- [ ] Time progresses
- [ ] Save/load works

### Secondary Features:
- [ ] Crafting system
- [ ] Shop system
- [ ] Building placement
- [ ] NPC interactions
- [ ] Quest system

### Advanced Features:
- [ ] Combat system
- [ ] Pet system
- [ ] Dungeon system
- [ ] Skill system
- [ ] Marriage system

---

## Success Criteria

The custom engine migration is successful if:

1. ✅ Game builds with 0 errors (COMPLETE)
2. ✅ Game launches without crashes
3. ✅ Core gameplay loop works (farming)
4. ✅ All major systems functional
5. ✅ Performance meets targets (60 FPS)
6. ✅ Save/load preserves all data
7. ✅ No visual regressions
8. ✅ No gameplay regressions

---

## Next Steps After Testing

Once runtime testing is complete:

1. Document any issues found
2. Fix critical bugs
3. Optimize performance if needed
4. Add any missing engine features
5. Consider removing MonoGame dependency entirely
6. Add engine-specific enhancements
7. Update all documentation

---

## Conclusion

This guide covers comprehensive testing of the MoonBrook Ridge game after custom engine migration. Follow each phase systematically to ensure all systems work correctly.

For additional help, see:
- [DEVELOPMENT.md](DEVELOPMENT.md) - Development setup
- [CONTROLS.md](CONTROLS.md) - Complete control reference  
- [PLAYTEST_GUIDE.md](PLAYTEST_GUIDE.md) - Playtest instructions
- [ENGINE_MIGRATION_STATUS.md](../../ENGINE_MIGRATION_STATUS.md) - Migration details
