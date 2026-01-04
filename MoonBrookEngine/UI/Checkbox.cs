using MoonBrookEngine.Graphics;
using MoonBrookEngine.Math;
using System;
using Vec2 = MoonBrookEngine.Math.Vector2;

namespace MoonBrookEngine.UI
{
    /// <summary>
    /// UI component for a checkbox (toggle on/off).
    /// </summary>
    public class Checkbox : UIElement
    {
        private string _label = string.Empty;
        private bool _isChecked;

        /// <summary>
        /// Gets or sets whether the checkbox is checked.
        /// </summary>
        public bool IsChecked
        {
            get => _isChecked;
            set
            {
                if (_isChecked != value)
                {
                    _isChecked = value;
                    CheckedChanged?.Invoke(_isChecked);
                }
            }
        }

        /// <summary>
        /// The label text displayed next to the checkbox.
        /// </summary>
        public string Label
        {
            get => _label;
            set => _label = value ?? string.Empty;
        }

        /// <summary>
        /// The font used to render the label.
        /// </summary>
        public BitmapFont? Font { get; set; }

        /// <summary>
        /// Size of the checkbox box itself.
        /// </summary>
        public float BoxSize { get; set; } = 20f;

        /// <summary>
        /// Spacing between box and label.
        /// </summary>
        public float LabelSpacing { get; set; } = 5f;

        /// <summary>
        /// Text color for the label.
        /// </summary>
        public Color LabelColor { get; set; } = Color.White;

        /// <summary>
        /// Background color of the checkbox box.
        /// </summary>
        public Color BoxColor { get; set; } = new Color(60, 60, 60);

        /// <summary>
        /// Color of the checkbox when checked.
        /// </summary>
        public Color CheckColor { get; set; } = new Color(100, 200, 100);

        /// <summary>
        /// Event fired when the checked state changes.
        /// </summary>
        public event Action<bool>? CheckedChanged;

        /// <summary>
        /// Creates a new Checkbox.
        /// </summary>
        public Checkbox(string label = "", BitmapFont? font = null)
        {
            Label = label;
            Font = font;
            Size = new Vec2(BoxSize + LabelSpacing, BoxSize);
            BorderThickness = 2;
            BorderColor = new Color(120, 120, 120);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            var worldPos = GetWorldPosition();

            // Draw checkbox box
            var boxRect = new Rectangle(
                (int)worldPos.X,
                (int)worldPos.Y,
                (int)BoxSize,
                (int)BoxSize
            );

            // Background
            spriteBatch.DrawRectangle(boxRect, BoxColor);

            // Border
            if (BorderThickness > 0)
            {
                spriteBatch.DrawRectangleOutline(boxRect, BorderColor, BorderThickness);
            }

            // Check mark (if checked)
            if (IsChecked)
            {
                var checkRect = new Rectangle(
                    (int)(worldPos.X + BoxSize * 0.25f),
                    (int)(worldPos.Y + BoxSize * 0.25f),
                    (int)(BoxSize * 0.5f),
                    (int)(BoxSize * 0.5f)
                );
                spriteBatch.DrawRectangle(checkRect, CheckColor);
            }

            // Draw label text
            if (!string.IsNullOrEmpty(Label) && Font != null)
            {
                var textPos = new Vec2(
                    worldPos.X + BoxSize + LabelSpacing,
                    worldPos.Y + (BoxSize - Font.LineSpacing) / 2
                );
                spriteBatch.DrawString(Font, Label, textPos, LabelColor);
            }
        }

        public override void OnClick(Vec2 mousePosition)
        {
            base.OnClick(mousePosition);
            IsChecked = !IsChecked; // Toggle
        }
    }
}
