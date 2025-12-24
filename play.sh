#!/bin/bash
# Quick play script for MoonBrook Ridge
# This script builds and runs the game

echo "🌾 MoonBrook Ridge - Starting Game..."
echo ""

cd MoonBrookRidge

# Build the game
echo "📦 Building game..."
dotnet build --configuration Release

if [ $? -ne 0 ]; then
    echo "❌ Build failed! Please check the errors above."
    exit 1
fi

echo ""
echo "✅ Build successful!"
echo "🎮 Launching game..."
echo ""

# Run the game
dotnet run --configuration Release
