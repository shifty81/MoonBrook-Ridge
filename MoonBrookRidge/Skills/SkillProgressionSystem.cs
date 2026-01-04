using System;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.Skills;

/// <summary>
/// Manages skill progression by awarding experience for various player actions.
/// Acts as a bridge between game events and the SkillTreeSystem.
/// </summary>
public class SkillProgressionSystem
{
    private SkillTreeSystem _skillSystem;
    
    // Experience amounts for various actions
    private const float FARMING_PLANT_XP = 2f;
    private const float FARMING_WATER_XP = 1f;
    private const float FARMING_HARVEST_XP = 5f;
    private const float FARMING_HOE_XP = 1f;
    
    private const float MINING_STONE_XP = 3f;
    private const float MINING_COPPER_XP = 5f;
    private const float MINING_IRON_XP = 8f;
    private const float MINING_GOLD_XP = 12f;
    private const float MINING_GEM_XP = 20f;
    
    private const float COMBAT_KILL_BASE_XP = 10f; // Multiplied by enemy difficulty
    private const float COMBAT_DAMAGE_XP = 0.5f; // Per 10 damage dealt
    private const float COMBAT_BOSS_BONUS_XP = 50f;
    
    private const float FISHING_CATCH_COMMON_XP = 3f;
    private const float FISHING_CATCH_RARE_XP = 8f;
    private const float FISHING_CATCH_LEGENDARY_XP = 20f;
    
    private const float CRAFTING_COMMON_XP = 2f;
    private const float CRAFTING_RARE_XP = 5f;
    private const float CRAFTING_LEGENDARY_XP = 15f;
    
    private const float MAGIC_CAST_SPELL_XP = 3f;
    private const float MAGIC_BREW_POTION_XP = 5f;
    
    public event Action<SkillCategory, float> OnExperienceGained;
    
    public SkillProgressionSystem(SkillTreeSystem skillSystem)
    {
        _skillSystem = skillSystem ?? throw new ArgumentNullException(nameof(skillSystem));
        
        // Subscribe to skill level up events to provide feedback
        _skillSystem.OnSkillLevelUp += (category, newLevel) =>
        {
            // This event can be used by the game to show notifications
            System.Diagnostics.Debug.WriteLine($"Skill Level Up! {category} is now level {newLevel}");
        };
    }
    
    #region Farming Actions
    
    /// <summary>
    /// Award XP for planting a crop
    /// </summary>
    public void OnCropPlanted()
    {
        AddExperience(SkillCategory.Farming, FARMING_PLANT_XP);
    }
    
    /// <summary>
    /// Award XP for watering crops
    /// </summary>
    public void OnCropWatered(int cropCount = 1)
    {
        AddExperience(SkillCategory.Farming, FARMING_WATER_XP * cropCount);
    }
    
    /// <summary>
    /// Award XP for harvesting crops
    /// </summary>
    public void OnCropHarvested(bool isGoldQuality = false)
    {
        float xp = FARMING_HARVEST_XP;
        if (isGoldQuality) xp *= 1.5f; // 50% bonus for gold quality
        AddExperience(SkillCategory.Farming, xp);
    }
    
    /// <summary>
    /// Award XP for tilling soil
    /// </summary>
    public void OnSoilTilled()
    {
        AddExperience(SkillCategory.Farming, FARMING_HOE_XP);
    }
    
    #endregion
    
    #region Mining Actions
    
    /// <summary>
    /// Award XP for mining resources
    /// </summary>
    public void OnResourceMined(string resourceType)
    {
        float xp = resourceType.ToLower() switch
        {
            "stone" => MINING_STONE_XP,
            "copper" or "copper ore" => MINING_COPPER_XP,
            "tin" or "tin ore" => MINING_COPPER_XP,
            "iron" or "iron ore" => MINING_IRON_XP,
            "gold" or "gold ore" => MINING_GOLD_XP,
            "scarlet" or "scarlet ore" => MINING_IRON_XP * 1.5f,
            "octarine" or "octarine ore" => MINING_GOLD_XP * 1.5f,
            "galaxite" or "galaxite ore" => MINING_GOLD_XP * 2f,
            "diamond" or "ruby" or "emerald" or "sapphire" => MINING_GEM_XP,
            _ => MINING_STONE_XP
        };
        
        AddExperience(SkillCategory.Mining, xp);
    }
    
