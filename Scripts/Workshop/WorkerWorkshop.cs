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
    public WorkerWorkshop() : base("worker", Worker.cardId, 20, new Dictionary<string, int> {
      { Cards.villager, 1 },
      { Worker.cardId, 1 },
      { Cards.gold, 5 },
    })
    { }

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
        Debug.LogFormat("WorkerWorkshop::CanHaveCard: 工人已达最大等级 {0}", workLevel.WorkLevel);
        return false;
      }

      if (haveCards == null) return false;

      bool anyMatch = haveCards.Any(kvp => string.Equals(otherCard.Id, kvp.Key));
      Debug.LogFormat("anyMatch = {0}", anyMatch);
      return anyMatch;
    }

    /// <summary>
    /// 判断卡片是否符合制作条件
    /// </summary>
    /// <returns></returns>
    public override bool AccordWithMaking()
    {
      if (!haveCards.TryGetValue(Cards.villager, out int villagerTotal)
      || !haveCards.TryGetValue(Worker.cardId, out int workerTotal)
      || !haveCards.TryGetValue(Cards.gold, out int goldTotal))
      {
        return false;
      }

      CardData? workerCardData = CardUtils.GetFirstCardById(this, Worker.cardId);
      IWorkLevel? workLevel = workerCardData as IWorkLevel;

      // 工人每升一级所需金币数是：当前等级*5
      int level = workLevel?.WorkLevel ?? 1;
      int upLevelGold = Mathf.Max(5, level * goldTotal);
      // haveCards是卡片需要哪些材料才能进行制作，此处进行判断
      bool allMatch = haveCards != null
        && ChildrenMatchingPredicateCount((CardData cd) => cd.Id == Cards.gold) >= upLevelGold
        && (ChildrenMatchingPredicateCount((CardData cd) => cd.Id == Cards.villager) >= villagerTotal
        || ChildrenMatchingPredicateCount((CardData cd) => cd.Id == Worker.cardId) >= workerTotal);
      Debug.LogFormat("AccordWithMaking = {0}", allMatch);
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
      outWorkLevel = null;
      return workingTime;
    }

    [TimedAction("complete_making")]
    public override void CompleteMaking()
    {
      if (!haveCards.TryGetValue(Cards.villager, out int villagerTotal)
      || !haveCards.TryGetValue(Worker.cardId, out int workerTotal)
      || !haveCards.TryGetValue(Cards.gold, out int goldTotal))
      {
        return;
      }

      CardData? workerCardData = CardUtils.GetFirstCardById(this, Worker.cardId);
      IWorkLevel? workLevel = workerCardData as IWorkLevel;

      // 工人每升一级所需金币数是：当前等级*5
      int level = workLevel?.WorkLevel ?? 1;
      int upLevelGold = Mathf.Max(5, level * goldTotal);
      Debug.LogFormat("Destroy gold -> {0}", upLevelGold);
      DestroyCardByIdFormWorkshop(Cards.gold, upLevelGold);
      
      if (workerCardData != null && workLevel != null)
      {
        // 不Destroy工人，而是升级工人
        Debug.LogFormat("Before LevelUp, workLevel = {0}", workLevel.WorkLevel);
        workLevel.LevelUp();
        Debug.LogFormat("After LevelUp, workLevel = {0}", workLevel.WorkLevel);
        WorldManager.instance.Restack(workerCardData.MyGameCard.AsList());
        // WorldManager.instance.StackSendCheckTarget(MyGameCard, workerCardData.MyGameCard, OutputDir, MyGameCard);
        // workerCardData.MyGameCard.SendDirection(Vector3.right);
        return;
      } else {
        // 销毁村民，创建工人卡牌
        DestroyCardByIdFormWorkshop(Cards.villager, villagerTotal);
        
		    // List<Equipable> allEquipables = combatable.GetAllEquipables();
        CardData cardData = WorldManager.instance.CreateCard(transform.position, resultCard, faceUp: false, checkAddToStack: false);
        WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir, MyGameCard);
        return;
      }
    }
  }

}