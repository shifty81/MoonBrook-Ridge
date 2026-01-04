using System.Numerics;

namespace MoonBrookEngine.ECS.Components;

/// <summary>
/// Component that represents velocity and acceleration for entity movement
/// </summary>
public class VelocityComponent : Component
{
    /// <summary>
    /// Current velocity in units per second
    /// </summary>
    public Vector2 Velocity { get; set; }
    
    /// <summary>
    /// Current acceleration in units per second squared
    /// </summary>
    public Vector2 Acceleration { get; set; }
    
    /// <summary>
    /// Maximum velocity magnitude (0 = no limit)
    /// </summary>
    public float MaxSpeed { get; set; }
    
    public VelocityComponent()
    {
        Velocity = Vector2.Zero;
        Acceleration = Vector2.Zero;
        MaxSpeed = 0f;
    }
    
    public VelocityComponent(Vector2 velocity)
    {
        Velocity = velocity;
        Acceleration = Vector2.Zero;
        MaxSpeed = 0f;
    }
    
    public VelocityComponent(Vector2 velocity, float maxSpeed)
    {
        Velocity = velocity;
        Acceleration = Vector2.Zero;
        MaxSpeed = maxSpeed;
    }
}
