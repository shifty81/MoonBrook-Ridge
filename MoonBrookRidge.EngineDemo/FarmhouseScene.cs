using MoonBrookEngine.Core;
using MoonBrookEngine.Scene;
using MoonBrookEngine.Graphics;
using MoonBrookEngine.Math;
using MoonBrookEngine.Input;
using Silk.NET.OpenGL;
using Silk.NET.Input;

namespace MoonBrookRidge.EngineDemo;

/// <summary>
/// First scene implementation showing the player farmhouse interior
/// Demonstrates basic scene setup with player character, tiles, and furniture
/// </summary>
public class FarmhouseScene : Scene
{
    private SpriteBatch? _spriteBatch;
    private Camera2D? _camera;
    private Texture2D? _whitePixel;
    // Reserved for future sprite integration - will load actual player sprite texture
#pragma warning disable CS0649 // Field is never assigned to - reserved for future use
    private Texture2D? _playerTexture;
#pragma warning restore CS0649
    
    // Player state
    private Vector2 _playerPosition;
    private Vector2 _playerVelocity;
    private float _playerSpeed = 150f; // pixels per second
    private Rectangle _playerBounds;
    
    // Room layout
    private const int TILE_SIZE = 16;
    private const int ROOM_WIDTH = 15; // tiles
    private const int ROOM_HEIGHT = 12; // tiles
    private Tile[,] _tiles;
    
    // Furniture objects
    private List<FurnitureObject> _furniture;
    
    // Input reference
    private InputManager? _inputManager;
    
    private struct Tile
    {
        public TileType Type;
        public Color Color;
        
        public Tile(TileType type, Color color)
        {
            Type = type;
            Color = color;
        }
    }
    
    private enum TileType
    {
        Floor,
        Wall,
        Door
    }
    
    private struct FurnitureObject
    {
        public string Name;
        public Rectangle Bounds;
        public Color Color;
        
        public FurnitureObject(string name, Rectangle bounds, Color color)
        {
            Name = name;
            Bounds = bounds;
            Color = color;
        }
    }
    
    public FarmhouseScene(GL gl, InputManager inputManager) : base(gl, "Farmhouse Interior")
    {
        _inputManager = inputManager;
        _tiles = new Tile[ROOM_WIDTH, ROOM_HEIGHT];
        _furniture = new List<FurnitureObject>();
    }
    
    public override void Initialize()
    {
        base.Initialize();
        
        // Create sprite batch for rendering
        _spriteBatch = new SpriteBatch(GL);
        
        // Create camera centered on the room
        int roomPixelWidth = ROOM_WIDTH * TILE_SIZE;
        int roomPixelHeight = ROOM_HEIGHT * TILE_SIZE;
        _camera = new Camera2D(1280, 720);
        _camera.Position = new Vector2(roomPixelWidth / 2f, roomPixelHeight / 2f);
        _camera.Zoom = 3.0f; // Zoom in for pixel art
        
        // Create white pixel texture for drawing primitives
        _whitePixel = Texture2D.CreateSolidColor(GL, 1, 1, 255, 255, 255, 255);
        
        // Initialize player at center of room
        _playerPosition = new Vector2(roomPixelWidth / 2f, roomPixelHeight / 2f);
        _playerBounds = new Rectangle((int)_playerPosition.X - 8, (int)_playerPosition.Y - 8, 16, 16);
        
        // Setup room layout
        InitializeRoom();
        
        Console.WriteLine("✅ Farmhouse scene initialized");
        Console.WriteLine($"   Room size: {ROOM_WIDTH}x{ROOM_HEIGHT} tiles ({roomPixelWidth}x{roomPixelHeight} pixels)");
        Console.WriteLine($"   Player position: {_playerPosition}");
    }
    
