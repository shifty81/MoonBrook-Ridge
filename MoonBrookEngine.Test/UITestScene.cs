using MoonBrookEngine.Core;
using MoonBrookEngine.Graphics;
using MoonBrookEngine.Math;
using MoonBrookEngine.UI;
using Silk.NET.OpenGL;
using Silk.NET.Input;
using System;
using Vec2 = MoonBrookEngine.Math.Vector2;

namespace MoonBrookEngine.Test
{
    /// <summary>
    /// Test scene demonstrating the UI system with various components.
    /// </summary>
    public class UITestScene : MoonBrookEngine.Scene.Scene
    {
        private UISystem? _uiSystem;
        private BitmapFont? _font;
        private SpriteBatch? _spriteBatch;
        private IInputContext? _inputContext;
        private IMouse? _mouse;
        private bool _lastMouseButtonState;

        public UITestScene(GL gl, IInputContext input) : base(gl, "UI Test Scene")
        {
            _inputContext = input;
        }

        public override void Initialize()
        {
            Console.WriteLine("=== UI Test Scene Initializing ===");

            // Create sprite batch for rendering
            _spriteBatch = new SpriteBatch(GL);

            // Create UI system
            _uiSystem = new UISystem();

            // Get mouse
            if (_inputContext != null && _inputContext.Mice.Count > 0)
            {
                _mouse = _inputContext.Mice[0];
            }

            // Load font for UI text (requires GL context)
            _font = new BitmapFont(GL);  // Stub font for now

            // Create a main panel
            var mainPanel = new Panel
            {
                Position = new Vec2(50, 50),
                Size = new Vec2(700, 500),
                BackgroundColor = new Color(30, 30, 30, 230),
                BorderColor = new Color(100, 100, 150),
                BorderThickness = 2
            };
            _uiSystem.AddElement(mainPanel);

            // Add title label
            var titleLabel = new Label("MoonBrook Engine - UI System Test", _font)
            {
                Position = new Vec2(20, 20),
                TextColor = new Color(255, 200, 100),
                TextScale = 1.5f,
                AutoSize = false,
                Size = new Vec2(660, 30)
            };
            titleLabel.Parent = mainPanel;
            mainPanel.AddChild(titleLabel);

            // Add description label
            var descLabel = new Label("Testing UI components: Button, Label, Checkbox, Slider, Panel", _font)
            {
                Position = new Vec2(20, 60),
                TextColor = new Color(200, 200, 200),
                TextScale = 0.8f,
                AutoSize = false,
                Size = new Vec2(660, 20)
            };
            descLabel.Parent = mainPanel;
            mainPanel.AddChild(descLabel);

            // Add buttons section
            var buttonLabel = new Label("Buttons:", _font)
            {
                Position = new Vec2(20, 100),
                TextColor = new Color(150, 200, 255)
            };
            buttonLabel.Parent = mainPanel;
            mainPanel.AddChild(buttonLabel);

            var button1 = new MoonBrookEngine.UI.Button("Click Me!", _font)
            {
                Position = new Vec2(20, 130),
                Size = new Vec2(150, 40)
            };
            button1.Parent = mainPanel;
            button1.Clicked += () => Console.WriteLine("Button 1 clicked!");
            mainPanel.AddChild(button1);

            var button2 = new MoonBrookEngine.UI.Button("Button 2", _font)
            {
                Position = new Vec2(180, 130),
                Size = new Vec2(150, 40),
                NormalColor = new Color(60, 80, 100),
                HoverColor = new Color(80, 100, 120)
            };
            button2.Parent = mainPanel;
            button2.Clicked += () => Console.WriteLine("Button 2 clicked!");
            mainPanel.AddChild(button2);

            var disabledButton = new MoonBrookEngine.UI.Button("Disabled", _font)
            {
                Position = new Vec2(340, 130),
                Size = new Vec2(150, 40),
                IsEnabled = false
            };
            disabledButton.Parent = mainPanel;
            mainPanel.AddChild(disabledButton);

            // Add checkboxes section
            var checkboxLabel = new Label("Checkboxes:", _font)
            {
                Position = new Vec2(20, 190),
                TextColor = new Color(150, 200, 255)
            };
            checkboxLabel.Parent = mainPanel;
            mainPanel.AddChild(checkboxLabel);

            var checkbox1 = new Checkbox("Enable Feature A", _font)
            {
                Position = new Vec2(20, 220)
            };
            checkbox1.Parent = mainPanel;
            checkbox1.CheckedChanged += (isChecked) => 
                Console.WriteLine($"Checkbox 1: {(isChecked ? "Checked" : "Unchecked")}");
            mainPanel.AddChild(checkbox1);

            var checkbox2 = new Checkbox("Enable Feature B", _font)
            {
                Position = new Vec2(20, 250),
                IsChecked = true
            };
            checkbox2.Parent = mainPanel;
            checkbox2.CheckedChanged += (isChecked) => 
                Console.WriteLine($"Checkbox 2: {(isChecked ? "Checked" : "Unchecked")}");
            mainPanel.AddChild(checkbox2);

            // Add sliders section
            var sliderLabel = new Label("Sliders:", _font)
            {
                Position = new Vec2(20, 290),
                TextColor = new Color(150, 200, 255)
            };
            sliderLabel.Parent = mainPanel;
            mainPanel.AddChild(sliderLabel);

            var slider1 = new Slider(0, 100, 50)
            {
                Position = new Vec2(20, 320),
                Size = new Vec2(300, 20),
                BorderColor = new Color(100, 100, 100),
                BorderThickness = 1
            };
            slider1.Parent = mainPanel;
            slider1.ValueChanged += (value) => Console.WriteLine($"Slider 1: {value:F1}");
            mainPanel.AddChild(slider1);

            var slider2 = new Slider(0, 1, 0.5f)
            {
                Position = new Vec2(20, 360),
                Size = new Vec2(300, 20),
                FillColor = new Color(200, 100, 100),
                BorderColor = new Color(100, 100, 100),
                BorderThickness = 1
            };
            slider2.Parent = mainPanel;
            slider2.ValueChanged += (value) => Console.WriteLine($"Slider 2: {value:F2}");
            mainPanel.AddChild(slider2);

            // Add info panel
            var infoPanel = new Panel
            {
                Position = new Vec2(20, 400),
                Size = new Vec2(660, 80),
                BackgroundColor = new Color(40, 60, 40, 180),
                BorderColor = new Color(100, 150, 100),
                BorderThickness = 1
            };
            infoPanel.Parent = mainPanel;
            mainPanel.AddChild(infoPanel);

            var infoLabel1 = new Label("Week 9: UI System Implementation Complete!", _font)
            {
                Position = new Vec2(10, 10),
                TextColor = new Color(100, 255, 100),
                TextScale = 1.2f
            };
            infoLabel1.Parent = infoPanel;
            infoPanel.AddChild(infoLabel1);

            var infoLabel2 = new Label("Components: Button, Label, Checkbox, Slider, Panel", _font)
            {
                Position = new Vec2(10, 35),
                TextColor = new Color(200, 255, 200),
                TextScale = 0.9f
            };
            infoLabel2.Parent = infoPanel;
            infoPanel.AddChild(infoLabel2);

            var infoLabel3 = new Label("Features: Mouse hover, click, Z-ordering, events", _font)
            {
                Position = new Vec2(10, 55),
                TextColor = new Color(200, 255, 200),
                TextScale = 0.9f
            };
            infoLabel3.Parent = infoPanel;
            infoPanel.AddChild(infoLabel3);

            Console.WriteLine($"UI Test Scene loaded with {_uiSystem.ElementCount} root elements!");
        }

        public override void Update(GameTime gameTime)
        {
            if (_uiSystem == null || _mouse == null)
                return;

            // Get mouse state
            var mousePos = new Vec2(_mouse.Position.X, _mouse.Position.Y);
            bool isMouseDown = _mouse.IsButtonPressed(MouseButton.Left);

            // Update UI system
            _uiSystem.Update((float)gameTime.DeltaTime, mousePos, isMouseDown);

            _lastMouseButtonState = isMouseDown;
        }

        public override void Render(GameTime gameTime)
        {
            if (_uiSystem == null || _spriteBatch == null)
                return;

            // Clear screen
            GL.ClearColor(0.1f, 0.1f, 0.15f, 1.0f);
            GL.Clear(ClearBufferMask.ColorBufferBit);

            // Draw UI without camera (screen space)
            _spriteBatch.Begin();
            _uiSystem.Draw(_spriteBatch);
            _spriteBatch.End();
        }

        public override void Dispose()
        {
            _uiSystem?.Clear();
            _spriteBatch?.Dispose();
            Console.WriteLine("UI Test Scene disposed!");
        }
    }
}
