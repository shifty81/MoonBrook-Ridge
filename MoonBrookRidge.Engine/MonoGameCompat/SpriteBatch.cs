using System.Numerics;

namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible SpriteBatch wrapper for MoonBrookEngine
/// Provides a familiar API for drawing sprites
/// </summary>
public class SpriteBatch : IDisposable
{
    private MoonBrookEngine.Graphics.SpriteBatch _engineBatch;
    private GraphicsDevice _graphicsDevice;
    private bool _isBegun;
    
    public GraphicsDevice GraphicsDevice => _graphicsDevice;
    
    public SpriteBatch(MoonBrookEngine.Graphics.SpriteBatch engineBatch)
    {
        _engineBatch = engineBatch;
        _graphicsDevice = null!; // Will be set when needed
        _isBegun = false;
    }
    
    public SpriteBatch(GraphicsDevice graphicsDevice)
    {
        _graphicsDevice = graphicsDevice;
        var gl = graphicsDevice.GetInternalGL();
        _engineBatch = new MoonBrookEngine.Graphics.SpriteBatch(gl);
        _isBegun = false;
    }
    
    public void Begin()
    {
        if (_isBegun)
            throw new InvalidOperationException("Begin cannot be called again until End has been called.");
        
        _engineBatch.Begin();
        _isBegun = true;
    }
    
    /// <summary>
    /// Begin with optional parameters (MonoGame-style overload)
    /// </summary>
    public void Begin(
        SpriteSortMode sortMode = SpriteSortMode.Deferred,
        BlendState? blendState = null,
        SamplerState? samplerState = null,
        DepthStencilState? depthStencilState = null,
        RasterizerState? rasterizerState = null,
        Effect? effect = null,
        Matrix? transformMatrix = null)
    {
        if (_isBegun)
            throw new InvalidOperationException("Begin cannot be called again until End has been called.");
        
        // For now, we ignore most of these parameters and just begin the batch
        // The custom engine doesn't support all these options yet
        // TODO: Implement full parameter support
        _engineBatch.Begin();
        _isBegun = true;
    }
    
    public void Begin(MoonBrookEngine.Graphics.Camera2D? camera)
    {
        if (_isBegun)
            throw new InvalidOperationException("Begin cannot be called again until End has been called.");
        
        _engineBatch.Begin(camera);
        _isBegun = true;
    }
    
    public void End()
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin must be called before End can be called.");
        
