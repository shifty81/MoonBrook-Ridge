using System.Numerics;
using Silk.NET.OpenGL;

namespace MoonBrookEngine.Graphics;

/// <summary>
/// Bitmap font for efficient text rendering using pre-rendered character atlas
/// Supports basic ASCII characters (32-126)
/// </summary>
public class BitmapFont : IDisposable
{
    private readonly GL _gl;
    private Texture2D? _atlasTexture;
    
    // Character information
    private readonly Dictionary<char, CharacterInfo> _characters;
    
    public float LineSpacing { get; set; }
    public float Spacing { get; set; }
    public char DefaultCharacter { get; set; }
    
    /// <summary>
    /// Character glyph information
    /// </summary>
    public struct CharacterInfo
    {
        public Math.Rectangle SourceRect;  // Position in atlas texture
        public Vector2 Offset;              // Rendering offset
        public float XAdvance;              // Horizontal advance after character
    }
    
    public BitmapFont(GL gl)
    {
        _gl = gl;
        _characters = new Dictionary<char, CharacterInfo>();
        LineSpacing = 16f;
        Spacing = 0f;
        DefaultCharacter = '?';
    }
    
    /// <summary>
    /// Create a simple default font with fixed-width characters
    /// This generates a basic bitmap font atlas at runtime
    /// </summary>
    public static BitmapFont CreateDefault(GL gl, int fontSize = 16)
    {
        var font = new BitmapFont(gl)
        {
            LineSpacing = fontSize,
            Spacing = 1f
        };
        
        // Create a simple texture atlas with fixed-width characters
        int charWidth = fontSize / 2;
        int charHeight = fontSize;
        int charsPerRow = 16;
        int numChars = 95; // ASCII 32-126
        int rows = (numChars + charsPerRow - 1) / charsPerRow;
        
        int atlasWidth = charsPerRow * charWidth;
        int atlasHeight = rows * charHeight;
        
        // Generate a simple monochrome font atlas
        // Each character is represented as a simple rectangle for now
        byte[] atlasData = new byte[atlasWidth * atlasHeight * 4];
        
        // Fill with transparent background
        for (int i = 0; i < atlasData.Length; i += 4)
        {
            atlasData[i + 0] = 255; // R
            atlasData[i + 1] = 255; // G
            atlasData[i + 2] = 255; // B
            atlasData[i + 3] = 0;   // A (transparent)
        }
        
        // Draw simple box glyphs for each character (for visibility)
        for (int i = 32; i < 127; i++)
        {
            int charIndex = i - 32;
            int col = charIndex % charsPerRow;
            int row = charIndex / charsPerRow;
            
            int startX = col * charWidth;
            int startY = row * charHeight;
            
            // Draw a simple border for each character (makes them visible)
            // Top and bottom borders
            for (int x = startX + 1; x < startX + charWidth - 1; x++)
            {
                // Top border
                int topIdx = ((startY + 1) * atlasWidth + x) * 4;
                if (topIdx >= 0 && topIdx < atlasData.Length - 3)
                {
                    atlasData[topIdx + 3] = 255; // A (opaque)
                }
                
                // Bottom border
                int bottomIdx = ((startY + charHeight - 2) * atlasWidth + x) * 4;
                if (bottomIdx >= 0 && bottomIdx < atlasData.Length - 3)
                {
                    atlasData[bottomIdx + 3] = 255; // A (opaque)
                }
            }
            
            // Left and right borders
            for (int y = startY + 1; y < startY + charHeight - 1; y++)
            {
                // Left border
                int leftIdx = (y * atlasWidth + startX + 1) * 4;
                if (leftIdx >= 0 && leftIdx < atlasData.Length - 3)
                {
                    atlasData[leftIdx + 3] = 255; // A (opaque)
                }
                
                // Right border
                int rightIdx = (y * atlasWidth + (startX + charWidth - 2)) * 4;
                if (rightIdx >= 0 && rightIdx < atlasData.Length - 3)
                {
                    atlasData[rightIdx + 3] = 255; // A (opaque)
                }
            }
            
            // Add a small dot in the center for better visibility
            int centerX = startX + charWidth / 2;
            int centerY = startY + charHeight / 2;
            int centerIdx = (centerY * atlasWidth + centerX) * 4;
            if (centerIdx >= 0 && centerIdx < atlasData.Length - 3)
            {
                atlasData[centerIdx + 3] = 255; // A (opaque)
            }
        }
        
        // Create texture from atlas data
        var atlasTexture = new Texture2D(gl, atlasData, atlasWidth, atlasHeight);
        
        // Set up character information
        var characterData = new Dictionary<char, CharacterInfo>();
        for (int i = 32; i < 127; i++)
        {
            char c = (char)i;
            int charIndex = i - 32;
            int col = charIndex % charsPerRow;
            int row = charIndex / charsPerRow;
            
            characterData[c] = new CharacterInfo
            {
                SourceRect = new Math.Rectangle(
                    col * charWidth,
                    row * charHeight,
                    charWidth,
                    charHeight
                ),
                Offset = Vector2.Zero,
                XAdvance = charWidth + 1
            };
        }
        
        font.LoadFromAtlas(atlasTexture, characterData);
        
        return font;
    }
    
    /// <summary>
    /// Load font from a bitmap atlas and character data
    /// </summary>
    public void LoadFromAtlas(Texture2D atlasTexture, Dictionary<char, CharacterInfo> characterData)
    {
        _atlasTexture = atlasTexture;
        _characters.Clear();
        foreach (var kvp in characterData)
        {
            _characters[kvp.Key] = kvp.Value;
        }
    }
    
    /// <summary>
    /// Measure the size of a string when rendered with this font
    /// </summary>
    public Vector2 MeasureString(string text)
    {
        if (string.IsNullOrEmpty(text))
            return Vector2.Zero;
        
        float maxWidth = 0f;
        float currentWidth = 0f;
        int lines = 1;
        
        foreach (char c in text)
        {
            if (c == '\n')
            {
                lines++;
                maxWidth = System.Math.Max(maxWidth, currentWidth);
                currentWidth = 0f;
            }
            else
            {
                if (_characters.TryGetValue(c, out var charInfo))
                {
                    currentWidth += charInfo.XAdvance + Spacing;
                }
                else if (_characters.TryGetValue(DefaultCharacter, out charInfo))
                {
                    currentWidth += charInfo.XAdvance + Spacing;
                }
            }
        }
        
        maxWidth = System.Math.Max(maxWidth, currentWidth);
        
        return new Vector2(maxWidth, lines * LineSpacing);
    }
    
    /// <summary>
    /// Get character info for rendering
    /// </summary>
    public bool TryGetCharacter(char c, out CharacterInfo charInfo)
    {
        if (_characters.TryGetValue(c, out charInfo))
            return true;
        
        if (_characters.TryGetValue(DefaultCharacter, out charInfo))
            return true;
        
        charInfo = default;
        return false;
    }
    
    /// <summary>
    /// Get the atlas texture for rendering
    /// </summary>
    public Texture2D? GetAtlasTexture() => _atlasTexture;
    
    /// <summary>
    /// Check if this font has an atlas texture
    /// </summary>
    public bool HasAtlas => _atlasTexture != null;
    
    public void Dispose()
    {
        // Don't dispose texture as it might be managed by ResourceManager
        _characters.Clear();
    }
}
