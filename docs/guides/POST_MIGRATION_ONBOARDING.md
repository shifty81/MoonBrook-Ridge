# Post-Migration Developer Onboarding Guide

**Date**: January 5, 2026  
**For**: New and returning developers post-custom engine migration  
**Status**: ✅ Engine Migration Complete - Ready for Development

---

## Welcome!

If you're reading this, the MoonBrook Ridge game has successfully migrated from MonoGame to a **custom game engine** with full MonoGame API compatibility. This guide will help you understand the current state of the project and how to continue development.

---

## What Changed in the Migration

### Before Migration
- ✅ Game used **MonoGame Framework** (external dependency)
- ✅ MonoGame handled graphics, input, content loading
- ✅ Game code called MonoGame APIs directly

### After Migration
- ✅ Game uses **MoonBrook Custom Engine** (internal)
- ✅ Custom engine implements MonoGame-compatible APIs
- ✅ Game code remains unchanged (API compatibility)
- ✅ Engine built on **SDL2** + **OpenGL** + **StbImageSharp**

### Why This Matters
1. **Full Control**: We own the engine and can customize it
2. **No External Dependencies**: No MonoGame NuGet package needed
3. **Learning Opportunity**: Understand game engine internals
4. **API Compatibility**: Existing game code works without changes
5. **Future Flexibility**: Can add custom features beyond MonoGame

---

## Project Structure

```
MoonBrook-Ridge/
├── MoonBrookEngine/              # Low-level engine (SDL2, OpenGL)
│   ├── Core/                     # Core engine systems
│   ├── Graphics/                 # Rendering pipeline
│   ├── Input/                    # Keyboard and mouse
│   └── Content/                  # Resource loading
│
├── MoonBrookRidge.Engine/        # MonoGame compatibility layer
│   ├── MonoGameCompat/           # MonoGame API implementations
│   │   ├── Game.cs              # Game loop and lifecycle
│   │   ├── GraphicsDevice.cs    # Graphics device wrapper
│   │   ├── SpriteBatch.cs       # 2D rendering
│   │   ├── Texture2D.cs         # Texture handling
│   │   ├── ContentManager.cs    # Content loading
│   │   ├── Color.cs             # Color utilities
│   │   ├── Vector2.cs           # Math utilities
│   │   ├── Keyboard.cs          # Keyboard input
│   │   └── Mouse.cs             # Mouse input
│   └── ... (all MonoGame APIs)
│
├── MoonBrookRidge/               # Game code
│   ├── Core/                     # Core game systems
│   ├── Characters/               # Player and NPCs
│   ├── World/                    # Maps and tiles
│   ├── Farming/                  # Farming mechanics
│   ├── Combat/                   # Combat systems
│   ├── UI/                       # User interface
│   └── Content/                  # Game assets
│
├── validate-engine.sh            # Build validation script
├── play.sh                       # Game launcher
└── README.md                     # Project overview
```

---

## Key Concepts

### 1. Engine Architecture

The engine has three layers:

```
┌─────────────────────────────────────────┐
│     Game Code (MoonBrookRidge)          │
│  Uses MonoGame APIs (unchanged)         │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│  Compatibility Layer (MoonBrookRidge.Engine)
│  Implements MonoGame APIs               │
└───────────────┬─────────────────────────┘
                │
┌───────────────▼─────────────────────────┐
│  Core Engine (MoonBrookEngine)          │
│  SDL2 + OpenGL + Platform Code          │
└─────────────────────────────────────────┘
```

### 2. MonoGame Compatibility

The compatibility layer implements these MonoGame classes:
- `Game` - Game loop and lifecycle
- `GraphicsDevice` - Graphics abstraction
- `SpriteBatch` - 2D sprite rendering
- `Texture2D` - Texture management
- `ContentManager` - Asset loading
- `Color`, `Vector2`, `Rectangle`, `Point` - Math and utilities
- `Keyboard`, `Mouse`, `KeyboardState`, `MouseState` - Input
- `GameTime` - Frame timing
- `SamplerState`, `BlendState`, `RasterizerState` - Render states

### 3. Content Pipeline

Content loading now works through the custom engine:
```csharp
// Still works the same way!
Texture2D texture = Content.Load<Texture2D>("Sprites/player");
SpriteFont font = Content.Load<SpriteFont>("Fonts/Arial");
```

