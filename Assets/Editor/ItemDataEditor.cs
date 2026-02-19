using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEditor;
using FarmGame.Data;

namespace FarmGame.Editor
{
    /// <summary>
    /// ItemData 自定义编辑器
    /// 根据 category 动态显示/隐藏字段，提供更友好的 Inspector 体验
    /// 
    /// 设计原则：
    /// 1. 保留 OnValidate 核心验证（ID、名称、图标、价格）
    /// 2. Custom Editor 只做动态面板显示
    /// 3. DrawRemainingProperties 确保子类字段不被吞掉
    /// </summary>
    [CustomEditor(typeof(ItemData), true)]
    public class ItemDataEditor : UnityEditor.Editor
    {
        #region 字段分组定义
        
        // 基础信息字段
        private static readonly string[] BasicInfoFields = new string[]
        {
            "itemID", "itemName", "description", "category"
        };
        
        // 视觉资源字段
        private static readonly string[] VisualsFields = new string[]
        {
            "icon", "bagSprite", "rotateBagIcon", "worldPrefab"
        };
        
        // 经济属性字段
        private static readonly string[] EconomyFields = new string[]
        {
            "buyPrice", "sellPrice"
        };
        
        // 堆叠属性字段
        private static readonly string[] StackFields = new string[]
        {
            "maxStackSize"
        };
        
        // 显示尺寸配置字段
        private static readonly string[] DisplaySizeFields = new string[]
        {
            "useCustomBagDisplaySize", "bagDisplayPixelSize", "bagDisplayOffset",
            "useCustomDisplaySize", "displayPixelSize", "worldDisplayOffset"
        };
        
        // 功能标记字段
        private static readonly string[] FunctionFields = new string[]
        {
            "canBeDiscarded", "isQuestItem"
        };
        
        // 放置配置字段（仅 Placeable 类型显示）
        private static readonly string[] PlacementFields = new string[]
        {
            "isPlaceable", "placementType", "placementPrefab", "buildingSize"
        };
        
        // 装备配置字段（仅 Equipment 类型显示）
        private static readonly string[] EquipmentFields = new string[]
        {
            "equipmentType"
        };
        
        // 消耗品配置字段（仅 Consumable 类型显示）
        private static readonly string[] ConsumableFields = new string[]
        {
            "consumableType"
        };
        
        // 所有已处理的字段（用于 DrawRemainingProperties）
        private HashSet<string> _handledProperties;
        
        #endregion
        
        #region 缓存
        
        private SerializedProperty _categoryProp;
        private ItemCategory _currentCategory;
        private HashSet<string> _obsoleteFields;
        
        #endregion
        
        #region Unity 生命周期
        
        private void OnEnable()
        {
            _categoryProp = serializedObject.FindProperty("category");
            _obsoleteFields = null; // 切换目标时重新构建
            BuildHandledPropertiesSet();
        }
        
        public override void OnInspectorGUI()
        {
            serializedObject.Update();
            
            // 获取当前 category
            if (_categoryProp != null)
            {
                _currentCategory = (ItemCategory)_categoryProp.enumValueIndex;
            }
            
            // 绘制 Script 字段（只读）
            DrawScriptField();
            
            // 绘制基础信息（不添加手动标题，使用 ItemData 自带的 [Header]）
            DrawProperties(BasicInfoFields);
            
            // 绘制视觉资源
            DrawProperties(VisualsFields);
            
            // 绘制经济属性
            DrawProperties(EconomyFields);
            
            // 绘制堆叠属性
            DrawProperties(StackFields);
            
            // 绘制显示尺寸配置
            DrawProperties(DisplaySizeFields);
            
            // 绘制功能标记
            DrawProperties(FunctionFields);
            
            // 条件显示：放置配置（仅 Furniture 类型或启用 isPlaceable）
            if (ShouldShowPlacementConfig())
            {
                DrawProperties(PlacementFields);
            }
            
            // 条件显示：装备配置（仅 Tool 类型）
            if (ShouldShowEquipmentConfig())
            {
                DrawProperties(EquipmentFields);
            }
            
            // 条件显示：消耗品配置（仅 Consumable 或 Food 类型）
            if (ShouldShowConsumableConfig())
            {
                DrawProperties(ConsumableFields);
            }
            
            // 绘制子类新增的字段（确保不被吞掉）
            DrawRemainingProperties();
            
            serializedObject.ApplyModifiedProperties();
        }
        
