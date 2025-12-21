using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Characters.NPCs;
using System;
using System.Collections.Generic;

namespace MoonBrookRidge.UI.Dialogue;

/// <summary>
/// Radial wheel UI for selecting dialogue options (Sims 4 style)
/// </summary>
public class RadialDialogueWheel
{
    private Vector2 _centerPosition;
    private float _radius;
    private List<DialogueOption> _options;
    private int _selectedIndex;
    private bool _isActive;
    
    private const float OPTION_CIRCLE_RADIUS = 40f;
    private MouseState _previousMouseState;
    
    public RadialDialogueWheel()
    {
        _radius = 150f;
        _options = new List<DialogueOption>();
        _isActive = false;
        _selectedIndex = -1;
    }
    
    public void Show(Vector2 position, List<DialogueOption> options)
    {
        _centerPosition = position;
        _options = options;
        _isActive = true;
        _selectedIndex = -1;
    }
    
    public void Hide()
    {
        _isActive = false;
        _options.Clear();
    }
    
    public void Update(GameTime gameTime)
    {
        if (!_isActive) return;
        
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
        
        // Calculate which option is being hovered
        _selectedIndex = GetHoveredOptionIndex(mousePos);
        
        // Check for click
        if (mouseState.LeftButton == ButtonState.Released && 
            _previousMouseState.LeftButton == ButtonState.Pressed)
        {
            if (_selectedIndex >= 0)
            {
                OnOptionSelected?.Invoke(_selectedIndex);
            }
        }
        
        _previousMouseState = mouseState;
    }
    
    private int GetHoveredOptionIndex(Vector2 mousePos)
    {
        if (_options.Count == 0) return -1;
        
        float angleStep = MathHelper.TwoPi / _options.Count;
        
        for (int i = 0; i < _options.Count; i++)
        {
            float angle = i * angleStep - MathHelper.PiOver2; // Start from top
            Vector2 optionPos = _centerPosition + new Vector2(
                (float)Math.Cos(angle) * _radius,
                (float)Math.Sin(angle) * _radius
            );
            
            float distance = Vector2.Distance(mousePos, optionPos);
            if (distance < OPTION_CIRCLE_RADIUS)
            {
                return i;
            }
        }
        
        return -1;
    }
    
    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        if (!_isActive) return;
        
        // Draw semi-transparent background circle
        DrawCircle(spriteBatch, _centerPosition, _radius + 20, new Color(0, 0, 0, 180));
        
        // Draw options in circle
        float angleStep = MathHelper.TwoPi / _options.Count;
        
        for (int i = 0; i < _options.Count; i++)
        {
            float angle = i * angleStep - MathHelper.PiOver2; // Start from top
            Vector2 optionPos = _centerPosition + new Vector2(
                (float)Math.Cos(angle) * _radius,
                (float)Math.Sin(angle) * _radius
            );
            
            // Draw option circle
            Color optionColor = (i == _selectedIndex) ? Color.Yellow : Color.White;
            DrawCircle(spriteBatch, optionPos, OPTION_CIRCLE_RADIUS, optionColor);
            
            // Draw option text
            string optionText = _options[i].Text;
            Vector2 textSize = font.MeasureString(optionText);
            Vector2 textPos = optionPos - textSize / 2;
            
            // Draw text shadow
            spriteBatch.DrawString(font, optionText, textPos + Vector2.One, Color.Black);
            spriteBatch.DrawString(font, optionText, textPos, Color.Black);
        }
        
        // Draw connecting lines from center to options
        for (int i = 0; i < _options.Count; i++)
        {
            float angle = i * angleStep - MathHelper.PiOver2;
            Vector2 optionPos = _centerPosition + new Vector2(
                (float)Math.Cos(angle) * _radius,
                (float)Math.Sin(angle) * _radius
            );
            
            DrawLine(spriteBatch, _centerPosition, optionPos, Color.White * 0.5f);
        }
    }
    
    private void DrawCircle(SpriteBatch spriteBatch, Vector2 center, float radius, Color color)
    {
        // Simple circle drawing using a pixel texture
        Texture2D pixel = CreatePixelTexture(spriteBatch.GraphicsDevice);
        Rectangle rect = new Rectangle((int)(center.X - radius), (int)(center.Y - radius), 
                                      (int)(radius * 2), (int)(radius * 2));
        spriteBatch.Draw(pixel, rect, color * 0.8f);
    }
    
    private void DrawLine(SpriteBatch spriteBatch, Vector2 start, Vector2 end, Color color)
    {
        Texture2D pixel = CreatePixelTexture(spriteBatch.GraphicsDevice);
        float length = Vector2.Distance(start, end);
        float angle = (float)Math.Atan2(end.Y - start.Y, end.X - start.X);
        
        spriteBatch.Draw(pixel, start, null, color, angle, Vector2.Zero, 
                        new Vector2(length, 2), SpriteEffects.None, 0);
    }
    
    private Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
    {
        Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
    
    public bool IsActive => _isActive;
    
    public event Action<int> OnOptionSelected;
}
