using MoonBrookRidge.Engine.MonoGameCompat;

namespace MoonBrookRidge.Combat;

/// <summary>
/// Factory for creating predefined enemy types
/// </summary>
public static class EnemyFactory
{
    // ===== COMMON ENEMIES =====
    
    public static Enemy CreateSlime(Vector2 position)
    {
        var slime = new Enemy("slime", "Green Slime", EnemyType.Slime, 
            health: 20f, damage: 5f, defense: 0.1f, speed: 50f, experience: 5)
        {
            Position = position
        };
        slime.AddLoot("Slime", 0.8f, 1, 3);
        slime.AddLoot("Gel", 0.5f, 1, 2);
        return slime;
    }
    
    public static Enemy CreateBat(Vector2 position)
    {
        var bat = new Enemy("bat", "Cave Bat", EnemyType.Bat, 
            health: 15f, damage: 8f, defense: 0.05f, speed: 100f, experience: 7)
        {
            Position = position
        };
        bat.AddLoot("Bat Wing", 0.6f);
        bat.AddLoot("Leather", 0.3f);
        return bat;
    }
    
    public static Enemy CreateSkeleton(Vector2 position)
    {
        var skeleton = new Enemy("skeleton", "Skeleton", EnemyType.Skeleton, 
            health: 40f, damage: 12f, defense: 0.15f, speed: 60f, experience: 15)
        {
            Position = position
        };
        skeleton.AddLoot("Bone", 0.9f, 1, 3);
        skeleton.AddLoot("Skull", 0.3f);
        skeleton.AddLoot("Iron Sword", 0.05f);
        return skeleton;
    }
    
    public static Enemy CreateGoblin(Vector2 position)
    {
        var goblin = new Enemy("goblin", "Goblin", EnemyType.Goblin, 
            health: 35f, damage: 10f, defense: 0.2f, speed: 70f, experience: 12)
        {
            Position = position
        };
        goblin.AddLoot("Goblin Ear", 0.7f);
        goblin.AddLoot("Coin", 0.8f, 5, 15);
        goblin.AddLoot("Wooden Club", 0.1f);
        return goblin;
    }
    
    public static Enemy CreateSpider(Vector2 position)
    {
        var spider = new Enemy("spider", "Giant Spider", EnemyType.Spider, 
            health: 30f, damage: 14f, defense: 0.1f, speed: 80f, experience: 18)
        {
            Position = position
        };
        spider.AddLoot("Spider Silk", 0.9f, 1, 2);
        spider.AddLoot("Spider Fang", 0.4f);
        spider.AddLoot("Poison Gland", 0.2f);
        return spider;
    }
    
    public static Enemy CreateWolf(Vector2 position)
    {
        var wolf = new Enemy("wolf", "Wild Wolf", EnemyType.Wolf, 
            health: 45f, damage: 16f, defense: 0.15f, speed: 110f, experience: 20)
        {
            Position = position
        };
        wolf.AddLoot("Wolf Pelt", 0.7f);
        wolf.AddLoot("Wolf Fang", 0.5f);
        wolf.AddLoot("Raw Meat", 0.8f, 1, 2);
        return wolf;
    }
    
    // ===== UNCOMMON ENEMIES =====
    
    public static Enemy CreateGhost(Vector2 position)
    {
        var ghost = new Enemy("ghost", "Phantom", EnemyType.Ghost, 
            health: 50f, damage: 20f, defense: 0.4f, speed: 90f, experience: 35)
        {
            Position = position
        };
        ghost.AddLoot("Ectoplasm", 0.8f, 1, 2);
        ghost.AddLoot("Ghost Essence", 0.3f);
        ghost.AddLoot("Spirit Crystal", 0.1f);
        return ghost;
    }
    
    public static Enemy CreateZombie(Vector2 position)
    {
        var zombie = new Enemy("zombie", "Zombie", EnemyType.Zombie, 
            health: 60f, damage: 18f, defense: 0.25f, speed: 40f, experience: 30)
        {
            Position = position
        };
        zombie.AddLoot("Rotten Flesh", 0.9f, 1, 3);
        zombie.AddLoot("Bone", 0.6f, 1, 2);
        zombie.AddLoot("Brain", 0.2f);
        return zombie;
    }
    
