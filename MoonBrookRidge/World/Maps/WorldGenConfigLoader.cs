using System;
using System.IO;
using System.Text.Json;

namespace MoonBrookRidge.World.Maps;

/// <summary>
/// Utility to load world generation configurations from JSON files
/// </summary>
public static class WorldGenConfigLoader
{
    /// <summary>
    /// Load world generation configuration from a JSON file
    /// </summary>
    /// <param name="filePath">Path to the JSON configuration file</param>
    /// <returns>WorldGenConfig object or null if loading fails</returns>
    public static WorldGenConfig LoadWorldConfig(string filePath)
    {
        try
        {
            if (!File.Exists(filePath))
            {
                Console.WriteLine($"World config file not found: {filePath}");
                return null;
            }
            
            string jsonContent = File.ReadAllText(filePath);
            var options = new JsonSerializerOptions
            {
                PropertyNameCaseInsensitive = true,
                ReadCommentHandling = JsonCommentHandling.Skip,
                AllowTrailingCommas = true
            };
            
            var config = JsonSerializer.Deserialize<WorldGenConfig>(jsonContent, options);
            
            if (config == null)
            {
                Console.WriteLine($"Failed to deserialize world config from: {filePath}");
                return null;
            }
            
            Console.WriteLine($"Successfully loaded world config from: {filePath}");
            return config;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error loading world config from {filePath}: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Save world generation configuration to a JSON file
    /// </summary>
    public static bool SaveWorldConfig(WorldGenConfig config, string filePath)
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
            
            Console.WriteLine($"Successfully saved world config to: {filePath}");
            return true;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error saving world config to {filePath}: {ex.Message}");
            return false;
        }
    }
}
