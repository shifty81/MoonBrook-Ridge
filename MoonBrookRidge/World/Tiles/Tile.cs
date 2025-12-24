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
            TileType.Grass or TileType.Grass01 or TileType.Grass02 or TileType.Grass03 => new Color(34, 139, 34),
            TileType.Dirt or TileType.Dirt01 or TileType.Dirt02 => new Color(139, 90, 43),
            TileType.Tilled or TileType.TilledDry => new Color(101, 67, 33),
            TileType.TilledWatered => new Color(70, 50, 30),
            TileType.Stone or TileType.Stone01 or TileType.Rock => Color.Gray,
            TileType.Water or TileType.Water01 => Color.Blue,
            TileType.Sand or TileType.Sand01 => Color.SandyBrown,
            TileType.WoodenFloor or TileType.Flooring => new Color(139, 90, 43),
            _ => Color.Green
        };
    }
    
    public bool CanPlant()
    {
        return (_type == TileType.Tilled || _type == TileType.TilledDry || _type == TileType.TilledWatered) 
               && _crop == null;
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
        if (_type == TileType.Tilled || _type == TileType.TilledDry)
        {
            _isWatered = true;
            _type = TileType.TilledWatered;
        }
    }
    
    public void RemoveCrop()
    {
        _crop = null;
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
    Grass01,
    Grass02,
    Grass03,
    Dirt,
    Dirt01,
    Dirt02,
    Tilled,
    TilledDry,
    TilledWatered,
    Stone,
    Stone01,
    Rock,
    Water,
    Water01,
    Sand,
    Sand01,
    WoodenFloor,
    Flooring
}

/// <summary>
/// Crop growing on a tile
/// </summary>
public class Crop
{
    private string _cropType;
    private int _growthStage;
    private int _maxGrowthStage;
    private float _hoursGrown;
    private float _hoursPerStage; // In game hours
    
    public Crop(string cropType, int maxStages, float hoursPerStage)
    {
        _cropType = cropType;
        _maxGrowthStage = maxStages;
        _hoursPerStage = hoursPerStage;
        _growthStage = 0;
        _hoursGrown = 0;
    }
    
    /// <summary>
    /// Updates crop growth based on elapsed game hours
    /// </summary>
    public void UpdateGrowth(float gameHoursElapsed)
    {
        if (_growthStage < _maxGrowthStage)
        {
            _hoursGrown += gameHoursElapsed;
            
            // Check if ready to advance to next stage
            while (_hoursGrown >= _hoursPerStage && _growthStage < _maxGrowthStage)
            {
                _growthStage++;
                _hoursGrown -= _hoursPerStage;
            }
        }
    }
    
    public void Update(GameTime gameTime)
    {
        // Kept for compatibility, but growth should be driven by UpdateGrowth()
    }
    
    public bool IsFullyGrown => _growthStage >= _maxGrowthStage;
    public int GrowthStage => _growthStage;
    public string CropType => _cropType;
}
