using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Characters.NPCs;

namespace MoonBrookRidge.UI.Dialogue;

/// <summary>
/// Chat bubble that appears above NPCs during conversations
/// </summary>
public class ChatBubble
{
    private Vector2 _position;
    private string _text;
    private bool _isVisible;
    private float _displayTime;
    private const float MAX_DISPLAY_TIME = 3f;
    
    public ChatBubble()
    {
        _isVisible = false;
    }
    
    public void Show(Vector2 position, string text)
    {
        _position = position;
        _text = text;
        _isVisible = true;
        _displayTime = MAX_DISPLAY_TIME;
    }
    
    public void Hide()
    {
        _isVisible = false;
    }
    
    public void Update(GameTime gameTime)
    {
        if (_isVisible)
        {
            _displayTime -= (float)gameTime.ElapsedGameTime.TotalSeconds;
            if (_displayTime <= 0)
            {
                Hide();
            }
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        if (!_isVisible) return;
        
        Vector2 textSize = font.MeasureString(_text);
        Vector2 bubbleSize = textSize + new Vector2(20, 10);
        
        // Position bubble above character
        Vector2 bubblePos = _position - new Vector2(bubbleSize.X / 2, 60);
        
        // Draw bubble background
        Texture2D pixel = CreatePixelTexture(spriteBatch.GraphicsDevice);
        Rectangle bubbleRect = new Rectangle((int)bubblePos.X, (int)bubblePos.Y, 
                                            (int)bubbleSize.X, (int)bubbleSize.Y);
        
        // Draw shadow
        spriteBatch.Draw(pixel, bubbleRect with { X = bubbleRect.X + 2, Y = bubbleRect.Y + 2 }, 
                        new Color(0, 0, 0, 128));
        
        // Draw bubble
        spriteBatch.Draw(pixel, bubbleRect, Color.White);
        
        // Draw border
        DrawRectangleBorder(spriteBatch, bubbleRect, Color.Black, 2);
        
        // Draw text
        Vector2 textPos = bubblePos + new Vector2(10, 5);
        spriteBatch.DrawString(font, _text, textPos, Color.Black);
        
        // Draw pointer (small triangle pointing down to character)
        DrawPointer(spriteBatch, bubblePos + new Vector2(bubbleSize.X / 2, bubbleSize.Y));
    }
    
    private void DrawPointer(SpriteBatch spriteBatch, Vector2 position)
    {
        // Simple triangle pointer
        Texture2D pixel = CreatePixelTexture(spriteBatch.GraphicsDevice);
        for (int i = 0; i < 10; i++)
        {
            Rectangle rect = new Rectangle((int)position.X - i/2, (int)position.Y + i, i, 1);
            spriteBatch.Draw(pixel, rect, Color.White);
        }
    }
    
    private void DrawRectangleBorder(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness)
    {
        Texture2D pixel = CreatePixelTexture(spriteBatch.GraphicsDevice);
        
        // Top
        spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Bottom
        spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
        // Left
        spriteBatch.Draw(pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Right
        spriteBatch.Draw(pixel, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
    }
    
    private Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
    {
        Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
    
    public bool IsVisible => _isVisible;
}
