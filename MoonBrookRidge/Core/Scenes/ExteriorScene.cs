using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.Core.Scenes;

/// <summary>
/// Exterior scene (farm, villages, overworlds)
/// Larger outdoor areas with different biomes
/// </summary>
public class ExteriorScene : Scene
{
    protected Texture2D _sunnysideTileset;
    protected SunnysideTilesetHelper _tilesetHelper;
    
    public ExteriorScene(string name, string sceneId, int width, int height) 
        : base(name, sceneId, SceneType.Exterior, width, height)
    {
    }
    
    public override void Initialize()
    {
        // Initialize with grass tiles by default
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                SetTile(x, y, new Tile(TileType.Grass, new Vector2(x, y)));
            }
        }
    }
    
    public override void LoadContent()
    {
        // Load textures - will be set by GameplayState
        _sunnysideTileset = null; // AssetManager.GetTexture("Textures/Tilesets/sunnyside_tileset");
        _tilesetHelper = null; // new SunnysideTilesetHelper(_sunnysideTileset);
    }
    
    public override void Draw(SpriteBatch spriteBatch, Camera2D camera)
    {
        // Draw terrain tiles with frustum culling
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var tile = GetTile(x, y);
                if (tile != null)
                {
                    Vector2 worldPosition = new Vector2(x * GameConstants.TILE_SIZE, y * GameConstants.TILE_SIZE);
                    
                    // Frustum culling - only draw visible tiles
                    if (camera.IsInView(worldPosition, GameConstants.TILE_SIZE, GameConstants.TILE_SIZE))
                    {
                        DrawTile(spriteBatch, tile, worldPosition);
                    }
                }
            }
        }
        
        // Draw scene objects
        foreach (var obj in _objects)
        {
            if (camera.IsInView(obj.Position, GameConstants.TILE_SIZE * 2, GameConstants.TILE_SIZE * 2))
            {
                obj.Draw(spriteBatch);
            }
        }
    }
    
    protected virtual void DrawTile(SpriteBatch spriteBatch, Tile tile, Vector2 position)
    {
        if (_tilesetHelper != null && tile.HasSpriteId())
        {
            _tilesetHelper.DrawTile(spriteBatch, tile.GetSpriteId(), position);
        }
        else if (_sunnysideTileset != null)
        {
            // Fallback to colored tile
            Color tileColor = tile.GetColor();
            Rectangle destRect = new Rectangle(
                (int)position.X,
                (int)position.Y,
                GameConstants.TILE_SIZE,
                GameConstants.TILE_SIZE
            );
            spriteBatch.Draw(_sunnysideTileset, destRect, new Rectangle(0, 0, 16, 16), tileColor * 0.8f);
        }
    }
}
