using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Linq;

namespace MoonBrookRidge.Mods;

/// <summary>
/// Loads and manages mods for the game
/// </summary>
public class ModLoader
{
    private const string MODS_FOLDER = "Mods";
    private const string MOD_CONFIG_FILE = "mods_config.json";
    private List<ModInfo> _discoveredMods;
    private Dictionary<string, bool> _modEnabledState;
    
    public ModLoader()
    {
        _discoveredMods = new List<ModInfo>();
        _modEnabledState = new Dictionary<string, bool>();
        LoadModConfiguration();
    }
    
    /// <summary>
    /// Discover all available mods in the Mods folder
    /// </summary>
    public List<ModInfo> DiscoverMods()
    {
        _discoveredMods.Clear();
        
        // Ensure Mods folder exists
        if (!Directory.Exists(MODS_FOLDER))
        {
            Directory.CreateDirectory(MODS_FOLDER);
            CreateExampleMod();
        }
        
        // Scan for mod manifest files
        var modFolders = Directory.GetDirectories(MODS_FOLDER);
        
        foreach (var modFolder in modFolders)
        {
            string manifestPath = Path.Combine(modFolder, "manifest.json");
            
            if (File.Exists(manifestPath))
            {
                try
                {
                    string jsonContent = File.ReadAllText(manifestPath);
                    var modInfo = JsonSerializer.Deserialize<ModInfo>(jsonContent);
                    
                    if (modInfo != null)
                    {
                        modInfo.FolderPath = modFolder;
                        
                        // Set enabled state from configuration
                        if (_modEnabledState.ContainsKey(modInfo.Id))
                        {
                            modInfo.IsEnabled = _modEnabledState[modInfo.Id];
                        }
                        
                        _discoveredMods.Add(modInfo);
                    }
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"Error loading mod from {modFolder}: {ex.Message}");
                }
            }
        }
        
