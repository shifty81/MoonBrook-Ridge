namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible Color struct wrapper for MoonBrookEngine
/// </summary>
public struct Color : IEquatable<Color>
{
    public byte R;
    public byte G;
    public byte B;
    public byte A;
    
    // Common colors
    public static readonly Color White = new Color(255, 255, 255, 255);
    public static readonly Color Black = new Color(0, 0, 0, 255);
    public static readonly Color Transparent = new Color(0, 0, 0, 0);
    public static readonly Color Red = new Color(255, 0, 0, 255);
    public static readonly Color Green = new Color(0, 255, 0, 255);
    public static readonly Color Blue = new Color(0, 0, 255, 255);
    public static readonly Color Yellow = new Color(255, 255, 0, 255);
    public static readonly Color Cyan = new Color(0, 255, 255, 255);
    public static readonly Color Magenta = new Color(255, 0, 255, 255);
    public static readonly Color CornflowerBlue = new Color(100, 149, 237, 255); // Classic MonoGame default
    
    // Additional colors used by the game
    public static readonly Color Brown = new Color(165, 42, 42, 255);
    public static readonly Color Orange = new Color(255, 165, 0, 255);
    public static readonly Color Gray = new Color(128, 128, 128, 255);
    public static readonly Color DarkGray = new Color(169, 169, 169, 255);
    public static readonly Color LightGray = new Color(211, 211, 211, 255);
    public static readonly Color Purple = new Color(128, 0, 128, 255);
    public static readonly Color DarkGreen = new Color(0, 100, 0, 255);
    public static readonly Color LightGreen = new Color(144, 238, 144, 255);
    public static readonly Color OrangeRed = new Color(255, 69, 0, 255);
    public static readonly Color DarkRed = new Color(139, 0, 0, 255);
    public static readonly Color SandyBrown = new Color(244, 164, 96, 255);
    public static readonly Color LimeGreen = new Color(50, 205, 50, 255);
    public static readonly Color Gold = new Color(255, 215, 0, 255);
    public static readonly Color SkyBlue = new Color(135, 206, 235, 255);
    public static readonly Color LightBlue = new Color(173, 216, 230, 255);
    public static readonly Color Silver = new Color(192, 192, 192, 255);
    public static readonly Color Violet = new Color(238, 130, 238, 255);
    public static readonly Color LightYellow = new Color(255, 255, 224, 255);
    public static readonly Color DarkGoldenrod = new Color(184, 134, 11, 255);
    public static readonly Color DarkOliveGreen = new Color(85, 107, 47, 255);
    public static readonly Color DarkBlue = new Color(0, 0, 139, 255);
    public static readonly Color YellowGreen = new Color(154, 205, 50, 255);
    public static readonly Color LightSeaGreen = new Color(32, 178, 170, 255);
    public static readonly Color Pink = new Color(255, 192, 203, 255);
    public static readonly Color MediumPurple = new Color(147, 112, 219, 255);
    public static readonly Color LightPink = new Color(255, 182, 193, 255);
    public static readonly Color DarkSlateGray = new Color(47, 79, 79, 255);
    public static readonly Color SaddleBrown = new Color(139, 69, 19, 255);
    
    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    
    public Color(int r, int g, int b, int a)
    {
        R = (byte)Math.Clamp(r, 0, 255);
        G = (byte)Math.Clamp(g, 0, 255);
        B = (byte)Math.Clamp(b, 0, 255);
        A = (byte)Math.Clamp(a, 0, 255);
    }
    
    public Color(float r, float g, float b, float a = 1.0f)
    {
        R = (byte)(r * 255);
        G = (byte)(g * 255);
        B = (byte)(b * 255);
        A = (byte)(a * 255);
    }
    
    // Implicit conversions to/from engine Color
    public static implicit operator MoonBrookEngine.Math.Color(Color c)
        => new MoonBrookEngine.Math.Color(c.R, c.G, c.B, c.A);
    
    public static implicit operator Color(MoonBrookEngine.Math.Color c)
        => new Color(c.R, c.G, c.B, c.A);
    
    // Operators
    public static Color operator *(Color color, float scale)
    {
        return new Color(
            (byte)Math.Clamp(color.R * scale, 0, 255),
            (byte)Math.Clamp(color.G * scale, 0, 255),
            (byte)Math.Clamp(color.B * scale, 0, 255),
            color.A
        );
    }
    
    public static bool operator ==(Color a, Color b)
        => a.R == b.R && a.G == b.G && a.B == b.B && a.A == b.A;
    
    public static bool operator !=(Color a, Color b)
        => !(a == b);
    
    // Methods
    public static Color Lerp(Color a, Color b, float amount)
    {
        amount = Math.Clamp(amount, 0, 1);
        return new Color(
            (byte)(a.R + (b.R - a.R) * amount),
            (byte)(a.G + (b.G - a.G) * amount),
            (byte)(a.B + (b.B - a.B) * amount),
            (byte)(a.A + (b.A - a.A) * amount)
        );
    }
    
    public System.Numerics.Vector4 ToVector4()
        => new System.Numerics.Vector4(R / 255f, G / 255f, B / 255f, A / 255f);
    
    public System.Numerics.Vector3 ToVector3()
        => new System.Numerics.Vector3(R / 255f, G / 255f, B / 255f);
    
    public bool Equals(Color other)
        => R == other.R && G == other.G && B == other.B && A == other.A;
    
    public override bool Equals(object? obj)
        => obj is Color other && Equals(other);
    
    public override int GetHashCode()
        => HashCode.Combine(R, G, B, A);
    
    public override string ToString()
        => $"{{R:{R} G:{G} B:{B} A:{A}}}";
}
