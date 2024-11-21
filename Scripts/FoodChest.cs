using UnityEngine;
using UnityEngine.InputSystem;

namespace ZjaveStacklandsPlus.Scripts
{
  class FoodChest : ResourceChest
  {
    /// <summary>
    /// 食物值列表
    /// </summary>
    [ExtraData("food_values")]
    [HideInInspector]
    public string FoodValues = "";

    public static string cardId = "zjave_food_chest";
    public static string blueprintId = "zjave_blueprint_food_chest";

    public FoodChest() {
      EnergyConnectors.Add(new CardConnectorData { EnergyConnectionType = CardDirection.input, EnergyConnectionStrength = ConnectionType.Transport, EnergyConnectionAmount = 3 });
      EnergyConnectors.Add(new CardConnectorData { EnergyConnectionType = CardDirection.output, EnergyConnectionStrength = ConnectionType.Transport, EnergyConnectionAmount = 3 });
    }

    protected override bool CanHaveCard(CardData otherCard)
    {
      // if代码或许有先后顺序的问题，因此重写方法也保证这个顺序
      if (!string.IsNullOrEmpty(HeldCardId) && otherCard.Id != HeldCardId)
      {
        return false;
      }

      if (otherCard.MyCardType != CardType.Food || otherCard.Id == "gold" || otherCard.Id == "shell")
      {
        return false;
      }
      return true;
    }

    public List<string> FoodValueList
    {
      get => [.. FoodValues.Split(',')];
      set => FoodValues = string.Join(",", value);
    }

    public virtual void AddFood(int foodValue)
    {
      List<string> CopyFoodValueList = [.. FoodValueList, foodValue.ToString()];
      CopyFoodValueList = CopyFoodValueList.Where(x => !string.IsNullOrEmpty(x)).ToList();
      FoodValues = string.Join(",", CopyFoodValueList);
    }

    public virtual int? PopFood()
    {
      if (string.IsNullOrEmpty(FoodValues))
      {
        return null;
      }
      
      string value = FoodValueList[0];
      try
      {
        int foodValue = int.Parse(value);
        FoodValueList = [.. FoodValueList.Skip(1)];
        Debug.LogFormat("PopFood Post 食物值 {0}", foodValue);
        return foodValue;
      }
      catch (Exception e)
      {
        Debug.LogErrorFormat("无法解析食物值 {0} {1}", value, e);
        return null;
      }
    }

    public virtual int FoodValueCount
    {
      get {
        try
        {
          int total = 0;
          foreach (var item in FoodValueList)
          {
            int foodValue = int.Parse(item);
            total += foodValue;
          }
          return total;
        }
        catch (Exception e)
        {
          Debug.LogErrorFormat("无法解析食物值 {0} {1}", FoodValues, e);
          return 0;
        }
      }
    }

    public virtual Sprite GetSpecialIcon()
    {
      SpecialIcon ??= SpriteManager.instance.FootFightIcon;
      return SpecialIcon;
    }

    public override void UpdateCard()
    {
      if (!MyGameCard.HasParent || MyGameCard.Parent.CardData is HeavyFoundation)
      {
        foreach (GameCard childCard in MyGameCard.GetChildCards())
        {
          if (string.IsNullOrEmpty(HeldCardId) || childCard.CardData.Id == HeldCardId)
          {
            if (ResourceCount >= MaxResourceCount)
            {
              Debug.LogFormat("食物箱子已满");
              break;
            }

            if (childCard.CardData is Food food)
            {
              AddFood(food.FoodValue);
              Debug.LogFormat("FoodValues {0}", FoodValues);
            }
            else
            {
              Debug.LogFormat("不是食物 {0}", childCard.CardData.GetType());
            }
          }
          else
          {
            Debug.LogFormat("不是食物箱子的子卡牌 {0}", childCard.CardData.GetType());
          }
        }
      }
      else
      {
        Debug.LogFormat("不是重型基座 {0}", MyGameCard.Parent.CardData.GetType());
      }
      base.UpdateCard();
      MyGameCard.SpecialIcon.sprite = GetSpecialIcon();

      if (string.IsNullOrEmpty(HeldCardId))
      {
        nameOverride = SokLoc.Translate("zjave_food_chest_name");
        descriptionOverride = null;
      }
    }

    public new GameCard RemoveResources(int count)
    {

      GameCard gameCard = base.RemoveResources(count);
      int? foodValue;
      if (gameCard.CardData is Food food && (foodValue = PopFood()) != null)
      {
        food.FoodValue = (int)foodValue;
      }
      return gameCard;
    }

	public override void Clicked()
	{
		if (!IsDamaged)
		{
			int count = 1;
			if (InputController.instance.GetKey(Key.LeftShift) || InputController.instance.GetKey(Key.RightShift))
			{
				count = 5;
			}
			if (ResourceCount > 0)
			{
				RemoveResources(count);
			}
			if (ResourceCount == 0)
			{
				HeldCardId = null;
			}
		}
	}
  }
}