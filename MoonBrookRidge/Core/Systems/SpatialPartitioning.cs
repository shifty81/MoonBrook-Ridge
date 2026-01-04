using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Interface for entities that can be stored in the spatial partitioning system
/// </summary>
public interface ISpatialEntity
{
    Vector2 Position { get; }
    Rectangle GetBounds();
    string EntityId { get; }
}

/// <summary>
/// Quadtree spatial partitioning system for efficient entity queries in large worlds
/// Reduces O(n²) collision checks to O(n log n) by dividing space into regions
/// </summary>
public class Quadtree
{
    private const int MAX_OBJECTS = 10;
    private const int MAX_LEVELS = 5;
    
    private int _level;
    private List<ISpatialEntity> _objects;
    private Rectangle _bounds;
    private Quadtree[] _nodes; // 4 child nodes
    
    public Quadtree(int level, Rectangle bounds)
    {
        _level = level;
        _objects = new List<ISpatialEntity>();
        _bounds = bounds;
        _nodes = new Quadtree[4];
    }
    
    /// <summary>
    /// Clears the quadtree
    /// </summary>
    public void Clear()
    {
        _objects.Clear();
        
        for (int i = 0; i < _nodes.Length; i++)
        {
            if (_nodes[i] != null)
            {
                _nodes[i].Clear();
                _nodes[i] = null;
            }
        }
    }
    
    /// <summary>
    /// Splits the node into 4 subnodes
    /// </summary>
    private void Split()
    {
        int subWidth = _bounds.Width / 2;
        int subHeight = _bounds.Height / 2;
        int x = _bounds.X;
        int y = _bounds.Y;
        
        // Top-right
        _nodes[0] = new Quadtree(_level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
        // Top-left
        _nodes[1] = new Quadtree(_level + 1, new Rectangle(x, y, subWidth, subHeight));
        // Bottom-left
        _nodes[2] = new Quadtree(_level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
        // Bottom-right
        _nodes[3] = new Quadtree(_level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
    }
    
    /// <summary>
    /// Determines which node the entity belongs to
    /// -1 means entity cannot completely fit within a child node and is part of the parent
    /// </summary>
    private int GetIndex(Rectangle entityBounds)
    {
        int index = -1;
        
        double verticalMidpoint = _bounds.X + (_bounds.Width / 2.0);
        double horizontalMidpoint = _bounds.Y + (_bounds.Height / 2.0);
        
        // Entity can completely fit within the top quadrants
        bool topQuadrant = (entityBounds.Y < horizontalMidpoint && 
                           entityBounds.Y + entityBounds.Height < horizontalMidpoint);
        
        // Entity can completely fit within the bottom quadrants
        bool bottomQuadrant = (entityBounds.Y > horizontalMidpoint);
        
        // Entity can completely fit within the left quadrants
        if (entityBounds.X < verticalMidpoint && 
            entityBounds.X + entityBounds.Width < verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 1; // Top-left
            }
            else if (bottomQuadrant)
            {
                index = 2; // Bottom-left
            }
        }
        // Entity can completely fit within the right quadrants
        else if (entityBounds.X > verticalMidpoint)
        {
            if (topQuadrant)
            {
                index = 0; // Top-right
            }
            else if (bottomQuadrant)
            {
                index = 3; // Bottom-right
            }
        }
        
        return index;
    }
    
    /// <summary>
    /// Inserts an entity into the quadtree
    /// </summary>
    public void Insert(ISpatialEntity entity)
    {
        if (_nodes[0] != null)
        {
            int index = GetIndex(entity.GetBounds());
            
            if (index != -1)
            {
                _nodes[index].Insert(entity);
                return;
            }
        }
        
        _objects.Add(entity);
        
        // Split if we have too many objects and haven't reached max depth
        if (_objects.Count > MAX_OBJECTS && _level < MAX_LEVELS)
        {
            // Split if not already split
            if (_nodes[0] == null)
            {
                Split();
            }
            
            // Move objects to children if possible (use reverse loop for efficient removal)
            for (int i = _objects.Count - 1; i >= 0; i--)
            {
                int index = GetIndex(_objects[i].GetBounds());
                if (index != -1)
                {
                    _nodes[index].Insert(_objects[i]);
                    _objects.RemoveAt(i);
                }
            }
        }
    }
    
    /// <summary>
    /// Returns all entities that could collide with the given rectangle
    /// </summary>
    public List<ISpatialEntity> Retrieve(List<ISpatialEntity> returnObjects, Rectangle bounds)
    {
        int index = GetIndex(bounds);
        
        // If we have child nodes and the entity fits in one
        if (index != -1 && _nodes[0] != null)
        {
            _nodes[index].Retrieve(returnObjects, bounds);
        }
        else
        {
            // Add all objects from child nodes if we span multiple quadrants
            if (_nodes[0] != null)
            {
                for (int i = 0; i < _nodes.Length; i++)
                {
                    _nodes[i].Retrieve(returnObjects, bounds);
                }
            }
        }
        
        // Add objects from this node
        returnObjects.AddRange(_objects);
        
        return returnObjects;
    }
    
    /// <summary>
    /// Returns all entities in the quadtree
    /// </summary>
    public List<ISpatialEntity> GetAllEntities()
    {
        List<ISpatialEntity> allEntities = new List<ISpatialEntity>();
        GetAllEntitiesRecursive(allEntities);
        return allEntities;
    }
    
    private void GetAllEntitiesRecursive(List<ISpatialEntity> entities)
    {
        entities.AddRange(_objects);
        
        if (_nodes[0] != null)
        {
            for (int i = 0; i < _nodes.Length; i++)
            {
                _nodes[i].GetAllEntitiesRecursive(entities);
            }
        }
    }
}

/// <summary>
/// Spatial partitioning manager for efficient entity queries
/// </summary>
public class SpatialPartitioningSystem
{
    private Quadtree _quadtree;
    private Rectangle _worldBounds;
    
    public SpatialPartitioningSystem(Rectangle worldBounds)
    {
        _worldBounds = worldBounds;
        _quadtree = new Quadtree(0, worldBounds);
    }
    
    /// <summary>
    /// Rebuilds the quadtree with the given entities
    /// Call this each frame or when entities move significantly
    /// </summary>
    public void Rebuild(List<ISpatialEntity> entities)
    {
        _quadtree.Clear();
        
        foreach (var entity in entities)
        {
            _quadtree.Insert(entity);
        }
    }
    
    /// <summary>
    /// Queries entities near the given bounds
    /// Returns only entities that could potentially collide
    /// </summary>
    public List<ISpatialEntity> Query(Rectangle bounds)
    {
        var result = new List<ISpatialEntity>();
        return _quadtree.Retrieve(result, bounds);
    }
    
    /// <summary>
    /// Queries entities within a radius of a point
    /// </summary>
    public List<ISpatialEntity> QueryRadius(Vector2 center, float radius)
    {
        Rectangle bounds = new Rectangle(
            (int)(center.X - radius),
            (int)(center.Y - radius),
            (int)(radius * 2),
            (int)(radius * 2)
        );
        
        var candidates = Query(bounds);
        var result = new List<ISpatialEntity>();
        
        // Filter by actual circle distance
        float radiusSquared = radius * radius;
        foreach (var entity in candidates)
        {
            float distSquared = Vector2.DistanceSquared(center, entity.Position);
            if (distSquared <= radiusSquared)
            {
                result.Add(entity);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Returns all entities in the spatial system
    /// </summary>
    public List<ISpatialEntity> GetAllEntities()
    {
        return _quadtree.GetAllEntities();
    }
}
