using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.Core.Scenes;
using MoonBrookRidge.Core;

namespace MoonBrookRidge.World.Interiors;

/// <summary>
/// The player's farmhouse interior with bedroom, kitchen, and living area
/// Now inherits from the new scene-based InteriorScene
/// </summary>
public class FarmhouseInterior : InteriorScene
{
    public FarmhouseInterior() : base("Farmhouse", "farmhouse_interior", 15, 12)
    {
        // Door transition to farm exterior at bottom center
        var doorTransition = new SceneTransition(
            "farmhouse_door",
            new Vector2(7, 11) * GameConstants.TILE_SIZE,
            "farm_exterior",
            new Vector2(125, 125), // Spawn position on farm
            TransitionType.Door,
            requiresInteraction: true
        );
        AddTransition(doorTransition);
    }
    
    /// <summary>
    /// Initialize the farmhouse room layout
    /// </summary>
    public override void Initialize()
    {
        // Initialize base interior (floors and walls)
        base.Initialize();
        
        // Clear door position (no wall at door)
        SetTile(7, 11, new Tile(TileType.WoodenFloor, new Vector2(7, 11)));
        
        AddFurniture();
    }
    
    /// <summary>
    /// Add furniture and decorations to the farmhouse
    /// </summary>
    private void AddFurniture()
    {
        // Bed (player spawns near this) - top right area
        var bed = new SceneObject(
            "Bed",
            new Vector2(10 * GameConstants.TILE_SIZE, 3 * GameConstants.TILE_SIZE),
            isBlocking: true,
            isInteractive: true
        );
        bed.SourceRect = new Rectangle(0, 32, 32, 32);
        bed.InteractionText = "Press X to sleep";
        AddObject(bed);
        
        // Nightstand next to bed
        var nightstand = new SceneObject(
            "Nightstand",
            new Vector2(9 * GameConstants.TILE_SIZE, 3 * GameConstants.TILE_SIZE),
            isBlocking: true,
            isInteractive: false
        );
        nightstand.SourceRect = new Rectangle(32, 16, 16, 16);
        AddObject(nightstand);
        
        // Table in center area
        var table = new SceneObject(
            "Table",
            new Vector2(6 * GameConstants.TILE_SIZE, 6 * GameConstants.TILE_SIZE),
            isBlocking: true,
            isInteractive: false
        );
        table.SourceRect = new Rectangle(0, 48, 32, 16);
        AddObject(table);
        
        // Chairs around table
        var chair1 = new SceneObject(
            "Chair",
            new Vector2(5 * GameConstants.TILE_SIZE, 6 * GameConstants.TILE_SIZE),
            isBlocking: true,
            isInteractive: false
        );
        chair1.SourceRect = new Rectangle(48, 48, 16, 16);
        AddObject(chair1);
        
        var chair2 = new SceneObject(
            "Chair",
            new Vector2(8 * GameConstants.TILE_SIZE, 6 * GameConstants.TILE_SIZE),
            isBlocking: true,
            isInteractive: false
        );
        chair2.SourceRect = new Rectangle(48, 48, 16, 16);
        AddObject(chair2);
        
        // Kitchen counter - left side
        var counter = new SceneObject(
            "Counter",
            new Vector2(2 * GameConstants.TILE_SIZE, 3 * GameConstants.TILE_SIZE),
            isBlocking: true,
            isInteractive: false
        );
        counter.SourceRect = new Rectangle(0, 0, 16, 16);
        AddObject(counter);
        
        // Stove
        var stove = new SceneObject(
            "Stove",
            new Vector2(3 * GameConstants.TILE_SIZE, 3 * GameConstants.TILE_SIZE),
            isBlocking: true,
            isInteractive: true
        );
        stove.SourceRect = new Rectangle(16, 0, 16, 16);
        stove.InteractionText = "Press X to cook";
        AddObject(stove);
        
        // Plant decoration (non-blocking)
        var plant = new SceneObject(
            "Plant",
            new Vector2(2 * GameConstants.TILE_SIZE, 9 * GameConstants.TILE_SIZE),
            isBlocking: false,
            isInteractive: false
        );
        plant.SourceRect = new Rectangle(64, 64, 16, 16);
        AddObject(plant);
        
        // Rug near door (non-blocking)
        var rug = new SceneObject(
            "Rug",
            new Vector2(6 * GameConstants.TILE_SIZE, 9 * GameConstants.TILE_SIZE),
            isBlocking: false,
            isInteractive: false
        );
        rug.SourceRect = new Rectangle(80, 80, 32, 16);
        AddObject(rug);
    }
    
    /// <summary>
    /// Called when player enters farmhouse
    /// </summary>
    public override void OnEnter(Vector2 spawnPosition)
    {
        // Spawn player near the bed (starting position)
        // spawnPosition will be used from transition or default to (7, 5)
    }
}
