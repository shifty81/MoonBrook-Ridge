using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.World.Tiles;

namespace MoonBrookRidge.World.Interiors;

/// <summary>
/// The player's farmhouse interior with bedroom, kitchen, and living area
/// </summary>
public class FarmhouseInterior : InteriorScene
{
    private Texture2D _roomBuilderAtlas;
    private Texture2D _interiorsAtlas;
    
    public FarmhouseInterior() : base("Farmhouse", 15, 12)
    {
        // Player spawns near the bed
        _playerSpawnPosition = new Vector2(7, 5);
        
        // Door is at the bottom center
        _doorPosition = new Vector2(7, 11);
        
        InitializeRoom();
    }
    
    /// <summary>
    /// Load textures for the farmhouse
    /// </summary>
    public void LoadContent(Texture2D roomBuilder, Texture2D interiors)
    {
        _roomBuilderAtlas = roomBuilder;
        _interiorsAtlas = interiors;
    }
    
    /// <summary>
    /// Initialize the farmhouse room layout
    /// </summary>
    private void InitializeRoom()
    {
        // Fill with wood floor tiles (using simple floor tile for now)
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // Create a simple floor tile
                SetTile(x, y, new Tile(TileType.WoodFloor, new Vector2(x, y)));
            }
        }
        
        // Add walls around the perimeter (marked as blocking but we'll draw them separately)
        for (int x = 0; x < _width; x++)
        {
            SetTile(x, 0, new Tile(TileType.Wall, new Vector2(x, 0)));
            SetTile(x, _height - 1, new Tile(TileType.Wall, new Vector2(x, _height - 1)));
        }
        
        for (int y = 0; y < _height; y++)
        {
            SetTile(0, y, new Tile(TileType.Wall, new Vector2(0, y)));
            SetTile(_width - 1, y, new Tile(TileType.Wall, new Vector2(_width - 1, y)));
        }
        
        // Clear door position (no wall at door)
        SetTile((int)_doorPosition.X, (int)_doorPosition.Y, new Tile(TileType.WoodFloor, _doorPosition));
        
        AddFurniture();
    }
    
    /// <summary>
    /// Add furniture and decorations to the farmhouse
    /// </summary>
    private void AddFurniture()
    {
        // Bed (player spawns near this) - top right area
        // Using tile coordinates from Interiors_free_16x16.png
        // Bed is typically in the first few rows
        AddObject(new InteriorObject(
            "Bed",
            new Vector2(10 * GameConstants.TILE_SIZE, 3 * GameConstants.TILE_SIZE),
            new Rectangle(0, 32, 32, 32), // 2x2 bed sprite
            isBlocking: true
        ));
        
        // Nightstand next to bed
        AddObject(new InteriorObject(
            "Nightstand",
            new Vector2(9 * GameConstants.TILE_SIZE, 3 * GameConstants.TILE_SIZE),
            new Rectangle(32, 16, 16, 16),
            isBlocking: true
        ));
        
        // Table in center area
        AddObject(new InteriorObject(
            "Table",
            new Vector2(6 * GameConstants.TILE_SIZE, 6 * GameConstants.TILE_SIZE),
            new Rectangle(0, 48, 32, 16), // 2x1 table
            isBlocking: true
        ));
        
        // Chairs around table
        AddObject(new InteriorObject(
            "Chair",
            new Vector2(5 * GameConstants.TILE_SIZE, 6 * GameConstants.TILE_SIZE),
            new Rectangle(48, 48, 16, 16),
            isBlocking: true
        ));
        
        AddObject(new InteriorObject(
            "Chair",
            new Vector2(8 * GameConstants.TILE_SIZE, 6 * GameConstants.TILE_SIZE),
            new Rectangle(48, 48, 16, 16),
            isBlocking: true
        ));
        
        // Kitchen counter - left side
        AddObject(new InteriorObject(
            "Counter",
            new Vector2(2 * GameConstants.TILE_SIZE, 3 * GameConstants.TILE_SIZE),
            new Rectangle(0, 0, 16, 16),
            isBlocking: true
        ));
        
        // Stove
        AddObject(new InteriorObject(
            "Stove",
            new Vector2(3 * GameConstants.TILE_SIZE, 3 * GameConstants.TILE_SIZE),
            new Rectangle(16, 0, 16, 16),
            isBlocking: true
        ));
        
        // Plant decoration (non-blocking)
        AddObject(new InteriorObject(
            "Plant",
            new Vector2(2 * GameConstants.TILE_SIZE, 9 * GameConstants.TILE_SIZE),
            new Rectangle(64, 64, 16, 16),
            isBlocking: false
        ));
        
        // Rug near door (non-blocking)
        AddObject(new InteriorObject(
            "Rug",
            new Vector2(6 * GameConstants.TILE_SIZE, 9 * GameConstants.TILE_SIZE),
            new Rectangle(80, 80, 32, 16),
            isBlocking: false
        ));
    }
    
    /// <summary>
    /// Draw the farmhouse interior
    /// </summary>
    public override void Draw(SpriteBatch spriteBatch, Texture2D floorAtlas, Texture2D objectAtlas)
    {
        // Use the loaded atlases if available, otherwise fall back to provided textures
        Texture2D floor = _roomBuilderAtlas ?? floorAtlas;
        Texture2D objects = _interiorsAtlas ?? objectAtlas;
        
        base.Draw(spriteBatch, floor, objects);
    }
}
