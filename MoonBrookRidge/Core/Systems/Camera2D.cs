using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// 2D camera with smooth following and zoom
/// </summary>
public class Camera2D
{
    private Vector2 _position;
    private float _zoom;
    private readonly Viewport _viewport;
    
    public Camera2D(Viewport viewport)
    {
        _viewport = viewport;
        _zoom = 2.0f; // Pixel art looks better with integer zoom
        _position = Vector2.Zero;
    }
    
    public void Follow(Vector2 targetPosition)
    {
        // Center camera on target
        _position = targetPosition - new Vector2(_viewport.Width / 2f / _zoom, _viewport.Height / 2f / _zoom);
    }
    
    public Matrix GetTransform()
    {
        return Matrix.CreateTranslation(-_position.X, -_position.Y, 0) *
               Matrix.CreateScale(_zoom, _zoom, 1);
    }
    
    /// <summary>
    /// Converts screen coordinates to world coordinates
    /// </summary>
    public Vector2 ScreenToWorld(Vector2 screenPosition)
    {
        // Inverse transform: screen -> world
        // First undo scaling, then undo translation
        Vector2 worldPos = new Vector2(
            screenPosition.X / _zoom + _position.X,
            screenPosition.Y / _zoom + _position.Y
        );
        return worldPos;
    }
    
    public Vector2 Position
    {
        get => _position;
        set => _position = value;
    }
    
    public float Zoom
    {
        get => _zoom;
        set => _zoom = MathHelper.Clamp(value, 0.5f, 4f);
    }
    
    /// <summary>
    /// Check if a world position is visible in the camera view
    /// </summary>
    public bool IsInView(Vector2 worldPosition, int objectWidth, int objectHeight)
    {
        // Calculate visible bounds
        Rectangle viewBounds = new Rectangle(
            (int)_position.X,
            (int)_position.Y,
            (int)(_viewport.Width / _zoom),
            (int)(_viewport.Height / _zoom)
        );
        
        // Check if object intersects with view bounds
        Rectangle objectBounds = new Rectangle(
            (int)worldPosition.X,
            (int)worldPosition.Y,
            objectWidth,
            objectHeight
        );
        
        return viewBounds.Intersects(objectBounds);
    }
}
