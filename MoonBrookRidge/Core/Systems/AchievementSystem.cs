using System;
using System.Collections.Generic;
using System.Linq;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Manages player achievements and tracks progress
/// </summary>
public class AchievementSystem
{
    private Dictionary<string, Achievement> _achievements;
    private HashSet<string> _unlockedAchievements;
    private Dictionary<string, int> _progressTrackers;
    
    public event Action<Achievement> OnAchievementUnlocked;
    
    public AchievementSystem()
    {
        _achievements = new Dictionary<string, Achievement>();
        _unlockedAchievements = new HashSet<string>();
        _progressTrackers = new Dictionary<string, int>();
        
        InitializeAchievements();
    }
    
    private void InitializeAchievements()
    {
        // Farming Achievements
        AddAchievement(new Achievement(
            "first_harvest",
            "First Harvest",
            "Harvest your first crop",
            AchievementCategory.Farming,
            1
        ));
        
        AddAchievement(new Achievement(
            "green_thumb",
            "Green Thumb",
            "Harvest 100 crops",
            AchievementCategory.Farming,
            100
        ));
        
        AddAchievement(new Achievement(
            "master_farmer",
            "Master Farmer",
            "Harvest 1000 crops",
            AchievementCategory.Farming,
            1000
        ));
        
        AddAchievement(new Achievement(
            "crop_variety",
            "Crop Variety",
            "Plant 10 different crop types",
            AchievementCategory.Farming,
            10
        ));
        
        AddAchievement(new Achievement(
            "seasonal_farmer",
            "Seasonal Farmer",
            "Harvest crops in all 4 seasons",
            AchievementCategory.Farming,
            4
        ));
        
        // Fishing Achievements
        AddAchievement(new Achievement(
            "first_catch",
            "First Catch",
            "Catch your first fish",
            AchievementCategory.Fishing,
            1
        ));
        
        AddAchievement(new Achievement(
            "angler",
            "Angler",
            "Catch 50 fish",
            AchievementCategory.Fishing,
            50
        ));
        
        AddAchievement(new Achievement(
            "master_angler",
            "Master Angler",
            "Catch 200 fish",
            AchievementCategory.Fishing,
            200
        ));
        
        AddAchievement(new Achievement(
            "legendary_catch",
            "Legendary Catch",
            "Catch a legendary fish",
            AchievementCategory.Fishing,
            1
        ));
        
        // Mining Achievements
        AddAchievement(new Achievement(
            "first_ore",
            "First Ore",
            "Mine your first ore",
            AchievementCategory.Mining,
            1
        ));
        
        AddAchievement(new Achievement(
            "miner",
            "Miner",
            "Mine 100 resources",
            AchievementCategory.Mining,
            100
        ));
        
        AddAchievement(new Achievement(
            "master_miner",
            "Master Miner",
            "Mine 500 resources",
            AchievementCategory.Mining,
            500
        ));
        
        AddAchievement(new Achievement(
            "treasure_hunter",
            "Treasure Hunter",
            "Find 10 gems",
            AchievementCategory.Mining,
            10
        ));
        
        // Social Achievements
        AddAchievement(new Achievement(
            "friendly",
            "Friendly",
            "Give 10 gifts to NPCs",
            AchievementCategory.Social,
            10
        ));
        
        AddAchievement(new Achievement(
            "popular",
            "Popular",
            "Reach 5 hearts with any NPC",
            AchievementCategory.Social,
            1
        ));
        
        AddAchievement(new Achievement(
            "beloved",
            "Beloved",
            "Reach 10 hearts with any NPC",
            AchievementCategory.Social,
            1
        ));
        
        AddAchievement(new Achievement(
            "socialite",
            "Socialite",
            "Talk to all NPCs",
            AchievementCategory.Social,
            4 // Assuming 4 NPCs currently
        ));
        
        // Crafting Achievements
        AddAchievement(new Achievement(
            "first_craft",
            "First Craft",
            "Craft your first item",
            AchievementCategory.Crafting,
            1
        ));
        
        AddAchievement(new Achievement(
            "craftsman",
            "Craftsman",
            "Craft 50 items",
            AchievementCategory.Crafting,
            50
        ));
        
        AddAchievement(new Achievement(
            "master_craftsman",
            "Master Craftsman",
            "Craft 200 items",
            AchievementCategory.Crafting,
            200
        ));
        
        AddAchievement(new Achievement(
            "recipe_collector",
            "Recipe Collector",
            "Learn 15 different recipes",
            AchievementCategory.Crafting,
            15
        ));
        
        // Wealth Achievements
        AddAchievement(new Achievement(
            "first_earnings",
            "First Earnings",
            "Earn 1,000 gold",
            AchievementCategory.Wealth,
            1000
        ));
        
        AddAchievement(new Achievement(
            "entrepreneur",
            "Entrepreneur",
            "Earn 10,000 gold",
            AchievementCategory.Wealth,
            10000
        ));
        
        AddAchievement(new Achievement(
            "millionaire",
            "Millionaire",
            "Earn 100,000 gold",
            AchievementCategory.Wealth,
            100000
        ));
        
        // Exploration Achievements
        AddAchievement(new Achievement(
            "explorer",
            "Explorer",
            "Visit all map locations",
            AchievementCategory.Exploration,
            1
        ));
        
        AddAchievement(new Achievement(
            "builder",
            "Builder",
            "Construct your first building",
            AchievementCategory.Exploration,
            1
        ));
        
        AddAchievement(new Achievement(
            "architect",
            "Architect",
            "Construct 5 buildings",
            AchievementCategory.Exploration,
            5
        ));
        
        // Survival Achievements
        AddAchievement(new Achievement(
            "survivor",
            "Survivor",
            "Survive 10 days",
            AchievementCategory.Survival,
            10
        ));
        
        AddAchievement(new Achievement(
            "seasoned",
            "Seasoned",
            "Complete one full year",
            AchievementCategory.Survival,
            1
        ));
        
        AddAchievement(new Achievement(
            "veteran",
            "Veteran",
            "Survive 5 years",
            AchievementCategory.Survival,
            5
        ));
    }
    
