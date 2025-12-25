# Archived Slates Integration

This directory contains the Slates tileset integration that was removed from the active codebase.

## Date Archived
December 25, 2024

## Reason for Archiving
The project decided to move back to using Sunnyside World assets as the primary tileset for the game. The Slates tileset integration has been archived for future reference and potential reuse.

## What Was Archived

### Documentation Files (in `docs/`)
- `SLATES_IMPLEMENTATION_STATUS.md` - Status of Slates integration implementation
- `SLATES_IMPLEMENTATION_SUMMARY.md` - Summary of the Slates implementation
- `SLATES_INTEGRATION_GUIDE.md` - Guide for integrating the Slates tileset
- `SLATES_QUICK_REFERENCE.md` - Quick reference for Slates usage
- `SLATES_USAGE_EXAMPLES.md` - Examples of how to use Slates tileset

### Code Files (in `code/World/Tiles/`)
- `SlatesTilesetHelper.cs` - Helper class for working with Slates 32x32px tileset
- `SlatesTileMapping.cs` - Mapping configuration for Slates tile IDs

## Original Tileset Information

- **Name:** Slates v.2
- **Tile Size:** 32x32 pixels
- **Grid Size:** 56 columns × 23 rows
- **Total Tiles:** 1,288 tiles
- **License:** CC-BY 4.0 - Attribution Required
- **Author:** Ivan Voirol

## Replacement

The Slates tileset has been replaced with:
- **Sunnyside World Assets** - 16x16px tileset
- Helper class: `SunnysideTilesetHelper.cs`
- Mapping class: `SunnysideTileMapping.cs`

## How to Restore (If Needed)

If you need to restore the Slates integration:

1. Copy the code files back to `MoonBrookRidge/World/Tiles/`
2. Add the Slates tileset back to `Content.mgcb`
3. Update `WorldMap.cs` to load and use the Slates tileset
4. Update `GameplayState.cs` to load the Slates tileset texture
5. Refer to the archived documentation for detailed integration steps

## Notes

- The Slates tileset PNG file (`Slates_32x32_v2.png`) is still present in the `sprites/tilesets/Slates/` directory
- The tile type enums (e.g., `SlatesGrassBasic`, `SlatesDirtTilled`, etc.) are still defined in `Tile.cs` for backward compatibility
- The archived code is fully functional and can be re-integrated if needed
