using System;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Settings menu for configuring display, UI scale, audio and other game options (Phase 7.4)
/// </summary>
public class SettingsMenu
{
    private AudioManager _audioManager;
    private Game1 _game;
    private SpriteFont _font = null!;
    private Texture2D _pixel = null!;
    
    public bool IsVisible { get; set; }
    
    private int _selectedOption;
    private const int MENU_WIDTH = 600;
    private const int MENU_HEIGHT = 550;
    private const int OPTION_HEIGHT = 45;
    
    private KeyboardState _previousKeyState;
    
    private enum SettingOption
    {
        Resolution,
        Fullscreen,
        UIScale,
        MusicVolume,
        SfxVolume,
        MusicEnabled,
        SfxEnabled,
        Apply,
        Close
    }
    
    private int _resolutionIndex = 0;
    private int _uiScaleIndex = 2; // Default to 1.0x (100%)
    
    public SettingsMenu(AudioManager audioManager, Game1 game)
    {
        _audioManager = audioManager;
        _game = game;
        _selectedOption = 0;
        
        // Find current resolution index
        var settings = game.Settings;
        for (int i = 0; i < GameSettings.ResolutionPresets.Length; i++)
        {
            if (GameSettings.ResolutionPresets[i].Width == settings.ResolutionWidth &&
                GameSettings.ResolutionPresets[i].Height == settings.ResolutionHeight)
            {
                _resolutionIndex = i;
                break;
            }
        }
        
        // Find current UI scale index
        for (int i = 0; i < GameSettings.UIScalePresets.Length; i++)
        {
            if (Math.Abs(GameSettings.UIScalePresets[i].Scale - settings.UIScale) < 0.01f)
            {
                _uiScaleIndex = i;
                break;
            }
        }
    }
    
    public void LoadContent(SpriteFont font, Texture2D pixel)
    {
        _font = font;
        _pixel = pixel;
    }
    
    public void Update(GameTime gameTime, KeyboardState keyState)
    {
        if (!IsVisible)
            return;
        
        int maxOption = (int)SettingOption.Close;
        
        // Navigate options
        if (keyState.IsKeyDown(Keys.Down) && !_previousKeyState.IsKeyDown(Keys.Down))
        {
            _selectedOption++;
            if (_selectedOption > maxOption) _selectedOption = 0;
            AudioHelper.PlayMenuHoverSound();
        }
        
        if (keyState.IsKeyDown(Keys.Up) && !_previousKeyState.IsKeyDown(Keys.Up))
        {
            _selectedOption--;
            if (_selectedOption < 0) _selectedOption = maxOption;
            AudioHelper.PlayMenuHoverSound();
        }
        
        // Adjust values with left/right
        if (keyState.IsKeyDown(Keys.Left) && !_previousKeyState.IsKeyDown(Keys.Left))
        {
            AdjustSetting(-1);
        }
        
        if (keyState.IsKeyDown(Keys.Right) && !_previousKeyState.IsKeyDown(Keys.Right))
        {
            AdjustSetting(1);
        }
        
        // Toggle boolean settings with Enter or Space
        if ((keyState.IsKeyDown(Keys.Enter) && !_previousKeyState.IsKeyDown(Keys.Enter)) ||
            (keyState.IsKeyDown(Keys.Space) && !_previousKeyState.IsKeyDown(Keys.Space)))
        {
            if (_selectedOption == (int)SettingOption.Fullscreen)
            {
                _game.Settings.IsFullscreen = !_game.Settings.IsFullscreen;
                AudioHelper.PlayMenuSelectSound();
            }
            else if (_selectedOption == (int)SettingOption.MusicEnabled)
            {
                _audioManager.IsMusicEnabled = !_audioManager.IsMusicEnabled;
                AudioHelper.PlayMenuSelectSound();
            }
            else if (_selectedOption == (int)SettingOption.SfxEnabled)
            {
                _audioManager.AreSfxEnabled = !_audioManager.AreSfxEnabled;
                AudioHelper.PlayMenuSelectSound();
            }
            else if (_selectedOption == (int)SettingOption.Apply)
            {
                ApplySettings();
                AudioHelper.PlayMenuSelectSound();
            }
            else if (_selectedOption == (int)SettingOption.Close)
            {
                IsVisible = false;
                AudioHelper.PlayMenuCloseSound();
            }
        }
        
        // Close with Escape
        if (keyState.IsKeyDown(Keys.Escape) && !_previousKeyState.IsKeyDown(Keys.Escape))
        {
            IsVisible = false;
            AudioHelper.PlayMenuCloseSound();
        }
        
        _previousKeyState = keyState;
    }
    
