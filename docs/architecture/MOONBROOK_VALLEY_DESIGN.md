# MoonBrook Valley - Auto-Shooter Roguelite Design Document

## Game Concept

**MoonBrook Valley** is an auto-shooter roguelite that combines the peaceful charm of village life simulation with intense underground combat. The quaint valley village hides a dark secret: beneath the idyllic farmlands lies a network of dangerous caves filled with relentless alien-like creatures. Only you know the truth, and it's up to you to venture into the depths and defend the unsuspecting villagers above.

## Core Game Loop

### Overworld (MoonBrook Valley Village)
The overworld is your **safe haven** where you:
- **Farm crops** for food and resources
- **Fish in rivers and ponds** for unique catches
- **Tame pets** that will accompany you into battle
- **Interact with villagers** and build relationships
- **Upgrade equipment** at the blacksmith
- **Purchase supplies** at the shop
- **Rest and prepare** for your next descent

**No Combat in Overworld**: The village and surrounding valley are peaceful. Combat only occurs in the caves below.

### Underground (Cave Systems)
The caves are **procedurally generated combat zones** where you:
- **Fight relentless waves** of enemies that spawn continuously
- **Mine resources** from cave walls and ore deposits
- **Collect XP and loot** from defeated enemies
- **Descend deeper floors** for better rewards and harder challenges
- **Discover biome-specific caves** with unique enemies and resources
- **Face escalating difficulty** the further you venture

## Auto-Combat System

### Automated Weapon System
Your weapons **automatically fire and reload** without player input:
- **Auto-Fire**: Weapons shoot at nearby enemies within range
- **Auto-Reload**: Weapons reload automatically when empty
- **No Manual Shooting**: Player never presses a "shoot" button
- **Weapon Management**: Player chooses which weapons to equip and when

### Player Agency
While weapons are automated, you control:
- **Movement**: Dodge enemies, position strategically (WASD controls)
- **Build Choices**: Select weapons, upgrades, and passive bonuses
- **Risk/Reward**: Decide whether to mine resources or focus on survival
- **Positioning**: Place yourself optimally for weapon coverage
- **Resource Collection**: Gather XP orbs and loot drops manually

### Weapon Firing Patterns
Different weapons have unique firing patterns:
- **Forward Firing**: Warthog, rifles, bows (shoots in direction player faces)
- **Backward Firing**: Sabata, rear guns (protects your back)
- **360° Firing**: Magic staff, elemental orbs (shoots all directions)
- **Targeted Firing**: Sniper weapons (prioritizes high-value targets)
- **Area of Effect**: Flamethrowers, shotguns (wide cone attacks)

### Weapon Synergy
Combine weapons for comprehensive coverage:
- **Example Build 1**: Warthog (forward) + Sabata (backward) = Full front/back coverage
- **Example Build 2**: Magic Staff (360°) + Crossbow (targeted) = Area clear + priority damage
- **Example Build 3**: Flamethrower (AoE) + Rifle (range) = Close + distant coverage

## Damage Type System

### Damage Types
Each weapon deals one or more damage types:

#### Kinetic (Physical)
- **Effect**: Pure damage, no special properties
- **Best Against**: Unarmored enemies
- **Weapons**: Rifles, bows, clubs, bullets

#### Fire (DoT)
- **Effect**: Burning damage over time
- **Best Against**: Clustered enemies, organic creatures
- **Weapons**: Flamethrower, fire wand, incendiary grenades

#### Cryo (Ice)
- **Effect**: Slows and eventually freezes enemies
- **Best Against**: Fast enemies, creating defensive zones
- **Weapons**: Ice staff, cryo grenades, frost bow

#### Electric (Lightning)
- **Effect**: Critical hit chance buff, chains to nearby enemies
- **Best Against**: Groups, metal enemies
- **Weapons**: Lightning gun, shock wand, tesla coils

#### Acid (Corrosive)
- **Effect**: Armor reduction, damage amplification buff
- **Best Against**: Armored enemies, bosses
- **Weapons**: Acid sprayer, poison dagger, toxin grenades

### Elemental Combos
Combining damage types creates powerful effects:
- **Fire + Electric** = Explosion damage
- **Cryo + Kinetic** = Shatter (bonus damage to frozen enemies)
- **Acid + Fire** = Toxic burn (DoT stacks faster)
- **Electric + Water** = Chain lightning (increased range)

## Pet Companion System

