using HarmonyLib;

namespace ZjaveWorkshopModNS
{
  class Patches
  {

    [HarmonyPatch(typeof(ResourceChest), nameof(ResourceChest.UpdateCard))]
    [HarmonyPostfix]
    public static void PostUpdateCard(ResourceChest __instance)
    {
      UnityEngine.Debug.Log("ResourceCount2: " + __instance.ResourceCount);
      UnityEngine.Debug.Log("SpecialIcon2: " + __instance.SpecialIcon);
      // 获取ZjaveWorkshopModNS下所以FoodChest类实例
      var foodChests = UnityEngine.Object.FindObjectsOfType<FoodChest>();
      foreach (var foodChest in foodChests)
      {
        // 设置foodChest的SpecialIcon
        foodChest.SpecialIcon = __instance.SpecialIcon;
      }
      
    }

  }
}