    private void AdjustSetting(int direction)
    {
        const float volumeStep = 0.1f;
        
        switch ((SettingOption)_selectedOption)
        {
            case SettingOption.Resolution:
                _resolutionIndex += direction;
                if (_resolutionIndex < 0) _resolutionIndex = GameSettings.ResolutionPresets.Length - 1;
                if (_resolutionIndex >= GameSettings.ResolutionPresets.Length) _resolutionIndex = 0;
                AudioHelper.PlayMenuSelectSound();
                break;
                
            case SettingOption.UIScale:
                _uiScaleIndex += direction;
                if (_uiScaleIndex < 0) _uiScaleIndex = GameSettings.UIScalePresets.Length - 1;
                if (_uiScaleIndex >= GameSettings.UIScalePresets.Length) _uiScaleIndex = 0;
                AudioHelper.PlayMenuSelectSound();
                break;
                
            case SettingOption.MusicVolume:
                _audioManager.MusicVolume += direction * volumeStep;
                AudioHelper.PlayMenuSelectSound();
                break;
                
            case SettingOption.SfxVolume:
                _audioManager.SfxVolume += direction * volumeStep;
                AudioHelper.PlayMenuSelectSound();
                break;
                
            case SettingOption.Fullscreen:
            case SettingOption.MusicEnabled:
            case SettingOption.SfxEnabled:
                // Toggle with left/right too
                if (direction != 0)
                {
                    if (_selectedOption == (int)SettingOption.Fullscreen)
                        _game.Settings.IsFullscreen = !_game.Settings.IsFullscreen;
                    else if (_selectedOption == (int)SettingOption.MusicEnabled)
                        _audioManager.IsMusicEnabled = !_audioManager.IsMusicEnabled;
                    else
                        _audioManager.AreSfxEnabled = !_audioManager.AreSfxEnabled;
                    AudioHelper.PlayMenuSelectSound();
                }
                break;
        }
    }
    
    private void ApplySettings()
    {
        // Apply resolution and fullscreen
        var resolution = GameSettings.ResolutionPresets[_resolutionIndex];
        _game.ApplyDisplaySettings(resolution.Width, resolution.Height, 
            _game.Settings.IsFullscreen, _game.Settings.IsBorderless);
        
        // Apply UI scale
        _game.Settings.UIScale = GameSettings.UIScalePresets[_uiScaleIndex].Scale;
        
        // Save audio settings
        _game.Settings.MusicVolume = _audioManager.MusicVolume;
        _game.Settings.SfxVolume = _audioManager.SfxVolume;
        _game.Settings.IsMusicEnabled = _audioManager.IsMusicEnabled;
        _game.Settings.AreSfxEnabled = _audioManager.AreSfxEnabled;
        
        // Save to file
        _game.Settings.Save();
    }
    
    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (!IsVisible || _font == null || _pixel == null)
            return;
        
        int screenWidth = graphicsDevice.Viewport.Width;
        int screenHeight = graphicsDevice.Viewport.Height;
        
        // Apply UI scale
        float uiScale = _game.UIScale;
        int scaledMenuWidth = (int)(MENU_WIDTH * uiScale);
        int scaledMenuHeight = (int)(MENU_HEIGHT * uiScale);
        
        int menuX = (screenWidth - scaledMenuWidth) / 2;
        int menuY = (screenHeight - scaledMenuHeight) / 2;
        
        // Background
        var bgRect = new Rectangle(menuX, menuY, scaledMenuWidth, scaledMenuHeight);
        spriteBatch.Draw(_pixel, bgRect, Color.Black * 0.9f);
        
        // Border
        DrawBorder(spriteBatch, bgRect, (int)(3 * uiScale), Color.White);
        
        // Title
        string title = "Settings";
        Vector2 titleSize = _font.MeasureString(title) * uiScale;
        Vector2 titlePos = new Vector2(menuX + scaledMenuWidth / 2 - titleSize.X / 2, menuY + 20 * uiScale);
        spriteBatch.DrawString(_font, title, titlePos, Color.White, 0f, Vector2.Zero, uiScale, SpriteEffects.None, 0f);
        
        // Options
        int startY = (int)(menuY + 70 * uiScale);
        int scaledOptionHeight = (int)(OPTION_HEIGHT * uiScale);
        
        var resolution = GameSettings.ResolutionPresets[_resolutionIndex];
        var uiScalePreset = GameSettings.UIScalePresets[_uiScaleIndex];
        
