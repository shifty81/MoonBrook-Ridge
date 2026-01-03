# Tools Directory

This directory contains utility scripts for managing and organizing the game's extensive sprite and tileset assets.

## Overview

MoonBrook Ridge uses the **Sunnyside World** asset pack (~11,000+ sprites) and GameMaker templates with autotiling and animation data. These tools help organize, process, and integrate these assets into the game.

## Scripts

### organize_sunnyside_assets.py

Organizes and copies 11,000+ Sunnyside World assets from `sprites/` to `Content/Textures/` for use in the game.

**Features:**
- Interactive user confirmation prompt (can be bypassed with --force)
- Comprehensive error handling and reporting
- Progress tracking with file counts
- Visual directory tree output of organized assets
- Asset categorization (Buildings, Characters, Crops, Tiles, etc.)
- Dry-run mode for safe testing

**Usage:**
```bash
# Dry run mode (no files copied, shows what would be done)
python3 tools/organize_sunnyside_assets.py

# Execute mode with confirmation (actually copies files)
python3 tools/organize_sunnyside_assets.py --execute

# Execute without confirmation prompt
python3 tools/organize_sunnyside_assets.py --execute --force

# Show help
python3 tools/organize_sunnyside_assets.py --help
```

**Interactive Prompts:**
The script will ask for confirmation before proceeding (unless --force is used):
- Enter "Yes" or "Y" to proceed with copying
- Enter "No" or "N" to cancel the operation

**Output:**
- Asset statistics by category
- Operation success/failure status
- Total files moved/copied count
- Directory tree visualization of Content/Textures
- Error report (if any errors occurred)

### organize_needs_sorted.py

Processes assets from the `sprites/needs sorted/` directory, which contains additional farming game items (1,081 total files).

**Categories:**
- Cooking items (320 sprites)
- Forage items (152 sprites)
- Minerals (171 sprites)
- Artifacts (123 sprites)
- Additional crops (172 sprites)
- Artisan goods (60 sprites)
- Fruits & vegetables (70 sprites)
- Tools (8 sprites)
- Farm animals (5 sprite sheets)

**Usage:**
```bash
# Preview organization (dry run)
python3 tools/organize_needs_sorted.py

# Execute organization
python3 tools/organize_needs_sorted.py --execute

# Generate markdown catalog of all assets
python3 tools/organize_needs_sorted.py --catalog
```

**Output:**
- Organized assets in `Content/Textures/Objects/` and `Content/Textures/Crops/`
- Asset catalog at `sprites/needs sorted/ASSET_CATALOG.md`

### parse_gamemaker_tilesets.py

Extracts autotiling rules and animation sequences from GameMaker .yy tileset files and generates C# code for runtime use.

**Extracted Data:**
- Autotiling rules (blob/wang tiling patterns)
- Tile animation sequences (25+ animations)
- Tile dimensions and counts
- Closed edge information

**Usage:**
```bash
# List all found GameMaker tileset files
python3 tools/parse_gamemaker_tilesets.py --list

# Parse and generate C# code (default output dir)
python3 tools/parse_gamemaker_tilesets.py

# Specify custom directories
python3 tools/parse_gamemaker_tilesets.py \
    --sprites-dir ../sprites \
    --output-dir ../MoonBrookRidge/World/Tiles/Generated
```

**Generated Files:**
- `World/Tiles/Generated/AutoTiling/` - Autotiling rule sets
  - `tileset_sunnysideworld_AutoTileRules.cs`
  - `tileset_forest_AutoTileRules.cs`
- `World/Tiles/Generated/Animations/` - Animation helpers
  - `tileset_sunnysideworld_Animations.cs`

### generate_tile_placeholders.py

Generates textured 16×16 pixel placeholder tiles for the game world.

**Usage:**
```bash
python3 tools/generate_tile_placeholders.py
```

**Requirements:**
- Python 3.12+
- Pillow (PIL)
- NumPy

**Install dependencies:**
```bash
pip3 install Pillow numpy
```

**What it generates:**
- Grass tiles (4 variants)
- Dirt tiles (2 variants)
- Water tile
- Sand tile
- Stone and rock tiles
- Tilled soil tiles (dry and watered)
- Wooden floor tile

All tiles are output to `MoonBrookRidge/Content/Textures/Tiles/`

See [VISUAL_IMPROVEMENTS.md](../VISUAL_IMPROVEMENTS.md) for more details.

## Requirements

All scripts (except generate_tile_placeholders.py) require Python 3.7+ with no external dependencies.

```bash
# Check Python version
python3 --version

# Run any script
python3 organize_sunnyside_assets.py --help
```

## Workflow

### Initial Setup

1. **Organize main assets:**
   ```bash
   python3 organize_sunnyside_assets.py --execute --force
   ```

2. **Organize additional assets:**
   ```bash
   python3 organize_needs_sorted.py --execute
   ```

3. **Generate autotiling/animation code:**
   ```bash
   python3 parse_gamemaker_tilesets.py
   ```

4. **Update Content.mgcb** to include new asset files

5. **Rebuild project:**
   ```bash
   cd ..
   dotnet build
   ```

## Documentation

For detailed usage information and integration examples, see:
- [ENHANCED_SPRITE_GUIDE.md](../ENHANCED_SPRITE_GUIDE.md) - Complete integration guide
- [SPRITE_GUIDE.md](../SPRITE_GUIDE.md) - Original sprite documentation
- [TILESET_GUIDE.md](../TILESET_GUIDE.md) - Tileset usage guide

---

**Last Updated**: January 2026
