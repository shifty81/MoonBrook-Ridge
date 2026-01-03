# Auto-Combat Mechanics - Technical Specification

## Overview

MoonBrook Valley features an **auto-shooter combat system** where weapons automatically fire and reload without player input. This document details the technical implementation and design philosophy behind this system.

## Design Philosophy

### Core Principles
1. **Automation Without Loss of Agency**: Weapons fire automatically, but players make meaningful strategic choices
2. **Positioning Over Reflexes**: Success comes from smart positioning rather than precise aim
3. **Build Diversity**: Multiple viable weapon combinations and strategies
4. **Clear Feedback**: Players always know what their weapons are doing

### What the Player Controls
- **Movement and positioning** (WASD)
- **Weapon loadout selection** (which weapons to equip)
- **Build path choices** (upgrades and passive bonuses)
- **Resource management** (energy/mana for weapons)
- **Grenade timing** (manual AoE abilities)
- **Pet commands** (summon, dismiss, special abilities)

### What Happens Automatically
- **Weapon firing** (shoots at valid targets)
- **Weapon reloading** (when ammo depleted)
- **Target selection** (based on weapon AI)
- **Projectile tracking** (for homing weapons)
- **Pet basic attacks** (pet auto-attacks nearby enemies)

## Weapon System Architecture

### AutoWeapon Class
```csharp
public class AutoWeapon
{
    // Identity
    public string Id { get; set; }
    public string Name { get; set; }
    public WeaponType Type { get; set; }
    
    // Stats
    public float Damage { get; set; }
    public float FireRate { get; set; } // Shots per second
    public float Range { get; set; } // Maximum target distance
    public int MagazineSize { get; set; }
    public float ReloadTime { get; set; } // Seconds to reload
    
    // Resource costs
    public float EnergyCost { get; set; } // Energy per shot
    public float ManaCost { get; set; } // Mana per shot (magic weapons)
    
    // Behavior
    public FiringPattern Pattern { get; set; }
    public TargetingPriority Priority { get; set; }
    public DamageType DamageType { get; set; }
    
    // State
    public int CurrentAmmo { get; set; }
    public float TimeSinceLastShot { get; set; }
    public bool IsReloading { get; set; }
    public float ReloadProgress { get; set; }
    
    // Methods
    public bool CanFire(float playerEnergy, float playerMana)
    {
        return CurrentAmmo > 0 
            && !IsReloading 
            && FireRate > 0  // Safety check: prevent division by zero
            && TimeSinceLastShot >= (1.0f / FireRate)
            && playerEnergy >= EnergyCost
            && playerMana >= ManaCost;
    }
    
    public void Fire(Enemy target, Vector2 playerPosition, Vector2 playerDirection)
    {
        if (!CanFire()) return;
        
        // Create projectile based on firing pattern
        var projectile = CreateProjectile(target, playerPosition, playerDirection);
        
        // Consume ammo and resources
        CurrentAmmo--;
        TimeSinceLastShot = 0;
        
        // Start reload if empty
        if (CurrentAmmo <= 0)
        {
            StartReload();
        }
    }
    
    public void Update(float deltaTime)
    {
        TimeSinceLastShot += deltaTime;
        
        if (IsReloading)
        {
            ReloadProgress += deltaTime;
            if (ReloadProgress >= ReloadTime)
            {
                CompleteReload();
            }
        }
    }
    
    private void StartReload()
    {
        IsReloading = true;
        ReloadProgress = 0;
    }
    
    private void CompleteReload()
    {
        IsReloading = false;
        CurrentAmmo = MagazineSize;
        ReloadProgress = 0;
    }
}
```

### Firing Patterns

