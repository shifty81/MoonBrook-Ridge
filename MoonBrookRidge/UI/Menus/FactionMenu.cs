using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Factions;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Menu for viewing faction reputation and rewards
/// </summary>
public class FactionMenu
{
    private SpriteFont _font;
    private Texture2D _pixelTexture;
    private FactionSystem _factionSystem;
    private bool _isActive;
    private int _selectedFactionIndex;
    
    // Colors
    private readonly Color _bgColor = new Color(20, 20, 30, 240);
    private readonly Color _headerColor = new Color(80, 40, 120);
    private readonly Color _textColor = Color.White;
    private readonly Color _highlightColor = new Color(200, 150, 255);
    private readonly Color _unlockedColor = new Color(50, 200, 50);
    private readonly Color _lockedColor = new Color(150, 150, 150);
    
    // Layout constants
    private const int PADDING = 20;
    
    public bool IsActive => _isActive;
    
    public FactionMenu(SpriteFont font, Texture2D pixelTexture, FactionSystem factionSystem)
    {
        _font = font;
        _pixelTexture = pixelTexture;
        _factionSystem = factionSystem;
        _isActive = false;
        _selectedFactionIndex = 0;
    }
    
    public void Show()
    {
        _isActive = true;
        _selectedFactionIndex = 0;
    }
    
    public void Hide()
    {
        _isActive = false;
    }
    
