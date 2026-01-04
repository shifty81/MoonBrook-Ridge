using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Farming.Tools;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Manages hotkey-based tool switching (1-9 keys map to specific tools)
/// </summary>
public class ToolHotkeyManager
{
    private readonly Dictionary<Keys, Tool> _hotkeyMap;
    private KeyboardState _previousKeyboardState;
    private Action<Tool>? _onToolSelected;
    
    public ToolHotkeyManager()
    {
        _previousKeyboardState = Keyboard.GetState();
        _hotkeyMap = new Dictionary<Keys, Tool>();
    }
    
    /// <summary>
    /// Set the callback for when a tool is selected
    /// </summary>
    public void SetToolSelectedCallback(Action<Tool> callback)
    {
        _onToolSelected = callback;
    }
    
    /// <summary>
    /// Register a tool with a hotkey
    /// </summary>
    public void RegisterTool(Keys key, Tool tool)
    {
        _hotkeyMap[key] = tool;
    }
    
    /// <summary>
    /// Clear all tool registrations
    /// </summary>
    public void ClearTools()
    {
        _hotkeyMap.Clear();
    }
    
    /// <summary>
    /// Update hotkey system and check for tool switches
    /// </summary>
    public void Update()
    {
        var keyboardState = Keyboard.GetState();
        
        foreach (var hotkey in _hotkeyMap)
        {
            // Check if key was just pressed (with debouncing)
            if (keyboardState.IsKeyDown(hotkey.Key) && !_previousKeyboardState.IsKeyDown(hotkey.Key))
            {
                // Invoke callback to switch to the tool
                _onToolSelected?.Invoke(hotkey.Value);
            }
        }
        
        _previousKeyboardState = keyboardState;
    }
    
    /// <summary>
    /// Get the hotkey for a specific tool
    /// </summary>
    public Keys? GetHotkeyForTool(Tool tool)
    {
        foreach (var kvp in _hotkeyMap)
        {
            if (kvp.Value == tool)
                return kvp.Key;
        }
        return null;
    }
    
    /// <summary>
    /// Get a readable string for hotkey hints
    /// </summary>
    public string GetHotkeyHint(Tool tool)
    {
        var key = GetHotkeyForTool(tool);
        if (key.HasValue)
        {
            return key.Value.ToString().Replace("D", ""); // "D1" -> "1"
        }
        return "Tab";
    }
}
