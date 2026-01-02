using System;
using System.Collections.Generic;
using System.Linq;

namespace MoonBrookRidge.Skills;

/// <summary>
/// Skill tree system for player progression and specialization
/// </summary>
public class SkillTreeSystem
{
    private Dictionary<SkillCategory, SkillTree> _skillTrees;
    private Dictionary<SkillCategory, int> _skillLevels;
    private Dictionary<SkillCategory, float> _skillExperience;
    private int _availableSkillPoints;
    
    public int AvailableSkillPoints => _availableSkillPoints;
    
    public event Action<Skill> OnSkillUnlocked;
    public event Action<SkillCategory, int> OnSkillLevelUp;
    
    public SkillTreeSystem()
    {
        _skillTrees = new Dictionary<SkillCategory, SkillTree>();
        _skillLevels = new Dictionary<SkillCategory, int>();
        _skillExperience = new Dictionary<SkillCategory, float>();
        _availableSkillPoints = 0;
        
        InitializeSkillTrees();
    }
    
    private void InitializeSkillTrees()
    {
        // Initialize all skill categories
        foreach (SkillCategory category in Enum.GetValues(typeof(SkillCategory)))
        {
            _skillTrees[category] = new SkillTree(category);
            _skillLevels[category] = 1;
            _skillExperience[category] = 0f;
        }
        
        // Populate skill trees
        PopulateFarmingTree();
        PopulateCombatTree();
        PopulateMagicTree();
        PopulateCraftingTree();
        PopulateMiningTree();
        PopulateFishingTree();
    }
    
    private void PopulateFarmingTree()
    {
        var tree = _skillTrees[SkillCategory.Farming];
        
        // Tier 1
        tree.AddSkill(new Skill("farm_01", "Green Thumb", "Crops grow 10% faster", 1, SkillType.Passive, 0.1f));
        tree.AddSkill(new Skill("farm_02", "Efficient Watering", "Watering can uses 20% less energy", 1, SkillType.Passive, 0.2f));
        
        // Tier 2
        tree.AddSkill(new Skill("farm_03", "Quality Crops", "10% chance for gold-quality harvest", 2, SkillType.Passive, 0.1f, new[] { "farm_01" }));
        tree.AddSkill(new Skill("farm_04", "Sprinkler Master", "Sprinklers cover 50% more area", 2, SkillType.Passive, 0.5f, new[] { "farm_02" }));
        
        // Tier 3
        tree.AddSkill(new Skill("farm_05", "Harvest Master", "Harvest entire 3x3 area at once", 3, SkillType.Active, 1f, new[] { "farm_03" }));
        tree.AddSkill(new Skill("farm_06", "Seasonal Expert", "All seasons yield bonus crops", 3, SkillType.Passive, 0.15f, new[] { "farm_03", "farm_04" }));
    }
    
    private void PopulateCombatTree()
    {
        var tree = _skillTrees[SkillCategory.Combat];
        
        // Tier 1
        tree.AddSkill(new Skill("combat_01", "Warrior Training", "Increase melee damage by 15%", 1, SkillType.Passive, 0.15f));
        tree.AddSkill(new Skill("combat_02", "Tough Skin", "Reduce damage taken by 10%", 1, SkillType.Passive, 0.1f));
        
        // Tier 2
        tree.AddSkill(new Skill("combat_03", "Critical Strike", "15% chance for double damage", 2, SkillType.Passive, 0.15f, new[] { "combat_01" }));
        tree.AddSkill(new Skill("combat_04", "Shield Mastery", "Block 25% of incoming damage", 2, SkillType.Passive, 0.25f, new[] { "combat_02" }));
        
        // Tier 3
        tree.AddSkill(new Skill("combat_05", "Berserker Rage", "Temporary 50% damage boost", 3, SkillType.Active, 0.5f, new[] { "combat_03" }));
        tree.AddSkill(new Skill("combat_06", "Life Steal", "Heal 10% of damage dealt", 3, SkillType.Passive, 0.1f, new[] { "combat_03", "combat_04" }));
    }
    
