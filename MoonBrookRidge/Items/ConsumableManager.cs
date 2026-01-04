using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Characters.Player;
using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Items;

/// <summary>
/// Manages consumable item usage
/// </summary>
public class ConsumableManager
{
    private InventorySystem _inventory;
    private PlayerCharacter _player;
    
    public ConsumableManager(InventorySystem inventory, PlayerCharacter player)
    {
        _inventory = inventory;
        _player = player;
    }
    
    /// <summary>
    /// Use a consumable item from inventory
    /// </summary>
    public bool UseConsumable(Item item)
    {
        if (item == null) return false;
        
        // Check if item is in inventory
        if (_inventory.GetItemCount(item.Name) <= 0)
        {
            return false;
        }
        
        bool consumed = false;
        
        // Handle different consumable types
        if (item is FoodItem food)
        {
            consumed = ConsumeFood(food);
        }
        else if (item is DrinkItem drink)
        {
            consumed = ConsumeDrink(drink);
        }
        
        // Remove item from inventory if successfully consumed
        if (consumed)
        {
            _inventory.RemoveItem(item.Name, 1);
        }
        
        return consumed;
    }
    
    /// <summary>
    /// Consume a food item
    /// </summary>
    private bool ConsumeFood(FoodItem food)
    {
        // Don't eat if hunger is already full
        if (_player.Stats.Hunger >= 100f)
        {
            return false;
        }
        
        // Apply food effects
        _player.Stats.Eat(food.HungerRestored, food.EnergyRestored);
        
        if (food.HealthRestored > 0)
        {
            _player.Stats.ModifyHealth(food.HealthRestored);
        }
        
        return true;
    }
    
    /// <summary>
    /// Consume a drink item
    /// </summary>
    private bool ConsumeDrink(DrinkItem drink)
    {
        // Don't drink if thirst is already full
        if (_player.Stats.Thirst >= 100f)
        {
            return false;
        }
        
        // Apply drink effects
        _player.Stats.Drink(drink.ThirstRestored, drink.EnergyRestored);
        
        if (drink.HealthRestored > 0)
        {
            _player.Stats.ModifyHealth(drink.HealthRestored);
        }
        
        return true;
    }
    
    /// <summary>
    /// Quick-use consumable by slot index
    /// </summary>
    public bool UseConsumableBySlot(int slotIndex)
    {
        var slots = _inventory.GetSlots();
        
        if (slotIndex < 0 || slotIndex >= slots.Count)
        {
            return false;
        }
        
        var slot = slots[slotIndex];
        if (slot.IsEmpty)
        {
            return false;
        }
        
        return UseConsumable(slot.Item);
    }
    
    /// <summary>
    /// Get a pre-made food item by name
    /// </summary>
    public static FoodItem GetFood(string name)
    {
        foreach (var food in ConsumableDatabase.Foods)
        {
            if (food.Name == name)
            {
                return food;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Get a pre-made drink item by name
    /// </summary>
    public static DrinkItem GetDrink(string name)
    {
        foreach (var drink in ConsumableDatabase.Drinks)
        {
            if (drink.Name == name)
            {
                return drink;
            }
        }
        return null;
    }
}
