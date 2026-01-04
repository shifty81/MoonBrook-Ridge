using System.Numerics;
using SysVec2 = System.Numerics.Vector2;
using Col = MoonBrookEngine.Math.Color;

namespace MoonBrookEngine.ECS.Components;

/// <summary>
/// Represents a single particle with its properties
/// </summary>
public class Particle
{
    public SysVec2 Position { get; set; }
    public SysVec2 Velocity { get; set; }
    public SysVec2 Acceleration { get; set; }
    public Col Color { get; set; }
    public Col StartColor { get; set; }
    public Col EndColor { get; set; }
    public float Size { get; set; }
    public float StartSize { get; set; }
    public float EndSize { get; set; }
    public float Lifetime { get; set; }
    public float Age { get; set; }
    public float Rotation { get; set; }
    public float RotationSpeed { get; set; }
    public bool IsActive { get; set; }
    
    public Particle()
    {
        IsActive = false;
        Color = Col.White;
        StartColor = Col.White;
        EndColor = Col.White;
        Size = 1f;
        StartSize = 1f;
        EndSize = 1f;
    }
    
    /// <summary>
    /// Reset particle to initial state for reuse (pooling)
    /// </summary>
    public void Reset()
    {
        IsActive = false;
        Position = SysVec2.Zero;
        Velocity = SysVec2.Zero;
        Acceleration = SysVec2.Zero;
        Age = 0f;
        Rotation = 0f;
    }
    
    /// <summary>
    /// Get normalized lifetime (0 to 1)
    /// </summary>
    public float NormalizedAge => Lifetime > 0 ? Age / Lifetime : 1f;
}

/// <summary>
/// Component for particle emitter that spawns and manages particles
/// </summary>
public class ParticleComponent : Component
{
    /// <summary>
    /// Maximum number of particles this emitter can have active
    /// </summary>
    public int MaxParticles { get; set; }
    
    /// <summary>
    /// Particles per second to spawn
    /// </summary>
    public float EmissionRate { get; set; }
    
    /// <summary>
    /// Particle lifetime in seconds
    /// </summary>
    public float ParticleLifetime { get; set; }
    
    /// <summary>
    /// Variance in particle lifetime (randomness)
    /// </summary>
    public float LifetimeVariance { get; set; }
    
    /// <summary>
    /// Start velocity of particles
    /// </summary>
    public SysVec2 StartVelocity { get; set; }
    
    /// <summary>
    /// Variance in start velocity (randomness)
    /// </summary>
    public SysVec2 VelocityVariance { get; set; }
    
    /// <summary>
    /// Start color of particles
    /// </summary>
    public Col StartColor { get; set; }
    
    /// <summary>
    /// End color of particles (interpolated over lifetime)
    /// </summary>
    public Col EndColor { get; set; }
    
    /// <summary>
    /// Start size of particles
    /// </summary>
    public float StartSize { get; set; }
    
    /// <summary>
    /// End size of particles (interpolated over lifetime)
    /// </summary>
    public float EndSize { get; set; }
    
    /// <summary>
    /// Size variance (randomness)
    /// </summary>
    public float SizeVariance { get; set; }
    
    /// <summary>
    /// Spawn area radius (particles spawn within this radius)
    /// </summary>
    public float SpawnRadius { get; set; }
    
    /// <summary>
    /// Gravity affecting particles
    /// </summary>
    public SysVec2 Gravity { get; set; }
    
    /// <summary>
    /// Wind/force affecting particles
    /// </summary>
    public SysVec2 Wind { get; set; }
    
    /// <summary>
    /// Rotation speed in radians per second
    /// </summary>
    public float RotationSpeed { get; set; }
    
    /// <summary>
    /// Variance in rotation speed
    /// </summary>
    public float RotationVariance { get; set; }
    
    /// <summary>
    /// Whether the emitter is currently active
    /// </summary>
    public bool IsEmitting { get; set; }
    
    /// <summary>
    /// Whether to loop emission or emit once
    /// </summary>
    public bool Loop { get; set; }
    
    /// <summary>
    /// Pool of particles for reuse
    /// </summary>
    public List<Particle> Particles { get; private set; }
    
    /// <summary>
    /// Time accumulator for emission
    /// </summary>
    public float EmissionAccumulator { get; set; }
    
    /// <summary>
    /// Cached count of active particles (updated by ParticleSystem)
    /// </summary>
    internal int _activeParticleCount;
    
    public ParticleComponent()
    {
        MaxParticles = 100;
        EmissionRate = 10f;
        ParticleLifetime = 2f;
        LifetimeVariance = 0.5f;
        StartVelocity = new SysVec2(0, -50f);
        VelocityVariance = new SysVec2(50f, 50f);
        StartColor = Col.White;
        EndColor = new Col(255, 255, 255, 0);
        StartSize = 4f;
        EndSize = 1f;
        SizeVariance = 1f;
        SpawnRadius = 5f;
        Gravity = SysVec2.Zero;
        Wind = SysVec2.Zero;
        RotationSpeed = 0f;
        RotationVariance = 0f;
        IsEmitting = true;
        Loop = true;
        Particles = new List<Particle>();
        EmissionAccumulator = 0f;
        
        // Pre-allocate particle pool
        for (int i = 0; i < MaxParticles; i++)
        {
            Particles.Add(new Particle());
        }
    }
    
    /// <summary>
    /// Get number of active particles (cached for performance)
    /// </summary>
    public int ActiveParticleCount => _activeParticleCount;
}
