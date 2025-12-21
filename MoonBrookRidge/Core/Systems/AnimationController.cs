using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Sprite animation system with state machine and directional support
/// </summary>
public class AnimationController
{
    private Dictionary<string, Animation> _animations;
    private Animation _currentAnimation;
    private string _currentAnimationName;
    private Direction _currentDirection;
    
    public AnimationController()
    {
        _animations = new Dictionary<string, Animation>();
        _currentDirection = Direction.Down;
    }
    
    /// <summary>
    /// Add an animation to the controller
    /// </summary>
    public void AddAnimation(string name, Animation animation)
    {
        _animations[name] = animation;
    }
    
    /// <summary>
    /// Play an animation
    /// </summary>
    public void Play(string animationName, bool restart = false)
    {
        if (_animations.ContainsKey(animationName))
        {
            if (_currentAnimationName != animationName || restart)
            {
                _currentAnimation = _animations[animationName];
                _currentAnimationName = animationName;
                _currentAnimation.Reset();
            }
        }
    }
    
    /// <summary>
    /// Update current animation
    /// </summary>
    public void Update(GameTime gameTime)
    {
        _currentAnimation?.Update(gameTime);
    }
    
    /// <summary>
    /// Get current frame source rectangle
    /// </summary>
    public Rectangle GetSourceRectangle()
    {
        return _currentAnimation?.GetSourceRectangle() ?? Rectangle.Empty;
    }
    
    /// <summary>
    /// Get current animation texture
    /// </summary>
    public Texture2D GetTexture()
    {
        return _currentAnimation?.Texture;
    }
    
    public Direction CurrentDirection
    {
        get => _currentDirection;
        set => _currentDirection = value;
    }
    
    public bool IsPlaying => _currentAnimation != null && _currentAnimation.IsPlaying;
}

/// <summary>
/// Single animation with frame management
/// </summary>
public class Animation
{
    public Texture2D Texture { get; private set; }
    private int _frameCount;
    private int _currentFrame;
    private float _frameTime; // Time per frame in seconds
    private float _timer;
    private bool _isLooping;
    private bool _isPlaying;
    private int _frameWidth;
    private int _frameHeight;
    
    public Animation(Texture2D texture, int frameCount, float frameTime, bool isLooping = true)
    {
        Texture = texture;
        _frameCount = frameCount;
        _frameTime = frameTime;
        _isLooping = isLooping;
        _isPlaying = true;
        _currentFrame = 0;
        _timer = 0;
        
        // Calculate frame dimensions
        _frameWidth = texture.Width / frameCount;
        _frameHeight = texture.Height;
    }
    
    public void Update(GameTime gameTime)
    {
        if (!_isPlaying) return;
        
        _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        if (_timer >= _frameTime)
        {
            _currentFrame++;
            _timer = 0;
            
            if (_currentFrame >= _frameCount)
            {
                if (_isLooping)
                {
                    _currentFrame = 0;
                }
                else
                {
                    _currentFrame = _frameCount - 1;
                    _isPlaying = false;
                }
            }
        }
    }
    
    public Rectangle GetSourceRectangle()
    {
        return new Rectangle(_currentFrame * _frameWidth, 0, _frameWidth, _frameHeight);
    }
    
    public void Reset()
    {
        _currentFrame = 0;
        _timer = 0;
        _isPlaying = true;
    }
    
    public void Pause() => _isPlaying = false;
    public void Resume() => _isPlaying = true;
    
    public bool IsPlaying => _isPlaying;
    public int CurrentFrame => _currentFrame;
}

/// <summary>
/// Directional animation set (up, down, left, right)
/// </summary>
public class DirectionalAnimation
{
    private Dictionary<Direction, Animation> _animations;
    
    public DirectionalAnimation()
    {
        _animations = new Dictionary<Direction, Animation>();
    }
    
    public void AddDirection(Direction direction, Animation animation)
    {
        _animations[direction] = animation;
    }
    
    public Animation GetAnimation(Direction direction)
    {
        return _animations.ContainsKey(direction) ? _animations[direction] : null;
    }
}

public enum Direction
{
    Up,
    Down,
    Left,
    Right
}
