#!/bin/bash
# Engine Migration Validation Script
# This script validates the custom engine implementation

set -e

echo "🔍 MoonBrook Ridge - Engine Migration Validation"
echo "================================================="
echo ""

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
NC='\033[0m' # No Color

ERRORS=0
WARNINGS=0

# Function to print status
print_status() {
    if [ $1 -eq 0 ]; then
        echo -e "${GREEN}✓${NC} $2"
    else
        echo -e "${RED}✗${NC} $2"
        ERRORS=$((ERRORS + 1))
    fi
}

print_warning() {
    echo -e "${YELLOW}⚠${NC} $1"
    WARNINGS=$((WARNINGS + 1))
}

# 1. Check build status
echo "📦 Checking build status..."
if dotnet build --configuration Debug > /dev/null 2>&1; then
    print_status 0 "Project builds successfully"
else
    print_status 1 "Project fails to build"
    echo "Run 'dotnet build' to see detailed errors"
    exit 1
fi

# 2. Check for compilation errors
echo ""
echo "🔨 Checking for compilation errors..."
BUILD_OUTPUT=$(dotnet build 2>&1)
ERROR_COUNT=$(echo "$BUILD_OUTPUT" | grep -c "Error(s)" || true)
if echo "$BUILD_OUTPUT" | grep -q "0 Error(s)"; then
    print_status 0 "No compilation errors"
else
    print_status 1 "Compilation errors detected"
fi

# 3. Check for critical warnings
echo ""
echo "⚠️  Checking for critical warnings..."
WARNING_COUNT=$(echo "$BUILD_OUTPUT" | grep -c "Warning(s)" || true)
if echo "$BUILD_OUTPUT" | grep -q "0 Warning(s)"; then
    print_status 0 "No build warnings"
else
    # Check if warnings are acceptable (nullable reference warnings are OK)
    if echo "$BUILD_OUTPUT" | grep -q "nullable"; then
        print_warning "Some nullable reference warnings present (acceptable)"
    else
        print_warning "Build warnings present"
    fi
fi

# 4. Check critical engine files exist
echo ""
echo "📁 Checking critical engine files..."
CRITICAL_FILES=(
    "MoonBrookRidge.Engine/MonoGameCompat/Game.cs"
    "MoonBrookRidge.Engine/MonoGameCompat/GraphicsDevice.cs"
    "MoonBrookRidge.Engine/MonoGameCompat/SpriteBatch.cs"
    "MoonBrookRidge.Engine/MonoGameCompat/Texture2D.cs"
    "MoonBrookRidge.Engine/MonoGameCompat/ContentManager.cs"
    "MoonBrookRidge.Engine/MonoGameCompat/Color.cs"
    "MoonBrookRidge.Engine/MonoGameCompat/Vector2.cs"
    "MoonBrookRidge.Engine/MonoGameCompat/Keyboard.cs"
    "MoonBrookRidge.Engine/MonoGameCompat/Mouse.cs"
)

for file in "${CRITICAL_FILES[@]}"; do
    if [ -f "$file" ]; then
        print_status 0 "Found $file"
    else
        print_status 1 "Missing $file"
    fi
done

# 5. Check Game1.cs uses custom engine
echo ""
echo "🎮 Checking game entry point..."
if grep -q "MoonBrookRidge.Engine.MonoGameCompat" MoonBrookRidge/Program.cs; then
    print_status 0 "Program.cs uses custom engine namespace"
else
    print_status 1 "Program.cs not using custom engine namespace"
fi

# 6. Check for MonoGame dependencies
echo ""
echo "📦 Checking dependencies..."
if grep -q "MonoGame.Framework" MoonBrookRidge/MoonBrookRidge.csproj; then
    print_warning "MonoGame.Framework still referenced in project (may be intentional)"
else
    print_status 0 "No MonoGame.Framework dependency found"
fi

# 7. Check Content directory
echo ""
echo "🎨 Checking Content directory..."
if [ -d "MoonBrookRidge/Content" ]; then
    print_status 0 "Content directory exists"
    
    # Check for critical content
    if [ -d "MoonBrookRidge/Content/Fonts" ]; then
        print_status 0 "Fonts directory exists"
    else
        print_warning "Fonts directory not found"
    fi
    
    if [ -d "MoonBrookRidge/Content/Textures" ]; then
        print_status 0 "Textures directory exists"
    else
        print_warning "Textures directory not found"
    fi
