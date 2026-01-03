using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Core.Systems;
using MoonBrookRidge.Characters.Player;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Map Menu with tabs for World Map, Waypoints, and Fast Travel
/// Opened with M key - consolidates map-related functionality
/// </summary>
public class MapMenu : TabbedMenu
{
    private WaypointSystem _waypointSystem;
    private TimeSystem _timeSystem;
    private PlayerCharacter _player;
    private Minimap _minimap;
    private List<Waypoint> _unlockedWaypoints;
    private int _selectedWaypointIndex;
    private bool _confirmationMode;
    private Waypoint _selectedWaypoint;
    private string _statusMessage;
    private float _messageTimer;
    
    // Tab indices
    private const int TAB_WORLDMAP = 0;
    private const int TAB_WAYPOINTS = 1;
    private const int TAB_FASTTRAVEL = 2;
    
    // Waypoint display
    private const int WAYPOINT_ITEM_HEIGHT = 70;
    private const float MESSAGE_DURATION = 2f;
    
    // Event raised when fast travel occurs
    public event Action<Vector2, int> OnFastTravel; // destination, cost
    
    public MapMenu(WaypointSystem waypointSystem, TimeSystem timeSystem, PlayerCharacter player, Minimap minimap)
    {
        _waypointSystem = waypointSystem;
        _timeSystem = timeSystem;
        _player = player;
        _minimap = minimap;
        _unlockedWaypoints = new List<Waypoint>();
        _selectedWaypointIndex = 0;
        _confirmationMode = false;
        _statusMessage = "";
        _messageTimer = 0f;
        
        // Initialize tabs
        AddTab("World Map", "View the world map");
        AddTab("Waypoints", "View discovered locations");
        AddTab("Fast Travel", "Travel to waypoints");
    }
    
    public override void Show()
    {
        base.Show();
        _unlockedWaypoints = _waypointSystem.GetUnlockedWaypoints();
        _selectedWaypointIndex = 0;
        _statusMessage = "";
        _confirmationMode = false;
        _selectedWaypoint = null;
    }
    
    protected override void UpdateTabContent(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
    {
        // Update message timer
        if (_messageTimer > 0)
        {
            _messageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        
        switch (_selectedTabIndex)
        {
            case TAB_WORLDMAP:
                UpdateWorldMapTab(keyboardState, mouseState);
                break;
            case TAB_WAYPOINTS:
                UpdateWaypointsTab(keyboardState, mouseState);
                break;
            case TAB_FASTTRAVEL:
                UpdateFastTravelTab(keyboardState, mouseState);
                break;
        }
    }
    
    private void UpdateWorldMapTab(KeyboardState keyboardState, MouseState mouseState)
    {
        // World map view - shows minimap in larger view
        // Toggle minimap visibility with Space
        if (keyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
        {
            _minimap.IsVisible = !_minimap.IsVisible;
        }
    }
    
    private void UpdateWaypointsTab(KeyboardState keyboardState, MouseState mouseState)
    {
        // Waypoints list - show all discovered waypoints
        if (_unlockedWaypoints.Count == 0) return;
        
        // Navigate waypoints with Up/Down
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedWaypointIndex--;
            if (_selectedWaypointIndex < 0)
                _selectedWaypointIndex = _unlockedWaypoints.Count - 1;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedWaypointIndex++;
            if (_selectedWaypointIndex >= _unlockedWaypoints.Count)
                _selectedWaypointIndex = 0;
        }
        
        // Click on waypoint to travel (confirmation)
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (_selectedWaypointIndex >= 0 && _selectedWaypointIndex < _unlockedWaypoints.Count)
            {
                _selectedWaypoint = _unlockedWaypoints[_selectedWaypointIndex];
                _confirmationMode = true;
            }
        }
    }
    
    private void UpdateFastTravelTab(KeyboardState keyboardState, MouseState mouseState)
    {
        if (_unlockedWaypoints.Count == 0)
        {
            _statusMessage = "No waypoints discovered yet. Explore the world!";
            _messageTimer = MESSAGE_DURATION;
            return;
        }
        
        // Confirmation mode
        if (_confirmationMode)
        {
            // Confirm travel with Enter/X
            if ((keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter)) ||
                (keyboardState.IsKeyDown(Keys.X) && !_previousKeyboardState.IsKeyDown(Keys.X)))
            {
                AttemptFastTravel();
            }
            
            // Cancel with Escape
            if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
            {
                _confirmationMode = false;
                _selectedWaypoint = null;
            }
            return;
        }
        
        // Navigate waypoints with Up/Down
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedWaypointIndex--;
            if (_selectedWaypointIndex < 0)
                _selectedWaypointIndex = _unlockedWaypoints.Count - 1;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedWaypointIndex++;
            if (_selectedWaypointIndex >= _unlockedWaypoints.Count)
                _selectedWaypointIndex = 0;
        }
        
