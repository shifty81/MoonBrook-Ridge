using Silk.NET.OpenGL;
using StbImageSharp;

namespace MoonBrookEngine.Graphics;

/// <summary>
/// 2D texture for sprite rendering
/// </summary>
public class Texture2D : IDisposable
{
    private GL _gl;
    private uint _handle;
    private bool _disposed;
    
    public int Width { get; private set; }
    public int Height { get; private set; }
    public uint Handle => _handle;
    
    /// <summary>
    /// Load texture from file
    /// </summary>
    public Texture2D(GL gl, string path)
    {
        _gl = gl;
        
        if (!File.Exists(path))
        {
            throw new FileNotFoundException($"Texture file not found: {path}");
        }
        
        // Load image using StbImage
        StbImage.stbi_set_flip_vertically_on_load(1);
        using var stream = File.OpenRead(path);
        ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        
        Width = image.Width;
        Height = image.Height;
        
        CreateTexture(image.Data, image.Width, image.Height);
    }
    
    /// <summary>
    /// Create texture from raw data
    /// </summary>
    public Texture2D(GL gl, byte[] data, int width, int height)
    {
        _gl = gl;
        Width = width;
        Height = height;
        
        CreateTexture(data, width, height);
    }
    
    /// <summary>
    /// Create a solid color texture (useful for debug rendering)
    /// </summary>
    public static Texture2D CreateSolidColor(GL gl, int width, int height, byte r, byte g, byte b, byte a)
    {
        byte[] data = new byte[width * height * 4];
        for (int i = 0; i < width * height; i++)
        {
            data[i * 4 + 0] = r;
            data[i * 4 + 1] = g;
            data[i * 4 + 2] = b;
            data[i * 4 + 3] = a;
        }
        
        return new Texture2D(gl, data, width, height);
    }
    
    private unsafe void CreateTexture(byte[] data, int width, int height)
    {
        // Create OpenGL texture
        _handle = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
        
        fixed (byte* ptr = data)
        {
            _gl.TexImage2D(
                TextureTarget.Texture2D,
                0,
                InternalFormat.Rgba,
                (uint)width,
                (uint)height,
                0,
                PixelFormat.Rgba,
                PixelType.UnsignedByte,
                ptr
            );
        }
        
        // Set texture parameters
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.ClampToEdge);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Nearest);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Nearest);
        
        _gl.BindTexture(TextureTarget.Texture2D, 0);
    }
    
    public void Bind(uint slot = 0)
    {
        _gl.ActiveTexture(TextureUnit.Texture0 + (int)slot);
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }
    
    public void Unbind()
    {
        _gl.BindTexture(TextureTarget.Texture2D, 0);
    }
    
    public void Dispose()
    {
        if (!_disposed)
        {
            _gl.DeleteTexture(_handle);
            _disposed = true;
        }
    }
}
