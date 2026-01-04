# Custom Engine Implementation - Initial Steps

**Date**: January 3, 2026  
**Status**: READY TO BEGIN

---

## Week 1: Foundation Setup

### Step 1: Technology Decision ✅ **DECIDED**

**Selected Stack:**
- **Graphics**: Silk.NET.OpenGL (OpenGL 4.5+)
- **Windowing**: Silk.NET.Windowing (cross-platform)
- **Audio**: Silk.NET.OpenAL
- **Image Loading**: StbImageSharp
- **Language**: C# (.NET 9.0)

**Why Silk.NET?**
- Pure C# - no C++ interop needed
- Cross-platform (Windows, Linux, macOS)
- Modern, actively maintained
- Good performance
- Familiar API for MonoGame developers

### Step 2: Project Structure

```bash
MoonBrook-Ridge/
├── MoonBrookEngine/              # NEW: Custom engine project
│   ├── MoonBrookEngine.csproj
│   ├── Core/
│   │   ├── Engine.cs
│   │   ├── GameLoop.cs
│   │   └── Time.cs
│   ├── Graphics/
│   │   ├── Renderer.cs
│   │   ├── Texture2D.cs
│   │   ├── SpriteBatch.cs
│   │   └── Shader.cs
│   ├── Input/
│   │   ├── InputManager.cs
│   │   ├── Keyboard.cs
│   │   └── Mouse.cs
│   └── Math/
│       ├── Vector2.cs (wrapper for System.Numerics)
│       ├── Rectangle.cs
│       └── Color.cs
├── MoonBrookRidge/              # EXISTING: Current game
│   └── ... (existing code)
└── MoonBrookRidge.Engine/       # NEW: Engine adapter project
    ├── EngineAdapter.cs
    └── MonoGameCompat/
        ├── SpriteBatch.cs
        ├── Texture2D.cs
        └── ... (compatibility shims)
```

### Step 3: Create Engine Project

```bash
cd /home/runner/work/MoonBrook-Ridge/MoonBrook-Ridge
dotnet new classlib -n MoonBrookEngine -f net9.0
cd MoonBrookEngine
dotnet add package Silk.NET.Windowing --version 2.20.0
dotnet add package Silk.NET.OpenGL --version 2.20.0
dotnet add package Silk.NET.Input --version 2.20.0
dotnet add package StbImageSharp --version 2.27.13
dotnet add package System.Numerics.Vectors --version 4.5.0
```

### Step 4: Minimal Engine Implementation

#### 4.1 Engine.cs - Main Engine Class

```csharp
using Silk.NET.Windowing;
using Silk.NET.OpenGL;
using System.Drawing;

namespace MoonBrookEngine.Core;

public class Engine
{
    private IWindow _window;
    private GL _gl;
    private bool _isRunning;
    
    public int Width { get; private set; }
    public int Height { get; private set; }
    public string Title { get; private set; }
    
    public Engine(string title, int width, int height)
    {
        Title = title;
        Width = width;
        Height = height;
        
        var options = WindowOptions.Default;
        options.Size = new Size(width, height);
        options.Title = title;
        options.VSync = true;
        
        _window = Window.Create(options);
        
        _window.Load += OnLoad;
        _window.Update += OnUpdate;
        _window.Render += OnRender;
        _window.Closing += OnClosing;
    }
    
    public void Run()
    {
        _isRunning = true;
        _window.Run();
    }
    
    public void Stop()
    {
        _isRunning = false;
        _window.Close();
    }
    
    private void OnLoad()
    {
        _gl = _window.CreateOpenGL();
        _gl.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
        
        // Initialize subsystems
        Initialize();
    }
    
    private void OnUpdate(double deltaTime)
    {
        if (!_isRunning) return;
        
        Update(deltaTime);
    }
    
    private void OnRender(double deltaTime)
    {
        if (!_isRunning) return;
        
        _gl.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
        
        Render(deltaTime);
    }
    
    private void OnClosing()
    {
        Shutdown();
        _gl?.Dispose();
    }
    
    // Override these in derived classes or use delegates
    protected virtual void Initialize() { }
    protected virtual void Update(double deltaTime) { }
    protected virtual void Render(double deltaTime) { }
    protected virtual void Shutdown() { }
}
```

#### 4.2 GameTime.cs - Time Management

```csharp
namespace MoonBrookEngine.Core;

public class GameTime
{
    public double TotalSeconds { get; set; }
    public double DeltaTime { get; set; }
    
    public GameTime(double totalSeconds, double deltaTime)
    {
        TotalSeconds = totalSeconds;
        DeltaTime = deltaTime;
    }
}
```

#### 4.3 Texture2D.cs - Texture Class

