using Microsoft.Xna.Framework;

namespace MoonBrookRidge.World.Tiles;

/// <summary>
/// Individual tile in the world
/// </summary>
public class Tile
{
    private TileType _type;
    private Vector2 _gridPosition;
    private bool _isWatered;
    private Crop _crop;
    
    public Tile(TileType type, Vector2 gridPosition)
    {
        _type = type;
        _gridPosition = gridPosition;
        _isWatered = false;
        _crop = null;
    }
    
    public void Update(GameTime gameTime)
    {
        // Update crop if present
        if (_crop != null)
        {
            _crop.Update(gameTime);
        }
        
        // Watered state resets at end of day
    }
    
    public Color GetColor()
    {
        return _type switch
        {
            TileType.Grass => new Color(34, 139, 34),
            TileType.Dirt => new Color(139, 90, 43),
            TileType.Tilled => new Color(101, 67, 33),
            TileType.Stone => Color.Gray,
            TileType.Water => Color.Blue,
            TileType.Sand => Color.SandyBrown,
            _ => Color.Green
        };
    }
    
    public bool CanPlant()
    {
        return _type == TileType.Tilled && _crop == null;
    }
    
    public void PlantCrop(Crop crop)
    {
        if (CanPlant())
        {
            _crop = crop;
        }
    }
    
    public void Water()
    {
        if (_type == TileType.Tilled)
        {
            _isWatered = true;
        }
    }
    
    // Properties
    public TileType Type
    {
        get => _type;
        set => _type = value;
    }
    
    public bool IsWatered => _isWatered;
    public Crop Crop => _crop;
    public Vector2 GridPosition => _gridPosition;
}

public enum TileType
{
    Grass,
    Dirt,
    Tilled,
    Stone,
    Water,
    Sand
}

/// <summary>
/// Crop growing on a tile
/// </summary>
public class Crop
{
    private string _cropType;
    private int _growthStage;
    private int _maxGrowthStage;
    private float _growthTimer;
    private float _growthTimePerStage; // In game hours
    
    public Crop(string cropType, int maxStages, float hoursPerStage)
    {
        _cropType = cropType;
        _maxGrowthStage = maxStages;
        _growthTimePerStage = hoursPerStage;
        _growthStage = 0;
        _growthTimer = 0;
    }
    
    public void Update(GameTime gameTime)
    {
        if (_growthStage < _maxGrowthStage)
        {
            _growthTimer += (float)gameTime.ElapsedGameTime.TotalSeconds;
            
            // Simple growth logic (would integrate with TimeSystem in full implementation)
            if (_growthTimer >= _growthTimePerStage * 3600) // Convert hours to seconds
            {
                _growthStage++;
                _growthTimer = 0;
            }
        }
    }
    
    public bool IsFullyGrown => _growthStage >= _maxGrowthStage;
    public int GrowthStage => _growthStage;
    public string CropType => _cropType;
}
