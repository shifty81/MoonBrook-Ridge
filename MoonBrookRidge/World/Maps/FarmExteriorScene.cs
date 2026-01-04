using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.Core.Scenes;
using MoonBrookRidge.Core;
using System;

namespace MoonBrookRidge.World.Maps;

/// <summary>
/// Farm exterior scene - the player's farm as a separate scene
/// Replaces the monolithic WorldMap with a scene-based approach
/// </summary>
public class FarmExteriorScene : ExteriorScene
{
    private Vector2 _farmhousePosition;
    
    public Vector2 FarmhousePosition => _farmhousePosition;
    
    public FarmExteriorScene(int width = 250, int height = 250) 
        : base("Farm", "farm_exterior", width, height)
    {
        // Farmhouse is at the center of the farm
        _farmhousePosition = new Vector2(125, 125);
    }
    
    public override void Initialize()
    {
        base.Initialize();
        
        // Create farmable terrain
        GenerateFarmTerrain();
        
        // Add farmhouse entrance
        AddFarmhouseEntrance();
        
        // Add decorative elements
        AddFarmScenery();
    }
    
    private void GenerateFarmTerrain()
    {
        Random random = new Random(42); // Fixed seed for consistency
        
        int centerX = _width / 2;
        int centerY = _height / 2;
        int farmRadius = 50; // 100x100 flat farmable area in center
        
        for (int x = 0; x < _width; x++)
        {
            for (int y = 0; y < _height; y++)
            {
                TileType tileType;
                
                // Calculate distance from center
                int distanceFromCenter = Math.Max(Math.Abs(x - centerX), Math.Abs(y - centerY));
                
                // Central farm area (100x100) - flat, mostly grass for farming
                bool isCentralFarm = distanceFromCenter <= farmRadius;
                
                if (isCentralFarm)
                {
                    // Flat farmable terrain in the center
                    int variant = random.Next(10);
                    if (variant < 6)
                        tileType = TileType.Grass;      // 60% base grass
                    else if (variant < 9)
                        tileType = TileType.Grass01;   // 30% light grass
                    else
                        tileType = TileType.Grass02;   // 10% medium grass
                }
                // Outer expansion areas - still farmable but with more variety
                else if (distanceFromCenter <= farmRadius + 25)
                {
                    // Near-farm expansion zone (50 tiles out from center farm)
                    int variant = random.Next(15);
                    if (variant < 5)
                        tileType = TileType.Grass;
                    else if (variant < 9)
                        tileType = TileType.Grass01;
                    else if (variant < 12)
                        tileType = TileType.Grass02;
                    else
                        tileType = TileType.Grass03;   // Some darker grass
                }
                // Far expansion areas - purchasable plots
                else
                {
                    // Outer wilderness - can be unlocked/purchased later
                    int variant = random.Next(20);
                    if (variant < 7)
                        tileType = TileType.Grass;
                    else if (variant < 12)
                        tileType = TileType.Grass01;
                    else if (variant < 16)
                        tileType = TileType.Grass02;
                    else
                        tileType = TileType.Grass03;
                }
                
                SetTile(x, y, new Tile(tileType, new Vector2(x, y)));
            }
        }
        
        // Add water features
        AddWaterFeatures(random, centerX, centerY);
    }
    
    private void AddWaterFeatures(Random random, int centerX, int centerY)
    {
        // Pond in the northeast
        for (int x = centerX + 60; x < centerX + 80 && x < _width; x++)
        {
            for (int y = centerY - 80; y < centerY - 60 && y >= 0; y++)
            {
                SetTile(x, y, new Tile(TileType.Water, new Vector2(x, y)));
            }
        }
        
        // Sand around the pond
        for (int x = centerX + 58; x < centerX + 82 && x < _width; x++)
        {
            for (int y = centerY - 82; y < centerY - 58 && y >= 0; y++)
            {
                var tile = GetTile(x, y);
                if (tile != null && tile.Type != TileType.Water)
                {
                    SetTile(x, y, new Tile(
                        random.Next(2) == 0 ? TileType.Sand : TileType.Sand01,
                        new Vector2(x, y)
                    ));
                }
            }
        }
    }
    
    private void AddFarmhouseEntrance()
    {
        // Place farmhouse at center
        int houseX = (int)_farmhousePosition.X;
        int houseY = (int)_farmhousePosition.Y;
        
        // Mark farmhouse footprint (3x3 tiles)
        for (int dx = -1; dx <= 1; dx++)
        {
            for (int dy = -1; dy <= 1; dy++)
            {
                int x = houseX + dx;
                int y = houseY + dy;
                if (x >= 0 && x < _width && y >= 0 && y < _height)
                {
                    SetTile(x, y, new Tile(TileType.WoodenFloor, new Vector2(x, y)));
                }
            }
        }
        
        // Add transition to farmhouse interior
        var farmhouseDoor = new SceneTransition(
            "farmhouse_door_exterior",
            _farmhousePosition * GameConstants.TILE_SIZE,
            "farmhouse_interior",
            new Vector2(7, 10), // Spawn just inside the door
            TransitionType.Door,
            requiresInteraction: true
        );
        farmhouseDoor.InteractionText = "Press X to enter farmhouse";
        
        AddTransition(farmhouseDoor);
        
        // Add farmhouse object
        var farmhouseObj = new SceneObject(
            "Farmhouse",
            _farmhousePosition * GameConstants.TILE_SIZE,
            isBlocking: true,
            isInteractive: false
        );
        AddObject(farmhouseObj);
    }
    
    private void AddFarmScenery()
    {
        Random random = new Random(123);
        
        // Add some decorative rocks and trees around the edges
        int sceneryCount = 50;
        
        for (int i = 0; i < sceneryCount; i++)
        {
            int x = random.Next(_width);
            int y = random.Next(_height);
            
            // Don't place near center (farm area)
            if (Math.Abs(x - _width / 2) < 60 && Math.Abs(y - _height / 2) < 60)
                continue;
            
            Vector2 position = new Vector2(x * GameConstants.TILE_SIZE, y * GameConstants.TILE_SIZE);
            
            string[] sceneryTypes = { "Tree", "Rock", "Bush" };
            string sceneryType = sceneryTypes[random.Next(sceneryTypes.Length)];
            
            var sceneryObj = new SceneObject(
                sceneryType,
                position,
                isBlocking: sceneryType != "Bush",
                isInteractive: sceneryType == "Tree" // Trees can be chopped
            );
            
            if (sceneryType == "Tree")
            {
                sceneryObj.InteractionText = "Press C to chop";
            }
            
            AddObject(sceneryObj);
        }
    }
    
    public override void OnEnter(Vector2 spawnPosition)
    {
        // Player enters farm (usually from farmhouse)
        base.OnEnter(spawnPosition);
    }
}
