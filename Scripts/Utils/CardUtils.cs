namespace ZjaveStacklandsPlus.Scripts.Utils
{
  class CardUtils
  {
    public static List<CardData> GetChildrenById(CardData? cardData, string? cardId) {
      if (cardData == null || cardId == null || cardId.Length == 0) {
        return [];
      }
      List<CardData> list = cardData.ChildrenMatchingPredicate((CardData c) => c.Id == cardId);
      return list;
    }

    public static CardData? GetFirstChildrenById(CardData? cardData, string? cardId) {
      List<CardData> list = GetChildrenById(cardData, cardId);
      if (list.Count == 0) {
        return null;
      }
      return list[0];
    }

    public static bool IsFoodById(string cardId) {
      switch (cardId) {
        case "apple":
        case "banana":
        case "beer":
        case "berry":
        case "bottle_of_rum":
        case "bottle_of_water":
        case "bread":
        case "cane_sugar":
        case "carrot":
        case "ceviche":
        case "chili_pepper":
        case "cooked_crab":
        case "cooked_meat":
        case "dough":
        case "egg":
        case "fish_and_chips":
        case "flour":
        case "french_fries":
        case "fried_fish":
        case "fried_meat":
        case "frittata":
        case "fruit_salad":
        case "grape":
        case "grilled_fish":
        case "herbal_tea":
        case "herbs":
        case "lime":
        case "milk":
        case "milkshake":
        case "mushroom":
        case "olive":
        case "olive_oil":
        case "omelette":
        case "onion":
        case "pancakes":
        case "pizza":
        case "potato":
        case "raw_crab_meat":
        case "raw_fish":
        case "raw_meat":
        case "royal_banquet":
        case "seafood_stew":
        case "seaweed":
        case "stew":
        case "sushi":
        case "tamago_sushi":
        case "tomato":
        case "water":
        case "wheat":
        case "wine":
          return true;
        default:
          return false;
      }
    }
  }
}