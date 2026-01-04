using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.UI;

/// <summary>
/// Displays toast-style notifications for game events
/// </summary>
public class NotificationSystem
{
    private readonly List<Notification> _activeNotifications;
    private readonly int _maxNotifications = 5;
    private readonly int _screenWidth;
    private readonly int _screenHeight;
    private Texture2D? _pixelTexture;
    
    public NotificationSystem(int screenWidth, int screenHeight)
    {
        _activeNotifications = new List<Notification>();
        _screenWidth = screenWidth;
        _screenHeight = screenHeight;
    }
    
    /// <summary>
    /// Initialize with a shared pixel texture
    /// </summary>
    public void Initialize(Texture2D pixelTexture)
    {
        _pixelTexture = pixelTexture;
    }
    
    /// <summary>
    /// Show a new notification
    /// </summary>
    public void Show(string message, NotificationType type = NotificationType.Info, float duration = 3.0f)
    {
        // Remove oldest notification if at max
        if (_activeNotifications.Count >= _maxNotifications)
        {
            _activeNotifications.RemoveAt(0);
        }
        
        var notification = new Notification(message, type, duration);
        _activeNotifications.Add(notification);
    }
    
    /// <summary>
    /// Update all active notifications
    /// </summary>
    public void Update(GameTime gameTime)
    {
        for (int i = _activeNotifications.Count - 1; i >= 0; i--)
        {
            _activeNotifications[i].Update(gameTime);
            
            // Remove expired notifications
            if (_activeNotifications[i].IsExpired)
            {
                _activeNotifications.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// Draw all active notifications
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        if (_pixelTexture == null) return;
        
        var yOffset = _screenHeight - 200;
        
        for (int i = 0; i < _activeNotifications.Count; i++)
        {
            var notification = _activeNotifications[i];
            var position = new Vector2(10, yOffset - (i * 40));
            notification.Draw(spriteBatch, font, position, _pixelTexture);
        }
    }
    
    private class Notification
    {
        private readonly string _message;
        private readonly NotificationType _type;
        private readonly float _duration;
        private float _elapsed;
        
        public bool IsExpired => _elapsed >= _duration;
        
        public Notification(string message, NotificationType type, float duration)
        {
            _message = message;
            _type = type;
            _duration = duration;
            _elapsed = 0;
        }
        
        public void Update(GameTime gameTime)
        {
            _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        
        public void Draw(SpriteBatch spriteBatch, SpriteFont font, Vector2 position, Texture2D pixelTexture)
        {
            // Calculate fade out alpha
            var fadeStart = _duration - 0.5f;
            var alpha = _elapsed > fadeStart ? 1.0f - ((_elapsed - fadeStart) / 0.5f) : 1.0f;
            alpha = MathHelper.Clamp(alpha, 0, 1);
            
            // Get color based on type
            var bgColor = GetBackgroundColor(_type);
            bgColor.A = (byte)(bgColor.A * alpha);
            
            var textColor = Color.White;
            textColor.A = (byte)(255 * alpha);
            
            // Measure text
            var textSize = font.MeasureString(_message);
            var padding = 10;
            
            // Draw background
            var bgRect = new Rectangle(
                (int)position.X,
                (int)position.Y,
                (int)textSize.X + padding * 2,
                (int)textSize.Y + padding
            );
            spriteBatch.Draw(pixelTexture, bgRect, bgColor);
            
            // Draw border
            DrawRectangleBorder(spriteBatch, bgRect, GetBorderColor(_type) * alpha, 2, pixelTexture);
            
            // Draw text
            spriteBatch.DrawString(font, _message, position + new Vector2(padding, padding / 2), textColor);
        }
        
        private Color GetBackgroundColor(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => new Color(0, 100, 0, 200),
                NotificationType.Warning => new Color(150, 100, 0, 200),
                NotificationType.Error => new Color(150, 0, 0, 200),
                NotificationType.Quest => new Color(100, 50, 150, 200),
                _ => new Color(0, 0, 0, 200)
            };
        }
        
        private Color GetBorderColor(NotificationType type)
        {
            return type switch
            {
                NotificationType.Success => Color.LightGreen,
                NotificationType.Warning => Color.Yellow,
                NotificationType.Error => Color.Red,
                NotificationType.Quest => Color.Purple,
                _ => Color.White
            };
        }
        
        private void DrawRectangleBorder(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness, Texture2D pixelTexture)
        {
            // Top
            spriteBatch.Draw(pixelTexture, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
            // Bottom
            spriteBatch.Draw(pixelTexture, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
            // Left
            spriteBatch.Draw(pixelTexture, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
            // Right
            spriteBatch.Draw(pixelTexture, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
        }
    }
}

public enum NotificationType
{
    Info,
    Success,
    Warning,
    Error,
    Quest
}
