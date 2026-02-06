using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using System;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Handles mouse input for gameplay including click-to-move, click-to-interact, and context menus
/// </summary>
public class MouseGameplayManager
{
    private MouseState _previousMouseState;
    private MouseState _currentMouseState;
    private bool _isDragging;
    private Vector2 _dragStartPosition;
    
    // Events for gameplay mouse actions
    public event Action<Vector2>? OnLeftClick;
    public event Action<Vector2>? OnRightClick;
    public event Action<Vector2>? OnMiddleClick;
    public event Action<Vector2, Vector2>? OnDrag;
    public event Action<Vector2>? OnHover;
    
    // Click thresholds
    private const float DRAG_THRESHOLD = 5f;
    private const double DOUBLE_CLICK_TIME = 0.3; // seconds
    private double _lastClickTime;
    private Vector2 _lastClickPosition;
    
    public MouseGameplayManager()
    {
        _previousMouseState = Mouse.GetState();
        _isDragging = false;
        _lastClickTime = 0;
    }
    
    /// <summary>
    /// Update mouse input state
    /// </summary>
    public void Update(GameTime gameTime)
    {
        _currentMouseState = Mouse.GetState();
        
        Vector2 mouseWorldPos = GetMousePosition();
        
        // Detect left click
        if (_currentMouseState.LeftButton == ButtonState.Pressed && 
            _previousMouseState.LeftButton == ButtonState.Released)
        {
            // Start potential drag
            _isDragging = false;
            _dragStartPosition = mouseWorldPos;
            
            // Check for double-click
            double timeSinceLastClick = gameTime.TotalGameTime.TotalSeconds - _lastClickTime;
            if (timeSinceLastClick < DOUBLE_CLICK_TIME && 
                Vector2.Distance(mouseWorldPos, _lastClickPosition) < DRAG_THRESHOLD)
            {
                OnDoubleClick(mouseWorldPos);
            }
            else
            {
                OnLeftClick?.Invoke(mouseWorldPos);
            }
            
            _lastClickTime = gameTime.TotalGameTime.TotalSeconds;
            _lastClickPosition = mouseWorldPos;
        }
        
        // Detect dragging
        if (_currentMouseState.LeftButton == ButtonState.Pressed && 
            _previousMouseState.LeftButton == ButtonState.Pressed)
        {
            float dragDistance = Vector2.Distance(mouseWorldPos, _dragStartPosition);
            if (dragDistance > DRAG_THRESHOLD)
            {
                _isDragging = true;
                OnDrag?.Invoke(_dragStartPosition, mouseWorldPos);
            }
        }
        
        // Detect left button release (end drag or click)
        if (_currentMouseState.LeftButton == ButtonState.Released && 
            _previousMouseState.LeftButton == ButtonState.Pressed)
        {
            if (_isDragging)
            {
                OnDragEnd(_dragStartPosition, mouseWorldPos);
                _isDragging = false;
            }
        }
        
        // Detect right click
        if (_currentMouseState.RightButton == ButtonState.Pressed && 
            _previousMouseState.RightButton == ButtonState.Released)
        {
            OnRightClick?.Invoke(mouseWorldPos);
        }
        
        // Detect middle click
        if (_currentMouseState.MiddleButton == ButtonState.Pressed && 
            _previousMouseState.MiddleButton == ButtonState.Released)
        {
            OnMiddleClick?.Invoke(mouseWorldPos);
        }
        
        // Always fire hover event
        OnHover?.Invoke(mouseWorldPos);
        
        _previousMouseState = _currentMouseState;
    }
    
    /// <summary>
    /// Called when double-click detected
    /// </summary>
    protected virtual void OnDoubleClick(Vector2 position)
    {
        // Can be used for special double-click actions
        // Like double-click to run to location
    }
    
    /// <summary>
    /// Called when drag operation ends
    /// </summary>
    protected virtual void OnDragEnd(Vector2 startPosition, Vector2 endPosition)
    {
        // Can be used for drag-and-drop operations
    }
    
    /// <summary>
    /// Get mouse position in screen space
    /// </summary>
    public Vector2 GetMousePosition()
    {
        return new Vector2(_currentMouseState.X, _currentMouseState.Y);
    }
    
    /// <summary>
    /// Get mouse position in world space (accounting for camera)
    /// </summary>
    public Vector2 GetMouseWorldPosition(Camera2D camera)
    {
        Vector2 screenPos = GetMousePosition();
        return camera.ScreenToWorld(screenPos);
    }
    
    /// <summary>
    /// Check if mouse button is currently pressed
    /// </summary>
    public bool IsLeftButtonDown()
    {
        return _currentMouseState.LeftButton == ButtonState.Pressed;
    }
    
    public bool IsRightButtonDown()
    {
        return _currentMouseState.RightButton == ButtonState.Pressed;
    }
    
    public bool IsMiddleButtonDown()
    {
        return _currentMouseState.MiddleButton == ButtonState.Pressed;
    }
    
    /// <summary>
    /// Check if a specific button was just clicked (pressed and released)
    /// </summary>
    public bool WasLeftButtonClicked()
    {
        return _currentMouseState.LeftButton == ButtonState.Released && 
               _previousMouseState.LeftButton == ButtonState.Pressed;
    }
    
    public bool WasRightButtonClicked()
    {
        return _currentMouseState.RightButton == ButtonState.Released && 
               _previousMouseState.RightButton == ButtonState.Pressed;
    }
    
    /// <summary>
    /// Get mouse wheel scroll delta
    /// </summary>
    public int GetScrollWheelDelta()
    {
        return _currentMouseState.ScrollWheelValue - _previousMouseState.ScrollWheelValue;
    }
    
    /// <summary>
    /// Check if mouse is over a specific rectangular area
    /// </summary>
    public bool IsMouseOver(Rectangle bounds)
    {
        Vector2 mousePos = GetMousePosition();
        return bounds.Contains(mousePos);
    }
    
    /// <summary>
    /// Check if mouse clicked within a specific rectangular area
    /// </summary>
    public bool WasClickedInBounds(Rectangle bounds)
    {
        return IsMouseOver(bounds) && WasLeftButtonClicked();
    }
    
    public bool IsDragging => _isDragging;
    public Vector2 DragStartPosition => _dragStartPosition;
}
