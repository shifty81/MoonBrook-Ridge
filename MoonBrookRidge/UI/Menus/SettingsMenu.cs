using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Settings menu for configuring audio and other game options
/// </summary>
public class SettingsMenu
{
    private AudioManager _audioManager;
    private SpriteFont _font;
    private Texture2D _pixel;
    
    public bool IsVisible { get; set; }
    
    private int _selectedOption;
    private const int MENU_WIDTH = 500;
    private const int MENU_HEIGHT = 400;
    private const int OPTION_HEIGHT = 50;
    
    private KeyboardState _previousKeyState;
    
    private enum SettingOption
    {
        MusicVolume,
        SfxVolume,
        MusicEnabled,
        SfxEnabled,
        Close
    }
    
    public SettingsMenu(AudioManager audioManager)
    {
        _audioManager = audioManager;
        _selectedOption = 0;
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
        
        // Navigate options
        if (keyState.IsKeyDown(Keys.Down) && !_previousKeyState.IsKeyDown(Keys.Down))
        {
            _selectedOption++;
            if (_selectedOption > 4) _selectedOption = 0;
            AudioHelper.PlayMenuHoverSound();
        }
        
        if (keyState.IsKeyDown(Keys.Up) && !_previousKeyState.IsKeyDown(Keys.Up))
        {
            _selectedOption--;
            if (_selectedOption < 0) _selectedOption = 4;
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
            if (_selectedOption == (int)SettingOption.MusicEnabled)
            {
                _audioManager.IsMusicEnabled = !_audioManager.IsMusicEnabled;
                AudioHelper.PlayMenuSelectSound();
            }
            else if (_selectedOption == (int)SettingOption.SfxEnabled)
            {
                _audioManager.AreSfxEnabled = !_audioManager.AreSfxEnabled;
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
            case SettingOption.MusicVolume:
                _audioManager.MusicVolume += direction * volumeStep;
                AudioHelper.PlayMenuSelectSound();
                break;
                
            case SettingOption.SfxVolume:
                _audioManager.SfxVolume += direction * volumeStep;
                AudioHelper.PlayMenuSelectSound();
                break;
                
            case SettingOption.MusicEnabled:
            case SettingOption.SfxEnabled:
                // Toggle with left/right too
                if (direction != 0)
                {
                    if (_selectedOption == (int)SettingOption.MusicEnabled)
                        _audioManager.IsMusicEnabled = !_audioManager.IsMusicEnabled;
                    else
                        _audioManager.AreSfxEnabled = !_audioManager.AreSfxEnabled;
                    AudioHelper.PlayMenuSelectSound();
                }
                break;
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (!IsVisible || _font == null || _pixel == null)
            return;
        
        int screenWidth = graphicsDevice.Viewport.Width;
        int screenHeight = graphicsDevice.Viewport.Height;
        
        int menuX = (screenWidth - MENU_WIDTH) / 2;
        int menuY = (screenHeight - MENU_HEIGHT) / 2;
        
        // Background
        var bgRect = new Rectangle(menuX, menuY, MENU_WIDTH, MENU_HEIGHT);
        spriteBatch.Draw(_pixel, bgRect, Color.Black * 0.9f);
        
        // Border
        DrawBorder(spriteBatch, bgRect, 3, Color.White);
        
        // Title
        string title = "Settings";
        Vector2 titleSize = _font.MeasureString(title);
        Vector2 titlePos = new Vector2(menuX + MENU_WIDTH / 2 - titleSize.X / 2, menuY + 20);
        spriteBatch.DrawString(_font, title, titlePos, Color.White);
        
        // Options
        int startY = menuY + 80;
        
        DrawOption(spriteBatch, menuX, startY + 0 * OPTION_HEIGHT, 0, 
            "Music Volume", $"{(_audioManager.MusicVolume * 100):F0}%");
        DrawOption(spriteBatch, menuX, startY + 1 * OPTION_HEIGHT, 1,
            "SFX Volume", $"{(_audioManager.SfxVolume * 100):F0}%");
        DrawOption(spriteBatch, menuX, startY + 2 * OPTION_HEIGHT, 2,
            "Music", _audioManager.IsMusicEnabled ? "ON" : "OFF");
        DrawOption(spriteBatch, menuX, startY + 3 * OPTION_HEIGHT, 3,
            "Sound Effects", _audioManager.AreSfxEnabled ? "ON" : "OFF");
        DrawOption(spriteBatch, menuX, startY + 4 * OPTION_HEIGHT, 4,
            "Close", "");
        
        // Volume bars
        DrawVolumeBar(spriteBatch, menuX + 300, startY + 15, _audioManager.MusicVolume);
        DrawVolumeBar(spriteBatch, menuX + 300, startY + OPTION_HEIGHT + 15, _audioManager.SfxVolume);
        
        // Controls hint
        string hint = "↑/↓: Navigate | ←/→: Adjust | Enter/Space: Toggle | ESC: Close";
        Vector2 hintSize = _font.MeasureString(hint);
        Vector2 hintPos = new Vector2(menuX + MENU_WIDTH / 2 - hintSize.X / 2, menuY + MENU_HEIGHT - 30);
        spriteBatch.DrawString(_font, hint, hintPos, Color.Gray);
    }
    
    private void DrawOption(SpriteBatch spriteBatch, int x, int y, int index, string label, string value)
    {
        bool isSelected = _selectedOption == index;
        Color bgColor = isSelected ? new Color(60, 60, 80) : new Color(30, 30, 40);
        Color textColor = isSelected ? Color.Yellow : Color.White;
        
        var optionRect = new Rectangle(x + 10, y, MENU_WIDTH - 20, OPTION_HEIGHT - 5);
        spriteBatch.Draw(_pixel, optionRect, bgColor);
        
        if (isSelected)
        {
            DrawBorder(spriteBatch, optionRect, 2, Color.Yellow);
        }
        
        Vector2 labelPos = new Vector2(x + 20, y + 10);
        spriteBatch.DrawString(_font, label, labelPos, textColor);
        
        if (!string.IsNullOrEmpty(value))
        {
            Vector2 valueSize = _font.MeasureString(value);
            Vector2 valuePos = new Vector2(x + MENU_WIDTH - valueSize.X - 30, y + 10);
            spriteBatch.DrawString(_font, value, valuePos, textColor);
        }
    }
    
    private void DrawVolumeBar(SpriteBatch spriteBatch, int x, int y, float volume)
    {
        int barWidth = 150;
        int barHeight = 20;
        
        // Background
        var bgRect = new Rectangle(x, y, barWidth, barHeight);
        spriteBatch.Draw(_pixel, bgRect, Color.DarkGray);
        
        // Fill
        int fillWidth = (int)(barWidth * volume);
        var fillRect = new Rectangle(x, y, fillWidth, barHeight);
        Color fillColor = volume > 0.7f ? Color.Green : volume > 0.3f ? Color.Yellow : Color.Red;
        spriteBatch.Draw(_pixel, fillRect, fillColor);
        
        // Border
        DrawBorder(spriteBatch, bgRect, 1, Color.White);
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
