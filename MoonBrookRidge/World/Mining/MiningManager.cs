using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Mining;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.World;

/// <summary>
/// Manages the mining system - mine levels, transitions, and rock mining
/// </summary>
public class MiningManager
{
    private Dictionary<int, MineLevel> _mineLevels;
    private MineLevel _currentLevel;
    private int _currentLevelNumber;
    private bool _inMine;
    
    private Texture2D _rockTexture;
    private List<SpriteInfo> _rockSprites;
    private Texture2D _grassTexture;
    private Texture2D _stoneTexture;
    
    public bool InMine => _inMine;
    public int CurrentLevel => _currentLevelNumber;
    
    public MiningManager(Texture2D rockTexture, List<SpriteInfo> rockSprites, 
                         Texture2D grassTexture, Texture2D stoneTexture)
    {
        _mineLevels = new Dictionary<int, MineLevel>();
        _inMine = false;
        _currentLevelNumber = 0;
        
        _rockTexture = rockTexture;
        _rockSprites = rockSprites;
        _grassTexture = grassTexture;
        _stoneTexture = stoneTexture;
    }
    
    /// <summary>
    /// Enter the mine at a specific level (1-based)
    /// </summary>
    public Vector2 EnterMine(int level = 1)
    {
        _inMine = true;
        _currentLevelNumber = level;
        
        // Generate level if not already created
        if (!_mineLevels.ContainsKey(level))
        {
            _mineLevels[level] = new MineLevel(level, 50, 50, _rockTexture, _rockSprites);
        }
        
        _currentLevel = _mineLevels[level];
        
        // Return the entrance position for player spawning
        return _currentLevel.EntrancePosition * 16; // Convert to pixel coordinates
    }
    
    /// <summary>
    /// Exit the current mine level
    /// </summary>
    public void ExitMine()
    {
        _inMine = false;
        _currentLevel = null;
    }
    
    /// <summary>
    /// Go down to the next level
    /// </summary>
    public Vector2 DescendLevel()
    {
        return EnterMine(_currentLevelNumber + 1);
    }
    
    /// <summary>
    /// Go up to the previous level
    /// </summary>
    public Vector2 AscendLevel()
    {
        if (_currentLevelNumber > 1)
        {
            return EnterMine(_currentLevelNumber - 1);
        }
        else
        {
            // Exit mine if on level 1
            ExitMine();
            return Vector2.Zero; // Signal to return to overworld
        }
    }
    
    /// <summary>
    /// Try to mine a rock at the given position
    /// </summary>
    public bool TryMine(Vector2 position, InventorySystem inventory)
    {
        if (!_inMine || _currentLevel == null)
            return false;
        
        if (_currentLevel.TryMineRock(position, out Item[] drops))
        {
            // Add drops to inventory
            if (drops != null)
            {
                foreach (var item in drops)
                {
                    inventory.AddItem(item, 1);
                }
            }
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Check if the player is near the exit stairs
    /// </summary>
    public bool IsNearExit(Vector2 playerPosition)
    {
        if (!_inMine || _currentLevel == null)
            return false;
        
        Vector2 exitPixelPos = _currentLevel.ExitPosition * 16;
        return Vector2.Distance(playerPosition, exitPixelPos) < 32;
    }
    
    /// <summary>
    /// Check if the player is near the entrance stairs (to go back up)
    /// </summary>
    public bool IsNearEntrance(Vector2 playerPosition)
    {
        if (!_inMine || _currentLevel == null)
            return false;
        
        Vector2 entrancePixelPos = _currentLevel.EntrancePosition * 16;
        return Vector2.Distance(playerPosition, entrancePixelPos) < 32;
    }
    
    /// <summary>
    /// Draw the current mine level
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        if (_inMine && _currentLevel != null)
        {
            _currentLevel.Draw(spriteBatch, _grassTexture, _stoneTexture);
        }
    }
}
