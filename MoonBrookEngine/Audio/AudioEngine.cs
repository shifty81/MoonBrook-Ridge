using Silk.NET.OpenAL;
using System;

namespace MoonBrookEngine.Audio;

/// <summary>
/// Audio engine for playing sounds and music using OpenAL
/// </summary>
public class AudioEngine : IDisposable
{
    private readonly AL _al;
    private ALContext _alc;
    private unsafe Device* _device;
    private unsafe Context* _context;
    private bool _isInitialized;
    
    public bool IsInitialized => _isInitialized;
    
    public AudioEngine()
    {
        _al = AL.GetApi();
        _alc = ALContext.GetApi();
    }
    
    /// <summary>
    /// Initialize the audio system
    /// </summary>
    public unsafe bool Initialize(string? deviceName = null)
    {
        try
        {
            // Open the default audio device
            _device = _alc.OpenDevice(deviceName);
            if (_device == null)
            {
                Console.WriteLine("Failed to open audio device");
                return false;
            }
            
            // Create audio context
            _context = _alc.CreateContext(_device, null);
            if (_context == null)
            {
                Console.WriteLine("Failed to create audio context");
                _alc.CloseDevice(_device);
                return false;
            }
            
            // Make context current
            if (!_alc.MakeContextCurrent(_context))
            {
                Console.WriteLine("Failed to make audio context current");
                _alc.DestroyContext(_context);
                _alc.CloseDevice(_device);
                return false;
            }
            
            _isInitialized = true;
            Console.WriteLine("✅ Audio engine initialized successfully");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"❌ Failed to initialize audio engine: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Set listener position (camera position in 3D space)
    /// </summary>
    public void SetListenerPosition(float x, float y, float z)
    {
        if (!_isInitialized) return;
        _al.SetListenerProperty(ListenerVector3.Position, x, y, z);
    }
    
    /// <summary>
    /// Set listener velocity
    /// </summary>
    public void SetListenerVelocity(float x, float y, float z)
    {
        if (!_isInitialized) return;
        _al.SetListenerProperty(ListenerVector3.Velocity, x, y, z);
    }
    
    /// <summary>
    /// Set listener orientation (forward and up vectors)
    /// </summary>
    public unsafe void SetListenerOrientation(float forwardX, float forwardY, float forwardZ, 
                                              float upX, float upY, float upZ)
    {
        if (!_isInitialized) return;
        
        float[] orientation = new float[6] 
        { 
            forwardX, forwardY, forwardZ,  // forward
            upX, upY, upZ                    // up
        };
        
        fixed (float* ptr = orientation)
        {
            _al.SetListenerProperty(ListenerFloatArray.Orientation, ptr);
        }
    }
    
    /// <summary>
    /// Set master volume (0.0 to 1.0)
    /// </summary>
    public void SetMasterVolume(float volume)
    {
        if (!_isInitialized) return;
        _al.SetListenerProperty(ListenerFloat.Gain, System.Math.Clamp(volume, 0f, 1f));
    }
    
    /// <summary>
    /// Create an audio buffer from PCM data
    /// </summary>
    public unsafe uint CreateBuffer(byte[] data, int channels, int sampleRate, int bitsPerSample)
    {
        if (!_isInitialized) return 0;
        
        // Determine format
        BufferFormat format;
        if (channels == 1 && bitsPerSample == 8)
            format = BufferFormat.Mono8;
        else if (channels == 1 && bitsPerSample == 16)
            format = BufferFormat.Mono16;
        else if (channels == 2 && bitsPerSample == 8)
            format = BufferFormat.Stereo8;
        else if (channels == 2 && bitsPerSample == 16)
            format = BufferFormat.Stereo16;
        else
        {
            Console.WriteLine($"Unsupported audio format: {channels} channels, {bitsPerSample} bits");
            return 0;
        }
        
        // Generate buffer
        uint buffer = _al.GenBuffer();
        
        // Upload data
        fixed (byte* ptr = data)
        {
            _al.BufferData(buffer, format, ptr, data.Length, sampleRate);
        }
        
        return buffer;
    }
    
    /// <summary>
    /// Delete an audio buffer
    /// </summary>
    public void DeleteBuffer(uint buffer)
    {
        if (!_isInitialized || buffer == 0) return;
        _al.DeleteBuffer(buffer);
    }
    
    /// <summary>
    /// Create an audio source
    /// </summary>
    public uint CreateSource()
    {
        if (!_isInitialized) return 0;
        return _al.GenSource();
    }
    
    /// <summary>
    /// Delete an audio source
    /// </summary>
    public void DeleteSource(uint source)
    {
        if (!_isInitialized || source == 0) return;
        _al.DeleteSource(source);
    }
    
    /// <summary>
    /// Play audio from a source
    /// </summary>
    public void Play(uint source)
    {
        if (!_isInitialized || source == 0) return;
        _al.SourcePlay(source);
    }
    
    /// <summary>
    /// Pause audio source
    /// </summary>
    public void Pause(uint source)
    {
        if (!_isInitialized || source == 0) return;
        _al.SourcePause(source);
    }
    
    /// <summary>
    /// Stop audio source
    /// </summary>
    public void Stop(uint source)
    {
        if (!_isInitialized || source == 0) return;
        _al.SourceStop(source);
    }
    
    /// <summary>
    /// Rewind audio source to beginning
    /// </summary>
    public void Rewind(uint source)
    {
        if (!_isInitialized || source == 0) return;
        _al.SourceRewind(source);
    }
    
    /// <summary>
    /// Check if source is playing
    /// </summary>
    public bool IsPlaying(uint source)
    {
        if (!_isInitialized || source == 0) return false;
        _al.GetSourceProperty(source, GetSourceInteger.SourceState, out int state);
        return state == (int)SourceState.Playing;
    }
    
    /// <summary>
    /// Set source buffer
    /// </summary>
    public void SetSourceBuffer(uint source, uint buffer)
    {
        if (!_isInitialized || source == 0) return;
        _al.SetSourceProperty(source, SourceInteger.Buffer, (int)buffer);
    }
    
    /// <summary>
    /// Set source volume (0.0 to 1.0)
    /// </summary>
    public void SetSourceVolume(uint source, float volume)
    {
        if (!_isInitialized || source == 0) return;
        _al.SetSourceProperty(source, SourceFloat.Gain, System.Math.Clamp(volume, 0f, 1f));
    }
    
    /// <summary>
    /// Set source pitch (1.0 = normal)
    /// </summary>
    public void SetSourcePitch(uint source, float pitch)
    {
        if (!_isInitialized || source == 0) return;
        _al.SetSourceProperty(source, SourceFloat.Pitch, pitch);
    }
    
    /// <summary>
    /// Set source position in 3D space
    /// </summary>
    public void SetSourcePosition(uint source, float x, float y, float z)
    {
        if (!_isInitialized || source == 0) return;
        _al.SetSourceProperty(source, SourceVector3.Position, x, y, z);
    }
    
    /// <summary>
    /// Set source looping
    /// </summary>
    public void SetSourceLooping(uint source, bool looping)
    {
        if (!_isInitialized || source == 0) return;
        _al.SetSourceProperty(source, SourceBoolean.Looping, looping);
    }
    
    public unsafe void Dispose()
    {
        if (!_isInitialized) return;
        
        if (_context != null)
        {
            _alc.MakeContextCurrent(null);
            _alc.DestroyContext(_context);
            _context = null;
        }
        
        if (_device != null)
        {
            _alc.CloseDevice(_device);
            _device = null;
        }
        
        _isInitialized = false;
        Console.WriteLine("Audio engine disposed");
    }
}
