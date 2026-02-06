using MoonBrookRidge.Engine.MonoGameCompat;
using System;
using System.Collections.Generic;

namespace MoonBrookRidge.Characters;

/// <summary>
/// Manages children and family dynamics
/// </summary>
public class FamilySystem
{
    private List<Child> _children;
    private MarriageSystem _marriageSystem;
    private int _daysSinceLastChild;
    
    // Family settings
    private const int MAX_CHILDREN = 2;
    private const int DAYS_BETWEEN_CHILDREN = 56; // 2 seasons (2 x 28 days)
    private const int DAYS_REQUIRED_FOR_CHILD = 28; // 1 season after marriage
    
    // Events
    public event Action<Child> OnChildBorn;
    public event Action<Child, ChildStage> OnChildGrowth;
    
    public FamilySystem(MarriageSystem marriageSystem)
    {
        _children = new List<Child>();
        _marriageSystem = marriageSystem;
        _daysSinceLastChild = 0;
    }
    
    /// <summary>
    /// Update family system each game day
    /// </summary>
    public void Update(int currentDay, int currentSeason, int currentYear)
    {
        // Can only have children if married
        if (!_marriageSystem.IsMarried) return;
        
        _daysSinceLastChild++;
        
        // Check if player can have another child
        if (CanHaveChild())
        {
            // Random chance for pregnancy/adoption (5% per day after requirements met)
            // Use current date for seed to get consistent daily random check
            Random rand = new Random(currentDay + currentSeason * 28 + currentYear * 112 + GetHashCode());
            if (rand.NextDouble() < 0.05)
            {
                AddChild(currentDay, currentSeason, currentYear);
            }
        }
        
        // Update all children (growth stages)
        foreach (var child in _children)
        {
            child.Update();
            
            // Check for growth stage change
            if (child.CheckGrowthStageChange())
            {
                OnChildGrowth?.Invoke(child, child.Stage);
            }
        }
    }
    
    /// <summary>
    /// Check if player can have another child
    /// </summary>
    private bool CanHaveChild()
    {
        if (!_marriageSystem.IsMarried) return false;
        if (_children.Count >= MAX_CHILDREN) return false;
        
        // Must be married for at least 1 season
        if (_marriageSystem.DaysSinceMarriage < DAYS_REQUIRED_FOR_CHILD) return false;
        
        // Must wait between children
        if (_daysSinceLastChild < DAYS_BETWEEN_CHILDREN) return false;
        
        return true;
    }
    
    /// <summary>
    /// Add a new child to the family
    /// </summary>
    private void AddChild(int birthDay, int birthSeason, int birthYear)
    {
        // Determine child name and gender
        // Use stable seed based on birth date and current child count
        Random rand = new Random((_children.Count * 1000) + birthDay + (birthSeason * 28) + (birthYear * 112));
        bool isBoy = rand.Next(0, 2) == 0;
        
        string childName = GenerateChildName(isBoy, _children.Count);
        
        Child newChild = new Child(childName, isBoy, birthDay, birthSeason, birthYear);
        _children.Add(newChild);
        _daysSinceLastChild = 0;
        
        OnChildBorn?.Invoke(newChild);
    }
    
    /// <summary>
    /// Generate a name for a child
    /// </summary>
    private string GenerateChildName(bool isBoy, int childIndex)
    {
        string[] boyNames = new string[] 
        { 
            "Thomas", "William", "James", "Benjamin", "Lucas", 
            "Oliver", "Noah", "Ethan", "Mason", "Alexander" 
        };
        
        string[] girlNames = new string[] 
        { 
            "Emily", "Sophia", "Isabella", "Olivia", "Ava", 
            "Charlotte", "Amelia", "Harper", "Evelyn", "Abigail" 
        };
        
        if (isBoy)
        {
            return boyNames[childIndex % boyNames.Length];
        }
        else
        {
            return girlNames[childIndex % girlNames.Length];
        }
    }
    
    /// <summary>
    /// Get child by index
    /// </summary>
    public Child GetChild(int index)
    {
        if (index < 0 || index >= _children.Count) return null;
        return _children[index];
    }
    
