using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Characters;
using System;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// UI for managing family (spouse and children)
/// </summary>
public class FamilyMenu
{
    private bool _isActive;
    private MarriageSystem _marriageSystem;
    private FamilySystem _familySystem;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture;
    private int _selectedChildIndex;
    private int _selectedAction; // 0-3 for Play, Gift, Teach, Hug
    
    private const int MENU_WIDTH = 700;
    private const int MENU_HEIGHT = 600;
    private const int PADDING = 20;
    
    public event Action<Child, ChildInteraction> OnChildInteraction;
    
    public FamilyMenu(MarriageSystem marriageSystem, FamilySystem familySystem)
    {
        _marriageSystem = marriageSystem;
        _familySystem = familySystem;
        _isActive = false;
        _selectedChildIndex = 0;
        _selectedAction = 0;
    }
    
    /// <summary>
    /// Show/hide family menu
    /// </summary>
    public void Toggle()
    {
        _isActive = !_isActive;
        if (_isActive)
        {
            _selectedChildIndex = 0;
            _selectedAction = 0;
        }
    }
    
    /// <summary>
    /// Hide menu
    /// </summary>
    public void Hide()
    {
        _isActive = false;
    }
    
    /// <summary>
    /// Update menu input
    /// </summary>
    public void Update(GameTime gameTime)
    {
        if (!_isActive) return;
        
        KeyboardState keyboardState = Keyboard.GetState();
        
        // Close menu
        if ((keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape)) ||
            (keyboardState.IsKeyDown(Keys.F) && !_previousKeyboardState.IsKeyDown(Keys.F)))
        {
            Hide();
        }
        
        int childCount = _familySystem.ChildCount;
        
        if (childCount > 0)
        {
            // Navigate children
            if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
            {
                _selectedChildIndex--;
                if (_selectedChildIndex < 0) _selectedChildIndex = childCount - 1;
            }
            
            if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
            {
                _selectedChildIndex++;
                if (_selectedChildIndex >= childCount) _selectedChildIndex = 0;
            }
            
            // Navigate actions
            if (keyboardState.IsKeyDown(Keys.Left) && !_previousKeyboardState.IsKeyDown(Keys.Left))
            {
                _selectedAction--;
                if (_selectedAction < 0) _selectedAction = 3;
            }
            
            if (keyboardState.IsKeyDown(Keys.Right) && !_previousKeyboardState.IsKeyDown(Keys.Right))
            {
                _selectedAction++;
                if (_selectedAction > 3) _selectedAction = 0;
            }
            
            // Perform action
            if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
            {
                PerformAction();
            }
        }
        
