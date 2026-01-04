using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Items;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.World;

/// <summary>
/// Represents a tree that can be chopped down for wood resources
/// </summary>
public class ChoppableTree : WorldObject
{
    public int HitsRequired { get; private set; }
    public int CurrentHits { get; private set; }
    public string TreeType { get; set; }
    
    private System.Random _random;
    
    public ChoppableTree(string name, Vector2 position, Texture2D texture, string treeType = "Oak", System.Random random = null) 
        : base(name, position, texture)
    {
        TreeType = treeType;
        HitsRequired = 3; // Standard trees require 3 hits
        CurrentHits = 0;
        _random = random ?? new System.Random();
    }
    
    public ChoppableTree(string name, Vector2 position, SpriteInfo spriteInfo, string treeType = "Oak", System.Random random = null)
        : base(name, position, spriteInfo)
    {
        TreeType = treeType;
        HitsRequired = 3; // Standard trees require 3 hits
        CurrentHits = 0;
        _random = random ?? new System.Random();
    }
    
    /// <summary>
    /// Hit the tree with an axe
    /// Returns true if the tree is destroyed
    /// </summary>
    public bool Hit(out Item[] drops)
    {
        CurrentHits++;
        
        // Visual feedback - make the tree shake or fade
        if (CurrentHits < HitsRequired)
        {
            // Tree damaged but not destroyed
            Tint = Color.Lerp(Color.White, Color.Gray, (float)CurrentHits / HitsRequired);
            drops = null;
            return false;
        }
        
        // Tree destroyed - generate drops
        drops = GenerateDrops();
        return true;
    }
    
    private Item[] GenerateDrops()
    {
        // Generate 2-4 wood items
        int numDrops = _random.Next(2, 5); // 2-4 wood drops
        Item[] drops = new Item[numDrops];
        
        for (int i = 0; i < numDrops; i++)
        {
            // All trees drop wood for now
            drops[i] = new Item("Wood", ItemType.Crafting, 99)
            {
                Description = "Wood from trees, used in crafting and building",
                SellPrice = 10,
                BuyPrice = 5
            };
        }
        
        return drops;
    }
    
    /// <summary>
    /// Get the progress percentage of chopping this tree
    /// </summary>
    public float GetChopProgress()
    {
        return (float)CurrentHits / HitsRequired;
    }
}
