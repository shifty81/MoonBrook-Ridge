#!/bin/bash
# Quick test script to verify the game compiles without running it
# Useful for CI/CD or quick validation

echo "🧪 MoonBrook Ridge - Quick Test"
echo ""

cd MoonBrookRidge

# Clean build
echo "🧹 Cleaning previous builds..."
dotnet clean > /dev/null 2>&1

# Build the game
echo "📦 Building game..."
dotnet build --configuration Debug

if [ $? -ne 0 ]; then
    echo ""
    echo "❌ Build failed! Please check the errors above."
    exit 1
fi

echo ""
echo "✅ Build successful! Game is ready to run."
echo ""
echo "To play the game, run:"
echo "  ./play.sh"
echo ""
echo "Or manually:"
echo "  cd MoonBrookRidge && dotnet run"
