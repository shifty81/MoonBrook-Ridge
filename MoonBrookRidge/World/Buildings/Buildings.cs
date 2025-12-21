using Microsoft.Xna.Framework;

namespace MoonBrookRidge.World.Buildings;

/// <summary>
/// Water well building for collecting water
/// </summary>
public class Well
{
    private Vector2 _position;
    private bool _hasWater;
    private float _refillTimer;
    private const float REFILL_TIME = 120f; // 2 minutes to refill
    
    public Well(Vector2 position)
    {
        _position = position;
        _hasWater = true;
        _refillTimer = 0;
    }
    
    public void Update(GameTime gameTime)
    {
        if (!_hasWater)
        {
            _refillTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            if (_refillTimer >= REFILL_TIME)
            {
                _hasWater = true;
                _refillTimer = 0;
            }
        }
    }
    
    /// <summary>
    /// Attempt to collect water from the well
    /// </summary>
    public bool CollectWater()
    {
        if (_hasWater)
        {
            _hasWater = false;
            return true;
        }
        return false;
    }
    
    public Vector2 Position => _position;
    public bool HasWater => _hasWater;
    public float RefillProgress => _refillTimer / REFILL_TIME;
}
