﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using FlatNode.Runtime;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public static class Utility
    {
        private const string GuiSkinPath = "Assets/FlatNode/Editor/Skin/SkillEditorSkin.guiskin";
        private static GUISkin LoadedGuiSkin;

        private const string CommentBoxTexturePath = "Assets/FlatNode/Editor/Skin/commentbox.png";
        private static Texture2D commentBoxTexture;

        private static void LoadSkinConfig()
        {
            if (LoadedGuiSkin == null)
            {
                LoadedGuiSkin = AssetDatabase.LoadAssetAtPath<GUISkin>(GuiSkinPath);
            }
        }

        public static GUIStyle GetGuiStyle(string styleName)
        {
            LoadSkinConfig();

            if (LoadedGuiSkin == null)
            {
                Debug.LogError("无法载入guistyle: " + GuiSkinPath);
                return GUIStyle.none;
            }

            return LoadedGuiSkin.GetStyle(styleName);
        }

        public static Texture2D GetCommentBoxTexture()
        {
            if (commentBoxTexture == null)
            {
                commentBoxTexture = AssetDatabase.LoadAssetAtPath<Texture2D>(CommentBoxTexturePath);
            }

            return commentBoxTexture;
        }

        public static GUIStyle SearchBoxStyle = "ToolbarSeachTextField";
        public static GUIStyle CancelButtonStyle = "ToolbarSeachCancelButton";
        public static GUIStyle DisabledCancelButtonStyle = "ToolbarSeachCancelButtonEmpty";
        public static GUIStyle SelectionStyle = "SelectionRect";

        private static GUIStyle navTitleStyle;

        public static GUIStyle NavTitleStyle
        {
            get
            {
                if (navTitleStyle == null)
                {
                    navTitleStyle = new GUIStyle(EditorStyles.whiteBoldLabel);
                    navTitleStyle.alignment = TextAnchor.MiddleCenter;
                    navTitleStyle.normal.textColor = Color.white;
                }

                return navTitleStyle;
            }
        }

        private static Texture2D lineTexture;

        public static Texture2D GetBezierLineTexture()
        {
            if (lineTexture == null)
            {
                lineTexture = AssetDatabase.LoadAssetAtPath<Texture2D>("Assets/FlatNode/Editor/Skin/Line");
            }

            return lineTexture;
        }

        public static float GetStringGuiWidth(string targetString, float singleChineseWidth = 10)
        {
            float result = 0;
            for (int i = 0; i < targetString.Length; i++)
            {
                char c = targetString[i];
                if (c >= 0x4e00 && c <= 0x9fbb)
                {
                    result += singleChineseWidth;
                }
                else
                {
                    result += singleChineseWidth * .5f;
                }
            }

            return result;
        }

        /// <summary>
        /// 这个版本更准确些
        /// </summary>
        /// <param name="targetString"></param>
        /// <param name="guiStyle"></param>
        /// <returns></returns>
        public static float GetStringGuiWidth(string targetString, GUIStyle guiStyle)
        {
            return guiStyle.CalcSize(new GUIContent(targetString)).x;
        }

        public static float GetEnumMaxGuiWidth(Type enumType, float singleChineseWidth = 10)
        {
            string[] enumStrings = Enum.GetNames(enumType);

            int maxLength = enumStrings.Max(x => x.Length);
            string maxLengthString = enumStrings.First(x => x.Length == maxLength);

            return GetStringGuiWidth(maxLengthString, singleChineseWidth);
        }

        public static string BeautifyTypeName(Type type, bool defaultFullName = false)
        {
            if (type == typeof(int))
            {
                return "int";
            }

            if (type == typeof(float))
            {
                return "float";
            }

            if (type == typeof(bool))
            {
                return "bool";
            }

            if (type == typeof(string))
            {
                return "string";
            }

            if (type.IsEnum && defaultFullName)
            {
                string enumTypeName = type.FullName;
                enumTypeName = enumTypeName.Replace('+', '.');
                return enumTypeName;
            }

            if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(List<>))
            {
                Type genericType = type.GetGenericArguments()[0];
                return string.Format("List<{0}>", BeautifyTypeName(genericType, defaultFullName));
            }

            return defaultFullName ? type.FullName : type.Name;
        }

        #region Reflection


        /// <summary>
        /// 获取工程中节点反射信息
        /// </summary>
        /// <returns></returns>
        public static List<Type> GetNodeTypeList()
        {
            List<Type> resultList = new List<Type>();
            Assembly[] assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int assemblyIndex = 0; assemblyIndex < assemblies.Length; assemblyIndex++)
            {
                String assemblyName = assemblies[assemblyIndex].GetName().Name;
                Assembly assembly = assemblies[assemblyIndex];

                if (!assemblyName.Equals("Assembly-CSharp"))
                {
                    continue;
                }

                Type[] types = assembly.GetTypes();
                for (int typeIndex = 0; typeIndex < types.Length; typeIndex++)
                {
                    Type targetType = types[typeIndex];
                    object[] attributes = targetType.GetCustomAttributes(typeof(GraphNodeAttribute), false);
                    if (attributes.Length > 0)
                    {
                        resultList.Add(targetType);
                    }
                }

                break;
            }

            return resultList;
        }

        #endregion

        private static EditorLayerInfo[] editorLayerInfos;

        #region LayerMaskHelper

        /// <summary>
        /// 缓存当前工程layer名字
        /// </summary>
        private static string[] unityLayerNames;
        
        /// <summary>
        /// 获取Unity Layer名称
        /// </summary>
        /// <returns></returns>
        public static string[] GetUnityLayerNames()
        {
            if (unityLayerNames == null)
            {
                List<string> layerNameList = new List<string>();
                for (int i = 0; i < 32; i++)
                {
                    string layerName = LayerMask.LayerToName(i);
                    if (string.IsNullOrEmpty(layerName))
                    {
                        continue;
                    }
                
                    layerNameList.Add(layerName);
                }

                unityLayerNames = layerNameList.ToArray();
            }
            return unityLayerNames;
//            
//            if (gameLayerNames == null)
//            {
//                Type gameLayerType = typeof(GameLayer);
//                FieldInfo[] fieldInfos = gameLayerType.GetFields(BindingFlags.Static | BindingFlags.Public);
//
//                gameLayerNames = new string[fieldInfos.Length];
//                for (int i = 0; i < fieldInfos.Length; i++)
//                {
//                    FieldInfo fieldInfo = fieldInfos[i];
//
//                    string fieldName = fieldInfo.Name;
//                    gameLayerNames[i] = fieldName;
//                }
//            }
//
//            return gameLayerNames;
        }

        public static int GetLayerMaxGuiLength(int singleCharWidth = 5)
        {
            string[] layerNames = GetUnityLayerNames();

            int maxLength = 0;
            for (int i = 0; i < layerNames.Length; i++)
            {
                string layerName = layerNames[i];
                int length = layerName.Length;

                if (length > maxLength)
                {
                    maxLength = length;
                }
            }

            return maxLength * singleCharWidth;
        }

        #endregion
    }

    public enum ControlType
    {
        None,
        DraggingNode,
        DraggingGraph,
        DraggingConnection,
        DraggingMultiSelection,
        DraggingMultiNodes,
        DraggingNewCommentBox,
        DraggingCommentBox,
        EditingComment,
        ResizingCommentBox,
    }

    public class TestClass
    {
        private List<GameObject> testField;
    }

    public class EditorLayerInfo
    {
        public string layerName;
        public int layerValue;

        public override string ToString()
        {
            return string.Format("Name: {0}, Value: {1}", layerName, layerValue);
        }
    }
}