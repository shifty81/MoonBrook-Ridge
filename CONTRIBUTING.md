# Contributing to MoonBrook Ridge

Thank you for contributing to MoonBrook Ridge! This guide will help you make effective contributions that can be reviewed efficiently by both human reviewers and automated tools.

## Pull Request Best Practices

### PR Size Guidelines

To ensure efficient code review and automated tool processing (including GitHub Copilot):

#### Recommended PR Sizes
- ✅ **Small PRs** (< 200 files): Ideal for code changes, bug fixes, and feature additions
- ⚠️ **Medium PRs** (200-1000 files): Acceptable for larger features or refactoring
- ❌ **Large PRs** (> 1000 files): Should be broken down or handled specially

#### Exception: Asset-Only PRs
Large PRs containing primarily binary assets (images, audio, etc.) should be:
- Clearly marked as asset additions in the title (e.g., "[Assets] Add character sprites")
- Separated from code changes
- Reviewed manually (automated tools may not process them effectively)

### Separating Code and Assets

When making changes that involve both code and large binary assets:

**❌ Don't do this:**
```
PR #1: Add 10,000 sprite files + new sprite loading system
```

**✅ Do this instead:**
```
PR #1: Add character sprite assets
PR #2: Implement new sprite loading system
```

This approach:
- Makes code review more focused
- Allows automated tools to process changes effectively
- Makes it easier to track what changed and why
- Simplifies rollback if issues are found

### Breaking Down Large Changes

For major refactoring or project reorganization:

**Phase 1: Structure**
- Move/rename directories
- Update namespaces
- Fix imports

**Phase 2: Assets**
- Add new assets (if needed)
- Update asset references

**Phase 3: Implementation**
- Add new features
- Implement changes

**Phase 4: Tests**
- Add/update tests
- Update documentation

### PR Description Best Practices

A good PR description should include:

1. **What**: Clear description of changes
2. **Why**: Reason for the changes
3. **How**: Brief explanation of implementation approach
4. **Testing**: How you tested the changes
5. **Screenshots**: For UI changes
6. **Breaking Changes**: Any breaking changes or migration steps

Example:
```markdown
## Add Farming System

### What
Implements a basic farming system allowing players to plant, water, and harvest crops.

### Why
Core gameplay feature needed for the farming simulation aspect of the game.

### How
- Added `CropSystem` class to manage crop lifecycle
- Integrated with existing `TimeSystem` for growth progression
- Added crop data definitions in JSON format

### Testing
- Tested planting, watering, and harvesting with all crop types
- Verified crop growth over multiple in-game days
- Checked integration with inventory system

### Breaking Changes
None
```

## Code Guidelines

### C# Style
- Follow standard C# naming conventions
- Use meaningful variable and method names
- Add XML documentation comments for public APIs
- Keep methods focused and small

### MonoGame Specific
- Dispose of resources properly
- Use content pipeline for assets
- Follow MonoGame best practices for Update/Draw patterns

## Asset Guidelines

### Adding Assets

1. **Organization**: Place assets in appropriate subdirectories under `sprites/`
2. **Naming**: Use consistent, descriptive names (e.g., `character_idle_frame_01.png`)
3. **Format**: Prefer PNG for sprites, WAV for audio
4. **Size**: Optimize assets before committing (compress images, trim audio)

### Large Asset Additions

If adding more than 100 asset files:
1. Create a separate PR just for assets
2. Title it clearly: `[Assets] Add <description>`
3. Include a manifest or list of what's being added
4. Consider using Git LFS for very large files (> 100MB total)

## Git Workflow

### Branch Naming
- `feature/description` - New features
- `bugfix/description` - Bug fixes
- `refactor/description` - Code refactoring
- `docs/description` - Documentation changes
- `assets/description` - Asset additions

### Commit Messages
Follow conventional commit format:
```
<type>: <description>

[optional body]
[optional footer]
```

Types: `feat`, `fix`, `docs`, `style`, `refactor`, `test`, `chore`

Example:
```
feat: add crop planting system

Implements basic crop planting with soil preparation and seed selection.
Part of farming system feature.

Closes #123
```

## Testing

- Run the game and test your changes
- If adding tests, ensure they pass locally
- Check for build errors and warnings
- Test on your target platform(s)

## Review Process

1. **Create PR**: Open a PR with clear description
2. **Automated Checks**: Wait for automated checks (if any)
3. **Address Feedback**: Respond to review comments
4. **Update PR**: Make requested changes
5. **Approval**: Wait for approval from maintainers
6. **Merge**: Maintainer will merge when ready

## Questions?

If you have questions about contributing, feel free to:
- Open a discussion on GitHub
- Ask in PR comments
- Reach out to maintainers

Thank you for contributing to MoonBrook Ridge! 🌙🏔️
