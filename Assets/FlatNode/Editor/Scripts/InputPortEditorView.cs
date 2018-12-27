using System;
using System.Collections.Generic;
using System.Linq;
using FlatNode.Runtime;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public class InputPortEditorView : PortEditorView
    {
        public InputPortReflectionInfo inputPortReflectionInfo;
        private const float LabelPadding = 50f;
        private const float LabelContentPadding = 10f;

        public Rect labelRect;
        private Rect labelContentRect;
        private Rect labelNameRect;
        private Rect labelInputAreaRect;

        private GUIStyle labelStyle;
        private GUIStyle labelTextStyle;

        private NodeVariable nodeVariable;

        /// <summary>
        /// 如果这个接口是枚举类型，需要知道这个枚举最长需要的GUI长度
        /// </summary>
        private float maxEnumNeedWidth = -1;

        /// <summary>
        /// 如果这个接口是<see cref="LayerMaskWrapper"/>类型，需要知道这个枚举最长需要的GUI长度
        /// </summary>
        private static float maxLayerMaskNeedWidth = -1;

        public override bool IsConnected
        {
            get { return base.IsConnected || IsDirectInput; }
        }

        /// <summary>
        /// 该input是否是直接输入值，而不是从其他节点获取值
        /// </summary>
        public bool IsDirectInput { get; private set; }

        public Type PortValueType
        {
            get
            {
                if (overridePortType != null)
                {
                    return overridePortType;
                }

                return inputPortReflectionInfo.inputValueType;
            }
        }

        /// <summary>
        /// 当不使用默认类型时，更改这个
        /// 目前主要针对读写技能变量的节点使用
        /// </summary>
        public Type overridePortType;

        public InputPortEditorView(NodeEditorView nodeView, InputPortReflectionInfo inputPortReflectionInfo) : base(nodeView)
        {
            FlowType = FlowType.In;
            this.inputPortReflectionInfo = inputPortReflectionInfo;

            labelStyle = Utility.GetGuiStyle("InputLabel");
            labelTextStyle = Utility.GetGuiStyle("InputLabelText");
        }

        /// <summary>
        /// 载入完成时，如果是<see cref="GetVariableNode"/>或者<see cref="SetVariableNode"/> 则需要初始化更新对应端口的类型
        /// </summary>
        public override void OnLoadFinish()
        {
            base.OnLoadFinish();

            //初始化读写节点的端口类型
            //如果input port端口的类型是VariableWrapper，则看是否设置的有值，有值的话更新整个端口的类型
            VariableWrapper variableWrapper = inputPortReflectionInfo.GetInputNodeVariableValue() as VariableWrapper;
            if (variableWrapper == null)
            {
                return;
            }

            int variableId = variableWrapper.variableId;
            GraphEditorData data = NodeView.graph.data;
            GraphVariableInfo graphVariableInfo = data.GetGraphVariableInfo(variableId);

            if (graphVariableInfo != null)
            {
                NodeView.UpdateSkillVariableNodeIOPortType(graphVariableInfo.valueType, true);
            }
            else
            {
                NodeView.UpdateSkillVariableNodeIOPortType(null, true);
            }
        }

        public override void Draw()
        {
            base.Draw();

            DrawNameAndType();

            //没有连接到其他节点的端口时，显示输入值的label
            if (connectedPortList.Count == 0)
            {
                //这里需要输入的是原始类型
                DrawInputFieldLabel(inputPortReflectionInfo.inputValueType);
                IsDirectInput = true;
            }
            else
            {
                IsDirectInput = false;
            }
        }

        /// <summary>
        /// 绘制Input的接口名称和类型提示
        /// </summary>
        private void DrawNameAndType()
        {
            Rect nameRect = new Rect(portViewRect);
            nameRect.width = nameRect.width - 5f;
            nameRect.x = nameRect.x + 5f;

            GUI.Label(nameRect,
                string.Format("{0}\n({1})", inputPortReflectionInfo.PortName, Utility.BeautifyTypeName(PortValueType)),
                Utility.GetGuiStyle("PortNameLeft"));
        }

        /// <summary>
        /// 绘制标签的背景和标签的名称
        /// </summary>
        /// <param name="labelName"></param>
        /// <param name="inputAreaWidth"></param>
        void CalculateAndDrawLabelCommonElement(string labelName, float inputAreaWidth)
        {
            float labelNameWidth = labelTextStyle.CalcSize(new GUIContent(labelName)).x;

            labelRect = new Rect(portViewRect);
            labelRect.width = labelNameWidth + 10f + inputAreaWidth + LabelContentPadding * 2f;
            labelRect.height = 30f;
            labelRect.x = portViewRect.x - LabelPadding - labelRect.width;
            labelRect.center = new Vector2(labelRect.center.x, portViewRect.center.y);

            labelContentRect = new Rect(labelRect);
            labelContentRect.width = labelContentRect.width - LabelContentPadding * 2f;
            labelContentRect.x = labelContentRect.x + LabelContentPadding;
            labelContentRect.y = labelContentRect.y + (labelContentRect.height - EditorGUIUtility.singleLineHeight) / 2f;
            labelContentRect.height = EditorGUIUtility.singleLineHeight;

            labelNameRect = new Rect(labelContentRect);
            labelNameRect.width = labelNameWidth;

            labelInputAreaRect = new Rect(labelNameRect);
            labelInputAreaRect.width = inputAreaWidth;
            labelInputAreaRect.x = labelContentRect.x + labelNameWidth + 10f;

            //背景
            GUI.Box(labelRect, string.Empty, labelStyle);
            DrawHelper.DrawLine(new Vector2(labelRect.xMax, labelRect.center.y), connectionCircleRect.center);
            //标签名
            GUI.Label(labelNameRect, labelName, labelTextStyle);
//            GUI.Box(labelNameRect,"",Utility.GetGuiStyle("NodeBg"));
        }

        /// <summary>
        /// 绘制各种类型的输入框
        /// </summary>
        /// <param name="type"></param>
        void DrawInputFieldLabel(Type type)
        {
            if (type == typeof(float))
            {
                string typeName = Utility.BeautifyTypeName(type) + ": ";
                CalculateAndDrawLabelCommonElement(typeName, 100f);

                EditorGUI.BeginChangeCheck();
                float value = (float) inputPortReflectionInfo.GetInputNodeVariableValue();
                value = EditorGUI.FloatField(labelInputAreaRect, value);
                if (EditorGUI.EndChangeCheck())
                {
                    inputPortReflectionInfo.SetInputNodeVariableValue(value);
                }
            }
            else if (type == typeof(int))
            {
                string typeName = Utility.BeautifyTypeName(type) + ": ";
                CalculateAndDrawLabelCommonElement(typeName, 100f);

                EditorGUI.BeginChangeCheck();
                int value = (int) inputPortReflectionInfo.GetInputNodeVariableValue();
                value = EditorGUI.IntField(labelInputAreaRect, value);
                if (EditorGUI.EndChangeCheck())
                {
                    inputPortReflectionInfo.SetInputNodeVariableValue(value);
                }
            }
            else if (type == typeof(string))
            {
                string typeName = Utility.BeautifyTypeName(type) + ": ";
                CalculateAndDrawLabelCommonElement(typeName, 100f);

                EditorGUI.BeginChangeCheck();
                string value = (string) inputPortReflectionInfo.GetInputNodeVariableValue();
                if (GUI.Button(labelInputAreaRect, value))
                {
                    PopupWindow.Show(labelInputAreaRect,
                        new StringEditPopupWindow(value,
                            stringContent => { inputPortReflectionInfo.SetInputNodeVariableValue(stringContent); }));
                }
            }
            else if (type == typeof(bool))
            {
                string typeName = Utility.BeautifyTypeName(type) + ": ";
                CalculateAndDrawLabelCommonElement(typeName, 20f);

                EditorGUI.BeginChangeCheck();
                bool value = (bool) inputPortReflectionInfo.GetInputNodeVariableValue();
                value = EditorGUI.Toggle(labelInputAreaRect, value);
                if (EditorGUI.EndChangeCheck())
                {
                    inputPortReflectionInfo.SetInputNodeVariableValue(value);
                }
            }
            else if (type.IsEnum)
            {
                if (maxEnumNeedWidth < 0)
                {
                    maxEnumNeedWidth = Utility.GetEnumMaxGuiWidth(type, 16);
                }

                string typeName = Utility.BeautifyTypeName(type) + ": ";
                CalculateAndDrawLabelCommonElement(typeName, maxEnumNeedWidth + 25); //25是enum选择框箭头的位置

                EditorGUI.BeginChangeCheck();
                Enum enumValue = (Enum) inputPortReflectionInfo.GetInputNodeVariableValue();
                enumValue = EditorGUI.EnumPopup(labelInputAreaRect, enumValue);
                if (EditorGUI.EndChangeCheck())
                {
                    inputPortReflectionInfo.SetInputNodeVariableValue(enumValue);
                }
            }
            //支持list<int>
            else if (type == typeof(List<int>))
            {
                string typeName = Utility.BeautifyTypeName(type) + ": ";
                CalculateAndDrawLabelCommonElement(typeName, 100f);

                List<int> list = inputPortReflectionInfo.GetInputNodeVariableValue() as List<int>;
                if (list == null)
                {
                    list = new List<int>();
                }

                string listHint = string.Join(",", list.Select(x => x.ToString()).ToArray());
                if (GUI.Button(labelInputAreaRect, listHint))
                {
                    ListIntEditorWindow.ShowWindow(list,
                        editedList => { inputPortReflectionInfo.SetInputNodeVariableValue(editedList); });
                }
            }
            else if (type == typeof(LayerMaskWrapper))
            {
                if (maxLayerMaskNeedWidth < 0)
                {
                    maxLayerMaskNeedWidth = Utility.GetLayerMaxGuiLength(8);
                }

                string typeName = Utility.BeautifyTypeName(type) + ": ";
                CalculateAndDrawLabelCommonElement(typeName, maxLayerMaskNeedWidth + 25);

                EditorGUI.BeginChangeCheck();
                LayerMaskWrapper layerMaskWrapperValue = (LayerMaskWrapper) inputPortReflectionInfo.GetInputNodeVariableValue();
                if (layerMaskWrapperValue == null)
                {
                    layerMaskWrapperValue = new LayerMaskWrapper();
                }

                string[] gameLayerNames = Utility.GetUnityLayerNames();

                int selectLayer = EditorGUI.MaskField(labelInputAreaRect, layerMaskWrapperValue.layer, gameLayerNames);
                if (EditorGUI.EndChangeCheck())
                {
                    layerMaskWrapperValue.layer = selectLayer;
//                    Debug.Log("select layer mask: " + layerMaskValue);
                    inputPortReflectionInfo.SetInputNodeVariableValue(layerMaskWrapperValue);
                }
            }
            //选择技能变量
            else if (type == typeof(VariableWrapper))
            {
                VariableWrapper variableWrapper = inputPortReflectionInfo.GetInputNodeVariableValue() as VariableWrapper;
                if (variableWrapper == null)
                {
                    variableWrapper = new VariableWrapper();
                }

                int varibleId = variableWrapper.variableId;
                GraphEditorData data = GraphEditorWindow.instance.data;
                GraphVariableInfo graphVariableInfo = data.GetGraphVariableInfo(varibleId);

                string buttonName;
                //当前没有选择任何一个变量
                if (graphVariableInfo == null)
                {
                    buttonName = "选择一个变量!";
                }
                else
                {
                    buttonName = graphVariableInfo.name;
                }

                CalculateAndDrawLabelCommonElement("变量: ", EditorStyles.label.CalcSize(new GUIContent(buttonName)).x + 20);
                if (GUI.Button(labelInputAreaRect, buttonName))
                {
                    PopupWindow.Show(labelInputAreaRect, new SelectGraphVariablePopupWindow(data.CurrentSkillVariableInfoList,
                        selectVariableId =>
                        {
                            variableWrapper.variableId = selectVariableId;
                            inputPortReflectionInfo.SetInputNodeVariableValue(variableWrapper);

                            Type variableType = GraphEditorWindow.instance.data
                                .GetGraphVariableInfo(selectVariableId).valueType;
                            NodeView.UpdateSkillVariableNodeIOPortType(variableType, true);
                        }));
                }
            }
            //所有需要自定义Input标签的类型写在这个分支前面
            else if (type.IsClass)
            {
                string typeName = Utility.BeautifyTypeName(type) + ": ";
                CalculateAndDrawLabelCommonElement(typeName, 30f);

                GUI.Label(labelInputAreaRect, "(Null)", labelTextStyle);
            }
            else
            {
                string typeName = Utility.BeautifyTypeName(type) + ": ";
                string content = string.Format("Default({0})", type.Name);
                CalculateAndDrawLabelCommonElement(typeName, Utility.GetStringGuiWidth(content, labelTextStyle.fontSize));

                GUI.Label(labelInputAreaRect, content, labelTextStyle);
            }
        }

        public void OnGraphVariableListChange()
        {
            if (inputPortReflectionInfo.inputValueType != typeof(VariableWrapper))
            {
                return;
            }

            VariableWrapper variableWrapper = inputPortReflectionInfo.GetInputNodeVariableValue() as VariableWrapper;
            if (variableWrapper == null)
            {
                //这里不要直接检查连线合法性，所有读写技能变量的节点类型都改变后，统一检查一次连线合法性。下同
                NodeView.UpdateSkillVariableNodeIOPortType(null, false);
                return;
            }

            GraphVariableInfo graphVariableInfo =
                GraphEditorWindow.instance.data.GetGraphVariableInfo(variableWrapper.variableId);
            if (graphVariableInfo == null)
            {
                NodeView.UpdateSkillVariableNodeIOPortType(null, false);
                return;
            }

            NodeView.UpdateSkillVariableNodeIOPortType(graphVariableInfo.valueType, false);
        }

        public bool IsInputLabelContains(Vector2 positionInWindow)
        {
            return labelRect.Contains(positionInWindow);
        }

        public override float GetNameWidth()
        {
            GUIStyle guiStyle = Utility.GetGuiStyle("PortNameLeft");
            float nameWidth = Utility.GetStringGuiWidth(inputPortReflectionInfo.PortName, guiStyle);
            float typeNameWidth = Utility.GetStringGuiWidth(Utility.BeautifyTypeName(PortValueType) + "()", guiStyle);
            return Mathf.Max(nameWidth, typeNameWidth);
        }

        public override void DisconnectAllConnections()
        {
        }

        public override string PortDescription
        {
            get { return inputPortReflectionInfo.inputPortAttribute.description; }
        }
    }
}