using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.Magic;

/// <summary>
/// Magic system managing spells, mana, and spell casting
/// </summary>
public class MagicSystem
{
    private List<Spell> _knownSpells;
    private List<Spell> _availableSpells;
    private float _mana;
    private float _maxMana;
    private const float MANA_REGEN_RATE = 1.0f; // Mana per second
    
    public float Mana => _mana;
    public float MaxMana => _maxMana;
    public List<Spell> KnownSpells => _knownSpells;
    
    public event Action<Spell> OnSpellCast;
    public event Action<Spell> OnSpellLearned;
    
    public MagicSystem(float maxMana = 100f)
    {
        _maxMana = maxMana;
        _mana = maxMana;
        _knownSpells = new List<Spell>();
        _availableSpells = new List<Spell>();
        
        // Initialize available spells
        InitializeSpells();
    }
    
    private void InitializeSpells()
    {
        // Starter spells - available to learn
        _availableSpells.Add(new Spell("heal", "Healing Touch", "Restore 30 health", 15f, SpellType.Healing, 30f));
        _availableSpells.Add(new Spell("speed", "Swift Step", "Increase movement speed for 10s", 20f, SpellType.Buff, 10f));
        _availableSpells.Add(new Spell("growth", "Nature's Blessing", "Instantly grow crops in 3x3 area", 40f, SpellType.Utility, 0f));
        _availableSpells.Add(new Spell("light", "Illumination", "Create light around player for 60s", 10f, SpellType.Utility, 60f));
        _availableSpells.Add(new Spell("water", "Rain Dance", "Water all crops on farm", 50f, SpellType.Utility, 0f));
        
        // Advanced spells - require skill levels
        _availableSpells.Add(new Spell("fireball", "Fireball", "Launch a fireball dealing 25 damage", 25f, SpellType.Combat, 25f));
        _availableSpells.Add(new Spell("teleport", "Teleport", "Instantly travel to waypoint", 60f, SpellType.Utility, 0f));
        _availableSpells.Add(new Spell("summon", "Summon Familiar", "Summon a helper creature", 75f, SpellType.Summon, 120f));
    }
    
    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Regenerate mana
        if (_mana < _maxMana)
        {
            _mana = MathHelper.Clamp(_mana + (MANA_REGEN_RATE * deltaTime), 0, _maxMana);
        }
    }
    
    public bool CanCastSpell(Spell spell)
    {
        return _knownSpells.Contains(spell) && _mana >= spell.ManaCost;
    }
    
    public bool CastSpell(string spellId)
    {
        var spell = _knownSpells.FirstOrDefault(s => s.Id == spellId);
        if (spell != null && CanCastSpell(spell))
        {
            _mana -= spell.ManaCost;
            OnSpellCast?.Invoke(spell);
            return true;
        }
        return false;
    }
    
    public bool LearnSpell(string spellId)
    {
        var spell = _availableSpells.FirstOrDefault(s => s.Id == spellId);
        if (spell != null && !_knownSpells.Contains(spell))
        {
            _knownSpells.Add(spell);
            OnSpellLearned?.Invoke(spell);
            return true;
        }
        return false;
    }
    
    public void RestoreMana(float amount)
    {
        _mana = MathHelper.Clamp(_mana + amount, 0, _maxMana);
    }
    
    public void IncreaseMaxMana(float amount)
    {
        _maxMana += amount;
        _mana = _maxMana; // Refill on upgrade
    }
    
    public List<Spell> GetAvailableSpells() => _availableSpells;
    
    /// <summary>
    /// Export magic system state for saving
    /// </summary>
    public MagicSaveData ExportSaveData()
    {
        return new MagicSaveData
        {
            CurrentMana = _mana,
            MaxMana = _maxMana,
            LearnedSpellIds = _knownSpells.Select(s => s.Id).ToArray()
        };
    }
    
    /// <summary>
    /// Import magic system state from save data
    /// </summary>
    public void ImportSaveData(MagicSaveData data)
    {
        if (data == null) return;
        
        _mana = data.CurrentMana;
        _maxMana = data.MaxMana;
        
        _knownSpells.Clear();
        if (data.LearnedSpellIds != null)
        {
            foreach (var spellId in data.LearnedSpellIds)
            {
                LearnSpell(spellId);
            }
        }
    }
}

/// <summary>
/// Individual spell definition
/// </summary>
public class Spell
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public float ManaCost { get; }
    public SpellType Type { get; }
    public float EffectValue { get; } // Damage, heal amount, duration, etc.
    public int RequiredLevel { get; set; }
    
    public Spell(string id, string name, string description, float manaCost, SpellType type, float effectValue)
    {
        Id = id;
        Name = name;
        Description = description;
        ManaCost = manaCost;
        Type = type;
        EffectValue = effectValue;
        RequiredLevel = 1;
    }
}

public enum SpellType
{
    Combat,      // Offensive spells (damage)
    Healing,     // Healing spells
    Buff,        // Temporary stat boosts
    Debuff,      // Enemy weakening
    Utility,     // Farming, teleport, light, etc.
    Summon       // Summon creatures or objects
}
