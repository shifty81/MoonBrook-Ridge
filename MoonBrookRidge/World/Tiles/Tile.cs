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
    private string _animationName;
    private int _spriteId = -1; // Sprite ID from tileset (-1 means use default for tile type)
    
    public Tile(TileType type, Vector2 gridPosition)
    {
        _type = type;
        _gridPosition = gridPosition;
        _isWatered = false;
        _crop = null;
        _animationName = string.Empty;
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
            // Legacy tiles
            TileType.Grass or TileType.Grass01 or TileType.Grass02 or TileType.Grass03 => new Color(34, 139, 34),
            TileType.Dirt or TileType.Dirt01 or TileType.Dirt02 => new Color(139, 90, 43),
            TileType.Tilled or TileType.TilledDry => new Color(101, 67, 33),
            TileType.TilledWatered => new Color(70, 50, 30),
            TileType.Stone or TileType.Stone01 or TileType.Rock => Color.Gray,
            TileType.Water or TileType.Water01 => Color.Blue,
            TileType.Sand or TileType.Sand01 => Color.SandyBrown,
            TileType.WoodenFloor or TileType.Flooring => new Color(139, 90, 43),
            TileType.Wall => new Color(100, 100, 100), // Gray wall
            
            // Slates grass variants
            TileType.SlatesGrassBasic or TileType.SlatesGrassMedium => new Color(34, 139, 34),
            TileType.SlatesGrassLight => new Color(80, 170, 80),
            TileType.SlatesGrassDark => new Color(20, 100, 20),
            TileType.SlatesGrassFlowers => new Color(50, 150, 50),
            
            // Slates dirt variants
            TileType.SlatesDirtBasic or TileType.SlatesDirtPath => new Color(139, 90, 43),
            TileType.SlatesDirtTilled => new Color(101, 67, 33),
            
            // Slates stone variants
            TileType.SlatesStoneFloor or TileType.SlatesStoneCobble => Color.Gray,
            TileType.SlatesStoneWall => new Color(100, 100, 100),
            TileType.SlatesStoneBrick => new Color(120, 100, 90),
            
            // Slates water variants
            TileType.SlatesWaterStill or TileType.SlatesWaterShallow => Color.Blue,
            TileType.SlatesWaterDeep => new Color(0, 50, 150),
            TileType.SlatesWaterAnimated => new Color(30, 100, 200),
            
            // Slates sand variants
            TileType.SlatesSandBasic or TileType.SlatesSandLight => Color.SandyBrown,
            TileType.SlatesSandStones => new Color(200, 180, 140),
            
            // Slates indoor variants
            TileType.SlatesIndoorWood => new Color(139, 90, 43),
            TileType.SlatesIndoorStone => Color.LightGray,
            TileType.SlatesIndoorTile => new Color(200, 200, 180),
            
            // Slates special terrain
            TileType.SlatesSnow => Color.White,
            TileType.SlatesIce => new Color(200, 230, 255),
            
            // Mine entrance
            TileType.MineEntrance => new Color(50, 50, 50),
            
            // Dungeon entrances (distinct colors for each type)
            TileType.DungeonEntranceSlime => new Color(100, 200, 100),      // Green
            TileType.DungeonEntranceSkeleton => new Color(200, 200, 200),   // Light gray/white
            TileType.DungeonEntranceSpider => new Color(100, 50, 100),      // Purple
            TileType.DungeonEntranceGoblin => new Color(150, 100, 50),      // Brown
            TileType.DungeonEntranceHaunted => new Color(80, 80, 120),      // Dark blue-gray
            TileType.DungeonEntranceDragon => new Color(200, 50, 50),       // Red
            TileType.DungeonEntranceDemon => new Color(150, 0, 0),          // Dark red
            TileType.DungeonEntranceRuins => new Color(150, 150, 100),      // Sandy gray
            
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
    
    /// <summary>
    /// Animation name for animated tiles (e.g., "water_1", "flame_1")
    /// Empty string means no animation.
    /// </summary>
    public string AnimationName
    {
        get => _animationName;
        set => _animationName = value ?? string.Empty;
    }
    
    /// <summary>
    /// Sprite ID from the tileset. -1 means use default for tile type.
    /// Set by autotiling or animation systems.
    /// </summary>
    public int SpriteId
    {
        get => _spriteId;
        set => _spriteId = value;
    }
    
    /// <summary>
    /// Check if this tile has a sprite ID set
    /// </summary>
    public bool HasSpriteId()
    {
        return _spriteId >= 0;
    }
    
    /// <summary>
    /// Get sprite ID or return -1 if not set
    /// </summary>
    public int GetSpriteId()
    {
        return _spriteId;
    }
    
    /// <summary>
    /// Check if this tile blocks movement
    /// </summary>
    public bool IsBlocking()
    {
        return _type == TileType.Rock || 
               _type == TileType.Stone || 
               _type == TileType.Wall ||
               _type == TileType.SlatesStoneWall ||
               _type == TileType.MineEntrance;
    }
    
    /// <summary>
    /// Set blocking state (for dynamic tiles)
    /// </summary>
    public void SetBlocking(bool blocking)
    {
        // Can be extended to support dynamic blocking
        // For now, just update type if needed
    }
}

public enum TileType
{
    // Legacy tile types (kept for compatibility)
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
    Flooring,
    Wall, // Interior wall
    
    // Slates tileset types - Grass variants
    SlatesGrassBasic,
    SlatesGrassLight,
    SlatesGrassMedium,
    SlatesGrassDark,
    SlatesGrassFlowers,
    
    // Slates tileset types - Dirt variants
    SlatesDirtBasic,
    SlatesDirtPath,
    SlatesDirtTilled,
    
    // Slates tileset types - Stone variants
    SlatesStoneFloor,
    SlatesStoneWall,
    SlatesStoneCobble,
    SlatesStoneBrick,
    
    // Slates tileset types - Water variants
    SlatesWaterStill,
    SlatesWaterAnimated,
    SlatesWaterDeep,
    SlatesWaterShallow,
    
    // Slates tileset types - Sand variants
    SlatesSandBasic,
    SlatesSandLight,
    SlatesSandStones,
    
    // Slates tileset types - Indoor variants
    SlatesIndoorWood,
    SlatesIndoorStone,
    SlatesIndoorTile,
    
    // Slates tileset types - Special terrain
    SlatesSnow,
    SlatesIce,
    
    // Mine/Cave entrance
    MineEntrance,
    
    // Dungeon entrances (8 types)
    DungeonEntranceSlime,
    DungeonEntranceSkeleton,
    DungeonEntranceSpider,
    DungeonEntranceGoblin,
    DungeonEntranceHaunted,
    DungeonEntranceDragon,
    DungeonEntranceDemon,
    DungeonEntranceRuins
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
