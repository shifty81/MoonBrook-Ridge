using Silk.NET.OpenGL;
using MoonBrookEngine.Graphics;
using MoonBrookEngine.Audio;

namespace MoonBrookEngine.Core;

/// <summary>
/// Manages game resources with caching and automatic disposal
/// Similar to MonoGame's ContentManager
/// </summary>
public class ResourceManager : IDisposable
{
    private readonly GL _gl;
    private readonly AudioEngine? _audioEngine;
    private readonly Dictionary<string, Texture2D> _textures;
    private readonly Dictionary<string, BitmapFont> _fonts;
    private readonly Dictionary<string, SoundEffect> _soundEffects;
    private readonly Dictionary<string, uint> _musicBuffers;
    private readonly string _rootDirectory;
    
    public string RootDirectory
    {
        get => _rootDirectory;
    }
    
    public ResourceManager(GL gl, AudioEngine? audioEngine = null, string rootDirectory = "Content")
    {
        _gl = gl;
        _audioEngine = audioEngine;
        _rootDirectory = rootDirectory;
        _textures = new Dictionary<string, Texture2D>();
        _fonts = new Dictionary<string, BitmapFont>();
        _soundEffects = new Dictionary<string, SoundEffect>();
        _musicBuffers = new Dictionary<string, uint>();
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
    /// Load or create a bitmap font
    /// For now, creates a default font since we don't have font atlas support yet
    /// </summary>
    public BitmapFont LoadFont(string assetName, int fontSize = 16)
    {
        // Normalize path separators
        assetName = assetName.Replace('\\', '/');
        
        // Check if already loaded
        if (_fonts.TryGetValue(assetName, out var cached))
        {
            return cached;
        }
        
        // For now, create a default font
        // TODO: Load actual font atlas from .fnt/.png files
        var font = BitmapFont.CreateDefault(_gl, fontSize);
        _fonts[assetName] = font;
        
        return font;
    }
    
    /// <summary>
    /// Unload a specific font
    /// </summary>
    public void UnloadFont(string assetName)
    {
        assetName = assetName.Replace('\\', '/');
        
        if (_fonts.TryGetValue(assetName, out var font))
        {
            font.Dispose();
            _fonts.Remove(assetName);
        }
    }
    
    /// <summary>
    /// Load a sound effect from WAV file
    /// </summary>
    public SoundEffect? LoadSoundEffect(string assetName)
    {
        if (_audioEngine == null)
        {
            Console.WriteLine("Audio engine not available - cannot load sound effects");
            return null;
        }
        
        // Normalize path separators
        assetName = assetName.Replace('\\', '/');
        
        // Check if already loaded
        if (_soundEffects.TryGetValue(assetName, out var cached))
        {
            return cached;
        }
        
        // Build full path
        string fullPath = Path.Combine(_rootDirectory, assetName);
        
        // Try common extensions if no extension specified
        if (!Path.HasExtension(fullPath))
        {
            string[] extensions = { ".wav" };
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
                Console.WriteLine($"Could not find sound effect: {assetName}");
                return null;
            }
            
            fullPath = foundPath;
        }
        
        // Load WAV file
        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"Sound file not found: {fullPath}");
            return null;
        }
        
        byte[] wavData = File.ReadAllBytes(fullPath);
        var soundEffect = SoundEffect.FromWavData(_audioEngine, wavData, assetName);
        
        if (soundEffect != null)
        {
            _soundEffects[assetName] = soundEffect;
        }
        
        return soundEffect;
    }
    
    /// <summary>
    /// Unload a specific sound effect
    /// </summary>
    public void UnloadSoundEffect(string assetName)
    {
        assetName = assetName.Replace('\\', '/');
        
        if (_soundEffects.TryGetValue(assetName, out var soundEffect))
        {
            soundEffect.Dispose();
            _soundEffects.Remove(assetName);
        }
    }
    
    /// <summary>
    /// Load music from WAV file and return buffer ID
    /// </summary>
    public uint LoadMusic(string assetName)
    {
        if (_audioEngine == null)
        {
            Console.WriteLine("Audio engine not available - cannot load music");
            return 0;
        }
        
        // Normalize path separators
        assetName = assetName.Replace('\\', '/');
        
        // Check if already loaded
        if (_musicBuffers.TryGetValue(assetName, out var cached))
        {
            return cached;
        }
        
        // Build full path
        string fullPath = Path.Combine(_rootDirectory, assetName);
        
        // Try common extensions if no extension specified
        if (!Path.HasExtension(fullPath))
        {
            string[] extensions = { ".wav", ".ogg", ".mp3" };
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
                Console.WriteLine($"Could not find music file: {assetName}");
                return 0;
            }
            
            fullPath = foundPath;
        }
        
        // Load music file
        if (!File.Exists(fullPath))
        {
            Console.WriteLine($"Music file not found: {fullPath}");
            return 0;
        }
        
        byte[] audioData = File.ReadAllBytes(fullPath);
        
        // Parse WAV header (simplified - assumes standard WAV format)
        // WAV format: RIFF header (12 bytes) + fmt chunk (24 bytes) + data chunk (8+ bytes)
        if (audioData.Length < 44)
        {
            Console.WriteLine($"Invalid WAV file (too small): {fullPath}");
            return 0;
        }
        
        // Check RIFF header
        if (audioData[0] != 'R' || audioData[1] != 'I' || audioData[2] != 'F' || audioData[3] != 'F')
        {
            Console.WriteLine($"Not a WAV file: {fullPath}");
            return 0;
        }
        
        // Parse WAV format
        int channels = BitConverter.ToInt16(audioData, 22);
        int sampleRate = BitConverter.ToInt32(audioData, 24);
        int bitsPerSample = BitConverter.ToInt16(audioData, 34);
        
        // Find data chunk
        int dataOffset = 36;
        while (dataOffset < audioData.Length - 8)
        {
            if (audioData[dataOffset] == 'd' && audioData[dataOffset + 1] == 'a' &&
                audioData[dataOffset + 2] == 't' && audioData[dataOffset + 3] == 'a')
            {
                break;
            }
            dataOffset++;
        }
        
        if (dataOffset >= audioData.Length - 8)
        {
            Console.WriteLine($"Could not find data chunk in WAV file: {fullPath}");
            return 0;
        }
        
        int dataSize = BitConverter.ToInt32(audioData, dataOffset + 4);
        byte[] pcmData = new byte[dataSize];
        Array.Copy(audioData, dataOffset + 8, pcmData, 0, dataSize);
        
        // Create buffer
        uint buffer = _audioEngine.CreateBuffer(pcmData, channels, sampleRate, bitsPerSample);
        
        if (buffer != 0)
        {
            _musicBuffers[assetName] = buffer;
        }
        
        return buffer;
    }
    
    /// <summary>
    /// Unload a specific music buffer
    /// </summary>
    public void UnloadMusic(string assetName)
    {
        if (_audioEngine == null) return;
        
        assetName = assetName.Replace('\\', '/');
        
        if (_musicBuffers.TryGetValue(assetName, out var buffer))
        {
            _audioEngine.DeleteBuffer(buffer);
            _musicBuffers.Remove(assetName);
        }
    }
    
    /// <summary>
    /// Get number of loaded textures
    /// </summary>
    public int LoadedTextureCount => _textures.Count;
    
    /// <summary>
    /// Get number of loaded fonts
    /// </summary>
    public int LoadedFontCount => _fonts.Count;
    
    /// <summary>
    /// Get number of loaded sound effects
    /// </summary>
    public int LoadedSoundEffectCount => _soundEffects.Count;
    
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
        
        foreach (var font in _fonts.Values)
        {
            font.Dispose();
        }
        _fonts.Clear();
        
        foreach (var soundEffect in _soundEffects.Values)
        {
            soundEffect.Dispose();
        }
        _soundEffects.Clear();
        
        if (_audioEngine != null)
        {
            foreach (var buffer in _musicBuffers.Values)
            {
                _audioEngine.DeleteBuffer(buffer);
            }
        }
        _musicBuffers.Clear();
    }
    
    public void Dispose()
    {
        UnloadAll();
    }
}
