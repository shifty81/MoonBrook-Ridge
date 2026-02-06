using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Combat;

/// <summary>
/// Types of projectiles that can be fired
/// </summary>
public enum ProjectileType
{
    Arrow,
    Bolt,        // Crossbow bolt
    Fireball,
    IceShard,
    LightningBolt,
    MagicMissile,
    Stone,       // Slingshot
    Kunai        // Thrown weapon
}

/// <summary>
/// Manages projectile effects with object pooling for performance
/// </summary>
public class ProjectileSystem
{
    private List<Projectile> _activeProjectiles;
    private Queue<Projectile> _projectilePool;
    private const int POOL_SIZE = 200;
    // Shared texture for drawing projectiles - created once and reused
    // Note: Since ProjectileSystem is typically a singleton in the game, this texture
    // lifecycle is managed by the game's overall lifetime and doesn't need explicit disposal
    private static Texture2D? _whitePixel;
    
    public ProjectileSystem()
    {
        _activeProjectiles = new List<Projectile>();
        _projectilePool = new Queue<Projectile>();
        
        // Pre-allocate projectile pool
        for (int i = 0; i < POOL_SIZE; i++)
        {
            _projectilePool.Enqueue(new Projectile());
        }
    }
    
    /// <summary>
    /// Spawns a projectile at a given position with velocity
    /// </summary>
    /// <param name="position">Starting position</param>
    /// <param name="velocity">Velocity vector (direction and speed)</param>
    /// <param name="type">Type of projectile</param>
    /// <param name="damage">Damage dealt by projectile</param>
    /// <param name="lifetime">How long the projectile lives (seconds)</param>
    /// <param name="ownerId">ID of the entity that fired this (to prevent self-damage)</param>
    public void SpawnProjectile(Vector2 position, Vector2 velocity, ProjectileType type, 
        float damage, float lifetime = 3.0f, string ownerId = "")
    {
        if (_projectilePool.Count > 0)
        {
            var projectile = _projectilePool.Dequeue();
            projectile.Initialize(position, velocity, type, damage, lifetime, ownerId);
            _activeProjectiles.Add(projectile);
        }
    }
    
    /// <summary>
    /// Updates all active projectiles
    /// </summary>
    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Update projectiles and remove dead ones
        for (int i = _activeProjectiles.Count - 1; i >= 0; i--)
        {
            _activeProjectiles[i].Update(deltaTime);
            
            if (_activeProjectiles[i].IsDead)
            {
                // Return to pool
                _projectilePool.Enqueue(_activeProjectiles[i]);
                _activeProjectiles.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// Draws all active projectiles
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        // Initialize shared texture if needed
        if (_whitePixel == null)
        {
            _whitePixel = new Texture2D(graphicsDevice, 1, 1);
            _whitePixel.SetData(new[] { Color.White });
        }
        
        foreach (var projectile in _activeProjectiles)
        {
            projectile.Draw(spriteBatch, _whitePixel);
        }
    }
    
    /// <summary>
    /// Gets all active projectiles (for collision detection)
    /// </summary>
    public List<Projectile> GetActiveProjectiles() => _activeProjectiles;
    
    /// <summary>
    /// Removes a projectile (when it hits something)
    /// </summary>
    public void RemoveProjectile(Projectile projectile)
    {
        if (_activeProjectiles.Remove(projectile))
        {
            _projectilePool.Enqueue(projectile);
        }
    }
    
    /// <summary>
    /// Clears all active projectiles
    /// </summary>
    public void Clear()
    {
        // Return all active projectiles to pool
        foreach (var projectile in _activeProjectiles)
        {
            _projectilePool.Enqueue(projectile);
        }
        _activeProjectiles.Clear();
    }
}

/// <summary>
/// Represents a single projectile
/// </summary>
public class Projectile
{
    public Vector2 Position { get; private set; }
    public Vector2 Velocity { get; private set; }
    public ProjectileType Type { get; private set; }
    public float Damage { get; private set; }
    public string OwnerId { get; private set; } = "";
    public bool IsDead => _age >= _lifetime;
    
