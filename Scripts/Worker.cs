namespace ZjaveStacklandsPlus.Scripts
{
  class Worker : Villager
  {
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