    public void Update(GameTime gameTime)
    {
        if (!_isActive) return;
        
        var keyboardState = Keyboard.GetState();
        var factions = _factionSystem.GetAllFactions();
        
        // Navigation
        if (keyboardState.IsKeyDown(Keys.Down))
        {
            _selectedFactionIndex = Math.Min(_selectedFactionIndex + 1, factions.Count - 1);
        }
        else if (keyboardState.IsKeyDown(Keys.Up))
        {
            _selectedFactionIndex = Math.Max(_selectedFactionIndex - 1, 0);
        }
        
        // Close menu
        if (keyboardState.IsKeyDown(Keys.Escape) || keyboardState.IsKeyDown(Keys.R))
        {
            Hide();
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
    {
        if (!_isActive) return;
        
        int screenWidth = graphicsDevice.Viewport.Width;
        int screenHeight = graphicsDevice.Viewport.Height;
        
        // Menu dimensions
        int menuWidth = 700;
        int menuHeight = 600;
        int menuX = (screenWidth - menuWidth) / 2;
        int menuY = (screenHeight - menuHeight) / 2;
        
        // Background
        spriteBatch.Draw(_pixelTexture, 
            new Rectangle(menuX, menuY, menuWidth, menuHeight), 
            _bgColor);
        
        // Header
        spriteBatch.Draw(_pixelTexture, 
            new Rectangle(menuX, menuY, menuWidth, 50), 
            _headerColor);
        
        string headerText = "Faction Reputation";
        Vector2 headerSize = font.MeasureString(headerText);
        spriteBatch.DrawString(font, headerText, 
            new Vector2(menuX + menuWidth / 2 - headerSize.X / 2, menuY + 15), 
            _textColor);
        
        // Faction list
        int yPos = menuY + 70;
        var factions = _factionSystem.GetAllFactions();
        
        for (int i = 0; i < factions.Count; i++)
        {
            var faction = factions[i];
            bool isSelected = (i == _selectedFactionIndex);
            
            // Faction row background
            if (isSelected)
            {
                spriteBatch.Draw(_pixelTexture, 
                    new Rectangle(menuX + PADDING, yPos - 5, menuWidth - PADDING * 2, 30), 
                    new Color(60, 60, 80, 150));
            }
            
            // Faction name
            Color factionColor = isSelected ? _highlightColor : _textColor;
            spriteBatch.DrawString(font, faction.Name, 
                new Vector2(menuX + PADDING + 5, yPos), factionColor);
            
            // Reputation level
            var level = _factionSystem.GetReputationLevel(faction.Id);
            string levelText = GetReputationLevelText(level);
            Color levelColor = GetReputationLevelColor(level);
            
            Vector2 nameSize = font.MeasureString(faction.Name);
            spriteBatch.DrawString(font, levelText, 
                new Vector2(menuX + PADDING + 250, yPos), levelColor);
            
            // Reputation bar
            int barWidth = 200;
            int barX = menuX + PADDING + 380;
            int barY = yPos + 5;
            int barHeight = 15;
            
            // Bar background
            spriteBatch.Draw(_pixelTexture, 
                new Rectangle(barX, barY, barWidth, barHeight), 
                new Color(40, 40, 40));
            
            // Bar fill
            int reputation = _factionSystem.GetReputation(faction.Id);
            float fillPercentage = (reputation + 3000) / 6000f; // Normalize -3000 to +3000 to 0-1
            int fillWidth = (int)(barWidth * Math.Max(0, Math.Min(1, fillPercentage)));
            
            spriteBatch.Draw(_pixelTexture, 
                new Rectangle(barX, barY, fillWidth, barHeight), 
                GetReputationBarColor(level));
            
            // Reputation value
            spriteBatch.DrawString(font, reputation.ToString(), 
                new Vector2(barX + barWidth + 10, yPos), _textColor);
            
            yPos += 35;
        }
        
        // Selected faction details
        if (_selectedFactionIndex < factions.Count)
        {
            DrawFactionDetails(spriteBatch, font, factions[_selectedFactionIndex], 
                menuX, menuY + 310, menuWidth);
        }
        
        // Instructions
        string instructions = "Up/Down: Navigate | R/ESC: Close";
        Vector2 instructionsSize = font.MeasureString(instructions);
        spriteBatch.DrawString(font, instructions, 
            new Vector2(menuX + menuWidth / 2 - instructionsSize.X / 2, menuY + menuHeight - 30), 
            new Color(180, 180, 180));
    }
    
    private void DrawFactionDetails(SpriteBatch spriteBatch, SpriteFont font, Faction faction, 
                                     int x, int y, int width)
    {
        // Details box
        spriteBatch.Draw(_pixelTexture, 
            new Rectangle(x + PADDING, y, width - PADDING * 2, 250), 
            new Color(40, 40, 50, 200));
        
        int textX = x + PADDING + 10;
        int textY = y + 10;
        
        // Faction description
        spriteBatch.DrawString(font, faction.Name, 
            new Vector2(textX, textY), _highlightColor);
        textY += 25;
        
        spriteBatch.DrawString(font, faction.Description, 
            new Vector2(textX, textY), _textColor);
        textY += 30;
        
        spriteBatch.DrawString(font, "Type: " + faction.Type, 
            new Vector2(textX, textY), new Color(200, 200, 200));
        textY += 30;
        
        // Rewards
        spriteBatch.DrawString(font, "Rewards:", 
            new Vector2(textX, textY), _highlightColor);
        textY += 25;
        
        var currentLevel = _factionSystem.GetReputationLevel(faction.Id);
        foreach (var reward in faction.Rewards)
        {
            bool isUnlocked = reward.RequiredLevel <= currentLevel;
            Color rewardColor = isUnlocked ? _unlockedColor : _lockedColor;
            string prefix = isUnlocked ? "[X]" : "[ ]";
            string rewardText = $"{prefix} [{GetReputationLevelText(reward.RequiredLevel)}] {reward.Name}";
            
            spriteBatch.DrawString(font, rewardText, 
                new Vector2(textX + 10, textY), rewardColor);
            textY += 20;
            
            // Show reward description for unlocked rewards
            if (isUnlocked)
            {
                spriteBatch.DrawString(font, "  " + reward.Description, 
                    new Vector2(textX + 20, textY), new Color(150, 150, 150));
                textY += 20;
            }
        }
    }
    
    private string GetReputationLevelText(ReputationLevel level)
    {
        return level switch
        {
            ReputationLevel.Hated => "Hated",
            ReputationLevel.Hostile => "Hostile",
            ReputationLevel.Unfriendly => "Unfriendly",
            ReputationLevel.Neutral => "Neutral",
            ReputationLevel.Friendly => "Friendly",
            ReputationLevel.Honored => "Honored",
            ReputationLevel.Revered => "Revered",
            ReputationLevel.Exalted => "Exalted",
            _ => "Unknown"
        };
    }
    
    private Color GetReputationLevelColor(ReputationLevel level)
    {
        return level switch
        {
            ReputationLevel.Hated => new Color(150, 0, 0),
            ReputationLevel.Hostile => new Color(200, 50, 0),
            ReputationLevel.Unfriendly => new Color(200, 100, 0),
            ReputationLevel.Neutral => new Color(200, 200, 200),
            ReputationLevel.Friendly => new Color(100, 200, 100),
            ReputationLevel.Honored => new Color(50, 200, 150),
            ReputationLevel.Revered => new Color(100, 150, 255),
            ReputationLevel.Exalted => new Color(200, 150, 255),
            _ => Color.Gray
        };
    }
    
    private Color GetReputationBarColor(ReputationLevel level)
    {
        return level switch
        {
            ReputationLevel.Hated or ReputationLevel.Hostile => new Color(180, 50, 50),
            ReputationLevel.Unfriendly => new Color(200, 120, 50),
            ReputationLevel.Neutral => new Color(150, 150, 150),
            ReputationLevel.Friendly => new Color(100, 180, 100),
            ReputationLevel.Honored or ReputationLevel.Revered => new Color(100, 150, 220),
            ReputationLevel.Exalted => new Color(180, 130, 220),
            _ => Color.Gray
        };
    }
}
