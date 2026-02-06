using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Helper class for playing contextual sound effects based on game actions
/// </summary>
public static class AudioHelper
{
    private static AudioManager? _audioManager;
    
    public static void Initialize(AudioManager audioManager)
    {
        _audioManager = audioManager;
    }
    
    // Tool Usage Sounds
    
    public static void PlayToolSound(string toolName)
    {
        string? soundName = toolName.ToLower() switch
        {
            "hoe" => SoundEffects.Hoe,
            "watering can" => SoundEffects.WateringCan,
            "pickaxe" => SoundEffects.Pickaxe,
            "axe" => SoundEffects.Axe,
            "scythe" => SoundEffects.Scythe,
            "fishing rod" => SoundEffects.FishingCast,
            _ => null
        };
        
        if (soundName != null)
        {
            _audioManager?.PlaySound(soundName, volume: 0.7f);
        }
    }
    
    public static void PlayHoeSound() => _audioManager?.PlaySound(SoundEffects.Hoe, 0.7f);
    public static void PlayWateringSound() => _audioManager?.PlaySound(SoundEffects.WateringCan, 0.6f);
    public static void PlayPickaxeSound() => _audioManager?.PlaySound(SoundEffects.Pickaxe, 0.8f);
    public static void PlayAxeSound() => _audioManager?.PlaySound(SoundEffects.Axe, 0.8f);
    public static void PlayScytheSound() => _audioManager?.PlaySound(SoundEffects.Scythe, 0.7f);
    
    // Fishing Sounds
    
    public static void PlayFishingCastSound() => _audioManager?.PlaySound(SoundEffects.FishingCast, 0.6f);
    public static void PlayFishingCatchSound() => _audioManager?.PlaySound(SoundEffects.FishingCatch, 0.8f);
    
    // UI Sounds
    
    public static void PlayMenuOpenSound() => _audioManager?.PlaySound(SoundEffects.MenuOpen, 0.5f);
    public static void PlayMenuCloseSound() => _audioManager?.PlaySound(SoundEffects.MenuClose, 0.5f);
    public static void PlayMenuSelectSound() => _audioManager?.PlaySound(SoundEffects.MenuSelect, 0.4f);
    public static void PlayMenuHoverSound() => _audioManager?.PlaySound(SoundEffects.MenuHover, 0.3f);
    public static void PlayAchievementSound() => _audioManager?.PlaySound(SoundEffects.Achievement, 0.9f);
    
    // Action Sounds
    
    public static void PlayPlantSeedSound() => _audioManager?.PlaySound(SoundEffects.PlantSeed, 0.6f);
    public static void PlayHarvestSound() => _audioManager?.PlaySound(SoundEffects.Harvest, 0.7f);
    public static void PlayPurchaseSound() => _audioManager?.PlaySound(SoundEffects.Purchase, 0.7f);
    public static void PlaySellSound() => _audioManager?.PlaySound(SoundEffects.Sell, 0.7f);
    public static void PlayCraftSound() => _audioManager?.PlaySound(SoundEffects.Craft, 0.7f);
    public static void PlayEatSound() => _audioManager?.PlaySound(SoundEffects.Eat, 0.6f);
    public static void PlayDrinkSound() => _audioManager?.PlaySound(SoundEffects.Drink, 0.6f);
    
    // World Sounds
    
    public static void PlayFootstepSound() => _audioManager?.PlaySound(SoundEffects.Footstep, 0.2f);
    public static void PlayDoorOpenSound() => _audioManager?.PlaySound(SoundEffects.DoorOpen, 0.6f);
    public static void PlaySplashSound() => _audioManager?.PlaySound(SoundEffects.Splash, 0.5f);
    
    // NPC Sounds
    
    public static void PlayNPCTalkSound() => _audioManager?.PlaySound(SoundEffects.NPCTalk, 0.5f);
    public static void PlayGiftGiveSound() => _audioManager?.PlaySound(SoundEffects.GiftGive, 0.7f);
    public static void PlayHeartLevelSound() => _audioManager?.PlaySound(SoundEffects.HeartLevel, 0.8f);
    
    // Music Control
    
    public static void PlaySeasonalMusic(string season, bool isNight)
    {
        if (_audioManager == null) return;
        
        string musicTrack = season.ToLower() switch
        {
            "spring" => isNight ? MusicTracks.SpringNight : MusicTracks.SpringDay,
            "summer" => isNight ? MusicTracks.SummerNight : MusicTracks.SummerDay,
            "fall" => isNight ? MusicTracks.FallNight : MusicTracks.FallDay,
            "winter" => isNight ? MusicTracks.WinterNight : MusicTracks.WinterDay,
            _ => MusicTracks.SpringDay
        };
        
        _audioManager.PlayMusic(musicTrack);
    }
    
    public static void PlayLocationMusic(string location)
    {
        if (_audioManager == null) return;
        
        string? musicTrack = location.ToLower() switch
        {
            "mine" => MusicTracks.Mine,
            "town" => MusicTracks.Town,
            "shop" => MusicTracks.Shop,
            _ => null
        };
        
        if (musicTrack != null)
        {
            _audioManager.PlayMusic(musicTrack);
        }
    }
    
    public static void PlayEventMusic(string eventType)
    {
        if (_audioManager == null) return;
        
        string? musicTrack = eventType.ToLower() switch
        {
            "festival" => MusicTracks.Festival,
            "wedding" => MusicTracks.Wedding,
            _ => null
        };
        
        if (musicTrack != null)
        {
            _audioManager.PlayMusic(musicTrack);
        }
    }
    
    public static void StopMusic() => _audioManager?.StopMusic();
    public static void PauseMusic() => _audioManager?.PauseMusic();
    public static void ResumeMusic() => _audioManager?.ResumeMusic();
}
