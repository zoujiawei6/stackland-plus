# 开发人员文档

[前置学习内容](https://modding.stacklands.co/en/latest/guides/tutorial.html)，且你必须具备`Python`和`c#`开发能力。

## 编译

修改`Mod.csproj.user`中的游戏目录为你电脑上的游戏目录：

```shell
python build.py
```

这会编译到`$USERPROFILE/AppData/LocalLow/sokpop/Stacklands/Mods`目录。

## 作坊生成器

安装python依赖包：

```shell
pip install pandas
```

参照`recipes.1.tsv`向`recipes.tsv`增加内容。

然后，运行`workshop_creator.py`：

```shell
python workshop_creator.py
```

### 调色

[调色工具](https://gradients.app/zh/colorpalette)

## 测试

对于使用`Visual Studio Code`的开发者，还需要安装：

* [C#支持](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csharp)
* [调试工具](https://marketplace.visualstudio.com/items?itemName=ms-dotnettools.csdevkit)。

## 持久化存储

### 持久化原理

官方代码中自定义了一个属性类`ExtraDataAttribute`。

然后在`CardData`的`GetExtraCardData`函数中有读取`[ExtraData("XXX")]`属性修饰的字段，但它没有通过反射去读取`private`修饰的字段，因此`ExtraData`只能修饰`public`类型的字段。

最后`GameCard`的`ToSavedCard`函数中调用了这个函数，逐层向上至`SaveManager`的`Save`函数。

### 持久化最佳实践

不封装字段的话就难以知道这个字段状态变化，这为扩展`Mod`造成了较大的难度。因此建议的最佳实践是开放且提供封装字段：

```c#
using UnityEngine;
using ZjaveStacklandsPlus.Scripts.Common;

namespace ZjaveStacklandsPlus.Scripts
{
  class Worker : Villager, IWorkLevel
  {
    [ExtraData("WorkLevel")]
    public int workLevel = 1;

    public int WorkLevel
    {
      get => workLevel;
      set => workLevel = value;
    }
  }
}
```

而你在代码中应当使用`instance.WorkLevel = 2`，而不是`instance.workLevel = 2`。

## 参考内容

### GameScripts类列表

| Name                        | 中文             |
| --------------------------- | ---------------- |
| AccessibilityScreen         | 可访问性屏幕     |
| AchievementElement          | 成就元素         |
| AchievementHelper           | 成就助手         |
| ActionTimeBase              | 动作时间基准     |
| ActionTimeModifier          | 动作时间修改器   |
| ActionTimeParams            | 动作时间参数     |
| AdvancedSettingsScreen      | 高级设置屏幕     |
| AllQuests                   | 所有任务         |
| Altar                       | 祭坛             |
| AngryRoyal                  | 愤怒的皇室       |
| Animal                      | 动物             |
| AnimalPen                   | 动物围栏         |
| AttackAnimation             | 攻击动画         |
| AttackAnimationMagic        | 攻击动画魔法     |
| AttackAnimationMelee        | 攻击动画近战     |
| AttackAnimationRanged       | 攻击动画远程     |
| AttackDamage                | 攻击伤害         |
| AttackSpeed                 | 攻击速度         |
| AttackType                  | 攻击类型         |
| AudioClipWithVolume         | 带音量的音频剪辑 |
| AudioManager                | 音频管理器       |
| BabyDragon                  | 小龙             |
| BackgroundRegion            | 背景区域         |
| BaitBag                     | 诱饵袋           |
| Barrel                      | 桶               |
| BaseVillager                | 基地村民         |
| Blueprint                   | 蓝图             |
| BlueprintAdmireCoin         | 蓝图钦佩硬币     |
| BlueprintAltar              | 蓝图祭坛         |
| BlueprintDeathCurseOver     | 蓝图死亡诅咒     |
| BlueprintFillBottle         | 蓝图填充瓶       |
| BlueprintFountainOfYouth    | 蓝图青春之泉     |
| BlueprintGreedCurseOver     | 蓝图贪婪诅咒     |
| BlueprintGroup              | 蓝图组           |
| BlueprintGrowth             | 蓝图成长         |
| BlueprintHappiness          | 蓝图幸福         |
| BlueprintHappinessCurseOver | 蓝图幸福诅咒     |
| BlueprintOffspring          | 蓝图后代         |
| BlueprintRecipe             | 蓝图配方         |
| Board                       | 木板               |
| BoardBackground             | 木板背景           |
| BoardMonths                 | 木板月             |
| BoardOptions                | 木板选项           |
| Boat                        | 船               |
| Bone                        | 骨头             |
| BoosterAddition             | 助推器添加       |
| Boosterpack                 | 助推器包         |
| BoosterpackData             | 助推器包数据     |
| BreedingPen                 | 育种围栏         |
| Brickyard                   | 砖厂             |
| Building                    | 建筑             |
| BuyBoosterBox               | 购买助推器盒     |
| CardAmountPair              | 卡数量对         |
