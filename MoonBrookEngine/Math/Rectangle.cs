namespace MoonBrookEngine.Math;

/// <summary>
/// Rectangle structure with position and size
/// </summary>
public struct Rectangle
{
    public float X;
    public float Y;
    public float Width;
    public float Height;
    
    public Rectangle(float x, float y, float width, float height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
    
    // Properties
    public float Left => X;
    public float Right => X + Width;
    public float Top => Y;
    public float Bottom => Y + Height;
    
    public Vector2 Location
    {
        get => new(X, Y);
        set { X = value.X; Y = value.Y; }
    }
    
    public Vector2 Center => new(X + Width / 2, Y + Height / 2);
    
    public static readonly Rectangle Empty = new(0, 0, 0, 0);
    
    // Methods
    public bool Contains(float x, float y)
    {
        return X <= x && x < X + Width && Y <= y && y < Y + Height;
    }
    
    public bool Contains(Vector2 point)
    {
        return Contains(point.X, point.Y);
    }
    
    public bool Contains(Rectangle rect)
    {
        return X <= rect.X && rect.X + rect.Width <= X + Width &&
               Y <= rect.Y && rect.Y + rect.Height <= Y + Height;
    }
    
    public bool Intersects(Rectangle other)
    {
        return other.Left < Right && Left < other.Right &&
               other.Top < Bottom && Top < other.Bottom;
    }
    
    public static Rectangle Intersect(Rectangle a, Rectangle b)
    {
        float x1 = MathF.Max(a.X, b.X);
        float y1 = MathF.Max(a.Y, b.Y);
        float x2 = MathF.Min(a.X + a.Width, b.X + b.Width);
        float y2 = MathF.Min(a.Y + a.Height, b.Y + b.Height);
        
        if (x2 >= x1 && y2 >= y1)
        {
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }
        
        return Empty;
    }
    
    public static Rectangle Union(Rectangle a, Rectangle b)
    {
        float x1 = MathF.Min(a.X, b.X);
        float y1 = MathF.Min(a.Y, b.Y);
        float x2 = MathF.Max(a.X + a.Width, b.X + b.Width);
        float y2 = MathF.Max(a.Y + a.Height, b.Y + b.Height);
        
        return new Rectangle(x1, y1, x2 - x1, y2 - y1);
    }
    
    public void Inflate(float horizontalAmount, float verticalAmount)
    {
        X -= horizontalAmount;
        Y -= verticalAmount;
        Width += horizontalAmount * 2;
        Height += verticalAmount * 2;
    }
    
    public void Offset(float offsetX, float offsetY)
    {
        X += offsetX;
        Y += offsetY;
    }
    
    public void Offset(Vector2 offset)
    {
        X += offset.X;
        Y += offset.Y;
    }
    
    // Operators
    public static bool operator ==(Rectangle a, Rectangle b) =>
        a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
    
    public static bool operator !=(Rectangle a, Rectangle b) => !(a == b);
    
    public override bool Equals(object? obj) => obj is Rectangle other && this == other;
    public override int GetHashCode() => HashCode.Combine(X, Y, Width, Height);
    public override string ToString() => $"{{X:{X} Y:{Y} Width:{Width} Height:{Height}}}";
}
