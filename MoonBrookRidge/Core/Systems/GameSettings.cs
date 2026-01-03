using System;
using System.IO;
using System.Text.Json;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Game settings including resolution, UI scale, and preferences (Phase 7.4)
/// </summary>
public class GameSettings
{
    private static GameSettings? _instance;
    private const string SETTINGS_FILE = "game_settings.json";
    
    // Display settings
    public int ResolutionWidth { get; set; } = 1280;
    public int ResolutionHeight { get; set; } = 720;
    public bool IsFullscreen { get; set; } = false;
    public bool IsBorderless { get; set; } = false;
    
    // UI settings
    public float UIScale { get; set; } = 1.0f; // 0.5x to 2.0x
    
    // Audio settings (for persistence)
    public float MusicVolume { get; set; } = 0.7f;
    public float SfxVolume { get; set; } = 0.8f;
    public bool IsMusicEnabled { get; set; } = true;
    public bool AreSfxEnabled { get; set; } = true;
    
    public static GameSettings Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = Load();
            }
            return _instance;
        }
    }
    
    private GameSettings()
    {
    }
    
    /// <summary>
    /// Load settings from file or create defaults
    /// </summary>
    public static GameSettings Load()
    {
        try
        {
            if (File.Exists(SETTINGS_FILE))
            {
                string json = File.ReadAllText(SETTINGS_FILE);
                var settings = JsonSerializer.Deserialize<GameSettings>(json);
                if (settings != null)
                {
                    return settings;
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading settings: {ex.Message}");
        }
        
        // Return default settings
        return new GameSettings();
    }
    
    /// <summary>
    /// Save settings to file
    /// </summary>
    public void Save()
    {
        try
        {
            var options = new JsonSerializerOptions { WriteIndented = true };
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(SETTINGS_FILE, json);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving settings: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Common resolution presets
    /// </summary>
    public static readonly (int Width, int Height, string Name)[] ResolutionPresets = new[]
    {
        (1280, 720, "1280x720 (HD)"),
        (1366, 768, "1366x768 (HD)"),
        (1600, 900, "1600x900 (HD+)"),
        (1920, 1080, "1920x1080 (Full HD)"),
        (2560, 1440, "2560x1440 (2K)"),
        (3840, 2160, "3840x2160 (4K)")
    };
    
    /// <summary>
    /// UI scale presets
    /// </summary>
    public static readonly (float Scale, string Name)[] UIScalePresets = new[]
    {
        (0.5f, "50% (Tiny)"),
        (0.75f, "75% (Small)"),
        (1.0f, "100% (Normal)"),
        (1.25f, "125% (Large)"),
        (1.5f, "150% (Extra Large)"),
        (2.0f, "200% (Huge)")
    };
}
