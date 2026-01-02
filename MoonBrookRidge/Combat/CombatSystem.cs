using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MoonBrookRidge.Combat;

/// <summary>
/// Combat system managing weapons, enemies, and battle mechanics
/// </summary>
public class CombatSystem
{
    private List<Enemy> _activeEnemies;
    private List<Weapon> _playerWeapons;
    private Weapon _equippedWeapon;
    
    public event Action<Enemy, float> OnEnemyDamaged;
    public event Action<Enemy> OnEnemyDefeated;
    public event Action<float> OnPlayerDamaged;
    
    public CombatSystem()
    {
        _activeEnemies = new List<Enemy>();
        _playerWeapons = new List<Weapon>();
        
        InitializeWeapons();
    }
    
    private void InitializeWeapons()
    {
        // Starter weapons
        var rustySword = new Weapon("rusty_sword", "Rusty Sword", "An old sword, but still sharp", 
            WeaponType.Melee, 10f, 1.0f, 5f, 50);
        _playerWeapons.Add(rustySword);
        _equippedWeapon = rustySword;
        
        // Melee weapons
        _playerWeapons.Add(new Weapon("wooden_club", "Wooden Club", "A sturdy wooden club", 
            WeaponType.Melee, 8f, 1.2f, 3f, 0));
        _playerWeapons.Add(new Weapon("iron_sword", "Iron Sword", "A well-crafted iron sword", 
            WeaponType.Melee, 20f, 1.0f, 10f, 100));
        _playerWeapons.Add(new Weapon("steel_sword", "Steel Sword", "A sharp steel blade", 
            WeaponType.Melee, 35f, 0.9f, 15f, 200));
        _playerWeapons.Add(new Weapon("golden_sword", "Golden Sword", "A legendary golden blade", 
            WeaponType.Melee, 50f, 0.8f, 25f, 500));
        
        // Ranged weapons
        _playerWeapons.Add(new Weapon("wooden_bow", "Wooden Bow", "A simple hunting bow", 
            WeaponType.Ranged, 12f, 1.5f, 15f, 75));
        _playerWeapons.Add(new Weapon("crossbow", "Crossbow", "A powerful crossbow", 
            WeaponType.Ranged, 25f, 2.0f, 20f, 150));
        _playerWeapons.Add(new Weapon("longbow", "Longbow", "A masterwork longbow", 
            WeaponType.Ranged, 40f, 1.8f, 30f, 300));
        
        // Magic weapons
        _playerWeapons.Add(new Weapon("magic_staff", "Magic Staff", "A staff that channels magic", 
            WeaponType.Magic, 15f, 1.2f, 20f, 100, usesMana: true));
        _playerWeapons.Add(new Weapon("fire_wand", "Fire Wand", "Launches fireballs", 
            WeaponType.Magic, 30f, 1.0f, 35f, 250, usesMana: true));
        _playerWeapons.Add(new Weapon("arcane_staff", "Arcane Staff", "Channels pure arcane energy", 
            WeaponType.Magic, 55f, 0.9f, 50f, 600, usesMana: true));
    }
    
    public void Update(GameTime gameTime)
    {
        // Update all active enemies
        for (int i = _activeEnemies.Count - 1; i >= 0; i--)
        {
            _activeEnemies[i].Update(gameTime);
            
            // Remove defeated enemies
            if (_activeEnemies[i].IsDead)
            {
                _activeEnemies.RemoveAt(i);
            }
        }
    }
    
    public bool AttackEnemy(Enemy target, float playerDamageModifier = 1.0f)
    {
        if (_equippedWeapon == null || target == null || target.IsDead)
        {
            return false;
        }
        
        // Calculate damage
        float baseDamage = _equippedWeapon.Damage;
        float finalDamage = baseDamage * playerDamageModifier;
        
        // Apply damage to enemy
        target.TakeDamage(finalDamage);
        OnEnemyDamaged?.Invoke(target, finalDamage);
        
        // Check if enemy defeated
        if (target.IsDead)
        {
            OnEnemyDefeated?.Invoke(target);
        }
        
        return true;
    }
    
    public void PlayerTakeDamage(float damage, float defenseModifier = 1.0f)
    {
        float finalDamage = damage * defenseModifier;
        OnPlayerDamaged?.Invoke(finalDamage);
    }
    