        _previousKeyboardState = keyboardState;
    }
    
    /// <summary>
    /// Perform the selected action on the selected child
    /// </summary>
    private void PerformAction()
    {
        var child = _familySystem.GetChild(_selectedChildIndex);
        if (child == null) return;
        
        ChildInteraction interaction = _selectedAction switch
        {
            0 => ChildInteraction.Play,
            1 => ChildInteraction.Gift,
            2 => ChildInteraction.Teach,
            3 => ChildInteraction.Hug,
            _ => ChildInteraction.Play
        };
        
        _familySystem.InteractWithChild(child, interaction);
        OnChildInteraction?.Invoke(child, interaction);
    }
    
    /// <summary>
    /// Draw family menu
    /// </summary>
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
            new Color(50, 40, 35));
        
        // Menu border
        DrawBorder(spriteBatch, menuX, menuY, MENU_WIDTH, MENU_HEIGHT, 3, new Color(220, 200, 180));
        
        // Title
        string title = "Family";
        Vector2 titleSize = font.MeasureString(title);
        spriteBatch.DrawString(font, title,
            new Vector2(menuX + MENU_WIDTH / 2 - titleSize.X / 2, menuY + PADDING),
            Color.White);
        
        int contentY = menuY + PADDING + 40;
        
        // Marriage status
        if (_marriageSystem.IsMarried && _marriageSystem.Spouse != null)
        {
            string spouseText = $"Married to: {_marriageSystem.Spouse.Name}";
            spriteBatch.DrawString(font, spouseText,
                new Vector2(menuX + PADDING, contentY),
                Color.LightGreen);
            
            int daysSince = _marriageSystem.DaysSinceMarriage;
            string daysText = $"Days married: {daysSince}";
            spriteBatch.DrawString(font, daysText,
                new Vector2(menuX + PADDING, contentY + 30),
                Color.LightGray);
            
            // Spouse greeting
            string greeting = _marriageSystem.GetSpouseGreeting();
            if (!string.IsNullOrEmpty(greeting))
            {
                string greetingText = $"\"{greeting}\"";
                spriteBatch.DrawString(font, greetingText,
                    new Vector2(menuX + PADDING, contentY + 60),
                    Color.LightYellow);
            }
            
            contentY += 110;
        }
        else
        {
            string notMarriedText = "Not married";
            spriteBatch.DrawString(font, notMarriedText,
                new Vector2(menuX + PADDING, contentY),
                Color.Gray);
            contentY += 40;
        }
        
        // Divider
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(menuX + PADDING, contentY, MENU_WIDTH - PADDING * 2, 2),
            Color.Gray);
        contentY += 20;
        
        // Children section
        string childrenTitle = $"Children ({_familySystem.ChildCount}/{2})";
        spriteBatch.DrawString(font, childrenTitle,
            new Vector2(menuX + PADDING, contentY),
            Color.White);
        contentY += 35;
        
        if (_familySystem.ChildCount == 0)
        {
            string noChildrenText = "No children yet";
            spriteBatch.DrawString(font, noChildrenText,
                new Vector2(menuX + PADDING + 20, contentY),
                Color.Gray);
        }
        else
        {
            // Draw each child
            for (int i = 0; i < _familySystem.ChildCount; i++)
            {
                var child = _familySystem.GetChild(i);
                if (child == null) continue;
                
                bool isSelected = (i == _selectedChildIndex);
                int childY = contentY + i * 100;
                
                // Child background
                if (isSelected)
                {
                    spriteBatch.Draw(_pixelTexture,
                        new Rectangle(menuX + PADDING, childY - 5, MENU_WIDTH - PADDING * 2, 95),
                        new Color(70, 60, 50));
                }
                
                // Child info
                string childName = $"{child.Name} ({(child.IsBoy ? "Boy" : "Girl")})";
                spriteBatch.DrawString(font, childName,
                    new Vector2(menuX + PADDING + 10, childY),
                    isSelected ? Color.Yellow : Color.White);
                
                string stageText = $"Stage: {child.Stage} ({child.DaysOld} days old)";
                spriteBatch.DrawString(font, stageText,
                    new Vector2(menuX + PADDING + 10, childY + 25),
                    Color.LightBlue);
                
                // Happiness bar
                DrawBar(spriteBatch, menuX + PADDING + 10, childY + 50, 150, 15, 
                    child.Happiness, 100, Color.Pink, "Happiness");
                
                // Education bar (if not baby)
                if (child.Stage != ChildStage.Baby)
                {
                    DrawBar(spriteBatch, menuX + PADDING + 180, childY + 50, 150, 15,
                        child.Education, 100, Color.LightBlue, "Education");
                }
                
                // Dialogue
                string dialogue = child.GetDialogue();
                if (!string.IsNullOrEmpty(dialogue))
                {
                    spriteBatch.DrawString(font, $"\"{dialogue}\"",
                        new Vector2(menuX + PADDING + 10, childY + 70),
                        Color.LightYellow);
                }
            }
            
            // Action buttons (when a child is selected)
            int actionY = menuY + MENU_HEIGHT - 100;
            string actionsText = "Actions:";
            spriteBatch.DrawString(font, actionsText,
                new Vector2(menuX + PADDING, actionY),
                Color.White);
            
            string[] actions = { "Play", "Gift", "Teach", "Hug" };
            int buttonWidth = 120;
            int buttonSpacing = 20;
            int startX = menuX + (MENU_WIDTH - (buttonWidth * 4 + buttonSpacing * 3)) / 2;
            
            for (int i = 0; i < actions.Length; i++)
            {
                int buttonX = startX + i * (buttonWidth + buttonSpacing);
                int buttonY = actionY + 30;
                bool isSelected = (i == _selectedAction);
                
                Color buttonColor = isSelected ? new Color(100, 150, 200) : new Color(60, 80, 100);
                spriteBatch.Draw(_pixelTexture,
                    new Rectangle(buttonX, buttonY, buttonWidth, 35),
                    buttonColor);
                DrawBorder(spriteBatch, buttonX, buttonY, buttonWidth, 35, 2, Color.White);
                
                Vector2 actionSize = font.MeasureString(actions[i]);
                spriteBatch.DrawString(font, actions[i],
                    new Vector2(buttonX + buttonWidth / 2 - actionSize.X / 2, buttonY + 17 - actionSize.Y / 2),
                    Color.White);
            }
        }
        
        // Instructions
        string instructions = "↑↓: Select Child | ←→: Select Action | Enter: Do Action | F/Esc: Close";
        Vector2 instructSize = font.MeasureString(instructions);
        spriteBatch.DrawString(font, instructions,
            new Vector2(menuX + MENU_WIDTH / 2 - instructSize.X / 2, menuY + MENU_HEIGHT - PADDING - 10),
            Color.LightGray);
    }
    
    /// <summary>
    /// Draw a progress bar
    /// </summary>
    private void DrawBar(SpriteBatch spriteBatch, int x, int y, int width, int height, 
        int current, int max, Color barColor, string label)
    {
        // Background
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(x, y, width, height),
            new Color(30, 30, 30));
        
        // Fill
        float percentage = (float)current / max;
        int fillWidth = (int)(width * percentage);
        spriteBatch.Draw(_pixelTexture,
            new Rectangle(x, y, fillWidth, height),
            barColor);
        
        // Border
        DrawBorder(spriteBatch, x, y, width, height, 1, Color.White);
        
        // Label (optional - only if it fits)
        if (width > 60)
        {
            string text = $"{current}/{max}";
            // Small text rendering would go here if we had a smaller font
        }
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
}
