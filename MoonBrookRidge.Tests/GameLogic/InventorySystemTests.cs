using Xunit;
using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Tests.GameLogic;

public class InventorySystemTests
{
    [Fact]
    public void Constructor_ShouldCreateInventoryWithMaxSlots()
    {
        var inventory = new InventorySystem(36);
        var slots = inventory.GetSlots();

        Assert.Equal(36, slots.Count);
        Assert.All(slots, slot => Assert.True(slot.IsEmpty));
    }

    [Fact]
    public void AddItem_ShouldAddItemToEmptySlot()
    {
        var inventory = new InventorySystem(36);
        var item = new Item("Wheat", ItemType.Crop, 99);

        var result = inventory.AddItem(item, 10);

        Assert.True(result);
        Assert.Equal(10, inventory.GetItemCount("Wheat"));
    }

    [Fact]
    public void AddItem_ShouldStackItemsInExistingSlot()
    {
        var inventory = new InventorySystem(36);
        var item = new Item("Wheat", ItemType.Crop, 99);

        inventory.AddItem(item, 10);
        var result = inventory.AddItem(item, 20);

        Assert.True(result);
        Assert.Equal(30, inventory.GetItemCount("Wheat"));
    }

    [Fact]
    public void AddItem_ShouldRespectMaxStackSize()
    {
        var inventory = new InventorySystem(36);
        var item = new Item("Wheat", ItemType.Crop, 20); // Max stack of 20

        inventory.AddItem(item, 15);
        var result = inventory.AddItem(item, 10); // Should create a new stack

        Assert.True(result);
        Assert.Equal(25, inventory.GetItemCount("Wheat"));

        var slots = inventory.GetSlots().Where(s => !s.IsEmpty).ToList();
        Assert.Equal(2, slots.Count); // Should be in 2 slots
    }

    [Fact]
    public void AddItem_ShouldReturnFalse_WhenInventoryFull()
    {
        var inventory = new InventorySystem(2); // Only 2 slots
        var item1 = new Item("Wheat", ItemType.Crop, 1); // Max stack of 1
        var item2 = new Item("Corn", ItemType.Crop, 1);
        var item3 = new Item("Carrot", ItemType.Crop, 1);

        inventory.AddItem(item1, 1);
        inventory.AddItem(item2, 1);
        var result = inventory.AddItem(item3, 1); // Should fail

        Assert.False(result);
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItemFromInventory()
    {
        var inventory = new InventorySystem(36);
        var item = new Item("Wheat", ItemType.Crop, 99);

        inventory.AddItem(item, 20);
        var result = inventory.RemoveItem("Wheat", 10);

        Assert.True(result);
        Assert.Equal(10, inventory.GetItemCount("Wheat"));
    }

    [Fact]
    public void RemoveItem_ShouldRemoveItemCompletely_WhenQuantityIsZero()
    {
        var inventory = new InventorySystem(36);
        var item = new Item("Wheat", ItemType.Crop, 99);

        inventory.AddItem(item, 10);
        var result = inventory.RemoveItem("Wheat", 10);

        Assert.True(result);
        Assert.Equal(0, inventory.GetItemCount("Wheat"));

        var slots = inventory.GetSlots();
        Assert.All(slots, slot => Assert.True(slot.IsEmpty));
    }

    [Fact]
    public void RemoveItem_ShouldReturnFalse_WhenNotEnoughQuantity()
    {
        var inventory = new InventorySystem(36);
        var item = new Item("Wheat", ItemType.Crop, 99);

        inventory.AddItem(item, 5);
        var result = inventory.RemoveItem("Wheat", 10);

        Assert.False(result);
        Assert.Equal(5, inventory.GetItemCount("Wheat")); // Should remain unchanged
    }

    [Fact]
    public void RemoveItem_ShouldRemoveFromMultipleStacks()
    {
        var inventory = new InventorySystem(36);
        var item = new Item("Wheat", ItemType.Crop, 20); // Max stack of 20

        inventory.AddItem(item, 20); // First stack
        inventory.AddItem(item, 20); // Second stack
        inventory.AddItem(item, 10); // Third stack (partial)

        var result = inventory.RemoveItem("Wheat", 35); // Remove from multiple stacks

        Assert.True(result);
        Assert.Equal(15, inventory.GetItemCount("Wheat"));
    }

    [Fact]
    public void GetItemCount_ShouldReturnZero_WhenItemNotInInventory()
    {
        var inventory = new InventorySystem(36);
        var count = inventory.GetItemCount("NonExistent");

        Assert.Equal(0, count);
    }

    [Fact]
    public void GetItemCount_ShouldReturnCorrectTotal_ForMultipleStacks()
    {
        var inventory = new InventorySystem(36);
        var item = new Item("Wheat", ItemType.Crop, 10);

        inventory.AddItem(item, 10);
        inventory.AddItem(item, 10);
        inventory.AddItem(item, 5);

        Assert.Equal(25, inventory.GetItemCount("Wheat"));
    }

    [Fact]
    public void InventorySlot_IsEmpty_ShouldReturnTrue_WhenItemIsNull()
    {
        var slot = new InventorySlot();
        Assert.True(slot.IsEmpty);
    }

    [Fact]
    public void InventorySlot_IsEmpty_ShouldReturnTrue_WhenQuantityIsZero()
    {
        var slot = new InventorySlot
        {
            Item = new Item("Wheat", ItemType.Crop, 99),
            Quantity = 0
        };
        Assert.True(slot.IsEmpty);
    }

    [Fact]
    public void InventorySlot_IsEmpty_ShouldReturnFalse_WhenItemAndQuantityPresent()
    {
        var slot = new InventorySlot
        {
            Item = new Item("Wheat", ItemType.Crop, 99),
            Quantity = 10
        };
        Assert.False(slot.IsEmpty);
    }

    [Fact]
    public void Item_ShouldStoreCorrectProperties()
    {
        var item = new Item("Wheat", ItemType.Crop, 99)
        {
            Description = "A crop",
            SellPrice = 10,
            BuyPrice = 5
        };

        Assert.Equal("Wheat", item.Name);
        Assert.Equal(ItemType.Crop, item.Type);
        Assert.Equal(99, item.MaxStackSize);
        Assert.Equal("A crop", item.Description);
        Assert.Equal(10, item.SellPrice);
        Assert.Equal(5, item.BuyPrice);
    }
}
