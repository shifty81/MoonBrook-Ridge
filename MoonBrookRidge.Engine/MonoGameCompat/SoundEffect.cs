using System;

namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible SoundEffect wrapper
/// </summary>
public class SoundEffect : IDisposable
{
    private readonly MoonBrookEngine.Audio.SoundEffect? _engineSoundEffect;

    public string Name { get; }
    
    internal SoundEffect(MoonBrookEngine.Audio.SoundEffect engineSoundEffect)
    {
        _engineSoundEffect = engineSoundEffect;
        Name = engineSoundEffect.Name;
    }

    /// <summary>
    /// Play the sound effect once
    /// </summary>
    public bool Play()
    {
        return Play(1.0f, 0.0f, 0.0f);
    }

    /// <summary>
    /// Play the sound effect with volume, pitch, and pan
    /// </summary>
    public bool Play(float volume, float pitch, float pan)
    {
        if (_engineSoundEffect == null)
            return false;

        var instance = _engineSoundEffect.CreateInstance();
        if (instance == null)
            return false;

        instance.Volume = volume;
        instance.Pitch = pitch;
        instance.Pan = pan;
        instance.Play();
        
        // Note: Instance will be cleaned up automatically after playing
        return true;
    }

    /// <summary>
    /// Create a sound effect instance for more control
    /// </summary>
    public SoundEffectInstance CreateInstance()
    {
        if (_engineSoundEffect == null)
            throw new InvalidOperationException("Engine sound effect is null");

        var engineInstance = _engineSoundEffect.CreateInstance();
        if (engineInstance == null)
            throw new InvalidOperationException("Failed to create sound effect instance");

        return new SoundEffectInstance(engineInstance);
    }

    public void Dispose()
    {
        _engineSoundEffect?.Dispose();
    }
}

/// <summary>
/// MonoGame-compatible SoundEffectInstance wrapper
/// </summary>
public class SoundEffectInstance : IDisposable
{
    private readonly MoonBrookEngine.Audio.SoundEffectInstance _engineInstance;

    public float Volume
    {
        get => _engineInstance.Volume;
        set => _engineInstance.Volume = value;
    }

    public float Pitch
    {
        get => _engineInstance.Pitch;
        set => _engineInstance.Pitch = value;
    }

    public float Pan
    {
        get => _engineInstance.Pan;
        set => _engineInstance.Pan = value;
    }

    public bool IsLooped
    {
        get => _engineInstance.IsLooped;
        set => _engineInstance.IsLooped = value;
    }

    public SoundState State
    {
        get
        {
            if (_engineInstance.IsPlaying)
                return SoundState.Playing;
            // Note: Engine doesn't track paused state separately, so we assume stopped
            else
                return SoundState.Stopped;
        }
    }

    internal SoundEffectInstance(MoonBrookEngine.Audio.SoundEffectInstance engineInstance)
    {
        _engineInstance = engineInstance;
    }

    public void Play()
    {
        _engineInstance.Play();
    }

    public void Pause()
    {
        _engineInstance.Pause();
    }

    public void Resume()
    {
        _engineInstance.Resume();
    }

    public void Stop()
    {
        _engineInstance.Stop();
    }

    public void Stop(bool immediate)
    {
        _engineInstance.Stop();
    }

    public void Dispose()
    {
        _engineInstance.Dispose();
    }
}

/// <summary>
/// Sound state enum
/// </summary>
public enum SoundState
{
    Playing,
    Paused,
    Stopped
}
