using UnityEngine;

namespace ZjaveStacklandsPlus.Scripts.Utils
{
  public class MathUtils
  {
    /// <summary>
    /// 计算生产时间
    /// </summary>
    /// <param name="level">当前等级</param>
    /// <param name="timeElapsed">经过的时间（秒）</param>
    /// <returns>实际生产时间（秒）</returns>
    public static float CalculateProductionTime(int level, float timeElapsed)
    {
      // 计算加成，使用等级来提高加成效果
      float efficiencyBonus = CalculateEfficiencyBonus(level, timeElapsed);

      // 计算实际生产时间
      float productionTime = timeElapsed * (1 - efficiencyBonus);
      return productionTime;
    }

    /// <summary>
    /// 计算效率加成
    /// </summary>
    /// <param name="level">当前等级</param>
    /// <param name="timeElapsed">经过的时间（秒）</param>
    /// <returns>效率加成（0-1之间的值）</returns>
    public static float CalculateEfficiencyBonus(int level, float timeElapsed)
    {
      // 最大加成设为0.5，允许更高的等级提供更高的加成
      float maxBonus = 0.5f;

      // 将等级转化为加成，假设每级提升0.1
      float levelBonus = level * 0.1f;

      // 使用指数衰减来模拟时间的影响
      float decayFactor = 0.1f; // 衰减因子，控制加成下降速度
      float efficiencyBonus = Mathf.Min(maxBonus, levelBonus) * Mathf.Exp(-decayFactor * timeElapsed);

      // 确保加成不小于0
      return Mathf.Max(efficiencyBonus, 0);
    }
  }
}
