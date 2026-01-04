using System.Numerics;

namespace MoonBrookEngine.Math;

/// <summary>
/// RGBA color (0-255 per channel)
/// </summary>
public struct Color
{
    public byte R;
    public byte G;
    public byte B;
    public byte A;
    
    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    
    public Color(int r, int g, int b, int a = 255)
    {
        R = (byte)System.Math.Clamp(r, 0, 255);
        G = (byte)System.Math.Clamp(g, 0, 255);
        B = (byte)System.Math.Clamp(b, 0, 255);
        A = (byte)System.Math.Clamp(a, 0, 255);
    }
    
    /// <summary>
    /// Create color from normalized float values (0.0 - 1.0)
    /// </summary>
    public static Color FromNormalized(float r, float g, float b, float a = 1.0f)
    {
        return new Color(
            (byte)(r * 255),
            (byte)(g * 255),
            (byte)(b * 255),
            (byte)(a * 255)
        );
    }
    
    /// <summary>
    /// Convert to normalized Vector4 for shader use
    /// </summary>
    public Vector4 ToVector4()
    {
        return new Vector4(R / 255f, G / 255f, B / 255f, A / 255f);
    }
    
    /// <summary>
    /// Multiply color by scalar (for tinting)
    /// </summary>
    public static Color operator *(Color c, float scalar)
    {
        return new Color(
            (byte)(c.R * scalar),
            (byte)(c.G * scalar),
            (byte)(c.B * scalar),
            c.A
        );
    }
    
    // Common colors
    public static readonly Color White = new Color(255, 255, 255, 255);
    public static readonly Color Black = new Color(0, 0, 0, 255);
    public static readonly Color Red = new Color(255, 0, 0, 255);
    public static readonly Color Green = new Color(0, 255, 0, 255);
    public static readonly Color Blue = new Color(0, 0, 255, 255);
    public static readonly Color Yellow = new Color(255, 255, 0, 255);
    public static readonly Color Cyan = new Color(0, 255, 255, 255);
    public static readonly Color Magenta = new Color(255, 0, 255, 255);
    public static readonly Color Transparent = new Color(0, 0, 0, 0);
    
    public override string ToString()
    {
        return $"Color({R}, {G}, {B}, {A})";
    }
}
