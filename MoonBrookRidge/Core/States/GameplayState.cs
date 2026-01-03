using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using MoonBrookRidge.Characters.Player;
using MoonBrookRidge.Characters.NPCs;
using MoonBrookRidge.World.Maps;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.World.Fishing;
using MoonBrookRidge.World.Buildings;
using MoonBrookRidge.UI.HUD;
using MoonBrookRidge.UI.Menus;
using MoonBrookRidge.UI;
using MoonBrookRidge.Core.Systems;
using MoonBrookRidge.Farming.Tools;
using MoonBrookRidge.Items;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.Items.Crafting;
using MoonBrookRidge.Items.Shop;
using MoonBrookRidge.Quests;
using MoonBrookRidge.Magic;
using MoonBrookRidge.Skills;
using MoonBrookRidge.Pets;
using MoonBrookRidge.Combat;
using MoonBrookRidge.Dungeons;
using MoonBrookRidge.Factions;
using MoonBrookRidge.Characters;

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
    private EventSystem _eventSystem;
    private EventNotification _eventNotification;
    private WeatherSystem _weatherSystem;
    private ParticleSystem _particleSystem;
    private Camera2D _camera;
    private InputManager _inputManager;
    private ToolManager _toolManager;
    private CollisionSystem _collisionSystem;
    private InventorySystem _inventory;
    private ConsumableManager _consumableManager;
    private SeedManager _seedManager;
    private SaveSystem _saveSystem;
    private NPCManager _npcManager;
    private CraftingSystem _craftingSystem;
    private CraftingMenu _craftingMenu;
    private ShopSystem _shopSystem;
    private ShopMenu _shopMenu;
    private GiftMenu _giftMenu;
    private QuestSystem _questSystem;
    private QuestMenu _questMenu;
    private MoonBrookRidge.World.MiningManager _miningManager;
    private FishingManager _fishingManager;
    private BuildingManager _buildingManager;
    private BuildingMenu _buildingMenu;
    private AchievementSystem _achievementSystem;
    private AchievementNotification _achievementNotification;
    private AchievementMenu _achievementMenu;
    private SettingsMenu _settingsMenu;
    // Phase 6 Systems
    private MagicSystem _magicSystem;
    private MagicMenu _magicMenu;
    private AlchemySystem _alchemySystem;
    private AlchemyMenu _alchemyMenu;
    private SkillTreeSystem _skillSystem;
    private SkillsMenu _skillsMenu;
    private PetSystem _petSystem;
    private PetMenu _petMenu;
    private List<WildPet> _wildPets;
    private CombatSystem _combatSystem;
    private ProjectileSystem _projectileSystem;
    private Dungeons.DungeonSystem _dungeonSystem;
    private DungeonMenu _dungeonMenu;
    private Factions.FactionSystem _factionSystem;
    private FactionMenu _factionMenu;
    private Biomes.BiomeSystem _biomeSystem;
    // Marriage and Family System (Phase 5 deferred item)
    private MarriageSystem _marriageSystem;
    private FamilySystem _familySystem;
    private MarriageProposalMenu _marriageProposalMenu;
    private FamilyMenu _familyMenu;
    // Phase 7 Systems - Performance & QoL
    private PerformanceMonitor _performanceMonitor;
    private AutoSaveSystem _autoSaveSystem;
    private Minimap _minimap;
    private NotificationSystem _notificationSystem;
    private ToolHotkeyManager _toolHotkeyManager;
    // Unified Player Menu - consolidates all character-related menus into one tabbed interface
    private UnifiedPlayerMenu _unifiedPlayerMenu;
    // Shared 1x1 white pixel texture for UI rendering - prevents memory leaks from creating new textures each frame
    private Texture2D _pixelTexture;
    private bool _isPaused;
    private KeyboardState _previousKeyboardState;
    private MouseState _previousMouseState;
    private GameEvent? _lastShownEvent;
    private int _previousDay; // Track day changes for marriage/family system

    public GameplayState(Game1 game) : base(game) { }

    public override void Initialize()
    {
        base.Initialize();
        
        // Initialize core systems
        _timeSystem = new TimeSystem();
        _eventSystem = new EventSystem(_timeSystem);
        _eventNotification = new EventNotification();
        _weatherSystem = new WeatherSystem(_timeSystem);
        _particleSystem = new ParticleSystem();
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
        
        // Add starting building materials for testing
        _inventory.AddItem(MineralFactory.GetMineralItem("stone"), 100);
        _inventory.AddItem(new Item("Wood", ItemType.Crafting), 150);
        _inventory.AddItem(MineralFactory.GetMineralItem("copper ore"), 20);
        _inventory.AddItem(MineralFactory.GetMineralItem("iron ore"), 50);
        
        // Give player starting money
        _player.SetMoney(10000);
        
        // Initialize tool manager with inventory
        _toolManager = new ToolManager(_worldMap, _player, _inventory);
        
        // Initialize seed manager
        _seedManager = new SeedManager(_inventory, _toolManager, _player);
        
        // Give player starting tools
        _toolManager.SetCurrentTool(new Hoe());
        
        // Initialize UI
        _hud = new HUDManager();
        
        // Initialize crafting system
        _craftingSystem = new CraftingSystem();
        _craftingMenu = new CraftingMenu(_craftingSystem, _inventory);
        
        // Initialize shop system
        _shopSystem = new ShopSystem();
        _shopMenu = new ShopMenu(_shopSystem, _inventory, _player);
        
        // Initialize gift menu
        _giftMenu = new GiftMenu(_inventory);
        
        // Initialize quest system
        _questSystem = new QuestSystem();
        _questMenu = new QuestMenu(_questSystem);
        
        // Initialize building system
        _buildingManager = new BuildingManager();
        _buildingMenu = new BuildingMenu(_buildingManager, _inventory);
        
        // Initialize NPC manager
        _npcManager = new NPCManager();
        
        // Initialize achievement system from Game1
        _achievementSystem = Game.AchievementSystem;
        _achievementNotification = new AchievementNotification();
        _achievementMenu = new AchievementMenu(_achievementSystem);
        
        // Subscribe to achievement unlocks
        _achievementSystem.OnAchievementUnlocked += (achievement) =>
        {
            _achievementNotification.ShowNotification(achievement);
        };
        
        // Initialize settings menu (Phase 7.4 - now includes display & UI scale settings)
        _settingsMenu = new SettingsMenu(Game.AudioManager, (Game1)Game);
        
        // Initialize Phase 6 systems
        _magicSystem = new MagicSystem(100f); // Start with 100 max mana
        _magicMenu = new MagicMenu(_magicSystem);
        
        _alchemySystem = new AlchemySystem();
        _alchemyMenu = new AlchemyMenu(_alchemySystem, _inventory);
        
        _skillSystem = new SkillTreeSystem();
        _skillsMenu = new SkillsMenu(_skillSystem);
        
        _petSystem = new PetSystem();
        _petMenu = new PetMenu(_petSystem);
        
        // Initialize wild pets for taming
        _wildPets = new List<WildPet>();
        SpawnWildPets();
        
        // Initialize Combat System
        _combatSystem = new CombatSystem();
        
        // Initialize Projectile System (Phase 7.4)
        _projectileSystem = new ProjectileSystem();
        
        // Initialize Dungeon System
        _dungeonSystem = new Dungeons.DungeonSystem();
        
        // Initialize Faction System
        _factionSystem = new Factions.FactionSystem();
        
        // Initialize Biome System
        _biomeSystem = new Biomes.BiomeSystem();
        
        // Initialize Marriage and Family System (Phase 5 deferred item)
        _marriageSystem = new MarriageSystem();
        _familySystem = new FamilySystem(_marriageSystem);
        _marriageProposalMenu = new MarriageProposalMenu(_marriageSystem);
        _familyMenu = new FamilyMenu(_marriageSystem, _familySystem);
        
        // Initialize Unified Player Menu - consolidates all character-related menus
        _unifiedPlayerMenu = new UnifiedPlayerMenu(
            _inventory,
            _skillSystem,
            _craftingSystem,
            _marriageSystem,
            _familySystem,
            _questSystem,
            _achievementSystem,
            _magicSystem,
            _alchemySystem,
            _petSystem,
            _factionSystem
        );
        
        // Hook up marriage events
        _marriageSystem.OnMarried += (spouse) =>
        {
            System.Console.WriteLine($"Congratulations! You married {spouse.Name}!");
            // Marriage achievement can be tracked manually if needed
        };
        
        _marriageSystem.OnProposalAccepted += (npc) =>
        {
            System.Console.WriteLine($"{npc.Name} accepted your proposal!");
        };
        
        _marriageSystem.OnProposalRejected += (npc) =>
        {
            System.Console.WriteLine($"{npc.Name} rejected your proposal. You need 10 hearts!");
        };
        
        // Hook up family events
        _familySystem.OnChildBorn += (child) =>
        {
            System.Console.WriteLine($"A new child was born: {child.Name}!");
            // Child achievement can be tracked manually if needed
        };
        
        _familySystem.OnChildGrowth += (child, stage) =>
        {
            System.Console.WriteLine($"{child.Name} has grown to {stage} stage!");
        };
        
        // Hook up marriage proposal menu events
        _marriageProposalMenu.OnProposalDecision += (npc, accepted) =>
        {
            if (accepted)
            {
                // Schedule wedding ceremony (happens immediately for now)
                _marriageSystem.MarryNPC(npc, _timeSystem.Day, (int)_timeSystem.CurrentSeason, _timeSystem.Year);
            }
        };
        
        // Hook up faction events
        _factionSystem.OnReputationLevelChanged += (faction, level) =>
        {
            System.Console.WriteLine($"Reputation with {faction.Name} reached {level}!");
        };
        
        _factionSystem.OnRewardUnlocked += (faction, reward) =>
        {
            System.Console.WriteLine($"Unlocked {reward.Name} from {faction.Name}!");
        };
        
        // Hook up quest choice system to faction system
        _questSystem.OnChoiceMade += (choice, consequence) =>
        {
            // Apply faction reputation changes from choice
            foreach (var factionChange in consequence.FactionChanges)
            {
                _factionSystem.ChangeReputation(factionChange.Key, factionChange.Value);
            }
        };
        
        // Hook up quest progress notifications
        _questSystem.OnObjectiveUpdated += (quest, objective) =>
        {
            string message = $"{quest.Title}: {objective.Description} ({objective.CurrentProgress}/{objective.RequiredProgress})";
            _notificationSystem?.Show(message, NotificationType.Quest, 3.0f);
        };
        
        _questSystem.OnQuestCompleted += (quest) =>
        {
            string message = $"Quest Complete: {quest.Title}";
            _notificationSystem?.Show(message, NotificationType.Success, 4.0f);
        };
        
        // Hook up dungeon events
        _dungeonSystem.OnDungeonEntered += (dungeon) =>
        {
            // Clear any existing enemies by killing them all
            var enemies = _combatSystem.GetActiveEnemies();
            foreach (var enemy in enemies.ToList())
            {
                enemy.TakeDamage(enemy.Health); // Kill all enemies
            }
            
            // Enter first room
            var firstRoom = dungeon.GetCurrentFloor()[0];
            _dungeonSystem.EnterRoom(firstRoom);
        };
        
        _dungeonSystem.OnRoomEntered += (room) =>
        {
            // Spawn room enemies
            foreach (var enemy in room.Enemies)
            {
                if (!enemy.IsDead)
                {
                    _combatSystem.SpawnEnemy(enemy);
                }
            }
        };
        
        _dungeonSystem.OnDungeonCleared += (dungeon) =>
        {
            // Reward player for clearing dungeon
            int reward = 1000 * dungeon.Difficulty;
            _player.AddMoney(reward);
        };
        
        // Hook up combat events
        _combatSystem.OnEnemyDefeated += (enemy) =>
        {
            // Award XP to skill system
            _skillSystem.AddExperience(SkillCategory.Combat, enemy.Experience);
            
            // Drop loot
            var random = new System.Random();
            foreach (var loot in enemy.LootTable)
            {
                if (random.NextDouble() <= loot.DropChance)
                {
                    int quantity = random.Next(loot.MinQuantity, loot.MaxQuantity + 1);
                    var item = new Item(loot.ItemName, ItemType.Crafting);
                    _inventory.AddItem(item, quantity);
                }
            }
            
            // Add money if it's a coin drop
            _player.AddMoney(50); // Base coin drop
        };
        
        _combatSystem.OnPlayerDamaged += (damage) =>
        {
            _player.ModifyHealth(-damage);
        };
        
        // Give player some starter spells for testing
        _magicSystem.LearnSpell("heal");
        _magicSystem.LearnSpell("light");
        
        // Hook up spell effects
        _magicSystem.OnSpellCast += (spell) =>
        {
            // Apply spell effects based on type
            if (spell.Id == "heal")
            {
                _player.ModifyHealth(spell.EffectValue);
            }
            else if (spell.Id == "growth")
            {
                // Grow crops in 3x3 area around player
                Vector2 playerTile = new Vector2(
                    (int)(_player.Position.X / GameConstants.TILE_SIZE),
                    (int)(_player.Position.Y / GameConstants.TILE_SIZE)
                );
                _worldMap.GrowCropsInArea(playerTile, 1); // 1 tile radius = 3x3
            }
            else if (spell.Id == "water")
            {
                // Water all crops on the farm
                _worldMap.WaterAllCrops();
            }
            // Other spells (speed, light, fireball, teleport, summon) will be implemented
            // when their required systems are added (buff system, lighting, combat, etc.)
            
            // Track spell casting for quests
            foreach (var quest in _questSystem.GetActiveQuests())
            {
                foreach (var objective in quest.Objectives)
                {
                    if (objective.Type == QuestObjectiveType.CastSpell && !objective.IsCompleted)
                    {
                        _questSystem.UpdateQuestProgress(quest.Id, objective.Id, 1);
                    }
                }
            }
        };
        
        // Hook up quest tracking events for Phase 6 objectives
        
        // Track dungeon entry
        _dungeonSystem.OnDungeonEntered += (dungeon) =>
        {
            // Update all active quests that require entering this dungeon type
            foreach (var quest in _questSystem.GetActiveQuests())
            {
                foreach (var objective in quest.Objectives)
                {
                    if (objective.Type == QuestObjectiveType.EnterDungeon && 
                        objective.TargetId == dungeon.Type.ToString() && 
                        !objective.IsCompleted)
                    {
                        _questSystem.UpdateQuestProgress(quest.Id, objective.Id, 1);
                    }
                }
            }
        };
        
        // Track room clearing
        _dungeonSystem.OnRoomCleared += (room) =>
        {
            // Update all active quests that require clearing rooms
            foreach (var quest in _questSystem.GetActiveQuests())
            {
                foreach (var objective in quest.Objectives)
                {
                    if (objective.Type == QuestObjectiveType.ClearRooms && !objective.IsCompleted)
                    {
                        _questSystem.UpdateQuestProgress(quest.Id, objective.Id, 1);
                    }
                }
            }
        };
        
        // Track dungeon completion
        _dungeonSystem.OnDungeonCleared += (dungeon) =>
        {
            // Update all active quests that require completing a dungeon
            foreach (var quest in _questSystem.GetActiveQuests())
            {
                foreach (var objective in quest.Objectives)
                {
                    if (objective.Type == QuestObjectiveType.CompleteDungeon && !objective.IsCompleted)
                    {
                        _questSystem.UpdateQuestProgress(quest.Id, objective.Id, 1);
                    }
                }
            }
        };
        
        // Track pet taming
        _petSystem.OnPetTamed += (pet) =>
        {
            // Update all active quests that require taming a pet
            foreach (var quest in _questSystem.GetActiveQuests())
            {
                foreach (var objective in quest.Objectives)
                {
                    if (objective.Type == QuestObjectiveType.TamePet && !objective.IsCompleted)
                    {
                        _questSystem.UpdateQuestProgress(quest.Id, objective.Id, 1);
                    }
                }
            }
        };
        
        // Track skill level ups
        _skillSystem.OnSkillLevelUp += (category, level) =>
        {
            // Update all active quests that require reaching a skill level
            foreach (var quest in _questSystem.GetActiveQuests())
            {
                foreach (var objective in quest.Objectives)
                {
                    if (objective.Type == QuestObjectiveType.ReachSkillLevel && !objective.IsCompleted)
                    {
                        // Check if this is the right category and level
                        if (objective.TargetId == category.ToString() && level >= objective.RequiredProgress)
                        {
                            objective.CurrentProgress = level;
                            objective.IsCompleted = true;
                        }
                    }
                }
            }
        };
        
        // Track skill unlocking
        _skillSystem.OnSkillUnlocked += (skill) =>
        {
            // Update all active quests that require unlocking a skill
            foreach (var quest in _questSystem.GetActiveQuests())
            {
                foreach (var objective in quest.Objectives)
                {
                    if (objective.Type == QuestObjectiveType.UnlockSkill && !objective.IsCompleted)
                    {
                        // For now, we track any skill unlock (can be enhanced later to track specific categories)
                        if (objective.TargetId == "Any")
                        {
                            _questSystem.UpdateQuestProgress(quest.Id, objective.Id, 1);
                        }
                    }
                }
            }
        };
        
        // Track spell learning
        _magicSystem.OnSpellLearned += (spell) =>
        {
            // Update all active quests that require learning a spell
            foreach (var quest in _questSystem.GetActiveQuests())
            {
                foreach (var objective in quest.Objectives)
                {
                    if (objective.Type == QuestObjectiveType.LearnSpell && !objective.IsCompleted)
                    {
                        _questSystem.UpdateQuestProgress(quest.Id, objective.Id, 1);
                    }
                }
            }
        };
        
        // Initialize save system
        _saveSystem = new SaveSystem();
        
        // Initialize Phase 7 systems - Performance & QoL
        _performanceMonitor = new PerformanceMonitor();
        _performanceMonitor.IsVisible = false; // Hidden by default, toggle with F3
        
        _autoSaveSystem = new AutoSaveSystem(5.0); // Auto-save every 5 minutes
        _autoSaveSystem.SetAutoSaveCallback(() =>
        {
            // Perform auto-save
            var saveData = CreateSaveData("autosave");
            if (_saveSystem.SaveGame("autosave", saveData))
            {
                _notificationSystem?.Show("Game Auto-Saved", NotificationType.Success, 2.0f);
            }
        });
        
        _minimap = new Minimap(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
        _minimap.IsVisible = true; // Visible by default
        
        _notificationSystem = new NotificationSystem(Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
        
        _toolHotkeyManager = new ToolHotkeyManager();
        _toolHotkeyManager.SetToolSelectedCallback((tool) =>
        {
            _toolManager.SetCurrentTool(tool);
            _notificationSystem?.Show($"Tool: {tool.Name}", NotificationType.Info, 1.5f);
        });
        
        // Register tools with hotkeys (1-6)
        _toolHotkeyManager.RegisterTool(Keys.D1, new Hoe());
        _toolHotkeyManager.RegisterTool(Keys.D2, new WateringCan());
        _toolHotkeyManager.RegisterTool(Keys.D3, new Scythe());
        _toolHotkeyManager.RegisterTool(Keys.D4, new Pickaxe());
        _toolHotkeyManager.RegisterTool(Keys.D5, new Axe());
        _toolHotkeyManager.RegisterTool(Keys.D6, new FishingRod());
        
        _isPaused = false;
        _previousDay = _timeSystem.Day; // Initialize day tracking for marriage/family
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
        
        // Load tree texture sheets and extract individual tree sprites
        // Tree1 & Tree2: 1536x256 = 6 trees of 256x256 each
        // Tree3 & Tree4: 1536x192 = 8 trees of 192x192 each
        var treeSprites = new Dictionary<string, SpriteInfo>();
        
        var tree1Sheet = Game.Content.Load<Texture2D>("Textures/Resources/Tree1");
        var tree1Extracted = SpriteSheetExtractor.ExtractSpritesFromHorizontalStrip(tree1Sheet, "Tree1_", 256, 256);
        foreach (var kvp in tree1Extracted) treeSprites[kvp.Key] = kvp.Value;
        
        var tree2Sheet = Game.Content.Load<Texture2D>("Textures/Resources/Tree2");
        var tree2Extracted = SpriteSheetExtractor.ExtractSpritesFromHorizontalStrip(tree2Sheet, "Tree2_", 256, 256);
        foreach (var kvp in tree2Extracted) treeSprites[kvp.Key] = kvp.Value;
        
        var tree3Sheet = Game.Content.Load<Texture2D>("Textures/Resources/Tree3");
        var tree3Extracted = SpriteSheetExtractor.ExtractSpritesFromHorizontalStrip(tree3Sheet, "Tree3_", 192, 192);
        foreach (var kvp in tree3Extracted) treeSprites[kvp.Key] = kvp.Value;
        
        var tree4Sheet = Game.Content.Load<Texture2D>("Textures/Resources/Tree4");
        var tree4Extracted = SpriteSheetExtractor.ExtractSpritesFromHorizontalStrip(tree4Sheet, "Tree4_", 192, 192);
        foreach (var kvp in tree4Extracted) treeSprites[kvp.Key] = kvp.Value;
        
        // Load rock texture sheets and extract individual rock sprites
        // Rocks are 128x128, likely containing 2x2 grid = 4 rocks of 64x64 each
        var rockSprites = new Dictionary<string, SpriteInfo>();
        
        var rock1Sheet = Game.Content.Load<Texture2D>("Textures/Resources/Rock1");
        var rock1Extracted = SpriteSheetExtractor.ExtractSpritesFromGrid(rock1Sheet, 64, 64);
        foreach (var kvp in rock1Extracted) rockSprites[$"Rock1_{kvp.Key}"] = kvp.Value;
        
        var rock2Sheet = Game.Content.Load<Texture2D>("Textures/Resources/Rock2");
        var rock2Extracted = SpriteSheetExtractor.ExtractSpritesFromGrid(rock2Sheet, 64, 64);
        foreach (var kvp in rock2Extracted) rockSprites[$"Rock2_{kvp.Key}"] = kvp.Value;
        
        var rock3Sheet = Game.Content.Load<Texture2D>("Textures/Resources/Rock3");
        var rock3Extracted = SpriteSheetExtractor.ExtractSpritesFromGrid(rock3Sheet, 64, 64);
        foreach (var kvp in rock3Extracted) rockSprites[$"Rock3_{kvp.Key}"] = kvp.Value;
        
        // Initialize mining manager with rock sprites
        var rockSpriteList = new List<SpriteInfo>(rockSprites.Values);
        _miningManager = new MoonBrookRidge.World.MiningManager(
            rock1Sheet, // Fallback texture if sprites aren't available
            rockSpriteList,
            Game.Content.Load<Texture2D>("Textures/Tiles/dirt_01"),
            Game.Content.Load<Texture2D>("Textures/Tiles/stone_01")
        );
        
        // Initialize fishing manager
        _fishingManager = new FishingManager();
        
        // Link managers to tool manager
        _toolManager.SetMiningManager(_miningManager);
        _toolManager.SetFishingManager(_fishingManager);
        
        // Populate world with Sunnyside-style objects using extracted sprites
        _worldMap.PopulateSunnysideWorldObjects(buildings, treeSprites, rockSprites);
        
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
        
        // Create shared pixel texture for UI rendering
        _pixelTexture = new Texture2D(Game.GraphicsDevice, 1, 1);
        _pixelTexture.SetData(new[] { Color.White });
        
        // Load achievement and settings UI
        _achievementMenu.LoadContent(Game.DefaultFont, _pixelTexture);
        _achievementNotification.LoadContent(Game.DefaultFont, _pixelTexture);
        _settingsMenu.LoadContent(Game.DefaultFont, _pixelTexture);
        
        // Initialize dungeon menu
        _dungeonMenu = new DungeonMenu(Game.DefaultFont, _pixelTexture, _dungeonSystem);
        
        // Initialize faction menu
        _factionMenu = new FactionMenu(Game.DefaultFont, _pixelTexture, _factionSystem);
        
        // Initialize Phase 7 UI components
        _minimap.Initialize(Game.GraphicsDevice);
        _performanceMonitor.Initialize(_pixelTexture);
        _notificationSystem.Initialize(_pixelTexture);
        
        // Initialize starter quests
        InitializeQuests();
    }

    public override void Update(GameTime gameTime)
    {
        // Track update time for performance monitoring
        var updateStartTime = DateTime.Now;
        
        // Update input first
        _inputManager.Update();
        
        // Get keyboard and mouse state
        var keyboardState = Keyboard.GetState();
        var mouseState = Mouse.GetState();
        
        // Update Phase 7 systems (always update)
        _performanceMonitor.Update(gameTime);
        _autoSaveSystem.Update(gameTime);
        _notificationSystem.Update(gameTime);
        _toolHotkeyManager.Update();
        
        // Check for performance monitor toggle (F3)
        if (keyboardState.IsKeyDown(Keys.F3) && !_previousKeyboardState.IsKeyDown(Keys.F3))
        {
            _performanceMonitor.IsVisible = !_performanceMonitor.IsVisible;
        }
        
        // Check for minimap toggle (Tab key - remapped from tool switching)
        if (keyboardState.IsKeyDown(Keys.Tab) && !_previousKeyboardState.IsKeyDown(Keys.Tab))
        {
            _minimap.IsVisible = !_minimap.IsVisible;
        }
        
        // Check for quick save/load (F5/F9) - using keyboard state
        if (keyboardState.IsKeyDown(Keys.F5) && !_previousKeyboardState.IsKeyDown(Keys.F5))
        {
            QuickSave();
            _autoSaveSystem.ResetTimer(); // Reset auto-save timer after manual save
            _notificationSystem.Show("Game Saved", NotificationType.Success, 2.0f);
        }
        if (keyboardState.IsKeyDown(Keys.F9) && !_previousKeyboardState.IsKeyDown(Keys.F9))
        {
            QuickLoad();
            _notificationSystem.Show("Game Loaded", NotificationType.Success, 2.0f);
        }
        
        // Update menus if active
        if (_craftingMenu.IsActive)
        {
            _craftingMenu.Update(gameTime);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in crafting menu
        }
        
        if (_shopMenu.IsActive)
        {
            _shopMenu.Update(gameTime);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in shop menu
        }
        
        if (_giftMenu.IsActive)
        {
            _giftMenu.Update(gameTime);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in gift menu
        }
        
        if (_questMenu.IsActive)
        {
            _questMenu.Update(gameTime);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in quest menu
        }
        
        if (_buildingMenu.IsActive)
        {
            _buildingMenu.Update(gameTime, _player.Money);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in building menu
        }
        
        if (_achievementMenu.IsVisible)
        {
            var keyState = Keyboard.GetState();
            _achievementMenu.Update(gameTime, keyState);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in achievement menu
        }
        
        if (_settingsMenu.IsVisible)
        {
            var keyState = Keyboard.GetState();
            _settingsMenu.Update(gameTime, keyState);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in settings menu
        }
        
        if (_magicMenu.IsActive)
        {
            _magicMenu.Update(gameTime);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in magic menu
        }
        
        if (_alchemyMenu.IsActive)
        {
            _alchemyMenu.Update(gameTime);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in alchemy menu
        }
        
        if (_skillsMenu.IsActive)
        {
            _skillsMenu.Update(gameTime);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in skills menu
        }
        
        if (_petMenu.IsActive)
        {
            _petMenu.Update(gameTime);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in pet menu
        }
        
        if (_dungeonMenu.IsOpen)
        {
            _dungeonMenu.Update(keyboardState, _previousKeyboardState, mouseState, _previousMouseState);
            _previousKeyboardState = keyboardState;
            _previousMouseState = mouseState;
            return; // Don't update game while in dungeon menu
        }
        
        if (_factionMenu.IsActive)
        {
            _factionMenu.Update(gameTime);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in faction menu
        }
        
        if (_marriageProposalMenu.IsActive)
        {
            _marriageProposalMenu.Update(gameTime);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in marriage proposal menu
        }
        
        if (_familyMenu.IsActive)
        {
            _familyMenu.Update(gameTime);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in family menu
        }
        
        // Check for unified player menu (E key) - with debouncing
        if (keyboardState.IsKeyDown(Keys.E) && !_previousKeyboardState.IsKeyDown(Keys.E))
        {
            _unifiedPlayerMenu.Toggle();
            _previousKeyboardState = keyboardState;
            return;
        }
        
        // Close unified player menu with ESC
        if (_unifiedPlayerMenu.IsActive && keyboardState.IsKeyDown(Keys.Escape) && !_previousKeyboardState.IsKeyDown(Keys.Escape))
        {
            _unifiedPlayerMenu.Hide();
            _previousKeyboardState = keyboardState;
            return;
        }
        
        // Update unified player menu if active
        if (_unifiedPlayerMenu.IsActive)
        {
            _unifiedPlayerMenu.Update(gameTime, keyboardState, mouseState);
            _previousKeyboardState = keyboardState;
            return; // Don't update game while in menu
        }
        
        // Context-specific keybinds (only available outside unified menu)
        
        // Check for shop menu (B key for "buy") - only when near a shop
        if (keyboardState.IsKeyDown(Keys.B) && !_previousKeyboardState.IsKeyDown(Keys.B))
        {
            _shopMenu.Show();
            _previousKeyboardState = keyboardState;
            return;
        }
        
        // Check for gift menu (G key) - Only available when near an NPC
        if (keyboardState.IsKeyDown(Keys.G) && !_previousKeyboardState.IsKeyDown(Keys.G))
        {
            var nearbyNPC = GetNearbyNPC();
            if (nearbyNPC != null)
            {
                _giftMenu.Show(nearbyNPC);
                _previousKeyboardState = keyboardState;
                return;
            }
        }
        
        // Check for taming wild pets (T key) - Only available when near a wild pet
        if (keyboardState.IsKeyDown(Keys.T) && !_previousKeyboardState.IsKeyDown(Keys.T))
        {
            TryTameWildPet();
            _previousKeyboardState = keyboardState;
            return;
        }
        
        // Check for marriage proposal menu (V key) - Only available when near an NPC with 10 hearts
        if (keyboardState.IsKeyDown(Keys.V) && !_previousKeyboardState.IsKeyDown(Keys.V))
        {
            var nearbyNPC = GetNearbyNPC();
            if (nearbyNPC != null && _marriageSystem.CanPropose(nearbyNPC))
            {
                _marriageProposalMenu.Show(nearbyNPC);
                _previousKeyboardState = keyboardState;
                return;
            }
        }
        
        // Check for settings menu (O key for "options")
        if (keyboardState.IsKeyDown(Keys.O) && !_previousKeyboardState.IsKeyDown(Keys.O))
        {
            _settingsMenu.IsVisible = true;
            _previousKeyboardState = keyboardState;
            return;
        }
        
        // Check for building menu (H key for "house/build")
        if (keyboardState.IsKeyDown(Keys.H) && !_previousKeyboardState.IsKeyDown(Keys.H))
        {
            _buildingMenu.Show();
            _previousKeyboardState = keyboardState;
            return;
        }
        
        // Check for map toggle (M key)
        if (keyboardState.IsKeyDown(Keys.M) && !_previousKeyboardState.IsKeyDown(Keys.M))
        {
            _minimap.IsVisible = !_minimap.IsVisible;
            _previousKeyboardState = keyboardState;
            return;
        }
        
        // Removed individual menu keybinds - all now consolidated in unified player menu (E key)
        // Removed: K (crafting), A (achievements), F (quests), L (alchemy), J (skills), P (pets), R (factions), Y (family)
        // These are now accessible via tabs in the unified menu
        
        // Check for pause
        if (_inputManager.IsOpenMenuPressed())
        {
            _isPaused = !_isPaused;
        }
        
        // Don't update game logic when paused
        if (_isPaused)
        {
            // Update pause menu here when implemented
            _previousKeyboardState = keyboardState;
            return;
        }
        
        // Handle building placement mode
        if (_buildingMenu.IsPlacementMode)
        {
            HandleBuildingPlacement();
            _previousKeyboardState = keyboardState;
            return; // Don't update other game logic during placement
        }
        
        // Update time system
        _timeSystem.Update(gameTime);
        
        // Check if day has changed and update marriage/family systems
        if (_timeSystem.Day != _previousDay)
        {
            _previousDay = _timeSystem.Day;
            _marriageSystem.Update();
            _familySystem.Update(_timeSystem.Day, (int)_timeSystem.CurrentSeason, _timeSystem.Year);
        }
        
        // Update weather system
        _weatherSystem.Update(gameTime);
        
        // Update particle system
        _particleSystem.Update(gameTime);
        
        // Update Phase 6 systems
        _magicSystem.Update(gameTime);
        _petSystem.Update(gameTime);
        _combatSystem.Update(gameTime);
        
        // Update Phase 7.4 systems
        _projectileSystem.Update(gameTime);
        
        // Check projectile collisions with enemies (Phase 7.4)
        CheckProjectileCollisions();
        
        // Update biome based on player position
        UpdateBiomeFromPosition();
        
        // Update wild pets
        UpdateWildPets(gameTime);
        
        // Update event system
        _eventSystem.Update(gameTime);
        
        // Check if a new event has been triggered and show notification
        if (_eventSystem.HasActiveEvent && _eventSystem.ActiveEvent != _lastShownEvent)
        {
            _eventNotification.Show(_eventSystem.ActiveEvent);
            _lastShownEvent = _eventSystem.ActiveEvent;
        }
        
        // Update event notification
        _eventNotification.Update(gameTime);
        
        // Update crop growth based on game time
        _worldMap.UpdateCropGrowth(_timeSystem.LastGameHoursElapsed);
        
        // Update world map
        _worldMap.Update(gameTime);
        
        // Handle tool usage input
        HandleToolInput();
        
        // Handle seed planting input
        HandleSeedPlantingInput();
        
        // Handle mine entrance/exit interaction
        HandleMineInteraction();
        
        // Handle dungeon entrance interaction
        HandleDungeonInteraction();
        
        // Handle combat interactions
        HandleCombat(gameTime);
        
        // Handle consumable usage input
        HandleConsumableInput();
        
        // Update player with input and collision
        _player.Update(gameTime, _inputManager, _collisionSystem, _biomeSystem);
        
        // Update NPCs
        _npcManager.Update(gameTime, _timeSystem, _player.Position, _inputManager.IsDoActionPressed());
        
        // Update fishing manager
        if (_fishingManager.IsFishing)
        {
            _fishingManager.Update(gameTime, _inventory);
        }
        
        // Update camera to follow player
        _camera.Follow(_player.Position);
        
        // Update HUD
        _hud.Update(gameTime, _player, _timeSystem);
        
        // Update achievement notification
        _achievementNotification.Update(gameTime);
        
        // Update minimap
        _minimap.Update(_worldMap, _player);
        
        // Check for sleep time (player exhaustion at 2 AM)
        if (_timeSystem.TimeOfDay >= 26f && _player.Energy < 10f)
        {
            ForceSleep();
        }
        
        // Record update time for performance monitoring
        var updateEndTime = DateTime.Now;
        _performanceMonitor.RecordUpdateTime((updateEndTime - updateStartTime).TotalMilliseconds);
        
        // Store keyboard state for next frame
        _previousKeyboardState = keyboardState;
    }
    
    private void HandleToolInput()
    {
        // Check for tool usage (C key)
        if (_inputManager.IsUseToolPressed())
        {
            // Special handling for fishing rod
            Tool currentTool = _toolManager.GetCurrentTool();
            if (currentTool is FishingRod && !_fishingManager.IsFishing)
            {
                // Check if near water and start fishing
                if (_fishingManager.IsNearWater(_player.Position, _worldMap.GetAllTiles()))
                {
                    string season = _timeSystem.CurrentSeason.ToString();
                    _fishingManager.StartFishing(season);
                    _player.Stats.ConsumeEnergy(currentTool.EnergyCost);
                    // Spawn fishing particles
                    _particleSystem.SpawnParticles(_player.Position, ParticleEffectType.Fish, 15);
                }
            }
            else
            {
                // Calculate tile position in front of player based on facing direction
                Vector2 toolPosition = CalculateToolTargetPosition();
                
                // Use the tool at that position
                _toolManager.UseTool(toolPosition, _player.Stats);
                
                // Spawn particles based on tool type
                SpawnToolParticles(currentTool, toolPosition);
            }
        }
        
        // Tool hotkey switching is handled by ToolHotkeyManager (1-6 keys)
        // Tab key now toggles minimap
    }
    
    private void SpawnToolParticles(Tool tool, Vector2 position)
    {
        switch (tool)
        {
            case Hoe:
                _particleSystem.SpawnParticles(position, ParticleEffectType.Dust, 12);
                break;
            case WateringCan:
                _particleSystem.SpawnParticles(position, ParticleEffectType.Water, 15);
                break;
            case Pickaxe:
                _particleSystem.SpawnParticles(position, ParticleEffectType.Rock, 20);
                break;
            case Axe:
                _particleSystem.SpawnParticles(position, ParticleEffectType.Wood, 18);
                break;
            case Scythe:
                _particleSystem.SpawnParticles(position, ParticleEffectType.Sparkle, 10);
                break;
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
    
    private void HandleMineInteraction()
    {
        // Check if player is standing on a mine entrance tile
        if (!_miningManager.InMine)
        {
            Vector2 gridPos = WorldToGridPosition(_player.Position);
            var tile = _worldMap.GetTile((int)gridPos.X, (int)gridPos.Y);
            
            if (tile != null && tile.Type == MoonBrookRidge.World.Tiles.TileType.MineEntrance)
            {
                // Check for interact key (X or Down) to enter mine
                if (_inputManager.IsDoActionPressed() || Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    // Enter the mine
                    Vector2 spawnPos = _miningManager.EnterMine(1);
                    _player.SetPosition(spawnPos);
                    
                    // Spawn enemies in the mine
                    SpawnEnemiesInMine(1);
                }
            }
        }
        else
        {
            // Player is in the mine - check for exit/descent/ascent
            
            // Check if near exit stairs (to go deeper)
            if (_miningManager.IsNearExit(_player.Position))
            {
                // Show prompt or check for Down key
                if (Keyboard.GetState().IsKeyDown(Keys.Down))
                {
                    Vector2 spawnPos = _miningManager.DescendLevel();
                    _player.SetPosition(spawnPos);
                    
                    // Spawn enemies for the new level
                    SpawnEnemiesInMine(_miningManager.CurrentLevel);
                }
            }
            
            // Check if near entrance stairs (to go up)
            if (_miningManager.IsNearEntrance(_player.Position))
            {
                // Show prompt or check for Up key
                if (Keyboard.GetState().IsKeyDown(Keys.Up))
                {
                    Vector2 spawnPos = _miningManager.AscendLevel();
                    
                    if (!_miningManager.InMine)
                    {
                        // Exited mine - return to mine entrance in overworld
                        // Clear all enemies
                        _combatSystem.GetActiveEnemies().Clear();
                        Vector2 entrancePixelPos = _worldMap.MineEntranceGridPosition * GameConstants.TILE_SIZE;
                        _player.SetPosition(entrancePixelPos);
                    }
                    else
                    {
                        _player.SetPosition(spawnPos);
                        // Spawn enemies for the level we ascended to
                        SpawnEnemiesInMine(_miningManager.CurrentLevel);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Handle dungeon entrance interaction
    /// </summary>
    private void HandleDungeonInteraction()
    {
        // Only check for dungeon entrances when not in mine or dungeon
        if (_miningManager.InMine || _dungeonSystem.ActiveDungeon != null)
            return;
        
        Vector2 gridPos = WorldToGridPosition(_player.Position);
        var tile = _worldMap.GetTile((int)gridPos.X, (int)gridPos.Y);
        
        if (tile == null) return;
        
        // Check if player is standing on a dungeon entrance
        DungeonType? dungeonType = GetDungeonTypeFromTile(tile.Type);
        
        if (dungeonType.HasValue)
        {
            // Check for interact key to enter dungeon
            if (_inputManager.IsDoActionPressed())
            {
                // Generate and enter dungeon
                int floors = 3; // Default 3 floors
                int difficulty = 1; // Default difficulty 1
                
                var dungeon = _dungeonSystem.GenerateDungeon(dungeonType.Value, floors, difficulty);
                _dungeonSystem.EnterDungeon(dungeon);
                
                // Move player to dungeon start position
                _player.SetPosition(new Vector2(400, 300));
                
                // Show dungeon entered message
                System.Console.WriteLine($"Entered {GetDungeonName(dungeonType.Value)}!");
            }
        }
    }
    
    /// <summary>
    /// Get dungeon type from tile type
    /// </summary>
    private DungeonType? GetDungeonTypeFromTile(TileType tileType)
    {
        return tileType switch
        {
            TileType.DungeonEntranceSlime => DungeonType.SlimeCave,
            TileType.DungeonEntranceSkeleton => DungeonType.SkeletonCrypt,
            TileType.DungeonEntranceSpider => DungeonType.SpiderNest,
            TileType.DungeonEntranceGoblin => DungeonType.GoblinWarrens,
            TileType.DungeonEntranceHaunted => DungeonType.HauntedManor,
            TileType.DungeonEntranceDragon => DungeonType.DragonLair,
            TileType.DungeonEntranceDemon => DungeonType.DemonRealm,
            TileType.DungeonEntranceRuins => DungeonType.AncientRuins,
            _ => null
        };
    }
    
    /// <summary>
    /// Get dungeon name from type
    /// </summary>
    private string GetDungeonName(DungeonType type)
    {
        return type switch
        {
            DungeonType.SlimeCave => "Slime Cave",
            DungeonType.SkeletonCrypt => "Skeleton Crypt",
            DungeonType.SpiderNest => "Spider Nest",
            DungeonType.GoblinWarrens => "Goblin Warrens",
            DungeonType.HauntedManor => "Haunted Manor",
            DungeonType.DragonLair => "Dragon Lair",
            DungeonType.DemonRealm => "Demon Realm",
            DungeonType.AncientRuins => "Ancient Ruins",
            _ => "Unknown Dungeon"
        };
    }
    
    /// <summary>
    /// Handle combat - enemy AI, attacks, and player attacking
    /// </summary>
    private void HandleCombat(GameTime gameTime)
    {
        // Combat active in mines or dungeons
        if (!_miningManager.InMine && _dungeonSystem.ActiveDungeon == null)
            return;
        
        var enemies = _combatSystem.GetActiveEnemies();
        
        // Enemy AI - move towards player and attack
        foreach (var enemy in enemies)
        {
            Vector2 direction = _player.Position - enemy.Position;
            float distance = direction.Length();
            
            if (distance > 0)
            {
                direction.Normalize();
                
                // Move towards player if not in attack range
                if (distance > 32f) // Attack range is 32 pixels (2 tiles)
                {
                    float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
                    enemy.Position += direction * enemy.Speed * deltaTime;
                }
                // Attack player if in range and cooldown ready
                else if (enemy.CanAttack())
                {
                    enemy.Attack();
                    _combatSystem.PlayerTakeDamage(enemy.Damage);
                }
            }
        }
        
        // Player attack input - Space key
        var keyboardState = Keyboard.GetState();
        if (keyboardState.IsKeyDown(Keys.Space) && !_previousKeyboardState.IsKeyDown(Keys.Space))
        {
            var weapon = _combatSystem.GetEquippedWeapon();
            if (weapon != null)
            {
                // Check energy/mana requirements
                bool canAttack = false;
                if (weapon.UsesMana)
                {
                    canAttack = _magicSystem.Mana >= weapon.EnergyCost;
                }
                else
                {
                    canAttack = _player.Energy >= weapon.EnergyCost;
                }
                
                if (canAttack)
                {
                    // Handle ranged weapons (spawn projectiles - Phase 7.4)
                    if (weapon.Type == WeaponType.Ranged)
                    {
                        // Calculate direction to nearest enemy or facing direction
                        Vector2 direction = Vector2.Zero;
                        Enemy nearestEnemy = null;
                        float nearestDistance = float.MaxValue;
                        
                        foreach (var enemy in enemies)
                        {
                            float distance = Vector2.Distance(_player.Position, enemy.Position);
                            if (distance < nearestDistance)
                            {
                                nearestEnemy = enemy;
                                nearestDistance = distance;
                            }
                        }
                        
                        // Aim at nearest enemy if within range, otherwise use facing direction
                        if (nearestEnemy != null && nearestDistance < 400f) // 400 pixel max range
                        {
                            direction = nearestEnemy.Position - _player.Position;
                            if (direction.Length() > 0)
                                direction.Normalize();
                        }
                        else
                        {
                            // Use player's facing direction
                            direction = _player.Facing switch
                            {
                                Direction.Up => new Vector2(0, -1),
                                Direction.Down => new Vector2(0, 1),
                                Direction.Left => new Vector2(-1, 0),
                                Direction.Right => new Vector2(1, 0),
                                _ => new Vector2(1, 0)
                            };
                        }
                        
                        // Determine projectile type based on weapon
                        ProjectileType projType = weapon.Id switch
                        {
                            "wooden_bow" => ProjectileType.Arrow,
                            "longbow" => ProjectileType.Arrow,
                            "crossbow" => ProjectileType.Bolt,
                            _ => ProjectileType.Arrow
                        };
                        
                        // Spawn projectile
                        float projectileSpeed = 300f; // pixels per second
                        Vector2 velocity = direction * projectileSpeed;
                        _projectileSystem.SpawnProjectile(
                            _player.Position,
                            velocity,
                            projType,
                            weapon.Damage,
                            3.0f, // 3 second lifetime
                            "player"
                        );
                        
                        // Consume energy
                        if (!weapon.UsesMana)
                        {
                            _player.Stats.ConsumeEnergy(weapon.EnergyCost);
                        }
                    }
                    // Handle melee weapons (instant hit)
                    else if (weapon.Type == WeaponType.Melee)
                    {
                        // Find nearest enemy in melee range
                        Enemy nearestEnemy = null;
                        float nearestDistance = float.MaxValue;
                        
                        foreach (var enemy in enemies)
                        {
                            float distance = Vector2.Distance(_player.Position, enemy.Position);
                            if (distance < 64f && distance < nearestDistance) // 64 pixel melee range (4 tiles)
                            {
                                nearestEnemy = enemy;
                                nearestDistance = distance;
                            }
                        }
                        
                        if (nearestEnemy != null)
                        {
                            // Consume energy
                            if (!weapon.UsesMana)
                            {
                                _player.Stats.ConsumeEnergy(weapon.EnergyCost);
                            }
                            _combatSystem.AttackEnemy(nearestEnemy);
                        }
                    }
                    // Handle magic weapons (spawn magic projectiles)
                    else if (weapon.Type == WeaponType.Magic)
                    {
                        // Calculate direction similar to ranged weapons
                        Vector2 direction = Vector2.Zero;
                        Enemy nearestEnemy = null;
                        float nearestDistance = float.MaxValue;
                        
                        foreach (var enemy in enemies)
                        {
                            float distance = Vector2.Distance(_player.Position, enemy.Position);
                            if (distance < nearestDistance)
                            {
                                nearestEnemy = enemy;
                                nearestDistance = distance;
                            }
                        }
                        
                        if (nearestEnemy != null && nearestDistance < 400f)
                        {
                            direction = nearestEnemy.Position - _player.Position;
                            if (direction.Length() > 0)
                                direction.Normalize();
                        }
                        else
                        {
                            // Use player's facing direction
                            direction = _player.Facing switch
                            {
                                Direction.Up => new Vector2(0, -1),
                                Direction.Down => new Vector2(0, 1),
                                Direction.Left => new Vector2(-1, 0),
                                Direction.Right => new Vector2(1, 0),
                                _ => new Vector2(1, 0)
                            };
                        }
                        
                        // Determine magic projectile type based on weapon
                        ProjectileType projType = weapon.Id switch
                        {
                            "fire_wand" => ProjectileType.Fireball,
                            "arcane_staff" => ProjectileType.MagicMissile,
                            _ => ProjectileType.MagicMissile
                        };
                        
                        // Spawn magic projectile
                        float projectileSpeed = 250f;
                        Vector2 velocity = direction * projectileSpeed;
                        _projectileSystem.SpawnProjectile(
                            _player.Position,
                            velocity,
                            projType,
                            weapon.Damage,
                            3.0f,
                            "player"
                        );
                        
                        // Consume mana
                        if (weapon.UsesMana)
                        {
                            _magicSystem.ConsumeMana(weapon.EnergyCost);
                        }
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Spawn enemies appropriate for the mine level
    /// </summary>
    private void SpawnEnemiesInMine(int level)
    {
        // Clear existing enemies
        var enemies = _combatSystem.GetActiveEnemies();
        enemies.Clear();
        
        // Calculate number of enemies based on level (3-8 enemies)
        var random = new System.Random();
        int enemyCount = 3 + (level - 1) + random.Next(3);
        
        // Spawn enemies in random positions around the mine
        for (int i = 0; i < enemyCount; i++)
        {
            // Random position offset from player spawn (200-400 pixels away)
            float angle = (float)(random.NextDouble() * Math.PI * 2);
            float distance = 200 + (float)(random.NextDouble() * 200);
            Vector2 offset = new Vector2((float)Math.Cos(angle) * distance, (float)Math.Sin(angle) * distance);
            Vector2 spawnPosition = _player.Position + offset;
            
            // Spawn different enemy types based on level
            Enemy enemy = null;
            if (level <= 2)
            {
                // Easy enemies
                int enemyType = random.Next(3);
                enemy = enemyType switch
                {
                    0 => EnemyFactory.CreateSlime(spawnPosition),
                    1 => EnemyFactory.CreateBat(spawnPosition),
                    _ => EnemyFactory.CreateGoblin(spawnPosition)
                };
            }
            else if (level <= 5)
            {
                // Medium enemies
                int enemyType = random.Next(4);
                enemy = enemyType switch
                {
                    0 => EnemyFactory.CreateSkeleton(spawnPosition),
                    1 => EnemyFactory.CreateSpider(spawnPosition),
                    2 => EnemyFactory.CreateWolf(spawnPosition),
                    _ => EnemyFactory.CreateGoblin(spawnPosition)
                };
            }
            else if (level <= 8)
            {
                // Hard enemies
                int enemyType = random.Next(4);
                enemy = enemyType switch
                {
                    0 => EnemyFactory.CreateGhost(spawnPosition),
                    1 => EnemyFactory.CreateZombie(spawnPosition),
                    2 => EnemyFactory.CreateOrc(spawnPosition),
                    _ => EnemyFactory.CreateSkeleton(spawnPosition)
                };
            }
            else
            {
                // Very hard enemies
                int enemyType = random.Next(3);
                enemy = enemyType switch
                {
                    0 => EnemyFactory.CreateFireElemental(spawnPosition),
                    1 => EnemyFactory.CreateDemon(spawnPosition),
                    _ => EnemyFactory.CreateOrc(spawnPosition)
                };
            }
            
            if (enemy != null)
            {
                _combatSystem.SpawnEnemy(enemy);
            }
        }
    }
    
    /// <summary>
    /// Convert world position to grid position
    /// </summary>
    private Vector2 WorldToGridPosition(Vector2 worldPosition)
    {
        return new Vector2(
            (int)(worldPosition.X / GameConstants.TILE_SIZE),
            (int)(worldPosition.Y / GameConstants.TILE_SIZE)
        );
    }
    
    private void HandleConsumableInput()
    {
        // Check for hotbar key presses (7-9, 0, -, =) for consumables
        // Note: 1-6 are now used for tool hotkeys
        int hotbarIndex = _inputManager.GetHotbarKeyPressed();
        
        if (hotbarIndex >= 6) // Only consumables in slots 7-12 (keys 7-9, 0, -, =)
        {
            var slots = _inventory.GetSlots();
            if (hotbarIndex < slots.Count && !slots[hotbarIndex].IsEmpty)
            {
                var item = slots[hotbarIndex].Item;
                
                // Check if player stats are already full
                bool hungerFull = item is FoodItem && _player.Hunger >= 100f;
                bool thirstFull = item is DrinkItem && _player.Thirst >= 100f;
                
                if (hungerFull)
                {
                    _notificationSystem.Show("Can't eat - hunger is full!", NotificationType.Warning, 2.0f);
                }
                else if (thirstFull)
                {
                    _notificationSystem.Show("Can't drink - thirst is full!", NotificationType.Warning, 2.0f);
                }
                else
                {
                    // Try to use the item
                    bool success = _consumableManager.UseConsumableBySlot(hotbarIndex);
                    if (success)
                    {
                        // Visual feedback - spawn sparkle particles
                        _particleSystem.SpawnParticles(_player.Position, ParticleEffectType.Sparkle, 8);
                        
                        // Notification feedback
                        string message = item is FoodItem ? $"Ate {item.Name}" : $"Drank {item.Name}";
                        _notificationSystem.Show(message, NotificationType.Success, 1.5f);
                    }
                }
            }
        }
    }
    
    private void HandleBuildingPlacement()
    {
        var mouseState = Mouse.GetState();
        var keyboardState = Keyboard.GetState();
        
        // Get tile position under mouse cursor
        Vector2 mouseWorldPos = GetMouseWorldPosition();
        Vector2 tilePos = new Vector2(
            (int)(mouseWorldPos.X / GameConstants.TILE_SIZE),
            (int)(mouseWorldPos.Y / GameConstants.TILE_SIZE)
        );
        
        // Left click to place building
        if (mouseState.LeftButton == ButtonState.Pressed && 
            _previousMouseState.LeftButton == ButtonState.Released)
        {
            int newGold = _buildingManager.ConstructBuilding(
                _buildingMenu.SelectedBuildingType,
                tilePos,
                _inventory,
                _player.Money,
                _worldMap.GetAllTiles()
            );
            
            if (newGold >= 0)
            {
                _player.SetMoney(newGold);
                _buildingMenu.ExitPlacementMode();
            }
        }
        
        // Right click or Escape to cancel placement
        if ((mouseState.RightButton == ButtonState.Pressed && 
             _previousMouseState.RightButton == ButtonState.Released) ||
            (keyboardState.IsKeyDown(Keys.Escape) && 
             !_previousKeyboardState.IsKeyDown(Keys.Escape)))
        {
            _buildingMenu.ExitPlacementMode();
        }
        
        _previousMouseState = mouseState;
    }
    
    private Vector2 GetMouseWorldPosition()
    {
        var mouseState = Mouse.GetState();
        Vector2 mouseScreenPos = new Vector2(mouseState.X, mouseState.Y);
        
        // Convert screen position to world position accounting for camera
        Vector2 mouseWorldPos = mouseScreenPos / _camera.Zoom + _camera.Position;
        
        return mouseWorldPos;
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
        
        // Save Phase 6 systems data
        saveData.Magic = _magicSystem.ExportSaveData();
        saveData.Skills = _skillSystem.ExportSaveData();
        saveData.Pets = _petSystem.ExportSaveData();
        saveData.DungeonProgress = _dungeonSystem.ExportSaveData();
        
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
        
        // Load Phase 6 systems data
        _magicSystem.ImportSaveData(saveData.Magic);
        _skillSystem.ImportSaveData(saveData.Skills);
        _petSystem.ImportSaveData(saveData.Pets);
        _dungeonSystem.ImportSaveData(saveData.DungeonProgress);
    }

    public override void Draw(SpriteBatch spriteBatch)
    {
        // Track draw time for performance monitoring
        var drawStartTime = DateTime.Now;
        
        // Draw world with camera transform
        spriteBatch.Begin(transformMatrix: _camera.GetTransform(), 
                         samplerState: SamplerState.PointClamp);
        
        // Calculate visible bounds for frustum culling
        Rectangle viewportBounds = new Rectangle(
            (int)(_camera.Position.X - Game.GraphicsDevice.Viewport.Width / (2 * _camera.Zoom)),
            (int)(_camera.Position.Y - Game.GraphicsDevice.Viewport.Height / (2 * _camera.Zoom)),
            (int)(Game.GraphicsDevice.Viewport.Width / _camera.Zoom),
            (int)(Game.GraphicsDevice.Viewport.Height / _camera.Zoom)
        );
        
        // Draw either mine or overworld
        if (_miningManager.InMine)
        {
            _miningManager.Draw(spriteBatch);
        }
        else
        {
            _worldMap.Draw(spriteBatch, viewportBounds);
        }
        
        _player.Draw(spriteBatch);
        _npcManager.Draw(spriteBatch, Game.DefaultFont, viewportBounds);
        
        // Draw wild pets (simple colored circles for now)
        foreach (var wildPet in _wildPets)
        {
            if (!wildPet.IsTamed)
            {
                // Draw a colored circle to represent the wild pet
                Color petColor = wildPet.DefinitionId switch
                {
                    "dog" => Color.Brown,
                    "cat" => Color.Orange,
                    "wolf" => Color.Gray,
                    "chicken" => Color.White,
                    "hawk" => Color.DarkGray,
                    _ => Color.Yellow
                };
                
                DrawFilledCircle(spriteBatch, wildPet.Position, 8, petColor);
                
                // Draw pet name above it
                Vector2 namePos = wildPet.Position - new Vector2(0, 20);
                spriteBatch.DrawString(Game.DefaultFont, wildPet.Name, namePos, Color.White);
                
                // Draw "T to Tame" if player is nearby
                if (wildPet.IsInRangeOf(_player.Position))
                {
                    Vector2 hintPos = wildPet.Position + new Vector2(0, 15);
                    spriteBatch.DrawString(Game.DefaultFont, "[T] Tame", hintPos, Color.LightGreen);
                }
            }
        }
        
        // Draw enemies and health bars (in world space with camera transform)
        if (_miningManager.InMine)
        {
            DrawEnemies(spriteBatch, viewportBounds);
        }
        
        // Draw projectiles (Phase 7.4 - in world space with camera transform)
        _projectileSystem.Draw(spriteBatch, Game.GraphicsDevice);
        
        // Draw particle effects in world space (with camera transform)
        _particleSystem.Draw(spriteBatch, Game.GraphicsDevice);
        
        // Draw weather effects in world space (with camera transform)
        _weatherSystem.Draw(spriteBatch, Game.GraphicsDevice, viewportBounds);
        
        // Draw building placement preview (in world space with camera transform)
        if (_buildingMenu.IsPlacementMode)
        {
            Vector2 mouseWorldPos = GetMouseWorldPosition();
            Vector2 tilePos = new Vector2(
                (int)(mouseWorldPos.X / GameConstants.TILE_SIZE),
                (int)(mouseWorldPos.Y / GameConstants.TILE_SIZE)
            );
            
            _buildingMenu.DrawPlacementPreview(spriteBatch, Game.DefaultFont, Game.GraphicsDevice,
                                              tilePos, _worldMap.GetAllTiles(), GameConstants.TILE_SIZE,
                                              _camera.Position, _camera.Zoom);
        }
        
        spriteBatch.End();
        
        // Draw biome screen tint overlay (between world and UI)
        spriteBatch.Begin();
        var currentBiomeDef = _biomeSystem.GetBiome(_biomeSystem.CurrentBiome);
        if (currentBiomeDef != null)
        {
            // Draw semi-transparent overlay with biome tint color
            Color tintColor = currentBiomeDef.TintColor * 0.15f; // 15% opacity
            spriteBatch.Draw(_pixelTexture, 
                new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height),
                tintColor);
        }
        spriteBatch.End();
        
        // Draw HUD (no camera transform)
        spriteBatch.Begin(samplerState: SamplerState.PointClamp);
        _hud.DrawPlayerStats(spriteBatch, Game.DefaultFont, _player, _timeSystem);
        
        // Draw current biome name in upper right
        if (currentBiomeDef != null)
        {
            string biomeText = $"Biome: {currentBiomeDef.Name}";
            Vector2 biomeTextSize = Game.DefaultFont.MeasureString(biomeText);
            Vector2 biomeTextPos = new Vector2(
                Game.GraphicsDevice.Viewport.Width - biomeTextSize.X - 10,
                10
            );
            spriteBatch.DrawString(Game.DefaultFont, biomeText, biomeTextPos, Color.White);
        }
        
        // Draw NPC UI (dialogue wheel)
        _npcManager.DrawUI(spriteBatch, Game.DefaultFont);
        
        // Draw fishing UI
        if (_fishingManager.IsFishing)
        {
            _fishingManager.Draw(spriteBatch, Game.DefaultFont, 
                                Game.GraphicsDevice.Viewport.Width, 
                                Game.GraphicsDevice.Viewport.Height);
        }
        
        // Draw menus (on top of everything)
        if (_craftingMenu.IsActive)
        {
            _craftingMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        if (_shopMenu.IsActive)
        {
            _shopMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        if (_giftMenu.IsActive)
        {
            _giftMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        if (_questMenu.IsActive)
        {
            _questMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        if (_buildingMenu.IsActive_Menu)
        {
            _buildingMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice, _player.Money);
        }
        
        if (_achievementMenu.IsVisible)
        {
            _achievementMenu.Draw(spriteBatch, Game.GraphicsDevice);
        }
        
        if (_settingsMenu.IsVisible)
        {
            _settingsMenu.Draw(spriteBatch, Game.GraphicsDevice);
        }
        
        // Draw Phase 6 menus
        if (_magicMenu.IsActive)
        {
            _magicMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        if (_alchemyMenu.IsActive)
        {
            _alchemyMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        if (_skillsMenu.IsActive)
        {
            _skillsMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        if (_petMenu.IsActive)
        {
            _petMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        if (_dungeonMenu.IsOpen)
        {
            _dungeonMenu.Draw(spriteBatch, Game.GraphicsDevice);
        }
        
        if (_factionMenu.IsActive)
        {
            _factionMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        if (_marriageProposalMenu.IsActive)
        {
            _marriageProposalMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        if (_familyMenu.IsActive)
        {
            _familyMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        // Draw unified player menu (consolidates most character-related menus)
        if (_unifiedPlayerMenu.IsActive)
        {
            _unifiedPlayerMenu.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        }
        
        // Draw event notification (on top of most UI but below menus)
        _eventNotification.Draw(spriteBatch, Game.DefaultFont, Game.GraphicsDevice);
        
        // Draw achievement notification (on top of events)
        _achievementNotification.Draw(spriteBatch, Game.GraphicsDevice);
        
        // Draw Phase 7 UI elements
        _minimap.Draw(spriteBatch, Game.DefaultFont);
        _notificationSystem.Draw(spriteBatch, Game.DefaultFont);
        _performanceMonitor.Draw(spriteBatch, Game.DefaultFont, 
            Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
        
        // Draw pause indicator
        if (_isPaused)
        {
            DrawPauseOverlay(spriteBatch);
        }
        
        spriteBatch.End();
        
        // Record draw time for performance monitoring
        var drawEndTime = DateTime.Now;
        _performanceMonitor.RecordDrawTime((drawEndTime - drawStartTime).TotalMilliseconds);
    }
    
    private void DrawPauseOverlay(SpriteBatch spriteBatch)
    {
        // Draw semi-transparent overlay
        Rectangle screen = new Rectangle(0, 0, Game.GraphicsDevice.Viewport.Width, Game.GraphicsDevice.Viewport.Height);
        spriteBatch.Draw(_pixelTexture, screen, Color.Black * 0.5f);
        
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
    
    private NPCCharacter GetNearbyNPC()
    {
        const float GIFT_DISTANCE = 64f; // Distance at which player can give gifts
        return _npcManager.GetNearbyNPC(_player.Position, GIFT_DISTANCE);
    }
    
    private void CreateTestNPC()
    {
        // NPC starting positions (in world coordinates)
        // Positioned around the town/farm area to provide good coverage
        const float EMMA_X = 600f, EMMA_Y = 400f;       // Emma (Farmer) - Farm area
        const float MARCUS_X = 400f, MARCUS_Y = 600f;   // Marcus (Blacksmith) - Workshop area
        const float LILY_X = 800f, LILY_Y = 350f;       // Lily (Merchant) - Shop area
        const float OLIVER_X = 350f, OLIVER_Y = 250f;   // Oliver (Fisherman) - Near water
        const float SARAH_X = 500f, SARAH_Y = 150f;     // Sarah (Doctor) - Clinic area
        const float JACK_X = 700f, JACK_Y = 300f;       // Jack (Carpenter) - Workshop area
        const float MAYA_X = 250f, MAYA_Y = 350f;       // Maya (Artist) - Art studio area
        
        // Create and add all NPCs using NPCFactory
        _npcManager.AddNPC(NPCFactory.CreateEmma(new Vector2(EMMA_X, EMMA_Y)));
        _npcManager.AddNPC(NPCFactory.CreateMarcus(new Vector2(MARCUS_X, MARCUS_Y)));
        _npcManager.AddNPC(NPCFactory.CreateLily(new Vector2(LILY_X, LILY_Y)));
        _npcManager.AddNPC(NPCFactory.CreateOliver(new Vector2(OLIVER_X, OLIVER_Y)));
        
        // Add new NPCs from Phase 5
        _npcManager.AddNPC(NPCFactory.CreateSarah(new Vector2(SARAH_X, SARAH_Y)));
        _npcManager.AddNPC(NPCFactory.CreateJack(new Vector2(JACK_X, JACK_Y)));
        _npcManager.AddNPC(NPCFactory.CreateMaya(new Vector2(MAYA_X, MAYA_Y)));
    }
    
    
    private void InitializeQuests()
    {
        // Quest 1: Emma's First Harvest
        var quest1 = new Quest("emma_harvest", "First Harvest", 
            "Emma needs help getting crops ready for the market.", "Emma");
        quest1.AddObjective(new QuestObjective("harvest_wheat", "Harvest 5 Wheat", 
            QuestObjectiveType.Harvest, "Wheat", 5));
        quest1.Reward = new QuestReward 
        { 
            Money = 100, 
            FriendshipPoints = 50, 
            FriendshipNPC = "Emma" 
        };
        quest1.Reward.Items.Add("Wheat Seeds", 10);
        _questSystem.AddAvailableQuest(quest1);
        
        // Quest 2: Marcus's Mining Request
        var quest2 = new Quest("marcus_ore", "Mining for Marcus", 
            "Marcus needs copper ore for his smithing work.", "Marcus");
        quest2.AddObjective(new QuestObjective("collect_copper", "Collect 10 Copper Ore", 
            QuestObjectiveType.CollectItem, "Copper Ore", 10));
        quest2.Reward = new QuestReward 
        { 
            Money = 200, 
            FriendshipPoints = 75, 
            FriendshipNPC = "Marcus" 
        };
        quest2.Reward.Items.Add("Iron Pickaxe", 1);
        _questSystem.AddAvailableQuest(quest2);
        
        // Quest 3: Lily's Shopping List
        var quest3 = new Quest("lily_items", "Lily's Supply Run", 
            "Lily needs various items to stock her shop.", "Lily");
        quest3.AddObjective(new QuestObjective("collect_wood", "Collect 20 Wood", 
            QuestObjectiveType.CollectItem, "Wood", 20));
        quest3.AddObjective(new QuestObjective("collect_stone", "Collect 15 Stone", 
            QuestObjectiveType.CollectItem, "Stone", 15));
        quest3.Reward = new QuestReward 
        { 
            Money = 250, 
            FriendshipPoints = 60, 
            FriendshipNPC = "Lily" 
        };
        _questSystem.AddAvailableQuest(quest3);
        
        // Quest 4: Oliver's Fishing Challenge
        var quest4 = new Quest("oliver_fish", "The Big Catch", 
            "Oliver wants you to prove your fishing skills.", "Oliver");
        quest4.AddObjective(new QuestObjective("catch_fish", "Catch 15 Fish", 
            QuestObjectiveType.Fish, "Fish", 15));
        quest4.Reward = new QuestReward 
        { 
            Money = 150, 
            FriendshipPoints = 80, 
            FriendshipNPC = "Oliver" 
        };
        quest4.Reward.Items.Add("Advanced Fishing Rod", 1);
        _questSystem.AddAvailableQuest(quest4);
        
        // Quest 5: Welcome to MoonBrook Ridge
        var quest5 = new Quest("welcome", "Welcome to Town", 
            "Get to know the residents of MoonBrook Ridge.", "Town Notice");
        quest5.AddObjective(new QuestObjective("talk_emma", "Talk to Emma", 
            QuestObjectiveType.TalkToNPC, "Emma", 1));
        quest5.AddObjective(new QuestObjective("talk_marcus", "Talk to Marcus", 
            QuestObjectiveType.TalkToNPC, "Marcus", 1));
        quest5.AddObjective(new QuestObjective("talk_lily", "Talk to Lily", 
            QuestObjectiveType.TalkToNPC, "Lily", 1));
        quest5.AddObjective(new QuestObjective("talk_oliver", "Talk to Oliver", 
            QuestObjectiveType.TalkToNPC, "Oliver", 1));
        quest5.Reward = new QuestReward 
        { 
            Money = 300, 
            FriendshipPoints = 0 
        };
        quest5.Reward.Items.Add("Starter Pack", 1);
        _questSystem.AddAvailableQuest(quest5);
        
        // Quest 6: Dungeon Explorer - Slime Cave
        var quest6 = new Quest("dungeon_slime", "Slime Cave Expedition", 
            "Investigate the Slime Cave and clear at least 3 rooms.", "Town Elder");
        quest6.AddObjective(new QuestObjective("enter_slime_cave", "Enter the Slime Cave", 
            QuestObjectiveType.EnterDungeon, "SlimeCave", 1));
        quest6.AddObjective(new QuestObjective("clear_3_rooms", "Clear 3 dungeon rooms", 
            QuestObjectiveType.ClearRooms, "Any", 3));
        quest6.Reward = new QuestReward 
        { 
            Money = 400, 
            FriendshipPoints = 0 
        };
        quest6.Reward.Items.Add("Iron Sword", 1);
        quest6.Reward.Items.Add("Health Potion", 5);
        _questSystem.AddAvailableQuest(quest6);
        
        // Quest 7: Pet Companion - Tame Your First Pet
        var quest7 = new Quest("pet_taming", "A Loyal Companion", 
            "Find and tame your first pet companion.", "Sarah");
        quest7.AddObjective(new QuestObjective("tame_pet", "Tame any pet", 
            QuestObjectiveType.TamePet, "Any", 1));
        quest7.Reward = new QuestReward 
        { 
            Money = 250, 
            FriendshipPoints = 50, 
            FriendshipNPC = "Sarah" 
        };
        quest7.Reward.Items.Add("Pet Food", 10);
        quest7.Reward.Items.Add("Pet Toy", 1);
        _questSystem.AddAvailableQuest(quest7);
        
        // Quest 8: Skilled Farmer - Master a Farming Skill
        var quest8 = new Quest("skill_farming", "Path of the Farmer", 
            "Reach Farming Level 5 and unlock your first farming skill.", "Emma");
        quest8.AddObjective(new QuestObjective("farming_level_5", "Reach Farming Level 5", 
            QuestObjectiveType.ReachSkillLevel, "Farming", 5));
        quest8.AddObjective(new QuestObjective("unlock_farming_skill", "Unlock any Farming skill", 
            QuestObjectiveType.UnlockSkill, "Farming", 1));
        quest8.Reward = new QuestReward 
        { 
            Money = 500, 
            FriendshipPoints = 100, 
            FriendshipNPC = "Emma" 
        };
        quest8.Reward.Items.Add("Quality Fertilizer", 20);
        _questSystem.AddAvailableQuest(quest8);
        
        // Quest 9: Arcane Studies - Learn Magic
        var quest9 = new Quest("magic_learning", "First Steps into Magic", 
            "Learn your first spell and cast it successfully.", "Town Mage");
        quest9.AddObjective(new QuestObjective("learn_spell", "Learn any spell", 
            QuestObjectiveType.LearnSpell, "Any", 1));
        quest9.AddObjective(new QuestObjective("cast_spell", "Cast a spell 3 times", 
            QuestObjectiveType.CastSpell, "Any", 3));
        quest9.Reward = new QuestReward 
        { 
            Money = 300, 
            FriendshipPoints = 0 
        };
        quest9.Reward.Items.Add("Mana Potion", 5);
        quest9.Reward.Items.Add("Spell Scroll", 1);
        _questSystem.AddAvailableQuest(quest9);
        
        // Quest 10: Dungeon Master - Complete Any Dungeon
        var quest10 = new Quest("dungeon_complete", "Dungeon Master", 
            "Complete any dungeon by clearing all rooms and defeating the boss.", "Marcus");
        quest10.AddObjective(new QuestObjective("complete_dungeon", "Complete a full dungeon", 
            QuestObjectiveType.CompleteDungeon, "Any", 1));
        quest10.Reward = new QuestReward 
        { 
            Money = 1000, 
            FriendshipPoints = 100, 
            FriendshipNPC = "Marcus" 
        };
        quest10.Reward.Items.Add("Steel Sword", 1);
        quest10.Reward.Items.Add("Golden Ring", 1);
        quest10.Reward.Items.Add("Rare Gem", 3);
        _questSystem.AddAvailableQuest(quest10);
    }
    
    /// <summary>
    /// Draw enemies and their health bars
    /// </summary>
    private void DrawEnemies(SpriteBatch spriteBatch, Rectangle? visibleBounds = null)
    {
        var enemies = _combatSystem.GetActiveEnemies();
        
        foreach (var enemy in enemies)
        {
            // Frustum culling for enemies (Phase 7.4)
            if (visibleBounds.HasValue)
            {
                Rectangle enemyBounds = new Rectangle(
                    (int)enemy.Position.X - 32,
                    (int)enemy.Position.Y - 32,
                    64, 64
                );
                
                if (!visibleBounds.Value.Intersects(enemyBounds))
                {
                    continue; // Skip enemies outside visible area
                }
            }
            
            // Draw enemy as a colored circle (placeholder until we have sprites)
            Color enemyColor = enemy.Type switch
            {
                EnemyType.Slime => Color.Green,
                EnemyType.Bat => Color.DarkGray,
                EnemyType.Skeleton => Color.White,
                EnemyType.Goblin => Color.Brown,
                EnemyType.Spider => Color.Purple,
                EnemyType.Wolf => Color.Gray,
                EnemyType.Ghost => Color.LightGray,
                EnemyType.Zombie => Color.DarkGreen,
                EnemyType.Orc => Color.Red,
                EnemyType.Dragon => Color.OrangeRed,
                EnemyType.Elemental => Color.Orange,
                EnemyType.Demon => Color.DarkRed,
                _ => Color.White
            };
            
            // Draw enemy circle (16x16 pixels)
            int enemySize = enemy.IsBoss ? 24 : 16;
            DrawFilledCircle(spriteBatch, enemy.Position, enemySize / 2, enemyColor);
            
            // Draw health bar above enemy
            int barWidth = 32;
            int barHeight = 4;
            Vector2 barPosition = enemy.Position - new Vector2(barWidth / 2, enemySize + 8);
            
            // Background (black)
            spriteBatch.Draw(_pixelTexture, 
                new Rectangle((int)barPosition.X, (int)barPosition.Y, barWidth, barHeight),
                Color.Black * 0.7f);
            
            // Health fill (red to green based on health percentage)
            float healthPercent = enemy.Health / enemy.MaxHealth;
            int fillWidth = (int)(barWidth * healthPercent);
            Color healthColor = healthPercent > 0.5f ? Color.Green : (healthPercent > 0.25f ? Color.Yellow : Color.Red);
            
            spriteBatch.Draw(_pixelTexture, 
                new Rectangle((int)barPosition.X, (int)barPosition.Y, fillWidth, barHeight),
                healthColor);
            
            // Draw enemy name above health bar
            if (enemy.IsBoss)
            {
                Vector2 nameSize = Game.DefaultFont.MeasureString(enemy.Name);
                Vector2 namePosition = barPosition - new Vector2(nameSize.X / 2, nameSize.Y + 2);
                spriteBatch.DrawString(Game.DefaultFont, enemy.Name, namePosition, Color.White);
            }
        }
    }
    
    /// <summary>
    /// Draw a filled circle (helper method for enemy rendering)
    /// </summary>
    private void DrawFilledCircle(SpriteBatch spriteBatch, Vector2 center, int radius, Color color)
    {
        // Simple approximation using rectangles
        for (int x = -radius; x <= radius; x++)
        {
            for (int y = -radius; y <= radius; y++)
            {
                if (x * x + y * y <= radius * radius)
                {
                    spriteBatch.Draw(_pixelTexture,
                        new Rectangle((int)center.X + x, (int)center.Y + y, 1, 1),
                        color);
                }
            }
        }
    }
    
    /// <summary>
    /// Spawn wild pets in the world for taming
    /// </summary>
    private void SpawnWildPets()
    {
        // Spawn a few wild pets around the map for players to discover and tame
        _wildPets.Add(new WildPet("dog", "Wild Dog", new Vector2(15, 20) * GameConstants.TILE_SIZE));
        _wildPets.Add(new WildPet("cat", "Stray Cat", new Vector2(35, 25) * GameConstants.TILE_SIZE));
        _wildPets.Add(new WildPet("wolf", "Wild Wolf", new Vector2(45, 15) * GameConstants.TILE_SIZE));
        _wildPets.Add(new WildPet("chicken", "Wild Chicken", new Vector2(20, 30) * GameConstants.TILE_SIZE));
        _wildPets.Add(new WildPet("hawk", "Wild Hawk", new Vector2(40, 35) * GameConstants.TILE_SIZE));
    }
    
    /// <summary>
    /// Update wild pets and handle taming interactions
    /// </summary>
    private void UpdateWildPets(GameTime gameTime)
    {
        foreach (var wildPet in _wildPets)
        {
            if (!wildPet.IsTamed)
            {
                wildPet.Update(gameTime);
            }
        }
    }
    
    /// <summary>
    /// Attempt to tame a nearby wild pet
    /// </summary>
    private void TryTameWildPet()
    {
        foreach (var wildPet in _wildPets)
        {
            if (!wildPet.IsTamed && wildPet.IsInRangeOf(_player.Position))
            {
                // Try to tame the pet
                if (_petSystem.TamePet(wildPet.DefinitionId, wildPet.Position))
                {
                    wildPet.IsTamed = true;
                    // Show notification (if notification system exists)
                    // For now, the quest system will track the taming through the event
                    return;
                }
            }
        }
    }
    
    /// <summary>
    /// Determine the biome based on player position and update if changed
    /// </summary>
    private void UpdateBiomeFromPosition()
    {
        // Convert player position to tile coordinates
        int tileX = (int)(_player.Position.X / GameConstants.TILE_SIZE);
        int tileY = (int)(_player.Position.Y / GameConstants.TILE_SIZE);
        
        // Simple position-based biome determination
        // This is a basic implementation - can be enhanced with tile-based biome data
        Biomes.BiomeType newBiome = Biomes.BiomeType.Farm; // Default
        
        // In mines or dungeons, use cave biomes
        if (_miningManager.InMine)
        {
            int depth = _miningManager.CurrentLevel;
            if (depth >= 10)
                newBiome = Biomes.BiomeType.DeepCave;
            else
                newBiome = Biomes.BiomeType.Cave;
        }
        else if (_dungeonSystem.ActiveDungeon != null)
        {
            // Dungeon-specific biomes based on dungeon type
            newBiome = _dungeonSystem.ActiveDungeon.Type switch
            {
                Dungeons.DungeonType.SlimeCave => Biomes.BiomeType.Cave,
                Dungeons.DungeonType.SkeletonCrypt => Biomes.BiomeType.HauntedForest,
                Dungeons.DungeonType.SpiderNest => Biomes.BiomeType.Cave,
                Dungeons.DungeonType.GoblinWarrens => Biomes.BiomeType.Cave,
                Dungeons.DungeonType.HauntedManor => Biomes.BiomeType.HauntedForest,
                Dungeons.DungeonType.DragonLair => Biomes.BiomeType.Volcanic,
                Dungeons.DungeonType.DemonRealm => Biomes.BiomeType.Volcanic,
                Dungeons.DungeonType.AncientRuins => Biomes.BiomeType.MagicalMeadow,
                _ => Biomes.BiomeType.Cave
            };
        }
        else
        {
            // Overworld position-based biomes
            // Simple quadrant-based system for demonstration
            if (tileX < 20 && tileY < 20)
                newBiome = Biomes.BiomeType.Farm; // Northwest = Farm
            else if (tileX >= 30 && tileY < 20)
                newBiome = Biomes.BiomeType.Forest; // Northeast = Forest
            else if (tileX < 20 && tileY >= 30)
                newBiome = Biomes.BiomeType.Swamp; // Southwest = Swamp
            else if (tileX >= 30 && tileY >= 30)
                newBiome = Biomes.BiomeType.Desert; // Southeast = Desert
            else
                newBiome = Biomes.BiomeType.Farm; // Center = Farm
        }
        
        // Update biome if it changed
        if (newBiome != _biomeSystem.CurrentBiome)
        {
            _biomeSystem.ChangeBiome(newBiome);
        }
    }
    
    /// <summary>
    /// Check collisions between projectiles and enemies (Phase 7.4)
    /// </summary>
    private void CheckProjectileCollisions()
    {
        var projectiles = _projectileSystem.GetActiveProjectiles();
        var enemies = _combatSystem.GetActiveEnemies();
        
        // Track projectiles to remove after iteration (avoid modifying collection during iteration)
        var projectilesToRemove = new List<Projectile>();
        
        // Check each projectile against each enemy
        foreach (var projectile in projectiles)
        {
            // Skip projectiles owned by non-player (future: enemy projectiles)
            if (projectile.OwnerId != "player")
                continue;
            
            // Check collision with each enemy
            foreach (var enemy in enemies)
            {
                if (enemy.IsDead)
                    continue;
                
                // Get bounding boxes
                Rectangle projectileBounds = projectile.GetBounds();
                Rectangle enemyBounds = new Rectangle(
                    (int)(enemy.Position.X - 16),
                    (int)(enemy.Position.Y - 16),
                    32, 32
                );
                
                // Check collision
                if (projectileBounds.Intersects(enemyBounds))
                {
                    // Apply damage through combat system (Phase 7.4)
                    _combatSystem.ApplyProjectileDamage(enemy, projectile.Damage);
                    
                    // Mark projectile for removal
                    projectilesToRemove.Add(projectile);
                    break; // Move to next projectile
                }
            }
        }
        
        // Remove all marked projectiles
        foreach (var projectile in projectilesToRemove)
        {
            _projectileSystem.RemoveProjectile(projectile);
        }
    }
}
