using MoonBrookEngine.Core;
using MoonBrookEngine.Graphics;
using Silk.NET.OpenGL;
using Vec2 = MoonBrookEngine.Math.Vector2;
using Col = MoonBrookEngine.Math.Color;

namespace MoonBrookEngine.Test;

/// <summary>
/// Enhanced test application demonstrating SpriteBatch and Camera2D
/// </summary>
public class TestGame
{
    private Engine _engine;
    private SpriteBatch? _spriteBatch;
    private Camera2D? _camera;
    private Texture2D? _testTexture;
    private List<SpriteData> _sprites;
    private float _cameraSpeed = 200f;
    private float _zoomSpeed = 0.5f;
    
    private struct SpriteData
    {
        public Vec2 Position;
        public Vec2 Velocity;
        public Col Color;
        public float Rotation;
        public float RotationSpeed;
    }
    
    public TestGame()
    {
        _engine = new Engine("MoonBrook Engine - SpriteBatch Demo", 1280, 720);
        _sprites = new List<SpriteData>();
        
        _engine.OnInitialize += Initialize;
        _engine.OnUpdate += Update;
        _engine.OnRender += Render;
        _engine.OnShutdown += Shutdown;
    }
    
    public void Run()
    {
        _engine.Run();
    }
    
    private void Initialize()
    {
        Console.WriteLine("Test game initializing...");
        
        // Create SpriteBatch with performance monitoring
        _spriteBatch = new SpriteBatch(_engine.GL, _engine.Performance);
        
        // Create Camera
        _camera = new Camera2D(_engine.Width, _engine.Height);
        
        // Create a 32x32 white texture
        _testTexture = Texture2D.CreateSolidColor(_engine.GL, 32, 32, 255, 255, 255, 255);
        
        // Create 100 bouncing sprites with random colors, positions, and velocities
        Random rand = new Random();
        for (int i = 0; i < 100; i++)
        {
            _sprites.Add(new SpriteData
            {
                Position = new Vec2(
                    rand.Next(0, 1280),
                    rand.Next(0, 720)
                ),
                Velocity = new Vec2(
                    rand.Next(-200, 200),
                    rand.Next(-200, 200)
                ),
                Color = new Col(
                    (byte)rand.Next(100, 255),
                    (byte)rand.Next(100, 255),
                    (byte)rand.Next(100, 255),
                    (byte)255
                ),
                Rotation = 0f,
                RotationSpeed = (float)(rand.NextDouble() * 4 - 2) // -2 to 2 radians/sec
            });
        }
        
        Console.WriteLine("Test game initialized successfully!");
        Console.WriteLine($"Created {_sprites.Count} sprites");
        Console.WriteLine();
        Console.WriteLine("Controls:");
        Console.WriteLine("  WASD - Move camera");
        Console.WriteLine("  Q/E - Zoom out/in");
        Console.WriteLine("  R - Reset camera");
        Console.WriteLine("  ESC - Exit");
    }
    
    private void Update(GameTime gameTime)
    {
        float dt = (float)gameTime.DeltaTime;
        
        // Check for ESC key to exit
        var keyboards = _engine.Input.Keyboards;
        if (keyboards.Count > 0)
        {
            var keyboard = keyboards[0];
            
            if (keyboard.IsKeyPressed(Silk.NET.Input.Key.Escape))
            {
                _engine.Stop();
            }
            
            // Camera controls
            if (_camera != null)
            {
                Vec2 movement = Vec2.Zero;
                
                if (keyboard.IsKeyPressed(Silk.NET.Input.Key.W))
                    movement.Y -= _cameraSpeed * dt;
                if (keyboard.IsKeyPressed(Silk.NET.Input.Key.S))
                    movement.Y += _cameraSpeed * dt;
                if (keyboard.IsKeyPressed(Silk.NET.Input.Key.A))
                    movement.X -= _cameraSpeed * dt;
                if (keyboard.IsKeyPressed(Silk.NET.Input.Key.D))
                    movement.X += _cameraSpeed * dt;
                
                _camera.Move(movement);
                
                // Zoom controls
                if (keyboard.IsKeyPressed(Silk.NET.Input.Key.Q))
                    _camera.Zoom -= _zoomSpeed * dt;
                if (keyboard.IsKeyPressed(Silk.NET.Input.Key.E))
                    _camera.Zoom += _zoomSpeed * dt;
                
                // Reset camera
                if (keyboard.IsKeyPressed(Silk.NET.Input.Key.R))
                {
                    _camera.Position = Vec2.Zero;
                    _camera.Zoom = 1.0f;
                }
            }
        }
        
        // Update sprites (bouncing)
        for (int i = 0; i < _sprites.Count; i++)
        {
            var sprite = _sprites[i];
            
            // Update position
            sprite.Position += sprite.Velocity * dt;
            
            // Update rotation
            sprite.Rotation += sprite.RotationSpeed * dt;
            
            // Bounce off edges
            if (sprite.Position.X < 0 || sprite.Position.X > 1280)
            {
                sprite.Velocity.X = -sprite.Velocity.X;
                sprite.Position.X = System.Math.Clamp(sprite.Position.X, 0, 1280);
            }
            if (sprite.Position.Y < 0 || sprite.Position.Y > 720)
            {
                sprite.Velocity.Y = -sprite.Velocity.Y;
                sprite.Position.Y = System.Math.Clamp(sprite.Position.Y, 0, 720);
            }
            
            _sprites[i] = sprite;
        }
        
        // Display performance metrics every second
        if ((int)gameTime.TotalSeconds % 1 == 0 && gameTime.TotalSeconds > 0 && (int)(gameTime.TotalSeconds * 10) % 10 == 0)
        {
            var stats = _engine.Performance.GetStats();
            Console.WriteLine($"{stats.ToString()} | Sprites: {_sprites.Count} | Camera Pos: {_camera?.Position} | Zoom: {_camera?.Zoom:F2}");
        }
    }
    
    private void Render(GameTime gameTime)
    {
        if (_spriteBatch == null || _camera == null || _testTexture == null) return;
        
        // Begin sprite batch with camera
        _spriteBatch.Begin(_camera);
        
        // Draw all sprites
        foreach (var sprite in _sprites)
        {
            _spriteBatch.Draw(
                _testTexture,
                sprite.Position,
                null,
                sprite.Color,
                sprite.Rotation,
                new Vec2(16, 16), // origin at center (32x32 texture)
                Vec2.One,
                0f
            );
        }
        
        // End sprite batch (flushes to GPU)
        _spriteBatch.End();
    }
    
    private void Shutdown()
    {
        Console.WriteLine("Test game shutting down...");
        
        _spriteBatch?.Dispose();
        _testTexture?.Dispose();
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== MoonBrook Engine Test ===");
        Console.WriteLine("Starting engine test application...");
        Console.WriteLine();
        
        try
        {
            var game = new TestGame();
            game.Run();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        Console.WriteLine("Test application ended.");
    }
}
