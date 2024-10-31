namespace ZjaveStacklandsPlus.Scripts
{
  class FoodChest : ResourceChest
  {
    protected override bool CanHaveCard(CardData otherCard)
    {
      // if代码或许有先后顺序的问题，因此重写方法也保证这个顺序
      if (!string.IsNullOrEmpty(HeldCardId) && otherCard.Id != HeldCardId)
      {
          return false;
      }

      if (otherCard is Food food && food.FoodValue <= 0 && WorldManager.instance.CurrentBoard.Id == "cities")
      {
          return true;
      }

      if (otherCard.MyCardType != CardType.Food || otherCard.Id == "gold" || otherCard.Id == "shell")
      {
        return false;
      }
      return base.CanHaveCard(otherCard);
    }

    public override void UpdateCard()
    {
      base.UpdateCard();

      if (string.IsNullOrEmpty(HeldCardId))
      {
          nameOverride = SokLoc.Translate("zjave_card_food_chest_name");
          descriptionOverride = null;
      }
    }

  }
}