    public void SpawnEnemy(Enemy enemy)
    {
        _activeEnemies.Add(enemy);
    }
    
    public void EquipWeapon(string weaponId)
    {
        var weapon = _playerWeapons.FirstOrDefault(w => w.Id == weaponId);
        if (weapon != null)
        {
            _equippedWeapon = weapon;
        }
    }
    
    public Weapon GetEquippedWeapon() => _equippedWeapon;
    
    public List<Weapon> GetPlayerWeapons() => _playerWeapons;
    
    public List<Enemy> GetActiveEnemies() => _activeEnemies;
    
    public void AddWeapon(Weapon weapon)
    {
        if (!_playerWeapons.Contains(weapon))
        {
            _playerWeapons.Add(weapon);
        }
    }
}

/// <summary>
/// Weapon definition for combat
/// </summary>
public class Weapon
{
    public string Id { get; }
    public string Name { get; }
    public string Description { get; }
    public WeaponType Type { get; }
    public float Damage { get; }
    public float AttackSpeed { get; } // Attacks per second
    public float EnergyCost { get; } // Energy consumed per attack
    public int Value { get; } // Sell price
    public bool UsesMana { get; } // Magic weapons use mana instead of energy
    
    public Weapon(string id, string name, string description, WeaponType type, 
        float damage, float attackSpeed, float energyCost, int value, bool usesMana = false)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
        Damage = damage;
        AttackSpeed = attackSpeed;
        EnergyCost = energyCost;
        Value = value;
        UsesMana = usesMana;
    }
}

/// <summary>
/// Enemy entity for combat
/// </summary>
public class Enemy
{
    public string Id { get; }
    public string Name { get; }
    public EnemyType Type { get; }
    public float Health { get; private set; }
    public float MaxHealth { get; }
    public float Damage { get; }
    public float Defense { get; } // Damage reduction (0-1)
    public float Speed { get; }
    public int Experience { get; } // XP given on defeat
    public List<LootDrop> LootTable { get; }
    public Vector2 Position { get; set; }
    public bool IsDead => Health <= 0;
    public bool IsBoss { get; }
    
    private float _attackCooldown;
    private const float ATTACK_INTERVAL = 2.0f; // Attack every 2 seconds
    
    public Enemy(string id, string name, EnemyType type, float health, float damage, 
        float defense, float speed, int experience, bool isBoss = false)
    {
        Id = id;
        Name = name;
        Type = type;
        MaxHealth = health;
        Health = health;
        Damage = damage;
        Defense = defense;
        Speed = speed;
        Experience = experience;
        IsBoss = isBoss;
        LootTable = new List<LootDrop>();
        _attackCooldown = ATTACK_INTERVAL;
    }
    
    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Update attack cooldown
        if (_attackCooldown > 0)
        {
            _attackCooldown -= deltaTime;
        }
    }
    
    public void TakeDamage(float amount)
    {
        float damageReduction = 1.0f - Defense;
        float finalDamage = amount * damageReduction;
        Health = Math.Max(0, Health - finalDamage);
    }
    
    public void Heal(float amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }
    
    public bool CanAttack()
    {
        return _attackCooldown <= 0;
    }
    
    public void Attack()
    {
        _attackCooldown = ATTACK_INTERVAL;
    }
    
    public void AddLoot(string itemName, float dropChance, int minQuantity = 1, int maxQuantity = 1)
    {
        LootTable.Add(new LootDrop(itemName, dropChance, minQuantity, maxQuantity));
    }
}

/// <summary>
/// Loot drop from enemies
/// </summary>
public class LootDrop
{
    public string ItemName { get; }
    public float DropChance { get; } // 0-1 probability
    public int MinQuantity { get; }
    public int MaxQuantity { get; }
    
    public LootDrop(string itemName, float dropChance, int minQuantity = 1, int maxQuantity = 1)
    {
        ItemName = itemName;
        DropChance = dropChance;
        MinQuantity = minQuantity;
        MaxQuantity = maxQuantity;
    }
}

public enum WeaponType
{
    Melee,   // Swords, axes, clubs
    Ranged,  // Bows, crossbows
    Magic    // Staffs, wands
}

public enum EnemyType
{
    Slime,
    Skeleton,
    Goblin,
    Spider,
    Bat,
    Wolf,
    Ghost,
    Zombie,
    Orc,
    Dragon,
    Elemental,
    Demon
}
