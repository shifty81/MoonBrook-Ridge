using System.Linq;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Farming.Tools;
using MoonBrookRidge.Characters.Player;

namespace MoonBrookRidge.Items;

/// <summary>
/// Manages seed planting from inventory
/// </summary>
public class SeedManager
{
    private InventorySystem _inventory;
    private ToolManager _toolManager;
    private PlayerCharacter _player;
    
    public SeedManager(InventorySystem inventory, ToolManager toolManager, PlayerCharacter player)
    {
        _inventory = inventory;
        _toolManager = toolManager;
        _player = player;
    }
    
    /// <summary>
    /// Try to plant a seed from inventory at the specified position
    /// </summary>
    public bool TryPlantSeed(Vector2 worldPosition)
    {
        // Find first seed in inventory
        var slots = _inventory.GetSlots();
        foreach (var slot in slots)
        {
            if (slot.Item is Seed seed && !slot.IsEmpty)
            {
                // Try to plant the seed
                bool planted = _toolManager.PlantSeed(
                    worldPosition,
                    seed.CropType,
                    seed.GrowthStages,
                    seed.HoursPerStage
                );
                
                if (planted)
                {
                    // Remove one seed from inventory
                    _inventory.RemoveItem(seed.Name, 1);
                    return true;
                }
                
                return false;
            }
        }
        
        return false; // No seeds in inventory
    }
    
    /// <summary>
    /// Check if player has any seeds
    /// </summary>
    public bool HasSeeds()
    {
        var slots = _inventory.GetSlots();
        return slots.Any(slot => slot.Item is Seed && !slot.IsEmpty);
    }
}
