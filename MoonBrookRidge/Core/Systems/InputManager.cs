using Microsoft.Xna.Framework.Input;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Manages all game input with configurable keybinds
/// </summary>
public class InputManager
{
    // Movement keys
    public Keys MoveUp { get; set; } = Keys.W;
    public Keys MoveDown { get; set; } = Keys.S;
    public Keys MoveLeft { get; set; } = Keys.A;
    public Keys MoveRight { get; set; } = Keys.D;
    
    // Alternative movement
    public Keys MoveUpAlt { get; set; } = Keys.Up;
    public Keys MoveDownAlt { get; set; } = Keys.Down;
    public Keys MoveLeftAlt { get; set; } = Keys.Left;
    public Keys MoveRightAlt { get; set; } = Keys.Right;
    
    // Action keys
    public Keys Run { get; set; } = Keys.LeftShift;
    public Keys OpenMenu { get; set; } = Keys.E;
    public Keys OpenMenuAlt { get; set; } = Keys.Escape;
    public Keys UseToolOrPlace { get; set; } = Keys.C;
    public Keys DoAction { get; set; } = Keys.X;
    public Keys OpenJournal { get; set; } = Keys.F;
    public Keys OpenMap { get; set; } = Keys.M;
    public Keys SwitchToolbar { get; set; } = Keys.Tab;
    
    // Hotbar keys
    public Keys[] HotbarKeys { get; set; } = 
    {
        Keys.D1, Keys.D2, Keys.D3, Keys.D4, Keys.D5,
        Keys.D6, Keys.D7, Keys.D8, Keys.D9, Keys.D0,
        Keys.OemMinus, Keys.OemPlus
    };
    
    private KeyboardState _currentKeyState;
    private KeyboardState _previousKeyState;
    private MouseState _currentMouseState;
    private MouseState _previousMouseState;
    
    public void Update()
    {
        _previousKeyState = _currentKeyState;
        _previousMouseState = _currentMouseState;
        
        _currentKeyState = Keyboard.GetState();
        _currentMouseState = Mouse.GetState();
    }
    
    // Movement checks
    public bool IsMoveUpPressed() => _currentKeyState.IsKeyDown(MoveUp) || _currentKeyState.IsKeyDown(MoveUpAlt);
    public bool IsMoveDownPressed() => _currentKeyState.IsKeyDown(MoveDown) || _currentKeyState.IsKeyDown(MoveDownAlt);
    public bool IsMoveLeftPressed() => _currentKeyState.IsKeyDown(MoveLeft) || _currentKeyState.IsKeyDown(MoveLeftAlt);
    public bool IsMoveRightPressed() => _currentKeyState.IsKeyDown(MoveRight) || _currentKeyState.IsKeyDown(MoveRightAlt);
    public bool IsRunPressed() => _currentKeyState.IsKeyDown(Run);
    
    // Action checks (pressed this frame)
    public bool IsOpenMenuPressed() => IsKeyPressed(OpenMenu) || IsKeyPressed(OpenMenuAlt);
    public bool IsUseToolPressed() => IsKeyPressed(UseToolOrPlace);
    public bool IsDoActionPressed() => IsKeyPressed(DoAction);
    public bool IsOpenJournalPressed() => IsKeyPressed(OpenJournal);
    public bool IsOpenMapPressed() => IsKeyPressed(OpenMap);
    public bool IsSwitchToolbarPressed() => IsKeyPressed(SwitchToolbar);
    
    // Hotbar checks
    public int GetHotbarKeyPressed()
    {
        for (int i = 0; i < HotbarKeys.Length; i++)
        {
            if (IsKeyPressed(HotbarKeys[i]))
            {
                return i;
            }
        }
        return -1;
    }
    
    // Mouse checks
    public bool IsLeftMousePressed() => 
        _currentMouseState.LeftButton == ButtonState.Pressed && 
        _previousMouseState.LeftButton == ButtonState.Released;
    
    public bool IsRightMousePressed() => 
        _currentMouseState.RightButton == ButtonState.Pressed && 
        _previousMouseState.RightButton == ButtonState.Released;
    
    public bool IsLeftMouseDown() => _currentMouseState.LeftButton == ButtonState.Pressed;
    public bool IsRightMouseDown() => _currentMouseState.RightButton == ButtonState.Pressed;
    
    // Helper methods
    private bool IsKeyPressed(Keys key) => 
        _currentKeyState.IsKeyDown(key) && _previousKeyState.IsKeyUp(key);
    
    public KeyboardState CurrentKeyState => _currentKeyState;
    public KeyboardState PreviousKeyState => _previousKeyState;
    public MouseState CurrentMouseState => _currentMouseState;
    public MouseState PreviousMouseState => _previousMouseState;
}
