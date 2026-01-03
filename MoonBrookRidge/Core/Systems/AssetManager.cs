using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Manages loading and caching of game assets with support for lazy loading and unloading.
/// Provides organized access to the 11,000+ Sunnyside World assets.
/// </summary>
public class AssetManager
{
    private readonly ContentManager _content;
    private readonly GraphicsDevice _graphicsDevice;
    private readonly Dictionary<string, Texture2D> _textureCache;
    private readonly Dictionary<string, List<string>> _categoryIndex;
    
    public AssetManager(ContentManager content, GraphicsDevice graphicsDevice)
    {
        _content = content;
        _graphicsDevice = graphicsDevice;
        _textureCache = new Dictionary<string, Texture2D>();
        _categoryIndex = new Dictionary<string, List<string>>();
        
        BuildCategoryIndex();
    }
    
    /// <summary>
    /// Build an index of available assets by category for quick lookup
    /// </summary>
    private void BuildCategoryIndex()
    {
        _categoryIndex["Tiles"] = new List<string>();
        _categoryIndex["Characters"] = new List<string>();
        _categoryIndex["Crops"] = new List<string>();
        _categoryIndex["Buildings"] = new List<string>();
        _categoryIndex["Resources"] = new List<string>();
        _categoryIndex["Decorations"] = new List<string>();
        _categoryIndex["Effects"] = new List<string>();
        _categoryIndex["Enemies"] = new List<string>();
        _categoryIndex["Objects"] = new List<string>();
    }
    
    /// <summary>
    /// Get a texture, loading it if not already cached
    /// </summary>
    public Texture2D GetTexture(string path)
    {
        // Normalize path
        path = NormalizePath(path);
        
        // Check cache first
        if (_textureCache.TryGetValue(path, out var cachedTexture))
        {
            return cachedTexture;
        }
        
        try
        {
            // Load from Content Pipeline
            var texture = _content.Load<Texture2D>(path);
            _textureCache[path] = texture;
            return texture;
        }
        catch (ContentLoadException)
        {
            // If not in Content Pipeline, try loading from file system
            return LoadTextureFromFile(path);
        }
    }
    
    /// <summary>
    /// Load a texture directly from file system (bypasses Content Pipeline)
    /// Useful for dynamically loading the organized Sunnyside assets
    /// </summary>
    private Texture2D LoadTextureFromFile(string path)
    {
        // Try various possible paths
        string[] possiblePaths = {
            Path.Combine("Content", "Textures", $"{path}.png"),
            Path.Combine("Content", "Textures", path),
            Path.Combine("Content", $"{path}.png"),
            path
        };
        
        foreach (var filePath in possiblePaths)
        {
            if (File.Exists(filePath))
            {
                try
                {
                    using var fileStream = new FileStream(filePath, FileMode.Open);
                    var texture = Texture2D.FromStream(_graphicsDevice, fileStream);
                    _textureCache[path] = texture;
                    return texture;
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"Error loading texture from {filePath}: {ex.Message}");
                }
            }
        }
        
