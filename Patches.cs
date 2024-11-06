using HarmonyLib;
using UnityEngine;

namespace ZjaveStacklandsPlus
{
  class Patches
  {

    // [HarmonyPatch(typeof(ResourceChest), nameof(ResourceChest.UpdateCard))]
    // [HarmonyPostfix]
    // public static void PostUpdateCard(ResourceChest __instance)
    // {
    //   var foodChests = UnityEngine.Object.FindObjectsOfType<FoodChest>();
    //   foreach (var foodChest in foodChests)
    //   {
    //     foodChest.SpecialIcon = __instance.SpecialIcon;
    //   }
    // }

    // [HarmonyPatch(typeof(CardData), nameof(CardData.FinishBlueprint))]
    // [HarmonyPostfix]
    // public static void PostUpdateCard(CardData __instance)
    // {
    //   Debug.LogFormat("222 finish blueprint {0}", __instance.MyGameCard.TimerBlueprintId);
    // }

  }
}