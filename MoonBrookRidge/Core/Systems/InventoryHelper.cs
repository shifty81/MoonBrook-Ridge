using System;
using System.Collections.Generic;
using System.Linq;
using MoonBrookRidge.Items;
using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Quality of life helper for inventory management
/// Provides sorting, stacking, and quick actions for player convenience
/// </summary>
public class InventoryHelper
{
    /// <summary>
    /// Sorts inventory by item type, then by name
    /// </summary>
    public static void SortInventory(InventorySystem inventory)
    {
        var slots = inventory.GetSlots();
        var items = new List<(Item item, int quantity)>();
        
        // Collect all items
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty)
            {
                items.Add((slot.Item, slot.Quantity));
            }
        }
        
        // Sort by type, then by name
        items.Sort((a, b) =>
        {
            int typeComparison = a.item.Type.CompareTo(b.item.Type);
            if (typeComparison != 0)
                return typeComparison;
            return string.Compare(a.item.Name, b.item.Name, StringComparison.Ordinal);
        });
        
        // Clear inventory
        foreach (var (item, quantity) in items)
        {
            inventory.RemoveItem(item.Name, quantity);
        }
        
        // Re-add items in sorted order
        foreach (var (item, quantity) in items)
        {
            inventory.AddItem(item, quantity);
        }
    }
    
    /// <summary>
    /// Finds the first slot containing a specific item
    /// </summary>
    public static int FindItemSlot(InventorySystem inventory, string itemName)
    {
        var slots = inventory.GetSlots();
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            if (!slot.IsEmpty && slot.Item.Name == itemName)
            {
                return i;
            }
        }
        return -1;
    }
    
    /// <summary>
    /// Counts total quantity of a specific item across all slots
    /// </summary>
    public static int CountItem(InventorySystem inventory, string itemName)
    {
        return inventory.GetItemCount(itemName);
    }
    
    /// <summary>
    /// Gets all unique item types in inventory
    /// </summary>
    public static List<ItemType> GetItemTypes(InventorySystem inventory)
    {
        var types = new HashSet<ItemType>();
        var slots = inventory.GetSlots();
        
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty)
            {
                types.Add(slot.Item.Type);
            }
        }
        
        return types.ToList();
    }
    
    /// <summary>
    /// Counts empty slots in inventory
    /// </summary>
    public static int CountEmptySlots(InventorySystem inventory)
    {
        var slots = inventory.GetSlots();
        return slots.Count(s => s.IsEmpty);
    }
    
    /// <summary>
    /// Checks if inventory has space for an item
    /// </summary>
    public static bool HasSpace(InventorySystem inventory, Item item, int quantity)
    {
        var slots = inventory.GetSlots();
        
        // Check if item already exists and can be stacked
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.Item.Name == item.Name)
            {
                int remainingSpace = slot.Item.MaxStackSize - slot.Quantity;
                quantity -= remainingSpace;
                if (quantity <= 0)
                    return true;
            }
        }
        
        // Check for empty slots
        int emptySlots = CountEmptySlots(inventory);
        int slotsNeeded = (int)Math.Ceiling(quantity / (double)item.MaxStackSize);
        
        return emptySlots >= slotsNeeded;
    }
    
    /// <summary>
    /// Removes all items of a specific type
    /// </summary>
    public static void RemoveAllOfType(InventorySystem inventory, ItemType type)
    {
        var slots = inventory.GetSlots();
        var itemsToRemove = new List<(string name, int quantity)>();
        
        foreach (var slot in slots)
        {
            if (!slot.IsEmpty && slot.Item.Type == type)
            {
                itemsToRemove.Add((slot.Item.Name, slot.Quantity));
            }
        }
        
        foreach (var (name, quantity) in itemsToRemove)
        {
            inventory.RemoveItem(name, quantity);
        }
    }
    
    /// <summary>
    /// Gets inventory statistics
    /// </summary>
    public static InventoryStats GetStats(InventorySystem inventory)
    {
        var slots = inventory.GetSlots();
        var stats = new InventoryStats
        {
            TotalSlots = slots.Count,
            EmptySlots = 0,
            UsedSlots = 0,
            TotalItems = 0,
            UniqueItems = 0
        };
        
        var uniqueNames = new HashSet<string>();
        
        foreach (var slot in slots)
        {
            if (slot.IsEmpty)
            {
                stats.EmptySlots++;
            }
            else
            {
                stats.UsedSlots++;
                stats.TotalItems += slot.Quantity;
                uniqueNames.Add(slot.Item.Name);
            }
        }
        
        stats.UniqueItems = uniqueNames.Count;
        return stats;
    }
}

/// <summary>
/// Statistics about inventory state
/// </summary>
public struct InventoryStats
{
    public int TotalSlots;
    public int EmptySlots;
    public int UsedSlots;
    public int TotalItems;
    public int UniqueItems;
    
    public float UsagePercentage => TotalSlots > 0 
        ? (UsedSlots / (float)TotalSlots) * 100f 
        : 0f;
}