### Pet Role
Your **pet companion** replaces traditional support drones:
- **Automated Support**: Pets act independently, no micromanagement
- **Combat Assistance**: Pets attack enemies automatically
- **Buff Provision**: Pets provide passive bonuses based on type
- **Healing/Support**: Some pets heal over time or provide shields

### Pet Types & Abilities

#### Combat Pets
- **Wolf**: High damage, melee attacks, aggro draw
- **Hawk**: Fast, ranged dive attacks, enemy tagging
- **Bear**: Tank, high HP, knockback attacks

#### Support Pets
- **Fairy**: HP regeneration over time, mana restoration
- **Spirit**: Damage buffs, elemental synergy bonuses
- **Phoenix**: Revive ability (once per descent), fire immunity

#### Utility Pets
- **Dog**: Finds hidden items, increased loot drops
- **Cat**: Enemy detection, critical hit chance boost
- **Owl**: Reveals map, enemy health bars

### Pet Progression

#### Leveling System
Pets gain XP alongside the player:
- **Level 1-10**: Basic abilities
- **Level 11-20**: Improved stats, secondary ability
- **Level 21-30**: Advanced abilities, special attacks
- **Level 30+**: Ultimate abilities, mastery bonuses

#### Skill Trees
Each pet has a unique skill tree with 3 branches:
- **Offensive Branch**: Damage, attack speed, critical hits
- **Defensive Branch**: HP, armor, dodge chance
- **Utility Branch**: Resource bonuses, XP gain, special abilities

Players allocate skill points earned from leveling and mastery runs.

#### Pet Charms (Equipment)
Pets can equip charms that enhance their capabilities:
- **Damage Charms**: +X% attack damage, elemental damage
- **Defense Charms**: +X HP, damage reduction, shields
- **Utility Charms**: +X% loot chance, XP gain, movement speed
- **Synergy Charms**: Bonuses that match player weapon types

**Example Charm**: "Flame Collar" - Pet attacks deal fire damage, +20% fire damage synergy with player's fire weapons

### Pet Taming
Pets can be found and tamed in the overworld:
- **Wild Encounters**: Find wild pets in different biomes
- **Taming Process**: Feed specific items, build trust (mini-game)
- **Exclusive Pets**: Some pets only available in caves or special events
- **Breeding**: Breed pets to create offspring with mixed traits (late-game)

## Grenade System

Grenades are **manually triggered** AoE weapons with cooldowns:

### Grenade Types

#### Explosive Grenade
- **Effect**: High damage in large radius
- **Best Use**: Dense enemy groups
- **Cooldown**: 30 seconds

#### Cryo Grenade
- **Effect**: Freezes all enemies in area
- **Best Use**: Crowd control, defensive breathing room
- **Cooldown**: 45 seconds

#### H-Grenade (Mining)
- **Effect**: Destroys cave walls, reveals hidden areas
- **Best Use**: Resource gathering, secret passages
- **Cooldown**: 60 seconds

#### Neurotoxin Grenade
- **Effect**: Damage over time cloud, reduces enemy damage
- **Best Use**: Area denial, boss fights
- **Cooldown**: 40 seconds

#### Electric Grenade
- **Effect**: Chain lightning, stuns enemies
- **Best Use**: Mixed groups, stopping charges
- **Cooldown**: 35 seconds

## Progression Systems

### Mastery System
Complete **challenge runs** with specific restrictions to earn permanent bonuses:
- **Challenge Examples**: 
  - No kinetic weapons run
  - Solo (no pet) run
  - Speedrun under X minutes
  - No damage taken run
  - Single weapon type run

**Rewards**:
- Skill points for character progression
- Unlockable weapons and gear
- New pet types
- Cosmetic rewards

### Gear Upgrades
Earn and upgrade equipment through:
- **Blacksmith**: Upgrade weapons with materials
- **Enchanter**: Add elemental properties to weapons
- **Armorer**: Improve armor for damage reduction
- **Jeweler**: Craft charms for pets

### Passive Bonuses (Perks)
Unlock permanent passive bonuses:
- **Movement Speed**: +5% per level (max 25%)
- **Damage**: +3% all damage per level (max 15%)
- **HP/Energy**: +10 max HP or energy per level
- **Resource Gain**: +5% XP, gold, loot chance per level
- **Weapon Slots**: Unlock additional weapon slots (start with 2, max 4)

## Cave & Biome System

### Biome Villages
Each biome has its own village with unique NPCs, culture, and quest offerings:

#### MoonBrook Valley (Home Village)
- **Theme**: Farming community, your starting hub
- **NPCs**: General merchants, blacksmith, innkeeper, quest board
- **Quests**: Introductory quests, basic cave exploration, farming tutorials
- **Specialty**: General goods, starter equipment

