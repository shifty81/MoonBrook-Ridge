using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.World.Tiles;

namespace MoonBrookRidge.World.Maps;

/// <summary>
/// Tile-based world map system
/// </summary>
public class WorldMap
{
    private Tile[,] _tiles;
    private int _width;
    private int _height;
    private const int TILE_SIZE = 16;
    private Texture2D _grassTexture;
    private Texture2D _plainsTexture;
    
    public WorldMap()
    {
        _width = 50;
        _height = 50;
        _tiles = new Tile[_width, _height];
        
        InitializeMap();
    }
    
    private void InitializeMap()
    {
        // Create a simple grass map for now
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _tiles[x, y] = new Tile(TileType.Grass, new Vector2(x, y));
            }
        }
    }
    
    public void LoadContent(Texture2D grassTexture, Texture2D plainsTexture)
    {
        _grassTexture = grassTexture;
        _plainsTexture = plainsTexture;
    }
    
    public void Update(GameTime gameTime)
    {
        // Update tiles (crop growth, etc.)
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                _tiles[x, y].Update(gameTime);
            }
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        // Use actual textures if available, otherwise fall back to colored squares
        if (_grassTexture != null)
        {
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Rectangle tileRect = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    Texture2D texture = GetTileTexture(_tiles[x, y].Type);
                    
                    // Draw a single tile from the texture
                    spriteBatch.Draw(texture, tileRect, new Rectangle(0, 0, 16, 16), Color.White);
                }
            }
        }
        else
        {
            // Fallback: colored squares
            Texture2D pixel = CreatePixelTexture(spriteBatch.GraphicsDevice);
            
            for (int x = 0; x < _width; x++)
            {
                for (int y = 0; y < _height; y++)
                {
                    Rectangle tileRect = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                    Color tileColor = _tiles[x, y].GetColor();
                    spriteBatch.Draw(pixel, tileRect, tileColor);
                }
            }
        }
    }
    
    private Texture2D GetTileTexture(TileType type)
    {
        // Select texture based on tile type
        return type switch
        {
            TileType.Grass => _grassTexture,
            TileType.Dirt => _plainsTexture ?? _grassTexture,
            TileType.Tilled => _plainsTexture ?? _grassTexture,
            TileType.Stone => _plainsTexture ?? _grassTexture,
            TileType.Water => _plainsTexture ?? _grassTexture,
            TileType.Sand => _plainsTexture ?? _grassTexture,
            _ => _grassTexture
        };
    }
    
    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            return _tiles[x, y];
        }
        return null;
    }
    
    private Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
    {
        Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
    
    public int Width => _width;
    public int Height => _height;
}
