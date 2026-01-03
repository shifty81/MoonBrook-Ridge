using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
                SetTile(x, y, new Tile(TileType.WoodFloor, new Vector2(x, y)));
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
        // Load interior textures
        _roomBuilderAtlas = AssetManager.GetTexture("Textures/Interiors/ModernInteriors/Tilesets/Room_Builder_free_16x16");
        _interiorsAtlas = AssetManager.GetTexture("Textures/Interiors/ModernInteriors/Tilesets/Interiors_free_16x16");
        _sunnysideTileset = AssetManager.GetTexture("Textures/Tilesets/sunnyside_tileset");
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
        // Simple colored rectangle for now
        // TODO: Use actual tile textures from atlas
        Color tileColor = tile.GetColor();
        
        Rectangle destRect = new Rectangle(
            (int)position.X,
            (int)position.Y,
            GameConstants.TILE_SIZE,
            GameConstants.TILE_SIZE
        );
        
        // Draw simple colored tile (will be replaced with texture atlas)
        if (_roomBuilderAtlas != null)
        {
            // For now, draw solid color. TODO: Map to actual atlas coordinates
            spriteBatch.Draw(_roomBuilderAtlas, destRect, new Rectangle(0, 0, 16, 16), tileColor * 0.8f);
        }
    }
}
