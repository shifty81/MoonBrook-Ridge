using System.Numerics;
using MoonBrookEngine.ECS;
using MoonBrookEngine.ECS.Components;

namespace MoonBrookEngine.Physics.Systems;

/// <summary>
/// System that updates and manages particle emitters
/// </summary>
public class ParticleSystem
{
    private readonly World _world;
    private readonly Random _random;
    
    public ParticleSystem(World world)
    {
        _world = world;
        _random = new Random();
    }
    
    /// <summary>
    /// Update all particle emitters
    /// </summary>
    public void Update(float deltaTime)
    {
        var particleEntities = _world.GetEntitiesWith<ParticleComponent, TransformComponent>();
        
        foreach (var entity in particleEntities)
        {
            var particleComp = _world.GetComponent<ParticleComponent>(entity);
            var transform = _world.GetComponent<TransformComponent>(entity);
            
            if (particleComp == null || transform == null)
                continue;
            
            // Emit new particles if emitter is active
            if (particleComp.IsEmitting)
            {
                EmitParticles(particleComp, transform, deltaTime);
            }
            
            // Update existing particles
            UpdateParticles(particleComp, deltaTime);
        }
    }
    
    /// <summary>
    /// Emit new particles based on emission rate
    /// </summary>
    private void EmitParticles(ParticleComponent emitter, TransformComponent transform, float deltaTime)
    {
        // Calculate how many particles to spawn this frame
        emitter.EmissionAccumulator += emitter.EmissionRate * deltaTime;
        int particlesToSpawn = (int)emitter.EmissionAccumulator;
        emitter.EmissionAccumulator -= particlesToSpawn;
        
        // Spawn particles
        for (int i = 0; i < particlesToSpawn; i++)
        {
            // Find an inactive particle to reuse
            var particle = emitter.Particles.FirstOrDefault(p => !p.IsActive);
            if (particle == null)
                break; // No available particles
            
            // Initialize particle
            particle.IsActive = true;
            particle.Age = 0f;
            
            // Random position within spawn radius
            var angle = (float)(_random.NextDouble() * MathF.PI * 2);
            var distance = (float)(_random.NextDouble() * emitter.SpawnRadius);
            var offset = new Vector2(
                MathF.Cos(angle) * distance,
                MathF.Sin(angle) * distance
            );
            particle.Position = transform.Position + offset;
            
            // Random velocity
            particle.Velocity = emitter.StartVelocity + new Vector2(
                ((float)_random.NextDouble() - 0.5f) * emitter.VelocityVariance.X * 2,
                ((float)_random.NextDouble() - 0.5f) * emitter.VelocityVariance.Y * 2
            );
            
            particle.Acceleration = Vector2.Zero;
            
            // Lifetime with variance
            particle.Lifetime = emitter.ParticleLifetime +
                ((float)_random.NextDouble() - 0.5f) * emitter.LifetimeVariance * 2;
            particle.Lifetime = MathF.Max(0.1f, particle.Lifetime); // Minimum lifetime
            
            // Colors
            particle.StartColor = emitter.StartColor;
            particle.EndColor = emitter.EndColor;
            particle.Color = emitter.StartColor;
            
            // Size with variance
            var sizeVariation = ((float)_random.NextDouble() - 0.5f) * emitter.SizeVariance * 2;
            particle.StartSize = MathF.Max(0.1f, emitter.StartSize + sizeVariation);
            particle.EndSize = MathF.Max(0.1f, emitter.EndSize + sizeVariation);
            particle.Size = particle.StartSize;
            
            // Rotation
            particle.Rotation = 0f;
            particle.RotationSpeed = emitter.RotationSpeed +
                ((float)_random.NextDouble() - 0.5f) * emitter.RotationVariance * 2;
        }
    }
    
    /// <summary>
    /// Update all active particles in an emitter
    /// </summary>
    private void UpdateParticles(ParticleComponent emitter, float deltaTime)
    {
        int activeCount = 0;
        
        foreach (var particle in emitter.Particles)
        {
            if (!particle.IsActive)
                continue;
            
            activeCount++;
            
            // Age particle
            particle.Age += deltaTime;
            
            // Kill particle if lifetime exceeded
            if (particle.Age >= particle.Lifetime)
            {
                particle.Reset();
                activeCount--; // Particle just deactivated
                continue;
            }
            
            // Apply forces (gravity, wind)
            particle.Acceleration = emitter.Gravity + emitter.Wind;
            
            // Update velocity and position
            particle.Velocity += particle.Acceleration * deltaTime;
            particle.Position += particle.Velocity * deltaTime;
            
            // Update rotation
            particle.Rotation += particle.RotationSpeed * deltaTime;
            
            // Interpolate color over lifetime
            float t = particle.NormalizedAge;
            particle.Color = LerpColor(particle.StartColor, particle.EndColor, t);
            
            // Interpolate size over lifetime
            particle.Size = particle.StartSize + (particle.EndSize - particle.StartSize) * t;
        }
        
        // Update cached active particle count
        emitter._activeParticleCount = activeCount;
    }
    
    /// <summary>
    /// Linear interpolate between two colors
    /// </summary>
    private Math.Color LerpColor(Math.Color start, Math.Color end, float t)
    {
        t = System.Math.Clamp(t, 0f, 1f);
        return new Math.Color(
            (byte)(start.R + (end.R - start.R) * t),
            (byte)(start.G + (end.G - start.G) * t),
            (byte)(start.B + (end.B - start.B) * t),
            (byte)(start.A + (end.A - start.A) * t)
        );
    }
}
