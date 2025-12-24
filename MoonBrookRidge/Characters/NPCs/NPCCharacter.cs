using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System.Collections.Generic;
using MoonBrookRidge.Core.Systems;

namespace MoonBrookRidge.Characters.NPCs;

/// <summary>
/// Non-player character with AI, schedule, and dialogue
/// </summary>
public class NPCCharacter
{
    private Vector2 _position;
    private string _name;
    private int _friendshipLevel;
    private NPCSchedule _schedule;
    private Dictionary<string, DialogueTree> _dialogueTrees;
    private Texture2D _sprite;
    private Vector2 _velocity;
    private const float WALK_SPEED = 60f;
    private static Texture2D _pixelTexture;
    
    // NPC stats and preferences
    private List<string> _lovedGifts;
    private List<string> _likedGifts;
    private List<string> _dislikedGifts;
    private List<string> _hatedGifts;
    
    public NPCCharacter(string name, Vector2 startPosition)
    {
        _name = name;
        _position = startPosition;
        _friendshipLevel = 0;
        _schedule = new NPCSchedule();
        _dialogueTrees = new Dictionary<string, DialogueTree>();
        _velocity = Vector2.Zero;
        
        InitializePreferences();
    }
    
    private void InitializePreferences()
    {
        _lovedGifts = new List<string>();
        _likedGifts = new List<string>();
        _dislikedGifts = new List<string>();
        _hatedGifts = new List<string>();
    }
    
    public void Update(GameTime gameTime, TimeSystem timeSystem)
    {
        // Update NPC position based on schedule
        var targetLocation = _schedule.Update(timeSystem);
        
        // Move towards target location
        if (targetLocation != null && Vector2.Distance(_position, targetLocation.Position) > 5f)
        {
            Vector2 direction = targetLocation.Position - _position;
            direction.Normalize();
            _velocity = direction * WALK_SPEED;
        }
        else
        {
            _velocity = Vector2.Zero;
        }
        
        // Update position
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        _position += _velocity * deltaTime;
        
        // AI behavior based on schedule
        UpdateBehavior();
    }
    
    private void UpdateBehavior()
    {
        // NPC behavior logic (walking to locations, performing actions, etc.)
    }
    
    public void LoadSprite(Texture2D sprite)
    {
        _sprite = sprite;
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        if (_sprite != null)
        {
            // Draw NPC sprite at position
            spriteBatch.Draw(
                _sprite,
                _position,
                null,
                Color.White,
                0f,
                new Vector2(_sprite.Width / 2, _sprite.Height / 2), // Center origin
                2f, // Scale up 2x for visibility
                SpriteEffects.None,
                0f
            );
        }
        else
        {
            // Fallback: Draw a simple colored rectangle representing the NPC
            if (_pixelTexture == null)
            {
                _pixelTexture = new Texture2D(spriteBatch.GraphicsDevice, 1, 1);
                _pixelTexture.SetData(new[] { Color.White });
            }
            
            Rectangle npcRect = new Rectangle((int)_position.X - 16, (int)_position.Y - 16, 32, 32);
            spriteBatch.Draw(_pixelTexture, npcRect, Color.Green);
        }
    }
    
    public void GiveGift(string itemName)
    {
        if (_lovedGifts.Contains(itemName))
        {
            ModifyFriendship(80);
        }
        else if (_likedGifts.Contains(itemName))
        {
            ModifyFriendship(45);
        }
        else if (_dislikedGifts.Contains(itemName))
        {
            ModifyFriendship(-20);
        }
        else if (_hatedGifts.Contains(itemName))
        {
            ModifyFriendship(-40);
        }
        else
        {
            ModifyFriendship(20); // Neutral gift
        }
    }
    
    public void ModifyFriendship(int amount)
    {
        _friendshipLevel = MathHelper.Clamp(_friendshipLevel + amount, 0, 2500);
        // 2500 = 10 hearts at 250 points per heart
    }
    
    public int GetHeartLevel()
    {
        return _friendshipLevel / 250;
    }
    
    public DialogueTree GetDialogue(string dialogueKey)
    {
        if (_dialogueTrees.ContainsKey(dialogueKey))
        {
            return _dialogueTrees[dialogueKey];
        }
        return null;
    }
    
    public void AddDialogueTree(string key, DialogueTree tree)
    {
        _dialogueTrees[key] = tree;
    }
    
    // Properties
    public string Name => _name;
    public Vector2 Position => _position;
    public int FriendshipLevel => _friendshipLevel;
    public NPCSchedule Schedule => _schedule;
}

/// <summary>
/// Daily schedule for NPCs (where they are at different times)
/// </summary>
public class NPCSchedule
{
    private Dictionary<float, ScheduleLocation> _schedule;
    private ScheduleLocation _currentLocation;
    
    public NPCSchedule()
    {
        _schedule = new Dictionary<float, ScheduleLocation>();
        _currentLocation = null;
    }
    
    public void AddScheduleEntry(float time, ScheduleLocation location)
    {
        _schedule[time] = location;
    }
    
    public ScheduleLocation Update(TimeSystem timeSystem)
    {
        // Determine current location based on time of day
        float currentTime = timeSystem.TimeOfDay;
        
        // Find the most recent schedule entry before current time
        float closestTime = -1f;
        ScheduleLocation targetLocation = null;
        
        foreach (var entry in _schedule)
        {
            if (entry.Key <= currentTime && entry.Key > closestTime)
            {
                closestTime = entry.Key;
                targetLocation = entry.Value;
            }
        }
        
        if (targetLocation != null)
        {
            _currentLocation = targetLocation;
        }
        
        return _currentLocation;
    }
    
    public ScheduleLocation CurrentLocation => _currentLocation;
}

public class ScheduleLocation
{
    public Vector2 Position { get; set; }
    public string LocationName { get; set; }
    public string Activity { get; set; }
}
