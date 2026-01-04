using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.Combat;
using MoonBrookRidge.Core.Systems;
using System.Collections.Generic;

namespace MoonBrookRidge.Core.Scenes;

/// <summary>
/// Dungeon floor scene - minimum 100 floors with increasing difficulty
/// Each floor is a separate scene with procedurally generated rooms
/// </summary>
public class DungeonScene : Scene
{
    private int _floorNumber;
    private int _difficulty;
    private List<Enemy> _enemies;
    private List<SceneObject> _loot;
    private bool _isCleared;
    private DungeonFloorType _floorType;
    
    public int FloorNumber => _floorNumber;
    public int Difficulty => _difficulty;
    public bool IsCleared => _isCleared;
    public DungeonFloorType FloorType => _floorType;
    
    public DungeonScene(string dungeonName, int floorNumber, int width = 50, int height = 50) 
        : base($"{dungeonName} - Floor {floorNumber}", $"dungeon_{dungeonName}_floor_{floorNumber}", SceneType.Dungeon, width, height)
    {
        _floorNumber = floorNumber;
        _difficulty = CalculateDifficulty(floorNumber);
        _enemies = new List<Enemy>();
        _loot = new List<SceneObject>();
        _isCleared = false;
        _floorType = DetermineFloorType(floorNumber);
    }
    
    /// <summary>
    /// Calculate difficulty based on floor number
    /// Difficulty increases every 10 floors
    /// </summary>
    private int CalculateDifficulty(int floor)
    {
        // Base difficulty + floor scaling
        return 1 + (floor / 10);
    }
    
    /// <summary>
    /// Determine floor type based on depth
    /// </summary>
    private DungeonFloorType DetermineFloorType(int floor)
    {
        if (floor % 10 == 0) return DungeonFloorType.Boss; // Every 10th floor is boss
        if (floor % 5 == 0) return DungeonFloorType.Treasure; // Every 5th floor has treasure
        return DungeonFloorType.Combat; // Regular combat floors
    }
    
    public override void Initialize()
    {
        // Generate procedural dungeon layout
        GenerateDungeonLayout();
        
        // Spawn enemies based on difficulty
        SpawnEnemies();
        
        // Add stairs to next floor
        AddStairsTransition();
    }
    
