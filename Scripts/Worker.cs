using ZjaveStacklandsPlus.Scripts.Common;

namespace ZjaveStacklandsPlus.Scripts
{
  class Worker : BaseWorkLevel
  {
    public static string cardId = "zjave_worker";

    /// <summary>
    /// 相比Villager的CanHaveCard，当前函数允许将食物放置到村民上
    /// </summary>
    /// <param name="otherCard"></param>
    /// <returns></returns>
    protected override bool CanHaveCard(CardData otherCard)
    {
      if (otherCard is not BaseVillager
          && otherCard.MyCardType != CardType.Resources
          && otherCard.MyCardType != CardType.Food
          && otherCard.MyCardType != CardType.Equipable)
      {
        return otherCard.Id == "naming_stone";
      }

      return true;
    }

    public override void UpdateCardText()
    {
      base.UpdateCardText();
      
      nameOverride = SokLoc.Translate("zjave_worker_name", LocParam.Create("level", WorkLevel.ToString()));
      if (WorkLevel < IWorkLevel.MaxWorkLevel)
      {
        string levelInfo = SokLoc.Translate("zjave_work_level_description", LocParam.Create("total", ((int)WorkingTime).ToString()), LocParam.Create("next", ((int)GetNextLevelTime()).ToString()));
        descriptionOverride = $"{descriptionOverride}\n{levelInfo}";
      }
    }
  }
}