using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Items;

/// <summary>
/// Represents a mineral or ore item from mining
/// </summary>
public class MineralItem : Item
{
    public MineralRarity Rarity { get; set; }
    
    public MineralItem(string name, int sellPrice, int buyPrice, MineralRarity rarity = MineralRarity.Common) 
        : base(name, ItemType.Mineral, maxStack: 99)
    {
        Rarity = rarity;
        SellPrice = sellPrice;
        BuyPrice = buyPrice;
        Description = $"{name} ore. Can be smelted or sold for profit.";
    }
}

/// <summary>
/// Represents a gem item from mining
/// </summary>
public class GemItem : Item
{
    public MineralRarity Rarity { get; set; }
    
    public GemItem(string name, int sellPrice, int buyPrice, MineralRarity rarity = MineralRarity.Rare) 
        : base(name, ItemType.Gem, maxStack: 99)
    {
        Rarity = rarity;
        SellPrice = sellPrice;
        BuyPrice = buyPrice;
        Description = $"A precious {name.ToLower()}. Highly valuable.";
    }
}

public enum MineralRarity
{
    Common,      // Stone, Copper
    Uncommon,    // Iron, Coal
    Rare,        // Gold, Silver
    VeryRare,    // Gems, Precious metals
    Legendary    // Rare gems
}

/// <summary>
/// Factory for creating mineral and gem items
/// </summary>
public static class MineralFactory
{
    public static MineralItem GetMineralItem(string mineralType, int quantity = 1)
    {
        MineralItem item = mineralType.ToLower() switch
        {
            "stone" => new MineralItem("Stone", 2, 0, MineralRarity.Common),
            "copper ore" => new MineralItem("Copper Ore", 15, 0, MineralRarity.Common),
            "iron ore" => new MineralItem("Iron Ore", 30, 0, MineralRarity.Uncommon),
            "coal" => new MineralItem("Coal", 25, 0, MineralRarity.Uncommon),
            "gold ore" => new MineralItem("Gold Ore", 100, 0, MineralRarity.Rare),
            "silver ore" => new MineralItem("Silver Ore", 75, 0, MineralRarity.Rare),
            "iridium ore" => new MineralItem("Iridium Ore", 250, 0, MineralRarity.Legendary),
            _ => new MineralItem("Unknown Mineral", 1, 0, MineralRarity.Common)
        };
        
        return item;
    }
    
    public static GemItem GetGemItem(string gemType, int quantity = 1)
    {
        GemItem item = gemType.ToLower() switch
        {
            "quartz" => new GemItem("Quartz", 20, 0, MineralRarity.Common),
            "amethyst" => new GemItem("Amethyst", 100, 0, MineralRarity.Rare),
            "topaz" => new GemItem("Topaz", 80, 0, MineralRarity.Rare),
            "emerald" => new GemItem("Emerald", 250, 0, MineralRarity.VeryRare),
            "ruby" => new GemItem("Ruby", 300, 0, MineralRarity.VeryRare),
            "diamond" => new GemItem("Diamond", 750, 0, MineralRarity.Legendary),
            _ => new GemItem("Unknown Gem", 10, 0, MineralRarity.Common)
        };
        
        return item;
    }
    
    /// <summary>
    /// Get a random mineral drop based on mine level (deeper = better ores)
    /// </summary>
    public static Item GetRandomMineralDrop(int mineLevel, System.Random random)
    {
        int roll = random.Next(100);
        
        // Common drops (60% chance)
        if (roll < 60)
        {
            if (random.Next(10) < 7) // 70% stone, 30% copper
                return GetMineralItem("stone");
            else
                return GetMineralItem("copper ore");
        }
        
        // Uncommon drops (25% chance)
        if (roll < 85)
        {
            if (mineLevel >= 3)
            {
                if (random.Next(10) < 6) // 60% iron, 40% coal
                    return GetMineralItem("iron ore");
                else
                    return GetMineralItem("coal");
            }
            return GetMineralItem("copper ore"); // Not deep enough for iron
        }
        
        // Rare drops (12% chance)
        if (roll < 97)
        {
            if (mineLevel >= 5)
            {
                if (random.Next(10) < 6) // 60% gold, 40% silver
                    return GetMineralItem("gold ore");
                else
                    return GetMineralItem("silver ore");
            }
            if (mineLevel >= 3)
                return GetMineralItem("iron ore");
            return GetMineralItem("copper ore");
        }
        
        // Very rare drops (3% chance) - gems
        if (mineLevel >= 5)
        {
            int gemRoll = random.Next(100);
            if (gemRoll < 40)
                return GetGemItem("quartz");
            else if (gemRoll < 70)
                return GetGemItem("amethyst");
            else if (gemRoll < 85)
                return GetGemItem("topaz");
            else if (gemRoll < 95)
                return GetGemItem("emerald");
            else if (gemRoll < 99)
                return GetGemItem("ruby");
            else
                return GetGemItem("diamond");
        }
        
        return GetGemItem("quartz"); // Fallback
    }
}
