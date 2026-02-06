using Xunit;
using MoonBrookRidge.Skills;

namespace MoonBrookRidge.Tests.GameLogic;

public class SkillTreeSystemTests
{
    [Fact]
    public void Constructor_ShouldInitializeAllSkillCategories()
    {
        var skills = new SkillTreeSystem();
        
        foreach (SkillCategory category in Enum.GetValues(typeof(SkillCategory)))
        {
            Assert.Equal(1, skills.GetSkillLevel(category));
            Assert.Equal(0f, skills.GetSkillExperience(category));
        }
    }

    [Fact]
    public void Constructor_ShouldStartWithZeroSkillPoints()
    {
        var skills = new SkillTreeSystem();
        Assert.Equal(0, skills.AvailableSkillPoints);
    }

    [Fact]
    public void AddExperience_ShouldIncreaseExperience()
    {
        var skills = new SkillTreeSystem();
        skills.AddExperience(SkillCategory.Farming, 50f);
        Assert.Equal(50f, skills.GetSkillExperience(SkillCategory.Farming));
    }

    [Fact]
    public void AddExperience_ShouldLevelUpWhenEnoughXP()
    {
        var skills = new SkillTreeSystem();
        // Level 2 requires 100 * 2^1.5 ≈ 283 XP
        skills.AddExperience(SkillCategory.Farming, 300f);
        Assert.Equal(2, skills.GetSkillLevel(SkillCategory.Farming));
        Assert.Equal(1, skills.AvailableSkillPoints);
    }

    [Fact]
    public void AddExperience_ShouldFireLevelUpEvent()
    {
        var skills = new SkillTreeSystem();
        SkillCategory? leveledCategory = null;
        int? newLevel = null;
        skills.OnSkillLevelUp += (category, level) =>
        {
            leveledCategory = category;
            newLevel = level;
        };
        
        skills.AddExperience(SkillCategory.Combat, 300f);
        
        Assert.Equal(SkillCategory.Combat, leveledCategory);
        Assert.Equal(2, newLevel);
    }

    [Fact]
    public void UnlockSkill_ShouldReturnFalse_WhenNoSkillPoints()
    {
        var skills = new SkillTreeSystem();
        var result = skills.UnlockSkill("farm_01");
        Assert.False(result);
    }

    [Fact]
    public void UnlockSkill_ShouldSucceedWithSkillPoints()
    {
        var skills = new SkillTreeSystem();
        // Gain enough XP for a skill point
        skills.AddExperience(SkillCategory.Farming, 300f);
        Assert.Equal(1, skills.AvailableSkillPoints);
        
        var result = skills.UnlockSkill("farm_01");
        Assert.True(result);
        Assert.Equal(0, skills.AvailableSkillPoints);
    }

    [Fact]
    public void UnlockSkill_ShouldReturnFalse_ForNonexistentSkill()
    {
        var skills = new SkillTreeSystem();
        skills.AddExperience(SkillCategory.Farming, 300f);
        var result = skills.UnlockSkill("nonexistent_skill");
        Assert.False(result);
    }

    [Fact]
    public void UnlockSkill_ShouldFireEvent()
    {
        var skills = new SkillTreeSystem();
        skills.AddExperience(SkillCategory.Farming, 300f);
        
        Skill? unlockedSkill = null;
        skills.OnSkillUnlocked += skill => unlockedSkill = skill;
        
        skills.UnlockSkill("farm_01");
        
        Assert.NotNull(unlockedSkill);
        Assert.Equal("farm_01", unlockedSkill!.Id);
    }

    [Fact]
    public void UnlockSkill_ShouldEnforcePrerequisites()
    {
        var skills = new SkillTreeSystem();
        // Get enough XP for 2 skill points
        skills.AddExperience(SkillCategory.Farming, 300f);
        skills.AddExperience(SkillCategory.Farming, 1000f);
        
        // farm_03 requires farm_01 - should fail without prerequisite
        var result = skills.UnlockSkill("farm_03");
        Assert.False(result);
        
        // Unlock prerequisite first
        skills.UnlockSkill("farm_01");
        result = skills.UnlockSkill("farm_03");
        Assert.True(result);
    }

    [Fact]
    public void GetSkillTree_ShouldReturnTreeForCategory()
    {
        var skills = new SkillTreeSystem();
        var farmingTree = skills.GetSkillTree(SkillCategory.Farming);
        Assert.NotNull(farmingTree);
        Assert.Equal(SkillCategory.Farming, farmingTree.Category);
    }

    [Fact]
    public void AddExperience_ShouldNotAffectOtherCategories()
    {
        var skills = new SkillTreeSystem();
        skills.AddExperience(SkillCategory.Farming, 50f);
        Assert.Equal(0f, skills.GetSkillExperience(SkillCategory.Combat));
        Assert.Equal(0f, skills.GetSkillExperience(SkillCategory.Mining));
    }
}
