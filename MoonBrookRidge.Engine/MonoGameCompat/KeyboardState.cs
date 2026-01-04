using System;
using System.Collections.Generic;
using System.Linq;

namespace MoonBrookRidge.Engine.MonoGameCompat;

/// <summary>
/// MonoGame-compatible KeyboardState
/// </summary>
public struct KeyboardState
{
    private readonly HashSet<Keys> _pressedKeys;

    public KeyboardState()
    {
        _pressedKeys = new HashSet<Keys>();
    }

    internal KeyboardState(IEnumerable<Keys> pressedKeys)
    {
        _pressedKeys = new HashSet<Keys>(pressedKeys);
    }

    public bool IsKeyDown(Keys key)
    {
        return _pressedKeys != null && _pressedKeys.Contains(key);
    }

    public bool IsKeyUp(Keys key)
    {
        return !IsKeyDown(key);
    }

    public Keys[] GetPressedKeys()
    {
        return _pressedKeys?.ToArray() ?? Array.Empty<Keys>();
    }

    public override bool Equals(object? obj)
    {
        if (obj is KeyboardState other)
        {
            return _pressedKeys?.SetEquals(other._pressedKeys ?? new HashSet<Keys>()) ?? other._pressedKeys == null;
        }
        return false;
    }

    public override int GetHashCode()
    {
        return _pressedKeys?.GetHashCode() ?? 0;
    }

    public static bool operator ==(KeyboardState a, KeyboardState b)
    {
        return a.Equals(b);
    }

    public static bool operator !=(KeyboardState a, KeyboardState b)
    {
        return !a.Equals(b);
    }
}
