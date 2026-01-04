using System;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Skills;
using MoonBrookRidge.Items.Crafting;
using MoonBrookRidge.Characters;
using System.Collections.Generic;
using System.Linq;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Unified Player Menu with tabs for Inventory, Skills, Social, Crafting, and Family
/// Inspired by Stardew Valley's tabbed menu system
/// </summary>
public class PlayerMenu : TabbedMenu
{
    private InventorySystem _inventory;
    private SkillTreeSystem _skillSystem;
    private CraftingSystem _craftingSystem;
    private MarriageSystem _marriageSystem;
    private FamilySystem _familySystem;
    
    // Tab-specific state
    private int _selectedInventorySlot;
    private int _selectedCraftingRecipe;
    private int _selectedChildIndex;
    private SkillCategory _selectedSkillCategory;
    
    public PlayerMenu(InventorySystem inventory, SkillTreeSystem skillSystem, 
                     CraftingSystem craftingSystem, MarriageSystem marriageSystem, 
                     FamilySystem familySystem)
    {
        _inventory = inventory;
        _skillSystem = skillSystem;
        _craftingSystem = craftingSystem;
        _marriageSystem = marriageSystem;
        _familySystem = familySystem;
        
        // Initialize tabs
        AddTab("Inventory", "View and manage your items");
        AddTab("Skills", "View skills and experience");
        AddTab("Social", "View relationships and family");
        AddTab("Crafting", "Craft items from recipes");
        
        _selectedInventorySlot = 0;
        _selectedCraftingRecipe = 0;
        _selectedChildIndex = 0;
        _selectedSkillCategory = SkillCategory.Farming;
    }
    
