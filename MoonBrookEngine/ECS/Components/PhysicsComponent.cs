using System.Numerics;

namespace MoonBrookEngine.ECS.Components;

/// <summary>
/// Component that defines physics properties for an entity
/// </summary>
public class PhysicsComponent : Component
{
    /// <summary>
    /// Mass of the entity (affects force application)
    /// </summary>
    public float Mass { get; set; }
    
    /// <summary>
    /// Drag coefficient (0-1, how much velocity is reduced per second)
    /// 0 = no drag, 1 = stops immediately
    /// </summary>
    public float Drag { get; set; }
    
    /// <summary>
    /// Gravity scale multiplier (1.0 = normal gravity, 0 = no gravity)
    /// </summary>
    public float GravityScale { get; set; }
    
    /// <summary>
    /// Bounciness/restitution (0-1, how much velocity is retained on collision)
    /// 0 = no bounce, 1 = perfect bounce
    /// </summary>
    public float Restitution { get; set; }
    
    /// <summary>
    /// Whether this entity is static (not affected by forces)
    /// </summary>
    public bool IsStatic { get; set; }
    
    /// <summary>
    /// Accumulated forces to be applied this frame
    /// </summary>
    public Vector2 AccumulatedForce { get; set; }
    
    public PhysicsComponent()
    {
        Mass = 1.0f;
        Drag = 0.0f;
        GravityScale = 1.0f;
        Restitution = 0.5f;
        IsStatic = false;
        AccumulatedForce = Vector2.Zero;
    }
    
    public PhysicsComponent(float mass, float drag = 0.0f, float gravityScale = 1.0f)
    {
        Mass = mass;
        Drag = drag;
        GravityScale = gravityScale;
        Restitution = 0.5f;
        IsStatic = false;
        AccumulatedForce = Vector2.Zero;
    }
    
    /// <summary>
    /// Apply a force to this entity (will be accumulated and applied in physics update)
    /// </summary>
    public void ApplyForce(Vector2 force)
    {
        if (!IsStatic)
        {
            AccumulatedForce += force;
        }
    }
    
    /// <summary>
    /// Apply an impulse (instant change in velocity)
    /// </summary>
    public void ApplyImpulse(Vector2 impulse, VelocityComponent? velocity)
    {
        if (!IsStatic && velocity != null)
        {
            velocity.Velocity += impulse / Mass;
        }
    }
    
    /// <summary>
    /// Clear accumulated forces (called after physics update)
    /// </summary>
    public void ClearForces()
    {
        AccumulatedForce = Vector2.Zero;
    }
}
