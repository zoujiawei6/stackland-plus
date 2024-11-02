namespace ZjaveStacklandsPlus.Scripts.Common
{
  /// <summary>
  /// 工作等级顶层接口
  /// </summary>
  public interface IWorkLevel
  {
    public static int MaxWorkLevel = 6;

    /// <summary>
    /// 工作等级
    /// </summary>
    int WorkLevel { get; set; }

    /// <summary>
    /// 自诞生开始，总共工作了多久
    /// </summary>
    float WorkingTime { get; set; }

    /// <summary>
    /// 增加总工作时长
    /// </summary>
    /// <param name="time"></param>
    /// <returns></returns>
    float CountWorkingTime(float time);

    /// <summary>
    /// 提升工人等级
    /// </summary>
    /// <returns></returns>
    int LevelUp();

    /// <summary>
    /// 获取升到下一级需要的工作时长
    /// </summary>
    /// <returns></returns>
    float GetNextLevelTime();

    /// <summary>
    /// 满级所需的总工作时长
    /// </summary>
    /// <returns></returns>
    float GetMaxLevelTime();
  }
}