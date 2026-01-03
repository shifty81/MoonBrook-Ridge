using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonBrookRidge.World.Interiors;

/// <summary>
/// Represents a furniture or decoration object in an interior
/// </summary>
public class InteriorObject
{
    public Vector2 Position { get; set; }
    public Rectangle SourceRect { get; set; }
    public bool IsBlocking { get; set; } // Whether player can walk through it
    public string Name { get; set; }
    
    public InteriorObject(string name, Vector2 position, Rectangle sourceRect, bool isBlocking = true)
    {
        Name = name;
        Position = position;
        SourceRect = sourceRect;
        IsBlocking = isBlocking;
    }
    
    /// <summary>
    /// Draw the interior object
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Texture2D atlas)
    {
        spriteBatch.Draw(
            atlas,
            Position,
            SourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            1f,
            SpriteEffects.None,
            0f
        );
    }
    
    /// <summary>
    /// Get collision bounds for this object
    /// </summary>
    public Rectangle GetBounds()
    {
        return new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            SourceRect.Width,
            SourceRect.Height
        );
    }
}