#### Pinewood Village (Forest Biome)
- **Theme**: Lumberjack and hunter community
- **NPCs**: Master carpenter, hunter's guild, herbalist
- **Quests**: 
  - **Cave Quests**: Clear forest caves of beasts, gather rare mushrooms
  - **Farm Quests**: Grow specific herbs, raise chickens
  - **Trading Quests**: Deliver wood to other villages
  - **Courier Quests**: Transport letters to MoonBrook Valley
- **Specialty**: Wood crafting, hunting supplies, nature magic

#### Stonehelm Village (Mountain Biome)
- **Theme**: Dwarven mining settlement
- **NPCs**: Master blacksmith, ore merchant, mining equipment specialist
- **Quests**: 
  - **Cave Quests**: Mine rare ores, defeat rock elementals
  - **Farm Quests**: Grow crops in rocky soil (challenge)
  - **Trading Quests**: Deliver refined metals
  - **Courier Quests**: Transport heavy mining equipment between villages
- **Specialty**: Weapons, armor, mining tools, ore refinement

#### Sandshore Village (Desert Biome)
- **Theme**: Trading outpost, exotic goods
- **NPCs**: Exotic merchants, fire mage, treasure hunter
- **Quests**: 
  - **Cave Quests**: Explore fire caves, find ancient treasures
  - **Farm Quests**: Grow desert plants (cacti, dates)
  - **Trading Quests**: Deliver spices and rare goods
  - **Courier Quests**: Trade route establishment
- **Specialty**: Fire weapons, heat-resistant armor, exotic foods

#### Frostpeak Village (Frozen Biome)
- **Theme**: Ice fishing and arctic survival community
- **NPCs**: Ice mage, cold-weather outfitter, fisher's guild
- **Quests**: 
  - **Cave Quests**: Clear ice caves, hunt frost creatures
  - **Farm Quests**: Grow cold-hardy crops
  - **Trading Quests**: Deliver preserved fish
  - **Courier Quests**: Supply runs through dangerous cold
- **Specialty**: Cryo weapons, cold resistance gear, ice fishing

#### Marshwood Village (Toxic Swamp Biome)
- **Theme**: Alchemist settlement, potion makers
- **NPCs**: Master alchemist, poison expert, healer
- **Quests**: 
  - **Cave Quests**: Gather toxic materials, study creatures
  - **Farm Quests**: Grow alchemical ingredients
  - **Trading Quests**: Deliver potions and antidotes
  - **Courier Quests**: Transport dangerous alchemical materials
- **Specialty**: Acid weapons, potions, antidotes, alchemical gear

#### Crystalgrove Village (Crystal Cave Biome)
- **Theme**: Magical academy, arcane researchers
- **NPCs**: Archmage, enchanter, magical creature tamer
- **Quests**: 
  - **Cave Quests**: Study magical phenomena, collect crystals
  - **Farm Quests**: Grow magical plants
  - **Trading Quests**: Deliver enchanted items
  - **Courier Quests**: Transport volatile magical reagents
- **Specialty**: Magic weapons, enchantments, mana items, pet charms

#### Ruinwatch Village (Ancient Ruins Biome)
- **Theme**: Archaeological expedition camp
- **NPCs**: Lead archaeologist, lore keeper, relic expert
- **Quests**: 
  - **Cave Quests**: Explore ruins, recover artifacts, defeat guardians
  - **Farm Quests**: Grow ancient seed varieties
  - **Trading Quests**: Deliver recovered artifacts to museums
  - **Courier Quests**: Transport fragile relics between scholars
- **Specialty**: Legendary weapons, ancient knowledge, unique pets

### Quest System Integration

#### Quest Types

**Cave Quests** (Combat & Exploration)
- **Extermination**: Clear X number of enemies in specific cave
- **Boss Hunting**: Defeat a specific boss enemy
- **Resource Gathering**: Mine X amount of specific ore/material
- **Exploration**: Reach floor X in specific biome cave
- **Rescue**: Find and rescue lost NPCs in caves
- **Discovery**: Find hidden rooms or secret areas

**Farm Quests** (Production & Management)
- **Crop Delivery**: Grow and deliver X amount of specific crop
- **Animal Product**: Produce X eggs/milk/wool from farm animals
- **Quality Challenge**: Deliver gold-star quality items
- **Seasonal**: Grow specific seasonal crops
- **Experimentation**: Try growing new crop varieties
- **Supply**: Keep villages stocked with food

