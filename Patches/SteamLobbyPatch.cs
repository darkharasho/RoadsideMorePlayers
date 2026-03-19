using HarmonyLib;
using Steamworks;

namespace RoadsideMorePlayers.Patches;

/// <summary>
/// Patches SteamMatchmaking.CreateLobby to override the max members parameter.
/// Only the host calls CreateLobby, so this is inherently host-only.
/// </summary>
public static class SteamLobbyPatch
{
    [HarmonyPatch(typeof(SteamMatchmaking), nameof(SteamMatchmaking.CreateLobby))]
    [HarmonyPrefix]
    public static void CreateLobby_Prefix(ref ELobbyType eLobbyType, ref int cMaxMembers)
    {
        var maxPlayers = Plugin.MaxPlayers.Value;
        Plugin.Logger.LogInfo($"[SteamLobbyPatch] Overriding lobby max members from {cMaxMembers} to {maxPlayers} (LobbyType: {eLobbyType})");
        cMaxMembers = maxPlayers;
    }
}
