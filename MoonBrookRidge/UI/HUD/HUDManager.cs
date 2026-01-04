using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Characters.Player;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.UI.HUD;

/// <summary>
/// Heads-up display showing player stats, time, money, hunger, and thirst
/// </summary>
public class HUDManager
{
    private Rectangle _healthBarRect;
    private Rectangle _energyBarRect;
    private Rectangle _hungerBarRect;
    private Rectangle _thirstBarRect;
    private Vector2 _timePosition;
    private Vector2 _moneyPosition;
    
    private const int BAR_HEIGHT = 20;
    private const int BAR_WIDTH = 200;
    private const int BAR_SPACING = 5;
    
    public HUDManager()
    {
        int x = 10;
        int y = 10;
        
        _healthBarRect = new Rectangle(x, y, BAR_WIDTH, BAR_HEIGHT);
        y += BAR_HEIGHT + BAR_SPACING;
        
        _energyBarRect = new Rectangle(x, y, BAR_WIDTH, BAR_HEIGHT);
        y += BAR_HEIGHT + BAR_SPACING;
        
        _hungerBarRect = new Rectangle(x, y, BAR_WIDTH, BAR_HEIGHT);
        y += BAR_HEIGHT + BAR_SPACING;
        
        _thirstBarRect = new Rectangle(x, y, BAR_WIDTH, BAR_HEIGHT);
        y += BAR_HEIGHT + BAR_SPACING;
        
        _timePosition = new Vector2(x, y + 5);
        _moneyPosition = new Vector2(x, y + 25);
    }
    
    public void Update(GameTime gameTime, PlayerCharacter player, TimeSystem timeSystem)
    {
        // Update logic if needed
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        // This will be implemented with actual rendering in the gameplay state
        // For now, placeholder
    }
    
    public void DrawPlayerStats(SpriteBatch spriteBatch, SpriteFont font, PlayerCharacter player, TimeSystem timeSystem)
    {
        Texture2D pixel = CreatePixelTexture(spriteBatch.GraphicsDevice);
        
        // Draw health bar
        DrawStatBar(spriteBatch, pixel, _healthBarRect, player.Health, player.MaxHealth, 
                   Color.DarkRed, Color.Red, "Health");
        
        // Draw energy bar
        DrawStatBar(spriteBatch, pixel, _energyBarRect, player.Energy, player.MaxEnergy, 
                   Color.DarkGoldenrod, Color.Gold, "Energy");
        
        // Draw hunger bar (with warning colors)
        Color hungerBg = player.Stats.IsHungerCritical() ? Color.DarkRed : Color.DarkOliveGreen;
        Color hungerFg = player.Stats.IsHungerCritical() ? Color.Red : 
                        player.Stats.IsHungerLow() ? Color.Orange : Color.LimeGreen;
        DrawStatBar(spriteBatch, pixel, _hungerBarRect, player.Stats.Hunger, 100f, 
                   hungerBg, hungerFg, "Hunger");
        
        // Draw thirst bar (with warning colors)
        Color thirstBg = player.Stats.IsThirstCritical() ? Color.DarkRed : Color.DarkBlue;
        Color thirstFg = player.Stats.IsThirstCritical() ? Color.Red : 
                        player.Stats.IsThirstLow() ? Color.Orange : Color.Cyan;
        DrawStatBar(spriteBatch, pixel, _thirstBarRect, player.Stats.Thirst, 100f, 
                   thirstBg, thirstFg, "Thirst");
        
        // Draw time
        string timeText = $"Time: {timeSystem.GetFormattedTime()}";
        DrawTextWithShadow(spriteBatch, font, timeText, _timePosition, Color.White);
        
        // Draw season and day
        string dateText = $"{timeSystem.CurrentSeason} {timeSystem.Day}, Year {timeSystem.Year}";
        Vector2 datePos = new Vector2(_timePosition.X, _timePosition.Y + 20);
        DrawTextWithShadow(spriteBatch, font, dateText, datePos, Color.White);
        
        // Draw money
        string moneyText = $"Money: ${player.Money}";
        DrawTextWithShadow(spriteBatch, font, moneyText, _moneyPosition, Color.Gold);
        
        // Draw warning messages
        DrawWarnings(spriteBatch, font, player);
    }
    
    private void DrawStatBar(SpriteBatch spriteBatch, Texture2D pixel, Rectangle rect, 
                            float current, float max, Color bgColor, Color fgColor, string label)
    {
        // Draw background
        spriteBatch.Draw(pixel, rect, bgColor);
        
        // Draw foreground (filled portion)
        float percent = current / max;
        Rectangle fillRect = rect;
        fillRect.Width = (int)(rect.Width * percent);
        spriteBatch.Draw(pixel, fillRect, fgColor);
        
        // Draw border
        DrawRectangleBorder(spriteBatch, pixel, rect, Color.Black, 2);
        
        // Draw label (optional, can be removed if too cluttered)
        // Vector2 labelPos = new Vector2(rect.X + 5, rect.Y + 2);
        // spriteBatch.DrawString(font, label, labelPos, Color.White);
    }
    
    private void DrawWarnings(SpriteBatch spriteBatch, SpriteFont font, PlayerCharacter player)
    {
        Vector2 warningPos = new Vector2(10, 200);
        
        if (player.Stats.IsHungerCritical())
        {
            DrawTextWithShadow(spriteBatch, font, "[!] STARVING!", warningPos, Color.Red);
            warningPos.Y += 20;
        }
        else if (player.Stats.IsHungerLow())
        {
            DrawTextWithShadow(spriteBatch, font, "[!] Hungry", warningPos, Color.Orange);
            warningPos.Y += 20;
        }
        
        if (player.Stats.IsThirstCritical())
        {
            DrawTextWithShadow(spriteBatch, font, "[!] DEHYDRATED!", warningPos, Color.Red);
            warningPos.Y += 20;
        }
        else if (player.Stats.IsThirstLow())
        {
            DrawTextWithShadow(spriteBatch, font, "[!] Thirsty", warningPos, Color.Orange);
            warningPos.Y += 20;
        }
        
        if (player.Energy < 20f)
        {
            DrawTextWithShadow(spriteBatch, font, "[!] Exhausted", warningPos, Color.Yellow);
        }
    }
    
    private void DrawTextWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, Vector2 position, Color color)
    {
        if (font != null)
        {
            // Draw shadow
            spriteBatch.DrawString(font, text, position + Vector2.One, Color.Black);
            // Draw text
            spriteBatch.DrawString(font, text, position, color);
        }
    }
    
    private void DrawRectangleBorder(SpriteBatch spriteBatch, Texture2D pixel, Rectangle rect, Color color, int thickness)
    {
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
}
