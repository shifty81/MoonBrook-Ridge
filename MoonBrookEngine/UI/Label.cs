using MoonBrookEngine.Graphics;
using MoonBrookEngine.Math;
using Vec2 = MoonBrookEngine.Math.Vector2;

namespace MoonBrookEngine.UI
{
    /// <summary>
    /// UI component for displaying text.
    /// </summary>
    public class Label : UIElement
    {
        private string _text = string.Empty;
        private BitmapFont? _font;

        /// <summary>
        /// The text to display.
        /// </summary>
        public string Text
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    UpdateSize();
                }
            }
        }

        /// <summary>
        /// The font to use for rendering the text.
        /// </summary>
        public BitmapFont? Font
        {
            get => _font;
            set
            {
                if (_font != value)
                {
                    _font = value;
                    UpdateSize();
                }
            }
        }

        /// <summary>
        /// The color of the text.
        /// </summary>
        public Color TextColor { get; set; } = Color.White;

        /// <summary>
        /// The scale of the text.
        /// </summary>
        public float TextScale { get; set; } = 1.0f;

        /// <summary>
        /// Text alignment within the label bounds.
        /// </summary>
        public TextAlignment Alignment { get; set; } = TextAlignment.Left;

        /// <summary>
        /// Whether to automatically size the label to fit the text.
        /// </summary>
        public bool AutoSize { get; set; } = true;

        /// <summary>
        /// Creates a new Label with the specified text.
        /// </summary>
        public Label(string text = "", BitmapFont? font = null)
        {
            Text = text;
            Font = font;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible || string.IsNullOrEmpty(Text) || Font == null)
                return;

            var worldPos = GetWorldPosition();
            var bounds = Bounds;

            // Draw background if not transparent
            if (BackgroundColor.A > 0)
            {
                spriteBatch.DrawRectangle(bounds, BackgroundColor);
            }

            // Draw border if thickness > 0
            if (BorderThickness > 0 && BorderColor.A > 0)
            {
                spriteBatch.DrawRectangleOutline(bounds, BorderColor, BorderThickness);
            }

            // Calculate text position based on alignment
            var textSize = MeasureText();
            var textPos = CalculateTextPosition(worldPos, textSize);

            // Draw text
            spriteBatch.DrawString(Font, Text, textPos, TextColor, TextScale);
        }

        private void UpdateSize()
        {
            if (AutoSize && Font != null && !string.IsNullOrEmpty(Text))
            {
                var textSize = MeasureText();
                Size = textSize + new Vector2(Padding.Left + Padding.Right, Padding.Top + Padding.Bottom);
            }
        }

        private Vec2 MeasureText()
        {
            if (Font == null || string.IsNullOrEmpty(Text))
                return Vec2.Zero;

            return Font.MeasureString(Text) * TextScale;
        }

        private Vec2 CalculateTextPosition(Vec2 worldPos, Vec2 textSize)
        {
            var bounds = Bounds;
            var x = bounds.X + Padding.Left;
            var y = bounds.Y + Padding.Top;

            switch (Alignment)
            {
                case TextAlignment.Left:
                    // Already at left
                    break;
                case TextAlignment.Center:
                    x = bounds.X + (bounds.Width - textSize.X) / 2;
                    break;
                case TextAlignment.Right:
                    x = bounds.X + bounds.Width - textSize.X - Padding.Right;
                    break;
            }

            return new Vec2(x, y);
        }
    }

    /// <summary>
    /// Text alignment options for labels.
    /// </summary>
    public enum TextAlignment
    {
        Left,
        Center,
        Right
    }
}
