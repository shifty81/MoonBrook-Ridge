#!/bin/bash
# Setup fonts for building MoonBrook Ridge on Linux
# This script creates font symlinks to allow Arial font references to work on Linux
# Note: This script is designed for Debian/Ubuntu systems

set -e  # Exit on error

echo "Setting up fonts for MoonBrook Ridge on Linux..."

# Check if Liberation fonts are installed
if ! dpkg -l | grep -q fonts-liberation; then
    echo "Installing Liberation fonts..."
    sudo apt-get update
    sudo apt-get install -y fonts-liberation
fi

# Verify the Liberation fonts directory exists
LIBERATION_DIR="/usr/share/fonts/truetype/liberation"
if [ ! -d "$LIBERATION_DIR" ]; then
    echo "Error: Liberation fonts directory not found at $LIBERATION_DIR"
    echo "Liberation fonts may not be installed correctly."
    exit 1
fi

# Create Arial symlinks using Liberation Sans
echo "Creating Arial font symlinks..."
sudo mkdir -p /usr/share/fonts/truetype/msttcorefonts

cd "$LIBERATION_DIR"

# Create symlinks with proper naming for MonoGame
# Verify font files exist before creating symlinks
FONT_FILES=(
    "LiberationSans-Regular.ttf:arial.ttf"
    "LiberationSans-Bold.ttf:arialbd.ttf"
    "LiberationSans-Italic.ttf:ariali.ttf"
    "LiberationSans-BoldItalic.ttf:arialbi.ttf"
)

for font_pair in "${FONT_FILES[@]}"; do
    source_font="${font_pair%%:*}"
    target_name="${font_pair##*:}"
    
    if [ ! -f "$source_font" ]; then
        echo "Warning: Font file $source_font not found, skipping..."
        continue
    fi
    
    sudo ln -sf "$(pwd)/$source_font" "/usr/share/fonts/truetype/msttcorefonts/$target_name"
done

# Refresh font cache
echo "Refreshing font cache..."
LOG_FILE="/tmp/fc-cache-$$.log"
if ! sudo fc-cache -f -v > "$LOG_FILE" 2>&1; then
    echo "Warning: fc-cache encountered issues. Check $LOG_FILE for details."
else
    echo "Font cache refreshed successfully."
    rm -f "$LOG_FILE"
fi

echo ""
echo "✓ Font setup complete! Liberation Sans is configured for use."
echo "  You can now build the project with: dotnet build"
echo ""
echo "Note: This script is designed for Debian/Ubuntu systems."
echo "For Fedora/RHEL/Arch, please install fonts-liberation via your package manager."
