using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Types of weather conditions
/// </summary>
public enum WeatherType
{
    Clear,
    Sunny,
    Cloudy,
    Rainy,
    Stormy,
    Snowy,
    Windy,
    Foggy
}

/// <summary>
/// Manages weather conditions and effects
/// </summary>
public class WeatherSystem
{
    private WeatherType _currentWeather;
    private float _weatherDuration;
    private float _weatherElapsed;
    private Random _random;
    private TimeSystem _timeSystem;
    
    // Weather particles
    private WeatherParticle[] _particles;
    private const int MAX_PARTICLES = 500;
    
    public WeatherSystem(TimeSystem timeSystem)
    {
        _timeSystem = timeSystem;
        _random = new Random();
        _currentWeather = WeatherType.Clear;
        _weatherDuration = 3600f; // Weather lasts for 1 hour in game time by default
        _weatherElapsed = 0f;
        
        // Initialize weather particles
        _particles = new WeatherParticle[MAX_PARTICLES];
        for (int i = 0; i < MAX_PARTICLES; i++)
        {
            _particles[i] = new WeatherParticle();
        }
    }
    
    /// <summary>
    /// Updates weather system
    /// </summary>
    public void Update(GameTime gameTime)
    {
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _weatherElapsed += deltaTime;
        
        // Check if it's time to change weather
        if (_weatherElapsed >= _weatherDuration)
        {
            ChangeWeather();
        }
        
        // Update weather particles based on current weather
        UpdateWeatherParticles(gameTime);
    }
    
    /// <summary>
    /// Changes to a new weather condition
    /// </summary>
    private void ChangeWeather()
    {
        _weatherElapsed = 0f;
        _currentWeather = DetermineNextWeather();
        _weatherDuration = DetermineWeatherDuration();
        
        // Reset particles for new weather type
        ResetWeatherParticles();
    }
    
    /// <summary>
    /// Determines the next weather based on season and current conditions
    /// </summary>
    private WeatherType DetermineNextWeather()
    {
        var season = _timeSystem.CurrentSeason;
        int roll = _random.Next(100);
        
        // Weather probabilities vary by season
        return season switch
        {
            TimeSystem.Season.Spring => roll switch
            {
                < 40 => WeatherType.Clear,
                < 60 => WeatherType.Cloudy,
                < 80 => WeatherType.Rainy,
                < 95 => WeatherType.Windy,
                _ => WeatherType.Sunny
            },
            TimeSystem.Season.Summer => roll switch
            {
                < 60 => WeatherType.Sunny,
                < 80 => WeatherType.Clear,
                < 90 => WeatherType.Cloudy,
                < 95 => WeatherType.Windy,
                _ => WeatherType.Stormy
            },
            TimeSystem.Season.Fall => roll switch
            {
                < 30 => WeatherType.Clear,
                < 50 => WeatherType.Cloudy,
                < 70 => WeatherType.Rainy,
                < 85 => WeatherType.Windy,
                < 95 => WeatherType.Foggy,
                _ => WeatherType.Stormy
            },
            TimeSystem.Season.Winter => roll switch
            {
                < 30 => WeatherType.Clear,
                < 50 => WeatherType.Cloudy,
                < 80 => WeatherType.Snowy,
                < 90 => WeatherType.Windy,
                _ => WeatherType.Foggy
            },
            _ => WeatherType.Clear
        };
    }
    
    /// <summary>
    /// Determines how long the weather will last (in seconds)
    /// </summary>
    private float DetermineWeatherDuration()
    {
        // Weather lasts between 30 minutes and 2 hours of game time
        return _random.Next(1800, 7200);
    }
    
    /// <summary>
    /// Updates weather particles for visual effects
    /// </summary>
    private void UpdateWeatherParticles(GameTime gameTime)
    {
        int activeParticles = GetActiveParticleCount();
        
        for (int i = 0; i < activeParticles; i++)
        {
            _particles[i].Update(gameTime, _currentWeather);
        }
    }
    
    /// <summary>
    /// Resets weather particles for the current weather type
    /// </summary>
    private void ResetWeatherParticles()
    {
        int activeParticles = GetActiveParticleCount();
        
        for (int i = 0; i < activeParticles; i++)
        {
            _particles[i].Reset(_currentWeather);
        }
    }
    
    /// <summary>
    /// Gets the number of active particles based on weather type
    /// </summary>
    private int GetActiveParticleCount()
    {
        return _currentWeather switch
        {
            WeatherType.Rainy => 400,
            WeatherType.Stormy => 500,
            WeatherType.Snowy => 300,
            WeatherType.Foggy => 50,
            _ => 0
        };
    }
    
