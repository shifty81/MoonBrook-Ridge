using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace MoonBrookEngine.Core;

/// <summary>
/// Logging system for structured, filterable logging
/// </summary>
public class Logger
{
    public enum Level
    {
        Debug = 0,
        Info = 1,
        Warning = 2,
        Error = 3,
        None = 99
    }
    
    private readonly string _name;
    private Level _minLevel;
    private readonly List<ILogHandler> _handlers;
    private readonly Dictionary<string, PerformanceTimer> _timers;
    
    public string Name => _name;
    public Level MinLevel
    {
        get => _minLevel;
        set => _minLevel = value;
    }
    
    public Logger(string name, Level minLevel = Level.Info)
    {
        _name = name;
        _minLevel = minLevel;
        _handlers = new List<ILogHandler>();
        _timers = new Dictionary<string, PerformanceTimer>();
        
        // Add default console handler
        AddHandler(new ConsoleLogHandler());
    }
    
    /// <summary>
    /// Add a log handler
    /// </summary>
    public void AddHandler(ILogHandler handler)
    {
        _handlers.Add(handler);
    }
    
    /// <summary>
    /// Remove a log handler
    /// </summary>
    public void RemoveHandler(ILogHandler handler)
    {
        _handlers.Remove(handler);
    }
    
    /// <summary>
    /// Log a debug message
    /// </summary>
    public void Debug(string message)
    {
        Log(Level.Debug, message);
    }
    
    /// <summary>
    /// Log an info message
    /// </summary>
    public void Info(string message)
    {
        Log(Level.Info, message);
    }
    
    /// <summary>
    /// Log a warning message
    /// </summary>
    public void Warning(string message)
    {
        Log(Level.Warning, message);
    }
    
    /// <summary>
    /// Log an error message
    /// </summary>
    public void Error(string message)
    {
        Log(Level.Error, message);
    }
    
    /// <summary>
    /// Log an error with exception
    /// </summary>
    public void Error(string message, Exception ex)
    {
        Log(Level.Error, $"{message}\n{ex}");
    }
    
    /// <summary>
    /// Log a message at the specified level
    /// </summary>
    public void Log(Level level, string message)
    {
        if (level < _minLevel)
            return;
        
        var entry = new LogEntry
        {
            Timestamp = DateTime.Now,
            Level = level,
            LoggerName = _name,
            Message = message
        };
        
        foreach (var handler in _handlers)
        {
            handler.Write(entry);
        }
    }
    
    /// <summary>
    /// Log a formatted message
    /// </summary>
    public void LogFormat(Level level, string format, params object[] args)
    {
        Log(level, string.Format(format, args));
    }
    
    /// <summary>
    /// Start a performance timer
    /// </summary>
    public void StartTimer(string name)
    {
        if (!_timers.ContainsKey(name))
        {
            _timers[name] = new PerformanceTimer(name);
        }
        _timers[name].Start();
    }
    
    /// <summary>
    /// Stop a performance timer and log the result
    /// </summary>
    public void StopTimer(string name)
    {
        if (_timers.TryGetValue(name, out var timer))
        {
            timer.Stop();
            Debug($"[PERF] {name}: {timer.ElapsedMilliseconds:F2}ms");
        }
    }
    
    /// <summary>
    /// Measure execution time of an action
    /// </summary>
    public void Measure(string name, Action action)
    {
        StartTimer(name);
        try
        {
            action();
        }
        finally
        {
            StopTimer(name);
        }
    }
    
    /// <summary>
    /// Measure execution time of a function
    /// </summary>
    public T Measure<T>(string name, Func<T> func)
    {
        StartTimer(name);
        try
        {
            return func();
        }
        finally
        {
            StopTimer(name);
        }
    }
}

/// <summary>
/// Log entry structure
/// </summary>
public struct LogEntry
{
    public DateTime Timestamp;
    public Logger.Level Level;
    public string LoggerName;
    public string Message;
    
    public override string ToString()
    {
        string levelStr = Level switch
        {
            Logger.Level.Debug => "DEBUG",
            Logger.Level.Info => "INFO",
            Logger.Level.Warning => "WARN",
            Logger.Level.Error => "ERROR",
            _ => "????"
        };
        
        return $"[{Timestamp:HH:mm:ss.fff}] [{levelStr}] [{LoggerName}] {Message}";
    }
}

/// <summary>
/// Interface for log handlers
/// </summary>
public interface ILogHandler
{
    void Write(LogEntry entry);
}

/// <summary>
/// Console log handler
/// </summary>
public class ConsoleLogHandler : ILogHandler
{
    private readonly bool _useColors;
    
    public ConsoleLogHandler(bool useColors = true)
    {
        _useColors = useColors;
    }
    
    public void Write(LogEntry entry)
    {
        if (_useColors)
        {
            var originalColor = Console.ForegroundColor;
            Console.ForegroundColor = entry.Level switch
            {
                Logger.Level.Debug => ConsoleColor.Gray,
                Logger.Level.Info => ConsoleColor.White,
                Logger.Level.Warning => ConsoleColor.Yellow,
                Logger.Level.Error => ConsoleColor.Red,
                _ => ConsoleColor.White
            };
            Console.WriteLine(entry.ToString());
            Console.ForegroundColor = originalColor;
        }
        else
        {
            Console.WriteLine(entry.ToString());
        }
    }
}

/// <summary>
/// File log handler
/// </summary>
public class FileLogHandler : ILogHandler
{
    private readonly string _filePath;
    private readonly object _lock = new();
    
    public FileLogHandler(string filePath)
    {
        _filePath = filePath;
    }
    
    public void Write(LogEntry entry)
    {
        lock (_lock)
        {
            try
            {
                System.IO.File.AppendAllText(_filePath, entry.ToString() + Environment.NewLine);
            }
            catch
            {
                // Silently fail to avoid recursive logging
            }
        }
    }
}

/// <summary>
/// Performance timer helper
/// </summary>
internal class PerformanceTimer
{
    private readonly Stopwatch _stopwatch;
    private readonly string _name;
    
    public string Name => _name;
    public double ElapsedMilliseconds => _stopwatch.Elapsed.TotalMilliseconds;
    public bool IsRunning => _stopwatch.IsRunning;
    
    public PerformanceTimer(string name)
    {
        _name = name;
        _stopwatch = new Stopwatch();
    }
    
    public void Start()
    {
        _stopwatch.Restart();
    }
    
    public void Stop()
    {
        _stopwatch.Stop();
    }
    
    public void Reset()
    {
        _stopwatch.Reset();
    }
}

/// <summary>
/// Global logger factory
/// </summary>
public static class LoggerFactory
{
    private static readonly Dictionary<string, Logger> _loggers = new();
    private static Logger.Level _globalMinLevel = Logger.Level.Info;
    
    /// <summary>
    /// Set global minimum log level
    /// </summary>
    public static void SetGlobalLevel(Logger.Level level)
    {
        _globalMinLevel = level;
        foreach (var logger in _loggers.Values)
        {
            logger.MinLevel = level;
        }
    }
    
    /// <summary>
    /// Get or create a logger
    /// </summary>
    public static Logger GetLogger(string name)
    {
        if (!_loggers.TryGetValue(name, out var logger))
        {
            logger = new Logger(name, _globalMinLevel);
            _loggers[name] = logger;
        }
        return logger;
    }
    
    /// <summary>
    /// Get or create a logger for a type
    /// </summary>
    public static Logger GetLogger<T>()
    {
        return GetLogger(typeof(T).Name);
    }
}
