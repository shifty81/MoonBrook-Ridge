namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible MouseState
/// </summary>
public struct MouseState
{
    public int X { get; internal set; }
    public int Y { get; internal set; }
    public ButtonState LeftButton { get; internal set; }
    public ButtonState RightButton { get; internal set; }
    public ButtonState MiddleButton { get; internal set; }
    public int ScrollWheelValue { get; internal set; }

    public MouseState()
    {
        X = 0;
        Y = 0;
        LeftButton = ButtonState.Released;
        RightButton = ButtonState.Released;
        MiddleButton = ButtonState.Released;
        ScrollWheelValue = 0;
    }

    internal MouseState(int x, int y, ButtonState left, ButtonState right, ButtonState middle, int scroll)
    {
        X = x;
        Y = y;
        LeftButton = left;
        RightButton = right;
        MiddleButton = middle;
        ScrollWheelValue = scroll;
    }

    public override bool Equals(object? obj)
    {
        if (obj is MouseState other)
        {
            return X == other.X && Y == other.Y &&
                   LeftButton == other.LeftButton &&
                   RightButton == other.RightButton &&
                   MiddleButton == other.MiddleButton &&
                   ScrollWheelValue == other.ScrollWheelValue;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Y, LeftButton, RightButton, MiddleButton, ScrollWheelValue);
    }

    public static bool operator ==(MouseState a, MouseState b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(MouseState a, MouseState b)
    {
        return !a.Equals(b);
    }
}
