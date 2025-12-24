using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.Core.Components;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.Characters.Player;

/// <summary>
/// Player character with movement, animation, and stats including hunger/thirst
/// </summary>
public class PlayerCharacter
{
    private Vector2 _position;
    private Vector2 _velocity;
    private Direction _facing;
    
    // Stats system
    private PlayerStats _stats;
    private PlayerActivity _currentActivity;
    
    // Movement
    private const float WALK_SPEED = 120f;
    private const float RUN_SPEED = 200f;
    
    // Animation
    private AnimationController _animationController;
    private AnimationController _toolAnimationController;
    private bool _hasToolEquipped;
    
    // Animation frame info (frames per second for each animation)
    private Dictionary<string, int> _animationFrameCounts;
    private const float DEFAULT_FRAME_TIME = 0.1f; // 10 FPS default
    
    public PlayerCharacter(Vector2 startPosition)
    {
        _position = startPosition;
        _velocity = Vector2.Zero;
        _facing = Direction.Down;
        _currentActivity = PlayerActivity.Idle;
        _hasToolEquipped = false;
        
        // Initialize stats
        _stats = new PlayerStats();
        
        // Initialize animation controllers
        _animationController = new AnimationController();
        _toolAnimationController = new AnimationController();
        
        // Setup animation frame counts
        _animationFrameCounts = new Dictionary<string, int>
        {
            ["walk"] = 8,
            ["run"] = 8,
            ["idle"] = 9,
            ["waiting"] = 9,
            ["dig"] = 13,
            ["mining"] = 10,
            ["axe"] = 10,
            ["watering"] = 5,
            ["casting"] = 15,
            ["reeling"] = 13,
            ["caught"] = 10,
            ["attack"] = 10,
            ["hurt"] = 8,
            ["death"] = 13
        };
    }
    
    public void LoadContent(Dictionary<string, Texture2D> animations, Dictionary<string, Texture2D> toolAnimations)
    {
        // Setup base character animations
        foreach (var anim in animations)
        {
            string name = anim.Key;
            Texture2D texture = anim.Value;
            int frameCount = _animationFrameCounts.ContainsKey(name) ? _animationFrameCounts[name] : 8;
            
            // Calculate frame time based on animation type
            float frameTime = name == "run" ? 0.08f : // Faster for running
                            name == "walk" ? 0.1f :   // Normal for walking
                            name == "idle" || name == "waiting" ? 0.15f : // Slower for idle
                            DEFAULT_FRAME_TIME;
            
            Animation animation = new Animation(texture, frameCount, frameTime, isLooping: true);
            _animationController.AddAnimation(name, animation);
        }
        
        // Setup tool overlay animations
        foreach (var anim in toolAnimations)
        {
            string name = anim.Key;
            Texture2D texture = anim.Value;
            int frameCount = _animationFrameCounts.ContainsKey(name) ? _animationFrameCounts[name] : 8;
            
            float frameTime = name == "run" ? 0.08f :
                            name == "walk" ? 0.1f :
                            name == "idle" || name == "waiting" ? 0.15f :
                            DEFAULT_FRAME_TIME;
            
            Animation animation = new Animation(texture, frameCount, frameTime, isLooping: true);
            _toolAnimationController.AddAnimation(name, animation);
        }
        
        // Start with idle animation
        _animationController.Play("idle");
    }
    
    public void Update(GameTime gameTime, InputManager input, CollisionSystem collision = null)
    {
        HandleInput(gameTime, input);
        UpdatePosition(gameTime, collision);
        UpdateActivity();
        UpdateAnimations();
        
        // Update stats (hunger/thirst decay)
        _stats.Update(gameTime, _currentActivity);
        
        // Update animations
        _animationController.Update(gameTime);
        if (_hasToolEquipped)
        {
            _toolAnimationController.Update(gameTime);
        }
    }
    
    private void UpdateAnimations()
    {
        // Determine which animation to play based on activity
        string animationName = "idle";
        
        switch (_currentActivity)
        {
            case PlayerActivity.Idle:
                animationName = "idle";
                break;
            case PlayerActivity.Walking:
                animationName = "walk";
                break;
            case PlayerActivity.Running:
                animationName = "run";
                break;
            case PlayerActivity.UsingTool:
                animationName = "dig"; // Generic tool use
                _hasToolEquipped = true;
                break;
            case PlayerActivity.Mining:
                animationName = "mining";
                _hasToolEquipped = true;
                break;
            case PlayerActivity.Chopping:
                animationName = "axe";
                _hasToolEquipped = true;
                break;
            case PlayerActivity.Watering:
                animationName = "watering";
                _hasToolEquipped = true;
                break;
            case PlayerActivity.Fishing:
                animationName = "casting";
                _hasToolEquipped = true;
                break;
            default:
                _hasToolEquipped = false;
                break;
        }
        
        // Update animation direction
        _animationController.CurrentDirection = _facing;
        
        // Play the appropriate animation
        _animationController.Play(animationName);
        
        // Play matching tool animation if tool is equipped
        if (_hasToolEquipped)
        {
            _toolAnimationController.CurrentDirection = _facing;
            _toolAnimationController.Play(animationName);
        }
    }
    
