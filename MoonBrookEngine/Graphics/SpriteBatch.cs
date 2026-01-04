using System.Numerics;
using Silk.NET.OpenGL;
using Vec2 = MoonBrookEngine.Math.Vector2;
using Rect = MoonBrookEngine.Math.Rectangle;
using Col = MoonBrookEngine.Math.Color;

namespace MoonBrookEngine.Graphics;

/// <summary>
/// Efficient sprite batch renderer that batches multiple sprites into single draw calls
/// </summary>
public class SpriteBatch : IDisposable
{
    private const int MaxBatchSize = 2048; // Maximum sprites per batch
    private const int VerticesPerSprite = 4;
    private const int IndicesPerSprite = 6;
    private const int FloatsPerVertex = 8; // x, y, u, v, r, g, b, a
    
    private readonly GL _gl;
    private readonly Shader _shader;
    private uint _vao;
    private uint _vbo;
    private uint _ebo;
    
    private float[] _vertexData;
    private int _spriteCount;
    private Texture2D? _currentTexture;
    private bool _isBegun;
    private Camera2D? _camera;
    private Matrix4x4 _transformMatrix;
    private Core.PerformanceMonitor? _performanceMonitor;
    private Texture2D? _whitePixelTexture;
    
    public SpriteBatch(GL gl, Core.PerformanceMonitor? performanceMonitor = null)
    {
        _gl = gl;
        _performanceMonitor = performanceMonitor;
        _vertexData = new float[MaxBatchSize * VerticesPerSprite * FloatsPerVertex];
        _spriteCount = 0;
        _isBegun = false;
        _transformMatrix = Matrix4x4.Identity;
        
        // Create default shader
        string vertexShader = @"
            #version 330 core
            layout (location = 0) in vec2 aPosition;
            layout (location = 1) in vec2 aTexCoord;
            layout (location = 2) in vec4 aColor;
            
            out vec2 TexCoord;
            out vec4 Color;
            
            uniform mat4 uProjection;
            uniform mat4 uView;
            
            void main()
            {
                gl_Position = uProjection * uView * vec4(aPosition, 0.0, 1.0);
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
        
        _shader = new Shader(_gl, vertexShader, fragmentShader);
        
        InitializeBuffers();
        CreateWhitePixelTexture();
    }
    
    private unsafe void CreateWhitePixelTexture()
    {
        // Create a 1x1 white pixel texture for solid color rendering
        byte[] pixelData = new byte[] { 255, 255, 255, 255 };
        _whitePixelTexture = new Texture2D(_gl, pixelData, 1, 1);
    }
    
    private unsafe void InitializeBuffers()
    {
        // Create VAO
        _vao = _gl.GenVertexArray();
        _gl.BindVertexArray(_vao);
        
        // Create VBO
        _vbo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        _gl.BufferData(BufferTargetARB.ArrayBuffer, 
            (nuint)(_vertexData.Length * sizeof(float)), 
            null, 
            BufferUsageARB.DynamicDraw);
        
        // Create EBO (index buffer)
        _ebo = _gl.GenBuffer();
        _gl.BindBuffer(BufferTargetARB.ElementArrayBuffer, _ebo);
        
        // Generate indices for quads (0,1,2, 2,3,0 pattern)
        uint[] indices = new uint[MaxBatchSize * IndicesPerSprite];
        for (uint i = 0; i < MaxBatchSize; i++)
        {
            uint offset = i * VerticesPerSprite;
            uint indexOffset = i * IndicesPerSprite;
            
            indices[indexOffset + 0] = offset + 0;
            indices[indexOffset + 1] = offset + 1;
            indices[indexOffset + 2] = offset + 2;
            indices[indexOffset + 3] = offset + 2;
            indices[indexOffset + 4] = offset + 3;
            indices[indexOffset + 5] = offset + 0;
        }
        
        fixed (uint* ptr = indices)
        {
            _gl.BufferData(BufferTargetARB.ElementArrayBuffer,
                (nuint)(indices.Length * sizeof(uint)),
                ptr,
                BufferUsageARB.StaticDraw);
        }
        
        // Position attribute (location 0)
        _gl.VertexAttribPointer(0, 2, VertexAttribPointerType.Float, false, 
            FloatsPerVertex * sizeof(float), (void*)0);
        _gl.EnableVertexAttribArray(0);
        
        // TexCoord attribute (location 1)
        _gl.VertexAttribPointer(1, 2, VertexAttribPointerType.Float, false,
            FloatsPerVertex * sizeof(float), (void*)(2 * sizeof(float)));
        _gl.EnableVertexAttribArray(1);
        
        // Color attribute (location 2)
        _gl.VertexAttribPointer(2, 4, VertexAttribPointerType.Float, false,
            FloatsPerVertex * sizeof(float), (void*)(4 * sizeof(float)));
        _gl.EnableVertexAttribArray(2);
        
        _gl.BindVertexArray(0);
    }
    
    public void Begin(Camera2D? camera = null, Matrix4x4? transformMatrix = null)
    {
        if (_isBegun)
        {
            throw new InvalidOperationException("Begin cannot be called until End has been called.");
        }
        
        _isBegun = true;
        _camera = camera;
        _transformMatrix = transformMatrix ?? Matrix4x4.Identity;
        _spriteCount = 0;
        _currentTexture = null;
    }
    
    public void End()
    {
        if (!_isBegun)
        {
            throw new InvalidOperationException("End cannot be called before Begin.");
        }
        
        Flush();
        _isBegun = false;
    }
    
    public void Draw(Texture2D texture, Vec2 position, Col color)
    {
        Draw(texture, position, null, color, 0f, Vec2.Zero, Vec2.One, 0f);
    }
    
    public void Draw(Texture2D texture, Vec2 position, Rect? sourceRectangle, Col color)
    {
        Draw(texture, position, sourceRectangle, color, 0f, Vec2.Zero, Vec2.One, 0f);
    }
    
    public void Draw(Texture2D texture, Rect destinationRectangle, Col color)
    {
        Draw(texture, new Vec2(destinationRectangle.X, destinationRectangle.Y), 
            null, color, 0f, Vec2.Zero, 
            new Vec2(destinationRectangle.Width / texture.Width, 
                       destinationRectangle.Height / texture.Height), 0f);
    }
    
    public void Draw(
        Texture2D texture,
        Vec2 position,
        Rect? sourceRectangle,
        Col color,
        float rotation,
        Vec2 origin,
        Vec2 scale,
        float layerDepth)
    {
        if (!_isBegun)
        {
            throw new InvalidOperationException("Begin must be called before Draw.");
        }
        
        // Flush if texture changes or batch is full
        if (_currentTexture != texture || _spriteCount >= MaxBatchSize)
        {
            Flush();
            _currentTexture = texture;
        }
        
        // Calculate source rectangle (texture coordinates)
        float srcX = 0f, srcY = 0f, srcW = texture.Width, srcH = texture.Height;
        if (sourceRectangle.HasValue)
        {
            Rect src = sourceRectangle.Value;
            srcX = src.X;
            srcY = src.Y;
            srcW = src.Width;
            srcH = src.Height;
        }
        
        float u0 = srcX / texture.Width;
        float v0 = srcY / texture.Height;
        float u1 = (srcX + srcW) / texture.Width;
        float v1 = (srcY + srcH) / texture.Height;
        
        // Calculate destination quad
        float width = srcW * scale.X;
        float height = srcH * scale.Y;
        
        // Create corners relative to origin
        float x0 = -origin.X * scale.X;
        float y0 = -origin.Y * scale.Y;
        float x1 = x0 + width;
        float y1 = y0 + height;
        
        // Apply rotation if needed
        float cos = MathF.Cos(rotation);
        float sin = MathF.Sin(rotation);
        
        // Transform and translate each corner
        Vec2 p0 = new(
            position.X + x0 * cos - y0 * sin,
            position.Y + x0 * sin + y0 * cos
        );
        Vec2 p1 = new(
            position.X + x1 * cos - y0 * sin,
            position.Y + x1 * sin + y0 * cos
        );
        Vec2 p2 = new(
            position.X + x1 * cos - y1 * sin,
            position.Y + x1 * sin + y1 * cos
        );
        Vec2 p3 = new(
            position.X + x0 * cos - y1 * sin,
            position.Y + x0 * sin + y1 * cos
        );
        
        // Normalize color
        float r = color.R / 255f;
        float g = color.G / 255f;
        float b = color.B / 255f;
        float a = color.A / 255f;
        
        // Add vertex data
        int offset = _spriteCount * VerticesPerSprite * FloatsPerVertex;
        
        // Vertex 0 (bottom-left)
        _vertexData[offset++] = p0.X;
        _vertexData[offset++] = p0.Y;
        _vertexData[offset++] = u0;
        _vertexData[offset++] = v0;
        _vertexData[offset++] = r;
        _vertexData[offset++] = g;
        _vertexData[offset++] = b;
        _vertexData[offset++] = a;
        
        // Vertex 1 (bottom-right)
        _vertexData[offset++] = p1.X;
        _vertexData[offset++] = p1.Y;
        _vertexData[offset++] = u1;
        _vertexData[offset++] = v0;
        _vertexData[offset++] = r;
        _vertexData[offset++] = g;
        _vertexData[offset++] = b;
        _vertexData[offset++] = a;
        
        // Vertex 2 (top-right)
        _vertexData[offset++] = p2.X;
        _vertexData[offset++] = p2.Y;
        _vertexData[offset++] = u1;
        _vertexData[offset++] = v1;
        _vertexData[offset++] = r;
        _vertexData[offset++] = g;
        _vertexData[offset++] = b;
        _vertexData[offset++] = a;
        
        // Vertex 3 (top-left)
        _vertexData[offset++] = p3.X;
        _vertexData[offset++] = p3.Y;
        _vertexData[offset++] = u0;
        _vertexData[offset++] = v1;
        _vertexData[offset++] = r;
        _vertexData[offset++] = g;
        _vertexData[offset++] = b;
        _vertexData[offset++] = a;
        
        _spriteCount++;
    }
    
    private unsafe void Flush()
    {
        if (_spriteCount == 0 || _currentTexture == null)
        {
            return;
        }
        
        // Upload vertex data to GPU
        _gl.BindBuffer(BufferTargetARB.ArrayBuffer, _vbo);
        fixed (float* ptr = _vertexData)
        {
            _gl.BufferSubData(BufferTargetARB.ArrayBuffer, 0,
                (nuint)(_spriteCount * VerticesPerSprite * FloatsPerVertex * sizeof(float)),
                ptr);
        }
        
        // Set up shader uniforms
        _shader.Use();
        _currentTexture.Bind(0);
        _shader.SetUniform("uTexture", 0);
        
        // Set camera matrices
        if (_camera != null)
        {
            _shader.SetUniform("uProjection", _camera.ProjectionMatrix);
            _shader.SetUniform("uView", _camera.ViewMatrix);
        }
        else
        {
            // Use identity matrices if no camera
            _shader.SetUniform("uProjection", Matrix4x4.Identity);
            _shader.SetUniform("uView", Matrix4x4.Identity);
        }
        
        // Draw
        _gl.BindVertexArray(_vao);
        _gl.DrawElements(PrimitiveType.Triangles, 
            (uint)(_spriteCount * IndicesPerSprite), 
            DrawElementsType.UnsignedInt, 
            null);
        _gl.BindVertexArray(0);
        
        // Record draw call
        _performanceMonitor?.RecordDrawCall();
        
        // Reset for next batch
        _spriteCount = 0;
        _currentTexture = null;
    }
    
    /// <summary>
    /// Draw text using a bitmap font
    /// </summary>
    public void DrawString(BitmapFont font, string text, Vec2 position, Col color)
    {
        DrawString(font, text, position, color, 1.0f);
    }
    
    /// <summary>
    /// Draw text using a bitmap font with scale
    /// </summary>
    public void DrawString(BitmapFont font, string text, Vec2 position, Col color, float scale)
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin() must be called before DrawString()");
        
        if (string.IsNullOrEmpty(text) || font == null)
            return;
        
        // If font has no atlas, we can't render (it's a stub font)
        if (!font.HasAtlas)
            return;
        
        var atlasTexture = font.GetAtlasTexture();
        if (atlasTexture == null)
            return;
        
        float x = position.X;
        float y = position.Y;
        
        foreach (char c in text)
        {
            if (c == '\n')
            {
                // Move to next line
                x = position.X;
                y += font.LineSpacing * scale;
                continue;
            }
            
            if (font.TryGetCharacter(c, out var charInfo))
            {
                // Draw character sprite
                Vec2 charPos = new Vec2(
                    x + charInfo.Offset.X * scale,
                    y + charInfo.Offset.Y * scale
                );
                
                Draw(atlasTexture, charPos, charInfo.SourceRect, color, 0f, Vec2.Zero, new Vec2(scale, scale), 0f);
                
                // Advance position
                x += (charInfo.XAdvance + font.Spacing) * scale;
            }
        }
    }
    
