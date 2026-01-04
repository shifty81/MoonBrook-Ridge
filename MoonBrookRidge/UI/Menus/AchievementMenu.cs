using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using System.Collections.Generic;
using System.Linq;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Menu for viewing all achievements and their progress
/// </summary>
public class AchievementMenu
{
    private AchievementSystem _achievementSystem;
    private SpriteFont _font;
    private Texture2D _pixel;
    
    private List<Achievement> _displayedAchievements;
    private AchievementCategory? _filterCategory;
    private int _scrollOffset;
    private const int ITEMS_PER_PAGE = 8;
    private const int ITEM_HEIGHT = 70;
    private const int MENU_WIDTH = 700;
    private const int MENU_HEIGHT = 600;
    
    private KeyboardState _previousKeyState;
    
    public bool IsVisible { get; set; }
    
    public AchievementMenu(AchievementSystem achievementSystem)
    {
        _achievementSystem = achievementSystem;
        _scrollOffset = 0;
        _filterCategory = null;
        RefreshDisplayedAchievements();
    }
    
    public void LoadContent(SpriteFont font, Texture2D pixel)
    {
        _font = font;
        _pixel = pixel;
    }
    
    private void RefreshDisplayedAchievements()
    {
        if (_filterCategory.HasValue)
        {
            _displayedAchievements = _achievementSystem.GetAchievementsByCategory(_filterCategory.Value);
        }
        else
        {
            _displayedAchievements = _achievementSystem.GetAllAchievements();
        }
    }
    
    public void Update(GameTime gameTime, KeyboardState keyState)
    {
        if (!IsVisible)
            return;
            
        // Scroll with arrow keys
        if (keyState.IsKeyDown(Keys.Down) && !_previousKeyState.IsKeyDown(Keys.Down))
        {
            _scrollOffset++;
            int maxScroll = System.Math.Max(0, _displayedAchievements.Count - ITEMS_PER_PAGE);
            if (_scrollOffset > maxScroll)
                _scrollOffset = maxScroll;
        }
        
        if (keyState.IsKeyDown(Keys.Up) && !_previousKeyState.IsKeyDown(Keys.Up))
        {
            _scrollOffset--;
            if (_scrollOffset < 0)
                _scrollOffset = 0;
        }
        
        // Filter by category (1-8 keys)
        if (keyState.IsKeyDown(Keys.D1) && !_previousKeyState.IsKeyDown(Keys.D1))
        {
            _filterCategory = AchievementCategory.Farming;
            RefreshDisplayedAchievements();
            _scrollOffset = 0;
        }
        if (keyState.IsKeyDown(Keys.D2) && !_previousKeyState.IsKeyDown(Keys.D2))
        {
            _filterCategory = AchievementCategory.Fishing;
            RefreshDisplayedAchievements();
            _scrollOffset = 0;
        }
        if (keyState.IsKeyDown(Keys.D3) && !_previousKeyState.IsKeyDown(Keys.D3))
        {
            _filterCategory = AchievementCategory.Mining;
            RefreshDisplayedAchievements();
            _scrollOffset = 0;
        }
        if (keyState.IsKeyDown(Keys.D4) && !_previousKeyState.IsKeyDown(Keys.D4))
        {
            _filterCategory = AchievementCategory.Social;
            RefreshDisplayedAchievements();
            _scrollOffset = 0;
        }
        if (keyState.IsKeyDown(Keys.D5) && !_previousKeyState.IsKeyDown(Keys.D5))
        {
            _filterCategory = AchievementCategory.Crafting;
            RefreshDisplayedAchievements();
            _scrollOffset = 0;
        }
        if (keyState.IsKeyDown(Keys.D6) && !_previousKeyState.IsKeyDown(Keys.D6))
        {
            _filterCategory = AchievementCategory.Wealth;
            RefreshDisplayedAchievements();
            _scrollOffset = 0;
        }
        if (keyState.IsKeyDown(Keys.D7) && !_previousKeyState.IsKeyDown(Keys.D7))
        {
            _filterCategory = AchievementCategory.Exploration;
            RefreshDisplayedAchievements();
            _scrollOffset = 0;
        }
        if (keyState.IsKeyDown(Keys.D8) && !_previousKeyState.IsKeyDown(Keys.D8))
        {
            _filterCategory = AchievementCategory.Survival;
            RefreshDisplayedAchievements();
            _scrollOffset = 0;
        }
        if (keyState.IsKeyDown(Keys.D0) && !_previousKeyState.IsKeyDown(Keys.D0))
        {
            _filterCategory = null; // Show all
            RefreshDisplayedAchievements();
            _scrollOffset = 0;
        }
        
        _previousKeyState = keyState;
    }
    
    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (!IsVisible || _font == null || _pixel == null)
            return;
            
        int screenWidth = graphicsDevice.Viewport.Width;
        int screenHeight = graphicsDevice.Viewport.Height;
        
        int menuX = (screenWidth - MENU_WIDTH) / 2;
        int menuY = (screenHeight - MENU_HEIGHT) / 2;
        
        // Background
        var bgRect = new Rectangle(menuX, menuY, MENU_WIDTH, MENU_HEIGHT);
        spriteBatch.Draw(_pixel, bgRect, Color.Black * 0.9f);
        
        // Border
        DrawBorder(spriteBatch, bgRect, 3, Color.Gold);
        
        // Title
        string title = "Achievements";
        Vector2 titleSize = _font.MeasureString(title);
        Vector2 titlePos = new Vector2(menuX + MENU_WIDTH / 2 - titleSize.X / 2, menuY + 10);
        spriteBatch.DrawString(_font, title, titlePos, Color.Gold);
        
