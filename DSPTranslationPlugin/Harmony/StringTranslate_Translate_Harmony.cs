﻿using System.Linq;
using HarmonyLib;
using TranslationCommon;
using UnityEngine;

namespace DSPSimpleBuilding
{
    [HarmonyPatch(typeof(StringTranslate), nameof(StringTranslate.Translate), typeof(string))]
    public static class StringTranslate_Translate_Harmony
    {
        /// <summary>
        ///     Translate requested text
        /// </summary>
        /// <param name="__result">Returned result</param>
        /// <param name="s">Input NAME text</param>
        /// <returns></returns>
        [HarmonyPrefix]
        public static bool Prefix(ref string __result, string s)
        {
            if (s == null)
            {
                return true;
            }

            if (TranslationManager.CurrentLanguage != null)
            {
                if (TranslationManager.TranslationDictionary.ContainsKey(s))
                {
                    __result = TranslationManager.TranslationDictionary[s].Translation;
                    return false;
                }

                return true;
            }
            
            return true;
        }

        /// <summary>
        ///     Add in game credits
        /// </summary>
        /// <param name="__result"></param>
        /// <param name="s"></param>
        [HarmonyPostfix]
        public static void Postfix(ref string __result, string s)
        {
            if (s == "需要重启完全生效")
            {
                __result +=
                    "\nTranslation tool made by Muchaszewski with the help of community" +
                    "\nhttps://github.com/Muchaszewski/DSP_TranslationMod";
            }
        }
    }
    
    /// <summary>
    ///     Localizer postfix for expanding translation box to match new credits text
    /// </summary>
    [HarmonyPatch(typeof(Localizer), "Refresh")]
    public static class Localizer_Refresh_Postfix
    {
        /// <summary>
        ///     Expand in game credits box
        /// </summary>
        /// <param name="__instance"></param>
        [HarmonyPostfix]
        public static void Postfix(Localizer __instance)
        {
            if (__instance.name == "tip" && __instance.transform.parent.name == "language")
            {
                var rect = __instance.GetComponent<RectTransform>();
                var sizeDelta = rect.sizeDelta;
                sizeDelta.x = 600;
                sizeDelta.y = 90;
                rect.sizeDelta = sizeDelta;
            }
            
        }
    }
}