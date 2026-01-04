using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.World.Buildings;

/// <summary>
/// Represents a piece of furniture that can be placed inside buildings
/// </summary>
public class Furniture
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public FurnitureType Type { get; set; }
    public Vector2 Position { get; set; } // Position relative to building interior
    public Point Size { get; set; } // Size in tiles (width, height)
    public bool IsWalkable { get; set; } // Can player walk through it?
    public int ComfortValue { get; set; } // Affects player happiness
    public string TexturePath { get; set; }
    
    /// <summary>
    /// Crafting cost for this furniture
    /// </summary>
    public FurnitureCost Cost { get; set; }
    
    public Furniture(string id, string name, string description, FurnitureType type, Point size)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
        Size = size;
        Position = Vector2.Zero;
        IsWalkable = false;
        ComfortValue = 10;
        TexturePath = $"Furniture/{type}/{id}";
        Cost = new FurnitureCost();
    }
}

/// <summary>
/// Types of furniture
/// </summary>
public enum FurnitureType
{
    Bed,           // Player sleep/rest
    Chair,         // Seating
    Table,         // Surface for items
    Storage,       // Chests and cabinets
    Decoration,    // Paintings, plants, etc.
    Light,         // Lamps and candles
    Appliance,     // Stove, fridge, etc.
    Workstation    // Crafting stations
}

/// <summary>
/// Crafting requirements for furniture
/// </summary>
public class FurnitureCost
{
    public int Gold { get; set; }
    public int Wood { get; set; }
    public int Stone { get; set; }
    public int Iron { get; set; }
    public int Cloth { get; set; }
    
    public FurnitureCost(int gold = 0, int wood = 0, int stone = 0, int iron = 0, int cloth = 0)
    {
        Gold = gold;
        Wood = wood;
        Stone = stone;
        Iron = iron;
        Cloth = cloth;
    }
}
