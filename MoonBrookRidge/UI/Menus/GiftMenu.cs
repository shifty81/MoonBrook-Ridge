using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Characters.NPCs;
using System.Collections.Generic;
using System.Linq;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Gift-giving menu UI for selecting items to give to NPCs
/// </summary>
public class GiftMenu
{
    private bool _isActive;
    private InventorySystem _inventory;
    private NPCCharacter _targetNPC;
    private List<InventorySlot> _giftableItems;
    private int _selectedItemIndex;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture;
    private string _statusMessage;
    private float _messageTimer;
    
    private const int MENU_WIDTH = 600;
    private const int MENU_HEIGHT = 500;
    private const int ITEM_HEIGHT = 45;
    private const int PADDING = 20;
    private const float MESSAGE_DURATION = 3f;
    
    public GiftMenu(InventorySystem inventory)
    {
        _inventory = inventory;
        _giftableItems = new List<InventorySlot>();
        _selectedItemIndex = 0;
        _isActive = false;
        _statusMessage = "";
        _messageTimer = 0f;
        _targetNPC = null;
    }
    
    public void Show(NPCCharacter targetNPC)
    {
        _isActive = true;
        _targetNPC = targetNPC;
        _selectedItemIndex = 0;
        _statusMessage = "";
        
        // Get all giftable items from inventory (all items except tools)
        _giftableItems = _inventory.GetSlots()
            .Where(slot => !slot.IsEmpty && slot.Item.Type != ItemType.Tool)
            .ToList();
    }
    
    public void Hide()
    {
        _isActive = false;
        _targetNPC = null;
    }
    
    public void Toggle(NPCCharacter targetNPC = null)
    {
        if (_isActive)
            Hide();
        else if (targetNPC != null)
            Show(targetNPC);
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
        
        // Close menu with Escape or G
        if ((keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape)) ||
            (keyboardState.IsKeyDown(Keys.G) && !_previousKeyboardState.IsKeyDown(Keys.G)))
        {
            Hide();
        }
        
        // Navigation
        if (_giftableItems.Count > 0)
        {
            if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _selectedItemIndex--;
                if (_selectedItemIndex < 0) _selectedItemIndex = _giftableItems.Count - 1;
            }
            
