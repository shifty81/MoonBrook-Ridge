using System.Numerics;
using Silk.NET.OpenGL;

namespace MoonBrookEngine.Graphics;

/// <summary>
/// Simple font rasterizer for generating basic ASCII font glyphs at runtime
/// Creates readable monospace characters without external dependencies
/// </summary>
internal static class SimpleFontRasterizer
{
    /// <summary>
    /// Rasterize a character to a pixel array
    /// </summary>
    public static byte[] RasterizeCharacter(char c, int width, int height)
    {
        byte[] pixels = new byte[width * height * 4];
        
        // Fill with transparent
        for (int i = 0; i < pixels.Length; i += 4)
        {
            pixels[i + 0] = 255; // R
            pixels[i + 1] = 255; // G
            pixels[i + 2] = 255; // B
            pixels[i + 3] = 0;   // A (transparent)
        }
        
        // Draw the character using simple shapes
        DrawCharacter(pixels, c, width, height);
        
        return pixels;
    }
    
    private static void DrawCharacter(byte[] pixels, char c, int width, int height)
    {
        // Simple 5x7 font patterns (scaled to fit width/height)
        // Each character is defined as lines and shapes
        
        int centerX = width / 2;
        int centerY = height / 2;
        int margin = System.Math.Max(1, width / 8);
        
        // Calculate scaled dimensions for 5x7 grid
        int gridWidth = width - margin * 2;
        int gridHeight = height - margin * 2;
        
        // Handle both upper and lower case
        char upper = char.ToUpper(c);
        
        switch (upper)
        {
            case ' ': // Space
                break;
                
            case 'A':
                DrawLine(pixels, width, height, margin, height - margin, centerX, margin);
                DrawLine(pixels, width, height, centerX, margin, width - margin, height - margin);
                DrawLine(pixels, width, height, margin + gridWidth / 4, centerY, width - margin - gridWidth / 4, centerY);
                break;
                
            case 'B':
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, margin, width - margin - 2, margin);
                DrawLine(pixels, width, height, width - margin - 2, margin, width - margin, margin + gridHeight / 6);
                DrawLine(pixels, width, height, width - margin, margin + gridHeight / 6, width - margin - 2, centerY);
                DrawLine(pixels, width, height, margin, centerY, width - margin - 2, centerY);
                DrawLine(pixels, width, height, width - margin - 2, centerY, width - margin, centerY + gridHeight / 6);
                DrawLine(pixels, width, height, width - margin, centerY + gridHeight / 6, width - margin - 2, height - margin);
                DrawLine(pixels, width, height, width - margin - 2, height - margin, margin, height - margin);
                break;
                
            case 'C':
                DrawLine(pixels, width, height, width - margin, margin, margin, margin);
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, height - margin, width - margin, height - margin);
                break;
                
