using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Buildings;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Menu for placing and managing furniture inside buildings
/// Accessible when player is inside a building
/// </summary>
public class FurnitureMenu
{
    private FurnitureSystem _furnitureSystem;
    private Building _currentBuilding;
    private bool _isActive;
    private Texture2D? _pixel;
    
    // UI State
    private int _selectedIndex;
    private List<Furniture> _availableTemplates;
    private FurnitureType _selectedCategory;
    private bool _placementMode; // True when actively placing furniture
    private Vector2 _previewPosition;
    
    // UI Layout
    private const int MENU_WIDTH = 600;
    private const int MENU_HEIGHT = 500;
    private const int PADDING = 20;
    private const int ITEM_HEIGHT = 40;
    
    public bool IsActive => _isActive;
    
    public event Action<Furniture>? OnFurniturePlaced;
    
    public FurnitureMenu(FurnitureSystem furnitureSystem)
    {
        _furnitureSystem = furnitureSystem;
        _availableTemplates = new List<Furniture>();
        _selectedIndex = 0;
        _selectedCategory = FurnitureType.Bed;
        _placementMode = false;
        _previewPosition = Vector2.Zero;
        _currentBuilding = null!; // Will be set when opening menu
    }
    
    public void Show(Building building)
    {
        _currentBuilding = building;
        _isActive = true;
        _selectedIndex = 0;
        _placementMode = false;
        UpdateAvailableTemplates();
    }
    
    public void Hide()
    {
        _isActive = false;
        _placementMode = false;
    }
    
    public void Toggle(Building building)
    {
        if (_isActive)
            Hide();
        else
            Show(building);
    }
    
    private void UpdateAvailableTemplates()
    {
        _availableTemplates = _furnitureSystem.GetTemplatesByType(_selectedCategory);
        if (_selectedIndex >= _availableTemplates.Count)
            _selectedIndex = 0;
    }
    
    public void Update(GameTime gameTime)
    {
        if (!_isActive) return;
        
        var keyboardState = Keyboard.GetState();
        
        if (_placementMode)
        {
            UpdatePlacementMode(keyboardState);
        }
        else
        {
            UpdateSelectionMode(keyboardState);
        }
    }
    
    private void UpdateSelectionMode(KeyboardState keyboardState)
    {
        // Navigate categories with Left/Right
        if (keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
        {
            int categoryIndex = (int)_selectedCategory;
            categoryIndex = (categoryIndex - 1 + Enum.GetValues<FurnitureType>().Length) % Enum.GetValues<FurnitureType>().Length;
            _selectedCategory = (FurnitureType)categoryIndex;
            UpdateAvailableTemplates();
        }
        
        if (keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
        {
            int categoryIndex = (int)_selectedCategory;
            categoryIndex = (categoryIndex + 1) % Enum.GetValues<FurnitureType>().Length;
            _selectedCategory = (FurnitureType)categoryIndex;
            UpdateAvailableTemplates();
        }
        
        // Navigate items with Up/Down
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedIndex = (_selectedIndex - 1 + _availableTemplates.Count) % _availableTemplates.Count;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedIndex = (_selectedIndex + 1) % _availableTemplates.Count;
        }
        
        // Enter placement mode
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (_availableTemplates.Count > 0)
            {
                _placementMode = true;
                _previewPosition = new Vector2(5, 5); // Start at top-left
            }
        }
        
        // Close menu
        if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            Hide();
        }
        
