using System.Numerics;

namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible Vector2 wrapper for MoonBrookEngine
/// </summary>
public struct Vector2 : IEquatable<Vector2>
{
    public float X;
    public float Y;
    
    public static readonly Vector2 Zero = new Vector2(0, 0);
    public static readonly Vector2 One = new Vector2(1, 1);
    public static readonly Vector2 UnitX = new Vector2(1, 0);
    public static readonly Vector2 UnitY = new Vector2(0, 1);
    
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
    
    // Implicit conversions to/from engine Vector2
    public static implicit operator MoonBrookEngine.Math.Vector2(Vector2 v)
        => new MoonBrookEngine.Math.Vector2(v.X, v.Y);
    
    public static implicit operator Vector2(MoonBrookEngine.Math.Vector2 v)
        => new Vector2(v.X, v.Y);
    
    // Implicit conversions to/from System.Numerics.Vector2
    public static implicit operator System.Numerics.Vector2(Vector2 v)
        => new System.Numerics.Vector2(v.X, v.Y);
    
    public static implicit operator Vector2(System.Numerics.Vector2 v)
        => new Vector2(v.X, v.Y);
    
    // Operators
    public static Vector2 operator +(Vector2 a, Vector2 b)
        => new Vector2(a.X + b.X, a.Y + b.Y);
    
    public static Vector2 operator -(Vector2 a, Vector2 b)
        => new Vector2(a.X - b.X, a.Y - b.Y);
    
    public static Vector2 operator *(Vector2 v, float scale)
        => new Vector2(v.X * scale, v.Y * scale);
    
    public static Vector2 operator *(float scale, Vector2 v)
        => new Vector2(v.X * scale, v.Y * scale);
    
    public static Vector2 operator /(Vector2 v, float divisor)
        => new Vector2(v.X / divisor, v.Y / divisor);
    
    public static Vector2 operator -(Vector2 v)
        => new Vector2(-v.X, -v.Y);
    
    public static bool operator ==(Vector2 a, Vector2 b)
        => a.X == b.X && a.Y == b.Y;
    
    public static bool operator !=(Vector2 a, Vector2 b)
        => !(a == b);
    
    // Methods
    public float Length() => MathF.Sqrt(X * X + Y * Y);
    public float LengthSquared() => X * X + Y * Y;
    
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
        var result = value;
        result.Normalize();
        return result;
    }
    
    public static float Distance(Vector2 a, Vector2 b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return MathF.Sqrt(dx * dx + dy * dy);
    }
    
    public static float DistanceSquared(Vector2 a, Vector2 b)
    {
        float dx = a.X - b.X;
        float dy = a.Y - b.Y;
        return dx * dx + dy * dy;
    }
    
    public static float Dot(Vector2 a, Vector2 b)
        => a.X * b.X + a.Y * b.Y;
    
    public static Vector2 Lerp(Vector2 a, Vector2 b, float amount)
        => new Vector2(
            a.X + (b.X - a.X) * amount,
            a.Y + (b.Y - a.Y) * amount
        );
    
    public bool Equals(Vector2 other)
        => X == other.X && Y == other.Y;
    
    public override bool Equals(object? obj)
        => obj is Vector2 other && Equals(other);
    
    public override int GetHashCode()
        => HashCode.Combine(X, Y);
    
    public override string ToString()
        => $"{{X:{X} Y:{Y}}}";
}
