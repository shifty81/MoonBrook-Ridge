using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Items;

/// <summary>
/// Consumable food item that restores hunger
/// </summary>
public class FoodItem : Item
{
    public float HungerRestored { get; set; }
    public float EnergyRestored { get; set; }
    public int HealthRestored { get; set; }
    
    public FoodItem(string name, float hungerRestored, float energyRestored = 0, int healthRestored = 0) 
        : base(name, ItemType.Food, maxStack: 99)
    {
        HungerRestored = hungerRestored;
        EnergyRestored = energyRestored;
        HealthRestored = healthRestored;
    }
}

/// <summary>
/// Consumable drink item that restores thirst
/// </summary>
public class DrinkItem : Item
{
    public float ThirstRestored { get; set; }
    public float EnergyRestored { get; set; }
    public int HealthRestored { get; set; }
    
    public DrinkItem(string name, float thirstRestored, float energyRestored = 0, int healthRestored = 0) 
        : base(name, ItemType.Food, maxStack: 99)
    {
        ThirstRestored = thirstRestored;
        EnergyRestored = energyRestored;
        HealthRestored = healthRestored;
    }
}

/// <summary>
/// Database of all consumable items in the game
/// </summary>
public static class ConsumableDatabase
{
    public static FoodItem[] Foods = 
    {
        // Basic crops
        new FoodItem("Carrot", hungerRestored: 15f, energyRestored: 5f) { SellPrice = 25, BuyPrice = 50 },
        new FoodItem("Potato", hungerRestored: 20f, energyRestored: 8f) { SellPrice = 30, BuyPrice = 60 },
        new FoodItem("Wheat Bread", hungerRestored: 40f, energyRestored: 15f) { SellPrice = 50, BuyPrice = 100 },
        new FoodItem("Corn", hungerRestored: 25f, energyRestored: 10f) { SellPrice = 35, BuyPrice = 70 },
        new FoodItem("Tomato", hungerRestored: 18f, energyRestored: 6f) { SellPrice = 28, BuyPrice = 55 },
        
        // Cooked meals
        new FoodItem("Vegetable Stew", hungerRestored: 60f, energyRestored: 25f, healthRestored: 10) 
            { SellPrice = 120, BuyPrice = 240 },
        new FoodItem("Baked Fish", hungerRestored: 55f, energyRestored: 30f, healthRestored: 15) 
            { SellPrice = 150, BuyPrice = 300 },
        new FoodItem("Farmer's Breakfast", hungerRestored: 70f, energyRestored: 40f, healthRestored: 20) 
            { SellPrice = 180, BuyPrice = 360 },
        new FoodItem("Pumpkin Pie", hungerRestored: 45f, energyRestored: 35f) 
            { SellPrice = 100, BuyPrice = 200 },
        
        // Foraged items
        new FoodItem("Wild Berry", hungerRestored: 10f, energyRestored: 3f) 
            { SellPrice = 15, BuyPrice = 30 },
        new FoodItem("Mushroom", hungerRestored: 12f, energyRestored: 5f) 
            { SellPrice = 20, BuyPrice = 40 },
        
        // Snacks
        new FoodItem("Apple", hungerRestored: 20f, energyRestored: 8f) 
            { SellPrice = 30, BuyPrice = 60 },
        new FoodItem("Cheese", hungerRestored: 35f, energyRestored: 12f) 
            { SellPrice = 60, BuyPrice = 120 },
        
        // NEW FOODS - Fresh produce
        new FoodItem("Strawberry", hungerRestored: 22f, energyRestored: 9f)
            { SellPrice = 35, BuyPrice = 70 },
        new FoodItem("Lettuce", hungerRestored: 12f, energyRestored: 4f)
            { SellPrice = 18, BuyPrice = 36 },
        new FoodItem("Melon", hungerRestored: 28f, energyRestored: 12f)
            { SellPrice = 45, BuyPrice = 90 },
        new FoodItem("Grape", hungerRestored: 16f, energyRestored: 7f)
            { SellPrice = 32, BuyPrice = 64 },
        
        // NEW FOODS - Cooked dishes
        new FoodItem("Fresh Salad", hungerRestored: 50f, energyRestored: 20f, healthRestored: 8)
            { SellPrice = 90, BuyPrice = 180 },
        new FoodItem("Pumpkin Soup", hungerRestored: 58f, energyRestored: 28f, healthRestored: 12)
            { SellPrice = 130, BuyPrice = 260 },
        new FoodItem("Strawberry Jam", hungerRestored: 38f, energyRestored: 18f)
            { SellPrice = 80, BuyPrice = 160 },
        new FoodItem("Grilled Corn", hungerRestored: 42f, energyRestored: 22f)
            { SellPrice = 75, BuyPrice = 150 },
        new FoodItem("Tomato Soup", hungerRestored: 48f, energyRestored: 24f, healthRestored: 8)
            { SellPrice = 95, BuyPrice = 190 },
        new FoodItem("Roasted Vegetables", hungerRestored: 65f, energyRestored: 30f, healthRestored: 15)
            { SellPrice = 140, BuyPrice = 280 },
        new FoodItem("Fish Sandwich", hungerRestored: 62f, energyRestored: 32f, healthRestored: 18)
            { SellPrice = 160, BuyPrice = 320 },
        new FoodItem("Fruit Salad", hungerRestored: 52f, energyRestored: 26f, healthRestored: 10)
            { SellPrice = 110, BuyPrice = 220 },
    };
    
    public static DrinkItem[] Drinks = 
    {
        // Basic drinks
        new DrinkItem("Water", thirstRestored: 40f) 
            { SellPrice = 5, BuyPrice = 10, Description = "Pure well water" },
        new DrinkItem("Spring Water", thirstRestored: 50f, energyRestored: 5f) 
            { SellPrice = 15, BuyPrice = 30, Description = "Fresh spring water" },
        
        // Crafted beverages
        new DrinkItem("Berry Juice", thirstRestored: 45f, energyRestored: 15f) 
            { SellPrice = 50, BuyPrice = 100 },
        new DrinkItem("Milk", thirstRestored: 35f, energyRestored: 10f, healthRestored: 5) 
            { SellPrice = 40, BuyPrice = 80 },
        new DrinkItem("Coffee", thirstRestored: 25f, energyRestored: 40f) 
            { SellPrice = 80, BuyPrice = 160, Description = "Increases energy significantly" },
        new DrinkItem("Tea", thirstRestored: 35f, energyRestored: 20f) 
            { SellPrice = 50, BuyPrice = 100 },
        
        // Premium drinks
        new DrinkItem("Energy Elixir", thirstRestored: 60f, energyRestored: 60f, healthRestored: 10) 
            { SellPrice = 200, BuyPrice = 400, Description = "Powerful energy boost" },
        new DrinkItem("Stamina Tonic", thirstRestored: 50f, energyRestored: 50f, healthRestored: 15) 
            { SellPrice = 180, BuyPrice = 360, Description = "Restores stamina and health" },
    };
    
    public static FoodItem GetFood(string name)
    {
        foreach (var food in Foods)
        {
            if (food.Name == name)
                return food;
        }
        return null;
    }
    
    public static DrinkItem GetDrink(string name)
    {
        foreach (var drink in Drinks)
        {
            if (drink.Name == name)
                return drink;
        }
        return null;
    }
}
