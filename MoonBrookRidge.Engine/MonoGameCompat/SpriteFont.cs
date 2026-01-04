namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible SpriteFont for MoonBrookEngine
/// </summary>
public class SpriteFont
{
    internal MoonBrookEngine.Graphics.BitmapFont? InternalFont { get; set; }
    
    // Stub properties for compatibility
    public float LineSpacing 
    { 
        get => InternalFont?.LineSpacing ?? 16f;
        set { if (InternalFont != null) InternalFont.LineSpacing = value; }
    }
    
    public float Spacing 
    { 
        get => InternalFont?.Spacing ?? 0f;
        set { if (InternalFont != null) InternalFont.Spacing = value; }
    }
    
    public char? DefaultCharacter 
    { 
        get => InternalFont?.DefaultCharacter ?? '?';
        set { if (InternalFont != null) InternalFont.DefaultCharacter = value ?? '?'; }
    }
    
    internal SpriteFont()
    {
    }
    
    internal SpriteFont(MoonBrookEngine.Graphics.BitmapFont font)
    {
        InternalFont = font;
    }
    
    /// <summary>
    /// Measure the size of a string when rendered with this font
    /// </summary>
    public Vector2 MeasureString(string text)
    {
        if (InternalFont != null)
        {
            var size = InternalFont.MeasureString(text);
            return new Vector2(size.X, size.Y);
        }
        
        // Fallback stub implementation
        if (string.IsNullOrEmpty(text))
            return Vector2.Zero;
        
        int maxWidth = 0;
        int currentWidth = 0;
        int lines = 1;
        
        foreach (char c in text)
        {
            if (c == '\n')
            {
                lines++;
                maxWidth = Math.Max(maxWidth, currentWidth);
                currentWidth = 0;
            }
            else
            {
                currentWidth += 8; // Approximate character width
            }
        }
        
        maxWidth = Math.Max(maxWidth, currentWidth);
        
        return new Vector2(maxWidth, lines * LineSpacing);
    }
}
