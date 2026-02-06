using System;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.World.Tiles;

/// <summary>
/// Helper class for working with the Sunnyside World 16x16px tileset
/// License: Asset pack specific - check sprites directory for details
/// </summary>
public class SunnysideTilesetHelper
{
    private Texture2D _tilesetTexture;
    private const int TILE_SIZE = 16;
    private const int COLUMNS = 64;
    private const int ROWS = 64;
    private const int TOTAL_TILES = 4096;

    public SunnysideTilesetHelper(Texture2D tilesetTexture)
    {
        _tilesetTexture = tilesetTexture;
    }

    /// <summary>
    /// Gets the source rectangle for a specific tile ID in the tileset
    /// </summary>
    /// <param name="tileId">Tile ID (0-4095)</param>
    /// <returns>Source rectangle for the tile</returns>
    public Rectangle GetTileSourceRectangle(int tileId)
    {
        if (tileId < 0 || tileId >= TOTAL_TILES)
        {
            throw new ArgumentOutOfRangeException(nameof(tileId), 
                $"Tile ID must be between 0 and {TOTAL_TILES - 1}");
        }

        int x = (tileId % COLUMNS) * TILE_SIZE;
        int y = (tileId / COLUMNS) * TILE_SIZE;
        
        return new Rectangle(x, y, TILE_SIZE, TILE_SIZE);
    }

    /// <summary>
    /// Draws a tile from the Sunnyside tileset
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch to draw with</param>
    /// <param name="tileId">Tile ID (0-4095)</param>
    /// <param name="position">Screen position to draw at</param>
    public void DrawTile(SpriteBatch spriteBatch, int tileId, Vector2 position)
    {
        Rectangle sourceRect = GetTileSourceRectangle(tileId);
        
        spriteBatch.Draw(
            _tilesetTexture,
            position,
            sourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            1.0f,
            SpriteEffects.None,
            0f
        );
    }

    /// <summary>
    /// Draws a tile from the Sunnyside tileset to a specific destination rectangle
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch to draw with</param>
    /// <param name="tileId">Tile ID (0-4095)</param>
    /// <param name="destinationRect">Destination rectangle</param>
    public void DrawTile(SpriteBatch spriteBatch, int tileId, Rectangle destinationRect)
    {
        Rectangle sourceRect = GetTileSourceRectangle(tileId);
        
        spriteBatch.Draw(
            _tilesetTexture,
            destinationRect,
            sourceRect,
            Color.White
        );
    }

    // Tileset properties
    public int TileSize => TILE_SIZE;
    public int Columns => COLUMNS;
    public int Rows => ROWS;
    public int TotalTiles => TOTAL_TILES;
    public Texture2D Texture => _tilesetTexture;
}
