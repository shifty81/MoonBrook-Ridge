using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.World.Buildings;
using MoonBrookRidge.Characters.NPCs;
using MoonBrookRidge.Core.Systems;
using MoonBrookRidge.Core;
using System;
using System.Collections.Generic;

namespace MoonBrookRidge.Core.Scenes;

/// <summary>
/// Dungeon overworld scene - static open area with fauna where player can build an outpost
/// Features a walled-off center area with gate and elevator/tunnel boring machine
/// </summary>
public class DungeonOverworldScene : ExteriorScene
{
    private string _dungeonName;
    private List<Building> _playerBuildings;
    private List<NPCCharacter> _fauna; // Wildlife in the area
    private Vector2 _elevatorPosition;
    private Vector2 _gatePosition;
    private Rectangle _walledArea;
    private bool _canBuild;
    private bool _isGateOpen;
    private TunnelBoringMachine _boringMachine;
    
    public string DungeonName => _dungeonName;
    public List<Building> PlayerBuildings => _playerBuildings;
    public bool CanBuild => _canBuild;
    public TunnelBoringMachine BoringMachine => _boringMachine;
    public bool IsGateOpen => _isGateOpen;
    
    public DungeonOverworldScene(string dungeonName, int width = 100, int height = 100) 
        : base($"{dungeonName} Base Camp", $"dungeon_overworld_{dungeonName}", width, height)
    {
        _dungeonName = dungeonName;
        _playerBuildings = new List<Building>();
        _fauna = new List<NPCCharacter>();
        _canBuild = true;
        _isGateOpen = false;
        Type = SceneType.DungeonOverworld; // Override to DungeonOverworld type
        
        // Initialize elevator/boring machine at center
        _elevatorPosition = new Vector2(_width / 2, _height / 2);
        _boringMachine = new TunnelBoringMachine(_elevatorPosition * GameConstants.TILE_SIZE, dungeonName);
    }
    
