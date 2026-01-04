using System.Numerics;

namespace MoonBrookEngine.Math;

/// <summary>
/// 2D vector with X and Y components - wrapper around System.Numerics.Vector2
/// Provides MonoGame-compatible API
/// </summary>
public struct Vector2
{
    public float X;
    public float Y;
    
    public Vector2(float x, float y)
    {
        X = x;
        Y = y;
    }
    
    public Vector2(float value)
    {
        X = value;
        Y = value;
    }
    
    // Static constants
    public static readonly Vector2 Zero = new(0, 0);
    public static readonly Vector2 One = new(1, 1);
    public static readonly Vector2 UnitX = new(1, 0);
    public static readonly Vector2 UnitY = new(0, 1);
    
    // Properties
    public float Length() => MathF.Sqrt(X * X + Y * Y);
    public float LengthSquared() => X * X + Y * Y;
    
    // Methods
    public void Normalize()
    {
        float length = Length();
        if (length > 0)
        {
            X /= length;
            Y /= length;
        }
    }
    
    public static Vector2 Normalize(Vector2 value)
    {
        float length = value.Length();
        return length > 0 ? new Vector2(value.X / length, value.Y / length) : Zero;
    }
    
    public static float Distance(Vector2 a, Vector2 b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }
    
    public static float Dot(Vector2 a, Vector2 b)
    {
        return a.X * b.X + a.Y * b.Y;
    }
    
    public static Vector2 Lerp(Vector2 a, Vector2 b, float t)
    {
        return new Vector2(
            a.X + (b.X - a.X) * t,
            a.Y + (b.Y - a.Y) * t
        );
    }
    
    // Operators
    public static Vector2 operator +(Vector2 a, Vector2 b) => new(a.X + b.X, a.Y + b.Y);
    public static Vector2 operator -(Vector2 a, Vector2 b) => new(a.X - b.X, a.Y - b.Y);
    public static Vector2 operator *(Vector2 a, float scalar) => new(a.X * scalar, a.Y * scalar);
    public static Vector2 operator *(float scalar, Vector2 a) => new(a.X * scalar, a.Y * scalar);
    public static Vector2 operator /(Vector2 a, float scalar) => new(a.X / scalar, a.Y / scalar);
    public static Vector2 operator -(Vector2 a) => new(-a.X, -a.Y);
    
    public static bool operator ==(Vector2 a, Vector2 b) => a.X == b.X && a.Y == b.Y;
    public static bool operator !=(Vector2 a, Vector2 b) => !(a == b);
    
    // Conversion to/from System.Numerics.Vector2
    public static implicit operator System.Numerics.Vector2(Vector2 v) => new(v.X, v.Y);
    public static implicit operator Vector2(System.Numerics.Vector2 v) => new(v.X, v.Y);
    
    public override bool Equals(object? obj) => obj is Vector2 other && this == other;
    public override int GetHashCode() => HashCode.Combine(X, Y);
    public override string ToString() => $"({X}, {Y})";
}
