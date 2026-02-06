using System;
using System.Collections.Generic;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Tiles.Animations;

namespace MoonBrookRidge.World.Tiles;

/// <summary>
/// Manages animated tiles using animation data extracted from GameMaker templates.
/// Provides water animations, flame effects, and other dynamic tile effects.
/// </summary>
public class TileAnimationManager
{
    private float _elapsedTime = 0f;
    private readonly Dictionary<string, TileAnimation> _animations;
    
    public TileAnimationManager()
    {
        _animations = new Dictionary<string, TileAnimation>();
        InitializeAnimations();
    }
    
    private void InitializeAnimations()
    {
        // Water animations (various types from GameMaker)
        RegisterAnimation("water_1", "animation_1", 5.0f);
        RegisterAnimation("water_2", "animation_2", 5.0f);
        RegisterAnimation("water_3", "animation_3", 5.0f);
        
        // Flame/torch animations
        RegisterAnimation("flame_1", "animation_4", 8.0f);
        RegisterAnimation("flame_2", "animation_5", 8.0f);
        
        // Flag/banner animations
        RegisterAnimation("flag", "animation_14", 6.0f);
        RegisterAnimation("banner", "animation_17", 6.0f);
        
        // Plant/flower animations (swaying)
        RegisterAnimation("plant_1", "animation_18", 4.0f);
        RegisterAnimation("plant_2", "animation_19", 4.0f);
        RegisterAnimation("plant_3", "animation_20", 4.0f);
        
        // Special effects
        RegisterAnimation("sparkle", "animation_24", 10.0f);
        RegisterAnimation("portal", "animation_25", 8.0f);
    }
    
    private void RegisterAnimation(string name, string animationKey, float frameRate)
    {
        if (SunnysideworldAnimations.Animations.TryGetValue(animationKey, out int[] frames))
        {
            _animations[name] = new TileAnimation
            {
                Name = name,
                Frames = frames,
                FrameRate = frameRate,
                CurrentFrameIndex = 0
            };
        }
    }
    
    /// <summary>
    /// Update all tile animations based on elapsed game time.
    /// </summary>
    public void Update(GameTime gameTime)
    {
        _elapsedTime += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Update each animation
        foreach (var animation in _animations.Values)
        {
            int frameIndex = (int)(_elapsedTime * animation.FrameRate) % animation.Frames.Length;
            animation.CurrentFrameIndex = frameIndex;
        }
    }
    
    /// <summary>
    /// Get the current tile ID for an animated tile.
    /// </summary>
    /// <param name="animationName">Name of the animation (e.g., "water_1", "flame_1")</param>
    /// <returns>The tile ID to render for this frame, or -1 if animation not found</returns>
    public int GetCurrentFrame(string animationName)
    {
        if (_animations.TryGetValue(animationName, out TileAnimation? animation) && animation != null)
        {
            return animation.Frames[animation.CurrentFrameIndex];
        }
        return -1;
    }
    
    /// <summary>
    /// Check if a specific animation is registered.
    /// </summary>
    public bool HasAnimation(string animationName)
    {
        return _animations.ContainsKey(animationName);
    }
    
    /// <summary>
    /// Get all available animation names.
    /// </summary>
    public IEnumerable<string> GetAvailableAnimations()
    {
        return _animations.Keys;
    }
    
    /// <summary>
    /// Reset the animation timer (useful for testing or scene transitions).
    /// </summary>
    public void ResetTimer()
    {
        _elapsedTime = 0f;
        foreach (var animation in _animations.Values)
        {
            animation.CurrentFrameIndex = 0;
        }
    }
    
    /// <summary>
    /// Apply an animation to a specific tile in the map.
    /// This should be called during map initialization for tiles that should be animated.
    /// </summary>
    /// <param name="tile">The tile to animate</param>
    /// <param name="animationName">Name of the animation to apply</param>
    /// <returns>True if animation was applied successfully</returns>
    public bool ApplyAnimationToTile(Tile tile, string animationName)
    {
        if (tile == null || !_animations.ContainsKey(animationName))
            return false;
        
        // Store the animation name in the tile's metadata
        // The tile's SpriteId will be updated during rendering
        tile.AnimationName = animationName;
        return true;
    }
    
    /// <summary>
    /// Update the sprite IDs for all animated tiles in a tile map.
    /// Call this before rendering to ensure animated tiles show the correct frame.
    /// </summary>
    /// <param name="tiles">The tile map to update</param>
    public void UpdateAnimatedTiles(Tile[,] tiles)
    {
        int width = tiles.GetLength(0);
        int height = tiles.GetLength(1);
        
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                Tile tile = tiles[x, y];
                
                if (tile != null && !string.IsNullOrEmpty(tile.AnimationName))
                {
                    int currentFrame = GetCurrentFrame(tile.AnimationName);
                    if (currentFrame >= 0)
                    {
                        tile.SpriteId = currentFrame;
                    }
                }
            }
        }
    }
}

/// <summary>
/// Represents a tile animation with its frames and timing.
/// </summary>
public class TileAnimation
{
    public string Name { get; set; }
    public int[] Frames { get; set; }
    public float FrameRate { get; set; }
    public int CurrentFrameIndex { get; set; }
}
