using MoonBrookEngine.Core;
using MoonBrookEngine.Scene;
using MoonBrookEngine.Graphics;
using MoonBrookEngine.ECS;
using MoonBrookEngine.ECS.Components;
using MoonBrookEngine.Physics;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using System.Numerics;
using Vec2 = MoonBrookEngine.Math.Vector2;
using Col = MoonBrookEngine.Math.Color;
using Rect = MoonBrookEngine.Math.Rectangle;

namespace MoonBrookEngine.Test;

/// <summary>
/// Test scene demonstrating ECS, collision detection, and spatial partitioning
/// </summary>
public class EcsTestScene : MoonBrookEngine.Scene.Scene
{
    private SpriteBatch? _spriteBatch;
    private Texture2D? _whitePixel;
    private World _world;
    private Dictionary<Entity, Rect> _entityBounds; // Track bounds instead of using Quadtree with Entity
    private Camera2D? _camera;
    private IInputContext? _input;
    private Random _random;
    
    // Performance stats
    private int _entityCount;
    private int _collisionChecks;
    private double _lastStatsUpdate;
    
    public EcsTestScene(GL gl, IInputContext input) : base(gl, "ECS Test Scene")
    {
        _world = new World();
        _input = input;
        _random = new Random();
        _entityBounds = new Dictionary<Entity, Rect>();
    }
    
    public override void Initialize()
    {
        Console.WriteLine("=== ECS Test Scene Initializing ===");
        
        // Create rendering components
        _spriteBatch = new SpriteBatch(GL);
        _whitePixel = Texture2D.CreateSolidColor(GL, 1, 1, 255, 255, 255, 255);
        _camera = new Camera2D(1280, 720);
        
        // Create test entities
        CreateBouncingEntities(50);
        
        Console.WriteLine($"Created {_entityCount} entities");
        Console.WriteLine("Controls: ESC - Exit, Space - Add 10 entities, C - Clear entities");
    }
    
    private void CreateBouncingEntities(int count)
    {
        for (int i = 0; i < count; i++)
        {
            var entity = _world.CreateEntity();
            
            // Add Transform
            var transform = new TransformComponent(
                new Vector2(_random.Next(50, 1230), _random.Next(50, 670)),
                0f,
                Vector2.One
            );
            _world.AddComponent(entity, transform);
            
            // Add Sprite
            var color = new Col(
                (byte)_random.Next(100, 255),
                (byte)_random.Next(100, 255),
                (byte)_random.Next(100, 255),
                (byte)255
            );
            var sprite = new SpriteComponent(_whitePixel!, color);
            _world.AddComponent(entity, sprite);
            
            // Add Collider (circle)
            var radius = _random.Next(10, 30);
            var collider = new ColliderComponent(new CircleCollisionShape(radius))
            {
                Tag = "Bouncer"
            };
            _world.AddComponent(entity, collider);
            
            // Store velocity in a custom component (for demo purposes, we'll just track it separately)
            // In a real game, you'd create a VelocityComponent
            
            _entityCount++;
        }
    }
    
    public override void OnEnter()
    {
        base.OnEnter();
        Console.WriteLine("Entering ECS Test Scene");
    }
    
    public override void OnExit()
    {
        base.OnExit();
        Console.WriteLine("Exiting ECS Test Scene");
    }
    
