using System.Numerics;
using MoonBrookEngine.Math;

namespace MoonBrookEngine.Physics;

/// <summary>
/// Quadtree for efficient spatial partitioning and collision detection
/// </summary>
/// <typeparam name="T">Type of object to store</typeparam>
public class Quadtree<T> where T : class
{
    private const int MaxObjects = 10;
    private const int MaxLevels = 5;
    
    private readonly int _level;
    private readonly Rectangle _bounds;
    private readonly List<QuadtreeObject> _objects;
    private readonly Quadtree<T>[] _nodes;
    
    public struct QuadtreeObject
    {
        public Rectangle Bounds;
        public T Data;
    }
    
    public Quadtree(int level, Rectangle bounds)
    {
        _level = level;
        _bounds = bounds;
        _objects = new List<QuadtreeObject>();
        _nodes = new Quadtree<T>[4];
    }
    
    public Quadtree(Rectangle bounds) : this(0, bounds)
    {
    }
    
    /// <summary>
    /// Clear the quadtree
    /// </summary>
    public void Clear()
    {
        _objects.Clear();
        
        for (int i = 0; i < _nodes.Length; i++)
        {
            if (_nodes[i] != null)
            {
                _nodes[i].Clear();
                _nodes[i] = null!;
            }
        }
    }
    
    /// <summary>
    /// Split the node into 4 subnodes
    /// </summary>
    private void Split()
    {
        float subWidth = _bounds.Width / 2;
        float subHeight = _bounds.Height / 2;
        float x = _bounds.X;
        float y = _bounds.Y;
        
        _nodes[0] = new Quadtree<T>(_level + 1, new Rectangle(x + subWidth, y, subWidth, subHeight));
        _nodes[1] = new Quadtree<T>(_level + 1, new Rectangle(x, y, subWidth, subHeight));
        _nodes[2] = new Quadtree<T>(_level + 1, new Rectangle(x, y + subHeight, subWidth, subHeight));
        _nodes[3] = new Quadtree<T>(_level + 1, new Rectangle(x + subWidth, y + subHeight, subWidth, subHeight));
    }
    
    /// <summary>
    /// Determine which node the object belongs to
    /// </summary>
    private int GetIndex(Rectangle bounds)
    {
        int index = -1;
        float verticalMidpoint = _bounds.X + (_bounds.Width / 2);
        float horizontalMidpoint = _bounds.Y + (_bounds.Height / 2);
        
        bool topQuadrant = (bounds.Y < horizontalMidpoint && bounds.Y + bounds.Height < horizontalMidpoint);
        bool bottomQuadrant = (bounds.Y > horizontalMidpoint);
        
        if (bounds.X < verticalMidpoint && bounds.X + bounds.Width < verticalMidpoint)
        {
            if (topQuadrant)
                index = 1;
            else if (bottomQuadrant)
                index = 2;
        }
        else if (bounds.X > verticalMidpoint)
        {
            if (topQuadrant)
                index = 0;
            else if (bottomQuadrant)
                index = 3;
        }
        
        return index;
    }
    
    /// <summary>
    /// Insert an object into the quadtree
    /// </summary>
    public void Insert(Rectangle bounds, T data)
    {
        if (_nodes[0] != null)
        {
            int index = GetIndex(bounds);
            
            if (index != -1)
            {
                _nodes[index].Insert(bounds, data);
                return;
            }
        }
        
        _objects.Add(new QuadtreeObject { Bounds = bounds, Data = data });
        
        if (_objects.Count > MaxObjects && _level < MaxLevels)
        {
            if (_nodes[0] == null)
                Split();
            
            int i = 0;
            while (i < _objects.Count)
            {
                int index = GetIndex(_objects[i].Bounds);
                if (index != -1)
                {
                    var obj = _objects[i];
                    _nodes[index].Insert(obj.Bounds, obj.Data);
                    _objects.RemoveAt(i);
                }
                else
                {
                    i++;
                }
            }
        }
    }
    
    /// <summary>
    /// Retrieve all objects that could collide with the given bounds
    /// </summary>
    public List<T> Retrieve(Rectangle bounds)
    {
        var returnObjects = new List<T>();
        Retrieve(bounds, returnObjects);
        return returnObjects;
    }
    
    private void Retrieve(Rectangle bounds, List<T> returnObjects)
    {
        int index = GetIndex(bounds);
        if (index != -1 && _nodes[0] != null)
        {
            _nodes[index].Retrieve(bounds, returnObjects);
        }
        
        foreach (var obj in _objects)
        {
            returnObjects.Add(obj.Data);
        }
    }
}
