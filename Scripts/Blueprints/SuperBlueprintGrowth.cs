using UnityEngine;
using ZjaveStacklandsPlus.Scripts.Workshops;

namespace ZjaveStacklandsPlus.Scripts.Blueprints
{
  public class SuperBlueprintGrowth : BlueprintGrowth
  {
    // 集合所有超级农场等等生长型作坊的cardId，目前而言官方可生长类型卡牌只有5个
    public readonly static string[] SuperGrowMethods = [
        SuperGardenWorkshop.cardId,
        SuperFarmWorkshop.cardId,
        SuperGreenhouseWorkshop.cardId,
    ];

    public override void Init(GameDataLoader loader)
    {
      base.Init(loader);
      
      List<Subprint> newSubprints = [.. Subprints];
      Subprints.Clear();
      foreach (Subprint subprint in newSubprints)
      {
        if (subprint.RequiredCards.Contains(Cards.soil) || subprint.RequiredCards.Contains(Cards.poop))
        {
          // 哪来的超级粪便和超级土壤，所以continue啦
          continue;
        }

        ToSuperRequiredCard(subprint, Cards.garden, SuperGardenWorkshop.cardId);
        ToSuperRequiredCard(subprint, Cards.farm, SuperFarmWorkshop.cardId);
        ToSuperRequiredCard(subprint, Cards.greenhouse, SuperGreenhouseWorkshop.cardId);
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
        RequiredCard1 = superCardId;
        string[] RequiredCards = [
          RequiredCard2,
          RequiredCard1,
          RequiredCard2,
          RequiredCard2,
          RequiredCard2,
          RequiredCard2,
          RequiredCard2,
        ];
        print.RequiredCards = RequiredCards;
      }
      else if (RequiredCard2 == cardId)
      {
        RequiredCard2 = superCardId;
        string[] RequiredCards = [
          RequiredCard1,
          RequiredCard2,
          RequiredCard1,
          RequiredCard1,
          RequiredCard1,
          RequiredCard1,
          RequiredCard1,
        ];
        print.RequiredCards = RequiredCards;
      }
    }
  }
}