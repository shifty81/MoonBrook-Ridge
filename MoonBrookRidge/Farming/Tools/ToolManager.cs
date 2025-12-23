using Microsoft.Xna.Framework;
using MoonBrookRidge.Characters.Player;
using MoonBrookRidge.World.Maps;
using MoonBrookRidge.World.Tiles;

namespace MoonBrookRidge.Farming.Tools;

/// <summary>
/// Manages tool usage and interaction with the world
/// </summary>
public class ToolManager
{
    private WorldMap _worldMap;
    private PlayerCharacter _player;
    private Tool _currentTool;
    
    public ToolManager(WorldMap worldMap, PlayerCharacter player)
    {
        _worldMap = worldMap;
        _player = player;
        _currentTool = null;
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
            // TODO: Implement rock mining
            toolUsed = false;
        }
        else if (_currentTool is Scythe)
        {
            toolUsed = UseHarvest(tile);
        }
        
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
            // TODO: Add harvested crop to inventory
            // For now, just remove the crop
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
        const int TILE_SIZE = 16;
        return new Vector2(
            (int)(worldPosition.X / TILE_SIZE),
            (int)(worldPosition.Y / TILE_SIZE)
        );
    }
}