        _previousKeyboardState = keyboardState;
    }
    
    private KeyboardState _previousKeyboardState;
    
    private void UpdatePlacementMode(KeyboardState keyboardState)
    {
        // Move preview position with arrow keys
        if (keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
        {
            _previewPosition.X = Math.Max(0, _previewPosition.X - 1);
        }
        
        if (keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
        {
            _previewPosition.X = Math.Min(20, _previewPosition.X + 1);
        }
        
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _previewPosition.Y = Math.Max(0, _previewPosition.Y - 1);
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _previewPosition.Y = Math.Min(20, _previewPosition.Y + 1);
        }
        
        // Confirm placement
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            var selectedTemplate = _availableTemplates[_selectedIndex];
            bool success = _furnitureSystem.PlaceFurniture(_currentBuilding, selectedTemplate.Id, _previewPosition);
            
            if (success)
            {
                OnFurniturePlaced?.Invoke(selectedTemplate);
                _placementMode = false;
            }
        }
        
        // Cancel placement
        if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            _placementMode = false;
        }
        
        _previousKeyboardState = keyboardState;
    }
    
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
    {
        if (!_isActive) return;
        
        // Lazy create pixel texture
        if (_pixel == null)
        {
            _pixel = new Texture2D(graphicsDevice, 1, 1);
            _pixel.SetData(new[] { Color.White });
        }
        
        // Calculate centered position
        int screenWidth = graphicsDevice.Viewport.Width;
        int screenHeight = graphicsDevice.Viewport.Height;
        int menuX = (screenWidth - MENU_WIDTH) / 2;
        int menuY = (screenHeight - MENU_HEIGHT) / 2;
        
        // Draw overlay
        spriteBatch.Draw(_pixel, new Rectangle(0, 0, screenWidth, screenHeight), Color.Black * 0.7f);
        
        // Draw menu background
        spriteBatch.Draw(_pixel, new Rectangle(menuX, menuY, MENU_WIDTH, MENU_HEIGHT), Color.DarkSlateGray);
        spriteBatch.Draw(_pixel, new Rectangle(menuX, menuY, MENU_WIDTH, MENU_HEIGHT), null, Color.White, 0f, Vector2.Zero, SpriteEffects.None, 0f);
        
        // Draw title
        string title = _placementMode ? "Place Furniture - Use Arrow Keys" : "Furniture Menu";
        Vector2 titleSize = font.MeasureString(title);
        spriteBatch.DrawString(font, title, new Vector2(menuX + MENU_WIDTH / 2 - titleSize.X / 2, menuY + 20), Color.White);
        
        if (_placementMode)
        {
            DrawPlacementMode(spriteBatch, font, menuX, menuY);
        }
        else
        {
            DrawSelectionMode(spriteBatch, font, menuX, menuY);
        }
    }
    
    private void DrawSelectionMode(SpriteBatch spriteBatch, SpriteFont font, int menuX, int menuY)
    {
        // Draw category tabs
        string categoryText = $"< {_selectedCategory} >";
        Vector2 categorySize = font.MeasureString(categoryText);
        spriteBatch.DrawString(font, categoryText, new Vector2(menuX + MENU_WIDTH / 2 - categorySize.X / 2, menuY + 60), Color.Yellow);
        
        // Draw furniture list
        int startY = menuY + 100;
        int displayCount = Math.Min(8, _availableTemplates.Count);
        
        if (_availableTemplates.Count == 0)
        {
            spriteBatch.DrawString(font, "No furniture in this category", new Vector2(menuX + PADDING, startY), Color.Gray);
        }
        else
        {
            for (int i = 0; i < displayCount; i++)
            {
                var furniture = _availableTemplates[i];
                int itemY = startY + i * ITEM_HEIGHT;
                
                Color textColor = i == _selectedIndex ? Color.Yellow : Color.White;
                
                // Draw selection highlight
                if (i == _selectedIndex && _pixel != null)
                {
                    spriteBatch.Draw(_pixel, new Rectangle(menuX + PADDING, itemY, MENU_WIDTH - PADDING * 2, ITEM_HEIGHT - 5), Color.Yellow * 0.3f);
                }
                
                // Draw furniture info
                string itemText = $"{furniture.Name} - {furniture.Size.X}x{furniture.Size.Y}";
                spriteBatch.DrawString(font, itemText, new Vector2(menuX + PADDING + 10, itemY + 5), textColor);
                
                // Draw cost
                string costText = $"{furniture.Cost.Gold}g, {furniture.Cost.Wood}w, {furniture.Cost.Stone}s";
                spriteBatch.DrawString(font, costText, new Vector2(menuX + PADDING + 10, itemY + 20), Color.LightGray);
            }
        }
        
        // Draw controls
        int controlY = menuY + MENU_HEIGHT - 80;
        spriteBatch.DrawString(font, "Arrow Keys: Navigate | Enter: Place | Esc: Close", 
            new Vector2(menuX + PADDING, controlY), Color.LightGray);
        
        // Draw stats
        var stats = _furnitureSystem.GetBuildingComfort(_currentBuilding.Position);
        spriteBatch.DrawString(font, $"Building Comfort: {stats}", 
            new Vector2(menuX + PADDING, controlY + 25), Color.LightGreen);
    }
    
    private void DrawPlacementMode(SpriteBatch spriteBatch, SpriteFont font, int menuX, int menuY)
    {
        var selectedTemplate = _availableTemplates[_selectedIndex];
        
        // Draw preview position
        string posText = $"Position: ({_previewPosition.X}, {_previewPosition.Y})";
        spriteBatch.DrawString(font, posText, new Vector2(menuX + PADDING, menuY + 100), Color.White);
        
        // Draw furniture info
        string infoText = $"{selectedTemplate.Name} ({selectedTemplate.Size.X}x{selectedTemplate.Size.Y})";
        spriteBatch.DrawString(font, infoText, new Vector2(menuX + PADDING, menuY + 140), Color.Yellow);
        
        spriteBatch.DrawString(font, selectedTemplate.Description, new Vector2(menuX + PADDING, menuY + 170), Color.LightGray);
        
        // Draw controls
        int controlY = menuY + MENU_HEIGHT - 80;
        spriteBatch.DrawString(font, "Arrow Keys: Move | Enter: Place | Esc: Cancel", 
            new Vector2(menuX + PADDING, controlY), Color.LightGray);
    }
}