    private Color _color;
    private float _size;
    private float _lifetime;
    private float _age;
    private float _rotation;
    // Note: Static Random is safe here as Projectile.Initialize is only called from
    // ProjectileSystem.SpawnProjectile on the main thread (single-threaded game loop)
    private static Random _random = new Random();
    
    public void Initialize(Vector2 position, Vector2 velocity, ProjectileType type, 
        float damage, float lifetime, string ownerId)
    {
        Position = position;
        Velocity = velocity;
        Type = type;
        Damage = damage;
        _lifetime = lifetime;
        OwnerId = ownerId;
        _age = 0f;
        
        // Calculate rotation based on velocity direction
        _rotation = MathF.Atan2(velocity.Y, velocity.X);
        
        // Configure appearance based on type
        switch (type)
        {
            case ProjectileType.Arrow:
                _color = new Color(139, 90, 43); // Brown
                _size = 8f;
                break;
                
            case ProjectileType.Bolt:
                _color = new Color(64, 64, 64); // Dark gray
                _size = 6f;
                break;
                
            case ProjectileType.Fireball:
                _color = new Color(255, 69, 0); // Red-orange
                _size = 10f;
                break;
                
            case ProjectileType.IceShard:
                _color = new Color(135, 206, 235); // Sky blue
                _size = 8f;
                break;
                
            case ProjectileType.LightningBolt:
                _color = new Color(255, 255, 0); // Yellow
                _size = 6f;
                break;
                
            case ProjectileType.MagicMissile:
                _color = new Color(147, 112, 219); // Purple
                _size = 7f;
                break;
                
            case ProjectileType.Stone:
                _color = new Color(128, 128, 128); // Gray
                _size = 5f;
                break;
                
            case ProjectileType.Kunai:
                _color = new Color(192, 192, 192); // Silver
                _size = 6f;
                break;
                
            default:
                _color = Color.White;
                _size = 5f;
                break;
        }
    }
    
    public void Update(float deltaTime)
    {
        _age += deltaTime;
        
        // Update position based on velocity
        Position += Velocity * deltaTime;
        
        // Some projectiles have gravity
        if (Type == ProjectileType.Arrow || Type == ProjectileType.Stone)
        {
            Velocity = new Vector2(Velocity.X, Velocity.Y + 300f * deltaTime);
            _rotation = MathF.Atan2(Velocity.Y, Velocity.X); // Update rotation
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        // Calculate fade for magic projectiles
        float alpha = 1f;
        if (Type == ProjectileType.Fireball || Type == ProjectileType.MagicMissile || 
            Type == ProjectileType.LightningBolt)
        {
            // Pulse effect for magic projectiles
            alpha = 0.8f + 0.2f * MathF.Sin(_age * 10f);
        }
        
        Color drawColor = _color * alpha;
        
        // Draw the projectile as a rotated rectangle
        Rectangle sourceRect = new Rectangle(0, 0, 1, 1);
        Rectangle destRect = new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)_size,
            (int)(_size * 0.4f) // Make it elongated
        );
        
        Vector2 origin = new Vector2(0.5f, 0.5f);
        
        spriteBatch.Draw(pixel, destRect, sourceRect, drawColor, _rotation, origin, 
            SpriteEffects.None, 0f);
        
        // Add glow effect for magic projectiles
        if (Type == ProjectileType.Fireball || Type == ProjectileType.LightningBolt || 
            Type == ProjectileType.MagicMissile)
        {
            Color glowColor = _color * (alpha * 0.3f);
            Rectangle glowRect = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                (int)(_size * 1.5f),
                (int)(_size * 1.5f)
            );
            spriteBatch.Draw(pixel, glowRect, sourceRect, glowColor, _rotation, origin, 
                SpriteEffects.None, 0f);
        }
    }
    
    /// <summary>
    /// Gets the bounding box for collision detection
    /// </summary>
    public Rectangle GetBounds()
    {
        return new Rectangle(
            (int)(Position.X - _size / 2),
            (int)(Position.Y - _size / 2),
            (int)_size,
            (int)_size
        );
    }
}
