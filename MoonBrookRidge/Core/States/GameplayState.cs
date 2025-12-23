using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Characters.Player;
using MoonBrookRidge.World.Maps;
using MoonBrookRidge.UI.HUD;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.Core.States;

/// <summary>
/// Main gameplay state - handles the core game loop
/// </summary>
public class GameplayState : GameState
{
    private PlayerCharacter _player;
    private WorldMap _worldMap;
    private HUDManager _hud;
    private TimeSystem _timeSystem;
    private Camera2D _camera;
    private InputManager _inputManager;
    private bool _isPaused;

    public GameplayState(Game1 game) : base(game) { }

    public override void Initialize()
    {
        base.Initialize();
        
        // Initialize core systems
        _timeSystem = new TimeSystem();
        _camera = new Camera2D(Game.GraphicsDevice.Viewport);
        _inputManager = new InputManager();
        
        // Initialize world and player
        _worldMap = new WorldMap();
        _player = new PlayerCharacter(new Vector2(400, 300));
        
        // Initialize UI
        _hud = new HUDManager();
        
        _isPaused = false;
    }

    public override void LoadContent()
    {
        base.LoadContent();
        
        // Load character animation textures
        var animations = new Dictionary<string, Texture2D>
        {
            ["walk"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_walk_strip8"),
            ["run"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_run_strip8"),
            ["idle"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_idle_strip9"),
            ["waiting"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_waiting_strip9"),
            ["dig"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_dig_strip13"),
            ["mining"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_mining_strip10"),
            ["axe"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_axe_strip10"),
            ["watering"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_watering_strip5"),
            ["casting"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_casting_strip15"),
            ["reeling"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_reeling_strip13"),
            ["caught"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_caught_strip10"),
            ["attack"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_attack_strip10"),
            ["hurt"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_hurt_strip8"),
            ["death"] = Game.Content.Load<Texture2D>("Textures/Characters/Animations/base_death_strip13")
        };
        
        // Load tool overlay animation textures
        var toolAnimations = new Dictionary<string, Texture2D>
        {
            ["walk"] = Game.Content.Load<Texture2D>("Textures/Characters/Tools/tools_walk_strip8"),
            ["run"] = Game.Content.Load<Texture2D>("Textures/Characters/Tools/tools_run_strip8"),
            ["idle"] = Game.Content.Load<Texture2D>("Textures/Characters/Tools/tools_idle_strip9"),
            ["waiting"] = Game.Content.Load<Texture2D>("Textures/Characters/Tools/tools_waiting_strip9"),
            ["dig"] = Game.Content.Load<Texture2D>("Textures/Characters/Tools/tools_dig_strip13"),
            ["mining"] = Game.Content.Load<Texture2D>("Textures/Characters/Tools/tools_mining_strip10"),
            ["axe"] = Game.Content.Load<Texture2D>("Textures/Characters/Tools/tools_axe_strip10"),
            ["watering"] = Game.Content.Load<Texture2D>("Textures/Characters/Tools/tools_watering_strip5"),
            ["casting"] = Game.Content.Load<Texture2D>("Textures/Characters/Tools/tools_casting_strip15"),
            ["reeling"] = Game.Content.Load<Texture2D>("Textures/Characters/Tools/tools_reeling_strip13"),
            ["caught"] = Game.Content.Load<Texture2D>("Textures/Characters/Tools/tools_caught_strip10")
        };
        
        // Pass animations to player
        _player.LoadContent(animations, toolAnimations);
        
        // Load world map content
        Texture2D grassTexture = Game.Content.Load<Texture2D>("Textures/Tiles/grass");
        Texture2D plainsTexture = Game.Content.Load<Texture2D>("Textures/Tiles/plains");
        _worldMap.LoadContent(grassTexture, plainsTexture);
    }

    public override void Update(GameTime gameTime)
    {
        // Update input first
        _inputManager.Update();
        
        // Check for pause
        if (_inputManager.IsOpenMenuPressed())
        {
            _isPaused = !_isPaused;
        }
        
        // Don't update game logic when paused
        if (_isPaused)
        {
            // Update pause menu here when implemented
            return;
        }
        
        // Update time system
        _timeSystem.Update(gameTime);
        
        // Update player with input
        _player.Update(gameTime, _inputManager);
        
        // Update camera to follow player
        _camera.Follow(_player.Position);
        
        // Update HUD
        _hud.Update(gameTime, _player, _timeSystem);
        
        // Check for sleep time (player exhaustion at 2 AM)
        if (_timeSystem.TimeOfDay >= 26f && _player.Energy < 10f)
        {
            ForceSleep();
        }
    }
    
    private void ForceSleep()
    {
        // Player passes out from exhaustion
        // Reset to next day, restore some energy, apply penalties
        _player.ModifyEnergy(50f); // Only restore half energy as penalty
        _player.ModifyHealth(-10f); // Lose some health
        // Time system will advance the day
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Draw world with camera transform
        spriteBatch.Begin(transformMatrix: _camera.GetTransform(), 
                         samplerState: SamplerState.PointClamp);
        
        _worldMap.Draw(spriteBatch);
        _player.Draw(spriteBatch);
        
        spriteBatch.End();
        
        // Draw HUD (no camera transform)
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _hud.DrawPlayerStats(spriteBatch, Game.DefaultFont, _player, _timeSystem);
        
        // Draw pause indicator
        if (_isPaused)
        {
            DrawPauseOverlay(spriteBatch);
        }
        
        spriteBatch.End();
    }
    
    private void DrawPauseOverlay(SpriteBatch spriteBatch)
    {
        // Draw semi-transparent overlay
        Texture2D pixel = CreatePixelTexture();
        Rectangle screen = new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
        spriteBatch.Draw(pixel, screen, Color.Black * 0.5f);
        
        // Draw "PAUSED" text
        if (Game.DefaultFont != null)
        {
            string pauseText = "PAUSED";
            Vector2 textSize = Game.DefaultFont.MeasureString(pauseText);
            Vector2 textPos = new Vector2(
                (Game.GraphicsDevice.Viewport.Width - textSize.X) / 2,
                (Game.GraphicsDevice.Viewport.Height - textSize.Y) / 2
            );
            
            spriteBatch.DrawString(Game.DefaultFont, pauseText, textPos + Vector2.One * 2, Color.Black);
            spriteBatch.DrawString(Game.DefaultFont, pauseText, textPos, Color.White);
        }
    }
    
    private Texture2D CreatePixelTexture()
    {
        Texture2D texture = new Texture2D(Game.GraphicsDevice, 1, 1);
        texture.SetData(new[] { Color.White });
        return texture;
    }
}