    private void InitializeRoom()
    {
        // Create floor tiles
        for (int y = 0; y < ROOM_HEIGHT; y++)
        {
            for (int x = 0; x < ROOM_WIDTH; x++)
            {
                // Walls on the edges
                if (x == 0 || x == ROOM_WIDTH - 1 || y == 0 || y == ROOM_HEIGHT - 1)
                {
                    // Door at bottom center
                    if (y == ROOM_HEIGHT - 1 && x == ROOM_WIDTH / 2)
                    {
                        _tiles[x, y] = new Tile(TileType.Door, new Color(139, 69, 19)); // Brown door
                    }
                    else
                    {
                        _tiles[x, y] = new Tile(TileType.Wall, new Color(101, 67, 33)); // Dark brown wall
                    }
                }
                else
                {
                    // Wooden floor with slight variation
                    int shade = 180 + (x + y) % 20;
                    _tiles[x, y] = new Tile(TileType.Floor, new Color((byte)shade, (byte)(shade - 50), (byte)80, (byte)255));
                }
            }
        }
        
        // Add furniture
        // Bed (top right)
        _furniture.Add(new FurnitureObject(
            "Bed",
            new Rectangle(10 * TILE_SIZE, 2 * TILE_SIZE, 3 * TILE_SIZE, 2 * TILE_SIZE),
            new Color(200, 50, 50) // Red bed
        ));
        
        // Table (center left)
        _furniture.Add(new FurnitureObject(
            "Table",
            new Rectangle(3 * TILE_SIZE, 5 * TILE_SIZE, 2 * TILE_SIZE, 2 * TILE_SIZE),
            new Color(139, 90, 43) // Wood table
        ));
        
        // Chair (near table)
        _furniture.Add(new FurnitureObject(
            "Chair",
            new Rectangle(3 * TILE_SIZE, 7 * TILE_SIZE, TILE_SIZE, TILE_SIZE),
            new Color(120, 80, 40) // Darker wood
        ));
        
        // Dresser (top left)
        _furniture.Add(new FurnitureObject(
            "Dresser",
            new Rectangle(2 * TILE_SIZE, 2 * TILE_SIZE, 2 * TILE_SIZE, TILE_SIZE),
            new Color(101, 67, 33) // Dark wood
        ));
        
        // Rug (center)
        _furniture.Add(new FurnitureObject(
            "Rug",
            new Rectangle(6 * TILE_SIZE, 5 * TILE_SIZE, 3 * TILE_SIZE, 3 * TILE_SIZE),
            new Color(150, 100, 150) // Purple rug
        ));
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        if (_inputManager == null) return;
        
        float deltaTime = (float)gameTime.DeltaTime;
        
        // Handle player movement with WASD or arrow keys
        _playerVelocity = Vector2.Zero;
        
        if (_inputManager.IsKeyDown(Key.W) || _inputManager.IsKeyDown(Key.Up))
            _playerVelocity.Y -= 1;
        if (_inputManager.IsKeyDown(Key.S) || _inputManager.IsKeyDown(Key.Down))
            _playerVelocity.Y += 1;
        if (_inputManager.IsKeyDown(Key.A) || _inputManager.IsKeyDown(Key.Left))
            _playerVelocity.X -= 1;
        if (_inputManager.IsKeyDown(Key.D) || _inputManager.IsKeyDown(Key.Right))
            _playerVelocity.X += 1;
        
        // Normalize diagonal movement
        if (_playerVelocity.Length() > 0)
        {
            _playerVelocity = Vector2.Normalize(_playerVelocity);
        }
        
        // Apply velocity to position
        Vector2 newPosition = _playerPosition + _playerVelocity * _playerSpeed * deltaTime;
        
        // Simple collision detection with room bounds
        int roomPixelWidth = ROOM_WIDTH * TILE_SIZE;
        int roomPixelHeight = ROOM_HEIGHT * TILE_SIZE;
        
        // Keep player inside room (with padding for walls)
        newPosition.X = System.Math.Clamp(newPosition.X, TILE_SIZE + 8, roomPixelWidth - TILE_SIZE - 8);
        newPosition.Y = System.Math.Clamp(newPosition.Y, TILE_SIZE + 8, roomPixelHeight - TILE_SIZE - 8);
        
        // Simple furniture collision (just check bounds)
        Rectangle newBounds = new Rectangle((int)newPosition.X - 8, (int)newPosition.Y - 8, 16, 16);
        bool collidesWithFurniture = false;
        
        foreach (var furniture in _furniture)
        {
            if (furniture.Bounds.Intersects(newBounds))
            {
                collidesWithFurniture = true;
                break;
            }
        }
        
        // Only update position if no collision
        if (!collidesWithFurniture)
        {
            _playerPosition = newPosition;
            _playerBounds = newBounds;
        }
        
        // Update camera to follow player (smooth follow)
        if (_camera != null)
        {
            Vector2 targetCameraPos = _playerPosition;
            _camera.Position = Vector2.Lerp(_camera.Position, targetCameraPos, deltaTime * 5f);
        }
    }
    
