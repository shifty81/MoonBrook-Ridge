using System.Numerics;
using MoonBrookEngine.Core;
using MoonBrookEngine.ECS;
using MoonBrookEngine.ECS.Components;
using MoonBrookEngine.Graphics;
using MoonBrookEngine.Physics;
using MoonBrookEngine.Physics.Systems;
using MoonBrookEngine.Scene;
using Silk.NET.Input;
using Silk.NET.OpenGL;
using Color = MoonBrookEngine.Math.Color;

namespace MoonBrookEngine.Test;

/// <summary>
/// Test scene demonstrating the physics system with bouncing entities
/// </summary>
public class PhysicsTestScene : MoonBrookEngine.Scene.Scene
{
    private World _world = null!;
    private PhysicsSystem _physicsSystem = null!;
    private SpriteBatch _spriteBatch = null!;
    private Camera2D _camera = null!;
    private Texture2D _circleTexture = null!;
    private IInputContext? _inputContext;
    private float _timeSinceLastSpawn = 0f;
    private const float SpawnInterval = 1.0f;
    private Random _random = new Random();
    
    private int _totalEntities = 0;
    private int _fps = 0;
    private float _fpsTimer = 0f;
    private int _frameCount = 0;
    
    public PhysicsTestScene(GL gl, IInputContext? inputContext = null) : base(gl, "Physics Test")
    {
        _inputContext = inputContext;
    }
    
    public override void Initialize()
    {
        // Create world and physics system
        _world = new World();
        _physicsSystem = new PhysicsSystem(_world);
        _physicsSystem.Gravity = new Vector2(0, 500f); // Moderate gravity
        
        // Create rendering systems
        _spriteBatch = new SpriteBatch(GL);
        _camera = new Camera2D(1280, 720);
        
        // Create a simple circle texture
        _circleTexture = CreateCircleTexture(32);
        
        // Create ground entities (static)
        CreateGround();
        
        // Create some initial bouncing entities
        for (int i = 0; i < 10; i++)
        {
            CreateBouncingEntity();
        }
        
        Console.WriteLine("Physics Test Scene Initialized");
        Console.WriteLine("Controls:");
        Console.WriteLine("  SPACE - Spawn entity");
        Console.WriteLine("  C - Clear all dynamic entities");
        Console.WriteLine("  UP/DOWN - Adjust gravity");
        Console.WriteLine("  ESC - Exit");
    }
    
    public override void OnEnter()
    {
        Console.WriteLine("Entering Physics Test Scene");
    }
    
    public override void OnExit()
    {
        Console.WriteLine("Exiting Physics Test Scene");
    }
    
    public override void Update(GameTime gameTime)
    {
        var deltaTime = (float)gameTime.DeltaTime;
        
        // Update FPS counter
        _frameCount++;
        _fpsTimer += deltaTime;
        if (_fpsTimer >= 1.0f)
        {
            _fps = _frameCount;
            _frameCount = 0;
            _fpsTimer = 0f;
        }
        
        // Handle input
        HandleInput(deltaTime);
        
        // Auto-spawn entities
        _timeSinceLastSpawn += deltaTime;
        if (_timeSinceLastSpawn >= SpawnInterval && _totalEntities < 100)
        {
            CreateBouncingEntity();
            _timeSinceLastSpawn = 0f;
        }
        
        // Update physics
        _physicsSystem.Update(deltaTime);
        
        // Remove entities that fall too far
        RemoveFallenEntities();
    }
    
