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
        // Seeds - all should exist in SeedFactory
        AddShopSeed("wheat seeds", 50, 20);
        AddShopSeed("carrot seeds", 80, 30);
        AddShopSeed("potato seeds", 100, 40);
        AddShopSeed("cabbage seeds", 120, 45);
        AddShopSeed("pumpkin seeds", 200, 80);
        
        // Food and drinks - all should exist in ConsumableDatabase
        AddShopFood("Apple", 30, 10);
        AddShopFood("Carrot", 20, 8);
        AddShopDrink("Water", 10, 5);
        AddShopDrink("Spring Water", 25, 10);
        
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
    
    private void AddShopSeed(string seedName, int buyPrice, int sellPrice)
    {
        var seed = SeedFactory.GetSeed(seedName);
        if (seed != null)
        {
            _shopInventory.Add(new ShopItem(seed, buyPrice, sellPrice));
        }
    }
    
    private void AddShopFood(string foodName, int buyPrice, int sellPrice)
    {
        var food = ConsumableManager.GetFood(foodName);
        if (food != null)
        {
            _shopInventory.Add(new ShopItem(food, buyPrice, sellPrice));
        }
    }
    
    private void AddShopDrink(string drinkName, int buyPrice, int sellPrice)
    {
        var drink = ConsumableManager.GetDrink(drinkName);
        if (drink != null)
        {
            _shopInventory.Add(new ShopItem(drink, buyPrice, sellPrice));
        }
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
