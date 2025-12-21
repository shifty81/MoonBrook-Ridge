using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Core.States;

namespace MoonBrookRidge;

/// <summary>
/// MoonBrook Ridge - A farming/life simulation game inspired by Stardew Valley with enhanced NPC interactions
/// </summary>
public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private StateManager _stateManager;
    private SpriteFont _defaultFont;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        // Set window size
        _graphics.PreferredBackBufferWidth = 1280;
        _graphics.PreferredBackBufferHeight = 720;
        _graphics.ApplyChanges();
        
        Window.Title = "MoonBrook Ridge";
    }

    protected override void Initialize()
    {
        // Initialize state manager
        _stateManager = new StateManager(this);
        
        base.Initialize();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Load default font
        _defaultFont = Content.Load<SpriteFont>("Fonts/Default");
        
        // Start with gameplay state
        _stateManager.ChangeState(new GameplayState(this));
    }

    protected override void Update(GameTime gameTime)
    {
        if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || 
            Keyboard.GetState().IsKeyDown(Keys.Escape))
            Exit();

        // Update current game state
        _stateManager?.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(new Color(34, 139, 34)); // Grass green background

        // Draw current game state
        _stateManager?.Draw(_spriteBatch);

        base.Draw(gameTime);
    }
    
    public SpriteBatch SpriteBatch => _spriteBatch;
    public SpriteFont DefaultFont => _defaultFont;
}
