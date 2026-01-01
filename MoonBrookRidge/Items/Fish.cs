using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Items;

/// <summary>
/// Represents a fish item caught from fishing
/// </summary>
public class FishItem : Item
{
    public FishRarity Rarity { get; set; }
    public FishHabitat Habitat { get; set; }
    public string Season { get; set; }
    
    public FishItem(string name, int sellPrice, int buyPrice, FishRarity rarity = FishRarity.Common, FishHabitat habitat = FishHabitat.River, string season = "All") 
        : base(name, ItemType.Fish, maxStack: 99)
    {
        Rarity = rarity;
        Habitat = habitat;
        Season = season;
        SellPrice = sellPrice;
        BuyPrice = buyPrice;
        
        // Proper pluralization for habitat
        string habitatText = habitat switch
        {
            FishHabitat.Ocean => "the ocean",
            FishHabitat.River => "rivers",
            FishHabitat.Lake => "lakes",
            FishHabitat.Any => "any water",
            _ => habitat.ToString().ToLower() + "s"
        };
        
        Description = $"A {rarity.ToString().ToLower()} {name.ToLower()}. Found in {habitatText}.";
    }
}

public enum FishRarity
{
    Common,      // Easy to catch, low value
    Uncommon,    // Moderate difficulty, decent value
    Rare,        // Hard to catch, high value
    Legendary    // Very rare, extremely valuable
}

public enum FishHabitat
{
    River,       // Rivers and streams
    Lake,        // Lakes and ponds
    Ocean,       // Ocean/sea water
    Any          // Can be found anywhere
}

/// <summary>
/// Factory for creating fish items
/// </summary>
public static class FishFactory
{
    public static FishItem GetFishItem(string fishType, int quantity = 1)
    {
        FishItem item = fishType.ToLower() switch
        {
            // Common fish
            "sunfish" => new FishItem("Sunfish", 30, 0, FishRarity.Common, FishHabitat.River, "Spring/Summer"),
            "chub" => new FishItem("Chub", 25, 0, FishRarity.Common, FishHabitat.River, "All"),
            "bream" => new FishItem("Bream", 35, 0, FishRarity.Common, FishHabitat.Lake, "All"),
            "carp" => new FishItem("Carp", 40, 0, FishRarity.Common, FishHabitat.Lake, "All"),
            "sardine" => new FishItem("Sardine", 20, 0, FishRarity.Common, FishHabitat.Ocean, "All"),
            "anchovy" => new FishItem("Anchovy", 25, 0, FishRarity.Common, FishHabitat.Ocean, "Spring/Fall"),
            
            // Uncommon fish
            "bass" => new FishItem("Bass", 75, 0, FishRarity.Uncommon, FishHabitat.River, "All"),
            "pike" => new FishItem("Pike", 85, 0, FishRarity.Uncommon, FishHabitat.Lake, "Summer/Fall"),
            "perch" => new FishItem("Perch", 70, 0, FishRarity.Uncommon, FishHabitat.River, "Winter"),
            "trout" => new FishItem("Trout", 90, 0, FishRarity.Uncommon, FishHabitat.River, "Spring/Fall"),
            "salmon" => new FishItem("Salmon", 100, 0, FishRarity.Uncommon, FishHabitat.River, "Fall"),
            "tuna" => new FishItem("Tuna", 110, 0, FishRarity.Uncommon, FishHabitat.Ocean, "Summer"),
            
            // Rare fish
            "catfish" => new FishItem("Catfish", 150, 0, FishRarity.Rare, FishHabitat.River, "Spring/Summer"),
            "walleye" => new FishItem("Walleye", 175, 0, FishRarity.Rare, FishHabitat.Lake, "Fall/Winter"),
            "sturgeon" => new FishItem("Sturgeon", 200, 0, FishRarity.Rare, FishHabitat.Lake, "Winter"),
            "swordfish" => new FishItem("Swordfish", 225, 0, FishRarity.Rare, FishHabitat.Ocean, "Summer"),
            "red snapper" => new FishItem("Red Snapper", 180, 0, FishRarity.Rare, FishHabitat.Ocean, "All"),
            
            // Legendary fish
            "legendary fish" => new FishItem("Legendary Fish", 500, 0, FishRarity.Legendary, FishHabitat.Any, "All"),
            "golden trout" => new FishItem("Golden Trout", 450, 0, FishRarity.Legendary, FishHabitat.River, "Summer"),
            "king salmon" => new FishItem("King Salmon", 550, 0, FishRarity.Legendary, FishHabitat.River, "Fall"),
            "giant squid" => new FishItem("Giant Squid", 600, 0, FishRarity.Legendary, FishHabitat.Ocean, "Winter"),
            
            _ => new FishItem("Unknown Fish", 10, 0, FishRarity.Common, FishHabitat.Any, "All")
        };
        
        return item;
    }
    
    /// <summary>
    /// Get a random fish drop based on habitat and current season
    /// </summary>
    public static FishItem GetRandomFish(FishHabitat habitat, string currentSeason, System.Random random)
    {
        int roll = random.Next(100);
        
        // Common fish (60% chance)
        if (roll < 60)
        {
            return habitat switch
            {
                FishHabitat.River => random.Next(2) == 0 ? GetFishItem("sunfish") : GetFishItem("chub"),
                FishHabitat.Lake => random.Next(2) == 0 ? GetFishItem("bream") : GetFishItem("carp"),
                FishHabitat.Ocean => random.Next(2) == 0 ? GetFishItem("sardine") : GetFishItem("anchovy"),
                _ => GetFishItem("chub")
            };
        }
        
        // Uncommon fish (30% chance)
        if (roll < 90)
        {
            return habitat switch
            {
                FishHabitat.River => random.Next(3) switch
                {
                    0 => GetFishItem("bass"),
                    1 => GetFishItem("trout"),
                    _ => GetFishItem("salmon")
                },
                FishHabitat.Lake => random.Next(2) == 0 ? GetFishItem("pike") : GetFishItem("perch"),
                FishHabitat.Ocean => GetFishItem("tuna"),
                _ => GetFishItem("bass")
            };
        }
        
        // Rare fish (9% chance)
        if (roll < 99)
        {
            return habitat switch
            {
                FishHabitat.River => GetFishItem("catfish"),
                FishHabitat.Lake => random.Next(2) == 0 ? GetFishItem("walleye") : GetFishItem("sturgeon"),
                FishHabitat.Ocean => random.Next(2) == 0 ? GetFishItem("swordfish") : GetFishItem("red snapper"),
                _ => GetFishItem("catfish")
            };
        }
        
        // Legendary fish (1% chance)
        return habitat switch
        {
            FishHabitat.River => random.Next(2) == 0 ? GetFishItem("golden trout") : GetFishItem("king salmon"),
            FishHabitat.Ocean => GetFishItem("giant squid"),
            _ => GetFishItem("legendary fish")
        };
    }
}