**Trading Quests** (Commerce)
- **Buy Orders**: Purchase specific items and deliver to quest giver
- **Sell Orders**: Sell your items to NPC buyers
- **Arbitrage**: Buy low in one village, sell high in another
- **Rare Item Hunt**: Find and deliver rare/legendary items
- **Bulk Orders**: Deliver large quantities of common items
- **Trade Route**: Establish permanent trading between villages

**Courier Quests** (Delivery & Transport)
- **Package Delivery**: Transport items between villages
- **Urgent Mail**: Time-limited deliveries
- **Fragile Cargo**: Deliver items without taking damage
- **Multi-Stop**: Deliver to multiple villages in sequence
- **Dangerous Route**: Deliver through hazardous terrain
- **VIP Escort**: Accompany NPCs traveling between villages

**Hybrid Quests** (Multiple Types)
- Example: "Gather fire crystals from Desert Caves, then craft fire swords and deliver to Stonehelm Village"
- Example: "Grow 20 wheat on your farm, deliver 10 to Pinewood Village, sell 10 in Crystalgrove"

### Village Reputation System

Each village tracks reputation separately:
- **Reputation Levels**: Stranger → Acquaintance → Friend → Trusted → Hero → Legend
- **Benefits per Level**:
  - **Stranger**: Basic quests, standard prices
  - **Acquaintance**: More quests, 5% discount
  - **Friend**: Special quests, 10% discount, unlock recipes
  - **Trusted**: Rare quests, 15% discount, unique equipment
  - **Hero**: Epic quests, 20% discount, legendary items
  - **Legend**: Exclusive quests, 25% discount, village-specific mount/pet

### Inter-Village Relationships

Villages have relationships with each other:
- **Allied Villages**: Quests that benefit both, shared resources
- **Rival Villages**: Competing trade quests, choose sides
- **Neutral Villages**: Independent trade, no politics
- **Conflict Events**: Temporary tensions that create unique quests

**Example**: Stonehelm and Sandshore compete for ore trade routes. Players can:
- Support Stonehelm: Better weapon prices, more mining quests
- Support Sandshore: Better fire weapon prices, more treasure quests
- Stay neutral: Balanced benefits, unique mediator quests

### Cave Entrances
Explore the overworld to discover cave entrances near each village:
- **Forest Caves** (near Pinewood): Basic enemies, common resources, beast-type creatures
- **Mountain Caves** (near Stonehelm): Rocky terrain, mineral-rich, elemental enemies
- **Desert Caves** (near Sandshore): Fire enemies, rare fire gems, scorpion-type enemies
- **Frozen Caves** (near Frostpeak): Ice enemies, cryo weapons, yeti-type enemies
- **Toxic Swamp Caves** (near Marshwood): Acid enemies, poison resources, plant-type enemies
- **Crystal Caves** (near Crystalgrove): High-value gems, magical enemies, golem-type enemies
- **Ancient Ruins Caves** (near Ruinwatch): Bosses, legendary loot, guardian-type enemies

### Procedural Generation
Each cave descent is unique:
- **Random Layouts**: Rooms, corridors, caverns generated randomly
- **Enemy Waves**: Spawn patterns vary each run
- **Resource Placement**: Ore deposits in different locations
- **Hidden Secrets**: Random secret rooms with bonus rewards

### Hazard Levels (Difficulty)
Choose difficulty before descending:
- **Hazard 1**: Normal difficulty, 1x rewards
- **Hazard 2**: +30% enemy stats, 1.5x rewards
- **Hazard 3**: +60% enemy stats, 2x rewards
- **Hazard 4**: +100% enemy stats, 2.5x rewards
- **Hazard 5**: +150% enemy stats, 3x rewards

Higher hazards require refined builds and better gear.

### Floor Progression
Descend deeper for escalating difficulty:
- **Floors 1-5**: Basic enemy types, common loot
- **Floors 6-10**: Elite enemies, uncommon loot
- **Floors 11-15**: Champion enemies, rare loot
- **Floors 16-20**: Boss encounters, legendary loot
- **Floors 21+**: Endless mode, scaling difficulty, unique rewards

## Key Differences from Other Games

### vs. Traditional Farming Sims
- **Combat Exclusive to Caves**: Overworld is peaceful, unlike games with combat everywhere
- **Auto-Combat Focus**: Strategic build choices rather than twitch reactions
- **Roguelite Elements**: Each cave run is unique and challenging

### vs. Traditional Auto-Shooters
- **Farming & Life Sim Overworld**: Not just a combat game, peaceful activities available
- **Pet Companions**: Unique pet system with deep customization
- **Mining Integration**: Combat and resource gathering intertwined

