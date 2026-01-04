using Silk.NET.OpenGL;
using MoonBrookEngine.Graphics;

namespace MoonBrookEngine.Core;

/// <summary>
/// Manages game resources with caching and automatic disposal
/// Similar to MonoGame's ContentManager
/// </summary>
public class ResourceManager : IDisposable
{
    private readonly GL _gl;
    private readonly Dictionary<string, Texture2D> _textures;
    private readonly string _rootDirectory;
    
    public string RootDirectory
    {
        get => _rootDirectory;
    }
    
    public ResourceManager(GL gl, string rootDirectory = "Content")
    {
        _gl = gl;
        _rootDirectory = rootDirectory;
        _textures = new Dictionary<string, Texture2D>();
    }
    
    /// <summary>
    /// Load a texture from file, or return cached version if already loaded
    /// </summary>
    public Texture2D LoadTexture(string assetName)
    {
        // Normalize path separators
        assetName = assetName.Replace('\\', '/');
        
        // Check if already loaded
        if (_textures.TryGetValue(assetName, out var cached))
        {
            return cached;
        }
        
        // Build full path
        string fullPath = Path.Combine(_rootDirectory, assetName);
        
        // Try common extensions if no extension specified
        if (!Path.HasExtension(fullPath))
        {
            string[] extensions = { ".png", ".jpg", ".jpeg", ".bmp" };
            string? foundPath = null;
            
            foreach (var ext in extensions)
            {
                string testPath = fullPath + ext;
                if (File.Exists(testPath))
                {
                    foundPath = testPath;
                    break;
                }
            }
            
            if (foundPath == null)
            {
                throw new FileNotFoundException($"Could not find texture: {assetName}");
            }
            
            fullPath = foundPath;
        }
        
        // Load texture
        var texture = new Texture2D(_gl, fullPath);
        _textures[assetName] = texture;
        
        return texture;
    }
    
    /// <summary>
    /// Unload a specific texture
    /// </summary>
    public void UnloadTexture(string assetName)
    {
        assetName = assetName.Replace('\\', '/');
        
        if (_textures.TryGetValue(assetName, out var texture))
        {
            texture.Dispose();
            _textures.Remove(assetName);
        }
    }
    
    /// <summary>
    /// Unload all resources
    /// </summary>
    public void UnloadAll()
    {
        foreach (var texture in _textures.Values)
        {
            texture.Dispose();
        }
        _textures.Clear();
    }
    
    /// <summary>
    /// Get number of loaded textures
    /// </summary>
    public int LoadedTextureCount => _textures.Count;
    
    public void Dispose()
    {
        UnloadAll();
    }
}
