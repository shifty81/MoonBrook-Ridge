# MoonBrookRidge.Engine - MonoGame Compatibility Layer

This project provides a MonoGame-compatible API wrapper for the custom MoonBrookEngine, enabling the existing MoonBrook Ridge game code to run on the custom engine with minimal to no changes.

## Overview

The compatibility layer wraps MoonBrookEngine types and provides MonoGame-compatible APIs, allowing for a gradual migration from MonoGame to the custom engine.

## Features

### Implemented ✅

#### Core Types
- **Vector2** - Full 2D vector math with implicit conversions
- **Color** - RGBA color with common presets
- **Rectangle** - 2D rectangle with intersection/collision methods
- **GameTime** - Time tracking for game loop

#### Graphics
- **Texture2D** - Texture wrapper with automatic loading
- **SpriteBatch** - Batched sprite rendering with all MonoGame overloads
- **GraphicsDevice** - Graphics device wrapper
- **GraphicsDeviceManager** - Graphics settings management
- **Viewport** - Viewport information

#### Content Loading
- **ContentManager** - Asset loading with caching
- **ResourceManager** - Underlying resource management (in MoonBrookEngine)

#### Game Framework
- **Game** - Base game class with standard lifecycle methods
  - `Initialize()` - Called once on startup
  - `LoadContent()` - Called after Initialize
  - `Update(GameTime)` - Called every frame for logic
  - `Draw(GameTime)` - Called every frame for rendering
  - `UnloadContent()` - Called on shutdown

### Not Yet Implemented ⏸️

- **SpriteFont** - Font rendering (stub only)
- **Input** - Keyboard/Mouse state wrapper (engine has InputManager)
- **Audio** - Sound and music playback
- **Effect/Shader** - Custom shader support

## Usage

### Simple Example

```csharp
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MyGame;

public class MyGame : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Texture2D _playerTexture;
    
    public MyGame()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        IsMouseVisible = true;
    }
    
    protected override void Initialize()
    {
        base.Initialize();
    }
    
    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        _playerTexture = Content.Load<Texture2D>("player");
    }
    
    protected override void Update(GameTime gameTime)
    {
        // Your update logic here
        base.Update(gameTime);
    }
    
    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.CornflowerBlue);
        
        _spriteBatch.Begin();
        _spriteBatch.Draw(_playerTexture, new Vector2(100, 100), Color.White);
        _spriteBatch.End();
        
        base.Draw(gameTime);
    }
}

class Program
{
    static void Main()
    {
        using var game = new MyGame();
        game.Run();
    }
}
```

### Migration from MonoGame

1. **Update Project References**
   ```xml
   <ItemGroup>
     <!-- Remove MonoGame -->
     <!-- <PackageReference Include="MonoGame.Framework.DesktopGL" Version="3.8.*" /> -->
     
     <!-- Add MoonBrookEngine -->
     <ProjectReference Include="..\MoonBrookRidge.Engine\MoonBrookRidge.Engine.csproj" />
   </ItemGroup>
   ```

2. **Update Using Statements**
   ```csharp
   // Old
   using Microsoft.Xna.Framework;
   using Microsoft.Xna.Framework.Graphics;
   
   // New
   using MoonBrookRidge.Engine.MonoGameCompat;
   ```

3. **Update Game Constructor (if needed)**
   ```csharp
   public Game1()
   {
       _graphics = new GraphicsDeviceManager(this);
       Content.RootDirectory = "Content";
       IsMouseVisible = true;
   }
   ```

4. **Build and Test**
   - Most code should work unchanged
   - SpriteBatch API is fully compatible
   - Texture loading works the same way
   - GameTime and basic types are compatible

## API Compatibility

### SpriteBatch Methods

All standard MonoGame SpriteBatch methods are supported:

```csharp
// Simple draw
spriteBatch.Draw(texture, position, color);

// With destination rectangle
spriteBatch.Draw(texture, destRect, color);

// With source rectangle
spriteBatch.Draw(texture, destRect, sourceRect, color);

// Full overload with rotation, origin, scale
spriteBatch.Draw(texture, position, sourceRect, color, 
                rotation, origin, scale, effects, layerDepth);
```

### Type Conversions

All wrapper types have implicit conversions to/from engine types:

```csharp
// These conversions happen automatically
Vector2 compatVec = new Vector2(10, 20);
MoonBrookEngine.Math.Vector2 engineVec = compatVec; // Implicit conversion

Color compatColor = Color.Red;
MoonBrookEngine.Math.Color engineColor = compatColor; // Implicit conversion
```

## Performance

The compatibility layer adds minimal overhead:
- Type conversions use implicit operators (zero cost)
- SpriteBatch delegates directly to engine implementation
- Texture loading uses the engine's ResourceManager with caching
- No boxing/unboxing of value types

## Known Limitations

1. **Font Rendering** - Not yet implemented (SpriteFont is a stub)
2. **Dynamic Resolution Changes** - Settings only applied at startup
3. **Some Input Methods** - Mouse/Keyboard state not wrapped yet (use engine's InputManager)
4. **Effects/Shaders** - Custom shaders not exposed through compatibility layer
5. **Audio** - Not yet implemented

## Project Structure

```
MoonBrookRidge.Engine/
├── MonoGameCompat/
│   ├── Game.cs                    # Base game class
│   ├── GraphicsDevice.cs          # Graphics device wrapper
│   ├── GraphicsDeviceManager.cs   # Graphics settings
│   ├── SpriteBatch.cs             # Sprite rendering
│   ├── Texture2D.cs               # Texture wrapper
│   ├── ContentManager.cs          # Asset loading
│   ├── SpriteFont.cs              # Font stub
│   ├── Vector2.cs                 # 2D vector
│   ├── Color.cs                   # RGBA color
│   └── Rectangle.cs               # 2D rectangle
└── MoonBrookRidge.Engine.csproj
```

## Dependencies

- **MoonBrookEngine** - The custom game engine
- **Silk.NET** (transitive) - Windowing and OpenGL
- **.NET 9.0**

## Future Enhancements

### High Priority
- Font rendering with bitmap fonts
- Input wrapper (Keyboard/Mouse state)
- Audio system integration

### Medium Priority
- Effect/Shader wrapper
- Dynamic resolution/fullscreen changes
- Additional helper methods

### Low Priority
- Render target support
- Advanced blending modes
- Particle system wrapper

## Contributing

When extending the compatibility layer:
1. Maintain MonoGame API compatibility
2. Add minimal overhead
3. Use implicit conversions for types
4. Document any differences from MonoGame
5. Add usage examples

## See Also

- [MoonBrookEngine README](../MoonBrookEngine/README.md) - Core engine documentation
- [CUSTOM_ENGINE_IMPLEMENTATION.md](../CUSTOM_ENGINE_IMPLEMENTATION.md) - Implementation plan
- [ENGINE_WEEK3_SUMMARY.md](../ENGINE_WEEK3_SUMMARY.md) - Recent engine progress
