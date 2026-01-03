#!/usr/bin/env python3
"""
GameMaker Tileset Parser
Extracts autotiling rules and animation data from GameMaker .yy tileset files
and generates C# data structures for use in MoonBrook Ridge.

This script:
1. Scans for .yy tileset files in sprites directory
2. Parses JSON structure to extract:
   - Tile dimensions
   - Autotiling rules (for seamless terrain transitions)
   - Tile animation sequences
3. Generates C# code for runtime use
"""

import json
import os
import re
from pathlib import Path
from typing import Dict, List, Tuple
from collections import defaultdict


def parse_gamemaker_tileset(yy_file_path: Path) -> Dict:
    """
    Parse a GameMaker .yy tileset file and extract useful information.
    
    Args:
        yy_file_path: Path to the .yy file
        
    Returns:
        Dictionary containing parsed tileset information
    """
    try:
        with open(yy_file_path, 'r', encoding='utf-8') as f:
            # GameMaker .yy files are JSON with trailing commas (not valid JSON)
            content = f.read()
            # Remove trailing commas before closing brackets/braces
            import re
            content = re.sub(r',(\s*[}\]])', r'\1', content)
            data = json.loads(content)
        
        tileset_info = {
            'name': data.get('name', ''),
            'tile_width': data.get('tileWidth', 16),
            'tile_height': data.get('tileHeight', 16),
            'tile_count': data.get('tile_count', 0),
            'autotile_sets': [],
            'animations': []
        }
        
        # Parse autotile sets
        autotile_sets = data.get('autoTileSets', [])
        for autotile in autotile_sets:
            tileset_info['autotile_sets'].append({
                'name': autotile.get('name', ''),
                'tiles': autotile.get('tiles', []),
                'closed_edge': autotile.get('closed_edge', False)
            })
        
        # Parse tile animations
        tile_animations = data.get('tileAnimationFrames', [])
        for animation in tile_animations:
            tileset_info['animations'].append({
                'name': animation.get('name', ''),
                'frames': animation.get('frames', [])
            })
        
        return tileset_info
        
    except Exception as e:
        print(f"Error parsing {yy_file_path}: {e}")
        return None


def generate_csharp_autotiling_rules(tileset_info: Dict, output_path: Path):
    """
    Generate C# code for autotiling rules extracted from GameMaker tilesets.
    
    Args:
        tileset_info: Parsed tileset information
        output_path: Path where to write the C# file
    """
    class_name = f"{tileset_info['name'].replace('tileset_', '').title()}AutoTileRules"
    
    csharp_code = f'''using System.Collections.Generic;

namespace MoonBrookRidge.World.Tiles.AutoTiling;

/// <summary>
/// Auto-generated autotiling rules from GameMaker tileset: {tileset_info['name']}
/// This file is auto-generated. Do not edit manually.
/// </summary>
public static class {class_name}
{{
    /// <summary>
    /// Tile dimensions
    /// </summary>
    public const int TileWidth = {tileset_info['tile_width']};
    public const int TileHeight = {tileset_info['tile_height']};
    public const int TotalTiles = {tileset_info['tile_count']};
    
    /// <summary>
    /// Autotile set definitions
    /// Each set contains 16 tiles arranged for blob/wang tiling:
    /// Tiles are indexed for corners and edges to create seamless terrain
    /// </summary>
    public static class AutoTileSets
    {{
'''
    
    # Generate autotile set definitions
    for autotile in tileset_info['autotile_sets']:
        set_name = autotile['name'].replace(' ', '').replace('-', '')
        if not set_name:
            continue
            
        tiles = autotile['tiles']
        if len(tiles) < 16:
            continue  # Need at least 16 tiles for full autotiling
        
        csharp_code += f'''        /// <summary>
        /// {autotile['name']} autotile set
        /// Closed edge: {autotile['closed_edge']}
        /// </summary>
        public static readonly int[] {set_name} = new int[]
        {{
'''
        # Format tiles in rows of 8
        for i in range(0, len(tiles), 8):
            tile_group = tiles[i:i+8]
            tile_str = ', '.join(str(t) for t in tile_group)
            csharp_code += f'            {tile_str}'
            if i + 8 < len(tiles):
                csharp_code += ','
            csharp_code += '\n'
        
        csharp_code += '        };\n\n'
    
    csharp_code += '''    }
    
    /// <summary>
    /// Get autotile ID based on neighboring tiles
    /// Uses the blob/wang tiling pattern
    /// </summary>
    /// <param name="north">True if tile to the north is same type</param>
    /// <param name="south">True if tile to the south is same type</param>
    /// <param name="east">True if tile to the east is same type</param>
    /// <param name="west">True if tile to the west is same type</param>
    /// <param name="autoTileSet">The autotile set to use</param>
    /// <returns>Index into the autotile set (0-15)</returns>
    public static int GetAutoTileIndex(bool north, bool south, bool east, bool west, int[] autoTileSet)
    {
        // Blob tiling pattern (16 tiles):
        // 0  = isolated
        // 1-4 = single edges
        // 5-12 = corners and combinations
        // 13-15 = multiple edges
        
        int index = 0;
        if (north) index |= 1;
        if (east) index |= 2;
        if (south) index |= 4;
        if (west) index |= 8;
        
        return autoTileSet[index];
    }
}
'''
    
    # Write to file
    output_path.parent.mkdir(parents=True, exist_ok=True)
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(csharp_code)
    
    print(f"Generated C# autotiling rules: {output_path}")


