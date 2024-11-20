using UnityEngine;

namespace ZjaveStacklandsPlus.Scripts.Utils
{
  class CardUtils
  {
    public static List<CardData> GetChildrenById(CardData? cardData, string? cardId)
    {
      if (cardData == null || cardId == null || cardId.Length == 0)
      {
        return [];
      }
      List<CardData> list = cardData.ChildrenMatchingPredicate((CardData c) => c.Id == cardId);
      return list;
    }

    public static CardData? GetFirstChildrenById(CardData? cardData, string? cardId)
    {
      List<CardData> list = GetChildrenById(cardData, cardId);
      if (list.Count == 0)
      {
        return null;
      }
      return list[0];
    }

    public static int GetFoodCount()
    {
      int foodCount = 0;
      List<Food> cards = WorldManager.instance.GetCards<Food>();
      foreach (Food item in cards)
      {
        foodCount += item.FoodValue;
      }

      return foodCount;
    }

    public static bool IsFoodById(string cardId)
    {
      switch (cardId)
      {
        case "apple":
        case "banana":
        case "beer":
        case "berry":
        case "bottle_of_rum":
        case "bottle_of_water":
        case "bread":
        case "cane_sugar":
        case "carrot":
        case "ceviche":
        case "chili_pepper":
        case "cooked_crab":
        case "cooked_meat":
        case "dough":
        case "egg":
        case "fish_and_chips":
        case "flour":
        case "french_fries":
        case "fried_fish":
        case "fried_meat":
        case "frittata":
        case "fruit_salad":
        case "grape":
        case "grilled_fish":
        case "herbal_tea":
        case "herbs":
        case "lime":
        case "milk":
        case "milkshake":
        case "mushroom":
        case "olive":
        case "olive_oil":
        case "omelette":
        case "onion":
        case "pancakes":
        case "pizza":
        case "potato":
        case "raw_crab_meat":
        case "raw_fish":
        case "raw_meat":
        case "royal_banquet":
        case "seafood_stew":
        case "seaweed":
        case "stew":
        case "sushi":
        case "tamago_sushi":
        case "tomato":
        case "water":
        case "wheat":
        case "wine":
          return true;
        default:
          return false;
      }
    }

    /// <summary>
    /// 找到蓝图的子输出
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="RequiredCards"></param>
    /// <returns></returns>
    public static Subprint? FindMatchingPrint(string cardId, string[] RequiredCards)
    {
      foreach (Blueprint blueprintPrefab in WorldManager.instance.BlueprintPrefabs)
      {
        // TODO 测试代码
        Debug.LogFormat("FindMatchingPrint {0}", blueprintPrefab.Id);
        Debug.LogFormat("FindMatchingPrint {0}", blueprintPrefab.Icon);
        if (blueprintPrefab.CardId != cardId)
        {
          continue;
        }

        List<Subprint> Subprints = blueprintPrefab.Subprints;
        foreach (Subprint subprint in Subprints)
        {
          Debug.LogFormat("FindMatchingPrint {0}", string.Join(",", subprint.RequiredCards));
          Debug.LogFormat("FindMatchingPrint {0}", string.Join(",", subprint.ResultCard));
          if (subprint.RequiredCards.SequenceEqual(RequiredCards))
          {
            return subprint;
          }
        }
      }

      return null;
    }

    public static CardData FindChildrenById(CardData cardData, string cardId) {
      List<CardData> cardDatas = cardData.ChildrenMatchingPredicate((CardData cd) => cd.Id == cardId);
      return cardDatas.First();
    }

    public static List<BaseVillager> FindVillager(CardData cardData) {
      List<CardData> cardDatas = cardData.ChildrenMatchingPredicate((CardData cd) => cd is BaseVillager);
      return cardDatas
        .Select(cd => (cd as BaseVillager)!)
        .ToList();
    }

    public static float? GetWorkTime(CardData cardData, string resultCardId, string toolCardId, string producerCardId, List<BaseVillager>? baseVillagers) {
      baseVillagers ??= FindVillager(cardData);
      // TODO 测试一下能不能找铁矿石的生产蓝图的子输出
      Subprint? subprint = FindMatchingPrint(resultCardId, [toolCardId, producerCardId]);
      // 铁矿石的生产时间
      BaseVillager miner = baseVillagers.Find(bv => bv.Id == producerCardId);
      CardData mine = FindChildrenById(cardData, toolCardId);
      // TODO 测试用，实际情况下必定能找到蓝图
      if (miner == null || mine == null || subprint == null) {
        Debug.LogFormat("没有找到铁矿石的生产蓝图 {0} {1} {2}", miner, mine, subprint);
        return null;
      }
      // 获取矿工的生产系数
      // 详见 InitActionTimeBases，目前来说，本类只支持铁矿参与自动化
      float timeFactor = miner.GetActionTimeModifier("finish_blueprint", mine);
      float subprintTime = subprint.Time;
      float workingTime = subprintTime * timeFactor;
      return workingTime;
    }

    public static float GetIronOreWorkingTimeByMiner(CardData cardData, List<BaseVillager>? baseVillagers) {
      return GetWorkTime(cardData, Cards.iron_ore, Cards.miner, Cards.mine, baseVillagers) ?? 45;
    }

    public static float GetWoodWorkingTimeByLumberjack(CardData cardData, List<BaseVillager>? baseVillagers) {
      return GetWorkTime(cardData, Cards.wood, Cards.lumberjack, Cards.lumber, baseVillagers) ?? 15;
    }

    public static float GetIronBarWorkingTime() {
      Subprint? subprint = FindMatchingPrint(Cards.iron_bar, [Cards.smelter, Cards.iron_ore, Cards.wood]);
      return subprint?.Time ?? 10;
    }
  }

}