#### Forward Firing
Shoots in the direction the player is facing:
```csharp
public class ForwardFiringPattern : IFiringPattern
{
    public Vector2 GetFiringDirection(Vector2 playerPosition, Vector2 playerDirection, Enemy target)
    {
        // Always shoot in player's facing direction
        return playerDirection.Normalized();
    }
    
    public bool CanTargetEnemy(Vector2 playerPosition, Vector2 playerDirection, Enemy enemy, float range)
    {
        // Check if enemy is within cone in front of player
        var toEnemy = (enemy.Position - playerPosition).Normalized();
        var angle = Vector2.Dot(playerDirection, toEnemy);
        
        // 60-degree cone in front (angle > 0.5 = 60 degrees from center, 120 degrees total)
        // Use 0.707f for 45-degree cone (90 degrees total) if narrower coverage is desired
        return angle > 0.5f && Vector2.Distance(playerPosition, enemy.Position) <= range;
    }
}
```

#### Backward Firing
Shoots behind the player:
```csharp
public class BackwardFiringPattern : IFiringPattern
{
    public Vector2 GetFiringDirection(Vector2 playerPosition, Vector2 playerDirection, Enemy target)
    {
        // Shoot opposite to player's facing direction
        return -playerDirection.Normalized();
    }
    
    public bool CanTargetEnemy(Vector2 playerPosition, Vector2 playerDirection, Enemy enemy, float range)
    {
        // Check if enemy is behind player
        var toEnemy = (enemy.Position - playerPosition).Normalized();
        var angle = Vector2.Dot(playerDirection, toEnemy);
        
        // 90-degree cone behind player (angle < -0.5)
        return angle < -0.5f && Vector2.Distance(playerPosition, enemy.Position) <= range;
    }
}
```

#### 360-Degree Firing
Shoots in all directions:
```csharp
public class OmnidirectionalFiringPattern : IFiringPattern
{
    public Vector2 GetFiringDirection(Vector2 playerPosition, Vector2 playerDirection, Enemy target)
    {
        // Shoot directly at target
        return (target.Position - playerPosition).Normalized();
    }
    
    public bool CanTargetEnemy(Vector2 playerPosition, Vector2 playerDirection, Enemy enemy, float range)
    {
        // Can target any enemy within range
        return Vector2.Distance(playerPosition, enemy.Position) <= range;
    }
}
```

#### Targeted Firing
Prioritizes specific targets:
```csharp
public class TargetedFiringPattern : IFiringPattern
{
    public TargetingPriority Priority { get; set; }
    
    public Vector2 GetFiringDirection(Vector2 playerPosition, Vector2 playerDirection, Enemy target)
    {
        // Shoot directly at high-priority target
        return (target.Position - playerPosition).Normalized();
    }
    
    public Enemy SelectTarget(List<Enemy> enemies, Vector2 playerPosition, float range)
    {
        var validTargets = enemies.Where(e => 
            Vector2.Distance(playerPosition, e.Position) <= range).ToList();
        
        if (!validTargets.Any()) return null;
        
        return Priority switch
        {
            TargetingPriority.Closest => validTargets.Aggregate((closest, next) => 
                Vector2.Distance(playerPosition, closest.Position) < 
                Vector2.Distance(playerPosition, next.Position) ? closest : next),
            TargetingPriority.LowestHP => validTargets.Aggregate((min, next) => 
                min.CurrentHP < next.CurrentHP ? min : next),
            TargetingPriority.HighestHP => validTargets.Aggregate((max, next) => 
                max.CurrentHP > next.CurrentHP ? max : next),
            TargetingPriority.HighestThreat => validTargets.Aggregate((max, next) => 
                max.ThreatLevel > next.ThreatLevel ? max : next),
            _ => validTargets.First()
        };
    }
}
```

### Targeting Priority System

```csharp
public enum TargetingPriority
{
    Closest,        // Target nearest enemy
    LowestHP,       // Target weakest enemy (execute priority)
    HighestHP,      // Target strongest enemy (focus fire)
    HighestThreat,  // Target most dangerous enemy (survival priority)
    Random          // Random target (for spreading damage)
}
```

## Auto-Combat Manager