        DrawOption(spriteBatch, menuX, startY + 0 * scaledOptionHeight, 0, uiScale,
            "Resolution", resolution.Name);
        DrawOption(spriteBatch, menuX, startY + 1 * scaledOptionHeight, 1, uiScale,
            "Fullscreen", _game.Settings.IsFullscreen ? "ON" : "OFF");
        DrawOption(spriteBatch, menuX, startY + 2 * scaledOptionHeight, 2, uiScale,
            "UI Scale", uiScalePreset.Name);
        DrawOption(spriteBatch, menuX, startY + 3 * scaledOptionHeight, 3, uiScale,
            "Music Volume", $"{(_audioManager.MusicVolume * 100):F0}%");
        DrawOption(spriteBatch, menuX, startY + 4 * scaledOptionHeight, 4, uiScale,
            "SFX Volume", $"{(_audioManager.SfxVolume * 100):F0}%");
        DrawOption(spriteBatch, menuX, startY + 5 * scaledOptionHeight, 5, uiScale,
            "Music", _audioManager.IsMusicEnabled ? "ON" : "OFF");
        DrawOption(spriteBatch, menuX, startY + 6 * scaledOptionHeight, 6, uiScale,
            "Sound Effects", _audioManager.AreSfxEnabled ? "ON" : "OFF");
        DrawOption(spriteBatch, menuX, startY + 7 * scaledOptionHeight, 7, uiScale,
            "Apply Changes", "");
        DrawOption(spriteBatch, menuX, startY + 8 * scaledOptionHeight, 8, uiScale,
            "Close", "");
        
        // Volume bars
        int barX = (int)(menuX + 350 * uiScale);
        DrawVolumeBar(spriteBatch, barX, (int)(startY + 3 * scaledOptionHeight + 12 * uiScale), 
            _audioManager.MusicVolume, uiScale);
        DrawVolumeBar(spriteBatch, barX, (int)(startY + 4 * scaledOptionHeight + 12 * uiScale), 
            _audioManager.SfxVolume, uiScale);
        
        // Controls hint
        string hint = "Up/Down: Navigate | Left/Right: Adjust | Enter: Select/Toggle | ESC: Close";
        Vector2 hintSize = _font.MeasureString(hint) * uiScale;
        Vector2 hintPos = new Vector2(menuX + scaledMenuWidth / 2 - hintSize.X / 2, 
            menuY + scaledMenuHeight - 35 * uiScale);
        spriteBatch.DrawString(_font, hint, hintPos, Color.Gray, 0f, Vector2.Zero, uiScale, SpriteEffects.None, 0f);
    }
    
    private void DrawOption(SpriteBatch spriteBatch, int x, int y, int index, float uiScale, string label, string value)
    {
        bool isSelected = _selectedOption == index;
        Color bgColor = isSelected ? new Color(60, 60, 80) : new Color(30, 30, 40);
        Color textColor = isSelected ? Color.Yellow : Color.White;
        
        int scaledWidth = (int)((MENU_WIDTH - 20) * uiScale);
        int scaledHeight = (int)((OPTION_HEIGHT - 5) * uiScale);
        
        var optionRect = new Rectangle((int)(x + 10 * uiScale), y, scaledWidth, scaledHeight);
        spriteBatch.Draw(_pixel, optionRect, bgColor);
        
        if (isSelected)
        {
            DrawBorder(spriteBatch, optionRect, Math.Max(1, (int)(2 * uiScale)), Color.Yellow);
        }
        
        Vector2 labelPos = new Vector2((int)(x + 20 * uiScale), (int)(y + 10 * uiScale));
        spriteBatch.DrawString(_font, label, labelPos, textColor, 0f, Vector2.Zero, uiScale, SpriteEffects.None, 0f);
        
        if (!string.IsNullOrEmpty(value))
        {
            Vector2 valueSize = _font.MeasureString(value) * uiScale;
            Vector2 valuePos = new Vector2((int)(x + scaledWidth - valueSize.X - 10 * uiScale), (int)(y + 10 * uiScale));
            spriteBatch.DrawString(_font, value, valuePos, textColor, 0f, Vector2.Zero, uiScale, SpriteEffects.None, 0f);
        }
    }
    
    private void DrawVolumeBar(SpriteBatch spriteBatch, int x, int y, float volume, float uiScale)
    {
        int barWidth = (int)(150 * uiScale);
        int barHeight = (int)(20 * uiScale);
        
        // Background
        var bgRect = new Rectangle(x, y, barWidth, barHeight);
        spriteBatch.Draw(_pixel, bgRect, Color.DarkGray);
        
        // Fill
        int fillWidth = (int)(barWidth * volume);
        var fillRect = new Rectangle(x, y, fillWidth, barHeight);
        Color fillColor = volume > 0.7f ? Color.Green : volume > 0.3f ? Color.Yellow : Color.Red;
        spriteBatch.Draw(_pixel, fillRect, fillColor);
        
        // Border
        DrawBorder(spriteBatch, bgRect, Math.Max(1, (int)(1 * uiScale)), Color.White);
    }
    
    private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, int thickness, Color color)
    {
        // Top
        spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Bottom
        spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
        // Left
        spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Right
        spriteBatch.Draw(_pixel, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
    }
}
