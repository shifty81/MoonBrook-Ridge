# Sprite Sheet Extraction Fix

## Problem Statement

Trees and rocks were being rendered as complete tilesheets instead of individual sprites, causing massive clumping and vertical lines of repeated images.

## Root Cause

The `WorldObject` class was loading entire texture files and using the full texture dimensions as the `SourceRectangle`. Since tree and rock textures are actually sprite sheets containing multiple individual sprites, this caused the entire sheet to be drawn at each position.

### Before Fix

```
Tree Texture File: Tree1.png (1536x256 pixels)
┌──────────────────────────────────────────────────┐
│ [Tree] [Tree] [Tree] [Tree] [Tree] [Tree]        │
│  256px  256px  256px  256px  256px  256px         │
└──────────────────────────────────────────────────┘
          Full width: 1536 pixels

WorldObject was drawing ALL of this at each tree position!
```

### After Fix

```
Tree Texture File: Tree1.png (1536x256 pixels)
┌──────────────────────────────────────────────────┐
│ [Tree] [Tree] [Tree] [Tree] [Tree] [Tree]        │
│   ↓      ↓      ↓      ↓      ↓      ↓           │
└──────────────────────────────────────────────────┘
     Each extracted as individual 256x256 sprite:
     
     Tree1_0: Rectangle(0, 0, 256, 256)
     Tree1_1: Rectangle(256, 0, 256, 256)
     Tree1_2: Rectangle(512, 0, 256, 256)
     ... and so on

WorldObject now draws only ONE sprite per position!
```

## Solution

### 1. Created `SpriteSheetExtractor` Utility Class

Location: `MoonBrookRidge/Core/Systems/SpriteSheetExtractor.cs`

This utility provides two extraction methods:

- **`ExtractSpritesFromHorizontalStrip`**: For horizontal sprite strips (like trees)
- **`ExtractSpritesFromGrid`**: For grid-based sprite sheets (like rocks)

### 2. Updated `WorldObject` Class

Added a new constructor that accepts `SpriteInfo`:

```csharp
public WorldObject(string name, Vector2 position, SpriteInfo spriteInfo)
{
    // Uses the texture and source rectangle from SpriteInfo
    Texture = spriteInfo.Texture;
    SourceRectangle = spriteInfo.SourceRectangle;  // Now only draws a portion!
}
```

### 3. Modified Resource Loading in `GameplayState.cs`

**Trees:**
- Tree1 & Tree2 (1536x256): Extracts 6 sprites of 256x256 each
- Tree3 & Tree4 (1536x192): Extracts 8 sprites of 192x192 each
- **Total: 28 unique tree sprites**

**Rocks:**
- Rock1, Rock2, Rock3 (128x128): Extracts 4 sprites (2x2 grid) of 64x64 each
- **Total: 12 unique rock sprites**

### 4. Updated `WorldMap.cs`

Modified `PopulateSunnysideWorldObjects` to:
- Accept `Dictionary<string, SpriteInfo>` instead of `Dictionary<string, Texture2D>`
- Randomly select from all available extracted sprites
- Create natural variety with 28 tree types and 12 rock types

## Results

### Sprite Extraction Summary

| Asset Type | File Size | Sprite Size | Count per Sheet | Total Sprites |
|------------|-----------|-------------|-----------------|---------------|
| Tree1      | 1536x256  | 256x256     | 6              | 6             |
| Tree2      | 1536x256  | 256x256     | 6              | 6             |
| Tree3      | 1536x192  | 192x192     | 8              | 8             |
| Tree4      | 1536x192  | 192x192     | 8              | 8             |
| Rock1      | 128x128   | 64x64       | 4 (2x2)        | 4             |
| Rock2      | 128x128   | 64x64       | 4 (2x2)        | 4             |
| Rock3      | 128x128   | 64x64       | 4 (2x2)        | 4             |
| **TOTAL**  |           |             |                | **40**        |

### Impact

✅ **Trees are no longer clumped together** - Each tree position now shows a single tree sprite (256x256 or 192x192 pixels)

✅ **Natural variety** - With 28 different tree sprites randomly distributed, the forest looks organic

✅ **Proper rock rendering** - Rocks now render as individual stones (64x64) instead of entire grid sheets

✅ **Reusable solution** - The `SpriteSheetExtractor` can be applied to other tilesheets in the project

## Future Extensions

This solution can be extended to handle other tilesheets in the project:

- Fences (64x64)
- Walls (128x128)
- Water decorations (3072x192)
- Shadow sprites (192x192)
- Decorations (various sizes)

The extraction utility is designed to be flexible and can handle any sprite sheet configuration by specifying the individual sprite dimensions.

## Testing

The implementation has been:
- ✅ Successfully built without errors
- ✅ Logic verified through calculation tests
- ✅ Math validated for all sprite extractions

Visual testing requires running the game in a graphical environment to see the properly rendered trees and rocks.