Under the hood:
- Textures loaded via **StbImageSharp** (PNG, JPEG, BMP)
- Fonts loaded from TTF files
- No need for .xnb files anymore!

---

## Development Workflow

### 1. Making Code Changes

Game code (in `MoonBrookRidge/`) works exactly as before:

```csharp
// This still works!
using MoonBrookRidge.Engine.MonoGameCompat;

public class MyGameState : GameState
{
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(texture, position, Color.White);
    }
}
```

**No changes needed** to game code due to API compatibility!

### 2. Building the Project

```bash
# Build all projects
dotnet build

# Or build specific project
cd MoonBrookRidge
dotnet build
```

### 3. Running the Game

```bash
# Quick launch
./play.sh

# Or manual launch
cd MoonBrookRidge
dotnet run
```

### 4. Validating Changes

```bash
# Run validation before committing
./validate-engine.sh
```

This checks:
- ✅ Build succeeds
- ✅ No errors or warnings
- ✅ All critical files present
- ✅ Engine implementations complete

---

## Common Development Tasks

### Adding New Game Features

**Process remains the same:**

1. Create/modify files in `MoonBrookRidge/`
2. Use MonoGame APIs as before
3. Build and test
4. Commit changes

**Example**: Adding a new enemy type
```csharp
// MoonBrookRidge/Characters/Enemies/Slime.cs
using MoonBrookRidge.Engine.MonoGameCompat;

public class Slime : Enemy
{
    public Slime(Texture2D texture, Vector2 position)
        : base(texture, position, health: 50)
    {
    }
    
    public override void Update(GameTime gameTime)
    {
        // Slime AI logic
        base.Update(gameTime);
    }
    
    public override void Draw(SpriteBatch spriteBatch)
    {
        spriteBatch.Draw(Texture, Position, Color.Green);
    }
}
```

### Adding New Engine Features

**If you need to add engine-level features:**

1. Decide if it belongs in:
   - `MoonBrookEngine/` - Low-level (SDL2, OpenGL)
   - `MoonBrookRidge.Engine/` - MonoGame compatibility

2. Implement the feature
3. Update compatibility layer if needed
4. Test thoroughly
5. Document the addition

**Example**: Adding a new MonoGame API
```csharp
// MoonBrookRidge.Engine/MonoGameCompat/SoundEffect.cs
public class SoundEffect
{
    public static SoundEffect FromFile(string path)
    {
        // Implementation using engine audio system
    }
    
    public void Play()
    {
        // Play sound
    }
}
```

### Working with Assets

**Asset loading is simplified:**

1. Place asset files in `MoonBrookRidge/Content/`
2. Organize by type:
   ```
   Content/
   ├── Textures/
   │   ├── Characters/
   │   ├── Tiles/
   │   └── UI/
   ├── Fonts/
   └── Audio/
   ```
3. Load in code:
   ```csharp
   var playerTexture = Content.Load<Texture2D>("Textures/Characters/player");
   var uiFont = Content.Load<SpriteFont>("Fonts/Arial");
   ```

**No Content Pipeline build needed** - assets loaded directly!

---

## Testing Your Changes

### 1. Build Testing
```bash
# Always validate before testing
./validate-engine.sh

# Should show:
# ✅ All critical checks passed!
# Build Status: Build succeeded.
```

### 2. Runtime Testing
```bash
# Launch game
./play.sh

# Follow testing guide
cat docs/guides/RUNTIME_TESTING_GUIDE.md
```

### 3. What to Check
- Does game launch without crashes?
- Do your changes work as expected?
- No visual regressions?
- Performance still good (60 FPS)?
- No console errors?

---

## Troubleshooting

### Build Errors

**"Type or namespace not found"**
```bash
# Check using statements
using MoonBrookRidge.Engine.MonoGameCompat;

# Not Microsoft.Xna.Framework!
```

**"Method not found"**
- Check if the MonoGame API is implemented
- See `MoonBrookRidge.Engine/MonoGameCompat/`
- Add implementation if missing

### Runtime Errors

**"Content not found"**
```bash
# Check file exists
ls MoonBrookRidge/Content/Textures/player.png

# Check path in Load call
Content.Load<Texture2D>("Textures/player") # No extension!
```

