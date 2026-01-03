using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MoonBrookRidge.World.Buildings;

/// <summary>
/// Manages furniture placement and building interiors
/// </summary>
public class FurnitureSystem
{
    private Dictionary<Vector2, List<Furniture>> _buildingFurniture; // Building Position -> Furniture list
    private Dictionary<string, Furniture> _furnitureTemplates;
    
    public event Action<Building, Furniture>? OnFurniturePlaced;
    public event Action<Building, Furniture>? OnFurnitureRemoved;
    
    public FurnitureSystem()
    {
        _buildingFurniture = new Dictionary<Vector2, List<Furniture>>();
        _furnitureTemplates = new Dictionary<string, Furniture>();
        
        InitializeFurnitureTemplates();
    }
    
    /// <summary>
    /// Initialize all available furniture templates
    /// </summary>
    private void InitializeFurnitureTemplates()
    {
        // Beds
        AddTemplate(new Furniture("basic_bed", "Basic Bed", "A simple wooden bed for sleeping", FurnitureType.Bed, new Point(2, 1))
        {
            ComfortValue = 20,
            Cost = new FurnitureCost(gold: 100, wood: 20, cloth: 5)
        });
        
        AddTemplate(new Furniture("double_bed", "Double Bed", "A comfortable bed for two", FurnitureType.Bed, new Point(3, 2))
        {
            ComfortValue = 40,
            Cost = new FurnitureCost(gold: 250, wood: 30, cloth: 10)
        });
        
        // Chairs
        AddTemplate(new Furniture("wooden_chair", "Wooden Chair", "A simple wooden chair", FurnitureType.Chair, new Point(1, 1))
        {
            ComfortValue = 5,
            Cost = new FurnitureCost(gold: 25, wood: 5)
        });
        
        AddTemplate(new Furniture("armchair", "Armchair", "A comfortable armchair", FurnitureType.Chair, new Point(1, 1))
        {
            ComfortValue = 15,
            Cost = new FurnitureCost(gold: 75, wood: 10, cloth: 5)
        });
        
        // Tables
        AddTemplate(new Furniture("small_table", "Small Table", "A small wooden table", FurnitureType.Table, new Point(1, 1))
        {
            ComfortValue = 5,
            Cost = new FurnitureCost(gold: 30, wood: 8)
        });
        
        AddTemplate(new Furniture("dining_table", "Dining Table", "A large dining table", FurnitureType.Table, new Point(3, 2))
        {
            ComfortValue = 20,
            Cost = new FurnitureCost(gold: 150, wood: 25)
        });
        
        // Storage
        AddTemplate(new Furniture("wooden_chest", "Wooden Chest", "Storage for items", FurnitureType.Storage, new Point(1, 1))
        {
            ComfortValue = 10,
            Cost = new FurnitureCost(gold: 50, wood: 15)
        });
        
        AddTemplate(new Furniture("bookshelf", "Bookshelf", "A tall bookshelf for books and decorations", FurnitureType.Storage, new Point(1, 2))
        {
            ComfortValue = 15,
            Cost = new FurnitureCost(gold: 80, wood: 20)
        });
        
        AddTemplate(new Furniture("dresser", "Dresser", "A dresser for clothing storage", FurnitureType.Storage, new Point(2, 1))
        {
            ComfortValue = 20,
            Cost = new FurnitureCost(gold: 100, wood: 25)
        });
        
        // Decorations
        AddTemplate(new Furniture("potted_plant", "Potted Plant", "A decorative plant", FurnitureType.Decoration, new Point(1, 1))
        {
            ComfortValue = 10,
            IsWalkable = true,
            Cost = new FurnitureCost(gold: 20, wood: 5)
        });
        
        AddTemplate(new Furniture("painting", "Painting", "A beautiful painting", FurnitureType.Decoration, new Point(1, 1))
        {
            ComfortValue = 15,
            IsWalkable = true,
            Cost = new FurnitureCost(gold: 50)
        });
        
        AddTemplate(new Furniture("rug", "Rug", "A decorative floor rug", FurnitureType.Decoration, new Point(2, 2))
        {
            ComfortValue = 10,
            IsWalkable = true,
            Cost = new FurnitureCost(gold: 40, cloth: 8)
        });
        
        // Lights
        AddTemplate(new Furniture("candle", "Candle", "A simple candle for light", FurnitureType.Light, new Point(1, 1))
        {
            ComfortValue = 5,
            IsWalkable = true,
            Cost = new FurnitureCost(gold: 10)
        });
        
        AddTemplate(new Furniture("lantern", "Lantern", "A hanging lantern", FurnitureType.Light, new Point(1, 1))
        {
            ComfortValue = 10,
            IsWalkable = true,
            Cost = new FurnitureCost(gold: 30, iron: 5)
        });
        
        // Appliances
        AddTemplate(new Furniture("stove", "Stove", "A cooking stove for preparing meals", FurnitureType.Appliance, new Point(2, 1))
        {
            ComfortValue = 30,
            Cost = new FurnitureCost(gold: 200, iron: 20, stone: 10)
        });
        
        AddTemplate(new Furniture("fireplace", "Fireplace", "A warm fireplace", FurnitureType.Appliance, new Point(2, 1))
        {
            ComfortValue = 40,
            Cost = new FurnitureCost(gold: 250, stone: 30, iron: 10)
        });
    }
    
