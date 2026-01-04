using MoonBrookEngine.Graphics;
using MoonBrookEngine.Math;
using System.Collections.Generic;
using System.Linq;
using Vec2 = MoonBrookEngine.Math.Vector2;

namespace MoonBrookEngine.UI
{
    /// <summary>
    /// UI component for grouping and containing other UI elements.
    /// Provides background rendering and child element management.
    /// </summary>
    public class Panel : UIElement
    {
        private readonly List<UIElement> _children = new();

        /// <summary>
        /// Gets the child elements contained in this panel.
        /// </summary>
        public IReadOnlyList<UIElement> Children => _children.AsReadOnly();

        /// <summary>
        /// Creates a new Panel.
        /// </summary>
        public Panel()
        {
            Size = new Vec2(200, 200); // Default panel size
            BackgroundColor = new Color(40, 40, 40, 200); // Semi-transparent dark background
            BorderThickness = 1;
            BorderColor = new Color(100, 100, 100);
        }

        /// <summary>
        /// Adds a child element to this panel.
        /// </summary>
        public void AddChild(UIElement child)
        {
            if (child == null)
                throw new System.ArgumentNullException(nameof(child));

            if (!_children.Contains(child))
            {
                child.Parent = this;
                _children.Add(child);
            }
        }

        /// <summary>
        /// Removes a child element from this panel.
        /// </summary>
        public void RemoveChild(UIElement child)
        {
            if (child == null)
                throw new System.ArgumentNullException(nameof(child));

            if (_children.Remove(child))
            {
                child.Parent = null;
            }
        }

        /// <summary>
        /// Removes all child elements from this panel.
        /// </summary>
        public void ClearChildren()
        {
            foreach (var child in _children)
            {
                child.Parent = null;
            }
            _children.Clear();
        }

        public override void Update(float deltaTime)
        {
            base.Update(deltaTime);

            // Update all children
            foreach (var child in _children.Where(c => c.IsEnabled))
            {
                child.Update(deltaTime);
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (!IsVisible)
                return;

            var bounds = Bounds;

            // Draw background
            if (BackgroundColor.A > 0)
            {
                spriteBatch.DrawRectangle(bounds, BackgroundColor);
            }

            // Draw border
            if (BorderThickness > 0 && BorderColor.A > 0)
            {
                spriteBatch.DrawRectangleOutline(bounds, BorderColor, BorderThickness);
            }

            // Draw children (sorted by Z-order)
            var sortedChildren = _children
                .Where(c => c.IsVisible)
                .OrderBy(c => c.ZOrder);

            foreach (var child in sortedChildren)
            {
                child.Draw(spriteBatch);
            }
        }

        public override void OnMouseDown(Vec2 mousePosition)
        {
            base.OnMouseDown(mousePosition);

            // Propagate to children (in reverse Z-order, so top elements get priority)
            var sortedChildren = _children
                .Where(c => c.IsVisible && c.IsEnabled && c.ContainsPoint(mousePosition))
                .OrderByDescending(c => c.ZOrder);

            foreach (var child in sortedChildren)
            {
                child.OnMouseDown(mousePosition);
                break; // Only the topmost child
            }
        }

        public override void OnMouseUp(Vec2 mousePosition)
        {
            base.OnMouseUp(mousePosition);

            // Propagate to children
            var sortedChildren = _children
                .Where(c => c.IsVisible && c.IsEnabled && c.ContainsPoint(mousePosition))
                .OrderByDescending(c => c.ZOrder);

            foreach (var child in sortedChildren)
            {
                child.OnMouseUp(mousePosition);
                break; // Only the topmost child
            }
        }

        public override void OnClick(Vec2 mousePosition)
        {
            base.OnClick(mousePosition);

            // Propagate to children
            var sortedChildren = _children
                .Where(c => c.IsVisible && c.IsEnabled && c.ContainsPoint(mousePosition))
                .OrderByDescending(c => c.ZOrder);

            foreach (var child in sortedChildren)
            {
                child.OnClick(mousePosition);
                break; // Only the topmost child
            }
        }
    }
}
