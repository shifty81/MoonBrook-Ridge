using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Farming.Tools;

/// <summary>
/// Base class for farming tools
/// </summary>
public abstract class Tool
{
    protected string _name;
    protected int _tier;
    protected float _energyCost;
    
    public Tool(string name, int tier = 1)
    {
        _name = name;
        _tier = tier;
        _energyCost = 2f;
    }
    
    public abstract void Use(Vector2 position);
    
    public string Name => _name;
    public int Tier => _tier;
    public float EnergyCost => _energyCost;
}

/// <summary>
/// Hoe for tilling soil
/// </summary>
public class Hoe : Tool
{
    public Hoe(int tier = 1) : base("Hoe", tier) { }
    
    public override void Use(Vector2 position)
    {
        // Till the tile at the given position
        // This would interact with the WorldMap to change tile type
    }
}

/// <summary>
/// Watering can for watering crops
/// </summary>
public class WateringCan : Tool
{
    private int _waterLevel;
    private int _maxWaterLevel;
    
    public WateringCan(int tier = 1) : base("Watering Can", tier)
    {
        _maxWaterLevel = 40 * tier;
        _waterLevel = _maxWaterLevel;
    }
    
    public override void Use(Vector2 position)
    {
        if (_waterLevel > 0)
        {
            // Water the tile at the given position
            _waterLevel--;
        }
    }
    
    public void Refill()
    {
        _waterLevel = _maxWaterLevel;
    }
    
    public int WaterLevel => _waterLevel;
    public int MaxWaterLevel => _maxWaterLevel;
}

/// <summary>
/// Axe for chopping trees
/// </summary>
public class Axe : Tool
{
    public Axe(int tier = 1) : base("Axe", tier) { }
    
    public override void Use(Vector2 position)
    {
        // Chop tree at position
    }
}

/// <summary>
/// Pickaxe for mining
/// </summary>
public class Pickaxe : Tool
{
    public Pickaxe(int tier = 1) : base("Pickaxe", tier) { }
    
    public override void Use(Vector2 position)
    {
        // Mine rock at position
    }
}

/// <summary>
/// Fishing rod for catching fish
/// </summary>
public class FishingRod : Tool
{
    public FishingRod(int tier = 1) : base("Fishing Rod", tier) { }
    
    public override void Use(Vector2 position)
    {
        // Start fishing minigame
    }
}

/// <summary>
/// Scythe for harvesting crops and cutting grass
/// </summary>
public class Scythe : Tool
{
    public Scythe(int tier = 1) : base("Scythe", tier) { }
    
    public override void Use(Vector2 position)
    {
        // Harvest crop or cut grass
    }
}
