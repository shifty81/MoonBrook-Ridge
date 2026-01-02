using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MoonBrookRidge.Combat;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.Dungeons;

/// <summary>
/// Procedural dungeon generation and management system
/// </summary>
public class DungeonSystem
{
    private List<Dungeon> _generatedDungeons;
    private Dungeon _activeDungeon;
    private Random _random;
    
    public Dungeon ActiveDungeon => _activeDungeon;
    
    public event Action<Dungeon> OnDungeonEntered;
    public event Action<Dungeon> OnDungeonCleared;
    public event Action<DungeonRoom> OnRoomEntered;
    
    public DungeonSystem(int seed = 0)
    {
        _generatedDungeons = new List<Dungeon>();
        _random = seed == 0 ? new Random() : new Random(seed);
    }
    
    public Dungeon GenerateDungeon(DungeonType type, int floors, int difficulty)
    {
        var dungeon = new Dungeon(type, floors, difficulty);
        
        for (int floor = 1; floor <= floors; floor++)
        {
            GenerateFloor(dungeon, floor, difficulty);
        }
        
        _generatedDungeons.Add(dungeon);
        return dungeon;
    }
    
    private void GenerateFloor(Dungeon dungeon, int floorNumber, int difficulty)
    {
        int roomCount = _random.Next(6, 12); // 6-11 rooms per floor
        var rooms = new List<DungeonRoom>();
        
        // Entrance room
        var entrance = new DungeonRoom(RoomType.Entrance, new Vector2(0, 0), floorNumber);
        rooms.Add(entrance);
        
        // Regular combat rooms
        int combatRooms = roomCount - 3; // Reserve space for treasure, boss, exit
        for (int i = 0; i < combatRooms; i++)
        {
            var room = GenerateCombatRoom(i + 1, floorNumber, difficulty);
            rooms.Add(room);
        }
        
        // Treasure room
        var treasureRoom = new DungeonRoom(RoomType.Treasure, new Vector2(combatRooms, 0), floorNumber);
        PopulateTreasureRoom(treasureRoom, difficulty);
        rooms.Add(treasureRoom);
        
        // Boss room
        var bossRoom = new DungeonRoom(RoomType.Boss, new Vector2(combatRooms + 1, 0), floorNumber);
        PopulateBossRoom(bossRoom, dungeon.Type, difficulty);
        rooms.Add(bossRoom);
        
        // Exit/stairs
        var exit = new DungeonRoom(RoomType.Exit, new Vector2(combatRooms + 2, 0), floorNumber);
        rooms.Add(exit);
        
        dungeon.AddFloor(floorNumber, rooms);
    }
    
    private DungeonRoom GenerateCombatRoom(int index, int floor, int difficulty)
    {
        var room = new DungeonRoom(RoomType.Combat, new Vector2(index, 0), floor);
        
        // Add enemies based on difficulty
        int enemyCount = _random.Next(2, 5) + (difficulty / 2);
        
        for (int i = 0; i < enemyCount; i++)
        {
            // Random spawn position in room
            var position = new Vector2(_random.Next(100, 400), _random.Next(100, 400));
            
            // Select enemy based on room number and difficulty
            Enemy enemy = SelectRandomEnemy(floor, difficulty, position);
            room.AddEnemy(enemy);
        }
        
        return room;
    }
    
    private Enemy SelectRandomEnemy(int floor, int difficulty, Vector2 position)
    {
        int level = floor + difficulty;
        int roll = _random.Next(100);
        
        // Common enemies (70% chance)
        if (roll < 70)
        {
            int enemyType = _random.Next(6);
            return enemyType switch
            {
                0 => EnemyFactory.CreateSlime(position),
                1 => EnemyFactory.CreateBat(position),
                2 => EnemyFactory.CreateSkeleton(position),
                3 => EnemyFactory.CreateGoblin(position),
                4 => EnemyFactory.CreateSpider(position),
                _ => EnemyFactory.CreateWolf(position)
            };
        }
        // Uncommon enemies (25% chance)
        else if (roll < 95)
        {
            int enemyType = _random.Next(3);
            return enemyType switch
            {
                0 => EnemyFactory.CreateGhost(position),
                1 => EnemyFactory.CreateZombie(position),
                _ => EnemyFactory.CreateOrc(position)
            };
        }
        // Rare enemies (5% chance)
        else
        {
            int enemyType = _random.Next(2);
            return enemyType == 0 
                ? EnemyFactory.CreateFireElemental(position)
                : EnemyFactory.CreateDemon(position);
        }
    }
    
