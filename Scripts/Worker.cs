using ZjaveStacklandsPlus.Scripts.Common;

namespace ZjaveStacklandsPlus.Scripts
{
  class Worker : BaseWorkLevel
  {
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
  }
}