### vs. Deep Rock Galactic
- **No Bosco**: Replaced with customizable pet companions
- **Overworld Hub**: Full village life sim, not just a space station
- **Permanent Progression**: Mastery system for permanent bonuses
- **Story Focus**: Narrative about defending the valley from underground threat

## Story & Theme

### The Secret of MoonBrook Valley

The quaint village of **MoonBrook Valley** sits atop a beautiful countryside, surrounded by lush forests, rolling hills, and crystal-clear rivers. The villagers live peaceful lives, farming the fertile land and fishing the abundant waters. They have no idea of the danger lurking beneath their feet.

**You** are the only one who knows the truth. Deep below the valley—and indeed, beneath every settlement in the region—an ancient evil stirs. Alien-like creatures, insectoid horrors, are multiplying in the darkness. If left unchecked, they will eventually swarm to the surface and destroy everything.

The valley region is home to **eight primary villages**, each settled in a different biome with its own culture, resources, and way of life. The villagers trade with each other, help each other, and live in harmony. But none of them know about the threat beneath their feet. While other smaller settlements dot the landscape, these eight villages are the major hubs where the underground threat is most concentrated.

Every day, you maintain the facade of a simple farmer and traveler, moving between villages, accepting their quests, and helping with their daily needs. But you have a secret mission: **explore the caves beneath each major village and push back the horde**. Each village's caves are connected to the same underground network, and the creatures grow stronger the deeper you go.

The villagers must never know the full truth—if they discovered the scale of the threat, panic would consume them. Instead, you accept their innocent requests for cave materials and rare resources, using these quests as cover for your true purpose: **defending all the villages of MoonBrook Valley**, one descent at a time.

### The Village Network

As you gain reputation across the eight villages, you'll uncover a larger conspiracy:
- Why do the caves beneath each biome connect to the same network?
- What ancient civilization built the ruins deep below?
- Are the creatures natural, or were they created?
- Do some villagers know more than they're letting on?

Your journey will take you across the entire region, from the sunny fields of MoonBrook Valley to the frozen peaks of Frostpeak Village, from the mystical Crystalgrove to the mysterious Ruinwatch. Each village needs you, even if they don't know why.

### Narrative Progression
As you descend deeper:
- **Discover Ancient Ruins**: Uncover the history of a fallen civilization
- **Learn the Origin**: Find out what created these creatures
- **Encounter the Source**: Face the ultimate threat deep below
- **Make a Choice**: Seal the caves forever, or find another solution?

## Controls & UI

### Movement & Combat
- **WASD**: Move character
- **Shift**: Run (drains energy)
- **Space**: Use equipped grenade
- **1-4**: Switch active weapon set
- **Tab**: Toggle between weapons in set

### Overworld Actions
- **E**: Interact (talk, mine, fish, etc.)
- **I**: Open inventory
- **P**: Open pet menu
- **M**: Open map
- **Esc**: Pause menu

### Cave-Specific
- **F**: Quick-mine (auto-mines nearest resource)
- **R**: Return to surface (teleport, only from safe rooms)
- **Q**: Use health potion
- **T**: Call pet to specific location

### UI Elements (In Cave)
- **Health Bar**: Top-left
- **Energy Bar**: Below health
- **XP Bar**: Bottom of screen
- **Weapon Display**: Right side (active weapons, ammo, reload status)
- **Pet Status**: Left side (pet HP, ability cooldowns)
- **Minimap**: Top-right corner
- **Enemy Counter**: Bottom-left (waves remaining)
- **Floor Indicator**: Top-center ("Floor 5 - Forest Cave")

## Technical Implementation Notes

