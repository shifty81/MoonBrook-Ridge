using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Media;
using System;
using System.Collections.Generic;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Manages all audio playback including music and sound effects
/// </summary>
public class AudioManager
{
    private Dictionary<string, SoundEffect> _soundEffects;
    private Dictionary<string, Song> _music;
    private Dictionary<string, SoundEffectInstance> _loopingSounds;
    
    private string _currentMusicTrack;
    private float _musicVolume;
    private float _sfxVolume;
    private bool _isMusicEnabled;
    private bool _areSfxEnabled;
    
    public float MusicVolume
    {
        get => _musicVolume;
        set
        {
            _musicVolume = Math.Clamp(value, 0f, 1f);
            MediaPlayer.Volume = _musicVolume;
        }
    }
    
    public float SfxVolume
    {
        get => _sfxVolume;
        set
        {
            _sfxVolume = Math.Clamp(value, 0f, 1f);
            // Update volume for all looping sounds
            foreach (var sound in _loopingSounds.Values)
            {
                sound.Volume = _sfxVolume;
            }
        }
    }
    
    public bool IsMusicEnabled
    {
        get => _isMusicEnabled;
        set
        {
            _isMusicEnabled = value;
            if (!_isMusicEnabled && MediaPlayer.State == MediaState.Playing)
            {
                MediaPlayer.Pause();
            }
            else if (_isMusicEnabled && MediaPlayer.State == MediaState.Paused)
            {
                MediaPlayer.Resume();
            }
        }
    }
    
    public bool AreSfxEnabled
    {
        get => _areSfxEnabled;
        set
        {
            _areSfxEnabled = value;
            if (!_areSfxEnabled)
            {
                StopAllLoopingSounds();
            }
        }
    }
    
    public AudioManager()
    {
        _soundEffects = new Dictionary<string, SoundEffect>();
        _music = new Dictionary<string, Song>();
        _loopingSounds = new Dictionary<string, SoundEffectInstance>();
        
        _musicVolume = 0.7f;
        _sfxVolume = 0.8f;
        _isMusicEnabled = true;
        _areSfxEnabled = true;
        
        MediaPlayer.Volume = _musicVolume;
        MediaPlayer.IsRepeating = true;
    }
    
    /// <summary>
    /// Register a sound effect for later playback
    /// </summary>
    public void LoadSoundEffect(string name, SoundEffect soundEffect)
    {
        if (!_soundEffects.ContainsKey(name))
        {
            _soundEffects[name] = soundEffect;
        }
    }
    
    /// <summary>
    /// Register a music track for later playback
    /// </summary>
    public void LoadMusic(string name, Song song)
    {
        if (!_music.ContainsKey(name))
        {
            _music[name] = song;
        }
    }
    
    /// <summary>
    /// Play a sound effect once
    /// </summary>
    public void PlaySound(string soundName, float volume = 1.0f, float pitch = 0f, float pan = 0f)
    {
        if (!_areSfxEnabled)
            return;
            
        if (_soundEffects.TryGetValue(soundName, out var sound))
        {
            float adjustedVolume = _sfxVolume * Math.Clamp(volume, 0f, 1f);
            sound.Play(adjustedVolume, pitch, pan);
        }
    }
    
    /// <summary>
    /// Play a looping sound effect
    /// </summary>
    public void PlayLoopingSound(string soundName, float volume = 1.0f)
    {
        if (!_areSfxEnabled)
            return;
            
        if (_soundEffects.TryGetValue(soundName, out var sound))
        {
            if (_loopingSounds.ContainsKey(soundName))
            {
                // Already playing
                return;
            }
            
            var instance = sound.CreateInstance();
            instance.IsLooped = true;
            instance.Volume = _sfxVolume * Math.Clamp(volume, 0f, 1f);
            instance.Play();
            
            _loopingSounds[soundName] = instance;
        }
    }
    
    /// <summary>
    /// Stop a looping sound effect
    /// </summary>
    public void StopLoopingSound(string soundName)
    {
        if (_loopingSounds.TryGetValue(soundName, out var sound))
        {
            sound.Stop();
            sound.Dispose();
            _loopingSounds.Remove(soundName);
        }
    }
    
    /// <summary>
    /// Stop all looping sounds
    /// </summary>
    public void StopAllLoopingSounds()
    {
        foreach (var sound in _loopingSounds.Values)
        {
            sound.Stop();
            sound.Dispose();
        }
        _loopingSounds.Clear();
    }
    
