using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Items;

/// <summary>
/// Represents a harvested crop item
/// </summary>
public class HarvestItem : Item
{
    public int HarvestCount { get; set; }
    public Quality ItemQuality { get; set; }
    
    public HarvestItem(string name, int sellPrice, int buyPrice, Quality quality = Quality.Normal) 
        : base(name, ItemType.Crop, maxStack: 99)
    {
        ItemQuality = quality;
        HarvestCount = 1;
        SellPrice = sellPrice;
        BuyPrice = buyPrice;
        Description = $"A fresh {name.ToLower()}. Can be sold or used in recipes.";
    }
}

public enum Quality
{
    Normal,
    Silver,
    Gold,
    Iridium
}

/// <summary>
/// Factory for creating harvest items from crops
/// </summary>
public static class HarvestFactory
{
    public static HarvestItem GetHarvestItem(string cropType, int quantity = 1, Quality quality = Quality.Normal)
    {
        HarvestItem item = cropType.ToLower() switch
        {
            "wheat" => new HarvestItem("Wheat", 25, 10, quality),
            "potato" => new HarvestItem("Potato", 35, 25, quality),
            "carrot" => new HarvestItem("Carrot", 30, 15, quality),
            "cabbage" => new HarvestItem("Cabbage", 50, 40, quality),
            "pumpkin" => new HarvestItem("Pumpkin", 80, 60, quality),
            "sunflower" => new HarvestItem("Sunflower", 60, 50, quality),
            "beetroot" => new HarvestItem("Beetroot", 45, 30, quality),
            "cauliflower" => new HarvestItem("Cauliflower", 100, 80, quality),
            "kale" => new HarvestItem("Kale", 55, 35, quality),
            "parsnip" => new HarvestItem("Parsnip", 28, 20, quality),
            "radish" => new HarvestItem("Radish", 20, 12, quality),
            // New crops
            "strawberry" => new HarvestItem("Strawberry", 65, 45, quality),
            "lettuce" => new HarvestItem("Lettuce", 32, 18, quality),
            "tomato" => new HarvestItem("Tomato", 70, 55, quality),
            "corn" => new HarvestItem("Corn", 85, 65, quality),
            "melon" => new HarvestItem("Melon", 95, 75, quality),
            "grape" => new HarvestItem("Grape", 90, 70, quality),
            "winter root" => new HarvestItem("Winter Root", 48, 30, quality),
            _ => new HarvestItem("Unknown Crop", 10, 5, quality)
        };
        
        item.HarvestCount = quantity;
        return item;
    }
    
    /// <summary>
    /// Calculate harvest quantity based on various factors
    /// </summary>
    public static int CalculateHarvestQuantity(bool wasWatered, Quality quality)
    {
        int baseQuantity = 1;
        
        // Watered crops yield more
        if (wasWatered)
        {
            baseQuantity += 1;
        }
        
        // Quality bonus
        baseQuantity += quality switch
        {
            Quality.Silver => 1,
            Quality.Gold => 2,
            Quality.Iridium => 3,
            _ => 0
        };
        
        return baseQuantity;
    }
}
