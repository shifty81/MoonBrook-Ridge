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
- **Performance Monitor**: Real-time FPS, frame time, and memory tracking

### Graphics (✅ Implemented)
- **Texture Loading**: PNG/JPG loading via StbImageSharp
- **Shader System**: GLSL shader compilation and uniform management
- **SpriteBatch**: Efficient sprite batching (2048 sprites per batch)
- **Camera2D**: Zoom, pan, rotation support
- **Particle Rendering**: Integrated particle system rendering
- **Color System**: RGBA color with normalization and common presets

### Physics (✅ Implemented)
- **Physics System**: Force-based physics with gravity and drag
- **Collision Detection**: Circle and rectangle collision shapes
- **Collision Response**: Impulse-based collision resolution
- **Trigger Events**: OnTriggerEnter/Exit, OnCollisionEnter/Exit callbacks
- **Spatial Partitioning**: Quadtree for efficient collision detection

### Animation (✅ Implemented)
- **Sprite Sheet Animation**: Frame-based animation system
- **Animation Component**: Multiple animations per entity
- **Looping & One-Shot**: Configurable playback modes
- **Animation Events**: Frame change and completion callbacks
- **Helper Methods**: Easy sprite sheet layout parsing

### UI System (✅ Implemented)
- **UIElement Base**: Position, size, visibility, hierarchy, events
- **UISystem**: Element management, input handling, Z-ordering
- **Label**: Text display with alignment and auto-sizing
- **Button**: Clickable buttons with hover/press states
- **Panel**: Container for child elements with background
- **Checkbox**: Toggle component with checked state events
- **Slider**: Value adjustment with draggable handle
- **Mouse Interaction**: Hover, click, and drag support

### Particle System (✅ Implemented)
- **Particle Emitters**: Configurable particle spawning
- **Particle Pooling**: Zero-allocation particle management
- **Particle Affectors**: Gravity, wind, color/size interpolation
- **Rotation Support**: Spinning particles with random speeds
- **Efficient Rendering**: Batch rendering of all particles

### Audio (✅ Implemented)
- **Audio System**: OpenAL-based audio playback
- **Sound Effects**: SoundEffect and SoundEffectInstance
- **Resource Management**: Integrated with ResourceManager

### ECS Architecture (✅ Implemented)
- **Entity Component System**: Clean separation of data and logic
- **Built-in Components**: Transform, Sprite, Velocity, Physics, Collider, Particle, Animation
- **World Management**: Entity lifecycle management
- **Component Queries**: Fast entity-component lookups

### Scene Management (✅ Implemented)
- **Scene System**: Multiple scene support
- **Scene Transitions**: Smooth scene switching
- **Resource Scoping**: Per-scene resource management

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

### Week 2 (✅ Complete)
- [x] SpriteBatch with automatic batching
- [x] Camera2D with zoom and pan
- [x] Rectangle and Vector2 math types

### Week 3 (✅ Complete)
- [x] Performance profiling (PerformanceMonitor)
- [x] Draw call tracking
- [x] Memory usage monitoring

### Week 4 (✅ Complete)
- [x] Input Manager abstraction layer
- [x] Font rendering (bitmap fonts)
- [x] Resource Manager
- [x] MonoGame compatibility layer

### Week 5 (✅ Complete)
- [x] Audio system (OpenAL)
- [x] SoundEffect and SoundEffectInstance classes
- [x] Audio integration with ResourceManager

### Week 6 (✅ Complete)
- [x] Scene Management System
- [x] Entity Component System (ECS)
- [x] Collision detection with spatial partitioning
- [x] Built-in components (Transform, Sprite, Collider)
- [x] World entity management

### Week 7 (✅ Complete)
- [x] Physics System with forces and gravity
- [x] VelocityComponent and PhysicsComponent
- [x] Collision response with impulses
- [x] Drag and restitution simulation
- [x] Physics test scene

### Week 8 (✅ Complete)
- [x] Trigger events (OnTriggerEnter, OnTriggerExit, OnCollisionEnter, OnCollisionExit)
- [x] Particle System with pooling
- [x] ParticleComponent and ParticleSystem
- [x] Particle affectors (gravity, wind, color interpolation, size interpolation)
- [x] Animation System with sprite sheet support
- [x] AnimationComponent and AnimationSystem
- [x] Animation events (OnAnimationComplete, OnFrameChange)
- [x] Helper methods for sprite sheet layouts

### Week 9 (✅ Complete)
- [x] UI System foundation (UIElement, UISystem)
- [x] SpriteBatch enhancements (DrawRectangle, DrawRectangleOutline, DrawString with scale)
- [x] Label component (text display with alignment)
- [x] Button component (clickable with hover/press states)
- [x] Panel component (container with child management)
- [x] Checkbox component (toggle with events)
- [x] Slider component (value adjustment with drag)
- [x] Mouse interaction (hover, click, drag support)
- [x] UI Test Scene demonstration

### Week 9+ (Planned)
- [x] UI System (Button, Label, Panel, Layout)
- [x] Label component (text display with alignment)
- [x] Button component (clickable with hover/press states)
- [x] Panel component (container with child management)
- [x] Checkbox component (toggle with events)
- [x] Slider component (value adjustment with drag)
- [x] Mouse interaction (hover, click, drag support)
- [x] UI Test Scene demonstration

### Week 10 (✅ Complete)
- [x] Fixed GameTime TimeSpan conversion issues
- [x] Implemented runtime font atlas generation
- [x] Created SimpleFontRasterizer for vector glyphs
- [x] Enhanced BitmapFont with recognizable characters
- [x] Code quality improvements (nullable types, bounds checks)
- [x] Security scan passed (0 alerts)

### Week 10+ (Planned)
- [ ] UI System (Button, Label, Panel, Layout)
- [ ] TextBox and advanced input components
- [ ] Layout managers (StackLayout, GridLayout)
- [ ] Audio enhancements (positional audio, audio pools)
- [ ] Advanced resource management (asset bundles, streaming)
- [ ] Rendering improvements (sprite sorting, frustum culling)
- [ ] Advanced physics (friction, angular velocity, constraints)
- [ ] Multi-threading support

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
