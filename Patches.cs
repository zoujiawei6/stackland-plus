using HarmonyLib;
using UnityEngine;
using ZjaveStacklandsPlus.Scripts;
using ZjaveStacklandsPlus.Scripts.Utils;

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

    /// <summary>
    /// 在统计食物前弹出食物箱里的食物
    /// </summary>
    /// <param name="__instance"></param>
    [HarmonyPatch(typeof(EndOfMonthCutscenes), nameof(EndOfMonthCutscenes.FeedVillagers))]
    [HarmonyPrefix]
    public static void FeedVillagers()
    {
      try
      {
        int requiredFoodCount = WorldManager.instance.GetRequiredFoodCount();
        int foodCount = CardUtils.GetFoodCount();
        int shortage = requiredFoodCount - foodCount;
        if (shortage <= 0)
        {
          return;
        }

        List<FoodChest> cards = WorldManager.instance.GetCards<FoodChest>();
        foreach (FoodChest item in cards)
        {
          CardData cardFromId = WorldManager.instance.GameDataLoader.GetCardFromId(item.HeldCardId);
          if (cardFromId is not Food food)
          {
            Debug.LogFormat("食物箱里的卡牌不是食物 {0}, {1}", item.HeldCardId, cardFromId.name);
            continue;
          }
          // 统计箱子里的食物值总数
          int count = food.FoodValue * item.ResourceCount;
          int differ = shortage - count;
          if (differ >= 0)
          {
            // 箱子里的食物不够弥补或恰好弥补缺口时，弹出箱子里的全部食物
            Debug.LogFormat("弹出食物箱里的食物 {0} {1}", food.FoodValue, item.ResourceCount);
            item.RemoveResources(item.ResourceCount);
            shortage = differ;
            item.Clicked();
            continue;
          }
          else if (differ < 0)
          {
            Debug.LogFormat("弹出食物缺口食物 {0} {1}", food.FoodValue, shortage);
            // 否则弹出食物的缺口数量的食物值，换算成卡牌时需要向上取整食物才够
            int require = Mathf.Max(1, (int)Mathf.Ceil(shortage / food.FoodValue));
            item.RemoveResources(require);
            break;
          }
        }
      }
      catch (Exception e)
      {
        Debug.LogErrorFormat("统计食物前弹出食物出错 {0}", e);
      }
    }

  }
}