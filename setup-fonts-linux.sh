#!/bin/bash
# Setup fonts for building MoonBrook Ridge on Linux
# NOTE: As of December 2024, the Liberation Sans font is now BUNDLED with the project.
# This script is no longer required for building the game, but can still be useful
# if you want to install Liberation fonts system-wide for other applications.
# Note: This script is designed for Debian/Ubuntu systems

set -e  # Exit on error

echo "=========================================="
echo "IMPORTANT NOTE:"
echo "Liberation Sans font is now bundled with MoonBrook Ridge!"
echo "This script is NO LONGER REQUIRED to build the game."
echo "=========================================="
echo ""
echo "However, if you want to install Liberation fonts system-wide"
echo "for other applications, this script can still do that for you."
echo ""
read -p "Continue with system font installation? (y/N) " -n 1 -r
echo
if [[ ! $REPLY =~ ^[Yy]$ ]]; then
    echo "Skipping system font installation."
    echo "You can build the game directly with: cd MoonBrookRidge && dotnet build"
    exit 0
fi

echo ""
echo "Setting up Liberation fonts system-wide on Linux..."

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
echo "✓ System font setup complete! Liberation Sans is installed system-wide."
echo ""
echo "Note: The game uses its bundled font, not the system font."
echo "You can now build the project with: cd MoonBrookRidge && dotnet build"
echo ""
echo "Note: This script is designed for Debian/Ubuntu systems."
echo "For Fedora/RHEL/Arch, please install fonts-liberation via your package manager."
