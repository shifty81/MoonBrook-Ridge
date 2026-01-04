namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible GraphicsDeviceManager wrapper for MoonBrookEngine
/// </summary>
public class GraphicsDeviceManager
{
    private Game _game;
    private int _preferredBackBufferWidth = 1280;
    private int _preferredBackBufferHeight = 720;
    private bool _isFullScreen = false;
    private bool _hardwareModeSwitch = true;
    
    public int PreferredBackBufferWidth
    {
        get => _preferredBackBufferWidth;
        set => _preferredBackBufferWidth = value;
    }
    
    public int PreferredBackBufferHeight
    {
        get => _preferredBackBufferHeight;
        set => _preferredBackBufferHeight = value;
    }
    
    public bool IsFullScreen
    {
        get => _isFullScreen;
        set => _isFullScreen = value;
    }
    
    public bool HardwareModeSwitch
    {
        get => _hardwareModeSwitch;
        set => _hardwareModeSwitch = value;
    }
    
    public GraphicsDevice? GraphicsDevice { get; internal set; }
    
    public GraphicsDeviceManager(Game game)
    {
        _game = game;
    }
    
    /// <summary>
    /// Apply graphics settings changes
    /// </summary>
    public void ApplyChanges()
    {
        // Settings will be applied by the Game class
        // This is called by user code to trigger changes
        _game.ApplyGraphicsChanges();
    }
}
