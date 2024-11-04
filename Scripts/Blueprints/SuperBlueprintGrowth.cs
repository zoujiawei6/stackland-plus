using UnityEngine;

namespace ZjaveStacklandsPlus.Scripts.Blueprints
{
  public class SuperBlueprintGrowth : BlueprintGrowth
  {
    public new class Growable(string toGrow, string statusTerm, string resultItem, int resultCount, float growSpeed)
    {
      public string ToGrow = toGrow;

      public string StatusTerm = statusTerm;

      public string ResultItem = resultItem;

      public int ResultCount = resultCount * 5;

      public float GrowSpeed = growSpeed;

      public string ResultAction = "";
    }

    private readonly List<Growable> growables = [];

    private readonly string[] growMethods = StickWorkshopMod.superGrowMethods;

    private readonly float[] growSpeedMultiplier = [0.75f, 0.5f, 0.5f];

    public override void Init(GameDataLoader loader)
    {
      growables.Clear();
      growables.Add(new Growable("berry", "idea_growth_status_0", "berrybush", 1, 120f));
      growables.Add(new Growable("apple", "idea_growth_status_1", "apple_tree", 1, 120f));
      growables.Add(new Growable("carrot", "idea_growth_status_2", "carrot", 2, 180f));
      growables.Add(new Growable("onion", "idea_growth_status_3", "onion", 2, 120f));
      growables.Add(new Growable("potato", "idea_growth_status_4", "potato", 2, 120f));
      growables.Add(new Growable("mushroom", "idea_growth_status_5", "mushroom", 2, 120f));
      growables.Add(new Growable("banana", "idea_growth_status_6", "banana_tree", 1, 120f));
      growables.Add(new Growable("cotton", "idea_growth_status_8", "cotton_plant", 1, 120f));
      growables.Add(new Growable("lime", "idea_growth_name_status_9", "lime", 2, 180f));
      growables.Add(new Growable("chili_pepper", "idea_growth_name_status_10", "chili_pepper", 2, 120f));
      growables.Add(new Growable("seaweed", "idea_growth_status_11", "seaweed", 2, 120f));
      growables.Add(new Growable("sugar", "idea_growth_status_12", "sugar_cane", 1, 120f));
      growables.Add(new Growable("wheat", "idea_growth_status_13", "wheat", 3, 120f)
      {
        ResultAction = "special:grow_wheat"
      });
      growables.Add(new Growable("grape", "idea_growth_status_14", "grape_vine", 1, 120f));
      growables.Add(new Growable("olive", "idea_growth_status_15", "olive_tree", 1, 120f));
      growables.Add(new Growable("tomato", "idea_growth_status_16", "tomato_plant", 1, 120f));
      growables.Add(new Growable("stick", "idea_growth_status_17", "tree", 1, 120f));
      growables.Add(new Growable("herbs", "idea_growth_status_18", "herbs", 2, 120f));
      PopulateSubprints(loader);
      base.Init(loader);
    }

    private void PopulateSubprints(GameDataLoader loader)
    {
      Subprints.Clear();
      for (int i = 0; i < growables.Count; i++)
      {
        Growable growable = growables[i];
        if (loader.GetCardFromId(growable.ToGrow, throwError: false) == null || loader.GetCardFromId(growable.ResultItem, throwError: false) == null)
        {
          continue;
        }

        for (int j = 0; j < growMethods.Length; j++)
        {
          string text = growMethods[j];
          List<string> list = new List<string>();
          for (int k = 0; k < growable.ResultCount; k++)
          {
            list.Add(growable.ResultItem);
          }

          Subprints.Add(new Subprint
          {
            RequiredCards = new string[2] { growable.ToGrow, text },
            ExtraResultCards = list.ToArray(),
            StatusTerm = growable.StatusTerm,
            Time = growable.GrowSpeed * growSpeedMultiplier[j],
            ResultAction = growable.ResultAction
          });
        }
      }
    }

    public override void BlueprintComplete(GameCard rootCard, List<GameCard> involvedCards, Subprint print)
    {
      base.BlueprintComplete(rootCard, involvedCards, print);
      if (rootCard.CardData is HeavyFoundation && rootCard.Child != null)
      {
        rootCard = rootCard.Child;
      }

      if (!(rootCard.CardData.Id == "greenhouse"))
      {
        return;
      }

      for (int i = 0; i < 5; i++)
      {
        CardData cardData = allResultCards.FirstOrDefault((CardData c) => growables.Any((Growable x) => x.ToGrow == c.Id));
        if (cardData != null)
        {
          cardData.MyGameCard.BounceTarget = null;
          cardData.MyGameCard.Velocity = null;
          cardData.MyGameCard.SetParent(rootCard);
          allResultCards.Remove(cardData);
          WorldManager.instance.Restack(allResultCards.Select((CardData x) => x.MyGameCard).ToList());
          WorldManager.instance.StackSendCheckTarget(rootCard, allResultCards[0].MyGameCard, rootCard.CardData.OutputDir, rootCard);
        }
      }
    }
  }
}