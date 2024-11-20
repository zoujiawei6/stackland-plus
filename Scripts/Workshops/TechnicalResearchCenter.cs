
using UnityEngine;

namespace ZjaveStacklandsPlus.Scripts.Workshops
{
  /// <summary>
  /// 技术研究中心
  /// </summary>
  /// <param name="ingredient">工坊关键字，用于创建卡片id</param>
  /// <param name="workingTime">制作所需时间</param>
  /// <param name="resultCard">工坊制作出的卡片</param>
  /// <param name="haveCards">制作所需的材料卡片，及其所需数量列表</param>
  public class TechnicalResearchCenter : CardData
  {
    public static string cardId = "zjave_technical_research_center";
    public static string statusId = "zjave_technical_research_center_status";
    public static string blueprintId = "zjave_blueprint_technical_research_center";
    public static float workingTime = 10;
    // 从超级花园升级超级农场和从超级农场升级超级温室所需时间
    public static float upgradeTime = 30;
    public static int synthesisQuantity = 6;

    [ExtraData("DestroyGardenCount")]
    public int destroyGardenCount = 0;
    [ExtraData("DestroyFarmCount")]
    public int destroyFarmCount = 0;
    [ExtraData("DestroyGreenhouseCount")]
    public int destroyGreenhouseCount = 0;

    [Term]
    public new string NameTerm = "zjave_technical_research_center_name";
    [Term]
    public new string DescriptionTerm = "zjave_technical_research_center_description";

    /// <summary>
    /// 能放置到当前卡片上的卡牌类型
    /// </summary>
    /// <param name="otherCard"></param>
    /// <returns></returns>
    protected override bool CanHaveCard(CardData otherCard)
    {
      return otherCard.Id == Cards.garden
        || otherCard.Id == Cards.farm
        || otherCard.Id == Cards.greenhouse
        || otherCard.Id == SuperGarden.cardId
        || otherCard.Id == SuperFarm.cardId
        || otherCard.Id == Cards.iron_bar
        || otherCard.Id == Cards.glass;
    }

    /// <summary>
    /// 统计已销毁的花园、农场、温室的数量
    /// </summary>
    /// <param name="cardId"></param>
    /// <param name="count"></param>
    public void DestroyCount(string cardId, int count)
    {
      if (cardId == Cards.garden)
      {
        destroyGardenCount += count;
      }
      else if (cardId == Cards.farm)
      {
        destroyFarmCount += count;
      }
      else if (cardId == Cards.greenhouse)
      {
        destroyGreenhouseCount += count;
      }
    }

    /// <summary>
    /// 判断卡片是否符合制作条件
    /// </summary>
    /// <returns></returns>
    public virtual bool AccordWithMaking()
    {
      return AnyChildMatchesPredicate((CardData cd) => cd.Id == Cards.garden)
        || AnyChildMatchesPredicate((CardData cd) => cd.Id == Cards.farm)
        || AnyChildMatchesPredicate((CardData cd) => cd.Id == Cards.greenhouse);
    }

    /// <summary>
    /// 判断是否符合升级条件（将超级花园升级成超级农场，或将超级农场升级成超级温室）。
    /// 升级条件要求铁块和玻璃个十二个，且有超级农场或超级温室卡片放置在研发中心上。
    /// </summary>
    /// <param name="cardId"></param>
    /// <returns></returns>
    public virtual bool AccordUpgrade(out string? cardId)
    {
      int total = 2 * synthesisQuantity;
      bool haveCard = false;
      if (AnyChildMatchesPredicate((CardData cd) => cd.Id == SuperGarden.cardId))
      {
        haveCard = true;
        cardId = SuperGarden.cardId;
      }
      else if (AnyChildMatchesPredicate((CardData cd) => cd.Id == SuperFarm.cardId))
      {
        haveCard = true;
        cardId = SuperFarm.cardId;
      }
      else
      {
        cardId = null;
        return false;
      }
      Debug.LogFormat("AccordUpgrade {0} {1} {2}", haveCard, cardId, total);
      return haveCard
        && ChildrenMatchingPredicateCount((CardData cd) => cd.Id == Cards.iron_bar) >= total
        && ChildrenMatchingPredicateCount((CardData cd) => cd.Id == Cards.glass) >= total;
    }

