namespace MoonBrookRidge.Core;

/// <summary>
/// Shared game constants used across multiple systems
/// </summary>
public static class GameConstants
{
    // Tile and world constants
    public const int TILE_SIZE = 16;
    public const float PLAYER_RADIUS = 16f; // Half the player collision size
    
    // World dimensions
    public const int DEFAULT_WORLD_WIDTH = 50;
    public const int DEFAULT_WORLD_HEIGHT = 50;
    
    // Game time constants
    public const float REAL_SECONDS_PER_GAME_MINUTE = 2.5f; // 1 real minute = 24 game minutes
    public const int GAME_HOURS_PER_DAY = 24;
    public const int GAME_DAYS_PER_SEASON = 28;
}
