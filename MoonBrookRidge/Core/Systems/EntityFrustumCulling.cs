using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Manages frustum culling for NPCs, enemies, and other entities
/// Only updates and renders entities within or near the camera viewport
/// Provides significant performance improvements for large worlds with many entities
/// </summary>
public class EntityFrustumCulling
{
    private Rectangle _visibleBounds;
    private Rectangle _updateBounds;
    
    // Buffer zones around viewport
    private const int RENDER_BUFFER = 128;  // Extra pixels to render around viewport (prevent pop-in)
    private const int UPDATE_BUFFER = 256;   // Extra pixels to update around viewport (AI, movement, etc.)
    
    /// <summary>
    /// Updates the viewport bounds based on camera
    /// </summary>
    /// <param name="cameraPosition">Center position of the camera</param>
    /// <param name="viewportWidth">Width of the viewport</param>
    /// <param name="viewportHeight">Height of the viewport</param>
    /// <param name="zoom">Camera zoom level</param>
    public void UpdateViewport(Vector2 cameraPosition, int viewportWidth, int viewportHeight, float zoom)
    {
        // Calculate actual visible area based on zoom
        int scaledWidth = (int)(viewportWidth / zoom);
        int scaledHeight = (int)(viewportHeight / zoom);
        
        // Visible bounds (for rendering)
        _visibleBounds = new Rectangle(
            (int)(cameraPosition.X - scaledWidth / 2) - RENDER_BUFFER,
            (int)(cameraPosition.Y - scaledHeight / 2) - RENDER_BUFFER,
            scaledWidth + (RENDER_BUFFER * 2),
            scaledHeight + (RENDER_BUFFER * 2)
        );
        
        // Update bounds (larger area for AI and movement)
        _updateBounds = new Rectangle(
            (int)(cameraPosition.X - scaledWidth / 2) - UPDATE_BUFFER,
            (int)(cameraPosition.Y - scaledHeight / 2) - UPDATE_BUFFER,
            scaledWidth + (UPDATE_BUFFER * 2),
            scaledHeight + (UPDATE_BUFFER * 2)
        );
    }
    
    /// <summary>
    /// Checks if an entity should be rendered (is visible on screen)
    /// </summary>
    /// <param name="position">Entity position</param>
    /// <returns>True if entity should be rendered</returns>
    public bool IsVisible(Vector2 position)
    {
        return _visibleBounds.Contains((int)position.X, (int)position.Y);
    }
    
    /// <summary>
    /// Checks if an entity should be rendered (with custom bounds)
    /// </summary>
    /// <param name="bounds">Entity bounds</param>
    /// <returns>True if entity should be rendered</returns>
    public bool IsVisible(Rectangle bounds)
    {
        return _visibleBounds.Intersects(bounds);
    }
    
    /// <summary>
    /// Checks if an entity should be updated (is near the viewport)
    /// </summary>
    /// <param name="position">Entity position</param>
    /// <returns>True if entity should be updated</returns>
    public bool ShouldUpdate(Vector2 position)
    {
        return _updateBounds.Contains((int)position.X, (int)position.Y);
    }
    
    /// <summary>
    /// Checks if an entity should be updated (with custom bounds)
    /// </summary>
    /// <param name="bounds">Entity bounds</param>
    /// <returns>True if entity should be updated</returns>
    public bool ShouldUpdate(Rectangle bounds)
    {
        return _updateBounds.Intersects(bounds);
    }
    
    /// <summary>
    /// Filters a list of entities to only those that should be updated
    /// </summary>
    public List<T> FilterForUpdate<T>(List<T> entities) where T : class
    {
        var result = new List<T>();
        
        foreach (var entity in entities)
        {
            Vector2 position = GetEntityPosition(entity);
            if (ShouldUpdate(position))
            {
                result.Add(entity);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Filters a list of entities to only those that should be rendered
    /// </summary>
    public List<T> FilterForRender<T>(List<T> entities) where T : class
    {
        var result = new List<T>();
        
        foreach (var entity in entities)
        {
            Vector2 position = GetEntityPosition(entity);
            if (IsVisible(position))
            {
                result.Add(entity);
            }
        }
        
        return result;
    }
    
    /// <summary>
    /// Gets the visible bounds rectangle
    /// </summary>
    public Rectangle GetVisibleBounds() => _visibleBounds;
    
    /// <summary>
    /// Gets the update bounds rectangle
    /// </summary>
    public Rectangle GetUpdateBounds() => _updateBounds;
    
    /// <summary>
    /// Extracts position from entity object using reflection/pattern matching
    /// Supports NPCs, Enemies, and other entities with Position property
    /// </summary>
    private Vector2 GetEntityPosition(object entity)
    {
        // Use pattern matching to get Position from various entity types
        var positionProperty = entity.GetType().GetProperty("Position");
        if (positionProperty != null && positionProperty.PropertyType == typeof(Vector2))
        {
            return (Vector2)positionProperty.GetValue(entity);
        }
        
        // Default to origin if position cannot be determined
        return Vector2.Zero;
    }
    
    /// <summary>
    /// Calculates culling statistics for debugging
    /// </summary>
    public CullingStats CalculateStats<T>(List<T> allEntities) where T : class
    {
        int total = allEntities.Count;
        int visible = 0;
        int updatable = 0;
        
        foreach (var entity in allEntities)
        {
            Vector2 pos = GetEntityPosition(entity);
            if (IsVisible(pos)) visible++;
            if (ShouldUpdate(pos)) updatable++;
        }
        
        return new CullingStats
        {
            TotalEntities = total,
            VisibleEntities = visible,
            UpdatableEntities = updatable,
            CulledForRender = total - visible,
            CulledForUpdate = total - updatable
        };
    }
}

/// <summary>
/// Statistics about frustum culling performance
/// </summary>
public struct CullingStats
{
    public int TotalEntities;
    public int VisibleEntities;
    public int UpdatableEntities;
    public int CulledForRender;
    public int CulledForUpdate;
    
    public float RenderCullPercentage => TotalEntities > 0 
        ? (CulledForRender / (float)TotalEntities) * 100f 
        : 0f;
    
    public float UpdateCullPercentage => TotalEntities > 0 
        ? (CulledForUpdate / (float)TotalEntities) * 100f 
        : 0f;
}
