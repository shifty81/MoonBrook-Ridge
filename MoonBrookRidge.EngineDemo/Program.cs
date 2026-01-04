using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.EngineDemo;

/// <summary>
/// Simple demo game showcasing the MonoGame compatibility layer
/// This demonstrates that existing MonoGame code can run on MoonBrookEngine with minimal changes
/// </summary>
public class DemoGame : Game
{
    private SpriteBatch? _spriteBatch;
    private Texture2D? _whitePixel;
    private SpriteFont? _font;
    
    // Game state
    private Vector2 _playerPosition;
    private Vector2 _playerVelocity;
    private float _playerRotation;
    private Color _playerColor;
    
    private List<Particle> _particles;
    private Random _random;
    
    // Performance display constants
    private const int StatsUpdateInterval = 2; // seconds between stats updates
    private const int TimeCheckModulo = 10; // for time-based throttling
    private const int ThrottleThreshold = 20; // throttle threshold
    
    private struct Particle
    {
        public Vector2 Position;
        public Vector2 Velocity;
        public Color Color;
        public float Life;
        public float MaxLife;
    }
    
    public DemoGame()
    {
        ContentRootDirectory = "Content";
        IsMouseVisible = true;
        
        // Set window size
        Graphics.PreferredBackBufferWidth = 1280;
        Graphics.PreferredBackBufferHeight = 720;
        
        Window.Title = "MoonBrook Engine - Compatibility Demo";
        
        _particles = new List<Particle>();
        _random = new Random();
    }
    
    protected override void Initialize()
    {
        // Initialize game state
        _playerPosition = new Vector2(640, 360);
        _playerVelocity = Vector2.Zero;
        _playerRotation = 0f;
        _playerColor = Color.White;
        
        Console.WriteLine("=== MoonBrook Engine Compatibility Demo ===");
        Console.WriteLine("This demo shows MonoGame-compatible code running on the custom engine");
        Console.WriteLine();
        Console.WriteLine("The code uses standard MonoGame APIs:");
        Console.WriteLine("  - SpriteBatch.Begin() / Draw() / End()");
        Console.WriteLine("  - Vector2, Color, Rectangle");
        Console.WriteLine("  - Game.Initialize/LoadContent/Update/Draw");
        Console.WriteLine();
        Console.WriteLine("But it runs on MoonBrookEngine with:");
        Console.WriteLine("  - Silk.NET windowing and OpenGL rendering");
        Console.WriteLine("  - Custom sprite batching system");
        Console.WriteLine("  - Zero MonoGame dependencies");
        Console.WriteLine();
        Console.WriteLine("Controls:");
        Console.WriteLine("  Mouse - Move player");
        Console.WriteLine("  ESC - Exit");
        Console.WriteLine();
        
        base.Initialize();
    }
    
    protected override void LoadContent()
    {
        // Create SpriteBatch (MonoGame-compatible API)
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        
        // Load font (MonoGame-compatible API)
        _font = Content.Load<SpriteFont>("Fonts/Default");
        
        // Create a white pixel texture for drawing primitives
        // In a real game, you'd load actual textures with Content.Load<Texture2D>()
        // For this demo, we'll use the engine's solid color texture creator
        var gl = ((MoonBrookRidge.Engine.MonoGameCompat.GraphicsDevice)GraphicsDevice).GetInternalGL();
        var engineTexture = MoonBrookEngine.Graphics.Texture2D.CreateSolidColor(
            gl, 1, 1, 255, 255, 255, 255
        );
        _whitePixel = new Texture2D(engineTexture);
        
        Console.WriteLine("✅ Content loaded successfully!");
        Console.WriteLine("✅ Font loaded successfully!");
        Console.WriteLine();
    }
    
    protected override void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Simple particle system - spawn particles continuously
        if (_particles.Count < 500)
        {
            for (int i = 0; i < 5; i++)
            {
                SpawnParticle();
            }
        }
        
        // Update player rotation
        _playerRotation += 2f * deltaTime;
        
        // Update particles
        for (int i = _particles.Count - 1; i >= 0; i--)
        {
            var particle = _particles[i];
            particle.Position += particle.Velocity * deltaTime;
            particle.Life -= deltaTime;
            
            // Fade out based on remaining life
            float alpha = particle.Life / particle.MaxLife;
            particle.Color = new Color(
                particle.Color.R,
                particle.Color.G,
                particle.Color.B,
                (byte)(alpha * 255)
            );
            
            if (particle.Life <= 0)
            {
                _particles.RemoveAt(i);
            }
            else
            {
                _particles[i] = particle;
            }
        }
        
