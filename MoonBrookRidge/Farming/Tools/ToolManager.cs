using Microsoft.Xna.Framework;
using MoonBrookRidge.Characters.Player;
using MoonBrookRidge.World;
using MoonBrookRidge.World.Maps;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.World.Fishing;
using MoonBrookRidge.Core;
using MoonBrookRidge.Items;
using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Farming.Tools;

/// <summary>
/// Manages tool usage and interaction with the world
/// </summary>
public class ToolManager
{
    private WorldMap _worldMap;
    private PlayerCharacter _player;
    private Tool _currentTool;
    private InventorySystem _inventory;
    private MiningManager _miningManager;
    private FishingManager _fishingManager;
    
    public ToolManager(WorldMap worldMap, PlayerCharacter player, InventorySystem inventory = null)
    {
        _worldMap = worldMap;
        _player = player;
        _inventory = inventory;
        _currentTool = null;
    }
    
    public void SetMiningManager(MiningManager miningManager)
    {
        _miningManager = miningManager;
    }
    
    public void SetFishingManager(FishingManager fishingManager)
    {
        _fishingManager = fishingManager;
    }
    
    public void SetCurrentTool(Tool tool)
    {
        _currentTool = tool;
    }
    
    public Tool GetCurrentTool() => _currentTool;
    
    /// <summary>
    /// Use the currently equipped tool at the given world position
    /// </summary>
    public bool UseTool(Vector2 worldPosition, PlayerStats stats)
    {
        if (_currentTool == null) return false;
        
        // Check if player has enough energy
        if (stats.Energy < _currentTool.EnergyCost) return false;
        
        // Convert world position to grid position
        Vector2 gridPos = WorldToGridPosition(worldPosition);
        Tile tile = _worldMap.GetTile((int)gridPos.X, (int)gridPos.Y);
        
        if (tile == null) return false;
        
        bool toolUsed = false;
        
        // Execute tool-specific logic
        if (_currentTool is Hoe)
        {
            toolUsed = UseTill(tile);
        }
        else if (_currentTool is WateringCan wateringCan)
        {
            toolUsed = UseWater(tile, wateringCan);
        }
        else if (_currentTool is Axe)
        {
            // TODO: Implement tree chopping
            toolUsed = false;
        }
        else if (_currentTool is Pickaxe)
        {
            // Try to mine rocks in the mine
            if (_miningManager != null && _miningManager.InMine)
            {
                toolUsed = _miningManager.TryMine(worldPosition, _inventory);
            }
            else
            {
                // TODO: Implement rock breaking in overworld
                toolUsed = false;
            }
        }
        else if (_currentTool is Scythe)
        {
            toolUsed = UseHarvest(tile);
        }
        // Note: FishingRod is handled separately in GameplayState.HandleToolInput
        
        // Consume energy if tool was used
        if (toolUsed)
        {
            stats.ConsumeEnergy(_currentTool.EnergyCost);
        }
        
        return toolUsed;
    }
    
    /// <summary>
    /// Till soil with hoe
    /// </summary>
    private bool UseTill(Tile tile)
    {
        // Can only till grass or dirt tiles
        if (tile.Type == TileType.Grass || tile.Type == TileType.Grass01 || 
            tile.Type == TileType.Grass02 || tile.Type == TileType.Grass03 ||
            tile.Type == TileType.Dirt || tile.Type == TileType.Dirt01 || tile.Type == TileType.Dirt02)
        {
            tile.Type = TileType.TilledDry;
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Water crops with watering can
    /// </summary>
    private bool UseWater(Tile tile, WateringCan wateringCan)
    {
        // Can only water tilled soil
        if ((tile.Type == TileType.Tilled || tile.Type == TileType.TilledDry) && wateringCan.WaterLevel > 0)
        {
            tile.Water();
            wateringCan.Use(Vector2.Zero); // Decreases water level
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Harvest crops with scythe
    /// </summary>
    private bool UseHarvest(Tile tile)
    {
        // Can only harvest fully grown crops
        if (tile.Crop != null && tile.Crop.IsFullyGrown)
        {
            // Get the crop type to create harvest item
            string cropType = tile.Crop.CropType;
            
            // Calculate harvest quantity (watered tiles give more)
            int quantity = HarvestFactory.CalculateHarvestQuantity(tile.IsWatered, Quality.Normal);
            
            // Add harvested crop to inventory
            if (_inventory != null)
            {
                HarvestItem harvest = HarvestFactory.GetHarvestItem(cropType, quantity);
                _inventory.AddItem(harvest, quantity);
            }
            
            // Remove the crop from the tile
            tile.RemoveCrop();
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Plant a seed at the given position
    /// </summary>
    public bool PlantSeed(Vector2 worldPosition, string cropType, int maxStages, float hoursPerStage)
    {
        Vector2 gridPos = WorldToGridPosition(worldPosition);
        Tile tile = _worldMap.GetTile((int)gridPos.X, (int)gridPos.Y);
        
        if (tile == null) return false;
        
        if (tile.CanPlant())
        {
            Crop crop = new Crop(cropType, maxStages, hoursPerStage);
            tile.PlantCrop(crop);
            return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Convert world position to grid position
    /// </summary>
    private Vector2 WorldToGridPosition(Vector2 worldPosition)
    {
        return new Vector2(
            (int)(worldPosition.X / GameConstants.TILE_SIZE),
            (int)(worldPosition.Y / GameConstants.TILE_SIZE)
        );
    }
}
