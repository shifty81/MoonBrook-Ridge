# MoonBrook Valley - Game Transformation Summary

## Overview

This document summarizes the transformation of MoonBrook Ridge into **MoonBrook Valley**, a unique auto-shooter roguelite that combines peaceful village life simulation with intense underground combat.

## Vision Statement

**MoonBrook Valley** is an auto-shooter roguelite where you defend a peaceful valley from underground threats. By day, you're a farmer tending crops, fishing, and helping villagers across 8 biome-based settlements. By night, you descend into procedurally-generated caves where your weapons automatically fire at endless waves of alien-like creatures, while your pet companion fights by your side.

## Core Game Loop

### Overworld (Peaceful)
**Activities**: Farming, fishing, pet taming, village trading, relationship building
**Location**: 8 biome villages across the valley region
**Gameplay**: Traditional farming sim mechanics, no combat

### Underground (Combat)
**Activities**: Auto-combat, resource mining, loot collection, floor progression
**Location**: Procedurally-generated caves beneath each biome
**Gameplay**: Auto-shooter mechanics where weapons fire automatically

## Key Design Principles

### 1. Clear Separation of Activities
- **Overworld = Safe**: No combat, focus on peaceful activities
- **Caves = Dangerous**: Pure combat focus with auto-shooter mechanics
- **No Mixing**: Combat never bleeds into overworld

### 2. Auto-Shooter Mechanics
- **Automatic Firing**: Equipped weapons shoot at enemies automatically
- **Automatic Reloading**: Weapons reload themselves
- **Player Focus**: Positioning, dodging, build choices, not aiming
- **Strategic Depth**: Weapon synergies, damage types, pet companions

### 3. Multi-Village Network
- **8 Unique Villages**: Each biome has its own settlement and culture
- **Reputation System**: Build standing with each village independently
- **Quest Variety**: Cave quests, farm quests, trading quests, courier quests
- **Inter-Village Relations**: Allies, rivals, trade routes

### 4. Pet Companion System
- **Combat Support**: Pets auto-attack enemies
- **Leveling System**: Pets gain XP and level up
- **Skill Trees**: Offensive, Defensive, Utility branches
- **Pet Charms**: Equipment that enhances pet capabilities
- **Variety**: Combat, Support, and Utility pet types

### 5. Progression & Replayability
- **Floor-Based Difficulty**: Deeper = harder enemies + better loot
- **Hazard Levels**: Choose difficulty (1-5 stars) before entering
- **Mastery System**: Challenge runs grant permanent bonuses
- **Biome Variety**: 7 different cave types with unique enemies

## Game Systems

### Auto-Combat System
- **AutoWeapon Class**: Manages weapon stats, firing, reloading
- **Firing Patterns**: Forward, backward, 360°, targeted
- **Damage Types**: Kinetic, fire, cryo, electric, acid
- **Status Effects**: Burning, freezing, stunning, armor reduction
- **Weapon Loadouts**: Equip 2-4 weapons simultaneously

### Pet Companion System
- **Pet Types**: Combat (Wolf, Hawk, Bear), Support (Fairy, Spirit, Phoenix), Utility (Dog, Cat, Owl)
- **Pet Abilities**: Auto-attacks, buffs, healing, item finding
- **Pet Progression**: Leveling, skill trees, equipment charms
- **Pet Taming**: Find and tame wild pets in different biomes

### Grenade System
- **Manual AoE**: Player-triggered explosions with cooldowns
- **Grenade Types**: Explosive, Cryo, H-Grenade (mining), Neurotoxin, Electric
- **Cooldowns**: 30-60 seconds per grenade type
- **Strategic Use**: Crowd control, mining, boss fights

### Village & Quest System
- **8 Villages**: MoonBrook Valley, Pinewood, Stonehelm, Sandshore, Frostpeak, Marshwood, Crystalgrove, Ruinwatch
- **Reputation Levels**: Stranger → Acquaintance → Friend → Trusted → Hero → Legend
- **Quest Types**: Cave (combat/exploration), Farm (production), Trading (commerce), Courier (delivery)
- **Rewards**: Gold, reputation, items, skill points, recipes

### Cave System
- **Procedural Generation**: Unique layouts each run
- **Wave Spawning**: Endless enemy waves
- **Floor Progression**: Descend for harder challenges
- **Biome Caves**: Forest, Mountain, Desert, Frozen, Swamp, Crystal, Ruins
- **Boss Encounters**: Special rooms with powerful enemies

## What Makes It Unique

### vs. Traditional Farming Sims
✅ **Has auto-shooter combat** (not just farming)
✅ **Combat restricted to caves** (overworld is peaceful)
✅ **Roguelite elements** (procedural generation, permadeath risk)
✅ **Pet companions with deep progression** (not just decorative)

