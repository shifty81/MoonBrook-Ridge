#!/usr/bin/env python3
"""
Sunnyside World Asset Organizer
Organizes and copies the 11,000+ Sunnyside World assets from sprites/ to Content/Textures/
for use in MoonBrook Ridge game.

This script:
1. Scans the sprites folder for all Sunnyside World assets
2. Organizes them by category (Buildings, Characters, Crops, etc.)
3. Copies them to appropriate Content/Textures subdirectories
4. Generates an asset catalog for easy reference
5. Filters out non-essential files (__MACOSX, duplicates, etc.)
"""

import os
import shutil
import json
import sys
from pathlib import Path
from collections import defaultdict

# Base directories
SPRITES_DIR = Path(__file__).parent.parent / "sprites"
CONTENT_DIR = Path(__file__).parent.parent / "MoonBrookRidge" / "Content" / "Textures"

# Directories to skip
SKIP_DIRS = {"__MACOSX", ".DS_Store", "obj", "bin"}
SKIP_EXTENSIONS = {".txt", ".md", ".json", ".yy", ".yyp"}

# Asset category mappings
ASSET_CATEGORIES = {
    "Buildings": {
        "source_patterns": [
            "Buildings/",
            "SUNNYSIDE_WORLD_BUILDINGS_V0.01/",
        ],
        "target_dir": "Buildings",
        "description": "Buildings, houses, castles, structures"
    },
    "Characters": {
        "source_patterns": [
            "SUNNYSIDE_WORLD_CHARACTERS_V0.3.1/",
            "SUNNYSIDE_WORLD_CHARACTERS_PARTS_V0.3.1/",
            "characters/",
        ],
        "target_dir": "Characters",
        "description": "Character sprites and animation parts"
    },
    "Crops": {
        "source_patterns": [
            "SUNNYSIDE_WORLD_CROPS_V0.01",
        ],
        "target_dir": "Crops",
        "description": "Crop growth stages and farming items"
    },
    "Resources": {
        "source_patterns": [
            "Resources/",
        ],
        "target_dir": "Resources",
        "description": "Harvestable resources (rocks, trees, gold, animals)"
    },
    "Decorations": {
        "source_patterns": [
            "Decorations/",
        ],
        "target_dir": "Decorations",
        "description": "Decorative objects (bushes, rocks, clouds, etc.)"
    },
    "Effects": {
        "source_patterns": [
            "SUNNYSIDE_WORLD_CHIMNEYSMOKE_v1.0/",
            "Particle",
            "particles/",
        ],
        "target_dir": "Effects",
        "description": "Particle effects and animations"
    },
    "Enemies": {
        "source_patterns": [
            "SUNNYSIDE_WORLD_GOBLIN_V0.1",
        ],
        "target_dir": "Enemies",
        "description": "Enemy sprites and animations"
    },
    "Tiles": {
        "source_patterns": [
            "Sunnyside World - Tileset/",
            "Sunnyside_World_ASSET_PACK_V2.1/",
            "Sunnyside World_Tileset_Beta",
            "tilesets/",
        ],
        "target_dir": "Tiles",
        "description": "Ground tiles and tilesets"
    },
    "Objects": {
        "source_patterns": [
            "objects/",
            "Units/",
        ],
        "target_dir": "Objects",
        "description": "Interactive objects and items"
    },
}


def get_user_confirmation(force: bool = False) -> bool:
    """
    Prompt user for confirmation to proceed with file operations.
    
    Interactively prompts the user to confirm whether to proceed with copying files.
    Accepts 'yes', 'y', 'no', or 'n' (case-insensitive). Will continue to prompt
    until a valid response is provided.
    
    Args:
        force: If True, skips confirmation and returns True
    
    Returns:
        bool: True if user confirms (yes/y), False if user declines (no/n)
    """
    if force:
        print("\n⚠️  Force mode enabled - skipping confirmation")
        return True
        
    print("\n" + "=" * 80)
    print("⚠️  This script will copy files from sprites/ to Content/Textures/")
    print("=" * 80)
    
    while True:
        response = input("\nDo you want to proceed with copying files? (Yes/No): ").strip().lower()
        if response in ['yes', 'y']:
            return True
        elif response in ['no', 'n']:
            return False
        else:
            print("Invalid input. Please enter 'Yes' or 'No'.")


