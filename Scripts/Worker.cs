using UnityEngine;
using ZjaveStacklandsPlus.Scripts.Common;

namespace ZjaveStacklandsPlus.Scripts
{
  class Worker : Villager, IWorkLevel
  {
    private int workLevel = 1;
    protected override bool CanHaveCard(CardData otherCard)
    {
      if (otherCard is not BaseVillager
          && otherCard.MyCardType != CardType.Resources
          && otherCard.MyCardType != CardType.Food
          && otherCard.MyCardType != CardType.Equipable)
      {
        return otherCard.Id == "naming_stone";
      }

      return true;
    }

    public int GetWorkLevel()
    {
      return workLevel;
    }

    public int SetWorkLevel(int newLevel)
    {
      workLevel = newLevel;
      return workLevel;
    }
  }
}