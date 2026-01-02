using System;
using System.Collections.Generic;
using System.Linq;
using MoonBrookRidge.Quests;

namespace MoonBrookRidge.Factions;

/// <summary>
/// Faction system for managing player reputation with different groups
/// </summary>
public class FactionSystem
{
    private List<Faction> _factions;
    private Dictionary<string, int> _playerReputation;
    
    public event Action<Faction, int> OnReputationChanged;
    public event Action<Faction, ReputationLevel> OnReputationLevelChanged;
    public event Action<Faction, FactionReward> OnRewardUnlocked;
    
    public FactionSystem()
    {
        _factions = new List<Faction>();
        _playerReputation = new Dictionary<string, int>();
        InitializeFactions();
    }
    
    /// <summary>
    /// Initialize all factions with their properties
    /// </summary>
    private void InitializeFactions()
    {
        // Farmers' Guild - Agriculture and farming
        var farmersGuild = new Faction(
            "farmers_guild",
            "Farmers' Guild",
            "A collective of farmers dedicated to agricultural excellence",
            FactionType.Economic
        );
        farmersGuild.AddReward(new FactionReward(ReputationLevel.Friendly, 
            "Seed Discount", "10% discount on all seeds"));
        farmersGuild.AddReward(new FactionReward(ReputationLevel.Honored, 
            "Crop Bonus", "+10% crop quality"));
        farmersGuild.AddReward(new FactionReward(ReputationLevel.Revered, 
            "Master Farmer Title", "Unlock exclusive farming recipes"));
        _factions.Add(farmersGuild);
        
        // Adventurers' League - Combat and exploration
        var adventurersLeague = new Faction(
            "adventurers_league",
            "Adventurers' League",
            "Bold explorers who seek treasure and glory in dungeons",
            FactionType.Combat
        );
        adventurersLeague.AddReward(new FactionReward(ReputationLevel.Friendly, 
            "Weapon Discount", "10% discount on weapons"));
        adventurersLeague.AddReward(new FactionReward(ReputationLevel.Honored, 
            "Combat Training", "+10% damage bonus"));
        adventurersLeague.AddReward(new FactionReward(ReputationLevel.Revered, 
            "Legendary Hero Title", "Access to legendary weapons"));
        _factions.Add(adventurersLeague);
        
        // Merchants' Coalition - Trading and commerce
        var merchantsCoalition = new Faction(
            "merchants_coalition",
            "Merchants' Coalition",
            "Shrewd traders who control the flow of goods and gold",
            FactionType.Economic
        );
        merchantsCoalition.AddReward(new FactionReward(ReputationLevel.Friendly, 
            "Better Prices", "5% better buying and selling prices"));
        merchantsCoalition.AddReward(new FactionReward(ReputationLevel.Honored, 
            "Merchant Network", "Access to rare items"));
        merchantsCoalition.AddReward(new FactionReward(ReputationLevel.Revered, 
            "Master Trader Title", "10% better prices, bulk discounts"));
        _factions.Add(merchantsCoalition);
        
        // Arcane Order - Magic and mysticism
        var arcaneOrder = new Faction(
            "arcane_order",
            "Arcane Order",
            "Ancient mages who guard magical knowledge",
            FactionType.Mystical
        );
        arcaneOrder.AddReward(new FactionReward(ReputationLevel.Friendly, 
            "Spell Discount", "Reduced mana costs for spells"));
        arcaneOrder.AddReward(new FactionReward(ReputationLevel.Honored, 
            "Arcane Knowledge", "Unlock advanced spells"));
        arcaneOrder.AddReward(new FactionReward(ReputationLevel.Revered, 
            "Archmage Title", "Maximum mana increased by 50"));
        _factions.Add(arcaneOrder);
        
        // Nature's Keepers - Environmental protection
        var naturesKeepers = new Faction(
            "natures_keepers",
            "Nature's Keepers",
            "Druids and rangers who protect the natural world",
            FactionType.Environmental
        );
        naturesKeepers.AddReward(new FactionReward(ReputationLevel.Friendly, 
            "Nature's Blessing", "Crops grow 5% faster"));
        naturesKeepers.AddReward(new FactionReward(ReputationLevel.Honored, 
            "Wild Harmony", "Animals are friendlier, easier to tame"));
        naturesKeepers.AddReward(new FactionReward(ReputationLevel.Revered, 
            "Guardian of Nature Title", "Crops never wilt, weather control"));
        _factions.Add(naturesKeepers);
        
        // Shadow Syndicate - Rogues and thieves
        var shadowSyndicate = new Faction(
            "shadow_syndicate",
            "Shadow Syndicate",
            "Secretive organization dealing in shadows and secrets",
            FactionType.Underground
        );
        shadowSyndicate.AddReward(new FactionReward(ReputationLevel.Friendly, 
            "Black Market Access", "Buy stolen goods at low prices"));
        shadowSyndicate.AddReward(new FactionReward(ReputationLevel.Honored, 
            "Shadow Skills", "Increased stealth and lockpicking"));
        shadowSyndicate.AddReward(new FactionReward(ReputationLevel.Revered, 
            "Shadow Master Title", "Access to forbidden items"));
        _factions.Add(shadowSyndicate);
        
        // Initialize reputation at neutral for all factions
        foreach (var faction in _factions)
        {
            _playerReputation[faction.Id] = 0;
        }
    }
    
