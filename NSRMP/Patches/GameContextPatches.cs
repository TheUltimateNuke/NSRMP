using HarmonyLib;
using UnityEngine;

namespace NSRMP.Patches;

internal static class GameContextPatches
{
    [HarmonyPatch(typeof(GameContext), nameof(GameContext.Awake))]
    private static class GameContext_Awake
    {
        private static void Postfix()
        {
            CommonEvents.OnGameContextAwake?.Invoke();
        }
    }
}