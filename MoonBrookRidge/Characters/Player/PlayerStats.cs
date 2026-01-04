using System;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Characters.Player;

/// <summary>
/// Player stats including health, energy, hunger, and thirst
/// </summary>
public class PlayerStats
{
    // Core stats
    private float _health;
    private float _maxHealth;
    private float _energy;
    private float _maxEnergy;
    private float _mana;
    private float _maxMana;
    
    // Survival stats (0-100%)
    private float _hunger;
    private float _thirst;
    
    // Stat decay rates (per second)
    private const float HUNGER_DECAY_RATE = 0.05f; // Lose 0.05% per second
    private const float THIRST_DECAY_RATE = 0.08f; // Thirst depletes faster than hunger
    
    // Activity multipliers
    private const float RUNNING_MULTIPLIER = 2.0f;
    private const float TOOL_USE_MULTIPLIER = 1.5f;
    private const float IDLE_MULTIPLIER = 0.5f;
    
    // Critical thresholds
    private const float LOW_STAT_THRESHOLD = 20f;
    private const float CRITICAL_STAT_THRESHOLD = 5f;
    
    // Debuff amounts
    private const float LOW_HUNGER_SPEED_PENALTY = 0.7f; // 30% slower
    private const float LOW_THIRST_ENERGY_PENALTY = 2f; // Extra energy drain
    private const float CRITICAL_HEALTH_DRAIN = 0.5f; // Health drain per second
    
    private int _money;
    
    public PlayerStats()
    {
        _maxHealth = 100f;
        _health = _maxHealth;
        _maxEnergy = 100f;
        _energy = _maxEnergy;
        _maxMana = 100f;
        _mana = _maxMana;
        
        // Start with full hunger and thirst
        _hunger = 100f;
        _thirst = 100f;
        
        _money = 500;
    }
    
    /// <summary>
    /// Update survival stats based on time and activity
    /// </summary>
    public void Update(GameTime gameTime, PlayerActivity currentActivity)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Calculate activity multiplier
        float activityMultiplier = GetActivityMultiplier(currentActivity);
        
        // Decay hunger and thirst
        _hunger = MathHelper.Clamp(_hunger - (HUNGER_DECAY_RATE * activityMultiplier * deltaTime), 0, 100);
        _thirst = MathHelper.Clamp(_thirst - (THIRST_DECAY_RATE * activityMultiplier * deltaTime), 0, 100);
        
