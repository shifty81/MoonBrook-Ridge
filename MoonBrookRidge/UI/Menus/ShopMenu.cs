using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Items.Shop;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Characters.Player;
using System.Collections.Generic;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Shop menu UI for buying and selling items
/// </summary>
public class ShopMenu
{
    private bool _isActive;
    private ShopSystem _shopSystem;
    private InventorySystem _inventory;
    private PlayerCharacter _player;
    private List<ShopItem> _shopInventory;
    private int _selectedItemIndex;
    private ShopMode _currentMode;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture;
    private string _statusMessage;
    private float _messageTimer;
    
    private const int MENU_WIDTH = 700;
    private const int MENU_HEIGHT = 600;
    private const int ITEM_HEIGHT = 50;
    private const int PADDING = 20;
    private const float MESSAGE_DURATION = 2f;
    
    private enum ShopMode
    {
        Buy,
        Sell
    }
    
    public ShopMenu(ShopSystem shopSystem, InventorySystem inventory, PlayerCharacter player)
    {
        _shopSystem = shopSystem;
        _inventory = inventory;
        _player = player;
        _shopInventory = new List<ShopItem>();
        _selectedItemIndex = 0;
        _currentMode = ShopMode.Buy;
        _isActive = false;
        _statusMessage = "";
        _messageTimer = 0f;
    }
    
    public void Show()
    {
        _isActive = true;
        _currentMode = ShopMode.Buy;
        _shopInventory = _shopSystem.GetShopInventory();
        _selectedItemIndex = 0;
        _statusMessage = "";
    }
    
    public void Hide()
    {
        _isActive = false;
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
        
        // Switch between Buy and Sell modes
        if (keyboardState.IsKeyDown(Keys.Tab) && !_previousKeyboardState.IsKeyDown(Keys.Tab))
        {
            _currentMode = (_currentMode == ShopMode.Buy) ? ShopMode.Sell : ShopMode.Buy;
            _selectedItemIndex = 0;
            _statusMessage = "";
        }
        
        // Navigation
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedItemIndex--;
            int maxIndex = (_currentMode == ShopMode.Buy) ? _shopInventory.Count - 1 : 
                          _inventory.GetSlots().Count - 1;
            if (_selectedItemIndex < 0)
                _selectedItemIndex = maxIndex;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedItemIndex++;
            int maxIndex = (_currentMode == ShopMode.Buy) ? _shopInventory.Count - 1 : 
                          _inventory.GetSlots().Count - 1;
            if (_selectedItemIndex > maxIndex)
                _selectedItemIndex = 0;
        }
        
