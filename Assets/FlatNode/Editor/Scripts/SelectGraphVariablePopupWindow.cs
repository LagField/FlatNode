using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public class SelectGraphVariablePopupWindow : PopupWindowContent
    {
        private List<GraphVariableInfo> localVariableInfoList;
        private Action<int> selectCallback;

        private Vector2 scrollPosition;

        public SelectGraphVariablePopupWindow(List<GraphVariableInfo> localVariableInfoList, Action<int> selectCallback)
        {
            this.localVariableInfoList = localVariableInfoList;
            this.selectCallback = selectCallback;
        }

        public override void OnGUI(Rect rect)
        {
            if (localVariableInfoList == null || localVariableInfoList.Count == 0)
            {
                GUILayout.Label("还没有定义变量，在 编辑器/技能/技能变量编辑器 中添加变量", Utility.GetGuiStyle("Title"));
                return;
            }

            //draw title 
            GUILayout.Label("选择一个变量", Utility.GetGuiStyle("Title"));
            GUILayout.Space(10);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < localVariableInfoList.Count; i++)
            {
                GraphVariableInfo graphVariableInfo = localVariableInfoList[i];

                if (DrawVariableItemButton(graphVariableInfo))
                {
                    if (selectCallback != null)
                    {
                        selectCallback(graphVariableInfo.id);
                    }
                    
                    editorWindow.Close();
                    break;
                }

                GUILayout.Space(10);
            }

            GUILayout.EndScrollView();
        }

        private bool DrawVariableItemButton(GraphVariableInfo graphVariableInfo)
        {
            GUILayout.BeginVertical(Utility.GetGuiStyle("LocalVariableItemBg"));

            GUILayout.BeginHorizontal();

            GUIStyle variableNameStyle = EditorStyles.label;
            variableNameStyle.alignment = TextAnchor.MiddleLeft;
            GUILayout.Label("变量: " + graphVariableInfo.name, variableNameStyle, GUILayout.MinHeight(30));
            if (GUILayout.Button("选择", GUILayout.MinHeight(30)))
            {
                return true;
            }

            GUILayout.EndHorizontal();

            GUILayout.EndVertical();
            return false;
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(300, 400);
        }
    }
}