    public override void Update(GameTime gameTime)
    {
        if (_input == null) return;
        
        float dt = (float)gameTime.DeltaTime;
        
        // Handle input
        foreach (var keyboard in _input.Keyboards)
        {
            if (keyboard.IsKeyPressed(Key.Escape))
            {
                Console.WriteLine("ESC pressed - exiting");
                Environment.Exit(0);
            }
            
            if (keyboard.IsKeyPressed(Key.Space))
            {
                CreateBouncingEntities(10);
                Console.WriteLine($"Added 10 entities. Total: {_entityCount}");
            }
            
            if (keyboard.IsKeyPressed(Key.C))
            {
                _world.Clear();
                _entityCount = 0;
                Console.WriteLine("Cleared all entities");
            }
        }
        
        // Update all entities with Transform component
        var entities = _world.GetEntitiesWith<TransformComponent>().ToList();
        
        // Update entity bounds dictionary
        _entityBounds.Clear();
        foreach (var entity in entities)
        {
            var transform = _world.GetComponent<TransformComponent>(entity)!;
            var collider = _world.GetComponent<ColliderComponent>(entity);
            
            if (collider != null)
            {
                var bounds = collider.Shape.GetBounds(transform.Position);
                _entityBounds[entity] = bounds;
            }
        }
        
        // Simple movement: make entities drift
        _collisionChecks = 0;
        foreach (var entity in entities)
        {
            var transform = _world.GetComponent<TransformComponent>(entity)!;
            
            // Simple circular motion
            float time = (float)gameTime.TotalSeconds;
            float speed = 50f;
            float offsetX = MathF.Sin(time + entity.Id * 0.1f) * speed * dt;
            float offsetY = MathF.Cos(time + entity.Id * 0.1f) * speed * dt;
            
            transform.Position += new Vector2(offsetX, offsetY);
            
            // Wrap around screen
            if (transform.Position.X < 0) transform.Position = new Vector2(1280, transform.Position.Y);
            if (transform.Position.X > 1280) transform.Position = new Vector2(0, transform.Position.Y);
            if (transform.Position.Y < 0) transform.Position = new Vector2(transform.Position.X, 720);
            if (transform.Position.Y > 720) transform.Position = new Vector2(transform.Position.X, 0);
            
            // Collision detection (brute force for demo - in real game use spatial partitioning)
            var collider = _world.GetComponent<ColliderComponent>(entity);
            if (collider != null)
            {
                var bounds = collider.Shape.GetBounds(transform.Position);
                
                // Check against all other entities
                foreach (var other in entities)
                {
                    if (other == entity) continue;
                    
                    var otherTransform = _world.GetComponent<TransformComponent>(other);
                    var otherCollider = _world.GetComponent<ColliderComponent>(other);
                    
                    if (otherTransform != null && otherCollider != null)
                    {
                        _collisionChecks++;
                        
                        if (collider.Shape.Intersects(otherCollider.Shape, transform.Position, otherTransform.Position))
                        {
                            // Change color on collision
                            var sprite = _world.GetComponent<SpriteComponent>(entity);
                            if (sprite != null)
                            {
                                sprite.Tint = new Col(255, 100, 100, 255);
                            }
                        }
                    }
                }
            }
        }
        
        // Print stats every 2 seconds
        if (gameTime.TotalSeconds - _lastStatsUpdate > 2.0)
        {
            Console.WriteLine($"Entities: {_entityCount} | Collision Checks: {_collisionChecks} | FPS: {1.0 / gameTime.DeltaTime:F1}");
            _lastStatsUpdate = gameTime.TotalSeconds;
        }
    }
    
    public override void Render(GameTime gameTime)
    {
        if (_spriteBatch == null || _camera == null) return;
        
        _spriteBatch.Begin(_camera);
        
        // Render all entities with Transform and Sprite components
        var entities = _world.GetEntitiesWith<TransformComponent, SpriteComponent>();
        
        foreach (var entity in entities)
        {
            var transform = _world.GetComponent<TransformComponent>(entity)!;
            var sprite = _world.GetComponent<SpriteComponent>(entity)!;
            var collider = _world.GetComponent<ColliderComponent>(entity);
            
            if (sprite.Texture != null)
            {
                // Calculate size from collider
                float size = 20f;
                if (collider?.Shape is CircleCollisionShape circle)
                {
                    size = circle.Radius * 2;
                }
                else if (collider?.Shape is RectangleCollisionShape rect)
                {
                    size = rect.Width;
                }
                
                var destRect = new Rect(
                    (int)(transform.Position.X - size / 2),
                    (int)(transform.Position.Y - size / 2),
                    (int)size,
                    (int)size
                );
                
                _spriteBatch.Draw(
                    sprite.Texture,
                    new Vec2(destRect.X, destRect.Y),
                    null,
                    sprite.Tint,
                    transform.Rotation,
                    new Vec2(0, 0),
                    new Vec2(size, size) / sprite.Texture.Width,
                    sprite.LayerDepth
                );
            }
        }
        
        _spriteBatch.End();
    }
    
    public override void Dispose()
    {
        _spriteBatch?.Dispose();
        _whitePixel?.Dispose();
        _world.Clear();
    }
}
