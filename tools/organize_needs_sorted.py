#!/usr/bin/env python3
"""
Organize "needs sorted" assets from sprites/needs sorted/ directory
into appropriate categories in MoonBrookRidge/Content/Textures/

This script specifically handles the unsorted farming game assets including:
- Cooking/crafting items
- Forage items
- Crops
- Minerals
- Artifacts
- Artisan goods
- Animals
"""

import os
import shutil
from pathlib import Path
from collections import defaultdict


# Base directories
SCRIPT_DIR = Path(__file__).parent
NEEDS_SORTED_DIR = SCRIPT_DIR.parent / "sprites" / "needs sorted"
CONTENT_DIR = SCRIPT_DIR.parent / "MoonBrookRidge" / "Content" / "Textures"

# Category mappings for needs sorted assets
CATEGORY_MAPPINGS = {
    "cooking": "Objects/Cooking",
    "crops": "Crops/Additional",
    "forage": "Objects/Forage",
    "minerals": "Objects/Minerals",
    "artifacts": "Objects/Artifacts",
    "artisan goods": "Objects/ArtisanGoods",
    "Fruits and Vegetables": "Objects/FruitsVegetables",
    "Tools": "Objects/Tools"
}


def count_assets() -> dict:
    """
    Count assets in needs sorted directory by category.
    
    Returns:
        Dictionary with category names and file counts
    """
    stats = defaultdict(int)
    
    if not NEEDS_SORTED_DIR.exists():
        print(f"❌ Directory not found: {NEEDS_SORTED_DIR}")
        return stats
    
    for item in NEEDS_SORTED_DIR.iterdir():
        if item.is_dir():
            png_files = list(item.glob("*.png"))
            stats[item.name] = len(png_files)
        elif item.suffix.lower() == ".png":
            stats["root_files"] += 1
    
    return stats


def organize_assets(dry_run=True):
    """
    Organize assets from needs sorted directory.
    
    Args:
        dry_run: If True, only report what would be done without copying files
        
    Returns:
        Tuple of (success: bool, files_processed: int, files_copied: int)
    """
    print("=" * 80)
    print("Needs Sorted Asset Organizer")
    print("=" * 80)
    print(f"Source: {NEEDS_SORTED_DIR}")
    print(f"Target: {CONTENT_DIR}")
    print(f"Mode: {'DRY RUN' if dry_run else 'EXECUTE'}")
    print()
    
    if not NEEDS_SORTED_DIR.exists():
        print(f"❌ Source directory not found: {NEEDS_SORTED_DIR}")
        return False, 0, 0
    
    # First, show what we have
    print("Analyzing needs sorted directory...")
    stats = count_assets()
    
    print("\nFound assets:")
    print("-" * 80)
    total_files = 0
    for category, count in sorted(stats.items()):
        print(f"{category:30s}: {count:5d} files")
        total_files += count
    print("-" * 80)
    print(f"{'Total':30s}: {total_files:5d} files")
    print()
    
    if dry_run:
        print("DRY RUN: Showing what would be copied...")
        print()
    
    files_processed = 0
    files_copied = 0
    
    # Process each category
    for source_name, target_path in CATEGORY_MAPPINGS.items():
        source_dir = NEEDS_SORTED_DIR / source_name
        
        if not source_dir.exists():
            continue
        
        target_dir = CONTENT_DIR / target_path
        
        print(f"Processing: {source_name} -> {target_path}")
        
        # Get all PNG files in source
        png_files = list(source_dir.glob("*.png"))
        
        for png_file in png_files:
            files_processed += 1
            target_file = target_dir / png_file.name
            
            if dry_run:
                if files_processed <= 5:  # Show first 5 examples
                    print(f"  Would copy: {png_file.name} -> {target_path}/")
            else:
                # Create target directory if needed
                target_dir.mkdir(parents=True, exist_ok=True)
                
                # Copy file
                shutil.copy2(png_file, target_file)
                files_copied += 1
        
        if dry_run and len(png_files) > 5:
            print(f"  ... and {len(png_files) - 5} more files")
        elif not dry_run:
            print(f"  Copied {len(png_files)} files")
    
    # Process root-level files
    print("\nProcessing root-level files:")
    for item in NEEDS_SORTED_DIR.iterdir():
        if item.is_file() and item.suffix.lower() == ".png":
            files_processed += 1
            target_dir = CONTENT_DIR / "Objects" / "Farm"
            target_file = target_dir / item.name
            
            if dry_run:
                print(f"  Would copy: {item.name} -> Objects/Farm/")
            else:
                target_dir.mkdir(parents=True, exist_ok=True)
                shutil.copy2(item, target_file)
                files_copied += 1
                print(f"  Copied: {item.name}")
    
    print()
    print("=" * 80)
    if dry_run:
        print(f"DRY RUN complete. Would process {files_processed} files.")
    else:
        print(f"✅ Successfully copied {files_copied} files!")
    print("=" * 80)
    
    return True, files_processed, files_copied


def generate_asset_catalog():
    """
    Generate a markdown catalog of all assets in needs sorted.
    """
    catalog_path = NEEDS_SORTED_DIR / "ASSET_CATALOG.md"
    
    print("Generating asset catalog...")
    
    with open(catalog_path, 'w') as f:
        f.write("# Needs Sorted Asset Catalog\n\n")
        f.write("This document lists all assets in the 'needs sorted' directory.\n\n")
        
        stats = count_assets()
        
        f.write("## Summary\n\n")
        f.write("| Category | File Count |\n")
        f.write("|----------|------------|\n")
        
        total = 0
        for category, count in sorted(stats.items()):
            f.write(f"| {category} | {count} |\n")
            total += count
        
        f.write(f"| **Total** | **{total}** |\n\n")
        
        # Detail each category
        for category, _ in sorted(stats.items()):
            if category == "root_files":
                continue
                
            f.write(f"## {category}\n\n")
            
            source_dir = NEEDS_SORTED_DIR / category
            if source_dir.exists() and source_dir.is_dir():
                png_files = sorted(source_dir.glob("*.png"))
                
                if len(png_files) > 0:
                    f.write("Files:\n")
                    for png_file in png_files[:20]:  # First 20
                        f.write(f"- {png_file.name}\n")
                    
                    if len(png_files) > 20:
                        f.write(f"- ... and {len(png_files) - 20} more\n")
                
                f.write("\n")
    
    print(f"✅ Asset catalog generated: {catalog_path}")


def main():
    import argparse
    
    parser = argparse.ArgumentParser(
        description='Organize assets from needs sorted directory'
    )
    parser.add_argument('--execute', action='store_true',
                       help='Actually copy files (default is dry-run)')
    parser.add_argument('--catalog', action='store_true',
                       help='Generate asset catalog markdown file')
    
    args = parser.parse_args()
    
    if args.catalog:
        generate_asset_catalog()
        return
    
    dry_run = not args.execute
    
    success, processed, copied = organize_assets(dry_run=dry_run)
    
    if not dry_run and success:
        print("\nNext steps:")
        print("1. Add new asset files to Content.mgcb")
        print("2. Update game code to reference new asset paths")
        print("3. Test loading and rendering of new assets")
    elif dry_run:
        print("\nTo actually copy files, run with --execute flag")


if __name__ == "__main__":
    main()