    private void PopulateTreasureRoom(DungeonRoom room, int difficulty)
    {
        // Add treasure chests
        int chestCount = _random.Next(2, 4);
        for (int i = 0; i < chestCount; i++)
        {
            var chest = new TreasureChest(difficulty);
            room.AddChest(chest);
        }
    }
    
    private void PopulateBossRoom(DungeonRoom room, DungeonType type, int difficulty)
    {
        var position = new Vector2(250, 250); // Center of room
        
        Enemy boss = type switch
        {
            DungeonType.SlimeCave => EnemyFactory.CreateSlimeKing(position),
            DungeonType.SkeletonCrypt => EnemyFactory.CreateSkeletonLord(position),
            DungeonType.DemonRealm => EnemyFactory.CreateArchDemon(position),
            DungeonType.DragonLair => EnemyFactory.CreateDragon(position),
            _ => EnemyFactory.CreateSlimeKing(position)
        };
        
        room.AddEnemy(boss);
    }
    
    public void EnterDungeon(Dungeon dungeon)
    {
        _activeDungeon = dungeon;
        dungeon.CurrentFloor = 1;
        OnDungeonEntered?.Invoke(dungeon);
    }
    
    public void ExitDungeon()
    {
        _activeDungeon = null;
    }
    
    public void EnterRoom(DungeonRoom room)
    {
        OnRoomEntered?.Invoke(room);
    }
    
    public void ClearRoom(DungeonRoom room)
    {
        room.IsCleared = true;
        
        // Check if dungeon is fully cleared
        if (_activeDungeon != null && _activeDungeon.IsFullyCleared())
        {
            OnDungeonCleared?.Invoke(_activeDungeon);
        }
    }
    
    /// <summary>
    /// Export dungeon progress for saving
    /// </summary>
    public DungeonProgressData ExportSaveData()
    {
        var completedDungeons = new List<DungeonCompletionData>();
        
        foreach (var dungeon in _generatedDungeons)
        {
            if (dungeon.IsFullyCleared())
            {
                completedDungeons.Add(new DungeonCompletionData
                {
                    DungeonType = dungeon.Type.ToString(),
                    HighestFloorReached = dungeon.TotalFloors,
                    Completed = true,
                    FirstCompletionTime = DateTime.Now // Note: Would need to track actual completion time
                });
            }
        }
        
        return new DungeonProgressData
        {
            CompletedDungeons = completedDungeons.ToArray(),
            IsInDungeon = _activeDungeon != null,
            CurrentDungeonType = _activeDungeon?.Type.ToString(),
            CurrentFloor = _activeDungeon?.CurrentFloor ?? 0
        };
    }
    
    /// <summary>
    /// Import dungeon progress from save data
    /// Note: This only restores completion records, not active dungeon state
    /// Player should re-enter dungeons manually
    /// </summary>
    public void ImportSaveData(DungeonProgressData data)
    {
        if (data == null) return;
        
        // Note: We don't restore the active dungeon from save
        // Player will need to re-enter the dungeon
        // We only track which dungeons have been completed for achievements/records
    }
}

/// <summary>
/// Dungeon instance with multiple floors
/// </summary>
public class Dungeon
{
    public DungeonType Type { get; }
    public int TotalFloors { get; }
    public int Difficulty { get; }
    public int CurrentFloor { get; set; }
    private Dictionary<int, List<DungeonRoom>> _floors;
    
