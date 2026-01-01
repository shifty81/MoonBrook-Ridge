using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.Core.Systems;
using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.World.Mining;

/// <summary>
/// Represents a single level of the mine
/// </summary>
public class MineLevel
{
    public int Level { get; private set; }
    public Tile[,] Tiles { get; private set; }
    public List<MineableRock> Rocks { get; private set; }
    public Vector2 EntrancePosition { get; private set; }
    public Vector2 ExitPosition { get; private set; }
    
    private int _width;
    private int _height;
    
    public MineLevel(int level, int width, int height, Texture2D rockTexture, List<SpriteInfo> rockSprites)
    {
        Level = level;
        _width = width;
        _height = height;
        Rocks = new List<MineableRock>();
        
        // Generate the mine level
        MineGenerator generator = new MineGenerator(level * 1000); // Seed based on level
        Tiles = generator.GenerateMineLevel(level, width, height);
        
        // Set entrance and exit positions
        EntrancePosition = new Vector2(width / 2, 2);
        ExitPosition = new Vector2(width / 2, height - 5);
        
        // Spawn mineable rocks
        int numRocks = 20 + (level * 5); // More rocks on deeper levels
        List<Vector2> rockPositions = generator.GetRockSpawnPositions(Tiles, numRocks);
        
        foreach (var pos in rockPositions)
        {
            MineableRock rock;
            
            if (rockSprites != null && rockSprites.Count > 0)
            {
                // Use extracted rock sprites
                var randomSprite = rockSprites[new System.Random().Next(rockSprites.Count)];
                rock = new MineableRock($"Rock_{pos.X}_{pos.Y}", pos * 16, randomSprite, level);
            }
            else
            {
                // Fallback to full texture
                rock = new MineableRock($"Rock_{pos.X}_{pos.Y}", pos * 16, rockTexture, level);
            }
            
            Rocks.Add(rock);
        }
    }
    
    /// <summary>
    /// Try to mine a rock at the given position
    /// </summary>
    public bool TryMineRock(Vector2 position, out Item[] drops)
    {
        drops = null;
        
        // Find rock at or near position
        var rock = Rocks.FirstOrDefault(r => 
            Vector2.Distance(r.Position, position) < 32); // Within 32 pixels
        
        if (rock == null)
            return false;
        
        // Hit the rock
        bool destroyed = rock.Hit(out drops);
        
        if (destroyed)
        {
            // Remove the rock from the list
            Rocks.Remove(rock);
        }
        
        return true;
    }
    
    /// <summary>
    /// Draw the mine level
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Texture2D grassTexture, Texture2D stoneTexture)
    {
        // Draw tiles
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Vector2 position = new Vector2(x * 16, y * 16);
                Texture2D texture = Tiles[x, y].Type == TileType.Stone ? stoneTexture : grassTexture;
                
                if (texture != null)
                {
                    spriteBatch.Draw(
                        texture,
                        position,
                        null,
                        Color.DarkGray, // Darker in mines
                        0f,
                        Vector2.Zero,
                        1f,
                        SpriteEffects.None,
                        0f
                    );
                }
            }
        }
        
        // Draw rocks
        foreach (var rock in Rocks)
        {
            rock.Draw(spriteBatch);
        }
    }
}
