# Development & Testing Setup

## Overview
This document provides setup instructions for developers and testers working on MoonBrook Ridge.

## 🎮 Playtest Scripts

### Quick Launch
The fastest way to play the game:
```bash
./play.sh
```
This script:
- Builds the game in Release mode
- Runs it automatically
- Shows clear error messages if build fails

### Build Verification
Test that the game compiles without running it:
```bash
./test-build.sh
```
Useful for:
- CI/CD pipelines
- Quick validation after code changes
- Checking for compilation errors

## 🔧 IDE Setup

### Visual Studio (Windows)
1. Open `MoonBrookRidge.sln`
2. Set `MoonBrookRidge` as startup project
3. Press `F5` to run with debugging
4. Press `Ctrl+F5` to run without debugging

### Visual Studio Code
1. Open the repository root folder
2. Install C# extension (ms-dotnettools.csharp)
3. Install .NET Extension Pack
4. Use Run and Debug panel (Ctrl+Shift+D)
5. Select ".NET Core Launch (console)" configuration

### JetBrains Rider
1. Open `MoonBrookRidge.sln`
2. Rider auto-configures run configurations
3. Press `Shift+F10` to run
4. Press `Shift+F9` to debug

## 🧪 Testing Workflow

### Daily Development Testing
```bash
# Quick compile check
./test-build.sh

# Full playtest
./play.sh
```

### Feature Testing Checklist
When implementing a new feature:

1. **Build Test**
   ```bash
   ./test-build.sh
   ```

2. **Code Review** - Check your changes
   ```bash
   git diff
   ```

3. **Run Game** - Test the feature
   ```bash
   ./play.sh
   ```

4. **Test Scenarios** - From PLAYTEST_GUIDE.md
   - Basic functionality
   - Edge cases
   - Integration with existing systems

## 🐛 Debugging

### Logging
Add debug output using:
```csharp
System.Diagnostics.Debug.WriteLine($"Debug info: {value}");
```

View output in:
- **Visual Studio**: Debug → Windows → Output
- **VS Code**: Debug Console
- **Rider**: Debug tool window

### Breakpoints
Set breakpoints in your IDE:
- **Visual Studio**: Click left margin or F9
- **VS Code**: Click left margin
- **Rider**: Click left margin or Ctrl+F8

### Performance Profiling
Monitor frame rate and performance:
```csharp
// In Game1.cs Update method
var fps = 1.0 / gameTime.ElapsedGameTime.TotalSeconds;
Debug.WriteLine($"FPS: {fps:F2}");
```

## 📝 Common Development Tasks

### Adding New Content
```bash
cd MoonBrookRidge/Content
# Add your .png files here

# Edit Content.mgcb to add them
# Then rebuild
cd ..
dotnet build
```

### Cleaning Build Artifacts
```bash
cd MoonBrookRidge
dotnet clean
rm -rf bin/ obj/
dotnet build
```

### Dependency Updates
```bash
cd MoonBrookRidge
dotnet restore --force
dotnet build
```

## 🎯 Current Feature Status

### ✅ Fully Functional
- Player movement and camera
- Time system with seasons
- Hunger/thirst mechanics
- Tool system (Hoe, Watering Can, Scythe)
- Seed planting system
- Crop growth based on game time
- Consumable items (food/drinks)
- Collision detection
- HUD display

### 🚧 In Progress
- Harvesting to inventory
- Save/load system
- Pause menu

### 📋 Planned
- Mining system (Pickaxe)
- Tree chopping (Axe)
- NPC interactions
- Shop system
- Quest system

## 💡 Optimization Tips

### Build Performance
```bash
# Incremental builds (faster)
dotnet build

# Full rebuild (when needed)
dotnet clean
dotnet build
```

### Runtime Performance
- Use Release configuration for performance testing
- Debug configuration has additional checks

```bash
# Release mode (faster, less debugging)
dotnet run --configuration Release

# Debug mode (slower, better debugging)
dotnet run --configuration Debug
```

## 🔍 Troubleshooting

### "MonoGame not found"
```bash
dotnet tool install --global dotnet-mgcb-editor
```

### Content Pipeline Errors
1. Check file paths in Content.mgcb
2. Ensure .png files exist
3. Verify file permissions
4. Try rebuilding content:
   ```bash
   cd MoonBrookRidge/Content
   dotnet mgcb Content.mgcb /rebuild
   ```

### Runtime Crashes
1. Check Debug output for exceptions
2. Verify all content files loaded
3. Check for null references in new code
4. Use try-catch blocks for debugging:
   ```csharp
   try {
       // Your code
   } catch (Exception ex) {
       Debug.WriteLine($"Error: {ex.Message}");
       Debug.WriteLine(ex.StackTrace);
   }
   ```

### Performance Issues
- Profile with Release build, not Debug
- Check for memory leaks (unloaded textures)
- Monitor update/draw time
- Use sprite batching efficiently

## 📚 Additional Resources

- [MonoGame Documentation](https://docs.monogame.net/)
- [C# Programming Guide](https://docs.microsoft.com/en-us/dotnet/csharp/)
- [PLAYTEST_GUIDE.md](PLAYTEST_GUIDE.md) - Feature testing guide
- [DEVELOPMENT.md](DEVELOPMENT.md) - Code structure guide
- [ASSET_LOADING_GUIDE.md](ASSET_LOADING_GUIDE.md) - Asset pipeline guide

## 🤝 Contributing

When adding features:
1. Test with `./test-build.sh`
2. Playtest thoroughly
3. Update PLAYTEST_GUIDE.md if needed
4. Document any new controls or mechanics
5. Check for compilation warnings
