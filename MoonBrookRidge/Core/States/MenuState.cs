using System;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.Core.States;

/// <summary>
/// Main menu state - shown at game start
/// </summary>
public class MenuState : GameState
{
    private InputManager _inputManager;
    private int _selectedOption;
    private string[] _menuOptions;
    private Color _selectedColor = Color.Yellow;
    private Color _normalColor = Color.White;
    private Texture2D _pixelTexture; // Cache pixel texture
    
    public MenuState(Game1 game) : base(game) 
    {
        _menuOptions = new string[]
        {
            "New Game",
            "Continue",
            "Load Game",
            "Settings",
            "Mods",
            "Exit"
        };
        _selectedOption = 0;
    }
    
    public override void Initialize()
    {
        base.Initialize();
        _inputManager = new InputManager();
        Console.WriteLine("=== MenuState Initialized ===");
    }
    
    public override void LoadContent()
    {
        base.LoadContent();
        // Create and cache pixel texture
        _pixelTexture = CreatePixelTexture();
        Console.WriteLine("=== MenuState Content Loaded ===");
        Console.WriteLine($"Pixel texture created: {_pixelTexture != null}");
    }
    
    public override void Update(GameTime gameTime)
    {
        _inputManager.Update();
        
        // Mouse support for menu
        MouseState mouseState = Mouse.GetState();
        Vector2 mousePos = new Vector2(mouseState.X, mouseState.Y);
        
        // Check if mouse is hovering over any menu option
        Vector2 menuStart = new Vector2(
            Game.GraphicsDevice.Viewport.Width / 2,
            300
        );
        
        for (int i = 0; i < _menuOptions.Length; i++)
        {
            if (Game.DefaultFont != null)
            {
                string option = _menuOptions[i];
                Vector2 optionSize = Game.DefaultFont.MeasureString(option);
                Vector2 optionPosition = menuStart + new Vector2(-optionSize.X / 2, i * 50);
                
                Rectangle optionBounds = new Rectangle(
                    (int)optionPosition.X - 10,
                    (int)optionPosition.Y,
                    (int)optionSize.X + 20,
                    (int)optionSize.Y
                );
                
                if (optionBounds.Contains(mousePos))
                {
                    _selectedOption = i;
                    
                    // Click to select
                    if (mouseState.LeftButton == ButtonState.Pressed)
                    {
                        HandleSelection();
                        return;
                    }
                }
            }
        }
        
        // Handle menu navigation with keyboard
        if (_inputManager.IsMoveUpPressed())
        {
            _selectedOption--;
            if (_selectedOption < 0)
            {
                _selectedOption = _menuOptions.Length - 1;
            }
        }
        
        if (_inputManager.IsMoveDownPressed())
        {
            _selectedOption++;
            if (_selectedOption >= _menuOptions.Length)
            {
                _selectedOption = 0;
            }
        }
        
        // Handle selection with keyboard
        if (_inputManager.IsUseToolPressed() || _inputManager.IsDoActionPressed())
        {
            HandleSelection();
        }
    }
    
