using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using Silk.NET.Maths;

namespace MoonBrookEngine.Core;

/// <summary>
/// Core game engine class - manages window, rendering context, and game loop
/// </summary>
public class Engine : IDisposable
{
    private IWindow _window;
    private GL? _gl;
    private IInputContext? _inputContext;
    private Input.InputManager? _inputManager;
    private Audio.AudioEngine? _audioEngine;
    private Audio.MusicPlayer? _musicPlayer;
    private Scene.SceneManager? _sceneManager;
    private bool _isRunning;
    private double _totalTime;
    private PerformanceMonitor _performanceMonitor;
    private Logger _logger;
    private DebugOverlay _debugOverlay;
    private ObjectPoolManager _poolManager;
    
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string Title { get; private set; }
    public GL GL => _gl ?? throw new InvalidOperationException("OpenGL context not initialized");
    public IInputContext Input => _inputContext ?? throw new InvalidOperationException("Input context not initialized");
    public Input.InputManager InputManager => _inputManager ?? throw new InvalidOperationException("Input manager not initialized");
    public Audio.AudioEngine? AudioEngine => _audioEngine;
    public Audio.MusicPlayer? MusicPlayer => _musicPlayer;
    public PerformanceMonitor Performance => _performanceMonitor;
    public Scene.SceneManager? SceneManager => _sceneManager;
    public Logger Logger => _logger;
    public DebugOverlay DebugOverlay => _debugOverlay;
    public ObjectPoolManager PoolManager => _poolManager;
    
    // Events for derived classes or game logic
    public event Action? OnInitialize;
    public event Action<GameTime>? OnUpdate;
    public event Action<GameTime>? OnRender;
    public event Action? OnShutdown;
    
    public Engine(string title, int width, int height)
    {
        Title = title;
        Width = width;
        Height = height;
        _totalTime = 0.0;
        _performanceMonitor = new PerformanceMonitor();
        _logger = LoggerFactory.GetLogger("Engine");
        _debugOverlay = new DebugOverlay(_performanceMonitor);
        _poolManager = new ObjectPoolManager();
        
        _logger.Info($"Initializing MoonBrook Engine: {title} ({width}x{height})");
        
        var options = WindowOptions.Default;
        options.Size = new Vector2D<int>(width, height);
        options.Title = title;
        options.VSync = true;
        options.PreferredDepthBufferBits = 24;
        
        _window = Window.Create(options);
        
        _window.Load += OnLoad;
        _window.Update += OnUpdateInternal;
        _window.Render += OnRenderInternal;
        _window.Closing += OnClosing;
        _window.Resize += OnResize;
    }
    
    public void Run()
    {
        _isRunning = true;
        _window.Run();
    }
    
    public void Stop()
    {
        _isRunning = false;
        _window?.Close();
    }
    
    private void OnLoad()
    {
        _logger.Info("Initializing OpenGL context");
        
        // Initialize OpenGL
        _gl = _window.CreateOpenGL();
        _gl.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
        _gl.Enable(EnableCap.Blend);
        _gl.BlendFunc(BlendingFactor.SrcAlpha, BlendingFactor.OneMinusSrcAlpha);
        
        _logger.Info($"OpenGL Version: {_gl.GetStringS(StringName.Version)}");
        _logger.Info($"OpenGL Renderer: {_gl.GetStringS(StringName.Renderer)}");
        
        // Initialize input
        _inputContext = _window.CreateInput();
        _inputManager = new Input.InputManager(_inputContext);
        _logger.Info("Input system initialized");
        
        // Initialize audio (optional - may fail on headless systems)
        _audioEngine = new Audio.AudioEngine();
        if (!_audioEngine.Initialize())
        {
            _logger.Warning("Audio engine initialization failed - audio will be disabled");
            _audioEngine = null;
        }
        else
        {
            _logger.Info("Audio engine initialized");
            _musicPlayer = new Audio.MusicPlayer(_audioEngine);
            _logger.Info("Music player initialized");
        }
        
        // Initialize scene manager (optional - for state-based games)
        _sceneManager = new Scene.SceneManager(_gl);
        _logger.Info("Scene manager initialized");
        
        _logger.Info("✅ MoonBrook Engine ready");
        
        // Call initialization hook
        OnInitialize?.Invoke();
    }
    
    private void OnUpdateInternal(double deltaTime)
    {
        if (!_isRunning) return;
        
        _performanceMonitor.BeginUpdate();
        
        // Update input state
        _inputManager?.Update();
        
        // Update music player
        _musicPlayer?.Update();
        
        _totalTime += deltaTime;
        
        var gameTime = new GameTime(_totalTime, deltaTime);
        
        // Update scene manager if it's being used
        _sceneManager?.Update(gameTime);
        
        OnUpdate?.Invoke(gameTime);
        
        _performanceMonitor.EndUpdate();
    }
    
    private void OnRenderInternal(double deltaTime)
    {
        if (!_isRunning || _gl == null) return;
        
        _performanceMonitor.BeginRender();
        
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        var gameTime = new GameTime(_totalTime, deltaTime);
        
        // Render scene manager if it's being used
        _sceneManager?.Render(gameTime);
        
        OnRender?.Invoke(gameTime);
        
        _performanceMonitor.EndRender();
        _performanceMonitor.EndFrame();
    }
    
    private void OnResize(Vector2D<int> newSize)
    {
        Width = newSize.X;
        Height = newSize.Y;
        
        if (_gl != null)
        {
            _gl.Viewport(0, 0, (uint)Width, (uint)Height);
        }
    }
    
    private void OnClosing()
    {
        OnShutdown?.Invoke();
        Dispose();
    }
    
    public void Dispose()
    {
        _sceneManager?.Dispose();
        _musicPlayer?.Dispose();
        _audioEngine?.Dispose();
        _inputContext?.Dispose();
        _gl?.Dispose();
        _window?.Dispose();
    }
}