    public override void Initialize()
    {
        base.Initialize();
        
        // Create open area with varied terrain and fauna
        System.Random random = new System.Random(_dungeonName.GetHashCode());
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                TileType tileType;
                
                // Most of the area is grass (buildable)
                if (x > 5 && x < _width - 5 && y > 5 && y < _height - 5)
                {
                    int variant = random.Next(10);
                    tileType = variant < 6 ? TileType.Grass : 
                               variant < 9 ? TileType.Grass01 : TileType.Grass02;
                }
                // Outer edges are stone/rock
                else
                {
                    int variant = random.Next(10);
                    tileType = variant < 5 ? TileType.Stone : TileType.Rock;
                }
                
                SetTile(x, y, new Tile(tileType, new Vector2(x, y)));
            }
        }
        
        // Create walled-off center area (20x20)
        CreateWalledArea();
        
        // Place elevator/boring machine at center
        PlaceElevatorShaft();
        
        // Place gate to walled area
        PlaceGate();
        
        // Add fauna (wildlife)
        SpawnFauna(random);
        
        // Add decorative scenery
        AddScenery(random);
    }
    
    /// <summary>
    /// Create the walled-off center area (20x20 tiles)
    /// </summary>
    private void CreateWalledArea()
    {
        int centerX = _width / 2;
        int centerY = _height / 2;
        int wallSize = 10; // 20x20 area (10 tiles in each direction from center)
        
        _walledArea = new Rectangle(
            centerX - wallSize,
            centerY - wallSize,
            wallSize * 2,
            wallSize * 2
        );
        
        // Create stone walls around the perimeter
        for (int x = _walledArea.Left; x <= _walledArea.Right; x++)
        {
            // Top wall
            SetTile(x, _walledArea.Top, new Tile(TileType.Wall, new Vector2(x, _walledArea.Top)));
            // Bottom wall
            SetTile(x, _walledArea.Bottom, new Tile(TileType.Wall, new Vector2(x, _walledArea.Bottom)));
        }
        
        for (int y = _walledArea.Top; y <= _walledArea.Bottom; y++)
        {
            // Left wall
            SetTile(_walledArea.Left, y, new Tile(TileType.Wall, new Vector2(_walledArea.Left, y)));
            // Right wall
            SetTile(_walledArea.Right, y, new Tile(TileType.Wall, new Vector2(_walledArea.Right, y)));
        }
        
        // Fill interior with stone floor
        for (int x = _walledArea.Left + 1; x < _walledArea.Right; x++)
        {
            for (int y = _walledArea.Top + 1; y < _walledArea.Bottom; y++)
            {
                SetTile(x, y, new Tile(TileType.Stone, new Vector2(x, y)));
            }
        }
    }
    
    /// <summary>
    /// Place the elevator shaft and boring machine at center
    /// </summary>
    private void PlaceElevatorShaft()
    {
        int centerX = _width / 2;
        int centerY = _height / 2;
        
        // Create elevator shaft (3x3 area)
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int x = centerX + dx;
                int y = centerY + dy;
                SetTile(x, y, new Tile(TileType.Stone01, new Vector2(x, y)));
            }
        }
        
        // Add transition to boring machine interface
        var elevatorTransition = new SceneTransition(
            $"elevator_{_dungeonName}",
            _elevatorPosition * GameConstants.TILE_SIZE,
            $"dungeon_{_dungeonName}_floor_1",
            new Vector2(_width / 2, 3),
            TransitionType.Portal,
            requiresInteraction: true
        );
        elevatorTransition.InteractionText = "Press X to use Tunnel Boring Machine";
        
        AddTransition(elevatorTransition);
        
        // Add elevator object
        var elevatorObj = new SceneObject(
            "Tunnel Boring Machine",
            _elevatorPosition * GameConstants.TILE_SIZE,
            isBlocking: true,
            isInteractive: true
        );
        AddObject(elevatorObj);
    }
    
    /// <summary>
    /// Place the gate at the entrance to the walled area
    /// </summary>
    private void PlaceGate()
    {
        // Gate on the south wall (bottom center)
        _gatePosition = new Vector2(_width / 2, _walledArea.Bottom);
        
        // Clear the wall tile for the gate
        SetTile((int)_gatePosition.X, (int)_gatePosition.Y, 
                new Tile(TileType.Stone, _gatePosition));
        
        // Add gate object
        var gateObj = new GateObject(
            "Secure Gate",
            _gatePosition * GameConstants.TILE_SIZE,
            isBlocking: !_isGateOpen
        );
        AddObject(gateObj);
    }
    
    /// <summary>
    /// Spawn fauna (wildlife) in the open areas
    /// </summary>
    private void SpawnFauna(System.Random random)
    {
        int faunaCount = 15; // Number of wildlife creatures
        
        for (int i = 0; i < faunaCount; i++)
        {
            // Spawn in buildable areas, avoiding walled center
            int x, y;
            do
            {
                x = random.Next(10, _width - 10);
                y = random.Next(10, _height - 10);
            } while (_walledArea.Contains(x, y));
            
            Vector2 position = new Vector2(x * GameConstants.TILE_SIZE, y * GameConstants.TILE_SIZE);
            
            // Create passive wildlife NPC
            string[] faunaTypes = { "Deer", "Rabbit", "Bird", "Squirrel", "Fox" };
            string faunaType = faunaTypes[random.Next(faunaTypes.Length)];
            
            var fauna = new NPCCharacter($"{faunaType}_{i}", position);
            _fauna.Add(fauna);
            
            // Add as scene object
            var faunaObj = new SceneObject(
                faunaType,
                position,
                isBlocking: false,
                isInteractive: false
            );
            AddObject(faunaObj);
        }
    }
    
    private void AddScenery(System.Random random)
    {
        // Add decorative rocks, trees, and bushes
        int sceneryCount = 50;
        
        for (int i = 0; i < sceneryCount; i++)
        {
            int x = random.Next(_width);
            int y = random.Next(_height);
            
            // Don't place in walled area
            if (_walledArea.Contains(x, y))
                continue;
            
            // Don't place too close to edges
            if (x < 6 || x >= _width - 6 || y < 6 || y >= _height - 6)
                continue;
            
            Vector2 position = new Vector2(x * GameConstants.TILE_SIZE, y * GameConstants.TILE_SIZE);
            
            int sceneryType = random.Next(3);
            string name = sceneryType switch
            {
                0 => "Rock",
                1 => "Tree",
                _ => "Bush"
            };
            
            var sceneryObj = new SceneObject(
                name,
                position,
                isBlocking: sceneryType != 2, // Bushes not blocking
                isInteractive: false
            );
            
            AddObject(sceneryObj);
        }
    }
    
    /// <summary>
    /// Open or close the gate (player/party access only)
    /// </summary>
    public void ToggleGate(bool open)
    {
        _isGateOpen = open;
        
        // Update gate object blocking state
        foreach (var obj in _objects)
        {
            if (obj is GateObject gate && obj.Name == "Secure Gate")
            {
                gate.IsBlocking = !_isGateOpen;
                break;
            }
        }
    }
    
    /// <summary>
    /// Check if player is authorized to enter walled area
    /// </summary>
    public bool IsPlayerAuthorized(string playerId)
    {
        // TODO: Implement party system check
        // For now, always allow the player
        return true;
    }
    
    /// <summary>
    /// Update fauna and boring machine
    /// </summary>
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        
        // Update fauna movement
        // Note: Fauna require TimeSystem for schedule updates, which may not be available in dungeon overworld
        // TODO: Implement proper fauna behavior without schedule dependency
        /*foreach (var animal in _fauna)
        {
            animal.Update(gameTime);
        }*/
        
        // Update boring machine
        _boringMachine?.Update(gameTime);
    }
    
    /// <summary>
    /// Add a building to the overworld (player construction)
    /// </summary>
    public bool AddBuilding(Building building, Vector2 position)
    {
        if (!_canBuild) return false;
        
        // Check if position is valid for building
        if (!IsValidBuildPosition(position, building.Width, building.Height))
        {
            return false;
        }
        
        building.Position = position;
        _playerBuildings.Add(building);
        
        // Mark tiles as occupied
        int gridX = (int)(position.X / GameConstants.TILE_SIZE);
        int gridY = (int)(position.Y / GameConstants.TILE_SIZE);
        
        for (int x = 0; x < building.Width; x++)
        {
            for (int y = 0; y < building.Height; y++)
            {
                var tile = GetTile(gridX + x, gridY + y);
                if (tile != null)
                {
                    tile.SetBlocking(true);
                }
            }
        }
        
        return true;
    }
    
    /// <summary>
    /// Check if a position is valid for building
    /// </summary>
    private bool IsValidBuildPosition(Vector2 position, int width, int height)
    {
        int gridX = (int)(position.X / GameConstants.TILE_SIZE);
        int gridY = (int)(position.Y / GameConstants.TILE_SIZE);
        
        // Check if within buildable area (avoiding edges and walled center)
        if (gridX < 6 || gridX + width > _width - 6 || gridY < 6 || gridY + height > _height - 6)
        {
            return false;
        }
        
        // Check if overlaps with walled area
        Rectangle buildingBounds = new Rectangle(gridX, gridY, width, height);
        if (_walledArea.Intersects(buildingBounds))
        {
            return false;
        }
        
        // Check if tiles are clear
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                var tile = GetTile(gridX + x, gridY + y);
                if (tile == null || tile.IsBlocking() || tile.Type != TileType.Grass && tile.Type != TileType.Grass01)
                {
                    return false;
                }
            }
        }
        
        // Check for overlap with existing buildings
        Rectangle newBuildingBounds = new Rectangle(gridX, gridY, width, height);
        foreach (var existing in _playerBuildings)
        {
            int existingGridX = (int)(existing.Position.X / GameConstants.TILE_SIZE);
            int existingGridY = (int)(existing.Position.Y / GameConstants.TILE_SIZE);
            Rectangle existingBounds = new Rectangle(existingGridX, existingGridY, existing.Width, existing.Height);
            
            if (newBuildingBounds.Intersects(existingBounds))
            {
                return false;
            }
        }
        
        return true;
    }
    
    public override void Draw(SpriteBatch spriteBatch, Camera2D camera)
    {
        // Draw base terrain
        base.Draw(spriteBatch, camera);
        
        // Draw fauna
        foreach (var animal in _fauna)
        {
            if (camera.IsInView(animal.Position, 32, 32))
            {
                animal.Draw(spriteBatch);
            }
        }
        
        // Draw player buildings
        // Note: Building rendering needs proper texture loading - using placeholder for now
        foreach (var building in _playerBuildings)
        {
            if (camera.IsInView(building.Position, building.Width, building.Height))
            {
                // TODO: Load and pass proper building textures
                building.Draw(spriteBatch, null, Color.White);
            }
        }
        
        // Draw boring machine
        if (_boringMachine != null && camera.IsInView(_boringMachine.Position, 64, 64))
        {
            _boringMachine.Draw(spriteBatch);
        }
    }
}

