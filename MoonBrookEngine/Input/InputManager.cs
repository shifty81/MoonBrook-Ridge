using Silk.NET.Input;

namespace MoonBrookEngine.Input;

/// <summary>
/// Simplified input management for keyboard and mouse
/// </summary>
public class InputManager
{
    private IInputContext _inputContext;
    private IKeyboard? _keyboard;
    private IMouse? _mouse;
    
    private HashSet<Key> _pressedKeys;
    private HashSet<Key> _previousPressedKeys;
    private HashSet<MouseButton> _pressedButtons;
    private HashSet<MouseButton> _previousPressedButtons;
    
    public Math.Vector2 MousePosition { get; private set; }
    public Math.Vector2 MouseDelta { get; private set; }
    public float ScrollDelta { get; private set; }
    
    public InputManager(IInputContext inputContext)
    {
        _inputContext = inputContext;
        _pressedKeys = new HashSet<Key>();
        _previousPressedKeys = new HashSet<Key>();
        _pressedButtons = new HashSet<MouseButton>();
        _previousPressedButtons = new HashSet<MouseButton>();
        
        MousePosition = Math.Vector2.Zero;
        MouseDelta = Math.Vector2.Zero;
        ScrollDelta = 0f;
        
        // Get first keyboard and mouse
        if (_inputContext.Keyboards.Count > 0)
        {
            _keyboard = _inputContext.Keyboards[0];
        }
        
        if (_inputContext.Mice.Count > 0)
        {
            _mouse = _inputContext.Mice[0];
            _mouse.MouseMove += OnMouseMove;
            _mouse.Scroll += OnMouseScroll;
        }
    }
    
    /// <summary>
    /// Update input state (call once per frame)
    /// </summary>
    public void Update()
    {
        // Save previous frame state
        _previousPressedKeys = new HashSet<Key>(_pressedKeys);
        _previousPressedButtons = new HashSet<MouseButton>(_pressedButtons);
        
        // Update current state
        _pressedKeys.Clear();
        if (_keyboard != null)
        {
            foreach (var key in Enum.GetValues<Key>())
            {
                if (_keyboard.IsKeyPressed(key))
                {
                    _pressedKeys.Add(key);
                }
            }
        }
        
        _pressedButtons.Clear();
        if (_mouse != null)
        {
            foreach (var button in Enum.GetValues<MouseButton>())
            {
                if (_mouse.IsButtonPressed(button))
                {
                    _pressedButtons.Add(button);
                }
            }
            
            MousePosition = new Math.Vector2(_mouse.Position.X, _mouse.Position.Y);
        }
        
        // Reset per-frame deltas
        MouseDelta = Math.Vector2.Zero;
        ScrollDelta = 0f;
    }
    
    /// <summary>
    /// Check if key is currently held down
    /// </summary>
    public bool IsKeyDown(Key key)
    {
        return _pressedKeys.Contains(key);
    }
    
    /// <summary>
    /// Check if key was just pressed this frame
    /// </summary>
    public bool IsKeyPressed(Key key)
    {
        return _pressedKeys.Contains(key) && !_previousPressedKeys.Contains(key);
    }
    
    /// <summary>
    /// Check if key was just released this frame
    /// </summary>
    public bool IsKeyReleased(Key key)
    {
        return !_pressedKeys.Contains(key) && _previousPressedKeys.Contains(key);
    }
    
    /// <summary>
    /// Check if mouse button is currently held down
    /// </summary>
    public bool IsButtonDown(MouseButton button)
    {
        return _pressedButtons.Contains(button);
    }
    
    /// <summary>
    /// Check if mouse button was just pressed this frame
    /// </summary>
    public bool IsButtonPressed(MouseButton button)
    {
        return _pressedButtons.Contains(button) && !_previousPressedButtons.Contains(button);
    }
    
    /// <summary>
    /// Check if mouse button was just released this frame
    /// </summary>
    public bool IsButtonReleased(MouseButton button)
    {
        return !_pressedButtons.Contains(button) && _previousPressedButtons.Contains(button);
    }
    
    /// <summary>
    /// Check if any key is currently held down
    /// </summary>
    public bool IsAnyKeyDown()
    {
        return _pressedKeys.Count > 0;
    }
    
    /// <summary>
    /// Check if any key was just pressed this frame
    /// </summary>
    public bool IsAnyKeyPressed()
    {
        return _pressedKeys.Except(_previousPressedKeys).Any();
    }
    
    private void OnMouseMove(IMouse mouse, System.Numerics.Vector2 position)
    {
        var newPos = new Math.Vector2(position.X, position.Y);
        MouseDelta = newPos - MousePosition;
        MousePosition = newPos;
    }
    
    private void OnMouseScroll(IMouse mouse, ScrollWheel wheel)
    {
        ScrollDelta = wheel.Y;
    }
}
