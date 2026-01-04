using MoonBrookEngine.Core;
using Silk.NET.OpenGL;

namespace MoonBrookEngine.Scene;

/// <summary>
/// Base class for all game scenes (states).
/// Similar to MonoGame's GameState pattern.
/// </summary>
public abstract class Scene
{
    /// <summary>
    /// Reference to the GL context
    /// </summary>
    protected GL GL { get; private set; }
    
    /// <summary>
    /// Whether this scene is currently active
    /// </summary>
    public bool IsActive { get; internal set; }
    
    /// <summary>
    /// Name of this scene for debugging
    /// </summary>
    public string Name { get; protected set; }
    
    protected Scene(GL gl, string name)
    {
        GL = gl;
        Name = name;
        IsActive = false;
    }
    
    /// <summary>
    /// Called once when the scene is first created
    /// </summary>
    public virtual void Initialize()
    {
    }
    
    /// <summary>
    /// Called when the scene becomes active
    /// </summary>
    public virtual void OnEnter()
    {
        IsActive = true;
    }
    
    /// <summary>
    /// Called when the scene becomes inactive
    /// </summary>
    public virtual void OnExit()
    {
        IsActive = false;
    }
    
    /// <summary>
    /// Update scene logic
    /// </summary>
    /// <param name="gameTime">Game timing information</param>
    public virtual void Update(GameTime gameTime)
    {
    }
    
    /// <summary>
    /// Render the scene
    /// </summary>
    /// <param name="gameTime">Game timing information</param>
    public virtual void Render(GameTime gameTime)
    {
    }
    
    /// <summary>
    /// Called when the scene is being destroyed
    /// </summary>
    public virtual void Dispose()
    {
    }
}
