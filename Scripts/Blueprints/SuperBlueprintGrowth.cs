using HarmonyLib;
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
      // 这里遵从原卡片的顺序（先果实卡，再建筑卡），能避免很多的问题
      print.RequiredCards = [
        RequiredCard1,
        RequiredCard2,
      ];
      // 本来要生成5个RequiredCard1，但因为上面已经写了一个了，所以少生成一个
      for (int i = 1; i < TechnicalResearchCenter.synthesisQuantity; i++)
      {
        print.RequiredCards = print.RequiredCards.AddToArray(RequiredCard1);
      }
      if (print.ExtraResultCards.Length == 0)
      {
        return;
      }
      print.ExtraResultCards = [.. print.ExtraResultCards, .. print.ExtraResultCards];
    }

  }
}