    private void HandleInput(GameTime gameTime, InputManager input)
    {
        Vector2 movement = Vector2.Zero;
        
        // Movement input
        if (input.IsMoveUpPressed())
        {
            movement.Y = -1;
            _facing = Direction.Up;
        }
        if (input.IsMoveDownPressed())
        {
            movement.Y = 1;
            _facing = Direction.Down;
        }
        if (input.IsMoveLeftPressed())
        {
            movement.X = -1;
            _facing = Direction.Left;
        }
        if (input.IsMoveRightPressed())
        {
            movement.X = 1;
            _facing = Direction.Right;
        }
        
        // Normalize movement vector
        if (movement != Vector2.Zero)
        {
            movement.Normalize();
        }
        
        // Check if running
        bool isRunning = input.IsRunPressed() && _stats.Energy > 1f;
        float speed = isRunning ? RUN_SPEED : WALK_SPEED;
        
        // Apply hunger debuff to speed
        speed *= _stats.GetMovementSpeedMultiplier();
        
        // Drain energy when running
        if (isRunning && movement != Vector2.Zero)
        {
            _stats.ModifyEnergy(-2f * (float)gameTime.ElapsedGameTime.TotalSeconds);
        }
        
        _velocity = movement * speed;
    }
    
    private void UpdatePosition(GameTime gameTime, CollisionSystem collision)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        Vector2 desiredPosition = _position + (_velocity * deltaTime);
        
        // Apply collision detection if available
        if (collision != null)
        {
            _position = collision.ResolveCollision(_position, desiredPosition);
        }
        else
        {
            _position = desiredPosition;
        }
    }
    
    private void UpdateActivity()
    {
        if (_velocity.Length() > 0)
        {
            _currentActivity = _velocity.Length() > WALK_SPEED ? 
                PlayerActivity.Running : PlayerActivity.Walking;
        }
        else
        {
            _currentActivity = PlayerActivity.Idle;
        }
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        Texture2D currentTexture = _animationController.GetTexture();
        
        if (currentTexture != null)
        {
            Rectangle sourceRect = _animationController.GetSourceRectangle();
            
            // Calculate sprite effects for left/right facing
            SpriteEffects effects = _facing == Direction.Left ? SpriteEffects.FlipHorizontally : SpriteEffects.None;
            
            // Draw the base character sprite
            spriteBatch.Draw(
                currentTexture,
                _position,
                sourceRect,
                Color.White,
                0f,
                new Vector2(sourceRect.Width / 2, sourceRect.Height / 2), // Center origin
                2f, // Scale up 2x for visibility
                effects,
                0f
            );
            
            // Draw tool overlay if equipped
            if (_hasToolEquipped)
            {
                Texture2D toolTexture = _toolAnimationController.GetTexture();
                if (toolTexture != null)
                {
                    Rectangle toolSourceRect = _toolAnimationController.GetSourceRectangle();
                    spriteBatch.Draw(
                        toolTexture,
                        _position,
                        toolSourceRect,
                        Color.White,
                        0f,
                        new Vector2(toolSourceRect.Width / 2, toolSourceRect.Height / 2),
                        2f,
                        effects,
                        0f
                    );
                }
            }
        }
        else
        {
            // Fallback: Draw a simple colored rectangle representing the player
            Texture2D pixel = CreatePixelTexture(spriteBatch.GraphicsDevice);
            Rectangle playerRect = new Rectangle((int)_position.X - 16, (int)_position.Y - 16, 32, 32);
            spriteBatch.Draw(pixel, playerRect, Color.Blue);
        }
    }
    
    private Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
    {
        Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
    
    /// <summary>
    /// Use a tool at the specified position
    /// </summary>
    public void UseTool(Vector2 toolPosition, PlayerActivity toolActivity)
    {
        _currentActivity = toolActivity;
        
        // Drain energy based on tool efficiency
        float energyCost = 2f * _stats.GetToolEfficiency();
        _stats.ModifyEnergy(-energyCost);
    }
    
    /// <summary>
    /// Consume food item
    /// </summary>
    public void EatFood(float hungerRestored, float energyRestored = 0)
    {
        _stats.Eat(hungerRestored, energyRestored);
    }
    
    /// <summary>
    /// Consume drink item
    /// </summary>
    public void DrinkBeverage(float thirstRestored, float energyRestored = 0)
    {
        _stats.Drink(thirstRestored, energyRestored);
    }
    
    // Properties
    public Vector2 Position => _position;
    public Direction Facing => _facing;
    public PlayerActivity CurrentActivity => _currentActivity;
    public PlayerStats Stats => _stats;
    
    // Legacy properties for compatibility
    public float Health => _stats.Health;
    public float MaxHealth => _stats.MaxHealth;
    public float Energy => _stats.Energy;
    public float MaxEnergy => _stats.MaxEnergy;
    public int Money => _stats.Money;
    public float Hunger => _stats.Hunger;
    public float Thirst => _stats.Thirst;
    
    public void ModifyHealth(float amount) => _stats.ModifyHealth(amount);
    public void ModifyEnergy(float amount) => _stats.ModifyEnergy(amount);
    public void ModifyMoney(int amount) => _stats.ModifyMoney(amount);
    
    // Setters for save/load
    public void SetPosition(Vector2 position) => _position = position;
    public void SetHealth(float health) => _stats.Health = health;
    public void SetEnergy(float energy) => _stats.Energy = energy;
    public void SetMoney(int money) => _stats.Money = money;
    public void SetHunger(float hunger) => _stats.Hunger = hunger;
    public void SetThirst(float thirst) => _stats.Thirst = thirst;
}

