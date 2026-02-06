using System;
using System.IO;
using System.Text.Json;
using MoonBrookRidge.Engine.MonoGameCompat;
using MoonBrookRidge.Characters.Player;
using MoonBrookRidge.Core.Systems;
using MoonBrookRidge.Items.Inventory;
using MoonBrookRidge.World.Tiles;

namespace MoonBrookRidge.Core.Systems;

/// <summary>
/// Manages saving and loading game data
/// </summary>
public class SaveSystem
{
    private const string SAVE_FOLDER = "Saves";
    private const string SAVE_EXTENSION = ".json";
    
    public SaveSystem()
    {
        // Ensure save folder exists
        EnsureSaveFolderExists();
    }
    
    /// <summary>
    /// Saves the current game state
    /// </summary>
    public bool SaveGame(string saveName, GameSaveData saveData)
    {
        try
        {
            string savePath = GetSavePath(saveName);
            string jsonData = JsonSerializer.Serialize(saveData, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(savePath, jsonData);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Save failed: {ex.Message}");
            return false;
        }
    }
    
    /// <summary>
    /// Loads a saved game
    /// </summary>
    public GameSaveData? LoadGame(string saveName)
    {
        try
        {
            string savePath = GetSavePath(saveName);
            
            if (!File.Exists(savePath))
            {
                System.Diagnostics.Debug.WriteLine($"Save file not found: {savePath}");
                return null;
            }
            
            string jsonData = File.ReadAllText(savePath);
            return JsonSerializer.Deserialize<GameSaveData>(jsonData);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Load failed: {ex.Message}");
            return null;
        }
    }
    
    /// <summary>
    /// Checks if a save file exists
    /// </summary>
    public bool SaveExists(string saveName)
    {
        return File.Exists(GetSavePath(saveName));
    }
    
    /// <summary>
    /// Gets all available save files
    /// </summary>
    public string[] GetAllSaves()
    {
        try
        {
            string saveFolder = GetSaveFolder();
            if (!Directory.Exists(saveFolder))
            {
                return Array.Empty<string>();
            }
            
            string[] files = Directory.GetFiles(saveFolder, $"*{SAVE_EXTENSION}");
            string[] saveNames = new string[files.Length];
            
            for (int i = 0; i < files.Length; i++)
            {
                saveNames[i] = Path.GetFileNameWithoutExtension(files[i]);
            }
            
            return saveNames;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to get saves: {ex.Message}");
            return Array.Empty<string>();
        }
    }
    
    /// <summary>
    /// Deletes a save file
    /// </summary>
    public bool DeleteSave(string saveName)
    {
        try
        {
            string savePath = GetSavePath(saveName);
            if (File.Exists(savePath))
            {
                File.Delete(savePath);
                return true;
            }
            return false;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Delete failed: {ex.Message}");
            return false;
        }
    }
    
    private string GetSaveFolder()
    {
        // Save in user's local app data folder
        string appData = Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData);
        return Path.Combine(appData, "MoonBrookRidge", SAVE_FOLDER);
    }
    
    private string GetSavePath(string saveName)
    {
        return Path.Combine(GetSaveFolder(), saveName + SAVE_EXTENSION);
    }
    
    private void EnsureSaveFolderExists()
    {
        string saveFolder = GetSaveFolder();
        if (!Directory.Exists(saveFolder))
        {
            Directory.CreateDirectory(saveFolder);
        }
    }
}

/// <summary>
/// Container for all game save data
/// </summary>
public class GameSaveData
{
    public string SaveName { get; set; } = string.Empty;
    public DateTime SaveTime { get; set; }
    public int PlayTimeSeconds { get; set; }
    
    // Player data
    public PlayerSaveData Player { get; set; } = new();
    
    // Time system data
    public TimeSaveData Time { get; set; } = new();
    
    // Inventory data
    public InventorySaveData Inventory { get; set; } = new();
    
    // World data (simplified - just crops for now)
    public WorldSaveData World { get; set; } = new();
    
    // Phase 6 data
    public MagicSaveData Magic { get; set; } = new();
    public SkillsSaveData Skills { get; set; } = new();
    public PetsSaveData Pets { get; set; } = new();
    public DungeonProgressData DungeonProgress { get; set; } = new();
    
    public GameSaveData()
    {
        SaveTime = DateTime.Now;
    }
}

public class PlayerSaveData
{
    public float PositionX { get; set; }
    public float PositionY { get; set; }
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public float Energy { get; set; }
    public float MaxEnergy { get; set; }
    public float Hunger { get; set; }
    public float Thirst { get; set; }
    public int Money { get; set; }
    // Note: Mana is saved in MagicSaveData, not here
}

public class TimeSaveData
{
    public float TimeOfDay { get; set; }
    public int Day { get; set; }
    public int Season { get; set; } // 0=Spring, 1=Summer, 2=Fall, 3=Winter
    public int Year { get; set; }
}

public class InventorySaveData
{
    public InventorySlotData[] Slots { get; set; } = Array.Empty<InventorySlotData>();
}

public class InventorySlotData
{
    public string ItemName { get; set; } = string.Empty;
    public string ItemType { get; set; } = string.Empty;
    public int Quantity { get; set; }
}

public class WorldSaveData
{
    public CropSaveData[] Crops { get; set; } = Array.Empty<CropSaveData>();
}

public class CropSaveData
{
    public int GridX { get; set; }
    public int GridY { get; set; }
    public string CropType { get; set; } = string.Empty;
    public int GrowthStage { get; set; }
    public int MaxGrowthStage { get; set; }
    public float HoursGrown { get; set; }
    public float HoursPerStage { get; set; }
}

/// <summary>
/// Save data for magic system
/// </summary>
public class MagicSaveData
{
    public float CurrentMana { get; set; }
    public float MaxMana { get; set; }
    public string[] LearnedSpellIds { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Save data for skill tree system
/// </summary>
public class SkillsSaveData
{
    public int AvailableSkillPoints { get; set; }
    public SkillCategorySaveData[] Categories { get; set; } = Array.Empty<SkillCategorySaveData>();
}

public class SkillCategorySaveData
{
    public string CategoryName { get; set; } = string.Empty;
    public int Level { get; set; }
    public float Experience { get; set; }
    public string[] UnlockedSkillIds { get; set; } = Array.Empty<string>();
}

/// <summary>
/// Save data for pet system
/// </summary>
public class PetsSaveData
{
    public PetSaveData[] OwnedPets { get; set; } = Array.Empty<PetSaveData>();
    public string ActivePetId { get; set; } = string.Empty;
}

public class PetSaveData
{
    public string DefinitionId { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public int Level { get; set; }
    public float Experience { get; set; }
    public float Health { get; set; }
    public float MaxHealth { get; set; }
    public float Hunger { get; set; }
    public float Happiness { get; set; }
    public float PositionX { get; set; }
    public float PositionY { get; set; }
}

/// <summary>
/// Save data for dungeon progress
/// </summary>
public class DungeonProgressData
{
    public DungeonCompletionData[] CompletedDungeons { get; set; } = Array.Empty<DungeonCompletionData>();
    public bool IsInDungeon { get; set; }
    public string CurrentDungeonType { get; set; } = string.Empty;
    public int CurrentFloor { get; set; }
}

public class DungeonCompletionData
{
    public string DungeonType { get; set; } = string.Empty;
    public int HighestFloorReached { get; set; }
    public bool Completed { get; set; }
    public DateTime FirstCompletionTime { get; set; }
}
