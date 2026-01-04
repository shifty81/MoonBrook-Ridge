using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.World.Mining;
using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Underground Crafting Menu - Core Keeper-inspired tiered workbench crafting
/// Shows available tiers, recipes, and station placement
/// </summary>
public class UndergroundCraftingMenu
{
    private bool _isActive;
    private UndergroundCraftingSystem _craftingSystem;
    private InventorySystem _inventory;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture;
    
    // UI State
    private int _selectedTierIndex;
    private List<WorkbenchTier> _availableTiers;
    private List<string> _currentRecipes;
    private int _selectedRecipeIndex;
    
    // UI Layout
    private const int MENU_WIDTH = 800;
    private const int MENU_HEIGHT = 600;
    private const int TIER_PANEL_WIDTH = 200;
    private const int RECIPE_PANEL_WIDTH = 580;
    private const int PADDING = 20;
    private const int TIER_HEIGHT = 60;
    private const int RECIPE_HEIGHT = 80;
    
    // Colors
    private readonly Color BACKGROUND_COLOR = new Color(20, 20, 25, 230);
    private readonly Color PANEL_COLOR = new Color(40, 40, 50, 255);
    private readonly Color SELECTED_COLOR = new Color(80, 120, 200, 255);
    private readonly Color TEXT_COLOR = Color.White;
    private readonly Color LOCKED_COLOR = new Color(100, 100, 100, 255);
    private readonly Color UNLOCKED_COLOR = new Color(60, 200, 100, 255);
    
    public bool IsActive => _isActive;
    
    public UndergroundCraftingMenu(UndergroundCraftingSystem craftingSystem, InventorySystem inventory)
    {
        _craftingSystem = craftingSystem;
        _inventory = inventory;
        _isActive = false;
        _selectedTierIndex = 0;
        _selectedRecipeIndex = 0;
        _availableTiers = new List<WorkbenchTier>();
        _currentRecipes = new List<string>();
    }
    
    public void Initialize(Texture2D pixelTexture)
    {
        _pixelTexture = pixelTexture;
    }
    
    public void Show()
    {
        _isActive = true;
        RefreshAvailableTiers();
        _selectedTierIndex = 0;
        UpdateRecipesForSelectedTier();
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
    
    private void RefreshAvailableTiers()
    {
        _availableTiers.Clear();
        
        // Add all tiers up to and including the current max tier
        foreach (WorkbenchTier tier in Enum.GetValues(typeof(WorkbenchTier)))
        {
            if (tier <= _craftingSystem.CurrentMaxTier)
            {
                _availableTiers.Add(tier);
            }
        }
        
        // Ensure we have at least the basic tier
        if (_availableTiers.Count == 0)
        {
            _availableTiers.Add(WorkbenchTier.Basic);
        }
    }
    
    private void UpdateRecipesForSelectedTier()
    {
        if (_selectedTierIndex >= 0 && _selectedTierIndex < _availableTiers.Count)
        {
            WorkbenchTier selectedTier = _availableTiers[_selectedTierIndex];
            _currentRecipes = _craftingSystem.GetRecipesForTier(selectedTier);
            _selectedRecipeIndex = 0;
        }
        else
        {
            _currentRecipes.Clear();
        }
    }
    
    public void Update(GameTime gameTime)
    {
        if (!_isActive) return;
        
        KeyboardState keyboardState = Keyboard.GetState();
        
        // Close menu
        if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            Hide();
        }
        
        // Navigate tiers (Left/Right)
        if (keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
        {
            _selectedTierIndex--;
            if (_selectedTierIndex < 0)
                _selectedTierIndex = _availableTiers.Count - 1;
            UpdateRecipesForSelectedTier();
        }
        
        if (keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
        {
            _selectedTierIndex++;
            if (_selectedTierIndex >= _availableTiers.Count)
                _selectedTierIndex = 0;
            UpdateRecipesForSelectedTier();
        }
        
        // Navigate recipes (Up/Down)
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedRecipeIndex--;
            if (_selectedRecipeIndex < 0)
                _selectedRecipeIndex = Math.Max(0, _currentRecipes.Count - 1);
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedRecipeIndex++;
            if (_selectedRecipeIndex >= _currentRecipes.Count)
                _selectedRecipeIndex = 0;
        }
        
        // Craft recipe (Enter) - TODO: Implement actual crafting
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            CraftSelectedRecipe();
        }
        
        _previousKeyboardState = keyboardState;
    }
    
    private void CraftSelectedRecipe()
    {
        if (_selectedRecipeIndex >= 0 && _selectedRecipeIndex < _currentRecipes.Count)
        {
            string recipeName = _currentRecipes[_selectedRecipeIndex];
            // TODO: Implement actual crafting logic
            // For now, just log that crafting was attempted
            Console.WriteLine($"Attempting to craft: {recipeName}");
        }
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
        string title = "Underground Workbench";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePosition = new Vector2(
            menuX + (MENU_WIDTH - titleSize.X) / 2,
            menuY + PADDING
        );
        spriteBatch.DrawString(font, title, titlePosition, TEXT_COLOR);
        
        int contentY = menuY + PADDING * 2 + (int)titleSize.Y;
        
        // Draw tier selection panel (left side)
        DrawTierPanel(spriteBatch, font, menuX + PADDING, contentY);
        
        // Draw recipe panel (right side)
        DrawRecipePanel(spriteBatch, font, menuX + PADDING + TIER_PANEL_WIDTH + PADDING, contentY);
        
        // Draw controls help
        DrawControls(spriteBatch, font, menuX, menuY + MENU_HEIGHT - PADDING * 2);
    }
    
