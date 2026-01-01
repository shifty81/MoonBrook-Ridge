using System.Collections.Generic;
using System;

namespace MoonBrookRidge.Quests;

/// <summary>
/// Quest system for tracking player objectives and tasks
/// </summary>
public class QuestSystem
{
    private List<Quest> _activeQuests;
    private List<Quest> _completedQuests;
    private List<Quest> _availableQuests;
    
    public QuestSystem()
    {
        _activeQuests = new List<Quest>();
        _completedQuests = new List<Quest>();
        _availableQuests = new List<Quest>();
    }
    
    public void AddAvailableQuest(Quest quest)
    {
        if (!_availableQuests.Contains(quest) && 
            !_activeQuests.Contains(quest) && 
            !_completedQuests.Contains(quest))
        {
            _availableQuests.Add(quest);
        }
    }
    
    public bool AcceptQuest(Quest quest)
    {
        if (_availableQuests.Contains(quest))
        {
            _availableQuests.Remove(quest);
            _activeQuests.Add(quest);
            quest.Status = QuestStatus.Active;
            return true;
        }
        return false;
    }
    
    public void UpdateQuestProgress(string questId, string objectiveId, int progress = 1)
    {
        var quest = _activeQuests.Find(q => q.Id == questId);
        if (quest != null)
        {
            quest.UpdateObjective(objectiveId, progress);
            
            // Check if quest is complete
            if (quest.IsComplete())
            {
                CompleteQuest(quest);
            }
        }
    }
    
    private void CompleteQuest(Quest quest)
    {
        _activeQuests.Remove(quest);
        _completedQuests.Add(quest);
        quest.Status = QuestStatus.Completed;
        
        // Trigger completion event
        OnQuestCompleted?.Invoke(quest);
    }
    
    public List<Quest> GetActiveQuests() => _activeQuests;
    public List<Quest> GetAvailableQuests() => _availableQuests;
    public List<Quest> GetCompletedQuests() => _completedQuests;
    
    public event Action<Quest> OnQuestCompleted;
}

/// <summary>
/// Individual quest with objectives and rewards
/// </summary>
public class Quest
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public string GiverName { get; set; }
    public QuestStatus Status { get; set; }
    public List<QuestObjective> Objectives { get; set; }
    public QuestReward Reward { get; set; }
    
    public Quest(string id, string title, string description, string giverName)
    {
        Id = id;
        Title = title;
        Description = description;
        GiverName = giverName;
        Status = QuestStatus.Available;
        Objectives = new List<QuestObjective>();
    }
    
    public void AddObjective(QuestObjective objective)
    {
        Objectives.Add(objective);
    }
    
    public void UpdateObjective(string objectiveId, int progress)
    {
        var objective = Objectives.Find(o => o.Id == objectiveId);
        if (objective != null)
        {
            objective.CurrentProgress += progress;
            if (objective.CurrentProgress >= objective.RequiredProgress)
            {
                objective.IsCompleted = true;
            }
        }
    }
    
    public bool IsComplete()
    {
        foreach (var objective in Objectives)
        {
            if (!objective.IsCompleted)
                return false;
        }
        return true;
    }
    
    public float GetCompletionPercentage()
    {
        if (Objectives.Count == 0) return 0f;
        
        int completedCount = 0;
        foreach (var objective in Objectives)
        {
            if (objective.IsCompleted)
                completedCount++;
        }
        
        return (float)completedCount / Objectives.Count * 100f;
    }
}

/// <summary>
/// Single objective within a quest
/// </summary>
public class QuestObjective
{
    public string Id { get; set; }
    public string Description { get; set; }
    public QuestObjectiveType Type { get; set; }
    public string TargetId { get; set; } // Item name, NPC name, location, etc.
    public int RequiredProgress { get; set; }
    public int CurrentProgress { get; set; }
    public bool IsCompleted { get; set; }
    
    public QuestObjective(string id, string description, QuestObjectiveType type, 
                         string targetId, int requiredProgress)
    {
        Id = id;
        Description = description;
        Type = type;
        TargetId = targetId;
        RequiredProgress = requiredProgress;
        CurrentProgress = 0;
        IsCompleted = false;
    }
    
    public string GetProgressText()
    {
        return $"{CurrentProgress}/{RequiredProgress}";
    }
}

/// <summary>
/// Quest rewards
/// </summary>
public class QuestReward
{
    public int Money { get; set; }
    public int FriendshipPoints { get; set; }
    public string FriendshipNPC { get; set; }
    public Dictionary<string, int> Items { get; set; } // Item name -> quantity
    
    public QuestReward()
    {
        Items = new Dictionary<string, int>();
    }
}

/// <summary>
/// Types of quest objectives
/// </summary>
public enum QuestObjectiveType
{
    CollectItem,      // Collect X of item Y
    TalkToNPC,        // Talk to specific NPC
    GiveItem,         // Give item to NPC
    Harvest,          // Harvest X crops
    Mine,             // Mine X resources
    Fish,             // Catch X fish
    Craft,            // Craft X items
    Visit,            // Visit a location
    Build,            // Build a structure
    Kill              // Defeat X enemies (future)
}

/// <summary>
/// Quest status
/// </summary>
public enum QuestStatus
{
    Available,
    Active,
    Completed,
    Failed
}
