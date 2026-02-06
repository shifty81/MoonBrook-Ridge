using MoonBrookRidge.Engine.MonoGameCompat;
using System;

namespace MoonBrookRidge.World.Fishing;

/// <summary>
/// Fishing minigame with timing-based challenge
/// Player must press the key when the indicator is in the success zone
/// </summary>
public class FishingMinigame
{
    private bool _isActive;
    private bool _isSuccess;
    private bool _isComplete;
    
    private float _progress;  // 0.0 to 1.0
    private float _speed;     // How fast the progress bar moves
    private bool _movingForward;
    
    private float _targetZoneStart;
    private float _targetZoneEnd;
    private float _targetZoneSize;
    
    private Texture2D _pixelTexture;
    
    private const float MIN_ZONE_SIZE = 0.15f;
    private const float MAX_ZONE_SIZE = 0.35f;
    
    private KeyboardState _previousKeyState;
    
    public bool IsActive => _isActive;
    public bool IsComplete => _isComplete;
    public bool IsSuccess => _isSuccess;
    
    public FishingMinigame()
    {
        _isActive = false;
        _isComplete = false;
        _isSuccess = false;
    }
    
    /// <summary>
    /// Start a new fishing minigame
    /// </summary>
    /// <param name="difficulty">0.0 (easy) to 1.0 (hard)</param>
    public void Start(float difficulty = 0.5f)
    {
        _isActive = true;
        _isComplete = false;
        _isSuccess = false;
        
        _progress = 0.0f;
        _movingForward = true;
        
        // Difficulty affects speed and zone size
        _speed = 0.5f + (difficulty * 1.5f); // 0.5 to 2.0
        _targetZoneSize = MAX_ZONE_SIZE - (difficulty * (MAX_ZONE_SIZE - MIN_ZONE_SIZE));
        
        // Random target zone position
        Random random = new Random();
        _targetZoneStart = (float)random.NextDouble() * (1.0f - _targetZoneSize);
        _targetZoneEnd = _targetZoneStart + _targetZoneSize;
    }
    
    public void Update(GameTime gameTime)
    {
        if (!_isActive || _isComplete)
            return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Move progress indicator back and forth
        if (_movingForward)
        {
            _progress += _speed * deltaTime;
            if (_progress >= 1.0f)
            {
                _progress = 1.0f;
                _movingForward = false;
            }
        }
        else
        {
            _progress -= _speed * deltaTime;
            if (_progress <= 0.0f)
            {
                _progress = 0.0f;
                _movingForward = true;
            }
        }
        
        // Check for input
        KeyboardState currentKeyState = Keyboard.GetState();
        
        // Player presses C or Space to attempt catch
        bool cKeyPressed = currentKeyState.IsKeyDown(Keys.C) && !_previousKeyState.IsKeyDown(Keys.C);
        bool spaceKeyPressed = currentKeyState.IsKeyDown(Keys.Space) && !_previousKeyState.IsKeyDown(Keys.Space);
        
        if (cKeyPressed || spaceKeyPressed)
        {
            CompleteFishing();
        }
        
        _previousKeyState = currentKeyState;
    }
    
    private void CompleteFishing()
    {
        _isComplete = true;
        
        // Check if progress is within target zone
        if (_progress >= _targetZoneStart && _progress <= _targetZoneEnd)
        {
            _isSuccess = true;
        }
        else
        {
            _isSuccess = false;
        }
    }
    
    public void Reset()
    {
        _isActive = false;
        _isComplete = false;
        _isSuccess = false;
        _progress = 0.0f;
    }
    
    /// <summary>
    /// Draw the fishing minigame UI
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, int screenWidth, int screenHeight)
    {
        if (!_isActive)
            return;
        
        // Create pixel texture if not already created
        if (_pixelTexture == null)
        {
            _pixelTexture = CreatePixelTexture(spriteBatch.GraphicsDevice);
        }
        
        // Draw semi-transparent background
        int panelWidth = 400;
        int panelHeight = 100;
        int panelX = (screenWidth - panelWidth) / 2;
        int panelY = screenHeight - 150;
        
        // Background panel
        spriteBatch.Draw(_pixelTexture, new Rectangle(panelX, panelY, panelWidth, panelHeight), Color.Black * 0.7f);
        
        // Progress bar background
        int barWidth = 360;
        int barHeight = 30;
        int barX = panelX + 20;
        int barY = panelY + 35;
        
        spriteBatch.Draw(_pixelTexture, new Rectangle(barX, barY, barWidth, barHeight), Color.DarkGray);
        
        // Target zone (green)
        int zoneX = barX + (int)(_targetZoneStart * barWidth);
        int zoneWidth = (int)(_targetZoneSize * barWidth);
        spriteBatch.Draw(_pixelTexture, new Rectangle(zoneX, barY, zoneWidth, barHeight), Color.Green * 0.5f);
        
        // Progress indicator (white line)
        int indicatorX = barX + (int)(_progress * barWidth);
        spriteBatch.Draw(_pixelTexture, new Rectangle(indicatorX - 2, barY - 5, 4, barHeight + 10), Color.White);
        
        // Instruction text
        string instruction = _isComplete 
            ? (_isSuccess ? "Success!" : "Missed!") 
            : "Press C or Space when in green zone!";
        
        Vector2 textSize = font.MeasureString(instruction);
        Vector2 textPos = new Vector2(panelX + (panelWidth - textSize.X) / 2, panelY + 10);
        
        spriteBatch.DrawString(font, instruction, textPos, _isComplete 
            ? (_isSuccess ? Color.LimeGreen : Color.Red) 
            : Color.White);
    }
    
    private Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
    {
        Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
}