else
    print_status 1 "Content directory missing"
fi

# 8. Check for output binaries
echo ""
echo "🔧 Checking build output..."
if [ -f "MoonBrookRidge/bin/Debug/net9.0/MoonBrookRidge.dll" ]; then
    print_status 0 "Main game DLL built"
else
    print_status 1 "Main game DLL not found"
fi

if [ -f "MoonBrookRidge/bin/Debug/net9.0/MoonBrookRidge.Engine.dll" ]; then
    print_status 0 "Engine DLL built"
else
    print_status 1 "Engine DLL not found"
fi

if [ -f "MoonBrookRidge/bin/Debug/net9.0/MoonBrookEngine.dll" ]; then
    print_status 0 "Core engine DLL built"
else
    print_status 1 "Core engine DLL not found"
fi

# 9. Check for executable
echo ""
echo "🚀 Checking executable..."
# Check for both Unix and Windows executables
if [ -x "MoonBrookRidge/bin/Debug/net9.0/MoonBrookRidge" ] || [ -f "MoonBrookRidge/bin/Debug/net9.0/MoonBrookRidge.exe" ]; then
    print_status 0 "Game executable exists"
else
    print_status 1 "Game executable not found"
fi

# 10. Validate key engine implementations
echo ""
echo "🔍 Validating engine implementations..."

# Check SpriteBatch has Draw methods
if grep -q "public void Draw" MoonBrookRidge.Engine/MonoGameCompat/SpriteBatch.cs; then
    print_status 0 "SpriteBatch has Draw methods"
else
    print_status 1 "SpriteBatch missing Draw methods"
fi

# Check ContentManager can load content
if grep -q "public T Load<T>" MoonBrookRidge.Engine/MonoGameCompat/ContentManager.cs; then
    print_status 0 "ContentManager has Load method"
else
    print_status 1 "ContentManager missing Load method"
fi

# Check Game class has required methods
if grep -q "protected virtual void Initialize" MoonBrookRidge.Engine/MonoGameCompat/Game.cs; then
    print_status 0 "Game class has Initialize method"
else
    print_status 1 "Game class missing Initialize method"
fi

if grep -q "protected virtual void LoadContent" MoonBrookRidge.Engine/MonoGameCompat/Game.cs; then
    print_status 0 "Game class has LoadContent method"
else
    print_status 1 "Game class missing LoadContent method"
fi

if grep -q "protected virtual void Update" MoonBrookRidge.Engine/MonoGameCompat/Game.cs; then
    print_status 0 "Game class has Update method"
else
    print_status 1 "Game class missing Update method"
fi

if grep -q "protected virtual void Draw" MoonBrookRidge.Engine/MonoGameCompat/Game.cs; then
    print_status 0 "Game class has Draw method"
else
    print_status 1 "Game class missing Draw method"
fi

# Summary
echo ""
echo "================================================="
echo "📊 Validation Summary"
echo "================================================="

if [ $ERRORS -eq 0 ]; then
    echo -e "${GREEN}✅ All critical checks passed!${NC}"
else
    echo -e "${RED}❌ $ERRORS error(s) found${NC}"
fi

if [ $WARNINGS -gt 0 ]; then
    echo -e "${YELLOW}⚠️  $WARNINGS warning(s) found${NC}"
fi

echo ""
echo "Build Status: $(echo "$BUILD_OUTPUT" | grep "Build succeeded" || echo "Build failed")"
echo ""

if [ $ERRORS -eq 0 ]; then
    echo "✅ Engine migration validation: PASSED"
    echo ""
    echo "Next steps:"
    echo "1. Run the game: ./play.sh or cd MoonBrookRidge && dotnet run"
    echo "2. Follow runtime testing guide: docs/guides/RUNTIME_TESTING_GUIDE.md"
    echo "3. Report any runtime issues found"
    exit 0
else
    echo "❌ Engine migration validation: FAILED"
    echo ""
    echo "Please fix the errors above before proceeding to runtime testing."
    exit 1
fi
