# Development Guide for MoonBrook Ridge

## Quick Start

### 1. Prerequisites
- .NET 9.0 SDK or later
- Git
- IDE: Visual Studio, VS Code, or JetBrains Rider

### 2. Clone and Build
```bash
git clone https://github.com/shifty81/MoonBrook-Ridge.git
cd MoonBrook-Ridge/MoonBrookRidge
dotnet restore
dotnet build
dotnet run
```

## Project Structure

```
MoonBrookRidge/
├── Core/                   # Core game systems
│   ├── Components/         # Shared components
│   ├── States/            # Game state management
│   └── Systems/           # Time, Camera, etc.
├── Characters/            # Player and NPC systems
├── World/                 # Map and tile systems
├── Farming/               # Farming mechanics
├── Items/                 # Inventory and crafting
├── UI/                    # User interface
├── Content/               # Game assets
├── Game1.cs              # Main game class
└── Program.cs            # Entry point
```

## Adding New Features

### Adding a New Game State

1. Create a new class in `Core/States/`
2. Inherit from `GameState`
3. Implement required methods:
   - `Initialize()`
   - `LoadContent()`
   - `Update(GameTime)`
   - `Draw(SpriteBatch)`

Example:
```csharp
public class MenuState : GameState
{
    public MenuState(Game1 game) : base(game) { }
    
    public override void Update(GameTime gameTime)
    {
        // Handle menu input
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        // Draw menu UI
    }
}
```

### Adding a New NPC

1. Create NPC instance:
```csharp
var emma = new NPCCharacter("Emma", new Vector2(200, 150));
```

2. Set up schedule:
```csharp
emma.Schedule.AddScheduleEntry(6.0f, new ScheduleLocation
{
    Position = new Vector2(200, 150),
    LocationName = "Home",
    Activity = "Wake up"
});
```

3. Add dialogue tree:
```csharp
var greeting = new DialogueNode("Hello! How are you today?", "Emma");
greeting.AddOption("I'm doing great!", responseNode1);
greeting.AddOption("Not so good...", responseNode2);
emma.AddDialogueTree("greeting", new DialogueTree(greeting));
```

### Adding a New Crop

1. Define crop in code:
```csharp
public static class CropDefinitions
{
    public static Crop CreateTomato()
    {
        return new Crop("Tomato", maxStages: 5, hoursPerStage: 24);
    }
}
```

2. Add sprites for each growth stage
3. Add seed item to shop

### Adding a New Tool

1. Create tool class:
```csharp
public class Hammer : Tool
{
    public Hammer(int tier = 1) : base("Hammer", tier) { }
    
    public override void Use(Vector2 position)
    {
        // Implement tool behavior
    }
}
```

2. Add to player's starting inventory or shop

## Working with Sprites

### Loading Sprites
```csharp
// In LoadContent()
Texture2D playerSprite = Content.Load<Texture2D>("Sprites/player_walk");
```

### Drawing Sprites
```csharp
spriteBatch.Draw(
    texture: playerSprite,
    position: new Vector2(100, 100),
    sourceRectangle: null,
    color: Color.White,
    rotation: 0f,
    origin: Vector2.Zero,
    scale: 1f,
    effects: SpriteEffects.None,
    layerDepth: 0f
);
```

### Animating Sprites
```csharp
// For a sprite strip with 8 frames
int frameWidth = texture.Width / 8;
int currentFrame = 0;
Rectangle sourceRect = new Rectangle(
    currentFrame * frameWidth, 
    0, 
    frameWidth, 
    texture.Height
);
```

## Debugging Tips

### Print to Console
```csharp
System.Diagnostics.Debug.WriteLine($"Player position: {player.Position}");
```

### Visual Debugging
```csharp
// Draw collision boxes
spriteBatch.DrawRectangle(collisionBox, Color.Red);
```

### Performance Monitoring
```csharp
// Check frame time
double frameTime = gameTime.ElapsedGameTime.TotalMilliseconds;
if (frameTime > 16.67) // 60 FPS threshold
{
    Debug.WriteLine($"Slow frame: {frameTime}ms");
}
```

## Common Tasks

### Change Window Size
Edit `Game1.cs` constructor:
```csharp
_graphics.PreferredBackBufferWidth = 1920;
_graphics.PreferredBackBufferHeight = 1080;
```

### Add Fullscreen Mode
```csharp
_graphics.IsFullScreen = true;
_graphics.ApplyChanges();
```