    private void HandleSelection()
    {
        switch (_selectedOption)
        {
            case 0: // New Game
                Game.StateManager.ChangeState(new GameplayState(Game));
                break;
            case 1: // Continue (last save)
                // TODO: Load most recent save
                Game.StateManager.ChangeState(new GameplayState(Game));
                break;
            case 2: // Load Game
                // TODO: Implement save/load selection menu
                break;
            case 3: // Settings
                // TODO: Implement settings menu
                break;
            case 4: // Mods
                Game.StateManager.ChangeState(new ModsMenuState(Game));
                break;
            case 5: // Exit
                Game.Exit();
                break;
        }
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        // Clear screen with a nice background color
        Game.GraphicsDevice.Clear(new Color(20, 30, 50));
        
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        
        // Try to draw with font, but fall back to simple menu if font is not working
        bool fontWorking = Game.DefaultFont != null;
        
        // Check if font can actually render (has internal font)
        if (fontWorking)
        {
            // Test if font can measure strings - if it returns 0 for a non-empty string, font is broken
            Vector2 testSize = Game.DefaultFont.MeasureString("Test");
            if (testSize.X <= 0 || testSize.Y <= 0)
            {
                Console.WriteLine($"WARNING: Font measurement failed - using fallback rendering (testSize: {testSize.X}x{testSize.Y})");
                fontWorking = false;
            }
        }
        else
        {
            Console.WriteLine("WARNING: No default font available - using fallback rendering");
        }
        
        if (fontWorking)
        {
            string title = "MoonBrook Ridge";
            Vector2 titleSize = Game.DefaultFont.MeasureString(title);
            Vector2 titlePosition = new Vector2(
                (Game.GraphicsDevice.Viewport.Width - titleSize.X) / 2,
                100
            );
            
            // Draw title with shadow
            spriteBatch.DrawString(Game.DefaultFont, title, titlePosition + new Vector2(2, 2), Color.Black);
            spriteBatch.DrawString(Game.DefaultFont, title, titlePosition, Color.LightBlue);
            
            // Draw menu options
            Vector2 menuStart = new Vector2(
                Game.GraphicsDevice.Viewport.Width / 2,
                300
            );
            
            for (int i = 0; i < _menuOptions.Length; i++)
            {
                string option = _menuOptions[i];
                Vector2 optionSize = Game.DefaultFont.MeasureString(option);
                Vector2 optionPosition = menuStart + new Vector2(-optionSize.X / 2, i * 50);
                
                Color color = (i == _selectedOption) ? _selectedColor : _normalColor;
                
                // Draw selection indicator
                if (i == _selectedOption)
                {
                    string arrow = "> ";
                    Vector2 arrowSize = Game.DefaultFont.MeasureString(arrow);
                    spriteBatch.DrawString(Game.DefaultFont, arrow, 
                        optionPosition - new Vector2(arrowSize.X, 0), _selectedColor);
                }
                
                // Draw option with shadow
                spriteBatch.DrawString(Game.DefaultFont, option, optionPosition + new Vector2(1, 1), Color.Black);
                spriteBatch.DrawString(Game.DefaultFont, option, optionPosition, color);
            }
            
            // Draw controls hint at bottom
            string hint = "Arrow Keys/Mouse: Navigate | C/X/Click: Select | ESC: Exit";
            Vector2 hintSize = Game.DefaultFont.MeasureString(hint);
            Vector2 hintPosition = new Vector2(
                (Game.GraphicsDevice.Viewport.Width - hintSize.X) / 2,
                Game.GraphicsDevice.Viewport.Height - 50
            );
            spriteBatch.DrawString(Game.DefaultFont, hint, hintPosition, Color.Gray);
        }
        else
        {
            // Fallback if font not loaded or not working
            Console.WriteLine("Using simple menu fallback rendering");
            DrawSimpleMenu(spriteBatch);
        }
        
        spriteBatch.End();
    }
    
    private void DrawSimpleMenu(SpriteBatch spriteBatch)
    {
        // Simple colored rectangles as menu items if font is not available
        // Draw title bar
        Rectangle titleRect = new Rectangle(
            Game.GraphicsDevice.Viewport.Width / 2 - 200,
            80,
            400,
            50
        );
        spriteBatch.Draw(_pixelTexture, titleRect, Color.LightBlue * 0.8f);
        
        // Draw menu option rectangles
        for (int i = 0; i < _menuOptions.Length; i++)
        {
            Color color = (i == _selectedOption) ? _selectedColor : _normalColor;
            Rectangle rect = new Rectangle(
                Game.GraphicsDevice.Viewport.Width / 2 - 150,
                200 + i * 60,
                300,
                45
            );
            
            // Draw border for selected item
            if (i == _selectedOption)
            {
                Rectangle borderRect = new Rectangle(rect.X - 5, rect.Y - 5, rect.Width + 10, rect.Height + 10);
                spriteBatch.Draw(_pixelTexture, borderRect, Color.Yellow * 0.5f);
            }
            
            spriteBatch.Draw(_pixelTexture, rect, color * 0.7f);
        }
        
        // Draw hint at bottom
        Rectangle hintRect = new Rectangle(
            Game.GraphicsDevice.Viewport.Width / 2 - 250,
            Game.GraphicsDevice.Viewport.Height - 60,
            500,
            30
        );
        spriteBatch.Draw(_pixelTexture, hintRect, Color.Gray * 0.5f);
    }
    
    private Texture2D CreatePixelTexture()
    {
        Texture2D texture = new Texture2D(Game.GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
}
