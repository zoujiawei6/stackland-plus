using Newtonsoft.Json;
using UnityEngine;
using ZjaveStacklandsPlus.Scripts.Common;
using ZjaveStacklandsPlus.Scripts.Utils;

namespace ZjaveStacklandsPlus.Scripts.Workshop
{

  public class WorkerWorkshop : ZjaveWorkshop
  {
    public static string cardId = "zjave_worker_workshop";
    public static string blueprintId = "zjave_blueprint_worker_workshop";
    public WorkerWorkshop() : base("worker", "worker", 20, new Dictionary<string, int> {
      { Cards.villager, 1 },
      { "zjave_worker", 1 },
      { Cards.gold, 5 },
    })
    {}

    /// <summary>
    /// 能放置到当前卡片上的卡牌类型
    /// </summary>
    /// <param name="otherCard"></param>
    /// <returns></returns>
    protected override bool CanHaveCard(CardData otherCard)
    {
      // 如果工人已达最大等级，则无法放入人才市场培训
      if (otherCard is IWorkLevel workLevel && workLevel.WorkLevel == IWorkLevel.MaxWorkLevel)
      {
        return false;
      }

      if (haveCards == null) return false;

      bool anyMatch = haveCards.Any(kvp => string.Equals(otherCard.Id, kvp.Key));
      return anyMatch;
    }

    /// <summary>
    /// 判断卡片是否符合制作条件
    /// </summary>
    /// <returns></returns>
    public override bool AccordWithMaking()
    {
      // haveCards是卡片需要哪些材料才能进行制作，此处进行判断
      bool allMatch = haveCards != null && haveCards.All(kvp =>
          ChildrenMatchingPredicateCount((CardData cd) => cd.Id == kvp.Key) >= kvp.Value
      );
      return allMatch;
    }

    /// <summary>
    /// 人才市场培训人才的时间无法受工人等级加成
    /// </summary>
    /// <param name="workingTime">工作所需时间</param>
    /// <param name="outWorkLevel">抛出取得的工作等级卡牌</param>
    /// <returns></returns>
    public override float WorkingTimeBonus(float workingTime, out IWorkLevel? outWorkLevel)
    {
      CardData? workerCardData = CardUtils.GetFirstCardById(this, "zjave_worker");
      if (workerCardData != null && workerCardData is IWorkLevel workLevel)
      {
        this.workLevel = workLevel;
        outWorkLevel = workLevel;
        return workingTime;
      }
      else
      {
        outWorkLevel = null;
        return workingTime;
      }
    }

    [TimedAction("complete_making")]
    public override void CompleteMaking()
    {
      foreach (var kvp in haveCards)
      {
        if (kvp.Key == "gold")
        {
          // 工人每升一级所需金币数是：当前等级*5
          int level = workLevel?.WorkLevel ?? 1;
          Debug.unityLogger.Log(LogType.Log, "kvp.Value * level = " + kvp.Value * level);
          MyGameCard.GetRootCard().CardData.DestroyChildrenMatchingPredicateAndRestack((CardData c) => c.Id == kvp.Key, kvp.Value * level);
        }
        else if (kvp.Key == "worker")
        {
          // 不删除工人，而是升级工人
          continue;
        }
        else
        {
          MyGameCard.GetRootCard().CardData.DestroyChildrenMatchingPredicateAndRestack((CardData c) => c.Id == kvp.Key, kvp.Value);
        }
      }

      CardData cardData = CreateWorkerCard();
      Debug.unityLogger.Log(LogType.Log, "cardData = " + cardData.Id);
      WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir, MyGameCard);
    }

    private CardData CreateWorkerCard()
    {
      if (workLevel is CardData cardData)
      {
        workLevel.LevelUp();
        Debug.unityLogger.Log(LogType.Log, "workLevel = " + workLevel.WorkLevel);
        return cardData;
      }
      else
      {
        return WorldManager.instance.CreateCard(transform.position, resultCard, faceUp: false, checkAddToStack: false);
      }
    }
  }

}