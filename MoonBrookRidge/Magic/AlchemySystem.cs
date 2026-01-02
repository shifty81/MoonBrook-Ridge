using System;
using System.Collections.Generic;
using System.Linq;
using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Magic;

/// <summary>
/// Alchemy system for brewing potions and elixirs
/// </summary>
public class AlchemySystem
{
    private List<PotionRecipe> _recipes;
    private List<Potion> _availablePotions;
    
    public event Action<Potion> OnPotionBrewed;
    
    public AlchemySystem()
    {
        _recipes = new List<PotionRecipe>();
        _availablePotions = new List<Potion>();
        
        InitializeRecipes();
    }
    
    private void InitializeRecipes()
    {
        // Basic health potions
        _recipes.Add(new PotionRecipe(
            "health_minor",
            "Minor Health Potion",
            new Dictionary<string, int> { { "Berry", 2 }, { "Herb", 1 } },
            new Potion("health_minor", "Minor Health Potion", "Restores 25 health", PotionEffect.Health, 25f, 0f)
        ));
        
        _recipes.Add(new PotionRecipe(
            "health_major",
            "Major Health Potion",
            new Dictionary<string, int> { { "Strawberry", 3 }, { "Herb", 2 }, { "Honey", 1 } },
            new Potion("health_major", "Major Health Potion", "Restores 60 health", PotionEffect.Health, 60f, 0f)
        ));
        
        // Mana potions
        _recipes.Add(new PotionRecipe(
            "mana_minor",
            "Minor Mana Potion",
            new Dictionary<string, int> { { "Blueberry", 2 }, { "Crystal", 1 } },
            new Potion("mana_minor", "Minor Mana Potion", "Restores 30 mana", PotionEffect.Mana, 30f, 0f)
        ));
        
        _recipes.Add(new PotionRecipe(
            "mana_major",
            "Major Mana Potion",
            new Dictionary<string, int> { { "Grape", 3 }, { "Crystal", 2 }, { "Magic Essence", 1 } },
            new Potion("mana_major", "Major Mana Potion", "Restores 75 mana", PotionEffect.Mana, 75f, 0f)
        ));
        
        // Energy potions
        _recipes.Add(new PotionRecipe(
            "energy_boost",
            "Energy Elixir",
            new Dictionary<string, int> { { "Coffee Bean", 2 }, { "Honey", 1 } },
            new Potion("energy_boost", "Energy Elixir", "Restores 40 energy", PotionEffect.Energy, 40f, 0f)
        ));
        
        // Buff potions
        _recipes.Add(new PotionRecipe(
            "speed_potion",
            "Swiftness Potion",
            new Dictionary<string, int> { { "Feather", 2 }, { "Herb", 1 }, { "Honey", 1 } },
            new Potion("speed_potion", "Swiftness Potion", "Increase movement speed for 60s", PotionEffect.SpeedBuff, 1.5f, 60f)
        ));
        
        _recipes.Add(new PotionRecipe(
            "strength_potion",
            "Strength Potion",
            new Dictionary<string, int> { { "Iron Ore", 1 }, { "Herb", 2 }, { "Honey", 1 } },
            new Potion("strength_potion", "Strength Potion", "Increase tool efficiency for 120s", PotionEffect.StrengthBuff, 1.3f, 120f)
        ));
        
        _recipes.Add(new PotionRecipe(
            "luck_potion",
            "Fortune Elixir",
            new Dictionary<string, int> { { "Four-Leaf Clover", 1 }, { "Gold Ore", 1 }, { "Honey", 1 } },
            new Potion("luck_potion", "Fortune Elixir", "Increase rare item chance for 180s", PotionEffect.LuckBuff, 2.0f, 180f)
        ));
        
        // Special potions
        _recipes.Add(new PotionRecipe(
            "night_vision",
            "Night Vision Potion",
            new Dictionary<string, int> { { "Glow Mushroom", 2 }, { "Crystal", 1 } },
            new Potion("night_vision", "Night Vision Potion", "See clearly in darkness for 300s", PotionEffect.NightVision, 1f, 300f)
        ));
        
        _recipes.Add(new PotionRecipe(
            "water_breathing",
            "Aqua Lung Potion",
            new Dictionary<string, int> { { "Seaweed", 3 }, { "Fish Scale", 2 } },
            new Potion("water_breathing", "Aqua Lung Potion", "Breathe underwater for 180s", PotionEffect.WaterBreathing, 1f, 180f)
        ));
    }
    
    public bool CanBrewPotion(string recipeId, InventorySystem inventory)
    {
        var recipe = _recipes.FirstOrDefault(r => r.Id == recipeId);
        if (recipe == null) return false;
        
        // Check if player has all ingredients
        foreach (var ingredient in recipe.Ingredients)
        {
            if (inventory.GetItemCount(ingredient.Key) < ingredient.Value)
            {
                return false;
            }
        }
        
        return true;
    }
    
    public Potion BrewPotion(string recipeId, InventorySystem inventory)
    {
        var recipe = _recipes.FirstOrDefault(r => r.Id == recipeId);
        if (recipe == null || !CanBrewPotion(recipeId, inventory))
        {
            return null;
        }
        
        // Consume ingredients
        foreach (var ingredient in recipe.Ingredients)
        {
            inventory.RemoveItem(ingredient.Key, ingredient.Value);
        }
        
        // Create potion
        var potion = recipe.Result;
        OnPotionBrewed?.Invoke(potion);
        
        return potion;
    }
    
    public List<PotionRecipe> GetAllRecipes() => _recipes;
    
    public List<PotionRecipe> GetAvailableRecipes(InventorySystem inventory)
    {
        return _recipes.Where(r => CanBrewPotion(r.Id, inventory)).ToList();
    }
}

/// <summary>
/// Potion recipe definition
/// </summary>
public class PotionRecipe
{
    public string Id { get; }
    public string Name { get; }
    public Dictionary<string, int> Ingredients { get; } // Item name -> quantity
    public Potion Result { get; }
    
    public PotionRecipe(string id, string name, Dictionary<string, int> ingredients, Potion result)
    {
        Id = id;
        Name = name;
        Ingredients = ingredients;
        Result = result;
    }
}

/// <summary>
/// Potion item with effects
/// </summary>
public class Potion : Item
{
    public PotionEffect Effect { get; }
    public float EffectValue { get; }
    public float Duration { get; } // Duration in seconds (0 = instant)
    
    public Potion(string id, string name, string description, PotionEffect effect, float effectValue, float duration)
        : base(name, ItemType.Food, 20) // Potions stack to 20
    {
        Description = description;
        Effect = effect;
        EffectValue = effectValue;
        Duration = duration;
    }
}

public enum PotionEffect
{
    Health,           // Restore health
    Mana,            // Restore mana
    Energy,          // Restore energy
    SpeedBuff,       // Increase movement speed
    StrengthBuff,    // Increase damage/tool efficiency
    LuckBuff,        // Increase rare item drops
    NightVision,     // See in darkness
    WaterBreathing,  // Breathe underwater
    Regeneration,    // Health over time
    Invulnerability  // Temporary immunity
}
