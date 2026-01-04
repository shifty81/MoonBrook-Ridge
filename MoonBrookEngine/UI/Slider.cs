using MoonBrookEngine.Graphics;
using MoonBrookEngine.Math;
using System;
using Vec2 = MoonBrookEngine.Math.Vector2;

namespace MoonBrookEngine.UI
{
    /// <summary>
    /// UI component for a slider (adjustable value between min and max).
    /// </summary>
    public class Slider : UIElement
    {
        private float _value;
        private float _minValue;
        private float _maxValue;
        private bool _isDragging;

        /// <summary>
        /// Gets or sets the current value of the slider.
        /// </summary>
        public float Value
        {
            get => _value;
            set
            {
                var newValue = System.Math.Clamp(value, _minValue, _maxValue);
                if (_value != newValue)
                {
                    _value = newValue;
                    ValueChanged?.Invoke(_value);
                }
            }
        }

        /// <summary>
        /// Minimum value of the slider.
        /// </summary>
        public float MinValue
        {
            get => _minValue;
            set
            {
                _minValue = value;
                if (_value < _minValue)
                    Value = _minValue;
            }
        }

        /// <summary>
        /// Maximum value of the slider.
        /// </summary>
        public float MaxValue
        {
            get => _maxValue;
            set
            {
                _maxValue = value;
                if (_value > _maxValue)
                    Value = _maxValue;
            }
        }

        /// <summary>
        /// Height of the slider track.
        /// </summary>
        public float TrackHeight { get; set; } = 4f;

        /// <summary>
        /// Size of the slider handle (thumb).
        /// </summary>
        public float HandleSize { get; set; } = 16f;

        /// <summary>
        /// Color of the slider track.
        /// </summary>
        public Color TrackColor { get; set; } = new Color(80, 80, 80);

        /// <summary>
        /// Color of the filled portion of the track.
        /// </summary>
        public Color FillColor { get; set; } = new Color(100, 150, 200);

        /// <summary>
        /// Color of the slider handle.
        /// </summary>
        public Color HandleColor { get; set; } = new Color(200, 200, 200);

        /// <summary>
        /// Color of the handle when hovered.
        /// </summary>
        public Color HandleHoverColor { get; set; } = new Color(220, 220, 220);

        /// <summary>
        /// Event fired when the slider value changes.
        /// </summary>
        public event Action<float>? ValueChanged;

        /// <summary>
        /// Creates a new Slider.
        /// </summary>
        public Slider(float minValue = 0f, float maxValue = 100f, float initialValue = 0f)
        {
            _minValue = minValue;
            _maxValue = maxValue;
            _value = System.Math.Clamp(initialValue, minValue, maxValue);
            Size = new Vec2(200, HandleSize);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            var worldPos = GetWorldPosition();
            var centerY = worldPos.Y + Size.Y / 2;

            // Draw track background
            var trackRect = new Rectangle(
                (int)worldPos.X,
                (int)(centerY - TrackHeight / 2),
                (int)Size.X,
                (int)TrackHeight
            );
            spriteBatch.DrawRectangle(trackRect, TrackColor);

            // Calculate handle position
            float normalizedValue = (_value - _minValue) / (_maxValue - _minValue);
            float handleX = worldPos.X + normalizedValue * Size.X;

            // Draw filled portion of track
            var fillRect = new Rectangle(
                (int)worldPos.X,
                (int)(centerY - TrackHeight / 2),
                (int)(handleX - worldPos.X),
                (int)TrackHeight
            );
            spriteBatch.DrawRectangle(fillRect, FillColor);

            // Draw handle
            var handleRect = new Rectangle(
                (int)(handleX - HandleSize / 2),
                (int)(centerY - HandleSize / 2),
                (int)HandleSize,
                (int)HandleSize
            );

            Color currentHandleColor = IsHovered || _isDragging ? HandleHoverColor : HandleColor;
            spriteBatch.DrawRectangle(handleRect, currentHandleColor);

            // Draw handle border
            if (BorderThickness > 0)
            {
                spriteBatch.DrawRectangleOutline(handleRect, BorderColor, BorderThickness);
            }
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Handle dragging
            if (_isDragging && IsEnabled)
            {
                // Value will be updated in OnMouseDown when dragging
            }
        }

        public override void OnMouseDown(Vec2 mousePosition)
        {
            base.OnMouseDown(mousePosition);
            _isDragging = true;
            UpdateValueFromMousePosition(mousePosition);
        }

        public override void OnMouseUp(Vec2 mousePosition)
        {
            base.OnMouseUp(mousePosition);
            _isDragging = false;
        }

        public override void OnMouseExit(Vec2 mousePosition)
        {
            base.OnMouseExit(mousePosition);
            _isDragging = false;
        }

        private void UpdateValueFromMousePosition(Vec2 mousePosition)
        {
            var worldPos = GetWorldPosition();
            float normalizedX = (mousePosition.X - worldPos.X) / Size.X;
            normalizedX = System.Math.Clamp(normalizedX, 0f, 1f);
            Value = _minValue + normalizedX * (_maxValue - _minValue);
        }
    }
}
