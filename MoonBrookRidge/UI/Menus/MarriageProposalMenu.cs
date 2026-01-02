using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Characters;
using MoonBrookRidge.Characters.NPCs;
using System;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// UI for proposing marriage to an NPC
/// </summary>
public class MarriageProposalMenu
{
    private bool _isActive;
    private NPCCharacter _targetNPC;
    private MarriageSystem _marriageSystem;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture;
    private int _selectedOption; // 0 = Yes, 1 = No
    
    private const int MENU_WIDTH = 500;
    private const int MENU_HEIGHT = 300;
    private const int PADDING = 20;
    
    public event Action<NPCCharacter, bool> OnProposalDecision; // NPC, accepted/declined
    
    public MarriageProposalMenu(MarriageSystem marriageSystem)
    {
        _marriageSystem = marriageSystem;
        _isActive = false;
        _selectedOption = 0;
    }
    
    /// <summary>
    /// Show the marriage proposal dialog
    /// </summary>
    public void Show(NPCCharacter npc)
    {
        if (npc == null || !_marriageSystem.CanPropose(npc))
        {
            return;
        }
        
        _isActive = true;
        _targetNPC = npc;
        _selectedOption = 0;
    }
    
    /// <summary>
    /// Hide the menu
    /// </summary>
    public void Hide()
    {
        _isActive = false;
        _targetNPC = null;
    }
    
    /// <summary>
    /// Update menu input
    /// </summary>
    public void Update(GameTime gameTime)
    {
        if (!_isActive) return;
        
        KeyboardState keyboardState = Keyboard.GetState();
        
        // Navigate between Yes/No
        if (keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
        {
            _selectedOption = 0; // Yes
        }
        
        if (keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
        {
            _selectedOption = 1; // No
        }
        
        // Confirm selection
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            ConfirmSelection();
        }
        
        // Cancel with Escape
        if (keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            _selectedOption = 1; // Treat as "No"
            ConfirmSelection();
        }
        
        _previousKeyboardState = keyboardState;
    }
    
    /// <summary>
    /// Confirm the player's choice
    /// </summary>
    private void ConfirmSelection()
    {
        bool proposing = (_selectedOption == 0);
        
        if (proposing && _targetNPC != null)
        {
            // Actually propose
            bool accepted = _marriageSystem.ProposeMarriage(_targetNPC);
            OnProposalDecision?.Invoke(_targetNPC, accepted);
        }
        else
        {
            // Player chose not to propose
            OnProposalDecision?.Invoke(_targetNPC, false);
        }
        
        Hide();
    }
    
    /// <summary>
    /// Draw the proposal menu
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, GraphicsDevice graphicsDevice)
    {
        if (!_isActive || _targetNPC == null) return;
        
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
            new Color(0, 0, 0, 200));
        
        // Menu background
        int menuX = (screenWidth - MENU_WIDTH) / 2;
        int menuY = (screenHeight - MENU_HEIGHT) / 2;
        
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(menuX, menuY, MENU_WIDTH, MENU_HEIGHT),
            new Color(60, 40, 50));
        
        // Menu border
        DrawBorder(spriteBatch, menuX, menuY, MENU_WIDTH, MENU_HEIGHT, 3, new Color(255, 200, 220));
        
        // Title
        string title = "Marriage Proposal";
        Vector2 titleSize = font.MeasureString(title);
        spriteBatch.DrawString(font, title,
            new Vector2(menuX + MENU_WIDTH / 2 - titleSize.X / 2, menuY + PADDING),
            Color.White);
        
        // Proposal text
        string proposalText = $"Will you marry {_targetNPC.Name}?";
        Vector2 proposalSize = font.MeasureString(proposalText);
        spriteBatch.DrawString(font, proposalText,
            new Vector2(menuX + MENU_WIDTH / 2 - proposalSize.X / 2, menuY + PADDING + 50),
            Color.LightPink);
        
        // Heart level display
        int heartLevel = _targetNPC.GetHeartLevel();
        string heartText = $"{_targetNPC.Name} has {heartLevel}/10 hearts";
        Vector2 heartSize = font.MeasureString(heartText);
        spriteBatch.DrawString(font, heartText,
            new Vector2(menuX + MENU_WIDTH / 2 - heartSize.X / 2, menuY + PADDING + 90),
            Color.Pink);
        
        // Yes/No buttons
        int buttonY = menuY + MENU_HEIGHT - PADDING - 60;
        int buttonWidth = 120;
        int buttonHeight = 50;
        int buttonSpacing = 40;
        
        int yesButtonX = menuX + MENU_WIDTH / 2 - buttonWidth - buttonSpacing / 2;
        int noButtonX = menuX + MENU_WIDTH / 2 + buttonSpacing / 2;
        
        // Yes button
        Color yesColor = (_selectedOption == 0) ? new Color(100, 200, 100) : new Color(60, 120, 60);
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(yesButtonX, buttonY, buttonWidth, buttonHeight),
            yesColor);
        DrawBorder(spriteBatch, yesButtonX, buttonY, buttonWidth, buttonHeight, 2, Color.White);
        
        string yesText = "Yes";
        Vector2 yesSize = font.MeasureString(yesText);
        spriteBatch.DrawString(font, yesText,
            new Vector2(yesButtonX + buttonWidth / 2 - yesSize.X / 2, buttonY + buttonHeight / 2 - yesSize.Y / 2),
            Color.White);
        
        // No button
        Color noColor = (_selectedOption == 1) ? new Color(200, 100, 100) : new Color(120, 60, 60);
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(noButtonX, buttonY, buttonWidth, buttonHeight),
            noColor);
        DrawBorder(spriteBatch, noButtonX, buttonY, buttonWidth, buttonHeight, 2, Color.White);
        
        string noText = "No";
        Vector2 noSize = font.MeasureString(noText);
        spriteBatch.DrawString(font, noText,
            new Vector2(noButtonX + buttonWidth / 2 - noSize.X / 2, buttonY + buttonHeight / 2 - noSize.Y / 2),
            Color.White);
        
        // Instructions
        string instructions = "←→: Select | Enter: Confirm | Esc: Cancel";
        Vector2 instructSize = font.MeasureString(instructions);
        spriteBatch.DrawString(font, instructions,
            new Vector2(menuX + MENU_WIDTH / 2 - instructSize.X / 2, menuY + MENU_HEIGHT - PADDING - 10),
            Color.LightGray);
    }
    
    /// <summary>
    /// Draw border around a rectangle
    /// </summary>
    private void DrawBorder(SpriteBatch spriteBatch, int x, int y, int width, int height, int thickness, Color color)
    {
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, width, thickness), color);
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y + height - thickness, width, thickness), color);
        spriteBatch.Draw(_pixelTexture, new Rectangle(x, y, thickness, height), color);
        spriteBatch.Draw(_pixelTexture, new Rectangle(x + width - thickness, y, thickness, height), color);
    }
    
    public bool IsActive => _isActive;
    public NPCCharacter TargetNPC => _targetNPC;
}
