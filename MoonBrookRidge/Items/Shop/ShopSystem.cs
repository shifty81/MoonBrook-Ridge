using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Characters.Player;
using System.Collections.Generic;

namespace MoonBrookRidge.Items.Shop;

/// <summary>
/// Shop system for buying and selling items
/// </summary>
public class ShopSystem
{
    private List<ShopItem> _shopInventory;
    
    public ShopSystem()
    {
        _shopInventory = new List<ShopItem>();
        InitializeShopInventory();
    }
    
    private void InitializeShopInventory()
    {
        // Seeds
        _shopInventory.Add(new ShopItem(SeedFactory.GetSeed("wheat seeds"), 50, 20));
        _shopInventory.Add(new ShopItem(SeedFactory.GetSeed("carrot seeds"), 80, 30));
        _shopInventory.Add(new ShopItem(SeedFactory.GetSeed("potato seeds"), 100, 40));
        _shopInventory.Add(new ShopItem(SeedFactory.GetSeed("cabbage seeds"), 120, 45));
        _shopInventory.Add(new ShopItem(SeedFactory.GetSeed("pumpkin seeds"), 200, 80));
        
        // Food and drinks
        _shopInventory.Add(new ShopItem(ConsumableManager.GetFood("Apple"), 30, 10));
        _shopInventory.Add(new ShopItem(ConsumableManager.GetFood("Carrot"), 20, 8));
        _shopInventory.Add(new ShopItem(ConsumableManager.GetDrink("Water"), 10, 5));
        _shopInventory.Add(new ShopItem(ConsumableManager.GetDrink("Spring Water"), 25, 10));
        
        // Basic crafting materials
        var wood = new Item("Wood", ItemType.Crafting);
        wood.BuyPrice = 50;
        wood.SellPrice = 10;
        _shopInventory.Add(new ShopItem(wood, 50, 10));
        
        var stone = new Item("Stone", ItemType.Mineral);
        stone.BuyPrice = 30;
        stone.SellPrice = 5;
        _shopInventory.Add(new ShopItem(stone, 30, 5));
    }
    
    public bool BuyItem(ShopItem shopItem, int quantity, InventorySystem inventory, PlayerCharacter player)
    {
        int totalCost = shopItem.BuyPrice * quantity;
        
        // Check if player has enough money
        if (player.Money < totalCost)
        {
            return false;
        }
        
        // Try to add item to inventory
        bool success = inventory.AddItem(shopItem.Item, quantity);
        
        if (success)
        {
            player.SpendMoney(totalCost);
            return true;
        }
        
        return false; // Inventory full
    }
    
    public bool SellItem(string itemName, int quantity, InventorySystem inventory, PlayerCharacter player)
    {
        // Check if player has the item
        int owned = inventory.GetItemCount(itemName);
        if (owned < quantity)
        {
            return false;
        }
        
        // Find the item in inventory to get its sell price
        var slots = inventory.GetSlots();
        Item itemToSell = null;
        foreach (var slot in slots)
        {
            if (slot.Item != null && slot.Item.Name == itemName)
            {
                itemToSell = slot.Item;
                break;
            }
        }
        
        if (itemToSell == null)
        {
            return false;
        }
        
        // Remove item from inventory
        bool success = inventory.RemoveItem(itemName, quantity);
        
        if (success)
        {
            int totalValue = itemToSell.SellPrice * quantity;
            player.AddMoney(totalValue);
            return true;
        }
        
        return false;
    }
    
    public List<ShopItem> GetShopInventory() => _shopInventory;
}

/// <summary>
/// Represents an item available in the shop
/// </summary>
public class ShopItem
{
    public Item Item { get; set; }
    public int BuyPrice { get; set; }
    public int SellPrice { get; set; }
    
    public ShopItem(Item item, int buyPrice, int sellPrice)
    {
        Item = item;
        BuyPrice = buyPrice;
        SellPrice = sellPrice;
        
        // Sync prices with item
        item.BuyPrice = buyPrice;
        item.SellPrice = sellPrice;
    }
}
