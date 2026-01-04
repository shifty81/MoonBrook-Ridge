using System.Numerics;
using MoonBrookEngine.ECS;
using MoonBrookEngine.ECS.Components;

namespace MoonBrookEngine.Physics.Systems;

/// <summary>
/// System that handles physics simulation for entities
/// </summary>
public class PhysicsSystem
{
    private readonly World _world;
    
    /// <summary>
    /// Global gravity vector (units per second squared)
    /// </summary>
    public Vector2 Gravity { get; set; }
    
    /// <summary>
    /// Whether to enable collision resolution
    /// </summary>
    public bool EnableCollisionResolution { get; set; }
    
    public PhysicsSystem(World world)
    {
        _world = world;
        Gravity = new Vector2(0, 980f); // Default: 980 pixels/sec² downward (like 9.8 m/s²)
        EnableCollisionResolution = true;
    }
    
    /// <summary>
    /// Update physics for all entities
    /// </summary>
    public void Update(float deltaTime)
    {
        // Get all entities with physics components
        var physicsEntities = _world.GetEntitiesWith<PhysicsComponent, TransformComponent>();
        
        foreach (var entity in physicsEntities)
        {
            var physics = _world.GetComponent<PhysicsComponent>(entity);
            var transform = _world.GetComponent<TransformComponent>(entity);
            var velocity = _world.GetComponent<VelocityComponent>(entity);
            
            if (physics == null || transform == null || physics.IsStatic)
                continue;
            
            // If entity has velocity component, update it
            if (velocity != null)
            {
                // Apply gravity
                if (physics.GravityScale > 0)
                {
                    var gravityForce = Gravity * physics.Mass * physics.GravityScale;
                    physics.ApplyForce(gravityForce);
                }
                
                // Apply accumulated forces: F = ma, so a = F/m
                if (physics.AccumulatedForce != Vector2.Zero)
                {
                    var acceleration = physics.AccumulatedForce / physics.Mass;
                    velocity.Acceleration = acceleration;
                }
                
                // Apply acceleration to velocity
                velocity.Velocity += velocity.Acceleration * deltaTime;
                
                // Apply drag
                if (physics.Drag > 0)
                {
                    var dragFactor = MathF.Pow(1.0f - physics.Drag, deltaTime);
                    velocity.Velocity *= dragFactor;
                }
                
                // Clamp to max speed if set
                if (velocity.MaxSpeed > 0)
                {
                    var speed = velocity.Velocity.Length();
                    if (speed > velocity.MaxSpeed)
                    {
                        velocity.Velocity = Vector2.Normalize(velocity.Velocity) * velocity.MaxSpeed;
                    }
                }
                
                // Update position based on velocity
                transform.Position += velocity.Velocity * deltaTime;
                
                // Clear forces for next frame
                physics.ClearForces();
            }
        }
        
        // Handle collisions if enabled
        if (EnableCollisionResolution)
        {
            ResolveCollisions();
        }
    }
    
    /// <summary>
    /// Resolve collisions between entities
    /// </summary>
    private void ResolveCollisions()
    {
        var colliderEntities = _world.GetEntitiesWith<ColliderComponent, TransformComponent>().ToList();
        
        for (int i = 0; i < colliderEntities.Count; i++)
        {
            for (int j = i + 1; j < colliderEntities.Count; j++)
            {
                var entity1 = colliderEntities[i];
                var entity2 = colliderEntities[j];
                
                var collider1 = _world.GetComponent<ColliderComponent>(entity1);
                var collider2 = _world.GetComponent<ColliderComponent>(entity2);
                var transform1 = _world.GetComponent<TransformComponent>(entity1);
                var transform2 = _world.GetComponent<TransformComponent>(entity2);
                
                if (collider1 == null || collider2 == null || 
                    transform1 == null || transform2 == null ||
                    collider1.Shape == null || collider2.Shape == null)
                    continue;
                
                // Check for collision
                if (collider1.Shape.Intersects(collider2.Shape, transform1.Position, transform2.Position))
                {
                    // Skip if either is a trigger
                    if (collider1.IsTrigger || collider2.IsTrigger)
                        continue;
                    
                    var physics1 = _world.GetComponent<PhysicsComponent>(entity1);
                    var physics2 = _world.GetComponent<PhysicsComponent>(entity2);
                    var velocity1 = _world.GetComponent<VelocityComponent>(entity1);
                    var velocity2 = _world.GetComponent<VelocityComponent>(entity2);
                    
                    // Simple collision response: bounce back
                    if (velocity1 != null && physics1 != null && !physics1.IsStatic)
                    {
                        var normal = Vector2.Normalize(transform1.Position - transform2.Position);
                        var relativeVelocity = velocity1.Velocity;
                        if (velocity2 != null)
                            relativeVelocity -= velocity2.Velocity;
                        
                        var velocityAlongNormal = Vector2.Dot(relativeVelocity, normal);
                        
                        // Only resolve if objects are moving toward each other
                        if (velocityAlongNormal < 0)
                        {
                            var restitution = physics1.Restitution;
                            if (physics2 != null)
                                restitution = MathF.Min(physics1.Restitution, physics2.Restitution);
                            
                            var impulseMagnitude = -(1 + restitution) * velocityAlongNormal;
                            var impulse = impulseMagnitude * normal;
                            
                            // Apply impulse
                            if (!physics1.IsStatic)
                                velocity1.Velocity += impulse;
                            
                            if (velocity2 != null && physics2 != null && !physics2.IsStatic)
                                velocity2.Velocity -= impulse;
                            
                            // Separate objects to prevent overlap
                            var separation = (collider1.Shape.GetBounds(transform1.Position).Width / 2 + 
                                            collider2.Shape.GetBounds(transform2.Position).Width / 2) * 0.01f;
                            if (!physics1.IsStatic)
                                transform1.Position += normal * separation;
                            if (physics2 != null && !physics2.IsStatic)
                                transform2.Position -= normal * separation;
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Apply a force to an entity
    /// </summary>
    public void ApplyForce(Entity entity, Vector2 force)
    {
        var physics = _world.GetComponent<PhysicsComponent>(entity);
        physics?.ApplyForce(force);
    }
    
    /// <summary>
    /// Apply an impulse to an entity
    /// </summary>
    public void ApplyImpulse(Entity entity, Vector2 impulse)
    {
        var physics = _world.GetComponent<PhysicsComponent>(entity);
        var velocity = _world.GetComponent<VelocityComponent>(entity);
        physics?.ApplyImpulse(impulse, velocity);
    }
}
