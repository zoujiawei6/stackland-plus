using System.Reflection;
using System.Reflection.Emit;
using HarmonyLib;
using UnityEngine;
using ZjaveStacklandsPlus.Scripts;
using ZjaveStacklandsPlus.Scripts.Utils;
using ZjaveStacklandsPlus.Scripts.Workshops;

namespace ZjaveStacklandsPlus
{
  /// <summary>
  /// 这是一个切面类，用于在游戏的某段函数前后插入指定的代码。
  /// 请遵循尽量不改动游戏原有逻辑的原则，且当前类必须捕获处理异常。
  /// </summary>
  class Patches
  {
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
            Debug.LogFormat("弹出食物箱里的食物1 {0} {1}", food.FoodValue, item.ResourceCount);
            item.RemoveResources(item.ResourceCount);
            shortage = differ;
            item.Clicked();
            continue;
          }
          else if (differ < 0)
          {
            Debug.LogFormat("弹出食物箱里的食物2 {0} {1}", food.FoodValue, shortage);
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

    /// <summary>
    /// 允许产线所需建筑在产线卡牌上相互叠放
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="__result"></param>
    [HarmonyPatch(typeof(CardData), "CanHaveCard")]
    [HarmonyPostfix]
    public static void BuildingCanHaveCard(CardData __instance, ref bool __result)
    {
      try
      {
        GameCard LastParent = __instance.MyGameCard.LastParent;
        if (!(LastParent != null && LastParent.CardData != null)) {
          return;
        }
        Debug.LogFormat("BuildingCanHaveCard 111 {0} {1}", __instance.Id, __instance.MyGameCard.LastParent.name);
        // 铁块产线上的指定类型的卡片可以相互堆叠
        if (LastParent.CardData.Id == IronBarWorkshop.cardId && 
          (IronBarWorkshop.CanHaveCardIds.Contains(__instance.Id) 
          || IronBarWorkshop.CanHaveCardTypes.Contains(__instance.MyCardType)))
        {
          // Debug.LogFormat("IronBarWorkshop BuildingCanHaveCard {0} {1}", __instance.Id, __instance.MyGameCard.Parent.name);
          __result = true;
        }
      }
      catch (Exception e)
      {
        Debug.LogErrorFormat("BuildingCanHaveCard Exception {0}", e.Message);
      }
    }

    [HarmonyPatch(typeof(CardData))]
    [HarmonyPatch("CanHaveCard")]
    public static class BuildingCanHaveCardPatch
    {
        [HarmonyPostfix]
        public static void CanHaveCard(CardData __instance, CardData otherCard, ref bool __result)
        {
          // 在每次调用时打印日志或添加其他逻辑
          BuildingCanHaveCard(__instance, ref __result);
        }
    }

    /// <summary>
    /// 允许具有装备栏的村民在产线上相互叠放
    /// </summary>
    /// <param name="__instance"></param>
    /// <param name="__result"></param>
    [HarmonyPatch(typeof(Equipable), "CanHaveCard")]
    [HarmonyPostfix]
    public static void EquipableCanHaveCard(CardData __instance, ref bool __result)
    {
      BuildingCanHaveCard(__instance, ref __result);
    }
  }
}