    private void AddTemplate(Furniture furniture)
    {
        _furnitureTemplates[furniture.Id] = furniture;
    }
    
    /// <summary>
    /// Get all available furniture templates
    /// </summary>
    public List<Furniture> GetAllTemplates()
    {
        return _furnitureTemplates.Values.ToList();
    }
    
    /// <summary>
    /// Get furniture templates by type
    /// </summary>
    public List<Furniture> GetTemplatesByType(FurnitureType type)
    {
        return _furnitureTemplates.Values.Where(f => f.Type == type).ToList();
    }
    
    /// <summary>
    /// Get a furniture template by ID
    /// </summary>
    public Furniture? GetTemplate(string furnitureId)
    {
        return _furnitureTemplates.TryGetValue(furnitureId, out var furniture) ? furniture : null;
    }
    
    /// <summary>
    /// Place furniture in a building
    /// </summary>
    public bool PlaceFurniture(Building building, string furnitureId, Vector2 position)
    {
        if (!_furnitureTemplates.TryGetValue(furnitureId, out var template))
            return false;
        
        // Create a new instance from template
        var furniture = new Furniture(template.Id, template.Name, template.Description, template.Type, template.Size)
        {
            Position = position,
            IsWalkable = template.IsWalkable,
            ComfortValue = template.ComfortValue,
            TexturePath = template.TexturePath,
            Cost = template.Cost
        };
        
        // Get or create furniture list for this building
        if (!_buildingFurniture.ContainsKey(building.Position))
        {
            _buildingFurniture[building.Position] = new List<Furniture>();
        }
        
        // Check for collision with existing furniture
        if (HasCollision(building.Position, furniture))
            return false;
        
        _buildingFurniture[building.Position].Add(furniture);
        OnFurniturePlaced?.Invoke(building, furniture);
        return true;
    }
    
    /// <summary>
    /// Remove furniture from a building
    /// </summary>
    public bool RemoveFurniture(Building building, Furniture furniture)
    {
        if (!_buildingFurniture.TryGetValue(building.Position, out var furnitureList))
            return false;
        
        bool removed = furnitureList.Remove(furniture);
        if (removed)
        {
            OnFurnitureRemoved?.Invoke(building, furniture);
        }
        return removed;
    }
    
    /// <summary>
    /// Get all furniture in a building
    /// </summary>
    public List<Furniture> GetBuildingFurniture(Vector2 buildingPosition)
    {
        return _buildingFurniture.TryGetValue(buildingPosition, out var furniture) 
            ? new List<Furniture>(furniture) 
            : new List<Furniture>();
    }
    
    /// <summary>
    /// Check if furniture placement would collide with existing furniture
    /// </summary>
    private bool HasCollision(Vector2 buildingPosition, Furniture newFurniture)
    {
        if (!_buildingFurniture.TryGetValue(buildingPosition, out var furnitureList))
            return false;
        
        var newBounds = new Rectangle(
            (int)newFurniture.Position.X,
            (int)newFurniture.Position.Y,
            newFurniture.Size.X,
            newFurniture.Size.Y
        );
        
        foreach (var existing in furnitureList)
        {
            var existingBounds = new Rectangle(
                (int)existing.Position.X,
                (int)existing.Position.Y,
                existing.Size.X,
                existing.Size.Y
            );
            
            if (newBounds.Intersects(existingBounds))
                return true;
        }
        
        return false;
    }
    
    /// <summary>
    /// Calculate total comfort value for a building
    /// </summary>
    public int GetBuildingComfort(Vector2 buildingPosition)
    {
        if (!_buildingFurniture.TryGetValue(buildingPosition, out var furniture))
            return 0;
        
        return furniture.Sum(f => f.ComfortValue);
    }
    
    /// <summary>
    /// Get furniture statistics
    /// </summary>
    public (int total, int byType) GetFurnitureStats(FurnitureType? type = null)
    {
        int total = _buildingFurniture.Values.Sum(list => list.Count);
        int byType = type.HasValue 
            ? _buildingFurniture.Values.Sum(list => list.Count(f => f.Type == type.Value))
            : 0;
        
        return (total, byType);
    }
}