    private void PopulateMagicTree()
    {
        var tree = _skillTrees[SkillCategory.Magic];
        
        // Tier 1
        tree.AddSkill(new Skill("magic_01", "Mana Efficiency", "Spells cost 15% less mana", 1, SkillType.Passive, 0.15f));
        tree.AddSkill(new Skill("magic_02", "Expanded Mana Pool", "Increase max mana by 25", 1, SkillType.Passive, 25f));
        
        // Tier 2
        tree.AddSkill(new Skill("magic_03", "Spell Power", "Increase spell effects by 20%", 2, SkillType.Passive, 0.2f, new[] { "magic_01" }));
        tree.AddSkill(new Skill("magic_04", "Rapid Regeneration", "Mana regenerates 50% faster", 2, SkillType.Passive, 0.5f, new[] { "magic_02" }));
        
        // Tier 3
        tree.AddSkill(new Skill("magic_05", "Arcane Mastery", "Unlock advanced spells", 3, SkillType.Unlock, 1f, new[] { "magic_03" }));
        tree.AddSkill(new Skill("magic_06", "Mana Shield", "Convert mana to damage absorption", 3, SkillType.Active, 2f, new[] { "magic_04" }));
    }
    
    private void PopulateCraftingTree()
    {
        var tree = _skillTrees[SkillCategory.Crafting];
        
        // Tier 1
        tree.AddSkill(new Skill("craft_01", "Efficient Crafting", "10% chance to save materials", 1, SkillType.Passive, 0.1f));
        tree.AddSkill(new Skill("craft_02", "Quality Craftsmanship", "Crafted items last 25% longer", 1, SkillType.Passive, 0.25f));
        
        // Tier 2
        tree.AddSkill(new Skill("craft_03", "Master Artisan", "Unlock rare crafting recipes", 2, SkillType.Unlock, 1f, new[] { "craft_01" }));
        tree.AddSkill(new Skill("craft_04", "Bulk Crafting", "Craft items in batches of 5", 2, SkillType.Active, 5f, new[] { "craft_01", "craft_02" }));
        
        // Tier 3
        tree.AddSkill(new Skill("craft_05", "Enchantment", "Add magical properties to items", 3, SkillType.Unlock, 1f, new[] { "craft_03" }));
    }
    
    private void PopulateMiningTree()
    {
        var tree = _skillTrees[SkillCategory.Mining];
        
        // Tier 1
        tree.AddSkill(new Skill("mine_01", "Efficient Mining", "Mining uses 20% less energy", 1, SkillType.Passive, 0.2f));
        tree.AddSkill(new Skill("mine_02", "Ore Detector", "Reveal ore locations nearby", 1, SkillType.Active, 10f));
        
        // Tier 2
        tree.AddSkill(new Skill("mine_03", "Prospector", "20% more ore from rocks", 2, SkillType.Passive, 0.2f, new[] { "mine_01" }));
        tree.AddSkill(new Skill("mine_04", "Gem Hunter", "Double chance to find gems", 2, SkillType.Passive, 2f, new[] { "mine_02" }));
        
        // Tier 3
        tree.AddSkill(new Skill("mine_05", "Mother Lode", "Rare chance to find ore veins", 3, SkillType.Passive, 0.05f, new[] { "mine_03", "mine_04" }));
    }
    
    private void PopulateFishingTree()
    {
        var tree = _skillTrees[SkillCategory.Fishing];
        
        // Tier 1
        tree.AddSkill(new Skill("fish_01", "Patient Angler", "Fish bite 25% faster", 1, SkillType.Passive, 0.25f));
        tree.AddSkill(new Skill("fish_02", "Strong Line", "Fishing line breaks 30% less", 1, SkillType.Passive, 0.3f));
        
        // Tier 2
        tree.AddSkill(new Skill("fish_03", "Trophy Hunter", "Catch bigger fish", 2, SkillType.Passive, 0.3f, new[] { "fish_01" }));
        tree.AddSkill(new Skill("fish_04", "Lucky Catch", "Higher chance for rare fish", 2, SkillType.Passive, 0.2f, new[] { "fish_02" }));
        
        // Tier 3
        tree.AddSkill(new Skill("fish_05", "Master Angler", "Catch legendary fish", 3, SkillType.Unlock, 1f, new[] { "fish_03", "fish_04" }));
    }
    