/// <summary>
/// Gate object for the walled area entrance
/// </summary>
public class GateObject : SceneObject
{
    public GateObject(string name, Vector2 position, bool isBlocking)
        : base(name, position, isBlocking, true)
    {
        InteractionText = isBlocking ? "Gate is closed (Player/Party only)" : "Gate is open";
    }
    
    public override void OnInteract()
    {
        // Toggle gate state
        IsBlocking = !IsBlocking;
        InteractionText = IsBlocking ? "Gate is closed (Player/Party only)" : "Gate is open";
    }
}

/// <summary>
/// Tunnel Boring Machine - Elevator that digs down floors and gives resources
/// Shows animated loading screen while boring
/// </summary>
public class TunnelBoringMachine
{
    public Vector2 Position { get; set; }
    public int CurrentDepth { get; private set; }
    public bool IsBoring { get; private set; }
    public float BoringProgress { get; private set; }
    public string DungeonName { get; private set; }
    
    private float _boringTimePerFloor = 3f; // 3 seconds per floor
    private float _boringTimer;
    private int _targetDepth;
    private Action<int, Dictionary<string, int>> _onFloorCompleted; // Callback with floor and resources
    
    public event Action<int> OnBoringStarted; // Floor number
    public event Action<int, Dictionary<string, int>> OnBoringCompleted; // Floor number, resources
    public event Action<float> OnBoringProgress; // Progress 0-1
    