def generate_csharp_animations(tileset_info: Dict, output_path: Path):
    """
    Generate C# code for tile animations.
    
    Args:
        tileset_info: Parsed tileset information
        output_path: Path where to write the C# file
    """
    class_name = f"{tileset_info['name'].replace('tileset_', '').title()}Animations"
    
    csharp_code = f'''using System.Collections.Generic;

namespace MoonBrookRidge.World.Tiles.Animations;

/// <summary>
/// Auto-generated tile animations from GameMaker tileset: {tileset_info['name']}
/// This file is auto-generated. Do not edit manually.
/// </summary>
public static class {class_name}
{{
    /// <summary>
    /// Tile animation sequences
    /// Key: Animation name, Value: Array of tile IDs representing frames
    /// </summary>
    public static readonly Dictionary<string, int[]> Animations = new Dictionary<string, int[]>
    {{
'''
    
    # Generate animation definitions
    for animation in tileset_info['animations']:
        anim_name = animation['name'].replace(' ', '_').replace('-', '_')
        frames = animation['frames']
        
        if not frames:
            continue
        
        frame_str = ', '.join(str(f) for f in frames)
        csharp_code += f'        {{ "{anim_name}", new int[] {{ {frame_str} }} }},\n'
    
    csharp_code += '''    };
    
    /// <summary>
    /// Get the frame for an animation at a specific time
    /// </summary>
    /// <param name="animationName">Name of the animation</param>
    /// <param name="elapsedTime">Elapsed time in seconds</param>
    /// <param name="frameRate">Frames per second (default 5)</param>
    /// <returns>Tile ID for the current frame</returns>
    public static int GetAnimationFrame(string animationName, float elapsedTime, float frameRate = 5.0f)
    {
        if (!Animations.TryGetValue(animationName, out int[] frames))
            return -1;
        
        int frameIndex = (int)(elapsedTime * frameRate) % frames.Length;
        return frames[frameIndex];
    }
}
'''
    
    # Write to file
    output_path.parent.mkdir(parents=True, exist_ok=True)
    with open(output_path, 'w', encoding='utf-8') as f:
        f.write(csharp_code)
    
    print(f"Generated C# tile animations: {output_path}")


def main():
    import argparse
    
    parser = argparse.ArgumentParser(description='Parse GameMaker tileset files and generate C# code')
    parser.add_argument('--sprites-dir', default='../sprites', 
                       help='Path to sprites directory (default: ../sprites)')
    parser.add_argument('--output-dir', default='../MoonBrookRidge/World/Tiles/Generated',
                       help='Output directory for generated C# files')
    parser.add_argument('--list', action='store_true',
                       help='List all found tileset files')
    
    args = parser.parse_args()
    
    # Resolve paths relative to script location
    script_dir = Path(__file__).parent
    sprites_dir = (script_dir / args.sprites_dir).resolve()
    output_dir = (script_dir / args.output_dir).resolve()
    
    print("=" * 80)
    print("GameMaker Tileset Parser")
    print("=" * 80)
    print(f"Sprites directory: {sprites_dir}")
    print(f"Output directory: {output_dir}")
    print()
    
    # Find all .yy tileset files
    tileset_files = list(sprites_dir.rglob("tilesets/*/*.yy"))
    
    if not tileset_files:
        print("No tileset .yy files found!")
        return
    
    print(f"Found {len(tileset_files)} tileset files:")
    for f in tileset_files:
        print(f"  - {f.relative_to(sprites_dir)}")
    print()
    
    if args.list:
        return
    
    # Parse each tileset and generate C# code
    generated_count = 0
    for tileset_file in tileset_files:
        print(f"Processing: {tileset_file.name}")
        tileset_info = parse_gamemaker_tileset(tileset_file)
        
        if not tileset_info:
            continue
        
        # Generate autotiling rules if any exist
        if tileset_info['autotile_sets']:
            autotile_output = output_dir / "AutoTiling" / f"{tileset_info['name']}_AutoTileRules.cs"
            generate_csharp_autotiling_rules(tileset_info, autotile_output)
            generated_count += 1
        
        # Generate animation code if any exist
        if tileset_info['animations']:
            animation_output = output_dir / "Animations" / f"{tileset_info['name']}_Animations.cs"
            generate_csharp_animations(tileset_info, animation_output)
            generated_count += 1
    
    print()
    print("=" * 80)
    print(f"✅ Successfully generated {generated_count} C# files")
    print("=" * 80)
    print()
    print("Next steps:")
    print("1. Review generated files in:", output_dir)
    print("2. Add generated files to your project")
    print("3. Use autotiling rules in your tile rendering code")
    print("4. Use animation helpers for animated tiles")


if __name__ == "__main__":
    main()
