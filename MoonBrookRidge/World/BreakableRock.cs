using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.Items;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.World;

/// <summary>
/// Represents a decorative rock that can be broken for stone resources in the overworld
/// </summary>
public class BreakableRock : WorldObject
{
    public int HitsRequired { get; private set; }
    public int CurrentHits { get; private set; }
    
    private System.Random _random;
    
    public BreakableRock(string name, Vector2 position, Texture2D texture, System.Random random = null) 
        : base(name, position, texture)
    {
        HitsRequired = 2; // Overworld rocks are easier than mine rocks
        CurrentHits = 0;
        _random = random ?? new System.Random();
    }
    
    public BreakableRock(string name, Vector2 position, SpriteInfo spriteInfo, System.Random random = null)
        : base(name, position, spriteInfo)
    {
        HitsRequired = 2; // Overworld rocks are easier than mine rocks
        CurrentHits = 0;
        _random = random ?? new System.Random();
    }
    
    /// <summary>
    /// Hit the rock with a pickaxe
    /// Returns true if the rock is destroyed
    /// </summary>
    public bool Hit(out Item[] drops)
    {
        CurrentHits++;
        
        // Visual feedback - make the rock crack
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
        // Generate 1-3 stone items
        int numDrops = _random.Next(1, 4); // 1-3 stone drops
        Item[] drops = new Item[numDrops];
        
        for (int i = 0; i < numDrops; i++)
        {
            // Overworld rocks drop basic stone
            drops[i] = new Item("Stone", ItemType.Mineral, 99)
            {
                Description = "Basic stone for building and crafting",
                SellPrice = 5,
                BuyPrice = 2
            };
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
