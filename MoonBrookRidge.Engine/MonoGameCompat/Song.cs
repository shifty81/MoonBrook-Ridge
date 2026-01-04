using System;

namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible Song class
/// </summary>
public class Song : IDisposable
{
    public string Name { get; }
    public TimeSpan Duration { get; }
    internal uint Buffer { get; }

    internal Song(string name, TimeSpan duration, uint buffer)
    {
        Name = name;
        Duration = duration;
        Buffer = buffer;
    }

    public void Dispose()
    {
        // Buffer is managed by ResourceManager, so no cleanup needed here
    }
}

/// <summary>
/// MonoGame-compatible MediaPlayer static class
/// </summary>
public static class MediaPlayer
{
    private static MoonBrookEngine.Audio.MusicPlayer? _musicPlayer;
    private static Song? _currentSong;

    internal static void Initialize(MoonBrookEngine.Audio.MusicPlayer? musicPlayer)
    {
        _musicPlayer = musicPlayer;
    }

    public static float Volume
    {
        get => _musicPlayer?.Volume ?? 1.0f;
        set
        {
            if (_musicPlayer != null)
                _musicPlayer.Volume = value;
        }
    }

    public static bool IsRepeating
    {
        get => _musicPlayer?.IsRepeating ?? false;
        set
        {
            if (_musicPlayer != null)
                _musicPlayer.IsRepeating = value;
        }
    }

    public static MediaState State
    {
        get
        {
            if (_musicPlayer == null) return MediaState.Stopped;
            
            return _musicPlayer.State switch
            {
                MoonBrookEngine.Audio.MusicState.Playing => MediaState.Playing,
                MoonBrookEngine.Audio.MusicState.Paused => MediaState.Paused,
                _ => MediaState.Stopped
            };
        }
    }

    public static void Play(Song song)
    {
        if (_musicPlayer == null)
        {
            Console.WriteLine($"MediaPlayer not initialized - cannot play: {song.Name}");
            return;
        }

        _currentSong = song;
        _musicPlayer.Play(song.Buffer, song.Name);
    }

    public static void Pause()
    {
        _musicPlayer?.Pause();
    }

    public static void Resume()
    {
        _musicPlayer?.Resume();
    }

    public static void Stop()
    {
        _musicPlayer?.Stop();
        _currentSong = null;
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
