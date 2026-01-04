using System.Numerics;

namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible SpriteBatch wrapper for MoonBrookEngine
/// Provides a familiar API for drawing sprites
/// </summary>
public class SpriteBatch : IDisposable
{
    private MoonBrookEngine.Graphics.SpriteBatch _engineBatch;
    private bool _isBegun;
    
    public SpriteBatch(MoonBrookEngine.Graphics.SpriteBatch engineBatch)
    {
        _engineBatch = engineBatch;
        _isBegun = false;
    }
    
    public SpriteBatch(GraphicsDevice graphicsDevice)
    {
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
    
    // DrawString for text rendering (will need font implementation)
    public void DrawString(
        SpriteFont font,
        string text,
        Vector2 position,
        Color color)
    {
        // TODO: Implement font rendering
        // For now, this is a stub
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
        // TODO: Implement font rendering with transforms
        // For now, this is a stub
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
