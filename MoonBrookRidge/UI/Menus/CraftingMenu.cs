using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Items.Crafting;
using MoonBrookRidge.Items.Inventory;
using System.Collections.Generic;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Crafting menu UI for browsing recipes and creating items
/// </summary>
public class CraftingMenu
{
    private bool _isActive;
    private CraftingSystem _craftingSystem;
    private InventorySystem _inventory;
    private List<Recipe> _recipes;
    private int _selectedRecipeIndex;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture;
    
    private const int MENU_WIDTH = 600;
    private const int MENU_HEIGHT = 500;
    private const int RECIPE_HEIGHT = 60;
    private const int PADDING = 20;
    
    public CraftingMenu(CraftingSystem craftingSystem, InventorySystem inventory)
    {
        _craftingSystem = craftingSystem;
        _inventory = inventory;
        _recipes = new List<Recipe>();
        _selectedRecipeIndex = 0;
        _isActive = false;
    }
    
    public void Show()
    {
        _isActive = true;
        _recipes = _craftingSystem.GetAllRecipes();
        _selectedRecipeIndex = 0;
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
        
        KeyboardState keyboardState = Keyboard.GetState();
        
        // Navigation
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedRecipeIndex--;
            if (_selectedRecipeIndex < 0)
                _selectedRecipeIndex = _recipes.Count - 1;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedRecipeIndex++;
            if (_selectedRecipeIndex >= _recipes.Count)
                _selectedRecipeIndex = 0;
        }
        
        // Craft selected recipe
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (_recipes.Count > 0 && _selectedRecipeIndex >= 0 && _selectedRecipeIndex < _recipes.Count)
            {
                Recipe selectedRecipe = _recipes[_selectedRecipeIndex];
                if (_craftingSystem.Craft(selectedRecipe.Name, _inventory))
                {
                    // Successfully crafted - could add sound/visual feedback here
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
        string title = "Crafting Menu";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(menuX + (MENU_WIDTH - titleSize.X) / 2, menuY + PADDING);
        DrawTextWithShadow(spriteBatch, font, title, titlePos, Color.Gold);
        
        // Draw recipes list
        int recipeStartY = menuY + PADDING * 3;
        int maxVisibleRecipes = (MENU_HEIGHT - PADDING * 5) / RECIPE_HEIGHT;
        
        if (_recipes.Count == 0)
        {
            string noRecipes = "No recipes available";
            Vector2 noRecipesSize = font.MeasureString(noRecipes);
            Vector2 noRecipesPos = new Vector2(menuX + (MENU_WIDTH - noRecipesSize.X) / 2, 
                                              menuY + MENU_HEIGHT / 2);
            DrawTextWithShadow(spriteBatch, font, noRecipes, noRecipesPos, Color.Gray);
        }
        else
        {
            for (int i = 0; i < _recipes.Count && i < maxVisibleRecipes; i++)
            {
                Recipe recipe = _recipes[i];
                int recipeY = recipeStartY + i * RECIPE_HEIGHT;
                bool isSelected = (i == _selectedRecipeIndex);
                bool canCraft = _craftingSystem.CanCraft(recipe.Name, _inventory);
                
                // Draw recipe background
                Rectangle recipeRect = new Rectangle(menuX + PADDING, recipeY, 
                                                    MENU_WIDTH - PADDING * 2, RECIPE_HEIGHT - 5);
                Color bgColor = isSelected ? new Color(80, 80, 100) : new Color(50, 50, 60);
                spriteBatch.Draw(_pixelTexture, recipeRect, bgColor);
                
                if (isSelected)
                {
                    DrawBorder(spriteBatch, recipeRect, Color.Yellow, 2);
                }
                
                // Draw recipe name
                Color textColor = canCraft ? Color.White : Color.Gray;
                Vector2 namePos = new Vector2(recipeRect.X + 10, recipeRect.Y + 5);
                DrawTextWithShadow(spriteBatch, font, recipe.Name, namePos, textColor);
                
                // Draw ingredients
                string ingredients = GetIngredientsString(recipe);
                Vector2 ingredientsPos = new Vector2(recipeRect.X + 10, recipeRect.Y + 25);
                DrawTextWithShadow(spriteBatch, font, ingredients, ingredientsPos, 
                                  canCraft ? Color.LightGreen : Color.Red);
                
                // Draw craftability status
                string status = canCraft ? "[X] Can Craft" : "[ ] Missing Items";
                Vector2 statusSize = font.MeasureString(status);
                Vector2 statusPos = new Vector2(recipeRect.Right - statusSize.X - 10, 
                                               recipeRect.Y + (recipeRect.Height - statusSize.Y) / 2);
                DrawTextWithShadow(spriteBatch, font, status, statusPos, 
                                  canCraft ? Color.LimeGreen : Color.OrangeRed);
            }
        }
        
        // Draw controls hint at bottom
        string hint = "Up/Down: Navigate | Enter: Craft | Esc: Close";
        Vector2 hintSize = font.MeasureString(hint);
        Vector2 hintPos = new Vector2(menuX + (MENU_WIDTH - hintSize.X) / 2, 
                                     menuY + MENU_HEIGHT - PADDING - hintSize.Y);
        DrawTextWithShadow(spriteBatch, font, hint, hintPos, Color.LightGray);
    }
    
    private string GetIngredientsString(Recipe recipe)
    {
        var parts = new List<string>();
        foreach (var ingredient in recipe.Ingredients)
        {
            int owned = _inventory.GetItemCount(ingredient.Key);
            parts.Add($"{ingredient.Key}: {owned}/{ingredient.Value}");
        }
        return string.Join(", ", parts);
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