        // Select waypoint for travel with Enter
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (_selectedWaypointIndex >= 0 && _selectedWaypointIndex < _unlockedWaypoints.Count)
            {
                _selectedWaypoint = _unlockedWaypoints[_selectedWaypointIndex];
                _confirmationMode = true;
            }
        }
    }
    
    private void AttemptFastTravel()
    {
        if (_selectedWaypoint == null) return;
        
        int travelCost = CalculateTravelCost(_player.Position, _selectedWaypoint.Position);
        
        // Check if player can afford travel
        if (_player.Money < travelCost)
        {
            _statusMessage = $"Not enough gold! Need {travelCost}g";
            _messageTimer = MESSAGE_DURATION;
            _confirmationMode = false;
            return;
        }
        
        // Perform fast travel
        _player.SpendMoney(travelCost);
        _timeSystem.AdvanceTime(1f); // 1 hour passes
        
        // Raise event to move player
        OnFastTravel?.Invoke(_selectedWaypoint.Position, travelCost);
        
        _statusMessage = $"Traveled to {_selectedWaypoint.Name}";
        _messageTimer = MESSAGE_DURATION;
        _confirmationMode = false;
        _selectedWaypoint = null;
        
        // Close menu after travel
        Hide();
    }
    
    private int CalculateTravelCost(Vector2 from, Vector2 to)
    {
        float distance = Vector2.Distance(from, to);
        // Base cost of 50g + 1g per tile distance
        return 50 + (int)(distance / 16f); // Divide by tile size
    }
    
    protected override void DrawTabContent(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        int contentY = y + PADDING;
        int contentHeight = height - PADDING * 2;
        
        switch (_selectedTabIndex)
        {
            case TAB_WORLDMAP:
                DrawWorldMapTab(spriteBatch, font, x, contentY, width, contentHeight);
                break;
            case TAB_WAYPOINTS:
                DrawWaypointsTab(spriteBatch, font, x, contentY, width, contentHeight);
                break;
            case TAB_FASTTRAVEL:
                DrawFastTravelTab(spriteBatch, font, x, contentY, width, contentHeight);
                break;
        }
        
        // Draw status message if active
        if (_messageTimer > 0 && !string.IsNullOrEmpty(_statusMessage))
        {
            Vector2 messageSize = font.MeasureString(_statusMessage);
            Vector2 messagePos = new Vector2(
                x + (width - messageSize.X) / 2,
                y + height - PADDING - messageSize.Y
            );
            
            spriteBatch.DrawString(font, _statusMessage, messagePos, Color.Yellow);
        }
    }
    
    private void DrawWorldMapTab(SpriteBatch spriteBatch, SpriteFont font, int menuX, int contentY, int width, int contentHeight)
    {
        // Draw instructions
        string instructions = "World Map View\n\nPress SPACE to toggle minimap\nExplore the world to discover new areas";
        Vector2 textPos = new Vector2(menuX + PADDING, contentY);
        spriteBatch.DrawString(font, instructions, textPos, Color.White);
        
        // Draw minimap in center if visible
        if (_minimap.IsVisible)
        {
            string minimapText = "[Minimap displayed in top-right corner]";
            Vector2 minimapTextSize = font.MeasureString(minimapText);
            Vector2 minimapTextPos = new Vector2(
                menuX + (width - minimapTextSize.X) / 2,
                contentY + contentHeight / 2
            );
            spriteBatch.DrawString(font, minimapText, minimapTextPos, Color.Gray);
        }
    }
    
    private void DrawWaypointsTab(SpriteBatch spriteBatch, SpriteFont font, int menuX, int contentY, int width, int contentHeight)
    {
        if (_unlockedWaypoints.Count == 0)
        {
            string noWaypoints = "No waypoints discovered yet.\nExplore the world to unlock fast travel locations!";
            Vector2 textSize = font.MeasureString(noWaypoints);
            Vector2 textPos = new Vector2(
                menuX + (width - textSize.X) / 2,
                contentY + contentHeight / 2 - textSize.Y / 2
            );
            spriteBatch.DrawString(font, noWaypoints, textPos, Color.Gray);
            return;
        }
        
        // Draw waypoint list
        for (int i = 0; i < _unlockedWaypoints.Count; i++)
        {
            var waypoint = _unlockedWaypoints[i];
            int itemY = contentY + i * WAYPOINT_ITEM_HEIGHT;
            
            // Skip if off-screen
            if (itemY + WAYPOINT_ITEM_HEIGHT < contentY || itemY > contentY + contentHeight)
                continue;
            
            // Highlight selected waypoint
            bool isSelected = i == _selectedWaypointIndex;
            Color bgColor = isSelected ? new Color(100, 80, 60) : new Color(40, 35, 30);
            
            // Draw background
            Rectangle itemRect = new Rectangle(
                menuX + PADDING,
                itemY,
                width - PADDING * 2,
                WAYPOINT_ITEM_HEIGHT - 5
            );
            spriteBatch.Draw(_pixelTexture, itemRect, bgColor);
            
            // Draw waypoint info
            Vector2 namePos = new Vector2(menuX + PADDING * 2, itemY + 5);
            Vector2 descPos = new Vector2(menuX + PADDING * 2, itemY + 30);
            Vector2 typePos = new Vector2(menuX + PADDING * 2, itemY + 50);
            
            spriteBatch.DrawString(font, waypoint.Name, namePos, Color.White);
            spriteBatch.DrawString(font, waypoint.Description, descPos, Color.LightGray);
            spriteBatch.DrawString(font, $"Type: {waypoint.Type}", typePos, Color.Gray);
        }
        
        // Draw instructions
        string instructions = "↑↓ Navigate | ENTER Select | ESC Close";
        Vector2 instrSize = font.MeasureString(instructions);
        Vector2 instrPos = new Vector2(
            menuX + (width - instrSize.X) / 2,
            contentY + contentHeight - 30
        );
        spriteBatch.DrawString(font, instructions, instrPos, Color.Yellow);
    }
    
    private void DrawFastTravelTab(SpriteBatch spriteBatch, SpriteFont font, int menuX, int contentY, int width, int contentHeight)
    {
        if (_unlockedWaypoints.Count == 0)
        {
            string noWaypoints = "No waypoints discovered yet.\nExplore the world to unlock fast travel locations!";
            Vector2 textSize = font.MeasureString(noWaypoints);
            Vector2 textPos = new Vector2(
                menuX + (width - textSize.X) / 2,
                contentY + contentHeight / 2 - textSize.Y / 2
            );
            spriteBatch.DrawString(font, noWaypoints, textPos, Color.Gray);
            return;
        }
        
        // Confirmation mode
        if (_confirmationMode && _selectedWaypoint != null)
        {
            int cost = CalculateTravelCost(_player.Position, _selectedWaypoint.Position);
            bool canAfford = _player.Money >= cost;
            
            string confirmText = $"Travel to {_selectedWaypoint.Name}?\n\n" +
                               $"Cost: {cost}g (Your money: {_player.Money}g)\n" +
                               $"Time: 1 hour will pass\n\n" +
                               (canAfford ? "Press ENTER to confirm | ESC to cancel" : "Not enough gold!");
            
            Vector2 textSize = font.MeasureString(confirmText);
            Vector2 textPos = new Vector2(
                menuX + (width - textSize.X) / 2,
                contentY + contentHeight / 2 - textSize.Y / 2
            );
            
            Color textColor = canAfford ? Color.White : Color.Red;
            spriteBatch.DrawString(font, confirmText, textPos, textColor);
            return;
        }
        
        // Draw waypoint list
        for (int i = 0; i < _unlockedWaypoints.Count; i++)
        {
            var waypoint = _unlockedWaypoints[i];
            int itemY = contentY + i * WAYPOINT_ITEM_HEIGHT;
            
            // Skip if off-screen
            if (itemY + WAYPOINT_ITEM_HEIGHT < contentY || itemY > contentY + contentHeight)
                continue;
            
            // Highlight selected waypoint
            bool isSelected = i == _selectedWaypointIndex;
            Color bgColor = isSelected ? new Color(100, 80, 60) : new Color(40, 35, 30);
            
            // Draw background
            Rectangle itemRect = new Rectangle(
                menuX + PADDING,
                itemY,
                width - PADDING * 2,
                WAYPOINT_ITEM_HEIGHT - 5
            );
            spriteBatch.Draw(_pixelTexture, itemRect, bgColor);
            
            // Calculate travel cost
            int cost = CalculateTravelCost(_player.Position, waypoint.Position);
            bool canAfford = _player.Money >= cost;
            
            // Draw waypoint info
            Vector2 namePos = new Vector2(menuX + PADDING * 2, itemY + 5);
            Vector2 descPos = new Vector2(menuX + PADDING * 2, itemY + 30);
            Vector2 costPos = new Vector2(menuX + PADDING * 2, itemY + 50);
            
            spriteBatch.DrawString(font, waypoint.Name, namePos, Color.White);
            spriteBatch.DrawString(font, waypoint.Description, descPos, Color.LightGray);
            
            string costText = $"Cost: {cost}g | Time: 1hr";
            Color costColor = canAfford ? Color.Gold : Color.Red;
            spriteBatch.DrawString(font, costText, costPos, costColor);
        }
        
        // Draw instructions
        string instructions = "↑↓ Navigate | ENTER Travel | ESC Close";
        Vector2 instrSize = font.MeasureString(instructions);
        Vector2 instrPos = new Vector2(
            menuX + (width - instrSize.X) / 2,
            contentY + contentHeight - 30
        );
        spriteBatch.DrawString(font, instructions, instrPos, Color.Yellow);
    }
    
    public void Initialize(GraphicsDevice graphicsDevice)
    {
        _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });
    }
    
    public new bool IsActive => _isActive;
}
