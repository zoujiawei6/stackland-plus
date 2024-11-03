
namespace ZjaveStacklandsPlus.Scripts
{
  /// <summary>
  /// 工坊顶层类
  /// </summary>
  /// <param name="ingredient">工坊关键字，用于创建卡片id</param>
  /// <param name="working_time">制作所需时间</param>
  /// <param name="resultCard">工坊制作出的卡片</param>
  /// <param name="haveCards">制作所需的材料卡片，及其所需数量列表</param>
  public class ZjaveWorkshop(string ingredient, string resultCard, float working_time, Dictionary<string, int> haveCards) : CardData
  {
    protected string card_id = string.Format("zjave_{0}_workshop", ingredient);
    protected string card_status = string.Format("zjave_{0}_workshop_status", ingredient);

    protected override bool CanHaveCard(CardData otherCard)
    {
      if (haveCards == null) return false;

      bool anyMatch = haveCards.Any(kvp => otherCard.Id == kvp.Key);
      return anyMatch || otherCard.Id == Cards.villager;
    }

    public override void UpdateCard()
    {
      bool allMatch = haveCards != null && haveCards.All(kvp =>
          ChildrenMatchingPredicateCount((CardData cd) => cd.Id == kvp.Key) >= kvp.Value
      );
      bool hasHumans = ChildrenMatchingPredicateCount((CardData cd) => cd.MyCardType == CardType.Humans) >= 1;
      if (haveCards != null && haveCards.ContainsKey("apple") && haveCards.ContainsKey("berry") && haveCards.ContainsKey("milk"))
      {
        hasHumans = false;
      }

      if (allMatch && haveCards != null)
      {
        // 如果包含apple, berry, milk则不用包含村民
        // TODO 本意是所有作坊都需要村民才能生产，但食物类卡牌无法放置在村民卡牌上；
        // TODO 为了避免修改原游戏的村民类，应该先制作工人卡牌，将村民转变为工人，然后修改工人的逻辑，让工人可以放置食物；而工人也是村民的一种
        if (haveCards.ContainsKey("apple") || haveCards.ContainsKey("berry") || haveCards.ContainsKey("milk"))
        {
          MyGameCard.StartTimer(working_time, CompleteMaking, SokLoc.Translate(card_status), GetActionId("CompleteMaking"));
        }
        // 否则需要村民才能生产
        else if (hasHumans)
        {
          MyGameCard.StartTimer(working_time, CompleteMaking, SokLoc.Translate(card_status), GetActionId("CompleteMaking"));
        }
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
      if (haveCards != null)
      {
        foreach (var kvp in haveCards)
        {
          MyGameCard.GetRootCard().CardData.DestroyChildrenMatchingPredicateAndRestack((CardData c) => c.Id == kvp.Key, kvp.Value);
        }
      }

      CardData cardData = WorldManager.instance.CreateCard(transform.position, resultCard, faceUp: false, checkAddToStack: false);
      WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir, MyGameCard);
    }
  }
}
