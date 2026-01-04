using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Magic;
using MoonBrookRidge.Items.Inventory;
using System.Collections.Generic;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Alchemy brewing interface UI for creating potions
/// </summary>
public class AlchemyMenu
{
    private bool _isActive;
    private AlchemySystem _alchemySystem;
    private InventorySystem _inventory;
    private List<PotionRecipe> _recipes;
    private int _selectedRecipeIndex;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture;
    
    private const int MENU_WIDTH = 650;
    private const int MENU_HEIGHT = 550;
    private const int RECIPE_HEIGHT = 75;
    private const int PADDING = 20;
    
    public AlchemyMenu(AlchemySystem alchemySystem, InventorySystem inventory)
    {
        _alchemySystem = alchemySystem;
        _inventory = inventory;
        _recipes = new List<PotionRecipe>();
        _selectedRecipeIndex = 0;
        _isActive = false;
    }
    
    public void Show()
    {
        _isActive = true;
        _recipes = _alchemySystem.GetAllRecipes();
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
        
        // Brew selected potion
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (_recipes.Count > 0 && _selectedRecipeIndex >= 0 && _selectedRecipeIndex < _recipes.Count)
            {
                PotionRecipe selectedRecipe = _recipes[_selectedRecipeIndex];
                var potion = _alchemySystem.BrewPotion(selectedRecipe.Id, _inventory);
                if (potion != null)
                {
                    // Successfully brewed - potion added to inventory
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
        spriteBatch.Draw(_pixelTexture, menuRect, new Color(40, 50, 40));
        DrawBorder(spriteBatch, menuRect, Color.LimeGreen, 3);
        
        // Draw title
        string title = "Alchemy Brewing";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(menuX + (MENU_WIDTH - titleSize.X) / 2, menuY + PADDING);
        DrawTextWithShadow(spriteBatch, font, title, titlePos, Color.LimeGreen);
        
        // Draw recipes list
        int recipeStartY = menuY + PADDING * 3;
        int maxVisibleRecipes = (MENU_HEIGHT - PADDING * 5) / RECIPE_HEIGHT;
        
        if (_recipes.Count == 0)
        {
            string noRecipes = "No potion recipes discovered yet";
            Vector2 noRecipesSize = font.MeasureString(noRecipes);
            Vector2 noRecipesPos = new Vector2(menuX + (MENU_WIDTH - noRecipesSize.X) / 2, 
                                              menuY + MENU_HEIGHT / 2);
            DrawTextWithShadow(spriteBatch, font, noRecipes, noRecipesPos, Color.Gray);
        }
        else
        {
            for (int i = 0; i < _recipes.Count && i < maxVisibleRecipes; i++)
            {
                PotionRecipe recipe = _recipes[i];
                int recipeY = recipeStartY + i * RECIPE_HEIGHT;
                bool isSelected = (i == _selectedRecipeIndex);
                bool canBrew = _alchemySystem.CanBrewPotion(recipe.Id, _inventory);
                
                // Draw recipe background
                Rectangle recipeRect = new Rectangle(menuX + PADDING, recipeY, 
                                                    MENU_WIDTH - PADDING * 2, RECIPE_HEIGHT - 5);
                Color bgColor = isSelected ? new Color(60, 80, 60) : new Color(40, 60, 40);
                spriteBatch.Draw(_pixelTexture, recipeRect, bgColor);
                
                if (isSelected)
                {
                    DrawBorder(spriteBatch, recipeRect, Color.YellowGreen, 2);
                }
                
                // Draw potion icon
                string potionIcon = GetPotionEffectIcon(recipe.Result.Effect);
                Vector2 iconPos = new Vector2(recipeRect.X + 10, recipeRect.Y + 10);
                DrawTextWithShadow(spriteBatch, font, potionIcon, iconPos, GetPotionEffectColor(recipe.Result.Effect));
                
                // Draw potion name
                Color textColor = canBrew ? Color.White : Color.Gray;
                Vector2 namePos = new Vector2(recipeRect.X + 50, recipeRect.Y + 5);
                DrawTextWithShadow(spriteBatch, font, recipe.Name, namePos, textColor);
                
                // Draw potion description
                Vector2 descPos = new Vector2(recipeRect.X + 50, recipeRect.Y + 25);
                DrawTextWithShadow(spriteBatch, font, recipe.Result.Description, descPos, 
                                  Color.LightGray * 0.8f);
                
                // Draw ingredients
                string ingredients = GetIngredientsString(recipe);
                Vector2 ingredientsPos = new Vector2(recipeRect.X + 50, recipeRect.Y + 45);
                DrawTextWithShadow(spriteBatch, font, ingredients, ingredientsPos, 
                                  canBrew ? Color.LightGreen : Color.OrangeRed);
                
                // Draw brewability status
                string status = canBrew ? "[X] Can Brew" : "[ ] Missing Items";
                Vector2 statusSize = font.MeasureString(status);
                Vector2 statusPos = new Vector2(recipeRect.Right - statusSize.X - 10, 
                                                recipeRect.Y + (recipeRect.Height - statusSize.Y) / 2);
                DrawTextWithShadow(spriteBatch, font, status, statusPos, 
                                  canBrew ? Color.LimeGreen : Color.Red);
            }
        }
        
        // Draw controls hint at bottom
        string hint = "Up/Down: Navigate | Enter: Brew Potion | Esc: Close";
        Vector2 hintSize = font.MeasureString(hint);
        Vector2 hintPos = new Vector2(menuX + (MENU_WIDTH - hintSize.X) / 2, 
                                     menuY + MENU_HEIGHT - PADDING - hintSize.Y);
        DrawTextWithShadow(spriteBatch, font, hint, hintPos, Color.LightGray);
    }
    
    private string GetIngredientsString(PotionRecipe recipe)
    {
        var parts = new List<string>();
        foreach (var ingredient in recipe.Ingredients)
        {
            int owned = _inventory.GetItemCount(ingredient.Key);
            parts.Add($"{ingredient.Key}: {owned}/{ingredient.Value}");
        }
        return string.Join(", ", parts);
    }
    
    private string GetPotionEffectIcon(PotionEffect effect)
    {
        return effect switch
        {
            PotionEffect.Health => "+",
            PotionEffect.Mana => "*",
            PotionEffect.Energy => "!",
            PotionEffect.SpeedBuff => ">",
            PotionEffect.StrengthBuff => "#",
            PotionEffect.LuckBuff => "$",
            PotionEffect.NightVision => "O",
            PotionEffect.WaterBreathing => "~",
            _ => "o"
        };
    }
    
    private Color GetPotionEffectColor(PotionEffect effect)
    {
        return effect switch
        {
            PotionEffect.Health => Color.Red,
            PotionEffect.Mana => Color.Cyan,
            PotionEffect.Energy => Color.Yellow,
            PotionEffect.SpeedBuff => Color.LightBlue,
            PotionEffect.StrengthBuff => Color.Orange,
            PotionEffect.LuckBuff => Color.Gold,
            PotionEffect.NightVision => Color.Purple,
            PotionEffect.WaterBreathing => Color.LightSeaGreen,
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
