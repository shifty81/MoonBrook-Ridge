using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Mining;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Automation device placement menu - similar to BuildingMenu
/// Allows player to place drills, conveyors, and other automation devices
/// </summary>
public class AutomationPlacementMenu
{
    private bool _isActive;
    private AutomationSystem _automationSystem;
    private UndergroundCraftingSystem _craftingSystem;
    private InventorySystem _inventory;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture;
    
    // Device selection
    private List<DeviceDefinition> _devices;
    private int _selectedDeviceIndex;
    
    // Placement mode
    private bool _isPlacementMode;
    private AutomationDeviceType _selectedDeviceType;
    private Vector2 _placementPosition;
    private ConveyorDirection _selectedDirection;
    
    // UI Layout
    private const int MENU_WIDTH = 700;
    private const int MENU_HEIGHT = 550;
    private const int DEVICE_HEIGHT = 90;
    private const int PADDING = 20;
    
    // Colors
    private readonly Color BACKGROUND_COLOR = new Color(20, 20, 25, 230);
    private readonly Color VALID_COLOR = new Color(50, 200, 100, 128);
    private readonly Color INVALID_COLOR = new Color(200, 50, 50, 128);
    
    public bool IsActive => _isActive;
    public bool IsPlacementMode => _isPlacementMode;
    public AutomationDeviceType SelectedDeviceType => _selectedDeviceType;
    public Vector2 PlacementPosition => _placementPosition;
    public ConveyorDirection SelectedDirection => _selectedDirection;
    
    public AutomationPlacementMenu(AutomationSystem automationSystem, 
                                   UndergroundCraftingSystem craftingSystem,
                                   InventorySystem inventory)
    {
        _automationSystem = automationSystem;
        _craftingSystem = craftingSystem;
        _inventory = inventory;
        _isActive = false;
        _isPlacementMode = false;
        _selectedDeviceIndex = 0;
        _selectedDirection = ConveyorDirection.Right;
        
        InitializeDeviceDefinitions();
    }
    
    public void Initialize(Texture2D pixelTexture)
    {
        _pixelTexture = pixelTexture;
    }
    
    private void InitializeDeviceDefinitions()
    {
        _devices = new List<DeviceDefinition>
        {
            new DeviceDefinition(
                AutomationDeviceType.Drill,
                "Mining Drill",
                "Automatically harvests resources from ore nodes. Place next to ore.",
                new Dictionary<string, int> { { "Iron Ore", 20 }, { "Scarlet Ore", 10 } },
                100
            ),
            new DeviceDefinition(
                AutomationDeviceType.ConveyorBelt,
                "Conveyor Belt",
                "Transports items between devices. Chain multiple together.",
                new Dictionary<string, int> { { "Iron Ore", 5 }, { "Copper Ore", 10 } },
                25
            ),
            new DeviceDefinition(
                AutomationDeviceType.Chest,
                "Storage Chest",
                "Stores up to 100 items. Can receive from conveyors.",
                new Dictionary<string, int> { { "Wood", 30 }, { "Iron Ore", 10 } },
                50
            ),
            new DeviceDefinition(
                AutomationDeviceType.Smelter,
                "Auto-Smelter",
                "Automatically smelts ores to bars. Connect with conveyors.",
                new Dictionary<string, int> { { "Stone", 40 }, { "Iron Ore", 20 }, { "Scarlet Ore", 5 } },
                150
            ),
            new DeviceDefinition(
                AutomationDeviceType.RoboticArm,
                "Robotic Arm",
                "Picks up and places items from conveyors to chests.",
                new Dictionary<string, int> { { "Iron Ore", 15 }, { "Scarlet Ore", 10 } },
                120
            ),
            new DeviceDefinition(
                AutomationDeviceType.Sorter,
                "Item Sorter",
                "Filters items by type to different output directions.",
                new Dictionary<string, int> { { "Iron Ore", 10 }, { "Octarine Ore", 5 } },
                80
            )
        };
    }
    
