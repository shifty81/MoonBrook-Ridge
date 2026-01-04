using MoonBrookEngine.Core;
using Silk.NET.OpenGL;

namespace MoonBrookEngine.Scene;

/// <summary>
/// Manages scene transitions and lifecycle.
/// Only one scene can be active at a time.
/// </summary>
public class SceneManager
{
    private readonly GL _gl;
    private readonly Dictionary<string, Scene> _scenes;
    private Scene? _currentScene;
    private Scene? _nextScene;
    private bool _isTransitioning;
    
    public Scene? CurrentScene => _currentScene;
    public bool IsTransitioning => _isTransitioning;
    
    public SceneManager(GL gl)
    {
        _gl = gl;
        _scenes = new Dictionary<string, Scene>();
        _currentScene = null;
        _nextScene = null;
        _isTransitioning = false;
    }
    
    /// <summary>
    /// Register a scene with the manager
    /// </summary>
    public void AddScene(string name, Scene scene)
    {
        if (_scenes.ContainsKey(name))
        {
            Console.WriteLine($"Warning: Scene '{name}' already exists. Replacing...");
            _scenes[name].Dispose();
        }
        
        _scenes[name] = scene;
        scene.Initialize();
    }
    
    /// <summary>
    /// Remove a scene from the manager
    /// </summary>
    public void RemoveScene(string name)
    {
        if (_scenes.TryGetValue(name, out var scene))
        {
            if (_currentScene == scene)
            {
                Console.WriteLine($"Warning: Cannot remove active scene '{name}'");
                return;
            }
            
            scene.Dispose();
            _scenes.Remove(name);
        }
    }
    
    /// <summary>
    /// Switch to a different scene
    /// </summary>
    /// <param name="name">Name of the scene to switch to</param>
    /// <param name="immediate">If true, transition happens immediately. If false, on next Update()</param>
    public void ChangeScene(string name, bool immediate = false)
    {
        if (!_scenes.TryGetValue(name, out var scene))
        {
            Console.WriteLine($"Error: Scene '{name}' not found!");
            return;
        }
        
        if (_currentScene == scene)
        {
            Console.WriteLine($"Scene '{name}' is already active");
            return;
        }
        
        if (immediate)
        {
            PerformTransition(scene);
        }
        else
        {
            _nextScene = scene;
            _isTransitioning = true;
        }
    }
    
    /// <summary>
    /// Get a scene by name (without activating it)
    /// </summary>
    public Scene? GetScene(string name)
    {
        _scenes.TryGetValue(name, out var scene);
        return scene;
    }
    
    /// <summary>
    /// Update the current scene
    /// </summary>
    public void Update(GameTime gameTime)
    {
        // Handle pending scene transition
        if (_isTransitioning && _nextScene != null)
        {
            PerformTransition(_nextScene);
            _nextScene = null;
            _isTransitioning = false;
        }
        
        // Update active scene
        _currentScene?.Update(gameTime);
    }
    
    /// <summary>
    /// Render the current scene
    /// </summary>
    public void Render(GameTime gameTime)
    {
        _currentScene?.Render(gameTime);
    }
    
    /// <summary>
    /// Dispose all scenes
    /// </summary>
    public void Dispose()
    {
        _currentScene?.OnExit();
        
        foreach (var scene in _scenes.Values)
        {
            scene.Dispose();
        }
        
        _scenes.Clear();
        _currentScene = null;
        _nextScene = null;
    }
    
    private void PerformTransition(Scene newScene)
    {
        // Exit current scene
        _currentScene?.OnExit();
        
        // Switch to new scene
        _currentScene = newScene;
        
        // Enter new scene
        _currentScene.OnEnter();
        
        Console.WriteLine($"Scene changed to: {newScene.Name}");
    }
}
