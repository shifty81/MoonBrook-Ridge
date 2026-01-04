# Fast Travel Icons Implementation Guide

## Overview

The Fast Travel menu currently uses ASCII text placeholders for waypoint icons. This guide explains how to replace them with proper sprite-based icons.

## Current Status

### Text Placeholders (Temporary)
- **Farm**: `[F]`
- **Village**: `[V]`
- **Dungeon**: `[D]`
- **Mineshaft**: `[M]`
- **Landmark**: `[L]`
- **Shop District**: `[S]`
- **Custom**: `[*]`

These work but are not visually appealing. They should be replaced with 32x32 pixel sprite icons.

## Implementation Plan

### 1. Icon Specifications

Each icon should be:
- **Size**: 32×32 pixels
- **Format**: PNG with transparency
- **Style**: Match the pixel art style of the game (16×16 or 32×32 pixel art)
- **Color**: Should use colors that match the waypoint color scheme (see below)

### 2. Recommended Icon Designs

#### Farm Icon (`icon_farm.png`)
- **Concept**: Small house or barn
- **Color Scheme**: Green/brown tones (LimeGreen highlight)
- **Details**: Simple barn shape with door, or farmhouse with chimney

#### Village Icon (`icon_village.png`)
- **Concept**: Multiple buildings cluster or town gate
- **Color Scheme**: Blue tones (SkyBlue highlight)
- **Details**: 2-3 small building silhouettes or town entrance arch

#### Dungeon Icon (`icon_dungeon.png`)
- **Concept**: Sword, shield, or dungeon entrance
- **Color Scheme**: Dark red tones (DarkRed highlight)
- **Details**: Crossed swords, castle gate, or stone archway

#### Mineshaft Icon (`icon_mineshaft.png`)
- **Concept**: Pickaxe, mine cart, or tunnel entrance
- **Color Scheme**: Gray tones (Gray highlight)
- **Details**: Pickaxe tool, mine cart on rails, or wooden support beams

#### Landmark Icon (`icon_landmark.png`)
- **Concept**: Flag, star marker, or signpost
- **Color Scheme**: Gold tones (Gold highlight)
- **Details**: Flag on pole, star burst, or directional signpost

#### Shop District Icon (`icon_shop.png`)
- **Concept**: Shop front, coin, or merchant stall
- **Color Scheme**: Orange tones (Orange highlight)
- **Details**: Shop awning, gold coins, or market stall

#### Custom Icon (`icon_custom.png`)
- **Concept**: Customizable star or sparkle
- **Color Scheme**: Magenta/purple tones (Magenta highlight)
- **Details**: Star with rays, sparkle effect, or question mark

## Implementation Steps

### Step 1: Create Icon Sprites

1. **Find or Create Icons**:
   - Check existing Sunnyside World assets for suitable icons
   - Create new 32×32 pixel art icons
   - Use online pixel art tools (Piskel, Aseprite, etc.)

2. **Save Icons**:
   ```
   MoonBrookRidge/Content/Textures/Icons/
   ├── icon_farm.png
   ├── icon_village.png
   ├── icon_dungeon.png
   ├── icon_mineshaft.png
   ├── icon_landmark.png
   ├── icon_shop.png
   └── icon_custom.png
   ```

### Step 2: Add to Content Pipeline

Add to `MoonBrookRidge/Content/Content.mgcb`:

```mgcb
#begin Textures/Icons/icon_farm.png
/importer:TextureImporter
/processor:TextureProcessor
/processorParam:ColorKeyColor=255,0,255,255
/processorParam:ColorKeyEnabled=True
/processorParam:GenerateMipmaps=False
/processorParam:PremultiplyAlpha=True
/processorParam:ResizeToPowerOfTwo=False
/processorParam:MakeSquare=False
/processorParam:TextureFormat=Color
/build:Textures/Icons/icon_farm.png

#begin Textures/Icons/icon_village.png
/importer:TextureImporter
/processor:TextureProcessor
/processorParam:ColorKeyColor=255,0,255,255
/processorParam:ColorKeyEnabled=True
/processorParam:GenerateMipmaps=False
/processorParam:PremultiplyAlpha=True
/processorParam:ResizeToPowerOfTwo=False
/processorParam:MakeSquare=False
/processorParam:TextureFormat=Color
/build:Textures/Icons/icon_village.png

# ... (repeat for all icons)
```

### Step 3: Load Icons in GameplayState

In `GameplayState.cs`, add icon loading after initializing the FastTravelMenu:

```csharp
// Load fast travel waypoint icons
var waypointIcons = new Dictionary<WaypointType, Texture2D>
{
    [WaypointType.Farm] = Game.Content.Load<Texture2D>("Textures/Icons/icon_farm"),
    [WaypointType.Village] = Game.Content.Load<Texture2D>("Textures/Icons/icon_village"),
    [WaypointType.DungeonEntrance] = Game.Content.Load<Texture2D>("Textures/Icons/icon_dungeon"),
    [WaypointType.MineshaftEntrance] = Game.Content.Load<Texture2D>("Textures/Icons/icon_mineshaft"),
    [WaypointType.Landmark] = Game.Content.Load<Texture2D>("Textures/Icons/icon_landmark"),
    [WaypointType.ShopDistrict] = Game.Content.Load<Texture2D>("Textures/Icons/icon_shop"),
    [WaypointType.Custom] = Game.Content.Load<Texture2D>("Textures/Icons/icon_custom")
};
_fastTravelMenu.LoadWaypointIcons(waypointIcons);
```

### Step 4: Test

1. Run the game
2. Press `W` to open the Fast Travel menu
3. Verify icons appear instead of text placeholders
4. Check that icons are colored correctly based on waypoint type

## Color Scheme Reference

The icons will be tinted with these colors based on waypoint type:

```csharp
WaypointType.Farm => Color.LimeGreen
WaypointType.Village => Color.SkyBlue
WaypointType.DungeonEntrance => Color.DarkRed
WaypointType.MineshaftEntrance => Color.Gray
WaypointType.Landmark => Color.Gold
WaypointType.ShopDistrict => Color.Orange
WaypointType.Custom => Color.Magenta
```

Design icons in grayscale or neutral colors, as they will be tinted in-game.

## Fallback Behavior

The system is designed with a fallback:
- If icon sprites are loaded → display sprite icons
- If icon sprites are NOT loaded → display text placeholders

This ensures the menu always works, even without custom icons.

## Alternative: Use Existing Assets

Check the Sunnyside World asset pack for existing icons that could be repurposed:
1. Look in `sprites/SUNNYSIDE_WORLD_*/objects/` for small decorative items
2. Look for items that represent the concepts (house, castle, pickaxe, etc.)
3. Scale to 32×32 if needed
4. Extract and use as waypoint icons

## Future Enhancements

- **Animated Icons**: Use sprite sheets for animated waypoint icons
- **Custom Icons Per Waypoint**: Allow each waypoint to have a unique icon
- **Icon Badges**: Add small badges to icons (locked, new, recommended, etc.)
- **Hover Effects**: Add glow or animation when hovering over icons

---

**Document Status**: Implementation guide ready  
**Last Updated**: January 2026  
**Related Files**: 
- `MoonBrookRidge/UI/Menus/FastTravelMenu.cs` - Icon rendering implementation
- `MoonBrookRidge/Core/States/GameplayState.cs` - Icon loading location
