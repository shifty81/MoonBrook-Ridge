using System;

namespace MoonBrookEngine.Audio;

/// <summary>
/// Music player for background music playback
/// Supports playing, pausing, stopping, and volume control
/// </summary>
public class MusicPlayer : IDisposable
{
    private readonly AudioEngine _audioEngine;
    private uint _currentSource;
    private uint _currentBuffer;
    private string _currentSongName = string.Empty;
    private bool _isDisposed;
    private float _volume = 1.0f;
    private bool _isRepeating = false;
    private MusicState _state = MusicState.Stopped;
    
    public float Volume
    {
        get => _volume;
        set
        {
            _volume = System.Math.Clamp(value, 0f, 1f);
            if (_currentSource != 0)
                _audioEngine.SetSourceVolume(_currentSource, _volume);
        }
    }
    
    public bool IsRepeating
    {
        get => _isRepeating;
        set
        {
            _isRepeating = value;
            if (_currentSource != 0)
                _audioEngine.SetSourceLooping(_currentSource, _isRepeating);
        }
    }
    
    public MusicState State => _state;
    
    public string CurrentSongName => _currentSongName;
    
    internal MusicPlayer(AudioEngine audioEngine)
    {
        _audioEngine = audioEngine;
    }
    
    /// <summary>
    /// Play a song from a buffer
    /// </summary>
    public void Play(uint buffer, string songName)
    {
        if (_isDisposed) return;
        
        // Stop current song if playing
        Stop();
        
        // Create new source for this song
        _currentSource = _audioEngine.CreateSource();
        if (_currentSource == 0)
        {
            Console.WriteLine($"Failed to create audio source for song: {songName}");
            return;
        }
        
        // Set up source
        _currentBuffer = buffer;
        _currentSongName = songName;
        _audioEngine.SetSourceBuffer(_currentSource, buffer);
        _audioEngine.SetSourceVolume(_currentSource, _volume);
        _audioEngine.SetSourceLooping(_currentSource, _isRepeating);
        
        // Play
        _audioEngine.Play(_currentSource);
        _state = MusicState.Playing;
    }
    
    /// <summary>
    /// Pause the current song
    /// </summary>
    public void Pause()
    {
        if (_isDisposed || _currentSource == 0 || _state != MusicState.Playing) return;
        
        _audioEngine.Pause(_currentSource);
        _state = MusicState.Paused;
    }
    
    /// <summary>
    /// Resume the current song
    /// </summary>
    public void Resume()
    {
        if (_isDisposed || _currentSource == 0 || _state != MusicState.Paused) return;
        
        _audioEngine.Play(_currentSource);
        _state = MusicState.Playing;
    }
    
    /// <summary>
    /// Stop the current song
    /// </summary>
    public void Stop()
    {
        if (_isDisposed) return;
        
        if (_currentSource != 0)
        {
            _audioEngine.Stop(_currentSource);
            _audioEngine.DeleteSource(_currentSource);
            _currentSource = 0;
        }
        
        _currentBuffer = 0;
        _currentSongName = string.Empty;
        _state = MusicState.Stopped;
    }
    
    /// <summary>
    /// Update the music player state (call each frame)
    /// </summary>
    public void Update()
    {
        if (_isDisposed || _currentSource == 0) return;
        
        // Check if song finished playing (and not looping)
        if (_state == MusicState.Playing && !_isRepeating)
        {
            if (!_audioEngine.IsPlaying(_currentSource))
            {
                Stop();
            }
        }
    }
    
    public void Dispose()
    {
        if (_isDisposed) return;
        
        Stop();
        _isDisposed = true;
    }
}

/// <summary>
/// Music playback state
/// </summary>
public enum MusicState
{
    Playing,
    Paused,
    Stopped
}