### Main Combat Loop
```csharp
public class AutoCombatManager
{
    private List<AutoWeapon> _equippedWeapons;
    private List<Enemy> _activeEnemies;
    private Player _player;
    private PetCompanion _activePet;
    
    public void Update(GameTime gameTime, Vector2 playerPosition, Vector2 playerDirection)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Update all weapons
        foreach (var weapon in _equippedWeapons)
        {
            weapon.Update(deltaTime);
            
            // Try to fire if ready
            if (weapon.CanFire(_player.CurrentEnergy, _player.CurrentMana))
            {
                var target = FindTarget(weapon, playerPosition, playerDirection);
                if (target != null)
                {
                    weapon.Fire(target, playerPosition, playerDirection);
                    
                    // Consume resources
                    _player.CurrentEnergy -= weapon.EnergyCost;
                    _player.CurrentMana -= weapon.ManaCost;
                }
            }
        }
        
        // Update pet combat
        if (_activePet != null)
        {
            _activePet.UpdateCombat(deltaTime, playerPosition, _activeEnemies);
        }
    }
    
    private Enemy FindTarget(AutoWeapon weapon, Vector2 playerPosition, Vector2 playerDirection)
    {
        // Get valid targets based on weapon's firing pattern
        var validTargets = _activeEnemies.Where(e => 
            weapon.Pattern.CanTargetEnemy(playerPosition, playerDirection, e, weapon.Range) 
            && !e.IsDead).ToList();
        
        if (!validTargets.Any()) return null;
        
        // Select target based on weapon's priority
        return weapon.Pattern.SelectTarget(validTargets, playerPosition, weapon.Range);
    }
}
```

## Weapon Loadout System

### Weapon Slots
Players can equip multiple weapons simultaneously:
- **Starting**: 2 weapon slots
- **Upgrade 1**: 3 weapon slots (unlock at level 10)
- **Upgrade 2**: 4 weapon slots (unlock at level 20)

### Loadout Combinations

#### Example Build 1: "Front and Back"
- **Slot 1**: Rifle (forward firing, kinetic damage)
- **Slot 2**: Shotgun (backward firing, kinetic damage)
- **Strategy**: Cover both directions, focus on positioning in corridors

#### Example Build 2: "Elemental Master"
- **Slot 1**: Fire Wand (360°, fire DoT)
- **Slot 2**: Ice Staff (forward, cryo slow)
- **Slot 3**: Lightning Gun (targeted, electric chain)
- **Strategy**: Apply multiple status effects, combo damage

#### Example Build 3: "Sniper Support"
- **Slot 1**: Longbow (targeted, high damage, slow fire rate)
- **Slot 2**: Auto-Pistol (360°, low damage, high fire rate)
- **Strategy**: Pistol handles crowds, bow takes out priority targets

#### Example Build 4: "Tank Brawler"
- **Slot 1**: Flamethrower (forward cone, fire DoT)
- **Slot 2**: Shotgun (forward, high close damage)
- **Pet**: Bear (tank, high HP, aggro draw)
- **Strategy**: Face-tank enemies, melt them with close-range damage

## Upgrade System

### Weapon Upgrades
Each weapon can be upgraded at the blacksmith:

**Level 1 → Level 2** (500 gold + materials)
- +20% damage
- +10% fire rate
- +2 magazine size

**Level 2 → Level 3** (1500 gold + rare materials)
- +30% damage
- +15% fire rate
- +5 magazine size
- Unlock special property

**Level 3 → Level 4** (5000 gold + legendary materials)
- +50% damage
- +20% fire rate
- +10 magazine size
- Enhanced special property

### Special Weapon Properties

**Piercing** (Available on bows, rifles)
- Projectiles pass through first enemy, hitting multiple targets
- Each pierce reduces damage by 30%

**Explosive** (Available on grenade launchers, rocket weapons)
- Shots create AoE explosion on impact
- Explosion radius: 3 tiles

**Homing** (Available on magic weapons, advanced tech)
- Projectiles track targets
- Cannot be dodged by fast enemies

**Chain** (Available on electric weapons)
- Damage chains to nearby enemies
- Up to 3 chains, each at 50% of previous damage

**Vampiric** (Available on melee weapons, dark magic)
- Heal 10% of damage dealt
- Works on all damage, including DoT

## Balancing Principles

