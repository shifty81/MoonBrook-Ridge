using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.Items;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.World.Tiles;
using System;

namespace MoonBrookRidge.World.Fishing;

/// <summary>
/// Manages the fishing system - detecting water, starting minigame, catching fish
/// </summary>
public class FishingManager
{
    private FishingMinigame _minigame;
    private bool _isFishing;
    private FishingState _currentState;
    private float _stateTimer;
    
    private Vector2 _fishingPosition;
    private FishHabitat _currentHabitat;
    private string _currentSeason;
    
    private Texture2D _pixelTexture;
    
    private Random _random;
    
    private const float CASTING_DURATION = 1.5f;
    private const float WAITING_DURATION = 2.0f;
    private const float CATCH_DISPLAY_DURATION = 2.0f;
    
    public bool IsFishing => _isFishing;
    public FishingState CurrentState => _currentState;
    public FishingMinigame Minigame => _minigame;
    
    public FishingManager()
    {
        _minigame = new FishingMinigame();
        _isFishing = false;
        _currentState = FishingState.Idle;
        _random = new Random();
        _currentSeason = "Spring"; // Default
    }
    
    /// <summary>
    /// Check if a position has water tile nearby
    /// </summary>
    public bool IsNearWater(Vector2 playerPosition, Tile[,] tiles, int tileSize = 16)
    {
        int gridX = (int)(playerPosition.X / tileSize);
        int gridY = (int)(playerPosition.Y / tileSize);
        
        // Check adjacent tiles
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int checkX = gridX + dx;
                int checkY = gridY + dy;
                
                if (checkX >= 0 && checkX < tiles.GetLength(0) && 
                    checkY >= 0 && checkY < tiles.GetLength(1))
                {
                    Tile tile = tiles[checkX, checkY];
                    if (tile != null && IsWaterTile(tile.Type))
                    {
                        _fishingPosition = new Vector2(checkX * tileSize, checkY * tileSize);
                        _currentHabitat = GetHabitatFromTileType(tile.Type);
                        return true;
                    }
                }
            }
        }
        
        return false;
    }
    
    private bool IsWaterTile(TileType type)
    {
        return type == TileType.Water || 
               type == TileType.Water01 ||
               type == TileType.SlatesWaterStill ||
               type == TileType.SlatesWaterAnimated ||
               type == TileType.SlatesWaterDeep ||
               type == TileType.SlatesWaterShallow;
    }
    
    private FishHabitat GetHabitatFromTileType(TileType type)
    {
        // For now, treat all water as river
        // Could be expanded based on tile type or location
        return type switch
        {
            TileType.SlatesWaterDeep => FishHabitat.Lake,
            _ => FishHabitat.River
        };
    }
    
    /// <summary>
    /// Start fishing at the current position
    /// </summary>
    public void StartFishing(string currentSeason)
    {
        if (_isFishing)
            return;
        
        _isFishing = true;
        _currentSeason = currentSeason;
        _currentState = FishingState.Casting;
        _stateTimer = CASTING_DURATION;
    }
    
    /// <summary>
    /// Update fishing state machine
    /// </summary>
    public void Update(GameTime gameTime, InventorySystem inventory)
    {
        if (!_isFishing)
            return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        switch (_currentState)
        {
            case FishingState.Casting:
                _stateTimer -= deltaTime;
                if (_stateTimer <= 0)
                {
                    // Move to waiting state
                    _currentState = FishingState.Waiting;
                    _stateTimer = WAITING_DURATION;
                }
                break;
                
            case FishingState.Waiting:
                _stateTimer -= deltaTime;
                if (_stateTimer <= 0)
                {
                    // Fish bites! Start minigame
                    _currentState = FishingState.Minigame;
                    float difficulty = GetFishingDifficulty();
                    _minigame.Start(difficulty);
                }
                break;
                
            case FishingState.Minigame:
                _minigame.Update(gameTime);
                
                if (_minigame.IsComplete)
                {
                    if (_minigame.IsSuccess)
                    {
                        // Caught a fish!
                        FishItem fish = FishFactory.GetRandomFish(_currentHabitat, _currentSeason, _random);
                        inventory.AddItem(fish, 1);
                        _currentState = FishingState.Caught;
                        _stateTimer = CATCH_DISPLAY_DURATION;
                    }
                    else
                    {
                        // Missed the fish
                        _currentState = FishingState.Failed;
                        _stateTimer = CATCH_DISPLAY_DURATION;
                    }
                }
                break;
                
            case FishingState.Caught:
            case FishingState.Failed:
                _stateTimer -= deltaTime;
                if (_stateTimer <= 0)
                {
                    // Return to idle
                    EndFishing();
                }
                break;
        }
    }
    
    /// <summary>
    /// Get fishing difficulty based on habitat and season
    /// </summary>
    private float GetFishingDifficulty()
    {
        float difficulty = 0.3f; // Base difficulty
        
        // Harder in deeper water
        if (_currentHabitat == FishHabitat.Lake)
            difficulty += 0.2f;
        else if (_currentHabitat == FishHabitat.Ocean)
            difficulty += 0.3f;
        
        return Math.Clamp(difficulty, 0.0f, 1.0f);
    }
    
    /// <summary>
    /// Cancel current fishing
    /// </summary>
    public void EndFishing()
    {
        _isFishing = false;
        _currentState = FishingState.Idle;
        _minigame.Reset();
    }
    
    public void Draw(SpriteBatch spriteBatch, SpriteFont font, int screenWidth, int screenHeight)
    {
        if (!_isFishing)
            return;
        
        // Create pixel texture if not already created
        if (_pixelTexture == null)
        {
            _pixelTexture = CreatePixelTexture(spriteBatch.GraphicsDevice);
        }
        
        // Draw fishing state
        if (_currentState == FishingState.Minigame)
        {
            _minigame.Draw(spriteBatch, font, screenWidth, screenHeight);
        }
        else
        {
            // Draw state text
            string stateText = _currentState switch
            {
                FishingState.Casting => "Casting...",
                FishingState.Waiting => "Waiting for bite...",
                FishingState.Caught => "Fish caught!",
                FishingState.Failed => "Fish got away...",
                _ => ""
            };
            
            if (!string.IsNullOrEmpty(stateText))
            {
                Vector2 textSize = font.MeasureString(stateText);
                Vector2 textPos = new Vector2((screenWidth - textSize.X) / 2, screenHeight - 100);
                
                // Draw background
                Rectangle bgRect = new Rectangle((int)textPos.X - 10, (int)textPos.Y - 10, 
                                                 (int)textSize.X + 20, (int)textSize.Y + 20);
                spriteBatch.Draw(_pixelTexture, bgRect, Color.Black * 0.7f);
                
                // Draw text
                Color textColor = _currentState switch
                {
                    FishingState.Caught => Color.LimeGreen,
                    FishingState.Failed => Color.Red,
                    _ => Color.White
                };
                
                spriteBatch.DrawString(font, stateText, textPos, textColor);
            }
        }
    }
    
    private Texture2D CreatePixelTexture(GraphicsDevice graphicsDevice)
    {
        Texture2D texture = new Texture2D(graphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
}

public enum FishingState
{
    Idle,
    Casting,
    Waiting,
    Minigame,
    Caught,
    Failed
}
