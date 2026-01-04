using System.Numerics;

namespace MoonBrookEngine.Physics;

/// <summary>
/// Base class for collision shapes
/// </summary>
public abstract class CollisionShape
{
    public Vector2 Offset { get; set; }
    
    protected CollisionShape()
    {
        Offset = Vector2.Zero;
    }
    
    /// <summary>
    /// Check if this shape intersects with another shape
    /// </summary>
    public abstract bool Intersects(CollisionShape other, Vector2 thisPosition, Vector2 otherPosition);
    
    /// <summary>
    /// Get the bounding box of this shape (for broad phase)
    /// </summary>
    public abstract MoonBrookEngine.Math.Rectangle GetBounds(Vector2 position);
}

/// <summary>
/// Rectangle collision shape
/// </summary>
public class RectangleCollisionShape : CollisionShape
{
    public float Width { get; set; }
    public float Height { get; set; }
    
    public RectangleCollisionShape(float width, float height)
    {
        Width = width;
        Height = height;
    }
    
    public override bool Intersects(CollisionShape other, Vector2 thisPosition, Vector2 otherPosition)
    {
        if (other is RectangleCollisionShape rect)
        {
            var thisBounds = GetBounds(thisPosition);
            var otherBounds = rect.GetBounds(otherPosition);
            return thisBounds.Intersects(otherBounds);
        }
        else if (other is CircleCollisionShape circle)
        {
            return circle.Intersects(this, otherPosition, thisPosition);
        }
        
        return false;
    }
    
    public override MoonBrookEngine.Math.Rectangle GetBounds(Vector2 position)
    {
        return new MoonBrookEngine.Math.Rectangle(
            (int)(position.X + Offset.X),
            (int)(position.Y + Offset.Y),
            (int)Width,
            (int)Height
        );
    }
}

/// <summary>
/// Circle collision shape
/// </summary>
public class CircleCollisionShape : CollisionShape
{
    public float Radius { get; set; }
    
    public CircleCollisionShape(float radius)
    {
        Radius = radius;
    }
    
    public override bool Intersects(CollisionShape other, Vector2 thisPosition, Vector2 otherPosition)
    {
        if (other is CircleCollisionShape circle)
        {
            var thisCenter = thisPosition + Offset;
            var otherCenter = otherPosition + circle.Offset;
            var distance = Vector2.Distance(thisCenter, otherCenter);
            return distance < (Radius + circle.Radius);
        }
        else if (other is RectangleCollisionShape rect)
        {
            // Circle-rectangle collision
            var circleCenter = thisPosition + Offset;
            var rectBounds = rect.GetBounds(otherPosition);
            
            // Find closest point on rectangle to circle
            var closestX = System.Math.Clamp(circleCenter.X, rectBounds.X, rectBounds.X + rectBounds.Width);
            var closestY = System.Math.Clamp(circleCenter.Y, rectBounds.Y, rectBounds.Y + rectBounds.Height);
            var closest = new Vector2(closestX, closestY);
            
            var distance = Vector2.Distance(circleCenter, closest);
            return distance < Radius;
        }
        
        return false;
    }
    
    public override MoonBrookEngine.Math.Rectangle GetBounds(Vector2 position)
    {
        var center = position + Offset;
        return new MoonBrookEngine.Math.Rectangle(
            (int)(center.X - Radius),
            (int)(center.Y - Radius),
            (int)(Radius * 2),
            (int)(Radius * 2)
        );
    }
}