### Fire Rate vs. Damage
- **Fast weapons**: Lower damage per shot, higher DPS, more resource intensive
- **Slow weapons**: Higher damage per shot, lower DPS, more resource efficient
- **Balance point**: ~20 DPS at level 1, scaling to ~100 DPS at max level

### Range vs. Power
- **Short range (≤5 tiles)**: High damage, wide AoE
- **Medium range (6-10 tiles)**: Balanced damage and fire rate
- **Long range (11+ tiles)**: Lower damage, precise targeting

### Magazine Size vs. Reload Time
- **Small magazine (5-10)**: Fast reload (1-2 seconds)
- **Medium magazine (11-20)**: Medium reload (2-3 seconds)
- **Large magazine (21-30)**: Slow reload (3-5 seconds)
- **Energy weapons (infinite)**: Overheat mechanic instead

### Resource Costs
- **Kinetic weapons**: Low energy cost (2-5 per shot)
- **Elemental weapons**: Medium energy cost (5-10 per shot)
- **Magic weapons**: High mana cost (10-20 per shot)
- **Ultimate weapons**: Very high cost (20-30 energy or mana per shot)

## Visual Feedback

### Weapon State Indicators
- **Ready to Fire**: Weapon icon glows green
- **Reloading**: Progress bar under weapon icon
- **Out of Ammo**: Icon pulsing red
- **Insufficient Resources**: Icon grayed out

### Combat Feedback
- **Projectile Trails**: Visual indication of shots fired
- **Hit Markers**: Visual/audio feedback on successful hits
- **Damage Numbers**: Floating damage numbers above enemies
- **Critical Hits**: Larger damage numbers, special effect
- **Status Effects**: Icons above enemies showing active effects

### Audio Feedback
- **Weapon Fire**: Unique sound per weapon type
- **Reload**: Click/whir sound during reload
- **Hit Sounds**: Different sounds for kinetic/fire/cryo/electric/acid
- **Empty Click**: Warning sound when trying to fire without ammo/resources

## Performance Optimization

### Object Pooling
Projectiles are pooled to avoid garbage collection:
```csharp
public class ProjectilePool
{
    private Queue<Projectile> _available;
    private List<Projectile> _active;
    
    public Projectile Get(Vector2 position, Vector2 direction, float damage)
    {
        Projectile projectile;
        if (_available.Count > 0)
        {
            projectile = _available.Dequeue();
            projectile.Reset(position, direction, damage);
        }
        else
        {
            projectile = new Projectile(position, direction, damage);
        }
        
        _active.Add(projectile);
        return projectile;
    }
    
    public void Return(Projectile projectile)
    {
        _active.Remove(projectile);
        _available.Enqueue(projectile);
    }
}
```

### Update Optimization
Only update weapons and check for targets when in combat zones (caves):
```csharp
public void Update(GameTime gameTime)
{
    // Skip auto-combat in overworld
    if (!_player.IsInCave)
    {
        return;
    }
    
    // Only check for targets periodically
    if (_timeSinceLastTargetScan >= TARGET_SCAN_INTERVAL)
    {
        RefreshTargets();
        _timeSinceLastTargetScan = 0;
    }
    
    _timeSinceLastTargetScan += (float)gameTime.ElapsedGameTime.TotalSeconds;
}
```

## Testing & Tuning

### Test Scenarios
1. **Single Enemy**: Verify weapon fires and hits correctly
2. **Dense Crowd**: Test AoE weapons and status effects
3. **Fast Enemies**: Test targeting and projectile speed
4. **Boss Fight**: Test sustained damage and resource management
5. **Multi-Direction**: Test weapon coverage with enemies on all sides

### Balance Metrics
- **Time to Kill** (TTK): Average 2-5 seconds for basic enemies at same level
- **Resource Efficiency**: Player should be able to sustain combat for 3-5 minutes
- **Weapon Variety**: At least 3 viable weapons per damage type
- **Build Diversity**: No single build more than 20% more effective than others

---

**Document Status**: Technical Specification Complete  
**Last Updated**: January 2026  
**Owner**: MoonBrook Ridge Development Team
