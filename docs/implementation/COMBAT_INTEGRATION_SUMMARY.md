# Auto-Combat System Integration Summary

## Date
January 3, 2026

## Overview
**MoonBrook Valley** features an auto-shooter roguelite combat system where weapons automatically fire and reload. This document describes the evolution from manual combat to the planned auto-combat system.

## Current State (Manual Combat)
The game currently has a manual combat system integrated into GameplayState:
- **Manual Attacks**: Press Space to attack nearby enemies
- **Enemy AI**: Enemies pursue and attack player automatically
- **Mine Spawning**: Enemies spawn in mines based on floor level
- **Loot System**: Defeated enemies drop items and gold

## Planned Auto-Combat System

### Core Auto-Combat Features
- **Automatic Weapon Fire**: Equipped weapons shoot at enemies automatically within range
- **Automatic Reloading**: Weapons reload themselves when magazine is empty
- **Multiple Weapon Slots**: Equip 2-4 weapons simultaneously (upgradeable)
- **Firing Patterns**: Forward, backward, 360°, and targeted firing modes
- **Damage Types**: Kinetic, fire, cryo, electric, acid with status effects
- **Manual Grenades**: Player manually triggers AoE grenade abilities (Space key)

## Implementation Plan for Auto-Combat

### Phase 1: AutoWeapon System
Create new classes for auto-combat:

```csharp
// Core auto-weapon class
public class AutoWeapon
{
    public string Id { get; set; }
    public float Damage { get; set; }
    public float FireRate { get; set; }  // Shots per second
    public float Range { get; set; }
    public int MagazineSize { get; set; }
    public float ReloadTime { get; set; }
    public FiringPattern Pattern { get; set; }
    public DamageType DamageType { get; set; }
    
    public bool CanFire() { /* Check ammo, reload, cooldown */ }
    public void Fire(Enemy target) { /* Create projectile */ }
    public void Update(float deltaTime) { /* Handle reload */ }
}

// Auto-combat manager
public class AutoCombatManager
{
    private List<AutoWeapon> _equippedWeapons;
    
    public void Update(GameTime gameTime, Vector2 playerPos, List<Enemy> enemies)
    {
        foreach (var weapon in _equippedWeapons)
        {
            if (weapon.CanFire())
            {
                var target = FindTarget(weapon, playerPos, enemies);
                if (target != null)
                {
                    weapon.Fire(target);
                }
            }
            weapon.Update(deltaTime);
        }
    }
}
```

### Phase 2: Pet Combat Integration
Enhance PetSystem for automated combat support:

```csharp
public class PetCompanion
{
    public PetType Type { get; set; }
    public float AttackDamage { get; set; }
    public float AttackRate { get; set; }
    public float AttackRange { get; set; }
    public List<PetAbility> Abilities { get; set; }
    
    public void UpdateCombat(float deltaTime, Vector2 playerPos, List<Enemy> enemies)
    {
        // Follow player
        UpdateFollowBehavior(playerPos);
        
        // Auto-attack nearby enemies
        if (CanAttack())
        {
            var target = FindNearestEnemy(enemies);
            if (target != null && IsInRange(target))
            {
                Attack(target);
            }
        }
        
        // Apply passive buffs
        ApplyPassiveEffects();
    }
}
```

### Phase 3: Damage Type System
Implement elemental damage and status effects:

```csharp
public enum DamageType
{
    Kinetic,    // Pure physical
    Fire,       // DoT burning
    Cryo,       // Slow/freeze
    Electric,   // Crit buff, chain damage
    Acid        // Armor reduction, damage amp
}

public void ApplyDamage(Enemy enemy, float amount, DamageType type)
{
    enemy.TakeDamage(amount);
    
    switch (type)
    {
        case DamageType.Fire:
            enemy.ApplyBurning(amount * 0.2f, 3f); // 20% DoT over 3s
            break;
        case DamageType.Cryo:
            enemy.ApplySlow(0.5f, 2f); // 50% slow for 2s
            break;
        case DamageType.Electric:
            ChainToNearbyEnemies(enemy, amount * 0.5f);
            break;
        case DamageType.Acid:
            enemy.ApplyArmorReduction(0.25f, 5f); // 25% reduction for 5s
            break;
    }
}
```

### Phase 4: Grenade System
Add manual AoE abilities:

```csharp
public class GrenadeSystem
{
    private Dictionary<GrenadeType, float> _cooldowns;
    
    public enum GrenadeType
    {
        Explosive,   // High AoE damage (30s cooldown)
        Cryo,        // Freeze area (45s cooldown)
        HGrenade,    // Mining/destruction (60s cooldown)
        Neurotoxin,  // DoT cloud (40s cooldown)
        Electric     // Chain stun (35s cooldown)
    }
    
    public bool ThrowGrenade(GrenadeType type, Vector2 position, List<Enemy> enemies)
    {
        if (_cooldowns[type] > 0) return false;
        
        // Apply grenade effect
        ApplyGrenadeEffect(type, position, enemies);
        
        // Start cooldown
        _cooldowns[type] = GetGrenadeCooldown(type);
        return true;
    }
}
```

