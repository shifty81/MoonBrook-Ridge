# Rendering Troubleshooting Guide

## Overview

This guide helps diagnose and fix rendering issues in MoonBrook Ridge after the custom engine migration.

## Recent Changes

**Improvements Made:**
1. ✅ Added comprehensive diagnostic logging to font loading system
2. ✅ Added diagnostic logging to GameplayState asset loading
3. ✅ Verified Content directory is configured to copy to output
4. ✅ Verified triple-fallback rendering system for tiles

## Expected Behavior

### Title Menu (MenuState)
- **Background**: Dark blue (RGB: 20, 30, 50)
- **Title**: "MoonBrook Ridge" at top (light blue text)
- **Menu Options**: 6 options (New Game, Continue, Load Game, Settings, Mods, Exit)
- **Fallback Mode**: If font fails, shows colored rectangles for menu items

### Game World (GameplayState)
- **Background**: Bright grass green (RGB: 120, 195, 85)
- **World**: Should render grass tiles in center area
- **Minimap**: Small map in top-right corner (black box if not rendering properly)
- **Player**: Character sprite at center
- **HUD**: Health, energy, time, date visible

## Diagnostic Steps

### Step 1: Check Console Output

Run the game and look for these console messages:

```
=== Loading Default Font ===
LoadFont called for: Fonts/Default
Root directory: Content
Looking for .spritefont at: Content/Fonts/Default.spritefont
Found .spritefont file at: Content/Fonts/Default.spritefont
Parsed descriptor - FontName: LiberationSans-Regular.ttf, Size: 14
TTF path resolved to: Content/Fonts/LiberationSans-Regular.ttf
Loading font from: Content/Fonts/LiberationSans-Regular.ttf (size: 14)
Font loaded successfully, has atlas: True
```

**Problem Signs:**
- ❌ `.spritefont file not found` - Content directory not copied to output
- ❌ `TTF font file not found` - Font file missing or path wrong
- ❌ `Creating default font` - Fallback to procedural font
- ❌ `has atlas: False` - Font atlas not created

### Step 2: Check GameplayState Loading

Look for these messages when starting a new game:

```
=== GameplayState LoadContent Started ===
Loading character animations...
Character animations loaded: 13 base, 11 tools
Loading crop textures...
Crop textures loaded: 5 crop types
Loading tile textures...
Tile textures loaded: 16 tile types
Loading Sunnyside World tileset...
Sunnyside tileset loaded: 1024x1024
World map initialized successfully
=== GameplayState LoadContent Completed Successfully ===
```

**Problem Signs:**
- ❌ Missing "LoadContent Started" - State never initialized
- ❌ Exception during texture loading - Content files missing
- ❌ Never reaches "Completed Successfully" - Crash during loading

### Step 3: Verify Content Directory

Check that build output contains Content directory:

```bash
# For .NET builds
ls -la MoonBrookRidge/bin/Debug/net9.0/Content/

# Should contain:
# - Fonts/
#   - Default.spritefont
#   - LiberationSans-Regular.ttf
# - Textures/
#   - Tiles/
#     - sunnyside_tileset.png
#     - grass.png, dirt_01.png, etc.
#   - Characters/
#     - Animations/*.png
#   - Crops/*.png
# - WorldGen/
#   - *.json
```

**Problem Signs:**
- ❌ Content directory missing entirely
- ❌ Empty directories
- ❌ Files not copied