    private void DrawTierPanel(SpriteBatch spriteBatch, SpriteFont font, int x, int y)
    {
        // Panel background
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(x, y, TIER_PANEL_WIDTH, MENU_HEIGHT - 120),
            PANEL_COLOR);
        
        // Panel title
        string panelTitle = "Tiers";
        spriteBatch.DrawString(font, panelTitle, new Vector2(x + 10, y + 10), TEXT_COLOR);
        
        // Draw tiers
        int tierY = y + 40;
        for (int i = 0; i < _availableTiers.Count; i++)
        {
            WorkbenchTier tier = _availableTiers[i];
            bool isSelected = i == _selectedTierIndex;
            bool isUnlocked = tier <= _craftingSystem.CurrentMaxTier;
            
            // Tier background
            Color bgColor = isSelected ? SELECTED_COLOR : (isUnlocked ? UNLOCKED_COLOR : LOCKED_COLOR);
            spriteBatch.Draw(_pixelTexture,
                new Rectangle(x + 5, tierY, TIER_PANEL_WIDTH - 10, TIER_HEIGHT - 5),
                bgColor);
            
            // Tier name
            string tierName = tier.ToString();
            spriteBatch.DrawString(font, tierName, new Vector2(x + 15, tierY + 10), TEXT_COLOR);
            
            // Lock indicator
            if (!isUnlocked)
            {
                spriteBatch.DrawString(font, "[LOCKED]", new Vector2(x + 15, tierY + 30), Color.Red);
            }
            
            tierY += TIER_HEIGHT;
        }
        
        // Current tier info
        if (_availableTiers.Count > 0 && _selectedTierIndex < _availableTiers.Count)
        {
            WorkbenchTier currentTier = _availableTiers[_selectedTierIndex];
            string tierInfo = $"Max Tier: {_craftingSystem.CurrentMaxTier}";
            spriteBatch.DrawString(font, tierInfo, 
                new Vector2(x + 10, y + MENU_HEIGHT - 160), TEXT_COLOR);
        }
    }
    
    private void DrawRecipePanel(SpriteBatch spriteBatch, SpriteFont font, int x, int y)
    {
        // Panel background
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(x, y, RECIPE_PANEL_WIDTH, MENU_HEIGHT - 120),
            PANEL_COLOR);
        
        // Panel title
        string panelTitle = $"Recipes ({_currentRecipes.Count} available)";
        spriteBatch.DrawString(font, panelTitle, new Vector2(x + 10, y + 10), TEXT_COLOR);
        
        if (_currentRecipes.Count == 0)
        {
            spriteBatch.DrawString(font, "No recipes available",
                new Vector2(x + 20, y + 50), LOCKED_COLOR);
            return;
        }
        
        // Draw recipes (with scrolling if needed)
        int recipeY = y + 40;
        int maxVisibleRecipes = (MENU_HEIGHT - 160) / RECIPE_HEIGHT;
        int startIndex = Math.Max(0, _selectedRecipeIndex - maxVisibleRecipes / 2);
        int endIndex = Math.Min(_currentRecipes.Count, startIndex + maxVisibleRecipes);
        
        for (int i = startIndex; i < endIndex; i++)
        {
            string recipeName = _currentRecipes[i];
            bool isSelected = i == _selectedRecipeIndex;
            
            // Recipe background
            Color bgColor = isSelected ? SELECTED_COLOR : new Color(50, 50, 60, 255);
            spriteBatch.Draw(_pixelTexture,
                new Rectangle(x + 5, recipeY, RECIPE_PANEL_WIDTH - 10, RECIPE_HEIGHT - 5),
                bgColor);
            
            // Recipe name
            spriteBatch.DrawString(font, recipeName, new Vector2(x + 15, recipeY + 10), TEXT_COLOR);
            
            // TODO: Show recipe requirements and availability
            spriteBatch.DrawString(font, "Requirements: TBD",
                new Vector2(x + 15, recipeY + 35), new Color(150, 150, 150));
            
            recipeY += RECIPE_HEIGHT;
        }
    }
    
    private void DrawControls(SpriteBatch spriteBatch, SpriteFont font, int x, int y)
    {
        string controls = "◄► Navigate Tiers | ▲▼ Select Recipe | ENTER Craft | ESC Close";
        Vector2 controlsSize = font.MeasureString(controls);
        spriteBatch.DrawString(font, controls,
            new Vector2(x + (MENU_WIDTH - controlsSize.X) / 2, y),
            new Color(200, 200, 200));
    }
}
