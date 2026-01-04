using System;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Skills;
using MoonBrookRidge.Items.Crafting;
using MoonBrookRidge.Characters;
using MoonBrookRidge.Quests;
using MoonBrookRidge.Core.Systems;
using MoonBrookRidge.Magic;
using MoonBrookRidge.Pets;
using MoonBrookRidge.Factions;
using System.Collections.Generic;
using System.Linq;

namespace MoonBrookRidge.UI.Menus;

/// <summary>
/// Unified Player Menu consolidating all character-related menus into tabs
/// Tabs: Inventory, Crafting, Skills, Magic, Alchemy, Quests, Achievements, Pets, Factions, Social
/// </summary>
public class UnifiedPlayerMenu : TabbedMenu
{
    // Systems
    private InventorySystem _inventory;
    private SkillTreeSystem _skillSystem;
    private CraftingSystem _craftingSystem;
    private MarriageSystem _marriageSystem;
    private FamilySystem _familySystem;
    private QuestSystem _questSystem;
    private AchievementSystem _achievementSystem;
    private MagicSystem _magicSystem;
    private AlchemySystem _alchemySystem;
    private PetSystem _petSystem;
    private FactionSystem _factionSystem;
    
    // Tab-specific state
    private int _selectedInventorySlot;
    private int _selectedCraftingRecipe;
    private int _selectedChildIndex;
    private SkillCategory _selectedSkillCategory;
    private int _selectedQuestIndex;
    private int _selectedAchievementIndex;
    private int _selectedSpellIndex;
    private int _selectedPotionIndex;
    private int _selectedPetIndex;
    private int _selectedFactionIndex;
    
    public UnifiedPlayerMenu(
        InventorySystem inventory,
        SkillTreeSystem skillSystem,
        CraftingSystem craftingSystem,
        MarriageSystem marriageSystem,
        FamilySystem familySystem,
        QuestSystem questSystem,
        AchievementSystem achievementSystem,
        MagicSystem magicSystem,
        AlchemySystem alchemySystem,
        PetSystem petSystem,
        FactionSystem factionSystem)
    {
        _inventory = inventory;
        _skillSystem = skillSystem;
        _craftingSystem = craftingSystem;
        _marriageSystem = marriageSystem;
        _familySystem = familySystem;
        _questSystem = questSystem;
        _achievementSystem = achievementSystem;
        _magicSystem = magicSystem;
        _alchemySystem = alchemySystem;
        _petSystem = petSystem;
        _factionSystem = factionSystem;
        
        // Initialize tabs
        AddTab("Inv", "Inventory");          // 0
        AddTab("Craft", "Crafting");        // 1
        AddTab("Skills", "Skills & XP");    // 2
        AddTab("Magic", "Spells & Magic");  // 3
        AddTab("Alchemy", "Potions");       // 4
        AddTab("Quests", "Active Quests");  // 5
        AddTab("Achv", "Achievements");     // 6
        AddTab("Pets", "Your Pets");        // 7
        AddTab("Factions", "Reputation");   // 8
        AddTab("Social", "Family");         // 9
        
        // Initialize state
        _selectedInventorySlot = 0;
        _selectedCraftingRecipe = 0;
        _selectedChildIndex = 0;
        _selectedSkillCategory = SkillCategory.Farming;
        _selectedQuestIndex = 0;
        _selectedAchievementIndex = 0;
        _selectedSpellIndex = 0;
        _selectedPotionIndex = 0;
        _selectedPetIndex = 0;
        _selectedFactionIndex = 0;
    }
    
    protected override void UpdateTabContent(GameTime gameTime, KeyboardState keyboardState, MouseState mouseState)
    {
        switch (_selectedTabIndex)
        {
            case 0: // Inventory
                UpdateInventoryTab(keyboardState, mouseState);
                break;
            case 1: // Crafting
                UpdateCraftingTab(keyboardState, mouseState);
                break;
            case 2: // Skills
                UpdateSkillsTab(keyboardState, mouseState);
                break;
            case 3: // Magic
                UpdateMagicTab(keyboardState, mouseState);
                break;
            case 4: // Alchemy
                UpdateAlchemyTab(keyboardState, mouseState);
                break;
            case 5: // Quests
                UpdateQuestsTab(keyboardState, mouseState);
                break;
            case 6: // Achievements
                UpdateAchievementsTab(keyboardState, mouseState);
                break;
            case 7: // Pets
                UpdatePetsTab(keyboardState, mouseState);
                break;
            case 8: // Factions
                UpdateFactionsTab(keyboardState, mouseState);
                break;
            case 9: // Social
                UpdateSocialTab(keyboardState, mouseState);
                break;
        }
    }
    
