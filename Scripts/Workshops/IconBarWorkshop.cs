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

    /// 铁矿石 = 1x铁矿, 1村民45s
    /// 铁矿石 = 1x铁矿, 1矿工45/2s
    /// 铁锭 = 1x铁矿石, 1x木材在冶炼炉中10 秒

    public bool HaveMiner() {
      return AnyChildMatchesPredicate((CardData cd) => cd.Id == Cards.miner);
    }

    public bool HaveHumans() {
      return AnyChildMatchesPredicate((CardData cd) => cd.MyCardType == CardType.Humans);
    }

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

    public virtual List<BaseVillager?> FindVillager() {
      List<CardData> cardDatas = ChildrenMatchingPredicate((CardData cd) => cd is BaseVillager);
      // 过滤空数据，并转换为BaseVillager列表
      return cardDatas
        .Select(cd => cd as BaseVillager)
        .Where(cd => cd != null)
        .ToList();
    }

    public override void UpdateCard()
    {
      List<BaseVillager?> baseVillagers = FindVillager();
      if (baseVillagers.Count < 3)
      {
        // 冶炼厂、锯木厂、矿场各需要一位村民
        return;
      }

      BaseVillager? miner = baseVillagers.Find(bv => bv?.Id == Cards.miner);

    }

    // public virtual float getMineWorkTime(BaseVillager? miner) {
    //   if (miner == null) {
    //     return 0;
    //   }
      
    // }



  }

}
