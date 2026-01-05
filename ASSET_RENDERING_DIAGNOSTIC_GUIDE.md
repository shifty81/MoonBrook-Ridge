# Asset Rendering Diagnostic Guide
**Date**: January 5, 2026  
**Purpose**: Help diagnose and fix font and asset rendering issues

---

## Problem Statement

User reports:
- Fonts not displaying on screen
- World shows only green and black background
- No game assets appearing
- Unable to test gameplay elements

## How to Use This Guide

1. **Run the game**: `cd MoonBrookRidge && dotnet run`
2. **Watch the console output** - look for the diagnostic messages documented below
3. **Find your issue** in the "Diagnostic Patterns" section
4. **Apply the corresponding fix**

---

## Expected Console Output (Successful Case)

### Font Loading
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
Atlas texture: 512x512
Font test measurement: 28x14
=== Font Loading Complete ===
```

### Texture Loading
```
[ResourceManager] Loading texture from: Content/Textures/Tiles/grass.png
[ResourceManager] Texture 'Textures/Tiles/grass' loaded successfully (16x16)
[ResourceManager] Loading texture from: Content/Textures/Tiles/sunnyside_tileset.png
[ResourceManager] Texture 'Textures/Tiles/sunnyside_tileset' loaded successfully (1024x1024)
```

### World Map Initialization
```
[WorldMap] Loading Sunnyside tileset: 1024x1024
[WorldMap] Sunnyside tileset helper created: 4096 tiles available
[WorldMap] Sunnyside tile mapping initialized: 14 tile types mapped
```

### First Draw
```
[WorldMap] First Draw call:
  - Sunnyside tileset: Available
  - Tile textures: 18 loaded
  - Tile mapping: 14 entries
  - Drawing tiles from (70,35) to (120,80)
```

---

## Diagnostic Patterns

### Issue 1: Font Has No Atlas

**Console Output:**
```
Font loaded successfully, has atlas: False
[SpriteBatch] WARNING: DrawString called with font that has no atlas! Font will not render.
```

**Cause:** Font loaded but texture atlas wasn't generated properly

**Fix:**
1. Check if LiberationSans-Regular.ttf is valid (should be 410KB)
2. Verify StbTrueTypeSharp package is installed
3. Check ResourceManager.LoadFont() error messages for clues

**Alternative Fix:**
Enable the default font fallback by ensuring BitmapFont.CreateDefault() is called

---

### Issue 2: Font File Not Found

**Console Output:**
```
.spritefont file not found at: Content/Fonts/Default.spritefont
Creating default font for: Fonts/Default
```

**Cause:** .spritefont descriptor file missing

**Fix:**
1. Verify Content/Fonts/Default.spritefont exists
2. Check that Content directory is being copied to bin/Debug/net9.0/
3. Rebuild: `dotnet build --no-incremental`

---

### Issue 3: TTF Font File Not Found

**Console Output:**
```
Found .spritefont file at: Content/Fonts/Default.spritefont
Warning: TTF font file not found: LiberationSans-Regular.ttf
Creating default font for: Fonts/Default
```

**Cause:** TTF font file missing

**Fix:**
1. Verify Content/Fonts/LiberationSans-Regular.ttf exists (should be 410KB)
2. Check MoonBrookRidge.csproj has: `<None Update="Content\**\*"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>`
3. Rebuild: `dotnet clean && dotnet build`

---

### Issue 4: Textures Not Loading

**Console Output:**
```
[ResourceManager] ERROR: Could not find texture: Textures/Tiles/grass
[ResourceManager] Searched: Content/Textures/Tiles/grass with extensions .png, .jpg, .jpeg, .bmp
```

**Cause:** Texture files missing from Content directory

**Fix:**
1. Verify texture exists: `ls -la MoonBrookRidge/Content/Textures/Tiles/grass.png`
2. Check Content.mgcb includes the texture
3. Rebuild content pipeline: `dotnet mgcb-editor Content/Content.mgcb` (if available)
4. Or copy texture manually to bin/Debug/net9.0/Content/Textures/Tiles/

---

### Issue 5: Sunnyside Tileset NULL

**Console Output:**
```
[WorldMap] First Draw call:
  - Sunnyside tileset: NULL
  - Tile textures: 18 loaded
