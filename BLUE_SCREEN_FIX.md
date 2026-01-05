# Blue Screen Fix - Menu Not Showing

## Problem
When running the game from Visual Studio, the game window opens but shows only a blue screen with no menu options or game content.

**MOST COMMON CAUSE**: The Content directory (containing textures and fonts) is not being copied to the build output directory, causing `FileNotFoundException` when trying to load assets.

## Root Cause
The issue can be caused by one or more of the following:

### 1. Content Files Not Copied to Output (MOST COMMON)
The `.csproj` file must be configured to copy the entire `Content` directory to the build output. Without this, the game cannot find textures, fonts, or other assets at runtime, resulting in `FileNotFoundException` errors.

**Fix**: Ensure `MoonBrookRidge.csproj` contains:
```xml
<ItemGroup>
  <!-- Include entire Content directory in output -->
  <None Update="Content\**\*">
    <CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory>
  </None>
</ItemGroup>
```

### 2. Font Loading Problems
The game uses a custom engine (`MoonBrookEngine`) with MonoGame compatibility:
- Font loading was failing or creating incomplete fonts without proper atlas textures
- When `SpriteBatch.DrawString()` was called with a broken font, it would silently return without drawing anything
- This resulted in a blue screen (the background color) with no visible menu text

## Solution
We've implemented multiple layers of fixes to ensure the menu is always visible:

### 0. Content Directory Configuration (PRIMARY FIX)
- Updated `MoonBrookRidge.csproj` to copy the entire `Content` directory to the build output
- Previously only `WorldGen/*.json` files were copied, causing all texture and font loads to fail
- This is the most important fix and resolves the majority of blue screen issues

### 1. Enhanced Font Character Support
- Upgraded `SimpleFontRasterizer.cs` to support the complete ASCII character set
- Added all uppercase letters (A-Z), all numbers (0-9), and common punctuation
- Previously only supported a handful of characters, causing most text to render as boxes

### 2. Improved Menu Fallback Rendering
- Added font health check in `MenuState.cs` to detect when fonts aren't working
- If font measurement returns zero or invalid values, the menu switches to fallback mode
- Enhanced `DrawSimpleMenu()` to draw clear colored rectangles representing menu items:
  - Light blue title bar at the top
  - White rectangles for each menu option
  - Yellow border around the selected option
  - Gray bar at the bottom for hints

### 3. Better Error Logging
- Added console output to track font loading progress
- Shows whether font loaded successfully, has internal atlas, and can measure strings
- Helps diagnose font-related issues when they occur

## What You Should See Now

### If Font Works (Best Case)
You should see the full menu with text:
- "MoonBrook Ridge" title at top
- Menu options: New Game, Continue, Load Game, Settings, Mods, Exit
- Yellow selection indicator (">") next to the selected option
- Control hints at the bottom

### If Font Fails (Fallback Mode)
You should see a functional menu with colored rectangles:
- Light blue rectangle at top (title)
- 6 white rectangles representing menu options
- Yellow border around the selected option
- Gray rectangle at bottom (hints)
- You can still navigate and select options using arrow keys, WASD, or mouse

## How to Use the Menu
- **Navigate**: Arrow keys, WASD, or move mouse over options
- **Select**: Press C, X, or click with mouse
- **Exit**: ESC key (closes active menu, doesn't exit game from main menu)

## Testing the Fix
1. Build the solution in Visual Studio (Build → Build Solution)
2. Run the game (Debug → Start Debugging or F5)
3. You should see either:
   - Full text menu (if font works)
   - Colored rectangle menu (if font fallback activated)
4. Try navigating and selecting options to confirm functionality

## If You Still See a Blue Screen
If you still see only a blue screen with no menu:

1. **Check Console Output**: Look at the Output window in Visual Studio for error messages
2. **Check for Exceptions**: Look for any red exception messages in the console
3. **Verify Content Files**: Make sure the `Content` directory is being copied to the build output
   - Check that `bin/Debug/net9.0/Content/` exists and contains Textures and Fonts subdirectories
   - The `.csproj` file should have: `<None Update="Content\**\*"><CopyToOutputDirectory>PreserveNewest</CopyToOutputDirectory></None>`
4. **Verify Specific Files**: Ensure `Content/Fonts/Default.spritefont` and `Content/Textures/Characters/Animations/base_walk_strip8.png` exist in the output directory
5. **Check Build Output**: Ensure all projects built successfully

## Technical Details

### Files Modified
- `MoonBrookEngine/Graphics/SimpleFontRasterizer.cs` - Added complete character set
- `MoonBrookRidge/Core/States/MenuState.cs` - Added font health check and improved fallback
- `MoonBrookRidge/Game1.cs` - Added font loading diagnostics
- `MoonBrookRidge/MoonBrookRidge.csproj` - **CRITICAL FIX**: Updated to copy entire Content directory to output (previously only copied WorldGen JSON files)

### Architecture Notes
The game uses a custom engine with MonoGame API compatibility:
- `MoonBrookEngine` - Core engine with Silk.NET windowing and OpenGL
- `MoonBrookRidge.Engine` - MonoGame compatibility layer
- `MoonBrookRidge` - Main game code

Font loading goes through several layers:
1. `Game1.LoadContent()` calls `Content.Load<SpriteFont>("Fonts/Default")`
2. `ContentManager` tries to load the font from disk
3. If loading fails, `BitmapFont.CreateDefault()` creates a runtime-generated font
4. `SimpleFontRasterizer` generates character glyphs procedurally
5. If the font atlas is missing or broken, `DrawString()` returns early
6. Our fix detects this and uses `DrawSimpleMenu()` instead

## Future Improvements
- Package a working TTF font with the game
- Implement proper SpriteFont atlas rendering
- Add more diagnostic information to help troubleshoot font issues
- Consider pre-generating font atlases at build time
