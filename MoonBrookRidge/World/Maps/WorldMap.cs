using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.Core.Systems;

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
    
    // Mine entrance location
    public Vector2 MineEntranceGridPosition { get; private set; }
    
    public WorldMap()
    {
        _width = 250;  // 5X larger than original 50x50 (250x250 = 62,500 tiles vs 50x50 = 2,500 tiles)
        _height = 250;
        _tiles = new Tile[_width, _height];
        _cropTextures = new Dictionary<string, Texture2D[]>();
        _tileTextures = new Dictionary<TileType, Texture2D>();
        _sunnysideTileMapping = new Dictionary<TileType, int>();
        _worldObjects = new List<WorldObject>();
        MineEntranceGridPosition = new Vector2(50, 200); // Adjusted for larger map
        
        InitializeMap();
    }
    
    /// <summary>
    /// Initialize world from a configuration object
    /// </summary>
    public void InitializeFromConfig(WorldGenConfig config)
    {
        _width = config.Width;
        _height = config.Height;
        _tiles = WorldGenConfigApplier.GenerateWorld(config);
        MineEntranceGridPosition = new Vector2(config.MineEntrance.X, config.MineEntrance.Y);
    }
    
    private void InitializeMap()
    {
        // Create a large flat farmable world - 5X bigger than before
        // The central area (100x100 tiles) is flat farmable land for the starting farm
        // Outer areas can be expanded through upgrades and plot purchases
        Random random = new Random(42); // Fixed seed for consistency
        
        // Define the central farm area - much larger now for 5X expansion
        int centerX = _width / 2;
        int centerY = _height / 2;
        int farmRadius = 50; // 100x100 flat farmable area in center
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                TileType tileType;
                
                // Calculate distance from center
                int distanceFromCenter = Math.Max(Math.Abs(x - centerX), Math.Abs(y - centerY));
                
                // Central farm area (100x100) - flat, mostly grass for farming
                bool isCentralFarm = distanceFromCenter <= farmRadius;
                
                if (isCentralFarm)
                {
                    // Flat farmable terrain in the center
                    int variant = random.Next(10);
                    if (variant < 6)
                        tileType = TileType.Grass;      // 60% base grass
                    else if (variant < 9)
                        tileType = TileType.Grass01;   // 30% light grass
                    else
                        tileType = TileType.Grass02;   // 10% medium grass
                }
                // Outer expansion areas - still farmable but with more variety
                else if (distanceFromCenter <= farmRadius + 25)
                {
                    // Near-farm expansion zone (50 tiles out from center farm)
                    int variant = random.Next(15);
                    if (variant < 5)
                        tileType = TileType.Grass;
                    else if (variant < 9)
                        tileType = TileType.Grass01;
                    else if (variant < 12)
                        tileType = TileType.Grass02;
                    else
                        tileType = TileType.Grass03;   // Some darker grass
                }
                // Far expansion areas - purchasable plots
                else
                {
                    // Outer wilderness - can be unlocked/purchased later
                    int variant = random.Next(20);
                    if (variant < 7)
                        tileType = TileType.Grass;
                    else if (variant < 12)
                        tileType = TileType.Grass01;
                    else if (variant < 16)
                        tileType = TileType.Grass02;
                    else
                        tileType = TileType.Grass03;
                }
                
                _tiles[x, y] = new Tile(tileType, new Vector2(x, y));
            }
        }
        
        // Add some water features in specific areas (not in center farm)
        // Pond in the northeast
        for (int x = centerX + 60; x < centerX + 80 && x < _width; x++)
        {
            for (int y = centerY - 80; y < centerY - 60 && y >= 0; y++)
            {
                _tiles[x, y] = new Tile(TileType.Water, new Vector2(x, y));
            }
        }
        
        // Sand around the pond
        for (int x = centerX + 58; x < centerX + 82 && x < _width; x++)
        {
            for (int y = centerY - 82; y < centerY - 58 && y >= 0; y++)
            {
                if (_tiles[x, y].Type != TileType.Water)
                {
                    _tiles[x, y] = new Tile(
                        random.Next(2) == 0 ? TileType.Sand : TileType.Sand01,
                        new Vector2(x, y)
                    );
                }
            }
        }
        
        // Place mine entrance
        _tiles[(int)MineEntranceGridPosition.X, (int)MineEntranceGridPosition.Y] = 
            new Tile(TileType.MineEntrance, MineEntranceGridPosition);
        
        // Place dungeon entrances in outer areas
        PlaceDungeonEntrances();
    }
    
    /// <summary>
    /// Place dungeon entrances strategically around the world
    /// Adjusted for 250x250 world size
    /// </summary>
    private void PlaceDungeonEntrances()
    {
        int centerX = _width / 2;
        int centerY = _height / 2;
        
        // Slime Cave - northeast (near water)
        _tiles[centerX + 70, centerY - 70] = new Tile(TileType.DungeonEntranceSlime, new Vector2(centerX + 70, centerY - 70));
        
        // Skeleton Crypt - northwest
        _tiles[centerX - 90, centerY - 90] = new Tile(TileType.DungeonEntranceSkeleton, new Vector2(centerX - 90, centerY - 90));
        
        // Spider Nest - west side
        _tiles[centerX - 100, centerY] = new Tile(TileType.DungeonEntranceSpider, new Vector2(centerX - 100, centerY));
        
        // Goblin Warrens - southwest
        _tiles[centerX - 80, centerY + 80] = new Tile(TileType.DungeonEntranceGoblin, new Vector2(centerX - 80, centerY + 80));
        
        // Haunted Manor - north
        _tiles[centerX, centerY - 100] = new Tile(TileType.DungeonEntranceHaunted, new Vector2(centerX, centerY - 100));
        
        // Dragon Lair - east side
        _tiles[centerX + 100, centerY] = new Tile(TileType.DungeonEntranceDragon, new Vector2(centerX + 100, centerY));
        
        // Demon Realm - southeast
        _tiles[centerX + 80, centerY + 90] = new Tile(TileType.DungeonEntranceDemon, new Vector2(centerX + 80, centerY + 90));
        
        // Ancient Ruins - south
        _tiles[centerX, centerY + 100] = new Tile(TileType.DungeonEntranceRuins, new Vector2(centerX, centerY + 100));
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
        Draw(spriteBatch, null);
    }
    
    /// <summary>
    /// Draw the world map with optional frustum culling
    /// </summary>
    /// <param name="spriteBatch">SpriteBatch to draw with</param>
    /// <param name="visibleBounds">Optional viewport bounds for frustum culling. If null, draws all tiles.</param>
    public void Draw(SpriteBatch spriteBatch, Rectangle? visibleBounds)
    {
        // Calculate visible tile range for frustum culling
        int startX = 0;
        int endX = _width;
        int startY = 0;
        int endY = _height;
        
        if (visibleBounds.HasValue)
        {
            // Add buffer of 2 tiles on each side to prevent pop-in at edges
            const int BUFFER_TILES = 2;
            
            Rectangle bounds = visibleBounds.Value;
            startX = Math.Max(0, (bounds.Left / TILE_SIZE) - BUFFER_TILES);
            endX = Math.Min(_width, (bounds.Right / TILE_SIZE) + BUFFER_TILES + 1);
            startY = Math.Max(0, (bounds.Top / TILE_SIZE) - BUFFER_TILES);
            endY = Math.Min(_height, (bounds.Bottom / TILE_SIZE) + BUFFER_TILES + 1);
        }
        
        // Use Sunnyside tileset if available, otherwise fall back to individual textures or colored squares
        for (int x = startX; x < endX; x++)
        {
            for (int y = startY; y < endY; y++)
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
        
        // Apply frustum culling to world objects
        foreach (var obj in sortedObjects)
        {
            if (visibleBounds.HasValue)
            {
                // Check if object is within visible bounds using actual object bounds
                // Add a small buffer to prevent pop-in at edges
                Rectangle objBounds = obj.GetBounds();
                Rectangle bufferedViewport = new Rectangle(
                    visibleBounds.Value.X - 32,
                    visibleBounds.Value.Y - 32,
                    visibleBounds.Value.Width + 64,
                    visibleBounds.Value.Height + 64
                );
                
                if (!objBounds.Intersects(bufferedViewport))
                {
                    continue; // Skip objects outside visible area
                }
            }
            
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
    
    public Tile[,] GetAllTiles()
    {
        return _tiles;
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
    /// Get world object at a specific position within a radius
    /// </summary>
    public WorldObject GetWorldObjectAt(Vector2 position, float radius = 16f)
    {
        foreach (var obj in _worldObjects)
        {
            float distance = Vector2.Distance(obj.Position, position);
            if (distance <= radius)
            {
                return obj;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Remove a world object from the world
    /// </summary>
    public bool RemoveWorldObject(WorldObject obj)
    {
        return _worldObjects.Remove(obj);
    }
    
    /// <summary>
    /// Get all world objects
    /// </summary>
    public IReadOnlyList<WorldObject> GetWorldObjects()
    {
        return _worldObjects.AsReadOnly();
    }
    
    /// <summary>
    /// Populate the world with natural elements - forests at borders and scattered throughout
    /// </summary>
    public void PopulateSunnysideWorldObjects(
        Dictionary<string, Texture2D> buildings,
        Dictionary<string, SpriteInfo> trees,
        Dictionary<string, SpriteInfo> rocks)
    {
        Random random = new Random(42); // Fixed seed for consistency
        
        // NO BUILDINGS - focusing on natural scene generation
        
        // Get all tree sprite keys (e.g., Tree1_0, Tree1_1, etc.)
        var treeKeys = trees.Keys.ToList();
        
        // Top border - dense tree line
        for (int x = 0; x < _width; x += 2)
        {
            string treeKey = treeKeys[random.Next(treeKeys.Count)];
            if (trees.ContainsKey(treeKey))
            {
                AddWorldObject(new ChoppableTree(treeKey, new Vector2(x * TILE_SIZE, 1 * TILE_SIZE), trees[treeKey], "Oak", random));
            }
        }
        
        // Bottom border - dense tree line
        for (int x = 0; x < _width; x += 2)
        {
            string treeKey = treeKeys[random.Next(treeKeys.Count)];
            if (trees.ContainsKey(treeKey))
            {
                AddWorldObject(new ChoppableTree(treeKey, new Vector2(x * TILE_SIZE, (_height - 2) * TILE_SIZE), trees[treeKey], "Oak", random));
            }
        }
        
        // Left border - dense tree line
        for (int y = 0; y < _height; y += 2)
        {
            string treeKey = treeKeys[random.Next(treeKeys.Count)];
            if (trees.ContainsKey(treeKey))
            {
                AddWorldObject(new ChoppableTree(treeKey, new Vector2(1 * TILE_SIZE, y * TILE_SIZE), trees[treeKey], "Oak", random));
            }
        }
        
        // Right border - dense tree line
        for (int y = 0; y < _height; y += 2)
        {
            string treeKey = treeKeys[random.Next(treeKeys.Count)];
            if (trees.ContainsKey(treeKey))
            {
                AddWorldObject(new ChoppableTree(treeKey, new Vector2((_width - 2) * TILE_SIZE, y * TILE_SIZE), trees[treeKey], "Oak", random));
            }
        }
        
        // Add forest clusters in non-farm areas for natural feel
        List<Vector2> forestPositions = new List<Vector2>
        {
            // Northwest forest cluster
            new Vector2(5 * TILE_SIZE, 5 * TILE_SIZE),
            new Vector2(7 * TILE_SIZE, 6 * TILE_SIZE),
            new Vector2(6 * TILE_SIZE, 8 * TILE_SIZE),
            new Vector2(8 * TILE_SIZE, 7 * TILE_SIZE),
            new Vector2(10 * TILE_SIZE, 5 * TILE_SIZE),
            new Vector2(11 * TILE_SIZE, 8 * TILE_SIZE),
            
            // Northeast forest cluster (around pond)
            new Vector2(32 * TILE_SIZE, 8 * TILE_SIZE),
            new Vector2(33 * TILE_SIZE, 11 * TILE_SIZE),
            new Vector2(47 * TILE_SIZE, 10 * TILE_SIZE),
            new Vector2(46 * TILE_SIZE, 13 * TILE_SIZE),
            
            // Southwest forest cluster
            new Vector2(6 * TILE_SIZE, 40 * TILE_SIZE),
            new Vector2(8 * TILE_SIZE, 42 * TILE_SIZE),
            new Vector2(10 * TILE_SIZE, 41 * TILE_SIZE),
            new Vector2(7 * TILE_SIZE, 44 * TILE_SIZE),
            new Vector2(11 * TILE_SIZE, 45 * TILE_SIZE),
            
            // Southeast forest cluster
            new Vector2(40 * TILE_SIZE, 40 * TILE_SIZE),
            new Vector2(42 * TILE_SIZE, 42 * TILE_SIZE),
            new Vector2(44 * TILE_SIZE, 41 * TILE_SIZE),
            new Vector2(43 * TILE_SIZE, 44 * TILE_SIZE),
            new Vector2(45 * TILE_SIZE, 45 * TILE_SIZE),
            
            // Scattered trees for organic feel (avoiding farm area)
            new Vector2(15 * TILE_SIZE, 10 * TILE_SIZE),
            new Vector2(17 * TILE_SIZE, 13 * TILE_SIZE),
            new Vector2(12 * TILE_SIZE, 15 * TILE_SIZE),
            new Vector2(38 * TILE_SIZE, 20 * TILE_SIZE),
            new Vector2(40 * TILE_SIZE, 25 * TILE_SIZE),
            new Vector2(15 * TILE_SIZE, 38 * TILE_SIZE),
            new Vector2(18 * TILE_SIZE, 40 * TILE_SIZE),
            new Vector2(36 * TILE_SIZE, 35 * TILE_SIZE)
        };
        
        foreach (var pos in forestPositions)
        {
            string treeKey = treeKeys[random.Next(treeKeys.Count)];
            if (trees.ContainsKey(treeKey))
            {
                AddWorldObject(new ChoppableTree(treeKey, pos, trees[treeKey], "Oak", random));
            }
        }
        
        // Add scattered rocks for natural terrain detail (fewer than before)
        var rockKeys = rocks.Keys.ToList();
        List<Vector2> rockPositions = new List<Vector2>
        {
            // Near pond
            new Vector2(32 * TILE_SIZE, 16 * TILE_SIZE),
            new Vector2(47 * TILE_SIZE, 7 * TILE_SIZE),
            
            // Forest areas
            new Vector2(8 * TILE_SIZE, 10 * TILE_SIZE),
            new Vector2(13 * TILE_SIZE, 43 * TILE_SIZE),
            new Vector2(42 * TILE_SIZE, 38 * TILE_SIZE),
            
            // Scattered
            new Vector2(18 * TILE_SIZE, 15 * TILE_SIZE),
            new Vector2(37 * TILE_SIZE, 28 * TILE_SIZE)
        };
        
        foreach (var pos in rockPositions)
        {
            string rockKey = rockKeys[random.Next(rockKeys.Count)];
            if (rocks.ContainsKey(rockKey))
            {
                AddWorldObject(new BreakableRock(rockKey, pos, rocks[rockKey], random));
            }
        }
    }
    
    private Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
    {
        Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
    
    /// <summary>
    /// Grow crops in an area around a center tile (for magic spell)
    /// </summary>
    public void GrowCropsInArea(Vector2 centerTile, int radius)
    {
        int startX = Math.Max(0, (int)centerTile.X - radius);
        int endX = Math.Min(_width - 1, (int)centerTile.X + radius);
        int startY = Math.Max(0, (int)centerTile.Y - radius);
        int endY = Math.Min(_height - 1, (int)centerTile.Y + radius);
        
        for (int x = startX; x <= endX; x++)
        {
            for (int y = startY; y <= endY; y++)
            {
                var tile = GetTile(x, y);
                if (tile != null && tile.Crop != null)
                {
                    // Advance crop growth by 24 game hours (1 full day)
                    tile.Crop.UpdateGrowth(24f);
                }
            }
        }
    }
    
    /// <summary>
    /// Water all crops on the entire map (for magic spell)
    /// </summary>
    public void WaterAllCrops()
    {
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var tile = GetTile(x, y);
                if (tile != null && tile.Crop != null)
                {
                    // Mark tile as watered
                    tile.Water();
                }
            }
        }
    }
    
    public int Width => _width;
    public int Height => _height;
}
