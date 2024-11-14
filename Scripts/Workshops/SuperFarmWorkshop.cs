namespace ZjaveStacklandsPlus.Scripts.Workshops
{
  public class SuperFarm : ZjaveGreenhouse
  {
    public static string cardId = "zjave_super_farm";
    public static string blueprintId = "zjave_blueprint_super_farm";
    public SuperFarm() {
      EnergyConnectors.Add(new CardConnectorData { EnergyConnectionType = CardDirection.input, EnergyConnectionStrength = ConnectionType.Transport, EnergyConnectionAmount = 3 });
      EnergyConnectors.Add(new CardConnectorData { EnergyConnectionType = CardDirection.output, EnergyConnectionStrength = ConnectionType.Transport, EnergyConnectionAmount = 3 });
    }
  }
  
  public class SuperGarden : ZjaveGreenhouse
  {
    public static string cardId = "zjave_super_garden";
    public static string blueprintId = "zjave_blueprint_super_garden_workshop";
    public SuperGarden() {
      EnergyConnectors.Add(new CardConnectorData { EnergyConnectionType = CardDirection.input, EnergyConnectionStrength = ConnectionType.Transport, EnergyConnectionAmount = 3 });
      EnergyConnectors.Add(new CardConnectorData { EnergyConnectionType = CardDirection.output, EnergyConnectionStrength = ConnectionType.Transport, EnergyConnectionAmount = 3 });
    }
  }
  
  public class SuperGreenhouse : ZjaveGreenhouse
  {
    public static string cardId = "zjave_super_greenhouse";
    public static string blueprintId = "zjave_blueprint_super_greenhouse";
    public SuperGreenhouse() {
      EnergyConnectors.Add(new CardConnectorData { EnergyConnectionType = CardDirection.input, EnergyConnectionStrength = ConnectionType.Transport, EnergyConnectionAmount = 3 });
      EnergyConnectors.Add(new CardConnectorData { EnergyConnectionType = CardDirection.output, EnergyConnectionStrength = ConnectionType.Transport, EnergyConnectionAmount = 3 });
    }
  }

}