    public static Enemy CreateOrc(Vector2 position)
    {
        var orc = new Enemy("orc", "Orc Warrior", EnemyType.Orc, 
            health: 80f, damage: 25f, defense: 0.3f, speed: 65f, experience: 45)
        {
            Position = position
        };
        orc.AddLoot("Orc Tusk", 0.7f);
        orc.AddLoot("Iron Ore", 0.5f, 2, 4);
        orc.AddLoot("Steel Sword", 0.15f);
        orc.AddLoot("Coin", 0.9f, 20, 40);
        return orc;
    }
    
    // ===== RARE/ELITE ENEMIES =====
    
    public static Enemy CreateFireElemental(Vector2 position)
    {
        var elemental = new Enemy("fire_elemental", "Fire Elemental", EnemyType.Elemental, 
            health: 100f, damage: 35f, defense: 0.35f, speed: 75f, experience: 70)
        {
            Position = position
        };
        elemental.AddLoot("Fire Core", 0.8f);
        elemental.AddLoot("Flame Crystal", 0.5f, 1, 2);
        elemental.AddLoot("Magic Essence", 0.4f);
        elemental.AddLoot("Fire Wand", 0.05f);
        return elemental;
    }
    
    public static Enemy CreateDemon(Vector2 position)
    {
        var demon = new Enemy("demon", "Lesser Demon", EnemyType.Demon, 
            health: 120f, damage: 40f, defense: 0.4f, speed: 85f, experience: 90)
        {
            Position = position
        };
        demon.AddLoot("Demon Horn", 0.9f);
        demon.AddLoot("Dark Essence", 0.7f, 1, 2);
        demon.AddLoot("Sulfur", 0.6f, 2, 4);
        demon.AddLoot("Magic Staff", 0.1f);
        return demon;
    }
    
    // ===== BOSS ENEMIES =====
    
    public static Enemy CreateSlimeKing(Vector2 position)
    {
        var king = new Enemy("slime_king", "Slime King", EnemyType.Slime, 
            health: 300f, damage: 30f, defense: 0.25f, speed: 30f, experience: 200, isBoss: true)
        {
            Position = position
        };
        king.AddLoot("Royal Jelly", 1.0f, 3, 5);
        king.AddLoot("Slime Crown", 1.0f);
        king.AddLoot("Coin", 1.0f, 100, 200);
        king.AddLoot("Golden Sword", 0.3f);
        return king;
    }
    
    public static Enemy CreateSkeletonLord(Vector2 position)
    {
        var lord = new Enemy("skeleton_lord", "Skeleton Lord", EnemyType.Skeleton, 
            health: 400f, damage: 45f, defense: 0.35f, speed: 70f, experience: 300, isBoss: true)
        {
            Position = position
        };
        lord.AddLoot("Ancient Bone", 1.0f, 5, 8);
        lord.AddLoot("Cursed Skull", 1.0f);
        lord.AddLoot("Necrotic Essence", 0.8f, 2, 4);
        lord.AddLoot("Steel Sword", 0.5f);
        lord.AddLoot("Coin", 1.0f, 150, 300);
        return lord;
    }
    
    public static Enemy CreateDragon(Vector2 position)
    {
        var dragon = new Enemy("dragon", "Ancient Dragon", EnemyType.Dragon, 
            health: 1000f, damage: 75f, defense: 0.5f, speed: 60f, experience: 1000, isBoss: true)
        {
            Position = position
        };
        dragon.AddLoot("Dragon Scale", 1.0f, 3, 6);
        dragon.AddLoot("Dragon Heart", 1.0f);
        dragon.AddLoot("Dragon Claw", 1.0f, 2, 4);
        dragon.AddLoot("Diamond", 0.8f, 2, 5);
        dragon.AddLoot("Arcane Staff", 0.4f);
        dragon.AddLoot("Coin", 1.0f, 500, 1000);
        return dragon;
    }
    
    public static Enemy CreateArchDemon(Vector2 position)
    {
        var archDemon = new Enemy("arch_demon", "Arch Demon", EnemyType.Demon, 
            health: 800f, damage: 65f, defense: 0.45f, speed: 90f, experience: 800, isBoss: true)
        {
            Position = position
        };
        archDemon.AddLoot("Demon Soul", 1.0f);
        archDemon.AddLoot("Infernal Horn", 1.0f, 2, 3);
        archDemon.AddLoot("Hellfire Essence", 1.0f, 3, 5);
        archDemon.AddLoot("Dark Crystal", 0.7f, 2, 4);
        archDemon.AddLoot("Fire Wand", 0.5f);
        archDemon.AddLoot("Coin", 1.0f, 400, 800);
        return archDemon;
    }
}
