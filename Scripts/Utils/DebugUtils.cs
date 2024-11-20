using Debug = UnityEngine.Debug;

namespace ZjaveStacklandsPlus.Scripts.Utils
{
  public static class DebugUtils
  {
    public static void LogBlueprint(Blueprint blueprint)
    {
      LogSubprints(blueprint.Subprints);
    }

    public static void LogSubprints(List<Subprint> Subprints)
    {
      for (int i = 0; i < Subprints.Count; i++)
      {
        Subprint print = Subprints[i];
        LogSubprint(print);
      }
    }

    public static void LogSubprint(Subprint? print)
    {
      if (print == null)
      {
        Debug.LogFormat("Subprint is null");
        return;
      }
      Debug.LogFormat("--------------------------");
      Debug.LogFormat("ExtraResultCard {0}", string.Join(",", print.ExtraResultCards));
      Debug.LogFormat("RequiredCards {0}", string.Join(",", print.RequiredCards));
      Debug.LogFormat("ResultCard {0}", print.ResultCard);
      Debug.LogFormat("SubprintIndex {0}", print.SubprintIndex);
      Debug.LogFormat("====================================================");
    }
  }
}