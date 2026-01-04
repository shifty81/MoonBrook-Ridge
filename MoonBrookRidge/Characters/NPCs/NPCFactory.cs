using MoonBrookRidge.Engine.MonoGameCompat;
using System.Collections.Generic;

namespace MoonBrookRidge.Characters.NPCs;

/// <summary>
/// Factory for creating NPCs with pre-configured personalities, schedules, and dialogues
/// </summary>
public static class NPCFactory
{
    /// <summary>
    /// Create Emma - The Farmer
    /// Loves crops and flowers, friendly and helpful
    /// </summary>
    public static NPCCharacter CreateEmma(Vector2 startPosition)
    {
        var emma = new NPCCharacter("Emma", startPosition);
        
        // Gift preferences
        emma.SetGiftPreferences(
            loved: new List<string> { "Sunflower", "Pumpkin", "Strawberry", "Cauliflower" },
            liked: new List<string> { "Wheat", "Carrot", "Potato", "Lettuce", "Corn" },
            disliked: new List<string> { "Stone", "Iron Ore", "Coal" },
            hated: new List<string> { "Trash", "Junk" }
        );
        
        // Daily schedule
        emma.Schedule.AddScheduleEntry(6.0f, new ScheduleLocation
        {
            Position = new Vector2(200, 150),
            LocationName = "Home",
            Activity = "Wake up"
        });
        
        emma.Schedule.AddScheduleEntry(8.0f, new ScheduleLocation
        {
            Position = new Vector2(300, 200),
            LocationName = "Farm",
            Activity = "Tending crops"
        });
        
        emma.Schedule.AddScheduleEntry(18.0f, new ScheduleLocation
        {
            Position = new Vector2(200, 150),
            LocationName = "Home",
            Activity = "Relaxing"
        });
        
        // Dialogue tree
        SetupEmmaDialogue(emma);
        
        return emma;
    }
    
    /// <summary>
    /// Create Marcus - The Blacksmith
    /// Loves minerals and ores, gruff but caring
    /// </summary>
    public static NPCCharacter CreateMarcus(Vector2 startPosition)
    {
        var marcus = new NPCCharacter("Marcus", startPosition);
        
        // Gift preferences
        marcus.SetGiftPreferences(
            loved: new List<string> { "Gold Bar", "Iron Bar", "Copper Bar", "Diamond", "Ruby" },
            liked: new List<string> { "Gold Ore", "Iron Ore", "Copper Ore", "Stone", "Coal" },
            disliked: new List<string> { "Wheat", "Flower", "Berry" },
            hated: new List<string> { "Trash", "Weeds" }
        );
        
        // Daily schedule
        marcus.Schedule.AddScheduleEntry(7.0f, new ScheduleLocation
        {
            Position = new Vector2(400, 300),
            LocationName = "Blacksmith",
            Activity = "Opening shop"
        });
        
        marcus.Schedule.AddScheduleEntry(9.0f, new ScheduleLocation
        {
            Position = new Vector2(420, 320),
            LocationName = "Forge",
            Activity = "Working at forge"
        });
        
        marcus.Schedule.AddScheduleEntry(19.0f, new ScheduleLocation
        {
            Position = new Vector2(400, 300),
            LocationName = "Blacksmith",
            Activity = "Closing shop"
        });
        
        SetupMarcusDialogue(marcus);
        
        return marcus;
    }
    
    /// <summary>
    /// Create Lily - The Merchant
    /// Loves gems and valuable items, savvy businesswoman
    /// </summary>
    public static NPCCharacter CreateLily(Vector2 startPosition)
    {
        var lily = new NPCCharacter("Lily", startPosition);
        
        // Gift preferences
        lily.SetGiftPreferences(
            loved: new List<string> { "Diamond", "Ruby", "Emerald", "Gold Bar", "Pearl" },
            liked: new List<string> { "Jewelry", "Gold Ore", "Silver", "Sunflower" },
            disliked: new List<string> { "Stone", "Dirt", "Weeds" },
            hated: new List<string> { "Trash", "Junk", "Garbage" }
        );
        
        // Daily schedule
        lily.Schedule.AddScheduleEntry(8.0f, new ScheduleLocation
        {
            Position = new Vector2(600, 200),
            LocationName = "Shop",
            Activity = "Opening shop"
        });
        
        lily.Schedule.AddScheduleEntry(10.0f, new ScheduleLocation
        {
            Position = new Vector2(620, 220),
            LocationName = "Counter",
            Activity = "Working"
        });
        
        lily.Schedule.AddScheduleEntry(20.0f, new ScheduleLocation
        {
            Position = new Vector2(600, 200),
            LocationName = "Shop",
            Activity = "Closing"
        });
        
        SetupLilyDialogue(lily);
        
        return lily;
    }
    
