using System;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Maps;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.Characters.Player;

namespace MoonBrookRidge.UI;

/// <summary>
/// Displays a minimap showing the player's surroundings
/// </summary>
public class Minimap
{
    private readonly int _minimapSize = 150; // Size in pixels
    private readonly int _viewRadius = 10; // How many tiles to show around player
    private Texture2D? _minimapTexture;
    private Texture2D? _pixelTexture;
    private bool _isVisible;
    private Vector2 _position;
    
    public bool IsVisible
    {
        get => _isVisible;
        set => _isVisible = value;
    }
    
    public Minimap(int screenWidth, int screenHeight)
    {
        _isVisible = true;
        // Position minimap in top-right corner
        _position = new Vector2(screenWidth - _minimapSize - 10, 10);
    }
    
    /// <summary>
    /// Initialize the minimap texture
    /// </summary>
    public void Initialize(GraphicsDevice graphicsDevice)
    {
        _minimapTexture = new Texture2D(graphicsDevice, _minimapSize, _minimapSize);
        _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });
    }
    
    /// <summary>
    /// Update minimap texture based on current world state
    /// </summary>
    public void Update(WorldMap worldMap, PlayerCharacter player)
    {
        if (!_isVisible || _minimapTexture == null) return;
        
        var pixels = new Color[_minimapSize * _minimapSize];
        var playerTileX = (int)(player.Position.X / 16);
        var playerTileY = (int)(player.Position.Y / 16);
        
        var pixelsPerTile = _minimapSize / (_viewRadius * 2 + 1);
        
        // Fill with background color
        for (int i = 0; i < pixels.Length; i++)
        {
            pixels[i] = new Color(20, 20, 20, 200); // Dark semi-transparent background
        }
        
        // Draw tiles
        for (int dy = -_viewRadius; dy <= _viewRadius; dy++)
        {
            for (int dx = -_viewRadius; dx <= _viewRadius; dx++)
            {
                var tileX = playerTileX + dx;
                var tileY = playerTileY + dy;
                
                // Check if tile is within world bounds
                if (tileX < 0 || tileX >= worldMap.Width || tileY < 0 || tileY >= worldMap.Height)
                    continue;
                
                var tile = worldMap.GetTile(tileX, tileY);
                var tileColor = GetTileColor(tile?.Type ?? TileType.Grass);
                
                // Calculate pixel position on minimap
                var pixelX = (dx + _viewRadius) * pixelsPerTile;
                var pixelY = (dy + _viewRadius) * pixelsPerTile;
                
                // Draw tile as a block of pixels
                for (int py = 0; py < pixelsPerTile; py++)
                {
                    for (int px = 0; px < pixelsPerTile; px++)
                    {
                        var index = (pixelY + py) * _minimapSize + (pixelX + px);
                        if (index >= 0 && index < pixels.Length)
                        {
                            pixels[index] = tileColor;
                        }
                    }
                }
            }
        }
        
        // Draw player position (center of minimap)
        var centerX = _minimapSize / 2;
        var centerY = _minimapSize / 2;
        var playerSize = 3; // Size of player marker
        
        for (int py = -playerSize; py <= playerSize; py++)
        {
            for (int px = -playerSize; px <= playerSize; px++)
            {
                var index = (centerY + py) * _minimapSize + (centerX + px);
                if (index >= 0 && index < pixels.Length)
                {
                    pixels[index] = Color.Yellow; // Player marker
                }
            }
        }
        
        _minimapTexture.SetData(pixels);
    }
    
    /// <summary>
    /// Get color representation for a tile type
    /// </summary>
    private Color GetTileColor(TileType tileType)
    {
        return tileType switch
        {
            TileType.Grass or TileType.Grass01 or TileType.Grass02 or TileType.Grass03 => new Color(34, 139, 34),
            TileType.Dirt or TileType.Dirt01 or TileType.Dirt02 => new Color(139, 69, 19),
            TileType.Tilled or TileType.TilledDry or TileType.TilledWatered => new Color(101, 67, 33),
            TileType.Stone or TileType.Stone01 or TileType.Rock => new Color(128, 128, 128),
            TileType.Water or TileType.Water01 => new Color(65, 105, 225),
            TileType.Sand or TileType.Sand01 => new Color(238, 214, 175),
            TileType.WoodenFloor or TileType.Flooring => new Color(160, 82, 45),
            _ => new Color(50, 50, 50) // Default dark gray
        };
    }
    
    /// <summary>
    /// Draw the minimap
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, SpriteFont font)
    {
        if (!_isVisible || _minimapTexture == null) return;
        
        // Draw border
        var borderRect = new Rectangle(
            (int)_position.X - 2,
            (int)_position.Y - 2,
            _minimapSize + 4,
            _minimapSize + 4
        );
        DrawRectangle(spriteBatch, borderRect, Color.White);
        
        // Draw minimap
        spriteBatch.Draw(_minimapTexture, _position, Color.White);
        
        // Draw label
        var labelPos = new Vector2(_position.X, _position.Y + _minimapSize + 5);
        spriteBatch.DrawString(font, "Map (Tab to toggle)", labelPos, Color.White);
    }
    
    /// <summary>
    /// Helper to draw a filled rectangle
    /// </summary>
    private void DrawRectangle(SpriteBatch spriteBatch, Rectangle rect, Color color)
    {
        if (_pixelTexture != null)
        {
            spriteBatch.Draw(_pixelTexture, rect, color);
        }
    }
    
    /// <summary>
    /// Update minimap position when screen size changes
    /// </summary>
    public void UpdatePosition(int screenWidth, int screenHeight)
    {
        _position = new Vector2(screenWidth - _minimapSize - 10, 10);
    }
}
