using System;
using System.IO;
using System.Text.Json;

namespace MoonBrookRidge.World.Mining;

/// <summary>
/// Utility to load mine generation configurations from JSON files
/// </summary>
public static class MineGenConfigLoader
{
    /// <summary>
    /// Load mine generation configuration from a JSON file
    /// </summary>
    /// <param name="filePath">Path to the JSON configuration file</param>
    /// <returns>MineGenConfig object or null if loading fails</returns>
    public static MineGenConfig LoadMineConfig(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"Mine config file not found: {filePath}");
                return null;
            }
            
            string jsonContent = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            
            var config = JsonSerializer.Deserialize<MineGenConfig>(jsonContent, options);
            
            if (config == null)
            {
                Console.WriteLine($"Failed to deserialize mine config from: {filePath}");
                return null;
            }
            
            Console.WriteLine($"Successfully loaded mine config from: {filePath}");
            return config;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading mine config from {filePath}: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Save mine generation configuration to a JSON file
    /// </summary>
    public static bool SaveMineConfig(MineGenConfig config, string filePath)
    {
        try
        {
            var options = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            };
            
            string jsonContent = JsonSerializer.Serialize(config, options);
            File.WriteAllText(filePath, jsonContent);
            
            Console.WriteLine($"Successfully saved mine config to: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving mine config to {filePath}: {ex.Message}");
            return false;
        }
    }
}
