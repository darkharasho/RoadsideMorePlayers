# RoadsideMorePlayers

A BepInEx mod for **Roadside Research** that increases the maximum player count beyond the default 4-player limit.

## Features

- Configurable max player count (default: 8, up to 32)
- Host-only enforcement — only the host needs the mod for networking to work
- Patches both Photon Fusion session limits and Steam lobby size
- Config file generated on first run for easy adjustment

## Requirements

- [BepInEx 6.0.0-be.755](https://builds.bepinex.dev/projects/bepinex_be) (Unity IL2CPP, win-x64)
- Roadside Research (Steam)

## Installation

### 1. Install BepInEx

1. Download `BepInEx-Unity.IL2CPP-win-x64-6.0.0-be.755+3fab71a.zip` from [BepInEx Bleeding Edge Builds](https://builds.bepinex.dev/projects/bepinex_be)
2. Extract the ZIP into your Roadside Research game folder (where `Roadside Research.exe` is)
3. **Linux/Steam Deck users:** Add this to the game's Steam Launch Options:
   ```
   WINEDLLOVERRIDES="winhttp=n,b" %command%
   ```
4. Launch the game once and close it. This generates the required config and interop files.
5. Edit `BepInEx/config/BepInEx.cfg` and set `UnityLogListening = false` under `[Logging]` (required for Unity 6 compatibility)

### 2. Install the Mod

1. Download `RoadsideMorePlayers.dll` from [Releases](https://github.com/darkharasho/RoadsideMorePlayers/releases)
2. Place it in `BepInEx/plugins/RoadsideMorePlayers/` inside your game folder
3. Launch the game

## Configuration

After the first launch with the mod installed, a config file is generated at:

```
BepInEx/config/com.github.RoadsideMorePlayers.cfg
```

```ini
[General]

## The maximum number of players allowed in a lobby. Only applies when you are the host. Vanilla default is 4.
## Acceptable value range: From 2 to 32
MaxPlayers = 8
```

Change `MaxPlayers` to your desired value and relaunch the game.

## How It Works

The mod uses HarmonyX to patch two methods at runtime:

1. **`NetworkRunner.StartGame(StartGameArgs)`** — Overrides the `PlayerCount` field in Photon Fusion's session creation args when hosting
2. **`SteamMatchmaking.CreateLobby(ELobbyType, int)`** — Overrides the `cMaxMembers` parameter when the host creates a Steam lobby

Both patches only apply when you are the host. Clients joining your lobby do not need the mod installed (though it's recommended for the best experience).

## Building from Source

### Requirements

- .NET 6.0 SDK
- BepInEx installed in the game directory (for interop assembly references)

### Build

```bash
dotnet build
```

The output DLL is automatically copied to `BepInEx/plugins/RoadsideMorePlayers/` after building.

The `.csproj` expects the game to be installed at the default Steam location. If your game is elsewhere, set the `GameDir` MSBuild property:

```bash
dotnet build -p:GameDir="/path/to/Roadside Research"
```

## Known Limitations

- High player counts (>16) may cause instability with voice chat, UI, or game logic
- The game's lobby UI may not display more than 4 player slots
- The game may have exactly 4 spawn points — extra players might spawn in unexpected locations
- Game updates may break the mod if the developer changes method signatures

## Credits

- Built with [BepInEx](https://github.com/BepInEx/BepInEx) and [HarmonyX](https://github.com/BepInEx/HarmonyX)
- Inspired by [MorePlayers for R.E.P.O.](https://github.com/zelofi/MorePlayers)