def print_directory_tree(directory: Path, prefix="", is_last=True, max_depth=None, current_depth=0):
    """
    Print directory structure in a visual tree format.
    
    Args:
        directory: Path to the directory to visualize
        prefix: Prefix string for tree formatting
        is_last: Whether this is the last item in the parent directory
        max_depth: Maximum depth to traverse (None for unlimited)
        current_depth: Current depth level
    """
    if not directory.exists():
        print(f"{prefix}[Directory does not exist: {directory}]")
        return
    
    if max_depth is not None and current_depth > max_depth:
        return
    
    # Print current directory
    if current_depth == 0:
        print(f"\n{directory.name}/")
    else:
        connector = "└── " if is_last else "├── "
        print(f"{prefix}{connector}{directory.name}/")
    
    # Get all entries in directory
    try:
        entries = list(directory.iterdir())
        # Separate directories and files efficiently
        dirs = []
        files = []
        for e in entries:
            if e.is_dir():
                dirs.append(e)
            else:
                files.append(e)
        # Sort each group separately
        dirs.sort(key=lambda x: x.name.lower())
        files.sort(key=lambda x: x.name.lower())
    except PermissionError:
        print(f"{prefix}    [Permission Denied]")
        return
    
    # Prepare prefix for children
    if current_depth == 0:
        new_prefix = ""
    else:
        extension = "    " if is_last else "│   "
        new_prefix = prefix + extension
    
    # Print directories first
    for i, entry in enumerate(dirs):
        is_last_entry = (i == len(dirs) - 1) and len(files) == 0
        print_directory_tree(entry, new_prefix, is_last_entry, max_depth, current_depth + 1)
    
    # Print files
    for i, entry in enumerate(files):
        is_last_file = (i == len(files) - 1)
        connector = "└── " if is_last_file else "├── "
        print(f"{new_prefix}{connector}{entry.name}")


def should_skip(path: Path) -> bool:
    """Check if a path should be skipped."""
    # Skip if in skip directories
    for part in path.parts:
        if part in SKIP_DIRS:
            return True
    
    # Skip if wrong extension
    if path.suffix.lower() in SKIP_EXTENSIONS:
        return True
    
    # Only process PNG files
    if path.is_file() and path.suffix.lower() != ".png":
        return True
    
    return False


def categorize_asset(asset_path: Path) -> str:
    """Determine which category an asset belongs to."""
    asset_str = str(asset_path)
    
    for category, info in ASSET_CATEGORIES.items():
        for pattern in info["source_patterns"]:
            if pattern in asset_str:
                return category
    
    return "Uncategorized"


def organize_assets(dry_run=False):
    """
    Organize and copy assets from sprites/ to Content/Textures/.
    
    Args:
        dry_run: If True, only scan and report without copying files
        
    Returns:
        tuple: (stats dict, success bool, copied_count int, errors list)
    """
    print("=" * 80)
    print("Sunnyside World Asset Organizer")
    print("=" * 80)
    print(f"Source: {SPRITES_DIR}")
    print(f"Target: {CONTENT_DIR}")
    print(f"Mode: {'DRY RUN (no files will be copied)' if dry_run else 'LIVE (files will be copied)'}")
    print()
    
    # Statistics and error tracking
    stats = defaultdict(lambda: {"count": 0, "files": []})
    errors = []
    success = True
    
    # Find all PNG files in sprites directory
    print("Scanning sprites directory...")
    try:
        png_files = list(SPRITES_DIR.rglob("*.png"))
        print(f"Found {len(png_files)} total PNG files")
    except Exception as e:
        error_msg = f"Error scanning sprites directory: {e}"
        print(f"❌ {error_msg}")
        errors.append(error_msg)
        return stats, False, 0, errors
    print()
    
    # Categorize assets
    print("Categorizing assets...")
    for png_file in png_files:
        if should_skip(png_file):
            continue
        
        try:
            category = categorize_asset(png_file)
            relative_path = png_file.relative_to(SPRITES_DIR)
            
            stats[category]["count"] += 1
            stats[category]["files"].append(str(relative_path))
        except Exception as e:
            error_msg = f"Error categorizing {png_file}: {e}"
            errors.append(error_msg)
            success = False
    
    # Print statistics
    print("\nAsset Statistics:")
    print("-" * 80)
    total_organized = 0
    for category in sorted(stats.keys()):
        count = stats[category]["count"]
        total_organized += count
        desc = ASSET_CATEGORIES.get(category, {}).get("description", "")
        print(f"{category:20s}: {count:5d} assets - {desc}")
    print("-" * 80)
    print(f"{'Total':20s}: {total_organized:5d} assets")
    print()
    
    if dry_run:
        print("DRY RUN complete. No files were copied.")
        print("Run with --execute to actually copy files.")
        return stats, success, 0, errors
    
    # Copy files
    print("Copying files to Content/Textures/...")
    copied_count = 0
    
    for png_file in png_files:
        if should_skip(png_file):
            continue
        
        try:
            category = categorize_asset(png_file)
            
            # Determine target directory
            if category in ASSET_CATEGORIES:
                target_base = CONTENT_DIR / ASSET_CATEGORIES[category]["target_dir"]
            else:
                target_base = CONTENT_DIR / "Uncategorized"
            
            # Preserve some directory structure
            relative_path = png_file.relative_to(SPRITES_DIR)
            
            # Clean up the path (remove version numbers and parent dirs)
            path_parts = list(relative_path.parts)
            cleaned_parts = []
            
            for part in path_parts[:-1]:  # All except filename
                # Skip version folder names
                if "V0." in part or "v1." in part or part.startswith("SUNNYSIDE_WORLD_"):
                    continue
                cleaned_parts.append(part)
            
            # Add filename
            cleaned_parts.append(path_parts[-1])
            
            # Build target path
            if len(cleaned_parts) > 1:
                target_path = target_base / Path(*cleaned_parts)
            else:
                target_path = target_base / cleaned_parts[0]
            
            # Create target directory
            target_path.parent.mkdir(parents=True, exist_ok=True)
            
            # Copy file (skip if already exists with same size)
            if target_path.exists():
                if target_path.stat().st_size == png_file.stat().st_size:
                    continue  # Skip identical file
            
            shutil.copy2(png_file, target_path)
            copied_count += 1
            if copied_count % 100 == 0:
                print(f"  Copied {copied_count} files...")
                
        except Exception as e:
            error_msg = f"Error copying {png_file}: {e}"
            print(f"  ❌ {error_msg}")
            errors.append(error_msg)
            success = False
    
    print(f"\nCopied {copied_count} new/updated files")
    
    # Set success to False if there were errors
    if errors:
        success = False
    
    print()
    
    return stats, success, copied_count, errors