        // Return a fallback texture (1x1 magenta pixel)
        return CreateFallbackTexture();
    }
    
    /// <summary>
    /// Create a 1x1 magenta texture as a fallback for missing assets
    /// </summary>
    private Texture2D CreateFallbackTexture()
    {
        var texture = new Texture2D(_graphicsDevice, 1, 1);
        texture.SetData(new[] { Microsoft.Xna.Framework.Color.Magenta });
        return texture;
    }
    
    /// <summary>
    /// Normalize path separators and remove extensions
    /// </summary>
    private string NormalizePath(string path)
    {
        // Remove .png extension if present
        if (path.EndsWith(".png"))
        {
            path = path.Substring(0, path.Length - 4);
        }
        
        // Normalize slashes
        path = path.Replace('\\', '/');
        
        return path;
    }
    
    /// <summary>
    /// Preload all assets in a category (e.g., "Tiles", "Characters")
    /// </summary>
    public void PreloadCategory(string category)
    {
        if (!_categoryIndex.ContainsKey(category))
        {
            Console.WriteLine($"Warning: Unknown category '{category}'");
            return;
        }
        
        var assetsToLoad = _categoryIndex[category];
        Console.WriteLine($"Preloading {assetsToLoad.Count} assets from category '{category}'...");
        
        int loaded = 0;
        foreach (var assetPath in assetsToLoad)
        {
            GetTexture(assetPath);
            loaded++;
            
            if (loaded % 100 == 0)
            {
                Console.WriteLine($"  Loaded {loaded}/{assetsToLoad.Count}...");
            }
        }
        
        Console.WriteLine($"Preloaded {loaded} assets from '{category}'");
    }
    
    /// <summary>
    /// Unload all cached textures in a category to free memory
    /// </summary>
    public void UnloadCategory(string category)
    {
        var toRemove = _textureCache.Keys
            .Where(k => k.Contains(category, StringComparison.OrdinalIgnoreCase))
            .ToList();
        
        foreach (var key in toRemove)
        {
            if (_textureCache.TryGetValue(key, out var texture))
            {
                texture?.Dispose();
                _textureCache.Remove(key);
            }
        }
        
        Console.WriteLine($"Unloaded {toRemove.Count} assets from category '{category}'");
    }
    
    /// <summary>
    /// Get all cached texture count (for debugging/monitoring)
    /// </summary>
    public int GetCachedTextureCount() => _textureCache.Count;
    
    /// <summary>
    /// Clear all cached textures
    /// </summary>
    public void ClearCache()
    {
        foreach (var texture in _textureCache.Values)
        {
            texture?.Dispose();
        }
        _textureCache.Clear();
        
        Console.WriteLine("Asset cache cleared");
    }
    
    /// <summary>
    /// Get crop texture by type and growth stage
    /// </summary>
    public Texture2D GetCropTexture(string cropType, int growthStage)
    {
        string path = $"Crops/{cropType}/{cropType}_{growthStage:D2}";
        return GetTexture(path);
    }
    
    /// <summary>
    /// Get character animation strip
    /// </summary>
    public Texture2D GetCharacterAnimation(string action, string part)
    {
        // action: WALKING, MINING, FISHING, etc.
        // part: base, longhair, shorthair, tools, etc.
        string path = $"Characters/{action}/{part}_{action.ToLower()}_strip";
        return GetTexture(path);
    }
    
    /// <summary>
    /// Get building sprite
    /// </summary>
    public Texture2D GetBuilding(string buildingType, string color = "Black")
    {
        string path = $"Buildings/{color} Buildings/{buildingType}";
        return GetTexture(path);
    }
    
    /// <summary>
    /// Get resource sprite (with optional highlight)
    /// </summary>
    public Texture2D GetResource(string resourceType, bool highlight = false)
    {
        string suffix = highlight ? "_Highlight" : "";
        string path = $"Resources/{resourceType}{suffix}";
        return GetTexture(path);
    }
    
    /// <summary>
    /// Get decoration sprite
    /// </summary>
    public Texture2D GetDecoration(string decorationType, int variant = 1)
    {
        string path = $"Decorations/{decorationType}/{decorationType}{variant}";
        return GetTexture(path);
    }
    
    /// <summary>
    /// Register an asset path in the category index
    /// Call this during initialization to populate the index
    /// </summary>
    public void RegisterAsset(string category, string assetPath)
    {
        if (!_categoryIndex.ContainsKey(category))
        {
            _categoryIndex[category] = new List<string>();
        }
        
        if (!_categoryIndex[category].Contains(assetPath))
        {
            _categoryIndex[category].Add(assetPath);
        }
    }
    
    /// <summary>
    /// Get memory usage statistics
    /// </summary>
    public AssetStats GetStats()
    {
        return new AssetStats
        {
            TotalCachedTextures = _textureCache.Count,
            CategoriesAvailable = _categoryIndex.Count,
            TotalRegisteredAssets = _categoryIndex.Values.Sum(list => list.Count)
        };
    }
}

/// <summary>
/// Statistics about loaded assets
/// </summary>
public struct AssetStats
{
    public int TotalCachedTextures { get; set; }
    public int CategoriesAvailable { get; set; }
    public int TotalRegisteredAssets { get; set; }
    
    public override string ToString()
    {
        return $"Cached: {TotalCachedTextures}, Registered: {TotalRegisteredAssets}, Categories: {CategoriesAvailable}";
    }
}
