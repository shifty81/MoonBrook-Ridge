using MoonBrookEngine.Math;
using System;
using Vec2 = MoonBrookEngine.Math.Vector2;

namespace MoonBrookEngine.UI
{
    /// <summary>
    /// Base class for all UI elements in the engine.
    /// Provides common properties like position, size, visibility, and interaction states.
    /// </summary>
    public abstract class UIElement
    {
        /// <summary>
        /// Unique identifier for this UI element.
        /// </summary>
        public string Id { get; set; } = Guid.NewGuid().ToString();

        /// <summary>
        /// Position of the UI element in screen coordinates.
        /// </summary>
        public Vec2 Position { get; set; }

        /// <summary>
        /// Size of the UI element (width, height).
        /// </summary>
        public Vec2 Size { get; set; }

        /// <summary>
        /// Whether the UI element is visible and should be rendered.
        /// </summary>
        public bool IsVisible { get; set; } = true;

        /// <summary>
        /// Whether the UI element is enabled and can receive input.
        /// </summary>
        public bool IsEnabled { get; set; } = true;

        /// <summary>
        /// Whether the mouse is currently hovering over this element.
        /// </summary>
        public bool IsHovered { get; protected set; }

        /// <summary>
        /// Z-order for rendering. Higher values are rendered on top.
        /// </summary>
        public int ZOrder { get; set; } = 0;

        /// <summary>
        /// Anchor point for positioning (0,0 = top-left, 0.5,0.5 = center, 1,1 = bottom-right).
        /// </summary>
        public Vec2 Anchor { get; set; } = new Vec2(0, 0);

        /// <summary>
        /// Parent UI element (for hierarchical UI).
        /// </summary>
        public UIElement? Parent { get; set; }

        /// <summary>
        /// Padding inside the element.
        /// </summary>
        public Padding Padding { get; set; } = new Padding();

        /// <summary>
        /// Margin outside the element.
        /// </summary>
        public Padding Margin { get; set; } = new Padding();

        /// <summary>
        /// Background color of the element.
        /// </summary>
        public Color BackgroundColor { get; set; } = Color.Transparent;

        /// <summary>
        /// Border color of the element.
        /// </summary>
        public Color BorderColor { get; set; } = Color.Transparent;

        /// <summary>
        /// Border thickness in pixels.
        /// </summary>
        public float BorderThickness { get; set; } = 0f;

        /// <summary>
        /// Gets the bounding rectangle of this element in screen coordinates.
        /// </summary>
        public Rectangle Bounds
        {
            get
            {
                var worldPos = GetWorldPosition();
                return new Rectangle(
                    (int)(worldPos.X - Size.X * Anchor.X),
                    (int)(worldPos.Y - Size.Y * Anchor.Y),
                    (int)Size.X,
                    (int)Size.Y
                );
            }
        }

        /// <summary>
        /// Gets the world position of this element (accounting for parent positions).
        /// </summary>
        public Vec2 GetWorldPosition()
        {
            if (Parent != null)
            {
                return Parent.GetWorldPosition() + Position;
            }
            return Position;
        }

        /// <summary>
        /// Checks if a point is inside this element's bounds.
        /// </summary>
        public bool ContainsPoint(Vec2 point)
        {
            return Bounds.Contains((int)point.X, (int)point.Y);
        }

        /// <summary>
        /// Called when the UI element is updated.
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds.</param>
        public virtual void Update(float deltaTime)
        {
        }

        /// <summary>
        /// Called when the UI element should be rendered.
        /// </summary>
        /// <param name="spriteBatch">The sprite batch to draw with.</param>
        public abstract void Draw(Graphics.SpriteBatch spriteBatch);

        /// <summary>
        /// Called when mouse button is pressed on this element.
        /// </summary>
        public virtual void OnMouseDown(Vec2 mousePosition)
        {
        }

        /// <summary>
        /// Called when mouse button is released on this element.
        /// </summary>
        public virtual void OnMouseUp(Vec2 mousePosition)
        {
        }

        /// <summary>
        /// Called when mouse enters this element.
        /// </summary>
        public virtual void OnMouseEnter(Vec2 mousePosition)
        {
            IsHovered = true;
        }

        /// <summary>
        /// Called when mouse exits this element.
        /// </summary>
        public virtual void OnMouseExit(Vec2 mousePosition)
        {
            IsHovered = false;
        }

        /// <summary>
        /// Called when mouse is clicked on this element (press and release).
        /// </summary>
        public virtual void OnClick(Vec2 mousePosition)
        {
        }
    }

    /// <summary>
    /// Represents padding or margin around a UI element.
    /// </summary>
    public struct Padding
    {
        public float Left { get; set; }
        public float Right { get; set; }
        public float Top { get; set; }
        public float Bottom { get; set; }

        public Padding(float all = 0f)
        {
            Left = Right = Top = Bottom = all;
        }

        public Padding(float horizontal, float vertical)
        {
            Left = Right = horizontal;
            Top = Bottom = vertical;
        }

        public Padding(float left, float top, float right, float bottom)
        {
            Left = left;
            Top = top;
            Right = right;
            Bottom = bottom;
        }
    }
}