    private void AddAchievement(Achievement achievement)
    {
        _achievements[achievement.Id] = achievement;
        _progressTrackers[achievement.Id] = 0;
    }
    
    /// <summary>
    /// Update progress for an achievement
    /// </summary>
    public void UpdateProgress(string achievementId, int amount = 1)
    {
        if (!_achievements.ContainsKey(achievementId))
            return;
            
        if (_unlockedAchievements.Contains(achievementId))
            return; // Already unlocked
            
        _progressTrackers[achievementId] += amount;
        
        var achievement = _achievements[achievementId];
        if (_progressTrackers[achievementId] >= achievement.RequiredProgress)
        {
            UnlockAchievement(achievementId);
        }
    }
    
    /// <summary>
    /// Set progress directly (for non-incremental achievements)
    /// </summary>
    public void SetProgress(string achievementId, int value)
    {
        if (!_achievements.ContainsKey(achievementId))
            return;
            
        if (_unlockedAchievements.Contains(achievementId))
            return;
            
        _progressTrackers[achievementId] = value;
        
        var achievement = _achievements[achievementId];
        if (_progressTrackers[achievementId] >= achievement.RequiredProgress)
        {
            UnlockAchievement(achievementId);
        }
    }
    
    private void UnlockAchievement(string achievementId)
    {
        if (_unlockedAchievements.Contains(achievementId))
            return;
            
        _unlockedAchievements.Add(achievementId);
        var achievement = _achievements[achievementId];
        achievement.IsUnlocked = true;
        achievement.UnlockedDate = DateTime.Now;
        
        OnAchievementUnlocked?.Invoke(achievement);
    }
    
    public int GetProgress(string achievementId)
    {
        return _progressTrackers.ContainsKey(achievementId) ? _progressTrackers[achievementId] : 0;
    }
    
    public bool IsUnlocked(string achievementId)
    {
        return _unlockedAchievements.Contains(achievementId);
    }
    
    public List<Achievement> GetAllAchievements()
    {
        return _achievements.Values.OrderBy(a => a.Category).ThenBy(a => a.Name).ToList();
    }
    
    public List<Achievement> GetUnlockedAchievements()
    {
        return _achievements.Values.Where(a => a.IsUnlocked).OrderByDescending(a => a.UnlockedDate).ToList();
    }
    
    public List<Achievement> GetAchievementsByCategory(AchievementCategory category)
    {
        return _achievements.Values.Where(a => a.Category == category).ToList();
    }
    
    public float GetCompletionPercentage()
    {
        if (_achievements.Count == 0) return 0;
        return (float)_unlockedAchievements.Count / _achievements.Count * 100f;
    }
}

/// <summary>
/// Represents a single achievement
/// </summary>
public class Achievement
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public AchievementCategory Category { get; set; }
    public int RequiredProgress { get; set; }
    public bool IsUnlocked { get; set; }
    public DateTime? UnlockedDate { get; set; }
    
    public Achievement(string id, string name, string description, AchievementCategory category, int requiredProgress)
    {
        Id = id;
        Name = name;
        Description = description;
        Category = category;
        RequiredProgress = requiredProgress;
        IsUnlocked = false;
        UnlockedDate = null;
    }
}

/// <summary>
/// Achievement categories
/// </summary>
public enum AchievementCategory
{
    Farming,
    Fishing,
    Mining,
    Social,
    Crafting,
    Wealth,
    Exploration,
    Survival
}
