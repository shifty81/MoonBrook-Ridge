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
    private List<QuestConsequence> _consequences;
    
    // Player karma and moral alignment
    public int PlayerKarma { get; private set; }
    public MoralAlignment PlayerAlignment { get; private set; }
    
    public event Action<Quest> OnQuestCompleted;
    public event Action<QuestChoice, QuestConsequence> OnChoiceMade;
    public event Action<int> OnKarmaChanged;
    
    public QuestSystem()
    {
        _activeQuests = new List<Quest>();
        _completedQuests = new List<Quest>();
        _availableQuests = new List<Quest>();
        _consequences = new List<QuestConsequence>();
        PlayerKarma = 0;
        PlayerAlignment = MoralAlignment.Neutral;
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
    
    /// <summary>
    /// Make a choice in a quest, triggering branching and consequences
    /// </summary>
    public void MakeQuestChoice(string questId, string choiceId)
    {
        var quest = _activeQuests.Find(q => q.Id == questId);
        if (quest == null) return;
        
        var choice = quest.Choices.Find(c => c.Id == choiceId);
        if (choice == null) return;
        
        // Apply choice to quest
        quest.MakeChoice(choiceId);
        
        // Create consequence
        var consequence = new QuestConsequence(questId, choiceId, 
            choice.MoralAlignment, choice.KarmaChange, $"You chose: {choice.Text}");
        
        // Apply faction reputation changes
        foreach (var factionChange in choice.FactionReputationChanges)
        {
            consequence.FactionChanges[factionChange.Key] = factionChange.Value;
        }
        
        _consequences.Add(consequence);
        
        // Apply karma change
        ChangeKarma(choice.KarmaChange);
        
        // Trigger event
        OnChoiceMade?.Invoke(choice, consequence);
    }
    
    /// <summary>
    /// Change player karma and recalculate alignment
    /// </summary>
    private void ChangeKarma(int change)
    {
        PlayerKarma += change;
        
        // Recalculate alignment based on karma
        if (PlayerKarma >= 50)
            PlayerAlignment = MoralAlignment.Good;
        else if (PlayerKarma <= -50)
            PlayerAlignment = MoralAlignment.Evil;
        else
            PlayerAlignment = MoralAlignment.Neutral;
        
        OnKarmaChanged?.Invoke(PlayerKarma);
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
        
        // Apply karma change from quest if present
        if (quest.KarmaChange != 0)
        {
            ChangeKarma(quest.KarmaChange);
        }
        
        // Trigger completion event
        OnQuestCompleted?.Invoke(quest);
    }
    
    public List<Quest> GetActiveQuests() => _activeQuests;
    public List<Quest> GetAvailableQuests() => _availableQuests;
    public List<Quest> GetCompletedQuests() => _completedQuests;
    public List<QuestConsequence> GetConsequences() => _consequences;
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
    
    // Advanced Quest System - Moral Choices and Branching
    public List<QuestChoice> Choices { get; set; }
    public string CurrentBranch { get; set; }
    public Dictionary<string, List<QuestObjective>> BranchObjectives { get; set; }
    public Dictionary<string, QuestReward> BranchRewards { get; set; }
    public MoralAlignment? MoralImpact { get; set; }
    public int KarmaChange { get; set; }
    
    public Quest(string id, string title, string description, string giverName)
    {
        Id = id;
        Title = title;
        Description = description;
        GiverName = giverName;
        Status = QuestStatus.Available;
        Objectives = new List<QuestObjective>();
        Choices = new List<QuestChoice>();
        BranchObjectives = new Dictionary<string, List<QuestObjective>>();
        BranchRewards = new Dictionary<string, QuestReward>();
        CurrentBranch = "default";
    }
    
    public void AddObjective(QuestObjective objective)
    {
        Objectives.Add(objective);
    }
    
    public void AddChoice(QuestChoice choice)
    {
        Choices.Add(choice);
    }
    
    public void MakeChoice(string choiceId)
    {
        var choice = Choices.Find(c => c.Id == choiceId);
        if (choice != null)
        {
            // Set current branch
            CurrentBranch = choice.BranchId;
            
            // Apply moral impact
            MoralImpact = choice.MoralAlignment;
            KarmaChange = choice.KarmaChange;
            
            // Replace objectives with branch-specific ones
            if (BranchObjectives.ContainsKey(CurrentBranch))
            {
                Objectives = new List<QuestObjective>(BranchObjectives[CurrentBranch]);
            }
            
            // Update reward if branch-specific reward exists
            if (BranchRewards.ContainsKey(CurrentBranch))
            {
                Reward = BranchRewards[CurrentBranch];
            }
        }
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

/// <summary>
/// Quest choice for branching storylines
/// </summary>
public class QuestChoice
{
    public string Id { get; set; }
    public string Text { get; set; }
    public string Description { get; set; }
    public string BranchId { get; set; }
    public MoralAlignment MoralAlignment { get; set; }
    public int KarmaChange { get; set; }
    public Dictionary<string, int> FactionReputationChanges { get; set; }
    
    public QuestChoice(string id, string text, string description, string branchId, 
                       MoralAlignment moralAlignment, int karmaChange)
    {
        Id = id;
        Text = text;
        Description = description;
        BranchId = branchId;
        MoralAlignment = moralAlignment;
        KarmaChange = karmaChange;
        FactionReputationChanges = new Dictionary<string, int>();
    }
}

/// <summary>
/// Moral alignment for quest choices
/// </summary>
public enum MoralAlignment
{
    Good,      // Selfless, helpful, law-abiding
    Neutral,   // Balanced, pragmatic
    Evil       // Selfish, harmful, law-breaking
}

/// <summary>
/// Quest consequence tracking
/// </summary>
public class QuestConsequence
{
    public string QuestId { get; set; }
    public string ChoiceId { get; set; }
    public MoralAlignment Alignment { get; set; }
    public int KarmaChange { get; set; }
    public Dictionary<string, int> FactionChanges { get; set; }
    public string ResultDescription { get; set; }
    
    public QuestConsequence(string questId, string choiceId, MoralAlignment alignment, 
                           int karmaChange, string resultDescription)
    {
        QuestId = questId;
        ChoiceId = choiceId;
        Alignment = alignment;
        KarmaChange = karmaChange;
        ResultDescription = resultDescription;
        FactionChanges = new Dictionary<string, int>();
    }
}

