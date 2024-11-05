
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
    public float workingTime = 10;
    public static Dictionary<string, int> superGardenHaveCards = new() { { Cards.garden, 1 } };
    public static Dictionary<string, int> superFarmHaveCards = new() { { Cards.farm, 1 } };
    public static Dictionary<string, int> superGreenhouseHaveCards = new() { { Cards.greenhouse, 1 } };
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
      return superGardenHaveCards.Any(kvp => otherCard.Id == kvp.Key)
        || superFarmHaveCards.Any(kvp => otherCard.Id == kvp.Key)
        || superGreenhouseHaveCards.Any(kvp => otherCard.Id == kvp.Key);
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
      return AllChildrenMatchingPredicateCount(superGardenHaveCards)
        || AllChildrenMatchingPredicateCount(superFarmHaveCards)
        || AllChildrenMatchingPredicateCount(superGreenhouseHaveCards);
    }

    protected virtual bool AllChildrenMatchingPredicateCount(Dictionary<string, int> haveCards)
    {
      return haveCards.All(kvp => ChildrenMatchingPredicateCount((CardData cd) => cd.Id == kvp.Key) >= kvp.Value);
    }

    public override void UpdateCard()
    {
      if (AccordWithMaking())
      {
        MyGameCard.StartTimer(workingTime, CompleteMaking, SokLoc.Translate(statusId), GetActionId("CompleteMaking"));
      }
      else
      {
        MyGameCard.CancelTimer(GetActionId("CompleteMaking"));
      }
      base.UpdateCard();
    }

    public override void UpdateCardText()
    {
      string countGardenInfo = SokLoc.Translate("zjave_destory_garden_total", LocParam.Create("count", (6 - destroyGardenCount).ToString()));
      string countFarmInfo = SokLoc.Translate("zjave_destory_farm_total", LocParam.Create("count", (6 - destroyFarmCount).ToString()));
      string countGreenhouseInfo = SokLoc.Translate("zjave_destory_greenhouse_total", LocParam.Create("count", (6 - destroyGreenhouseCount).ToString()));
      descriptionOverride = $"{countGardenInfo}\n{countFarmInfo}\n{countGreenhouseInfo}";
    }

    public override bool CanHaveCardsWhileHasStatus()
    {
      return true;
    }

    [TimedAction("complete_making")]
    public virtual void CompleteMaking()
    {
      if (AllChildrenMatchingPredicateCount(superGardenHaveCards))
      {
        CompleteSuperMaking(superGardenHaveCards, Cards.garden);
      }

      if (AllChildrenMatchingPredicateCount(superFarmHaveCards))
      {
        CompleteSuperMaking(superFarmHaveCards, Cards.farm);
      }

      if (AllChildrenMatchingPredicateCount(superGreenhouseHaveCards))
      {
        CompleteSuperMaking(superGreenhouseHaveCards, Cards.greenhouse);
      }
    }

    private void CompleteSuperMaking(Dictionary<string, int> haveCards, string resultCard)
    {
      foreach (var kvp in haveCards)
      {
        DestroyCardByIdFormWorkshop(kvp.Key, kvp.Value);
        DestroyCount(kvp.Key, kvp.Value);
      }

      if (resultCard == SuperGarden.cardId && destroyGardenCount != 0 && destroyGardenCount % 6 != 0)
      {
        return;
      }
      if (resultCard == SuperFarm.cardId && destroyGardenCount != 0 && destroyGardenCount % 6 != 0)
      {
        return;
      }
      if (resultCard == SuperGreenhouse.cardId && destroyGardenCount != 0 && destroyGardenCount % 6 != 0)
      {
        return;
      }
      CardData cardData = WorldManager.instance.CreateCard(transform.position, resultCard, faceUp: false, checkAddToStack: false);
      WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir, MyGameCard);
    }

    public virtual void DestroyCardByIdFormWorkshop(string cardId, int count)
    {
      MyGameCard.GetRootCard().CardData.DestroyChildrenMatchingPredicateAndRestack((CardData c) => c.Id == cardId, count);
    }
  }
}