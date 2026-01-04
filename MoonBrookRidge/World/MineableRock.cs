using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Items;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.World;

/// <summary>
/// Represents a rock that can be mined for resources
/// </summary>
public class MineableRock : WorldObject
{
    public int HitsRequired { get; private set; }
    public int CurrentHits { get; private set; }
    public int MineLevel { get; set; } // Determines what drops it can give
    
    private System.Random _random;
    
    public MineableRock(string name, Vector2 position, Texture2D texture, int mineLevel = 1, System.Random random = null) 
        : base(name, position, texture)
    {
        MineLevel = mineLevel;
        HitsRequired = DetermineHitsRequired(mineLevel);
        CurrentHits = 0;
        _random = random ?? new System.Random();
    }
    
    public MineableRock(string name, Vector2 position, SpriteInfo spriteInfo, int mineLevel = 1, System.Random random = null)
        : base(name, position, spriteInfo)
    {
        MineLevel = mineLevel;
        HitsRequired = DetermineHitsRequired(mineLevel);
        CurrentHits = 0;
        _random = random ?? new System.Random();
    }
    
    private int DetermineHitsRequired(int level)
    {
        // Deeper rocks require more hits
        return level switch
        {
            1 => 3,
            2 => 4,
            3 => 5,
            4 => 6,
            5 => 7,
            _ => 8
        };
    }
    
    /// <summary>
    /// Hit the rock with a pickaxe
    /// Returns true if the rock is destroyed
    /// </summary>
    public bool Hit(out Item[] drops)
    {
        CurrentHits++;
        
        // Visual feedback - make the rock flash or shake
        if (CurrentHits < HitsRequired)
        {
            // Rock damaged but not destroyed
            Tint = Color.Gray;
            drops = null;
            return false;
        }
        
        // Rock destroyed - generate drops
        drops = GenerateDrops();
        return true;
    }
    
    private Item[] GenerateDrops()
    {
        // Generate 1-3 items based on mine level
        int numDrops = _random.Next(1, 4); // 1-3 drops
        Item[] drops = new Item[numDrops];
        
        for (int i = 0; i < numDrops; i++)
        {
            drops[i] = MineralFactory.GetRandomMineralDrop(MineLevel, _random);
        }
        
        return drops;
    }
    
    /// <summary>
    /// Get the progress percentage of breaking this rock
    /// </summary>
    public float GetBreakProgress()
    {
        return (float)CurrentHits / HitsRequired;
    }
}