```csharp
using Silk.NET.OpenGL;
using StbImageSharp;

namespace MoonBrookEngine.Graphics;

public class Texture2D : IDisposable
{
    private GL _gl;
    private uint _handle;
    
    public int Width { get; private set; }
    public int Height { get; private set; }
    
    public Texture2D(GL gl, string path)
    {
        _gl = gl;
        
        // Load image using StbImage
        StbImage.stbi_set_flip_vertically_on_load(1);
        using var stream = File.OpenRead(path);
        ImageResult image = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);
        
        Width = image.Width;
        Height = image.Height;
        
        // Create OpenGL texture
        _handle = _gl.GenTexture();
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
        
        _gl.TexImage2D(TextureTarget.Texture2D, 0, InternalFormat.Rgba, 
            (uint)Width, (uint)Height, 0, PixelFormat.Rgba, PixelType.UnsignedByte, image.Data);
        
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapS, (int)GLEnum.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureWrapT, (int)GLEnum.Repeat);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMinFilter, (int)GLEnum.Linear);
        _gl.TexParameter(TextureTarget.Texture2D, TextureParameterName.TextureMagFilter, (int)GLEnum.Linear);
    }
    
    public void Bind()
    {
        _gl.BindTexture(TextureTarget.Texture2D, _handle);
    }
    
    public void Dispose()
    {
        _gl.DeleteTexture(_handle);
    }
}
```

### Step 5: Proof of Concept - "Hello Triangle"

Create a simple test app that renders a textured quad:

```csharp
using MoonBrookEngine.Core;
using MoonBrookEngine.Graphics;

namespace MoonBrookEngine.Test;

public class TestGame : Engine
{
    private Texture2D _testTexture;
    
    public TestGame() : base("MoonBrook Engine Test", 1280, 720)
    {
    }
    
    protected override void Initialize()
    {
        // Load test texture
        _testTexture = new Texture2D(_gl, "test.png");
    }
    
    protected override void Update(double deltaTime)
    {
        // Test update logic
    }
    
    protected override void Render(double deltaTime)
    {
        // Render test texture
        _testTexture.Bind();
        // TODO: Draw quad
    }
    
    protected override void Shutdown()
    {
        _testTexture?.Dispose();
    }
}

class Program
{
    static void Main()
    {
        var game = new TestGame();
        game.Run();
    }
}
```

---

## Week 2-3: Core Rendering

### Implement SpriteBatch

```csharp
public class SpriteBatch
{
    // Batch sprite rendering
    // - Quad generation
    // - Texture batching
    // - Vertex buffer management
    // - Shader compilation
    // - Draw call optimization
}
```

### Implement Camera2D

```csharp
public class Camera2D
{
    public Vector2 Position { get; set; }
    public float Zoom { get; set; }
    public Matrix4x4 ViewMatrix { get; }
    public Matrix4x4 ProjectionMatrix { get; }
}
```

---

## Week 4: MonoGame Compatibility Layer

Create wrapper classes that maintain API compatibility:

```csharp
namespace MoonBrookRidge.Engine.MonoGameCompat;

// Wrappers that delegate to engine classes
public class SpriteBatch
{
    private MoonBrookEngine.Graphics.SpriteBatch _engineBatch;
    
    public void Begin() => _engineBatch.Begin();
    public void Draw(Texture2D texture, Vector2 pos, Color color)
        => _engineBatch.Draw(texture.InternalTexture, pos, color);
    public void End() => _engineBatch.End();
}

public class Texture2D
{
    internal MoonBrookEngine.Graphics.Texture2D InternalTexture;
    public int Width => InternalTexture.Width;
    public int Height => InternalTexture.Height;
}
```

This allows existing MoonBrookRidge code to work with minimal changes!

---

## Immediate Action Items

### Today:
1. ✅ Create `CUSTOM_ENGINE_CONVERSION_PLAN.md`
2. ✅ Create `CUSTOM_ENGINE_IMPLEMENTATION.md` (this file)
3. [ ] Create `MoonBrookEngine` project
4. [ ] Add Silk.NET packages
5. [ ] Implement basic Engine.cs

### This Week:
1. [ ] Implement Texture2D loading
2. [ ] Create proof-of-concept rendering test
3. [ ] Measure baseline performance
4. [ ] Document findings

### Next Week:
1. [ ] Implement SpriteBatch with batching
2. [ ] Add shader support
3. [ ] Port Camera2D system
4. [ ] Test with MoonBrookRidge sprites

---

## Success Metrics

| Milestone | Target Date | Success Criteria |
|-----------|-------------|------------------|
| Engine foundation | Week 1 | Window opens, clears to color |
| Texture loading | Week 2 | Can load and display PNG |
| Sprite rendering | Week 3 | Can render 1000+ sprites at 60 FPS |
| MonoGame compat | Week 4 | Existing code runs with minimal changes |
| Player movement | Week 6 | Player can move around world |
| Full feature parity | Week 16 | All Phase 1-10 features work |

---

## Questions for User

Before proceeding, please confirm:

1. **Proceed with custom engine?** (vs. staying with MonoGame)
2. **Timeline acceptable?** (5-6 months for full conversion)
3. **Technology stack approved?** (Silk.NET + OpenGL)
4. **Gradual migration preferred?** (vs. clean slate)
5. **Priority:** Performance or features first?

---

## Next Steps

**If approved**, I will:
1. Create the `MoonBrookEngine` project structure
2. Implement the basic engine foundation
3. Create a proof-of-concept test
4. Demonstrate sprite rendering performance
5. Begin gradual migration of game systems

**Estimated time for PoC**: 2-3 hours of development

---

Let me know if you'd like me to proceed with creating the engine foundation!
