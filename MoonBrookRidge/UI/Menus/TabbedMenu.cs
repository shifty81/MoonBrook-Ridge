using MoonBrookRidge.Engine.MonoGameCompat;
using System;
using System.Collections.Generic;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Base class for tabbed menu system inspired by Stardew Valley
/// </summary>
public abstract class TabbedMenu
{
    protected List<MenuTab> _tabs;
    protected int _selectedTabIndex;
    protected bool _isActive;
    protected Texture2D _pixelTexture = null!;
    protected KeyboardState _previousKeyboardState;
    protected MouseState _previousMouseState;
    
    // Menu dimensions
    protected const int MENU_WIDTH = 1000;
    protected const int MENU_HEIGHT = 700;
    protected const int TAB_HEIGHT = 60;
    protected const int TAB_WIDTH = 160;
    protected const int PADDING = 20;
    
    // Colors
    protected readonly Color TabActiveColor = new Color(100, 80, 60);
    protected readonly Color TabInactiveColor = new Color(60, 50, 40);
    protected readonly Color TabHoverColor = new Color(80, 70, 55);
    protected readonly Color BackgroundColor = new Color(40, 35, 30);
    protected readonly Color BorderColor = new Color(200, 180, 150);
    
    public TabbedMenu()
    {
        _tabs = new List<MenuTab>();
        _selectedTabIndex = 0;
        _isActive = false;
    }
    
    /// <summary>
    /// Add a tab to the menu
    /// </summary>
    protected void AddTab(string name, string description)
    {
        _tabs.Add(new MenuTab
        {
            Name = name,
            Description = description,
            Index = _tabs.Count
        });
    }
    
    /// <summary>
    /// Show the tabbed menu
    /// </summary>
    public virtual void Show()
    {
        _isActive = true;
        _selectedTabIndex = 0;
    }
    
    /// <summary>
    /// Hide the tabbed menu
    /// </summary>
    public virtual void Hide()
    {
        _isActive = false;
    }
    
    /// <summary>
    /// Toggle the menu visibility
    /// </summary>
    public virtual void Toggle()
    {
        if (_isActive)
            Hide();
        else
            Show();
    }
    
    /// <summary>
    /// Update menu with keyboard and mouse input
    /// </summary>
    public virtual void Update(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
    {
        if (!_isActive) return;
        
        // Close menu with Escape or E
        if ((keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape)) ||
            (keyboardState.IsKeyDown(Keys.E) && !_previousKeyboardState.IsKeyDown(Keys.E)))
        {
            Hide();
        }
        
        // Tab navigation with Q/E or number keys
        if (keyboardState.IsKeyDown(Keys.Q) && !_previousKeyboardState.IsKeyDown(Keys.Q))
        {
            _selectedTabIndex--;
            if (_selectedTabIndex < 0) _selectedTabIndex = _tabs.Count - 1;
        }
        
        if (keyboardState.IsKeyDown(Keys.Tab) && !_previousKeyboardState.IsKeyDown(Keys.Tab))
        {
            _selectedTabIndex++;
            if (_selectedTabIndex >= _tabs.Count) _selectedTabIndex = 0;
        }
        
        // Number keys 1-9 for direct tab selection
        for (int i = 0; i < Math.Min(_tabs.Count, 9); i++)
        {
            Keys numberKey = Keys.D1 + i;
            if (keyboardState.IsKeyDown(numberKey) && !_previousKeyboardState.IsKeyDown(numberKey))
            {
                _selectedTabIndex = i;
            }
        }
        
        // Mouse input for tab clicking
        HandleMouseInput(mouseState);
        
        // Update current tab content
        UpdateTabContent(gameTime, keyboardState, mouseState);
        
        _previousKeyboardState = keyboardState;
        _previousMouseState = mouseState;
    }
    
    /// <summary>
    /// Handle mouse input for tabs
    /// </summary>
    protected virtual void HandleMouseInput(MouseState mouseState)
    {
        // Check if mouse clicked on any tab
        if (mouseState.LeftButton == ButtonState.Pressed && 
            _previousMouseState.LeftButton == ButtonState.Released)
        {
            int menuX = GetMenuX();
            int menuY = GetMenuY();
            int tabY = menuY;
            
            for (int i = 0; i < _tabs.Count; i++)
            {
                int tabX = menuX + i * TAB_WIDTH;
                Rectangle tabBounds = new Rectangle(tabX, tabY, TAB_WIDTH, TAB_HEIGHT);
                
                if (tabBounds.Contains(mouseState.Position))
                {
                    _selectedTabIndex = i;
                    OnTabSelected(_tabs[i]);
                    break;
                }
            }
        }
    }
    
