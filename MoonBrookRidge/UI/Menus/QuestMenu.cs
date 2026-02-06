using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Quests;
using System.Collections.Generic;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Quest/Journal menu UI for viewing and managing quests
/// </summary>
public class QuestMenu
{
    private bool _isActive;
    private QuestSystem _questSystem;
    private int _selectedQuestIndex;
    private QuestTab _currentTab;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture = null!;
    
    private const int MENU_WIDTH = 800;
    private const int MENU_HEIGHT = 600;
    private const int PADDING = 20;
    private const int QUEST_LIST_WIDTH = 300;
    
    private enum QuestTab
    {
        Active,
        Available,
        Completed
    }
    
    public QuestMenu(QuestSystem questSystem)
    {
        _questSystem = questSystem;
        _selectedQuestIndex = 0;
        _currentTab = QuestTab.Active;
        _isActive = false;
    }
    
    public void Show()
    {
        _isActive = true;
        _selectedQuestIndex = 0;
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
        
        // Close menu with Escape or F
        if ((keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape)) ||
            (keyboardState.IsKeyDown(Keys.F) && !_previousKeyboardState.IsKeyDown(Keys.F)))
        {
            Hide();
        }
        
        // Switch tabs with Tab or number keys
        if (keyboardState.IsKeyDown(Keys.Tab) && !_previousKeyboardState.IsKeyDown(Keys.Tab))
        {
            _currentTab = (QuestTab)(((int)_currentTab + 1) % 3);
            _selectedQuestIndex = 0;
        }
        
        if (keyboardState.IsKeyDown(Keys.D1) && !_previousKeyboardState.IsKeyDown(Keys.D1))
        {
            _currentTab = QuestTab.Active;
            _selectedQuestIndex = 0;
        }
        if (keyboardState.IsKeyDown(Keys.D2) && !_previousKeyboardState.IsKeyDown(Keys.D2))
        {
            _currentTab = QuestTab.Available;
            _selectedQuestIndex = 0;
        }
        if (keyboardState.IsKeyDown(Keys.D3) && !_previousKeyboardState.IsKeyDown(Keys.D3))
        {
            _currentTab = QuestTab.Completed;
            _selectedQuestIndex = 0;
        }
        
        // Navigation
        List<Quest> currentQuests = GetCurrentTabQuests();
        if (currentQuests.Count > 0)
        {
            if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _selectedQuestIndex--;
                if (_selectedQuestIndex < 0) _selectedQuestIndex = currentQuests.Count - 1;
            }
            
