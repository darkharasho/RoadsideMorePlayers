using BepInEx;
using BepInEx.Configuration;
using BepInEx.Logging;
using BepInEx.Unity.IL2CPP;
using HarmonyLib;

namespace RoadsideMorePlayers;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public class Plugin : BasePlugin
{
    public const string PluginGuid = "com.github.RoadsideMorePlayers";
    public const string PluginName = "RoadsideMorePlayers";
    public const string PluginVersion = "1.0.0";

    public static ManualLogSource Logger { get; private set; }
    public static ConfigEntry<int> MaxPlayers { get; private set; }

    private Harmony _harmony;

    public override void Load()
    {
        Logger = Log;

        MaxPlayers = Config.Bind(
            "General",
            "MaxPlayers",
            8,
            new ConfigDescription(
                "The maximum number of players allowed in a lobby. Only applies when you are the host. Vanilla default is 4.",
                new AcceptableValueRange<int>(2, 32)
            )
        );

        Logger.LogInfo($"{PluginName} v{PluginVersion} loaded! MaxPlayers set to {MaxPlayers.Value}");

        _harmony = new Harmony(PluginGuid);
        _harmony.PatchAll(typeof(Patches.FusionPlayerCountPatch));
        _harmony.PatchAll(typeof(Patches.SteamLobbyPatch));

        Logger.LogInfo("All patches applied successfully.");
    }
}