    /// <summary>
    /// Create Oliver - The Fisherman
    /// Loves fish and seafood, laid-back and wise
    /// </summary>
    public static NPCCharacter CreateOliver(Vector2 startPosition)
    {
        var oliver = new NPCCharacter("Oliver", startPosition);
        
        // Gift preferences
        oliver.SetGiftPreferences(
            loved: new List<string> { "Salmon", "Legendary Fish", "Sturgeon", "King Salmon", "Swordfish" },
            liked: new List<string> { "Bass", "Trout", "Catfish", "Pike", "Tuna" },
            disliked: new List<string> { "Ore", "Stone", "Coal" },
            hated: new List<string> { "Trash", "Garbage", "Junk" }
        );
        
        // Daily schedule
        oliver.Schedule.AddScheduleEntry(5.0f, new ScheduleLocation
        {
            Position = new Vector2(100, 400),
            LocationName = "Dock",
            Activity = "Early fishing"
        });
        
        oliver.Schedule.AddScheduleEntry(12.0f, new ScheduleLocation
        {
            Position = new Vector2(150, 420),
            LocationName = "Dock",
            Activity = "Lunch break"
        });
        
        oliver.Schedule.AddScheduleEntry(21.0f, new ScheduleLocation
        {
            Position = new Vector2(100, 400),
            LocationName = "Dock",
            Activity = "Evening fishing"
        });
        
        SetupOliverDialogue(oliver);
        
        return oliver;
    }
    
    // NEW NPCs
    
    /// <summary>
    /// Create Sarah - The Doctor
    /// Loves medicinal herbs and healthy foods, caring and knowledgeable
    /// </summary>
    public static NPCCharacter CreateSarah(Vector2 startPosition)
    {
        var sarah = new NPCCharacter("Sarah", startPosition);
        
        // Gift preferences
        sarah.SetGiftPreferences(
            loved: new List<string> { "Energy Elixir", "Stamina Tonic", "Apple", "Berry", "Grape" },
            liked: new List<string> { "Fresh Salad", "Fruit Salad", "Vegetable Stew", "Tea" },
            disliked: new List<string> { "Junk Food", "Beer", "Wine" },
            hated: new List<string> { "Trash", "Cigarettes" }
        );
        
        // Daily schedule
        sarah.Schedule.AddScheduleEntry(8.0f, new ScheduleLocation
        {
            Position = new Vector2(500, 150),
            LocationName = "Clinic",
            Activity = "Opening clinic"
        });
        
        sarah.Schedule.AddScheduleEntry(9.0f, new ScheduleLocation
        {
            Position = new Vector2(520, 170),
            LocationName = "Office",
            Activity = "Seeing patients"
        });
        
        sarah.Schedule.AddScheduleEntry(17.0f, new ScheduleLocation
        {
            Position = new Vector2(480, 130),
            LocationName = "Garden",
            Activity = "Tending herb garden"
        });
        
        sarah.Schedule.AddScheduleEntry(20.0f, new ScheduleLocation
        {
            Position = new Vector2(500, 150),
            LocationName = "Home",
            Activity = "Resting"
        });
        
        SetupSarahDialogue(sarah);
        
        return sarah;
    }
    
