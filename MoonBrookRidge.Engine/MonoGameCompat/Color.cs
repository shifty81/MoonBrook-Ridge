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
    
    public Color(byte r, byte g, byte b, byte a = 255)
    {
        R = r;
        G = g;
        B = b;
        A = a;
    }
    
    public Color(int r, int g, int b, int a = 255)
    {
        R = (byte)r;
        G = (byte)g;
        B = (byte)b;
        A = (byte)a;
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
