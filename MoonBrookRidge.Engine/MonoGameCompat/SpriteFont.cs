namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible SpriteFont stub for MoonBrookEngine
/// TODO: Implement actual font rendering
/// </summary>
public class SpriteFont
{
    // Stub properties
    public float LineSpacing { get; set; } = 16f;
    public float Spacing { get; set; } = 0f;
    public char? DefaultCharacter { get; set; } = '?';
    
    internal SpriteFont()
    {
    }
    
    /// <summary>
    /// Measure the size of a string when rendered with this font
    /// </summary>
    public Vector2 MeasureString(string text)
    {
        // Stub implementation - returns approximate size
        // TODO: Implement actual text measurement
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
