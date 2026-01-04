using MoonBrookEngine.Graphics;
using MoonBrookEngine.Math;
using System;
using Vec2 = MoonBrookEngine.Math.Vector2;

namespace MoonBrookEngine.UI
{
    /// <summary>
    /// UI component for clickable buttons with text.
    /// </summary>
    public class Button : UIElement
    {
        private string _text = string.Empty;
        private bool _isPressed;

        /// <summary>
        /// The text displayed on the button.
        /// </summary>
        public string Text
        {
            get => _text;
            set => _text = value ?? string.Empty;
        }

        /// <summary>
        /// The font used to render the button text.
        /// </summary>
        public BitmapFont? Font { get; set; }

        /// <summary>
        /// Text color when button is in normal state.
        /// </summary>
        public Color TextColor { get; set; } = Color.White;

        /// <summary>
        /// Text color when button is hovered.
        /// </summary>
        public Color HoverTextColor { get; set; } = Color.White;

        /// <summary>
        /// Text color when button is pressed.
        /// </summary>
        public Color PressedTextColor { get; set; } = new Color(200, 200, 200);

        /// <summary>
        /// Text color when button is disabled.
        /// </summary>
        public Color DisabledTextColor { get; set; } = new Color(100, 100, 100);

        /// <summary>
        /// Background color when button is in normal state.
        /// </summary>
        public Color NormalColor { get; set; } = new Color(80, 80, 80);

        /// <summary>
        /// Background color when button is hovered.
        /// </summary>
        public Color HoverColor { get; set; } = new Color(100, 100, 100);

        /// <summary>
        /// Background color when button is pressed.
        /// </summary>
        public Color PressedColor { get; set; } = new Color(60, 60, 60);

        /// <summary>
        /// Background color when button is disabled.
        /// </summary>
        public Color DisabledColor { get; set; } = new Color(50, 50, 50);

        /// <summary>
        /// Text scale factor.
        /// </summary>
        public float TextScale { get; set; } = 1.0f;

        /// <summary>
        /// Event fired when the button is clicked.
        /// </summary>
        public event Action? Clicked;

        /// <summary>
        /// Creates a new Button with the specified text.
        /// </summary>
        public Button(string text = "", BitmapFont? font = null)
        {
            Text = text;
            Font = font;
            Size = new Vec2(120, 40); // Default button size
            Padding = new Padding(10, 5, 10, 5);
            BorderThickness = 2;
            BorderColor = new Color(120, 120, 120);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            var bounds = Bounds;

            // Determine current colors based on state
            Color currentBgColor;
            Color currentTextColor;

            if (!IsEnabled)
            {
                currentBgColor = DisabledColor;
                currentTextColor = DisabledTextColor;
            }
            else if (_isPressed)
            {
                currentBgColor = PressedColor;
                currentTextColor = PressedTextColor;
            }
            else if (IsHovered)
            {
                currentBgColor = HoverColor;
                currentTextColor = HoverTextColor;
            }
            else
            {
                currentBgColor = NormalColor;
                currentTextColor = TextColor;
            }

            // Draw background
            spriteBatch.DrawRectangle(bounds, currentBgColor);

            // Draw border
            if (BorderThickness > 0)
            {
                spriteBatch.DrawRectangleOutline(bounds, BorderColor, BorderThickness);
            }

            // Draw text (centered)
            if (!string.IsNullOrEmpty(Text) && Font != null)
            {
                var textSize = Font.MeasureString(Text) * TextScale;
                var textPos = new Vec2(
                    bounds.X + (bounds.Width - textSize.X) / 2,
                    bounds.Y + (bounds.Height - textSize.Y) / 2
                );

                spriteBatch.DrawString(Font, Text, textPos, currentTextColor, TextScale);
            }
        }

        public override void OnMouseDown(Vec2 mousePosition)
        {
            base.OnMouseDown(mousePosition);
            _isPressed = true;
        }

        public override void OnMouseUp(Vec2 mousePosition)
        {
            base.OnMouseUp(mousePosition);
            _isPressed = false;
        }

        public override void OnMouseExit(Vec2 mousePosition)
        {
            base.OnMouseExit(mousePosition);
            _isPressed = false;
        }

        public override void OnClick(Vec2 mousePosition)
        {
            base.OnClick(mousePosition);
            Clicked?.Invoke();
        }
    }
}
