using Fusion;
using HarmonyLib;

namespace RoadsideMorePlayers.Patches;

/// <summary>
/// Patches NetworkRunner.StartGame to override the PlayerCount in StartGameArgs.
/// Only modifies the value when the local peer is starting as Host or Server.
/// </summary>
public static class FusionPlayerCountPatch
{
    [HarmonyPatch(typeof(NetworkRunner), nameof(NetworkRunner.StartGame))]
    [HarmonyPrefix]
    public static void StartGame_Prefix(ref StartGameArgs args)
    {
        var maxPlayers = Plugin.MaxPlayers.Value;

        // Only override when hosting
        if (args.GameMode == GameMode.Host || args.GameMode == GameMode.Server)
        {
            var original = args.PlayerCount;
            args.PlayerCount = new Il2CppSystem.Nullable<int>(maxPlayers);
            Plugin.Logger.LogInfo($"[FusionPatch] Overriding PlayerCount from {original} to {maxPlayers} (GameMode: {args.GameMode})");
        }
        else
        {
            Plugin.Logger.LogInfo($"[FusionPatch] Not hosting (GameMode: {args.GameMode}), skipping PlayerCount override.");
        }
    }
}
