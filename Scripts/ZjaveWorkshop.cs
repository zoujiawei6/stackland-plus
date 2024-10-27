
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
      return anyMatch || otherCard.Id == Cards.villager || otherCard.Id == "zjave_worker";
    }

    public override void UpdateCard()
    {
      // haveCards是卡片需要哪些材料才能进行制作，此处进行判断
      bool allMatch = haveCards != null && haveCards.All(kvp =>
          ChildrenMatchingPredicateCount((CardData cd) => cd.Id == kvp.Key) >= kvp.Value
      );
      bool hasVillager = ChildrenMatchingPredicateCount((CardData cd) => cd.Id == Cards.villager) >= 1;
      bool hasWorker = ChildrenMatchingPredicateCount((CardData cd) => cd.Id == "zjave_worker") >= 1;
      if (allMatch && (hasVillager || hasWorker))
      {
          MyGameCard.StartTimer(working_time, CompleteMaking, SokLoc.Translate(card_status), GetActionId("CompleteMaking"));
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
      WorldManager.instance.StackSend(cardData.MyGameCard, MyGameCard);
    }
  }
}
