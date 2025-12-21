using System.Collections.Generic;
using MoonBrookRidge.Items.Inventory;

namespace MoonBrookRidge.Items.Crafting;

/// <summary>
/// Crafting system for creating items from recipes
/// </summary>
public class CraftingSystem
{
    private Dictionary<string, Recipe> _recipes;
    
    public CraftingSystem()
    {
        _recipes = new Dictionary<string, Recipe>();
        InitializeRecipes();
    }
    
    private void InitializeRecipes()
    {
        // Example recipes (would be loaded from data files in full implementation)
        
        // Basic fence
        var fenceRecipe = new Recipe("Wood Fence");
        fenceRecipe.AddIngredient("Wood", 2);
        fenceRecipe.SetOutput("Wood Fence", 1);
        _recipes.Add("Wood Fence", fenceRecipe);
        
        // Chest
        var chestRecipe = new Recipe("Chest");
        chestRecipe.AddIngredient("Wood", 50);
        chestRecipe.SetOutput("Chest", 1);
        _recipes.Add("Chest", chestRecipe);
    }
    
    public bool CanCraft(string recipeName, InventorySystem inventory)
    {
        if (!_recipes.ContainsKey(recipeName)) return false;
        
        Recipe recipe = _recipes[recipeName];
        
        foreach (var ingredient in recipe.Ingredients)
        {
            if (inventory.GetItemCount(ingredient.Key) < ingredient.Value)
            {
                return false;
            }
        }
        
        return true;
    }
    
    public bool Craft(string recipeName, InventorySystem inventory)
    {
        if (!CanCraft(recipeName, inventory)) return false;
        
        Recipe recipe = _recipes[recipeName];
        
        // Remove ingredients
        foreach (var ingredient in recipe.Ingredients)
        {
            inventory.RemoveItem(ingredient.Key, ingredient.Value);
        }
        
        // Add crafted item
        Item craftedItem = new Item(recipe.OutputName, ItemType.Crafting);
        inventory.AddItem(craftedItem, recipe.OutputQuantity);
        
        return true;
    }
    
    public List<Recipe> GetAllRecipes()
    {
        return new List<Recipe>(_recipes.Values);
    }
    
    public void AddRecipe(Recipe recipe)
    {
        _recipes[recipe.Name] = recipe;
    }
}

/// <summary>
/// Crafting recipe
/// </summary>
public class Recipe
{
    public string Name { get; set; }
    public Dictionary<string, int> Ingredients { get; private set; }
    public string OutputName { get; private set; }
    public int OutputQuantity { get; private set; }
    
    public Recipe(string name)
    {
        Name = name;
        Ingredients = new Dictionary<string, int>();
    }
    
    public void AddIngredient(string itemName, int quantity)
    {
        Ingredients[itemName] = quantity;
    }
    
    public void SetOutput(string itemName, int quantity)
    {
        OutputName = itemName;
        OutputQuantity = quantity;
    }
}
