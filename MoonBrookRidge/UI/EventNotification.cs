using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.UI;

/// <summary>
/// Displays event notifications to the player
/// </summary>
public class EventNotification
{
    private string _message;
    private float _displayTime;
    private float _elapsed;
    private bool _isVisible;
    private Color _backgroundColor;
    private Color _textColor;
    
    private const float NOTIFICATION_DURATION = 5f; // Display for 5 seconds
    private const int PADDING = 20;
    
    public EventNotification()
    {
        _isVisible = false;
        _backgroundColor = new Color(0, 0, 0, 200); // Semi-transparent black
        _textColor = Color.White;
    }
    
    /// <summary>
    /// Shows a notification for an event
    /// </summary>
    public void Show(GameEvent gameEvent)
    {
        _message = $"📅 {gameEvent.Name}\n{gameEvent.Description}";
        _displayTime = NOTIFICATION_DURATION;
        _elapsed = 0f;
        _isVisible = true;
    }
    
    /// <summary>
    /// Shows a custom notification message
    /// </summary>
    public void Show(string message, float duration = NOTIFICATION_DURATION)
    {
        _message = message;
        _displayTime = duration;
        _elapsed = 0f;
        _isVisible = true;
    }
    
    /// <summary>
    /// Updates the notification display
    /// </summary>
    public void Update(GameTime gameTime)
    {
        if (!_isVisible) return;
        
        _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_elapsed >= _displayTime)
        {
            _isVisible = false;
        }
    }
    
    /// <summary>
    /// Draws the notification on screen
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
    {
        if (!_isVisible) return;
        
        // Calculate fade effect
        float alpha = 1f;
        if (_elapsed > _displayTime - 1f)
        {
            // Fade out in the last second
            alpha = (_displayTime - _elapsed);
        }
        else if (_elapsed < 0.5f)
        {
            // Fade in during first half second
            alpha = _elapsed * 2f;
        }
        
        // Measure text
        Vector2 textSize = font.MeasureString(_message);
        
        // Position at top center of screen
        int screenWidth = graphicsDevice.Viewport.Width;
        int boxWidth = (int)textSize.X + PADDING * 2;
        int boxHeight = (int)textSize.Y + PADDING * 2;
        int boxX = (screenWidth - boxWidth) / 2;
        int boxY = 100; // Position near top of screen
        
        // Draw background box
        var bgColor = _backgroundColor * alpha;
        DrawRectangle(spriteBatch, new Rectangle(boxX, boxY, boxWidth, boxHeight), bgColor);
        
        // Draw border
        DrawRectangleBorder(spriteBatch, new Rectangle(boxX, boxY, boxWidth, boxHeight), Color.Gold * alpha, 2);
        
        // Draw text
        Vector2 textPosition = new Vector2(boxX + PADDING, boxY + PADDING);
        spriteBatch.DrawString(font, _message, textPosition, _textColor * alpha);
    }
    
    /// <summary>
    /// Helper method to draw a filled rectangle
    /// </summary>
    private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rectangle, Color color)
    {
        // Create a 1x1 white texture if needed
        Texture2D whiteTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        whiteTexture.SetData(new[] { Color.White });
        
        spriteBatch.Draw(whiteTexture, rectangle, color);
    }
    
    /// <summary>
    /// Helper method to draw a rectangle border
    /// </summary>
    private void DrawRectangleBorder(SpriteBatch spriteBatch, Rectangle rectangle, Color color, int thickness)
    {
        Texture2D whiteTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        whiteTexture.SetData(new[] { Color.White });
        
        // Top
        spriteBatch.Draw(whiteTexture, new Rectangle(rectangle.X, rectangle.Y, rectangle.Width, thickness), color);
        // Bottom
        spriteBatch.Draw(whiteTexture, new Rectangle(rectangle.X, rectangle.Y + rectangle.Height - thickness, rectangle.Width, thickness), color);
        // Left
        spriteBatch.Draw(whiteTexture, new Rectangle(rectangle.X, rectangle.Y, thickness, rectangle.Height), color);
        // Right
        spriteBatch.Draw(whiteTexture, new Rectangle(rectangle.X + rectangle.Width - thickness, rectangle.Y, thickness, rectangle.Height), color);
    }
    
    public bool IsVisible => _isVisible;
}
