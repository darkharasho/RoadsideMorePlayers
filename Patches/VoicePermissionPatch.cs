using System;
using System.Collections;
using Audio.Voice;
using BepInEx.Unity.IL2CPP.Utils;
using HarmonyLib;
using UnityEngine;
using Object = UnityEngine.Object;

namespace RoadsideMorePlayers.Patches;

/// <summary>
/// Fixes voice chat race condition where specific player pairs can't hear each other.
/// The game's voice permission handshake (RpcSetPermissionToSendVoice) can arrive
/// before LinkDissonanceComms finishes initializing, causing the voice channel to
/// never open despite permissions being set. This patch forces a delayed refresh
/// of voice permissions after each PlayerVoice spawn.
/// </summary>
public static class VoicePermissionPatch
{
    [HarmonyPatch(typeof(PlayerVoice), nameof(PlayerVoice.Spawned))]
    [HarmonyPostfix]
    public static void Spawned_Postfix(PlayerVoice __instance)
    {
        Plugin.Logger.LogInfo("[VoicePatch] PlayerVoice spawned, scheduling permission refresh...");
        __instance.StartCoroutine(DelayedPermissionRefresh());
    }

    private static IEnumerator DelayedPermissionRefresh()
    {
        // Wait for LinkDissonanceComms coroutine and permission RPCs to settle
        yield return new WaitForSeconds(5f);

        var allVoices = Object.FindObjectsOfType<PlayerVoice>();
        if (allVoices == null)
            yield break;

        Plugin.Logger.LogInfo($"[VoicePatch] Refreshing voice permissions for {allVoices.Length} player(s)");

        foreach (var voice in allVoices)
        {
            if (voice == null) continue;
            try
            {
                voice.RefreshCommunicationWithPermissions();
            }
            catch (Exception e)
            {
                Plugin.Logger.LogWarning($"[VoicePatch] Refresh failed: {e.Message}");
            }
        }
    }
}
