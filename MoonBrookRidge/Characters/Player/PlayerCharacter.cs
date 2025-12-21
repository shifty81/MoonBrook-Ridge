using System;
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
    
    public PlayerCharacter(Vector2 startPosition)
    {
        _position = startPosition;
        _velocity = Vector2.Zero;
        _facing = Direction.Down;
        _currentActivity = PlayerActivity.Idle;
        
        // Initialize stats
        _stats = new PlayerStats();
        
        // Initialize animation controller
        _animationController = new AnimationController();
    }
    
    public void Update(GameTime gameTime, InputManager input)
    {
        HandleInput(gameTime, input);
        UpdatePosition(gameTime);
        UpdateActivity();
        
        // Update stats (hunger/thirst decay)
        _stats.Update(gameTime, _currentActivity);
        
        // Update animations
        _animationController.Update(gameTime);
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
    
    private void UpdatePosition(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _position += _velocity * deltaTime;
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
        // Get current animation texture and source rect
        Texture2D texture = _animationController.GetTexture();
        Rectangle sourceRect = _animationController.GetSourceRectangle();
        
        if (texture != null)
        {
            spriteBatch.Draw(
                texture,
                _position,
                sourceRect,
                Color.White,
                0f,
                new Vector2(sourceRect.Width / 2, sourceRect.Height / 2),
                1f,
                SpriteEffects.None,
                0f
            );
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
    
    public void ModifyHealth(float amount) => _stats.ModifyHealth(amount);
    public void ModifyEnergy(float amount) => _stats.ModifyEnergy(amount);
    public void ModifyMoney(int amount) => _stats.ModifyMoney(amount);
}

