namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible Point struct
/// </summary>
public struct Point
{
    public int X;
    public int Y;

    public Point(int x, int y)
    {
        X = x;
        Y = y;
    }

    public static Point Zero => new Point(0, 0);

    public override bool Equals(object? obj)
    {
        if (obj is Point other)
            return X == other.X && Y == other.Y;
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y);
    }

    public static bool operator ==(Point a, Point b)
    {
        return a.X == b.X && a.Y == b.Y;
    }

    public static bool operator !=(Point a, Point b)
    {
        return !(a == b);
    }

    public static Point operator +(Point a, Point b)
    {
        return new Point(a.X + b.X, a.Y + b.Y);
    }

    public static Point operator -(Point a, Point b)
    {
        return new Point(a.X - b.X, a.Y - b.Y);
    }

    public static Point operator *(Point a, int scalar)
    {
        return new Point(a.X * scalar, a.Y * scalar);
    }

    public static Point operator /(Point a, int scalar)
    {
        return new Point(a.X / scalar, a.Y / scalar);
    }

    public override string ToString()
    {
        return $"{{X:{X} Y:{Y}}}";
    }

    public Vector2 ToVector2()
    {
        return new Vector2(X, Y);
    }
}
