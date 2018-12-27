using System;
using System.Collections;
using System.Collections.Generic;
using FlatNode.Runtime;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public class DisplayAllNodeDescriptionWindow : EditorWindow
    {
        public class NodeInformation
        {
            public Type type;
            public SkillNodeAttribute attribute;

            public string nodeScriptName;
            public string nodeName;
            public string nodeDescription;

            public NodeInformation(Type type, SkillNodeAttribute attribute)
            {
                this.type = type;
                this.attribute = attribute;

                nodeScriptName = type.Name;
                nodeName = attribute.nodeName;
                nodeDescription = attribute.nodeDescription;
            }

            public bool Contain(string searchString)
            {
                return nodeScriptName.ToLower().Contains(searchString) || nodeName.ToLower().Contains(searchString) ||
                       nodeDescription.ToLower().Contains(searchString);
            }
        }

        private List<NodeInformation> nodeInfoList;
        private Vector2 scrollPosition;
        private string searchString;

        [MenuItem("FlatNode/显示所有节点说明")]
        public static void ShowWindow()
        {
            DisplayAllNodeDescriptionWindow nodeDescriptionWindow = GetWindow<DisplayAllNodeDescriptionWindow>();
            nodeDescriptionWindow.minSize = new Vector2(400, 500);
            nodeDescriptionWindow.titleContent = new GUIContent("节点说明");
        }

        private void OnEnable()
        {
            //拿到所有节点信息
            nodeInfoList = new List<NodeInformation>();
            List<Type> nodeTypeList = Utility.GetNodeTypeList();
            for (int i = 0; i < nodeTypeList.Count; i++)
            {
                Type type = nodeTypeList[i];

                object[] attributeObjects = type.GetCustomAttributes(typeof(SkillNodeAttribute), false);
                if (attributeObjects.Length == 0)
                {
                    continue;
                }

                SkillNodeAttribute attribute = attributeObjects[0] as SkillNodeAttribute;
                nodeInfoList.Add(new NodeInformation(type, attribute));
            }
            
            //排序
            nodeInfoList.Sort((a, b) => a.nodeScriptName.CompareTo(b.nodeScriptName));

            searchString = string.Empty;
        }

        private void OnGUI()
        {
            GUILayout.Label("所有节点说明", Utility.GetGuiStyle("Title"));
            GUILayout.Space(10);

            GUILayout.BeginHorizontal();
            GUILayout.Label("搜索: ", GUILayout.MaxWidth(30));
            searchString = GUILayout.TextField(searchString).ToLower();
            GUILayout.EndHorizontal();
            GUILayout.Space(10);

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);

            for (int i = 0; i < nodeInfoList.Count; i++)
            {
                NodeInformation nodeInformation = nodeInfoList[i];

                if (!string.IsNullOrEmpty(searchString))
                {
                    if (nodeInformation.Contain(searchString))
                    {
                        DrawNodeAttribute(nodeInformation);
                        GUILayout.Space(10);
                    }
                }
                else
                {
                    DrawNodeAttribute(nodeInformation);
                    GUILayout.Space(10);
                }
            }

            GUILayout.EndScrollView();
        }

        private void DrawNodeAttribute(NodeInformation nodeInformation)
        {
            GUILayout.BeginHorizontal();

            GUILayout.Label("脚本名: ", GUILayout.MaxWidth(40));
            GUILayout.Label(nodeInformation.nodeScriptName, GUILayout.MaxWidth(180));

            GUILayout.Space(10);

            GUILayout.Label("节点名: ", GUILayout.MaxWidth(40));
            GUILayout.Label(nodeInformation.nodeName, GUILayout.MaxWidth(180));

            GUILayout.Space(10);

            GUILayout.Label("节点说明: ", GUILayout.MaxWidth(50));
            GUILayout.Label(nodeInformation.nodeDescription);

            GUILayout.EndHorizontal();
        }
    }
}