**Fix**: Verify `MoonBrookRidge.csproj` contains:
```xml
<ItemGroup>
  <None Update="Content\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### Step 4: Check for Exceptions

Look for exception stack traces in console output. Common issues:

**FileNotFoundException**
```
System.IO.FileNotFoundException: Could not find file 'Content/Textures/...'
```
→ **Fix**: Content directory not copied to output

**NullReferenceException in Draw methods**
```
NullReferenceException at WorldMap.Draw
```
→ **Fix**: Textures not loaded, check LoadContent completion

**OpenGL errors**
```
GL.ERROR_INVALID_VALUE
```
→ **Fix**: Texture data corrupted or invalid dimensions

## Common Issues and Fixes

### Issue 1: Blue Screen Title Menu

**Symptom**: Title menu is completely blue with no text or menu options

**Diagnosis**:
```
Console shows: "Creating default font for: Fonts/Default"
Console shows: "Default font has atlas: False"
```

**Root Cause**: Default font creation failed to generate atlas

**Fix**: The font system now has better logging. Check:
1. Is StbTrueTypeSharp package installed? 
   ```bash
   dotnet list MoonBrookEngine package | grep StbTrueType
   ```
2. If missing, add it:
   ```bash
   cd MoonBrookEngine
   dotnet add package StbTrueTypeSharp
   ```

### Issue 2: Green Screen with Black Box

**Symptom**: Game world is solid green with a black rectangle in top-right corner

**Diagnosis**:
```
Green background = Normal (RGB: 120, 195, 85)
Black box = Minimap not rendering
```

**Root Cause**: World tiles not rendering, minimap has no data

**Possible Fixes**:
1. **Check texture loading**:
   - Look for errors loading `sunnyside_tileset.png`
   - Verify file exists in output: `bin/.../Content/Textures/Tiles/sunnyside_tileset.png`

2. **Check world initialization**:
   - Verify `WorldMap.LoadSunnysideTileset()` was called
   - Check console for "World map initialized successfully"

3. **Force fallback rendering**:
   - If textures fail, world should still render colored squares
   - If nothing renders, check for exceptions in `WorldMap.Draw()`

### Issue 3: No Player Movement

**Symptom**: Player character doesn't move with WASD/arrow keys

**Possible Causes**:
1. Input not being processed (InputManager issue)
2. Game is paused (check for pause overlay)
3. Player sprite not rendering (but Update still works)
4. Camera not following player correctly

**Debug**:
- Add logging to Update() to verify it's being called
- Check if ESC was pressed (might have paused immediately)
- Verify InputManager.Update() is called before input checks

### Issue 4: Assets Not Rendering

**Symptom**: Some or all sprites don't appear

**Check**:
1. Are textures loaded?
   ```
   Console: "Tile textures loaded: 16 tile types"
   Console: "Character animations loaded: 13 base, 11 tools"
   ```

2. Are textures valid?
   - Width and height > 0
   - Data not null
   - Format supported (PNG should work)

3. Is SpriteBatch.Begin() called before drawing?
   - Every Draw() should have Begin() and End()
   - Check for missing Begin() or double Begin()

## Advanced Debugging

### Enable Verbose Logging

Add to Game1.cs Initialize():
```csharp
Console.WriteLine($"Graphics: {GraphicsDevice.Viewport.Width}x{GraphicsDevice.Viewport.Height}");
Console.WriteLine($"Content Root: {Content.RootDirectory}");
```

### Check OpenGL State

Add to ResourceManager:
```csharp
Console.WriteLine($"GL Version: {_gl.GetStringS(StringName.Version)}");
Console.WriteLine($"GL Vendor: {_gl.GetStringS(StringName.Vendor)}");
```

### Verify SpriteBatch Operations

Add to SpriteBatch.Draw():
```csharp
if (!_isBegun)
    throw new InvalidOperationException("Begin must be called before Draw.");
Console.WriteLine($"Drawing: {texture.Width}x{texture.Height} at {position}");
```

## Testing Checklist

After applying fixes:

- [ ] Build succeeds with 0 errors
- [ ] Content directory copied to output
- [ ] Game window opens
- [ ] Console shows font loaded successfully
- [ ] Console shows GameplayState loaded successfully
- [ ] Title menu appears with text
- [ ] Can navigate menu with keyboard/mouse
- [ ] New Game loads into world
- [ ] World tiles render (grass, dirt, etc.)
- [ ] Player sprite visible at center
- [ ] HUD shows stats
- [ ] Player moves with WASD
- [ ] Minimap shows world overview

## Getting Help

If issues persist:

1. **Collect logs**: Copy ALL console output to a file
2. **Take screenshots**: Especially of the blank screens
3. **Check system**: 
   - OS version
   - .NET SDK version (`dotnet --version`)
   - Graphics card/driver
4. **Share findings**: Include logs, screenshots, and system info

## Known Limitations

- **Headless environments**: Game requires graphics device, won't run in CI without Xvfb
- **Font rendering**: Default procedural fonts are basic, TTF fonts are much better
- **Performance**: First launch may be slow while loading ~1600 content files
- **OpenGL version**: Requires OpenGL 3.3+ (most systems from 2010+ support this)
