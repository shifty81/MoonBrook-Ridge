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
/// Fast Travel menu UI for navigating to discovered waypoints
/// </summary>
public class FastTravelMenu
{
    private bool _isActive;
    private WaypointSystem _waypointSystem;
    private TimeSystem _timeSystem;
    private PlayerCharacter _player;
    private List<Waypoint> _unlockedWaypoints;
    private int _selectedIndex;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture;
    private string _statusMessage;
    private float _messageTimer;
    private bool _confirmationMode;
    private Waypoint _selectedWaypoint;
    
    private const int MENU_WIDTH = 800;
    private const int MENU_HEIGHT = 650;
    private const int ITEM_HEIGHT = 70;
    private const int PADDING = 20;
    private const float MESSAGE_DURATION = 2f;
    
    // Event raised when fast travel occurs
    public event Action<Vector2, int> OnFastTravel; // destination, cost
    
    public FastTravelMenu(WaypointSystem waypointSystem, TimeSystem timeSystem, PlayerCharacter player)
    {
        _waypointSystem = waypointSystem;
        _timeSystem = timeSystem;
        _player = player;
        _unlockedWaypoints = new List<Waypoint>();
        _selectedIndex = 0;
        _isActive = false;
        _statusMessage = "";
        _messageTimer = 0f;
        _confirmationMode = false;
    }
    
    public void Show()
    {
        _isActive = true;
        _unlockedWaypoints = _waypointSystem.GetUnlockedWaypoints();
        _selectedIndex = 0;
        _statusMessage = "";
        _confirmationMode = false;
        _selectedWaypoint = null;
    }
    
    public void Hide()
    {
        _isActive = false;
        _confirmationMode = false;
    }
    
    public void Toggle()
    {
        if (_isActive)
            Hide();
        else
            Show();
    }
    
    public void Update(GameTime gameTime)
    {
        if (!_isActive) return;
        
        // Update message timer
        if (_messageTimer > 0)
        {
            _messageTimer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
        
        KeyboardState keyboardState = Keyboard.GetState();
        
        if (_confirmationMode)
        {
            HandleConfirmation(keyboardState);
        }
        else
        {
            HandleWaypointSelection(keyboardState);
        }
        
        _previousKeyboardState = keyboardState;
    }
    
    private void HandleWaypointSelection(KeyboardState keyboardState)
    {
        // Navigation
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedIndex--;
            if (_selectedIndex < 0)
                _selectedIndex = _unlockedWaypoints.Count - 1;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedIndex++;
            if (_selectedIndex >= _unlockedWaypoints.Count)
                _selectedIndex = 0;
        }
        
        // Select waypoint for travel
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (_unlockedWaypoints.Count > 0 && _selectedIndex < _unlockedWaypoints.Count)
            {
                _selectedWaypoint = _unlockedWaypoints[_selectedIndex];
                _confirmationMode = true;
                _statusMessage = "";
            }
        }
        
