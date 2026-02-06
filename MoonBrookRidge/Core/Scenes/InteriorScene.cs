using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.Core.Scenes;

/// <summary>
/// Interior scene (farmhouse, shops, houses, buildings)
/// All interiors are separate scenes with their own tile grids
/// </summary>
public class InteriorScene : Scene
{
    protected Texture2D _roomBuilderAtlas;
    protected Texture2D _interiorsAtlas;
    protected Texture2D _sunnysideTileset;
    protected AssetManager _assetManager;
    
    public InteriorScene(string name, string sceneId, int width, int height) 
        : base(name, sceneId, SceneType.Interior, width, height)
    {
    }
    
    public override void Initialize()
    {
        // Initialize floor with wood floor tiles by default
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                SetTile(x, y, new Tile(TileType.WoodenFloor, new Vector2(x, y)));
            }
        }
        
        // Create walls around perimeter
        CreateWalls();
    }
    
    protected virtual void CreateWalls()
    {
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
    }
    
    public override void LoadContent()
    {
        // Load interior textures - will be set by GameplayState
        // For now, textures will be null and we'll use colored rectangles
        _roomBuilderAtlas = null; // AssetManager.GetTexture("Textures/Interiors/ModernInteriors/Tilesets/Room_Builder_free_16x16");
        _interiorsAtlas = null; // AssetManager.GetTexture("Textures/Interiors/ModernInteriors/Tilesets/Interiors_free_16x16");
        _sunnysideTileset = null; // AssetManager.GetTexture("Textures/Tilesets/sunnyside_tileset");
    }
    
    public override void Draw(SpriteBatch spriteBatch, Camera2D camera)
    {
        // Draw floor tiles
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var tile = GetTile(x, y);
                if (tile != null)
                {
                    Vector2 worldPosition = new Vector2(x * GameConstants.TILE_SIZE, y * GameConstants.TILE_SIZE);
                    
                    // Only draw if in camera view (basic frustum culling)
                    if (camera.IsInView(worldPosition, GameConstants.TILE_SIZE, GameConstants.TILE_SIZE))
                    {
                        DrawTile(spriteBatch, tile, worldPosition);
                    }
                }
            }
        }
        
        // Draw objects
        foreach (var obj in _objects)
        {
            if (camera.IsInView(obj.Position, GameConstants.TILE_SIZE, GameConstants.TILE_SIZE))
            {
                obj.Draw(spriteBatch);
            }
        }
    }
    
    protected virtual void DrawTile(SpriteBatch spriteBatch, Tile tile, Vector2 position)
    {
        Color tileColor = tile.GetColor();
        
        Rectangle destRect = new Rectangle(
            (int)position.X,
            (int)position.Y,
            GameConstants.TILE_SIZE,
            GameConstants.TILE_SIZE
        );
        
        // Try to use texture atlas if available
        if (_roomBuilderAtlas != null)
        {
            // Map tile types to atlas coordinates (16x16 tiles in atlas)
            Rectangle sourceRect = GetInteriorTileSource(tile.Type);
            spriteBatch.Draw(_roomBuilderAtlas, destRect, sourceRect, Color.White);
        }
        else if (_sunnysideTileset != null)
        {
            // Fallback to sunnyside tileset for basic floor/wall tiles
            Rectangle sourceRect = GetSunnysideTileSource(tile.Type);
            spriteBatch.Draw(_sunnysideTileset, destRect, sourceRect, Color.White);
        }
        // Note: When no texture is available, tile won't be drawn
        // SpriteBatch.Draw with null texture is not supported in this engine
    }
    
    /// <summary>
    /// Get source rectangle from interior atlas for tile type
    /// </summary>
    private Rectangle GetInteriorTileSource(TileType type)
    {
        // Map tile types to atlas positions (assuming 16x16 tiles)
        // These are example mappings - adjust based on actual atlas layout
        return type switch
        {
            TileType.WoodenFloor => new Rectangle(0, 0, 16, 16),
            TileType.Wall => new Rectangle(16, 0, 16, 16),
            TileType.SlatesStoneFloor => new Rectangle(32, 0, 16, 16),
            _ => new Rectangle(0, 0, 16, 16) // Default floor
        };
    }
    
    /// <summary>
    /// Get source rectangle from sunnyside tileset for basic tiles
    /// </summary>
    private Rectangle GetSunnysideTileSource(TileType type)
    {
        // Map to basic sunnyside tiles as fallback
        return type switch
        {
            TileType.WoodenFloor => new Rectangle(0, 128, 16, 16),  // Wood texture
            TileType.Wall => new Rectangle(0, 16, 16, 16),          // Stone wall
            TileType.SlatesStoneFloor => new Rectangle(0, 32, 16, 16),    // Stone floor
            _ => new Rectangle(0, 0, 16, 16)                         // Grass as default
        };
    }
}