**"OpenGL error" or graphics issues**
- Update graphics drivers
- Check OpenGL version (need 3.0+)
- Run on dedicated GPU if available

### Performance Issues

**Low FPS**
- Check with performance monitor (F3 key)
- Profile with built-in tools
- Look for unnecessary allocations
- Consider spatial partitioning improvements

---

## Best Practices

### Code Style

1. **Follow existing patterns** in game code
2. **Use MonoGame APIs** when possible
3. **Document engine changes** thoroughly
4. **Test on multiple platforms** if possible

### Git Workflow

1. **Validate before commit**:
   ```bash
   ./validate-engine.sh
   ```

2. **Write clear commit messages**:
   ```
   Add slime enemy type with basic AI
   
   - Created Slime.cs with movement logic
   - Added slime sprite loading
   - Integrated into enemy spawning system
   ```

3. **Keep commits focused** - one feature per commit

### Performance

1. **Avoid allocations in Update/Draw loops**
   ```csharp
   // Bad - allocates every frame
   public void Update(GameTime gameTime)
   {
       var enemies = GetEnemies(); // New list allocation!
   }
   
   // Good - reuse collection
   private List<Enemy> _enemies = new();
   public void Update(GameTime gameTime)
   {
       GetEnemies(_enemies); // Reuses list
   }
   ```

2. **Use object pooling** for frequently created objects
3. **Profile before optimizing** - use F3 performance monitor

---

## What's Next?

Now that the engine migration is complete, development can continue on:

### Immediate Priorities
1. **Runtime Testing** - Verify everything works in practice
2. **Bug Fixes** - Address any issues found during testing
3. **Performance Tuning** - Optimize if needed

### Future Development
1. **New Game Features** - Continue implementing roadmap
2. **Engine Enhancements** - Add custom capabilities
3. **Platform Support** - Test on Windows, Linux, macOS
4. **Content Creation** - Add more assets, levels, features

### Long-term Goals
1. **Complete MonoGame API Coverage** - Implement any missing APIs as needed
2. **Advanced Graphics** - Shaders, particles, lighting
3. **Audio System** - Complete sound and music support
4. **Networking** - Multiplayer support (if desired)

---

## Resources

### Documentation
- [README.md](../../README.md) - Project overview and features
- [RUNTIME_TESTING_GUIDE.md](RUNTIME_TESTING_GUIDE.md) - Testing procedures
- [RUNTIME_TESTING_PREPARATION.md](../../RUNTIME_TESTING_PREPARATION.md) - Testing preparation
- [ENGINE_MIGRATION_STATUS.md](../../ENGINE_MIGRATION_STATUS.md) - Migration details
- [ARCHITECTURE.md](../architecture/ARCHITECTURE.md) - System architecture
- [DEVELOPMENT.md](DEVELOPMENT.md) - Development guide
- [CONTROLS.md](CONTROLS.md) - Game controls reference

### Code References
- `MoonBrookEngine/` - Core engine implementation
- `MoonBrookRidge.Engine/MonoGameCompat/` - MonoGame API layer
- `MoonBrookRidge/` - Game code examples

### External Resources
- [SDL2 Documentation](https://wiki.libsdl.org/) - For engine work
- [OpenGL Reference](https://www.khronos.org/opengl/) - For graphics
- [MonoGame API Reference](https://docs.monogame.net/api/) - For compatibility reference

---

## Getting Help

### For Engine Issues
1. Check `MoonBrookRidge.Engine/MonoGameCompat/` for implementations
2. Review `MoonBrookEngine/` for low-level code
3. Create a GitHub issue with `engine` label

### For Game Issues
1. Check existing game code for examples
2. Review relevant system documentation
3. Create a GitHub issue with appropriate label

### For General Questions
1. Check documentation first
2. Review recent PRs for examples
3. Ask in GitHub discussions

---

## Summary

**You're now ready to develop!** The custom engine migration is complete, and the project is ready for continued development. Game code works exactly as before, thanks to MonoGame API compatibility. Follow this guide for smooth development, and don't hesitate to improve the engine as needed.

**Welcome to post-migration MoonBrook Ridge development!** 🎮🌾

---

**Date**: January 5, 2026  
**Status**: ✅ Ready for Development  
**Next Step**: Runtime testing and continued feature development
