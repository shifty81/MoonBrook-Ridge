#!/usr/bin/env python3
"""
Generate better placeholder tile textures (16x16) for MoonBrook Ridge game.
These will look much better than solid colors while still being easy to replace later.
"""

from PIL import Image, ImageDraw, ImageFilter
import numpy as np
import random
import os

# Set random seed for consistency
random.seed(42)
np.random.seed(42)

def add_noise(img, intensity=0.1):
    """Add subtle noise to make texture more interesting"""
    arr = np.array(img, dtype=float)
    noise = np.random.normal(0, intensity * 255, arr.shape)
    arr = np.clip(arr + noise, 0, 255)
    return Image.fromarray(arr.astype('uint8'))

def create_grass_tile(base_color=(34, 139, 34), variant=0):
    """Create a textured grass tile with blades and variation"""
    img = Image.new('RGBA', (16, 16), base_color + (255,))
    draw = ImageDraw.Draw(img)
    
    # Add some color variation
    for y in range(16):
        for x in range(16):
            r, g, b = base_color
            # Add subtle color variation
            variation = random.randint(-10, 10)
            new_g = max(0, min(255, g + variation))
            img.putpixel((x, y), (r, new_g, b, 255))
    
    # Add some grass blade details
    blade_color = (max(0, base_color[0] - 20), 
                   min(255, base_color[1] + 20), 
                   max(0, base_color[2] - 10), 
                   255)
    
    # Random grass blades
    num_blades = 8 + variant * 2
    for _ in range(num_blades):
        x = random.randint(0, 15)
        y = random.randint(0, 15)
        # Small vertical line for grass blade
        if y < 15:
            img.putpixel((x, y), blade_color)
            if random.random() > 0.5 and y < 14:
                img.putpixel((x, y + 1), blade_color)
    
    # Add subtle noise
    img = add_noise(img, 0.05)
    
    return img

def create_dirt_tile(base_color=(139, 90, 43)):
    """Create a textured dirt tile with grain"""
    img = Image.new('RGBA', (16, 16), base_color + (255,))
    
    # Add variation and spots
    for y in range(16):
        for x in range(16):
            r, g, b = base_color
            variation = random.randint(-15, 15)
            new_r = max(0, min(255, r + variation))
            new_g = max(0, min(255, g + variation))
            new_b = max(0, min(255, b + variation))
            img.putpixel((x, y), (new_r, new_g, new_b, 255))
    
    # Add some darker spots/pebbles
    draw = ImageDraw.Draw(img)
    for _ in range(8):
        x = random.randint(0, 15)
        y = random.randint(0, 15)
        dark = (max(0, base_color[0] - 30), 
                max(0, base_color[1] - 30), 
                max(0, base_color[2] - 30))
        if random.random() > 0.5:
            draw.point((x, y), fill=dark + (255,))
        else:
            # Small cluster
            for dx, dy in [(0, 0), (1, 0), (0, 1)]:
                if x + dx < 16 and y + dy < 16:
                    draw.point((x + dx, y + dy), fill=dark + (255,))
    
    img = add_noise(img, 0.06)
    
    return img

