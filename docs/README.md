# MoonBrook Ridge Documentation

This directory contains all project documentation organized by topic.

## Directory Structure

### 📐 [architecture/](architecture/)
Design documents and architectural decisions:
- Game world design (MoonBrook Valley, World Below, World Expansion)
- Custom engine architecture and conversion plans
- High-level system design

**Key Documents:**
- [ARCHITECTURE.md](architecture/ARCHITECTURE.md) - Overall project architecture
- [CUSTOM_ENGINE_CONVERSION_PLAN.md](architecture/CUSTOM_ENGINE_CONVERSION_PLAN.md) - Plan for migrating from MonoGame
- [MOONBROOK_VALLEY_DESIGN.md](architecture/MOONBROOK_VALLEY_DESIGN.md) - World design

### ⚙️ [engine/](engine/)
Custom MoonBrook Engine documentation:
- Engine development summaries by week
- Engine integration guides
- Optimization plans and implementations

**Key Documents:**
- Weekly summaries (WEEK1-9) tracking engine development progress
- Integration guides for using the engine
- Performance optimization documentation

### 🔨 [implementation/](implementation/)
Implementation summaries and phase completions:
- Phase completion summaries (Phases 5-10)
- Task completion reports
- Integration summaries for specific features
- PR summaries

### 🎮 [systems/](systems/)
Game systems documentation:
- Auto-combat mechanics
- Individual system documentation

### 🎨 [assets/](assets/)
Asset integration and usage guides:
- Sprite guides and implementations
- Tileset guides and implementations
- Asset loading and management
- Sunnyside asset integration
- WorldGen asset guides

**Key Documents:**
- Asset loading, integration, and status documents
- Sprite sheet guides
- Tileset implementation guides

### 📖 [guides/](guides/)
Developer and player guides:
- Controls documentation
- Development setup
- Testing guides
- Playtest guide

**Key Documents:**
- [CONTROLS.md](guides/CONTROLS.md) - Game controls
- [DEVELOPMENT.md](guides/DEVELOPMENT.md) - Development guide
- [DEV_SETUP.md](guides/DEV_SETUP.md) - Setup instructions
- [TESTING_GUIDE.md](guides/TESTING_GUIDE.md) - Testing procedures

### 🖼️ [images/](images/)
Screenshots and visual documentation:
- Game screenshots
- UI mockups
- Visual references

## Quick Links

### For New Developers
1. Start with [DEVELOPMENT.md](guides/DEVELOPMENT.md)
2. Read [DEV_SETUP.md](guides/DEV_SETUP.md)
3. Review [ARCHITECTURE.md](architecture/ARCHITECTURE.md)

### For Understanding the Game
1. Read [../README.md](../README.md) - Main project README
2. Check [MOONBROOK_VALLEY_DESIGN.md](architecture/MOONBROOK_VALLEY_DESIGN.md)
3. Review [CONTROLS.md](guides/CONTROLS.md)

### For Engine Development
1. Read [CUSTOM_ENGINE_CONVERSION_PLAN.md](architecture/CUSTOM_ENGINE_CONVERSION_PLAN.md)
2. Follow the weekly summaries in [engine/](engine/)
3. Review integration guides in [engine/](engine/)

### For Asset Work
1. Check [assets/](assets/) for asset guides
2. Review sprite and tileset documentation
3. See asset loading guides

## Document Types

- **SUMMARY.md** - Summary of completed work
- **INTEGRATION.md** - Integration guides for features
- **GUIDE.md** - How-to guides
- **DESIGN.md** - Design documents
- **COMPLETION.md** - Phase/task completion reports
- **IMPLEMENTATION.md** - Implementation details

## Contributing

When adding new documentation:
1. Place it in the appropriate subdirectory
2. Use descriptive names following existing patterns
3. Update this README if adding a major new document
4. Link from relevant places (main README, guides, etc.)

## History

Documentation was reorganized on 2026-01-04 to improve discoverability and maintainability. Previously, 90+ documentation files were in the project root. They have now been organized into logical subdirectories.
