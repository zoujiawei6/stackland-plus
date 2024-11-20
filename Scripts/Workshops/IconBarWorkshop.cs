using UnityEngine;
using ZjaveStacklandsPlus.Scripts.Utils;

namespace ZjaveStacklandsPlus.Scripts.Workshops
{
  /// <summary>
  /// </summary>
  public class IconBarWorkshop : CardData
  {
    public static string cardId = "zjave_icon_bar_workshop";
    public static string statusId = "zjave_icon_bar_workshop_status";
    public static string blueprintId = "zjave_blueprint_icon_bar_workshop";

    /// <summary>
    /// 能放置到当前卡片上的卡牌类型
    /// </summary>
    /// <param name="otherCard"></param>
    /// <returns></returns>
    protected override bool CanHaveCard(CardData otherCard)
    {
      return otherCard.Id == Cards.lumbercamp
        || otherCard.Id == Cards.mine
        || otherCard.Id == Cards.smelter
        || otherCard.MyCardType == CardType.Humans
        ;
    }

    public virtual List<BaseVillager> FindVillager() {
      List<CardData> cardDatas = ChildrenMatchingPredicate((CardData cd) => cd is BaseVillager);
      return cardDatas
        .Select(cd => (cd as BaseVillager)!)
        .ToList();
    }

    public override void UpdateCard()
    {
      if (!AnyChildMatchesPredicate((CardData cd) => cd.Id == Cards.mine)
        || !AnyChildMatchesPredicate((CardData cd) => cd.Id == Cards.lumber)
        || !AnyChildMatchesPredicate((CardData cd) => cd.Id == Cards.smelter))
      {
        CancelTimer();
        return;
      }
      List<BaseVillager> baseVillagers = FindVillager();
      if (baseVillagers.Count < 3)
      {
        CancelTimer();
        // 冶炼厂、锯木厂、矿场各需要一位村民
        return;
      }

      /// 铁矿石 = 1x铁矿, 1村民45s
      /// 铁矿石 = 1x铁矿, 1矿工45/2s
      /// 铁锭 = 1x铁矿石, 1x木材在冶炼炉中10 秒
      float woodWorkTime = CardUtils.GetIronOreWorkingTimeByMiner(this, baseVillagers);
      float ironOreWorkTime = CardUtils.GetWoodWorkingTimeByLumberjack(this, baseVillagers);
      float ironBarWorkTime = CardUtils.GetIronBarWorkingTime();
      float workingTime = (woodWorkTime + ironOreWorkTime + ironBarWorkTime) / 3;

      MyGameCard.StartTimer(workingTime, CompleteMaking, SokLoc.Translate(statusId), GetActionId("CompleteMaking"));
      base.UpdateCard();
    }

    public virtual void CancelTimer() {
      MyGameCard.CancelTimer(GetActionId("CompleteMaking"));
    }

    [TimedAction("complete_making")]
    public virtual void CompleteMaking()
    {
      CardData cardData = WorldManager.instance.CreateCard(transform.position, Cards.iron_bar, faceUp: false, checkAddToStack: false);
      WorldManager.instance.StackSendCheckTarget(MyGameCard, cardData.MyGameCard, OutputDir, MyGameCard);
    }
  }

}