    public void AddExperience(SkillCategory category, float amount)
    {
        _skillExperience[category] += amount;
        
        // Check for level up
        int currentLevel = _skillLevels[category];
        float requiredXP = GetRequiredExperienceForLevel(currentLevel + 1);
        
        if (_skillExperience[category] >= requiredXP)
        {
            _skillLevels[category]++;
            _skillExperience[category] -= requiredXP;
            _availableSkillPoints++;
            
            OnSkillLevelUp?.Invoke(category, _skillLevels[category]);
        }
    }
    
    private float GetRequiredExperienceForLevel(int level)
    {
        // Progressive XP requirement: 100 * level^1.5
        return 100f * MathF.Pow(level, 1.5f);
    }
    
    public bool UnlockSkill(string skillId)
    {
        foreach (var tree in _skillTrees.Values)
        {
            var skill = tree.GetSkill(skillId);
            if (skill != null && _availableSkillPoints > 0)
            {
                // Check prerequisites
                if (skill.Prerequisites != null)
                {
                    foreach (var prereq in skill.Prerequisites)
                    {
                        if (!tree.IsSkillUnlocked(prereq))
                        {
                            return false; // Prerequisites not met
                        }
                    }
                }
                
                // Check level requirement
                if (_skillLevels[tree.Category] < skill.RequiredLevel)
                {
                    return false;
                }
                
                // Unlock skill
                tree.UnlockSkill(skillId);
                _availableSkillPoints--;
                OnSkillUnlocked?.Invoke(skill);
                return true;
            }
        }
        return false;
    }
    
    public SkillTree GetSkillTree(SkillCategory category) => _skillTrees[category];
    
    public int GetSkillLevel(SkillCategory category) => _skillLevels[category];
    
    public float GetSkillExperience(SkillCategory category) => _skillExperience[category];
}

/// <summary>
/// Skill tree for a specific category
/// </summary>
public class SkillTree
{
    public SkillCategory Category { get; }
    private List<Skill> _skills;
    private HashSet<string> _unlockedSkills;
    
    public SkillTree(SkillCategory category)
    {
        Category = category;
        _skills = new List<Skill>();
        _unlockedSkills = new HashSet<string>();
    }
    
    public void AddSkill(Skill skill)
    {
        _skills.Add(skill);
    }
    
    public void UnlockSkill(string skillId)
    {
        _unlockedSkills.Add(skillId);
    }
    
    public bool IsSkillUnlocked(string skillId) => _unlockedSkills.Contains(skillId);
    
    public Skill GetSkill(string skillId) => _skills.FirstOrDefault(s => s.Id == skillId);
    
    public List<Skill> GetAllSkills() => _skills;
    
    public List<Skill> GetUnlockedSkills() => _skills.Where(s => _unlockedSkills.Contains(s.Id)).ToList();
}

/// <summary>
/// Individual skill definition
/// </summary>
public class Skill
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public int RequiredLevel { get; }
    public SkillType Type { get; }
    public float Value { get; } // Multiplier, duration, etc.
    public string[] Prerequisites { get; } // Required skill IDs
    
    public Skill(string id, string name, string description, int requiredLevel, SkillType type, float value, string[] prerequisites = null)
    {
        Id = id;
        Name = name;
        Description = description;
        RequiredLevel = requiredLevel;
        Type = type;
        Value = value;
        Prerequisites = prerequisites;
    }
}

public enum SkillCategory
{
    Farming,
    Combat,
    Magic,
    Crafting,
    Mining,
    Fishing
}

public enum SkillType
{
    Passive,  // Always active bonuses
    Active,   // Activated abilities
    Unlock    // Unlock new features/recipes
}
