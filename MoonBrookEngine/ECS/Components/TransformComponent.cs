using System.Numerics;

namespace MoonBrookEngine.ECS.Components;

/// <summary>
/// Transform component for position, rotation, and scale
/// </summary>
public class TransformComponent : Component
{
    public Vector2 Position { get; set; }
    public float Rotation { get; set; } // Radians
    public Vector2 Scale { get; set; }
    
    public TransformComponent()
    {
        Position = Vector2.Zero;
        Rotation = 0f;
        Scale = Vector2.One;
    }
    
    public TransformComponent(Vector2 position)
    {
        Position = position;
        Rotation = 0f;
        Scale = Vector2.One;
    }
    
    public TransformComponent(Vector2 position, float rotation, Vector2 scale)
    {
        Position = position;
        Rotation = rotation;
        Scale = scale;
    }
}
