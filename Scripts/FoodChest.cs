using UnityEngine;

namespace ZjaveStacklandsPlus.Scripts
{
  class FoodChest : ResourceChest
  {
    public static string cardId = "zjave_food_chest";
    public static string blueprintId = "zjave_blueprint_food_chest";
    protected override bool CanHaveCard(CardData otherCard)
    {
      // if代码或许有先后顺序的问题，因此重写方法也保证这个顺序
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

    public virtual Sprite GetSpecialIcon()
    {
      SpecialIcon ??= SpriteManager.instance.FootFightIcon;
      return SpecialIcon;
    }

    public override void UpdateCard()
    {
      base.UpdateCard();
      MyGameCard.SpecialIcon.sprite = GetSpecialIcon();

      if (string.IsNullOrEmpty(HeldCardId))
      {
          nameOverride = SokLoc.Translate("zjave_food_chest_name");
          descriptionOverride = null;
      }
    }
  }
}