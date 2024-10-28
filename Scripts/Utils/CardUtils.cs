namespace ZjaveStacklandsPlus.Scripts.Utils
{
  class CardUtils
  {
    public static List<CardData> GetCardById(CardData? cardData, string? cardId) {
      if (cardData == null || cardId == null || cardId.Length == 0) {
        return [];
      }
      List<CardData> list = cardData.ChildrenMatchingPredicate((CardData c) => c.Id == cardId);
      return list;
    }

    public static CardData? GetFirstCardById(CardData? cardData, string? cardId) {
      List<CardData> list = GetCardById(cardData, cardId);
      if (list.Count == 0) {
        return null;
      }
      return list[0];
    }
  }
}