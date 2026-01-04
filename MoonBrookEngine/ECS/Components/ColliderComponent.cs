using MoonBrookEngine.Physics;

namespace MoonBrookEngine.ECS.Components;

/// <summary>
/// Collider component for collision detection
/// </summary>
public class ColliderComponent : Component
{
    public CollisionShape Shape { get; set; }
    public bool IsTrigger { get; set; } // If true, doesn't block movement but fires collision events
    public string Tag { get; set; } // For collision filtering
    
    public ColliderComponent(CollisionShape shape)
    {
        Shape = shape;
        IsTrigger = false;
        Tag = "Default";
    }
}