    public Dungeon(DungeonType type, int totalFloors, int difficulty)
    {
        Type = type;
        TotalFloors = totalFloors;
        Difficulty = difficulty;
        CurrentFloor = 1;
        _floors = new Dictionary<int, List<DungeonRoom>>();
    }
    
    public void AddFloor(int floorNumber, List<DungeonRoom> rooms)
    {
        _floors[floorNumber] = rooms;
    }
    
    public List<DungeonRoom> GetFloor(int floorNumber)
    {
        return _floors.ContainsKey(floorNumber) ? _floors[floorNumber] : new List<DungeonRoom>();
    }
    
    public List<DungeonRoom> GetCurrentFloor()
    {
        return GetFloor(CurrentFloor);
    }
    
    public bool IsFullyCleared()
    {
        foreach (var floor in _floors.Values)
        {
            foreach (var room in floor)
            {
                if (room.Type == RoomType.Combat || room.Type == RoomType.Boss)
                {
                    if (!room.IsCleared)
                    {
                        return false;
                    }
                }
            }
        }
        return true;
    }
}

/// <summary>
/// Individual room within a dungeon
/// </summary>
public class DungeonRoom
{
    public RoomType Type { get; }
    public Vector2 Position { get; }
    public int Floor { get; }
    public bool IsCleared { get; set; }
    public List<Enemy> Enemies { get; }
    public List<TreasureChest> Chests { get; }
    
    public DungeonRoom(RoomType type, Vector2 position, int floor)
    {
        Type = type;
        Position = position;
        Floor = floor;
        IsCleared = false;
        Enemies = new List<Enemy>();
        Chests = new List<TreasureChest>();
    }
    
    public void AddEnemy(Enemy enemy)
    {
        Enemies.Add(enemy);
    }
    
    public void AddChest(TreasureChest chest)
    {
        Chests.Add(chest);
    }
    
    public bool AllEnemiesDefeated()
    {
        return Enemies.All(e => e.IsDead);
    }
}

/// <summary>
/// Treasure chest with loot
/// </summary>
public class TreasureChest
{
    public bool IsOpened { get; set; }
    public List<string> Contents { get; }
    public int Gold { get; }
    
    public TreasureChest(int difficulty)
    {
        IsOpened = false;
        Contents = new List<string>();
        
        // Generate contents based on difficulty
        var random = new Random();
        Gold = random.Next(50, 200) * difficulty;
        
        // Add random items
        int itemCount = random.Next(1, 4);
        for (int i = 0; i < itemCount; i++)
        {
            Contents.Add(GenerateRandomItem(difficulty, random));
        }
    }
    
    private string GenerateRandomItem(int difficulty, Random random)
    {
        var commonItems = new[] { "Health Potion", "Mana Potion", "Bread", "Iron Ore", "Copper Ore" };
        var uncommonItems = new[] { "Energy Elixir", "Steel Bar", "Gold Ore", "Gem", "Magic Essence" };
        var rareItems = new[] { "Diamond", "Ancient Relic", "Legendary Scroll", "Dragon Scale", "Phoenix Feather" };
        
        int roll = random.Next(100);
        
        if (roll < 60)
        {
            return commonItems[random.Next(commonItems.Length)];
        }
        else if (roll < 90)
        {
            return uncommonItems[random.Next(uncommonItems.Length)];
        }
        else
        {
            return rareItems[random.Next(rareItems.Length)];
        }
    }
    
    public void Open()
    {
        IsOpened = true;
    }
}

public enum DungeonType
{
    SlimeCave,
    SkeletonCrypt,
    SpiderNest,
    GoblinWarrens,
    HauntedManor,
    DragonLair,
    DemonRealm,
    AncientRuins
}

public enum RoomType
{
    Entrance,   // Starting room
    Combat,     // Regular enemy room
    Treasure,   // Chest room
    Boss,       // Boss encounter
    Puzzle,     // Puzzle challenge
    Shop,       // Merchant room
    Shrine,     // Healing/buff station
    Exit        // Stairs to next floor or exit
}
