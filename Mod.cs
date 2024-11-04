﻿using HarmonyLib;
using System.Reflection;
using UnityEngine;
using ZjaveStacklandsPlus.Scripts;
using ZjaveStacklandsPlus.Scripts.Workshops;

namespace ZjaveStacklandsPlus
{
    public class StickWorkshopMod : Mod
    {
        // 集合所有超级农场等等生长型作坊的cardId，目前而言官方可生长类型卡牌只有5个
        public static string[] superGrowMethods = new string[5];
        /**
         * Awake 是 Unity 引擎中的生命周期方法之一，在游戏对象被实例化时调用。
         * 
         * 何时使用 Awake？
         * 
         * 在加载任何游戏数据之前会调用 Awake。这是您应该创建和声补丁、扩展枚举等的部分。
         * ModManager是目前唯一可用的管理器，因此尝试访问其他管理器类将导致错误。
         */
        public void Awake()
        {
            // TODO 发布时设为false
            Debug.unityLogger.logEnabled = true;
            Harmony.PatchAll(typeof(Patches));
        }

        /**
         * Ready() 方法是 Mod 类的重写方法，在游戏准备好加载模组时调用。在这里，插件会对游戏的数据进行修改和扩展。
         * 何时使用 Ready？
         * 
         * WorldManager.CardDataPrefabs 所有游戏数据加载完成后，会调用 Ready。
         * 例如，如果你想修改数据，就应该在此时进行修改
         */
        public override void Ready()
        {
            Logger.Log("Ready!");

            AddCardToSetCardBag(SetCardBagType.BasicBuildingIdea, "zjave_blueprint_food_chest", 1);
            // 获取当前程序集中的所有类型
            Type[] allTypes = Assembly.GetExecutingAssembly().GetTypes();
            AddCardsToSetBasicBuildingIdeaCardBag(allTypes);
        }

        /// <summary>
        /// 获取命名空间中的所有继承了ZjaveWorkshop的类，并通过反射获取静态字段 blueprintId，
        /// 将其通过 AddCardToSetCardBag 方法添加到 CardBag
        /// </summary>
        private void AddCardsToSetBasicBuildingIdeaCardBag(Type[] allTypes) {
            // 筛选出 ZjaveStacklandsPlus 命名空间下，继承自 ZjaveWorkshop 的类型
            var workshopTypes = allTypes.Where(t =>
                t.IsClass &&                     // 需要是类
                t.Namespace == "ZjaveStacklandsPlus" &&  // 限定在指定命名空间
                t.IsSubclassOf(typeof(ZjaveWorkshop))   // 继承自 ZjaveWorkshop
            );

            // 遍历所有找到的类型，获取静态字段 blueprintId
            foreach (Type type in workshopTypes)
            {
                // 获取 blueprintId 静态字段
                FieldInfo fieldInfo = type.GetField("blueprintId", BindingFlags.Static | BindingFlags.Public);
                if (fieldInfo != null)
                {
                    try
                    {
                        var blueprintId = fieldInfo.GetValue(null) as string; // 静态字段，无需实例化类，传递 null
                        WorldManager.instance.GameDataLoader.AddCardToSetCardBag(SetCardBagType.BasicBuildingIdea, blueprintId, 1);
                    } catch (Exception e) {
                        Logger.Log(e.ToString());
                    }
                }
            }
        }

        public static void GetAllGrowableCardIds(Type[] allTypes)
        {
            int index = 0;
            // 遍历程序集中的所有类型
            foreach (Type type in allTypes)
            {
                if (index >= superGrowMethods.Length)
                {
                    break;
                }
                // 检查是否在目标命名空间中并且是否继承自 ZjaveGreenhouse
                if (type.Namespace == "ZjaveStacklandsPlus.Scripts.Workshops"
                    && type.IsSubclassOf(typeof(ZjaveGreenhouse)))
                {
                    // 尝试获取静态字段 cardId
                    FieldInfo fieldInfo = type.GetField("cardId", BindingFlags.Public | BindingFlags.Static);
                    
                    if (fieldInfo != null && fieldInfo.FieldType == typeof(string))
                    {
                        // 获取 cardId 的值
                        string? cardIdValue = fieldInfo.GetValue(null) as string;

                        // 确保 cardId 不为空或 null
                        if (!string.IsNullOrEmpty(cardIdValue))
                        {
                            superGrowMethods[index] = cardIdValue;
                            index++;
                        }
                    }
                }
            }
        }

        private void AddCardToSetCardBag(SetCardBagType setCardBagType, string cardId, int chance)
        {
            WorldManager.instance.GameDataLoader.AddCardToSetCardBag(setCardBagType, cardId, chance);
        }
    }
}