        // Close menu
        if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            Hide();
        }
    }
    
    private void HandleConfirmation(KeyboardState keyboardState)
    {
        // Confirm travel
        if (keyboardState.IsKeyDown(Keys.Y) && !_previousKeyboardState.IsKeyDown(Keys.Y))
        {
            PerformFastTravel();
        }
        
        // Cancel
        if (keyboardState.IsKeyDown(Keys.N) && !_previousKeyboardState.IsKeyDown(Keys.N) ||
            keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            _confirmationMode = false;
            _selectedWaypoint = null;
            _statusMessage = "";
        }
    }
    
    private void PerformFastTravel()
    {
        if (_selectedWaypoint == null) return;
        
        // Attempt to travel
        Vector2 destination;
        int cost;
        bool success = _waypointSystem.TravelToWaypoint(
            _selectedWaypoint.Id, 
            _player.Money, 
            out destination, 
            out cost
        );
        
        if (success)
        {
            // Deduct cost
            _player.SpendMoney(cost);
            
            // Advance time
            float timeCost = _waypointSystem.GetTimeCost();
            _timeSystem.AdvanceTime(timeCost);
            
            // Raise event for GameplayState to move player
            OnFastTravel?.Invoke(destination, cost);
            
            _statusMessage = $"Traveled to {_selectedWaypoint.Name}! Cost: ${cost}";
            _messageTimer = MESSAGE_DURATION;
            
            // Close menu after successful travel
            _confirmationMode = false;
            Hide();
        }
        else
        {
            // Travel failed
            if (_player.Money < cost)
            {
                _statusMessage = $"Not enough money! Need ${cost}, have ${_player.Money}";
            }
            else
            {
                _statusMessage = "Cannot travel to this location";
            }
            _messageTimer = MESSAGE_DURATION;
            _confirmationMode = false;
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
    {
        if (!_isActive) return;
        
        // Create pixel texture if needed
        if (_pixelTexture == null)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }
        
        int screenWidth = graphicsDevice.Viewport.Width;
        int screenHeight = graphicsDevice.Viewport.Height;
        
        // Calculate menu position (centered)
        int menuX = (screenWidth - MENU_WIDTH) / 2;
        int menuY = (screenHeight - MENU_HEIGHT) / 2;
        
        // Draw semi-transparent background overlay
        spriteBatch.Draw(_pixelTexture, new Rectangle(0, 0, screenWidth, screenHeight), 
                        Color.Black * 0.7f);
        
        // Draw menu background
        Rectangle menuRect = new Rectangle(menuX, menuY, MENU_WIDTH, MENU_HEIGHT);
        spriteBatch.Draw(_pixelTexture, menuRect, new Color(40, 40, 50));
        DrawBorder(spriteBatch, menuRect, Color.White, 3);
        
        if (_confirmationMode && _selectedWaypoint != null)
        {
            DrawConfirmation(spriteBatch, font, menuX, menuY);
        }
        else
        {
            DrawWaypointList(spriteBatch, font, menuX, menuY);
        }
    }
    
    private void DrawWaypointList(SpriteBatch spriteBatch, SpriteFont font, int menuX, int menuY)
    {
        // Draw title
        string title = "Fast Travel - Select Destination";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(menuX + (MENU_WIDTH - titleSize.X) / 2, menuY + PADDING);
        DrawTextWithShadow(spriteBatch, font, title, titlePos, Color.Gold);
        
        // Draw player money
        string moneyText = $"Money: ${_player.Money}";
        Vector2 moneyPos = new Vector2(menuX + MENU_WIDTH - PADDING - font.MeasureString(moneyText).X, 
                                       menuY + PADDING);
        DrawTextWithShadow(spriteBatch, font, moneyText, moneyPos, Color.LimeGreen);
        
        // Draw waypoint stats
        var stats = _waypointSystem.GetStats();
        string statsText = $"Discovered: {stats.UnlockedWaypoints}/{stats.TotalWaypoints}";
        Vector2 statsPos = new Vector2(menuX + PADDING, menuY + PADDING * 2.5f);
        DrawTextWithShadow(spriteBatch, font, statsText, statsPos, Color.Cyan);
        
        // Draw waypoints list
        int itemStartY = menuY + PADDING * 4;
        int maxVisibleItems = (MENU_HEIGHT - PADDING * 8) / ITEM_HEIGHT;
        
        if (_unlockedWaypoints.Count == 0)
        {
            string noWaypoints = "No waypoints discovered yet. Explore the world to find them!";
            Vector2 noWaypointsSize = font.MeasureString(noWaypoints);
            Vector2 noWaypointsPos = new Vector2(menuX + (MENU_WIDTH - noWaypointsSize.X) / 2, 
                                                 menuY + MENU_HEIGHT / 2);
            DrawTextWithShadow(spriteBatch, font, noWaypoints, noWaypointsPos, Color.Gray);
        }
        else
        {
            for (int i = 0; i < _unlockedWaypoints.Count && i < maxVisibleItems; i++)
            {
                Waypoint waypoint = _unlockedWaypoints[i];
                int itemY = itemStartY + i * ITEM_HEIGHT;
                bool isSelected = (i == _selectedIndex);
                
                // Calculate cost
                Vector2 dest;
                int cost;
                _waypointSystem.TravelToWaypoint(waypoint.Id, int.MaxValue, out dest, out cost);
                bool canAfford = _player.Money >= cost;
                
                // Draw waypoint background
                Rectangle itemRect = new Rectangle(menuX + PADDING, itemY, 
                                                  MENU_WIDTH - PADDING * 2, ITEM_HEIGHT - 5);
                Color bgColor = isSelected ? new Color(80, 80, 100) : new Color(50, 50, 60);
                spriteBatch.Draw(_pixelTexture, itemRect, bgColor);
                
                if (isSelected)
                {
                    DrawBorder(spriteBatch, itemRect, Color.Yellow, 2);
                }
                
                // Draw waypoint icon based on type
                string icon = GetWaypointIcon(waypoint.Type);
                Color iconColor = GetWaypointColor(waypoint.Type);
                Vector2 iconPos = new Vector2(itemRect.X + 10, itemRect.Y + 10);
                DrawTextWithShadow(spriteBatch, font, icon, iconPos, iconColor);
                
                // Draw waypoint name
                Color textColor = canAfford ? Color.White : Color.Gray;
                Vector2 namePos = new Vector2(itemRect.X + 50, itemRect.Y + 8);
                DrawTextWithShadow(spriteBatch, font, waypoint.Name, namePos, textColor);
                
                // Draw description
                Vector2 descPos = new Vector2(itemRect.X + 50, itemRect.Y + 30);
                DrawTextWithShadow(spriteBatch, font, waypoint.Description, descPos, Color.LightGray);
                
                // Draw cost
                string costText = cost == 0 ? "FREE" : $"${cost}";
                Vector2 costSize = font.MeasureString(costText);
                Vector2 costPos = new Vector2(itemRect.Right - costSize.X - 10, itemRect.Y + 15);
                Color costColor = cost == 0 ? Color.LimeGreen : (canAfford ? Color.Yellow : Color.Red);
                DrawTextWithShadow(spriteBatch, font, costText, costPos, costColor);
            }
        }
        
        // Draw status message
        if (_messageTimer > 0 && !string.IsNullOrEmpty(_statusMessage))
        {
            Vector2 messageSize = font.MeasureString(_statusMessage);
            Vector2 messagePos = new Vector2(menuX + (MENU_WIDTH - messageSize.X) / 2, 
                                            menuY + MENU_HEIGHT - PADDING * 3);
            DrawTextWithShadow(spriteBatch, font, _statusMessage, messagePos, Color.Yellow);
        }
        
        // Draw controls hint at bottom
        string hint = "Up/Down: Navigate | Enter: Select | Esc: Close";
        Vector2 hintSize = font.MeasureString(hint);
        Vector2 hintPos = new Vector2(menuX + (MENU_WIDTH - hintSize.X) / 2, 
                                     menuY + MENU_HEIGHT - PADDING - hintSize.Y);
        DrawTextWithShadow(spriteBatch, font, hint, hintPos, Color.LightGray);
    }
    
    private void DrawConfirmation(SpriteBatch spriteBatch, SpriteFont font, int menuX, int menuY)
    {
        // Calculate cost
        Vector2 dest;
        int cost;
        _waypointSystem.TravelToWaypoint(_selectedWaypoint.Id, int.MaxValue, out dest, out cost);
        
        // Draw confirmation box
        int confirmWidth = 600;
        int confirmHeight = 300;
        int confirmX = menuX + (MENU_WIDTH - confirmWidth) / 2;
        int confirmY = menuY + (MENU_HEIGHT - confirmHeight) / 2;
        
        Rectangle confirmRect = new Rectangle(confirmX, confirmY, confirmWidth, confirmHeight);
        spriteBatch.Draw(_pixelTexture, confirmRect, new Color(30, 30, 40));
        DrawBorder(spriteBatch, confirmRect, Color.Gold, 4);
        
        // Draw title
        string title = "Confirm Fast Travel";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(confirmX + (confirmWidth - titleSize.X) / 2, confirmY + PADDING);
        DrawTextWithShadow(spriteBatch, font, title, titlePos, Color.Gold);
        
        // Draw destination info
        int infoY = confirmY + PADDING * 3;
        string destText = $"Destination: {_selectedWaypoint.Name}";
        Vector2 destPos = new Vector2(confirmX + PADDING, infoY);
        DrawTextWithShadow(spriteBatch, font, destText, destPos, Color.White);
        
        infoY += 30;
        string descText = $"{_selectedWaypoint.Description}";
        Vector2 descPos = new Vector2(confirmX + PADDING, infoY);
        DrawTextWithShadow(spriteBatch, font, descText, descPos, Color.LightGray);
        
        infoY += 40;
        string costText = $"Travel Cost: {(cost == 0 ? "FREE" : $"${cost}")}";
        Vector2 costPos = new Vector2(confirmX + PADDING, infoY);
        Color costColor = cost == 0 ? Color.LimeGreen : (_player.Money >= cost ? Color.Yellow : Color.Red);
        DrawTextWithShadow(spriteBatch, font, costText, costPos, costColor);
        
        infoY += 30;
        float timeCost = _waypointSystem.GetTimeCost();
        string timeText = $"Time Cost: {timeCost:F1} hour{(timeCost != 1 ? "s" : "")}";
        Vector2 timePos = new Vector2(confirmX + PADDING, infoY);
        DrawTextWithShadow(spriteBatch, font, timeText, timePos, Color.Cyan);
        
        // Draw player money
        infoY += 40;
        string moneyText = $"Your Money: ${_player.Money}";
        Vector2 moneyPos = new Vector2(confirmX + PADDING, infoY);
        Color moneyColor = _player.Money >= cost ? Color.LimeGreen : Color.Red;
        DrawTextWithShadow(spriteBatch, font, moneyText, moneyPos, moneyColor);
        
        // Draw status message if any
        if (_messageTimer > 0 && !string.IsNullOrEmpty(_statusMessage))
        {
            infoY += 30;
            Vector2 messagePos = new Vector2(confirmX + PADDING, infoY);
            DrawTextWithShadow(spriteBatch, font, _statusMessage, messagePos, Color.Red);
        }
        
        // Draw confirmation prompt
        string prompt = "Travel to this location?";
        Vector2 promptSize = font.MeasureString(prompt);
        Vector2 promptPos = new Vector2(confirmX + (confirmWidth - promptSize.X) / 2, 
                                       confirmY + confirmHeight - PADDING * 3);
        DrawTextWithShadow(spriteBatch, font, prompt, promptPos, Color.White);
        
        // Draw controls hint
        string hint = "Y: Confirm | N: Cancel";
        Vector2 hintSize = font.MeasureString(hint);
        Vector2 hintPos = new Vector2(confirmX + (confirmWidth - hintSize.X) / 2, 
                                     confirmY + confirmHeight - PADDING - hintSize.Y);
        DrawTextWithShadow(spriteBatch, font, hint, hintPos, Color.LightGray);
    }
    
    private string GetWaypointIcon(WaypointType type)
    {
        return type switch
        {
            WaypointType.Farm => "🏡",
            WaypointType.Village => "🏘",
            WaypointType.DungeonEntrance => "⚔",
            WaypointType.MineshaftEntrance => "⛏",
            WaypointType.Landmark => "📍",
            WaypointType.ShopDistrict => "🏪",
            WaypointType.Custom => "⭐",
            _ => "📍"
        };
    }
    
    private Color GetWaypointColor(WaypointType type)
    {
        return type switch
        {
            WaypointType.Farm => Color.LimeGreen,
            WaypointType.Village => Color.SkyBlue,
            WaypointType.DungeonEntrance => Color.DarkRed,
            WaypointType.MineshaftEntrance => Color.Gray,
            WaypointType.Landmark => Color.Gold,
            WaypointType.ShopDistrict => Color.Orange,
            WaypointType.Custom => Color.Magenta,
            _ => Color.White
        };
    }
    
    private void DrawTextWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, 
                                   Vector2 position, Color color)
    {
        // Draw shadow
        spriteBatch.DrawString(font, text, position + Vector2.One, Color.Black);
        // Draw text
        spriteBatch.DrawString(font, text, position, color);
    }
    
    private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, Color color, int thickness)
    {
        // Top
        spriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Bottom
        spriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y + rect.Height - thickness, 
                                                     rect.Width, thickness), color);
        // Left
        spriteBatch.Draw(_pixelTexture, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Right
        spriteBatch.Draw(_pixelTexture, new Rectangle(rect.X + rect.Width - thickness, rect.Y, 
                                                     thickness, rect.Height), color);
    }
    
    public bool IsActive => _isActive;
}