```

**Cause:** LoadSunnysideTileset() wasn't called or tileset texture failed to load

**Fix:**
1. Check that GameplayState.LoadContent() calls: `_worldMap.LoadSunnysideTileset(sunnysideTileset);`
2. Verify tileset loaded: Look for `[ResourceManager] Texture 'Textures/Tiles/sunnyside_tileset' loaded successfully`
3. Check file exists: `ls -la MoonBrookRidge/bin/Debug/net9.0/Content/Textures/Tiles/sunnyside_tileset.png`

---

### Issue 6: No Tiles Drawing

**Console Output:**
```
[WorldMap] First Draw call:
  - Sunnyside tileset: Available
  - Tile textures: 0 loaded
  - Tile mapping: 0 entries
```

**Cause:** Tileset initialized but mapping not created

**Fix:**
This is a code bug. The InitializeSunnysideTileMapping() method should populate _sunnysideTileMapping.
Check WorldMap.cs line ~221 for the mapping initialization.

---

### Issue 7: Green Screen Only (No Rendering)

**Symptoms:**
- Console shows all assets loaded successfully
- No error messages
- Screen is pure green with no sprites

**Possible Causes:**
1. **SpriteBatch.Begin() not called** - Check GameplayState.Draw()
2. **Camera off-screen** - Check Camera2D position
3. **Z-ordering issue** - Sprites drawn but behind background
4. **GL context issue** - Engine SpriteBatch not rendering

**Diagnostic Steps:**
1. Add debug output in GameplayState.Draw() to verify it's being called
2. Check camera position: Should be near player (125, 125) for new map
3. Try disabling camera: Draw without camera transform
4. Check Engine SpriteBatch.End() is being called

---

### Issue 8: Black Box in Corner (Minimap)

**This is NORMAL!**

The black box in the top-right corner is the minimap (Phase 7 feature).
If you see this, the game IS rendering! The issue is elsewhere.

**To verify:**
- The minimap should be ~150x150 pixels
- It should show a simplified version of the world
- Press Tab to toggle it on/off

---

## Manual Verification Steps

### Step 1: Verify Files Exist
```bash
cd MoonBrookRidge

# Check font files
ls -lh bin/Debug/net9.0/Content/Fonts/LiberationSans-Regular.ttf
ls -lh bin/Debug/net9.0/Content/Fonts/Default.spritefont

# Check tileset
ls -lh bin/Debug/net9.0/Content/Textures/Tiles/sunnyside_tileset.png

# Check individual tiles
ls -lh bin/Debug/net9.0/Content/Textures/Tiles/grass.png
```

Expected output:
- LiberationSans-Regular.ttf: 410KB
- Default.spritefont: 2-3KB (XML file)
- sunnyside_tileset.png: 131KB (1024x1024)
- grass.png: Few KB (16x16)

### Step 2: Test Font Rendering
Add this test to Game1.cs Draw() method:
```csharp
protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(new Color(120, 195, 85));
    
    _spriteBatch.Begin();
    if (_defaultFont != null)
    {
        _spriteBatch.DrawString(_defaultFont, "TEST TEXT", new Vector2(100, 100), Color.Red);
    }
    _spriteBatch.End();
    
    base.Draw(gameTime);
}
```

If you see "TEST TEXT" in red at position (100,100), fonts are working!

### Step 3: Test Texture Rendering
Add this test to Game1.cs Draw() method:
```csharp
protected override void Draw(GameTime gameTime)
{
    GraphicsDevice.Clear(new Color(120, 195, 85));
    
    _spriteBatch.Begin();
    try
    {
        var testTexture = Content.Load<Texture2D>("Textures/Tiles/grass");
        _spriteBatch.Draw(testTexture, new Vector2(200, 200), Color.White);
    }
    catch (Exception ex)
    {
        Console.WriteLine($"Test texture failed: {ex.Message}");
    }
    _spriteBatch.End();
    
    base.Draw(gameTime);
}
```

If you see a grass tile at position (200,200), textures are working!

---

## Common Fixes

### Fix 1: Rebuild Content
```bash
cd MoonBrookRidge
dotnet clean
rm -rf bin obj
dotnet build
```

### Fix 2: Force Content Copy
If Content isn't copying, verify .csproj:
```xml
<ItemGroup>
  <None Update="Content\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### Fix 3: Check Content.mgcb
