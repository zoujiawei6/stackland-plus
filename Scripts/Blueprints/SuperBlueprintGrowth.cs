using System.Diagnostics;
using UnityEngine;
using ZjaveStacklandsPlus.Scripts.Utils;
using ZjaveStacklandsPlus.Scripts.Workshops;
using Debug = UnityEngine.Debug;

namespace ZjaveStacklandsPlus.Scripts.Blueprints
{
  public class SuperBlueprintGrowth : BlueprintGrowth
  {
    public override void Init(GameDataLoader loader)
    {
      base.Init(loader);

      DebugUtils.LogSubprints(Subprints);
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
      string ExtraResultCard = print.ExtraResultCards[0];
      print.ExtraResultCards = [
        ExtraResultCard,
        ExtraResultCard,
        ExtraResultCard,
        ExtraResultCard,
        ExtraResultCard,
        ExtraResultCard,
      ];
    }

    public override Subprint GetMatchingSubprint(GameCard card, out SubprintMatchInfo matchInfo)
    {
        matchInfo = default(SubprintMatchInfo);
        foreach (Subprint subprint in Subprints)
        {
            if (subprint.StackMatchesSubprint(card, out matchInfo))
            {
                return subprint;
            }
        }

        return null;
    }

    public override void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
    {
      // 打印调用者堆栈
      Debug.LogFormat("BlueprintComplete {0}", string.Join(", ", new StackTrace(true).GetFrames().Select(x => x.GetMethod().Name)));
      List<GameCard> list = new List<GameCard>(involvedCards);
      List<string> allCardsToRemove = print.GetAllCardsToRemove();
      CardData cardData = null;
      List<CardData> list2 = new List<CardData>();
      for (int i = 0; i < allCardsToRemove.Count; i++)
      {
        string[] possibleRemovables = allCardsToRemove[i].Split('|');
        GameCard gameCard = list.FirstOrDefault((GameCard x) => possibleRemovables.Contains(x.CardData.Id));
        if (gameCard != null)
        {
          Debug.LogFormat("gameCard {0} is {1}", gameCard.CardData.Id, gameCard.CardData.Name);
          gameCard.DestroyCard(spawnSmoke: true);
          list.Remove(gameCard);
        }
      }
      allResultCards.Clear();
      Vector3 outputDirection = ((rootCard != null) ? rootCard.CardData.OutputDir : Vector3.zero);
      if (!string.IsNullOrEmpty(print.ResultCard))
      {
        cardData = WorldManager.instance.CreateCard(rootCard.transform.position, print.ResultCard, faceUp: false, checkAddToStack: false);
        Debug.LogFormat("cardData1 {0} is {1}", cardData.Id, cardData.Name);
        allResultCards.Add(cardData);
      }
      if (!string.IsNullOrEmpty(print.ResultAction))
      {
        GameCard gameCard2 = involvedCards.FirstOrDefault((GameCard x) => x.CardData is Combatable);
        if (gameCard2 != null)
        {
          gameCard2.CardData.ParseAction(print.ResultAction);
        }
        else
        {
          rootCard.CardData.ParseAction(print.ResultAction);
        }
      }
      if (print.ExtraResultCards != null)
      {
        Debug.LogFormat("print.RequiredCards = {0}", string.Join(", ", print.RequiredCards));
        Debug.LogFormat("print.ResultCard = {0}", print.ResultCard);
        Debug.LogFormat("print.ExtraResultCards = {0}", string.Join(", ", print.ExtraResultCards));
        for (int j = 0; j < print.ExtraResultCards.Length; j++)
        {
          CardData item = WorldManager.instance.CreateCard(rootCard.transform.position, print.ExtraResultCards[j], faceUp: false, checkAddToStack: false);
          Debug.LogFormat("item {0} is {1}", item.Id, item.Name);
          list2.Add(item);
          allResultCards.Add(item);
        }
      }
      GameCard gameCard3 = involvedCards.FirstOrDefault((GameCard x) => x.CardData.HasOutputConnector());
      if (CombineResultCards)
      {
        WorldManager.instance.Restack(allResultCards.Select((CardData x) => x.MyGameCard).ToList());
        Debug.LogFormat("allResultCards[0].MyGameCard {0} is {1}", allResultCards[0].MyGameCard.CardData.Id, allResultCards[0].MyGameCard.CardData.Name);
        if (gameCard3 != null)
        {
          WorldManager.instance.StackSendCheckTarget(gameCard3, allResultCards[0].MyGameCard, outputDirection, gameCard3);
        }
        else
        {
          WorldManager.instance.StackSend(allResultCards[0].MyGameCard, outputDirection);
        }
      }
      else
      {
        if (cardData != null)
        {
          Debug.LogFormat("cardData.MyGameCard {0} is {1}", cardData.MyGameCard.CardData.Id, cardData.MyGameCard.CardData.Name);
          if (gameCard3 != null)
          {
            WorldManager.instance.StackSendCheckTarget(gameCard3, cardData.MyGameCard, outputDirection, gameCard3);
          }
          else
          {
            WorldManager.instance.StackSend(cardData.MyGameCard, outputDirection);
          }
        }
        if (list2.Count > 0)
        {
          Debug.LogFormat("list2[0].MyGameCard {0} is {1}", list2[0].MyGameCard.CardData.Id, list2[0].MyGameCard.CardData.Name);
          WorldManager.instance.Restack(list2.Select((CardData x) => x.MyGameCard).ToList());
          if (gameCard3 != null)
          {
            WorldManager.instance.StackSendCheckTarget(gameCard3, list2[0].MyGameCard, outputDirection, gameCard3);
          }
          else
          {
            WorldManager.instance.StackSend(list2[0].MyGameCard, outputDirection);
          }
        }
      }
      if (print.ResultPolution > 0)
      {
        (WorldManager.instance.CreateCard(rootCard.transform.position, "pollution", faceUp: true, checkAddToStack: false) as Pollution).PollutionAmount = print.ResultPolution;
      }
      if (print.ResultWellbeing != 0)
      {
        CitiesManager.instance.AddWellbeing(print.ResultWellbeing);
        WorldManager.instance.CreateFloatingText(allResultCards[0].MyGameCard, print.ResultWellbeing > 0, print.ResultWellbeing, SokLoc.Translate("label_blueprint_wellbeing"), Icons.Wellbeing, desiredBehaviour: true, 0, 0f, closeOnHover: true);
      }
      WorldManager.instance.Restack(list);
    }
  }
}