    /// <summary>
    /// Interact with a child (play, gift, etc.)
    /// </summary>
    public void InteractWithChild(Child child, ChildInteraction interaction)
    {
        if (child == null || !_children.Contains(child)) return;
        
        switch (interaction)
        {
            case ChildInteraction.Play:
                child.ModifyHappiness(15);
                break;
            case ChildInteraction.Gift:
                child.ModifyHappiness(25);
                break;
            case ChildInteraction.Teach:
                child.ModifyEducation(10);
                break;
            case ChildInteraction.Hug:
                child.ModifyHappiness(10);
                break;
        }
    }
    
    // Properties
    public int ChildCount => _children.Count;
    public List<Child> Children => new List<Child>(_children); // Return copy
    public bool HasChildren => _children.Count > 0;
}

/// <summary>
/// Represents a child character
/// </summary>
public class Child
{
    private string _name;
    private bool _isBoy;
    private int _birthDay;
    private int _birthSeason;
    private int _birthYear;
    private int _daysOld;
    private ChildStage _stage;
    private int _happiness; // 0-100
    private int _education; // 0-100
    
    // Growth stage durations (in days)
    private const int BABY_DAYS = 28;      // 1 season
    private const int TODDLER_DAYS = 56;   // 2 seasons
    // After toddler, stays as child
    
    public Child(string name, bool isBoy, int birthDay, int birthSeason, int birthYear)
    {
        _name = name;
        _isBoy = isBoy;
        _birthDay = birthDay;
        _birthSeason = birthSeason;
        _birthYear = birthYear;
        _daysOld = 0;
        _stage = ChildStage.Baby;
        _happiness = 50;
        _education = 0;
    }
    
    /// <summary>
    /// Update child each day
    /// </summary>
    public void Update()
    {
        _daysOld++;
        
        // Happiness slowly decreases over time (needs attention)
        if (_happiness > 0)
        {
            _happiness = Math.Max(0, _happiness - 1);
        }
    }
    
    /// <summary>
    /// Check if child should advance to next growth stage
    /// </summary>
    public bool CheckGrowthStageChange()
    {
        ChildStage oldStage = _stage;
        
        if (_stage == ChildStage.Baby && _daysOld >= BABY_DAYS)
        {
            _stage = ChildStage.Toddler;
        }
        else if (_stage == ChildStage.Toddler && _daysOld >= BABY_DAYS + TODDLER_DAYS)
        {
            _stage = ChildStage.Child;
        }
        
        return _stage != oldStage;
    }
    
    /// <summary>
    /// Modify child's happiness
    /// </summary>
    public void ModifyHappiness(int amount)
    {
        _happiness = MathHelper.Clamp(_happiness + amount, 0, 100);
    }
    
    /// <summary>
    /// Modify child's education
    /// </summary>
    public void ModifyEducation(int amount)
    {
        if (_stage == ChildStage.Baby) return; // Babies can't learn yet
        
        _education = MathHelper.Clamp(_education + amount, 0, 100);
    }
    
    /// <summary>
    /// Get dialogue based on stage and happiness
    /// </summary>
    public string GetDialogue()
    {
        switch (_stage)
        {
            case ChildStage.Baby:
                return _happiness > 50 ? "*giggles*" : "*cries*";
                
            case ChildStage.Toddler:
                if (_happiness > 70)
                    return "Play! Play!";
                else if (_happiness > 30)
                    return "Mama/Dada!";
                else
                    return "*whimpers*";
                    
            case ChildStage.Child:
                if (_happiness > 70)
                    return "I love living on the farm!";
                else if (_happiness > 30)
                    return "Hi parent!";
                else
                    return "I'm bored...";
                    
            default:
                return "";
        }
    }
    
    /// <summary>
    /// Check if child can help with farm chores
    /// </summary>
    public bool CanHelp()
    {
        return _stage == ChildStage.Child && _happiness > 50 && _education > 30;
    }
    
    // Properties
    public string Name => _name;
    public bool IsBoy => _isBoy;
    public int DaysOld => _daysOld;
    public ChildStage Stage => _stage;
    public int Happiness => _happiness;
    public int Education => _education;
    public int BirthDay => _birthDay;
    public int BirthSeason => _birthSeason;
    public int BirthYear => _birthYear;
}

/// <summary>
/// Growth stages for children
/// </summary>
public enum ChildStage
{
    Baby,       // 0-28 days
    Toddler,    // 28-84 days
    Child       // 84+ days
}

/// <summary>
/// Types of interactions with children
/// </summary>
public enum ChildInteraction
{
    Play,
    Gift,
    Teach,
    Hug
}
