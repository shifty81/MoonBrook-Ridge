using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.World;

/// <summary>
/// Represents a decorative or functional object in the game world (buildings, trees, rocks, etc.)
/// </summary>
public class WorldObject
{
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Texture2D Texture { get; set; }
    public Rectangle SourceRectangle { get; set; }
    public Vector2 Origin { get; set; }
    public float Scale { get; set; } = 1.0f;
    public Color Tint { get; set; } = Color.White;
    public int Layer { get; set; } = 1; // 0=Ground, 1=Objects, 2=Characters
    public bool IsVisible { get; set; } = true;
    
    public WorldObject(string name, Vector2 position, Texture2D texture)
    {
        if (texture == null)
            throw new ArgumentNullException(nameof(texture), "Texture cannot be null");
            
        Name = name;
        Position = position;
        Texture = texture;
        SourceRectangle = new Rectangle(0, 0, texture.Width, texture.Height);
        Origin = Vector2.Zero;
    }
    
    /// <summary>
    /// Constructor for creating a world object from extracted sprite info
    /// </summary>
    public WorldObject(string name, Vector2 position, SpriteInfo spriteInfo)
    {
        if (spriteInfo?.Texture == null)
            throw new ArgumentNullException(nameof(spriteInfo), "SpriteInfo and its Texture cannot be null");
            
        Name = name;
        Position = position;
        Texture = spriteInfo.Texture;
        SourceRectangle = spriteInfo.SourceRectangle;
        Origin = Vector2.Zero;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (IsVisible && Texture != null)
        {
            spriteBatch.Draw(
                texture: Texture,
                position: Position,
                sourceRectangle: SourceRectangle,
                color: Tint,
                rotation: 0f,
                origin: Origin,
                scale: Scale,
                effects: SpriteEffects.None,
                layerDepth: 0f
            );
        }
    }
    
    /// <summary>
    /// Get the bounding rectangle for this object
    /// </summary>
    public Rectangle GetBounds()
    {
        return new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            (int)(SourceRectangle.Width * Scale),
            (int)(SourceRectangle.Height * Scale)
        );
    }
}