    /// <summary>
    /// Change reputation with a faction
    /// </summary>
    public void ChangeReputation(string factionId, int change)
    {
        if (!_playerReputation.ContainsKey(factionId))
            return;
        
        var faction = GetFaction(factionId);
        if (faction == null) return;
        
        int oldReputation = _playerReputation[factionId];
        ReputationLevel oldLevel = GetReputationLevel(factionId);
        
        // Apply change
        _playerReputation[factionId] += change;
        
        // Clamp between -3000 and 3000
        _playerReputation[factionId] = Math.Max(-3000, Math.Min(3000, _playerReputation[factionId]));
        
        int newReputation = _playerReputation[factionId];
        ReputationLevel newLevel = GetReputationLevel(factionId);
        
        // Trigger events
        if (newReputation != oldReputation)
        {
            OnReputationChanged?.Invoke(faction, newReputation);
        }
        
        if (newLevel != oldLevel)
        {
            OnReputationLevelChanged?.Invoke(faction, newLevel);
            
            // Check for newly unlocked rewards
            var reward = faction.GetRewardForLevel(newLevel);
            if (reward != null && newLevel > oldLevel)
            {
                OnRewardUnlocked?.Invoke(faction, reward);
            }
        }
    }
    
    /// <summary>
    /// Get current reputation value with a faction
    /// </summary>
    public int GetReputation(string factionId)
    {
        return _playerReputation.ContainsKey(factionId) ? _playerReputation[factionId] : 0;
    }
    
    /// <summary>
    /// Get reputation level with a faction
    /// </summary>
    public ReputationLevel GetReputationLevel(string factionId)
    {
        int reputation = GetReputation(factionId);
        
        if (reputation >= 2000) return ReputationLevel.Exalted;
        if (reputation >= 1500) return ReputationLevel.Revered;
        if (reputation >= 1000) return ReputationLevel.Honored;
        if (reputation >= 500) return ReputationLevel.Friendly;
        if (reputation >= 0) return ReputationLevel.Neutral;
        if (reputation >= -500) return ReputationLevel.Unfriendly;
        if (reputation >= -1000) return ReputationLevel.Hostile;
        return ReputationLevel.Hated;
    }
    
    /// <summary>
    /// Get faction by ID
    /// </summary>
    public Faction GetFaction(string factionId)
    {
        return _factions.Find(f => f.Id == factionId);
    }
    
    /// <summary>
    /// Get all factions
    /// </summary>
    public List<Faction> GetAllFactions()
    {
        return new List<Faction>(_factions);
    }
    
    /// <summary>
    /// Check if player has unlocked a specific reward
    /// </summary>
    public bool HasUnlockedReward(string factionId, ReputationLevel requiredLevel)
    {
        return GetReputationLevel(factionId) >= requiredLevel;
    }
    
    /// <summary>
    /// Get all unlocked rewards for a faction
    /// </summary>
    public List<FactionReward> GetUnlockedRewards(string factionId)
    {
        var faction = GetFaction(factionId);
        if (faction == null) return new List<FactionReward>();
        
        var currentLevel = GetReputationLevel(factionId);
        return faction.Rewards.FindAll(r => r.RequiredLevel <= currentLevel);
    }
}

/// <summary>
/// Individual faction with reputation rewards
/// </summary>
public class Faction
{
    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public FactionType Type { get; set; }
    public List<FactionReward> Rewards { get; set; }
    public List<string> OpposingFactionIds { get; set; }
    
    public Faction(string id, string name, string description, FactionType type)
    {
        Id = id;
        Name = name;
        Description = description;
        Type = type;
        Rewards = new List<FactionReward>();
        OpposingFactionIds = new List<string>();
    }
    
    public void AddReward(FactionReward reward)
    {
        Rewards.Add(reward);
    }
    
    public FactionReward GetRewardForLevel(ReputationLevel level)
    {
        return Rewards.Find(r => r.RequiredLevel == level);
    }
}

/// <summary>
/// Reward for reaching a reputation level
/// </summary>
public class FactionReward
{
    public ReputationLevel RequiredLevel { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    
    public FactionReward(ReputationLevel requiredLevel, string name, string description)
    {
        RequiredLevel = requiredLevel;
        Name = name;
        Description = description;
    }
}

/// <summary>
/// Faction types
/// </summary>
public enum FactionType
{
    Economic,       // Trade and commerce
    Combat,         // Warriors and fighters
    Mystical,       // Magic users
    Environmental,  // Nature protectors
    Underground,    // Rogues and criminals
    Religious       // Priests and clerics
}

/// <summary>
/// Reputation levels
/// </summary>
public enum ReputationLevel
{
    Hated = -4,       // < -1000
    Hostile = -3,     // -1000 to -500
    Unfriendly = -2,  // -500 to 0
    Neutral = 0,      // 0 to 500
    Friendly = 1,     // 500 to 1000
    Honored = 2,      // 1000 to 1500
    Revered = 3,      // 1500 to 2000
    Exalted = 4       // >= 2000
}
