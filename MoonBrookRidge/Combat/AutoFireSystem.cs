using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;

namespace MoonBrookRidge.Combat;

/// <summary>
/// Auto-shooter combat system that automatically fires equipped weapons at nearby enemies
/// </summary>
public class AutoFireSystem
{
    private float _fireCooldown;
    private const float AUTO_FIRE_RANGE = 200f; // Auto-fire range in pixels
    private const float MANUAL_FIRE_RANGE = 300f; // Manual fire range (for grenades, etc.)
    
    public bool IsAutoFireEnabled { get; set; }
    public FiringPattern CurrentPattern { get; set; }
    
    public event Action<Vector2, float, string> OnAutoFire; // Position, damage, weaponId
    
    public AutoFireSystem()
    {
        IsAutoFireEnabled = true; // Auto-fire enabled by default
        CurrentPattern = FiringPattern.Forward;
        _fireCooldown = 0f;
    }
    
    /// <summary>
    /// Update auto-fire system - automatically fires at nearby enemies
    /// </summary>
    public void Update(GameTime gameTime, Vector2 playerPosition, Weapon equippedWeapon, 
                       List<Enemy> activeEnemies, float playerFacing)
    {
        if (!IsAutoFireEnabled || equippedWeapon == null || activeEnemies.Count == 0)
        {
            return;
        }
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Update fire cooldown
        if (_fireCooldown > 0)
        {
            _fireCooldown -= deltaTime;
            return; // Still on cooldown
        }
        
        // Find target based on firing pattern
        Enemy target = FindTarget(playerPosition, activeEnemies, playerFacing);
        
        if (target != null)
        {
            // Fire weapon
            FireAtTarget(playerPosition, target.Position, equippedWeapon);
            
            // Set cooldown based on weapon attack speed
            _fireCooldown = 1.0f / equippedWeapon.AttackSpeed;
        }
    }
    
    /// <summary>
    /// Find the best target based on firing pattern
    /// </summary>
    private Enemy FindTarget(Vector2 playerPosition, List<Enemy> enemies, float playerFacing)
    {
        switch (CurrentPattern)
        {
            case FiringPattern.Forward:
                return FindTargetInDirection(playerPosition, enemies, playerFacing, 90f);
                
            case FiringPattern.Backward:
                return FindTargetInDirection(playerPosition, enemies, playerFacing + 180f, 90f);
                
            case FiringPattern.Circle360:
                return FindNearestTarget(playerPosition, enemies);
                
            case FiringPattern.Dual:
                // Fire forward and backward simultaneously
                return FindNearestTarget(playerPosition, enemies);
                
            case FiringPattern.Tri:
                // Fire in 3 directions (120° spread)
                return FindNearestTarget(playerPosition, enemies);
                
            default:
                return FindNearestTarget(playerPosition, enemies);
        }
    }
    
    /// <summary>
    /// Find nearest enemy within range
    /// </summary>
    private Enemy FindNearestTarget(Vector2 playerPosition, List<Enemy> enemies)
    {
        Enemy nearest = null;
        float nearestDistance = float.MaxValue;
        
        foreach (var enemy in enemies)
        {
            if (enemy.IsDead)
                continue;
                
            float distance = Vector2.Distance(playerPosition, enemy.Position);
            
            if (distance <= AUTO_FIRE_RANGE && distance < nearestDistance)
            {
                nearest = enemy;
                nearestDistance = distance;
            }
        }
        
        return nearest;
    }
    
    /// <summary>
    /// Find target in a specific direction with cone angle
    /// </summary>
    private Enemy FindTargetInDirection(Vector2 playerPosition, List<Enemy> enemies, 
                                        float direction, float coneAngle)
    {
        Enemy target = null;
        float nearestDistance = float.MaxValue;
        
        foreach (var enemy in enemies)
        {
            if (enemy.IsDead)
                continue;
                
            float distance = Vector2.Distance(playerPosition, enemy.Position);
            
            if (distance > AUTO_FIRE_RANGE)
                continue;
            
            // Calculate angle to enemy
            Vector2 toEnemy = enemy.Position - playerPosition;
            float angleToEnemy = (float)Math.Atan2(toEnemy.Y, toEnemy.X) * (180f / MathF.PI);
            
            // Normalize angles to 0-360
            angleToEnemy = (angleToEnemy + 360f) % 360f;
            direction = (direction + 360f) % 360f;
            
            // Check if enemy is within cone
            float angleDiff = Math.Abs(angleToEnemy - direction);
            if (angleDiff > 180f)
                angleDiff = 360f - angleDiff;
            
            if (angleDiff <= coneAngle / 2f && distance < nearestDistance)
            {
                target = enemy;
                nearestDistance = distance;
            }
        }
        
        return target;
    }
    
    /// <summary>
    /// Fire weapon at target
    /// </summary>
    private void FireAtTarget(Vector2 fromPosition, Vector2 targetPosition, Weapon weapon)
    {
        // Calculate direction to target
        Vector2 direction = targetPosition - fromPosition;
        if (direction.Length() > 0)
        {
            direction.Normalize();
        }
        
        // Trigger fire event (will be handled by projectile system)
        OnAutoFire?.Invoke(fromPosition, weapon.Damage, weapon.Id);
    }
    
    /// <summary>
    /// Manually fire in a direction (for grenades, special abilities)
    /// </summary>
    public void ManualFire(Vector2 position, Vector2 direction, Weapon weapon)
    {
        if (weapon == null)
            return;
            
        OnAutoFire?.Invoke(position, weapon.Damage, weapon.Id);
    }
    
    /// <summary>
    /// Change firing pattern
    /// </summary>
    public void SetFiringPattern(FiringPattern pattern)
    {
        CurrentPattern = pattern;
    }
    
    /// <summary>
    /// Toggle auto-fire on/off
    /// </summary>
    public void ToggleAutoFire()
    {
        IsAutoFireEnabled = !IsAutoFireEnabled;
    }
}

/// <summary>
/// Firing patterns for auto-shooter combat
/// </summary>
public enum FiringPattern
{
    Forward,    // Fire in player facing direction
    Backward,   // Fire behind player
    Circle360,  // Fire at nearest enemy in any direction
    Dual,       // Fire forward and backward simultaneously
    Tri,        // Fire in 3 directions (120° spread)
    Quad,       // Fire in 4 directions (90° spread)
    Omni        // Fire in all 8 directions
}
