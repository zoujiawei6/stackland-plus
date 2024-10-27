import json
import csv
from string_utils import StringUtils
from file_utils import FileUtils
from collections import Counter

class WorkshopCreator:
    """
    WorkshopCreator 类用于根据输入字符串生成 JSON 文件，并将文件保存到指定位置。
    """

    def __init__(self, card_json_path="./Cards", blueprint_json_path="./Blueprints"):
        """
        初始化方法，设定 JSON 文件的 schema 路径。

        :param card_json_path: str, JSON 文件中引用的 schema 路径
        """
        self.card_json_path = card_json_path
        self.blueprint_json_path = blueprint_json_path

    def create_blueprint_json(self, ingredient: str, required_cards: str = None, time: int = 3):
        """
        创建包含作坊蓝图的 JSON 文件。

        :param ingredient: str, 指定作坊生成何种资源
        :param required_cards: str, 建造作坊所需的卡牌
        :return: None
        """
        # 定义 JSON 文件的结构
        card = {
            "$schema": "../schemas/blueprint.schema.json",
            "id": f"zjave_blueprint_{ingredient}_workshop",  # 替换字符串 salad
            "nameTerm": f"zjave_{ingredient}_workshop_name",  # 替换字符串 salad
            "group": "Building",
            "subprints": [
                {
                    "RequiredCards": required_cards,
                    "ResultCard": f"zjave_{ingredient}_workshop",  # 替换字符串 salad
                    "Time": time,
                    "StatusTerm": f"zjave_{ingredient}_workshop_status"  # 替换字符串 salad
                }
            ]
        }
        filename = f"{self.blueprint_json_path}/{ingredient}_workshop.json"
        FileUtils.write_to_json_file(card, filename)

    def create_card_json(self, ingredient: str, value: int = 5, icon: str = None):
        """
        创建包含作坊卡牌的 JSON 文件。

        :param ingredient: str, 指定作坊生成何种资源
        :param value: int, 作坊卡牌的价值
        :param icon: str, 作坊卡牌的图标
        :return: None
        """
        # ingredient下滑线转驼峰形式
        ingredient_cu = StringUtils.snake_to_camel_uppercase(ingredient)
        icon = icon if icon else ingredient
        # 定义 JSON 文件的结构
        card = {
          "$schema": "../schemas/card.schema.json",
          "id": f"zjave_{ingredient}_workshop",
          "nameTerm": f"zjave_{ingredient}_workshop_name",
          "descriptionTerm": f"zjave_{ingredient}_workshop_description",
          "icon": icon,
          "value": value,
          "type": "Structures",
          "script": f"ZjaveWorkshopModNS.{ingredient_cu}Workshop",
          "_IsBuilding": True,
          "_HasUniquePalette": True,
          "_MyPalette": {
            "Color": "#FF5743",
            "Color2": "#E35342",
            "Icon": "#FFFFFF"
          }
        }
        filename = f"{self.card_json_path}/{ingredient}_workshop.json"
        FileUtils.write_to_json_file(card, filename)

    def create_workshop_cs(self, result_card: str, have_cards: dict[str, int] = None, working_time: float = 10):
        """
        创建包含作坊脚本的 C# 文件。

        :param result_card: str, 指定作坊生成何种资源
        :return: None
        """
        # result_card下滑线转驼峰形式
        result_card_cu = StringUtils.snake_to_camel_uppercase(result_card)
        make_cards = [f"{{ \"{card}\", {have_cards[card]} }}" for card in have_cards]
        make_cards_str = ",\n      ".join(make_cards)
        # 定义 C# 文件的结构
        cs = f"""
  public class {result_card_cu}Workshop : ZjaveWorkshop
  {{
    public static string cardId = "zjave_{result_card}_workshop";
    public static string blueprintId = "zjave_blueprint_{result_card}_workshop";
    public {result_card_cu}Workshop() : base("{result_card}", Cards.{result_card}, {working_time}, new Dictionary<string, int> {{
      {make_cards_str}
    }})
    {{
    }}
  }}
"""
        filename = f"./ZjaveWorkshops.cs"
        FileUtils.insert_before_last_brace(cs, filename)

    def simple_create(self, result_card: str, required_cards: str = None, have_cards: dict[str, int] = None, value: int = 5, icon: str = None, time: int = 3, working_time: int = 10):
        """
        创建包含作坊卡牌和蓝图的 JSON 文件。

        :param result_card: str, 指定作坊生成何种资源
        :param required_cards: str, 建造作坊所需的卡牌
        :param have_cards: list, 作坊使用这些资源来生产卡牌
        :param value: int, 作坊卡牌的价值
        :param icon: str, 作坊卡牌的图标
        :return: None
        """
        self.create_card_json(result_card, value, icon)
        self.create_blueprint_json(result_card, required_cards, time)
        self.create_workshop_cs(result_card, have_cards, working_time)
        self.append_to_localization(result_card)

    def append_to_localization(self, ingredient: str):
        # ingredient下滑线转驼峰形式
        ingredient_cu = StringUtils.snake_to_camel_uppercase(ingredient)
        # 定义要追加的数据
        data = [
            [f"zjave_{ingredient}_workshop_name", "", f"{ingredient_cu} Workshop", f"{ingredient_cu}作坊", f"{ingredient_cu}作坊"],
            [f"zjave_{ingredient}_workshop_description", "", f"Synthetic {ingredient_cu}", f"合成{ingredient_cu}", f"合成{ingredient_cu}"],
            [f"zjave_{ingredient}_workshop_status", "", "Working", "工作中...", "工作中..."]
        ]
        file_path = f"./localization.tsv"

        result = FileUtils.append_to_csv_file(data, file_path)
        if result:
            print(f"已生成国际化翻译（记得修改）： {file_path}")
            print("可以使用GPT自动补全其它国家语言的国际化")

# 使用工具类
if __name__ == "__main__":
    # 创建 WorkshopCreator 对象
    recipes_tsv = FileUtils.read_recipes_tsv()
    if recipes_tsv.empty:
        print("未找到 recipes.tsv 文件或文件内容为空。")
        exit(1)

    creator = WorkshopCreator()
    # 使用 for 循环读取每一列并赋值给变量
    for index, row in recipes_tsv.iterrows():
        result_card = row['result_card']
        required_cards = row['required_cards']
        have_cards = row['have_cards']
        value = row['value']
        icon = row['icon']
        time = row['time']
        working_time = row['working_time']
            
        # 使用 Counter 计算 have_cards 中每个卡片的数量
        have_cards_list = [card.strip() for card in have_cards.split(',')]
        have_cards_count = dict(Counter(have_cards_list))
        creator.simple_create(result_card, required_cards, have_cards_count, value, icon, time, working_time)
    
    print("\n\n若需生成schemas文件夹中的内容，请使用游戏中自带的Generating Schemas生成内容到schemas文件夹中")
