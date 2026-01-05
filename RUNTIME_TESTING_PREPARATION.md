# Runtime Testing Preparation Guide

**Date**: January 5, 2026  
**Status**: ✅ Ready for Runtime Testing  
**PR**: #82 - Next Steps Follow-up

---

## Purpose

This document prepares developers for runtime testing after the successful completion of the custom engine migration. Since runtime testing requires a graphical environment (X11/Wayland on Linux or native Windows), this guide ensures developers have everything they need to proceed efficiently.

---

## Prerequisites Met ✅

### 1. Build Status
- ✅ **0 compilation errors**
- ✅ **0 build warnings**
- ✅ All projects build successfully
- ✅ All critical engine files present
- ✅ Engine implementations complete

### 2. Validation Tooling
- ✅ `validate-engine.sh` - Automated validation script
- ✅ Validates build, files, and engine completeness
- ✅ Cross-platform support (Unix/Windows)

### 3. Documentation
- ✅ [RUNTIME_TESTING_GUIDE.md](docs/guides/RUNTIME_TESTING_GUIDE.md) - Comprehensive testing procedures (50+ test cases)
- ✅ [ENGINE_MIGRATION_STATUS.md](ENGINE_MIGRATION_STATUS.md) - Migration completion status
- ✅ [NEXT_STEPS_COMPLETION.md](NEXT_STEPS_COMPLETION.md) - Summary of completed work
- ✅ [README.md](README.md) - Updated with engine migration information

---

## Quick Start for Runtime Testing

### Step 1: Validate Build
```bash
./validate-engine.sh
```

Expected output:
```
✅ All critical checks passed!
Build Status: Build succeeded.
✅ Engine migration validation: PASSED
```

### Step 2: Run the Game
```bash
./play.sh
```

Or manually:
```bash
cd MoonBrookRidge
dotnet run
```

### Step 3: Follow Testing Guide
Open and follow [RUNTIME_TESTING_GUIDE.md](docs/guides/RUNTIME_TESTING_GUIDE.md) which includes:
- Phase 1: Basic Initialization & Launch
- Phase 2: Core Systems (Movement, Camera, HUD, Time)
- Phase 3: Gameplay Systems (Farming, Tools, Inventory, Crafting, Shop, Building)
- Phase 4: Advanced Features (NPCs, Quests, Combat, Pets, Dungeons)
- Phase 5: Save/Load & Performance

---

## System Requirements

### Minimum Requirements
- **OS**: Windows 10/11, Ubuntu 20.04+, or macOS 10.15+
- **Display**: Graphical environment (X11, Wayland, or native Windows)
- **.NET**: .NET 9.0 SDK or later
- **RAM**: 2GB minimum
- **GPU**: OpenGL 3.0+ support

### Recommended for Testing
- **OS**: Windows 11 or Ubuntu 22.04+
- **RAM**: 4GB+
- **GPU**: Dedicated GPU with OpenGL 4.0+ support
- **Display**: 1920x1080 resolution

---

## Testing Checklist

### Phase 1: Basic Functionality ✅ (Ready to Test)
- [ ] Game launches without crashes
- [ ] Window displays correctly (1280x720)
- [ ] Main menu is accessible
- [ ] Graphics render properly
- [ ] No console errors during startup

### Phase 2: Core Systems ✅ (Ready to Test)
- [ ] Player movement (WASD/Arrow keys)
- [ ] Camera follows player smoothly
- [ ] HUD displays correctly (health, energy, time, money)
- [ ] Time system advances properly
- [ ] Day/night cycle works

### Phase 3: Gameplay Systems ✅ (Ready to Test)
- [ ] Farming mechanics (till, plant, water, harvest)
- [ ] Tool system (hoe, watering can, axe, pickaxe, scythe, fishing rod)
- [ ] Inventory management
- [ ] Crafting system (K key)
- [ ] Shop system (B key)
- [ ] Building placement (H key)

### Phase 4: Advanced Features ✅ (Ready to Test)
- [ ] NPC interactions and dialogue
- [ ] Quest system (F key)
- [ ] Combat system (Space key)
- [ ] Pet/companion system (P key)
- [ ] Dungeon exploration (8 dungeon types)
- [ ] Magic and alchemy systems

### Phase 5: Data Persistence ✅ (Ready to Test)
- [ ] Save game functionality (F5)
- [ ] Load game functionality (F9)
- [ ] Auto-save system (every 5 minutes)
- [ ] Game state persists correctly

### Phase 6: Performance ✅ (Ready to Test)
- [ ] Consistent 60 FPS (target)
- [ ] No memory leaks during extended play
- [ ] Smooth camera movement
- [ ] No visual stuttering
- [ ] Fast loading times (<3 seconds)

---

## Known Considerations

### Engine Migration Specifics

1. **Custom Engine vs MonoGame**
   - The game now runs on a custom engine with MonoGame API compatibility
   - Some behavior differences may exist (report if found)
   - Performance characteristics may differ from original MonoGame

2. **Graphics Pipeline**
   - Uses OpenGL via SDL2 backend
   - Texture loading via StbImageSharp
   - Font rendering via custom implementation

3. **Input System**
   - Keyboard and mouse input handled by custom engine
   - All game controls should work identically
   - Report any input lag or missed inputs

---

## Issue Reporting Template

If you encounter issues during runtime testing, please report using this template:

```markdown
### Issue Title
Brief description of the problem

### Category
- [ ] Crash/Exception
- [ ] Visual/Rendering
- [ ] Performance
- [ ] Gameplay Mechanic
- [ ] Input/Controls
- [ ] Save/Load
- [ ] Audio
- [ ] Other

### Steps to Reproduce
1. Launch game
2. [Specific actions taken]
3. [When issue occurs]

### Expected Behavior
What should happen

### Actual Behavior
What actually happens

### Environment
- OS: [e.g., Windows 11, Ubuntu 22.04]
- .NET Version: [run `dotnet --version`]
- GPU: [if relevant]
- Display Resolution: [if relevant]

### Console Output
```
[Paste any error messages or relevant console output]
```

### Screenshots
[Attach screenshots if applicable]

### Additional Context
Any other relevant information
```

---

## Performance Monitoring

### Built-in Performance Tools

1. **Performance Monitor** (F3 key)
   - Real-time FPS counter
   - Memory usage tracking
   - Frame timing metrics
   - Entity count display

2. **Console Output**
   - Watch for warnings or errors
   - Check for performance bottlenecks
   - Monitor resource loading times

### Expected Performance Targets

| Metric | Target | Minimum Acceptable |
|--------|--------|-------------------|
| FPS | 60 | 30 |
| Frame Time | 16.67ms | 33ms |
| Memory Usage | < 500MB | < 1GB |
| Load Time | < 3s | < 10s |

---

## Development Environment Setup

### For New Developers

1. **Clone Repository**
   ```bash
   git clone https://github.com/shifty81/MoonBrook-Ridge.git
   cd MoonBrook-Ridge
   ```

2. **Install .NET SDK**
   - Download from [dotnet.microsoft.com](https://dotnet.microsoft.com/download/dotnet/9.0)
   - Verify: `dotnet --version` (should be 9.0+)

3. **Restore Dependencies**
   ```bash
   cd MoonBrookRidge
   dotnet restore
   ```

4. **Build Project**
   ```bash
   dotnet build
   ```

5. **Validate Engine**
   ```bash
   cd ..
   ./validate-engine.sh
   ```

6. **Run Game**
   ```bash
   ./play.sh
   ```

---

## Testing Tools and Scripts

### Available Scripts

1. **`validate-engine.sh`**
   - Comprehensive validation of build and engine
   - Run before any testing session
   - Confirms all prerequisites met

2. **`play.sh`**
   - Convenient launcher for the game
   - Handles directory navigation
   - Cross-platform compatible

3. **`test-build.sh`**
   - Tests build in both Debug and Release configurations
   - Validates compilation success

---

## Next Steps After Runtime Testing

### If Testing Succeeds ✅
1. Mark runtime testing as complete in ENGINE_MIGRATION_STATUS.md
2. Update overall migration progress to 100%
3. Close migration-related issues/PRs
4. Begin next development phase (new features, optimizations)

### If Issues Found ⚠️
1. Document all issues with reproduction steps
2. Categorize by severity (Critical, High, Medium, Low)
3. Create GitHub issues for each problem
4. Prioritize critical issues for immediate fixing
5. Re-test after fixes applied

### Critical Issues (Must Fix Before Completion)
- Game crashes on launch
- Cannot save/load game
- Major visual corruption
- Complete system failures (e.g., no player movement)
- Performance < 30 FPS on target hardware

### Non-Critical Issues (Can Defer)
- Minor visual glitches
- Occasional stutters
- Polish and UX improvements
- Feature enhancements

---

## Automation Opportunities

### Potential CI/CD Additions

1. **Automated Build Testing**
   - Already covered by `validate-engine.sh`
   - Can be integrated into CI pipeline

2. **Headless Testing** (Future)
   - Unit tests for game logic
   - Integration tests for systems
   - Mock graphics for automated testing

3. **Performance Benchmarking** (Future)
   - Automated performance tests
   - Regression detection
   - Memory leak detection

---

## Resources

### Documentation
- [RUNTIME_TESTING_GUIDE.md](docs/guides/RUNTIME_TESTING_GUIDE.md) - Detailed testing procedures
- [ENGINE_MIGRATION_STATUS.md](ENGINE_MIGRATION_STATUS.md) - Migration status and history
- [ARCHITECTURE.md](docs/architecture/ARCHITECTURE.md) - System architecture
- [DEVELOPMENT.md](docs/guides/DEVELOPMENT.md) - Development guidelines
- [CONTROLS.md](docs/guides/CONTROLS.md) - Complete control reference

### Related PRs
- PR #81: "continue next steps please work" - Validation and testing infrastructure
- PR #80: Previous engine migration work
- PR #82: This PR - Next steps follow-up

---

## Success Criteria

The custom engine migration is considered **complete** when:

- [x] Build succeeds with 0 errors and 0 warnings ✅
- [x] Validation script passes all checks ✅
- [x] Comprehensive testing guide created ✅
- [x] Documentation updated ✅
- [ ] Game launches and displays main menu 🔄 *Ready to test*
- [ ] Player can move and interact 🔄 *Ready to test*
- [ ] All core systems function correctly 🔄 *Ready to test*
- [ ] Performance meets targets (60 FPS) 🔄 *Ready to test*
- [ ] Save/load works correctly 🔄 *Ready to test*
- [ ] No critical bugs or regressions 🔄 *Ready to test*

**Current Status**: 📊 **Compilation Complete (100%)** | Runtime Testing Ready (0%)  
**Overall Progress**: ~95% complete (pending runtime verification)

---

## Contact and Support

### For Questions or Issues
- Create a GitHub issue: https://github.com/shifty81/MoonBrook-Ridge/issues
- Tag with appropriate labels: `engine-migration`, `runtime-testing`, `bug`, etc.
- Reference this document and the testing guide

### For Documentation Updates
- Update relevant .md files in the repository
- Submit PR with clear description
- Follow [CONTRIBUTING.md](CONTRIBUTING.md) guidelines

---

**Ready to Test**: All prerequisites met. Game is ready for runtime testing in a graphical environment.

**Date**: January 5, 2026  
**Next Action**: Runtime testing by developer with graphical environment access
