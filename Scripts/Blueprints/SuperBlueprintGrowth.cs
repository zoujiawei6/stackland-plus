using ZjaveStacklandsPlus.Scripts.Workshops;

namespace ZjaveStacklandsPlus.Scripts.Blueprints
{
  public class SuperBlueprintGrowth : BlueprintGrowth
  {
    public override void Init(GameDataLoader loader)
    {
      base.Init(loader);

      List<Subprint> newSubprints = [.. Subprints];
      Subprints.Clear();
      for (int i = 0; i < newSubprints.Count; i++)
      {
        Subprint subprint = newSubprints[i];
        ToSuperRequiredCard(subprint, Cards.garden, SuperGarden.cardId);
        ToSuperRequiredCard(subprint, Cards.farm, SuperFarm.cardId);
        ToSuperRequiredCard(subprint, Cards.greenhouse, SuperGreenhouse.cardId);
        subprint.SubprintIndex = i;
        Subprints.Add(subprint);
      }
    }

    /// <summary>
    /// 将print的字段RequiredCards数组里的农场、花园、温室类的cardId替换成超级农场、超级花园、超级温室的cardId。
    /// 并将另一个“种子”卡牌的消耗数量*6，并设置生成数量*6。
    /// 
    /// 本函数并不假定建筑卡片固定在数组位置：RequiredCards[1]
    /// </summary>
    /// <param name="print"></param>
    /// <param name="cardId"></param>
    /// <param name="superCardId"></param>
    private void ToSuperRequiredCard(Subprint print, string cardId, string superCardId)
    {
      string RequiredCard1 = print.RequiredCards[0];
      string RequiredCard2 = print.RequiredCards[1];
      if (RequiredCard1 == cardId)
      {
        RequiredCard1 = RequiredCard2;
        RequiredCard2 = superCardId;
      }
      else if (RequiredCard2 == cardId)
      {
        RequiredCard2 = superCardId;
      }
      // Debug.LogFormat("RequiredCards = {0}", string.Join(", ", print.RequiredCards));
      print.RequiredCards = [
        RequiredCard1,
        RequiredCard2,
        RequiredCard1,
        RequiredCard1,
        RequiredCard1,
        RequiredCard1,
        RequiredCard1,
      ];
      if (print.ExtraResultCards.Length == 0)
      {
        return;
      }
      print.ExtraResultCards = [.. print.ExtraResultCards, .. print.ExtraResultCards];
    }

    public override void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
    {
      base.BlueprintComplete(rootCard, involvedCards, print);
      if (rootCard.CardData is HeavyFoundation && rootCard.Child != null)
      {
        rootCard = rootCard.Child;
      }
      if (rootCard.CardData.Id == SuperGreenhouse.cardId)
      {
        GreenhouseBlueprintComplete(rootCard, involvedCards, print);
      }
    }

    public virtual void GreenhouseBlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
    {
      // 1. 查看函数：BlueprintGrowth.BlueprintComplete
      // 2. 查看函数：BlueprintGrowth.PopulateSubprints
      // 3. RequiredCards = new string[2] { growable.ToGrow, text }
      // 那么可以证明 RequiredCards[0] == growable.ToGrow
      int growTotal = 0;
      GameCard gameCard = rootCard;
      for (int i = 0; i < TechnicalResearchCenter.synthesisQuantity; i++)
      {
        CardData cardData = allResultCards.FirstOrDefault((CardData c) => Subprints.Any((Subprint x) => x.RequiredCards[0] == c.Id));
        if (cardData != null)
        {
          cardData.MyGameCard.BounceTarget = gameCard;
          cardData.MyGameCard.Velocity = null;
          cardData.MyGameCard.SetParent(gameCard);
          allResultCards.Remove(cardData);
          gameCard = cardData.MyGameCard;
        }
        growTotal++;
      }
      if (growTotal > 0)
      {
        WorldManager.instance.Restack(allResultCards.Select((CardData x) => x.MyGameCard).ToList());
        WorldManager.instance.StackSendCheckTarget(rootCard, allResultCards[0].MyGameCard, rootCard.CardData.OutputDir, rootCard);
      }
    }
  }
}