    public override void Render(GameTime gameTime)
    {
        // Clear screen
        GL.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        // Begin rendering
        _spriteBatch.Begin(_camera);
        
        // Render all entities with sprites
        var renderableEntities = _world.GetEntitiesWith<TransformComponent, SpriteComponent>();
        foreach (var entity in renderableEntities)
        {
            var transform = _world.GetComponent<TransformComponent>(entity);
            var sprite = _world.GetComponent<SpriteComponent>(entity);
            
            if (transform != null && sprite != null && sprite.Texture != null)
            {
                _spriteBatch.Draw(
                    sprite.Texture,
                    transform.Position,
                    sprite.SourceRect,
                    sprite.Tint,
                    transform.Rotation,
                    new Vector2(sprite.Texture.Width / 2, sprite.Texture.Height / 2),
                    transform.Scale,
                    sprite.LayerDepth
                );
            }
        }
        
        _spriteBatch.End();
        
        // Print stats to console occasionally
        if (_frameCount % 60 == 0)
        {
            Console.WriteLine($"FPS: {_fps} | Entities: {_totalEntities} | Gravity: {_physicsSystem.Gravity.Y:F0}");
        }
    }
    
    private void HandleInput(float deltaTime)
    {
        if (_inputContext == null) return;
        
        // Get keyboard input
        foreach (var keyboard in _inputContext.Keyboards)
        {
            // Spawn entity
            if (keyboard.IsKeyPressed(Key.Space))
            {
                for (int i = 0; i < 5; i++)
                    CreateBouncingEntity();
            }
            
            // Clear dynamic entities
            if (keyboard.IsKeyPressed(Key.C))
            {
                ClearDynamicEntities();
            }
            
            // Adjust gravity
            if (keyboard.IsKeyPressed(Key.Up))
            {
                _physicsSystem.Gravity = new Vector2(0, MathF.Max(_physicsSystem.Gravity.Y - 100f * deltaTime, -1000f));
            }
            if (keyboard.IsKeyPressed(Key.Down))
            {
                _physicsSystem.Gravity = new Vector2(0, MathF.Min(_physicsSystem.Gravity.Y + 100f * deltaTime, 1000f));
            }
        }
    }
    
    private void CreateGround()
    {
        // Create ground at bottom
        var ground = _world.CreateEntity();
        _world.AddComponent(ground, new TransformComponent(new System.Numerics.Vector2(640, 680), 0f, new System.Numerics.Vector2(20f, 1f)));
        _world.AddComponent(ground, new SpriteComponent(_circleTexture, Color.FromNormalized(0.3f, 0.3f, 0.3f, 1f)));
        _world.AddComponent(ground, new PhysicsComponent(1000f) { IsStatic = true, Restitution = 0.8f });
        _world.AddComponent(ground, new ColliderComponent(new RectangleCollisionShape(640, 40)));
        
        // Create left wall
        var leftWall = _world.CreateEntity();
        _world.AddComponent(leftWall, new TransformComponent(new System.Numerics.Vector2(20, 360), 0f, new System.Numerics.Vector2(1f, 10f)));
        _world.AddComponent(leftWall, new SpriteComponent(_circleTexture, Color.FromNormalized(0.3f, 0.3f, 0.3f, 1f)));
        _world.AddComponent(leftWall, new PhysicsComponent(1000f) { IsStatic = true, Restitution = 0.8f });
        _world.AddComponent(leftWall, new ColliderComponent(new RectangleCollisionShape(40, 720)));
        
        // Create right wall
        var rightWall = _world.CreateEntity();
        _world.AddComponent(rightWall, new TransformComponent(new System.Numerics.Vector2(1260, 360), 0f, new System.Numerics.Vector2(1f, 10f)));
        _world.AddComponent(rightWall, new SpriteComponent(_circleTexture, Color.FromNormalized(0.3f, 0.3f, 0.3f, 1f)));
        _world.AddComponent(rightWall, new PhysicsComponent(1000f) { IsStatic = true, Restitution = 0.8f });
        _world.AddComponent(rightWall, new ColliderComponent(new RectangleCollisionShape(40, 720)));
    }
    
