using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace MoonBrookRidge.Pets;

/// <summary>
/// Pet companion system for taming and managing pets
/// </summary>
public class PetSystem
{
    private List<Pet> _ownedPets;
    private Pet _activePet;
    private List<PetDefinition> _availablePets;
    
    public Pet ActivePet => _activePet;
    public List<Pet> OwnedPets => _ownedPets;
    
    public event Action<Pet> OnPetTamed;
    public event Action<Pet> OnPetSummoned;
    public event Action<Pet> OnPetDismissed;
    
    public PetSystem()
    {
        _ownedPets = new List<Pet>();
        _availablePets = new List<PetDefinition>();
        
        InitializePetDefinitions();
    }
    
    private void InitializePetDefinitions()
    {
        // Companion pets
        _availablePets.Add(new PetDefinition("dog", "Dog", PetType.Companion, 
            "A loyal companion", 50f, 1.2f, PetAbility.FindItems));
        _availablePets.Add(new PetDefinition("cat", "Cat", PetType.Companion, 
            "An independent feline", 40f, 1.5f, PetAbility.ScareAnimals));
        
        // Farm helpers
        _availablePets.Add(new PetDefinition("chicken", "Chicken", PetType.FarmHelper, 
            "Produces eggs daily", 30f, 0.8f, PetAbility.ProduceEggs));
        _availablePets.Add(new PetDefinition("sheep", "Sheep", PetType.FarmHelper, 
            "Produces wool weekly", 60f, 0.6f, PetAbility.ProduceWool));
        _availablePets.Add(new PetDefinition("cow", "Cow", PetType.FarmHelper, 
            "Produces milk daily", 80f, 0.5f, PetAbility.ProduceMilk));
        
        // Combat pets
        _availablePets.Add(new PetDefinition("wolf", "Tamed Wolf", PetType.Combat, 
            "A fierce protector", 100f, 1.3f, PetAbility.AttackEnemies, damage: 15f));
        _availablePets.Add(new PetDefinition("hawk", "Hunting Hawk", PetType.Combat, 
            "Swoops at enemies", 60f, 1.8f, PetAbility.AttackEnemies, damage: 10f));
        
        // Magical pets
        _availablePets.Add(new PetDefinition("fairy", "Fairy", PetType.Magical, 
            "Helps with magic", 50f, 1.5f, PetAbility.BoostMagic));
        _availablePets.Add(new PetDefinition("spirit", "Forest Spirit", PetType.Magical, 
            "Accelerates crop growth", 70f, 1.0f, PetAbility.GrowCrops));
        _availablePets.Add(new PetDefinition("phoenix", "Phoenix", PetType.Magical, 
            "Revives player once", 150f, 1.4f, PetAbility.Revive));
    }
    
    public void Update(GameTime gameTime)
    {
        if (_activePet != null)
        {
            _activePet.Update(gameTime);
        }
    }
    
    public bool TamePet(string petDefinitionId, Vector2 position)
    {
        var definition = _availablePets.Find(p => p.Id == petDefinitionId);
        if (definition == null)
        {
            return false;
        }
        
        // Check if already owned
        if (_ownedPets.Exists(p => p.DefinitionId == petDefinitionId))
        {
            return false;
        }
        
        // Create and add pet
        var pet = new Pet(definition, position);
        _ownedPets.Add(pet);
        OnPetTamed?.Invoke(pet);
        
        return true;
    }
    
    public void SummonPet(Pet pet)
    {
        if (_activePet != null)
        {
            DismissPet();
        }
        
        _activePet = pet;
        pet.IsActive = true;
        OnPetSummoned?.Invoke(pet);
    }
    
    public void DismissPet()
    {
        if (_activePet != null)
        {
            _activePet.IsActive = false;
            OnPetDismissed?.Invoke(_activePet);
            _activePet = null;
        }
    }
    
    public void FeedPet(Pet pet, float amount)
    {
        pet.Feed(amount);
    }
    
    public void InteractWithPet(Pet pet)
    {
        pet.Interact();
    }
    
    public List<PetDefinition> GetAvailablePets() => _availablePets;
}

/// <summary>
/// Pet definition template
/// </summary>
public class PetDefinition
{
    public string Id { get; }
    public string Name { get; }
    public PetType Type { get; }
    public string Description { get; }
    public float MaxHealth { get; }
    public float Speed { get; }
    public PetAbility Ability { get; }
    public float Damage { get; } // For combat pets
    
