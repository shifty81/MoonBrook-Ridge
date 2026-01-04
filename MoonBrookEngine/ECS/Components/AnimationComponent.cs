using MoonBrookEngine.Math;

namespace MoonBrookEngine.ECS.Components;

/// <summary>
/// Represents a single frame in an animation
/// </summary>
public class AnimationFrame
{
    /// <summary>
    /// Source rectangle on the sprite sheet
    /// </summary>
    public Rectangle SourceRect { get; set; }
    
    /// <summary>
    /// Duration of this frame in seconds
    /// </summary>
    public float Duration { get; set; }
    
    public AnimationFrame(Rectangle sourceRect, float duration = 0.1f)
    {
        SourceRect = sourceRect;
        Duration = duration;
    }
}

/// <summary>
/// Represents a named animation sequence
/// </summary>
public class Animation
{
    /// <summary>
    /// Name of the animation
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// Frames in this animation
    /// </summary>
    public List<AnimationFrame> Frames { get; private set; }
    
    /// <summary>
    /// Whether this animation should loop
    /// </summary>
    public bool Loop { get; set; }
    
    /// <summary>
    /// Total duration of the animation
    /// </summary>
    public float TotalDuration => Frames.Sum(f => f.Duration);
    
    public Animation(string name, bool loop = true)
    {
        Name = name;
        Loop = loop;
        Frames = new List<AnimationFrame>();
    }
    
    /// <summary>
    /// Add a frame to the animation
    /// </summary>
    public void AddFrame(Rectangle sourceRect, float duration = 0.1f)
    {
        Frames.Add(new AnimationFrame(sourceRect, duration));
    }
}

/// <summary>
/// Component for entity animation using sprite sheets
/// </summary>
public class AnimationComponent : Component
{
    /// <summary>
    /// Dictionary of available animations by name
    /// </summary>
    public Dictionary<string, Animation> Animations { get; private set; }
    
    /// <summary>
    /// Currently playing animation
    /// </summary>
    public Animation? CurrentAnimation { get; private set; }
    
    /// <summary>
    /// Current frame index
    /// </summary>
    public int CurrentFrameIndex { get; set; }
    
    /// <summary>
    /// Time elapsed in current frame
    /// </summary>
    public float FrameTime { get; set; }
    
    /// <summary>
    /// Whether the animation is currently playing
    /// </summary>
    public bool IsPlaying { get; set; }
    
    /// <summary>
    /// Playback speed multiplier (1.0 = normal speed)
    /// </summary>
    public float Speed { get; set; }
    
    /// <summary>
    /// Event fired when animation completes (non-looping animations)
    /// </summary>
    public event Action<string>? OnAnimationComplete;
    
    /// <summary>
    /// Event fired when frame changes
    /// </summary>
    public event Action<string, int>? OnFrameChange;
    
    public AnimationComponent()
    {
        Animations = new Dictionary<string, Animation>();
        CurrentFrameIndex = 0;
        FrameTime = 0f;
        IsPlaying = false;
        Speed = 1.0f;
    }
    
    /// <summary>
    /// Add an animation to the component
    /// </summary>
    public void AddAnimation(Animation animation)
    {
        Animations[animation.Name] = animation;
    }
    
    /// <summary>
    /// Play an animation by name
    /// </summary>
    public void Play(string animationName, bool restart = false)
    {
        if (!Animations.TryGetValue(animationName, out var animation))
            return;
        
        // If already playing this animation and not restarting, do nothing
        if (CurrentAnimation == animation && IsPlaying && !restart)
            return;
        
        CurrentAnimation = animation;
        IsPlaying = true;
        
        if (restart || CurrentAnimation != animation)
        {
            CurrentFrameIndex = 0;
            FrameTime = 0f;
        }
    }
    
    /// <summary>
    /// Stop the current animation
    /// </summary>
    public void Stop()
    {
        IsPlaying = false;
    }
    
    /// <summary>
    /// Pause the current animation
    /// </summary>
    public void Pause()
    {
        IsPlaying = false;
    }
    
    /// <summary>
    /// Resume the current animation
    /// </summary>
    public void Resume()
    {
        IsPlaying = true;
    }
    
    /// <summary>
    /// Get the current frame's source rectangle
    /// </summary>
    public Rectangle? GetCurrentFrameRect()
    {
        if (CurrentAnimation == null || CurrentAnimation.Frames.Count == 0)
            return null;
        
        if (CurrentFrameIndex < 0 || CurrentFrameIndex >= CurrentAnimation.Frames.Count)
            CurrentFrameIndex = 0;
        
        return CurrentAnimation.Frames[CurrentFrameIndex].SourceRect;
    }
    
    /// <summary>
    /// Fire the OnAnimationComplete event
    /// </summary>
    internal void FireAnimationComplete()
    {
        if (CurrentAnimation != null)
            OnAnimationComplete?.Invoke(CurrentAnimation.Name);
    }
    
    /// <summary>
    /// Fire the OnFrameChange event
    /// </summary>
    internal void FireFrameChange()
    {
        if (CurrentAnimation != null)
            OnFrameChange?.Invoke(CurrentAnimation.Name, CurrentFrameIndex);
    }
}
