using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.World.Mining;

/// <summary>
/// Types of automation devices (Core Keeper inspired)
/// </summary>
public enum AutomationDeviceType
{
    Drill,          // Auto-harvests resources from ore nodes
    ConveyorBelt,   // Transports items between locations
    RoboticArm,     // Picks up and places items
    Chest,          // Storage container
    Sorter,         // Filters items by type
    Smelter         // Auto-smelts ores
}

/// <summary>
/// Direction for conveyor belts and item flow
/// </summary>
public enum ConveyorDirection
{
    Up,
    Down,
    Left,
    Right
}

/// <summary>
/// Represents an automation device in the underground
/// </summary>
public class AutomationDevice
{
    public string Id { get; set; }
    public AutomationDeviceType Type { get; set; }
    public Vector2 Position { get; set; }
    public bool IsActive { get; set; }
    public ConveyorDirection Direction { get; set; }
    
    // For drills
    public Vector2? TargetResourcePosition { get; set; }
    public float HarvestTimer { get; set; }
    public float HarvestInterval { get; set; } // Seconds between harvests
    
    // For storage
    public List<string> StoredItems { get; set; }
    public int StorageCapacity { get; set; }
    
    public AutomationDevice(AutomationDeviceType type, Vector2 position)
    {
        Id = Guid.NewGuid().ToString();
        Type = type;
        Position = position;
        IsActive = true;
        Direction = ConveyorDirection.Right;
        HarvestTimer = 0f;
        HarvestInterval = 5f; // Default 5 seconds
        StoredItems = new List<string>();
        StorageCapacity = 100;
    }
}

/// <summary>
/// Item traveling on a conveyor belt
/// </summary>
public class ConveyorItem
{
    public string ItemId { get; set; }
    public Vector2 Position { get; set; }
    public Vector2 TargetPosition { get; set; }
    public float MoveSpeed { get; set; }
    
    public ConveyorItem(string itemId, Vector2 startPosition, Vector2 targetPosition)
    {
        ItemId = itemId;
        Position = startPosition;
        TargetPosition = targetPosition;
        MoveSpeed = 1f; // Tiles per second
    }
}

/// <summary>
/// Core Keeper-inspired automation system
/// Manages drills, conveyor belts, and automated resource collection
/// </summary>
public class AutomationSystem
{
    private List<AutomationDevice> _devices;
    private List<ConveyorItem> _conveyorItems;
    private const float TILE_SIZE = 16f;
    
    public event Action<AutomationDevice, string>? OnItemHarvested; // Device, item ID
    public event Action<AutomationDevice>? OnDevicePlaced;
    
    public AutomationSystem()
    {
        _devices = new List<AutomationDevice>();
        _conveyorItems = new List<ConveyorItem>();
    }
    
    /// <summary>
    /// Place an automation device
    /// </summary>
    public AutomationDevice PlaceDevice(AutomationDeviceType type, Vector2 position)
    {
        var device = new AutomationDevice(type, position);
        _devices.Add(device);
        OnDevicePlaced?.Invoke(device);
        return device;
    }
    
    /// <summary>
    /// Remove a device
    /// </summary>
    public bool RemoveDevice(AutomationDevice device)
    {
        return _devices.Remove(device);
    }
    
    /// <summary>
    /// Update all automation devices
    /// </summary>
    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Update drills
        foreach (var device in _devices)
        {
            if (!device.IsActive) continue;
            
            switch (device.Type)
            {
                case AutomationDeviceType.Drill:
                    UpdateDrill(device, deltaTime);
                    break;
                    
                case AutomationDeviceType.Smelter:
                    UpdateSmelter(device, deltaTime);
                    break;
            }
        }
        
