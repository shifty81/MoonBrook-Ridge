using System;
using System.Collections.Generic;
using System.Linq;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Manages sprite rendering with proper Z-ordering based on Y position
/// </summary>
public class RenderingSystem
{
    private List<Renderable> _renderables;
    private SpriteBatch _spriteBatch;
    
    public RenderingSystem(SpriteBatch spriteBatch)
    {
        _spriteBatch = spriteBatch;
        _renderables = new List<Renderable>();
    }
    
    /// <summary>
    /// Register an object to be rendered
    /// </summary>
    public void Register(Renderable renderable)
    {
        if (!_renderables.Contains(renderable))
        {
            _renderables.Add(renderable);
        }
    }
    
    /// <summary>
    /// Unregister an object from rendering
    /// </summary>
    public void Unregister(Renderable renderable)
    {
        _renderables.Remove(renderable);
    }
    
    /// <summary>
    /// Draw all registered objects with proper Z-ordering
    /// Objects with higher Y position are drawn last (on top)
    /// </summary>
    public void Draw(Camera2D camera)
    {
        // Sort by Y position (and then by layer for same Y)
        var sorted = _renderables
            .OrderBy(r => r.Layer)
            .ThenBy(r => r.Position.Y)
            .ToList();
        
        _spriteBatch.Begin(
            sortMode: SpriteSortMode.Deferred,
            blendState: BlendState.AlphaBlend,
            samplerState: SamplerState.PointClamp,
            transformMatrix: camera.GetTransform()
        );
        
        foreach (var renderable in sorted)
        {
            if (renderable.IsVisible)
            {
                renderable.Draw(_spriteBatch);
            }
        }
        
        _spriteBatch.End();
    }
    
    /// <summary>
    /// Clear all registered renderables
    /// </summary>
    public void Clear()
    {
        _renderables.Clear();
    }
}

/// <summary>
/// Base class for any object that can be rendered with Z-ordering
/// </summary>
public abstract class Renderable
{
    public Vector2 Position { get; set; }
    public int Layer { get; set; } // 0=Ground, 1=Objects, 2=Characters, 3=Effects
    public bool IsVisible { get; set; } = true;
    
    public abstract void Draw(SpriteBatch spriteBatch);
    
    /// <summary>
    /// Calculate depth value for sprite sorting (0.0 = back, 1.0 = front)
    /// </summary>
    protected float GetDepth(float worldHeight)
    {
        // Normalize Y position to 0-1 range based on world height
        float depth = Position.Y / worldHeight;
        return MathHelper.Clamp(depth, 0f, 1f);
    }
}

/// <summary>
/// Sprite renderable with animation support
/// </summary>
public class SpriteRenderable : Renderable
{
    public Texture2D Texture { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public Color Tint { get; set; } = Color.White;
    public Vector2 Origin { get; set; }
    public float Rotation { get; set; }
    public Vector2 Scale { get; set; } = Vector2.One;
    public SpriteEffects Effects { get; set; } = SpriteEffects.None;
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        if (Texture != null)
        {
            spriteBatch.Draw(
                texture: Texture,
                position: Position,
                sourceRectangle: SourceRectangle != Rectangle.Empty ? SourceRectangle : null,
                color: Tint,
                rotation: Rotation,
                origin: Origin,
                scale: Scale,
                effects: Effects,
                layerDepth: 0f
            );
        }
    }
}
