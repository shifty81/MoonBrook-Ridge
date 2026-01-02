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
        
        // Basic fertilizer
        var fertilizerRecipe = new Recipe("Fertilizer");
        fertilizerRecipe.AddIngredient("Wood", 1);
        fertilizerRecipe.AddIngredient("Stone", 1);
        fertilizerRecipe.SetOutput("Fertilizer", 5);
        _recipes.Add("Fertilizer", fertilizerRecipe);
        
        // Scarecrow
        var scarecrowRecipe = new Recipe("Scarecrow");
        scarecrowRecipe.AddIngredient("Wood", 10);
        scarecrowRecipe.SetOutput("Scarecrow", 1);
        _recipes.Add("Scarecrow", scarecrowRecipe);
        
        // Stone path
        var pathRecipe = new Recipe("Stone Path");
        pathRecipe.AddIngredient("Stone", 3);
        pathRecipe.SetOutput("Stone Path", 1);
        _recipes.Add("Stone Path", pathRecipe);
        
        // NEW RECIPES - Tools & Equipment
        var copperAxe = new Recipe("Copper Axe");
        copperAxe.AddIngredient("Wood", 5);
        copperAxe.AddIngredient("Copper Ore", 10);
        copperAxe.SetOutput("Copper Axe", 1);
        _recipes.Add("Copper Axe", copperAxe);
        
        var copperPickaxe = new Recipe("Copper Pickaxe");
        copperPickaxe.AddIngredient("Wood", 5);
        copperPickaxe.AddIngredient("Copper Ore", 10);
        copperPickaxe.SetOutput("Copper Pickaxe", 1);
        _recipes.Add("Copper Pickaxe", copperPickaxe);
        
        var sprinkler = new Recipe("Sprinkler");
        sprinkler.AddIngredient("Iron Bar", 1);
        sprinkler.AddIngredient("Copper Bar", 1);
        sprinkler.SetOutput("Sprinkler", 1);
        _recipes.Add("Sprinkler", sprinkler);
        
        // NEW RECIPES - Food Processing
        var flour = new Recipe("Flour");
        flour.AddIngredient("Wheat", 1);
        flour.SetOutput("Flour", 1);
        _recipes.Add("Flour", flour);
        
        var bread = new Recipe("Bread");
        bread.AddIngredient("Flour", 1);
        bread.SetOutput("Wheat Bread", 1);
        _recipes.Add("Bread", bread);
        
        var salad = new Recipe("Salad");
        salad.AddIngredient("Lettuce", 1);
        salad.AddIngredient("Tomato", 1);
        salad.AddIngredient("Carrot", 1);
        salad.SetOutput("Fresh Salad", 1);
        _recipes.Add("Salad", salad);
        
        var vegStew = new Recipe("Vegetable Stew");
        vegStew.AddIngredient("Potato", 2);
        vegStew.AddIngredient("Carrot", 1);
        vegStew.AddIngredient("Cabbage", 1);
        vegStew.SetOutput("Vegetable Stew", 1);
        _recipes.Add("Vegetable Stew", vegStew);
        
        var pumpkinSoup = new Recipe("Pumpkin Soup");
        pumpkinSoup.AddIngredient("Pumpkin", 1);
        pumpkinSoup.AddIngredient("Milk", 1);
        pumpkinSoup.SetOutput("Pumpkin Soup", 1);
        _recipes.Add("Pumpkin Soup", pumpkinSoup);
        
        var berryJam = new Recipe("Berry Jam");
        berryJam.AddIngredient("Strawberry", 3);
        berryJam.SetOutput("Strawberry Jam", 1);
        _recipes.Add("Berry Jam", berryJam);
        
        // NEW RECIPES - Decorations
        var woodenSign = new Recipe("Wooden Sign");
        woodenSign.AddIngredient("Wood", 5);
        woodenSign.SetOutput("Wooden Sign", 1);
        _recipes.Add("Wooden Sign", woodenSign);
        
        var torch = new Recipe("Torch");
        torch.AddIngredient("Wood", 1);
        torch.AddIngredient("Coal", 1);
        torch.SetOutput("Torch", 5);
        _recipes.Add("Torch", torch);
        
        var flowerPot = new Recipe("Flower Pot");
        flowerPot.AddIngredient("Clay", 3);
        flowerPot.SetOutput("Flower Pot", 1);
        _recipes.Add("Flower Pot", flowerPot);
        
        // NEW RECIPES - Advanced Materials
        var ironBar = new Recipe("Iron Bar");
        ironBar.AddIngredient("Iron Ore", 5);
        ironBar.AddIngredient("Coal", 1);
        ironBar.SetOutput("Iron Bar", 1);
        _recipes.Add("Iron Bar", ironBar);
        
        var copperBar = new Recipe("Copper Bar");
        copperBar.AddIngredient("Copper Ore", 5);
        copperBar.AddIngredient("Coal", 1);
        copperBar.SetOutput("Copper Bar", 1);
        _recipes.Add("Copper Bar", copperBar);
        
        var goldBar = new Recipe("Gold Bar");
        goldBar.AddIngredient("Gold Ore", 5);
        goldBar.AddIngredient("Coal", 1);
        goldBar.SetOutput("Gold Bar", 1);
        _recipes.Add("Gold Bar", goldBar);
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