### Quest System Architecture
```csharp
public enum QuestType
{
    Cave,           // Combat and exploration in caves
    Farm,           // Production on player's farm
    Trading,        // Buy/sell/arbitrage
    Courier,        // Delivery between villages
    Hybrid          // Multiple quest types combined
}

public enum QuestObjectiveType
{
    // Cave objectives
    KillEnemies,
    DefeatBoss,
    GatherResource,
    ReachFloor,
    RescueNPC,
    DiscoverSecret,
    
    // Farm objectives
    GrowCrop,
    ProduceAnimalProduct,
    CraftItem,
    ReachQuality,
    
    // Trading objectives
    BuyItem,
    SellItem,
    DeliverItem,
    
    // Courier objectives
    DeliverPackage,
    EscortNPC,
    
    // Hybrid
    MultiStep
}

public class Quest
{
    public string Id { get; set; }
    public string Title { get; set; }
    public string Description { get; set; }
    public QuestType Type { get; set; }
    public string VillageId { get; set; } // Which village offers this quest
    public int RequiredReputation { get; set; } // Minimum reputation to unlock
    
    public List<QuestObjective> Objectives { get; set; }
    public QuestReward Reward { get; set; }
    
    public bool IsComplete => Objectives.All(o => o.IsComplete);
}

public class QuestObjective
{
    public QuestObjectiveType Type { get; set; }
    public string TargetId { get; set; } // Enemy ID, item ID, village ID, etc.
    public int CurrentCount { get; set; }
    public int RequiredCount { get; set; }
    public string LocationRequirement { get; set; } // e.g., "desert_cave", "pinewood_village"
    
    public bool IsComplete => CurrentCount >= RequiredCount;
    
    public float Progress => (float)CurrentCount / RequiredCount;
}

public class QuestReward
{
    public int Gold { get; set; }
    public int Reputation { get; set; }
    public List<ItemReward> Items { get; set; }
    public int SkillPoints { get; set; }
    public string UnlockRecipe { get; set; }
}

public class Village
{
    public string Id { get; set; }
    public string Name { get; set; }
    public BiomeType Biome { get; set; }
    public Vector2 Position { get; set; }
    
    public int PlayerReputation { get; set; }
    public ReputationLevel ReputationLevel { get; set; }
    
    public List<NPC> Villagers { get; set; }
    public List<Quest> AvailableQuests { get; set; }
    public List<string> AlliedVillages { get; set; }
    public List<string> RivalVillages { get; set; }
    
    public float GetPriceModifier()
    {
        // Reputation affects prices
        return ReputationLevel switch
        {
            ReputationLevel.Stranger => 1.0f,
            ReputationLevel.Acquaintance => 0.95f,
            ReputationLevel.Friend => 0.90f,
            ReputationLevel.Trusted => 0.85f,
            ReputationLevel.Hero => 0.80f,
            ReputationLevel.Legend => 0.75f,
            _ => 1.0f
        };
    }
}

public enum ReputationLevel
{
    Stranger = 0,      // 0-99 rep
    Acquaintance = 1,  // 100-299 rep
    Friend = 2,        // 300-599 rep
    Trusted = 3,       // 600-999 rep
    Hero = 4,          // 1000-1499 rep
    Legend = 5         // 1500+ rep
}

public class VillageQuestSystem
{
    private List<Village> _villages;
    private List<Quest> _activeQuests;
    private List<Quest> _completedQuests;
    
    public void GenerateQuestsForVillage(Village village)
    {
        // Generate quests based on village biome and player reputation
        var questPool = GetAvailableQuestsForVillage(village);
        
        // Filter by reputation requirement
        var eligibleQuests = questPool.Where(q => 
            village.PlayerReputation >= q.RequiredReputation).ToList();
        
        // Select 3-5 random quests
        var selectedQuests = SelectRandomQuests(eligibleQuests, 3, 5);
        
        village.AvailableQuests = selectedQuests;
    }
    
    public void UpdateQuestProgress(string questId, QuestObjectiveType objectiveType, 
        string targetId, int amount = 1)
    {
        var quest = _activeQuests.FirstOrDefault(q => q.Id == questId);
        if (quest == null) return;
        
        foreach (var objective in quest.Objectives)
        {
            if (objective.Type == objectiveType && 
                (objective.TargetId == targetId || objective.TargetId == "any"))
            {
                objective.CurrentCount += amount;
            }
        }
        
        // Check if quest complete
        if (quest.IsComplete)
        {
            CompleteQuest(quest);
        }
    }
    
    private void CompleteQuest(Quest quest)
    {
        // Award rewards
        var village = _villages.First(v => v.Id == quest.VillageId);
        village.PlayerReputation += quest.Reward.Reputation;
        
        // Update reputation level
        UpdateReputationLevel(village);
        
        // Give rewards to player
        AwardQuestRewards(quest.Reward);
        
        // Move to completed
        _activeQuests.Remove(quest);
        _completedQuests.Add(quest);
    }
}
```

