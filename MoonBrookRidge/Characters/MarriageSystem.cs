using Microsoft.Xna.Framework;
using MoonBrookRidge.Characters.NPCs;
using MoonBrookRidge.Characters.Player;
using System;
using System.Collections.Generic;

namespace MoonBrookRidge.Characters;

/// <summary>
/// Manages marriage and romantic relationships in the game
/// </summary>
public class MarriageSystem
{
    private NPCCharacter _spouse;
    private bool _isMarried;
    private int _marriageDay;
    private int _marriageSeason;
    private int _marriageYear;
    private int _daysSinceMarriage;
    
    // Marriage requirements
    private const int HEARTS_REQUIRED_FOR_MARRIAGE = 10; // Max hearts
    private const int FRIENDSHIP_REQUIRED = 2500; // 10 hearts * 250 points
    
    // Events
    public event Action<NPCCharacter> OnMarried;
    public event Action OnDivorced;
    public event Action<NPCCharacter> OnProposalAccepted;
    public event Action<NPCCharacter> OnProposalRejected;
    
    public MarriageSystem()
    {
        _isMarried = false;
        _spouse = null;
        _daysSinceMarriage = 0;
    }
    
    /// <summary>
    /// Check if the player can propose to an NPC
    /// </summary>
    public bool CanPropose(NPCCharacter npc)
    {
        if (_isMarried) return false;
        if (npc == null) return false;
        
        // Must have maximum friendship level
        return npc.FriendshipLevel >= FRIENDSHIP_REQUIRED;
    }
    
    /// <summary>
    /// Propose marriage to an NPC
    /// </summary>
    public bool ProposeMarriage(NPCCharacter npc)
    {
        if (!CanPropose(npc))
        {
            OnProposalRejected?.Invoke(npc);
            return false;
        }
        
        // Acceptance is guaranteed if friendship is max
        OnProposalAccepted?.Invoke(npc);
        return true;
    }
    
    /// <summary>
    /// Complete the marriage ceremony (called after wedding event)
    /// </summary>
    public void MarryNPC(NPCCharacter npc, int currentDay, int currentSeason, int currentYear)
    {
        if (npc == null || _isMarried) return;
        
        _spouse = npc;
        _isMarried = true;
        _marriageDay = currentDay;
        _marriageSeason = currentSeason;
        _marriageYear = currentYear;
        _daysSinceMarriage = 0;
        
        OnMarried?.Invoke(npc);
    }
    
    /// <summary>
    /// Divorce the current spouse
    /// </summary>
    public void Divorce()
    {
        if (!_isMarried || _spouse == null) return;
        
        // Reset spouse friendship to 5 hearts (harsh penalty)
        _spouse.ModifyFriendship(-1250); // Reduce by half
        
        _spouse = null;
        _isMarried = false;
        _daysSinceMarriage = 0;
        
        OnDivorced?.Invoke();
    }
    
    /// <summary>
    /// Update marriage system (call each game day)
    /// </summary>
    public void Update()
    {
        if (_isMarried)
        {
            _daysSinceMarriage++;
        }
    }
    
    /// <summary>
    /// Get spouse greeting based on days married
    /// </summary>
    public string GetSpouseGreeting()
    {
        if (!_isMarried || _spouse == null) return "";
        
        string[] greetings = new string[]
        {
            $"Good morning, dear! Have a wonderful day!",
            $"Hey love! I made you breakfast.",
            $"Morning! Don't forget to water the crops!",
            $"You're up early! Want some coffee?",
            $"Another beautiful day on the farm!",
            $"I love living here with you.",
            $"The farm is looking great, darling!",
            $"Take care of yourself today, okay?"
        };
        
        int greetingIndex = (_daysSinceMarriage / 7) % greetings.Length;
        return greetings[greetingIndex];
    }
    
    /// <summary>
    /// Check if spouse might give you a gift or help today
    /// </summary>
    public bool SpouseHelpsToday()
    {
        if (!_isMarried) return false;
        
        // 30% chance spouse helps with farm work each day
        Random rand = new Random(_daysSinceMarriage);
        return rand.NextDouble() < 0.3;
    }
    
    /// <summary>
    /// Get what the spouse might help with
    /// </summary>
    public SpouseHelpType GetSpouseHelpType()
    {
        if (!_isMarried || !SpouseHelpsToday()) return SpouseHelpType.None;
        
        Random rand = new Random(_daysSinceMarriage + 100);
        int helpType = rand.Next(0, 4);
        
        return helpType switch
        {
            0 => SpouseHelpType.WaterCrops,
            1 => SpouseHelpType.FeedAnimals,
            2 => SpouseHelpType.RepairFences,
            3 => SpouseHelpType.CookFood,
            _ => SpouseHelpType.None
        };
    }
    
    // Properties
    public bool IsMarried => _isMarried;
    public NPCCharacter Spouse => _spouse;
    public int DaysSinceMarriage => _daysSinceMarriage;
    public int MarriageDay => _marriageDay;
    public int MarriageSeason => _marriageSeason;
    public int MarriageYear => _marriageYear;
}

/// <summary>
/// Types of help a spouse can provide
/// </summary>
public enum SpouseHelpType
{
    None,
    WaterCrops,     // Waters all crops
    FeedAnimals,    // Feeds all animals
    RepairFences,   // Repairs farm structures
    CookFood        // Provides a food item
}
