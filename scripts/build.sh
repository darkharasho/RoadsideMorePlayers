#!/usr/bin/env bash
set -euo pipefail

SCRIPT_DIR="$(cd "$(dirname "${BASH_SOURCE[0]}")" && pwd)"
PROJECT_DIR="$(dirname "$SCRIPT_DIR")"

cd "$PROJECT_DIR"

# Allow overriding game directory
GAME_DIR="${GAME_DIR:-$HOME/.local/share/Steam/steamapps/common/Roadside Research}"

echo "=== Building RoadsideMorePlayers ==="
echo "Game directory: $GAME_DIR"

if [ ! -d "$GAME_DIR/BepInEx/interop" ]; then
    echo "ERROR: BepInEx interop assemblies not found at $GAME_DIR/BepInEx/interop/"
    echo "Install BepInEx and run the game once to generate them."
    exit 1
fi

dotnet build -c Release -p:GameDir="$GAME_DIR" "$@"

echo ""
echo "=== Build complete ==="
echo "Output: bin/Release/net6.0/RoadsideMorePlayers.dll"
echo "Also copied to: $GAME_DIR/BepInEx/plugins/RoadsideMorePlayers/"
