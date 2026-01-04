using Silk.NET.OpenGL;

namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible ContentManager wrapper for MoonBrookEngine
/// </summary>
public class ContentManager : IDisposable
{
    private MoonBrookEngine.Core.ResourceManager _resourceManager;
    private Dictionary<string, object> _loadedAssets;
    
    public string RootDirectory
    {
        get => _resourceManager.RootDirectory;
        set
        {
            // Note: Cannot change root directory after creation
            // This is a limitation compared to MonoGame
            if (value != _resourceManager.RootDirectory)
            {
                throw new NotSupportedException("Cannot change RootDirectory after ContentManager creation");
            }
        }
    }
    
    internal ContentManager(GL gl, string rootDirectory = "Content")
    {
        _resourceManager = new MoonBrookEngine.Core.ResourceManager(gl, rootDirectory);
        _loadedAssets = new Dictionary<string, object>();
    }
    
    /// <summary>
    /// Load an asset of the specified type
    /// </summary>
    public T Load<T>(string assetName)
    {
        // Check if already loaded
        if (_loadedAssets.TryGetValue(assetName, out var cached))
        {
            if (cached is T typedAsset)
                return typedAsset;
        }
        
        // Load based on type
        if (typeof(T) == typeof(Texture2D))
        {
            var engineTexture = _resourceManager.LoadTexture(assetName);
            var texture = new Texture2D(engineTexture);
            _loadedAssets[assetName] = texture;
            return (T)(object)texture;
        }
        else if (typeof(T) == typeof(SpriteFont))
        {
            // TODO: Implement font loading
            throw new NotImplementedException("Font loading not yet implemented");
        }
        
        throw new NotSupportedException($"Asset type {typeof(T).Name} is not supported");
    }
    
    /// <summary>
    /// Unload all loaded assets
    /// </summary>
    public void Unload()
    {
        foreach (var asset in _loadedAssets.Values)
        {
            if (asset is IDisposable disposable)
                disposable.Dispose();
        }
        
        _loadedAssets.Clear();
        _resourceManager.UnloadAll();
    }
    
    public void Dispose()
    {
        Unload();
        _resourceManager.Dispose();
    }
}
