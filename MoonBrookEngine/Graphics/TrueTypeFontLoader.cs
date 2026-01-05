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
        
        // Initialize StbTrueType
        var fontInfo = new StbTrueType.stbtt_fontinfo();
        unsafe
        {
            fixed (byte* dataPtr = fontData)
            {
                if (StbTrueType.stbtt_InitFont(fontInfo, dataPtr, 0) == 0)
                {
                    throw new Exception($"Failed to initialize font: {ttfPath}");
                }
            }
        }

        // Calculate scale for the desired font size
        float scale = StbTrueType.stbtt_ScaleForPixelHeight(fontInfo, fontSize);

        // Get font metrics
        int ascent, descent, lineGap;
        StbTrueType.stbtt_GetFontVMetrics(fontInfo, out ascent, out descent, out lineGap);
        
        float lineSpacing = (ascent - descent + lineGap) * scale;

        // Pre-calculate character sizes for atlas packing
        var charList = characters.ToList();
        var charData = new Dictionary<char, CharData>();
        int maxCharHeight = 0;

        foreach (char c in charList)
        {
            int glyphIndex = StbTrueType.stbtt_FindGlyphIndex(fontInfo, c);
            if (glyphIndex == 0 && c != ' ') // Missing glyph (except space)
                continue;

            int advance, leftSideBearing;
            StbTrueType.stbtt_GetGlyphHMetrics(fontInfo, glyphIndex, out advance, out leftSideBearing);

            int x0, y0, x1, y1;
            StbTrueType.stbtt_GetGlyphBitmapBox(fontInfo, glyphIndex, scale, scale, out x0, out y0, out x1, out y1);

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

            maxCharHeight = Math.Max(maxCharHeight, height);
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
            atlasPixels[i + 3] = 0;   // A
        }

        // Rasterize each character into the atlas
        foreach (var kvp in charData)
        {
            char c = kvp.Key;
            var data = kvp.Value;

            if (data.Width == 0 || data.Height == 0)
                continue;

            // Allocate buffer for this character
            byte[] charBitmap = new byte[data.Width * data.Height];

            unsafe
            {
                fixed (byte* fontDataPtr = fontData)
                fixed (byte* charBitmapPtr = charBitmap)
                {
                    StbTrueType.stbtt_MakeGlyphBitmap(
                        fontInfo,
                        charBitmapPtr,
                        data.Width,
                        data.Height,
                        data.Width,
                        scale,
                        scale,
                        data.GlyphIndex
                    );
                }
            }

            // Copy character bitmap into atlas
            for (int y = 0; y < data.Height; y++)
            {
                for (int x = 0; x < data.Width; x++)
                {
                    int srcIdx = y * data.Width + x;
                    int dstX = data.AtlasX + x;
                    int dstY = data.AtlasY + y;
                    int dstIdx = (dstY * atlasWidth + dstX) * 4;

                    byte alpha = charBitmap[srcIdx];
                    atlasPixels[dstIdx + 3] = alpha; // Set alpha channel
                }
            }
        }

        // Create OpenGL texture from atlas
        var atlasTexture = new Texture2D(_gl, atlasWidth, atlasHeight);
        atlasTexture.SetData(atlasPixels);

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