    private void GenerateDungeonLayout()
    {
        // Fill with stone/cave tiles
        System.Random random = new System.Random(_floorNumber);
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                TileType tileType;
                
                // Create walls on edges
                if (x == 0 || x == _width - 1 || y == 0 || y == _height - 1)
                {
                    tileType = TileType.Wall;
                }
                // Create some random wall obstacles (30% chance)
                else if (random.Next(100) < 30)
                {
                    tileType = TileType.Rock;
                }
                else
                {
                    // Floor tiles with variation
                    int variant = random.Next(10);
                    tileType = variant < 7 ? TileType.Stone : TileType.Stone01;
                }
                
                SetTile(x, y, new Tile(tileType, new Vector2(x, y)));
            }
        }
        
        // Clear starting area near entrance
        for (int x = _width / 2 - 3; x < _width / 2 + 3; x++)
        {
            for (int y = 2; y < 6; y++)
            {
                SetTile(x, y, new Tile(TileType.Stone, new Vector2(x, y)));
            }
        }
    }
    
    private void SpawnEnemies()
    {
        if (_floorType == DungeonFloorType.Treasure) return; // No enemies on treasure floors
        
        System.Random random = new System.Random(_floorNumber + 1000);
        
        // Number of enemies scales with difficulty
        int enemyCount = 5 + (_difficulty * 3);
        if (_floorType == DungeonFloorType.Boss) enemyCount = 1; // Boss floor has 1 strong enemy
        
        for (int i = 0; i < enemyCount; i++)
        {
            // Random spawn position (avoid edges)
            Vector2 spawnPos = new Vector2(
                random.Next(5, _width - 5) * GameConstants.TILE_SIZE,
                random.Next(10, _height - 5) * GameConstants.TILE_SIZE
            );
            
            // Create enemy with scaled stats
            Enemy enemy = CreateScaledEnemy(spawnPos, random);
            _enemies.Add(enemy);
            AddObject(new SceneObject($"Enemy_{i}", spawnPos, true, false));
        }
    }
    
    private Enemy CreateScaledEnemy(Vector2 position, System.Random random)
    {
        // Create enemy with stats scaled by floor difficulty
        var enemyTypes = new[] { EnemyType.Slime, EnemyType.Skeleton, EnemyType.Spider, EnemyType.Goblin, EnemyType.Ghost, EnemyType.Demon };
        EnemyType type = enemyTypes[random.Next(enemyTypes.Length)];
        
        int baseHealth = 50 + (_difficulty * 20);
        int baseDamage = 10 + (_difficulty * 5);
        int baseDefense = _difficulty * 2;
        float baseSpeed = 50f + (_difficulty * 5f);
        int baseExp = 25 + (_difficulty * 10);
        bool isBoss = false;
        
        if (_floorType == DungeonFloorType.Boss)
        {
            baseHealth *= 5; // Boss has 5x health
            baseDamage *= 2; // Boss has 2x damage
            baseDefense *= 2;
            baseExp *= 3;
            isBoss = true;
        }
        
        string id = isBoss ? $"Boss_{type}" : type.ToString();
        string name = isBoss ? $"Boss {type}" : type.ToString();
        
        // Constructor: id, name, type, health, damage, defense, speed, experience, isBoss
        var enemy = new Enemy(id, name, type, baseHealth, baseDamage, baseDefense / 100f, baseSpeed, baseExp, isBoss);
        enemy.Position = position;
        return enemy;
    }
    
    private void AddStairsTransition()
    {
        // Stairs at bottom center leading to next floor
        Vector2 stairsPos = new Vector2(_width / 2, _height - 3);
        string nextFloorId = $"dungeon_{Name.Split('-')[0].Trim()}_floor_{_floorNumber + 1}";
        Vector2 nextSpawnPos = new Vector2(_width / 2, 3); // Spawn at top of next floor
        
        var stairsTransition = new SceneTransition(
            $"stairs_floor_{_floorNumber}",
            stairsPos * GameConstants.TILE_SIZE,
            nextFloorId,
            nextSpawnPos,
            TransitionType.Stairs,
            requiresInteraction: true
        );
        
        AddTransition(stairsTransition);
        
        // Also add stairs back up to previous floor (if not floor 1)
        if (_floorNumber > 1)
        {
            Vector2 upStairsPos = new Vector2(_width / 2, 2);
            string prevFloorId = $"dungeon_{Name.Split('-')[0].Trim()}_floor_{_floorNumber - 1}";
            Vector2 prevSpawnPos = new Vector2(_width / 2, _height - 4);
            
            var upStairsTransition = new SceneTransition(
                $"stairs_up_floor_{_floorNumber}",
                upStairsPos * GameConstants.TILE_SIZE,
                prevFloorId,
                prevSpawnPos,
                TransitionType.Stairs,
                requiresInteraction: true
            );
            
            AddTransition(upStairsTransition);
        }
    }
    
    public override void LoadContent()
    {
        // Load dungeon textures (use existing cave/stone textures)
    }
    
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // Check if all enemies are defeated
        if (!_isCleared && _enemies.Count > 0)
        {
            int aliveCount = 0;
            foreach (var enemy in _enemies)
            {
                if (!enemy.IsDead)
                {
                    aliveCount++;
                    enemy.Update(gameTime);
                }
            }
            
            if (aliveCount == 0)
            {
                _isCleared = true;
                SpawnRewards();
            }
        }
    }
    
    private void SpawnRewards()
    {
        // Spawn loot when floor is cleared
        System.Random random = new System.Random(_floorNumber + 2000);
        
        int goldAmount = 50 + (_difficulty * 25);
        int itemCount = 1 + (_difficulty / 2);
        
        // Add rewards based on floor type
        if (_floorType == DungeonFloorType.Boss)
        {
            goldAmount *= 3;
            itemCount *= 2;
        }
        
        // TODO: Spawn actual loot items
    }
    
    public override void Draw(SpriteBatch spriteBatch, Camera2D camera)
    {
        // Draw dungeon tiles
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                var tile = GetTile(x, y);
                if (tile != null)
                {
                    Vector2 worldPosition = new Vector2(x * GameConstants.TILE_SIZE, y * GameConstants.TILE_SIZE);
                    
                    if (camera.IsInView(worldPosition, GameConstants.TILE_SIZE, GameConstants.TILE_SIZE))
                    {
                        DrawDungeonTile(spriteBatch, tile, worldPosition);
                    }
                }
            }
        }
        
        // Draw enemies
        // Note: Enemy class doesn't have Draw method yet - TODO: implement enemy rendering
        /*foreach (var enemy in _enemies)
        {
            if (!enemy.IsDead && camera.IsInView(enemy.Position, 32, 32))
            {
                enemy.Draw(spriteBatch);
            }
        }*/
        
        // Draw objects
        foreach (var obj in _objects)
        {
            if (camera.IsInView(obj.Position, GameConstants.TILE_SIZE, GameConstants.TILE_SIZE))
            {
                obj.Draw(spriteBatch);
            }
        }
    }
    
    private void DrawDungeonTile(SpriteBatch spriteBatch, Tile tile, Vector2 position)
    {
        Color tileColor = tile.GetColor();
        Rectangle destRect = new Rectangle(
            (int)position.X,
            (int)position.Y,
            GameConstants.TILE_SIZE,
            GameConstants.TILE_SIZE
        );
        
        // Draw simple colored tile (will be enhanced with textures)
        // TODO: Use actual dungeon tileset
    }
    
    public List<Enemy> GetEnemies()
    {
        return _enemies;
    }
}

/// <summary>
/// Types of dungeon floors
/// </summary>
public enum DungeonFloorType
{
    Combat,     // Regular combat floor
    Treasure,   // Treasure room (no enemies)
    Boss,       // Boss fight floor
    Shop,       // Merchant floor
    Rest        // Safe room for healing
}
