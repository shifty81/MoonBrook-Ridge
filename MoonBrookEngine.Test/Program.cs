using MoonBrookEngine.Core;
using MoonBrookEngine.Graphics;
using MoonBrookEngine.Math;
using Silk.NET.OpenGL;

namespace MoonBrookEngine.Test;

/// <summary>
/// Simple test application to demonstrate the engine
/// Renders a colored quad to the screen
/// </summary>
public class TestGame
{
    private Engine _engine;
    private Graphics.Shader? _shader;
    private uint _vao;
    private uint _vbo;
    private Texture2D? _whiteTexture;
    
    public TestGame()
    {
        _engine = new Engine("MoonBrook Engine - Test", 1280, 720);
        
        _engine.OnInitialize += Initialize;
        _engine.OnUpdate += Update;
        _engine.OnRender += Render;
        _engine.OnShutdown += Shutdown;
    }
    
    public void Run()
    {
        _engine.Run();
    }
    
    private unsafe void Initialize()
    {
        Console.WriteLine("Test game initializing...");
        
        // Create a simple vertex and fragment shader
        string vertexShader = @"
            #version 330 core
            layout (location = 0) in vec2 aPosition;
            layout (location = 1) in vec2 aTexCoord;
            layout (location = 2) in vec4 aColor;
            
            out vec2 TexCoord;
            out vec4 Color;
            
            void main()
            {
                gl_Position = vec4(aPosition, 0.0, 1.0);
                TexCoord = aTexCoord;
                Color = aColor;
            }
        ";
        
        string fragmentShader = @"
            #version 330 core
            in vec2 TexCoord;
            in vec4 Color;
            out vec4 FragColor;
            
            uniform sampler2D uTexture;
            
            void main()
            {
                FragColor = texture(uTexture, TexCoord) * Color;
            }
        ";
        
        _shader = new Graphics.Shader(_engine.GL, vertexShader, fragmentShader);
        
        // Create a white 1x1 texture for colored quads
        _whiteTexture = Texture2D.CreateSolidColor(_engine.GL, 1, 1, 255, 255, 255, 255);
        
        // Create a colored quad (vertex format: x, y, u, v, r, g, b, a)
        float[] vertices = new float[]
        {
            // Position        // TexCoord  // Color (RGBA normalized)
            -0.5f, -0.5f,      0.0f, 0.0f,  1.0f, 0.0f, 0.0f, 1.0f,  // Bottom-left (red)
             0.5f, -0.5f,      1.0f, 0.0f,  0.0f, 1.0f, 0.0f, 1.0f,  // Bottom-right (green)
             0.5f,  0.5f,      1.0f, 1.0f,  0.0f, 0.0f, 1.0f, 1.0f,  // Top-right (blue)
            -0.5f,  0.5f,      0.0f, 1.0f,  1.0f, 1.0f, 0.0f, 1.0f,  // Top-left (yellow)
        };
        
        // Create VAO and VBO
        _vao = _engine.GL.GenVertexArray();
        _vbo = _engine.GL.GenBuffer();
        
        _engine.GL.BindVertexArray(_vao);
        _engine.GL.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        
        unsafe
        {
            fixed (float* ptr = vertices)
            {
                _engine.GL.BufferData(BufferTargetARB.ArrayBuffer, (nuint)(vertices.Length * sizeof(float)), ptr, BufferUsageARB.StaticDraw);
            }
        }
        
        // Position attribute (location 0)
        _engine.GL.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)0);
        _engine.GL.EnableVertexAttribArray(0);
        
        // TexCoord attribute (location 1)
        _engine.GL.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(2 * sizeof(float)));
        _engine.GL.EnableVertexAttribArray(1);
        
        // Color attribute (location 2)
        _engine.GL.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false, 8 * sizeof(float), (void*)(4 * sizeof(float)));
        _engine.GL.EnableVertexAttribArray(2);
        
        _engine.GL.BindVertexArray(0);
        
        Console.WriteLine("Test game initialized successfully!");
        Console.WriteLine("You should see a colored quad on the screen.");
        Console.WriteLine("Press ESC to close.");
    }
    
    private void Update(GameTime gameTime)
    {
        // Check for ESC key to exit
        var keyboards = _engine.Input.Keyboards;
        if (keyboards.Count > 0)
        {
            var keyboard = keyboards[0];
            if (keyboard.IsKeyPressed(Silk.NET.Input.Key.Escape))
            {
                _engine.Stop();
            }
        }
        
        // Display FPS every second
        if ((int)gameTime.TotalSeconds % 1 == 0 && gameTime.TotalSeconds > 0 && (int)(gameTime.TotalSeconds * 10) % 10 == 0)
        {
            Console.WriteLine($"FPS: {gameTime.FPS:F2}");
        }
    }
    
    private void Render(GameTime gameTime)
    {
        if (_shader == null || _whiteTexture == null) return;
        
        // Use shader and bind texture
        _shader.Use();
        _whiteTexture.Bind(0);
        _shader.SetUniform("uTexture", 0);
        
        // Draw the quad
        _engine.GL.BindVertexArray(_vao);
        _engine.GL.DrawArrays(PrimitiveType.TriangleFan, 0, 4);
        _engine.GL.BindVertexArray(0);
    }
    
    private void Shutdown()
    {
        Console.WriteLine("Test game shutting down...");
        
        _shader?.Dispose();
        _whiteTexture?.Dispose();
        
        if (_vao != 0) _engine.GL.DeleteVertexArray(_vao);
        if (_vbo != 0) _engine.GL.DeleteBuffer(_vbo);
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
