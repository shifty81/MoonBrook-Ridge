using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Skills;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Skill tree menu UI for viewing and unlocking skills
/// </summary>
public class SkillsMenu
{
    private bool _isActive;
    private SkillTreeSystem _skillSystem;
    private SkillCategory _selectedCategory;
    private int _selectedSkillIndex;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture = null!;
    
    private const int MENU_WIDTH = 750;
    private const int MENU_HEIGHT = 600;
    private const int CATEGORY_BUTTON_WIDTH = 110;
    private const int CATEGORY_BUTTON_HEIGHT = 40;
    private const int SKILL_HEIGHT = 55;
    private const int PADDING = 20;
    
    public SkillsMenu(SkillTreeSystem skillSystem)
    {
        _skillSystem = skillSystem;
        _selectedCategory = SkillCategory.Farming;
        _selectedSkillIndex = 0;
        _isActive = false;
    }
    
    public void Show()
    {
        _isActive = true;
        _selectedSkillIndex = 0;
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
        
        var skills = _skillSystem.GetSkillTree(_selectedCategory).GetAllSkills();
        
        // Category navigation (Left/Right)
        if (keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
        {
            int categoryIndex = (int)_selectedCategory;
            categoryIndex--;
            if (categoryIndex < 0)
                categoryIndex = Enum.GetValues(typeof(SkillCategory)).Length - 1;
            _selectedCategory = (SkillCategory)categoryIndex;
            _selectedSkillIndex = 0;
        }
        
        if (keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
        {
            int categoryIndex = (int)_selectedCategory;
            categoryIndex++;
            if (categoryIndex >= Enum.GetValues(typeof(SkillCategory)).Length)
                categoryIndex = 0;
            _selectedCategory = (SkillCategory)categoryIndex;
            _selectedSkillIndex = 0;
        }
        
        // Skill navigation (Up/Down)
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedSkillIndex--;
            if (_selectedSkillIndex < 0)
                _selectedSkillIndex = skills.Count - 1;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedSkillIndex++;
            if (_selectedSkillIndex >= skills.Count)
                _selectedSkillIndex = 0;
        }
        
        // Unlock selected skill
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (skills.Count > 0 && _selectedSkillIndex >= 0 && _selectedSkillIndex < skills.Count)
            {
                Skill selectedSkill = skills[_selectedSkillIndex];
                _skillSystem.UnlockSkill(selectedSkill.Id);
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
        spriteBatch.Draw(_pixelTexture, menuRect, new Color(50, 40, 40));
        DrawBorder(spriteBatch, menuRect, Color.Orange, 3);
        
        // Draw title
        string title = "Skill Tree";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(menuX + (MENU_WIDTH - titleSize.X) / 2, menuY + PADDING);
        DrawTextWithShadow(spriteBatch, font, title, titlePos, Color.Orange);
        
        // Draw skill points
        int skillPointsY = menuY + PADDING * 2 + 20;
        string pointsText = $"Skill Points Available: {_skillSystem.AvailableSkillPoints}";
        Vector2 pointsSize = font.MeasureString(pointsText);
        Vector2 pointsPos = new Vector2(menuX + (MENU_WIDTH - pointsSize.X) / 2, skillPointsY);
        DrawTextWithShadow(spriteBatch, font, pointsText, pointsPos, Color.Gold);
        
        // Draw category buttons
        int categoryY = skillPointsY + 30;
        DrawCategoryButtons(spriteBatch, font, menuX, categoryY);
        
        // Draw XP bar for selected category
        int xpBarY = categoryY + CATEGORY_BUTTON_HEIGHT + 15;
        DrawXPBar(spriteBatch, font, menuX, xpBarY);
        
        // Draw skills list for selected category
        int skillsStartY = xpBarY + 45;
        DrawSkillsList(spriteBatch, font, menuX, skillsStartY);
        
        // Draw controls hint at bottom
        string hint = "Left/Right: Categories | Up/Down: Skills | Enter: Unlock | Esc: Close";
        Vector2 hintSize = font.MeasureString(hint);
        Vector2 hintPos = new Vector2(menuX + (MENU_WIDTH - hintSize.X) / 2, 
                                     menuY + MENU_HEIGHT - PADDING - hintSize.Y);
        DrawTextWithShadow(spriteBatch, font, hint, hintPos, Color.LightGray);
    }
    
    private void DrawCategoryButtons(SpriteBatch spriteBatch, SpriteFont font, int menuX, int y)
    {
        var categories = (SkillCategory[])Enum.GetValues(typeof(SkillCategory));
        int totalWidth = categories.Length * (CATEGORY_BUTTON_WIDTH + 5) - 5;
        int startX = menuX + (MENU_WIDTH - totalWidth) / 2;
        
        for (int i = 0; i < categories.Length; i++)
        {
            var category = categories[i];
            int buttonX = startX + i * (CATEGORY_BUTTON_WIDTH + 5);
            
            Rectangle buttonRect = new Rectangle(buttonX, y, CATEGORY_BUTTON_WIDTH, CATEGORY_BUTTON_HEIGHT);
            Color bgColor = (category == _selectedCategory) ? new Color(100, 80, 60) : new Color(60, 50, 40);
            spriteBatch.Draw(_pixelTexture, buttonRect, bgColor);
            
            if (category == _selectedCategory)
            {
                DrawBorder(spriteBatch, buttonRect, Color.Orange, 2);
            }
            else
            {
                DrawBorder(spriteBatch, buttonRect, Color.Gray, 1);
            }
            
            // Draw category name
            string catName = category.ToString();
            Vector2 textSize = font.MeasureString(catName);
            Vector2 textPos = new Vector2(buttonX + (CATEGORY_BUTTON_WIDTH - textSize.X) / 2,
                                         y + (CATEGORY_BUTTON_HEIGHT - textSize.Y) / 2);
            Color textColor = (category == _selectedCategory) ? Color.White : Color.Gray;
            DrawTextWithShadow(spriteBatch, font, catName, textPos, textColor);
        }
    }
    
    private void DrawXPBar(SpriteBatch spriteBatch, SpriteFont font, int menuX, int y)
    {
        int currentLevel = _skillSystem.GetSkillLevel(_selectedCategory);
        float currentXP = _skillSystem.GetSkillExperience(_selectedCategory);
        float requiredXP = 100f * MathF.Pow(currentLevel + 1, 1.5f);
        
        int barWidth = MENU_WIDTH - PADDING * 2;
        int barHeight = 25;
        
        // Background
        Rectangle barBg = new Rectangle(menuX + PADDING, y, barWidth, barHeight);
        spriteBatch.Draw(_pixelTexture, barBg, new Color(30, 30, 30));
        DrawBorder(spriteBatch, barBg, Color.Orange, 2);
        
        // XP fill
        float xpPercent = currentXP / requiredXP;
        int fillWidth = (int)(barWidth * xpPercent);
        Rectangle barFill = new Rectangle(menuX + PADDING, y, fillWidth, barHeight);
        spriteBatch.Draw(_pixelTexture, barFill, Color.Orange * 0.7f);
        
        // Level and XP text
        string levelText = $"Level {currentLevel} - {(int)currentXP}/{(int)requiredXP} XP";
        Vector2 textSize = font.MeasureString(levelText);
        Vector2 textPos = new Vector2(menuX + PADDING + (barWidth - textSize.X) / 2, 
                                      y + (barHeight - textSize.Y) / 2);
        DrawTextWithShadow(spriteBatch, font, levelText, textPos, Color.White);
    }
    
    private void DrawSkillsList(SpriteBatch spriteBatch, SpriteFont font, int menuX, int startY)
    {
        var skillTree = _skillSystem.GetSkillTree(_selectedCategory);
        var skills = skillTree.GetAllSkills();
        
        int maxVisibleSkills = 6;
        
        if (skills.Count == 0)
        {
            string noSkills = "No skills available in this category";
            Vector2 noSkillsSize = font.MeasureString(noSkills);
            Vector2 noSkillsPos = new Vector2(menuX + (MENU_WIDTH - noSkillsSize.X) / 2, startY + 50);
            DrawTextWithShadow(spriteBatch, font, noSkills, noSkillsPos, Color.Gray);
            return;
        }
        
        for (int i = 0; i < skills.Count && i < maxVisibleSkills; i++)
        {
            Skill skill = skills[i];
            int skillY = startY + i * SKILL_HEIGHT;
            bool isSelected = (i == _selectedSkillIndex);
            bool isUnlocked = skillTree.IsSkillUnlocked(skill.Id);
            bool canUnlock = CanUnlockSkill(skill, skillTree);
            
            // Draw skill background
            Rectangle skillRect = new Rectangle(menuX + PADDING, skillY, 
                                                MENU_WIDTH - PADDING * 2, SKILL_HEIGHT - 5);
            Color bgColor = isSelected ? new Color(80, 70, 60) : new Color(60, 50, 40);
            if (isUnlocked)
                bgColor = new Color(60, 80, 60); // Green tint for unlocked
            
            spriteBatch.Draw(_pixelTexture, skillRect, bgColor);
            
            if (isSelected)
            {
                DrawBorder(spriteBatch, skillRect, Color.Yellow, 2);
            }
            
            // Draw tier indicator
            string tier = $"T{skill.RequiredLevel}";
            Vector2 tierPos = new Vector2(skillRect.X + 5, skillRect.Y + 5);
            DrawTextWithShadow(spriteBatch, font, tier, tierPos, GetTierColor(skill.RequiredLevel));
            
            // Draw skill type icon
            string typeIcon = GetSkillTypeIcon(skill.Type);
            Vector2 iconPos = new Vector2(skillRect.X + 35, skillRect.Y + 5);
            DrawTextWithShadow(spriteBatch, font, typeIcon, iconPos, GetSkillTypeColor(skill.Type));
            
            // Draw skill name
            Color textColor = isUnlocked ? Color.LightGreen : (canUnlock ? Color.White : Color.Gray);
            Vector2 namePos = new Vector2(skillRect.X + 60, skillRect.Y + 5);
            DrawTextWithShadow(spriteBatch, font, skill.Name, namePos, textColor);
            
            // Draw skill description
            Vector2 descPos = new Vector2(skillRect.X + 60, skillRect.Y + 25);
            DrawTextWithShadow(spriteBatch, font, skill.Description, descPos, Color.LightGray * 0.8f);
            
            // Draw status
            string status = isUnlocked ? "[X] Unlocked" : 
                           (canUnlock ? "[ ] Available" : "[-] Locked");
            Vector2 statusSize = font.MeasureString(status);
            Vector2 statusPos = new Vector2(skillRect.Right - statusSize.X - 10, 
                                            skillRect.Y + (skillRect.Height - statusSize.Y) / 2);
            Color statusColor = isUnlocked ? Color.LightGreen : 
                               (canUnlock ? Color.Yellow : Color.Red);
            DrawTextWithShadow(spriteBatch, font, status, statusPos, statusColor);
        }
    }
    
    private bool CanUnlockSkill(Skill skill, SkillTree tree)
    {
        if (tree.IsSkillUnlocked(skill.Id))
            return false;
        
        if (_skillSystem.AvailableSkillPoints <= 0)
            return false;
        
        if (_skillSystem.GetSkillLevel(_selectedCategory) < skill.RequiredLevel)
            return false;
        
        if (skill.Prerequisites != null)
        {
            foreach (var prereq in skill.Prerequisites)
            {
                if (!tree.IsSkillUnlocked(prereq))
                    return false;
            }
        }
        
        return true;
    }
    
    private Color GetTierColor(int tier)
    {
        return tier switch
        {
            1 => Color.Silver,
            2 => Color.Gold,
            3 => Color.Orange,
            _ => Color.White
        };
    }
    
    private string GetSkillTypeIcon(SkillType type)
    {
        return type switch
        {
            SkillType.Passive => "o",
            SkillType.Active => "*",
            SkillType.Unlock => "^",
            _ => "?"
        };
    }
    
    private Color GetSkillTypeColor(SkillType type)
    {
        return type switch
        {
            SkillType.Passive => Color.LightBlue,
            SkillType.Active => Color.Yellow,
            SkillType.Unlock => Color.Violet,
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