    /// <summary>
    /// Draws weather effects
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Rectangle viewportBounds)
    {
        // Draw overlay tint based on weather
        DrawWeatherOverlay(spriteBatch, graphicsDevice, viewportBounds);
        
        // Draw weather particles
        int activeParticles = GetActiveParticleCount();
        for (int i = 0; i < activeParticles; i++)
        {
            _particles[i].Draw(spriteBatch, graphicsDevice);
        }
    }
    
    /// <summary>
    /// Draws a weather overlay to tint the screen
    /// </summary>
    private void DrawWeatherOverlay(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice, Rectangle viewportBounds)
    {
        Color overlayColor = _currentWeather switch
        {
            WeatherType.Rainy => new Color(100, 100, 120, 40),
            WeatherType.Stormy => new Color(60, 60, 80, 60),
            WeatherType.Snowy => new Color(220, 230, 240, 30),
            WeatherType.Foggy => new Color(200, 200, 200, 80),
            WeatherType.Cloudy => new Color(180, 180, 180, 20),
            _ => Color.Transparent
        };
        
        if (overlayColor != Color.Transparent)
        {
            Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
            pixel.SetData(new[] { Color.White });
            spriteBatch.Draw(pixel, viewportBounds, overlayColor);
        }
    }
    
    /// <summary>
    /// Gets the current weather condition
    /// </summary>
    public WeatherType CurrentWeather => _currentWeather;
    
    /// <summary>
    /// Gets a description of the current weather
    /// </summary>
    public string GetWeatherDescription()
    {
        return _currentWeather switch
        {
            WeatherType.Clear => "Clear skies",
            WeatherType.Sunny => "Sunny and warm",
            WeatherType.Cloudy => "Cloudy",
            WeatherType.Rainy => "Rainy",
            WeatherType.Stormy => "Stormy",
            WeatherType.Snowy => "Snowing",
            WeatherType.Windy => "Windy",
            WeatherType.Foggy => "Foggy",
            _ => "Unknown"
        };
    }
    
    /// <summary>
    /// Checks if crops can be watered naturally by rain
    /// </summary>
    public bool IsRaining => _currentWeather == WeatherType.Rainy || _currentWeather == WeatherType.Stormy;
    
    /// <summary>
    /// Gets the movement speed modifier based on weather
    /// </summary>
    public float GetMovementModifier()
    {
        return _currentWeather switch
        {
            WeatherType.Stormy => 0.85f, // Slow down in storms
            WeatherType.Snowy => 0.90f,  // Slow down in snow
            WeatherType.Foggy => 0.95f,  // Slightly slower in fog
            _ => 1.0f
        };
    }
}

/// <summary>
/// Represents a single weather particle (rain drop, snowflake, etc.)
/// </summary>
internal class WeatherParticle
{
    private Vector2 _position;
    private Vector2 _velocity;
    private float _size;
    private Color _color;
    private bool _isActive;
    private Random _random;
    private static Random _sharedRandom = new Random();
    
    public WeatherParticle()
    {
        _random = _sharedRandom;
        _isActive = false;
    }
    
    public void Reset(WeatherType weatherType)
    {
        _isActive = true;
        
        // Random starting position across the screen
        _position = new Vector2(_random.Next(0, 1280), _random.Next(-100, 720));
        
        switch (weatherType)
        {
            case WeatherType.Rainy:
                _velocity = new Vector2(_random.Next(-50, -30), _random.Next(400, 600));
                _size = _random.Next(1, 3);
                _color = new Color(150, 150, 200, 180);
                break;
                
            case WeatherType.Stormy:
                _velocity = new Vector2(_random.Next(-100, -50), _random.Next(500, 800));
                _size = _random.Next(2, 4);
                _color = new Color(120, 120, 180, 200);
                break;
                
            case WeatherType.Snowy:
                _velocity = new Vector2(_random.Next(-20, 20), _random.Next(50, 150));
                _size = _random.Next(2, 5);
                _color = new Color(255, 255, 255, 220);
                break;
                
            case WeatherType.Foggy:
                _velocity = new Vector2(_random.Next(-10, 10), _random.Next(-5, 5));
                _size = _random.Next(20, 50);
                _color = new Color(200, 200, 200, 40);
                break;
                
            default:
                _isActive = false;
                break;
        }
    }
    
    public void Update(GameTime gameTime, WeatherType weatherType)
    {
        if (!_isActive) return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Update position
        _position += _velocity * deltaTime;
        
        // Reset if particle goes off screen
        if (_position.Y > 720 || _position.X < -100 || _position.X > 1380)
        {
            Reset(weatherType);
        }
    }
    
    public void Draw(SpriteBatch spriteBatch, GraphicsDevice graphicsDevice)
    {
        if (!_isActive) return;
        
        Texture2D pixel = new Texture2D(graphicsDevice, 1, 1);
        pixel.SetData(new[] { Color.White });
        
        Rectangle rect = new Rectangle((int)_position.X, (int)_position.Y, (int)_size, (int)_size);
        spriteBatch.Draw(pixel, rect, _color);
    }
}
