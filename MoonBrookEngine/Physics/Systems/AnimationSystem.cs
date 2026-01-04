using MoonBrookEngine.ECS;
using MoonBrookEngine.ECS.Components;

namespace MoonBrookEngine.Physics.Systems;

/// <summary>
/// System that updates animation components
/// </summary>
public class AnimationSystem
{
    private readonly World _world;
    
    public AnimationSystem(World world)
    {
        _world = world;
    }
    
    /// <summary>
    /// Update all animation components
    /// </summary>
    public void Update(float deltaTime)
    {
        var animationEntities = _world.GetEntitiesWith<AnimationComponent>();
        
        foreach (var entity in animationEntities)
        {
            var animationComp = _world.GetComponent<AnimationComponent>(entity);
            
            if (animationComp == null || !animationComp.IsPlaying || animationComp.CurrentAnimation == null)
                continue;
            
            UpdateAnimation(animationComp, deltaTime);
        }
    }
    
    /// <summary>
    /// Update a single animation component
    /// </summary>
    private void UpdateAnimation(AnimationComponent animationComp, float deltaTime)
    {
        var animation = animationComp.CurrentAnimation;
        if (animation == null || animation.Frames.Count == 0)
            return;
        
        // Ensure frame index is valid
        if (animationComp.CurrentFrameIndex < 0 || animationComp.CurrentFrameIndex >= animation.Frames.Count)
            animationComp.CurrentFrameIndex = 0;
        
        var currentFrame = animation.Frames[animationComp.CurrentFrameIndex];
        
        // Advance frame time with speed multiplier
        animationComp.FrameTime += deltaTime * animationComp.Speed;
        
        // Check if we should advance to next frame
        if (animationComp.FrameTime >= currentFrame.Duration)
        {
            animationComp.FrameTime -= currentFrame.Duration;
            
            int previousFrameIndex = animationComp.CurrentFrameIndex;
            animationComp.CurrentFrameIndex++;
            
            // Check if animation completed
            if (animationComp.CurrentFrameIndex >= animation.Frames.Count)
            {
                if (animation.Loop)
                {
                    // Loop back to start
                    animationComp.CurrentFrameIndex = 0;
                }
                else
                {
                    // Stop at last frame
                    animationComp.CurrentFrameIndex = animation.Frames.Count - 1;
                    animationComp.IsPlaying = false;
                    animationComp.FireAnimationComplete();
                    return;
                }
            }
            
            // Fire frame change event if frame actually changed
            if (previousFrameIndex != animationComp.CurrentFrameIndex)
            {
                animationComp.FireFrameChange();
            }
        }
    }
    
    /// <summary>
    /// Create an animation from a sprite sheet with uniform frame sizes
    /// </summary>
    public static Animation CreateFromSpriteSheet(
        string name,
        int sheetWidth,
        int sheetHeight,
        int frameWidth,
        int frameHeight,
        int frameCount,
        float frameDuration = 0.1f,
        bool loop = true,
        int startX = 0,
        int startY = 0)
    {
        var animation = new Animation(name, loop);
        
        int columns = (sheetWidth - startX) / frameWidth;
        
        for (int i = 0; i < frameCount; i++)
        {
            int column = i % columns;
            int row = i / columns;
            
            int x = startX + column * frameWidth;
            int y = startY + row * frameHeight;
            
            animation.AddFrame(new Math.Rectangle(x, y, frameWidth, frameHeight), frameDuration);
        }
        
        return animation;
    }
    
    /// <summary>
    /// Create an animation from a horizontal strip of frames
    /// </summary>
    public static Animation CreateFromHorizontalStrip(
        string name,
        int frameWidth,
        int frameHeight,
        int frameCount,
        float frameDuration = 0.1f,
        bool loop = true,
        int startX = 0,
        int startY = 0)
    {
        var animation = new Animation(name, loop);
        
        for (int i = 0; i < frameCount; i++)
        {
            int x = startX + i * frameWidth;
            int y = startY;
            
            animation.AddFrame(new Math.Rectangle(x, y, frameWidth, frameHeight), frameDuration);
        }
        
        return animation;
    }
    
    /// <summary>
    /// Create an animation from a vertical strip of frames
    /// </summary>
    public static Animation CreateFromVerticalStrip(
        string name,
        int frameWidth,
        int frameHeight,
        int frameCount,
        float frameDuration = 0.1f,
        bool loop = true,
        int startX = 0,
        int startY = 0)
    {
        var animation = new Animation(name, loop);
        
        for (int i = 0; i < frameCount; i++)
        {
            int x = startX;
            int y = startY + i * frameHeight;
            
            animation.AddFrame(new Math.Rectangle(x, y, frameWidth, frameHeight), frameDuration);
        }
        
        return animation;
    }
}
