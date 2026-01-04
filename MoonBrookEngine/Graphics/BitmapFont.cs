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
        // For now, we'll create character info but use solid rectangles
        // In a real implementation, this would load from a font atlas image
        
        int charWidth = fontSize / 2;
        int charHeight = fontSize;
        int charsPerRow = 16;
        
        // Generate character info for printable ASCII (32-126)
        for (int i = 32; i < 127; i++)
        {
            char c = (char)i;
            int col = (i - 32) % charsPerRow;
            int row = (i - 32) / charsPerRow;
            
            font._characters[c] = new CharacterInfo
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