    public override void UpdateCard()
    {
      if (AccordUpgrade(out string? cardId) || AccordWithMaking())
      {
        MyGameCard.StartTimer(workingTime, CompleteMaking, SokLoc.Translate(statusId), GetActionId("CompleteMaking"));
      }
      else
      {
        MyGameCard.CancelTimer(GetActionId("CompleteMaking"));
      }
      base.UpdateCard();
    }

    public virtual int getRemainingRequiredCardsById(string cardId)
    {
      if (cardId == Cards.garden)
      {
        return synthesisQuantity - (destroyGardenCount % synthesisQuantity);
      }
      else if (cardId == Cards.farm)
      {
        return synthesisQuantity - (destroyFarmCount % synthesisQuantity);
      }
      else if (cardId == Cards.greenhouse)
      {
        return synthesisQuantity - (destroyGreenhouseCount % synthesisQuantity);
      }
      return 0;
    }

    public override void UpdateCardText()
    {
      string countGardenInfo = SokLoc.Translate("zjave_destory_garden_total", LocParam.Create("count", getRemainingRequiredCardsById(Cards.garden).ToString()));
      string countFarmInfo = SokLoc.Translate("zjave_destory_farm_total", LocParam.Create("count", getRemainingRequiredCardsById(Cards.farm).ToString()));
      string countGreenhouseInfo = SokLoc.Translate("zjave_destory_greenhouse_total", LocParam.Create("count", getRemainingRequiredCardsById(Cards.greenhouse).ToString()));
      descriptionOverride = $"{countGardenInfo}\n{countFarmInfo}\n{countGreenhouseInfo}";
    }

    public override bool CanHaveCardsWhileHasStatus()
    {
      return true;
    }

    [TimedAction("complete_making")]
    public virtual void CompleteMaking()
    {
      CompleteSuperMaking(Cards.garden, SuperGarden.cardId);
      CompleteSuperMaking(Cards.farm, SuperFarm.cardId);
      CompleteSuperMaking(Cards.greenhouse, SuperGreenhouse.cardId);
      if (AccordUpgrade(out string? cardId) && cardId != null)
      {
        string resultCard = cardId == SuperGarden.cardId ? SuperFarm.cardId : SuperGreenhouse.cardId;
        Debug.LogFormat("DDD CompleteUpgrade {0} {1}", cardId, resultCard);
        CompleteUpgrade(cardId, resultCard);
        return;
      }
    }

    private void CompleteSuperMaking(string haveCard, string resultCard)
    {
      if (!AnyChildMatchesPredicate((CardData cd) => cd.Id == haveCard))
      {
        return;
      }
      DestroyCardByIdFormWorkshop(haveCard, 1);
      DestroyCount(haveCard, 1);

      CompleteSuperMaking(resultCard, SuperGarden.cardId, destroyGardenCount);
      CompleteSuperMaking(resultCard, SuperFarm.cardId, destroyFarmCount);
      CompleteSuperMaking(resultCard, SuperGreenhouse.cardId, destroyGreenhouseCount);
    }

    private void CompleteUpgrade(string haveCard, string resultCard)
    {
      int total = 2 * synthesisQuantity;
      DestroyCardByIdFormWorkshop(Cards.iron_bar, total);
      DestroyCardByIdFormWorkshop(Cards.glass, total);
      DestroyCardByIdFormWorkshop(haveCard, 1);
      Complete(resultCard);
    }

    /// <summary>
    /// 当消耗的生长类卡牌达到指定数量时（如6个），完成一个超级生产类卡牌
    /// </summary>
    private void CompleteSuperMaking(string resultCard, string equalResultCard, int count)
    {
      if (resultCard == equalResultCard && count != 0 && count % synthesisQuantity == 0)
      {
        Complete(resultCard);
        return;
      }
    }

    private void Complete(string resultCard)
    {
      CardData cardData = WorldManager.instance.CreateCard(transform.position, resultCard, faceUp: false, checkAddToStack: false);
      WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir, MyGameCard);
    }

    public virtual void DestroyCardByIdFormWorkshop(string cardId, int count)
    {
      Debug.LogFormat("DestroyCardByIdFormWorkshop {0} {1}", cardId, count);
      MyGameCard.GetRootCard().CardData.DestroyChildrenMatchingPredicateAndRestack((CardData c) => c.Id == cardId, count);
    }
  }
}