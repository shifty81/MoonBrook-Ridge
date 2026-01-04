namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible Rectangle struct wrapper for MoonBrookEngine
/// </summary>
public struct Rectangle : IEquatable<Rectangle>
{
    public int X;
    public int Y;
    public int Width;
    public int Height;
    
    public static readonly Rectangle Empty = new Rectangle(0, 0, 0, 0);
    
    public Rectangle(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
    
    public int Left => X;
    public int Right => X + Width;
    public int Top => Y;
    public int Bottom => Y + Height;
    
    public Vector2 Location
    {
        get => new Vector2(X, Y);
        set
        {
            X = (int)value.X;
            Y = (int)value.Y;
        }
    }
    
    public Vector2 Center => new Vector2(X + Width / 2f, Y + Height / 2f);
    
    public bool IsEmpty => Width == 0 && Height == 0;
    
    // Implicit conversions to/from engine Rectangle
    public static implicit operator MoonBrookEngine.Math.Rectangle(Rectangle r)
        => new MoonBrookEngine.Math.Rectangle(r.X, r.Y, r.Width, r.Height);
    
    public static implicit operator Rectangle(MoonBrookEngine.Math.Rectangle r)
        => new Rectangle((int)r.X, (int)r.Y, (int)r.Width, (int)r.Height);
    
    // Operators
    public static bool operator ==(Rectangle a, Rectangle b)
        => a.X == b.X && a.Y == b.Y && a.Width == b.Width && a.Height == b.Height;
    
    public static bool operator !=(Rectangle a, Rectangle b)
        => !(a == b);
    
    // Methods
    public bool Contains(int x, int y)
        => x >= X && x < X + Width && y >= Y && y < Y + Height;
    
    public bool Contains(Vector2 point)
        => Contains((int)point.X, (int)point.Y);
    
    public bool Contains(Rectangle rect)
        => rect.X >= X && rect.Y >= Y &&
           rect.X + rect.Width <= X + Width &&
           rect.Y + rect.Height <= Y + Height;
    
    public bool Intersects(Rectangle rect)
        => rect.X < X + Width && X < rect.X + rect.Width &&
           rect.Y < Y + Height && Y < rect.Y + rect.Height;
    
    public static Rectangle Intersect(Rectangle a, Rectangle b)
    {
        int x1 = Math.Max(a.X, b.X);
        int x2 = Math.Min(a.X + a.Width, b.X + b.Width);
        int y1 = Math.Max(a.Y, b.Y);
        int y2 = Math.Min(a.Y + a.Height, b.Y + b.Height);
        
        if (x2 >= x1 && y2 >= y1)
            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        
        return Empty;
    }
    
    public static Rectangle Union(Rectangle a, Rectangle b)
    {
        int x = Math.Min(a.X, b.X);
        int y = Math.Min(a.Y, b.Y);
        int right = Math.Max(a.Right, b.Right);
        int bottom = Math.Max(a.Bottom, b.Bottom);
        
        return new Rectangle(x, y, right - x, bottom - y);
    }
    
    public void Inflate(int horizontalAmount, int verticalAmount)
    {
        X -= horizontalAmount;
        Y -= verticalAmount;
        Width += horizontalAmount * 2;
        Height += verticalAmount * 2;
    }
    
    public void Offset(int offsetX, int offsetY)
    {
        X += offsetX;
        Y += offsetY;
    }
    
    public void Offset(Vector2 offset)
    {
        X += (int)offset.X;
        Y += (int)offset.Y;
    }
    
    public bool Equals(Rectangle other)
        => X == other.X && Y == other.Y && Width == other.Width && Height == other.Height;
    
    public override bool Equals(object? obj)
        => obj is Rectangle other && Equals(other);
    
    public override int GetHashCode()
        => HashCode.Combine(X, Y, Width, Height);
    
    public override string ToString()
        => $"{{X:{X} Y:{Y} Width:{Width} Height:{Height}}}";
}
