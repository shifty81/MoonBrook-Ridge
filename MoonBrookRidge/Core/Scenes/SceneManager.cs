using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.Core.Scenes;

/// <summary>
/// Central manager for all scenes in the game
/// Handles scene loading, transitions, and the scene graph
/// </summary>
public class SceneManager
{
    private Dictionary<string, Scene> _scenes;
    private Scene _currentScene;
    private Scene _previousScene;
    private bool _isTransitioning;
    private float _transitionProgress;
    private const float TRANSITION_DURATION = 0.5f; // 0.5 seconds
    
    public Scene CurrentScene => _currentScene;
    public Scene PreviousScene => _previousScene;
    public bool IsTransitioning => _isTransitioning;
    
    public event Action<Scene, Scene> OnSceneChanged; // (from, to)
    public event Action<SceneTransition> OnTransitionStarted;
    public event Action OnTransitionCompleted;
    
    public SceneManager()
    {
        _scenes = new Dictionary<string, Scene>();
        _isTransitioning = false;
        _transitionProgress = 0f;
    }
    
    /// <summary>
    /// Register a scene with the manager
    /// </summary>
    public void RegisterScene(Scene scene)
    {
        if (!_scenes.ContainsKey(scene.SceneId))
        {
            _scenes.Add(scene.SceneId, scene);
            scene.Initialize();
            scene.LoadContent();
        }
    }
    
    /// <summary>
    /// Get a scene by ID
    /// </summary>
    public Scene GetScene(string sceneId)
    {
        if (_scenes.TryGetValue(sceneId, out var scene))
        {
            return scene;
        }
        return null;
    }
    
    /// <summary>
    /// Set the current active scene
    /// </summary>
    public void SetCurrentScene(string sceneId, Vector2 spawnPosition)
    {
        if (_scenes.TryGetValue(sceneId, out var newScene))
        {
            _previousScene = _currentScene;
            _currentScene = newScene;
            
            _previousScene?.OnExit();
            _currentScene.OnEnter(spawnPosition);
            
            OnSceneChanged?.Invoke(_previousScene, _currentScene);
        }
    }
    
    /// <summary>
    /// Transition to a new scene using a transition object
    /// </summary>
    public void TransitionToScene(SceneTransition transition)
    {
        if (_isTransitioning) return;
        
        var targetScene = GetScene(transition.TargetSceneId);
        if (targetScene == null) return;
        
        _isTransitioning = true;
        _transitionProgress = 0f;
        
        OnTransitionStarted?.Invoke(transition);
        
        // Immediate transition for now (can add fade effects later)
        SetCurrentScene(transition.TargetSceneId, transition.TargetSpawnPosition * GameConstants.TILE_SIZE);
        
        _isTransitioning = false;
        OnTransitionCompleted?.Invoke();
    }
    
    /// <summary>
    /// Update the current scene and handle transitions
    /// </summary>
    public void Update(GameTime gameTime)
    {
        if (_isTransitioning)
        {
            _transitionProgress += (float)gameTime.ElapsedGameTime.TotalSeconds / TRANSITION_DURATION;
            
            if (_transitionProgress >= 1f)
            {
                _isTransitioning = false;
                _transitionProgress = 0f;
                OnTransitionCompleted?.Invoke();
            }
        }
        
        _currentScene?.Update(gameTime);
    }
    
    /// <summary>
    /// Draw the current scene
    /// </summary>
    public void Draw(SpriteBatch spriteBatch, Camera2D camera)
    {
        _currentScene?.Draw(spriteBatch, camera);
        
        // Draw transition effect if transitioning
        if (_isTransitioning)
        {
            // Simple fade effect (can be enhanced later)
            float alpha = _transitionProgress < 0.5f 
                ? _transitionProgress * 2f 
                : (1f - _transitionProgress) * 2f;
            
            // Draw overlay (requires pixel texture)
            // TODO: Add fade overlay
        }
    }
    
    /// <summary>
    /// Get all registered scenes
    /// </summary>
    public IEnumerable<Scene> GetAllScenes()
    {
        return _scenes.Values;
    }
    
    /// <summary>
    /// Check if a scene is registered
    /// </summary>
    public bool HasScene(string sceneId)
    {
        return _scenes.ContainsKey(sceneId);
    }
    
    /// <summary>
    /// Remove a scene from the manager
    /// </summary>
    public void UnregisterScene(string sceneId)
    {
        if (_scenes.ContainsKey(sceneId))
        {
            var scene = _scenes[sceneId];
            if (scene == _currentScene)
            {
                throw new InvalidOperationException("Cannot unregister the current active scene");
            }
            _scenes.Remove(sceneId);
        }
    }
}
