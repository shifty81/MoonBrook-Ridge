using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.Core;

namespace MoonBrookRidge.World.Buildings;

/// <summary>
/// Building type enumeration
/// </summary>
public enum BuildingType
{
    Barn,
    Coop,
    Shed,
    Silo,
    Well,
    Greenhouse,
    Mill,
    Workshop
}

/// <summary>
/// Represents a placed building in the world
/// </summary>
public class Building
{
    public BuildingType Type { get; set; }
    public Vector2 Position { get; set; }
    public int TileWidth { get; set; }
    public int TileHeight { get; set; }
    public int Tier { get; set; }
    public bool IsConstructed { get; set; }
    
    /// <summary>
    /// Width in pixels
    /// </summary>
    public int Width => TileWidth * GameConstants.TILE_SIZE;
    
    /// <summary>
    /// Height in pixels
    /// </summary>
    public int Height => TileHeight * GameConstants.TILE_SIZE;
    
    public Building(BuildingType type, Vector2 position, int width = 3, int height = 3)
    {
        Type = type;
        Position = position;
        TileWidth = width;
        TileHeight = height;
        Tier = 1;
        IsConstructed = true;
    }
    
    /// <summary>
    /// Check if this building occupies a specific tile position
    /// </summary>
    public bool OccupiesTile(Vector2 tilePosition)
    {
        return tilePosition.X >= Position.X && 
               tilePosition.X < Position.X + TileWidth &&
               tilePosition.Y >= Position.Y && 
               tilePosition.Y < Position.Y + TileHeight;
    }
    
    /// <summary>
    /// Get all tiles occupied by this building
    /// </summary>
    public List<Vector2> GetOccupiedTiles()
    {
        var tiles = new List<Vector2>();
        for (int x = 0; x < TileWidth; x++)
        {
            for (int y = 0; y < TileHeight; y++)
            {
                tiles.Add(new Vector2(Position.X + x, Position.Y + y));
            }
        }
        return tiles;
    }
    
    /// <summary>
    /// Draw the building
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Texture2D texture, Color color)
    {
        if (texture != null)
        {
            var destRect = new Rectangle(
                (int)Position.X,
                (int)Position.Y,
                Width,
                Height
            );
            spriteBatch.Draw(texture, destRect, color);
        }
    }
}

/// <summary>
/// Building definition with costs and requirements
/// </summary>
public class BuildingDefinition
{
    public BuildingType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public int TileWidth { get; set; }
    public int TileHeight { get; set; }
    public int GoldCost { get; set; }
    public Dictionary<string, int> MaterialCosts { get; private set; }
    
    public BuildingDefinition(BuildingType type, string name, string description, 
                            int width, int height, int goldCost)
    {
        Type = type;
        Name = name;
        Description = description;
        TileWidth = width;
        TileHeight = height;
        GoldCost = goldCost;
        MaterialCosts = new Dictionary<string, int>();
    }
    
    public void AddMaterial(string materialName, int quantity)
    {
        MaterialCosts[materialName] = quantity;
    }
}

/// <summary>
/// Manages building definitions and provides building info
/// </summary>
public static class BuildingDefinitions
{
    private static Dictionary<BuildingType, BuildingDefinition> _definitions;
    
    static BuildingDefinitions()
    {
        _definitions = new Dictionary<BuildingType, BuildingDefinition>();
        InitializeDefinitions();
    }
    
    private static void InitializeDefinitions()
    {
        // Barn - houses animals
        var barn = new BuildingDefinition(
            BuildingType.Barn,
            "Barn",
            "A large barn for housing livestock. Can hold up to 12 animals.",
            4, 3, 6000
        );
        barn.AddMaterial("Wood", 200);
        barn.AddMaterial("Stone", 100);
        _definitions[BuildingType.Barn] = barn;
        
        // Coop - houses chickens and ducks
        var coop = new BuildingDefinition(
            BuildingType.Coop,
            "Coop",
            "A cozy coop for chickens and ducks. Can hold up to 8 birds.",
            3, 2, 4000
        );
        coop.AddMaterial("Wood", 150);
        coop.AddMaterial("Stone", 50);
        _definitions[BuildingType.Coop] = coop;
        
        // Shed - storage building
        var shed = new BuildingDefinition(
            BuildingType.Shed,
            "Shed",
            "A storage shed. Provides 120 additional inventory slots.",
            3, 2, 3000
        );
        shed.AddMaterial("Wood", 100);
        shed.AddMaterial("Stone", 50);
        _definitions[BuildingType.Shed] = shed;
        
        // Silo - stores hay
        var silo = new BuildingDefinition(
            BuildingType.Silo,
            "Silo",
            "Stores hay for feeding animals. Holds up to 240 hay.",
            2, 3, 1000
        );
        silo.AddMaterial("Wood", 50);
        silo.AddMaterial("Stone", 50);
        silo.AddMaterial("Copper Ore", 10);
        _definitions[BuildingType.Silo] = silo;
        
        // Well - provides water
        var well = new BuildingDefinition(
            BuildingType.Well,
            "Well",
            "A stone well for collecting water. Water refills every 2 minutes.",
            1, 1, 500
        );
        well.AddMaterial("Stone", 75);
        _definitions[BuildingType.Well] = well;
        
        // Greenhouse - grows crops year-round
        var greenhouse = new BuildingDefinition(
            BuildingType.Greenhouse,
            "Greenhouse",
            "Grow crops in any season. Includes 48 tillable tiles.",
            6, 5, 15000
        );
        greenhouse.AddMaterial("Wood", 300);
        greenhouse.AddMaterial("Stone", 100);
        greenhouse.AddMaterial("Iron Ore", 50);
        greenhouse.AddMaterial("Gold Ore", 20);
        _definitions[BuildingType.Greenhouse] = greenhouse;
        
        // Mill - processes crops into products
        var mill = new BuildingDefinition(
            BuildingType.Mill,
            "Mill",
            "Process crops into flour, sugar, and other products.",
            3, 3, 5000
        );
        mill.AddMaterial("Wood", 150);
        mill.AddMaterial("Stone", 100);
        mill.AddMaterial("Iron Ore", 25);
        _definitions[BuildingType.Mill] = mill;
        
        // Workshop - advanced crafting
        var workshop = new BuildingDefinition(
            BuildingType.Workshop,
            "Workshop",
            "Unlock advanced crafting recipes and smelt ores.",
            3, 2, 4000
        );
        workshop.AddMaterial("Wood", 100);
        workshop.AddMaterial("Stone", 100);
        workshop.AddMaterial("Iron Ore", 50);
        _definitions[BuildingType.Workshop] = workshop;
    }
    
