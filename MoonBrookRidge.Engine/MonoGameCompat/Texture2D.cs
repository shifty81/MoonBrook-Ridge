namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible Texture2D wrapper for MoonBrookEngine
/// </summary>
public class Texture2D : IDisposable
{
    internal MoonBrookEngine.Graphics.Texture2D InternalTexture { get; private set; }
    
    public int Width => InternalTexture.Width;
    public int Height => InternalTexture.Height;
    
    public Rectangle Bounds => new Rectangle(0, 0, Width, Height);
    
    internal Texture2D(MoonBrookEngine.Graphics.Texture2D engineTexture)
    {
        InternalTexture = engineTexture;
    }
    
    public void Dispose()
    {
        InternalTexture?.Dispose();
    }
}