    /// <summary>
    /// Create Jack - The Carpenter
    /// Loves wood and building materials, creative and hardworking
    /// </summary>
    public static NPCCharacter CreateJack(Vector2 startPosition)
    {
        var jack = new NPCCharacter("Jack", startPosition);
        
        // Gift preferences
        jack.SetGiftPreferences(
            loved: new List<string> { "Hardwood", "Mahogany", "Oak", "Wood Fence", "Chest" },
            liked: new List<string> { "Wood", "Stone", "Iron Bar", "Copper Bar" },
            disliked: new List<string> { "Fish", "Crops" },
            hated: new List<string> { "Trash", "Weeds" }
        );
        
        // Daily schedule
        jack.Schedule.AddScheduleEntry(6.0f, new ScheduleLocation
        {
            Position = new Vector2(700, 300),
            LocationName = "Workshop",
            Activity = "Starting work"
        });
        
        jack.Schedule.AddScheduleEntry(8.0f, new ScheduleLocation
        {
            Position = new Vector2(720, 320),
            LocationName = "Workbench",
            Activity = "Building"
        });
        
        jack.Schedule.AddScheduleEntry(12.0f, new ScheduleLocation
        {
            Position = new Vector2(680, 280),
            LocationName = "Break area",
            Activity = "Lunch"
        });
        
        jack.Schedule.AddScheduleEntry(13.0f, new ScheduleLocation
        {
            Position = new Vector2(720, 320),
            LocationName = "Workbench",
            Activity = "Afternoon work"
        });
        
        jack.Schedule.AddScheduleEntry(18.0f, new ScheduleLocation
        {
            Position = new Vector2(700, 300),
            LocationName = "Workshop",
            Activity = "Cleaning up"
        });
        
        SetupJackDialogue(jack);
        
        return jack;
    }
    
    /// <summary>
    /// Create Maya - The Artist
    /// Loves flowers and beautiful things, creative and free-spirited
    /// </summary>
    public static NPCCharacter CreateMaya(Vector2 startPosition)
    {
        var maya = new NPCCharacter("Maya", startPosition);
        
        // Gift preferences
        maya.SetGiftPreferences(
            loved: new List<string> { "Sunflower", "Strawberry", "Grape", "Diamond", "Rainbow Shell" },
            liked: new List<string> { "Flower", "Butterfly", "Feather", "Colored Dye", "Paint" },
            disliked: new List<string> { "Ore", "Coal", "Stone" },
            hated: new List<string> { "Trash", "Garbage", "Bug Meat" }
        );
        
        // Daily schedule
        maya.Schedule.AddScheduleEntry(9.0f, new ScheduleLocation
        {
            Position = new Vector2(250, 350),
            LocationName = "Art Studio",
            Activity = "Painting"
        });
        
        maya.Schedule.AddScheduleEntry(12.0f, new ScheduleLocation
        {
            Position = new Vector2(300, 300),
            LocationName = "Town Square",
            Activity = "People watching"
        });
        
        maya.Schedule.AddScheduleEntry(14.0f, new ScheduleLocation
        {
            Position = new Vector2(350, 400),
            LocationName = "Meadow",
            Activity = "Sketching nature"
        });
        
        maya.Schedule.AddScheduleEntry(19.0f, new ScheduleLocation
        {
            Position = new Vector2(250, 350),
            LocationName = "Art Studio",
            Activity = "Evening painting"
        });
        
        SetupMayaDialogue(maya);
        
        return maya;
    }
    
    // Dialogue Setup Methods
    
    private static void SetupEmmaDialogue(NPCCharacter npc)
    {
        var greeting = new DialogueNode("Oh, hello! Lovely day for farming, isn't it?", "Emma");
        
        var option1 = new DialogueNode("I agree! The crops are growing well.", "Emma");
        var option2 = new DialogueNode("I'm new to farming. Any tips?", "Emma");
        var option3 = new DialogueNode("See you later!", "Emma");
        
        greeting.AddOption("Indeed it is!", option1);
        greeting.AddOption("I could use some advice.", option2);
        greeting.AddOption("Goodbye.", option3);
        
        var dialogueTree = new DialogueTree(greeting);
        npc.AddDialogueTree("greeting", dialogueTree);
    }
    
