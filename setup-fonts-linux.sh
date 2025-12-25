#!/bin/bash
# Setup fonts for building MoonBrook Ridge on Linux
# This script ensures Liberation Sans fonts are properly installed
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

# Verify Liberation Sans font files exist
echo "Verifying Liberation Sans font files..."
REQUIRED_FONTS=(
    "LiberationSans-Regular.ttf"
    "LiberationSans-Bold.ttf"
    "LiberationSans-Italic.ttf"
    "LiberationSans-BoldItalic.ttf"
)

MISSING_FONTS=0
for font_file in "${REQUIRED_FONTS[@]}"; do
    if [ ! -f "$LIBERATION_DIR/$font_file" ]; then
        echo "  ✗ Missing: $font_file"
        MISSING_FONTS=$((MISSING_FONTS + 1))
    else
        echo "  ✓ Found: $font_file"
    fi
done

if [ $MISSING_FONTS -gt 0 ]; then
    echo "Error: $MISSING_FONTS font file(s) are missing."
    echo "Try reinstalling with: sudo apt-get install --reinstall fonts-liberation"
    exit 1
fi

# Refresh font cache
echo "Refreshing font cache..."
LOG_FILE="/tmp/fc-cache-$$.log"
if ! sudo fc-cache -f -v > "$LOG_FILE" 2>&1; then
    echo "Warning: fc-cache encountered issues. Check $LOG_FILE for details."
    exit 1
else
    echo "Font cache refreshed successfully."
    rm -f "$LOG_FILE"
fi

echo ""
echo "✓ Font setup complete! Liberation Sans is properly configured."
echo "  You can now build the project with: cd MoonBrookRidge && dotnet build"
echo ""
echo "Note: This script is designed for Debian/Ubuntu systems."
echo "For Fedora/RHEL/Arch, please install fonts-liberation via your package manager."
