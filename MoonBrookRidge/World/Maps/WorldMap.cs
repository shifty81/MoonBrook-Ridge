using System;
using System.Collections.Generic;
using System.Linq;
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
    
    // Tile textures
    private Dictionary<TileType, Texture2D> _tileTextures;
    
    // Sunnyside World tileset support
    private SunnysideTilesetHelper _sunnysideTileset;
    private Dictionary<TileType, int> _sunnysideTileMapping;
    
    // Crop textures by type and growth stage
    private Dictionary<string, Texture2D[]> _cropTextures;
    
    // World objects (buildings, trees, rocks, decorations)
    private List<WorldObject> _worldObjects;
    
    public WorldMap()
    {
        _width = 50;
        _height = 50;
        _tiles = new Tile[_width, _height];
        _cropTextures = new Dictionary<string, Texture2D[]>();
        _tileTextures = new Dictionary<TileType, Texture2D>();
        _sunnysideTileMapping = new Dictionary<TileType, int>();
        _worldObjects = new List<WorldObject>();
        
        InitializeMap();
    }
    
    private void InitializeMap()
    {
        // Create a varied map with legacy tiles
        Random random = new Random(42); // Fixed seed for consistency
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                // Create varied terrain
                TileType tileType;
                
                // Border areas - mix of grass types
                if (x < 5 || y < 5 || x >= _width - 5 || y >= _height - 5)
                {
                    int grassVariant = random.Next(4);
                    tileType = grassVariant switch
                    {
                        0 => TileType.Grass,
                        1 => TileType.Grass01,
                        2 => TileType.Grass02,
                        _ => TileType.Grass03
                    };
                }
                // Small dirt path area
                else if (x >= 10 && x < 15 && y >= 10 && y < 15)
                {
                    tileType = random.Next(2) == 0 ? TileType.Dirt : TileType.Dirt01;
                }
                // Stone area
                else if (x >= 35 && x < 40 && y >= 35 && y < 40)
                {
                    tileType = random.Next(2) == 0 ? TileType.Stone : TileType.Stone01;
                }
                // Water area (small pond)
                else if (x >= 15 && x < 20 && y >= 30 && y < 35)
                {
                    tileType = TileType.Water;
                }
                // Sand corner
                else if (x >= 40 && y >= 40)
                {
                    tileType = TileType.Sand;
                }
                // Default grass with variation
                else
                {
                    int variant = random.Next(10);
                    tileType = variant < 5 ? TileType.Grass : 
                              variant < 7 ? TileType.Grass01 :
                              variant < 8 ? TileType.Grass02 : TileType.Grass03;
                }
                
                _tiles[x, y] = new Tile(tileType, new Vector2(x, y));
            }
        }
    }
    
    public void LoadContent(Dictionary<TileType, Texture2D> tileTextures, Dictionary<string, Texture2D[]> cropTextures = null)
    {
        _tileTextures = tileTextures;
        
        // Load crop textures if provided
        if (cropTextures != null)
        {
            _cropTextures = cropTextures;
        }
    }
    
    /// <summary>
    /// Load and initialize the Sunnyside World tileset
    /// </summary>
    public void LoadSunnysideTileset(Texture2D sunnysideTilesetTexture)
    {
        _sunnysideTileset = new SunnysideTilesetHelper(sunnysideTilesetTexture);
        InitializeSunnysideTileMapping();
    }
    
    /// <summary>
    /// Initialize the mapping from TileType to Sunnyside tile IDs
    /// Uses random selection from available tiles for variety
    /// </summary>
    private void InitializeSunnysideTileMapping()
    {
        Random random = new Random(42); // Fixed seed for consistency
        
        // Map each TileType to a specific tile ID from the Sunnyside tileset
        _sunnysideTileMapping = new Dictionary<TileType, int>
        {
            // Grass variants
            [TileType.Grass] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Grass.Basic, random),
            [TileType.Grass01] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Grass.Light, random),
            [TileType.Grass02] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Grass.Medium, random),
            [TileType.Grass03] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Grass.Dark, random),
            
            // Dirt variants
            [TileType.Dirt] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Dirt.Basic, random),
            [TileType.Dirt01] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Dirt.Basic, random),
            [TileType.Dirt02] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Dirt.Path, random),
            [TileType.Tilled] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Dirt.Tilled, random),
            [TileType.TilledDry] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Dirt.Tilled, random),
            [TileType.TilledWatered] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Dirt.Tilled, random),
            
            // Stone variants
            [TileType.Stone] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Stone.Floor, random),
            [TileType.Stone01] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Stone.Wall, random),
            [TileType.Rock] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Stone.Wall, random),
            
            // Water variants
            [TileType.Water] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Water.Basic, random),
            [TileType.Water01] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Water.Shallow, random),
            
            // Sand variants
            [TileType.Sand] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Sand.Basic, random),
            [TileType.Sand01] = SunnysideTileMapping.GetRandomTile(SunnysideTileMapping.Sand.Light, random)
        };
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
    
    /// <summary>
    /// Updates all crops with elapsed game hours
    /// </summary>
    public void UpdateCropGrowth(float gameHoursElapsed)
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var crop = _tiles[x, y].Crop;
                if (crop != null)
                {
                    crop.UpdateGrowth(gameHoursElapsed);
                }
            }
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        // Use Sunnyside tileset if available, otherwise fall back to individual textures or colored squares
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                Tile tile = _tiles[x, y];
                Rectangle tileRect = new Rectangle(x * TILE_SIZE, y * TILE_SIZE, TILE_SIZE, TILE_SIZE);
                
                // Try to draw using Sunnyside tileset first
                if (_sunnysideTileset != null && _sunnysideTileMapping.TryGetValue(tile.Type, out int tileId))
                {
                    _sunnysideTileset.DrawTile(spriteBatch, tileId, tileRect);
                }
                // Fall back to individual textures
                else if (_tileTextures.Count > 0)
                {
                    Texture2D texture = GetTileTexture(tile.Type);
                    
                    if (texture != null)
                    {
                        // Draw the tile
                        spriteBatch.Draw(texture, tileRect, new Rectangle(0, 0, 16, 16), Color.White);
                    }
                    else
                    {
                        // Fallback to colored square if texture not found
                        Texture2D pixel = CreatePixelTexture(spriteBatch.GraphicsDevice);
                        spriteBatch.Draw(pixel, tileRect, tile.GetColor());
                    }
                }
                else
                {
                    // Final fallback: colored squares
                    Texture2D pixel = CreatePixelTexture(spriteBatch.GraphicsDevice);
                    spriteBatch.Draw(pixel, tileRect, tile.GetColor());
                }
                
                // Draw crop if present
                if (tile.Crop != null)
                {
                    DrawCrop(spriteBatch, tile, tileRect);
                }
            }
        }
        
        // Draw world objects (buildings, trees, rocks, etc.)
        // Sort by Y position for proper depth
        var sortedObjects = _worldObjects.OrderBy(obj => obj.Position.Y).ToList();
        foreach (var obj in sortedObjects)
        {
            obj.Draw(spriteBatch);
        }
    }
    
    private void DrawCrop(SpriteBatch spriteBatch, Tile tile, Rectangle tileRect)
    {
        Crop crop = tile.Crop;
        if (crop == null) return;
        
        // Get the crop texture array for this crop type
        if (_cropTextures.TryGetValue(crop.CropType.ToLower(), out Texture2D[] stages))
        {
            // Clamp growth stage to available textures
            int stage = Math.Clamp(crop.GrowthStage, 0, stages.Length - 1);
            Texture2D cropTexture = stages[stage];
            
            if (cropTexture != null)
            {
                // Draw crop centered on tile, scaled to fit
                spriteBatch.Draw(cropTexture, tileRect, Color.White);
            }
        }
    }
    
    private Texture2D GetTileTexture(TileType type)
    {
        // Try to get texture from dictionary
        if (_tileTextures.TryGetValue(type, out Texture2D texture))
        {
            return texture;
        }
        
        // Fallback to base types
        return type switch
        {
            TileType.Grass01 or TileType.Grass02 or TileType.Grass03 
                => _tileTextures.GetValueOrDefault(TileType.Grass),
            TileType.Dirt01 or TileType.Dirt02 
                => _tileTextures.GetValueOrDefault(TileType.Dirt),
            TileType.TilledDry or TileType.TilledWatered 
                => _tileTextures.GetValueOrDefault(TileType.Tilled),
            TileType.Stone01 or TileType.Rock 
                => _tileTextures.GetValueOrDefault(TileType.Stone),
            TileType.Water01 
                => _tileTextures.GetValueOrDefault(TileType.Water),
            TileType.Sand01 
                => _tileTextures.GetValueOrDefault(TileType.Sand),
            _ => null
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
    
    /// <summary>
    /// Plant some test crops for demonstration
    /// </summary>
    public void PlantTestCrops()
    {
        // Create a small farm area with different crops
        // Plant in a 10x5 grid starting at (20, 20)
        for (int x = 20; x < 30; x++)
        {
            for (int y = 20; y < 25; y++)
            {
                // Use legacy tilled dirt for the farm area
                _tiles[x, y].Type = TileType.Tilled;
                
                // Plant different crops in rows
                string cropType = (y - 20) switch
                {
                    0 => "wheat",
                    1 => "potato",
                    2 => "carrot",
                    3 => "cabbage",
                    4 => "beetroot",
                    _ => "wheat"
                };
                
                // Plant crop with different growth stages for variety
                int maxStages = 6;
                float hoursPerStage = 4f;
                Crop crop = new Crop(cropType, maxStages, hoursPerStage);
                
                // Give crops different growth stages for visual variety
                for (int stage = 0; stage < (x - 20) / 2; stage++)
                {
                    crop.Update(new GameTime(TimeSpan.Zero, TimeSpan.FromHours(hoursPerStage)));
                }
                
                _tiles[x, y].PlantCrop(crop);
            }
        }
    }
    
    /// <summary>
    /// Add a decorative world object (building, tree, rock, etc.)
    /// </summary>
    public void AddWorldObject(WorldObject obj)
    {
        _worldObjects.Add(obj);
    }
    
    /// <summary>
    /// Clear all world objects
    /// </summary>
    public void ClearWorldObjects()
    {
        _worldObjects.Clear();
    }
    
    /// <summary>
    /// Populate the world with decorative objects to match Sunnyside style
    /// </summary>
    public void PopulateSunnysideWorldObjects(
        Dictionary<string, Texture2D> buildings,
        Dictionary<string, Texture2D> trees,
        Dictionary<string, Texture2D> rocks)
    {
        Random random = new Random(42); // Fixed seed for consistency
        
        // Add colorful buildings in various locations (like the reference image)
        if (buildings.ContainsKey("House1"))
        {
            AddWorldObject(new WorldObject("House1", new Vector2(5 * TILE_SIZE, 8 * TILE_SIZE), buildings["House1"]));
        }
        
        if (buildings.ContainsKey("House2"))
        {
            AddWorldObject(new WorldObject("House2", new Vector2(15 * TILE_SIZE, 5 * TILE_SIZE), buildings["House2"]));
        }
        
        if (buildings.ContainsKey("House3_Yellow"))
        {
            AddWorldObject(new WorldObject("House3_Yellow", new Vector2(28 * TILE_SIZE, 10 * TILE_SIZE), buildings["House3_Yellow"]));
        }
        
        // Add towers for visual interest
        if (buildings.ContainsKey("Tower_Blue"))
        {
            AddWorldObject(new WorldObject("Tower_Blue", new Vector2(38 * TILE_SIZE, 12 * TILE_SIZE), buildings["Tower_Blue"]));
        }
        
        if (buildings.ContainsKey("Tower_Red"))
        {
            AddWorldObject(new WorldObject("Tower_Red", new Vector2(42 * TILE_SIZE, 8 * TILE_SIZE), buildings["Tower_Red"]));
        }
        
        // Add castles for grand structures
        if (buildings.ContainsKey("Castle_Blue"))
        {
            AddWorldObject(new WorldObject("Castle_Blue", new Vector2(35 * TILE_SIZE, 25 * TILE_SIZE), buildings["Castle_Blue"]));
        }
        
        // Add some smaller buildings
        if (buildings.ContainsKey("Barracks_Red"))
        {
            AddWorldObject(new WorldObject("Barracks_Red", new Vector2(10 * TILE_SIZE, 30 * TILE_SIZE), buildings["Barracks_Red"]));
        }
        
        if (buildings.ContainsKey("Monastery_Yellow"))
        {
            AddWorldObject(new WorldObject("Monastery_Yellow", new Vector2(25 * TILE_SIZE, 35 * TILE_SIZE), buildings["Monastery_Yellow"]));
        }
        
        // Add trees around the map for decoration
        string[] treeKeys = { "Tree1", "Tree2", "Tree3", "Tree4" };
        List<Vector2> treePositions = new List<Vector2>
        {
            new Vector2(8 * TILE_SIZE, 15 * TILE_SIZE),
            new Vector2(12 * TILE_SIZE, 18 * TILE_SIZE),
            new Vector2(18 * TILE_SIZE, 12 * TILE_SIZE),
            new Vector2(22 * TILE_SIZE, 8 * TILE_SIZE),
            new Vector2(32 * TILE_SIZE, 6 * TILE_SIZE),
            new Vector2(40 * TILE_SIZE, 20 * TILE_SIZE),
            new Vector2(5 * TILE_SIZE, 40 * TILE_SIZE),
            new Vector2(15 * TILE_SIZE, 42 * TILE_SIZE),
            new Vector2(30 * TILE_SIZE, 45 * TILE_SIZE),
            new Vector2(45 * TILE_SIZE, 35 * TILE_SIZE)
        };
        
        foreach (var pos in treePositions)
        {
            string treeKey = treeKeys[random.Next(treeKeys.Length)];
            if (trees.ContainsKey(treeKey))
            {
                AddWorldObject(new WorldObject(treeKey, pos, trees[treeKey]));
            }
        }
        
        // Add rocks for terrain detail
        string[] rockKeys = { "Rock1", "Rock2", "Rock3" };
        List<Vector2> rockPositions = new List<Vector2>
        {
            new Vector2(36 * TILE_SIZE, 36 * TILE_SIZE),
            new Vector2(38 * TILE_SIZE, 38 * TILE_SIZE),
            new Vector2(37 * TILE_SIZE, 39 * TILE_SIZE),
            new Vector2(3 * TILE_SIZE, 10 * TILE_SIZE),
            new Vector2(24 * TILE_SIZE, 6 * TILE_SIZE)
        };
        
        foreach (var pos in rockPositions)
        {
            string rockKey = rockKeys[random.Next(rockKeys.Length)];
            if (rocks.ContainsKey(rockKey))
            {
                AddWorldObject(new WorldObject(rockKey, pos, rocks[rockKey]));
            }
        }
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