            if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _selectedQuestIndex++;
                if (_selectedQuestIndex >= currentQuests.Count) _selectedQuestIndex = 0;
            }
            
            // Accept quest if in Available tab
            if (_currentTab == QuestTab.Available && 
                (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter)))
            {
                var quest = currentQuests[_selectedQuestIndex];
                _questSystem.AcceptQuest(quest);
                _selectedQuestIndex = 0;
            }
        }
        
        _previousKeyboardState = keyboardState;
    }
    
    private List<Quest> GetCurrentTabQuests()
    {
        return _currentTab switch
        {
            QuestTab.Active => _questSystem.GetActiveQuests(),
            QuestTab.Available => _questSystem.GetAvailableQuests(),
            QuestTab.Completed => _questSystem.GetCompletedQuests(),
            _ => new List<Quest>()
        };
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
        
        // Semi-transparent background overlay
        spriteBatch.Draw(_pixelTexture, 
            new Rectangle(0, 0, screenWidth, screenHeight), 
            new Color(0, 0, 0, 180));
        
        // Menu background
        int menuX = (screenWidth - MENU_WIDTH) / 2;
        int menuY = (screenHeight - MENU_HEIGHT) / 2;
        
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(menuX, menuY, MENU_WIDTH, MENU_HEIGHT),
            new Color(40, 35, 30));
        
        // Menu border
        DrawBorder(spriteBatch, menuX, menuY, MENU_WIDTH, MENU_HEIGHT, 3, new Color(200, 180, 150));
        
        // Title
        string title = "Quest Journal";
        Vector2 titleSize = font.MeasureString(title);
        spriteBatch.DrawString(font, title,
            new Vector2(menuX + MENU_WIDTH / 2 - titleSize.X / 2, menuY + PADDING),
            Color.White);
        
        // Tab buttons
        int tabY = menuY + PADDING + 35;
        DrawTab(spriteBatch, font, "Active (1)", menuX + PADDING, tabY, _currentTab == QuestTab.Active);
        DrawTab(spriteBatch, font, "Available (2)", menuX + PADDING + 120, tabY, _currentTab == QuestTab.Available);
        DrawTab(spriteBatch, font, "Completed (3)", menuX + PADDING + 260, tabY, _currentTab == QuestTab.Completed);
        
        // Quest list section
        int listY = tabY + 40;
        DrawQuestList(spriteBatch, font, menuX + PADDING, listY);
        
        // Quest details section
        DrawQuestDetails(spriteBatch, font, menuX + PADDING + QUEST_LIST_WIDTH + PADDING, listY);
        
        // Instructions
        string instructions = "Up/Down: Select | Enter: Accept | Tab/1-3: Switch Tab | Esc/F: Close";
        Vector2 instructSize = font.MeasureString(instructions);
        spriteBatch.DrawString(font, instructions,
            new Vector2(menuX + MENU_WIDTH / 2 - instructSize.X / 2, menuY + MENU_HEIGHT - PADDING - 20),
            Color.LightGray);
    }
    
    private void DrawTab(SpriteBatch spriteBatch, SpriteFont font, string text, int x, int y, bool isActive)
    {
        Color bgColor = isActive ? new Color(80, 70, 60) : new Color(60, 55, 50);
        Color textColor = isActive ? Color.Yellow : Color.Gray;
        
        Vector2 textSize = font.MeasureString(text);
        int width = (int)textSize.X + 20;
        int height = (int)textSize.Y + 10;
        
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, width, height), bgColor);
        DrawBorder(spriteBatch, x, y, width, height, 2, isActive ? Color.Yellow : Color.Gray);
        
        spriteBatch.DrawString(font, text, new Vector2(x + 10, y + 5), textColor);
    }
    
    private void DrawQuestList(SpriteBatch spriteBatch, SpriteFont font, int x, int y)
    {
        // List background
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(x, y, QUEST_LIST_WIDTH, 400),
            new Color(30, 25, 20));
        DrawBorder(spriteBatch, x, y, QUEST_LIST_WIDTH, 400, 2, Color.Gray);
        
        List<Quest> quests = GetCurrentTabQuests();
        
        if (quests.Count == 0)
        {
            string noQuestsText = _currentTab switch
            {
                QuestTab.Active => "No active quests",
                QuestTab.Available => "No available quests",
                QuestTab.Completed => "No completed quests",
                _ => ""
            };
            
            Vector2 textSize = font.MeasureString(noQuestsText);
            spriteBatch.DrawString(font, noQuestsText,
                new Vector2(x + QUEST_LIST_WIDTH / 2 - textSize.X / 2, y + 180),
                Color.Gray);
        }
        else
        {
            for (int i = 0; i < quests.Count && i < 10; i++)
            {
                var quest = quests[i];
                int itemY = y + 10 + i * 40;
                bool isSelected = (i == _selectedQuestIndex);
                
                // Selection highlight
                if (isSelected)
                {
                    spriteBatch.Draw(_pixelTexture,
                        new Rectangle(x + 5, itemY, QUEST_LIST_WIDTH - 10, 35),
                        new Color(80, 70, 60));
                }
                
                // Quest title
                Color titleColor = isSelected ? Color.Yellow : Color.White;
                spriteBatch.DrawString(font, quest.Title,
                    new Vector2(x + 10, itemY + 5),
                    titleColor);
                
                // Quest giver
                string giverText = $"- {quest.GiverName}";
                spriteBatch.DrawString(font, giverText,
                    new Vector2(x + 15, itemY + 20),
                    Color.LightGray);
            }
        }
    }
    
    private void DrawQuestDetails(SpriteBatch spriteBatch, SpriteFont font, int x, int y)
    {
        int detailsWidth = MENU_WIDTH - QUEST_LIST_WIDTH - PADDING * 3;
        
        // Details background
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(x, y, detailsWidth, 400),
            new Color(30, 25, 20));
        DrawBorder(spriteBatch, x, y, detailsWidth, 400, 2, Color.Gray);
        
        List<Quest> quests = GetCurrentTabQuests();
        if (quests.Count == 0 || _selectedQuestIndex >= quests.Count) return;
        
        Quest selectedQuest = quests[_selectedQuestIndex];
        int detailY = y + 10;
        
        // Quest title
        spriteBatch.DrawString(font, selectedQuest.Title,
            new Vector2(x + 10, detailY),
            Color.Yellow);
        detailY += 30;
        
        // Quest giver
        spriteBatch.DrawString(font, $"Given by: {selectedQuest.GiverName}",
            new Vector2(x + 10, detailY),
            Color.LightGray);
        detailY += 25;
        
        // Description
        spriteBatch.DrawString(font, selectedQuest.Description,
            new Vector2(x + 10, detailY),
            Color.White);
        detailY += 50;
        
        // Objectives
        spriteBatch.DrawString(font, "Objectives:",
            new Vector2(x + 10, detailY),
            Color.Cyan);
        detailY += 25;
        
        foreach (var objective in selectedQuest.Objectives)
        {
            string checkmark = objective.IsCompleted ? "[X]" : "[ ]";
            Color objColor = objective.IsCompleted ? Color.Green : Color.White;
            
            spriteBatch.DrawString(font, $"{checkmark} {objective.Description}",
                new Vector2(x + 20, detailY),
                objColor);
            
            if (!objective.IsCompleted && objective.RequiredProgress > 1)
            {
                spriteBatch.DrawString(font, $"   ({objective.GetProgressText()})",
                    new Vector2(x + 30, detailY + 18),
                    Color.Gray);
                detailY += 18;
            }
            
            detailY += 25;
        }
        
        // Rewards
        if (selectedQuest.Reward != null)
        {
            detailY += 10;
            spriteBatch.DrawString(font, "Rewards:",
                new Vector2(x + 10, detailY),
                Color.Gold);
            detailY += 25;
            
            if (selectedQuest.Reward.Money > 0)
            {
                spriteBatch.DrawString(font, $"• ${selectedQuest.Reward.Money}",
                    new Vector2(x + 20, detailY),
                    Color.LightGreen);
                detailY += 20;
            }
            
            if (selectedQuest.Reward.FriendshipPoints > 0 && !string.IsNullOrEmpty(selectedQuest.Reward.FriendshipNPC))
            {
                spriteBatch.DrawString(font, $"• {selectedQuest.Reward.FriendshipPoints} friendship with {selectedQuest.Reward.FriendshipNPC}",
                    new Vector2(x + 20, detailY),
                    Color.Pink);
                detailY += 20;
            }
            
            foreach (var item in selectedQuest.Reward.Items)
            {
                spriteBatch.DrawString(font, $"• {item.Key} x{item.Value}",
                    new Vector2(x + 20, detailY),
                    Color.LightBlue);
                detailY += 20;
            }
        }
    }
    
    private void DrawBorder(SpriteBatch spriteBatch, int x, int y, int width, int height, int thickness, Color color)
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
    
    public bool IsActive => _isActive;
}
