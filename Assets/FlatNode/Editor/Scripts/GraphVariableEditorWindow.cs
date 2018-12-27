using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public class GraphVariableEditorWindow : EditorWindow
    {
        public static GraphVariableEditorWindow instance;

        /// <summary>
        /// 目前支持的类型
        /// 要添加新的类型依次往后添加，不要改变已有类型的顺序
        /// </summary>
        private static readonly List<Type> supportTypeList = new List<Type>
        {
            typeof(int), typeof(float), typeof(string), typeof(bool), typeof(Vector3), typeof(Vector2),
            typeof(Quaternion)
        };

        private string[] supportTypeNames;
        private Vector2 scrollPosition;

        private GraphEditorData data;
        private double repaintTimer;

        [MenuItem("FlatNode/变量编辑器",priority = 1)]
        public static void ShowWindow()
        {
            GraphVariableEditorWindow graphEditorWindow = GetWindow<GraphVariableEditorWindow>();
            graphEditorWindow.minSize = new Vector2(300, 400);
            graphEditorWindow.titleContent = new GUIContent("变量编辑器");
        }

        private void OnEnable()
        {
            supportTypeNames = null;
        }

        private void OnDestroy()
        {
            instance = null;
        }

        private void OnGUI()
        {
            instance = this;


            //初始化
            if (supportTypeNames == null)
            {
                supportTypeNames = new string[supportTypeList.Count];

                for (int i = 0; i < supportTypeList.Count; i++)
                {
                    supportTypeNames[i] = Utility.BeautifyTypeName(supportTypeList[i]);
                }
            }

            //draw title 
            GUILayout.Label("行为图属性", Utility.GetGuiStyle("Title"));
            GUILayout.Space(10);

            GraphEditorWindow graphEditorWindow = GraphEditorWindow.instance;
            if (graphEditorWindow == null || graphEditorWindow.data == null)
            {
                GUILayout.Label("请先打开行为图编辑器窗口", Utility.GetGuiStyle("SkillGraphName"));
                return;
            }

            data = graphEditorWindow.data;

            DrawToolButtons();

            GUILayout.Space(30);

            DrawLocalVariableItemList();
        }

        private void Update()
        {
            if (EditorApplication.timeSinceStartup - repaintTimer > 0.5)
            {
                repaintTimer = EditorApplication.timeSinceStartup;
                Repaint();
            }
        }

        private void DrawToolButtons()
        {
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            //增加变量
            if (GUILayout.Button("增加变量", GUILayout.MinWidth(100), GUILayout.MinHeight(30)))
            {
                if (data.CurrentSkillVariableInfoList == null)
                {
                    return;
                }

                int newId = GetNewVariableId();
                data.CurrentSkillVariableInfoList.Add(new GraphVariableInfo
                {
                    id = newId
                });
            }

            GUILayout.EndHorizontal();
        }

        private void DrawLocalVariableItemList()
        {
            if (data.CurrentSkillVariableInfoList == null)
            {
                return;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < data.CurrentSkillVariableInfoList.Count; i++)
            {
                GraphVariableInfo variableInfo = data.CurrentSkillVariableInfoList[i];

                if (DrawLocalVariableItem(variableInfo))
                {
                    data.CurrentSkillVariableInfoList.RemoveAt(i);
                    GraphEditorWindow.instance.data.OnGraphVariableListChange();
                    break;
                }

                GUILayout.Space(10);
            }

            GUILayout.EndScrollView();
        }

        /// <summary>
        /// 绘制单个变量内容
        /// </summary>
        /// <param name="graphVariableInfo"></param>
        /// <returns>true:玩家对这个item进行了删除操作</returns>
        private bool DrawLocalVariableItem(GraphVariableInfo graphVariableInfo)
        {
            GUILayout.BeginVertical(Utility.GetGuiStyle("LocalVariableItemBg"));

            GUILayout.BeginHorizontal();
            //变量id
            GUILayout.Label("id: " + graphVariableInfo.id);

            GUILayout.FlexibleSpace();

            //删除当前变量
            if (GUILayout.Button("X"))
            {
                return true;
            }

            GUILayout.EndHorizontal();

            //类型选择
            GUILayout.BeginHorizontal();
            GUILayout.Label("类型: ", GUILayout.MaxWidth(30));
            int typeIndex = GetTypeIndex(graphVariableInfo.valueType);
            if (typeIndex < 0)
            {
                typeIndex = 0;
                graphVariableInfo.valueType = supportTypeList[0];
            }

            EditorGUI.BeginChangeCheck();
            int selectIndex =
                EditorGUILayout.Popup(typeIndex, supportTypeNames, GUILayout.MinHeight(20), GUILayout.MaxWidth(150));
            if (EditorGUI.EndChangeCheck())
            {
                Type selectedType = GetTypeAtIndex(selectIndex);
                if (selectedType == null)
                {
                    return false;
                }

                graphVariableInfo.valueType = selectedType;

                //更改了类型之后，也需要检查连线是否还合法
                GraphEditorWindow.instance.data.OnGraphVariableListChange();
            }

            GUILayout.EndHorizontal();

            //变量名称
            string valueName = graphVariableInfo.name;
            if (valueName == null)
            {
                valueName = string.Empty;
            }

            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("变量备注: ", GUILayout.MaxWidth(50));

            EditorGUI.BeginChangeCheck();
            valueName = EditorGUILayout.TextField(valueName);
            if (EditorGUI.EndChangeCheck())
            {
                graphVariableInfo.name = valueName;
            }

            GUILayout.EndHorizontal();

            //变量初始值编辑
            DrawValueEditField(graphVariableInfo);

            GUILayout.EndVertical();

            return false;
        }

        /// <summary>
        /// 根据不同类型，绘制不同的输入框
        /// </summary>
        /// <param name="graphVariableInfo"></param>
        private void DrawValueEditField(GraphVariableInfo graphVariableInfo)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("初始值: ", GUILayout.MaxWidth(60));

            Type valueType = graphVariableInfo.valueType;
            string valueString = graphVariableInfo.valueString;
            if (valueString == null)
            {
                graphVariableInfo.valueString = string.Empty;
                valueString = string.Empty;
            }

            if (valueType == typeof(int))
            {
                int intValue;
                if (!int.TryParse(valueString, out intValue))
                {
                    intValue = 0;
                }

                EditorGUI.BeginChangeCheck();
                intValue = EditorGUILayout.IntField(intValue);
                if (EditorGUI.EndChangeCheck())
                {
                    graphVariableInfo.valueString = intValue.ToString();
                }
            }
            else if (valueType == typeof(float))
            {
                float floatValue;
                if (!float.TryParse(valueString, out floatValue))
                {
                    floatValue = 0f;
                }

                EditorGUI.BeginChangeCheck();
                floatValue = EditorGUILayout.FloatField(floatValue);
                if (EditorGUI.EndChangeCheck())
                {
                    graphVariableInfo.valueString = floatValue.ToString("0.00");
                }
            }
            else if (valueType == typeof(bool))
            {
                bool boolValue;
                if (!bool.TryParse(valueString, out boolValue))
                {
                    boolValue = false;
                }

                EditorGUI.BeginChangeCheck();
                boolValue = EditorGUILayout.Toggle(boolValue);
                if (EditorGUI.EndChangeCheck())
                {
                    graphVariableInfo.valueString = boolValue.ToString();
                }
            }
            else if (valueType == typeof(string))
            {
                graphVariableInfo.valueString = EditorGUILayout.TextField(valueString);
            }
            else
            {
                EditorGUILayout.LabelField("无法设定初始值，需要运行时设置");
            }

            GUILayout.EndHorizontal();
        }

        private int GetTypeIndex(Type type)
        {
            if (type == null)
            {
                return -1;
            }

            return supportTypeList.IndexOf(type);
        }

        private Type GetTypeAtIndex(int typeIndex)
        {
            if (typeIndex < 0 || typeIndex >= supportTypeList.Count)
            {
                Debug.LogError("获取的类型index超出范围: " + typeIndex);
                return null;
            }

            return supportTypeList[typeIndex];
        }

        /// <summary>
        /// 分配一个新的id
        /// todo 也许可以放到SkillEditorData里面去
        /// </summary>
        /// <returns></returns>
        private int GetNewVariableId()
        {
            if (data.CurrentSkillVariableInfoList == null)
            {
                return 0;
            }

            for (int i = 0; i < int.MaxValue; i++)
            {
                bool hasAssigned = false;
                for (int j = 0; j < data.CurrentSkillVariableInfoList.Count; j++)
                {
                    int assignedId = data.CurrentSkillVariableInfoList[j].id;

                    if (i == assignedId)
                    {
                        hasAssigned = true;
                        break;
                    }
                }

                if (!hasAssigned)
                {
                    return i;
                }
            }

            Debug.LogError("从0到int.maxvalue的id都被分配了？");
            return int.MaxValue;
        }

        public void Refresh()
        {
            Repaint();
        }
    }
}