            if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _selectedItemIndex++;
                if (_selectedItemIndex >= _giftableItems.Count) _selectedItemIndex = 0;
            }
            
            // Give gift with Enter or X
            if ((keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter)) ||
                (keyboardState.IsKeyDown(Keys.X) && !_previousKeyboardState.IsKeyDown(Keys.X)))
            {
                GiveSelectedGift();
            }
        }
        
        _previousKeyboardState = keyboardState;
    }
    
    private void GiveSelectedGift()
    {
        if (_targetNPC == null || _giftableItems.Count == 0 || _selectedItemIndex < 0) return;
        
        var selectedSlot = _giftableItems[_selectedItemIndex];
        if (selectedSlot == null || selectedSlot.Item == null) return;
        
        // Give the gift to NPC
        string itemName = selectedSlot.Item.Name;
        int oldFriendship = _targetNPC.FriendshipLevel;
        _targetNPC.GiveGift(itemName);
        int newFriendship = _targetNPC.FriendshipLevel;
        int friendshipChange = newFriendship - oldFriendship;
        
        // Remove item from inventory
        _inventory.RemoveItem(itemName, 1);
        
        // Update status message based on response
        if (friendshipChange >= 80)
        {
            _statusMessage = $"{_targetNPC.Name} loved the {itemName}! (+{friendshipChange})";
        }
        else if (friendshipChange >= 40)
        {
            _statusMessage = $"{_targetNPC.Name} liked the {itemName}! (+{friendshipChange})";
        }
        else if (friendshipChange > 0)
        {
            _statusMessage = $"{_targetNPC.Name} accepted the {itemName}. (+{friendshipChange})";
        }
        else if (friendshipChange < 0)
        {
            _statusMessage = $"{_targetNPC.Name} didn't like the {itemName}... ({friendshipChange})";
        }
        
        _messageTimer = MESSAGE_DURATION;
        
        // Refresh giftable items list
        _giftableItems = _inventory.GetSlots()
            .Where(slot => !slot.IsEmpty && slot.Item.Type != ItemType.Tool)
            .ToList();
        
        // Adjust selection if necessary
        if (_selectedItemIndex >= _giftableItems.Count)
        {
            _selectedItemIndex = System.Math.Max(0, _giftableItems.Count - 1);
        }
        
        // Close menu if no items left
        if (_giftableItems.Count == 0)
        {
            Hide();
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
        
        // Semi-transparent background overlay
        spriteBatch.Draw(_pixelTexture, 
            new Rectangle(0, 0, screenWidth, screenHeight), 
            new Color(0, 0, 0, 180));
        
        // Menu background
        int menuX = (screenWidth - MENU_WIDTH) / 2;
        int menuY = (screenHeight - MENU_HEIGHT) / 2;
        
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(menuX, menuY, MENU_WIDTH, MENU_HEIGHT),
            new Color(40, 35, 30));
        
        // Menu border
        DrawBorder(spriteBatch, menuX, menuY, MENU_WIDTH, MENU_HEIGHT, 3, new Color(200, 180, 150));
        
        // Title
        string title = _targetNPC != null ? $"Gift for {_targetNPC.Name}" : "Select Gift";
        Vector2 titleSize = font.MeasureString(title);
        spriteBatch.DrawString(font, title,
            new Vector2(menuX + MENU_WIDTH / 2 - titleSize.X / 2, menuY + PADDING),
            Color.White);
        
        // Heart level display
        if (_targetNPC != null)
        {
            int heartLevel = _targetNPC.GetHeartLevel();
            string heartText = $"Hearts: {heartLevel}/10 ({_targetNPC.FriendshipLevel} pts)";
            Vector2 heartSize = font.MeasureString(heartText);
            spriteBatch.DrawString(font, heartText,
                new Vector2(menuX + MENU_WIDTH / 2 - heartSize.X / 2, menuY + PADDING + 30),
                Color.LightPink);
        }
        
        // Instructions
        string instructions = "↑↓: Select | Enter/X: Give | Esc/G: Close";
        Vector2 instructSize = font.MeasureString(instructions);
        spriteBatch.DrawString(font, instructions,
            new Vector2(menuX + MENU_WIDTH / 2 - instructSize.X / 2, menuY + PADDING + 60),
            Color.LightGray);
        
        // Item list
        int listY = menuY + PADDING + 100;
        int maxVisibleItems = 7;
        int startIndex = System.Math.Max(0, _selectedItemIndex - maxVisibleItems / 2);
        int endIndex = System.Math.Min(_giftableItems.Count, startIndex + maxVisibleItems);
        
        if (_giftableItems.Count == 0)
        {
            string noItemsText = "No items available to gift";
            Vector2 noItemsSize = font.MeasureString(noItemsText);
            spriteBatch.DrawString(font, noItemsText,
                new Vector2(menuX + MENU_WIDTH / 2 - noItemsSize.X / 2, listY + 50),
                Color.Gray);
        }
        else
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                var slot = _giftableItems[i];
                int itemY = listY + (i - startIndex) * ITEM_HEIGHT;
                bool isSelected = (i == _selectedItemIndex);
                
                // Selection highlight
                if (isSelected)
                {
                    spriteBatch.Draw(_pixelTexture,
                        new Rectangle(menuX + PADDING, itemY, MENU_WIDTH - PADDING * 2, ITEM_HEIGHT - 5),
                        new Color(80, 70, 60));
                }
                
                // Item name and quantity
                string itemText = $"{slot.Item.Name} x{slot.Quantity}";
                Color textColor = isSelected ? Color.Yellow : Color.White;
                spriteBatch.DrawString(font, itemText,
                    new Vector2(menuX + PADDING + 10, itemY + 10),
                    textColor);
                
                // Item type
                string typeText = $"({slot.Item.Type})";
                Vector2 typeSize = font.MeasureString(typeText);
                spriteBatch.DrawString(font, typeText,
                    new Vector2(menuX + MENU_WIDTH - PADDING - typeSize.X - 10, itemY + 10),
                    new Color(150, 150, 150));
            }
        }
        
        // Status message
        if (_messageTimer > 0 && !string.IsNullOrEmpty(_statusMessage))
        {
            Vector2 messageSize = font.MeasureString(_statusMessage);
            int messageY = menuY + MENU_HEIGHT - PADDING - 30;
            
            // Message background
            spriteBatch.Draw(_pixelTexture,
                new Rectangle(menuX + PADDING, messageY - 5, MENU_WIDTH - PADDING * 2, 35),
                new Color(20, 20, 20, 200));
            
            // Message text
            Color messageColor = _statusMessage.Contains("loved") ? Color.LightGreen :
                               _statusMessage.Contains("liked") ? Color.LightBlue :
                               _statusMessage.Contains("didn't") ? Color.OrangeRed : Color.White;
            
            spriteBatch.DrawString(font, _statusMessage,
                new Vector2(menuX + MENU_WIDTH / 2 - messageSize.X / 2, messageY),
                messageColor);
        }
    }
    
    private void DrawBorder(SpriteBatch spriteBatch, int x, int y, int width, int height, int thickness, Color color)
    {
        // Top
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, width, thickness), color);
        // Bottom
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y + height - thickness, width, thickness), color);
        // Left
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, thickness, height), color);
        // Right
        spriteBatch.Draw(_pixelTexture, new Rectangle(x + width - thickness, y, thickness, height), color);
    }
    
    public bool IsActive => _isActive;
    public NPCCharacter TargetNPC => _targetNPC;
}
