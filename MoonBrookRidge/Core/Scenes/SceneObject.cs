using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.Scenes;

/// <summary>
/// Represents any object in a scene (furniture, decoration, NPC, interactive object, etc.)
/// </summary>
public class SceneObject
{
    public string Name { get; set; }
    public Vector2 Position { get; set; }
    public Rectangle SourceRect { get; set; }
    public bool IsBlocking { get; set; }
    public bool IsInteractive { get; set; }
    public string InteractionText { get; set; }
    public Texture2D Texture { get; set; }
    
    public SceneObject(string name, Vector2 position, bool isBlocking = false, bool isInteractive = false)
    {
        Name = name;
        Position = position;
        IsBlocking = isBlocking;
        IsInteractive = isInteractive;
        InteractionText = string.Empty;
    }
    
    /// <summary>
    /// Update the object
    /// </summary>
    public virtual void Update(GameTime gameTime)
    {
        // Override in derived classes for animated objects
    }
    
    /// <summary>
    /// Draw the object
    /// </summary>
    public virtual void Draw(SpriteBatch spriteBatch)
    {
        if (Texture != null)
        {
            spriteBatch.Draw(
                Texture,
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
    }
    
    /// <summary>
    /// Get collision bounds for this object
    /// </summary>
    public virtual Rectangle GetBounds()
    {
        if (SourceRect != Rectangle.Empty)
        {
            return new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                SourceRect.Width,
                SourceRect.Height
            );
        }
        return new Rectangle((int)Position.X, (int)Position.Y, GameConstants.TILE_SIZE, GameConstants.TILE_SIZE);
    }
    
    /// <summary>
    /// Called when player interacts with this object
    /// </summary>
    public virtual void OnInteract()
    {
        // Override in derived classes
    }
}
