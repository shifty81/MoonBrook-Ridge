using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Magic;
using System.Collections.Generic;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Magic spell book menu UI for viewing and casting spells
/// </summary>
public class MagicMenu
{
    private bool _isActive;
    private MagicSystem _magicSystem;
    private List<Spell> _knownSpells;
    private int _selectedSpellIndex;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture = null!;
    
    private const int MENU_WIDTH = 650;
    private const int MENU_HEIGHT = 550;
    private const int SPELL_HEIGHT = 70;
    private const int PADDING = 20;
    
    public MagicMenu(MagicSystem magicSystem)
    {
        _magicSystem = magicSystem;
        _knownSpells = new List<Spell>();
        _selectedSpellIndex = 0;
        _isActive = false;
    }
    
    public void Show()
    {
        _isActive = true;
        _knownSpells = _magicSystem.KnownSpells;
        _selectedSpellIndex = 0;
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
            _selectedSpellIndex--;
            if (_selectedSpellIndex < 0)
                _selectedSpellIndex = _knownSpells.Count - 1;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedSpellIndex++;
            if (_selectedSpellIndex >= _knownSpells.Count)
                _selectedSpellIndex = 0;
        }
        
        // Cast selected spell
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (_knownSpells.Count > 0 && _selectedSpellIndex >= 0 && _selectedSpellIndex < _knownSpells.Count)
            {
                Spell selectedSpell = _knownSpells[_selectedSpellIndex];
                if (_magicSystem.CanCastSpell(selectedSpell))
                {
                    _magicSystem.CastSpell(selectedSpell.Id);
                    // Successfully cast - spell effect handled by event subscribers
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
        spriteBatch.Draw(_pixelTexture, menuRect, new Color(30, 20, 50));
        DrawBorder(spriteBatch, menuRect, Color.MediumPurple, 3);
        
        // Draw title
        string title = "Spell Book";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(menuX + (MENU_WIDTH - titleSize.X) / 2, menuY + PADDING);
        DrawTextWithShadow(spriteBatch, font, title, titlePos, Color.MediumPurple);
        
        // Draw mana bar
        int manaBarY = menuY + PADDING * 3;
        DrawManaBar(spriteBatch, font, menuX, manaBarY);
        
        // Draw spells list
        int spellStartY = manaBarY + 40;
        int maxVisibleSpells = (MENU_HEIGHT - spellStartY + menuY - PADDING * 3) / SPELL_HEIGHT;
        
        if (_knownSpells.Count == 0)
        {
            string noSpells = "No spells learned yet. Find spell tomes to learn magic!";
            Vector2 noSpellsSize = font.MeasureString(noSpells);
            Vector2 noSpellsPos = new Vector2(menuX + (MENU_WIDTH - noSpellsSize.X) / 2, 
                                              menuY + MENU_HEIGHT / 2);
            DrawTextWithShadow(spriteBatch, font, noSpells, noSpellsPos, Color.Gray);
        }
        else
        {
            for (int i = 0; i < _knownSpells.Count && i < maxVisibleSpells; i++)
            {
                Spell spell = _knownSpells[i];
                int spellY = spellStartY + i * SPELL_HEIGHT;
                bool isSelected = (i == _selectedSpellIndex);
                bool canCast = _magicSystem.CanCastSpell(spell);
                
                // Draw spell background
                Rectangle spellRect = new Rectangle(menuX + PADDING, spellY, 
                                                    MENU_WIDTH - PADDING * 2, SPELL_HEIGHT - 5);
                Color bgColor = isSelected ? new Color(60, 40, 80) : new Color(40, 30, 60);
                spriteBatch.Draw(_pixelTexture, spellRect, bgColor);
                
                if (isSelected)
                {
                    DrawBorder(spriteBatch, spellRect, Color.Violet, 2);
                }
                
                // Draw spell icon/type indicator
                string typeIcon = GetSpellTypeIcon(spell.Type);
                Vector2 iconPos = new Vector2(spellRect.X + 10, spellRect.Y + 10);
                DrawTextWithShadow(spriteBatch, font, typeIcon, iconPos, GetSpellTypeColor(spell.Type));
                
                // Draw spell name
                Color textColor = canCast ? Color.White : Color.Gray;
                Vector2 namePos = new Vector2(spellRect.X + 50, spellRect.Y + 5);
                DrawTextWithShadow(spriteBatch, font, spell.Name, namePos, textColor);
                
                // Draw spell description
                Vector2 descPos = new Vector2(spellRect.X + 50, spellRect.Y + 25);
                DrawTextWithShadow(spriteBatch, font, spell.Description, descPos, 
                                  Color.LightGray * 0.8f);
                
                // Draw mana cost
                string manaCost = $"{spell.ManaCost} MP";
                Vector2 costSize = font.MeasureString(manaCost);
                Vector2 costPos = new Vector2(spellRect.Right - costSize.X - 10, 
                                              spellRect.Y + (spellRect.Height - costSize.Y) / 2);
                DrawTextWithShadow(spriteBatch, font, manaCost, costPos, 
                                  canCast ? Color.Cyan : Color.Red);
            }
        }
        
        // Draw controls hint at bottom
        string hint = "Up/Down: Navigate | Enter: Cast Spell | Esc: Close";
        Vector2 hintSize = font.MeasureString(hint);
        Vector2 hintPos = new Vector2(menuX + (MENU_WIDTH - hintSize.X) / 2, 
                                     menuY + MENU_HEIGHT - PADDING - hintSize.Y);
        DrawTextWithShadow(spriteBatch, font, hint, hintPos, Color.LightGray);
    }
    
    private void DrawManaBar(SpriteBatch spriteBatch, SpriteFont font, int x, int y)
    {
        int barWidth = MENU_WIDTH - PADDING * 2;
        int barHeight = 25;
        
        // Background
        Rectangle barBg = new Rectangle(x + PADDING, y, barWidth, barHeight);
        spriteBatch.Draw(_pixelTexture, barBg, new Color(20, 20, 30));
        DrawBorder(spriteBatch, barBg, Color.Cyan, 2);
        
        // Mana fill
        float manaPercent = _magicSystem.Mana / _magicSystem.MaxMana;
        int fillWidth = (int)(barWidth * manaPercent);
        Rectangle barFill = new Rectangle(x + PADDING, y, fillWidth, barHeight);
        spriteBatch.Draw(_pixelTexture, barFill, new Color(50, 150, 255) * 0.7f);
        
        // Mana text
        string manaText = $"Mana: {(int)_magicSystem.Mana} / {(int)_magicSystem.MaxMana}";
        Vector2 textSize = font.MeasureString(manaText);
        Vector2 textPos = new Vector2(x + PADDING + (barWidth - textSize.X) / 2, y + (barHeight - textSize.Y) / 2);
        DrawTextWithShadow(spriteBatch, font, manaText, textPos, Color.White);
    }
    
    private string GetSpellTypeIcon(SpellType type)
    {
        return type switch
        {
            SpellType.Combat => "!",
            SpellType.Healing => "+",
            SpellType.Buff => "^",
            SpellType.Utility => "*",
            SpellType.Summon => "@",
            _ => "?"
        };
    }
    
    private Color GetSpellTypeColor(SpellType type)
    {
        return type switch
        {
            SpellType.Combat => Color.Red,
            SpellType.Healing => Color.LightGreen,
            SpellType.Buff => Color.Gold,
            SpellType.Utility => Color.Cyan,
            SpellType.Summon => Color.Violet,
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
