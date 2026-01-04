using MoonBrookEngine.Core;
using MoonBrookEngine.Test;

namespace MoonBrookEngine.Test;

/// <summary>
/// Physics demo application
/// </summary>
class ProgramPhysicsDemo
{
    static void Main(string[] args)
    {
        Console.WriteLine("MoonBrook Engine - Physics System Demo");
        Console.WriteLine("======================================");
        
        var engine = new Engine("MoonBrook Engine - Physics Demo", 1280, 720);
        
        // Initialize needs to be done before accessing Input
        engine.OnInitialize += () =>
        {
            // Create and add the physics test scene
            var physicsScene = new PhysicsTestScene(engine.GL, engine.Input);
            engine.SceneManager?.AddScene("Physics", physicsScene);
            engine.SceneManager?.ChangeScene("Physics", immediate: true);
        };
        
        Console.WriteLine("Starting engine...");
        engine.Run();
        
        Console.WriteLine("Engine stopped.");
    }
}