    /// <summary>
    /// Draw a filled rectangle
    /// </summary>
    public void DrawRectangle(Rect rectangle, Col color)
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin() must be called before DrawRectangle()");
        
        if (_whitePixelTexture == null)
            return;
        
        Draw(_whitePixelTexture, new Vec2(rectangle.X, rectangle.Y), null, color, 0f, Vec2.Zero, 
            new Vec2(rectangle.Width, rectangle.Height), 0f);
    }
    
    /// <summary>
    /// Draw a rectangle outline
    /// </summary>
    public void DrawRectangleOutline(Rect rectangle, Col color, float thickness = 1f)
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin() must be called before DrawRectangleOutline()");
        
        if (_whitePixelTexture == null)
            return;
        
        // Top
        Draw(_whitePixelTexture, new Vec2(rectangle.X, rectangle.Y), null, color, 0f, Vec2.Zero,
            new Vec2(rectangle.Width, thickness), 0f);
        
        // Bottom
        Draw(_whitePixelTexture, new Vec2(rectangle.X, rectangle.Y + rectangle.Height - thickness), null, color, 0f, Vec2.Zero,
            new Vec2(rectangle.Width, thickness), 0f);
        
        // Left
        Draw(_whitePixelTexture, new Vec2(rectangle.X, rectangle.Y), null, color, 0f, Vec2.Zero,
            new Vec2(thickness, rectangle.Height), 0f);
        
        // Right
        Draw(_whitePixelTexture, new Vec2(rectangle.X + rectangle.Width - thickness, rectangle.Y), null, color, 0f, Vec2.Zero,
            new Vec2(thickness, rectangle.Height), 0f);
    }
    
    /// <summary>
    /// Draw a particle system
    /// </summary>
    public void DrawParticles(ECS.Components.ParticleComponent particleComponent, Texture2D particleTexture)
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin() must be called before DrawParticles()");
        
        if (particleComponent == null || particleTexture == null)
            return;
        
        // Draw each active particle
        foreach (var particle in particleComponent.Particles)
        {
            if (!particle.IsActive)
                continue;
            
            var position = new Vec2(particle.Position.X, particle.Position.Y);
            var color = particle.Color;
            var scale = new Vec2(particle.Size, particle.Size);
            
            // Draw particle as a sprite
            Draw(particleTexture, position, null, color, particle.Rotation, 
                new Vec2(particleTexture.Width / 2f, particleTexture.Height / 2f), 
                scale, 0f);
        }
    }
    
    public void Dispose()
    {
        _shader?.Dispose();
        _whitePixelTexture?.Dispose();
        if (_vao != 0) _gl.DeleteVertexArray(_vao);
        if (_vbo != 0) _gl.DeleteBuffer(_vbo);
        if (_ebo != 0) _gl.DeleteBuffer(_ebo);
    }
}
