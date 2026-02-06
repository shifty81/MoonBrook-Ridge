using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Utility class for extracting individual sprites from sprite sheets/tilesheets
/// </summary>
public static class SpriteSheetExtractor
{
    /// <summary>
    /// Extract individual sprites from a horizontal strip sprite sheet
    /// </summary>
    /// <param name="spriteSheet">The source sprite sheet texture</param>
    /// <param name="spriteWidth">Width of each individual sprite</param>
    /// <param name="spriteHeight">Height of each individual sprite (uses full height if not specified)</param>
    /// <returns>Dictionary of sprite textures with index keys</returns>
    public static Dictionary<string, SpriteInfo> ExtractSpritesFromHorizontalStrip(
        Texture2D spriteSheet,
        int spriteWidth,
        int? spriteHeight = null)
    {
        if (spriteSheet == null)
            throw new ArgumentNullException(nameof(spriteSheet));
            
        int height = spriteHeight ?? spriteSheet.Height;
        int count = spriteSheet.Width / spriteWidth;
        
        var sprites = new Dictionary<string, SpriteInfo>();
        
        for (int i = 0; i < count; i++)
        {
            var sourceRect = new Rectangle(i * spriteWidth, 0, spriteWidth, height);
            sprites[$"{i}"] = new SpriteInfo
            {
                Texture = spriteSheet,
                SourceRectangle = sourceRect
            };
        }
        
        return sprites;
    }
    
    /// <summary>
    /// Extract individual sprites from a grid-based sprite sheet
    /// </summary>
    /// <param name="spriteSheet">The source sprite sheet texture</param>
    /// <param name="spriteWidth">Width of each individual sprite</param>
    /// <param name="spriteHeight">Height of each individual sprite</param>
    /// <returns>Dictionary of sprite textures with "row_col" keys</returns>
    public static Dictionary<string, SpriteInfo> ExtractSpritesFromGrid(
        Texture2D spriteSheet,
        int spriteWidth,
        int spriteHeight)
    {
        if (spriteSheet == null)
            throw new ArgumentNullException(nameof(spriteSheet));
            
        int columns = spriteSheet.Width / spriteWidth;
        int rows = spriteSheet.Height / spriteHeight;
        
        var sprites = new Dictionary<string, SpriteInfo>();
        
        for (int row = 0; row < rows; row++)
        {
            for (int col = 0; col < columns; col++)
            {
                var sourceRect = new Rectangle(col * spriteWidth, row * spriteHeight, spriteWidth, spriteHeight);
                sprites[$"{row}_{col}"] = new SpriteInfo
                {
                    Texture = spriteSheet,
                    SourceRectangle = sourceRect
                };
            }
        }
        
        return sprites;
    }
    
    /// <summary>
    /// Extract sprites with a custom name prefix
    /// </summary>
    public static Dictionary<string, SpriteInfo> ExtractSpritesFromHorizontalStrip(
        Texture2D spriteSheet,
        string namePrefix,
        int spriteWidth,
        int? spriteHeight = null)
    {
        var sprites = ExtractSpritesFromHorizontalStrip(spriteSheet, spriteWidth, spriteHeight);
        var namedSprites = new Dictionary<string, SpriteInfo>();
        
        foreach (var kvp in sprites)
        {
            namedSprites[$"{namePrefix}{kvp.Key}"] = kvp.Value;
        }
        
        return namedSprites;
    }
}

/// <summary>
/// Contains information about an extracted sprite
/// </summary>
public class SpriteInfo
{
    public Texture2D Texture { get; set; }
    public Rectangle SourceRectangle { get; set; }
}
