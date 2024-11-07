using System.Diagnostics;
using UnityEngine;
using Debug = UnityEngine.Debug;

namespace ZjaveStacklandsPlus.Scripts.Workshops
{
  public class ZjaveGreenhouse : Greenhouse
  {
    public override void UpdateCard()
    {
      if (MyGameCard.IsDemoCard || !MyGameCard.MyBoard.IsCurrent)
      {
        return;
      }
      MyGameCard.HighlightActive = false;
      if (WorldManager.instance.DraggingCard != null && WorldManager.instance.DraggingCard != MyGameCard)
      {
        if (CanHaveCardOnTop(WorldManager.instance.DraggingCard.CardData) && !MyGameCard.HasChild && !MyGameCard.IsChildOf(WorldManager.instance.DraggingCard))
        {
          MyGameCard.HighlightActive = true;
        }
        if (!(MyGameCard.removedChild == WorldManager.instance.DraggingCard))
        {
          GameCard cardWithStatusInStack = MyGameCard.GetCardWithStatusInStack();
          if (cardWithStatusInStack != null && !cardWithStatusInStack.CardData.CanHaveCardsWhileHasStatus())
          {
            MyGameCard.HighlightActive = false;
          }
        }
      }
      if (MyGameCard.StackUpdate)
      {
        if (HasStatusEffectOfType<StatusEffect_MaxOnBoard>())
        {
          RemoveStatusEffect<StatusEffect_MaxOnBoard>();
        }
        if (!WorkerAmountMet() && MyGameCard.TimerRunning && !MyGameCard.SkipCitiesChecks)
        {
          MyGameCard.CancelAnyTimer();
        }
        CheckBlueprintInStack();
      }
      if (!MyGameCard.BeingDragged && MyGameCard.LastParent != null && !MyGameCard.HasParent)
      {
        if (MyGameCard.LastParent.GetRootCard().CardData.DetermineCanHaveCardsWhenIsRoot)
        {
          CheckStackValidityAndRestack();
        }
        MyGameCard.LastParent = null;
      }
      if (WorkerAmount > 0)
      {
        bool flag = WorkerAmountMet();
        if (EducatedWorkers)
        {
          if (MyGameCard.WorkerChildren.Any((GameCard c) => c.CardData is Worker worker && worker.GetWorkerType() != WorkerType.Educated && worker.GetWorkerType() != WorkerType.Robot) || !flag)
          {
            if (!HasStatusEffectOfType<StatusEffect_NoEducatedWorkers>())
            {
              AddStatusEffect(new StatusEffect_NoEducatedWorkers());
            }
          }
          else
          {
            RemoveStatusEffect<StatusEffect_NoEducatedWorkers>();
          }
        }
        else if (!flag)
        {
          if (!HasStatusEffectOfType<StatusEffect_NoWorkers>())
          {
            AddStatusEffect(new StatusEffect_NoWorkers());
          }
        }
        else
        {
          RemoveStatusEffect<StatusEffect_NoWorkers>();
        }
      }
      else
      {
        if (HasStatusEffectOfType<StatusEffect_NoWorkers>())
        {
          RemoveStatusEffect<StatusEffect_NoWorkers>();
        }
        if (HasStatusEffectOfType<StatusEffect_NoEducatedWorkers>())
        {
          RemoveStatusEffect<StatusEffect_NoEducatedWorkers>();
        }
      }
      if (!IsOn && ((WorkerAmount > 0 && WorkerAmountMet()) || WorkerAmount == 0))
      {
        AddStatusEffect(new StatusEffect_CardOff());
      }
      else
      {
        RemoveStatusEffect<StatusEffect_CardOff>();
      }
      for (int num = StatusEffects.Count - 1; num >= 0; num--)
      {
        StatusEffects[num].Update();
      }
      UpdateCardText();
    }

    private void CheckBlueprintInStack()
    {
      Debug.LogFormat("EEEEEE CheckBlueprintInStack");
      if (!MyGameCard.HasParent)
      {
        Subprint subprint = FindMatchingPrint();
        Debug.LogFormat("EEEEEE subprint.ExtraResultCard {0}", string.Join(",", subprint.ExtraResultCards));
        Debug.LogFormat("EEEEEE subprint.RequiredCards {0}", string.Join(",", subprint.RequiredCards));
        Debug.LogFormat("EEEEEE subprint.ResultCard {0}", subprint.ResultCard);
        Debug.LogFormat("EEEEEE subprint.SubprintIndex {0}", subprint.SubprintIndex);
        if (subprint != null)
        {
          string id = subprint.ParentBlueprint.Id;
          int subprintIndex = subprint.SubprintIndex;
          BaseVillager baseVillager = (from BaseVillager x in CardsInStackMatchingPredicate((CardData x) => x is BaseVillager)
                                       orderby x.GetActionTimeModifier("finish_blueprint", this)
                                       select x).Reverse().FirstOrDefault();
          Worker worker = (from Worker x in CardsInStackMatchingPredicate((CardData x) => x is Worker)
                           orderby x.GetActionTimeModifier()
                           select x).Reverse().FirstOrDefault();
          if (!subprint.ParentBlueprint.IsInvention || (subprint.ParentBlueprint.IsInvention && WorldManager.instance.HasFoundCard(id)))
          {
            CardData consumer = CardsInStackMatchingPredicate((CardData x) => x is IEnergyConsumer).FirstOrDefault();
            if (baseVillager != null)
            {
              MyGameCard.StartBlueprintTimer(baseVillager.GetActionTimeModifier("finish_blueprint", this) * subprint.Time, FinishBlueprint, subprint.StatusName, GetActionId("FinishBlueprint"), id, subprintIndex, consumer, subprint.ParentBlueprint.IgnoreEnergyWorkerDemand);
            }
            else if (worker != null)
            {
              MyGameCard.StartBlueprintTimer(worker.GetActionTimeModifier() * subprint.Time, FinishBlueprint, subprint.StatusName, GetActionId("FinishBlueprint"), id, subprintIndex, consumer, subprint.ParentBlueprint.IgnoreEnergyWorkerDemand);
            }
            else
            {
              MyGameCard.StartBlueprintTimer(subprint.Time, FinishBlueprint, subprint.StatusName, GetActionId("FinishBlueprint"), id, subprintIndex, consumer, subprint.ParentBlueprint.IgnoreEnergyWorkerDemand);
            }
          }
        }
        else
        {
          MyGameCard.CancelTimer(GetActionId("FinishBlueprint"));
        }
      }
      else
      {
        MyGameCard.CancelTimer(GetActionId("FinishBlueprint"));
      }
      if (MyGameCard.TimerRunning && MyGameCard.TimerActionId == "finish_blueprint" && FindMatchingPrint() == null)
      {
        MyGameCard.CancelTimer(GetActionId("FinishBlueprint"));
      }
      MyGameCard.StackUpdate = false;
    }

