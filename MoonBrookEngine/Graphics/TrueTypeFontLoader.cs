using Silk.NET.OpenGL;
using StbTrueTypeSharp;

namespace MoonBrookEngine.Graphics;

/// <summary>
/// Loads TrueType fonts and generates bitmap font atlases
/// Uses StbTrueTypeSharp for font rasterization
/// </summary>
public class TrueTypeFontLoader
{
    private readonly GL _gl;

    public TrueTypeFontLoader(GL gl)
    {
        _gl = gl;
    }

    /// <summary>
    /// Load a TrueType font and generate a bitmap font atlas
    /// </summary>
    public BitmapFont LoadFromFile(string ttfPath, float fontSize, IEnumerable<char> characters, float spacing = 0f)
    {
        // Read TTF file
        byte[] fontData = File.ReadAllBytes(ttfPath);
        
        // Initialize StbTrueType using the high-level API
        var fontInfo = StbTrueType.CreateFont(fontData, 0);
        if (fontInfo == null)
        {
            throw new Exception($"Failed to initialize font: {ttfPath}");
        }

        // Calculate scale for the desired font size
        float scale = fontInfo.ScaleForPixelHeight(fontSize);

        // Get font metrics
        fontInfo.GetFontVMetrics(out int ascent, out int descent, out int lineGap);
        float lineSpacing = (ascent - descent + lineGap) * scale;

        // Pre-calculate character sizes for atlas packing
        var charList = characters.ToList();
        var charData = new Dictionary<char, CharData>();
        int maxCharHeight = 0;

        foreach (char c in charList)
        {
            int glyphIndex = fontInfo.FindGlyphIndex(c);
            if (glyphIndex == 0 && c != ' ') // Missing glyph (except space)
                continue;

            fontInfo.GetGlyphHMetrics(glyphIndex, out int advance, out int leftSideBearing);
            fontInfo.GetGlyphBitmapBox(glyphIndex, scale, scale, out int x0, out int y0, out int x1, out int y1);

            int width = x1 - x0;
            int height = y1 - y0;

            charData[c] = new CharData
            {
                GlyphIndex = glyphIndex,
                Width = width,
                Height = height,
                OffsetX = x0,
                OffsetY = y0,
                Advance = advance * scale,
                LeftSideBearing = leftSideBearing * scale
            };

            maxCharHeight = System.Math.Max(maxCharHeight, height);
        }

        // Create atlas layout (simple horizontal packing with wrapping)
        int atlasWidth = 512;
        int padding = 2;
        int currentX = padding;
        int currentY = padding;
        int rowHeight = maxCharHeight + padding;

        foreach (var kvp in charData)
        {
            var data = kvp.Value;
            
            if (currentX + data.Width + padding > atlasWidth)
            {
                // Wrap to next row
                currentX = padding;
                currentY += rowHeight;
            }

            data.AtlasX = currentX;
            data.AtlasY = currentY;
            charData[kvp.Key] = data;

            currentX += data.Width + padding;
        }

        int atlasHeight = currentY + rowHeight;
        
        // Round up atlas height to power of 2 for better GPU compatibility
        atlasHeight = NextPowerOf2(atlasHeight);

        // Create atlas bitmap
        byte[] atlasPixels = new byte[atlasWidth * atlasHeight * 4];
        
        // Fill with transparent white
        for (int i = 0; i < atlasPixels.Length; i += 4)
        {
            atlasPixels[i] = 255;     // R
            atlasPixels[i + 1] = 255; // G
            atlasPixels[i + 2] = 255; // B
            atlasPixels[i + 3] = 0;   // A (transparent)
        }

        // Rasterize each character into the atlas
        foreach (var kvp in charData)
        {
            char c = kvp.Key;
            var data = kvp.Value;

            if (data.Width == 0 || data.Height == 0)
                continue;

            // Rasterize character using StbTrueType high-level API
            var charBitmap = fontInfo.GetGlyphBitmap(scale, scale, data.GlyphIndex, out int bitmapWidth, out int bitmapHeight);
            
            if (charBitmap == null || charBitmap.Length == 0)
                continue;

            // Copy character bitmap into atlas
            for (int y = 0; y < bitmapHeight && y < data.Height; y++)
            {
                for (int x = 0; x < bitmapWidth && x < data.Width; x++)
                {
                    int srcIdx = y * bitmapWidth + x;
                    int dstX = data.AtlasX + x;
                    int dstY = data.AtlasY + y;
                    int dstIdx = (dstY * atlasWidth + dstX) * 4;

                    if (srcIdx >= 0 && srcIdx < charBitmap.Length &&
                        dstIdx >= 0 && dstIdx < atlasPixels.Length - 3)
                    {
                        byte alpha = charBitmap[srcIdx];
                        atlasPixels[dstIdx + 3] = alpha; // Set alpha channel
                    }
                }
            }
        }

        // Create OpenGL texture from atlas using correct Texture2D constructor
        var atlasTexture = new Texture2D(_gl, atlasPixels, atlasWidth, atlasHeight);

        // Create BitmapFont
        var font = new BitmapFont(_gl)
        {
            LineSpacing = lineSpacing,
            Spacing = spacing,
            DefaultCharacter = '?'
        };

        font.SetAtlasTexture(atlasTexture);

        // Add character information to font
        foreach (var kvp in charData)
        {
            var data = kvp.Value;
            
            font.AddCharacter(kvp.Key, new BitmapFont.CharacterInfo
            {
                SourceRect = new MoonBrookEngine.Math.Rectangle(
                    data.AtlasX,
                    data.AtlasY,
                    data.Width,
                    data.Height
                ),
                Offset = new System.Numerics.Vector2(data.OffsetX, data.OffsetY),
                XAdvance = data.Advance
            });
        }

        return font;
    }

    private struct CharData
    {
        public int GlyphIndex;
        public int Width;
        public int Height;
        public int OffsetX;
        public int OffsetY;
        public float Advance;
        public float LeftSideBearing;
        public int AtlasX;
        public int AtlasY;
    }

    private static int NextPowerOf2(int value)
    {
        value--;
        value |= value >> 1;
        value |= value >> 2;
        value |= value >> 4;
        value |= value >> 8;
        value |= value >> 16;
        value++;
        return value;
    }
}
