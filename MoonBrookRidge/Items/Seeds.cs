using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Items;

/// <summary>
/// Represents a seed that can be planted
/// </summary>
public class Seed : Item
{
    public string CropType { get; set; }
    public int GrowthStages { get; set; }
    public float HoursPerStage { get; set; }
    public string Season { get; set; }
    
    public Seed(string name, string cropType, int growthStages, float hoursPerStage, string season, int buyPrice, int sellPrice) 
        : base(name, ItemType.Seed, maxStack: 99)
    {
        CropType = cropType;
        GrowthStages = growthStages;
        HoursPerStage = hoursPerStage;
        Season = season;
        BuyPrice = buyPrice;
        SellPrice = sellPrice;
        Description = $"Plant in {season}. Grows in {growthStages} stages.";
    }
}

/// <summary>
/// Factory for creating seed items
/// </summary>
public static class SeedFactory
{
    public static Seed GetSeed(string seedName)
    {
        return seedName.ToLower() switch
        {
            "wheat seeds" => new Seed("Wheat Seeds", "wheat", 6, 1.0f, "Spring/Fall", 10, 5),
            "potato seeds" => new Seed("Potato Seeds", "potato", 6, 1.5f, "Spring", 25, 10),
            "carrot seeds" => new Seed("Carrot Seeds", "carrot", 6, 1.2f, "Spring/Fall", 15, 7),
            "cabbage seeds" => new Seed("Cabbage Seeds", "cabbage", 6, 2.0f, "Spring", 40, 20),
            "pumpkin seeds" => new Seed("Pumpkin Seeds", "pumpkin", 6, 3.0f, "Fall", 60, 30),
            "sunflower seeds" => new Seed("Sunflower Seeds", "sunflower", 6, 2.5f, "Summer", 50, 25),
            "beetroot seeds" => new Seed("Beetroot Seeds", "beetroot", 6, 1.8f, "Summer/Fall", 30, 15),
            "cauliflower seeds" => new Seed("Cauliflower Seeds", "cauliflower", 6, 3.5f, "Spring", 80, 40),
            "kale seeds" => new Seed("Kale Seeds", "kale", 6, 2.2f, "Spring/Fall", 35, 17),
            "parsnip seeds" => new Seed("Parsnip Seeds", "parsnip", 6, 1.0f, "Spring", 20, 10),
            "radish seeds" => new Seed("Radish Seeds", "radish", 6, 0.8f, "Summer", 12, 6),
            _ => null
        };
    }
    
    public static Seed[] GetAllSeeds()
    {
        return new Seed[]
        {
            GetSeed("wheat seeds"),
            GetSeed("potato seeds"),
            GetSeed("carrot seeds"),
            GetSeed("cabbage seeds"),
            GetSeed("pumpkin seeds"),
            GetSeed("sunflower seeds"),
            GetSeed("beetroot seeds"),
            GetSeed("cauliflower seeds"),
            GetSeed("kale seeds"),
            GetSeed("parsnip seeds"),
            GetSeed("radish seeds")
        };
    }
}
