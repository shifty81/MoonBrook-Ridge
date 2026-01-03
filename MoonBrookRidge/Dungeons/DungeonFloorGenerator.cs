using System.Collections.Generic;
using MoonBrookRidge.Core.Scenes;

namespace MoonBrookRidge.Dungeons;

/// <summary>
/// Generates dungeon floors on demand - supports minimum 100 floors per dungeon
/// Creates floors lazily as player progresses deeper
/// </summary>
public class DungeonFloorGenerator
{
    private string _dungeonName;
    private Dictionary<int, DungeonScene> _generatedFloors;
    private int _maxFloorGenerated;
    private const int MAX_FLOORS = 150; // Support up to 150 floors
    
    public string DungeonName => _dungeonName;
    public int MaxFloorGenerated => _maxFloorGenerated;
    
    public DungeonFloorGenerator(string dungeonName)
    {
        _dungeonName = dungeonName;
        _generatedFloors = new Dictionary<int, DungeonScene>();
        _maxFloorGenerated = 0;
    }
    
    /// <summary>
    /// Get or generate a dungeon floor
    /// </summary>
    public DungeonScene GetFloor(int floorNumber)
    {
        if (floorNumber < 1 || floorNumber > MAX_FLOORS)
        {
            return null;
        }
        
        // Return cached floor if already generated
        if (_generatedFloors.TryGetValue(floorNumber, out var existingFloor))
        {
            return existingFloor;
        }
        
        // Generate new floor
        var newFloor = GenerateFloor(floorNumber);
        _generatedFloors[floorNumber] = newFloor;
        
        if (floorNumber > _maxFloorGenerated)
        {
            _maxFloorGenerated = floorNumber;
        }
        
        return newFloor;
    }
    
    /// <summary>
    /// Generate a single dungeon floor
    /// </summary>
    private DungeonScene GenerateFloor(int floorNumber)
    {
        // Create dungeon scene for this floor
        var dungeonFloor = new DungeonScene(_dungeonName, floorNumber);
        dungeonFloor.Initialize();
        dungeonFloor.LoadContent();
        
        return dungeonFloor;
    }
    
    /// <summary>
    /// Pre-generate a range of floors (useful for smoother gameplay)
    /// </summary>
    public void PreGenerateFloors(int startFloor, int endFloor)
    {
        for (int floor = startFloor; floor <= endFloor && floor <= MAX_FLOORS; floor++)
        {
            if (!_generatedFloors.ContainsKey(floor))
            {
                GetFloor(floor);
            }
        }
    }
    
    /// <summary>
    /// Check if a floor has been generated
    /// </summary>
    public bool IsFloorGenerated(int floorNumber)
    {
        return _generatedFloors.ContainsKey(floorNumber);
    }
    
    /// <summary>
    /// Get all generated floors
    /// </summary>
    public IEnumerable<DungeonScene> GetAllGeneratedFloors()
    {
        return _generatedFloors.Values;
    }
    
    /// <summary>
    /// Clear generated floors to save memory (keep only recent floors)
    /// </summary>
    public void ClearDistantFloors(int currentFloor, int keepRange = 5)
    {
        List<int> floorsToRemove = new List<int>();
        
        foreach (var floorNum in _generatedFloors.Keys)
        {
            // Keep floors within range of current floor
            if (System.Math.Abs(floorNum - currentFloor) > keepRange)
            {
                floorsToRemove.Add(floorNum);
            }
        }
        
        foreach (var floorNum in floorsToRemove)
        {
            _generatedFloors.Remove(floorNum);
        }
    }
    
    /// <summary>
    /// Get total number of generated floors
    /// </summary>
    public int GetGeneratedFloorCount()
    {
        return _generatedFloors.Count;
    }
}
