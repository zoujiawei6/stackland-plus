using UnityEngine;
using ZjaveStacklandsPlus.Scripts.Common;

namespace ZjaveStacklandsPlus.Scripts
{
  class Worker : Villager, IWorkLevel
  {
    /// <summary>
    /// 工作等级
    /// </summary>
    [ExtraData("WorkLevel")]
    public int workLevel = 1;
    /// <summary>
    /// 村民自诞生开始，总共工作了多久
    /// </summary>
    [ExtraData("WorkingTime")]
    public int workingTime = 0;

    /// <summary>
    /// 相比Villager的CanHaveCard，当前函数允许将食物放置到村民上
    /// </summary>
    /// <param name="otherCard"></param>
    /// <returns></returns>
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

    public int WorkLevel
    {
      get => workLevel;
      set => workLevel = Mathf.Min(value, IWorkLevel.MaxWorkLevel);
    }

    /// <summary>
    /// 设置工人的总工作时长时，等级也会变化
    /// </summary>
    public int WorkingTime {
      get => workingTime;
      set
      {
        workingTime = Mathf.Max(0, value);
        // 每工作两个月亮年，升一级
        WorkLevel = (int)Mathf.Floor(workingTime / WorldManager.instance.MonthTime / 2.0f); 
      }
    }
  }
}