        _engineBatch.End();
        _isBegun = false;
    }
    
    // Basic Draw overload - position only
    public void Draw(Texture2D texture, Vector2 position, Color color)
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin must be called before Draw.");
        
        _engineBatch.Draw(
            texture.InternalTexture,
            position,
            null,
            color,
            0f,
            MoonBrookEngine.Math.Vector2.Zero,
            MoonBrookEngine.Math.Vector2.One,
            0f
        );
    }
    
    // Draw with destination rectangle
    public void Draw(Texture2D texture, Rectangle destinationRectangle, Color color)
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin must be called before Draw.");
        
        // Convert destination rectangle to position and scale
        var position = new MoonBrookEngine.Math.Vector2(destinationRectangle.X, destinationRectangle.Y);
        var scale = new MoonBrookEngine.Math.Vector2(
            destinationRectangle.Width / (float)texture.Width,
            destinationRectangle.Height / (float)texture.Height
        );
        
        _engineBatch.Draw(
            texture.InternalTexture,
            position,
            null,
            color,
            0f,
            MoonBrookEngine.Math.Vector2.Zero,
            scale,
            0f
        );
    }
    
    // Draw with source and destination rectangles
    public void Draw(Texture2D texture, Rectangle destinationRectangle, Rectangle? sourceRectangle, Color color)
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin must be called before Draw.");
        
        var position = new MoonBrookEngine.Math.Vector2(destinationRectangle.X, destinationRectangle.Y);
        
        MoonBrookEngine.Math.Rectangle? srcRect = null;
        if (sourceRectangle.HasValue)
        {
            var src = sourceRectangle.Value;
            srcRect = new MoonBrookEngine.Math.Rectangle(src.X, src.Y, src.Width, src.Height);
        }
        
        // Calculate scale based on destination vs source (or texture) size
        float srcWidth = sourceRectangle?.Width ?? texture.Width;
        float srcHeight = sourceRectangle?.Height ?? texture.Height;
        
        var scale = new MoonBrookEngine.Math.Vector2(
            destinationRectangle.Width / srcWidth,
            destinationRectangle.Height / srcHeight
        );
        
        _engineBatch.Draw(
            texture.InternalTexture,
            position,
            srcRect,
            color,
            0f,
            MoonBrookEngine.Math.Vector2.Zero,
            scale,
            0f
        );
    }
    
    // Full Draw overload with destination rectangle and all parameters
    public void Draw(
        Texture2D texture,
        Rectangle destinationRectangle,
        Rectangle? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        SpriteEffects effects,
        float layerDepth)
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin must be called before Draw.");
        
        // Convert destination rectangle to position
        var position = new MoonBrookEngine.Math.Vector2(destinationRectangle.X, destinationRectangle.Y);
        
        MoonBrookEngine.Math.Rectangle? srcRect = null;
        if (sourceRectangle.HasValue)
        {
            var src = sourceRectangle.Value;
            srcRect = new MoonBrookEngine.Math.Rectangle(src.X, src.Y, src.Width, src.Height);
        }
        
        // Calculate scale based on destination vs source (or texture) size
        float srcWidth = sourceRectangle?.Width ?? texture.Width;
        float srcHeight = sourceRectangle?.Height ?? texture.Height;
        
        var scale = new MoonBrookEngine.Math.Vector2(
            destinationRectangle.Width / srcWidth,
            destinationRectangle.Height / srcHeight
        );
        
        // Convert effects to flags (bit field)
        float flipFlags = 0f;
        if ((effects & SpriteEffects.FlipHorizontally) != 0)
            flipFlags += 1f;
        if ((effects & SpriteEffects.FlipVertically) != 0)
            flipFlags += 2f;
        
        _engineBatch.Draw(
            texture.InternalTexture,
            position,
            srcRect,
            color,
            rotation,
            origin,
            scale,
            flipFlags
        );
    }
    
    // Full Draw overload with all parameters
    public void Draw(
        Texture2D texture,
        Vector2 position,
        Rectangle? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        float scale,
        SpriteEffects effects,
        float layerDepth)
    {
        Draw(texture, position, sourceRectangle, color, rotation, origin, new Vector2(scale), effects, layerDepth);
    }
    
    // Full Draw overload with Vector2 scale
    public void Draw(
        Texture2D texture,
        Vector2 position,
        Rectangle? sourceRectangle,
        Color color,
        float rotation,
        Vector2 origin,
        Vector2 scale,
        SpriteEffects effects,
        float layerDepth)
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin must be called before Draw.");
        
        MoonBrookEngine.Math.Rectangle? srcRect = null;
        if (sourceRectangle.HasValue)
        {
            var src = sourceRectangle.Value;
            srcRect = new MoonBrookEngine.Math.Rectangle(src.X, src.Y, src.Width, src.Height);
        }
        
        // Convert effects to flags (bit field)
        float flipFlags = 0f;
        if ((effects & SpriteEffects.FlipHorizontally) != 0)
            flipFlags += 1f;
        if ((effects & SpriteEffects.FlipVertically) != 0)
            flipFlags += 2f;
        
        _engineBatch.Draw(
            texture.InternalTexture,
            position,
            srcRect,
            color,
            rotation,
            origin,
            scale,
            flipFlags
        );
    }
    
    // DrawString for text rendering
    public void DrawString(
        SpriteFont font,
        string text,
        Vector2 position,
        Color color)
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin must be called before DrawString.");
        
        if (font?.InternalFont == null)
            return; // Can't render without internal font
        
        _engineBatch.DrawString(font.InternalFont, text, position, color);
    }
    
    public void DrawString(
        SpriteFont font,
        string text,
        Vector2 position,
        Color color,
        float rotation,
        Vector2 origin,
        float scale,
        SpriteEffects effects,
        float layerDepth)
    {
        if (!_isBegun)
            throw new InvalidOperationException("Begin must be called before DrawString.");
        
        // For now, ignore rotation, origin, scale, effects, and layerDepth
        // Just render the text at the position
        // TODO: Implement full text transform support
        if (font?.InternalFont != null)
        {
            _engineBatch.DrawString(font.InternalFont, text, position, color);
        }
    }
    
    public void Dispose()
    {
        _engineBatch?.Dispose();
    }
}

/// <summary>
/// Effects that can be applied when drawing sprites
/// </summary>
[Flags]
public enum SpriteEffects
{
    None = 0,
    FlipHorizontally = 1,
    FlipVertically = 2
}

/// <summary>
/// Defines sprite sort rendering options
/// </summary>
public enum SpriteSortMode
{
    Deferred,
    Immediate,
    Texture,
    BackToFront,
    FrontToBack
}

/// <summary>
/// Defines blend modes for rendering
/// </summary>
public class BlendState
{
    public static readonly BlendState Opaque = new BlendState();
    public static readonly BlendState AlphaBlend = new BlendState();
    public static readonly BlendState Additive = new BlendState();
    public static readonly BlendState NonPremultiplied = new BlendState();
}

/// <summary>
/// Defines depth and stencil state
/// </summary>
public class DepthStencilState
{
    public static readonly DepthStencilState Default = new DepthStencilState();
    public static readonly DepthStencilState DepthRead = new DepthStencilState();
    public static readonly DepthStencilState None = new DepthStencilState();
}

/// <summary>
/// Defines rasterizer state
/// </summary>
public class RasterizerState
{
    public static readonly RasterizerState CullClockwise = new RasterizerState();
    public static readonly RasterizerState CullCounterClockwise = new RasterizerState();
    public static readonly RasterizerState CullNone = new RasterizerState();
}

/// <summary>
/// Defines shader effects (stub for compatibility)
/// </summary>
public class Effect : IDisposable
{
    public void Dispose() { }
}