        return _discoveredMods;
    }
    
    /// <summary>
    /// Load mod configuration (which mods are enabled)
    /// </summary>
    private void LoadModConfiguration()
    {
        string configPath = Path.Combine(MODS_FOLDER, MOD_CONFIG_FILE);
        
        if (File.Exists(configPath))
        {
            try
            {
                string jsonContent = File.ReadAllText(configPath);
                _modEnabledState = JsonSerializer.Deserialize<Dictionary<string, bool>>(jsonContent) 
                                  ?? new Dictionary<string, bool>();
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading mod configuration: {ex.Message}");
                _modEnabledState = new Dictionary<string, bool>();
            }
        }
    }
    
    /// <summary>
    /// Save mod configuration (which mods are enabled)
    /// </summary>
    public void SaveModConfiguration()
    {
        // Update enabled state dictionary
        foreach (var mod in _discoveredMods)
        {
            _modEnabledState[mod.Id] = mod.IsEnabled;
        }
        
        string configPath = Path.Combine(MODS_FOLDER, MOD_CONFIG_FILE);
        
        try
        {
            if (!Directory.Exists(MODS_FOLDER))
            {
                Directory.CreateDirectory(MODS_FOLDER);
            }
            
            string jsonContent = JsonSerializer.Serialize(_modEnabledState, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(configPath, jsonContent);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error saving mod configuration: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Load all enabled mods and apply their content
    /// </summary>
    public void LoadEnabledMods()
    {
        foreach (var mod in _discoveredMods.Where(m => m.IsEnabled))
        {
            try
            {
                LoadModContent(mod);
                System.Diagnostics.Debug.WriteLine($"Loaded mod: {mod.Name} v{mod.Version}");
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading mod {mod.Name}: {ex.Message}");
            }
        }
    }
    
    /// <summary>
    /// Load content from a specific mod
    /// </summary>
    private void LoadModContent(ModInfo mod)
    {
        // Load custom items
        string itemsPath = Path.Combine(mod.FolderPath, "items.json");
        if (File.Exists(itemsPath))
        {
            LoadModItems(itemsPath);
        }
        
        // Load custom recipes
        string recipesPath = Path.Combine(mod.FolderPath, "recipes.json");
        if (File.Exists(recipesPath))
        {
            LoadModRecipes(recipesPath);
        }
        
        // Load custom crops
        string cropsPath = Path.Combine(mod.FolderPath, "crops.json");
        if (File.Exists(cropsPath))
        {
            LoadModCrops(cropsPath);
        }
        
        // Load custom NPCs
        string npcsPath = Path.Combine(mod.FolderPath, "npcs.json");
        if (File.Exists(npcsPath))
        {
            LoadModNPCs(npcsPath);
        }
        
        // Load custom quests
        string questsPath = Path.Combine(mod.FolderPath, "quests.json");
        if (File.Exists(questsPath))
        {
            LoadModQuests(questsPath);
        }
    }
    
    private void LoadModItems(string path)
    {
        try
        {
            string jsonContent = File.ReadAllText(path);
            var modItems = JsonSerializer.Deserialize<List<ModItemDefinition>>(jsonContent);
            
            if (modItems != null)
            {
                foreach (var modItem in modItems)
                {
                    // Validate item definition
                    if (string.IsNullOrEmpty(modItem.Name))
                    {
                        System.Diagnostics.Debug.WriteLine($"Skipping mod item with no name");
                        continue;
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Loaded mod item: {modItem.Name} (Type: {modItem.Type})");
                    // Note: Actual item registration would happen here in a full implementation
                    // This would require access to a global item registry
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading items from {path}: {ex.Message}");
        }
    }
    
    private void LoadModRecipes(string path)
    {
        try
        {
            string jsonContent = File.ReadAllText(path);
            var modRecipes = JsonSerializer.Deserialize<List<ModRecipeDefinition>>(jsonContent);
            
            if (modRecipes != null)
            {
                foreach (var modRecipe in modRecipes)
                {
                    // Validate recipe definition
                    if (string.IsNullOrEmpty(modRecipe.Name) || 
                        modRecipe.Ingredients == null || 
                        modRecipe.Ingredients.Count == 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Skipping invalid mod recipe: {modRecipe.Name}");
                        continue;
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Loaded mod recipe: {modRecipe.Name} -> {modRecipe.OutputName} x{modRecipe.OutputQuantity}");
                    // Note: Actual recipe registration would happen here in a full implementation
                    // This would require access to the CraftingSystem
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading recipes from {path}: {ex.Message}");
        }
    }
    
    private void LoadModCrops(string path)
    {
        try
        {
            string jsonContent = File.ReadAllText(path);
            var modCrops = JsonSerializer.Deserialize<List<ModCropDefinition>>(jsonContent);
            
            if (modCrops != null)
            {
                foreach (var modCrop in modCrops)
                {
                    // Validate crop definition
                    if (string.IsNullOrEmpty(modCrop.Name) || modCrop.GrowthDays <= 0)
                    {
                        System.Diagnostics.Debug.WriteLine($"Skipping invalid mod crop: {modCrop.Name}");
                        continue;
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Loaded mod crop: {modCrop.Name} (Growth: {modCrop.GrowthDays} days, Seasons: {string.Join(",", modCrop.Seasons)})");
                    // Note: Actual crop registration would happen here in a full implementation
                    // This would require access to crop and seed managers
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading crops from {path}: {ex.Message}");
        }
    }
    
    private void LoadModNPCs(string path)
    {
        try
        {
            string jsonContent = File.ReadAllText(path);
            var modNPCs = JsonSerializer.Deserialize<List<ModNPCDefinition>>(jsonContent);
            
            if (modNPCs != null)
            {
                foreach (var modNPC in modNPCs)
                {
                    // Validate NPC definition
                    if (string.IsNullOrEmpty(modNPC.Name))
                    {
                        System.Diagnostics.Debug.WriteLine($"Skipping mod NPC with no name");
                        continue;
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Loaded mod NPC: {modNPC.Name} (Birthday: {modNPC.Birthday})");
                    // Note: Actual NPC registration would happen here in a full implementation
                    // This would require access to NPCManager
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading NPCs from {path}: {ex.Message}");
        }
    }
    
    private void LoadModQuests(string path)
    {
        try
        {
            string jsonContent = File.ReadAllText(path);
            var modQuests = JsonSerializer.Deserialize<List<ModQuestDefinition>>(jsonContent);
            
            if (modQuests != null)
            {
                foreach (var modQuest in modQuests)
                {
                    // Validate quest definition
                    if (string.IsNullOrEmpty(modQuest.Id) || string.IsNullOrEmpty(modQuest.Title))
                    {
                        System.Diagnostics.Debug.WriteLine($"Skipping invalid mod quest: {modQuest.Id}");
                        continue;
                    }
                    
                    System.Diagnostics.Debug.WriteLine($"Loaded mod quest: {modQuest.Title} (ID: {modQuest.Id}, Type: {modQuest.Type})");
                    // Note: Actual quest registration would happen here in a full implementation
                    // This would require access to QuestSystem
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Error loading quests from {path}: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Create an example mod to demonstrate the mod structure
    /// </summary>
    private void CreateExampleMod()
    {
        string exampleModFolder = Path.Combine(MODS_FOLDER, "ExampleMod");
        
        if (!Directory.Exists(exampleModFolder))
        {
            Directory.CreateDirectory(exampleModFolder);
            
            // Create example manifest
            var exampleMod = new ModInfo
            {
                Id = "example.basicmod",
                Name = "Example Mod",
                Version = "1.0.0",
                Author = "MoonBrook Ridge Team",
                Description = "An example mod showing the basic structure. Add your own items, recipes, crops, NPCs, and quests!",
                IsEnabled = false
            };
            
            string manifestJson = JsonSerializer.Serialize(exampleMod, new JsonSerializerOptions 
            { 
                WriteIndented = true 
            });
            
            File.WriteAllText(Path.Combine(exampleModFolder, "manifest.json"), manifestJson);
            
            // Create example README
            string readme = @"# Example Mod for MoonBrook Ridge

This is an example mod showing the structure.

## Mod Structure

- manifest.json - Mod metadata (required)
- items.json - Custom items (optional)
- recipes.json - Custom crafting recipes (optional)
- crops.json - Custom crops (optional)
- npcs.json - Custom NPCs (optional)
- quests.json - Custom quests (optional)

## Creating Your Own Mod

1. Copy this folder and rename it
2. Edit manifest.json with your mod's information
3. Add your custom content in JSON files
4. Enable your mod in the Mods menu!

Happy modding!
";
            File.WriteAllText(Path.Combine(exampleModFolder, "README.md"), readme);
        }
    }
    
    public List<ModInfo> DiscoveredMods => _discoveredMods;
}

/// <summary>
/// Information about a mod
/// </summary>
public class ModInfo
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
    public string Version { get; set; } = "1.0.0";
    public string Author { get; set; } = "";
    public string Description { get; set; } = "";
    public bool IsEnabled { get; set; } = false;
    public string FolderPath { get; set; } = "";
}

/// <summary>
/// Mod item definition from JSON
/// </summary>
public class ModItemDefinition
{
    public string Name { get; set; } = "";
    public string Description { get; set; } = "";
    public string Type { get; set; } = "Crafting";
    public int MaxStackSize { get; set; } = 99;
    public int SellPrice { get; set; } = 0;
    public int BuyPrice { get; set; } = 0;
}

/// <summary>
/// Mod recipe definition from JSON
/// </summary>
public class ModRecipeDefinition
{
    public string Name { get; set; } = "";
    public Dictionary<string, int> Ingredients { get; set; } = new Dictionary<string, int>();
    public string OutputName { get; set; } = "";
    public int OutputQuantity { get; set; } = 1;
}

/// <summary>
/// Mod crop definition from JSON
/// </summary>
public class ModCropDefinition
{
    public string Name { get; set; } = "";
    public int GrowthDays { get; set; } = 7;
    public List<string> Seasons { get; set; } = new List<string>();
    public int SeedPrice { get; set; } = 50;
    public int HarvestSellPrice { get; set; } = 100;
    public bool Regrowable { get; set; } = false;
    public int RegrowthDays { get; set; } = 0;
}

/// <summary>
/// Mod NPC definition from JSON
/// </summary>
public class ModNPCDefinition
{
    public string Name { get; set; } = "";
    public string Birthday { get; set; } = "Spring 1";
    public List<string> LovedGifts { get; set; } = new List<string>();
    public List<string> LikedGifts { get; set; } = new List<string>();
    public List<string> DislikedGifts { get; set; } = new List<string>();
    public List<string> HatedGifts { get; set; } = new List<string>();
    public Dictionary<string, string> Dialogues { get; set; } = new Dictionary<string, string>();
}

/// <summary>
/// Mod quest definition from JSON
/// </summary>
public class ModQuestDefinition
{
    public string Id { get; set; } = "";
    public string Title { get; set; } = "";
    public string Description { get; set; } = "";
    public string Type { get; set; } = "Task";
    public Dictionary<string, int> Rewards { get; set; } = new Dictionary<string, int>();
    public int GoldReward { get; set; } = 0;
    public int ExperienceReward { get; set; } = 0;
}
