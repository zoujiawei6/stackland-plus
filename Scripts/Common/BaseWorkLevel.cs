using UnityEngine;

namespace ZjaveStacklandsPlus.Scripts.Common
{
  /// <summary>
  /// 抽象类 BaseWorkLevel 实现 IWorkLevel 接口，提供部分基础实现。
  /// </summary>
  public abstract class BaseWorkLevel : Villager, IWorkLevel
  {
    // 静态成员，表示工作的最大等级
    protected static int MaxWorkLevel => IWorkLevel.MaxWorkLevel;

    /// <summary>
    /// 工作等级
    /// </summary>
    [ExtraData("WorkLevel")]
    public int workLevel = 1;
    /// <summary>
    /// 村民自诞生开始，总共工作了多久
    /// </summary>
    [ExtraData("WorkingTime")]
    public float workingTime = 0;

    public float PlusWorkingTime(float time)
    {
      float total = WorkingTime + time;
      WorkingTime = total;
      return total;
    }

    public int LevelUp()
    {
      WorkLevel = Mathf.Min(workLevel + 1, IWorkLevel.MaxWorkLevel);
      return WorkLevel;
    }

    public int WorkLevel
    {
      get => workLevel;
      set => workLevel = Mathf.Min(value, IWorkLevel.MaxWorkLevel);
    }

    /// <summary>
    /// 设置工人的总工作时长时，等级也会变化
    /// </summary>
    public float WorkingTime
    {
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