    /// <summary>
    /// Play background music
    /// </summary>
    public void PlayMusic(string musicName)
    {
        if (!_isMusicEnabled)
            return;
            
        if (_currentMusicTrack == musicName && MediaPlayer.State == MediaState.Playing)
            return; // Already playing this track
            
        if (_music.TryGetValue(musicName, out var song))
        {
            try
            {
                MediaPlayer.Play(song);
                _currentMusicTrack = musicName;
            }
            catch (Exception)
            {
                // Music playback may fail on some systems, handle gracefully
            }
        }
    }
    
    /// <summary>
    /// Stop current music
    /// </summary>
    public void StopMusic()
    {
        MediaPlayer.Stop();
        _currentMusicTrack = null;
    }
    
    /// <summary>
    /// Pause current music
    /// </summary>
    public void PauseMusic()
    {
        if (MediaPlayer.State == MediaState.Playing)
        {
            MediaPlayer.Pause();
        }
    }
    
    /// <summary>
    /// Resume paused music
    /// </summary>
    public void ResumeMusic()
    {
        if (MediaPlayer.State == MediaState.Paused && _isMusicEnabled)
        {
            MediaPlayer.Resume();
        }
    }
    
    /// <summary>
    /// Fade out music over specified duration
    /// </summary>
    public void FadeOutMusic(float duration)
    {
        // Simple implementation - for more complex fade, use Update() method
        MediaPlayer.Volume = 0f;
        StopMusic();
        MediaPlayer.Volume = _musicVolume;
    }
    
    /// <summary>
    /// Get the current playing music track name
    /// </summary>
    public string GetCurrentMusicTrack()
    {
        return _currentMusicTrack;
    }
    
    /// <summary>
    /// Check if a specific sound effect is loaded
    /// </summary>
    public bool HasSoundEffect(string soundName)
    {
        return _soundEffects.ContainsKey(soundName);
    }
    
    /// <summary>
    /// Check if a specific music track is loaded
    /// </summary>
    public bool HasMusic(string musicName)
    {
        return _music.ContainsKey(musicName);
    }
    
    /// <summary>
    /// Cleanup and dispose all audio resources
    /// </summary>
    public void Dispose()
    {
        StopAllLoopingSounds();
        StopMusic();
        
        foreach (var sound in _soundEffects.Values)
        {
            sound?.Dispose();
        }
        _soundEffects.Clear();
        
        _music.Clear();
    }
}

/// <summary>
/// Predefined sound effect names for easy reference
/// </summary>
public static class SoundEffects
{
    // Tool sounds
    public const string Hoe = "hoe";
    public const string WateringCan = "watering_can";
    public const string Pickaxe = "pickaxe";
    public const string Axe = "axe";
    public const string Scythe = "scythe";
    public const string FishingCast = "fishing_cast";
    public const string FishingCatch = "fishing_catch";
    
    // UI sounds
    public const string MenuOpen = "menu_open";
    public const string MenuClose = "menu_close";
    public const string MenuSelect = "menu_select";
    public const string MenuHover = "menu_hover";
    public const string Achievement = "achievement";
    
    // Action sounds
    public const string PlantSeed = "plant_seed";
    public const string Harvest = "harvest";
    public const string Purchase = "purchase";
    public const string Sell = "sell";
    public const string Craft = "craft";
    public const string Eat = "eat";
    public const string Drink = "drink";
    
    // World sounds
    public const string Footstep = "footstep";
    public const string DoorOpen = "door_open";
    public const string Splash = "splash";
    
    // NPC sounds
    public const string NPCTalk = "npc_talk";
    public const string GiftGive = "gift_give";
    public const string HeartLevel = "heart_level";
}

/// <summary>
/// Predefined music track names for easy reference
/// </summary>
public static class MusicTracks
{
    // Season music
    public const string SpringDay = "spring_day";
    public const string SummerDay = "summer_day";
    public const string FallDay = "fall_day";
    public const string WinterDay = "winter_day";
    
    public const string SpringNight = "spring_night";
    public const string SummerNight = "summer_night";
    public const string FallNight = "fall_night";
    public const string WinterNight = "winter_night";
    
    // Special location music
    public const string Mine = "mine";
    public const string Town = "town";
    public const string Shop = "shop";
    
    // Event music
    public const string Festival = "festival";
    public const string Wedding = "wedding";
    
    // Menu music
    public const string MainMenu = "main_menu";
}
