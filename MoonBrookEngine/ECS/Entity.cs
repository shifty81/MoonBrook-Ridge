namespace MoonBrookEngine.ECS;

/// <summary>
/// Lightweight entity identifier.
/// Entities are just IDs - all data lives in components.
/// </summary>
public struct Entity : IEquatable<Entity>
{
    private static ulong _nextId = 1;
    
    public ulong Id { get; private set; }
    public bool IsValid => Id != 0;
    
    /// <summary>
    /// Create a new entity with a unique ID
    /// </summary>
    public static Entity Create()
    {
        return new Entity { Id = _nextId++ };
    }
    
    /// <summary>
    /// Invalid entity (ID = 0)
    /// </summary>
    public static Entity Null => new Entity { Id = 0 };
    
    public bool Equals(Entity other)
    {
        return Id == other.Id;
    }
    
    public override bool Equals(object? obj)
    {
        return obj is Entity entity && Equals(entity);
    }
    
    public override int GetHashCode()
    {
        return Id.GetHashCode();
    }
    
    public static bool operator ==(Entity left, Entity right)
    {
        return left.Equals(right);
    }
    
    public static bool operator !=(Entity left, Entity right)
    {
        return !left.Equals(right);
    }
    
    public override string ToString()
    {
        return $"Entity({Id})";
    }
}
