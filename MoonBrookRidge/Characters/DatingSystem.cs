using System;
using System.Collections.Generic;
using MoonBrookRidge.Characters.NPCs;

namespace MoonBrookRidge.Characters;

/// <summary>
/// Dating stages for NPC relationships
/// </summary>
public enum DatingStage
{
    None,           // Not dating (0-4 hearts)
    Friend,         // Friends (5-6 hearts)
    Dating,         // Dating (7-8 hearts)
    Engaged,        // Engaged (9-10 hearts)
    Married         // Married
}

/// <summary>
/// Represents a dating relationship between NPCs
/// </summary>
public class NPCRelationship
{
    public NPCCharacter NPC1 { get; set; }
    public NPCCharacter NPC2 { get; set; }
    public RelationshipType Type { get; set; }
    public int StrengthLevel { get; set; } // -100 to 100 (negative = rivalry, positive = friendship)
    
    public NPCRelationship(NPCCharacter npc1, NPCCharacter npc2, RelationshipType type, int strength = 0)
    {
        NPC1 = npc1;
        NPC2 = npc2;
        Type = type;
        StrengthLevel = strength;
    }
}

/// <summary>
/// Types of NPC-to-NPC relationships
/// </summary>
public enum RelationshipType
{
    Friend,
    Rival,
    Family,
    Romantic
}

/// <summary>
/// Enhanced dating system with multiple stages and NPC-to-NPC relationships
/// </summary>
public class DatingSystem
{
    private Dictionary<NPCCharacter, DatingStage> _playerRelationships;
    private List<NPCRelationship> _npcRelationships;
    private Dictionary<NPCCharacter, int> _jealousyLevels; // Tracks jealousy when dating multiple NPCs
    
    // Dating requirements
    private const int HEARTS_FOR_FRIEND = 5;      // 1250 friendship points
    private const int HEARTS_FOR_DATING = 7;      // 1750 friendship points
    private const int HEARTS_FOR_ENGAGED = 9;     // 2250 friendship points
    private const int HEARTS_FOR_MARRIED = 10;    // 2500 friendship points
    
    private const int POINTS_PER_HEART = 250;
    
    // Events
    public event Action<NPCCharacter, DatingStage>? OnRelationshipChanged;
    public event Action<NPCCharacter, int>? OnJealousyIncreased; // NPC, jealousy level
    public event Action<NPCRelationship>? OnNPCRelationshipChanged;
    
    public DatingSystem()
    {
        _playerRelationships = new Dictionary<NPCCharacter, DatingStage>();
        _npcRelationships = new List<NPCRelationship>();
        _jealousyLevels = new Dictionary<NPCCharacter, int>();
    }
    
    /// <summary>
    /// Get the current dating stage with an NPC
    /// </summary>
    public DatingStage GetDatingStage(NPCCharacter npc)
    {
        if (_playerRelationships.TryGetValue(npc, out var stage))
            return stage;
        return DatingStage.None;
    }
    
    /// <summary>
    /// Update dating stage based on friendship level
    /// Call this when friendship changes
    /// </summary>
    public void UpdateDatingStage(NPCCharacter npc, int friendshipPoints)
    {
        int hearts = friendshipPoints / POINTS_PER_HEART;
        
        DatingStage newStage = hearts switch
        {
            >= HEARTS_FOR_MARRIED => DatingStage.Married,
            >= HEARTS_FOR_ENGAGED => DatingStage.Engaged,
            >= HEARTS_FOR_DATING => DatingStage.Dating,
            >= HEARTS_FOR_FRIEND => DatingStage.Friend,
            _ => DatingStage.None
        };
        
        DatingStage oldStage = GetDatingStage(npc);
        
        if (newStage != oldStage)
        {
            _playerRelationships[npc] = newStage;
            OnRelationshipChanged?.Invoke(npc, newStage);
            
            // Check for jealousy from other romantic partners
            if (newStage == DatingStage.Dating || newStage == DatingStage.Engaged)
            {
                CheckForJealousy(npc);
            }
        }
    }
    
    /// <summary>
    /// Check if dating multiple NPCs and increase jealousy
    /// </summary>
    private void CheckForJealousy(NPCCharacter newPartner)
    {
        var romanticPartners = new List<NPCCharacter>();
        
        foreach (var kvp in _playerRelationships)
        {
            if (kvp.Value == DatingStage.Dating || kvp.Value == DatingStage.Engaged)
            {
                romanticPartners.Add(kvp.Key);
            }
        }
        
        // If dating multiple people, increase jealousy
        if (romanticPartners.Count > 1)
        {
            foreach (var partner in romanticPartners)
            {
                if (partner != newPartner)
                {
                    int currentJealousy = _jealousyLevels.GetValueOrDefault(partner, 0);
                    currentJealousy += 20; // Significant jealousy increase
                    _jealousyLevels[partner] = currentJealousy;
                    
                    OnJealousyIncreased?.Invoke(partner, currentJealousy);
                    
                    // High jealousy can damage friendship
                    if (currentJealousy >= 100)
                    {
                        // Break up automatically if jealousy too high
                        _playerRelationships[partner] = DatingStage.Friend;
                        _jealousyLevels[partner] = 0;
                        OnRelationshipChanged?.Invoke(partner, DatingStage.Friend);
                    }
                }
            }
        }
    }
    
