using System.Numerics;
using Vec2 = MoonBrookEngine.Math.Vector2;

namespace MoonBrookEngine.Graphics;

/// <summary>
/// 2D camera with position, zoom, and rotation
/// Provides view and projection matrices for 2D rendering
/// </summary>
public class Camera2D
{
    private Vec2 _position;
    private float _zoom;
    private float _rotation;
    private int _viewportWidth;
    private int _viewportHeight;
    private Matrix4x4 _viewMatrix;
    private Matrix4x4 _projectionMatrix;
    private bool _viewMatrixDirty;
    private bool _projectionMatrixDirty;
    
    public Vec2 Position
    {
        get => _position;
        set
        {
            if (_position != value)
            {
                _position = value;
                _viewMatrixDirty = true;
            }
        }
    }
    
    public float Zoom
    {
        get => _zoom;
        set
        {
            float newZoom = MathF.Max(0.1f, value); // Minimum zoom of 0.1x
            if (_zoom != newZoom)
            {
                _zoom = newZoom;
                _viewMatrixDirty = true;
            }
        }
    }
    
    public float Rotation
    {
        get => _rotation;
        set
        {
            if (_rotation != value)
            {
                _rotation = value;
                _viewMatrixDirty = true;
            }
        }
    }
    
    public int ViewportWidth
    {
        get => _viewportWidth;
        set
        {
            if (_viewportWidth != value)
            {
                _viewportWidth = value;
                _projectionMatrixDirty = true;
            }
        }
    }
    
    public int ViewportHeight
    {
        get => _viewportHeight;
        set
        {
            if (_viewportHeight != value)
            {
                _viewportHeight = value;
                _projectionMatrixDirty = true;
            }
        }
    }
    
    public Matrix4x4 ViewMatrix
    {
        get
        {
            if (_viewMatrixDirty)
            {
                UpdateViewMatrix();
            }
            return _viewMatrix;
        }
    }
    
    public Matrix4x4 ProjectionMatrix
    {
        get
        {
            if (_projectionMatrixDirty)
            {
                UpdateProjectionMatrix();
            }
            return _projectionMatrix;
        }
    }
    
    public Camera2D(int viewportWidth, int viewportHeight)
    {
        _position = Vec2.Zero;
        _zoom = 1.0f;
        _rotation = 0.0f;
        _viewportWidth = viewportWidth;
        _viewportHeight = viewportHeight;
        _viewMatrixDirty = true;
        _projectionMatrixDirty = true;
    }
    
    private void UpdateViewMatrix()
    {
        // Create view matrix: translate to camera position, rotate, then scale by zoom
        // Order: Scale -> Rotate -> Translate (reversed for matrix multiplication)
        
        Matrix4x4 translation = Matrix4x4.CreateTranslation(-_position.X, -_position.Y, 0);
        Matrix4x4 rotation = Matrix4x4.CreateRotationZ(_rotation);
        Matrix4x4 scale = Matrix4x4.CreateScale(_zoom, _zoom, 1.0f);
        
        // Center the camera on the viewport
        Matrix4x4 origin = Matrix4x4.CreateTranslation(_viewportWidth / 2f, _viewportHeight / 2f, 0);
        
        _viewMatrix = translation * rotation * scale * origin;
        _viewMatrixDirty = false;
    }
    
    private void UpdateProjectionMatrix()
    {
        // Orthographic projection for 2D rendering
        _projectionMatrix = Matrix4x4.CreateOrthographicOffCenter(
            0, _viewportWidth,
            _viewportHeight, 0,
            -1, 1
        );
        _projectionMatrixDirty = false;
    }
    
    /// <summary>
    /// Transform a point from world space to screen space
    /// </summary>
    public Vec2 WorldToScreen(Vec2 worldPosition)
    {
        System.Numerics.Vector2 pos = worldPosition;
        System.Numerics.Vector4 worldPos = new(pos.X, pos.Y, 0, 1);
        System.Numerics.Vector4 screenPos = System.Numerics.Vector4.Transform(worldPos, ViewMatrix);
        return new Vec2(screenPos.X, screenPos.Y);
    }
    
    /// <summary>
    /// Transform a point from screen space to world space
    /// </summary>
    public Vec2 ScreenToWorld(Vec2 screenPosition)
    {
        // Invert the view matrix to go from screen to world
        if (Matrix4x4.Invert(ViewMatrix, out Matrix4x4 inverse))
        {
            System.Numerics.Vector2 pos = screenPosition;
            System.Numerics.Vector4 screenPos = new(pos.X, pos.Y, 0, 1);
            System.Numerics.Vector4 worldPos = System.Numerics.Vector4.Transform(screenPos, inverse);
            return new Vec2(worldPos.X, worldPos.Y);
        }
        
        return screenPosition;
    }
    
    /// <summary>
    /// Move the camera by a delta amount
    /// </summary>
    public void Move(Vec2 delta)
    {
        Position += delta;
    }
    
    /// <summary>
    /// Rotate the camera by a delta amount (in radians)
    /// </summary>
    public void Rotate(float deltaRadians)
    {
        Rotation += deltaRadians;
    }
    
    /// <summary>
    /// Look at a specific world position
    /// </summary>
    public void LookAt(Vec2 worldPosition)
    {
        Position = worldPosition;
    }
    
    /// <summary>
    /// Get the visible world bounds based on camera position and zoom
    /// </summary>
    public Math.Rectangle GetViewBounds()
    {
        Vec2 topLeft = ScreenToWorld(Vec2.Zero);
        Vec2 bottomRight = ScreenToWorld(new Vec2(_viewportWidth, _viewportHeight));
        
        return new Math.Rectangle(
            topLeft.X,
            topLeft.Y,
            bottomRight.X - topLeft.X,
            bottomRight.Y - topLeft.Y
        );
    }
}
