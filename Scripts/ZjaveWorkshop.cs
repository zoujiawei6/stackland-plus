using ZjaveStacklandsPlus.Scripts.Utils;

namespace ZjaveStacklandsPlus.Scripts
{
  /// <summary>
  /// 工坊顶层类
  /// </summary>
  /// <param name="ingredient">工坊关键字，用于创建卡片id</param>
  /// <param name="workingTime">制作所需时间</param>
  /// <param name="resultCard">工坊制作出的卡片</param>
  /// <param name="haveCards">制作所需的材料卡片，及其所需数量列表</param>
  public class ZjaveWorkshop(string ingredient, string resultCard, float workingTime, Dictionary<string, int> haveCards) : CardData
  {
    protected string cardStatus = string.Format("zjave_{0}_workshop_status", ingredient);
    public string ingredient = ingredient;
    public string resultCard = resultCard;
    public float workingTime = workingTime;
    protected Dictionary<string, int> haveCards = haveCards;

    /// <summary>
    /// 判断传入的卡片是否是人类
    /// </summary>
    /// <param name="otherCard"></param>
    /// <returns></returns>
    public virtual bool CanHaveHuman(CardData otherCard)
    {
      return otherCard.MyCardType == CardType.Humans;
    }

    /// <summary>
    /// 能放置到当前卡片上的卡牌类型
    /// </summary>
    /// <param name="otherCard"></param>
    /// <returns></returns>
    protected override bool CanHaveCard(CardData otherCard)
    {
      if (haveCards == null) return false;

      bool anyMatch = haveCards.Any(kvp => otherCard.Id == kvp.Key);
      return anyMatch || CanHaveHuman(otherCard);
    }

    /// <summary>
    /// 判断卡片是否符合制作条件
    /// </summary>
    /// <returns></returns>
    public virtual bool AccordWithMaking()
    {
      // haveCards是卡片需要哪些材料才能进行制作，此处进行判断
      bool allMatch = haveCards != null && haveCards.All(kvp =>
          ChildrenMatchingPredicateCount((CardData cd) => cd.Id == kvp.Key) >= kvp.Value
      );
      bool hasHuman = AnyChildMatchesPredicate(CanHaveHuman);
      // 食物类作坊不需要村民，其它类型作坊需要
      return allMatch && (hasHuman || CardUtils.IsFoodById(resultCard));
    }

    public override void UpdateCard()
    {
      if (AccordWithMaking())
      {
        MyGameCard.StartTimer(workingTime, CompleteMaking, SokLoc.Translate(cardStatus), GetActionId("CompleteMaking"));
      }
      else
      {
        MyGameCard.CancelTimer(GetActionId("CompleteMaking"));
      }
      base.UpdateCard();
    }

    public override bool CanHaveCardsWhileHasStatus()
    {
      return true;
    }

    [TimedAction("complete_making")]
    public virtual void CompleteMaking()
    {
      foreach (var kvp in haveCards)
      {
        DestroyCardByIdFormWorkshop(kvp.Key, kvp.Value);
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
