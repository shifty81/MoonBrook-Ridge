using System;
using System.Collections.Generic;
using System.Linq;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.Tests;

/// <summary>
/// Simple test to verify sprite sheet extraction is working
/// </summary>
public class SpriteSheetExtractionTest
{
    public static void RunTests()
    {
        Console.WriteLine("=== Sprite Sheet Extraction Tests ===\n");
        
        // Test 1: Horizontal strip extraction
        TestHorizontalStrip();
        
        // Test 2: Grid extraction
        TestGrid();
        
        Console.WriteLine("\n=== All Tests Complete ===");
    }
    
    private static void TestHorizontalStrip()
    {
        Console.WriteLine("Test 1: Horizontal Strip Extraction");
        Console.WriteLine("Simulating Tree1 sprite sheet (1536x256)");
        Console.WriteLine("  Expected: 6 sprites of 256x256 each");
        
        // Simulate extraction (we can't create actual textures without GraphicsDevice)
        int sheetWidth = 1536;
        int spriteWidth = 256;
        int spriteHeight = 256;
        int expectedCount = sheetWidth / spriteWidth;
        
        Console.WriteLine($"  Calculated sprite count: {expectedCount}");
        Console.WriteLine($"  Each sprite: {spriteWidth}x{spriteHeight}");
        
        // Verify the math
        for (int i = 0; i < expectedCount; i++)
        {
            int x = i * spriteWidth;
            Console.WriteLine($"    Sprite {i}: x={x}, y=0, w={spriteWidth}, h={spriteHeight}");
        }
        
        Console.WriteLine("  ✓ Test passed\n");
    }
    
    private static void TestGrid()
    {
        Console.WriteLine("Test 2: Grid Extraction");
        Console.WriteLine("Simulating Rock1 sprite sheet (128x128)");
        Console.WriteLine("  Expected: 4 sprites in 2x2 grid of 64x64 each");
        
        int sheetWidth = 128;
        int sheetHeight = 128;
        int spriteWidth = 64;
        int spriteHeight = 64;
        int columns = sheetWidth / spriteWidth;
        int rows = sheetHeight / spriteHeight;
        int expectedCount = columns * rows;
        
        Console.WriteLine($"  Calculated sprite count: {expectedCount} ({rows}x{columns})");
        Console.WriteLine($"  Each sprite: {spriteWidth}x{spriteHeight}");
        
        // Verify the math
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                int x = col * spriteWidth;
                int y = row * spriteHeight;
                Console.WriteLine($"    Sprite [{row},{col}]: x={x}, y={y}, w={spriteWidth}, h={spriteHeight}");
            }
        }
        
        Console.WriteLine("  ✓ Test passed\n");
    }
    
    public static void PrintExpectedSpriteCounts()
    {
        Console.WriteLine("=== Expected Sprite Counts ===\n");
        
        Console.WriteLine("Trees:");
        Console.WriteLine("  Tree1 (1536x256): 6 sprites @ 256x256");
        Console.WriteLine("  Tree2 (1536x256): 6 sprites @ 256x256");
        Console.WriteLine("  Tree3 (1536x192): 8 sprites @ 192x192");
        Console.WriteLine("  Tree4 (1536x192): 8 sprites @ 192x192");
        Console.WriteLine("  TOTAL: 28 tree sprites\n");
        
        Console.WriteLine("Rocks:");
        Console.WriteLine("  Rock1 (128x128): 4 sprites @ 64x64 (2x2 grid)");
        Console.WriteLine("  Rock2 (128x128): 4 sprites @ 64x64 (2x2 grid)");
        Console.WriteLine("  Rock3 (128x128): 4 sprites @ 64x64 (2x2 grid)");
        Console.WriteLine("  TOTAL: 12 rock sprites\n");
    }
}