### Phase 5: Cave-Only Combat Zones
Restrict combat to cave systems:

```csharp
public void Update(GameTime gameTime)
{
    // Only run auto-combat in caves
    if (_currentLocation == LocationType.Cave)
    {
        _autoCombatManager.Update(gameTime, _player.Position, _activeEnemies);
        _petCompanion.UpdateCombat(gameTime, _player.Position, _activeEnemies);
    }
    else if (_currentLocation == LocationType.Overworld)
    {
        // Peaceful overworld - farming, fishing, village activities
        // No enemy spawns, no combat
    }
}
```
- **CombatSystem Integration**: Added CombatSystem instance to GameplayState
- **Enemy Spawning**: Automatic enemy generation when entering/descending mine levels
- **Player Combat**: Space key triggers attacks on nearby enemies (64 pixel range)
- **Enemy AI**: Enemies pursue player and attack when in melee range (32 pixels)
- **Damage System**: Both player and enemies can deal and receive damage
- **Loot System**: Defeated enemies drop items and gold automatically

### Enemy Spawning System
Implemented `SpawnEnemiesInMine(int level)` method with scaling difficulty:

**Enemy Count**: 3 + level + random(3) enemies per level

**Level 1-2 (Easy)**:
- Green Slime (20 HP, 5 dmg)
- Cave Bat (15 HP, 8 dmg)
- Goblin (35 HP, 10 dmg)

**Level 3-5 (Medium)**:
- Skeleton (40 HP, 12 dmg)
- Giant Spider (30 HP, 14 dmg)
- Wild Wolf (45 HP, 16 dmg)

**Level 6-8 (Hard)**:
- Phantom (50 HP, 20 dmg)
- Zombie (60 HP, 18 dmg)
- Orc Warrior (80 HP, 25 dmg)

**Level 9+ (Very Hard)**:
- Fire Elemental (100 HP, 35 dmg)
- Lesser Demon (120 HP, 40 dmg)

### Visual Rendering
- **Enemy Display**: Enemies rendered as colored circles (color-coded by type)
- **Health Bars**: Dynamic health bars above each enemy
- **Boss Indicators**: Boss enemies are larger (24px vs 16px) with names displayed
- **Health Colors**: Green (>50%) → Yellow (25-50%) → Red (<25%)

### Combat Mechanics
**Player Attacks**:
- Press Space to attack nearest enemy in range
- Weapon energy/mana cost enforced
- Attack range: 64 pixels (4 tiles)

**Enemy Behavior**:
- Pursue player within detection range
- Attack every 2 seconds when in melee range (32 pixels)
- Move at individual speeds (varies by enemy type)

**Loot Drops**:
- Each enemy has a loot table with drop chances
- Items automatically added to player inventory
- Base 50 gold awarded per enemy defeat
- Combat XP awarded to Combat skill category

### Level Management
- Enemies cleared when exiting mines
- New enemies spawned when descending/ascending levels
- Each level gets fresh enemy set

## Files Modified

### Core/States/GameplayState.cs
- Added `using MoonBrookRidge.Combat;` and `using System;`
- Added `private CombatSystem _combatSystem;` field
- Initialized CombatSystem in Initialize()
- Hooked up combat events (OnEnemyDefeated, OnPlayerDamaged)
- Added `HandleCombat(GameTime)` method for combat logic
- Added `SpawnEnemiesInMine(int level)` for enemy generation
- Added `DrawEnemies(SpriteBatch)` for enemy rendering
- Added `DrawFilledCircle(...)` helper for circle rendering
- Modified HandleMineInteraction to spawn/clear enemies

### CONTROLS.md
- Added "Attack (Combat)" entry with Space key
- Added complete "Combat System" section with:
  - Basic combat controls
  - Weapon types and stats
  - Enemy types and mechanics
  - Combat strategy tips
  - Mine combat specifics

### README.md
- Added "Attack (Combat)" to controls table
- Updated Phase 6 status to mark combat as integrated

## Technical Details

### Enemy AI Algorithm
```csharp
foreach (var enemy in enemies)
{
    Vector2 direction = player.Position - enemy.Position;
    float distance = direction.Length();
    
    if (distance > 32f) // Not in attack range
    {
        // Move towards player
        enemy.Position += direction.Normalize() * enemy.Speed * deltaTime;
    }
    else if (enemy.CanAttack()) // In attack range and ready
    {
        enemy.Attack();
        _combatSystem.PlayerTakeDamage(enemy.Damage);
    }
}
```

### Player Attack Logic
```csharp
// Find nearest enemy in 64px range
// Check weapon energy/mana cost
// Consume energy/mana if available
// Call AttackEnemy() to deal damage
```

