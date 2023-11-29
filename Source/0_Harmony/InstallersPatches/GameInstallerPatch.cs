using System;
using BeatLeader.Installers;
using BeatLeader.Utils;
using HarmonyLib;
using JetBrains.Annotations;

namespace BeatLeader {
    [HarmonyPatch(typeof(GameplayCoreInstaller), "InstallBindings")]
    public static class GameInstallerPatch {
        [UsedImplicitly]
        // ReSharper disable once InconsistentNaming
        private static void Postfix(GameplayCoreInstaller __instance) {
            try {
                var container = __instance.GetContainer();
                OnGameplayCoreInstaller.Install(container);
            } catch (Exception ex) {
                Plugin.Log.Critical($"---\nGameInstaller exception: {ex.Message}\n{ex.StackTrace}\n---");
            }
        }
    }
}