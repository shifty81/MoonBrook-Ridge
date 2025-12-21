using Microsoft.Xna.Framework;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Manages in-game time, seasons, and calendar
/// </summary>
public class TimeSystem
{
    public enum Season { Spring, Summer, Fall, Winter }
    
    private float _timeOfDay; // 0-24 hours
    private int _day;
    private Season _season;
    private int _year;
    
    private const float MINUTES_PER_GAME_HOUR = 2.5f; // Real seconds per game hour
    private const int DAYS_PER_SEASON = 28;
    
    public TimeSystem()
    {
        _timeOfDay = 6f; // Start at 6 AM
        _day = 1;
        _season = Season.Spring;
        _year = 1;
    }
    
    public void Update(GameTime gameTime)
    {
        float realSecondsElapsed = (float)gameTime.ElapsedGameTime.TotalSeconds;
        float gameHoursElapsed = realSecondsElapsed / (MINUTES_PER_GAME_HOUR * 60f);
        
        _timeOfDay += gameHoursElapsed;
        
        // Advance day at 2 AM (time to sleep)
        if (_timeOfDay >= 26f) // 2 AM next day
        {
            AdvanceDay();
        }
    }
    
    private void AdvanceDay()
    {
        _timeOfDay = 6f; // Reset to 6 AM
        _day++;
        
        if (_day > DAYS_PER_SEASON)
        {
            _day = 1;
            AdvanceSeason();
        }
    }
    
    private void AdvanceSeason()
    {
        _season = _season switch
        {
            Season.Spring => Season.Summer,
            Season.Summer => Season.Fall,
            Season.Fall => Season.Winter,
            Season.Winter => Season.Spring,
            _ => Season.Spring
        };
        
        if (_season == Season.Spring)
        {
            _year++;
        }
    }
    
    public float TimeOfDay => _timeOfDay;
    public int Day => _day;
    public Season CurrentSeason => _season;
    public int Year => _year;
    
    public string GetFormattedTime()
    {
        int hour = (int)_timeOfDay;
        int minute = (int)((_timeOfDay - hour) * 60);
        string period = hour < 12 ? "AM" : "PM";
        int displayHour = hour > 12 ? hour - 12 : (hour == 0 ? 12 : hour);
        return $"{displayHour}:{minute:D2} {period}";
    }
}