    public void Show()
    {
        // Check if automation is unlocked (Scarlet tier)
        if (!_craftingSystem.IsAutomationUnlocked())
        {
            // Player hasn't reached Scarlet tier yet
            return;
        }
        
        _isActive = true;
        _selectedDeviceIndex = 0;
        _isPlacementMode = false;
    }
    
    public void Hide()
    {
        _isActive = false;
        _isPlacementMode = false;
    }
    
    public void Toggle()
    {
        if (_isActive)
            Hide();
        else
            Show();
    }
    
    public void EnterPlacementMode(AutomationDeviceType deviceType)
    {
        _isPlacementMode = true;
        _selectedDeviceType = deviceType;
        _placementPosition = Vector2.Zero;
        _selectedDirection = ConveyorDirection.Right;
        _isActive = false; // Hide menu during placement
    }
    
    public void ExitPlacementMode()
    {
        _isPlacementMode = false;
        _isActive = true; // Show menu again
    }
    
    public void Update(GameTime gameTime, Vector2 playerPosition)
    {
        if (_isPlacementMode) return; // Placement handled by GameplayState
        if (!_isActive) return;
        
        KeyboardState keyboardState = Keyboard.GetState();
        
        // Navigation
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedDeviceIndex--;
            if (_selectedDeviceIndex < 0)
                _selectedDeviceIndex = _devices.Count - 1;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedDeviceIndex++;
            if (_selectedDeviceIndex >= _devices.Count)
                _selectedDeviceIndex = 0;
        }
        
