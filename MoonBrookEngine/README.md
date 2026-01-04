# MoonBrook Engine

A custom 2D game engine built with C# and Silk.NET for the MoonBrook Ridge game project.

## Overview

MoonBrook Engine is a lightweight, high-performance 2D game engine designed specifically for MoonBrook Ridge. It provides a clean, modular architecture with full control over rendering, input, and game loop management.

## Features

### Core Engine (✅ Implemented)
- **Window Management**: Silk.NET-based windowing with resizing support
- **OpenGL Rendering**: OpenGL 3.3+ with modern rendering pipeline
- **Game Loop**: Fixed timestep update loop with VSync support
- **Input System**: Keyboard and mouse input via Silk.NET.Input
- **Event System**: Initialize, Update, Render, and Shutdown events

### Graphics (✅ Implemented)
- **Texture Loading**: PNG/JPG loading via StbImageSharp
- **Shader System**: GLSL shader compilation and uniform management
- **2D Rendering**: Vertex array objects and buffer management
- **Color System**: RGBA color with normalization and common presets

### In Progress
- Sprite Batching (reduces draw calls by 10x)
- Camera2D with zoom and pan
- Particle System
- Audio System

## Technology Stack

- **Language**: C# .NET 9.0
- **Graphics**: Silk.NET.OpenGL 2.21.0 (OpenGL 3.3+)
- **Windowing**: Silk.NET.Windowing 2.21.0
- **Input**: Silk.NET.Input 2.21.0
- **Image Loading**: StbImageSharp 2.27.14

## Project Structure

```
MoonBrookEngine/
├── Core/
│   ├── Engine.cs          # Main engine class
│   └── GameTime.cs        # Time management
├── Graphics/
│   ├── Texture2D.cs       # Texture loading and management
│   ├── Shader.cs          # GLSL shader compilation
│   └── (SpriteBatch.cs)   # TODO: Batch rendering
├── Input/
│   └── (InputManager.cs)  # TODO: Input abstraction
└── Math/
    └── Color.cs           # RGBA color type
```

## Getting Started

### Prerequisites
- .NET 9.0 SDK
- OpenGL 3.3+ compatible graphics card

### Building the Engine

```bash
cd MoonBrookEngine
dotnet build
```

### Running the Test Application

```bash
cd MoonBrookEngine.Test
dotnet run
```

The test application will:
1. Create a 1280x720 window
2. Initialize OpenGL
3. Render a colored quad (red, green, blue, yellow corners)
4. Display FPS in console
5. Close on ESC key

## Example Usage

```csharp
using MoonBrookEngine.Core;
using MoonBrookEngine.Graphics;

var engine = new Engine("My Game", 1280, 720);

engine.OnInitialize += () =>
{
    // Load textures, shaders, etc.
};

engine.OnUpdate += (gameTime) =>
{
    // Update game logic
    // gameTime.DeltaTime - seconds since last frame
    // gameTime.FPS - current frames per second
};

engine.OnRender += (gameTime) =>
{
    // Render your game
};

engine.Run();
```

## Performance Targets

| Metric | Current | Target | Status |
|--------|---------|--------|--------|
| Window Creation | <100ms | <100ms | ✅ |
| Texture Loading | ~5ms | <10ms | ✅ |
| FPS (Empty Scene) | 60 FPS | 60+ FPS | ✅ |
| Draw Calls | N/A | <100 | 🚧 |
| Memory Usage | ~50 MB | <100 MB | ✅ |

## Roadmap

### Week 1 (✅ Complete)
- [x] Engine foundation (window, rendering context)
- [x] Texture loading from files
- [x] Shader compilation and management
- [x] Basic rendering (colored quads)
- [x] Input system integration
- [x] Test application

### Week 2-3 (In Progress)
- [ ] SpriteBatch with automatic batching
- [ ] Camera2D with zoom and pan
- [ ] Rectangle and Vector2 math types
- [ ] Font rendering (TrueType via FreeType)
- [ ] Performance profiling

### Week 4 (Planned)
- [ ] MonoGame compatibility layer
- [ ] Basic particle system
- [ ] Audio system (OpenAL)
- [ ] Resource manager

## Compatibility with MonoGame

One of the goals is to provide a compatibility layer so existing MoonBrookRidge code can work with minimal changes:

```csharp
// MonoGame compatibility shims
namespace MoonBrookRidge.Engine.MonoGameCompat
{
    public class SpriteBatch
    {
        private MoonBrookEngine.Graphics.SpriteBatch _engine;
        
        public void Begin() => _engine.Begin();
        public void Draw(Texture2D tex, Vector2 pos, Color color)
            => _engine.Draw(tex.InternalTexture, pos, color);
        public void End() => _engine.End();
    }
}
```

## Contributing

See the main [CUSTOM_ENGINE_CONVERSION_PLAN.md](../CUSTOM_ENGINE_CONVERSION_PLAN.md) for the full conversion strategy.

## License

Same as MoonBrook Ridge project.

## Acknowledgments

- Silk.NET team for excellent .NET bindings
- StbImage for simple image loading
- MonoGame for API inspiration