### Village Network System
```csharp
public class VillageNetworkManager
{
    private List<Village> _villages;
    private Dictionary<string, List<string>> _tradeRoutes;
    
    public void InitializeVillages()
    {
        _villages = new List<Village>
        {
            new Village("moonbrook", "MoonBrook Valley", BiomeType.Grassland, new Vector2(1000, 1000)),
            new Village("pinewood", "Pinewood Village", BiomeType.Forest, new Vector2(2000, 800)),
            new Village("stonehelm", "Stonehelm Village", BiomeType.Mountain, new Vector2(2500, 600)),
            new Village("sandshore", "Sandshore Village", BiomeType.Desert, new Vector2(3000, 1500)),
            new Village("frostpeak", "Frostpeak Village", BiomeType.Frozen, new Vector2(1500, 200)),
            new Village("marshwood", "Marshwood Village", BiomeType.Swamp, new Vector2(500, 1800)),
            new Village("crystalgrove", "Crystalgrove Village", BiomeType.Crystal, new Vector2(1800, 1600)),
            new Village("ruinwatch", "Ruinwatch Village", BiomeType.Ruins, new Vector2(2800, 1200))
        };
        
        // Set up alliances and rivalries
        SetupVillageRelations();
    }
    
    private void SetupVillageRelations()
    {
        // Example: Stonehelm and Sandshore are rivals (ore trade competition)
        var stonehelm = _villages.First(v => v.Id == "stonehelm");
        var sandshore = _villages.First(v => v.Id == "sandshore");
        stonehelm.RivalVillages.Add("sandshore");
        sandshore.RivalVillages.Add("stonehelm");
        
        // Pinewood and Marshwood are allies (nature-based)
        var pinewood = _villages.First(v => v.Id == "pinewood");
        var marshwood = _villages.First(v => v.Id == "marshwood");
        pinewood.AlliedVillages.Add("marshwood");
        marshwood.AlliedVillages.Add("pinewood");
    }
    
    public List<Quest> GetCourierQuests(Village fromVillage)
    {
        var courierQuests = new List<Quest>();
        
        // Generate delivery quests to other villages
        foreach (var targetVillage in _villages.Where(v => v.Id != fromVillage.Id))
        {
            var distance = Vector2.Distance(fromVillage.Position, targetVillage.Position);
            var reward = (int)(distance * 0.5f); // Reward based on distance
            
            courierQuests.Add(new Quest
            {
                Id = $"courier_{fromVillage.Id}_to_{targetVillage.Id}",
                Title = $"Delivery to {targetVillage.Name}",
                Description = $"Deliver a package from {fromVillage.Name} to {targetVillage.Name}",
                Type = QuestType.Courier,
                VillageId = fromVillage.Id,
                Objectives = new List<QuestObjective>
                {
                    new QuestObjective
                    {
                        Type = QuestObjectiveType.DeliverPackage,
                        TargetId = targetVillage.Id,
                        RequiredCount = 1
                    }
                },
                Reward = new QuestReward
                {
                    Gold = reward,
                    Reputation = 10
                }
            });
        }
        
        return courierQuests;
    }
}
```

### Auto-Combat System Architecture
```csharp
public class AutoWeaponSystem
{
    private List<AutoWeapon> _equippedWeapons;
    private float _globalCooldown = 0.1f;
    
    public void Update(GameTime gameTime, List<Enemy> enemies, Vector2 playerPosition)
    {
        foreach (var weapon in _equippedWeapons)
        {
            if (weapon.CanFire() && !weapon.IsReloading())
            {
                var target = FindTarget(weapon, enemies, playerPosition);
                if (target != null)
                {
                    weapon.Fire(target);
                }
            }
            
            weapon.Update(gameTime);
        }
    }
    
    private Enemy FindTarget(AutoWeapon weapon, List<Enemy> enemies, Vector2 playerPosition)
    {
        // Weapon-specific targeting logic
        // Priority: closest, lowest HP, highest threat, etc.
    }
}
```

### Pet System Integration
```csharp
public class PetCompanion
{
    public PetType Type { get; set; }
    public int Level { get; set; }
    public SkillTree Skills { get; set; }
    public List<PetCharm> EquippedCharms { get; set; }
    
    public void Update(GameTime gameTime, Vector2 playerPosition, List<Enemy> enemies)
    {
        // Follow player
        UpdatePosition(playerPosition);
        
        // Auto-attack
        if (CanAttack())
        {
            var target = FindNearestEnemy(enemies);
            if (target != null)
            {
                Attack(target);
            }
        }
        
        // Apply passive buffs
        ApplyBuffs();
    }
}
```