### Change Game Speed
Modify `TimeSystem.cs`:
```csharp
private const float MINUTES_PER_GAME_HOUR = 5f; // Slower game time
```

### Add New Item Type
Add to `ItemType` enum in `InventorySystem.cs`:
```csharp
public enum ItemType
{
    Tool,
    Seed,
    Crop,
    Furniture,  // New type
    // ...
}
```

## Testing

### Manual Testing Checklist
- [ ] Player can move in all directions
- [ ] Camera follows player smoothly
- [ ] Time advances correctly
- [ ] Season changes after 28 days
- [ ] HUD displays correctly
- [ ] Inventory operations work
- [ ] Crops grow when watered
- [ ] NPCs appear and move
- [ ] Dialogue wheel appears on interaction

### Unit Testing (Future)
```csharp
[Test]
public void TestInventoryStacking()
{
    var inventory = new InventorySystem(10);
    var item = new Item("Wood", ItemType.Crafting, maxStack: 99);
    
    Assert.True(inventory.AddItem(item, 50));
    Assert.True(inventory.AddItem(item, 30));
    Assert.Equal(80, inventory.GetItemCount("Wood"));
}
```

## Code Style

### Naming Conventions
- Classes: `PascalCase`
- Methods: `PascalCase`
- Private fields: `_camelCase`
- Public properties: `PascalCase`
- Local variables: `camelCase`
- Constants: `UPPER_CASE`

### Documentation
Add XML documentation to public APIs:
```csharp
/// <summary>
/// Plants a crop at the specified tile position
/// </summary>
/// <param name="crop">The crop to plant</param>
/// <param name="position">Grid position to plant at</param>
/// <returns>True if planting was successful</returns>
public bool PlantCrop(Crop crop, Vector2 position)
{
    // ...
}
```

## Performance Guidelines

### Do's
✅ Use `SpriteBatch` for all 2D rendering
✅ Batch draw calls by texture
✅ Use `SamplerState.PointClamp` for pixel art
✅ Cache frequently used calculations
✅ Use object pooling for particles
✅ Profile before optimizing

### Don'ts
❌ Don't create new objects every frame
❌ Don't use LINQ in Update() loops
❌ Don't draw outside camera view
❌ Don't use texture.GetData() frequently
❌ Don't ignore performance warnings

## Git Workflow

### Branching Strategy
- `main` - Stable releases
- `develop` - Development branch
- `feature/*` - New features
- `bugfix/*` - Bug fixes

### Commit Messages
```
feat: Add fishing minigame
fix: Correct crop growth timing
docs: Update sprite guide
refactor: Simplify dialogue system
test: Add inventory unit tests
```

## Useful Resources

### MonoGame
- [Official Documentation](https://docs.monogame.net/)
- [Community Forums](https://community.monogame.net/)
- [GitHub Repository](https://github.com/MonoGame/MonoGame)

### Game Development
- [Game Programming Patterns](https://gameprogrammingpatterns.com/)
- [Red Blob Games](https://www.redblobgames.com/) - Algorithms
- [GDC Vault](https://www.gdcvault.com/) - Conference talks

### Pixel Art
- [Lospec Palette List](https://lospec.com/palette-list)
- [Pixel Art Tutorial](https://www.pixilart.com/blog/pixel-art-for-beginners)

## Troubleshooting

### "Content not found" Error
1. Verify file is in Content folder
2. Check Content.mgcb includes the file
3. Ensure Build Action is set to "Build"
4. Rebuild content project

### Sprite Looks Blurry
Use `SamplerState.PointClamp`:
```csharp
spriteBatch.Begin(samplerState: SamplerState.PointClamp);
```

### Game Runs Slowly
1. Profile with `dotnet-trace`
2. Check for expensive operations in Update()
3. Reduce draw calls
4. Optimize collision detection

### Input Not Working
1. Check keyboard state polling
2. Verify key mappings
3. Ensure window has focus

## Next Steps

### Immediate Priorities
1. Load actual sprites from asset pack
2. Implement sprite animation system
3. Add font and improve HUD rendering
4. Create main menu state
5. Implement NPC pathfinding

### Future Features
- Save/load system
- Mining caves with procedural generation
- Fishing minigame
- Shop system
- Events and festivals
- Marriage and relationships
- Achievements

## Getting Help

- Check documentation: `README.md`, `ARCHITECTURE.md`, `SPRITE_GUIDE.md`
- Review example code in existing systems
- Ask questions in GitHub Issues
- Join MonoGame community forums

---

Happy coding! 🎮