        // Buy or Sell action
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (_currentMode == ShopMode.Buy)
            {
                HandleBuy();
            }
            else
            {
                HandleSell();
            }
        }
        
        // Close menu
        if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            Hide();
        }
        
        _previousKeyboardState = keyboardState;
    }
    
    private void HandleBuy()
    {
        if (_shopInventory.Count == 0 || _selectedItemIndex >= _shopInventory.Count)
            return;
        
        ShopItem selectedItem = _shopInventory[_selectedItemIndex];
        int quantity = 1; // Buy 1 at a time for now
        
        bool success = _shopSystem.BuyItem(selectedItem, quantity, _inventory, _player);
        
        if (success)
        {
            _statusMessage = $"Purchased {selectedItem.Item.Name} for ${selectedItem.BuyPrice}";
            _messageTimer = MESSAGE_DURATION;
        }
        else
        {
            if (_player.Money < selectedItem.BuyPrice)
            {
                _statusMessage = "Not enough money!";
            }
            else
            {
                _statusMessage = "Inventory full!";
            }
            _messageTimer = MESSAGE_DURATION;
        }
    }
    
    private void HandleSell()
    {
        var slots = _inventory.GetSlots();
        if (_selectedItemIndex >= slots.Count)
            return;
        
        var slot = slots[_selectedItemIndex];
        if (slot.IsEmpty)
        {
            _statusMessage = "No item to sell";
            _messageTimer = MESSAGE_DURATION;
            return;
        }
        
        int quantity = 1; // Sell 1 at a time for now
        bool success = _shopSystem.SellItem(slot.Item.Name, quantity, _inventory, _player);
        
        if (success)
        {
            _statusMessage = $"Sold {slot.Item.Name} for ${slot.Item.SellPrice}";
            _messageTimer = MESSAGE_DURATION;
        }
        else
        {
            _statusMessage = "Cannot sell item";
            _messageTimer = MESSAGE_DURATION;
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
        
        // Draw title
        string title = $"Shop - {_currentMode} Mode";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(menuX + (MENU_WIDTH - titleSize.X) / 2, menuY + PADDING);
        DrawTextWithShadow(spriteBatch, font, title, titlePos, Color.Gold);
        
        // Draw player money
        string moneyText = $"Money: ${_player.Money}";
        Vector2 moneyPos = new Vector2(menuX + MENU_WIDTH - PADDING - font.MeasureString(moneyText).X, 
                                       menuY + PADDING);
        DrawTextWithShadow(spriteBatch, font, moneyText, moneyPos, Color.LimeGreen);
        
        // Draw items list
        int itemStartY = menuY + PADDING * 3;
        int maxVisibleItems = (MENU_HEIGHT - PADDING * 6) / ITEM_HEIGHT;
        
        if (_currentMode == ShopMode.Buy)
        {
            DrawBuyMode(spriteBatch, font, menuX, itemStartY, maxVisibleItems);
        }
        else
        {
            DrawSellMode(spriteBatch, font, menuX, itemStartY, maxVisibleItems);
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
        string hint = "Up/Down: Navigate | Enter: Buy/Sell | Tab: Switch Mode | Esc: Close";
        Vector2 hintSize = font.MeasureString(hint);
        Vector2 hintPos = new Vector2(menuX + (MENU_WIDTH - hintSize.X) / 2, 
                                     menuY + MENU_HEIGHT - PADDING - hintSize.Y);
        DrawTextWithShadow(spriteBatch, font, hint, hintPos, Color.LightGray);
    }
    
    private void DrawBuyMode(SpriteBatch spriteBatch, SpriteFont font, int menuX, int startY, int maxVisible)
    {
        for (int i = 0; i < _shopInventory.Count && i < maxVisible; i++)
        {
            ShopItem shopItem = _shopInventory[i];
            int itemY = startY + i * ITEM_HEIGHT;
            bool isSelected = (i == _selectedItemIndex);
            bool canAfford = _player.Money >= shopItem.BuyPrice;
            
            // Draw item background
            Rectangle itemRect = new Rectangle(menuX + PADDING, itemY, 
                                              MENU_WIDTH - PADDING * 2, ITEM_HEIGHT - 5);
            Color bgColor = isSelected ? new Color(80, 80, 100) : new Color(50, 50, 60);
            spriteBatch.Draw(_pixelTexture, itemRect, bgColor);
            
            if (isSelected)
            {
                DrawBorder(spriteBatch, itemRect, Color.Yellow, 2);
            }
            
            // Draw item name
            Color textColor = canAfford ? Color.White : Color.Gray;
            Vector2 namePos = new Vector2(itemRect.X + 10, itemRect.Y + 8);
            DrawTextWithShadow(spriteBatch, font, shopItem.Item.Name, namePos, textColor);
            
            // Draw price
            string priceText = $"${shopItem.BuyPrice}";
            Vector2 priceSize = font.MeasureString(priceText);
            Vector2 pricePos = new Vector2(itemRect.Right - priceSize.X - 10, itemRect.Y + 8);
            DrawTextWithShadow(spriteBatch, font, priceText, pricePos, 
                              canAfford ? Color.LimeGreen : Color.Red);
            
            // Draw owned count
            int owned = _inventory.GetItemCount(shopItem.Item.Name);
            string ownedText = $"Owned: {owned}";
            Vector2 ownedPos = new Vector2(itemRect.X + 10, itemRect.Y + 28);
            DrawTextWithShadow(spriteBatch, font, ownedText, ownedPos, Color.LightGray);
        }
    }
    
    private void DrawSellMode(SpriteBatch spriteBatch, SpriteFont font, int menuX, int startY, int maxVisible)
    {
        var slots = _inventory.GetSlots();
        var nonEmptySlots = new List<(int slotIndex, InventorySlot slot)>();
        
        // Build list of non-empty slots
        for (int i = 0; i < slots.Count; i++)
        {
            if (!slots[i].IsEmpty)
            {
                nonEmptySlots.Add((i, slots[i]));
            }
        }
        
        // Draw items
        for (int displayIndex = 0; displayIndex < nonEmptySlots.Count && displayIndex < maxVisible; displayIndex++)
        {
            var (slotIndex, slot) = nonEmptySlots[displayIndex];
            
            int itemY = startY + displayIndex * ITEM_HEIGHT;
            bool isSelected = (slotIndex == _selectedItemIndex);
            
            // Draw item background
            Rectangle itemRect = new Rectangle(menuX + PADDING, itemY, 
                                              MENU_WIDTH - PADDING * 2, ITEM_HEIGHT - 5);
            Color bgColor = isSelected ? new Color(80, 80, 100) : new Color(50, 50, 60);
            spriteBatch.Draw(_pixelTexture, itemRect, bgColor);
            
            if (isSelected)
            {
                DrawBorder(spriteBatch, itemRect, Color.Yellow, 2);
            }
            
            // Draw item name and quantity
            string itemText = $"{slot.Item.Name} x{slot.Quantity}";
            Vector2 namePos = new Vector2(itemRect.X + 10, itemRect.Y + 8);
            DrawTextWithShadow(spriteBatch, font, itemText, namePos, Color.White);
            
            // Draw sell price
            string priceText = $"${slot.Item.SellPrice} each";
            Vector2 priceSize = font.MeasureString(priceText);
            Vector2 pricePos = new Vector2(itemRect.Right - priceSize.X - 10, itemRect.Y + 8);
            DrawTextWithShadow(spriteBatch, font, priceText, pricePos, Color.LimeGreen);
        }
        
        if (nonEmptySlots.Count == 0)
        {
            string noItems = "No items to sell";
            Vector2 noItemsSize = font.MeasureString(noItems);
            Vector2 noItemsPos = new Vector2(menuX + (MENU_WIDTH - noItemsSize.X) / 2, startY + 50);
            DrawTextWithShadow(spriteBatch, font, noItems, noItemsPos, Color.Gray);
        }
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
