namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible GraphicsDevice stub for MoonBrookEngine
/// Most rendering is handled by SpriteBatch, this provides compatibility
/// </summary>
public class GraphicsDevice
{
    private Silk.NET.OpenGL.GL _gl;
    
    public int PreferredBackBufferWidth { get; internal set; }
    public int PreferredBackBufferHeight { get; internal set; }
    
    internal GraphicsDevice(Silk.NET.OpenGL.GL gl, int width, int height)
    {
        _gl = gl;
        PreferredBackBufferWidth = width;
        PreferredBackBufferHeight = height;
    }
    
    /// <summary>
    /// Clear the screen with a color
    /// </summary>
    public void Clear(Color color)
    {
        _gl.ClearColor(color.R / 255f, color.G / 255f, color.B / 255f, color.A / 255f);
        _gl.Clear((uint)(Silk.NET.OpenGL.ClearBufferMask.ColorBufferBit | 
                         Silk.NET.OpenGL.ClearBufferMask.DepthBufferBit));
    }
    
    /// <summary>
    /// Get the current viewport
    /// </summary>
    public Viewport Viewport => new Viewport(0, 0, PreferredBackBufferWidth, PreferredBackBufferHeight);
}

/// <summary>
/// MonoGame-compatible Viewport struct
/// </summary>
public struct Viewport
{
    public int X;
    public int Y;
    public int Width;
    public int Height;
    
    public Viewport(int x, int y, int width, int height)
    {
        X = x;
        Y = y;
        Width = width;
        Height = height;
    }
    
    public Rectangle Bounds => new Rectangle(X, Y, Width, Height);
}