        // Update conveyor items
        UpdateConveyorItems(deltaTime);
    }
    
    /// <summary>
    /// Update drill harvesting
    /// </summary>
    private void UpdateDrill(AutomationDevice drill, float deltaTime)
    {
        if (drill.TargetResourcePosition == null)
            return;
        
        drill.HarvestTimer += deltaTime;
        
        if (drill.HarvestTimer >= drill.HarvestInterval)
        {
            drill.HarvestTimer = 0f;
            
            // Harvest resource
            string harvestedItem = GetResourceAtPosition(drill.TargetResourcePosition.Value);
            
            if (!string.IsNullOrEmpty(harvestedItem))
            {
                // Try to output to adjacent conveyor or chest
                OutputItem(drill, harvestedItem);
                OnItemHarvested?.Invoke(drill, harvestedItem);
            }
        }
    }
    
    /// <summary>
    /// Update auto-smelter
    /// </summary>
    private void UpdateSmelter(AutomationDevice smelter, float deltaTime)
    {
        // Check for ore in storage
        if (smelter.StoredItems.Count > 0)
        {
            smelter.HarvestTimer += deltaTime;
            
            if (smelter.HarvestTimer >= smelter.HarvestInterval)
            {
                smelter.HarvestTimer = 0f;
                
                // Smelt first ore
                string ore = smelter.StoredItems[0];
                smelter.StoredItems.RemoveAt(0);
                
                string bar = ConvertOreToBar(ore);
                OutputItem(smelter, bar);
            }
        }
    }
    
    /// <summary>
    /// Output item to adjacent storage or conveyor
    /// </summary>
    private void OutputItem(AutomationDevice device, string itemId)
    {
        // Find adjacent devices
        var adjacentDevices = GetAdjacentDevices(device.Position);
        
        foreach (var adjacent in adjacentDevices)
        {
            if (adjacent.Type == AutomationDeviceType.ConveyorBelt)
            {
                // Add to conveyor
                var conveyorItem = new ConveyorItem(itemId, device.Position, adjacent.Position);
                _conveyorItems.Add(conveyorItem);
                return;
            }
            else if (adjacent.Type == AutomationDeviceType.Chest && adjacent.StoredItems.Count < adjacent.StorageCapacity)
            {
                // Add to chest
                adjacent.StoredItems.Add(itemId);
                return;
            }
        }
        
        // No output found, drop on ground (handled by game logic)
    }
    
    /// <summary>
    /// Update items moving on conveyor belts
    /// </summary>
    private void UpdateConveyorItems(float deltaTime)
    {
        var itemsToRemove = new List<ConveyorItem>();
        
        foreach (var item in _conveyorItems)
        {
            // Move towards target
            Vector2 direction = Vector2.Normalize(item.TargetPosition - item.Position);
            item.Position += direction * item.MoveSpeed * deltaTime * TILE_SIZE;
            
            // Check if reached target
            if (Vector2.Distance(item.Position, item.TargetPosition) < 1f)
            {
                // Find next conveyor or destination
                var nextDevice = GetDeviceAt(item.TargetPosition);
                
                if (nextDevice != null)
                {
                    if (nextDevice.Type == AutomationDeviceType.ConveyorBelt)
                    {
                        // Continue to next conveyor
                        item.TargetPosition = GetNextConveyorPosition(nextDevice);
                    }
                    else if (nextDevice.Type == AutomationDeviceType.Chest)
                    {
                        // Store in chest
                        if (nextDevice.StoredItems.Count < nextDevice.StorageCapacity)
                        {
                            nextDevice.StoredItems.Add(item.ItemId);
                        }
                        itemsToRemove.Add(item);
                    }
                    else
                    {
                        // Remove item (reached destination)
                        itemsToRemove.Add(item);
                    }
                }
                else
                {
                    // No device, drop item
                    itemsToRemove.Add(item);
                }
            }
        }
        
        // Remove completed items
        foreach (var item in itemsToRemove)
        {
            _conveyorItems.Remove(item);
        }
    }
    
    /// <summary>
    /// Get resource at a position (stub - would connect to actual world resources)
    /// </summary>
    private string GetResourceAtPosition(Vector2 position)
    {
        // This would check actual world resources
        // For now, return a sample resource
        return "CopperOre";
    }
    
    /// <summary>
    /// Convert ore to bar
    /// </summary>
    private string ConvertOreToBar(string ore)
    {
        return ore switch
        {
            "CopperOre" => "CopperBar",
            "TinOre" => "TinBar",
            "IronOre" => "IronBar",
            "ScarletOre" => "ScarletBar",
            "OctarineOre" => "OctarineBar",
            "GalaxiteOre" => "GalaxiteBar",
            "SolariteOre" => "SolariteBar",
            _ => ore
        };
    }
    
    /// <summary>
    /// Get adjacent devices (4 cardinal directions)
    /// </summary>
    private List<AutomationDevice> GetAdjacentDevices(Vector2 position)
    {
        var adjacent = new List<AutomationDevice>();
        
        Vector2[] offsets = 
        {
            new Vector2(0, -1),  // Up
            new Vector2(0, 1),   // Down
            new Vector2(-1, 0),  // Left
            new Vector2(1, 0)    // Right
        };
        
        foreach (var offset in offsets)
        {
            var checkPos = position + offset;
            var device = GetDeviceAt(checkPos);
            if (device != null)
            {
                adjacent.Add(device);
            }
        }
        
        return adjacent;
    }
    
    /// <summary>
    /// Get device at position
    /// </summary>
    private AutomationDevice? GetDeviceAt(Vector2 position)
    {
        return _devices.Find(d => Vector2.Distance(d.Position, position) < 0.5f);
    }
    
    /// <summary>
    /// Get next position for conveyor based on direction
    /// </summary>
    private Vector2 GetNextConveyorPosition(AutomationDevice conveyor)
    {
        return conveyor.Direction switch
        {
            ConveyorDirection.Up => conveyor.Position + new Vector2(0, -1),
            ConveyorDirection.Down => conveyor.Position + new Vector2(0, 1),
            ConveyorDirection.Left => conveyor.Position + new Vector2(-1, 0),
            ConveyorDirection.Right => conveyor.Position + new Vector2(1, 0),
            _ => conveyor.Position
        };
    }
    
    /// <summary>
    /// Set drill target resource
    /// </summary>
    public void SetDrillTarget(AutomationDevice drill, Vector2 resourcePosition)
    {
        if (drill.Type == AutomationDeviceType.Drill)
        {
            drill.TargetResourcePosition = resourcePosition;
        }
    }
    
    /// <summary>
    /// Get all devices
    /// </summary>
    public List<AutomationDevice> GetAllDevices()
    {
        return new List<AutomationDevice>(_devices);
    }
    
    /// <summary>
    /// Get conveyor items for rendering
    /// </summary>
    public List<ConveyorItem> GetConveyorItems()
    {
        return new List<ConveyorItem>(_conveyorItems);
    }
    
    /// <summary>
    /// Get automation statistics
    /// </summary>
    public (int drills, int conveyors, int chests) GetAutomationStats()
    {
        int drills = 0, conveyors = 0, chests = 0;
        
        foreach (var device in _devices)
        {
            switch (device.Type)
            {
                case AutomationDeviceType.Drill:
                    drills++;
                    break;
                case AutomationDeviceType.ConveyorBelt:
                    conveyors++;
                    break;
                case AutomationDeviceType.Chest:
                    chests++;
                    break;
            }
        }
        
        return (drills, conveyors, chests);
    }
}
