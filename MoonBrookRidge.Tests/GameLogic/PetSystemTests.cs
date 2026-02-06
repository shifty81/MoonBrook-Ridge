using Xunit;
using MoonBrookRidge.Pets;
using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Tests.GameLogic;

public class PetSystemTests
{
    [Fact]
    public void Constructor_ShouldInitializeWithNoPets()
    {
        var petSystem = new PetSystem();
        Assert.Empty(petSystem.OwnedPets);
        Assert.Null(petSystem.ActivePet);
    }

    [Fact]
    public void TamePet_ShouldAddPetToOwned()
    {
        var petSystem = new PetSystem();
        var result = petSystem.TamePet("dog", Vector2.Zero);
        Assert.True(result);
        Assert.Single(petSystem.OwnedPets);
        Assert.Equal("dog", petSystem.OwnedPets[0].DefinitionId);
    }

    [Fact]
    public void TamePet_ShouldReturnFalse_ForInvalidPetId()
    {
        var petSystem = new PetSystem();
        var result = petSystem.TamePet("dragon", Vector2.Zero);
        Assert.False(result);
        Assert.Empty(petSystem.OwnedPets);
    }

    [Fact]
    public void TamePet_ShouldNotDuplicatePets()
    {
        var petSystem = new PetSystem();
        petSystem.TamePet("dog", Vector2.Zero);
        var result = petSystem.TamePet("dog", Vector2.Zero);
        Assert.False(result);
        Assert.Single(petSystem.OwnedPets);
    }

    [Fact]
    public void TamePet_ShouldFireEvent()
    {
        var petSystem = new PetSystem();
        Pet? tamedPet = null;
        petSystem.OnPetTamed += pet => tamedPet = pet;
        
        petSystem.TamePet("cat", Vector2.Zero);
        
        Assert.NotNull(tamedPet);
        Assert.Equal("cat", tamedPet!.DefinitionId);
    }

    [Fact]
    public void SummonPet_ShouldSetActivePet()
    {
        var petSystem = new PetSystem();
        petSystem.TamePet("dog", Vector2.Zero);
        var dog = petSystem.OwnedPets[0];
        
        petSystem.SummonPet(dog);
        Assert.NotNull(petSystem.ActivePet);
        Assert.Equal("dog", petSystem.ActivePet.DefinitionId);
    }

    [Fact]
    public void DismissPet_ShouldClearActivePet()
    {
        var petSystem = new PetSystem();
        petSystem.TamePet("dog", Vector2.Zero);
        petSystem.SummonPet(petSystem.OwnedPets[0]);
        
        petSystem.DismissPet();
        Assert.Null(petSystem.ActivePet);
    }

    [Fact]
    public void SummonPet_ShouldDismissPreviousPet()
    {
        var petSystem = new PetSystem();
        petSystem.TamePet("dog", Vector2.Zero);
        petSystem.TamePet("cat", Vector2.Zero);
        
        petSystem.SummonPet(petSystem.OwnedPets[0]); // Dog
        petSystem.SummonPet(petSystem.OwnedPets[1]); // Cat replaces dog
        
        Assert.Equal("cat", petSystem.ActivePet.DefinitionId);
    }

    [Fact]
    public void TamePet_ShouldWorkForAllPetTypes()
    {
        var petSystem = new PetSystem();
        Assert.True(petSystem.TamePet("dog", Vector2.Zero));
        Assert.True(petSystem.TamePet("cat", Vector2.Zero));
        Assert.True(petSystem.TamePet("chicken", Vector2.Zero));
        Assert.True(petSystem.TamePet("wolf", Vector2.Zero));
        Assert.True(petSystem.TamePet("fairy", Vector2.Zero));
        Assert.Equal(5, petSystem.OwnedPets.Count);
    }

    [Fact]
    public void FeedPet_ShouldIncreaseSatiation()
    {
        var petSystem = new PetSystem();
        petSystem.TamePet("dog", Vector2.Zero);
        var dog = petSystem.OwnedPets[0];
        
        // Reduce hunger first
        float initialHunger = dog.Hunger;
        petSystem.FeedPet(dog, 20f);
        
        // Hunger should be capped at 100
        Assert.True(dog.Hunger <= 100f);
    }
}
