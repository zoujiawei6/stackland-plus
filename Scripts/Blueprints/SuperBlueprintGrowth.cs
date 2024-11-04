using UnityEngine;

namespace ZjaveStacklandsPlus.Scripts.Blueprints
{
  public class SuperBlueprintGrowth : BlueprintGrowth
  {
    /// <summary>
    /// 增加5倍的产量
    /// </summary>
    /// <param name="toGrow"></param>
    /// <param name="statusTerm"></param>
    /// <param name="resultItem"></param>
    /// <param name="resultCount"></param>
    /// <param name="growSpeed"></param>
    public new class Growable(string toGrow, string statusTerm, string resultItem, int resultCount, float growSpeed)
     : BlueprintGrowth.Growable(toGrow, statusTerm, resultItem, resultCount * 5, growSpeed)
    {
    }

    // 相比官方的，粪便和土壤无法合成，所以删掉第一二个数据
    private float[] growSpeedMultiplier = [0.75f, 0.5f, 0.5f];

    public override void Init(GameDataLoader loader)
    {
      string[] superGrowMethods = StickWorkshopMod.superGrowMethods.Where(x => x != null).ToArray();
      if (superGrowMethods.Length == 0)
      {
        Debug.LogFormat("SuperBlueprintGrowth::Init: superGrowMethods is empty");
        return;
      }

      // 在调用基类 Init 方法之前修改 growMethods
      Type type = typeof(BlueprintGrowth);
      
      type.GetField("growMethods", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
          ?.SetValue(this, superGrowMethods);
      
      type.GetField("growSpeedMultiplier", System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
          ?.SetValue(this, growSpeedMultiplier);

      // 调用基类的 Init 方法以进行初始化
      base.Init(loader);
    }

  }
}