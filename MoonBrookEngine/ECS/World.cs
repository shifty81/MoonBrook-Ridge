using MoonBrookEngine.Core;

namespace MoonBrookEngine.ECS;

/// <summary>
/// Manages entities and their components.
/// Provides entity creation, component management, and querying.
/// </summary>
public class World
{
    private readonly Dictionary<Entity, List<Component>> _entityComponents;
    private readonly Dictionary<Type, List<Entity>> _componentIndex;
    private readonly HashSet<Entity> _entities;
    
    public int EntityCount => _entities.Count;
    
    public World()
    {
        _entityComponents = new Dictionary<Entity, List<Component>>();
        _componentIndex = new Dictionary<Type, List<Entity>>();
        _entities = new HashSet<Entity>();
    }
    
    /// <summary>
    /// Create a new entity
    /// </summary>
    public Entity CreateEntity()
    {
        var entity = Entity.Create();
        _entities.Add(entity);
        _entityComponents[entity] = new List<Component>();
        return entity;
    }
    
    /// <summary>
    /// Destroy an entity and all its components
    /// </summary>
    public void DestroyEntity(Entity entity)
    {
        if (!_entities.Contains(entity))
            return;
        
        // Remove from component index
        if (_entityComponents.TryGetValue(entity, out var components))
        {
            foreach (var component in components)
            {
                component.OnDetach();
                RemoveFromIndex(entity, component.GetType());
            }
            components.Clear();
        }
        
        // Remove entity
        _entityComponents.Remove(entity);
        _entities.Remove(entity);
    }
    
    /// <summary>
    /// Add a component to an entity
    /// </summary>
    public T AddComponent<T>(Entity entity, T component) where T : Component
    {
        if (!_entities.Contains(entity))
        {
            Console.WriteLine($"Warning: Entity {entity} does not exist");
            return component;
        }
        
        component.Owner = entity;
        _entityComponents[entity].Add(component);
        
        // Add to component index
        var componentType = typeof(T);
        if (!_componentIndex.ContainsKey(componentType))
        {
            _componentIndex[componentType] = new List<Entity>();
        }
        _componentIndex[componentType].Add(entity);
        
        component.OnAttach();
        return component;
    }
    
    /// <summary>
    /// Get a component from an entity
    /// </summary>
    public T? GetComponent<T>(Entity entity) where T : Component
    {
        if (!_entityComponents.TryGetValue(entity, out var components))
            return null;
        
        return components.OfType<T>().FirstOrDefault();
    }
    
    /// <summary>
    /// Check if an entity has a component
    /// </summary>
    public bool HasComponent<T>(Entity entity) where T : Component
    {
        return GetComponent<T>(entity) != null;
    }
    
    /// <summary>
    /// Remove a component from an entity
    /// </summary>
    public void RemoveComponent<T>(Entity entity) where T : Component
    {
        if (!_entityComponents.TryGetValue(entity, out var components))
            return;
        
        var component = components.OfType<T>().FirstOrDefault();
        if (component != null)
        {
            component.OnDetach();
            components.Remove(component);
            RemoveFromIndex(entity, typeof(T));
        }
    }
    
    /// <summary>
    /// Get all entities that have a specific component type
    /// </summary>
    public IEnumerable<Entity> GetEntitiesWith<T>() where T : Component
    {
        var componentType = typeof(T);
        if (_componentIndex.TryGetValue(componentType, out var entities))
        {
            return entities;
        }
        return Enumerable.Empty<Entity>();
    }
    
    /// <summary>
    /// Get all entities that have multiple component types
    /// </summary>
    public IEnumerable<Entity> GetEntitiesWith<T1, T2>() 
        where T1 : Component 
        where T2 : Component
    {
        var entities1 = new HashSet<Entity>(GetEntitiesWith<T1>());
        var entities2 = GetEntitiesWith<T2>();
        return entities2.Where(e => entities1.Contains(e));
    }
    
    /// <summary>
    /// Get all entities
    /// </summary>
    public IEnumerable<Entity> GetAllEntities()
    {
        return _entities;
    }
    
    /// <summary>
    /// Clear all entities and components
    /// </summary>
    public void Clear()
    {
        foreach (var entity in _entities.ToList())
        {
            DestroyEntity(entity);
        }
    }
    
    private void RemoveFromIndex(Entity entity, Type componentType)
    {
        if (_componentIndex.TryGetValue(componentType, out var entities))
        {
            entities.Remove(entity);
        }
    }
}