        #endregion
        
        #region 绘制方法
        
        private void DrawScriptField()
        {
            var scriptProp = serializedObject.FindProperty("m_Script");
            if (scriptProp != null)
            {
                using (new EditorGUI.DisabledScope(true))
                {
                    EditorGUILayout.PropertyField(scriptProp);
                }
            }
        }
        
        /// <summary>
        /// 绘制指定字段列表（不添加手动标题，使用 ItemData 自带的 [Header]）
        /// </summary>
        private void DrawProperties(string[] fieldNames)
        {
            foreach (var fieldName in fieldNames)
            {
                var prop = serializedObject.FindProperty(fieldName);
                if (prop != null)
                {
                    EditorGUILayout.PropertyField(prop, true);
                }
            }
        }
        
        /// <summary>
        /// 绘制所有未被手动处理的属性（子类兼容性）
        /// 确保子类新增的字段不会因为父类 Editor 而在面板上消失
        /// 自动过滤带 [System.Obsolete] 标记的字段（不在 Inspector 中显示废弃字段）
        /// </summary>
        private void DrawRemainingProperties()
        {
            // 构建废弃字段集合（缓存到首次使用）
            if (_obsoleteFields == null)
            {
                _obsoleteFields = new HashSet<string>();
                var targetType = target.GetType();
                while (targetType != null && targetType != typeof(UnityEngine.Object))
                {
                    var fields = targetType.GetFields(
                        BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.DeclaredOnly);
                    foreach (var field in fields)
                    {
                        if (field.GetCustomAttributes(typeof(System.ObsoleteAttribute), true).Any())
                        {
                            _obsoleteFields.Add(field.Name);
                        }
                    }
                    targetType = targetType.BaseType;
                }
            }

            SerializedProperty iterator = serializedObject.GetIterator();
            bool enterChildren = true;
            bool hasDrawnHeader = false;
            
            while (iterator.NextVisible(enterChildren))
            {
                enterChildren = false;
                
                // 跳过脚本引用和已处理的属性
                if (iterator.name == "m_Script")
                    continue;
                    
                if (_handledProperties.Contains(iterator.name))
                    continue;

                // 跳过带 [Obsolete] 标记的废弃字段
                if (_obsoleteFields.Contains(iterator.name))
                    continue;
                
                // 第一次绘制未处理属性时，添加分隔标题
                if (!hasDrawnHeader)
                {
                    EditorGUILayout.Space(10);
                    EditorGUILayout.LabelField("=== 子类专属属性 ===", EditorStyles.boldLabel);
                    hasDrawnHeader = true;
                }
                
                EditorGUILayout.PropertyField(iterator, true);
            }
        }
        
        #endregion
        
        #region 条件判断
        
        private bool ShouldShowPlacementConfig()
        {
            // Furniture 类型（可放置家具）或已启用 isPlaceable
            var isPlaceableProp = serializedObject.FindProperty("isPlaceable");
            return _currentCategory == ItemCategory.Furniture || 
                   (isPlaceableProp != null && isPlaceableProp.boolValue);
        }
        
        private bool ShouldShowEquipmentConfig()
        {
            // Tool 类型（包含工具和武器）
            return _currentCategory == ItemCategory.Tool;
        }
        
        private bool ShouldShowConsumableConfig()
        {
            // Consumable 或 Food 类型
            return _currentCategory == ItemCategory.Consumable ||
                   _currentCategory == ItemCategory.Food;
        }
        
        #endregion
        
        #region 辅助方法
        
        private void BuildHandledPropertiesSet()
        {
            _handledProperties = new HashSet<string>
            {
                "m_Script"
            };
            
            AddToSet(BasicInfoFields);
            AddToSet(VisualsFields);
            AddToSet(EconomyFields);
            AddToSet(StackFields);
            AddToSet(DisplaySizeFields);
            AddToSet(FunctionFields);
            AddToSet(PlacementFields);
            AddToSet(EquipmentFields);
            AddToSet(ConsumableFields);
        }
        
        private void AddToSet(string[] fields)
        {
            foreach (var field in fields)
            {
                _handledProperties.Add(field);
            }
        }
        
        #endregion
    }
}