    public PetDefinition(string id, string name, PetType type, string description, 
        float maxHealth, float speed, PetAbility ability, float damage = 0f)
    {
        Id = id;
        Name = name;
        Type = type;
        Description = description;
        MaxHealth = maxHealth;
        Speed = speed;
        Ability = ability;
        Damage = damage;
    }
}

/// <summary>
/// Pet instance owned by player
/// </summary>
public class Pet
{
    public string DefinitionId { get; }
    public string Name { get; set; }
    public PetType Type { get; }
    public float Health { get; private set; }
    public float MaxHealth { get; private set; }
    public float Happiness { get; private set; }
    public float Hunger { get; private set; }
    public PetAbility Ability { get; }
    public float Damage { get; }
    public Vector2 Position { get; set; }
    public bool IsActive { get; set; }
    public int Level { get; private set; }
    public float Experience { get; private set; }
    
    private float _abilityoCooldown;
    private const float HUNGER_DECAY_RATE = 0.02f; // Per second
    private const float HAPPINESS_DECAY_RATE = 0.01f; // Per second
    
    public Pet(PetDefinition definition, Vector2 position)
    {
        DefinitionId = definition.Id;
        Name = definition.Name;
        Type = definition.Type;
        MaxHealth = definition.MaxHealth;
        Health = MaxHealth;
        Ability = definition.Ability;
        Damage = definition.Damage;
        Position = position;
        Happiness = 100f;
        Hunger = 100f;
        Level = 1;
        Experience = 0f;
        IsActive = false;
    }
    
    public void Update(GameTime gameTime)
    {
        if (!IsActive) return;
        
        float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
        
        // Decay hunger and happiness
        Hunger = MathHelper.Clamp(Hunger - (HUNGER_DECAY_RATE * deltaTime), 0, 100);
        Happiness = MathHelper.Clamp(Happiness - (HAPPINESS_DECAY_RATE * deltaTime), 0, 100);
        
        // Update ability cooldown
        if (_abilityoCooldown > 0)
        {
            _abilityoCooldown -= deltaTime;
        }
        
        // Low hunger affects happiness
        if (Hunger < 20f)
        {
            Happiness = MathHelper.Clamp(Happiness - (HAPPINESS_DECAY_RATE * 2f * deltaTime), 0, 100);
        }
    }
    
    public void Feed(float amount)
    {
        Hunger = MathHelper.Clamp(Hunger + amount, 0, 100);
        Happiness = MathHelper.Clamp(Happiness + (amount * 0.3f), 0, 100);
    }
    
    public void Interact()
    {
        // Petting/playing with pet
        Happiness = MathHelper.Clamp(Happiness + 10f, 0, 100);
    }
    
    public void TakeDamage(float amount)
    {
        Health = Math.Max(0, Health - amount);
    }
    
    public void Heal(float amount)
    {
        Health = Math.Min(MaxHealth, Health + amount);
    }
    
    public void AddExperience(float amount)
    {
        Experience += amount;
        
        // Check for level up
        float requiredXP = 100f * Level;
        while (Experience >= requiredXP && Level < 10)
        {
            Experience -= requiredXP;
            Level++;
            
            // Increase stats on level up
            MaxHealth *= 1.1f;
            Health = MaxHealth;
        }
    }
    
    public bool CanUseAbility()
    {
        return IsActive && _abilityoCooldown <= 0 && Happiness > 50f;
    }
    
    public void UseAbility()
    {
        if (CanUseAbility())
        {
            _abilityoCooldown = 60f; // 60 second cooldown
        }
    }
    
    public float GetEffectiveness()
    {
        // Pet effectiveness based on happiness and hunger
        float effectiveness = 1.0f;
        
        if (Happiness < 50f)
        {
            effectiveness *= 0.7f;
        }
        
        if (Hunger < 30f)
        {
            effectiveness *= 0.5f;
        }
        
        return effectiveness;
    }
}

public enum PetType
{
    Companion,    // Dogs, cats - follow player
    FarmHelper,   // Animals that produce resources
    Combat,       // Fight alongside player
    Magical       // Special magical abilities
}

public enum PetAbility
{
    None,
    FindItems,        // Helps find rare items
    ScareAnimals,     // Keeps wild animals away
    ProduceEggs,      // Chicken produces eggs
    ProduceWool,      // Sheep produces wool
    ProduceMilk,      // Cow produces milk
    AttackEnemies,    // Combat pet attacks
    BoostMagic,       // Increase magic power
    GrowCrops,        // Accelerate crop growth
    Revive            // Revive player on death
}
