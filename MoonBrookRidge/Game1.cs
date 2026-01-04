using System;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Core.States;
using MoonBrookRidge.Core.Systems;

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
    private AudioManager _audioManager;
    private AchievementSystem _achievementSystem;
    private GameSettings _settings;

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
        
        // Load settings (Phase 7.4)
        _settings = GameSettings.Instance;
        
        // Apply display settings
        _graphics.PreferredBackBufferWidth = _settings.ResolutionWidth;
        _graphics.PreferredBackBufferHeight = _settings.ResolutionHeight;
        _graphics.IsFullScreen = _settings.IsFullscreen;
        _graphics.ApplyChanges();
        
        // Allow window resizing (Phase 7.4)
        Window.AllowUserResizing = true;
        Window.ClientSizeChanged += OnWindowSizeChanged;
        
        Window.Title = "MoonBrook Ridge";
    }

    protected override void Initialize()
    {
        // Initialize audio system
        _audioManager = new AudioManager();
        AudioHelper.Initialize(_audioManager);
        
        // Apply audio settings from saved preferences (Phase 7.4)
        _audioManager.MusicVolume = _settings.MusicVolume;
        _audioManager.SfxVolume = _settings.SfxVolume;
        _audioManager.IsMusicEnabled = _settings.IsMusicEnabled;
        _audioManager.AreSfxEnabled = _settings.AreSfxEnabled;
        
        // Initialize achievement system
        _achievementSystem = new AchievementSystem();
        
        // Initialize state manager
        _stateManager = new StateManager(this);
        
        base.Initialize();
    }
    
    /// <summary>
    /// Handle window size changes (Phase 7.4)
    /// </summary>
    private void OnWindowSizeChanged(object? sender, EventArgs e)
    {
        // Update resolution in settings when window is resized
        if (!_settings.IsFullscreen)
        {
            _settings.ResolutionWidth = Window.ClientBounds.Width;
            _settings.ResolutionHeight = Window.ClientBounds.Height;
        }
    }
    
    /// <summary>
    /// Apply display settings (resolution, fullscreen, etc.) - Phase 7.4
    /// </summary>
    public void ApplyDisplaySettings(int width, int height, bool fullscreen, bool borderless)
    {
        _graphics.PreferredBackBufferWidth = width;
        _graphics.PreferredBackBufferHeight = height;
        _graphics.IsFullScreen = fullscreen;
        _graphics.HardwareModeSwitch = !borderless; // Borderless uses software mode
        _graphics.ApplyChanges();
        
        // Update settings
        _settings.ResolutionWidth = width;
        _settings.ResolutionHeight = height;
        _settings.IsFullscreen = fullscreen;
        _settings.IsBorderless = borderless;
        _settings.Save();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Load default font
        _defaultFont = Content.Load<SpriteFont>("Fonts/Default");
        
        // Start with menu state
        _stateManager.ChangeState(new MenuState(this));
    }

    protected override void Update(GameTime gameTime)
    {
        // Note: Back button on gamepad can still be used to exit if desired
        // Removed ESC key exit - ESC should only close menus/pause, not exit game
        
        // Update current game state
        _stateManager?.Update(gameTime);

        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        // Use a bright, vibrant green background matching Sunnyside style
        GraphicsDevice.Clear(new Color(120, 195, 85)); // Bright grass green

        // Draw current game state
        _stateManager?.Draw(_spriteBatch);

        base.Draw(gameTime);
    }
    
    public SpriteBatch SpriteBatch => _spriteBatch;
    public SpriteFont DefaultFont => _defaultFont;
    public StateManager StateManager => _stateManager;
    public AudioManager AudioManager => _audioManager;
    public AchievementSystem AchievementSystem => _achievementSystem;
    public GraphicsDeviceManager Graphics => _graphics;
    public GameSettings Settings => _settings;
    public float UIScale => _settings.UIScale;
}