    public static BuildingDefinition GetDefinition(BuildingType type)
    {
        return _definitions.ContainsKey(type) ? _definitions[type] : null;
    }
    
    public static List<BuildingDefinition> GetAllDefinitions()
    {
        return new List<BuildingDefinition>(_definitions.Values);
    }
}

/// <summary>
/// Manager for building construction and placement
/// </summary>
public class BuildingManager
{
    private List<Building> _placedBuildings;
    
    public BuildingManager()
    {
        _placedBuildings = new List<Building>();
    }
    
    /// <summary>
    /// Check if a position is valid for building placement
    /// </summary>
    public bool IsValidPlacement(BuildingType type, Vector2 position, Tile[,] worldTiles)
    {
        var definition = BuildingDefinitions.GetDefinition(type);
        if (definition == null) return false;
        
        // Check if within world bounds
        int worldWidth = worldTiles.GetLength(0);
        int worldHeight = worldTiles.GetLength(1);
        
        if (position.X < 0 || position.Y < 0 ||
            position.X + definition.TileWidth > worldWidth ||
            position.Y + definition.TileHeight > worldHeight)
        {
            return false;
        }
        
        // Check if all tiles are valid (grass or dirt)
        for (int x = 0; x < definition.TileWidth; x++)
        {
            for (int y = 0; y < definition.TileHeight; y++)
            {
                int tileX = (int)position.X + x;
                int tileY = (int)position.Y + y;
                
                var tile = worldTiles[tileX, tileY];
                
                // Can only build on grass or dirt
                if (tile.Type != TileType.Grass && tile.Type != TileType.Dirt)
                {
                    return false;
                }
            }
        }
        
        // Check for overlap with existing buildings
        foreach (var existingBuilding in _placedBuildings)
        {
            for (int x = 0; x < definition.TileWidth; x++)
            {
                for (int y = 0; y < definition.TileHeight; y++)
                {
                    Vector2 checkPos = new Vector2(position.X + x, position.Y + y);
                    if (existingBuilding.OccupiesTile(checkPos))
                    {
                        return false;
                    }
                }
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Check if player has enough resources to build
    /// </summary>
    public bool CanAfford(BuildingType type, InventorySystem inventory, int playerGold)
    {
        var definition = BuildingDefinitions.GetDefinition(type);
        if (definition == null) return false;
        
        // Check gold
        if (playerGold < definition.GoldCost) return false;
        
        // Check materials
        foreach (var material in definition.MaterialCosts)
        {
            if (inventory.GetItemCount(material.Key) < material.Value)
            {
                return false;
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Attempt to construct a building. Returns new player gold amount, or -1 if failed.
    /// </summary>
    public int ConstructBuilding(BuildingType type, Vector2 position, 
                                  InventorySystem inventory, int playerGold,
                                  Tile[,] worldTiles)
    {
        var definition = BuildingDefinitions.GetDefinition(type);
        if (definition == null) return -1;
        
        // Validate placement and resources
        if (!IsValidPlacement(type, position, worldTiles)) return -1;
        if (!CanAfford(type, inventory, playerGold)) return -1;
        
        // Consume resources
        int newGold = playerGold - definition.GoldCost;
        
        foreach (var material in definition.MaterialCosts)
        {
            inventory.RemoveItem(material.Key, material.Value);
        }
        
        // Create and place building
        var building = new Building(type, position, definition.TileWidth, definition.TileHeight);
        _placedBuildings.Add(building);
        
        return newGold;
    }
    
    /// <summary>
    /// Get building at a specific tile position
    /// </summary>
    public Building GetBuildingAtPosition(Vector2 tilePosition)
    {
        return _placedBuildings.FirstOrDefault(b => b.OccupiesTile(tilePosition));
    }
    
    /// <summary>
    /// Get all placed buildings
    /// </summary>
    public List<Building> GetAllBuildings()
    {
        return new List<Building>(_placedBuildings);
    }
    
    /// <summary>
    /// Remove a building (for demolition)
    /// </summary>
    public bool RemoveBuilding(Building building)
    {
        return _placedBuildings.Remove(building);
    }
}
