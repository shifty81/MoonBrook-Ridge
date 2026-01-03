using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Monitors game performance metrics including FPS, memory usage, and update times
/// </summary>
public class PerformanceMonitor
{
    private readonly Queue<double> _fpsHistory;
    private readonly Queue<long> _memoryHistory;
    private const int HistorySize = 60; // Track last 60 frames
    
    private double _totalElapsed;
    private int _frameCount;
    private double _fps;
    private double _averageFps;
    private long _currentMemory;
    private double _updateTime;
    private double _drawTime;
    
    public bool IsVisible { get; set; }
    public double FPS => _fps;
    public double AverageFPS => _averageFps;
    public long MemoryUsageMB => _currentMemory / (1024 * 1024);
    public double UpdateTimeMs => _updateTime;
    public double DrawTimeMs => _drawTime;
    
    public PerformanceMonitor()
    {
        _fpsHistory = new Queue<double>(HistorySize);
        _memoryHistory = new Queue<long>(HistorySize);
        IsVisible = false;
    }
    
    /// <summary>
    /// Update performance metrics
    /// </summary>
    public void Update(GameTime gameTime)
    {
        _totalElapsed += gameTime.ElapsedGameTime.TotalSeconds;
        _frameCount++;
        
        // Update FPS every second
        if (_totalElapsed >= 1.0)
        {
            _fps = _frameCount / _totalElapsed;
            _frameCount = 0;
            _totalElapsed = 0;
            
            // Add to history
            _fpsHistory.Enqueue(_fps);
            if (_fpsHistory.Count > HistorySize)
                _fpsHistory.Dequeue();
            
            // Calculate average FPS
            _averageFps = _fpsHistory.Average();
            
            // Update memory usage
            _currentMemory = GC.GetTotalMemory(false);
            _memoryHistory.Enqueue(_currentMemory);
            if (_memoryHistory.Count > HistorySize)
                _memoryHistory.Dequeue();
        }
    }
    
    /// <summary>
    /// Record update time
    /// </summary>
    public void RecordUpdateTime(double milliseconds)
    {
        _updateTime = milliseconds;
    }
    
    /// <summary>
    /// Record draw time
    /// </summary>
    public void RecordDrawTime(double milliseconds)
    {
        _drawTime = milliseconds;
    }
    
    /// <summary>
    /// Draw performance overlay
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, int screenWidth, int screenHeight)
    {
        if (!IsVisible) return;
        
        var position = new Vector2(10, screenHeight - 120);
        var bgColor = new Color(0, 0, 0, 180);
        var textColor = Color.White;
        
        // Draw background
        var bgRect = new Rectangle((int)position.X - 5, (int)position.Y - 5, 250, 110);
        DrawRectangle(spriteBatch, bgRect, bgColor);
        
        // Draw FPS
        spriteBatch.DrawString(font, $"FPS: {_fps:F1} (Avg: {_averageFps:F1})", position, textColor);
        position.Y += 20;
        
        // Draw memory
        var memoryColor = MemoryUsageMB > 500 ? Color.Red : Color.White;
        spriteBatch.DrawString(font, $"Memory: {MemoryUsageMB} MB", position, memoryColor);
        position.Y += 20;
        
        // Draw update time
        var updateColor = _updateTime > 16.67 ? Color.Yellow : Color.White;
        spriteBatch.DrawString(font, $"Update: {_updateTime:F2}ms", position, updateColor);
        position.Y += 20;
        
        // Draw draw time
        var drawColor = _drawTime > 16.67 ? Color.Yellow : Color.White;
        spriteBatch.DrawString(font, $"Draw: {_drawTime:F2}ms", position, drawColor);
        position.Y += 20;
        
        // Draw performance warning
        if (_fps < 30)
        {
            spriteBatch.DrawString(font, "⚠ Low FPS", position, Color.Red);
        }
        else if (_fps < 50)
        {
            spriteBatch.DrawString(font, "⚠ Moderate FPS", position, Color.Yellow);
        }
    }
    
    /// <summary>
    /// Helper to draw a filled rectangle
    /// </summary>
    private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
    {
        var pixel = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        spriteBatch.Draw(pixel, rect, color);
    }
}
