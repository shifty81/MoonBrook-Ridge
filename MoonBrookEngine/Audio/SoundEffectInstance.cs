using System;

namespace MoonBrookEngine.Audio;

/// <summary>
/// Represents a playable instance of a sound effect with control over playback
/// Similar to MonoGame's SoundEffectInstance
/// </summary>
public class SoundEffectInstance : IDisposable
{
    private readonly AudioEngine _audioEngine;
    private readonly uint _buffer;
    private readonly uint _source;
    private bool _isDisposed;
    
    public string Name { get; }
    
    public bool IsLooped
    {
        get => !_isDisposed && _audioEngine.IsPlaying(_source);
        set
        {
            if (!_isDisposed)
                _audioEngine.SetSourceLooping(_source, value);
        }
    }
    
    public float Volume
    {
        get => 1.0f; // TODO: Store and return actual value
        set
        {
            if (!_isDisposed)
                _audioEngine.SetSourceVolume(_source, value);
        }
    }
    
    public float Pitch
    {
        get => 0f; // TODO: Store and return actual value
        set
        {
            if (!_isDisposed)
                _audioEngine.SetSourcePitch(_source, 1.0f + value);
        }
    }
    
    public float Pan
    {
        get => 0f; // TODO: Store and return actual value
        set
        {
            if (!_isDisposed)
                _audioEngine.SetSourcePosition(_source, value, 0, 0);
        }
    }
    
    internal SoundEffectInstance(AudioEngine audioEngine, uint buffer, string name)
    {
        _audioEngine = audioEngine;
        _buffer = buffer;
        Name = name;
        
        // Create a dedicated source for this instance
        _source = audioEngine.CreateSource();
        if (_source != 0)
        {
            audioEngine.SetSourceBuffer(_source, buffer);
        }
    }
    
    /// <summary>
    /// Play or resume the sound
    /// </summary>
    public void Play()
    {
        if (_isDisposed || _source == 0) return;
        _audioEngine.Play(_source);
    }
    
    /// <summary>
    /// Pause the sound
    /// </summary>
    public void Pause()
    {
        if (_isDisposed || _source == 0) return;
        _audioEngine.Pause(_source);
    }
    
    /// <summary>
    /// Stop the sound and reset to beginning
    /// </summary>
    public void Stop()
    {
        if (_isDisposed || _source == 0) return;
        _audioEngine.Stop(_source);
    }
    
    /// <summary>
    /// Stop the sound immediately
    /// </summary>
    public void Stop(bool immediate)
    {
        Stop(); // For now, always immediate
    }
    
    /// <summary>
    /// Resume playback
    /// </summary>
    public void Resume()
    {
        Play();
    }
    
    /// <summary>
    /// Check if currently playing
    /// </summary>
    public bool IsPlaying => !_isDisposed && _audioEngine.IsPlaying(_source);
    
    public void Dispose()
    {
        if (_isDisposed) return;
        
        if (_source != 0)
        {
            _audioEngine.Stop(_source);
            _audioEngine.DeleteSource(_source);
        }
        
        _isDisposed = true;
    }
}
