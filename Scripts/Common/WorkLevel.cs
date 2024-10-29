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
    float AddWorkingTime(float time);
  }
}