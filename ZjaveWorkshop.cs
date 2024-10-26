
namespace ZjaveWorkshopModNS
{
  /// <summary>
  /// 工坊顶层类
  /// </summary>
  /// <param name="ingredient">工坊关键字，用于创建卡片id</param>
  /// <param name="resultCard">工坊制作出的卡片</param>
  /// <param name="haveCards">制作所需的材料卡片，及其所需数量列表</param>
  public class ZjaveWorkshop(string ingredient, string resultCard, Dictionary<string, int> haveCards) : CardData
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
      if (allMatch && ChildrenMatchingPredicateCount((CardData cd) => cd.Id == Cards.villager) >= 1)
      {
        MyGameCard.StartTimer(10f, CompleteMaking, SokLoc.Translate(card_status), GetActionId("CompleteMaking"));
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