    private void CreateBouncingEntity()
    {
        var x = _random.Next(100, 1180);
        var y = _random.Next(50, 200);
        
        var entity = _world.CreateEntity();
        
        // Transform
        var scale = (float)_random.NextDouble() * 0.5f + 0.5f; // 0.5 to 1.0
        _world.AddComponent(entity, new TransformComponent(new System.Numerics.Vector2(x, y), 0f, new System.Numerics.Vector2(scale, scale)));
        
        // Sprite with random color
        var color = Color.FromNormalized(
            (float)_random.NextDouble() * 0.5f + 0.5f,
            (float)_random.NextDouble() * 0.5f + 0.5f,
            (float)_random.NextDouble() * 0.5f + 0.5f,
            1f
        );
        _world.AddComponent(entity, new SpriteComponent(_circleTexture, color));
        
        // Physics
        var mass = scale * 2f;
        var physics = new PhysicsComponent(mass, drag: 0.01f)
        {
            Restitution = (float)_random.NextDouble() * 0.3f + 0.6f, // 0.6 to 0.9
            GravityScale = 1.0f
        };
        _world.AddComponent(entity, physics);
        
        // Velocity with random initial velocity
        var velocity = new VelocityComponent
        {
            Velocity = new System.Numerics.Vector2(
                (float)_random.NextDouble() * 200f - 100f,
                (float)_random.NextDouble() * 100f - 50f
            ),
            MaxSpeed = 800f
        };
        _world.AddComponent(entity, velocity);
        
        // Collider
        var radius = 16f * scale;
        _world.AddComponent(entity, new ColliderComponent(new CircleCollisionShape(radius)));
        
        _totalEntities++;
    }
    
    private void ClearDynamicEntities()
    {
        var entitiesToRemove = new List<Entity>();
        
        var physicsEntities = _world.GetEntitiesWith<PhysicsComponent>();
        foreach (var entity in physicsEntities)
        {
            var physics = _world.GetComponent<PhysicsComponent>(entity);
            if (physics != null && !physics.IsStatic)
            {
                entitiesToRemove.Add(entity);
            }
        }
        
        foreach (var entity in entitiesToRemove)
        {
            _world.DestroyEntity(entity);
            _totalEntities--;
        }
        
        Console.WriteLine($"Cleared {entitiesToRemove.Count} dynamic entities");
    }
    
    private void RemoveFallenEntities()
    {
        var entitiesToRemove = new List<Entity>();
        
        var entities = _world.GetEntitiesWith<TransformComponent>();
        foreach (var entity in entities)
        {
            var transform = _world.GetComponent<TransformComponent>(entity);
            var physics = _world.GetComponent<PhysicsComponent>(entity);
            
            if (transform != null && physics != null && !physics.IsStatic)
            {
                // Remove if fallen below screen
                if (transform.Position.Y > 1000f)
                {
                    entitiesToRemove.Add(entity);
                }
            }
        }
        
        foreach (var entity in entitiesToRemove)
        {
            _world.DestroyEntity(entity);
            _totalEntities--;
        }
    }
    
    private Texture2D CreateCircleTexture(int size)
    {
        // Create a simple white circle texture
        var pixels = new byte[size * size * 4];
        var center = size / 2f;
        var radius = size / 2f - 1f;
        
        for (int y = 0; y < size; y++)
        {
            for (int x = 0; x < size; x++)
            {
                var dx = x - center;
                var dy = y - center;
                var distance = MathF.Sqrt(dx * dx + dy * dy);
                
                var index = (y * size + x) * 4;
                if (distance <= radius)
                {
                    pixels[index] = 255;     // R
                    pixels[index + 1] = 255; // G
                    pixels[index + 2] = 255; // B
                    pixels[index + 3] = 255; // A
                }
                else
                {
                    pixels[index] = 0;
                    pixels[index + 1] = 0;
                    pixels[index + 2] = 0;
                    pixels[index + 3] = 0;
                }
            }
        }
        
        return new Texture2D(GL, pixels, size, size);
    }
    
    public override void Dispose()
    {
        _circleTexture?.Dispose();
        _spriteBatch?.Dispose();
    }
}
