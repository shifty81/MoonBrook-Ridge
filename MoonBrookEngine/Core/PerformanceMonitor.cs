using System.Diagnostics;

namespace MoonBrookEngine.Core;

/// <summary>
/// Performance monitoring and profiling
/// </summary>
public class PerformanceMonitor
{
    private Stopwatch _updateTimer;
    private Stopwatch _renderTimer;
    private Queue<double> _frameTimes;
    private int _drawCallCount;
    private long _lastMemory;
    
    public double UpdateTime { get; private set; }
    public double RenderTime { get; private set; }
    public double AverageFrameTime { get; private set; }
    public double FPS { get; private set; }
    public int DrawCalls { get; private set; }
    public long MemoryUsage { get; private set; }
    public int FrameCount { get; private set; }
    
    private const int MaxFrameSamples = 60;
    
    public PerformanceMonitor()
    {
        _updateTimer = new Stopwatch();
        _renderTimer = new Stopwatch();
        _frameTimes = new Queue<double>(MaxFrameSamples);
        Reset();
    }
    
    /// <summary>
    /// Reset all counters
    /// </summary>
    public void Reset()
    {
        UpdateTime = 0;
        RenderTime = 0;
        AverageFrameTime = 0;
        FPS = 0;
        DrawCalls = 0;
        MemoryUsage = 0;
        FrameCount = 0;
        _drawCallCount = 0;
        _frameTimes.Clear();
    }
    
    /// <summary>
    /// Begin update timing
    /// </summary>
    public void BeginUpdate()
    {
        _updateTimer.Restart();
    }
    
    /// <summary>
    /// End update timing
    /// </summary>
    public void EndUpdate()
    {
        _updateTimer.Stop();
        UpdateTime = _updateTimer.Elapsed.TotalMilliseconds;
    }
    
    /// <summary>
    /// Begin render timing
    /// </summary>
    public void BeginRender()
    {
        _renderTimer.Restart();
        _drawCallCount = 0;
    }
    
    /// <summary>
    /// End render timing
    /// </summary>
    public void EndRender()
    {
        _renderTimer.Stop();
        RenderTime = _renderTimer.Elapsed.TotalMilliseconds;
        DrawCalls = _drawCallCount;
    }
    
    /// <summary>
    /// Record a draw call
    /// </summary>
    public void RecordDrawCall()
    {
        _drawCallCount++;
    }
    
    /// <summary>
    /// End frame and calculate metrics
    /// </summary>
    public void EndFrame()
    {
        FrameCount++;
        
        double frameTime = UpdateTime + RenderTime;
        
        // Add to frame time history
        _frameTimes.Enqueue(frameTime);
        if (_frameTimes.Count > MaxFrameSamples)
        {
            _frameTimes.Dequeue();
        }
        
        // Calculate average frame time
        AverageFrameTime = _frameTimes.Average();
        
        // Calculate FPS
        if (AverageFrameTime > 0)
        {
            FPS = 1000.0 / AverageFrameTime;
        }
        
        // Update memory usage (sample every 10 frames to reduce overhead)
        if (FrameCount % 10 == 0)
        {
            _lastMemory = GC.GetTotalMemory(false);
            MemoryUsage = _lastMemory;
        }
        else
        {
            MemoryUsage = _lastMemory;
        }
    }
    
    /// <summary>
    /// Get formatted performance string
    /// </summary>
    public string GetPerformanceString()
    {
        return $"FPS: {FPS:F1} | Frame: {AverageFrameTime:F2}ms (Update: {UpdateTime:F2}ms, Render: {RenderTime:F2}ms) | " +
               $"Draw Calls: {DrawCalls} | Memory: {MemoryUsage / 1024.0 / 1024.0:F1} MB";
    }
    
    /// <summary>
    /// Get detailed performance statistics
    /// </summary>
    public PerformanceStats GetStats()
    {
        return new PerformanceStats
        {
            FPS = FPS,
            AverageFrameTime = AverageFrameTime,
            UpdateTime = UpdateTime,
            RenderTime = RenderTime,
            DrawCalls = DrawCalls,
            MemoryUsageMB = MemoryUsage / 1024.0 / 1024.0,
            FrameCount = FrameCount
        };
    }
}

/// <summary>
/// Performance statistics snapshot
/// </summary>
public struct PerformanceStats
{
    public double FPS { get; init; }
    public double AverageFrameTime { get; init; }
    public double UpdateTime { get; init; }
    public double RenderTime { get; init; }
    public int DrawCalls { get; init; }
    public double MemoryUsageMB { get; init; }
    public int FrameCount { get; init; }
    
    public override string ToString()
    {
        return $"FPS: {FPS:F1}, Frame: {AverageFrameTime:F2}ms, Update: {UpdateTime:F2}ms, " +
               $"Render: {RenderTime:F2}ms, Draws: {DrawCalls}, Memory: {MemoryUsageMB:F1}MB";
    }
}
