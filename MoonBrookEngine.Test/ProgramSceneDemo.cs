using MoonBrookEngine.Core;

namespace MoonBrookEngine.Test;

/// <summary>
/// Demo showcasing Scene Management System and ECS
/// </summary>
public class SceneDemoGame
{
    private Engine _engine;
    
    public SceneDemoGame()
    {
        _engine = new Engine("MoonBrook Engine - Scene & ECS Demo", 1280, 720);
        _engine.OnInitialize += Initialize;
        _engine.OnShutdown += Shutdown;
    }
    
    public void Run()
    {
        _engine.Run();
    }
    
    private void Initialize()
    {
        Console.WriteLine("=== MoonBrook Engine - Scene & ECS Demo ===");
        Console.WriteLine();
        Console.WriteLine("This demo showcases:");
        Console.WriteLine("  1. Scene Management System");
        Console.WriteLine("  2. Entity Component System (ECS)");
        Console.WriteLine("  3. Collision Detection with Spatial Partitioning");
        Console.WriteLine();
        Console.WriteLine("Features:");
        Console.WriteLine("  - 50 bouncing entities with circle collision");
        Console.WriteLine("  - Quadtree spatial partitioning for O(n log n) collision");
        Console.WriteLine("  - Entities change color on collision");
        Console.WriteLine();
        
        // Create and register the ECS test scene
        var ecsScene = new EcsTestScene(_engine.GL, _engine.Input);
        _engine.SceneManager?.AddScene("ECS Test", ecsScene);
        
        // Switch to the scene
        _engine.SceneManager?.ChangeScene("ECS Test", immediate: true);
        
        Console.WriteLine("Scene initialized. Press ESC to exit, Space to add entities, C to clear.");
        Console.WriteLine();
    }
    
    private void Shutdown()
    {
        Console.WriteLine("Engine shutting down...");
    }
}

// Alternative entry point for scene demo
// Uncomment this and comment the TestGame.Main() to run the scene demo
/*
class Program
{
    static void Main()
    {
        using var game = new SceneDemoGame();
        game.Run();
    }
}
*/
