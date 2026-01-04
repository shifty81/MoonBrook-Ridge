using System;

namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible Song class (stub for now)
/// </summary>
public class Song : IDisposable
{
    public string Name { get; }
    public TimeSpan Duration { get; }

    internal Song(string name, TimeSpan duration)
    {
        Name = name;
        Duration = duration;
    }

    public void Dispose()
    {
        // TODO: Implement when engine has music support
    }
}

/// <summary>
/// MonoGame-compatible MediaPlayer static class (stub for now)
/// </summary>
public static class MediaPlayer
{
    private static float _volume = 1.0f;
    private static bool _isRepeating = false;
    private static MediaState _state = MediaState.Stopped;

    public static float Volume
    {
        get => _volume;
        set => _volume = Math.Clamp(value, 0f, 1f);
    }

    public static bool IsRepeating
    {
        get => _isRepeating;
        set => _isRepeating = value;
    }

    public static MediaState State => _state;

    public static void Play(Song song)
    {
        // TODO: Implement when engine has music support
        _state = MediaState.Playing;
        Console.WriteLine($"MediaPlayer.Play stub called for: {song.Name}");
    }

    public static void Pause()
    {
        // TODO: Implement when engine has music support
        _state = MediaState.Paused;
    }

    public static void Resume()
    {
        // TODO: Implement when engine has music support
        _state = MediaState.Playing;
    }

    public static void Stop()
    {
        // TODO: Implement when engine has music support
        _state = MediaState.Stopped;
    }
}

/// <summary>
/// Media state enum
/// </summary>
public enum MediaState
{
    Playing,
    Paused,
    Stopped
}