        // Select device for placement
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (_selectedDeviceIndex >= 0 && _selectedDeviceIndex < _devices.Count)
            {
                DeviceDefinition device = _devices[_selectedDeviceIndex];
                
                // Check if player can afford
                if (CanAffordDevice(device))
                {
                    EnterPlacementMode(device.Type);
                }
            }
        }
        
        // Close menu
        if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            Hide();
        }
        
        _previousKeyboardState = keyboardState;
    }
    
    private bool CanAffordDevice(DeviceDefinition device)
    {
        foreach (var material in device.MaterialCosts)
        {
            if (_inventory.GetItemCount(material.Key) < material.Value)
            {
                return false;
            }
        }
        return true;
    }
    
    public bool TryPlaceDevice(Vector2 position)
    {
        if (!_isPlacementMode) return false;
        
        DeviceDefinition device = _devices.Find(d => d.Type == _selectedDeviceType);
        if (device == null) return false;
        
        // Check affordability again
        if (!CanAffordDevice(device))
            return false;
        
        // Consume materials
        foreach (var material in device.MaterialCosts)
        {
            _inventory.RemoveItem(material.Key, material.Value);
        }
        
        // Place device
        AutomationDevice placedDevice = _automationSystem.PlaceDevice(_selectedDeviceType, position);
        
        // Set direction for conveyors
        if (_selectedDeviceType == AutomationDeviceType.ConveyorBelt)
        {
            placedDevice.Direction = _selectedDirection;
        }
        
        return true;
    }
    
    public void RotateDirection()
    {
        _selectedDirection = _selectedDirection switch
        {
            ConveyorDirection.Right => ConveyorDirection.Down,
            ConveyorDirection.Down => ConveyorDirection.Left,
            ConveyorDirection.Left => ConveyorDirection.Up,
            ConveyorDirection.Up => ConveyorDirection.Right,
            _ => ConveyorDirection.Right
        };
    }
    
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
    {
        if (!_isActive) return;
        
        int screenWidth = graphicsDevice.Viewport.Width;
        int screenHeight = graphicsDevice.Viewport.Height;
        
        int menuX = (screenWidth - MENU_WIDTH) / 2;
        int menuY = (screenHeight - MENU_HEIGHT) / 2;
        
        // Draw background overlay
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(0, 0, screenWidth, screenHeight),
            new Color(0, 0, 0, 150));
        
        // Draw main menu background
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(menuX, menuY, MENU_WIDTH, MENU_HEIGHT),
            BACKGROUND_COLOR);
        
        // Draw title
        string title = "Automation Devices";
        if (!_craftingSystem.IsAutomationUnlocked())
        {
            title = "Automation Locked - Reach Scarlet Tier!";
        }
        
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePosition = new Vector2(
            menuX + (MENU_WIDTH - titleSize.X) / 2,
            menuY + PADDING
        );
        spriteBatch.DrawString(font, title, titlePosition, Color.White);
        
        // Draw devices list
        int deviceY = menuY + PADDING * 2 + (int)titleSize.Y;
        
        for (int i = 0; i < _devices.Count; i++)
        {
            DeviceDefinition device = _devices[i];
            bool isSelected = i == _selectedDeviceIndex;
            bool canAfford = CanAffordDevice(device);
            
            // Device background
            Color bgColor = isSelected ? new Color(80, 120, 200, 255) : new Color(40, 40, 50, 255);
            spriteBatch.Draw(_pixelTexture,
                new Rectangle(menuX + PADDING, deviceY, MENU_WIDTH - PADDING * 2, DEVICE_HEIGHT),
                bgColor);
            
            // Device name
            spriteBatch.DrawString(font, device.Name,
                new Vector2(menuX + PADDING * 2, deviceY + 10),
                canAfford ? Color.White : Color.Gray);
            
            // Device description
            spriteBatch.DrawString(font, device.Description,
                new Vector2(menuX + PADDING * 2, deviceY + 35),
                new Color(180, 180, 180));
            
            // Materials cost
            string costText = $"Cost: {string.Join(", ", device.GetCostString())}";
            spriteBatch.DrawString(font, costText,
                new Vector2(menuX + PADDING * 2, deviceY + 60),
                canAfford ? Color.LightGreen : Color.Red);
            
            deviceY += DEVICE_HEIGHT + 10;
        }
        
        // Draw controls
        string controls = "▲▼ Navigate | ENTER Place | ESC Close";
        spriteBatch.DrawString(font, controls,
            new Vector2(menuX + PADDING, menuY + MENU_HEIGHT - 30),
            new Color(200, 200, 200));
    }
    
    public void DrawPlacementPreview(SpriteBatch spriteBatch, Vector2 worldPosition, Camera2D camera)
    {
        if (!_isPlacementMode) return;
        
        // Calculate screen position (world position relative to camera)
        Vector2 screenPos = (worldPosition - camera.Position) * camera.Zoom;
        
        // Draw placement preview (simple colored square for now)
        Rectangle previewRect = new Rectangle(
            (int)screenPos.X,
            (int)screenPos.Y,
            16, // Tile size
            16
        );
        
        bool isValid = true; // TODO: Add validation logic
        Color previewColor = isValid ? VALID_COLOR : INVALID_COLOR;
        
        spriteBatch.Draw(_pixelTexture, previewRect, previewColor);
        
        // Draw direction indicator for conveyors
        if (_selectedDeviceType == AutomationDeviceType.ConveyorBelt)
        {
            string directionArrow = _selectedDirection switch
            {
                ConveyorDirection.Right => "→",
                ConveyorDirection.Down => "↓",
                ConveyorDirection.Left => "←",
                ConveyorDirection.Up => "↑",
                _ => "?"
            };
            
            // TODO: Draw arrow on preview
        }
    }
}

/// <summary>
/// Definition of an automation device with costs and properties
/// </summary>
public class DeviceDefinition
{
    public AutomationDeviceType Type { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public Dictionary<string, int> MaterialCosts { get; set; }
    public int GoldCost { get; set; }
    
    public DeviceDefinition(AutomationDeviceType type, string name, string description,
                           Dictionary<string, int> materials, int goldCost)
    {
        Type = type;
        Name = name;
        Description = description;
        MaterialCosts = materials;
        GoldCost = goldCost;
    }
    
    public List<string> GetCostString()
    {
        var costs = new List<string>();
        if (GoldCost > 0)
        {
            costs.Add($"{GoldCost}g");
        }
        foreach (var material in MaterialCosts)
        {
            costs.Add($"{material.Value} {material.Key}");
        }
        return costs;
    }
}
