using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using System;
using System.Collections.Generic;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.UI;

/// <summary>
/// Displays achievement unlock notifications as toast messages
/// </summary>
public class AchievementNotification
{
    private struct Notification
    {
        public Achievement Achievement;
        public float TimeRemaining;
        public float SlideProgress;
        public bool IsSlideIn;
    }
    
    private List<Notification> _activeNotifications;
    private const float NOTIFICATION_DURATION = 5f; // seconds
    private const float SLIDE_DURATION = 0.3f; // seconds
    private const int NOTIFICATION_WIDTH = 300;
    private const int NOTIFICATION_HEIGHT = 80;
    private const int NOTIFICATION_MARGIN = 10;
    
    private SpriteFont _font;
    private Texture2D _pixel;
    
    public AchievementNotification()
    {
        _activeNotifications = new List<Notification>();
    }
    
    public void LoadContent(SpriteFont font, Texture2D pixel)
    {
        _font = font;
        _pixel = pixel;
    }
    
    /// <summary>
    /// Show a new achievement notification
    /// </summary>
    public void ShowNotification(Achievement achievement)
    {
        var notification = new Notification
        {
            Achievement = achievement,
            TimeRemaining = NOTIFICATION_DURATION,
            SlideProgress = 0f,
            IsSlideIn = true
        };
        
        _activeNotifications.Add(notification);
    }
    
    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        for (int i = _activeNotifications.Count - 1; i >= 0; i--)
        {
            var notification = _activeNotifications[i];
            
            // Update slide animation
            if (notification.IsSlideIn)
            {
                notification.SlideProgress += deltaTime / SLIDE_DURATION;
                if (notification.SlideProgress >= 1f)
                {
                    notification.SlideProgress = 1f;
                    notification.IsSlideIn = false;
                }
            }
            
            // Update timer
            notification.TimeRemaining -= deltaTime;
            
            // Start slide out when time is almost up
            if (notification.TimeRemaining <= SLIDE_DURATION && notification.SlideProgress > 0f)
            {
                notification.SlideProgress -= deltaTime / SLIDE_DURATION;
                if (notification.SlideProgress <= 0f)
                {
                    _activeNotifications.RemoveAt(i);
                    continue;
                }
            }
            
            _activeNotifications[i] = notification;
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (_activeNotifications.Count == 0 || _font == null || _pixel == null)
            return;
            
        int screenWidth = graphicsDevice.Viewport.Width;
        int screenHeight = graphicsDevice.Viewport.Height;
        
        for (int i = 0; i < _activeNotifications.Count; i++)
        {
            var notification = _activeNotifications[i];
            
            // Calculate position with slide animation
            int baseY = screenHeight - (NOTIFICATION_HEIGHT + NOTIFICATION_MARGIN) * (i + 1);
            int slideOffset = (int)((1f - notification.SlideProgress) * NOTIFICATION_WIDTH);
            int x = screenWidth - NOTIFICATION_WIDTH - NOTIFICATION_MARGIN + slideOffset;
            int y = baseY;
            
            // Background
            var bgRect = new Rectangle(x, y, NOTIFICATION_WIDTH, NOTIFICATION_HEIGHT);
            spriteBatch.Draw(_pixel, bgRect, Color.Black * 0.8f);
            
            // Border
            DrawBorder(spriteBatch, bgRect, 2, GetCategoryColor(notification.Achievement.Category));
            
            // Title
            string title = "Achievement Unlocked!";
            Vector2 titleSize = _font.MeasureString(title);
            Vector2 titlePos = new Vector2(x + 10, y + 10);
            spriteBatch.DrawString(_font, title, titlePos, Color.Gold);
            
            // Achievement name
            string achievementName = notification.Achievement.Name;
            Vector2 namePos = new Vector2(x + 10, y + 30);
            spriteBatch.DrawString(_font, achievementName, namePos, Color.White);
            
            // Achievement description (smaller text if possible, or truncated)
            string desc = notification.Achievement.Description;
            if (desc.Length > 40)
                desc = desc.Substring(0, 37) + "...";
            Vector2 descPos = new Vector2(x + 10, y + 50);
            spriteBatch.DrawString(_font, desc, descPos, Color.LightGray * 0.8f);
        }
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
    
    private Color GetCategoryColor(AchievementCategory category)
    {
        return category switch
        {
            AchievementCategory.Farming => new Color(76, 175, 80),    // Green
            AchievementCategory.Fishing => new Color(33, 150, 243),   // Blue
            AchievementCategory.Mining => new Color(158, 158, 158),   // Gray
            AchievementCategory.Social => new Color(233, 30, 99),     // Pink
            AchievementCategory.Crafting => new Color(255, 152, 0),   // Orange
            AchievementCategory.Wealth => new Color(255, 235, 59),    // Yellow
            AchievementCategory.Exploration => new Color(156, 39, 176), // Purple
            AchievementCategory.Survival => new Color(244, 67, 54),   // Red
            _ => Color.White
        };
    }
    
    public int GetActiveNotificationCount()
    {
        return _activeNotifications.Count;
    }
}