    // TAB UPDATE METHODS
    private void UpdateInventoryTab(KeyboardState keyboardState, MouseState mouseState)
    {
        // Navigate inventory with arrow keys
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedInventorySlot -= 6;
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
    }
    
    private void UpdateCraftingTab(KeyboardState keyboardState, MouseState mouseState)
    {
        var recipes = _craftingSystem.GetAllRecipes().ToList();
        if (recipes.Count == 0) return;
        
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
        
        // Craft with Enter
        if (keyboardState.IsKeyDown(Keys.Enter) && !_previousKeyboardState.IsKeyDown(Keys.Enter))
        {
            var selectedRecipe = recipes[_selectedCraftingRecipe];
            if (_craftingSystem.CanCraft(selectedRecipe.Name, _inventory))
            {
                _craftingSystem.Craft(selectedRecipe.Name, _inventory);
            }
        }
    }
    
    private void UpdateSkillsTab(KeyboardState keyboardState, MouseState mouseState)
    {
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            int current = (int)_selectedSkillCategory;
            current--;
            if (current < 0) current = 5;
            _selectedSkillCategory = (SkillCategory)current;
        }
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            int current = (int)_selectedSkillCategory;
            current++;
            if (current > 5) current = 0;
            _selectedSkillCategory = (SkillCategory)current;
        }
    }
    
    private void UpdateMagicTab(KeyboardState keyboardState, MouseState mouseState)
    {
        var spells = _magicSystem.GetAvailableSpells().ToList();
        if (spells.Count == 0) return;
        
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedSpellIndex--;
            if (_selectedSpellIndex < 0) _selectedSpellIndex = spells.Count - 1;
        }
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedSpellIndex++;
            if (_selectedSpellIndex >= spells.Count) _selectedSpellIndex = 0;
        }
    }
    
    private void UpdateAlchemyTab(KeyboardState keyboardState, MouseState mouseState)
    {
        var recipes = _alchemySystem.GetAllRecipes().ToList();
        if (recipes.Count == 0) return;
        
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedPotionIndex--;
            if (_selectedPotionIndex < 0) _selectedPotionIndex = recipes.Count - 1;
        }
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedPotionIndex++;
            if (_selectedPotionIndex >= recipes.Count) _selectedPotionIndex = 0;
        }
    }
    
    private void UpdateQuestsTab(KeyboardState keyboardState, MouseState mouseState)
    {
        var quests = _questSystem.GetActiveQuests().ToList();
        if (quests.Count == 0) return;
        
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedQuestIndex--;
            if (_selectedQuestIndex < 0) _selectedQuestIndex = quests.Count - 1;
        }
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedQuestIndex++;
            if (_selectedQuestIndex >= quests.Count) _selectedQuestIndex = 0;
        }
    }
    
    private void UpdateAchievementsTab(KeyboardState keyboardState, MouseState mouseState)
    {
        var achievements = _achievementSystem.GetAllAchievements().ToList();
        if (achievements.Count == 0) return;
        
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedAchievementIndex--;
            if (_selectedAchievementIndex < 0) _selectedAchievementIndex = achievements.Count - 1;
        }
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedAchievementIndex++;
            if (_selectedAchievementIndex >= achievements.Count) _selectedAchievementIndex = 0;
        }
    }
    
    private void UpdatePetsTab(KeyboardState keyboardState, MouseState mouseState)
    {
        var pets = _petSystem.GetAvailablePets().ToList();
        if (pets.Count == 0) return;
        
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedPetIndex--;
            if (_selectedPetIndex < 0) _selectedPetIndex = pets.Count - 1;
        }
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedPetIndex++;
            if (_selectedPetIndex >= pets.Count) _selectedPetIndex = 0;
        }
    }
    
    private void UpdateFactionsTab(KeyboardState keyboardState, MouseState mouseState)
    {
        var factions = _factionSystem.GetAllFactions().ToList();
        if (factions.Count == 0) return;
        
        if (keyboardState.IsKeyDown(Keys.Up) && !_previousKeyboardState.IsKeyDown(Keys.Up))
        {
            _selectedFactionIndex--;
            if (_selectedFactionIndex < 0) _selectedFactionIndex = factions.Count - 1;
        }
        if (keyboardState.IsKeyDown(Keys.Down) && !_previousKeyboardState.IsKeyDown(Keys.Down))
        {
            _selectedFactionIndex++;
            if (_selectedFactionIndex >= factions.Count) _selectedFactionIndex = 0;
        }
    }
    
    private void UpdateSocialTab(KeyboardState keyboardState, MouseState mouseState)
    {
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
    
    // TAB DRAW METHODS
    protected override void DrawTabContent(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        switch (_selectedTabIndex)
        {
            case 0: DrawInventoryTab(spriteBatch, font, x, y, width, height); break;
            case 1: DrawCraftingTab(spriteBatch, font, x, y, width, height); break;
            case 2: DrawSkillsTab(spriteBatch, font, x, y, width, height); break;
            case 3: DrawMagicTab(spriteBatch, font, x, y, width, height); break;
            case 4: DrawAlchemyTab(spriteBatch, font, x, y, width, height); break;
            case 5: DrawQuestsTab(spriteBatch, font, x, y, width, height); break;
            case 6: DrawAchievementsTab(spriteBatch, font, x, y, width, height); break;
            case 7: DrawPetsTab(spriteBatch, font, x, y, width, height); break;
            case 8: DrawFactionsTab(spriteBatch, font, x, y, width, height); break;
            case 9: DrawSocialTab(spriteBatch, font, x, y, width, height); break;
        }
        
        // Draw instructions at bottom
        string instructions = "Tab/Q: Switch | E/Esc: Close | ←↑↓→: Navigate | Enter: Select";
        Vector2 instructSize = font.MeasureString(instructions);
        spriteBatch.DrawString(font, instructions,
            new Vector2(x + width / 2 - instructSize.X / 2, y + height - 30),
            Color.LightGray);
    }
    
    private void DrawInventoryTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        spriteBatch.DrawString(font, "Inventory", new Vector2(x + PADDING, y + PADDING), Color.White);
        
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
            
            Color slotColor = (i == _selectedInventorySlot) ? new Color(100, 100, 100) : new Color(50, 50, 50);
            spriteBatch.Draw(_pixelTexture, new Rectangle(slotX, slotY, slotSize, slotSize), slotColor);
            DrawBorder(spriteBatch, slotX, slotY, slotSize, slotSize, 2, Color.White);
            
            if (!slots[i].IsEmpty)
            {
                string itemName = slots[i].Item.Name.Length > 8 ? slots[i].Item.Name.Substring(0, 8) : slots[i].Item.Name;
                spriteBatch.DrawString(font, itemName, new Vector2(slotX + 5, slotY + 5), Color.White);
                spriteBatch.DrawString(font, $"x{slots[i].Quantity}", new Vector2(slotX + 5, slotY + 35), Color.Yellow);
            }
        }
        
        if (!slots[_selectedInventorySlot].IsEmpty)
        {
            var item = slots[_selectedInventorySlot].Item;
            int detailX = x + PADDING;
            int detailY = y + PADDING + 60;
            spriteBatch.DrawString(font, "Selected:", new Vector2(detailX, detailY), Color.LightGray);
            spriteBatch.DrawString(font, item.Name, new Vector2(detailX, detailY + 30), Color.White);
            spriteBatch.DrawString(font, $"Type: {item.Type}", new Vector2(detailX, detailY + 60), Color.LightBlue);
            spriteBatch.DrawString(font, $"Qty: {slots[_selectedInventorySlot].Quantity}", new Vector2(detailX, detailY + 90), Color.Yellow);
        }
    }
    
    private void DrawCraftingTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        spriteBatch.DrawString(font, "Crafting", new Vector2(x + PADDING, y + PADDING), Color.White);
        
        var recipes = _craftingSystem.GetAllRecipes().ToList();
        if (recipes.Count == 0)
        {
            spriteBatch.DrawString(font, "No recipes available", new Vector2(x + PADDING, y + PADDING + 60), Color.Gray);
            return;
        }
        
        int recipeY = y + PADDING + 60;
        for (int i = 0; i < Math.Min(recipes.Count, 10); i++)
        {
            bool isSelected = (i == _selectedCraftingRecipe);
            bool canCraft = _craftingSystem.CanCraft(recipes[i].Name, _inventory);
            
            Color bgColor = isSelected ? new Color(80, 70, 60) : new Color(50, 45, 40);
            spriteBatch.Draw(_pixelTexture, new Rectangle(x + PADDING, recipeY + i * 45, 400, 40), bgColor);
            
            Color textColor = canCraft ? Color.White : Color.Gray;
            if (isSelected) textColor = Color.Yellow;
            
            spriteBatch.DrawString(font, recipes[i].OutputName, new Vector2(x + PADDING + 10, recipeY + i * 45 + 10), textColor);
        }
    }
    
    private void DrawSkillsTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        spriteBatch.DrawString(font, "Skills", new Vector2(x + PADDING, y + PADDING), Color.White);
        
        int categoryY = y + PADDING + 60;
        string[] categoryNames = { "Farming", "Combat", "Magic", "Crafting", "Mining", "Fishing" };
        
        for (int i = 0; i < 6; i++)
        {
            SkillCategory category = (SkillCategory)i;
            bool isSelected = (category == _selectedSkillCategory);
            
            Color bgColor = isSelected ? new Color(80, 70, 60) : new Color(50, 45, 40);
            spriteBatch.Draw(_pixelTexture, new Rectangle(x + PADDING, categoryY + i * 60, 200, 50), bgColor);
            
            Color textColor = isSelected ? Color.Yellow : Color.White;
            spriteBatch.DrawString(font, categoryNames[i], new Vector2(x + PADDING + 10, categoryY + i * 60 + 15), textColor);
            
            int level = _skillSystem.GetSkillLevel(category);
            spriteBatch.DrawString(font, $"Lv {level}", new Vector2(x + PADDING + 150, categoryY + i * 60 + 15), Color.LightGreen);
        }
    }
    
    private void DrawMagicTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        spriteBatch.DrawString(font, "Magic & Spells", new Vector2(x + PADDING, y + PADDING), Color.White);
        
        var spells = _magicSystem.GetAvailableSpells().ToList();
        if (spells.Count == 0)
        {
            spriteBatch.DrawString(font, "No spells learned yet", new Vector2(x + PADDING, y + PADDING + 60), Color.Gray);
            return;
        }
        
        int spellY = y + PADDING + 60;
        for (int i = 0; i < Math.Min(spells.Count, 10); i++)
        {
            bool isSelected = (i == _selectedSpellIndex);
            Color textColor = isSelected ? Color.Yellow : Color.White;
            spriteBatch.DrawString(font, spells[i].Name, new Vector2(x + PADDING + 10, spellY + i * 35), textColor);
        }
    }
    
    private void DrawAlchemyTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        spriteBatch.DrawString(font, "Alchemy", new Vector2(x + PADDING, y + PADDING), Color.White);
        
        var recipes = _alchemySystem.GetAllRecipes().ToList();
        if (recipes.Count == 0)
        {
            spriteBatch.DrawString(font, "No potion recipes available", new Vector2(x + PADDING, y + PADDING + 60), Color.Gray);
            return;
        }
        
        int potionY = y + PADDING + 60;
        for (int i = 0; i < Math.Min(recipes.Count, 10); i++)
        {
            bool isSelected = (i == _selectedPotionIndex);
            Color textColor = isSelected ? Color.Yellow : Color.White;
            spriteBatch.DrawString(font, recipes[i].Name, new Vector2(x + PADDING + 10, potionY + i * 35), textColor);
        }
    }
    
    private void DrawQuestsTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        spriteBatch.DrawString(font, "Active Quests", new Vector2(x + PADDING, y + PADDING), Color.White);
        
        var quests = _questSystem.GetActiveQuests().ToList();
        if (quests.Count == 0)
        {
            spriteBatch.DrawString(font, "No active quests", new Vector2(x + PADDING, y + PADDING + 60), Color.Gray);
            return;
        }
        
        int questY = y + PADDING + 60;
        for (int i = 0; i < Math.Min(quests.Count, 8); i++)
        {
            bool isSelected = (i == _selectedQuestIndex);
            Color textColor = isSelected ? Color.Yellow : Color.White;
            spriteBatch.DrawString(font, quests[i].Title, new Vector2(x + PADDING + 10, questY + i * 45), textColor);
            spriteBatch.DrawString(font, quests[i].Description, new Vector2(x + PADDING + 10, questY + i * 45 + 20), Color.LightGray);
        }
    }
    
    private void DrawAchievementsTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        spriteBatch.DrawString(font, "Achievements", new Vector2(x + PADDING, y + PADDING), Color.White);
        
        var achievements = _achievementSystem.GetAllAchievements().ToList();
        if (achievements.Count == 0)
        {
            spriteBatch.DrawString(font, "No achievements yet", new Vector2(x + PADDING, y + PADDING + 60), Color.Gray);
            return;
        }
        
        int achvY = y + PADDING + 60;
        for (int i = 0; i < Math.Min(achievements.Count, 10); i++)
        {
            bool isSelected = (i == _selectedAchievementIndex);
            bool isUnlocked = achievements[i].IsUnlocked;
            Color textColor = isUnlocked ? (isSelected ? Color.Yellow : Color.LightGreen) : (isSelected ? Color.Yellow : Color.Gray);
            
            string prefix = isUnlocked ? "✓ " : "○ ";
            spriteBatch.DrawString(font, prefix + achievements[i].Name, new Vector2(x + PADDING + 10, achvY + i * 35), textColor);
        }
    }
    
    private void DrawPetsTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        spriteBatch.DrawString(font, "Your Pets", new Vector2(x + PADDING, y + PADDING), Color.White);
        
        var pets = _petSystem.GetAvailablePets().ToList();
        if (pets.Count == 0)
        {
            spriteBatch.DrawString(font, "No pets yet", new Vector2(x + PADDING, y + PADDING + 60), Color.Gray);
            return;
        }
        
        int petY = y + PADDING + 60;
        for (int i = 0; i < Math.Min(pets.Count, 8); i++)
        {
            bool isSelected = (i == _selectedPetIndex);
            Color textColor = isSelected ? Color.Yellow : Color.White;
            spriteBatch.DrawString(font, $"{pets[i].Name} ({pets[i].Type})", new Vector2(x + PADDING + 10, petY + i * 45), textColor);
            spriteBatch.DrawString(font, $"HP: {pets[i].MaxHealth} | Speed: {pets[i].Speed}", 
                new Vector2(x + PADDING + 10, petY + i * 45 + 20), Color.LightBlue);
        }
    }
    
    private void DrawFactionsTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        spriteBatch.DrawString(font, "Faction Reputation", new Vector2(x + PADDING, y + PADDING), Color.White);
        
        var factions = _factionSystem.GetAllFactions().ToList();
        if (factions.Count == 0)
        {
            spriteBatch.DrawString(font, "No factions discovered", new Vector2(x + PADDING, y + PADDING + 60), Color.Gray);
            return;
        }
        
        int factionY = y + PADDING + 60;
        for (int i = 0; i < Math.Min(factions.Count, 8); i++)
        {
            bool isSelected = (i == _selectedFactionIndex);
            Color textColor = isSelected ? Color.Yellow : Color.White;
            int reputation = _factionSystem.GetReputation(factions[i].Name);
            
            spriteBatch.DrawString(font, factions[i].Name, new Vector2(x + PADDING + 10, factionY + i * 45), textColor);
            spriteBatch.DrawString(font, $"Reputation: {reputation}", new Vector2(x + PADDING + 10, factionY + i * 45 + 20), Color.LightGreen);
        }
    }
    
    private void DrawSocialTab(SpriteBatch spriteBatch, SpriteFont font, int x, int y, int width, int height)
    {
        spriteBatch.DrawString(font, "Social & Family", new Vector2(x + PADDING, y + PADDING), Color.White);
        
        int contentY = y + PADDING + 60;
        
        if (_marriageSystem.IsMarried && _marriageSystem.Spouse != null)
        {
            spriteBatch.DrawString(font, $"Married to: {_marriageSystem.Spouse.Name}", 
                new Vector2(x + PADDING, contentY), Color.LightGreen);
            spriteBatch.DrawString(font, $"Days married: {_marriageSystem.DaysSinceMarriage}", 
                new Vector2(x + PADDING, contentY + 30), Color.White);
            contentY += 80;
        }
        else
        {
            spriteBatch.DrawString(font, "Not married", new Vector2(x + PADDING, contentY), Color.Gray);
            contentY += 40;
        }
        
        spriteBatch.Draw(_pixelTexture, new Rectangle(x + PADDING, contentY, width - PADDING * 2, 2), Color.Gray);
        contentY += 20;
        
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
                Color nameColor = isSelected ? Color.Yellow : Color.White;
                spriteBatch.DrawString(font, $"{child.Name} ({(child.IsBoy ? "Boy" : "Girl")})", 
                    new Vector2(x + PADDING + 10, contentY + i * 40), nameColor);
            }
        }
    }
}
