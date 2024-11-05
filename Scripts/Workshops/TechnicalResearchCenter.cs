namespace ZjaveStacklandsPlus.Scripts.Workshops
{
  /// <summary>
  /// 技术研究中心
  /// </summary>
  /// <param name="ingredient">工坊关键字，用于创建卡片id</param>
  /// <param name="workingTime">制作所需时间</param>
  /// <param name="resultCard">工坊制作出的卡片</param>
  /// <param name="haveCards">制作所需的材料卡片，及其所需数量列表</param>
  public class TechnicalResearchCenter : ZjaveWorkshop
  {

    public static string cardId = "zjave_technical_research_center";
    public static string blueprintId = "zjave_blueprint_technical_research_center";
    public TechnicalResearchCenter() : base("technical_research_center", Cards.garden, 3, new Dictionary<string, int> {
      { Cards.stick, 6 },
    }
    // , new Dictionary<string, int> {
    //   { Cards.garden, 6 },
    //   { Cards.farm, 6 },
    //   { Cards.greenhouse, 6 },
    // }
    )
    {
    }

    // /// <summary>
    // /// 判断卡片是否符合制作条件
    // /// </summary>
    // /// <returns></returns>
    // public override bool AccordWithMaking()
    // {
    //   // haveCards是卡片需要哪些材料才能进行制作，此处进行判断
    //   bool allMatch = haveCards != null && haveCards.All(kvp =>
    //       ChildrenMatchingPredicateCount((CardData cd) => cd.Id == kvp.Key) >= kvp.Value
    //   );
    //   bool hasHuman = AnyChildMatchesPredicate(CanHaveHuman);
    //   // 食物类作坊不需要村民，其它类型作坊需要
    //   return allMatch && (hasHuman || CardUtils.IsFoodById(resultCard));
    // }
  }
}