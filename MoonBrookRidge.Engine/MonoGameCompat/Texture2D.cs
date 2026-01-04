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
    
    private Color[]? _colorData;
    private GraphicsDevice? _graphicsDevice;
    
    public Texture2D(MoonBrookEngine.Graphics.Texture2D engineTexture)
    {
        InternalTexture = engineTexture;
    }
    
    /// <summary>
    /// Creates a new Texture2D with the specified width and height
    /// </summary>
    public Texture2D(GraphicsDevice graphicsDevice, int width, int height)
    {
        _graphicsDevice = graphicsDevice;
        
        // Create a blank texture with the specified dimensions
        byte[] data = new byte[width * height * 4];
        for (int i = 0; i < width * height; i++)
        {
            data[i * 4 + 0] = 255; // R
            data[i * 4 + 1] = 255; // G
            data[i * 4 + 2] = 255; // B
            data[i * 4 + 3] = 255; // A - White by default
        }
        
        var gl = graphicsDevice.GetInternalGL();
        InternalTexture = new MoonBrookEngine.Graphics.Texture2D(gl, data, width, height);
        _colorData = new Color[width * height];
        for (int i = 0; i < _colorData.Length; i++)
        {
            _colorData[i] = new Color(255, 255, 255, 255);
        }
    }
    
    /// <summary>
    /// Sets the texture data
    /// </summary>
    public void SetData<T>(T[] data) where T : struct
    {
        if (data is Color[] colorData)
        {
            if (_graphicsDevice == null)
            {
                throw new InvalidOperationException("Cannot SetData on a texture that was not created with GraphicsDevice");
            }
            
            // Convert Color[] to byte array
            byte[] byteData = new byte[colorData.Length * 4];
            for (int i = 0; i < colorData.Length; i++)
            {
                byteData[i * 4 + 0] = colorData[i].R;
                byteData[i * 4 + 1] = colorData[i].G;
                byteData[i * 4 + 2] = colorData[i].B;
                byteData[i * 4 + 3] = colorData[i].A;
            }
            
            // Store the data for later retrieval
            _colorData = new Color[colorData.Length];
            Array.Copy(colorData, _colorData, colorData.Length);
            
            // Recreate the texture with new data
            var gl = _graphicsDevice.GetInternalGL();
            InternalTexture?.Dispose();
            InternalTexture = new MoonBrookEngine.Graphics.Texture2D(gl, byteData, Width, Height);
        }
        else
        {
            throw new NotSupportedException($"SetData only supports Color[] arrays. Got {typeof(T).Name}[]");
        }
    }
    
    /// <summary>
    /// Gets the texture data
    /// </summary>
    public void GetData<T>(T[] data) where T : struct
    {
        if (data is Color[] colorData && _colorData != null)
        {
            // Return the stored color data
            Array.Copy(_colorData, colorData, Math.Min(_colorData.Length, colorData.Length));
        }
        else
        {
            throw new NotSupportedException($"GetData only supports Color[] arrays. Got {typeof(T).Name}[]");
        }
    }
    
    public void Dispose()
    {
        InternalTexture?.Dispose();
    }
}