    /// <summary>
    /// Award XP for chopping wood (uses farming category for forestry)
    /// </summary>
    public void OnWoodChopped(int woodCount = 1)
    {
        // Wood chopping counts as farming (forestry)
        float xp = 3f * woodCount; // 3 XP per wood
        AddExperience(SkillCategory.Farming, xp);
    }
    
    #endregion
    
    #region Combat Actions
    
    /// <summary>
    /// Award XP for dealing damage
    /// </summary>
    public void OnDamageDealt(float damage)
    {
        // Award XP for every 10 damage dealt
        float xp = (damage / 10f) * COMBAT_DAMAGE_XP;
        AddExperience(SkillCategory.Combat, xp);
    }
    
    /// <summary>
    /// Award XP for killing an enemy
    /// </summary>
    public void OnEnemyKilled(string enemyType, int enemyLevel = 1, bool isBoss = false)
    {
        float xp = COMBAT_KILL_BASE_XP * enemyLevel;
        
        // Bonus XP for bosses
        if (isBoss)
        {
            xp += COMBAT_BOSS_BONUS_XP;
        }
        
        AddExperience(SkillCategory.Combat, xp);
    }
    
    #endregion
    
    #region Fishing Actions
    
    /// <summary>
    /// Award XP for catching fish
    /// </summary>
    public void OnFishCaught(string fishType, bool isRare = false, bool isLegendary = false)
    {
        float xp;
        if (isLegendary)
            xp = FISHING_CATCH_LEGENDARY_XP;
        else if (isRare)
            xp = FISHING_CATCH_RARE_XP;
        else
            xp = FISHING_CATCH_COMMON_XP;
        
        AddExperience(SkillCategory.Fishing, xp);
    }
    
    #endregion
    
    #region Crafting Actions
    
    /// <summary>
    /// Award XP for crafting items
    /// </summary>
    public void OnItemCrafted(string itemName, string rarity = "common")
    {
        float xp = rarity.ToLower() switch
        {
            "legendary" => CRAFTING_LEGENDARY_XP,
            "rare" => CRAFTING_RARE_XP,
            _ => CRAFTING_COMMON_XP
        };
        
        AddExperience(SkillCategory.Crafting, xp);
    }
    
    #endregion
    
    #region Magic Actions
    
    /// <summary>
    /// Award XP for casting spells
    /// </summary>
    public void OnSpellCast(string spellName)
    {
        AddExperience(SkillCategory.Magic, MAGIC_CAST_SPELL_XP);
    }
    
    /// <summary>
    /// Award XP for brewing potions (alchemy)
    /// </summary>
    public void OnPotionBrewed(string potionName)
    {
        AddExperience(SkillCategory.Magic, MAGIC_BREW_POTION_XP);
    }
    
    #endregion
    
    #region Helper Methods
    
    /// <summary>
    /// Add experience to a skill category and trigger event
    /// </summary>
    private void AddExperience(SkillCategory category, float amount)
    {
        if (amount <= 0) return;
        
        _skillSystem.AddExperience(category, amount);
        OnExperienceGained?.Invoke(category, amount);
    }
    
    /// <summary>
    /// Get current skill level for a category
    /// </summary>
    public int GetSkillLevel(SkillCategory category)
    {
        return _skillSystem.GetSkillLevel(category);
    }
    
    /// <summary>
    /// Get current experience for a category
    /// </summary>
    public float GetSkillExperience(SkillCategory category)
    {
        return _skillSystem.GetSkillExperience(category);
    }
    
    /// <summary>
    /// Get the SkillTreeSystem instance for direct access to skill trees
    /// </summary>
    public SkillTreeSystem GetSkillTreeSystem()
    {
        return _skillSystem;
    }
    
    #endregion
}
