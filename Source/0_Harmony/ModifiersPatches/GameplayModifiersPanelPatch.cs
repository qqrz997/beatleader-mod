﻿using System;
using System.Collections.Generic;
using BeatLeader.Models;
using BeatLeader.Utils;
using HarmonyLib;
using JetBrains.Annotations;
using BGLib.Polyglot;
using TMPro;
using UnityEngine;
using ModifiersCore;

namespace BeatLeader {
    [HarmonyPatch(typeof(GameplayModifiersPanelController), "RefreshTotalMultiplierAndRankUI")]
    internal static class GameplayModifiersPanelPatch {
        public static event Action<GameplayModifiers>? ModifiersChangedEvent;

        public static bool isPatchRequired = false;
        public static bool hasModifiers = false;
        public static Dictionary<string, float> ModifiersMap;
        public static Dictionary<string, float>? ModifiersRating;
        
        [UsedImplicitly]
        private static void Postfix(
            GameplayModifiersPanelController __instance,
            Color ____positiveColor,
            Color ____negativeColor,
            GameplayModifiersModelSO ____gameplayModifiersModel,
            GameplayModifiers ____gameplayModifiers,
            TextMeshProUGUI ____totalMultiplierValueText,
            TextMeshProUGUI ____maxRankValueText
        ) {
            if (!isPatchRequired || !hasModifiers) return;
            
            List<GameplayModifierParamsSO> modifierParams = ____gameplayModifiersModel.CreateModifierParamsList(____gameplayModifiers);
            float totalMultiplier = 1;

            foreach (var param in modifierParams) {
                if (param.multiplierConditionallyValid) continue; // for now only NoFail being ignored

                var key = ModifiersManager.defaultModifierIds.ContainsKey(param.modifierNameLocalizationKey) ? ModifiersManager.defaultModifierIds[param.modifierNameLocalizationKey] : param.modifierNameLocalizationKey;
                
                if (ModifiersMap.ContainsKey(key.ToLower())) {
                    totalMultiplier += ModifiersMap[key.ToLower()];
                }
            }

            if (totalMultiplier < 0) totalMultiplier = 0; // thanks Beat Games for Zen mode -1000%

            var color = (totalMultiplier >= 1f) ? ____positiveColor : ____negativeColor;
            ____totalMultiplierValueText.text = string.Format(Localization.Instance.SelectedCultureInfo, "{0:P0}", totalMultiplier);
            ____totalMultiplierValueText.color = color;

            var rank = ModifiersUtils.GetRankForMultiplier(totalMultiplier);
            ____maxRankValueText.text = rank;
            ____maxRankValueText.color = color;
            
            ModifiersChangedEvent?.Invoke(____gameplayModifiers);
        }
    }
}