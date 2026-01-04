namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible static Mouse class
/// </summary>
public static class Mouse
{
    private static Game? _game;
    
    internal static void Initialize(Game game)
    {
        _game = game;
    }
    
    public static MouseState GetState()
    {
        if (_game == null || _game.Engine == null)
        {
            return new MouseState();
        }
        
        var engineInput = _game.Engine.InputManager;
        var mousePos = engineInput.MousePosition;
        
        var left = engineInput.IsButtonDown(Silk.NET.Input.MouseButton.Left) 
            ? ButtonState.Pressed 
            : ButtonState.Released;
        var right = engineInput.IsButtonDown(Silk.NET.Input.MouseButton.Right) 
            ? ButtonState.Pressed 
            : ButtonState.Released;
        var middle = engineInput.IsButtonDown(Silk.NET.Input.MouseButton.Middle) 
            ? ButtonState.Pressed 
            : ButtonState.Released;
        
        return new MouseState(
            (int)mousePos.X,
            (int)mousePos.Y,
            left,
            right,
            middle,
            0 // Scroll wheel value - we'll need to track this separately if needed
        );
    }
}