    /// <summary>
    /// Get tab bounds for mouse hover detection
    /// </summary>
    protected Rectangle GetTabBounds(int tabIndex, int screenWidth, int screenHeight)
    {
        int menuX = GetMenuX();
        int menuY = GetMenuY();
        int tabX = menuX + tabIndex * TAB_WIDTH;
        return new Rectangle(tabX, menuY, TAB_WIDTH, TAB_HEIGHT);
    }
    
    /// <summary>
    /// Update the content of the currently selected tab
    /// </summary>
    protected abstract void UpdateTabContent(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState);
    
    /// <summary>
    /// Called when a tab is selected
    /// </summary>
    protected virtual void OnTabSelected(MenuTab tab)
    {
        // Override in derived classes
    }
    
    /// <summary>
    /// Draw the tabbed menu
    /// </summary>
    public virtual void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
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
            new Color(0, 0, 0, 200));
        
        int menuX = GetMenuX();
        int menuY = GetMenuY();
        
        // Draw tabs
        DrawTabs(spriteBatch, font, menuX, menuY, graphicsDevice.Viewport);
        
        // Draw main content area
        int contentY = menuY + TAB_HEIGHT;
        int contentHeight = MENU_HEIGHT - TAB_HEIGHT;
        
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(menuX, contentY, MENU_WIDTH, contentHeight),
            BackgroundColor);
        
        // Draw border around content
        DrawBorder(spriteBatch, menuX, contentY, MENU_WIDTH, contentHeight, 3, BorderColor);
        
        // Draw content of selected tab
        DrawTabContent(spriteBatch, font, menuX, contentY, MENU_WIDTH, contentHeight);
    }
    
    /// <summary>
    /// Draw the tabs at the top of the menu
    /// </summary>
    protected virtual void DrawTabs(SpriteBatch spriteBatch, SpriteFont font, int menuX, int menuY, Viewport viewport)
    {
        MouseState mouseState = Mouse.GetState();
        
        for (int i = 0; i < _tabs.Count; i++)
        {
            int tabX = menuX + i * TAB_WIDTH;
            bool isSelected = (i == _selectedTabIndex);
            bool isHovered = false;
            
            // Check if mouse is hovering over this tab
            Rectangle tabBounds = new Rectangle(tabX, menuY, TAB_WIDTH, TAB_HEIGHT);
            if (tabBounds.Contains(mouseState.Position))
            {
                isHovered = true;
            }
            
            // Choose tab color
            Color tabColor = isSelected ? TabActiveColor : (isHovered ? TabHoverColor : TabInactiveColor);
            
            // Draw tab background
            spriteBatch.Draw(_pixelTexture,
                new Rectangle(tabX, menuY, TAB_WIDTH, TAB_HEIGHT),
                tabColor);
            
            // Draw tab border
            DrawBorder(spriteBatch, tabX, menuY, TAB_WIDTH, TAB_HEIGHT, 2, BorderColor);
            
            // Draw tab name
            string tabName = _tabs[i].Name;
            Vector2 textSize = font.MeasureString(tabName);
            Vector2 textPos = new Vector2(
                tabX + TAB_WIDTH / 2 - textSize.X / 2,
                menuY + TAB_HEIGHT / 2 - textSize.Y / 2
            );
            
            Color textColor = isSelected ? Color.White : Color.LightGray;
            spriteBatch.DrawString(font, tabName, textPos, textColor);
        }
    }
    
    /// <summary>
    /// Draw the content of the currently selected tab
    /// </summary>
    protected abstract void DrawTabContent(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height);
    
    /// <summary>
    /// Draw a border around a rectangle
    /// </summary>
    protected void DrawBorder(SpriteBatch spriteBatch, int x, int y, int width, int height, int thickness, Color color)
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
    
    /// <summary>
    /// Get menu X position (centered)
    /// </summary>
    protected int GetMenuX()
    {
        return (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Width - MENU_WIDTH) / 2;
    }
    
    /// <summary>
    /// Get menu Y position (centered)
    /// </summary>
    protected int GetMenuY()
    {
        return (GraphicsAdapter.DefaultAdapter.CurrentDisplayMode.Height - MENU_HEIGHT) / 2;
    }
    
    public bool IsActive => _isActive;
    public int SelectedTabIndex => _selectedTabIndex;
    public MenuTab? SelectedTab => _tabs.Count > 0 ? _tabs[_selectedTabIndex] : null;
}

/// <summary>
/// Represents a single tab in the menu
/// </summary>
public class MenuTab
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public int Index { get; set; }
}
