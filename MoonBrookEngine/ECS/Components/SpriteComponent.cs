using MoonBrookEngine.Graphics;
using MoonBrookEngine.Math;

namespace MoonBrookEngine.ECS.Components;

/// <summary>
/// Sprite component for rendering 2D sprites
/// </summary>
public class SpriteComponent : Component
{
    public Texture2D? Texture { get; set; }
    public Rectangle? SourceRect { get; set; } // Null = entire texture
    public Color Tint { get; set; }
    public float LayerDepth { get; set; }
    
    public SpriteComponent()
    {
        Tint = Color.White;
        LayerDepth = 0f;
    }
    
    public SpriteComponent(Texture2D texture)
    {
        Texture = texture;
        Tint = Color.White;
        LayerDepth = 0f;
    }
    
    public SpriteComponent(Texture2D texture, Color tint)
    {
        Texture = texture;
        Tint = tint;
        LayerDepth = 0f;
    }
}
