using Silk.NET.OpenGL;
using System;

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
            // Note: Changing root directory is limited compared to MonoGame
            // The underlying ResourceManager cannot change root directory after creation
            // This is documented as a known limitation in ENGINE_INTEGRATION_GUIDE.md
            if (value != _resourceManager.RootDirectory)
            {
                throw new NotSupportedException(
                    "Cannot change RootDirectory after ContentManager creation. " +
                    "This is a known limitation - see ENGINE_INTEGRATION_GUIDE.md");
            }
        }
    }
    
    internal ContentManager(GL gl, MoonBrookEngine.Audio.AudioEngine? audioEngine = null, string rootDirectory = "Content")
    {
        _resourceManager = new MoonBrookEngine.Core.ResourceManager(gl, audioEngine, rootDirectory);
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
            // Load font (creates default for now)
            var engineFont = _resourceManager.LoadFont(assetName);
            var font = new SpriteFont(engineFont);
            _loadedAssets[assetName] = font;
            return (T)(object)font;
        }
        else if (typeof(T) == typeof(SoundEffect))
        {
            // Load sound effect
            var engineSound = _resourceManager.LoadSoundEffect(assetName);
            if (engineSound == null)
                throw new System.IO.FileNotFoundException($"Sound effect not found: {assetName}");
            var sound = new SoundEffect(engineSound);
            _loadedAssets[assetName] = sound;
            return (T)(object)sound;
        }
        else if (typeof(T) == typeof(Song))
        {
            // Load song using music buffer
            uint buffer = _resourceManager.LoadMusic(assetName);
            if (buffer == 0)
                throw new System.IO.FileNotFoundException($"Music file not found: {assetName}");
            
            // For now, we don't know the duration without parsing the audio file
            // So we'll just use TimeSpan.Zero as a placeholder
            var song = new Song(assetName, TimeSpan.Zero, buffer);
            _loadedAssets[assetName] = song;
            return (T)(object)song;
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
