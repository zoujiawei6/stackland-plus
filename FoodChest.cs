namespace ZjaveWorkshopModNS
{
  class FoodChest : ResourceChest
  {

    protected override bool CanHaveCard(CardData otherCard)
    {
      if (!string.IsNullOrEmpty(HeldCardId) && otherCard.Id != HeldCardId)
      {
        return false;
      }

      if (otherCard.MyCardType != CardType.Food || otherCard.Id == "gold" || otherCard.Id == "shell")
      {
        return false;
      }

      return true;
    }

    public override void UpdateCard()
    {
      base.UpdateCard();

      if (!string.IsNullOrEmpty(HeldCardId))
      {
          nameOverride = SokLoc.Translate("card_resource_chest_name_override", LocParam.Create("resource", WorldManager.instance.GameDataLoader.GetCardFromId(HeldCardId).Name));
          descriptionOverride = SokLoc.Translate(ChestTerm, LocParam.Create("resource", WorldManager.instance.GameDataLoader.GetCardFromId(HeldCardId).Name), LocParam.Create("amount", ResourceCount.ToString()));
      }
      else
      {
          nameOverride = SokLoc.Translate("zjave_card_food_chest_name");
          descriptionOverride = null;
      }
    }

  }
}