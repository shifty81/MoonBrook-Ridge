using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Types of particle effects
/// </summary>
public enum ParticleEffectType
{
    None,
    Dust,           // For farming/digging
    Sparkle,        // For harvesting crops
    Water,          // For watering
    Rock,           // For mining
    Wood,           // For chopping trees
    Fish,           // For fishing
    Heart,          // For gifting/relationship increase
    Coin            // For selling items
}

/// <summary>
/// Manages particle effects in the game
/// </summary>
public class ParticleSystem
{
    private List<Particle> _activeParticles;
    private Queue<Particle> _particlePool;
    private const int POOL_SIZE = 500;
    private static Texture2D? _whitePixel; // Shared texture for drawing particles
    
    public ParticleSystem()
    {
        _activeParticles = new List<Particle>();
        _particlePool = new Queue<Particle>();
        
        // Pre-allocate particle pool
        for (int i = 0; i < POOL_SIZE; i++)
        {
            _particlePool.Enqueue(new Particle());
        }
    }
    
    /// <summary>
    /// Spawns particles at a given position
    /// </summary>
    public void SpawnParticles(Vector2 position, ParticleEffectType type, int count = 10)
    {
        for (int i = 0; i < count && _particlePool.Count > 0; i++)
        {
            var particle = _particlePool.Dequeue();
            particle.Initialize(position, type);
            _activeParticles.Add(particle);
        }
    }
    
    /// <summary>
    /// Updates all active particles
    /// </summary>
    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Update particles and remove dead ones
        for (int i = _activeParticles.Count - 1; i >= 0; i--)
        {
            _activeParticles[i].Update(deltaTime);
            
            if (_activeParticles[i].IsDead)
            {
                // Return to pool
                _particlePool.Enqueue(_activeParticles[i]);
                _activeParticles.RemoveAt(i);
            }
        }
    }
    
    /// <summary>
    /// Draws all active particles
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        // Initialize shared texture if needed
        if (_whitePixel == null)
        {
            _whitePixel = new Texture2D(graphicsDevice, 1, 1);
            _whitePixel.SetData(new[] { Color.White });
        }
        
        foreach (var particle in _activeParticles)
        {
            particle.Draw(spriteBatch, _whitePixel);
        }
    }
}

/// <summary>
/// Represents a single particle
/// </summary>
internal class Particle
{
    private Vector2 _position;
    private Vector2 _velocity;
    private Color _color;
    private float _size;
    private float _lifetime;
    private float _age;
    private float _rotation;
    private float _rotationSpeed;
    private ParticleEffectType _type;
    private static Random _random = new Random();
    
    public void Initialize(Vector2 position, ParticleEffectType type)
    {
        _position = position;
        _type = type;
        _age = 0f;
        
        // Configure particle based on type
        switch (type)
        {
            case ParticleEffectType.Dust:
                _velocity = new Vector2(
                    _random.Next(-50, 50),
                    _random.Next(-100, -50)
                );
                _color = new Color(139, 90, 43, 200); // Brown dust
                _size = _random.Next(2, 5);
                _lifetime = _random.Next(5, 10) / 10f;
                _rotation = 0f;
                _rotationSpeed = _random.Next(-2, 2);
                break;
                
            case ParticleEffectType.Sparkle:
                _velocity = new Vector2(
                    _random.Next(-30, 30),
                    _random.Next(-80, -40)
                );
                _color = new Color(255, 215, 0, 255); // Gold sparkle
                _size = _random.Next(3, 6);
                _lifetime = _random.Next(8, 15) / 10f;
                _rotation = 0f;
                _rotationSpeed = _random.Next(-5, 5);
                break;
                
            case ParticleEffectType.Water:
                _velocity = new Vector2(
                    _random.Next(-20, 20),
                    _random.Next(-60, -30)
                );
                _color = new Color(100, 149, 237, 180); // Blue water
                _size = _random.Next(2, 4);
                _lifetime = _random.Next(4, 8) / 10f;
                _rotation = 0f;
                _rotationSpeed = 0f;
                break;
                
            case ParticleEffectType.Rock:
                _velocity = new Vector2(
                    _random.Next(-60, 60),
                    _random.Next(-120, -60)
                );
                _color = new Color(105, 105, 105, 220); // Gray rock
                _size = _random.Next(3, 6);
                _lifetime = _random.Next(6, 12) / 10f;
                _rotation = 0f;
                _rotationSpeed = _random.Next(-3, 3);
                break;
                
            case ParticleEffectType.Wood:
                _velocity = new Vector2(
                    _random.Next(-40, 40),
                    _random.Next(-100, -50)
                );
                _color = new Color(139, 90, 43, 220); // Brown wood
                _size = _random.Next(3, 6);
                _lifetime = _random.Next(6, 12) / 10f;
                _rotation = 0f;
                _rotationSpeed = _random.Next(-3, 3);
                break;
                
            case ParticleEffectType.Fish:
                _velocity = new Vector2(
                    _random.Next(-30, 30),
                    _random.Next(-80, -40)
                );
                _color = new Color(173, 216, 230, 200); // Light blue
                _size = _random.Next(2, 5);
                _lifetime = _random.Next(5, 10) / 10f;
                _rotation = 0f;
                _rotationSpeed = _random.Next(-4, 4);
                break;
                
            case ParticleEffectType.Heart:
                _velocity = new Vector2(
                    _random.Next(-20, 20),
                    _random.Next(-60, -30)
                );
                _color = new Color(255, 105, 180, 255); // Hot pink heart
                _size = _random.Next(4, 8);
                _lifetime = _random.Next(10, 15) / 10f;
                _rotation = 0f;
                _rotationSpeed = _random.Next(-2, 2);
                break;
                
            case ParticleEffectType.Coin:
                _velocity = new Vector2(
                    _random.Next(-30, 30),
                    _random.Next(-100, -50)
                );
                _color = new Color(255, 215, 0, 255); // Gold coin
                _size = _random.Next(4, 7);
                _lifetime = _random.Next(8, 12) / 10f;
                _rotation = 0f;
                _rotationSpeed = _random.Next(-6, 6);
                break;
                
            default:
                _velocity = Vector2.Zero;
                _color = Color.White;
                _size = 1;
                _lifetime = 0.5f;
                _rotation = 0f;
                _rotationSpeed = 0f;
                break;
        }
    }
    
    public void Update(float deltaTime)
    {
        _age += deltaTime;
        
        // Apply gravity
        _velocity.Y += 200f * deltaTime;
        
        // Update position
        _position += _velocity * deltaTime;
        
        // Update rotation
        _rotation += _rotationSpeed * deltaTime;
        
        // Apply drag
        _velocity *= 0.98f;
    }
    
    public void Draw(SpriteBatch spriteBatch, Texture2D pixel)
    {
        // Calculate fade based on lifetime
        float fadeProgress = _age / _lifetime;
        float alpha = 1f - fadeProgress;
        
        // Create color with fade
        Color drawColor = _color * alpha;
        
        // Draw the particle
        Rectangle rect = new Rectangle(
            (int)_position.X - (int)_size / 2,
            (int)_position.Y - (int)_size / 2,
            (int)_size,
            (int)_size
        );
        
        spriteBatch.Draw(pixel, rect, drawColor);
    }
    
    public bool IsDead => _age >= _lifetime;
}
