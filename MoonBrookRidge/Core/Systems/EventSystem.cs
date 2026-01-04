using System;
using System.Collections.Generic;
using System.Linq;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Manages game events and festivals tied to the calendar
/// </summary>
public class EventSystem
{
    private List<GameEvent> _upcomingEvents;
    private GameEvent? _activeEvent;
    private TimeSystem _timeSystem;
    
    public EventSystem(TimeSystem timeSystem)
    {
        _timeSystem = timeSystem;
        _upcomingEvents = new List<GameEvent>();
        InitializeSeasonalEvents();
    }
    
    /// <summary>
    /// Initializes all seasonal festivals and events
    /// </summary>
    private void InitializeSeasonalEvents()
    {
        // Spring Events
        _upcomingEvents.Add(new Festival(
            "Egg Festival",
            "Celebrate spring with an egg hunt!",
            TimeSystem.Season.Spring,
            13,
            FestivalType.EggHunt
        ));
        
        _upcomingEvents.Add(new Festival(
            "Flower Dance",
            "Dance with a partner among the flowers.",
            TimeSystem.Season.Spring,
            24,
            FestivalType.Dance
        ));
        
        // Summer Events
        _upcomingEvents.Add(new Festival(
            "Luau",
            "The town comes together for a beach feast!",
            TimeSystem.Season.Summer,
            11,
            FestivalType.Feast
        ));
        
        _upcomingEvents.Add(new Festival(
            "Moonlight Jellies",
            "Watch the magical jellyfish migration.",
            TimeSystem.Season.Summer,
            28,
            FestivalType.Viewing
        ));
        
        // Fall Events
        _upcomingEvents.Add(new Festival(
            "Harvest Festival",
            "Celebrate the autumn harvest with the community.",
            TimeSystem.Season.Fall,
            15,
            FestivalType.Harvest
        ));
        
        _upcomingEvents.Add(new Festival(
            "Spirit's Eve",
            "A spooky celebration with costumes and treats.",
            TimeSystem.Season.Fall,
            27,
            FestivalType.Holiday
        ));
        
        // Winter Events
        _upcomingEvents.Add(new Festival(
            "Festival of Ice",
            "Ice fishing and winter activities!",
            TimeSystem.Season.Winter,
            8,
            FestivalType.Fishing
        ));
        
        _upcomingEvents.Add(new Festival(
            "Feast of the Winter Star",
            "Exchange gifts and celebrate the season.",
            TimeSystem.Season.Winter,
            25,
            FestivalType.Holiday
        ));
    }
    
    /// <summary>
    /// Updates the event system, checking for triggered events
    /// </summary>
    public void Update(GameTime gameTime)
    {
        // Check if any event should be triggered today
        if (_activeEvent == null)
        {
            CheckForEventTriggers();
        }
        
        // Update active event if there is one
        _activeEvent?.Update(gameTime);
        
        // End event if time has passed
        if (_activeEvent != null && _activeEvent.HasEnded)
        {
            _activeEvent = null;
        }
    }
    
    /// <summary>
    /// Checks if any events should trigger based on current date
    /// </summary>
    private void CheckForEventTriggers()
    {
        var currentSeason = _timeSystem.CurrentSeason;
        var currentDay = _timeSystem.Day;
        
        var matchingEvent = _upcomingEvents.FirstOrDefault(e => 
            e.Season == currentSeason && e.DayOfSeason == currentDay && !e.HasBeenTriggered(_timeSystem.Year));
        
        if (matchingEvent != null)
        {
            TriggerEvent(matchingEvent);
        }
    }
    
    /// <summary>
    /// Triggers a specific event
    /// </summary>
    private void TriggerEvent(GameEvent gameEvent)
    {
        _activeEvent = gameEvent;
        gameEvent.Trigger(_timeSystem.Year);
    }
    
    /// <summary>
    /// Gets the next upcoming event
    /// </summary>
    public GameEvent? GetNextEvent()
    {
        var currentSeason = _timeSystem.CurrentSeason;
        var currentDay = _timeSystem.Day;
        
        // Find the next event in the current season
        var nextInSeason = _upcomingEvents
            .Where(e => e.Season == currentSeason && e.DayOfSeason > currentDay)
            .OrderBy(e => e.DayOfSeason)
            .FirstOrDefault();
        
        if (nextInSeason != null)
            return nextInSeason;
        
        // If no events left this season, get first event of next season
        var nextSeason = GetNextSeason(currentSeason);
        return _upcomingEvents
            .Where(e => e.Season == nextSeason)
            .OrderBy(e => e.DayOfSeason)
            .FirstOrDefault();
    }
    
    private TimeSystem.Season GetNextSeason(TimeSystem.Season current)
    {
        return current switch
        {
            TimeSystem.Season.Spring => TimeSystem.Season.Summer,
            TimeSystem.Season.Summer => TimeSystem.Season.Fall,
            TimeSystem.Season.Fall => TimeSystem.Season.Winter,
            TimeSystem.Season.Winter => TimeSystem.Season.Spring,
            _ => TimeSystem.Season.Spring
        };
    }
    
    public GameEvent? ActiveEvent => _activeEvent;
    public bool HasActiveEvent => _activeEvent != null;
}

/// <summary>
/// Base class for game events
/// </summary>
public abstract class GameEvent
{
    public string Name { get; protected set; }
    public string Description { get; protected set; }
    public TimeSystem.Season Season { get; protected set; }
    public int DayOfSeason { get; protected set; }
    
    protected bool _isActive;
    protected HashSet<int> _triggeredYears;
    
    public GameEvent(string name, string description, TimeSystem.Season season, int day)
    {
        Name = name;
        Description = description;
        Season = season;
        DayOfSeason = day;
        _isActive = false;
        _triggeredYears = new HashSet<int>();
    }
    
    public virtual void Trigger(int year)
    {
        _isActive = true;
        _triggeredYears.Add(year);
    }
    
    public virtual void Update(GameTime gameTime)
    {
        // Override in derived classes for event-specific logic
    }
    
    public bool HasBeenTriggered(int year) => _triggeredYears.Contains(year);
    public bool IsActive => _isActive;
    public virtual bool HasEnded => false; // Override in derived classes
}

/// <summary>
/// Types of festivals
/// </summary>
public enum FestivalType
{
    EggHunt,
    Dance,
    Feast,
    Viewing,
    Harvest,
    Holiday,
    Fishing
}

/// <summary>
/// Festival event type
/// </summary>
public class Festival : GameEvent
{
    public FestivalType Type { get; private set; }
    private float _duration;
    private float _elapsed;
    
    public Festival(string name, string description, TimeSystem.Season season, int day, FestivalType type)
        : base(name, description, season, day)
    {
        Type = type;
        _duration = 4f; // Festival lasts 4 in-game hours
        _elapsed = 0f;
    }
    
    public override void Trigger(int year)
    {
        base.Trigger(year);
        _elapsed = 0f;
    }
    
    public override void Update(GameTime gameTime)
    {
        if (_isActive)
        {
            _elapsed += (float)gameTime.ElapsedGameTime.TotalSeconds;
        }
    }
    
    public override bool HasEnded => _elapsed >= _duration;
    public float Progress => Math.Min(_elapsed / _duration, 1f);
}
