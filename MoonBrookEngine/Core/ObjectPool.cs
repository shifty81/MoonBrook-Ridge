using System;
using System.Collections.Generic;

namespace MoonBrookEngine.Core;

/// <summary>
/// Generic object pool for reducing allocations and GC pressure
/// </summary>
/// <typeparam name="T">Type of objects to pool</typeparam>
public class ObjectPool<T> where T : class
{
    private readonly Stack<T> _available;
    private readonly Func<T> _factory;
    private readonly Action<T>? _reset;
    private readonly int _maxSize;
    private int _totalCreated;
    
    /// <summary>
    /// Number of objects currently available in the pool
    /// </summary>
    public int AvailableCount => _available.Count;
    
    /// <summary>
    /// Total number of objects created by this pool
    /// </summary>
    public int TotalCreated => _totalCreated;
    
    /// <summary>
    /// Number of objects currently in use (checked out)
    /// </summary>
    public int InUseCount => _totalCreated - _available.Count;
    
    /// <summary>
    /// Create a new object pool
    /// </summary>
    /// <param name="factory">Function to create new instances</param>
    /// <param name="reset">Optional reset function called when returning objects</param>
    /// <param name="maxSize">Maximum pool size (0 = unlimited)</param>
    /// <param name="prewarm">Number of objects to create initially</param>
    public ObjectPool(Func<T> factory, Action<T>? reset = null, int maxSize = 0, int prewarm = 0)
    {
        _factory = factory ?? throw new ArgumentNullException(nameof(factory));
        _reset = reset;
        _maxSize = maxSize;
        _available = new Stack<T>(prewarm > 0 ? prewarm : 16);
        _totalCreated = 0;
        
        // Prewarm the pool
        for (int i = 0; i < prewarm; i++)
        {
            var obj = _factory();
            _available.Push(obj);
            _totalCreated++;
        }
    }
    
    /// <summary>
    /// Get an object from the pool (or create new if empty)
    /// </summary>
    public T Get()
    {
        if (_available.Count > 0)
        {
            return _available.Pop();
        }
        
        // Create new object
        _totalCreated++;
        return _factory();
    }
    
    /// <summary>
    /// Return an object to the pool
    /// </summary>
    public void Return(T obj)
    {
        if (obj == null)
            return;
        
        // Check max size limit
        if (_maxSize > 0 && _available.Count >= _maxSize)
        {
            // Pool is full, let object be garbage collected
            return;
        }
        
        // Reset object state if handler provided
        _reset?.Invoke(obj);
        
        _available.Push(obj);
    }
    
    /// <summary>
    /// Return multiple objects to the pool
    /// </summary>
    public void ReturnRange(IEnumerable<T> objects)
    {
        foreach (var obj in objects)
        {
            Return(obj);
        }
    }
    
    /// <summary>
    /// Clear the pool and reset statistics
    /// </summary>
    public void Clear()
    {
        _available.Clear();
        _totalCreated = _available.Count;
    }
    
    /// <summary>
    /// Get pool statistics as a string
    /// </summary>
    public string GetStats()
    {
        return $"Pool<{typeof(T).Name}>: Available={AvailableCount}, InUse={InUseCount}, Total={TotalCreated}";
    }
}

/// <summary>
/// Poolable object interface for automatic reset
/// </summary>
public interface IPoolable
{
    /// <summary>
    /// Reset object state when returned to pool
    /// </summary>
    void Reset();
}

/// <summary>
/// Manager for multiple object pools
/// </summary>
public class ObjectPoolManager
{
    private readonly Dictionary<Type, object> _pools;
    
    public ObjectPoolManager()
    {
        _pools = new Dictionary<Type, object>();
    }
    
    /// <summary>
    /// Register a pool for a specific type
    /// </summary>
    public void RegisterPool<T>(ObjectPool<T> pool) where T : class
    {
        _pools[typeof(T)] = pool;
    }
    
    /// <summary>
    /// Create and register a new pool
    /// </summary>
    public ObjectPool<T> CreatePool<T>(Func<T> factory, Action<T>? reset = null, int maxSize = 0, int prewarm = 0) where T : class
    {
        var pool = new ObjectPool<T>(factory, reset, maxSize, prewarm);
        RegisterPool(pool);
        return pool;
    }
    
    /// <summary>
    /// Get a pool for a specific type
    /// </summary>
    public ObjectPool<T>? GetPool<T>() where T : class
    {
        if (_pools.TryGetValue(typeof(T), out var pool))
        {
            return pool as ObjectPool<T>;
        }
        return null;
    }
    
    /// <summary>
    /// Get an object from the registered pool
    /// </summary>
    public T Get<T>() where T : class
    {
        var pool = GetPool<T>();
        if (pool == null)
            throw new InvalidOperationException($"No pool registered for type {typeof(T).Name}");
        return pool.Get();
    }
    
    /// <summary>
    /// Return an object to the registered pool
    /// </summary>
    public void Return<T>(T obj) where T : class
    {
        var pool = GetPool<T>();
        pool?.Return(obj);
    }
    
    /// <summary>
    /// Get statistics for all pools
    /// </summary>
    public string GetAllStats()
    {
        var stats = new System.Text.StringBuilder();
        stats.AppendLine("Object Pool Statistics:");
        foreach (var kvp in _pools)
        {
            // Use reflection to call GetStats on each pool
            var getStatsMethod = kvp.Value.GetType().GetMethod("GetStats");
            if (getStatsMethod != null)
            {
                var poolStats = getStatsMethod.Invoke(kvp.Value, null);
                stats.AppendLine($"  {poolStats}");
            }
        }
        return stats.ToString();
    }
    
    /// <summary>
    /// Clear all pools
    /// </summary>
    public void ClearAll()
    {
        foreach (var pool in _pools.Values)
        {
            var clearMethod = pool.GetType().GetMethod("Clear");
            clearMethod?.Invoke(pool, null);
        }
    }
}
