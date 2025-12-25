using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonBrookRidge.World.Tiles;

/// <summary>
/// Helper class for working with the Slates 32x32px tileset by Ivan Voirol
/// License: CC-BY 4.0 - Attribution Required
/// </summary>
public class SlatesTilesetHelper
{
    private Texture2D _tilesetTexture;
    private const int TILE_SIZE = 32;
    private const int COLUMNS = 56;
    private const int ROWS = 23;
    private const int TOTAL_TILES = 1288;

    public SlatesTilesetHelper(Texture2D tilesetTexture)
    {
        _tilesetTexture = tilesetTexture;
    }

    /// <summary>
    /// Gets the source rectangle for a specific tile ID in the tileset
    /// </summary>
    /// <param name="tileId">Tile ID (0-1287)</param>
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
    /// Draws a tile from the Slates tileset
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch to draw with</param>
    /// <param name="tileId">Tile ID (0-1287)</param>
    /// <param name="position">Screen position to draw at</param>
    /// <param name="scale">Scale factor (1.0 = 32x32 pixels, 0.5 = 16x16 pixels)</param>
    public void DrawTile(SpriteBatch spriteBatch, int tileId, Vector2 position, float scale = 1.0f)
    {
        Rectangle sourceRect = GetTileSourceRectangle(tileId);
        
        spriteBatch.Draw(
            _tilesetTexture,
            position,
            sourceRect,
            Color.White,
            0f,
            Vector2.Zero,
            scale,
            SpriteEffects.None,
            0f
        );
    }

    /// <summary>
    /// Draws a tile from the Slates tileset to a specific destination rectangle
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch to draw with</param>
    /// <param name="tileId">Tile ID (0-1287)</param>
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

    /// <summary>
    /// Extracts a single tile from the tileset as a new texture
    /// Useful for converting 32x32 tiles to the game's 16x16 format
    /// </summary>
    /// <param name="graphicsDevice">GraphicsDevice to use</param>
    /// <param name="tileId">Tile ID to extract</param>
    /// <param name="targetSize">Size to scale the tile to (default: 32)</param>
    /// <returns>New texture containing the extracted tile</returns>
    public Texture2D ExtractTile(GraphicsDevice graphicsDevice, int tileId, int targetSize = 32)
    {
        Rectangle sourceRect = GetTileSourceRectangle(tileId);
        
        // Create a render target for the extracted tile
        RenderTarget2D renderTarget = new RenderTarget2D(
            graphicsDevice,
            targetSize,
            targetSize,
            false,
            SurfaceFormat.Color,
            DepthFormat.None
        );

        // Render the tile to the target
        graphicsDevice.SetRenderTarget(renderTarget);
        graphicsDevice.Clear(Color.Transparent);

        SpriteBatch batch = new SpriteBatch(graphicsDevice);
        batch.Begin(samplerState: SamplerState.PointClamp);
        batch.Draw(
            _tilesetTexture,
            new Rectangle(0, 0, targetSize, targetSize),
            sourceRect,
            Color.White
        );
        batch.End();

        graphicsDevice.SetRenderTarget(null);

        return renderTarget;
    }

    /// <summary>
    /// Gets tile information for debugging
    /// </summary>
    public string GetTileInfo(int tileId)
    {
        if (tileId < 0 || tileId >= TOTAL_TILES)
        {
            return "Invalid tile ID";
        }

        int column = tileId % COLUMNS;
        int row = tileId / COLUMNS;
        Rectangle sourceRect = GetTileSourceRectangle(tileId);

        return $"Tile ID: {tileId}\n" +
               $"Position: Col {column}, Row {row}\n" +
               $"Source Rect: {sourceRect.X}, {sourceRect.Y}, {sourceRect.Width}, {sourceRect.Height}";
    }

    // Tileset properties
    public int TileSize => TILE_SIZE;
    public int Columns => COLUMNS;
    public int Rows => ROWS;
    public int TotalTiles => TOTAL_TILES;
    public Texture2D Texture => _tilesetTexture;
}
