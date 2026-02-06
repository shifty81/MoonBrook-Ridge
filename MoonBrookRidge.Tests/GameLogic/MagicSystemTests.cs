using Xunit;
using MoonBrookRidge.Magic;

namespace MoonBrookRidge.Tests.GameLogic;

public class MagicSystemTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithFullMana()
    {
        var magic = new MagicSystem(100f);
        Assert.Equal(100f, magic.Mana);
        Assert.Equal(100f, magic.MaxMana);
    }

    [Fact]
    public void Constructor_ShouldInitializeWithCustomMana()
    {
        var magic = new MagicSystem(200f);
        Assert.Equal(200f, magic.Mana);
        Assert.Equal(200f, magic.MaxMana);
    }

    [Fact]
    public void LearnSpell_ShouldAddSpellToKnownSpells()
    {
        var magic = new MagicSystem();
        var result = magic.LearnSpell("heal");
        Assert.True(result);
        Assert.Single(magic.KnownSpells);
        Assert.Equal("heal", magic.KnownSpells[0].Id);
    }

    [Fact]
    public void LearnSpell_ShouldReturnFalse_ForUnknownSpell()
    {
        var magic = new MagicSystem();
        var result = magic.LearnSpell("nonexistent");
        Assert.False(result);
        Assert.Empty(magic.KnownSpells);
    }

    [Fact]
    public void LearnSpell_ShouldNotDuplicateSpells()
    {
        var magic = new MagicSystem();
        magic.LearnSpell("heal");
        var result = magic.LearnSpell("heal");
        Assert.False(result);
        Assert.Single(magic.KnownSpells);
    }

    [Fact]
    public void CastSpell_ShouldConsumeMana()
    {
        var magic = new MagicSystem(100f);
        magic.LearnSpell("heal"); // Costs 15 mana
        var result = magic.CastSpell("heal");
        Assert.True(result);
        Assert.Equal(85f, magic.Mana);
    }

    [Fact]
    public void CastSpell_ShouldReturnFalse_WhenNotEnoughMana()
    {
        var magic = new MagicSystem(10f); // Only 10 mana
        magic.LearnSpell("heal"); // Costs 15 mana
        var result = magic.CastSpell("heal");
        Assert.False(result);
        Assert.Equal(10f, magic.Mana);
    }

    [Fact]
    public void CastSpell_ShouldReturnFalse_WhenSpellNotLearned()
    {
        var magic = new MagicSystem(100f);
        var result = magic.CastSpell("heal");
        Assert.False(result);
        Assert.Equal(100f, magic.Mana);
    }

    [Fact]
    public void ConsumeMana_ShouldReduceMana()
    {
        var magic = new MagicSystem(100f);
        magic.ConsumeMana(30f);
        Assert.Equal(70f, magic.Mana);
    }

    [Fact]
    public void ConsumeMana_ShouldNotGoBelowZero()
    {
        var magic = new MagicSystem(50f);
        magic.ConsumeMana(100f);
        Assert.Equal(0f, magic.Mana);
    }

    [Fact]
    public void RestoreMana_ShouldIncreaseMana()
    {
        var magic = new MagicSystem(100f);
        magic.ConsumeMana(50f);
        magic.RestoreMana(30f);
        Assert.Equal(80f, magic.Mana);
    }

    [Fact]
    public void RestoreMana_ShouldNotExceedMaxMana()
    {
        var magic = new MagicSystem(100f);
        magic.RestoreMana(50f);
        Assert.Equal(100f, magic.Mana);
    }

    [Fact]
    public void IncreaseMaxMana_ShouldIncreaseMaxAndRefill()
    {
        var magic = new MagicSystem(100f);
        magic.IncreaseMaxMana(50f);
        Assert.Equal(150f, magic.MaxMana);
        Assert.Equal(150f, magic.Mana);
    }

    [Fact]
    public void GetAvailableSpells_ShouldReturnAllSpells()
    {
        var magic = new MagicSystem();
        var spells = magic.GetAvailableSpells();
        Assert.True(spells.Count >= 8); // At least 8 spells defined
    }

    [Fact]
    public void OnSpellCast_EventShouldFire()
    {
        var magic = new MagicSystem(100f);
        magic.LearnSpell("heal");
        Spell? castSpell = null;
        magic.OnSpellCast += spell => castSpell = spell;
        
        magic.CastSpell("heal");
        
        Assert.NotNull(castSpell);
        Assert.Equal("heal", castSpell!.Id);
    }

    [Fact]
    public void OnSpellLearned_EventShouldFire()
    {
        var magic = new MagicSystem();
        Spell? learnedSpell = null;
        magic.OnSpellLearned += spell => learnedSpell = spell;
        
        magic.LearnSpell("speed");
        
        Assert.NotNull(learnedSpell);
        Assert.Equal("speed", learnedSpell!.Id);
    }

    [Fact]
    public void ExportSaveData_ShouldCaptureCurrentState()
    {
        var magic = new MagicSystem(100f);
        magic.LearnSpell("heal");
        magic.LearnSpell("speed");
        magic.ConsumeMana(20f);
        
        var saveData = magic.ExportSaveData();
        
        Assert.Equal(80f, saveData.CurrentMana);
        Assert.Equal(100f, saveData.MaxMana);
        Assert.Equal(2, saveData.LearnedSpellIds.Length);
        Assert.Contains("heal", saveData.LearnedSpellIds);
        Assert.Contains("speed", saveData.LearnedSpellIds);
    }
}