def create_water_tile(base_color=(30, 100, 200)):
    """Create animated water tile with wave pattern"""
    img = Image.new('RGBA', (16, 16), base_color + (255,))
    draw = ImageDraw.Draw(img)
    
    # Create wave pattern
    for y in range(16):
        for x in range(16):
            r, g, b = base_color
            # Create wave effect
            wave = int(10 * np.sin((x + y) * 0.5))
            new_b = max(0, min(255, b + wave))
            new_g = max(0, min(255, g + wave // 2))
            img.putpixel((x, y), (r, new_g, new_b, 255))
    
    # Add some lighter highlights
    for _ in range(5):
        x = random.randint(0, 15)
        y = random.randint(0, 15)
        light = (min(255, base_color[0] + 40), 
                 min(255, base_color[1] + 60), 
                 min(255, base_color[2] + 40))
        draw.point((x, y), fill=light + (255,))
    
    img = add_noise(img, 0.04)
    
    return img

def create_sand_tile(base_color=(244, 164, 96)):
    """Create textured sand tile"""
    img = Image.new('RGBA', (16, 16), base_color + (255,))
    
    # Add grainy texture
    for y in range(16):
        for x in range(16):
            r, g, b = base_color
            variation = random.randint(-12, 12)
            new_r = max(0, min(255, r + variation))
            new_g = max(0, min(255, g + variation))
            new_b = max(0, min(255, b + variation))
            img.putpixel((x, y), (new_r, new_g, new_b, 255))
    
    # Add some darker sand grains
    for _ in range(15):
        x = random.randint(0, 15)
        y = random.randint(0, 15)
        dark = (max(0, base_color[0] - 20), 
                max(0, base_color[1] - 20), 
                max(0, base_color[2] - 20))
        img.putpixel((x, y), dark + (255,))
    
    img = add_noise(img, 0.07)
    
    return img

def create_stone_tile(base_color=(128, 128, 128)):
    """Create textured stone tile"""
    img = Image.new('RGBA', (16, 16), base_color + (255,))
    
    # Add rocky variation
    for y in range(16):
        for x in range(16):
            r, g, b = base_color
            variation = random.randint(-20, 20)
            new_val = max(0, min(255, r + variation))
            img.putpixel((x, y), (new_val, new_val, new_val, 255))
    
    # Add cracks/texture lines
    draw = ImageDraw.Draw(img)
    dark = (max(0, base_color[0] - 40), 
            max(0, base_color[1] - 40), 
            max(0, base_color[2] - 40))
    
    # Random crack lines
    for _ in range(3):
        x = random.randint(0, 15)
        y = random.randint(0, 15)
        if random.random() > 0.5:
            # Horizontal crack
            for i in range(random.randint(2, 5)):
                if x + i < 16:
                    draw.point((x + i, y), fill=dark + (255,))
        else:
            # Vertical crack
            for i in range(random.randint(2, 5)):
                if y + i < 16:
                    draw.point((x, y + i), fill=dark + (255,))
    
    img = add_noise(img, 0.08)
    
    return img

def create_tilled_soil_tile(base_color=(101, 67, 33), watered=False):
    """Create tilled soil tile with furrows"""
    if watered:
        # Darker when watered
        base_color = (max(0, base_color[0] - 30), 
                      max(0, base_color[1] - 17), 
                      max(0, base_color[2] - 10))
    
    img = Image.new('RGBA', (16, 16), base_color + (255,))
    draw = ImageDraw.Draw(img)
    
    # Add variation
    for y in range(16):
        for x in range(16):
            r, g, b = base_color
            variation = random.randint(-10, 10)
            new_r = max(0, min(255, r + variation))
            new_g = max(0, min(255, g + variation))
            new_b = max(0, min(255, b + variation))
            img.putpixel((x, y), (new_r, new_g, new_b, 255))
    
    # Add horizontal furrows (tilled lines)
    dark = (max(0, base_color[0] - 20), 
            max(0, base_color[1] - 20), 
            max(0, base_color[2] - 20))
    
    # Create horizontal tilled pattern
    for y in range(0, 16, 4):
        for x in range(16):
            if random.random() > 0.3:
                draw.point((x, y), fill=dark + (255,))
            if y + 1 < 16 and random.random() > 0.6:
                draw.point((x, y + 1), fill=dark + (255,))
    
    img = add_noise(img, 0.06)
    
    return img

def create_rock_tile(base_color=(100, 100, 100)):
    """Create rocky/boulder tile"""
    img = Image.new('RGBA', (16, 16), base_color + (255,))
    
    # Create boulder-like appearance
    for y in range(16):
        for x in range(16):
            # Distance from center
            dist = np.sqrt((x - 8) ** 2 + (y - 8) ** 2)
            brightness = int(20 - dist * 2)  # Lighter in center
            
            r, g, b = base_color
            variation = random.randint(-15, 15) + brightness
            new_val = max(0, min(255, r + variation))
            img.putpixel((x, y), (new_val, new_val, new_val, 255))
    
    # Add some highlights
    draw = ImageDraw.Draw(img)
    light = (min(255, base_color[0] + 50), 
             min(255, base_color[1] + 50), 
             min(255, base_color[2] + 50))
    
    for _ in range(5):
        x = random.randint(4, 11)
        y = random.randint(4, 11)
        draw.point((x, y), fill=light + (255,))
    
    img = add_noise(img, 0.08)
    
    return img

def main():
    output_dir = "/home/runner/work/MoonBrook-Ridge/MoonBrook-Ridge/MoonBrookRidge/Content/Textures/Tiles"
    
    print("Generating placeholder tiles...")
    
    # Grass variants
    grass = create_grass_tile((34, 139, 34), 0)
    grass.save(f"{output_dir}/grass.png")
    print("✓ grass.png")
    
    grass_01 = create_grass_tile((50, 160, 50), 1)
    grass_01.save(f"{output_dir}/grass_01.png")
    print("✓ grass_01.png")
    
    grass_02 = create_grass_tile((40, 150, 40), 2)
    grass_02.save(f"{output_dir}/grass_02.png")
    print("✓ grass_02.png")
    
    grass_03 = create_grass_tile((28, 120, 28), 1)
    grass_03.save(f"{output_dir}/grass_03.png")
    print("✓ grass_03.png")
    
    # Dirt variants
    dirt_01 = create_dirt_tile((139, 90, 43))
    dirt_01.save(f"{output_dir}/dirt_01.png")
    print("✓ dirt_01.png")
    
    dirt_02 = create_dirt_tile((125, 80, 38))
    dirt_02.save(f"{output_dir}/dirt_02.png")
    print("✓ dirt_02.png")
    
    # Water
    water_01 = create_water_tile((30, 100, 200))
    water_01.save(f"{output_dir}/water_01.png")
    print("✓ water_01.png")
    
    # Sand
    sand_01 = create_sand_tile((244, 164, 96))
    sand_01.save(f"{output_dir}/sand_01.png")
    print("✓ sand_01.png")
    
    # Stone
    stone_01 = create_stone_tile((128, 128, 128))
    stone_01.save(f"{output_dir}/stone_01.png")
    print("✓ stone_01.png")
    
    # Rock
    rock = create_rock_tile((100, 100, 100))
    rock.save(f"{output_dir}/rock.png")
    print("✓ rock.png")
    
    # Tilled soil (dry)
    tilled_dry = create_tilled_soil_tile((101, 67, 33), watered=False)
    tilled_dry.save(f"{output_dir}/tilled_soil_dry.png")
    print("✓ tilled_soil_dry.png")
    
    # Tilled soil (watered)
    tilled_watered = create_tilled_soil_tile((101, 67, 33), watered=True)
    tilled_watered.save(f"{output_dir}/tilled_soil_watered.png")
    print("✓ tilled_soil_watered.png")
    
    # Additional tiles
    tilled_01 = create_tilled_soil_tile((95, 62, 30), watered=False)
    tilled_01.save(f"{output_dir}/tilled_01.png")
    print("✓ tilled_01.png")
    
    tilled_02 = create_tilled_soil_tile((88, 58, 28), watered=True)
    tilled_02.save(f"{output_dir}/tilled_02.png")
    print("✓ tilled_02.png")
    
    # Wooden floor
    wooden = create_dirt_tile((139, 90, 43))  # Similar to dirt but for floor
    wooden.save(f"{output_dir}/wooden_floor.png")
    print("✓ wooden_floor.png")
    
    print("\n✅ All placeholder tiles generated successfully!")
    print(f"Location: {output_dir}")

if __name__ == "__main__":
    main()
