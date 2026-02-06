using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Buildings;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.World.Tiles;
using System.Collections.Generic;
using System;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Building construction menu for browsing and placing buildings
/// </summary>
public class BuildingMenu
{
    private bool _isActive;
    private BuildingManager _buildingManager;
    private InventorySystem _inventory;
    private List<BuildingDefinition> _definitions;
    private int _selectedBuildingIndex;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture;
    
    // Placement mode
    private bool _isPlacementMode;
    private BuildingType _selectedBuildingType;
    private Vector2 _placementPosition;
    
    private const int MENU_WIDTH = 700;
    private const int MENU_HEIGHT = 550;
    private const int BUILDING_HEIGHT = 90;
    private const int PADDING = 20;
    
    public bool IsActive => _isActive;
    public bool IsPlacementMode => _isPlacementMode;
    public BuildingType SelectedBuildingType => _selectedBuildingType;
    public Vector2 PlacementPosition => _placementPosition;
    
    public BuildingMenu(BuildingManager buildingManager, InventorySystem inventory)
    {
        _buildingManager = buildingManager;
        _inventory = inventory;
        _definitions = new List<BuildingDefinition>();
        _selectedBuildingIndex = 0;
        _isActive = false;
        _isPlacementMode = false;
    }
    
    public void Show()
    {
        _isActive = true;
        _definitions = BuildingDefinitions.GetAllDefinitions();
        _selectedBuildingIndex = 0;
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
    
    public void EnterPlacementMode(BuildingType buildingType)
    {
        _isPlacementMode = true;
        _selectedBuildingType = buildingType;
        _placementPosition = Vector2.Zero;
        _isActive = false; // Hide menu during placement
    }
    
    public void ExitPlacementMode()
    {
        _isPlacementMode = false;
        _isActive = true; // Show menu again
    }
    
    public void Update(GameTime gameTime, int playerGold)
    {
        if (_isPlacementMode) return; // Placement handled by GameplayState
        if (!_isActive) return;
        
        KeyboardState keyboardState = Keyboard.GetState();
        
        // Navigation
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedBuildingIndex--;
            if (_selectedBuildingIndex < 0)
                _selectedBuildingIndex = _definitions.Count - 1;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedBuildingIndex++;
            if (_selectedBuildingIndex >= _definitions.Count)
                _selectedBuildingIndex = 0;
        }
        
        // Enter placement mode for selected building
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (_definitions.Count > 0 && _selectedBuildingIndex >= 0 && 
                _selectedBuildingIndex < _definitions.Count)
            {
                BuildingDefinition selectedDef = _definitions[_selectedBuildingIndex];
                
                // Check if player can afford it
                if (_buildingManager.CanAfford(selectedDef.Type, _inventory, playerGold))
                {
                    EnterPlacementMode(selectedDef.Type);
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
    
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice, int playerGold)
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
        string title = "Building Construction";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(menuX + (MENU_WIDTH - titleSize.X) / 2, menuY + PADDING);
        DrawTextWithShadow(spriteBatch, font, title, titlePos, Color.Gold);
        
        // Draw player gold
        string goldText = $"Gold: {playerGold}";
        Vector2 goldPos = new Vector2(menuX + PADDING, menuY + PADDING * 3);
        DrawTextWithShadow(spriteBatch, font, goldText, goldPos, Color.Yellow);
        
        // Draw buildings list
        int buildingStartY = menuY + PADDING * 5;
        int maxVisibleBuildings = (MENU_HEIGHT - PADDING * 7) / BUILDING_HEIGHT;
        
        if (_definitions.Count == 0)
        {
            string noBuildings = "No buildings available";
            Vector2 noBuildingsSize = font.MeasureString(noBuildings);
            Vector2 noBuildingsPos = new Vector2(menuX + (MENU_WIDTH - noBuildingsSize.X) / 2, 
                                              menuY + MENU_HEIGHT / 2);
            DrawTextWithShadow(spriteBatch, font, noBuildings, noBuildingsPos, Color.Gray);
        }
        else
        {
            for (int i = 0; i < _definitions.Count && i < maxVisibleBuildings; i++)
            {
                BuildingDefinition building = _definitions[i];
                int buildingY = buildingStartY + i * BUILDING_HEIGHT;
                bool isSelected = (i == _selectedBuildingIndex);
                bool canAfford = _buildingManager.CanAfford(building.Type, _inventory, playerGold);
                
                // Draw building background
                Rectangle buildingRect = new Rectangle(menuX + PADDING, buildingY, 
                                                    MENU_WIDTH - PADDING * 2, BUILDING_HEIGHT - 5);
                Color bgColor = isSelected ? new Color(80, 80, 100) : new Color(50, 50, 60);
                spriteBatch.Draw(_pixelTexture, buildingRect, bgColor);
                
                if (isSelected)
                {
                    DrawBorder(spriteBatch, buildingRect, Color.Yellow, 2);
                }
                
                // Draw building name
                Color textColor = canAfford ? Color.White : Color.Gray;
                Vector2 namePos = new Vector2(buildingRect.X + 10, buildingRect.Y + 5);
                DrawTextWithShadow(spriteBatch, font, building.Name, namePos, textColor);
                
                // Draw description
                Vector2 descPos = new Vector2(buildingRect.X + 10, buildingRect.Y + 25);
                DrawTextWithShadow(spriteBatch, font, building.Description, descPos, Color.LightGray, 0.7f);
                
                // Draw size
                string sizeText = $"Size: {building.TileWidth}x{building.TileHeight}";
                Vector2 sizePos = new Vector2(buildingRect.X + 10, buildingRect.Y + 45);
                DrawTextWithShadow(spriteBatch, font, sizeText, sizePos, Color.Cyan, 0.8f);
                
                // Draw cost
                string costText = $"Gold: {building.GoldCost}";
                Vector2 costPos = new Vector2(buildingRect.X + 10, buildingRect.Y + 65);
                Color costColor = playerGold >= building.GoldCost ? Color.LimeGreen : Color.Red;
                DrawTextWithShadow(spriteBatch, font, costText, costPos, costColor, 0.8f);
                
                // Draw materials
                string materialsText = GetMaterialsString(building);
                Vector2 materialsPos = new Vector2(buildingRect.X + 150, buildingRect.Y + 65);
                Color materialsColor = HasAllMaterials(building) ? Color.LimeGreen : Color.Red;
                DrawTextWithShadow(spriteBatch, font, materialsText, materialsPos, materialsColor, 0.7f);
                
                // Draw affordability status
                string status = canAfford ? "[X] Can Build" : "[ ] Insufficient Resources";
                Vector2 statusSize = font.MeasureString(status);
                Vector2 statusPos = new Vector2(buildingRect.Right - statusSize.X - 10, 
                                               buildingRect.Y + 10);
                DrawTextWithShadow(spriteBatch, font, status, statusPos, 
                                  canAfford ? Color.LimeGreen : Color.OrangeRed);
            }
        }
        
        // Draw controls hint at bottom
        string hint = "Up/Down: Navigate | Enter: Build | Esc: Close";
        Vector2 hintSize = font.MeasureString(hint);
        Vector2 hintPos = new Vector2(menuX + (MENU_WIDTH - hintSize.X) / 2, 
                                     menuY + MENU_HEIGHT - PADDING - hintSize.Y);
        DrawTextWithShadow(spriteBatch, font, hint, hintPos, Color.LightGray);
    }
    
    public void DrawPlacementPreview(SpriteBatch spriteBatch, SpriteFont font, 
                                    GraphicsDevice graphicsDevice, Vector2 tilePosition,
                                    Tile[,] worldTiles, int tileSize, Vector2 cameraPosition, float zoom)
    {
        if (!_isPlacementMode) return;
        
        if (_pixelTexture == null)
        {
            _pixelTexture = new Texture2D(graphicsDevice, 1, 1);
            _pixelTexture.SetData(new[] { Color.White });
        }
        
        var definition = BuildingDefinitions.GetDefinition(_selectedBuildingType);
        if (definition == null) return;
        
        bool isValid = _buildingManager.IsValidPlacement(_selectedBuildingType, tilePosition, worldTiles);
        Color previewColor = isValid ? Color.Green * 0.5f : Color.Red * 0.5f;
        
        // Draw preview tiles
        for (int x = 0; x < definition.TileWidth; x++)
        {
            for (int y = 0; y < definition.TileHeight; y++)
            {
                Vector2 worldPos = new Vector2(
                    (tilePosition.X + x) * tileSize,
                    (tilePosition.Y + y) * tileSize
                );
                
                Vector2 screenPos = (worldPos - cameraPosition) * zoom;
                Rectangle tileRect = new Rectangle(
                    (int)screenPos.X, 
                    (int)screenPos.Y, 
                    (int)(tileSize * zoom), 
                    (int)(tileSize * zoom)
                );
                
                spriteBatch.Draw(_pixelTexture, tileRect, previewColor);
                DrawBorder(spriteBatch, tileRect, isValid ? Color.White : Color.Red, 2);
            }
        }
        
        // Draw building name at cursor
        string buildingName = definition.Name;
        Vector2 nameSize = font.MeasureString(buildingName);
        Vector2 worldPosCenter = new Vector2(
            (tilePosition.X + definition.TileWidth / 2f) * tileSize,
            (tilePosition.Y - 1) * tileSize
        );
        Vector2 screenPosCenter = (worldPosCenter - cameraPosition) * zoom;
        Vector2 namePos = new Vector2(
            screenPosCenter.X - (nameSize.X / 2),
            screenPosCenter.Y
        );
        
        DrawTextWithShadow(spriteBatch, font, buildingName, namePos, isValid ? Color.White : Color.Red);
        
        // Draw hint at bottom of screen
        string hint = "Left Click: Place | Right Click/Esc: Cancel";
        Vector2 hintSize = font.MeasureString(hint);
        Vector2 hintPos = new Vector2(
            (graphicsDevice.Viewport.Width - hintSize.X) / 2,
            graphicsDevice.Viewport.Height - hintSize.Y - 20
        );
        
        // Draw hint background
        Rectangle hintBg = new Rectangle(
            (int)hintPos.X - 10, 
            (int)hintPos.Y - 5, 
            (int)hintSize.X + 20, 
            (int)hintSize.Y + 10
        );
        spriteBatch.Draw(_pixelTexture, hintBg, Color.Black * 0.7f);
        DrawTextWithShadow(spriteBatch, font, hint, hintPos, Color.White);
    }
    
    private string GetMaterialsString(BuildingDefinition building)
    {
        var parts = new List<string>();
        foreach (var material in building.MaterialCosts)
        {
            int owned = _inventory.GetItemCount(material.Key);
            parts.Add($"{material.Key}: {owned}/{material.Value}");
        }
        return parts.Count > 0 ? string.Join(", ", parts) : "No materials needed";
    }
    
    private bool HasAllMaterials(BuildingDefinition building)
    {
        foreach (var material in building.MaterialCosts)
        {
            if (_inventory.GetItemCount(material.Key) < material.Value)
            {
                return false;
            }
        }
        return true;
    }
    
    private void DrawTextWithShadow(SpriteBatch spriteBatch, SpriteFont font, string text, 
                                   Vector2 position, Color color, float scale = 1.0f)
    {
        // Draw shadow
        spriteBatch.DrawString(font, text, position + Vector2.One, Color.Black, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
        // Draw text
        spriteBatch.DrawString(font, text, position, color, 0f, Vector2.Zero, scale, SpriteEffects.None, 0f);
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
    
    public bool IsActive_Menu => _isActive;
}
