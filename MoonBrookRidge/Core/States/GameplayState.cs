using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Characters.Player;
using MoonBrookRidge.World.Maps;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.UI.HUD;
using MoonBrookRidge.Core.Systems;
using MoonBrookRidge.Farming.Tools;
using MoonBrookRidge.Items;
using MoonBrookRidge.Items.Inventory;

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
    private ToolManager _toolManager;
    private CollisionSystem _collisionSystem;
    private InventorySystem _inventory;
    private ConsumableManager _consumableManager;
    private SeedManager _seedManager;
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
        
        // Initialize collision system
        _collisionSystem = new CollisionSystem(_worldMap);
        
        // Initialize inventory and give player starting items
        _inventory = new InventorySystem(36);
        _consumableManager = new ConsumableManager(_inventory, _player);
        
        // Add starting items to inventory
        _inventory.AddItem(ConsumableManager.GetFood("Carrot"), 5);
        _inventory.AddItem(ConsumableManager.GetFood("Apple"), 3);
        _inventory.AddItem(ConsumableManager.GetFood("Wheat Bread"), 2);
        _inventory.AddItem(ConsumableManager.GetDrink("Water"), 10);
        _inventory.AddItem(ConsumableManager.GetDrink("Spring Water"), 3);
        
        // Add starting seeds
        _inventory.AddItem(SeedFactory.GetSeed("wheat seeds"), 20);
        _inventory.AddItem(SeedFactory.GetSeed("carrot seeds"), 10);
        _inventory.AddItem(SeedFactory.GetSeed("potato seeds"), 10);
        
        // Initialize tool manager with inventory
        _toolManager = new ToolManager(_worldMap, _player, _inventory);
        
        // Initialize seed manager
        _seedManager = new SeedManager(_inventory, _toolManager, _player);
        
        // Give player starting tools
        _toolManager.SetCurrentTool(new Hoe());
        
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
        
        // Load crop textures
        var cropTextures = new Dictionary<string, Texture2D[]>
        {
            ["wheat"] = new Texture2D[6]
            {
                Game.Content.Load<Texture2D>("Textures/Crops/wheat_00"),
                Game.Content.Load<Texture2D>("Textures/Crops/wheat_01"),
                Game.Content.Load<Texture2D>("Textures/Crops/wheat_02"),
                Game.Content.Load<Texture2D>("Textures/Crops/wheat_03"),
                Game.Content.Load<Texture2D>("Textures/Crops/wheat_04"),
                Game.Content.Load<Texture2D>("Textures/Crops/wheat_05")
            },
            ["potato"] = new Texture2D[6]
            {
                Game.Content.Load<Texture2D>("Textures/Crops/potato_00"),
                Game.Content.Load<Texture2D>("Textures/Crops/potato_01"),
                Game.Content.Load<Texture2D>("Textures/Crops/potato_02"),
                Game.Content.Load<Texture2D>("Textures/Crops/potato_03"),
                Game.Content.Load<Texture2D>("Textures/Crops/potato_04"),
                Game.Content.Load<Texture2D>("Textures/Crops/potato_05")
            },
            ["carrot"] = new Texture2D[6]
            {
                Game.Content.Load<Texture2D>("Textures/Crops/carrot_00"),
                Game.Content.Load<Texture2D>("Textures/Crops/carrot_01"),
                Game.Content.Load<Texture2D>("Textures/Crops/carrot_02"),
                Game.Content.Load<Texture2D>("Textures/Crops/carrot_03"),
                Game.Content.Load<Texture2D>("Textures/Crops/carrot_04"),
                Game.Content.Load<Texture2D>("Textures/Crops/carrot_05")
            },
            ["cabbage"] = new Texture2D[6]
            {
                Game.Content.Load<Texture2D>("Textures/Crops/cabbage_00"),
                Game.Content.Load<Texture2D>("Textures/Crops/cabbage_01"),
                Game.Content.Load<Texture2D>("Textures/Crops/cabbage_02"),
                Game.Content.Load<Texture2D>("Textures/Crops/cabbage_03"),
                Game.Content.Load<Texture2D>("Textures/Crops/cabbage_04"),
                Game.Content.Load<Texture2D>("Textures/Crops/cabbage_05")
            },
            ["beetroot"] = new Texture2D[6]
            {
                Game.Content.Load<Texture2D>("Textures/Crops/beetroot_00"),
                Game.Content.Load<Texture2D>("Textures/Crops/beetroot_01"),
                Game.Content.Load<Texture2D>("Textures/Crops/beetroot_02"),
                Game.Content.Load<Texture2D>("Textures/Crops/beetroot_03"),
                Game.Content.Load<Texture2D>("Textures/Crops/beetroot_04"),
                Game.Content.Load<Texture2D>("Textures/Crops/beetroot_05")
            }
        };
        
        // Load tile textures
        var tileTextures = new Dictionary<TileType, Texture2D>
        {
            [TileType.Grass] = Game.Content.Load<Texture2D>("Textures/Tiles/grass"),
            [TileType.Grass01] = Game.Content.Load<Texture2D>("Textures/Tiles/grass_01"),
            [TileType.Grass02] = Game.Content.Load<Texture2D>("Textures/Tiles/grass_02"),
            [TileType.Grass03] = Game.Content.Load<Texture2D>("Textures/Tiles/grass_03"),
            [TileType.Dirt] = Game.Content.Load<Texture2D>("Textures/Tiles/plains"),
            [TileType.Dirt01] = Game.Content.Load<Texture2D>("Textures/Tiles/dirt_01"),
            [TileType.Dirt02] = Game.Content.Load<Texture2D>("Textures/Tiles/dirt_02"),
            [TileType.Tilled] = Game.Content.Load<Texture2D>("Textures/Tiles/tilled_01"),
            [TileType.TilledDry] = Game.Content.Load<Texture2D>("Textures/Tiles/tilled_soil_dry"),
            [TileType.TilledWatered] = Game.Content.Load<Texture2D>("Textures/Tiles/tilled_soil_watered"),
            [TileType.Stone] = Game.Content.Load<Texture2D>("Textures/Tiles/stone_01"),
            [TileType.Stone01] = Game.Content.Load<Texture2D>("Textures/Tiles/stone_01"),
            [TileType.Rock] = Game.Content.Load<Texture2D>("Textures/Tiles/rock"),
            [TileType.Water] = Game.Content.Load<Texture2D>("Textures/Tiles/water_01"),
            [TileType.Water01] = Game.Content.Load<Texture2D>("Textures/Tiles/water_01"),
            [TileType.Sand] = Game.Content.Load<Texture2D>("Textures/Tiles/sand_01"),
            [TileType.Sand01] = Game.Content.Load<Texture2D>("Textures/Tiles/sand_01"),
            [TileType.WoodenFloor] = Game.Content.Load<Texture2D>("Textures/Tiles/wooden_floor"),
            [TileType.Flooring] = Game.Content.Load<Texture2D>("Textures/Tiles/flooring")
        };
        
        // Load world map content
        _worldMap.LoadContent(tileTextures, cropTextures);
        
        // Plant some test crops to demonstrate the system
        _worldMap.PlantTestCrops();
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
        
        // Update crop growth based on game time
        _worldMap.UpdateCropGrowth(_timeSystem.LastGameHoursElapsed);
        
        // Update world map
        _worldMap.Update(gameTime);
        
        // Handle tool usage input
        HandleToolInput();
        
        // Handle seed planting input
        HandleSeedPlantingInput();
        
        // Handle consumable usage input
        HandleConsumableInput();
        
        // Update player with input and collision
        _player.Update(gameTime, _inputManager, _collisionSystem);
        
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
    
    private void HandleToolInput()
    {
        // Check for tool usage (C key)
        if (_inputManager.IsUseToolPressed())
        {
            // Calculate tile position in front of player based on facing direction
            Vector2 toolPosition = CalculateToolTargetPosition();
            
            // Use the tool at that position
            _toolManager.UseTool(toolPosition, _player.Stats);
        }
        
        // TODO: Add hotkey switching between tools (1-9 keys)
        // For now, cycle tools with Tab key
        if (_inputManager.IsSwitchToolbarPressed())
        {
            CycleTools();
        }
    }
    
    private Vector2 CalculateToolTargetPosition()
    {
        // Calculate position one tile away from player in facing direction
        Vector2 offset = _player.Facing switch
        {
            Direction.Up => new Vector2(0, -GameConstants.TILE_SIZE),
            Direction.Down => new Vector2(0, GameConstants.TILE_SIZE),
            Direction.Left => new Vector2(-GameConstants.TILE_SIZE, 0),
            Direction.Right => new Vector2(GameConstants.TILE_SIZE, 0),
            _ => Vector2.Zero
        };
        
        return _player.Position + offset;
    }
    
    private void CycleTools()
    {
        // Cycle through basic tools
        Tool currentTool = _toolManager.GetCurrentTool();
        
        if (currentTool is Hoe)
        {
            _toolManager.SetCurrentTool(new WateringCan());
        }
        else if (currentTool is WateringCan)
        {
            _toolManager.SetCurrentTool(new Scythe());
        }
        else if (currentTool is Scythe)
        {
            _toolManager.SetCurrentTool(new Pickaxe());
        }
        else if (currentTool is Pickaxe)
        {
            _toolManager.SetCurrentTool(new Axe());
        }
        else
        {
            _toolManager.SetCurrentTool(new Hoe());
        }
    }
    
    private void HandleSeedPlantingInput()
    {
        // Check for interact key (X key) to plant seeds
        if (_inputManager.IsDoActionPressed())
        {
            // Calculate position in front of player
            Vector2 plantPosition = CalculateToolTargetPosition();
            
            // Try to plant a seed
            _seedManager.TryPlantSeed(plantPosition);
        }
    }
    
    private void HandleConsumableInput()
    {
        // Check for hotbar key presses (1-9, 0, -, =)
        int hotbarIndex = _inputManager.GetHotbarKeyPressed();
        
        if (hotbarIndex >= 0)
        {
            // Try to use the item in that hotbar slot
            _consumableManager.UseConsumableBySlot(hotbarIndex);
        }
        
        // TODO: Add visual/audio feedback for consumption
        // TODO: Add "can't eat/drink when full" message
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
