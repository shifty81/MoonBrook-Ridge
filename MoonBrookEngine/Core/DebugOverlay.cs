using System;
using System.Collections.Generic;

namespace MoonBrookEngine.Core;

/// <summary>
/// Debug overlay for displaying performance metrics and debug information
/// </summary>
public class DebugOverlay
{
    private readonly PerformanceMonitor _perfMonitor;
    private readonly Logger _logger;
    private bool _isEnabled;
    private readonly Dictionary<string, string> _customMetrics;
    
    public bool IsEnabled
    {
        get => _isEnabled;
        set => _isEnabled = value;
    }
    
    public DebugOverlay(PerformanceMonitor perfMonitor)
    {
        _perfMonitor = perfMonitor;
        _logger = LoggerFactory.GetLogger("DebugOverlay");
        _isEnabled = false;
        _customMetrics = new Dictionary<string, string>();
    }
    
    /// <summary>
    /// Toggle debug overlay on/off
    /// </summary>
    public void Toggle()
    {
        _isEnabled = !_isEnabled;
        _logger.Info($"Debug overlay: {(_isEnabled ? "ON" : "OFF")}");
    }
    
    /// <summary>
    /// Add a custom metric to display
    /// </summary>
    public void AddMetric(string name, string value)
    {
        _customMetrics[name] = value;
    }
    
    /// <summary>
    /// Remove a custom metric
    /// </summary>
    public void RemoveMetric(string name)
    {
        _customMetrics.Remove(name);
    }
    
    /// <summary>
    /// Clear all custom metrics
    /// </summary>
    public void ClearMetrics()
    {
        _customMetrics.Clear();
    }
    
    /// <summary>
    /// Get debug text for rendering
    /// </summary>
    public List<string> GetDebugLines()
    {
        if (!_isEnabled)
            return new List<string>();
        
        var lines = new List<string>();
        
        // Performance metrics
        lines.Add($"FPS: {_perfMonitor.FPS:F1}");
        lines.Add($"Frame Time: {_perfMonitor.AverageFrameTime:F2}ms");
        lines.Add($"Update Time: {_perfMonitor.UpdateTime:F2}ms");
        lines.Add($"Render Time: {_perfMonitor.RenderTime:F2}ms");
        lines.Add($"Draw Calls: {_perfMonitor.DrawCalls}");
        lines.Add($"Frame Count: {_perfMonitor.FrameCount}");
        
        // Memory info
        var gcMemory = GC.GetTotalMemory(false) / (1024.0 * 1024.0);
        lines.Add($"Memory: {gcMemory:F1} MB");
        lines.Add($"GC Gen0: {GC.CollectionCount(0)}");
        
        // Custom metrics
        if (_customMetrics.Count > 0)
        {
            lines.Add(""); // Blank line
            foreach (var kvp in _customMetrics)
            {
                lines.Add($"{kvp.Key}: {kvp.Value}");
            }
        }
        
        return lines;
    }
    
    /// <summary>
    /// Draw the debug overlay using a sprite batch and bitmap font
    /// Note: Requires BitmapFont to have DrawText method - stub for now
    /// </summary>
    public void Draw(Graphics.SpriteBatch spriteBatch, Graphics.BitmapFont? font)
    {
        if (!_isEnabled || font == null)
            return;
        
        // TODO: Implement once BitmapFont has DrawText and LineHeight
        // For now, use PrintToConsole or GetDebugLines for text-based debugging
    }
    
    /// <summary>
    /// Print debug info to console
    /// </summary>
    public void PrintToConsole()
    {
        if (!_isEnabled)
            return;
        
        Console.WriteLine("=== Debug Info ===");
        foreach (var line in GetDebugLines())
        {
            Console.WriteLine(line);
        }
        Console.WriteLine("==================");
    }
}
