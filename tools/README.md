# Tools Directory

This directory contains utility scripts for the MoonBrook Ridge project.

## Scripts

### organize_sunnyside_assets.py

Organizes and copies 11,000+ Sunnyside World assets from `sprites/` to `Content/Textures/` for use in the game.

**Features:**
- Interactive user confirmation prompt
- Comprehensive error handling and reporting
- Progress tracking with file counts
- Visual directory tree output of organized assets
- Asset categorization (Buildings, Characters, Crops, etc.)
- Dry-run mode for safe testing

**Usage:**
```bash
# Dry run mode (no files copied, shows what would be done)
python3 tools/organize_sunnyside_assets.py

# Execute mode (actually copies files)
python3 tools/organize_sunnyside_assets.py --execute
```

**Interactive Prompts:**
The script will ask for confirmation before proceeding:
- Enter "Yes" or "Y" to proceed with copying
- Enter "No" or "N" to cancel the operation

**Output:**
- Asset statistics by category
- Operation success/failure status
- Total files moved/copied count
- Directory tree visualization of Content/Textures
- Error report (if any errors occurred)

**Requirements:**
- Python 3.6+ (uses pathlib and type hints)
- No external dependencies (uses only standard library)

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
