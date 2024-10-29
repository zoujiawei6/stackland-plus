using UnityEngine;

namespace ZjaveStacklandsPlus.Scripts.Utils
{
  public class MathUtils
  {
    /// <summary>
    /// 生产时间
    /// </summary>
    /// <param name="level">当前等级</param>
    /// <param name="timeElapsed">经过的时间（秒）</param>
    /// <param name="maxBonus">最大效率加成（可选，默认值为0.5）</param>
    /// <returns>实际生产时间（秒），最大减少至50%</returns>
    public static float CalculateProductionTime(int level, float timeElapsed, float maxBonus = 0.5f)
    {
      // 假设 a 的范围在 1 到 6，我们可以将它进行一个归一化到 0 - 1 的区间
      float normalizedA = Mathf.InverseLerp(1f, 6f, level);

      // 使用指数函数，让前期变化较大，后期变化较小
      float result = timeElapsed * Mathf.Pow(0.75f, normalizedA * 3);

      return Mathf.Max(result, timeElapsed * maxBonus);
    }
  }
}
