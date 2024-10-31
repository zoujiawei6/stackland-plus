namespace ZjaveStacklandsPlus.Scripts
{
  
  public class StickWorkshop : ZjaveWorkshop
  {
    public static string cardId = "zjave_stick_workshop";
    public static string blueprintId = "zjave_blueprint_stick_workshop";
    public StickWorkshop() : base("stick", Cards.stick, 7, new Dictionary<string, int> {
      { "wood", 1 }
    })
    {
    }
  }

  public class FruitSaladWorkshop : ZjaveWorkshop
  {
    public static string cardId = "zjave_fruit_salad_workshop";
    public static string blueprintId = "zjave_blueprint_fruit_salad_workshop";
    public FruitSaladWorkshop() : base("fruit_salad", Cards.fruit_salad, 7, new Dictionary<string, int> {
      { "apple", 1 },
      { "berry", 1 }
    })
    {
    }
  }

  public class ShedWorkshop : ZjaveWorkshop
  {
    public static string cardId = "zjave_shed_workshop";
    public static string blueprintId = "zjave_blueprint_shed_workshop";
    public ShedWorkshop() : base("shed", Cards.shed, 20, new Dictionary<string, int> {
      { "stone", 1 },
      { "stick", 1 },
      { "wood", 1 }
    })
    {
    }
  }

  public class WarehouseWorkshop : ZjaveWorkshop
  {
    public static string cardId = "zjave_warehouse_workshop";
    public static string blueprintId = "zjave_blueprint_warehouse_workshop";
    public WarehouseWorkshop() : base("warehouse", Cards.warehouse, 20, new Dictionary<string, int> {
      { "iron_bar", 1 },
      { "stone", 1 }
    })
    {
    }
  }

  public class MilkshakeWorkshop : ZjaveWorkshop
  {
    public static string cardId = "zjave_milkshake_workshop";
    public static string blueprintId = "zjave_blueprint_milkshake_workshop";
    public MilkshakeWorkshop() : base("milkshake", Cards.milkshake, 7, new Dictionary<string, int> {
      { "milk", 1 },
      { "berry", 1 }
    })
    {
    }
  }

}