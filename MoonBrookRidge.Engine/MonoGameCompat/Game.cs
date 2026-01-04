using MoonBrookEngine.Core;

namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible Game base class for MoonBrookEngine
/// This allows existing MonoGame game code to run on the custom engine with minimal changes
/// </summary>
public abstract class Game : IDisposable
{
    private MoonBrookEngine.Core.Engine? _engine;
    private GraphicsDeviceManager? _graphics;
    private ContentManager? _content;
    private bool _isInitialized;
    private GameTime _gameTime;
    
    public GraphicsDeviceManager Graphics => _graphics ?? throw new InvalidOperationException("Graphics not initialized");
    public GraphicsDevice GraphicsDevice => _graphics?.GraphicsDevice ?? throw new InvalidOperationException("GraphicsDevice not initialized");
    public ContentManager Content => _content ?? throw new InvalidOperationException("Content not initialized");
    
    /// <summary>
    /// Internal engine instance (for compatibility layer use)
    /// </summary>
    internal MoonBrookEngine.Core.Engine? Engine => _engine;
    
    private string _contentRootDirectory = "Content";
    
    /// <summary>
    /// Root directory for content files
    /// </summary>
    public string ContentRootDirectory
    {
        get => _contentRootDirectory;
        set => _contentRootDirectory = value;
    }
    
    /// <summary>
    /// Is the mouse cursor visible?
    /// </summary>
    public bool IsMouseVisible { get; set; } = false;
    
    /// <summary>
    /// Game window
    /// </summary>
    public GameWindow Window { get; private set; }
    
    protected Game()
    {
        Window = new GameWindow();
        _gameTime = new GameTime(0, 0);
        _graphics = new GraphicsDeviceManager(this);
    }
    
    /// <summary>
    /// Run the game
    /// </summary>
    public void Run()
    {
        if (_graphics == null)
            throw new InvalidOperationException("GraphicsDeviceManager must be initialized in the constructor");
        
        // Create engine with configured settings
        _engine = new MoonBrookEngine.Core.Engine(
            Window.Title,
            _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight
        );
        
        // Hook up engine events
        _engine.OnInitialize += OnEngineInitialize;
        _engine.OnUpdate += OnEngineUpdate;
        _engine.OnRender += OnEngineRender;
        _engine.OnShutdown += OnEngineShutdown;
        
        // Run the engine
        _engine.Run();
    }
    
    private void OnEngineInitialize()
    {
        // Create graphics device wrapper
        _graphics!.GraphicsDevice = new GraphicsDevice(
            _engine!.GL,
            _graphics.PreferredBackBufferWidth,
            _graphics.PreferredBackBufferHeight
        );
        
        // Create content manager with audio engine support
        _content = new ContentManager(_engine.GL, _engine.AudioEngine, _contentRootDirectory);
        
        // Initialize static input classes
        Keyboard.Initialize(this);
        Mouse.Initialize(this);
        
        // Call game initialization
        if (!_isInitialized)
        {
            Initialize();
            _isInitialized = true;
        }
        
        // Load content
        LoadContent();
    }
    
    private void OnEngineUpdate(MoonBrookEngine.Core.GameTime engineGameTime)
    {
        // Convert engine GameTime to MonoGame-compatible GameTime
        _gameTime = new GameTime(engineGameTime.TotalSeconds, engineGameTime.DeltaTime);
        
        // Call game update
        Update(_gameTime);
    }
    
    private void OnEngineRender(MoonBrookEngine.Core.GameTime engineGameTime)
    {
        // Call game draw
        Draw(_gameTime);
    }
    
    private void OnEngineShutdown()
    {
        // Call game unload
        UnloadContent();
    }
    
    /// <summary>
    /// Initialize game (called once before LoadContent)
    /// </summary>
    protected virtual void Initialize()
    {
    }
    
    /// <summary>
    /// Load game content (called once after Initialize)
    /// </summary>
    protected virtual void LoadContent()
    {
    }
    
    /// <summary>
    /// Unload game content (called when exiting)
    /// </summary>
    protected virtual void UnloadContent()
    {
    }
    
    /// <summary>
    /// Update game logic (called every frame)
    /// </summary>
    protected virtual void Update(GameTime gameTime)
    {
    }
    
    /// <summary>
    /// Draw game (called every frame)
    /// </summary>
    protected virtual void Draw(GameTime gameTime)
    {
    }
    
    /// <summary>
    /// Exit the game
    /// </summary>
    public void Exit()
    {
        _engine?.Stop();
    }
    
    /// <summary>
    /// Apply graphics settings changes (called by GraphicsDeviceManager.ApplyChanges)
    /// </summary>
    internal void ApplyGraphicsChanges()
    {
        if (_engine != null && _graphics != null)
        {
            // TODO: Implement dynamic resolution/fullscreen changes
            // For now, settings are only applied at startup
        }
    }
    
    public void Dispose()
    {
        _content?.Dispose();
        _engine?.Dispose();
    }
}

/// <summary>
/// MonoGame-compatible GameTime class
/// </summary>
public class GameTime
{
    public TimeSpan TotalGameTime { get; }
    public TimeSpan ElapsedGameTime { get; }
    
    public GameTime(double totalSeconds, double elapsedSeconds)
    {
        TotalGameTime = TimeSpan.FromSeconds(totalSeconds);
        ElapsedGameTime = TimeSpan.FromSeconds(elapsedSeconds);
    }
    
    public GameTime(TimeSpan totalGameTime, TimeSpan elapsedGameTime)
    {
        TotalGameTime = totalGameTime;
        ElapsedGameTime = elapsedGameTime;
    }
}

/// <summary>
/// MonoGame-compatible GameWindow class
/// </summary>
public class GameWindow
{
    private string _title = "MoonBrook Ridge";
    private bool _allowUserResizing = false;
    
    public string Title
    {
        get => _title;
        set => _title = value;
    }
    
    public bool AllowUserResizing
    {
        get => _allowUserResizing;
        set => _allowUserResizing = value;
    }
    
    public Rectangle ClientBounds { get; internal set; } = new Rectangle(0, 0, 1280, 720);
    
    // Events
    public event EventHandler? ClientSizeChanged;
    
    internal void OnClientSizeChanged()
    {
        ClientSizeChanged?.Invoke(this, EventArgs.Empty);
    }
}
