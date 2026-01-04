using MoonBrookEngine.Core;
using MoonBrookEngine.Input;
using Silk.NET.Input;

namespace MoonBrookRidge.EngineDemo;

/// <summary>
/// Simple game application that demonstrates the farmhouse scene
/// Shows the player character in the farmhouse interior with basic movement
/// </summary>
public class FarmhouseGame : IDisposable
{
    private MoonBrookEngine.Core.Engine? _engine;
    private FarmhouseScene? _farmhouseScene;
    
    public FarmhouseGame()
    {
    }
    
    public void Run()
    {
        // Create engine
        _engine = new MoonBrookEngine.Core.Engine("MoonBrook Ridge - Farmhouse Scene", 1280, 720);
        
        // Hook up events
        _engine.OnInitialize += Initialize;
        _engine.OnUpdate += Update;
        _engine.OnRender += Render;
        _engine.OnShutdown += Shutdown;
        
        Console.WriteLine("=== MoonBrook Engine - Farmhouse Scene Demo ===");
        Console.WriteLine("This demonstrates the first scene integration:");
        Console.WriteLine("  - Player farmhouse interior");
        Console.WriteLine("  - Tile-based room layout");
        Console.WriteLine("  - Furniture objects");
        Console.WriteLine("  - Player movement with collision");
        Console.WriteLine("  - Camera following");
        Console.WriteLine();
        Console.WriteLine("Controls:");
        Console.WriteLine("  WASD / Arrow Keys - Move player");
        Console.WriteLine("  ESC - Exit");
        Console.WriteLine();
        
        // Run the engine
        _engine.Run();
    }
    
    private void Initialize()
    {
        if (_engine == null) return;
        
        // Create the farmhouse scene
        _farmhouseScene = new FarmhouseScene(_engine.GL, _engine.InputManager);
        
        // Register with scene manager (optional - for multiple scenes)
        if (_engine.SceneManager != null)
        {
            _engine.SceneManager.AddScene("farmhouse", _farmhouseScene);
            _engine.SceneManager.ChangeScene("farmhouse", immediate: true);
        }
        else
        {
            // If no scene manager, initialize directly
            _farmhouseScene.Initialize();
            _farmhouseScene.OnEnter();
        }
        
        Console.WriteLine("✅ Farmhouse game initialized");
    }
    
    private void Update(GameTime gameTime)
    {
        if (_engine == null) return;
        
        // Handle ESC to exit
        if (_engine.InputManager.IsKeyPressed(Key.Escape))
        {
            Console.WriteLine("ESC pressed - exiting...");
            _engine.Stop();
            return;
        }
        
        // Update the scene (either through scene manager or directly)
        if (_engine.SceneManager != null)
        {
            _engine.SceneManager.Update(gameTime);
        }
        else
        {
            _farmhouseScene?.Update(gameTime);
        }
    }
    
    private void Render(GameTime gameTime)
    {
        // Render the scene (either through scene manager or directly)
        if (_engine?.SceneManager != null)
        {
            _engine.SceneManager.Render(gameTime);
        }
        else
        {
            _farmhouseScene?.Render(gameTime);
        }
    }
    
    private void Shutdown()
    {
        Console.WriteLine("Shutting down farmhouse game...");
        _farmhouseScene?.Dispose();
        _farmhouseScene = null;
    }
    
    public void Dispose()
    {
        _farmhouseScene?.Dispose();
        _engine?.Dispose();
    }
}
