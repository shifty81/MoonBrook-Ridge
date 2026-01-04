using MoonBrookRidge.Engine.MonoGameCompat;
using System;

namespace MoonBrookRidge.Pets;

/// <summary>
/// Represents a wild pet that can be tamed by the player
/// </summary>
public class WildPet
{
    public string DefinitionId { get; }
    public string Name { get; }
    public Vector2 Position { get; set; }
    public bool IsTamed { get; set; }
    private Random _random;
    private float _wanderTimer;
    private const float WANDER_INTERVAL = 3.0f; // Wander every 3 seconds
    
    public WildPet(string definitionId, string name, Vector2 position)
    {
        DefinitionId = definitionId;
        Name = name;
        Position = position;
        IsTamed = false;
        _random = new Random();
        _wanderTimer = 0f;
    }
    
    public void Update(GameTime gameTime)
    {
        if (IsTamed)
            return;
            
        // Simple wandering behavior
        _wanderTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_wanderTimer >= WANDER_INTERVAL)
        {
            _wanderTimer = 0f;
            
            // Random small movement
            float dx = (float)(_random.NextDouble() - 0.5) * 32f; // -16 to +16 pixels
            float dy = (float)(_random.NextDouble() - 0.5) * 32f;
            
            Position += new Vector2(dx, dy);
        }
    }
    
    /// <summary>
    /// Check if player is close enough to interact with this wild pet
    /// </summary>
    public bool IsInRangeOf(Vector2 playerPosition)
    {
        float distance = Vector2.Distance(Position, playerPosition);
        return distance < 48f; // Within ~3 tiles
    }
}
