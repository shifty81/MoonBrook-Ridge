using System;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Handles automatic saving at regular intervals
/// </summary>
public class AutoSaveSystem
{
    private double _timeSinceLastSave;
    private readonly double _autoSaveInterval; // in seconds
    private bool _isEnabled;
    private Action? _onAutoSave;
    
    public bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }
    
    public double AutoSaveIntervalMinutes => _autoSaveInterval / 60.0;
    public double TimeSinceLastSaveMinutes => _timeSinceLastSave / 60.0;
    public double TimeUntilNextSaveMinutes => Math.Max(0, (_autoSaveInterval - _timeSinceLastSave) / 60.0);
    
    /// <summary>
    /// Create a new auto-save system
    /// </summary>
    /// <param name="intervalMinutes">Auto-save interval in minutes (default: 5)</param>
    public AutoSaveSystem(double intervalMinutes = 5.0)
    {
        _autoSaveInterval = intervalMinutes * 60.0; // Convert to seconds
        _timeSinceLastSave = 0;
        _isEnabled = true; // Enabled by default
    }
    
    /// <summary>
    /// Set a callback to be invoked when auto-save is triggered
    /// The callback should perform the actual save operation
    /// </summary>
    public void SetAutoSaveCallback(Action callback)
    {
        _onAutoSave = callback;
    }
    
    /// <summary>
    /// Update the auto-save timer
    /// </summary>
    public void Update(GameTime gameTime)
    {
        if (!_isEnabled) return;
        
        _timeSinceLastSave += gameTime.ElapsedGameTime.TotalSeconds;
        
        // Check if it's time to auto-save
        if (_timeSinceLastSave >= _autoSaveInterval)
        {
            TriggerAutoSave();
        }
    }
    
    /// <summary>
    /// Trigger an auto-save and reset the timer
    /// </summary>
    public void TriggerAutoSave()
    {
        try
        {
            _onAutoSave?.Invoke();
            _timeSinceLastSave = 0;
        }
        catch (Exception ex)
        {
            // Log error but don't crash the game
            Console.WriteLine($"Auto-save failed: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Reset the auto-save timer (call this after manual saves)
    /// </summary>
    public void ResetTimer()
    {
        _timeSinceLastSave = 0;
    }
}