        // Completion percentage
        float completion = _achievementSystem.GetCompletionPercentage();
        string completionText = $"Completed: {completion:F1}%";
        Vector2 completionPos = new Vector2(menuX + 10, menuY + 40);
        spriteBatch.DrawString(_font, completionText, completionPos, Color.White);
        
        // Filter info
        string filterText = _filterCategory.HasValue 
            ? $"Filter: {_filterCategory.Value}" 
            : "Filter: All (Press 1-8 to filter, 0 for all)";
        Vector2 filterPos = new Vector2(menuX + 10, menuY + 60);
        spriteBatch.DrawString(_font, filterText, filterPos, Color.LightGray * 0.8f);
        
        // Achievement list
        int startY = menuY + 90;
        int visibleCount = System.Math.Min(ITEMS_PER_PAGE, _displayedAchievements.Count - _scrollOffset);
        
        for (int i = 0; i < visibleCount; i++)
        {
            int achievementIndex = i + _scrollOffset;
            var achievement = _displayedAchievements[achievementIndex];
            
            int itemY = startY + i * ITEM_HEIGHT;
            DrawAchievementItem(spriteBatch, achievement, menuX + 10, itemY, MENU_WIDTH - 20);
        }
        
        // Scroll indicator
        if (_displayedAchievements.Count > ITEMS_PER_PAGE)
        {
            string scrollText = $"{_scrollOffset + 1}-{_scrollOffset + visibleCount} of {_displayedAchievements.Count}";
            Vector2 scrollSize = _font.MeasureString(scrollText);
            Vector2 scrollPos = new Vector2(menuX + MENU_WIDTH - scrollSize.X - 10, menuY + MENU_HEIGHT - 30);
            spriteBatch.DrawString(_font, scrollText, scrollPos, Color.White);
        }
        
        // Controls hint
        string hint = "Up/Down: Scroll | ESC: Close";
        Vector2 hintPos = new Vector2(menuX + 10, menuY + MENU_HEIGHT - 30);
        spriteBatch.DrawString(_font, hint, hintPos, Color.LightGray * 0.7f);
    }
    
    private void DrawAchievementItem(SpriteBatch spriteBatch, Achievement achievement, int x, int y, int width)
    {
        Color bgColor = achievement.IsUnlocked ? new Color(40, 80, 40) : new Color(40, 40, 40);
        var itemRect = new Rectangle(x, y, width, ITEM_HEIGHT - 5);
        spriteBatch.Draw(_pixel, itemRect, bgColor * 0.8f);
        
        // Border with category color
        Color borderColor = achievement.IsUnlocked 
            ? GetCategoryColor(achievement.Category) 
            : Color.Gray;
        DrawBorder(spriteBatch, itemRect, 2, borderColor);
        
        // Achievement name
        string name = achievement.IsUnlocked ? achievement.Name : "???";
        Vector2 namePos = new Vector2(x + 10, y + 5);
        Color nameColor = achievement.IsUnlocked ? Color.White : Color.Gray;
        spriteBatch.DrawString(_font, name, namePos, nameColor);
        
        // Description
        string desc = achievement.IsUnlocked ? achievement.Description : "Locked";
        Vector2 descPos = new Vector2(x + 10, y + 25);
        Color descColor = achievement.IsUnlocked ? Color.LightGray : Color.DarkGray;
        spriteBatch.DrawString(_font, desc, descPos, descColor * 0.8f);
        
        // Progress
        int currentProgress = _achievementSystem.GetProgress(achievement.Id);
        string progressText = achievement.IsUnlocked 
            ? "[X] Unlocked" 
            : $"Progress: {currentProgress}/{achievement.RequiredProgress}";
        Vector2 progressPos = new Vector2(x + 10, y + 45);
        spriteBatch.DrawString(_font, progressText, progressPos, Color.Yellow * 0.9f);
        
        // Category badge
        string categoryText = achievement.Category.ToString();
        Vector2 catSize = _font.MeasureString(categoryText);
        Vector2 catPos = new Vector2(x + width - catSize.X - 10, y + 5);
        spriteBatch.DrawString(_font, categoryText, catPos, borderColor * 0.8f);
    }
    
    private void DrawBorder(SpriteBatch spriteBatch, Rectangle rect, int thickness, Color color)
    {
        // Top
        spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, rect.Width, thickness), color);
        // Bottom
        spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y + rect.Height - thickness, rect.Width, thickness), color);
        // Left
        spriteBatch.Draw(_pixel, new Rectangle(rect.X, rect.Y, thickness, rect.Height), color);
        // Right
        spriteBatch.Draw(_pixel, new Rectangle(rect.X + rect.Width - thickness, rect.Y, thickness, rect.Height), color);
    }
    
    private Color GetCategoryColor(AchievementCategory category)
    {
        return category switch
        {
            AchievementCategory.Farming => new Color(76, 175, 80),
            AchievementCategory.Fishing => new Color(33, 150, 243),
            AchievementCategory.Mining => new Color(158, 158, 158),
            AchievementCategory.Social => new Color(233, 30, 99),
            AchievementCategory.Crafting => new Color(255, 152, 0),
            AchievementCategory.Wealth => new Color(255, 235, 59),
            AchievementCategory.Exploration => new Color(156, 39, 176),
            AchievementCategory.Survival => new Color(244, 67, 54),
            _ => Color.White
        };
    }
}