    private static void SetupMarcusDialogue(NPCCharacter npc)
    {
        var greeting = new DialogueNode("What brings you to the forge?", "Marcus");
        
        var option1 = new DialogueNode("Good quality work, as always.", "Marcus");
        var option2 = new DialogueNode("That sounds useful!", "Marcus");
        var option3 = new DialogueNode("Thanks anyway.", "Marcus");
        
        greeting.AddOption("Just looking around.", option1);
        greeting.AddOption("Can you upgrade tools?", option2);
        greeting.AddOption("Just passing through.", option3);
        
        var dialogueTree = new DialogueTree(greeting);
        npc.AddDialogueTree("greeting", dialogueTree);
    }
    
    private static void SetupLilyDialogue(NPCCharacter npc)
    {
        var greeting = new DialogueNode("Welcome to my shop! Looking for something special?", "Lily");
        
        var option1 = new DialogueNode("I appreciate that, dear customer!", "Lily");
        var option2 = new DialogueNode("Of course! Quality goods at fair prices.", "Lily");
        var option3 = new DialogueNode("Come back anytime!", "Lily");
        
        greeting.AddOption("Just browsing.", option1);
        greeting.AddOption("What's your best item?", option2);
        greeting.AddOption("Maybe later.", option3);
        
        var dialogueTree = new DialogueTree(greeting);
        npc.AddDialogueTree("greeting", dialogueTree);
    }
    
    private static void SetupOliverDialogue(NPCCharacter npc)
    {
        var greeting = new DialogueNode("Ahoy there! Perfect weather for fishing today.", "Oliver");
        
        var option1 = new DialogueNode("Patience and the right spot, that's all.", "Oliver");
        var option2 = new DialogueNode("Been at it for decades. Never gets old.", "Oliver");
        var option3 = new DialogueNode("See you around, friend.", "Oliver");
        
        greeting.AddOption("Any catches today?", option1);
        greeting.AddOption("How long have you been fishing?", option2);
        greeting.AddOption("Take care.", option3);
        
        var dialogueTree = new DialogueTree(greeting);
        npc.AddDialogueTree("greeting", dialogueTree);
    }
    
    private static void SetupSarahDialogue(NPCCharacter npc)
    {
        var greeting = new DialogueNode("Hello! How are you feeling today?", "Sarah");
        
        var option1 = new DialogueNode("Wonderful! Prevention is the best medicine.", "Sarah");
        var option2 = new DialogueNode("Fresh vegetables, regular sleep, and stay active!", "Sarah");
        var option3 = new DialogueNode("Take care of yourself!", "Sarah");
        
        greeting.AddOption("I'm feeling great!", option1);
        greeting.AddOption("Any health tips?", option2);
        greeting.AddOption("See you later.", option3);
        
        var dialogueTree = new DialogueTree(greeting);
        npc.AddDialogueTree("greeting", dialogueTree);
    }
    
    private static void SetupJackDialogue(NPCCharacter npc)
    {
        var greeting = new DialogueNode("Hey! Working on a new project today.", "Jack");
        
        var option1 = new DialogueNode("Thanks! Been working on my craft for years.", "Jack");
        var option2 = new DialogueNode("Bring me the materials and I'll build anything!", "Jack");
        var option3 = new DialogueNode("See ya around!", "Jack");
        
        greeting.AddOption("Your work is impressive!", option1);
        greeting.AddOption("Can you build custom furniture?", option2);
        greeting.AddOption("Good luck with it!", option3);
        
        var dialogueTree = new DialogueTree(greeting);
        npc.AddDialogueTree("greeting", dialogueTree);
    }
    
    private static void SetupMayaDialogue(NPCCharacter npc)
    {
        var greeting = new DialogueNode("Oh, hello! The light is so beautiful today...", "Maya");
        
        var option1 = new DialogueNode("Thank you! I try to capture the essence of nature.", "Maya");
        var option2 = new DialogueNode("The world is full of beauty if you look closely.", "Maya");
        var option3 = new DialogueNode("Farewell, friend.", "Maya");
        
        greeting.AddOption("Your paintings are stunning!", option1);
        greeting.AddOption("What inspires you?", option2);
        greeting.AddOption("See you around.", option3);
        
        var dialogueTree = new DialogueTree(greeting);
        npc.AddDialogueTree("greeting", dialogueTree);
    }
}