### Damage Type System
```csharp
public enum DamageType
{
    Kinetic,
    Fire,
    Cryo,
    Electric,
    Acid
}

public class Damage
{
    public float Amount { get; set; }
    public DamageType Type { get; set; }
    
    public void ApplyToEnemy(Enemy enemy)
    {
        // Base damage
        enemy.TakeDamage(Amount);
        
        // Apply special effects
        switch (Type)
        {
            case DamageType.Fire:
                enemy.ApplyBurning(Amount * 0.2f, 3f); // 20% damage over 3 seconds
                break;
            case DamageType.Cryo:
                enemy.ApplySlow(0.5f, 2f); // 50% slow for 2 seconds
                break;
            case DamageType.Electric:
                enemy.ApplyStun(0.5f); // 0.5 second stun
                ChainToNearby(enemy, Amount * 0.5f);
                break;
            case DamageType.Acid:
                enemy.ApplyArmorReduction(0.25f, 5f); // 25% armor reduction for 5 seconds
                break;
        }
    }
}
```

## Development Roadmap

### Phase 1: Auto-Combat Foundation (Week 1-2)
- [ ] Implement AutoWeaponSystem class
- [ ] Create weapon auto-fire logic
- [ ] Add weapon auto-reload mechanics
- [ ] Implement firing pattern system (forward, backward, 360°)

### Phase 2: Pet Companion System (Week 3-4)
- [ ] Enhance PetSystem with combat abilities
- [ ] Implement pet leveling and XP system
- [ ] Create pet skill tree UI
- [ ] Add pet charm/equipment system
- [ ] Implement pet AI (follow, attack, support)

### Phase 3: Cave Combat Zones (Week 5-6)
- [ ] Update dungeon system for wave-based spawning
- [ ] Implement floor progression system
- [ ] Add biome-specific caves
- [ ] Create cave entrance system in overworld
- [ ] Restrict enemy spawns to caves only

### Phase 4: Multi-Village System (Week 7-8) ⭐ NEW
- [ ] Implement Village class and VillageNetworkManager
- [ ] Create 8 biome villages with unique NPCs
- [ ] Implement reputation system per village
- [ ] Create village-to-village relationships (allies, rivals)
- [ ] Add village-specific shops and services
- [ ] Implement fast travel between discovered villages

### Phase 5: Quest System (Week 9-10) ⭐ NEW
- [ ] Implement Quest and QuestObjective classes
- [ ] Create VillageQuestSystem for quest generation
- [ ] Add cave quest types (extermination, boss hunting, resource gathering)
- [ ] Add farm quest types (crop delivery, animal products)
- [ ] Add trading quest types (buy orders, sell orders, arbitrage)
- [ ] Add courier quest types (package delivery, escort missions)
- [ ] Implement quest progress tracking and completion

### Phase 6: Damage & Grenade Systems (Week 11-12)
- [ ] Implement damage type system (kinetic, fire, cryo, electric, acid)
- [ ] Add elemental status effects
- [ ] Create grenade system with manual triggering
- [ ] Implement grenade types (explosive, cryo, H-grenade, neurotoxin, electric)

### Phase 7: Progression & Mastery (Week 13-14)
- [ ] Create mastery/challenge run system
- [ ] Implement skill point rewards
- [ ] Add passive bonus system
- [ ] Create hazard difficulty selection
- [ ] Implement gear upgrade systems
- [ ] Add reputation rewards (discounts, unique items)

### Phase 8: Polish & Balance (Week 15-16)
- [ ] Balance weapon auto-fire rates
- [ ] Tune enemy spawn rates and difficulty
- [ ] Balance pet abilities
- [ ] Polish UI for auto-combat feedback
- [ ] Add tutorial for new mechanics
- [ ] Balance quest rewards and reputation gains
- [ ] Test inter-village economy and quest flow
- [ ] Extensive playtesting and iteration

## Success Metrics

### Player Engagement
- **Average Cave Run Duration**: 15-30 minutes
- **Average Depth Reached**: Floors 8-12 for new players
- **Return Rate**: 70%+ players return for multiple runs
- **Pet Usage**: 90%+ players actively use pets

### Game Balance
- **Weapon Diversity**: No single weapon >30% usage rate
- **Pet Diversity**: No single pet >25% usage rate
- **Difficulty Curve**: 40% completion rate at Hazard 3
- **Progression Feel**: Players reach endgame in 20-30 hours

### Fun Factor
- **"Just One More Run"**: High replayability from procedural generation
- **Build Variety**: Hundreds of viable weapon/pet/perk combinations
- **Strategic Depth**: Positioning and build choices matter more than reflexes
- **Satisfying Feedback**: Auto-combat feels powerful and responsive

---

**Document Status**: Design Complete  
**Last Updated**: January 2026  
**Owner**: MoonBrook Ridge Development Team
