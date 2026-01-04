using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.Scenes;

/// <summary>
/// Represents a transition point between scenes (door, portal, stairs, etc.)
/// </summary>
public class SceneTransition
{
    public string TransitionId { get; set; }
    public Vector2 Position { get; set; }
    public string TargetSceneId { get; set; }
    public Vector2 TargetSpawnPosition { get; set; }
    public TransitionType Type { get; set; }
    public float TriggerRadius { get; set; }
    public string InteractionText { get; set; }
    public bool RequiresInteraction { get; set; }
    
    public SceneTransition(
        string transitionId,
        Vector2 position,
        string targetSceneId,
        Vector2 targetSpawnPosition,
        TransitionType type = TransitionType.Door,
        bool requiresInteraction = true)
    {
        TransitionId = transitionId;
        Position = position;
        TargetSceneId = targetSceneId;
        TargetSpawnPosition = targetSpawnPosition;
        Type = type;
        TriggerRadius = 1.5f * GameConstants.TILE_SIZE; // 1.5 tiles
        RequiresInteraction = requiresInteraction;
        InteractionText = type switch
        {
            TransitionType.Door => "Press X to enter",
            TransitionType.Stairs => "Press X to use stairs",
            TransitionType.Portal => "Press X to enter portal",
            TransitionType.Ladder => "Press X to climb ladder",
            _ => "Press X to interact"
        };
    }
    
    /// <summary>
    /// Check if player is at this transition point
    /// </summary>
    public bool IsPlayerAtTransition(Vector2 playerPosition)
    {
        float distance = Vector2.Distance(playerPosition, Position);
        return distance <= TriggerRadius;
    }
}

/// <summary>
/// Types of transitions between scenes
/// </summary>
public enum TransitionType
{
    Door,       // Standard door
    Stairs,     // Stairs up/down
    Portal,     // Magic portal
    Ladder,     // Ladder
    Teleport,   // Instant teleport
    Auto        // Automatic transition (walk through)
}