            case 'D':
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, margin, width - margin - 2, margin);
                DrawLine(pixels, width, height, width - margin - 2, margin, width - margin, centerY);
                DrawLine(pixels, width, height, width - margin, centerY, width - margin - 2, height - margin);
                DrawLine(pixels, width, height, width - margin - 2, height - margin, margin, height - margin);
                break;
                
            case 'E':
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, margin, width - margin, margin);
                DrawLine(pixels, width, height, margin, centerY, width - margin * 2, centerY);
                DrawLine(pixels, width, height, margin, height - margin, width - margin, height - margin);
                break;
                
            case 'F':
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, margin, width - margin, margin);
                DrawLine(pixels, width, height, margin, centerY, width - margin * 2, centerY);
                break;
                
            case 'G':
                DrawLine(pixels, width, height, width - margin, margin, margin, margin);
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, height - margin, width - margin, height - margin);
                DrawLine(pixels, width, height, width - margin, height - margin, width - margin, centerY);
                DrawLine(pixels, width, height, width - margin, centerY, centerX, centerY);
                break;
                
            case 'H':
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, width - margin, margin, width - margin, height - margin);
                DrawLine(pixels, width, height, margin, centerY, width - margin, centerY);
                break;
                
            case 'I':
                DrawLine(pixels, width, height, margin, margin, width - margin, margin);
                DrawLine(pixels, width, height, centerX, margin, centerX, height - margin);
                DrawLine(pixels, width, height, margin, height - margin, width - margin, height - margin);
                break;
                
            case 'K':
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, width - margin, margin, margin, centerY);
                DrawLine(pixels, width, height, margin, centerY, width - margin, height - margin);
                break;
                
            case 'L':
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, height - margin, width - margin, height - margin);
                break;
                
            case 'M':
                DrawLine(pixels, width, height, margin, height - margin, margin, margin);
                DrawLine(pixels, width, height, margin, margin, centerX, centerY);
                DrawLine(pixels, width, height, centerX, centerY, width - margin, margin);
                DrawLine(pixels, width, height, width - margin, margin, width - margin, height - margin);
                break;
                
            case 'N':
                DrawLine(pixels, width, height, margin, height - margin, margin, margin);
                DrawLine(pixels, width, height, margin, margin, width - margin, height - margin);
                DrawLine(pixels, width, height, width - margin, height - margin, width - margin, margin);
                break;
                
            case 'O':
                DrawRect(pixels, width, height, margin, margin, gridWidth, gridHeight, false);
                break;
                
            case 'P':
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, margin, width - margin, margin);
                DrawLine(pixels, width, height, width - margin, margin, width - margin, centerY);
                DrawLine(pixels, width, height, width - margin, centerY, margin, centerY);
                break;
                
            case 'R':
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, margin, width - margin, margin);
                DrawLine(pixels, width, height, width - margin, margin, width - margin, centerY);
                DrawLine(pixels, width, height, width - margin, centerY, margin, centerY);
                DrawLine(pixels, width, height, margin, centerY, width - margin, height - margin);
                break;
                
            case 'S':
                DrawLine(pixels, width, height, width - margin, margin, margin, margin);
                DrawLine(pixels, width, height, margin, margin, margin, centerY);
                DrawLine(pixels, width, height, margin, centerY, width - margin, centerY);
                DrawLine(pixels, width, height, width - margin, centerY, width - margin, height - margin);
                DrawLine(pixels, width, height, width - margin, height - margin, margin, height - margin);
                break;
                
            case 'T':
                DrawLine(pixels, width, height, margin, margin, width - margin, margin);
                DrawLine(pixels, width, height, centerX, margin, centerX, height - margin);
                break;
                
            case 'U':
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, height - margin, width - margin, height - margin);
                DrawLine(pixels, width, height, width - margin, height - margin, width - margin, margin);
                break;
                
            case 'V':
                DrawLine(pixels, width, height, margin, margin, centerX, height - margin);
                DrawLine(pixels, width, height, centerX, height - margin, width - margin, margin);
                break;
                
            case 'W':
                DrawLine(pixels, width, height, margin, margin, margin + gridWidth / 4, height - margin);
                DrawLine(pixels, width, height, margin + gridWidth / 4, height - margin, centerX, centerY);
                DrawLine(pixels, width, height, centerX, centerY, width - margin - gridWidth / 4, height - margin);
                DrawLine(pixels, width, height, width - margin - gridWidth / 4, height - margin, width - margin, margin);
                break;
                
            case 'X':
                DrawLine(pixels, width, height, margin, margin, width - margin, height - margin);
                DrawLine(pixels, width, height, width - margin, margin, margin, height - margin);
                break;
                
            case 'Y':
                DrawLine(pixels, width, height, margin, margin, centerX, centerY);
                DrawLine(pixels, width, height, width - margin, margin, centerX, centerY);
                DrawLine(pixels, width, height, centerX, centerY, centerX, height - margin);
                break;
                
            case 'Z':
                DrawLine(pixels, width, height, margin, margin, width - margin, margin);
                DrawLine(pixels, width, height, width - margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, height - margin, width - margin, height - margin);
                break;
                
            // Numbers
            case '0':
                DrawRect(pixels, width, height, margin, margin, gridWidth, gridHeight, false);
                DrawLine(pixels, width, height, margin, margin, width - margin, height - margin);
                break;
                
            case '1':
                DrawLine(pixels, width, height, centerX, margin, centerX, height - margin);
                DrawLine(pixels, width, height, centerX - gridWidth / 4, margin + gridHeight / 4, centerX, margin);
                break;
                
            case '2':
                DrawLine(pixels, width, height, margin, margin, width - margin, margin);
                DrawLine(pixels, width, height, width - margin, margin, width - margin, centerY);
                DrawLine(pixels, width, height, width - margin, centerY, margin, centerY);
                DrawLine(pixels, width, height, margin, centerY, margin, height - margin);
                DrawLine(pixels, width, height, margin, height - margin, width - margin, height - margin);
                break;
                
            case '3':
                DrawLine(pixels, width, height, margin, margin, width - margin, margin);
                DrawLine(pixels, width, height, width - margin, margin, width - margin, height - margin);
                DrawLine(pixels, width, height, margin, height - margin, width - margin, height - margin);
                DrawLine(pixels, width, height, margin, centerY, width - margin, centerY);
                break;
                
            case '4':
                DrawLine(pixels, width, height, margin, margin, margin, centerY);
                DrawLine(pixels, width, height, margin, centerY, width - margin, centerY);
                DrawLine(pixels, width, height, width - margin, margin, width - margin, height - margin);
                break;
                
            case '5':
                DrawLine(pixels, width, height, width - margin, margin, margin, margin);
                DrawLine(pixels, width, height, margin, margin, margin, centerY);
                DrawLine(pixels, width, height, margin, centerY, width - margin, centerY);
                DrawLine(pixels, width, height, width - margin, centerY, width - margin, height - margin);
                DrawLine(pixels, width, height, width - margin, height - margin, margin, height - margin);
                break;
                
            case '6':
                DrawLine(pixels, width, height, width - margin, margin, margin, margin);
                DrawLine(pixels, width, height, margin, margin, margin, height - margin);
                DrawLine(pixels, width, height, margin, centerY, width - margin, centerY);
                DrawLine(pixels, width, height, width - margin, centerY, width - margin, height - margin);
                DrawLine(pixels, width, height, width - margin, height - margin, margin, height - margin);
                break;
                
            case '7':
                DrawLine(pixels, width, height, margin, margin, width - margin, margin);
                DrawLine(pixels, width, height, width - margin, margin, centerX, height - margin);
                break;
                
            case '8':
                DrawRect(pixels, width, height, margin, margin, gridWidth, gridHeight / 2, false);
                DrawRect(pixels, width, height, margin, centerY, gridWidth, gridHeight / 2, false);
                break;
                
            case '9':
                DrawLine(pixels, width, height, margin, margin, width - margin, margin);
                DrawLine(pixels, width, height, width - margin, margin, width - margin, height - margin);
                DrawLine(pixels, width, height, margin, margin, margin, centerY);
                DrawLine(pixels, width, height, margin, centerY, width - margin, centerY);
                DrawLine(pixels, width, height, margin, height - margin, width - margin, height - margin);
                break;
                
            // Punctuation
            case '.':
                DrawRect(pixels, width, height, centerX - 1, height - margin - 2, 3, 3, true);
                break;
                
            case ',':
                DrawRect(pixels, width, height, centerX - 1, height - margin - 2, 3, 3, true);
                DrawLine(pixels, width, height, centerX, height - margin, centerX - 2, height - margin + 3);
                break;
                
            case ':':
                DrawRect(pixels, width, height, centerX - 1, margin + gridHeight / 3, 3, 3, true);
                DrawRect(pixels, width, height, centerX - 1, height - margin - gridHeight / 3, 3, 3, true);
                break;
                
            case ';':
                DrawRect(pixels, width, height, centerX - 1, margin + gridHeight / 3, 3, 3, true);
                DrawRect(pixels, width, height, centerX - 1, height - margin - gridHeight / 3 - 3, 3, 3, true);
                // Draw tail below the bottom dot (but keep it within bounds)
                int tailStartY = height - margin - gridHeight / 3;
                DrawLine(pixels, width, height, centerX, tailStartY, centerX - 2, System.Math.Min(tailStartY + 3, height - margin));
                break;
                
            case '!':
                DrawLine(pixels, width, height, centerX, margin, centerX, centerY + 2);
                DrawRect(pixels, width, height, centerX - 1, height - margin - 2, 3, 3, true);
                break;
                
            case '?':
                DrawLine(pixels, width, height, margin, margin + gridHeight / 4, width - margin, margin + gridHeight / 4);
                DrawLine(pixels, width, height, width - margin, margin + gridHeight / 4, width - margin, centerY - 2);
                DrawLine(pixels, width, height, width - margin, centerY - 2, centerX, centerY + 2);
                DrawRect(pixels, width, height, centerX - 1, height - margin - 2, 3, 3, true);
                break;
                
            case '-':
            case '_':
                DrawLine(pixels, width, height, margin, centerY, width - margin, centerY);
                break;
                
            case '/':
                DrawLine(pixels, width, height, margin, height - margin, width - margin, margin);
                break;
                
            case '\\':
                DrawLine(pixels, width, height, margin, margin, width - margin, height - margin);
                break;
                
            case '|':
                DrawLine(pixels, width, height, centerX, margin, centerX, height - margin);
                break;
                
            case '>':
                DrawLine(pixels, width, height, margin, margin, width - margin, centerY);
                DrawLine(pixels, width, height, width - margin, centerY, margin, height - margin);
                break;
                
            case '<':
                DrawLine(pixels, width, height, width - margin, margin, margin, centerY);
                DrawLine(pixels, width, height, margin, centerY, width - margin, height - margin);
                break;
                
            case '+':
                DrawLine(pixels, width, height, centerX, margin, centerX, height - margin);
                DrawLine(pixels, width, height, margin, centerY, width - margin, centerY);
                break;
                
            case '=':
                DrawLine(pixels, width, height, margin, centerY - gridHeight / 6, width - margin, centerY - gridHeight / 6);
                DrawLine(pixels, width, height, margin, centerY + gridHeight / 6, width - margin, centerY + gridHeight / 6);
                break;
                
            case '(':
            case '[':
                DrawLine(pixels, width, height, width - margin, margin, centerX, margin);
                DrawLine(pixels, width, height, centerX, margin, centerX, height - margin);
                DrawLine(pixels, width, height, centerX, height - margin, width - margin, height - margin);
                break;
                
            case ')':
            case ']':
                DrawLine(pixels, width, height, margin, margin, centerX, margin);
                DrawLine(pixels, width, height, centerX, margin, centerX, height - margin);
                DrawLine(pixels, width, height, centerX, height - margin, margin, height - margin);
                break;
                
            default:
                // For unknown characters, draw a small dot
                DrawRect(pixels, width, height, centerX - 2, centerY - 2, 4, 4, true);
                break;
        }
    }
    
    private static void DrawLine(byte[] pixels, int width, int height, int x1, int y1, int x2, int y2)
    {
        // Bresenham's line algorithm
        int dx = System.Math.Abs(x2 - x1);
        int dy = System.Math.Abs(y2 - y1);
        int sx = x1 < x2 ? 1 : -1;
        int sy = y1 < y2 ? 1 : -1;
        int err = dx - dy;
        
        while (true)
        {
            SetPixel(pixels, width, height, x1, y1);
            
            if (x1 == x2 && y1 == y2) break;
            
            int e2 = 2 * err;
            if (e2 > -dy)
            {
                err -= dy;
                x1 += sx;
            }
            if (e2 < dx)
            {
                err += dx;
                y1 += sy;
            }
        }
    }
    
    private static void DrawRect(byte[] pixels, int width, int height, int x, int y, int w, int h, bool filled)
    {
        if (filled)
        {
            for (int py = y; py < y + h && py < height; py++)
            {
                for (int px = x; px < x + w && px < width; px++)
                {
                    SetPixel(pixels, width, height, px, py);
                }
            }
        }
        else
        {
            DrawLine(pixels, width, height, x, y, x + w, y);
            DrawLine(pixels, width, height, x + w, y, x + w, y + h);
            DrawLine(pixels, width, height, x + w, y + h, x, y + h);
            DrawLine(pixels, width, height, x, y + h, x, y);
        }
    }
    
    private static void SetPixel(byte[] pixels, int width, int height, int x, int y)
    {
        if (x < 0 || x >= width || y < 0 || y >= height)
            return;
        
        int idx = (y * width + x) * 4;
        if (idx >= 0 && idx < pixels.Length - 4)
        {
            pixels[idx + 3] = 255; // Set alpha to opaque
        }
    }
}