## Known Limitations

1. **Visual Representation**: Enemies are currently simple colored circles
   - Future: Add proper enemy sprites
   
2. **Combat Animations**: Player doesn't animate when attacking
   - Future: Trigger attack animation state

3. **Visual Effects**: No particles or impact effects
   - Future: Add combat particle effects

4. **Skill Integration**: Skill bonuses not yet applied to combat
   - Future: Add damage/defense modifiers from skill tree

5. **Weapon Switching**: No UI for equipping different weapons
   - Future: Add weapon selection menu

6. **Sound Effects**: Combat is silent
   - Future: Add attack/hit/death sounds

## Testing Recommendations

### Manual Testing Checklist
- [ ] Enter mine and verify enemies spawn
- [ ] Test player attack with Space key
- [ ] Verify enemies pursue player
- [ ] Verify enemies deal damage to player
- [ ] Test enemy health bars update correctly
- [ ] Verify loot drops appear in inventory
- [ ] Test difficulty scaling across levels
- [ ] Verify enemies clear when exiting mine
- [ ] Test with different weapons (energy vs mana)

### Balance Testing
- [ ] Verify enemy spawn counts feel appropriate
- [ ] Test enemy damage isn't too high/low
- [ ] Verify player damage is balanced
- [ ] Check loot drop rates
- [ ] Ensure combat feels rewarding

## Next Steps

### High Priority
1. Add combat particle effects (hits, damage numbers)
2. Trigger player attack animation
3. Add weapon switching UI
4. Playtest and balance enemy stats

### Medium Priority  
5. Add combat sound effects
6. Integrate skill bonuses into damage calculations
7. Add proper enemy sprites
8. Add boss encounter special effects

### Low Priority
9. Add weapon durability system
10. Create specialized combat achievements
11. Add combo/timing mechanics

## Integration with New Requirements

The combat system is ready to support the new Stardew Valley + Zelda hybrid vision:

**Already Compatible**:
- ✅ Real-time action RPG combat
- ✅ Multiple weapon types (melee, ranged, magic)
- ✅ Enemy variety and boss battles
- ✅ Loot system for resource gathering

**Future Enhancements Needed**:
- Tool-based puzzle combat (hookshot, elemental tools)
- Elemental interactions (fire/ice/lightning)
- Monster taming/capturing
- Mini-boss encounters in dungeons
- Combat-integrated resource gathering
- Cooking buffs for combat

## Transition to Auto-Combat System

The current manual combat system (Space key to attack) will evolve into an auto-shooter roguelite system:

### Key Changes Required

#### 1. Replace Manual Attacks with Auto-Fire
- **Current**: Player presses Space to attack nearest enemy
- **Future**: Weapons auto-fire at enemies within range and firing pattern
- **Change**: Space key repurposed for throwing grenades

#### 2. Implement Weapon Loadout System
- **Current**: Single weapon at a time
- **Future**: 2-4 equipped weapons firing simultaneously
- **Addition**: Each weapon has firing pattern (forward, backward, 360°, targeted)

#### 3. Add Damage Type System
- **Current**: Simple damage values
- **Future**: Kinetic, fire, cryo, electric, acid with status effects
- **Addition**: Elemental combos and synergies

#### 4. Enhance Pet System for Combat
- **Current**: Pets are passive companions
- **Future**: Pets auto-attack enemies and provide buffs
- **Addition**: Pet leveling, skill trees, and charms

#### 5. Restrict Combat to Caves
- **Current**: Combat in mines
- **Future**: Combat only in procedurally generated caves
- **Change**: Overworld becomes completely peaceful

### Migration Path

**Step 1**: Create AutoWeaponSystem alongside existing CombatSystem
**Step 2**: Add weapon loadout UI and management
**Step 3**: Implement firing patterns and targeting AI
**Step 4**: Add damage types and status effects
**Step 5**: Enhance PetSystem with combat abilities
**Step 6**: Create GrenadeSystem for manual AoE
**Step 7**: Update cave generation for combat zones
**Step 8**: Disable enemy spawns in overworld
**Step 9**: Replace Space key attack with grenade throw
**Step 10**: Remove or adapt manual attack code

## Conclusion

The current manual combat system provides a solid foundation. Players can fight enemies in mines using the Space key, with enemies that intelligently pursue and attack. This will evolve into an auto-shooter system where weapons fire automatically and the player focuses on positioning and build strategy.

**Current Status**: ✅ Manual Combat Integrated
**Future Status**: 🔄 Transitioning to Auto-Combat System
**Documentation**: See MOONBROOK_VALLEY_DESIGN.md and AUTO_COMBAT_MECHANICS.md
**Build Status**: ✅ 0 Errors, 8 Warnings (pre-existing)
**Ready for**: Auto-combat implementation
