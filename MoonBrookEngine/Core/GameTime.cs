namespace MoonBrookEngine.Core;

/// <summary>
/// Represents timing information for the game loop
/// </summary>
public class GameTime
{
    /// <summary>
    /// Total elapsed time since game start in seconds
    /// </summary>
    public double TotalSeconds { get; }
    
    /// <summary>
    /// Time elapsed since last frame in seconds
    /// </summary>
    public double DeltaTime { get; }
    
    /// <summary>
    /// Time elapsed since last frame in milliseconds
    /// </summary>
    public double DeltaTimeMs => DeltaTime * 1000.0;
    
    /// <summary>
    /// Current frames per second (approximation based on delta time)
    /// </summary>
    public double FPS => DeltaTime > 0 ? 1.0 / DeltaTime : 0;
    
    public GameTime(double totalSeconds, double deltaTime)
    {
        TotalSeconds = totalSeconds;
        DeltaTime = deltaTime;
    }
}
