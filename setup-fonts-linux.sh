#!/bin/bash
# Setup fonts for building MoonBrook Ridge on Linux
# This script creates font symlinks to allow Arial font references to work on Linux

echo "Setting up fonts for MoonBrook Ridge on Linux..."

# Check if Liberation fonts are installed
if ! dpkg -l | grep -q fonts-liberation; then
    echo "Installing Liberation fonts..."
    sudo apt-get update
    sudo apt-get install -y fonts-liberation
fi

# Create Arial symlinks using Liberation Sans
echo "Creating Arial font symlinks..."
sudo mkdir -p /usr/share/fonts/truetype/msttcorefonts

cd /usr/share/fonts/truetype/liberation

# Create symlinks with proper naming for MonoGame
sudo ln -sf "$(pwd)/LiberationSans-Regular.ttf" /usr/share/fonts/truetype/msttcorefonts/arial.ttf
sudo ln -sf "$(pwd)/LiberationSans-Bold.ttf" /usr/share/fonts/truetype/msttcorefonts/arialbd.ttf
sudo ln -sf "$(pwd)/LiberationSans-Italic.ttf" /usr/share/fonts/truetype/msttcorefonts/ariali.ttf
sudo ln -sf "$(pwd)/LiberationSans-BoldItalic.ttf" /usr/share/fonts/truetype/msttcorefonts/arialbi.ttf

# Refresh font cache
echo "Refreshing font cache..."
sudo fc-cache -f -v > /dev/null 2>&1

echo "Font setup complete! Arial font references will now use Liberation Sans."
echo "You can now build the project with: dotnet build"
