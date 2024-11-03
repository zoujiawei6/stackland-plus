
using UnityEngine;
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
    protected string cardStatus = string.Format("zjave_{0}_workshop_status", ingredient);
    protected IWorkLevel? workLevel = null;
    public string ingredient = ingredient;
    public string resultCard = resultCard;
    public float workingTime = workingTime;
    private float bonusWorkingTime = 0;
    protected Dictionary<string, int> haveCards = haveCards;

    /// <summary>
    /// 判断传入的卡片是否是村民
    /// </summary>
    /// <param name="otherCard"></param>
    /// <returns></returns>
    public virtual bool CanHaveVillager(CardData otherCard)
    {
      return otherCard.Id == Cards.villager || otherCard.Id == "zjave_worker";
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
      return anyMatch || CanHaveVillager(otherCard);
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
      bool hasVillager = AnyChildMatchesPredicate((CardData cd) => cd.Id == Cards.villager);
      bool hasWorker = AnyChildMatchesPredicate((CardData cd) => cd.Id == "zjave_worker");
      return allMatch && (hasVillager || hasWorker);
    }

    public override void UpdateCard()
    {
      if (AccordWithMaking())
      {
        bonusWorkingTime = WorkingTimeBonus(workingTime, out IWorkLevel? workLevel);
        MyGameCard.StartTimer(bonusWorkingTime, CompleteMaking, SokLoc.Translate(cardStatus), GetActionId("CompleteMaking"));
      }
      else
      {
        MyGameCard.CancelTimer(GetActionId("CompleteMaking"));
      }
      base.UpdateCard();
    }

    /// <summary>
    /// 工作时间加成，熟练的工人具有更高的工作效率。最大加成是2倍效率。
    /// 因为本游戏困难的是初期而非后期，因此采用反对数函数来增加初期的工作效率加成，前期升级加成可观，而对后期的加成不大。
    /// </summary>
    /// <param name="workingTime">工作所需时间</param>
    /// <param name="outWorkLevel">抛出取得的工作等级卡牌</param>
    /// <returns></returns>
    public virtual float WorkingTimeBonus(float workingTime, out IWorkLevel? outWorkLevel)
    {
      // 工人等级越高，效率越高，生产越快。
      CardData? workerCardData = CardUtils.GetFirstCardById(this, Worker.cardId);
      if (workerCardData != null && workerCardData is IWorkLevel workLevel)
      {
        this.workLevel = workLevel;
        int level = workLevel.WorkLevel;
        outWorkLevel = workLevel;

        // 木棍所需原材料少，固定为6秒。否则经济不再是难题
        return this is StickWorkshop ? workingTime : MathUtils.CalculateProductionTime(level, workingTime, 0.5f);
      }
      else
      {
        outWorkLevel = null;
        return workingTime;
      }
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

      Debug.LogFormat("本次生产耗时 = {0}", bonusWorkingTime);
      if (workLevel != null)
      {
        Debug.LogFormat("总工作时间 统计前 = {0}", workLevel?.WorkingTime);
        workLevel?.CountWorkingTime(bonusWorkingTime);
        Debug.LogFormat("总工作时间 统计后 = {0}", workLevel?.WorkingTime);
      }
    }

    public virtual void DestroyCardByIdFormWorkshop(string cardId, int count)
    {
      MyGameCard.GetRootCard().CardData.DestroyChildrenMatchingPredicateAndRestack((CardData c) => c.Id == cardId, count);
    }
  }
}