def generate_catalog(stats):
    """Generate an asset catalog JSON file."""
    catalog_path = CONTENT_DIR / "asset_catalog.json"
    
    catalog = {
        "version": "1.0",
        "description": "Sunnyside World Asset Catalog for MoonBrook Ridge",
        "total_assets": sum(s["count"] for s in stats.values()),
        "categories": {}
    }
    
    for category, data in sorted(stats.items()):
        catalog["categories"][category] = {
            "count": data["count"],
            "description": ASSET_CATEGORIES.get(category, {}).get("description", ""),
            "target_directory": ASSET_CATEGORIES.get(category, {}).get("target_dir", "Uncategorized"),
            "sample_files": data["files"][:10]  # First 10 as samples
        }
    
    with open(catalog_path, 'w') as f:
        json.dump(catalog, f, indent=2)
    
    print(f"Asset catalog saved to: {catalog_path}")


def main():
    # Check command-line arguments
    dry_run = "--execute" not in sys.argv
    force = "--force" in sys.argv or "-f" in sys.argv
    show_help = "--help" in sys.argv or "-h" in sys.argv
    
    if show_help:
        print("Sunnyside World Asset Organizer")
        print()
        print("Usage: python3 organize_sunnyside_assets.py [OPTIONS]")
        print()
        print("Options:")
        print("  --execute      Actually copy files (default is dry-run)")
        print("  --force, -f    Skip confirmation prompt")
        print("  --help, -h     Show this help message")
        print()
        print("Examples:")
        print("  python3 organize_sunnyside_assets.py                    # Dry run with confirmation")
        print("  python3 organize_sunnyside_assets.py --execute          # Execute with confirmation")
        print("  python3 organize_sunnyside_assets.py --execute --force  # Execute without confirmation")
        sys.exit(0)
    
    if dry_run:
        print("Running in DRY RUN mode. Use --execute to actually copy files.")
        print()
    
    # Get user confirmation before proceeding (unless --force is used)
    if not get_user_confirmation(force):
        print("\n❌ Operation cancelled by user.")
        print("No files were copied.")
        sys.exit(0)
    
    print("\n✅ Proceeding with operation...\n")
    
    # Organize assets
    stats, success, copied_count, errors = organize_assets(dry_run=dry_run)
    
    # Print operation results
    print("\n" + "=" * 80)
    print("OPERATION SUMMARY")
    print("=" * 80)
    
    if success and not errors:
        print("✅ Operation completed successfully!")
    elif errors:
        print("⚠️  Operation completed with errors:")
        for error in errors[:10]:  # Show first 10 errors
            print(f"   - {error}")
        if len(errors) > 10:
            print(f"   ... and {len(errors) - 10} more errors")
    else:
        print("❌ Operation failed!")
    
    print(f"\n📊 Total files moved/copied: {copied_count}")
    
    # Generate catalog
    if not dry_run and stats:
        try:
            generate_catalog(stats)
        except Exception as e:
            print(f"\n⚠️  Error generating catalog: {e}")
    
    # Display directory tree of target folder
    if CONTENT_DIR.exists():
        print("\n" + "=" * 80)
        print("DIRECTORY STRUCTURE - Content/Textures")
        print("=" * 80)
        print_directory_tree(CONTENT_DIR, max_depth=3)
        print("=" * 80)
    
    if not dry_run:
        print(f"\nNext steps:")
        print(f"1. Review the organized assets in {CONTENT_DIR}")
        print("2. Update Content.mgcb to include new assets")
        print("3. Update asset loading code to use the new structure")
    else:
        print("\n📊 Dry run complete. Review the statistics above.")
        print("Run with --execute to actually copy files.")


if __name__ == "__main__":
    main()
