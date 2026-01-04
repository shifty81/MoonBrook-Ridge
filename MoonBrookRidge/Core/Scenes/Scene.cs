using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.World.Tiles;
using MoonBrookRidge.Core.Systems;
using System.Collections.Generic;

namespace MoonBrookRidge.Core.Scenes;

/// <summary>
/// Base class for all scenes in the game (interiors, exteriors, dungeons, etc.)
/// Similar to how Stardew Valley handles different locations
/// </summary>
public abstract class Scene
{
    protected Tile[,] _tiles;
    protected int _width;
    protected int _height;
    protected List<SceneObject> _objects;
    protected List<SceneTransition> _transitions;
    
    public string Name { get; protected set; }
    public string SceneId { get; protected set; }
    public SceneType Type { get; protected set; }
    public Scene ParentScene { get; set; }
    public int Width => _width;
    public int Height => _height;
    
    protected Scene(string name, string sceneId, SceneType type, int width, int height)
    {
        Name = name;
        SceneId = sceneId;
        Type = type;
        _width = width;
        _height = height;
        _tiles = new Tile[width, height];
        _objects = new List<SceneObject>();
        _transitions = new List<SceneTransition>();
    }
    
    /// <summary>
    /// Initialize the scene (called once when scene is created)
    /// </summary>
    public abstract void Initialize();
    
    /// <summary>
    /// Load content for this scene
    /// </summary>
    public abstract void LoadContent();
    
    /// <summary>
    /// Update scene logic
    /// </summary>
    public virtual void Update(GameTime gameTime)
    {
        foreach (var obj in _objects)
        {
            obj.Update(gameTime);
        }
    }
    
    /// <summary>
    /// Draw the scene
    /// </summary>
    public abstract void Draw(SpriteBatch spriteBatch, Camera2D camera);
    
    /// <summary>
    /// Get tile at grid position
    /// </summary>
    public Tile GetTile(int x, int y)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            return _tiles[x, y];
        }
        return null;
    }
    
    /// <summary>
    /// Set tile at grid position
    /// </summary>
    protected void SetTile(int x, int y, Tile tile)
    {
        if (x >= 0 && x < _width && y >= 0 && y < _height)
        {
            _tiles[x, y] = tile;
        }
    }
    
    /// <summary>
    /// Add a scene object (furniture, decoration, NPC, etc.)
    /// </summary>
    public void AddObject(SceneObject obj)
    {
        _objects.Add(obj);
    }
    
    /// <summary>
    /// Get all scene objects
    /// </summary>
    public List<SceneObject> GetObjects()
    {
        return _objects;
    }
    
    /// <summary>
    /// Add a transition point (door, portal, stairs, etc.)
    /// </summary>
    public void AddTransition(SceneTransition transition)
    {
        _transitions.Add(transition);
    }
    
    /// <summary>
    /// Check if player is at a transition point
    /// </summary>
    public SceneTransition GetTransitionAtPosition(Vector2 position)
    {
        foreach (var transition in _transitions)
        {
            if (transition.IsPlayerAtTransition(position))
            {
                return transition;
            }
        }
        return null;
    }
    
    /// <summary>
    /// Get all transitions in this scene
    /// </summary>
    public List<SceneTransition> GetTransitions()
    {
        return _transitions;
    }
    
    /// <summary>
    /// Check if position is blocked by collision
    /// </summary>
    public virtual bool IsPositionBlocked(Vector2 position)
    {
        int gridX = (int)(position.X / GameConstants.TILE_SIZE);
        int gridY = (int)(position.Y / GameConstants.TILE_SIZE);
        
        var tile = GetTile(gridX, gridY);
        if (tile != null && tile.IsBlocking())
        {
            return true;
        }
        
        // Check objects
        foreach (var obj in _objects)
        {
            if (obj.IsBlocking && obj.GetBounds().Contains(position))
            {
                return true;
            }
        }
        
        return false;
    }
    
    /// <summary>
    /// Called when player enters this scene
    /// </summary>
    public virtual void OnEnter(Vector2 spawnPosition)
    {
        // Override in derived classes
    }
    
    /// <summary>
    /// Called when player exits this scene
    /// </summary>
    public virtual void OnExit()
    {
        // Override in derived classes
    }
}

/// <summary>
/// Types of scenes in the game
/// </summary>
public enum SceneType
{
    Interior,      // Indoors (houses, shops, etc.)
    Exterior,      // Outdoors (farm, villages, etc.)
    Dungeon,       // Dungeon floor
    DungeonOverworld, // Dungeon base camp for building
    Cave,          // Mining cave
    Special        // Special event scenes
}