    public TunnelBoringMachine(Vector2 position, string dungeonName)
    {
        Position = position;
        DungeonName = dungeonName;
        CurrentDepth = 0;
        IsBoring = false;
        BoringProgress = 0f;
    }
    
    /// <summary>
    /// Start boring down to a specific floor
    /// </summary>
    public void StartBoring(int targetFloor)
    {
        if (IsBoring || targetFloor <= CurrentDepth) return;
        
        _targetDepth = targetFloor;
        IsBoring = true;
        _boringTimer = 0f;
        BoringProgress = 0f;
        
        OnBoringStarted?.Invoke(targetFloor);
    }
    
    /// <summary>
    /// Update boring machine animation and progress
    /// </summary>
    public void Update(GameTime gameTime)
    {
        if (!IsBoring) return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _boringTimer += deltaTime;
        
        int floorsTodig = _targetDepth - CurrentDepth;
        float totalTime = floorsTodig * _boringTimePerFloor;
        BoringProgress = Math.Min(_boringTimer / totalTime, 1f);
        
        OnBoringProgress?.Invoke(BoringProgress);
        
        // Check if floor is completed
        int currentFloorBeingBored = CurrentDepth + (int)(_boringTimer / _boringTimePerFloor) + 1;
        if (currentFloorBeingBored > CurrentDepth && currentFloorBeingBored <= _targetDepth)
        {
            // Floor completed - give resources
            var resources = GenerateFloorResources(currentFloorBeingBored);
            CurrentDepth = currentFloorBeingBored;
            OnBoringCompleted?.Invoke(currentFloorBeingBored, resources);
        }
        
        // Check if boring is complete
        if (BoringProgress >= 1f)
        {
            IsBoring = false;
            _boringTimer = 0f;
        }
    }
    
    /// <summary>
    /// Generate resources based on floor depth
    /// </summary>
    private Dictionary<string, int> GenerateFloorResources(int floor)
    {
        var resources = new Dictionary<string, int>();
        System.Random random = new System.Random(floor + DungeonName.GetHashCode());
        
        // Stone is always given
        resources["Stone"] = 10 + (floor * 2);
        
        // Ore chances increase with depth
        if (floor >= 5)
        {
            resources["Copper Ore"] = random.Next(1, 5 + (floor / 5));
        }
        if (floor >= 15)
        {
            resources["Iron Ore"] = random.Next(1, 3 + (floor / 10));
        }
        if (floor >= 30)
        {
            resources["Gold Ore"] = random.Next(1, 2 + (floor / 20));
        }
        if (floor >= 50)
        {
            resources["Diamond"] = random.Next(0, 1 + (floor / 50));
        }
        
        return resources;
    }
    
    /// <summary>
    /// Draw the boring machine
    /// </summary>
    public void Draw(SpriteBatch spriteBatch)
    {
        // TODO: Draw actual boring machine sprite
        // For now, draw placeholder
        Rectangle rect = new Rectangle(
            (int)Position.X,
            (int)Position.Y,
            GameConstants.TILE_SIZE * 3,
            GameConstants.TILE_SIZE * 3
        );
        
        // Draw machine body (placeholder)
        Color machineColor = IsBoring ? Color.Orange : Color.Gray;
        // TODO: Replace with actual texture drawing
        
        // Draw progress bar if boring
        if (IsBoring)
        {
            int barWidth = GameConstants.TILE_SIZE * 3;
            int barHeight = 4;
            Rectangle progressBar = new Rectangle(
                (int)Position.X,
                (int)Position.Y - 10,
                (int)(barWidth * BoringProgress),
                barHeight
            );
            // TODO: Draw progress bar
        }
    }
}
