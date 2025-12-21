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
        _schedule.Update(timeSystem);
        
        // AI behavior based on schedule
        UpdateBehavior();
    }
    
    private void UpdateBehavior()
    {
        // NPC behavior logic (walking to locations, performing actions, etc.)
    }
    
    public void Draw(SpriteBatch spriteBatch)
    {
        // Draw NPC sprite
        // Placeholder for now
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
}

/// <summary>
/// Daily schedule for NPCs (where they are at different times)
/// </summary>
public class NPCSchedule
{
    private Dictionary<float, ScheduleLocation> _schedule;
    
    public NPCSchedule()
    {
        _schedule = new Dictionary<float, ScheduleLocation>();
    }
    
    public void AddScheduleEntry(float time, ScheduleLocation location)
    {
        _schedule[time] = location;
    }
    
    public void Update(TimeSystem timeSystem)
    {
        // Determine current location based on time of day
    }
}

public class ScheduleLocation
{
    public Vector2 Position { get; set; }
    public string LocationName { get; set; }
    public string Activity { get; set; }
}
