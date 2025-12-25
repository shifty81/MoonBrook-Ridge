using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Characters.Player;
using MoonBrookRidge.Characters.NPCs;
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
    private SaveSystem _saveSystem;
    private NPCManager _npcManager;
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
        // Spawn player in the center of the farm area
        // Farm area is defined in WorldMap.InitializeMap() as tiles 20-35 x 20-35
        // Center is at tile (27.5, 27.5)
        _player = new PlayerCharacter(new Vector2(27.5f * GameConstants.TILE_SIZE, 27.5f * GameConstants.TILE_SIZE));
        
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
        
        // Initialize NPC manager
        _npcManager = new NPCManager();
        
        // Initialize save system
        _saveSystem = new SaveSystem();
        
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
        
        // Load world map content with legacy textures (as fallback)
        _worldMap.LoadContent(tileTextures, cropTextures);
        
        // Load the Sunnyside World tileset as the primary tileset
        Texture2D sunnysideTileset = Game.Content.Load<Texture2D>("Textures/Tiles/sunnyside_tileset");
        _worldMap.LoadSunnysideTileset(sunnysideTileset);
        
        // Load building textures
        var buildings = new Dictionary<string, Texture2D>
        {
            ["House1"] = Game.Content.Load<Texture2D>("Textures/Buildings/House1"),
            ["House2"] = Game.Content.Load<Texture2D>("Textures/Buildings/House2"),
            ["House3_Yellow"] = Game.Content.Load<Texture2D>("Textures/Buildings/House3_Yellow"),
            ["Tower_Blue"] = Game.Content.Load<Texture2D>("Textures/Buildings/Tower_Blue"),
            ["Tower_Red"] = Game.Content.Load<Texture2D>("Textures/Buildings/Tower_Red"),
            ["Tower_Yellow"] = Game.Content.Load<Texture2D>("Textures/Buildings/Tower_Yellow"),
            ["Tower_Purple"] = Game.Content.Load<Texture2D>("Textures/Buildings/Tower_Purple"),
            ["Castle_Blue"] = Game.Content.Load<Texture2D>("Textures/Buildings/Castle_Blue"),
            ["Castle_Red"] = Game.Content.Load<Texture2D>("Textures/Buildings/Castle_Red"),
            ["Castle_Yellow"] = Game.Content.Load<Texture2D>("Textures/Buildings/Castle_Yellow"),
            ["Castle_Black"] = Game.Content.Load<Texture2D>("Textures/Buildings/Castle_Black"),
            ["Barracks_Red"] = Game.Content.Load<Texture2D>("Textures/Buildings/Barracks_Red"),
            ["Barracks_Blue"] = Game.Content.Load<Texture2D>("Textures/Buildings/Barracks_Blue"),
            ["Barracks_Yellow"] = Game.Content.Load<Texture2D>("Textures/Buildings/Barracks_Yellow"),
            ["Barracks_Purple"] = Game.Content.Load<Texture2D>("Textures/Buildings/Barracks_Purple"),
            ["Monastery_Blue"] = Game.Content.Load<Texture2D>("Textures/Buildings/Monastery_Blue"),
            ["Monastery_Red"] = Game.Content.Load<Texture2D>("Textures/Buildings/Monastery_Red"),
            ["Monastery_Yellow"] = Game.Content.Load<Texture2D>("Textures/Buildings/Monastery_Yellow"),
            ["Archery_Blue"] = Game.Content.Load<Texture2D>("Textures/Buildings/Archery_Blue"),
            ["Archery_Red"] = Game.Content.Load<Texture2D>("Textures/Buildings/Archery_Red"),
            ["Archery_Yellow"] = Game.Content.Load<Texture2D>("Textures/Buildings/Archery_Yellow")
        };
        
        // Load tree textures
        var trees = new Dictionary<string, Texture2D>
        {
            ["Tree1"] = Game.Content.Load<Texture2D>("Textures/Resources/Tree1"),
            ["Tree2"] = Game.Content.Load<Texture2D>("Textures/Resources/Tree2"),
            ["Tree3"] = Game.Content.Load<Texture2D>("Textures/Resources/Tree3"),
            ["Tree4"] = Game.Content.Load<Texture2D>("Textures/Resources/Tree4")
        };
        
        // Load rock textures
        var rocks = new Dictionary<string, Texture2D>
        {
            ["Rock1"] = Game.Content.Load<Texture2D>("Textures/Resources/Rock1"),
            ["Rock2"] = Game.Content.Load<Texture2D>("Textures/Resources/Rock2"),
            ["Rock3"] = Game.Content.Load<Texture2D>("Textures/Resources/Rock3")
        };
        
        // Populate world with Sunnyside-style objects
        _worldMap.PopulateSunnysideWorldObjects(buildings, trees, rocks);
        
        // Plant some test crops to demonstrate the system
        _worldMap.PlantTestCrops();
        
        // Load NPC sprites (using player sprite as placeholder for now)
        var npcSprites = new Dictionary<string, Texture2D>
        {
            ["farmer"] = animations["idle"] // Use idle animation sprite as NPC sprite
        };
        _npcManager.LoadContent(npcSprites);
        
        // Create a test NPC
        CreateTestNPC();
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
        
        // Check for quick save/load (F5/F9)
        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.F5))
        {
            QuickSave();
        }
        if (keyboardState.IsKeyDown(Keys.F9))
        {
            QuickLoad();
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
        
        // Update NPCs
        _npcManager.Update(gameTime, _timeSystem, _player.Position, _inputManager.IsDoActionPressed());
        
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
    
    /// <summary>
    /// Quick save the game (F5 key)
    /// </summary>
    public void QuickSave()
    {
        var saveData = CreateSaveData("quicksave");
        if (_saveSystem.SaveGame("quicksave", saveData))
        {
            System.Diagnostics.Debug.WriteLine("Quick save successful!");
        }
    }
    
    /// <summary>
    /// Quick load the game (F9 key)
    /// </summary>
    public void QuickLoad()
    {
        var saveData = _saveSystem.LoadGame("quicksave");
        if (saveData != null)
        {
            LoadSaveData(saveData);
            System.Diagnostics.Debug.WriteLine("Quick load successful!");
        }
    }
    
    private GameSaveData CreateSaveData(string saveName)
    {
        var saveData = new GameSaveData
        {
            SaveName = saveName,
            SaveTime = System.DateTime.Now
        };
        
        // Save player data
        saveData.Player = new PlayerSaveData
        {
            PositionX = _player.Position.X,
            PositionY = _player.Position.Y,
            Health = _player.Health,
            MaxHealth = _player.MaxHealth,
            Energy = _player.Energy,
            MaxEnergy = _player.MaxEnergy,
            Hunger = _player.Hunger,
            Thirst = _player.Thirst,
            Money = _player.Money
        };
        
        // Save time data
        saveData.Time = new TimeSaveData
        {
            TimeOfDay = _timeSystem.TimeOfDay,
            Day = _timeSystem.Day,
            Season = (int)_timeSystem.CurrentSeason,
            Year = _timeSystem.Year
        };
        
        // Save inventory data
        var slots = _inventory.GetSlots();
        saveData.Inventory = new InventorySaveData
        {
            Slots = new InventorySlotData[slots.Count]
        };
        
        for (int i = 0; i < slots.Count; i++)
        {
            var slot = slots[i];
            saveData.Inventory.Slots[i] = new InventorySlotData
            {
                ItemName = slot.Item?.Name ?? "",
                ItemType = slot.Item?.Type.ToString() ?? "",
                Quantity = slot.Quantity
            };
        }
        
        // Note: World data (crops) not saved yet - would need WorldMap API changes
        saveData.World = new WorldSaveData { Crops = System.Array.Empty<CropSaveData>() };
        
        return saveData;
    }
    
    private void LoadSaveData(GameSaveData saveData)
    {
        // Load player data
        _player.SetPosition(new Vector2(saveData.Player.PositionX, saveData.Player.PositionY));
        _player.SetHealth(saveData.Player.Health);
        _player.SetEnergy(saveData.Player.Energy);
        _player.SetHunger(saveData.Player.Hunger);
        _player.SetThirst(saveData.Player.Thirst);
        _player.SetMoney(saveData.Player.Money);
        
        // Load time data (would need TimeSystem API to set these)
        // For now, just log that we would load it
        System.Diagnostics.Debug.WriteLine($"Would load time: Day {saveData.Time.Day}, Season {saveData.Time.Season}");
        
        // Load inventory (simplified - just clear and add items back)
        // Full implementation would recreate exact item types
        System.Diagnostics.Debug.WriteLine($"Would load {saveData.Inventory.Slots.Length} inventory slots");
        
        // Note: Full load would also restore crops from World data
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Draw world with camera transform
        spriteBatch.Begin(transformMatrix: _camera.GetTransform(), 
                         samplerState: SamplerState.PointClamp);
        
        _worldMap.Draw(spriteBatch);
        _player.Draw(spriteBatch);
        _npcManager.Draw(spriteBatch, Game.DefaultFont);
        
        spriteBatch.End();
        
        // Draw HUD (no camera transform)
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _hud.DrawPlayerStats(spriteBatch, Game.DefaultFont, _player, _timeSystem);
        
        // Draw NPC UI (dialogue wheel)
        _npcManager.DrawUI(spriteBatch, Game.DefaultFont);
        
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
    
    private void CreateTestNPC()
    {
        // Create a test NPC named "Emma" the farmer
        var emma = new NPCCharacter("Emma", new Vector2(600, 400));
        
        // Add a daily schedule for Emma
        emma.Schedule.AddScheduleEntry(6.0f, new ScheduleLocation 
        { 
            Position = new Vector2(600, 400), 
            LocationName = "Home",
            Activity = "Waking up"
        });
        emma.Schedule.AddScheduleEntry(9.0f, new ScheduleLocation 
        { 
            Position = new Vector2(700, 300), 
            LocationName = "Town Square",
            Activity = "Shopping"
        });
        emma.Schedule.AddScheduleEntry(14.0f, new ScheduleLocation 
        { 
            Position = new Vector2(500, 500), 
            LocationName = "Farm",
            Activity = "Working"
        });
        emma.Schedule.AddScheduleEntry(18.0f, new ScheduleLocation 
        { 
            Position = new Vector2(600, 400), 
            LocationName = "Home",
            Activity = "Relaxing"
        });
        
        // Create a simple greeting dialogue tree
        var greetingNode = new DialogueNode("Hello there! Welcome to MoonBrook Ridge!", "Emma");
        var option1Response = new DialogueNode("I'm Emma, I've been farming here for years. How can I help you?", "Emma");
        var option2Response = new DialogueNode("The weather has been great for crops lately!", "Emma");
        
        greetingNode.AddOption("Who are you?", option1Response);
        greetingNode.AddOption("How's the farm?", option2Response);
        
        var dialogueTree = new DialogueTree(greetingNode);
        emma.AddDialogueTree("greeting", dialogueTree);
        
        // Add Emma to the NPC manager
        _npcManager.AddNPC(emma);
    }
}
