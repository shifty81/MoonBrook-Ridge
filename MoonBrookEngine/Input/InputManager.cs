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
    
    private HashSet<Key> _currentPressedKeys;
    private HashSet<Key> _previousPressedKeys;
    private HashSet<MouseButton> _currentPressedButtons;
    private HashSet<MouseButton> _previousPressedButtons;
    
    public Math.Vector2 MousePosition { get; private set; }
    public Math.Vector2 MouseDelta { get; private set; }
    public float ScrollDelta { get; private set; }
    
    public InputManager(IInputContext inputContext)
    {
        _inputContext = inputContext;
        _currentPressedKeys = new HashSet<Key>();
        _previousPressedKeys = new HashSet<Key>();
        _currentPressedButtons = new HashSet<MouseButton>();
        _previousPressedButtons = new HashSet<MouseButton>();
        
        MousePosition = Math.Vector2.Zero;
        MouseDelta = Math.Vector2.Zero;
        ScrollDelta = 0f;
        
        // Get first keyboard and mouse
        if (_inputContext.Keyboards.Count > 0)
        {
            _keyboard = _inputContext.Keyboards[0];
            _keyboard.KeyDown += OnKeyDown;
            _keyboard.KeyUp += OnKeyUp;
        }
        
        if (_inputContext.Mice.Count > 0)
        {
            _mouse = _inputContext.Mice[0];
            _mouse.MouseDown += OnMouseDown;
            _mouse.MouseUp += OnMouseUp;
            _mouse.MouseMove += OnMouseMove;
            _mouse.Scroll += OnMouseScroll;
        }
    }
    
    /// <summary>
    /// Update input state (call once per frame)
    /// </summary>
    public void Update()
    {
        // Swap current and previous state (no allocations)
        var temp = _previousPressedKeys;
        _previousPressedKeys = _currentPressedKeys;
        _currentPressedKeys = temp;
        _currentPressedKeys.Clear();
        
        // Copy current key state from Silk.NET (event-driven tracking is better but this works)
        if (_keyboard != null)
        {
            // Only check keys that are likely to be pressed
            foreach (var key in _keyboard.SupportedKeys)
            {
                if (_keyboard.IsKeyPressed(key))
                {
                    _currentPressedKeys.Add(key);
                }
            }
        }
        
        // Swap mouse button state
        var tempButtons = _previousPressedButtons;
        _previousPressedButtons = _currentPressedButtons;
        _currentPressedButtons = tempButtons;
        _currentPressedButtons.Clear();
        
        if (_mouse != null)
        {
            if (_mouse.IsButtonPressed(MouseButton.Left))
                _currentPressedButtons.Add(MouseButton.Left);
            if (_mouse.IsButtonPressed(MouseButton.Right))
                _currentPressedButtons.Add(MouseButton.Right);
            if (_mouse.IsButtonPressed(MouseButton.Middle))
                _currentPressedButtons.Add(MouseButton.Middle);
            
            MousePosition = new Math.Vector2(_mouse.Position.X, _mouse.Position.Y);
        }
        
        // Mouse delta and scroll are updated via events
    }
    
    /// <summary>
    /// Check if key is currently held down
    /// </summary>
    public bool IsKeyDown(Key key)
    {
        return _currentPressedKeys.Contains(key);
    }
    
    /// <summary>
    /// Check if key was just pressed this frame
    /// </summary>
    public bool IsKeyPressed(Key key)
    {
        return _currentPressedKeys.Contains(key) && !_previousPressedKeys.Contains(key);
    }
    
    /// <summary>
    /// Check if key was just released this frame
    /// </summary>
    public bool IsKeyReleased(Key key)
    {
        return !_currentPressedKeys.Contains(key) && _previousPressedKeys.Contains(key);
    }
    
    /// <summary>
    /// Check if mouse button is currently held down
    /// </summary>
    public bool IsButtonDown(MouseButton button)
    {
        return _currentPressedButtons.Contains(button);
    }
    
    /// <summary>
    /// Check if mouse button was just pressed this frame
    /// </summary>
    public bool IsButtonPressed(MouseButton button)
    {
        return _currentPressedButtons.Contains(button) && !_previousPressedButtons.Contains(button);
    }
    
    /// <summary>
    /// Check if mouse button was just released this frame
    /// </summary>
    public bool IsButtonReleased(MouseButton button)
    {
        return !_currentPressedButtons.Contains(button) && _previousPressedButtons.Contains(button);
    }
    
    /// <summary>
    /// Check if any key is currently held down
    /// </summary>
    public bool IsAnyKeyDown()
    {
        return _currentPressedKeys.Count > 0;
    }
    
    /// <summary>
    /// Check if any key was just pressed this frame
    /// </summary>
    public bool IsAnyKeyPressed()
    {
        return _currentPressedKeys.Except(_previousPressedKeys).Any();
    }
    
    private void OnKeyDown(IKeyboard keyboard, Key key, int arg3)
    {
        _currentPressedKeys.Add(key);
    }
    
    private void OnKeyUp(IKeyboard keyboard, Key key, int arg3)
    {
        _currentPressedKeys.Remove(key);
    }
    
    private void OnMouseDown(IMouse mouse, MouseButton button)
    {
        _currentPressedButtons.Add(button);
    }
    
    private void OnMouseUp(IMouse mouse, MouseButton button)
    {
        _currentPressedButtons.Remove(button);
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
