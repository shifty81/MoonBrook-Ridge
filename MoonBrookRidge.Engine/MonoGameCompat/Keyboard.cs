using System;
using System.Collections.Generic;

namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible static Keyboard class
/// </summary>
public static class Keyboard
{
    private static Game? _game;
    
    internal static void Initialize(Game game)
    {
        _game = game;
    }
    
    public static KeyboardState GetState()
    {
        if (_game == null || _game.Engine == null)
        {
            return new KeyboardState();
        }
        
        var engineInput = _game.Engine.InputManager;
        var pressedKeys = new List<Keys>();
        
        // Map Silk.NET Key to MonoGame Keys
        // We'll check all keys we support
        foreach (Keys key in Enum.GetValues<Keys>())
        {
            if (key == Keys.None) continue;
            
            var silkKey = MapToSilkKey(key);
            if (silkKey != Silk.NET.Input.Key.Unknown && engineInput.IsKeyDown(silkKey))
            {
                pressedKeys.Add(key);
            }
        }
        
        return new KeyboardState(pressedKeys);
    }
    
    private static Silk.NET.Input.Key MapToSilkKey(Keys key)
    {
        // Map MonoGame Keys to Silk.NET Key enum
        return key switch
        {
            Keys.A => Silk.NET.Input.Key.A,
            Keys.B => Silk.NET.Input.Key.B,
            Keys.C => Silk.NET.Input.Key.C,
            Keys.D => Silk.NET.Input.Key.D,
            Keys.E => Silk.NET.Input.Key.E,
            Keys.F => Silk.NET.Input.Key.F,
            Keys.G => Silk.NET.Input.Key.G,
            Keys.H => Silk.NET.Input.Key.H,
            Keys.I => Silk.NET.Input.Key.I,
            Keys.J => Silk.NET.Input.Key.J,
            Keys.K => Silk.NET.Input.Key.K,
            Keys.L => Silk.NET.Input.Key.L,
            Keys.M => Silk.NET.Input.Key.M,
            Keys.N => Silk.NET.Input.Key.N,
            Keys.O => Silk.NET.Input.Key.O,
            Keys.P => Silk.NET.Input.Key.P,
            Keys.Q => Silk.NET.Input.Key.Q,
            Keys.R => Silk.NET.Input.Key.R,
            Keys.S => Silk.NET.Input.Key.S,
            Keys.T => Silk.NET.Input.Key.T,
            Keys.U => Silk.NET.Input.Key.U,
            Keys.V => Silk.NET.Input.Key.V,
            Keys.W => Silk.NET.Input.Key.W,
            Keys.X => Silk.NET.Input.Key.X,
            Keys.Y => Silk.NET.Input.Key.Y,
            Keys.Z => Silk.NET.Input.Key.Z,
            
            Keys.D0 => Silk.NET.Input.Key.Number0,
            Keys.D1 => Silk.NET.Input.Key.Number1,
            Keys.D2 => Silk.NET.Input.Key.Number2,
            Keys.D3 => Silk.NET.Input.Key.Number3,
            Keys.D4 => Silk.NET.Input.Key.Number4,
            Keys.D5 => Silk.NET.Input.Key.Number5,
            Keys.D6 => Silk.NET.Input.Key.Number6,
            Keys.D7 => Silk.NET.Input.Key.Number7,
            Keys.D8 => Silk.NET.Input.Key.Number8,
            Keys.D9 => Silk.NET.Input.Key.Number9,
            
            Keys.NumPad0 => Silk.NET.Input.Key.Keypad0,
            Keys.NumPad1 => Silk.NET.Input.Key.Keypad1,
            Keys.NumPad2 => Silk.NET.Input.Key.Keypad2,
            Keys.NumPad3 => Silk.NET.Input.Key.Keypad3,
            Keys.NumPad4 => Silk.NET.Input.Key.Keypad4,
            Keys.NumPad5 => Silk.NET.Input.Key.Keypad5,
            Keys.NumPad6 => Silk.NET.Input.Key.Keypad6,
            Keys.NumPad7 => Silk.NET.Input.Key.Keypad7,
            Keys.NumPad8 => Silk.NET.Input.Key.Keypad8,
            Keys.NumPad9 => Silk.NET.Input.Key.Keypad9,
            
            Keys.F1 => Silk.NET.Input.Key.F1,
            Keys.F2 => Silk.NET.Input.Key.F2,
            Keys.F3 => Silk.NET.Input.Key.F3,
            Keys.F4 => Silk.NET.Input.Key.F4,
            Keys.F5 => Silk.NET.Input.Key.F5,
            Keys.F6 => Silk.NET.Input.Key.F6,
            Keys.F7 => Silk.NET.Input.Key.F7,
            Keys.F8 => Silk.NET.Input.Key.F8,
            Keys.F9 => Silk.NET.Input.Key.F9,
            Keys.F10 => Silk.NET.Input.Key.F10,
            Keys.F11 => Silk.NET.Input.Key.F11,
            Keys.F12 => Silk.NET.Input.Key.F12,
            
            Keys.Left => Silk.NET.Input.Key.Left,
            Keys.Right => Silk.NET.Input.Key.Right,
            Keys.Up => Silk.NET.Input.Key.Up,
            Keys.Down => Silk.NET.Input.Key.Down,
            
            Keys.Space => Silk.NET.Input.Key.Space,
            Keys.Enter => Silk.NET.Input.Key.Enter,
            Keys.Escape => Silk.NET.Input.Key.Escape,
            Keys.Back => Silk.NET.Input.Key.Backspace,
            Keys.Tab => Silk.NET.Input.Key.Tab,
            Keys.LeftShift => Silk.NET.Input.Key.ShiftLeft,
            Keys.RightShift => Silk.NET.Input.Key.ShiftRight,
            Keys.LeftControl => Silk.NET.Input.Key.ControlLeft,
            Keys.RightControl => Silk.NET.Input.Key.ControlRight,
            Keys.LeftAlt => Silk.NET.Input.Key.AltLeft,
            Keys.RightAlt => Silk.NET.Input.Key.AltRight,
            Keys.CapsLock => Silk.NET.Input.Key.CapsLock,
            
            Keys.Insert => Silk.NET.Input.Key.Insert,
            Keys.Delete => Silk.NET.Input.Key.Delete,
            Keys.Home => Silk.NET.Input.Key.Home,
            Keys.End => Silk.NET.Input.Key.End,
            Keys.PageUp => Silk.NET.Input.Key.PageUp,
            Keys.PageDown => Silk.NET.Input.Key.PageDown,
            
            Keys.OemSemicolon => Silk.NET.Input.Key.Semicolon,
            Keys.OemPlus => Silk.NET.Input.Key.Equal,
            Keys.OemComma => Silk.NET.Input.Key.Comma,
            Keys.OemMinus => Silk.NET.Input.Key.Minus,
            Keys.OemPeriod => Silk.NET.Input.Key.Period,
            Keys.OemQuestion => Silk.NET.Input.Key.Slash,
            Keys.OemTilde => Silk.NET.Input.Key.GraveAccent,
            Keys.OemOpenBrackets => Silk.NET.Input.Key.LeftBracket,
            Keys.OemPipe => Silk.NET.Input.Key.BackSlash,
            Keys.OemCloseBrackets => Silk.NET.Input.Key.RightBracket,
            Keys.OemQuotes => Silk.NET.Input.Key.Apostrophe,
            
            _ => Silk.NET.Input.Key.Unknown
        };
    }
}
