using System.Collections.Generic;
using System.Linq;

namespace MoonBrookRidge.Items.Inventory;

/// <summary>
/// Player inventory system
/// </summary>
public class InventorySystem
{
    private List<InventorySlot> _slots;
    private int _maxSlots;
    
    public InventorySystem(int maxSlots = 36)
    {
        _maxSlots = maxSlots;
        _slots = new List<InventorySlot>();
        
        // Initialize empty slots
        for (int i = 0; i < maxSlots; i++)
        {
            _slots.Add(new InventorySlot());
        }
    }
    
    public bool AddItem(Item item, int quantity = 1)
    {
        // Try to stack with existing items first
        var existingSlot = _slots.FirstOrDefault(s => s.Item != null && 
                                                      s.Item.Name == item.Name && 
                                                      s.Quantity < item.MaxStackSize);
        
        if (existingSlot != null)
        {
            int amountToAdd = System.Math.Min(quantity, item.MaxStackSize - existingSlot.Quantity);
            existingSlot.Quantity += amountToAdd;
            quantity -= amountToAdd;
            
            if (quantity <= 0) return true;
        }
        
        // Add to empty slot
        var emptySlot = _slots.FirstOrDefault(s => s.Item == null);
        if (emptySlot != null)
        {
            emptySlot.Item = item;
            emptySlot.Quantity = quantity;
            return true;
        }
        
        return false; // Inventory full
    }
    
    public bool RemoveItem(string itemName, int quantity = 1)
    {
        var slotsWithItem = _slots.Where(s => s.Item != null && s.Item.Name == itemName).ToList();
        
        int totalQuantity = slotsWithItem.Sum(s => s.Quantity);
        if (totalQuantity < quantity) return false;
        
        int remaining = quantity;
        foreach (var slot in slotsWithItem)
        {
            if (remaining <= 0) break;
            
            int toRemove = System.Math.Min(remaining, slot.Quantity);
            slot.Quantity -= toRemove;
            remaining -= toRemove;
            
            if (slot.Quantity <= 0)
            {
                slot.Item = null;
            }
        }
        
        return true;
    }
    
    public int GetItemCount(string itemName)
    {
        return _slots.Where(s => s.Item != null && s.Item.Name == itemName)
                    .Sum(s => s.Quantity);
    }
    
    public List<InventorySlot> GetSlots() => _slots;
}

/// <summary>
/// Single inventory slot
/// </summary>
public class InventorySlot
{
    public Item Item { get; set; }
    public int Quantity { get; set; }
    
    public bool IsEmpty => Item == null || Quantity <= 0;
}

/// <summary>
/// Base item class
/// </summary>
public class Item
{
    public string Name { get; set; }
    public string Description { get; set; }
    public ItemType Type { get; set; }
    public int MaxStackSize { get; set; }
    public int SellPrice { get; set; }
    public int BuyPrice { get; set; }
    
    public Item(string name, ItemType type, int maxStack = 99)
    {
        Name = name;
        Type = type;
        MaxStackSize = maxStack;
    }
}

public enum ItemType
{
    Tool,
    Seed,
    Crop,
    Fish,
    Mineral,
    Gem,
    Crafting,
    Food,
    Gift
}
