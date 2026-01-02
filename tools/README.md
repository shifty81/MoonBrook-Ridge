# Tools Directory

This directory contains utility scripts for the MoonBrook Ridge project.

## Scripts

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
