
using ZjaveStacklandsPlus.Scripts.Common;
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
      bool hasVillager = AnyChildMatchesPredicate((CardData cd) => cd.Id == Cards.villager);
      bool hasWorker = AnyChildMatchesPredicate((CardData cd) => cd.Id == "zjave_worker");

      // 更高等级的工人能更快生产
      CardData? workerCardData = CardUtils.GetFirstCardById(this, "zjave_worker");
      if (workerCardData != null && workerCardData is IWorkLevel workLevel)
      {
        int level = workLevel.GetWorkLevel();
        workingTime = WorkingTimeBonus(level, workingTime);
      }
      
      if (allMatch && (hasVillager || hasWorker))
      {
        // TODO 工人具有更高的熟练度，可以更快生产
        MyGameCard.StartTimer(workingTime, CompleteMaking, SokLoc.Translate(card_status), GetActionId("CompleteMaking"));
      }
      else
      {
        MyGameCard.CancelTimer(GetActionId("CompleteMaking"));
      }
      base.UpdateCard();
    }

    /// <summary>
    /// 工作时间加成，熟练的工人具有更高的工作效率
    /// </summary>
    /// <param name="level">当前工作等级</param>
    /// <param name="workingTime">工作所需时间</param>
    /// <returns></returns>
    public virtual float WorkingTimeBonus(int level, float workingTime)
    {
      return MathUtils.CalculateProductionTime(level, workingTime);
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
