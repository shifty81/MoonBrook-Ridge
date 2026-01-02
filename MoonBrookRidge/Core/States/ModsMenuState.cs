using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Core.Systems;
using MoonBrookRidge.Mods;
using System.Collections.Generic;

namespace MoonBrookRidge.Core.States;

/// <summary>
/// Mods menu state for enabling/disabling and configuring mods
/// </summary>
public class ModsMenuState : GameState
{
    private ModLoader _modLoader;
    private List<ModInfo> _availableMods;
    private int _selectedModIndex;
    private Texture2D _pixelTexture;
    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;
    
    public ModsMenuState(Game1 game) : base(game)
    {
        _selectedModIndex = 0;
    }
    
    public override void Initialize()
    {
        base.Initialize();
        _modLoader = new ModLoader();
        _availableMods = _modLoader.DiscoverMods();
    }
    
    public override void LoadContent()
    {
        base.LoadContent();
        _pixelTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });
    }
    
    public override void Update(GameTime gameTime)
    {
        KeyboardState keyboardState = Keyboard.GetState();
        MouseState mouseState = Mouse.GetState();
        
        // Back to main menu
        if ((keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape)) ||
            (keyboardState.IsKeyDown(Keys.Back) && !_previousKeyboardState.IsKeyDown(Keys.Back)))
        {
            Game.StateManager.ChangeState(new MenuState(Game));
            return;
        }
        
        if (_availableMods.Count > 0)
        {
            // Navigate mods list
            if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _selectedModIndex--;
                if (_selectedModIndex < 0) _selectedModIndex = _availableMods.Count - 1;
            }
            
            if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _selectedModIndex++;
                if (_selectedModIndex >= _availableMods.Count) _selectedModIndex = 0;
            }
            
            // Toggle mod enabled/disabled
            if ((keyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space)) ||
                (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter)))
            {
                ToggleMod(_availableMods[_selectedModIndex]);
            }
            
            // Mouse support
            HandleMouseInput(mouseState);
        }
        
        _previousKeyboardState = keyboardState;
        _previousMouseState = mouseState;
    }
    
    private void HandleMouseInput(MouseState mouseState)
    {
        Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
        int screenWidth = Game.GraphicsDevice.Viewport.Width;
        int screenHeight = Game.GraphicsDevice.Viewport.Height;
        
        int listX = screenWidth / 2 - 400;
        int listY = 150;
        int modHeight = 60;
        
        // Check hover and click on mods
        for (int i = 0; i < _availableMods.Count && i < 10; i++)
        {
            Rectangle modBounds = new Rectangle(listX, listY + i * modHeight, 800, modHeight - 10);
            
            if (modBounds.Contains(mousePos))
            {
                _selectedModIndex = i;
                
                // Click to toggle
                if (mouseState.LeftButton == ButtonState.Pressed && 
                    _previousMouseState.LeftButton == ButtonState.Released)
                {
                    ToggleMod(_availableMods[i]);
                }
            }
        }
        
        // Back button
        Rectangle backButton = new Rectangle(screenWidth / 2 - 100, screenHeight - 100, 200, 50);
        if (backButton.Contains(mousePos) && 
            mouseState.LeftButton == ButtonState.Pressed && 
            _previousMouseState.LeftButton == ButtonState.Released)
        {
            Game.StateManager.ChangeState(new MenuState(Game));
        }
    }
    
    private void ToggleMod(ModInfo mod)
    {
        mod.IsEnabled = !mod.IsEnabled;
        _modLoader.SaveModConfiguration();
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        Game.GraphicsDevice.Clear(new Color(25, 25, 35));
        
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        if (Game.DefaultFont != null)
        {
            int screenWidth = Game.GraphicsDevice.Viewport.Width;
            int screenHeight = Game.GraphicsDevice.Viewport.Height;
            
            // Title
            string title = "Mods Manager";
            Vector2 titleSize = Game.DefaultFont.MeasureString(title);
            Vector2 titlePos = new Vector2((screenWidth - titleSize.X) / 2, 50);
            spriteBatch.DrawString(Game.DefaultFont, title, titlePos + new Vector2(2, 2), Color.Black);
            spriteBatch.DrawString(Game.DefaultFont, title, titlePos, Color.LightGreen);
            
            // Mods count
            string countText = $"{_availableMods.Count} mods found";
            spriteBatch.DrawString(Game.DefaultFont, countText, 
                new Vector2(screenWidth / 2 - 400, 110), Color.LightGray);
            
            // Draw mods list
            int listY = 150;
            int modHeight = 60;
            int displayCount = System.Math.Min(_availableMods.Count, 10);
            
            for (int i = 0; i < displayCount; i++)
            {
                var mod = _availableMods[i];
                bool isSelected = (i == _selectedModIndex);
                
                int listX = screenWidth / 2 - 400;
                Rectangle modBg = new Rectangle(listX, listY + i * modHeight, 800, modHeight - 10);
                
                // Background
                Color bgColor = isSelected ? new Color(60, 60, 80) : new Color(40, 40, 50);
                spriteBatch.Draw(_pixelTexture, modBg, bgColor);
                
                // Border
                DrawBorder(spriteBatch, modBg.X, modBg.Y, modBg.Width, modBg.Height, 2, 
                    isSelected ? Color.LightBlue : Color.Gray);
                
                // Mod name
                Color nameColor = mod.IsEnabled ? Color.LightGreen : Color.Gray;
                spriteBatch.DrawString(Game.DefaultFont, mod.Name, 
                    new Vector2(listX + 10, listY + i * modHeight + 10), nameColor);
                
                // Mod version and status
                string status = mod.IsEnabled ? "[ENABLED]" : "[DISABLED]";
                Color statusColor = mod.IsEnabled ? Color.Green : Color.Red;
                spriteBatch.DrawString(Game.DefaultFont, $"v{mod.Version} {status}", 
                    new Vector2(listX + 10, listY + i * modHeight + 35), statusColor);
                
                // Author
                spriteBatch.DrawString(Game.DefaultFont, $"by {mod.Author}", 
                    new Vector2(listX + 600, listY + i * modHeight + 10), Color.LightGray);
            }
            
            // Selected mod description
            if (_availableMods.Count > 0 && _selectedModIndex < _availableMods.Count)
            {
                var selectedMod = _availableMods[_selectedModIndex];
                int descY = listY + displayCount * modHeight + 30;
                
                spriteBatch.DrawString(Game.DefaultFont, "Description:", 
                    new Vector2(screenWidth / 2 - 400, descY), Color.White);
                spriteBatch.DrawString(Game.DefaultFont, selectedMod.Description, 
                    new Vector2(screenWidth / 2 - 400, descY + 30), Color.LightGray);
            }
            
            // Back button
            Rectangle backButton = new Rectangle(screenWidth / 2 - 100, screenHeight - 100, 200, 50);
            MouseState mouseState = Mouse.GetState();
            bool isHovered = backButton.Contains(new Point(mouseState.X, mouseState.Y));
            
            Color buttonColor = isHovered ? new Color(70, 70, 90) : new Color(50, 50, 70);
            spriteBatch.Draw(_pixelTexture, backButton, buttonColor);
            DrawBorder(spriteBatch, backButton.X, backButton.Y, backButton.Width, backButton.Height, 2, Color.White);
            
            string backText = "Back";
            Vector2 backSize = Game.DefaultFont.MeasureString(backText);
            spriteBatch.DrawString(Game.DefaultFont, backText,
                new Vector2(backButton.X + (backButton.Width - backSize.X) / 2, 
                           backButton.Y + (backButton.Height - backSize.Y) / 2),
                Color.White);
            
            // Instructions
            string instructions = "↑↓/Mouse: Navigate | Space/Enter/Click: Toggle | ESC/Back Button: Return";
            Vector2 instSize = Game.DefaultFont.MeasureString(instructions);
            spriteBatch.DrawString(Game.DefaultFont, instructions,
                new Vector2((screenWidth - instSize.X) / 2, screenHeight - 40), Color.Gray);
        }
        
        spriteBatch.End();
    }
    
    private void DrawBorder(SpriteBatch spriteBatch, int x, int y, int width, int height, int thickness, Color color)
    {
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, width, thickness), color);
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y + height - thickness, width, thickness), color);
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, thickness, height), color);
        spriteBatch.Draw(_pixelTexture, new Rectangle(x + width - thickness, y, thickness, height), color);
    }
}