        // Simple stats display (throttled to reduce console spam)
        if ((int)gameTime.TotalGameTime.TotalSeconds % StatsUpdateInterval == 0 && gameTime.TotalGameTime.TotalSeconds > 0.1)
        {
            if ((int)(gameTime.TotalGameTime.TotalSeconds * TimeCheckModulo) % ThrottleThreshold < 2)
            {
                // Calculate actual FPS from delta time
                float fps = deltaTime > 0 ? 1.0f / deltaTime : 0;
                Console.WriteLine($"Particles: {_particles.Count}, FPS: {fps:F1}, Position: {_playerPosition}");
            }
        }
        
        base.Update(gameTime);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        // Clear to a nice gradient-like color
        GraphicsDevice.Clear(new Color(20, 30, 48));
        
        if (_spriteBatch == null || _whitePixel == null)
            return;
        
        // Begin sprite batch (standard MonoGame API)
        _spriteBatch.Begin();
        
        // Draw background grid
        DrawGrid(_spriteBatch, _whitePixel);
        
        // Draw all particles
        foreach (var particle in _particles)
        {
            _spriteBatch.Draw(
                _whitePixel,
                particle.Position,
                null,
                particle.Color,
                0f,
                new Vector2(0.5f, 0.5f),
                4f,
                SpriteEffects.None,
                0f
            );
        }
        
        // Draw player (rotating square)
        _spriteBatch.Draw(
            _whitePixel,
            _playerPosition,
            null,
            _playerColor,
            _playerRotation,
            new Vector2(0.5f, 0.5f),
            32f,
            SpriteEffects.None,
            0f
        );
        
        // Draw text overlay (MonoGame-compatible font rendering)
        if (_font != null)
        {
            _spriteBatch.DrawString(_font, "MoonBrook Engine - Font Demo", new Vector2(10, 10), Color.White);
            _spriteBatch.DrawString(_font, $"Particles: {_particles.Count}", new Vector2(10, 30), Color.Yellow);
            _spriteBatch.DrawString(_font, $"Position: ({_playerPosition.X:F0}, {_playerPosition.Y:F0})", new Vector2(10, 50), Color.Cyan);
            _spriteBatch.DrawString(_font, "Note: Font is default/stub - atlas rendering not yet implemented", new Vector2(10, 680), new Color(180, 180, 180));
        }
        
        // End sprite batch
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
    
    private void DrawGrid(SpriteBatch spriteBatch, Texture2D pixel)
    {
        int spacing = 64;
        Color gridColor = new Color(40, 50, 68, 128);
        
        // Vertical lines
        for (int x = 0; x < 1280; x += spacing)
        {
            spriteBatch.Draw(
                pixel,
                new Rectangle(x, 0, 1, 720),
                gridColor
            );
        }
        
        // Horizontal lines
        for (int y = 0; y < 720; y += spacing)
        {
            spriteBatch.Draw(
                pixel,
                new Rectangle(0, y, 1280, 1),
                gridColor
            );
        }
    }
    
    private void SpawnParticle()
    {
        float angle = (float)(_random.NextDouble() * Math.PI * 2);
        float speed = 50f + (float)_random.NextDouble() * 100f;
        
        var particle = new Particle
        {
            Position = _playerPosition + new Vector2(
                (float)(_random.NextDouble() * 20 - 10),
                (float)(_random.NextDouble() * 20 - 10)
            ),
            Velocity = new Vector2(
                MathF.Cos(angle) * speed,
                MathF.Sin(angle) * speed
            ),
            Color = new Color(
                _random.Next(100, 255),
                _random.Next(100, 255),
                _random.Next(100, 255),
                255
            ),
            Life = 2f,
            MaxLife = 2f
        };
        
        _particles.Add(particle);
    }
}

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("=== MoonBrook Engine Demo Launcher ===");
        Console.WriteLine();
        Console.WriteLine("Select demo to run:");
        Console.WriteLine("  1. Compatibility Demo (particle effects)");
        Console.WriteLine("  2. Farmhouse Scene (first game scene)");
        Console.WriteLine();
        Console.Write("Enter choice (1 or 2): ");
        
        string? choice = Console.ReadLine();
        Console.WriteLine();
        
        try
        {
            if (choice == "2")
            {
                // Run farmhouse scene demo
                Console.WriteLine("Starting Farmhouse Scene Demo...");
                Console.WriteLine();
                using var game = new FarmhouseGame();
                game.Run();
            }
            else
            {
                // Default to compatibility demo
                Console.WriteLine("Starting Compatibility Demo...");
                Console.WriteLine();
                using var game = new DemoGame();
                game.Run();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            Console.WriteLine($"Stack trace: {ex.StackTrace}");
        }
        
        Console.WriteLine();
        Console.WriteLine("Demo ended.");
    }
}