The Content.mgcb file should include:
```
#begin Fonts/LiberationSans-Regular.ttf
/copy:Fonts/LiberationSans-Regular.ttf

#begin Textures/Tiles/sunnyside_tileset.png
/importer:TextureImporter
/processor:TextureProcessor
/build:Textures/Tiles/sunnyside_tileset.png
```

---

## Advanced Debugging

### Enable More Verbose Logging

Add to Game1.cs Initialize():
```csharp
protected override void Initialize()
{
    Console.WriteLine("=== GAME INITIALIZATION ===");
    Console.WriteLine($"Graphics: {_graphics.PreferredBackBufferWidth}x{_graphics.PreferredBackBufferHeight}");
    Console.WriteLine($"Content Root: {ContentRootDirectory}");
    Console.WriteLine($"Working Directory: {Directory.GetCurrentDirectory()}");
    
    base.Initialize();
}
```

### Check OpenGL Context

Add to MoonBrookEngine Texture2D constructor:
```csharp
public Texture2D(GL gl, string path)
{
    Console.WriteLine($"[Texture2D] Creating texture from: {path}");
    Console.WriteLine($"[Texture2D] GL context: {gl != null}");
    
    // ... existing code
    
    Console.WriteLine($"[Texture2D] Texture handle: {_handle}");
    Console.WriteLine($"[Texture2D] Texture bound successfully");
}
```

---

## Still Not Working?

If you've tried everything above and the game still doesn't render:

1. **Take a screenshot** of what you see
2. **Copy the console output** to a file: `dotnet run > output.txt 2>&1`
3. **Report the issue** with:
   - Screenshot
   - Console output
   - Operating system
   - .NET version: `dotnet --version`
   - Graphics card info

The diagnostic logging we added should pinpoint exactly where the rendering pipeline is failing.

---

## Success Indicators

You'll know the fix worked when you see:

✅ **Fonts:**
- Menu text is readable
- HUD displays stats
- No warning messages about null fonts or missing atlases

✅ **Tiles:**
- World shows textured grass/dirt tiles (not just colored squares)
- Variety in tile appearances
- World map message shows "Sunnyside tileset: Available"

✅ **Sprites:**
- Player character visible and animated
- Buildings, trees, rocks visible
- Crops grow and show different stages

✅ **UI:**
- Minimap in top-right corner shows world layout
- Menus display correctly
- All text is readable

---

## Reference

**Related Documents:**
- RENDERING_FIX_SUMMARY.md - Previous rendering investigation
- RENDERING_TROUBLESHOOTING_GUIDE.md - General troubleshooting guide
- RUNTIME_STATUS_ASSESSMENT.md - Runtime verification status
- ASSET_IMPLEMENTATION_REVIEW.md - Asset loading review

**Modified Files (This Fix):**
- MoonBrookEngine/Core/ResourceManager.cs - Added texture loading diagnostics
- MoonBrookRidge.Engine/MonoGameCompat/SpriteBatch.cs - Added font rendering diagnostics
- MoonBrookRidge/World/Maps/WorldMap.cs - Added tileset and rendering diagnostics

---

*Last Updated: January 5, 2026*
