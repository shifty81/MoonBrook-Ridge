using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Pets;
using System.Collections.Generic;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Pet management menu UI for viewing and managing pets
/// </summary>
public class PetMenu
{
    private bool _isActive;
    private PetSystem _petSystem;
    private List<Pet> _ownedPets;
    private int _selectedPetIndex;
    private KeyboardState _previousKeyboardState;
    private Texture2D _pixelTexture;
    
    private const int MENU_WIDTH = 700;
    private const int MENU_HEIGHT = 550;
    private const int PET_HEIGHT = 90;
    private const int PADDING = 20;
    
    public PetMenu(PetSystem petSystem)
    {
        _petSystem = petSystem;
        _ownedPets = new List<Pet>();
        _selectedPetIndex = 0;
        _isActive = false;
    }
    
    public void Show()
    {
        _isActive = true;
        _ownedPets = _petSystem.OwnedPets;
        _selectedPetIndex = 0;
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
            _selectedPetIndex--;
            if (_selectedPetIndex < 0)
                _selectedPetIndex = _ownedPets.Count - 1;
        }
        
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedPetIndex++;
            if (_selectedPetIndex >= _ownedPets.Count)
                _selectedPetIndex = 0;
        }
        
        // Summon/dismiss pet
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            if (_ownedPets.Count > 0 && _selectedPetIndex >= 0 && _selectedPetIndex < _ownedPets.Count)
            {
                Pet selectedPet = _ownedPets[_selectedPetIndex];
                if (_petSystem.ActivePet == selectedPet)
                {
                    _petSystem.DismissPet();
                }
                else
                {
                    _petSystem.SummonPet(selectedPet);
                }
            }
        }
        
        // Interact with pet (feed/pet)
        if (keyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
        {
            if (_ownedPets.Count > 0 && _selectedPetIndex >= 0 && _selectedPetIndex < _ownedPets.Count)
            {
                Pet selectedPet = _ownedPets[_selectedPetIndex];
                _petSystem.InteractWithPet(selectedPet);
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
        spriteBatch.Draw(_pixelTexture, menuRect, new Color(50, 45, 40));
        DrawBorder(spriteBatch, menuRect, Color.SaddleBrown, 3);
        
        // Draw title
        string title = "Pet Management";
        Vector2 titleSize = font.MeasureString(title);
        Vector2 titlePos = new Vector2(menuX + (MENU_WIDTH - titleSize.X) / 2, menuY + PADDING);
        DrawTextWithShadow(spriteBatch, font, title, titlePos, Color.SandyBrown);
        
        // Draw active pet indicator
        int activePetY = menuY + PADDING * 2 + 20;
        string activePetText = _petSystem.ActivePet != null ? 
            $"Active Pet: {_petSystem.ActivePet.Name}" : "No Active Pet";
        Vector2 activePetSize = font.MeasureString(activePetText);
        Vector2 activePetPos = new Vector2(menuX + (MENU_WIDTH - activePetSize.X) / 2, activePetY);
        DrawTextWithShadow(spriteBatch, font, activePetText, activePetPos, 
                          _petSystem.ActivePet != null ? Color.LightGreen : Color.Gray);
        
        // Draw pets list
        int petStartY = activePetY + 35;
        int maxVisiblePets = 4;
        
        if (_ownedPets.Count == 0)
        {
            string noPets = "No pets owned yet. Tame animals to add them to your collection!";
            Vector2 noPetsSize = font.MeasureString(noPets);
            Vector2 noPetsPos = new Vector2(menuX + (MENU_WIDTH - noPetsSize.X) / 2, 
                                            menuY + MENU_HEIGHT / 2);
            DrawTextWithShadow(spriteBatch, font, noPets, noPetsPos, Color.Gray);
        }
        else
        {
            for (int i = 0; i < _ownedPets.Count && i < maxVisiblePets; i++)
            {
                Pet pet = _ownedPets[i];
                int petY = petStartY + i * PET_HEIGHT;
                bool isSelected = (i == _selectedPetIndex);
                bool isActive = (_petSystem.ActivePet == pet);
                
                // Draw pet background
                Rectangle petRect = new Rectangle(menuX + PADDING, petY, 
                                                  MENU_WIDTH - PADDING * 2, PET_HEIGHT - 5);
                Color bgColor = isSelected ? new Color(80, 70, 60) : new Color(60, 55, 50);
                if (isActive)
                    bgColor = new Color(70, 90, 60); // Green tint for active
                
                spriteBatch.Draw(_pixelTexture, petRect, bgColor);
                
                if (isSelected)
                {
                    DrawBorder(spriteBatch, petRect, Color.Yellow, 2);
                }
                
                // Draw pet type icon
                string typeIcon = GetPetTypeIcon(pet.Type);
                Vector2 iconPos = new Vector2(petRect.X + 10, petRect.Y + 10);
                DrawTextWithShadow(spriteBatch, font, typeIcon, iconPos, GetPetTypeColor(pet.Type));
                
                // Draw pet name and level
                string nameText = $"{pet.Name} (Lvl {pet.Level})";
                Vector2 namePos = new Vector2(petRect.X + 45, petRect.Y + 5);
                DrawTextWithShadow(spriteBatch, font, nameText, namePos, Color.White);
                
                // Draw health bar
                DrawStatBar(spriteBatch, font, petRect.X + 45, petRect.Y + 28, 
                           250, 15, pet.Health, pet.MaxHealth, "HP", Color.Red);
                
                // Draw hunger bar
                DrawStatBar(spriteBatch, font, petRect.X + 45, petRect.Y + 47, 
                           120, 12, pet.Hunger, 100f, "Hunger", Color.Orange);
                
                // Draw happiness bar
                DrawStatBar(spriteBatch, font, petRect.X + 45 + 130, petRect.Y + 47, 
                           120, 12, pet.Happiness, 100f, "Happy", Color.Yellow);
                
                // Draw pet ability
                string abilityText = GetPetAbilityName(pet.Ability);
                Vector2 abilityPos = new Vector2(petRect.X + 45, petRect.Y + 65);
                DrawTextWithShadow(spriteBatch, font, $"Ability: {abilityText}", abilityPos, 
                                  Color.Cyan * 0.9f);
                
                // Draw status
                string status = isActive ? "[X] Active" : "[ ] Inactive";
                Vector2 statusSize = font.MeasureString(status);
                Vector2 statusPos = new Vector2(petRect.Right - statusSize.X - 10, 
                                                petRect.Y + (petRect.Height - statusSize.Y) / 2);
                DrawTextWithShadow(spriteBatch, font, status, statusPos, 
                                  isActive ? Color.LightGreen : Color.Gray);
            }
        }
        
        // Draw controls hint at bottom
        string hint = "Up/Down: Navigate | Enter: Summon/Dismiss | Space: Interact | Esc: Close";
        Vector2 hintSize = font.MeasureString(hint);
        Vector2 hintPos = new Vector2(menuX + (MENU_WIDTH - hintSize.X) / 2, 
                                     menuY + MENU_HEIGHT - PADDING - hintSize.Y);
        DrawTextWithShadow(spriteBatch, font, hint, hintPos, Color.LightGray);
    }
    
    private void DrawStatBar(SpriteBatch spriteBatch, SpriteFont font, int x, int y, 
                            int width, int height, float current, float max, string label, Color color)
    {
        // Background
        Rectangle barBg = new Rectangle(x, y, width, height);
        spriteBatch.Draw(_pixelTexture, barBg, new Color(20, 20, 20));
        DrawBorder(spriteBatch, barBg, color * 0.7f, 1);
        
        // Fill
        float percent = current / max;
        int fillWidth = (int)(width * percent);
        Rectangle barFill = new Rectangle(x, y, fillWidth, height);
        spriteBatch.Draw(_pixelTexture, barFill, color * 0.7f);
        
        // Text (smaller font size effect by scaling)
        string text = $"{label}: {(int)current}/{(int)max}";
        Vector2 textSize = font.MeasureString(text) * 0.7f;
        Vector2 textPos = new Vector2(x + (width - textSize.X) / 2, y + (height - textSize.Y) / 2);
        spriteBatch.DrawString(font, text, textPos, Color.White, 0f, Vector2.Zero, 0.7f, 
                              SpriteEffects.None, 0f);
    }
    
    private string GetPetTypeIcon(PetType type)
    {
        return type switch
        {
            PetType.Companion => "♥",
            PetType.FarmHelper => "⚒",
            PetType.Combat => "⚔",
            PetType.Magical => "★",
            _ => "?"
        };
    }
    
    private Color GetPetTypeColor(PetType type)
    {
        return type switch
        {
            PetType.Companion => Color.Pink,
            PetType.FarmHelper => Color.Brown,
            PetType.Combat => Color.Red,
            PetType.Magical => Color.Violet,
            _ => Color.White
        };
    }
    
    private string GetPetAbilityName(PetAbility ability)
    {
        return ability switch
        {
            PetAbility.FindItems => "Find Items",
            PetAbility.ScareAnimals => "Scare Animals",
            PetAbility.ProduceEggs => "Produce Eggs",
            PetAbility.ProduceWool => "Produce Wool",
            PetAbility.ProduceMilk => "Produce Milk",
            PetAbility.AttackEnemies => "Attack Enemies",
            PetAbility.BoostMagic => "Boost Magic",
            PetAbility.GrowCrops => "Grow Crops",
            PetAbility.Revive => "Revive Player",
            _ => "None"
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