    /// <summary>
    /// Get jealousy level for an NPC (0-100+)
    /// </summary>
    public int GetJealousyLevel(NPCCharacter npc)
    {
        return _jealousyLevels.GetValueOrDefault(npc, 0);
    }
    
    /// <summary>
    /// Reduce jealousy by giving gifts or spending time
    /// </summary>
    public void ReduceJealousy(NPCCharacter npc, int amount)
    {
        if (_jealousyLevels.ContainsKey(npc))
        {
            _jealousyLevels[npc] = Math.Max(0, _jealousyLevels[npc] - amount);
        }
    }
    
    /// <summary>
    /// Add or update NPC-to-NPC relationship
    /// </summary>
    public void SetNPCRelationship(NPCCharacter npc1, NPCCharacter npc2, RelationshipType type, int strength)
    {
        var existing = _npcRelationships.Find(r => 
            (r.NPC1 == npc1 && r.NPC2 == npc2) || 
            (r.NPC1 == npc2 && r.NPC2 == npc1));
        
        if (existing != null)
        {
            existing.Type = type;
            existing.StrengthLevel = strength;
            OnNPCRelationshipChanged?.Invoke(existing);
        }
        else
        {
            var newRelationship = new NPCRelationship(npc1, npc2, type, strength);
            _npcRelationships.Add(newRelationship);
            OnNPCRelationshipChanged?.Invoke(newRelationship);
        }
    }
    
    /// <summary>
    /// Get relationship between two NPCs
    /// </summary>
    public NPCRelationship? GetNPCRelationship(NPCCharacter npc1, NPCCharacter npc2)
    {
        return _npcRelationships.Find(r => 
            (r.NPC1 == npc1 && r.NPC2 == npc2) || 
            (r.NPC1 == npc2 && r.NPC2 == npc1));
    }
    
    /// <summary>
    /// Get all relationships for an NPC
    /// </summary>
    public List<NPCRelationship> GetNPCRelationships(NPCCharacter npc)
    {
        return _npcRelationships.FindAll(r => r.NPC1 == npc || r.NPC2 == npc);
    }
    
    /// <summary>
    /// Check if two NPCs are rivals
    /// </summary>
    public bool AreRivals(NPCCharacter npc1, NPCCharacter npc2)
    {
        var relationship = GetNPCRelationship(npc1, npc2);
        return relationship != null && relationship.Type == RelationshipType.Rival;
    }
    
    /// <summary>
    /// Check if two NPCs are friends
    /// </summary>
    public bool AreFriends(NPCCharacter npc1, NPCCharacter npc2)
    {
        var relationship = GetNPCRelationship(npc1, npc2);
        return relationship != null && relationship.Type == RelationshipType.Friend && relationship.StrengthLevel > 50;
    }
    
    /// <summary>
    /// Initialize default NPC relationships
    /// Call this when setting up the game
    /// </summary>
    public void InitializeDefaultRelationships(List<NPCCharacter> npcs)
    {
        // Example: Emma and Oliver are friends (both love nature)
        // Marcus and Jack are rivals (both craftsmen)
        // Lily and Maya are friends (both appreciate beauty)
        
        // This would be expanded based on NPC personalities
        if (npcs.Count >= 7)
        {
            // Friends
            SetNPCRelationship(npcs[0], npcs[3], RelationshipType.Friend, 60); // Emma & Oliver
            SetNPCRelationship(npcs[2], npcs[6], RelationshipType.Friend, 70); // Lily & Maya
            
            // Rivals
            SetNPCRelationship(npcs[1], npcs[5], RelationshipType.Rival, -40); // Marcus & Jack
        }
    }
    
    /// <summary>
    /// Get count of NPCs at each dating stage
    /// </summary>
    public Dictionary<DatingStage, int> GetDatingStatistics()
    {
        var stats = new Dictionary<DatingStage, int>();
        
        foreach (DatingStage stage in Enum.GetValues(typeof(DatingStage)))
        {
            stats[stage] = 0;
        }
        
        foreach (var kvp in _playerRelationships)
        {
            stats[kvp.Value]++;
        }
        
        return stats;
    }
    
    /// <summary>
    /// Get dialogue modifier based on dating stage and jealousy
    /// </summary>
    public string GetDialogueModifier(NPCCharacter npc)
    {
        var stage = GetDatingStage(npc);
        int jealousy = GetJealousyLevel(npc);
        
        if (jealousy > 70)
            return "very_jealous";
        if (jealousy > 40)
            return "jealous";
        
        return stage switch
        {
            DatingStage.Engaged => "engaged",
            DatingStage.Dating => "dating",
            DatingStage.Friend => "friendly",
            DatingStage.Married => "married",
            _ => "neutral"
        };
    }
}
