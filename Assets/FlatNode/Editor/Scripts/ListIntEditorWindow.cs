using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public class ListIntEditorWindow : EditorWindow
    {
        public List<int> targetList;
        public Action<List<int>> callback;
        private Vector2 scrollPosition;
        
        public static void ShowWindow(List<int> targetList,Action<List<int>> callback)
        {
            ListIntEditorWindow window = GetWindow<ListIntEditorWindow>();
            window.minSize = new Vector2(200, 300);
            window.maxSize = new Vector2(300, 400);
            window.titleContent = new GUIContent("List<int>编辑");
//            EditorWindow.GetWindow<>()

            if (targetList == null)
            {
                window.targetList = new List<int>();
            }
            else
            {
                window.targetList = new List<int>(targetList);
            }
            window.callback = callback;
        }

        private void OnGUI()
        {
            GUILayout.Label("编辑List<int>",Utility.GetGuiStyle("Title"),GUILayout.MinHeight(70f));
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();

            GUILayout.Label("增加节点");
            if (GUILayout.Button("+",GUILayout.MinWidth(30),GUILayout.MaxWidth(30)))
            {
                if (targetList == null)
                {
                    targetList = new List<int>();
                }
                
                targetList.Add(0);
                GUI.FocusControl(null);
            }
            
            GUILayout.EndHorizontal();

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            if (targetList == null)
            {
                GUILayout.FlexibleSpace();
            }
            else
            {
                for (int i = 0; i < targetList.Count; i++)
                {
                    GUILayout.BeginHorizontal();
                    
                    GUILayout.Space(20);
                    int value = targetList[i];
                    
                    EditorGUI.BeginChangeCheck();
                    value = EditorGUILayout.IntField(value, GUILayout.MinWidth(150));
                    if (EditorGUI.EndChangeCheck())
                    {
                        targetList[i] = value;
                    }
                    
                    GUILayout.Space(20);

                    if (GUILayout.Button("-",GUILayout.MinWidth(20)))
                    {
                        targetList.RemoveAt(i);
                        GUILayout.EndHorizontal();
                        GUI.FocusControl(null);
                        break;
                    }
                    
                    GUILayout.EndHorizontal();
                }
            }
            GUILayout.EndScrollView();

            GUILayout.FlexibleSpace();
            if (GUILayout.Button("确认保存",GUILayout.MinHeight(50f)))
            {
                GUI.FocusControl(null);

                if (callback != null)
                {
                    callback(targetList);
                    ListIntEditorWindow window = GetWindow<ListIntEditorWindow>();
                    if (window != null) 
                    {
                        window.Close();
                    }
                }
            }
        }
    }
}