    public override void Render(GameTime gameTime)
    {
        base.Render(gameTime);
        
        if (_spriteBatch == null || _whitePixel == null || _camera == null) return;
        
        // Clear to black
        GL.ClearColor(0.05f, 0.05f, 0.05f, 1.0f);
        GL.Clear(ClearBufferMask.ColorBufferBit);
        
        // Begin rendering with camera
        _spriteBatch.Begin(_camera);
        
        // Draw floor and walls
        for (int y = 0; y < ROOM_HEIGHT; y++)
        {
            for (int x = 0; x < ROOM_WIDTH; x++)
            {
                Tile tile = _tiles[x, y];
                Rectangle destRect = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                _spriteBatch.Draw(_whitePixel, destRect, tile.Color);
            }
        }
        
        // Draw furniture (sorted by Y position for depth)
        foreach (var furniture in _furniture.OrderBy(f => f.Bounds.Y))
        {
            _spriteBatch.Draw(_whitePixel, furniture.Bounds, furniture.Color);
            
            // Draw darker outline
            DrawRectangleOutline(_spriteBatch, _whitePixel, furniture.Bounds, 
                new Color((byte)(furniture.Color.R / 2), (byte)(furniture.Color.G / 2), (byte)(furniture.Color.B / 2)));
        }
        
        // Draw player (as a simple colored square for now)
        _spriteBatch.Draw(_whitePixel, _playerBounds, new Color(255, 200, 100)); // Light skin tone
        
        // Draw player outline
        DrawRectangleOutline(_spriteBatch, _whitePixel, _playerBounds, new Color(0, 0, 0));
        
        _spriteBatch.End();
        
        // Draw UI overlay (without camera)
        _spriteBatch.Begin();
        
        // Draw instructions
        DrawText(_spriteBatch, _whitePixel, "FARMHOUSE INTERIOR - First Scene", 10, 10, new Color(255, 255, 255));
        DrawText(_spriteBatch, _whitePixel, "WASD/Arrows: Move", 10, 30, new Color(200, 200, 200));
        DrawText(_spriteBatch, _whitePixel, "ESC: Exit", 10, 50, new Color(200, 200, 200));
        DrawText(_spriteBatch, _whitePixel, $"Position: ({(int)_playerPosition.X}, {(int)_playerPosition.Y})", 10, 80, new Color(150, 150, 150));
        
        _spriteBatch.End();
    }
    
    private void DrawRectangleOutline(SpriteBatch batch, Texture2D pixel, Rectangle rect, Color color)
    {
        // Top
        batch.Draw(pixel, new Rectangle(rect.X, rect.Y, rect.Width, 1), color);
        // Bottom
        batch.Draw(pixel, new Rectangle(rect.X, rect.Y + rect.Height - 1, rect.Width, 1), color);
        // Left
        batch.Draw(pixel, new Rectangle(rect.X, rect.Y, 1, rect.Height), color);
        // Right
        batch.Draw(pixel, new Rectangle(rect.X + rect.Width - 1, rect.Y, 1, rect.Height), color);
    }
    
    private void DrawText(SpriteBatch batch, Texture2D pixel, string text, int x, int y, Color color)
    {
        // Simple pixel font rendering (each character is 6x8 pixels)
        int charWidth = 6;
        int charHeight = 8;
        int currentX = x;
        
        foreach (char c in text)
        {
            if (c == ' ')
            {
                currentX += charWidth;
                continue;
            }
            
            // Draw a simple filled rectangle for each character
            // In a real implementation, this would use a bitmap font
            batch.Draw(pixel, new Rectangle(currentX, y, charWidth - 1, charHeight), color);
            currentX += charWidth;
        }
    }
    
    public override void Dispose()
    {
        _whitePixel?.Dispose();
        _playerTexture?.Dispose();
        _spriteBatch?.Dispose();
        base.Dispose();
    }
}
