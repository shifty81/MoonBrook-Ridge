namespace MoonBrookEngine.ECS;

/// <summary>
/// Base class for all components.
/// Components are pure data containers attached to entities.
/// </summary>
public abstract class Component
{
    /// <summary>
    /// The entity this component is attached to
    /// </summary>
    public Entity Owner { get; internal set; }
    
    /// <summary>
    /// Whether this component is currently enabled
    /// </summary>
    public bool Enabled { get; set; } = true;
    
    /// <summary>
    /// Called when the component is first added to an entity
    /// </summary>
    public virtual void OnAttach()
    {
    }
    
    /// <summary>
    /// Called when the component is removed from an entity
    /// </summary>
    public virtual void OnDetach()
    {
    }
}
