using MoonBrookEngine.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using Vec2 = MoonBrookEngine.Math.Vector2;

namespace MoonBrookEngine.UI
{
    /// <summary>
    /// System that manages and updates all UI elements.
    /// Handles input events, layout updates, and rendering order.
    /// </summary>
    public class UISystem
    {
        private readonly List<UIElement> _elements = new();
        private readonly List<UIElement> _elementsToAdd = new();
        private readonly List<UIElement> _elementsToRemove = new();
        private UIElement? _hoveredElement;
        private UIElement? _pressedElement;
        private Vec2 _lastMousePosition;
        private bool _lastMouseButtonState;

        /// <summary>
        /// Gets the number of UI elements currently managed by this system.
        /// </summary>
        public int ElementCount => _elements.Count;

        /// <summary>
        /// Adds a UI element to the system.
        /// </summary>
        public void AddElement(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            _elementsToAdd.Add(element);
        }

        /// <summary>
        /// Removes a UI element from the system.
        /// </summary>
        public void RemoveElement(UIElement element)
        {
            if (element == null)
                throw new ArgumentNullException(nameof(element));

            _elementsToRemove.Add(element);
        }

        /// <summary>
        /// Finds a UI element by its ID.
        /// </summary>
        public UIElement? FindElementById(string id)
        {
            return _elements.FirstOrDefault(e => e.Id == id);
        }

        /// <summary>
        /// Gets all UI elements of a specific type.
        /// </summary>
        public IEnumerable<T> GetElementsOfType<T>() where T : UIElement
        {
            return _elements.OfType<T>();
        }

        /// <summary>
        /// Clears all UI elements from the system.
        /// </summary>
        public void Clear()
        {
            _elements.Clear();
            _elementsToAdd.Clear();
            _elementsToRemove.Clear();
            _hoveredElement = null;
            _pressedElement = null;
        }

        /// <summary>
        /// Updates all UI elements and handles input events.
        /// </summary>
        /// <param name="deltaTime">Time since last update in seconds.</param>
        /// <param name="mousePosition">Current mouse position.</param>
        /// <param name="isMouseButtonDown">Whether the left mouse button is currently pressed.</param>
        public void Update(float deltaTime, Vec2 mousePosition, bool isMouseButtonDown)
        {
            // Process pending additions and removals
            ProcessPendingChanges();

            // Update all elements
            foreach (var element in _elements.Where(e => e.IsEnabled))
            {
                element.Update(deltaTime);
            }

            // Handle mouse hover
            HandleMouseHover(mousePosition);

            // Handle mouse clicks
            HandleMouseInput(mousePosition, isMouseButtonDown);

            _lastMousePosition = mousePosition;
            _lastMouseButtonState = isMouseButtonDown;
        }

        /// <summary>
        /// Renders all visible UI elements in Z-order.
        /// </summary>
        public void Draw(SpriteBatch spriteBatch)
        {
            // Sort by Z-order (lower values rendered first, higher values on top)
            var sortedElements = _elements
                .Where(e => e.IsVisible)
                .OrderBy(e => e.ZOrder);

            foreach (var element in sortedElements)
            {
                element.Draw(spriteBatch);
            }
        }

        private void ProcessPendingChanges()
        {
            // Remove elements
            foreach (var element in _elementsToRemove)
            {
                _elements.Remove(element);
                if (_hoveredElement == element)
                    _hoveredElement = null;
                if (_pressedElement == element)
                    _pressedElement = null;
            }
            _elementsToRemove.Clear();

            // Add elements
            _elements.AddRange(_elementsToAdd);
            _elementsToAdd.Clear();
        }

        private void HandleMouseHover(Vec2 mousePosition)
        {
            // Find the topmost element under the mouse (highest Z-order)
            var elementUnderMouse = _elements
                .Where(e => e.IsVisible && e.IsEnabled && e.ContainsPoint(mousePosition))
                .OrderByDescending(e => e.ZOrder)
                .FirstOrDefault();

            // Handle mouse exit/enter
            if (_hoveredElement != elementUnderMouse)
            {
                _hoveredElement?.OnMouseExit(_lastMousePosition);
                elementUnderMouse?.OnMouseEnter(mousePosition);
                _hoveredElement = elementUnderMouse;
            }
        }

        private void HandleMouseInput(Vec2 mousePosition, bool isMouseButtonDown)
        {
            // Mouse button pressed
            if (isMouseButtonDown && !_lastMouseButtonState)
            {
                if (_hoveredElement != null)
                {
                    _pressedElement = _hoveredElement;
                    _pressedElement.OnMouseDown(mousePosition);
                }
            }
            // Mouse button released
            else if (!isMouseButtonDown && _lastMouseButtonState)
            {
                if (_pressedElement != null)
                {
                    _pressedElement.OnMouseUp(mousePosition);

                    // If released on the same element, it's a click
                    if (_pressedElement == _hoveredElement)
                    {
                        _pressedElement.OnClick(mousePosition);
                    }

                    _pressedElement = null;
                }
            }
        }
    }
}