        // Apply consequences for low stats
        ApplyStatEffects(deltaTime);
    }
    
    private float GetActivityMultiplier(PlayerActivity activity)
    {
        return activity switch
        {
            PlayerActivity.Idle => IDLE_MULTIPLIER,
            PlayerActivity.Walking => 1.0f,
            PlayerActivity.Running => RUNNING_MULTIPLIER,
            PlayerActivity.UsingTool => TOOL_USE_MULTIPLIER,
            PlayerActivity.Mining => TOOL_USE_MULTIPLIER * 1.2f,
            PlayerActivity.Fishing => IDLE_MULTIPLIER * 1.5f,
            _ => 1.0f
        };
    }
    
    private void ApplyStatEffects(float deltaTime)
    {
        // Critical hunger - drain health
        if (_hunger <= 0)
        {
            _health = MathHelper.Clamp(_health - (CRITICAL_HEALTH_DRAIN * deltaTime), 0, _maxHealth);
        }
        
        // Critical thirst - drain health faster
        if (_thirst <= 0)
        {
            _health = MathHelper.Clamp(_health - (CRITICAL_HEALTH_DRAIN * 1.5f * deltaTime), 0, _maxHealth);
        }
        
        // Low thirst - extra energy drain during activities
        if (_thirst < LOW_STAT_THRESHOLD && _energy > 0)
        {
            _energy = MathHelper.Clamp(_energy - (LOW_THIRST_ENERGY_PENALTY * deltaTime), 0, _maxEnergy);
        }
        
        // Check for blackout
        if (_health <= 0)
        {
            OnPlayerBlackout();
        }
    }
    
    private void OnPlayerBlackout()
    {
        // Player blacks out - will be handled by game state
        // Reset to safe state with penalties
        _health = _maxHealth * 0.5f;
        _energy = _maxEnergy * 0.25f;
        _hunger = 50f;
        _thirst = 50f;
        // Lose some money
        _money = (int)(_money * 0.9f);
    }
    
    /// <summary>
    /// Consume food to restore hunger
    /// </summary>
    public void Eat(float hungerRestored, float energyRestored = 0)
    {
        _hunger = MathHelper.Clamp(_hunger + hungerRestored, 0, 100);
        if (energyRestored > 0)
        {
            _energy = MathHelper.Clamp(_energy + energyRestored, 0, _maxEnergy);
        }
    }
    
    /// <summary>
    /// Drink to restore thirst
    /// </summary>
    public void Drink(float thirstRestored, float energyRestored = 0)
    {
        _thirst = MathHelper.Clamp(_thirst + thirstRestored, 0, 100);
        if (energyRestored > 0)
        {
            _energy = MathHelper.Clamp(_energy + energyRestored, 0, _maxEnergy);
        }
    }
    
    /// <summary>
    /// Get movement speed multiplier based on hunger level
    /// </summary>
    public float GetMovementSpeedMultiplier()
    {
        if (_hunger < LOW_STAT_THRESHOLD)
        {
            return LOW_HUNGER_SPEED_PENALTY;
        }
        return 1.0f;
    }
    
    /// <summary>
    /// Get tool efficiency based on stats
    /// </summary>
    public float GetToolEfficiency()
    {
        float efficiency = 1.0f;
        
        if (_hunger < LOW_STAT_THRESHOLD)
        {
            efficiency *= 0.7f; // 30% less efficient
        }
        
        if (_energy < 20f)
        {
            efficiency *= 0.8f; // 20% less efficient when tired
        }
        
        return efficiency;
    }
    
    /// <summary>
    /// Check if stat is at critical level
    /// </summary>
    public bool IsHungerCritical() => _hunger <= CRITICAL_STAT_THRESHOLD;
    public bool IsThirstCritical() => _thirst <= CRITICAL_STAT_THRESHOLD;
    public bool IsHungerLow() => _hunger <= LOW_STAT_THRESHOLD;
    public bool IsThirstLow() => _thirst <= LOW_STAT_THRESHOLD;
    
    // Property accessors
    public float Health { get => _health; set => _health = MathHelper.Clamp(value, 0, _maxHealth); }
    public float MaxHealth { get => _maxHealth; set => _maxHealth = value; }
    public float Energy { get => _energy; set => _energy = MathHelper.Clamp(value, 0, _maxEnergy); }
    public float MaxEnergy { get => _maxEnergy; set => _maxEnergy = value; }
    public float Mana { get => _mana; set => _mana = MathHelper.Clamp(value, 0, _maxMana); }
    public float MaxMana { get => _maxMana; set => _maxMana = value; }
    public float Hunger { get => _hunger; set => _hunger = MathHelper.Clamp(value, 0, 100f); }
    public float Thirst { get => _thirst; set => _thirst = MathHelper.Clamp(value, 0, 100f); }
    public int Money { get => _money; set => _money = Math.Max(0, value); }
    
    public void ModifyHealth(float amount) => Health += amount;
    public void ModifyEnergy(float amount) => Energy += amount;
    public void ModifyMana(float amount) => Mana += amount;
    public void ModifyMoney(int amount) => Money += amount;
    
    /// <summary>
    /// Consume energy for tool usage or actions
    /// </summary>
    public void ConsumeEnergy(float amount)
    {
        Energy -= amount;
    }
}

/// <summary>
/// Player activity states for survival stat decay calculation
/// </summary>
public enum PlayerActivity
{
    Idle,
    Walking,
    Running,
    UsingTool,
    Mining,
    Fishing,
    Chopping,
    Watering
}