### vs. Traditional Auto-Shooters
✅ **Full farming sim overworld** (not just a hub/menu)
✅ **8 villages to explore and interact with** (not just one base)
✅ **Quest variety** (cave, farm, trading, courier)
✅ **Peaceful activities** (fishing, farming, relationships)

### vs. Deep Rock Galactic
✅ **Pet companions instead of Bosco** (more customization)
✅ **Full village life** (not just space station)
✅ **Permanent progression** (mastery system)
✅ **Story focus** (defending valley from below)

### vs. Vampire Survivors
✅ **Peaceful overworld** (not constant combat)
✅ **Farming & village activities** (more than just combat)
✅ **Multi-village system** (exploration and relationships)
✅ **Pet companions** (not just passive upgrades)

## Story & Theme

### The Secret
The valley's 8 villages live in peaceful ignorance of the danger below. An ancient network of caves beneath their feet teems with alien-like insectoid creatures. You're the only one who knows—and the only one willing to fight.

### Your Double Life
- **Day**: Farmer, trader, helper—maintain the facade of normalcy
- **Night**: Cave delver, defender—fight back the underground horde
- **Secret**: Villagers must never know the full truth

### Progression
As you descend deeper and gain village trust:
- Discover ancient ruins and learn the creatures' origin
- Uncover why all caves connect to the same network
- Face the ultimate threat at the deepest levels
- Decide the fate of the valley

## Documentation

### Design Documents
- **MOONBROOK_VALLEY_DESIGN.md**: Complete game design (17KB)
  - Core mechanics, systems, progression
  - Village network, quest types, pet system
  - Technical implementation details

- **AUTO_COMBAT_MECHANICS.md**: Auto-combat specification (15KB)
  - AutoWeapon system architecture
  - Firing patterns and targeting AI
  - Damage types and status effects
  - Performance optimization strategies

### Updated Documents
- **README.md**: Updated game concept and feature list
- **CONTROLS.md**: Auto-combat controls, grenade system
- **COMBAT_INTEGRATION_SUMMARY.md**: Transition plan from manual to auto-combat
- **WORLD_BELOW_DESIGN.md**: Cave-only combat zones

## Implementation Roadmap

### Phase 1: Documentation ✅ COMPLETE
Complete design documentation for all systems

### Phase 2: Pet Enhancement (2-3 weeks)
Enhance PetSystem with combat abilities, leveling, skill trees, charms

### Phase 3: Auto-Combat Core (2-3 weeks)
Implement AutoWeaponSystem, firing patterns, auto-reload

### Phase 4: Cave Zones (2-3 weeks)
Update dungeon system for wave-based combat, restrict to caves only

### Phase 5: Village Network (3-4 weeks)
Implement 8 villages, reputation, village-specific features

### Phase 6: Quest System (2-3 weeks)
Implement cave, farm, trading, and courier quests

### Phase 7: Damage & Grenades (2-3 weeks)
Add damage types, status effects, grenade system

### Phase 8: Polish (2-3 weeks)
Balance, UI updates, tutorials, integration testing

**Total Estimated Time**: 15-20 weeks (4-5 months)

## Success Criteria

### Player Engagement
- Average cave run: 15-30 minutes
- Average depth reached: Floors 8-12 for new players
- Return rate: 70%+ players return for multiple runs
- Pet usage: 90%+ players actively use pets

### Game Balance
- Weapon diversity: No single weapon >30% usage
- Pet diversity: No single pet >25% usage
- Difficulty curve: 40% completion rate at Hazard 3
- Progression: Players reach endgame in 20-30 hours

### Fun Factor
- "Just one more run" replayability
- Hundreds of viable build combinations
- Positioning matters more than reflexes
- Satisfying auto-combat feedback

## Next Steps

1. **Review & Approval**: Ensure design aligns with vision
2. **Technical Planning**: Break down implementation into tasks
3. **Asset Planning**: Identify needed sprites, sounds, UI elements
4. **Prototype**: Build minimal auto-combat prototype
5. **Iterate**: Playtest and refine mechanics
6. **Full Implementation**: Execute roadmap phases

---

**Status**: Design Complete, Ready for Implementation  
**Last Updated**: January 3, 2026  
**Owner**: MoonBrook Ridge Development Team

## Related Documents

- [MOONBROOK_VALLEY_DESIGN.md](MOONBROOK_VALLEY_DESIGN.md) - Complete game design
- [AUTO_COMBAT_MECHANICS.md](AUTO_COMBAT_MECHANICS.md) - Auto-combat technical spec
- [README.md](README.md) - Project overview
- [CONTROLS.md](CONTROLS.md) - Control reference
- [COMBAT_INTEGRATION_SUMMARY.md](COMBAT_INTEGRATION_SUMMARY.md) - Combat transition plan
- [WORLD_BELOW_DESIGN.md](WORLD_BELOW_DESIGN.md) - Cave system design
