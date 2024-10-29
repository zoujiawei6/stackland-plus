# 改进Stacklands

[本Mod开源](https://github.com/zoujiawei6/stackland-plus)

Language: English/语言:简体中文/語言：繁體中文

为Mod提供翻译：[点击链接](https://docs.google.com/spreadsheets/d/1y8gX4RXLmBSlPuslm2VQ52LOpDLZgcWI_LKRpxwh82k/edit?usp=sharing)

## news

### Workshop cards

| workshop             | result_card | required_cards                                         | have_cards         | value | icon        | time |
| -------------------- | ----------- | ------------------------------------------------------ | ------------------ | ----- | ----------- | ---- |
| Stick Workshop       | stick       | stick, wood, wood, stone, stone, villager              | wood               | 5     | stick       | 15   |
| Fruit Salad Workshop | fruit_salad | apple, berry, wood, wood, stone, stone, villager       | apple, berry       | 5     | fruit_salad | 15   |
| Shed Workshop        | shed        | wood, wood, stone, stone, villager, stone, stick, wood | stone, stick, wood | 5     | shed        | 30   |
| Warehouse Workshop   | warehouse   | wood, wood, stone, stone, villager, iron_bar, stone    | iron_bar, stone    | 5     | warehouse   | 30   |
| Milkshake Workshop   | milkshake   | wood, wood, stone, stone, villager, milk, berry        | milk, berry        | 5     | milkshake   | 15   |

Tips: A workshop must have a villager to produce resources

### Other cards

| result_card | required_cards                       |
| ----------- | ------------------------------------ |
| food_chest  | plank, plank, iron_bar, flint, flint |

## 新增卡牌

### 建筑

* 食物箱：新增能储存食物的箱子。

#### 作坊类

| 作坊         | 合成材料                         |
| ------------ | -------------------------------- |
| 水果沙拉作坊 | 木头x2，石头x2，村民，苹果，浆果 |
| 木棍作坊     | 木头x2，石头x2，村民，木棍       |
| 棚屋作坊     | 木头x3，石头x2，村民，石头，木棍 |
| 仓库作坊     | 木头x2，石头x3，村民，铁块       |
| 奶昔作坊     | 木头x2，石头x2，村民，牛奶，浆果 |

我认为作坊需要人才能运转，因此在作坊上你必需放置一个村民，才能使其工作。

所有作坊的建造材料有一个通用公式：

> 都需要“木头x2，石头x2，村民，成品材料”才能建造。

比如“水果沙拉作坊”，水果沙拉的原材料为“苹果，浆果”，加上通用材料“木头x2，石头x2，村民”，那么它的合成材料为：

> 木头x2，石头x2，村民，苹果，浆果

## 版本记录

| 作坊           | 版本  |
| -------------- | ----- |
| 水果沙拉作坊   | 0.0.1 |
| 木棍作坊       | 0.0.2 |
| 自动化生成作坊 | 0.0.3 |
| 棚屋作坊       | 0.2.0 |
| 仓库作坊       | 0.3.0 |
| 奶昔作坊       | 0.4.0 |
| 食物箱         | 0.5.0 |