    protected override void UpdateTabContent(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
    {
        switch (_selectedTabIndex)
        {
            case 0: // Inventory
                UpdateInventoryTab(keyboardState, mouseState);
                break;
            case 1: // Skills
                UpdateSkillsTab(keyboardState, mouseState);
                break;
            case 2: // Social/Family
                UpdateSocialTab(keyboardState, mouseState);
                break;
            case 3: // Crafting
                UpdateCraftingTab(keyboardState, mouseState);
                break;
        }
    }
    
    private void UpdateInventoryTab(KeyboardState keyboardState, MouseState mouseState)
    {
        // Navigate inventory with arrow keys
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedInventorySlot -= 6; // 6 columns
            if (_selectedInventorySlot < 0) _selectedInventorySlot = 0;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedInventorySlot += 6;
            if (_selectedInventorySlot >= 36) _selectedInventorySlot = 35;
        }
        
        if (keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
        {
            _selectedInventorySlot--;
            if (_selectedInventorySlot < 0) _selectedInventorySlot = 0;
        }
        
        if (keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
        {
            _selectedInventorySlot++;
            if (_selectedInventorySlot >= 36) _selectedInventorySlot = 35;
        }
        
        // Mouse click on inventory slots
        HandleInventoryMouseClick(mouseState);
    }
    
    private void HandleInventoryMouseClick(MouseState mouseState)
    {
        if (mouseState.LeftButton == ButtonState.Pressed && 
            _previousMouseState.LeftButton == ButtonState.Released)
        {
            int menuX = GetMenuX();
            int menuY = GetMenuY() + TAB_HEIGHT;
            
            // Calculate slot grid position
            int slotSize = 64;
            int slotsPerRow = 6;
            int startX = menuX + PADDING + 150; // Offset for item details
            int startY = menuY + PADDING + 60;
            
            for (int i = 0; i < 36; i++)
            {
                int row = i / slotsPerRow;
                int col = i % slotsPerRow;
                int slotX = startX + col * (slotSize + 10);
                int slotY = startY + row * (slotSize + 10);
                
                Rectangle slotBounds = new Rectangle(slotX, slotY, slotSize, slotSize);
                if (slotBounds.Contains(mouseState.Position))
                {
                    _selectedInventorySlot = i;
                    break;
                }
            }
        }
    }
    
    private void UpdateSkillsTab(KeyboardState keyboardState, MouseState mouseState)
    {
        // Navigate skills categories
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            int current = (int)_selectedSkillCategory;
            current--;
            if (current < 0) current = 5; // 6 categories
            _selectedSkillCategory = (SkillCategory)current;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            int current = (int)_selectedSkillCategory;
            current++;
            if (current > 5) current = 0;
            _selectedSkillCategory = (SkillCategory)current;
        }
        
        // Mouse click on skill categories
        HandleSkillCategoryMouseClick(mouseState);
    }
    
    private void HandleSkillCategoryMouseClick(MouseState mouseState)
    {
        if (mouseState.LeftButton == ButtonState.Pressed && 
            _previousMouseState.LeftButton == ButtonState.Released)
        {
            int menuX = GetMenuX();
            int menuY = GetMenuY() + TAB_HEIGHT;
            
            int categoryY = menuY + PADDING + 60;
            int categoryHeight = 50;
            
            for (int i = 0; i < 6; i++)
            {
                Rectangle categoryBounds = new Rectangle(
                    menuX + PADDING, 
                    categoryY + i * (categoryHeight + 10), 
                    200, 
                    categoryHeight
                );
                
                if (categoryBounds.Contains(mouseState.Position))
                {
                    _selectedSkillCategory = (SkillCategory)i;
                    break;
                }
            }
        }
    }
    
    private void UpdateSocialTab(KeyboardState keyboardState, MouseState mouseState)
    {
        // Navigate children if any
        if (_familySystem.ChildCount > 0)
        {
            if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _selectedChildIndex--;
                if (_selectedChildIndex < 0) _selectedChildIndex = _familySystem.ChildCount - 1;
            }
            
            if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _selectedChildIndex++;
                if (_selectedChildIndex >= _familySystem.ChildCount) _selectedChildIndex = 0;
            }
        }
    }
    
    private void UpdateCraftingTab(KeyboardState keyboardState, MouseState mouseState)
    {
        var recipes = _craftingSystem.GetAllRecipes().ToList();
        
        if (recipes.Count > 0)
        {
            if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _selectedCraftingRecipe--;
                if (_selectedCraftingRecipe < 0) _selectedCraftingRecipe = recipes.Count - 1;
            }
            
            if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _selectedCraftingRecipe++;
                if (_selectedCraftingRecipe >= recipes.Count) _selectedCraftingRecipe = 0;
            }
            
            // Craft with Enter or mouse click
            if ((keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter)) ||
                HandleCraftButtonClick(mouseState))
            {
                var selectedRecipe = recipes[_selectedCraftingRecipe];
                if (_craftingSystem.CanCraft(selectedRecipe.Name, _inventory))
                {
                    _craftingSystem.Craft(selectedRecipe.Name, _inventory);
                }
            }
        }
        
        // Mouse click on recipe list
        HandleRecipeMouseClick(mouseState, recipes);
    }
    
    private void HandleRecipeMouseClick(MouseState mouseState, List<Recipe> recipes)
    {
        if (mouseState.LeftButton == ButtonState.Pressed && 
            _previousMouseState.LeftButton == ButtonState.Released)
        {
            int menuX = GetMenuX();
            int menuY = GetMenuY() + TAB_HEIGHT;
            
            int recipeY = menuY + PADDING + 60;
            int recipeHeight = 45;
            
            for (int i = 0; i < recipes.Count && i < 10; i++)
            {
                Rectangle recipeBounds = new Rectangle(
                    menuX + PADDING, 
                    recipeY + i * recipeHeight, 
                    400, 
                    recipeHeight - 5
                );
                
                if (recipeBounds.Contains(mouseState.Position))
                {
                    _selectedCraftingRecipe = i;
                    break;
                }
            }
        }
    }
    
    private bool HandleCraftButtonClick(MouseState mouseState)
    {
        if (mouseState.LeftButton == ButtonState.Pressed && 
            _previousMouseState.LeftButton == ButtonState.Released)
        {
            int menuX = GetMenuX();
            int menuY = GetMenuY() + TAB_HEIGHT;
            
            // Craft button in bottom right
            Rectangle craftButton = new Rectangle(
                menuX + MENU_WIDTH - PADDING - 150,
                menuY + MENU_HEIGHT - TAB_HEIGHT - PADDING - 50,
                150,
                50
            );
            
            return craftButton.Contains(mouseState.Position);
        }
        return false;
    }
    
    protected override void DrawTabContent(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        switch (_selectedTabIndex)
        {
            case 0: // Inventory
                DrawInventoryTab(spriteBatch, font, x, y, width, height);
                break;
            case 1: // Skills
                DrawSkillsTab(spriteBatch, font, x, y, width, height);
                break;
            case 2: // Social/Family
                DrawSocialTab(spriteBatch, font, x, y, width, height);
                break;
            case 3: // Crafting
                DrawCraftingTab(spriteBatch, font, x, y, width, height);
                break;
        }
        
        // Draw instructions at bottom
        DrawInstructions(spriteBatch, font, x, y, width, height);
    }
    
    private void DrawInventoryTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        // Title
        string title = "Inventory";
        Vector2 titleSize = font.MeasureString(title);
        spriteBatch.DrawString(font, title, new Vector2(x + PADDING, y + PADDING), Color.White);
        
        // Draw inventory grid (6x6)
        int slotSize = 64;
        int slotsPerRow = 6;
        int startX = x + PADDING + 150;
        int startY = y + PADDING + 60;
        
        var slots = _inventory.GetSlots();
        for (int i = 0; i < 36; i++)
        {
            int row = i / slotsPerRow;
            int col = i % slotsPerRow;
            int slotX = startX + col * (slotSize + 10);
            int slotY = startY + row * (slotSize + 10);
            
            // Slot background
            Color slotColor = (i == _selectedInventorySlot) ? new Color(100, 100, 100) : new Color(50, 50, 50);
            spriteBatch.Draw(_pixelTexture, new Rectangle(slotX, slotY, slotSize, slotSize), slotColor);
            DrawBorder(spriteBatch, slotX, slotY, slotSize, slotSize, 2, Color.White);
            
            // Draw item if slot has one
            if (!slots[i].IsEmpty)
            {
                string itemName = slots[i].Item.Name.Length > 8 ? slots[i].Item.Name.Substring(0, 8) : slots[i].Item.Name;
                spriteBatch.DrawString(font, itemName, new Vector2(slotX + 5, slotY + 5), Color.White);
                spriteBatch.DrawString(font, $"x{slots[i].Quantity}", new Vector2(slotX + 5, slotY + 35), Color.Yellow);
            }
        }
        
        // Draw selected item details on left
        if (!slots[_selectedInventorySlot].IsEmpty)
        {
            var item = slots[_selectedInventorySlot].Item;
            int detailX = x + PADDING;
            int detailY = y + PADDING + 60;
            
            spriteBatch.DrawString(font, "Selected Item:", new Vector2(detailX, detailY), Color.LightGray);
            spriteBatch.DrawString(font, item.Name, new Vector2(detailX, detailY + 30), Color.White);
            spriteBatch.DrawString(font, $"Type: {item.Type}", new Vector2(detailX, detailY + 60), Color.LightBlue);
            spriteBatch.DrawString(font, $"Qty: {slots[_selectedInventorySlot].Quantity}", new Vector2(detailX, detailY + 90), Color.Yellow);
        }
    }
    
    private void DrawSkillsTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        string title = "Skills";
        spriteBatch.DrawString(font, title, new Vector2(x + PADDING, y + PADDING), Color.White);
        
        // Draw skill categories on left
        int categoryY = y + PADDING + 60;
        string[] categoryNames = { "Farming", "Combat", "Magic", "Crafting", "Mining", "Fishing" };
        
        for (int i = 0; i < 6; i++)
        {
            SkillCategory category = (SkillCategory)i;
            bool isSelected = (category == _selectedSkillCategory);
            
            Color bgColor = isSelected ? new Color(80, 70, 60) : new Color(50, 45, 40);
            spriteBatch.Draw(_pixelTexture, 
                new Rectangle(x + PADDING, categoryY + i * 60, 200, 50), 
                bgColor);
            
            Color textColor = isSelected ? Color.Yellow : Color.White;
            spriteBatch.DrawString(font, categoryNames[i], 
                new Vector2(x + PADDING + 10, categoryY + i * 60 + 15), textColor);
            
            // Draw level and XP
            int level = _skillSystem.GetSkillLevel(category);
            float xp = _skillSystem.GetSkillExperience(category);
            spriteBatch.DrawString(font, $"Lv {level}", 
                new Vector2(x + PADDING + 150, categoryY + i * 60 + 15), Color.LightGreen);
        }
        
        // Draw selected category details on right
        DrawSkillCategoryDetails(spriteBatch, font, x + PADDING + 250, categoryY, width - 300);
    }
    
    private void DrawSkillCategoryDetails(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int maxWidth)
    {
        var skillTree = _skillSystem.GetSkillTree(_selectedSkillCategory);
        int level = _skillSystem.GetSkillLevel(_selectedSkillCategory);
        float currentXP = _skillSystem.GetSkillExperience(_selectedSkillCategory);
        float xpForNextLevel = GetRequiredExperienceForLevel(level + 1);
        int skillPoints = _skillSystem.AvailableSkillPoints;
        
        spriteBatch.DrawString(font, $"Level: {level}", new Vector2(x, y), Color.White);
        spriteBatch.DrawString(font, $"XP: {(int)currentXP} / {(int)xpForNextLevel}", new Vector2(x, y + 30), Color.LightBlue);
        spriteBatch.DrawString(font, $"Skill Points: {skillPoints}", new Vector2(x, y + 60), Color.Yellow);
        
        // XP Progress bar
        int barWidth = 400;
        int barHeight = 20;
        float progress = currentXP / xpForNextLevel;
        
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y + 100, barWidth, barHeight), new Color(30, 30, 30));
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y + 100, (int)(barWidth * progress), barHeight), Color.Green);
        DrawBorder(spriteBatch, x, y + 100, barWidth, barHeight, 2, Color.White);
        
        // List skills for this category
        var skills = skillTree.GetAllSkills();
        int skillY = y + 140;
        int displayedSkills = 0;
        
        foreach (var skill in skills)
        {
            if (displayedSkills >= 6) break; // Limit display
            
            bool isUnlocked = skillTree.IsSkillUnlocked(skill.Id);
            Color skillColor = isUnlocked ? Color.LightGreen : Color.Gray;
            
            string skillText = $"{skill.Name} (Req Lv {skill.RequiredLevel})";
            spriteBatch.DrawString(font, skillText, new Vector2(x, skillY), skillColor);
            
            skillY += 30;
            displayedSkills++;
        }
    }
    
    // Helper method to calculate required XP for a level (matches SkillTreeSystem formula)
    private float GetRequiredExperienceForLevel(int level)
    {
        return 100f * MathF.Pow(level, 1.5f);
    }
    
    private void DrawSocialTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        string title = "Social & Family";
        spriteBatch.DrawString(font, title, new Vector2(x + PADDING, y + PADDING), Color.White);
        
        int contentY = y + PADDING + 60;
        
        // Marriage status
        if (_marriageSystem.IsMarried && _marriageSystem.Spouse != null)
        {
            spriteBatch.DrawString(font, $"Married to: {_marriageSystem.Spouse.Name}", 
                new Vector2(x + PADDING, contentY), Color.LightGreen);
            spriteBatch.DrawString(font, $"Days married: {_marriageSystem.DaysSinceMarriage}", 
                new Vector2(x + PADDING, contentY + 30), Color.White);
            
            string greeting = _marriageSystem.GetSpouseGreeting();
            spriteBatch.DrawString(font, $"\"{greeting}\"", 
                new Vector2(x + PADDING, contentY + 70), Color.LightYellow);
            
            contentY += 120;
        }
        else
        {
            spriteBatch.DrawString(font, "Not married", new Vector2(x + PADDING, contentY), Color.Gray);
            contentY += 40;
        }
        
        // Divider
        spriteBatch.Draw(_pixelTexture, new Rectangle(x + PADDING, contentY, width - PADDING * 2, 2), Color.Gray);
        contentY += 20;
        
        // Children
        spriteBatch.DrawString(font, $"Children ({_familySystem.ChildCount}/2):", 
            new Vector2(x + PADDING, contentY), Color.White);
        contentY += 40;
        
        if (_familySystem.ChildCount == 0)
        {
            spriteBatch.DrawString(font, "No children yet", new Vector2(x + PADDING + 20, contentY), Color.Gray);
        }
        else
        {
            for (int i = 0; i < _familySystem.ChildCount; i++)
            {
                var child = _familySystem.GetChild(i);
                bool isSelected = (i == _selectedChildIndex);
                
                Color bgColor = isSelected ? new Color(70, 60, 50) : BackgroundColor;
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(x + PADDING, contentY, width - PADDING * 2, 90), bgColor);
                
                Color nameColor = isSelected ? Color.Yellow : Color.White;
                spriteBatch.DrawString(font, $"{child.Name} ({(child.IsBoy ? "Boy" : "Girl")})", 
                    new Vector2(x + PADDING + 10, contentY + 10), nameColor);
                spriteBatch.DrawString(font, $"Stage: {child.Stage} ({child.DaysOld} days)", 
                    new Vector2(x + PADDING + 10, contentY + 40), Color.LightBlue);
                
                contentY += 95;
            }
        }
    }
    
    private void DrawCraftingTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        string title = "Crafting";
        spriteBatch.DrawString(font, title, new Vector2(x + PADDING, y + PADDING), Color.White);
        
        var recipes = _craftingSystem.GetAllRecipes().ToList();
        
        if (recipes.Count == 0)
        {
            spriteBatch.DrawString(font, "No recipes available", 
                new Vector2(x + PADDING, y + PADDING + 60), Color.Gray);
            return;
        }
        
        // Draw recipe list on left
        int recipeY = y + PADDING + 60;
        int displayCount = System.Math.Min(recipes.Count, 10);
        
        for (int i = 0; i < displayCount; i++)
        {
            bool isSelected = (i == _selectedCraftingRecipe);
            bool canCraft = _craftingSystem.CanCraft(recipes[i].Name, _inventory);
            
            Color bgColor = isSelected ? new Color(80, 70, 60) : new Color(50, 45, 40);
            spriteBatch.Draw(_pixelTexture, 
                new Rectangle(x + PADDING, recipeY + i * 45, 400, 40), bgColor);
            
            Color textColor = canCraft ? Color.White : Color.Gray;
            if (isSelected) textColor = Color.Yellow;
            
            spriteBatch.DrawString(font, recipes[i].OutputName, 
                new Vector2(x + PADDING + 10, recipeY + i * 45 + 10), textColor);
        }
        
        // Draw selected recipe details on right
        if (_selectedCraftingRecipe < recipes.Count)
        {
            var recipe = recipes[_selectedCraftingRecipe];
            int detailX = x + PADDING + 450;
            int detailY = y + PADDING + 60;
            
            spriteBatch.DrawString(font, "Recipe Details:", new Vector2(detailX, detailY), Color.White);
            detailY += 40;
            
            spriteBatch.DrawString(font, $"Output: {recipe.OutputName} x{recipe.OutputQuantity}", 
                new Vector2(detailX, detailY), Color.LightGreen);
            detailY += 40;
            
            spriteBatch.DrawString(font, "Ingredients:", new Vector2(detailX, detailY), Color.White);
            detailY += 30;
            
            foreach (var ingredient in recipe.Ingredients)
            {
                int owned = _inventory.GetItemCount(ingredient.Key);
                Color ingredColor = (owned >= ingredient.Value) ? Color.LightGreen : Color.Red;
                
                spriteBatch.DrawString(font, $"  {ingredient.Key} x{ingredient.Value} ({owned})", 
                    new Vector2(detailX, detailY), ingredColor);
                detailY += 25;
            }
            
            // Craft button
            bool canCraft = _craftingSystem.CanCraft(recipe.Name, _inventory);
            Rectangle craftButton = new Rectangle(
                x + width - PADDING - 150,
                y + height - PADDING - 50,
                150, 50
            );
            
            Color buttonColor = canCraft ? new Color(100, 150, 100) : new Color(80, 80, 80);
            spriteBatch.Draw(_pixelTexture, craftButton, buttonColor);
            DrawBorder(spriteBatch, craftButton.X, craftButton.Y, craftButton.Width, craftButton.Height, 2, Color.White);
            
            string buttonText = "Craft";
            Vector2 buttonTextSize = font.MeasureString(buttonText);
            spriteBatch.DrawString(font, buttonText,
                new Vector2(craftButton.X + craftButton.Width / 2 - buttonTextSize.X / 2,
                           craftButton.Y + craftButton.Height / 2 - buttonTextSize.Y / 2),
                Color.White);
        }
    }
    
    private void DrawInstructions(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        string instructions = "Tab/Q: Switch Tabs | E/Esc: Close | Mouse: Click to Interact | Arrows: Navigate";
        Vector2 instructSize = font.MeasureString(instructions);
        spriteBatch.DrawString(font, instructions,
            new Vector2(x + width / 2 - instructSize.X / 2, y + height - PADDING - 5),
            Color.LightGray);
    }
}