    [TimedAction("finish_blueprint")]
    public new void FinishBlueprint()
    {
        Blueprint blueprintWithId = WorldManager.instance.GetBlueprintWithId(MyGameCard.TimerBlueprintId);
        if (blueprintWithId != null)
        {
          for (int i = 0; i < blueprintWithId.Subprints.Count; i++)
          {
            Subprint print = blueprintWithId.Subprints[i];
            Debug.LogFormat("GGGGGG print.ExtraResultCard {0}", string.Join(",", print.ExtraResultCards));
            Debug.LogFormat("GGGGGG print.RequiredCards {0}", string.Join(",", print.RequiredCards));
            Debug.LogFormat("GGGGGG print.ResultCard {0}", print.ResultCard);
            Debug.LogFormat("GGGGGG print.SubprintIndex {0}", print.SubprintIndex);
            Debug.LogFormat("====================================================");
          }
            Subprint subprint = blueprintWithId.Subprints[MyGameCard.TimerSubprintIndex];
            Debug.LogFormat("FFFFFF subprint.ExtraResultCard {0}", string.Join(",", subprint.ExtraResultCards));
            Debug.LogFormat("FFFFFF subprint.RequiredCards {0}", string.Join(",", subprint.RequiredCards));
            Debug.LogFormat("FFFFFF subprint.ResultCard {0}", subprint.ResultCard);
            Debug.LogFormat("FFFFFF subprint.SubprintIndex {0}", subprint.SubprintIndex);
            blueprintWithId.BlueprintComplete(MyGameCard, MyGameCard.GetAllCardsInStack(), blueprintWithId.Subprints[MyGameCard.TimerSubprintIndex]);
        }
    }

    private Subprint FindMatchingPrint()
    {
      // 打印当前方法的调用者的堆栈
      Debug.LogFormat("EEEEEE FindMatchingPrint {0}", new StackTrace().GetFrame(1).GetMethod().Name);
      Subprint? result = null;
      int num = int.MaxValue;
      int num2 = int.MinValue;
      foreach (Blueprint blueprintPrefab in WorldManager.instance.BlueprintPrefabs)
      {
        if (!blueprintPrefab.CanCurrentlyBeMade || (WorldManager.instance.CurrentBoard.Location == Location.Cities && blueprintPrefab.CardUpdateType != CardUpdateType.Cities))
        {
          continue;
        }
        SubprintMatchInfo matchInfo;
        GameCard gameCard = MyGameCard.GetRootCard();
        Subprint matchingSubprint = blueprintPrefab.GetMatchingSubprint(gameCard, out matchInfo);
        if (matchingSubprint == null)
        {
          continue;
        }
        if (blueprintPrefab.HasMaxAmountOnBoard && WorldManager.instance.GetCardCount(matchingSubprint.ResultCard) >= blueprintPrefab.MaxAmountOnBoard)
        {
          if (!HasStatusEffectOfType<StatusEffect_MaxOnBoard>())
          {
            AddStatusEffect(new StatusEffect_MaxOnBoard());
          }
        }
        else if (matchInfo.MatchCount > num2 || (matchInfo.MatchCount == num2 && matchInfo.FullyMatchedAt < num))
        {
          Debug.LogFormat("AAAAAA Blueprint {0} has {1} matches at {2}", blueprintPrefab.Id, matchInfo.MatchCount, matchInfo.FullyMatchedAt);
          Debug.LogFormat("AAAAAA matchingSubprint {0}", matchingSubprint.ExtraResultCards);
          Debug.LogFormat("AAAAAA num {0} num2 {1}", num, num2);
          num = matchInfo.FullyMatchedAt;
          num2 = matchInfo.MatchCount;
          result = matchingSubprint;
        }
      }
      return result;
    }

    private void CheckStackValidityAndRestack()
    {
      List<GameCard> allCardsInStack = MyGameCard.GetAllCardsInStack();
      List<GameCard> list = new List<GameCard>();
      for (int i = 0; i < allCardsInStack.Count; i++)
      {
        list.Add(allCardsInStack[i]);
        allCardsInStack[i].RemoveFromStack();
        if (i < allCardsInStack.Count - 1 && !allCardsInStack[i].CardData.CanHaveCardOnTop(allCardsInStack[i + 1].CardData))
        {
          WorldManager.instance.Restack(list);
          list.Clear();
        }
      }
      WorldManager.instance.Restack(list);
    }

  }

}
