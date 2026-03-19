#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"
DIST_DIR="$PROJECT_DIR/dist"
BEPINEX_URL="https://builds.bepinex.dev/projects/bepinex_be/755/BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.755%2B3fab71a.zip"
BEPINEX_ZIP="/tmp/bepinex-bundle.zip"

VERSION="${1:-1.0.0}"
GAME_DIR="${GAME_DIR:-$HOME/.local/share/Steam/steamapps/common/Roadside Research}"
MOD_DLL="$PROJECT_DIR/bin/Release/net6.0/RoadsideMorePlayers.dll"

echo "=== Packaging RoadsideMorePlayers v$VERSION ==="

# Build first
"$SCRIPT_DIR/build.sh"

if [ ! -f "$MOD_DLL" ]; then
    echo "ERROR: Build output not found at $MOD_DLL"
    exit 1
fi

rm -rf "$DIST_DIR"
mkdir -p "$DIST_DIR"

# --- Package 1: Mod-only (for users who already have BepInEx) ---
echo ""
echo "--- Creating mod-only package ---"
MOD_ONLY_DIR="$DIST_DIR/mod-only"
mkdir -p "$MOD_ONLY_DIR/BepInEx/plugins/RoadsideMorePlayers"
cp "$MOD_DLL" "$MOD_ONLY_DIR/BepInEx/plugins/RoadsideMorePlayers/"

cd "$MOD_ONLY_DIR"
zip -r "$DIST_DIR/RoadsideMorePlayers-v${VERSION}.zip" .
echo "Created: dist/RoadsideMorePlayers-v${VERSION}.zip"

# --- Package 2: Bundled with BepInEx (for new users) ---
echo ""
echo "--- Creating bundled package (with BepInEx) ---"

# Download BepInEx if not cached
if [ ! -f "$BEPINEX_ZIP" ]; then
    echo "Downloading BepInEx 6.0.0-be.755..."
    curl -L -o "$BEPINEX_ZIP" "$BEPINEX_URL"
fi

BUNDLE_DIR="$DIST_DIR/bundled"
mkdir -p "$BUNDLE_DIR"

# Extract BepInEx
cd "$BUNDLE_DIR"
unzip -q "$BEPINEX_ZIP"

# Add our mod
mkdir -p "$BUNDLE_DIR/BepInEx/plugins/RoadsideMorePlayers"
cp "$MOD_DLL" "$BUNDLE_DIR/BepInEx/plugins/RoadsideMorePlayers/"

# Pre-create config directory with BepInEx config fix for Unity 6
mkdir -p "$BUNDLE_DIR/BepInEx/config"
cat > "$BUNDLE_DIR/BepInEx/config/BepInEx.cfg" << 'CFGEOF'
[Logging]

UnityLogListening = false
CFGEOF

zip -r "$DIST_DIR/RoadsideMorePlayers-v${VERSION}-BepInEx-Bundle.zip" .
echo "Created: dist/RoadsideMorePlayers-v${VERSION}-BepInEx-Bundle.zip"

# Cleanup
rm -rf "$MOD_ONLY_DIR" "$BUNDLE_DIR"

echo ""
echo "=== Packaging complete ==="
echo "Files in dist/:"
ls -lh "$DIST_DIR"/*.zip
