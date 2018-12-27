using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using FlatNode.Runtime;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    /// <summary>
    /// 生成节点的一些代码，避免反射
    /// </summary>
    public class GraphCodeGenerator
    {
        [MenuItem("FlatNode/生成节点静态代码")]
        public static void GenerateSkillNodeCode()
        {
            List<Type> nodeTypeList = Utility.GetNodeTypeList();
            GraphCodeGenerator graphCodeGenerator = new GraphCodeGenerator(nodeTypeList);

            graphCodeGenerator.Generate();

            EditorUtility.DisplayDialog("完成", "静态代码生成完成。\n " + graphCodeGenerator.outputFilePath, "确定");
            AssetDatabase.Refresh();
        }

        private List<Type> nodeTypeList;

        /// <summary>
        /// 输出文件路径
        /// </summary>
        private readonly string outputFilePath =
            Application.dataPath + "/FlatNode/Runtime/GenerateCode/NodeGenerateCode.cs";

        private StringBuilder sb;
        private int indent;

        public GraphCodeGenerator(List<Type> nodeTypeList)
        {
            this.nodeTypeList = nodeTypeList;
            sb = new StringBuilder(1024);
        }

        private void Generate()
        {
            WriteNameSpace();
            GenerateNodeFactoryDictionaryCode();
            WriteLine("");
            GenerateNodeInputParseCode();
            WriteLine("");
            GenerateOutputFuncInitCode();
            WriteTail();
            FlushToFile();
        }

        private void WriteNameSpace()
        {
            WriteLine("//自动生成代码，不要修改");
            WriteLine("using System;");
            WriteLine("using System.Collections.Generic;");
            WriteLine("using FlatNode.Runtime.Flat;");
            WriteLine("");
            WriteLine("namespace FlatNode.Runtime");
            WriteLine("{");
            AddIndent(4);

            WriteLine("public static partial class FlatNodeUtility");
            WriteLine("{");

            AddIndent(4);
        }

        private void WriteTail()
        {
            AddIndent(-4);
            WriteLine("}");
            AddIndent(-4);
            WriteLine("}");
        }

        /// <summary>
        /// 生成一个可以通过类型名称直接返回节点实例的字典
        /// </summary>
        private void GenerateNodeFactoryDictionaryCode()
        {
            WriteLine("private static Dictionary<string, Func<NodeBase>> nodeFactoryDictionary;");

            WriteLine("");

            WriteLine("private static void InitNodeFactoryDictionary()");
            WriteLine("{");
            AddIndent(4);

            WriteLine("if (nodeFactoryDictionary != null){return;}");
            WriteLine("");

            WriteLine("nodeFactoryDictionary = new Dictionary<string, Func<NodeBase>>");
            WriteLine("{");
            AddIndent(4);

            for (int i = 0; i < nodeTypeList.Count; i++)
            {
                Type nodeType = nodeTypeList[i];
                WriteLine(string.Format("{{\"{0}\",()=> new {1}()}},", nodeType.FullName, nodeType.Name));
            }

            AddIndent(-4);
            WriteLine("};");

            AddIndent(-4);
            WriteLine("}");
        }

        /// <summary>
        /// 生成parse input端口信息的代码
        /// </summary>
        private void GenerateNodeInputParseCode()
        {
            WriteLine("private static Dictionary<Type, Action<NodeBase, NodeInputFieldInfo>> inputPortParseFuncDictionary;");
            WriteLine("");

            WriteLine("private static void InitInputPortParseFuncsDictionary()");
            WriteLine("{");
            AddIndent(4);

            WriteLine("if (inputPortParseFuncDictionary != null){return;}");
            WriteLine("");

            WriteLine("inputPortParseFuncDictionary = new Dictionary<Type, Action<NodeBase, NodeInputFieldInfo>>");
            WriteLine("{");
            AddIndent(4);

            for (int i = 0; i < nodeTypeList.Count; i++)
            {
                Type nodeType = nodeTypeList[i];
                NodeInputPortFieldInfo[] nodeInputPortFieldInfos = GetNodeInputPortFieldInfo(nodeType);
                string nodeTypeName = nodeType.Name;
                string camelClassName = FirstCharacterToLower(nodeTypeName);
                if (nodeInputPortFieldInfos == null || nodeInputPortFieldInfos.Length == 0)
                {
                    continue;
                }

                WriteLine("{");
                AddIndent(4);

                WriteLine(string.Format("typeof({0}),(nodeInstance, inputFieldInfo) =>", nodeTypeName));

                WriteLine("{");
                AddIndent(4);

                WriteLine(string.Format("{0} {1} = nodeInstance as {0};", nodeTypeName, camelClassName));
                WriteLine("string inputPortFieldName = inputFieldInfo.FieldName;");
                WriteLine("int targetNodeId = inputFieldInfo.TargetNodeId;");
                WriteLine("int targetPortId = inputFieldInfo.TargetPortId;");
                WriteLine("string valueString = inputFieldInfo.ValueString;");

                WriteLine("");

                for (int j = 0; j < nodeInputPortFieldInfos.Length; j++)
                {
                    string fieldName = nodeInputPortFieldInfos[j].fieldName;
                    Type valueType = nodeInputPortFieldInfos[j].valueType;
                    string beautifyValueTypeName = Utility.BeautifyTypeName(valueType, true);

                    if (j == 0)
                    {
                        WriteLine(string.Format("if (inputPortFieldName == \"{0}\")", fieldName));
                    }
                    else
                    {
                        WriteLine(string.Format("else if (inputPortFieldName == \"{0}\")", fieldName));
                    }

                    WriteLine("{");
                    AddIndent(4);

                    WriteLine("if (targetNodeId >= 0 && targetPortId >= 0)");
                    WriteLine("{");
                    AddIndent(4);

                    WriteLine(string.Format("{0}.{1} = new NodeInputVariable<{2}>();", camelClassName, fieldName,
                        beautifyValueTypeName));
                    WriteLine(string.Format("{0}.{1}.targetNodeId = targetNodeId;", camelClassName, fieldName));
                    WriteLine(string.Format("{0}.{1}.targetPortId = targetPortId;", camelClassName, fieldName));

                    AddIndent(-4);
                    WriteLine("}");

                    if (valueType == typeof(float) || valueType == typeof(int) || valueType == typeof(string) ||
                        valueType == typeof(bool) || valueType.IsEnum || valueType == typeof(List<int>) ||
                        valueType == typeof(LayerMaskWrapper) || valueType == typeof(VariableWrapper))
                    {
                        WriteLine("else");
                        WriteLine("{");
                        AddIndent(4);
                        
                        WriteLine(string.Format("{0}.{1} = new NodeInputVariable<{2}>();", camelClassName, fieldName,
                            beautifyValueTypeName));
                        
                        if (valueType == typeof(float))
                        {
                            WriteLine(string.Format("{0}.{1}.value = float.Parse(valueString);", camelClassName, fieldName));
                        }
                        else if (valueType == typeof(int))
                        {
                            WriteLine(string.Format("{0}.{1}.value = int.Parse(valueString);", camelClassName, fieldName));
                        }
                        else if (valueType == typeof(string))
                        {
                            WriteLine(string.Format("{0}.{1}.value = valueString;", camelClassName, fieldName));
                        }
                        else if (valueType == typeof(bool))
                        {
                            WriteLine(string.Format("{0}.{1}.value = Boolean.Parse(valueString);", camelClassName, fieldName));
                        }
                        else if (valueType.IsEnum)
                        {
                            WriteLine(string.Format("{0}.{1}.value = ({2})Enum.Parse(typeof({2}), valueString);", camelClassName,
                                fieldName, beautifyValueTypeName));
                        }
                        else if (valueType == typeof(List<int>))
                        {
                            WriteLine(string.Format("{0}.{1}.value = ParseIntList(valueString);", camelClassName, fieldName));
                        }
                        else if (valueType == typeof(LayerMaskWrapper))
                        {
                            WriteLine("LayerMaskWrapper layerMaskWrapper = valueString;");
                            WriteLine(string.Format("{0}.{1}.value = layerMaskWrapper;", camelClassName, fieldName));
                        }
                        else if (valueType == typeof(VariableWrapper))
                        {
                            WriteLine("int variableId = int.Parse(valueString);");
                            WriteLine("VariableWrapper variableWrapper = variableId;");
                            WriteLine(string.Format("{0}.{1}.value = variableWrapper;", camelClassName, fieldName));
                        }

                        AddIndent(-4);
                        WriteLine("}");
                    }

                    AddIndent(-4);
                    WriteLine("}");
                }


                AddIndent(-4);
                WriteLine("}");

                AddIndent(-4);
                WriteLine("},");
            }

            AddIndent(-4);
            WriteLine("};");

            AddIndent(-4);
            WriteLine("}");
        }

        /// <summary>
        /// 生出初始化输出函数数组的代码
        /// </summary>
        private void GenerateOutputFuncInitCode()
        {
            WriteLine("private static Dictionary<Type, Action<NodeBase>> nodeOutputInitFuncDictionary;");
            WriteLine("private static void InitOutputFuncsDictionary()");
            WriteLine("{");
            AddIndent(4);

            WriteLine("if (nodeOutputInitFuncDictionary != null)");
            WriteLine("{");
            AddIndent(4);
            WriteLine("return;");
            AddIndent(-4);
            WriteLine("}");

            WriteLine("");
            WriteLine("nodeOutputInitFuncDictionary = new Dictionary<Type, Action<NodeBase>>");
            WriteLine("{");
            AddIndent(4);


            for (int i = 0; i < nodeTypeList.Count; i++)
            {
                Type nodeType = nodeTypeList[i];
                NodeOutputMethodInfo[] outputMethodInfos = GetSortedOutputMethod(nodeType);
                if (outputMethodInfos == null || outputMethodInfos.Length == 0)
                {
                    continue;
                }

                WriteLine("{");
                AddIndent(4);
                string typeName = nodeType.Name;
                WriteLine(string.Format("typeof({0}),nodeInstance =>", nodeType.Name));
                WriteLine("{");

                AddIndent(4);

                string camelClassName = FirstCharacterToLower(typeName);
                WriteLine(string.Format("{0} {1} = nodeInstance as {0};", nodeType.Name, camelClassName));
                WriteLine(string.Format("{0}.outputPortFuncs = new Func<NodeVariable>[]", camelClassName));
                WriteLine("{");
                AddIndent(4);

                //把所有的方法名写进去
                for (int j = 0; j < outputMethodInfos.Length; j++)
                {
                    WriteLine(string.Format("{0}.{1},", camelClassName, outputMethodInfos[j].methodInfo.Name));
                }

                AddIndent(-4);
                WriteLine("};");

                AddIndent(-4);

                WriteLine("}");
                AddIndent(-4);
                WriteLine("},");
            }

            AddIndent(-4);
            WriteLine("};");
            AddIndent(-4);
            WriteLine("}");
        }

        private void FlushToFile()
        {
            if (sb.Length == 0)
            {
                return;
            }

            string fileDirectoryPath = Directory.GetParent(outputFilePath).FullName;
            Debug.Log(fileDirectoryPath);
            if (!Directory.Exists(fileDirectoryPath))
            {
                Directory.CreateDirectory(fileDirectoryPath);
            }

            File.WriteAllText(outputFilePath, sb.ToString());
        }

        private void WriteLine(string line)
        {
            for (int i = 0; i < indent; i++)
            {
                sb.Append(" ");
            }

            sb.Append(line);
            sb.AppendLine();
        }

        private void AddIndent(int value)
        {
            indent += value;
            indent = Mathf.Max(0, indent);
        }

        private static string FirstCharacterToLower(string str)
        {
            if (string.IsNullOrEmpty(str) || char.IsLower(str, 0))
                return str;

            return char.ToLowerInvariant(str[0]) + str.Substring(1);
        }

        private static NodeOutputMethodInfo[] GetSortedOutputMethod(Type nodeType)
        {
            if (!nodeType.IsSubclassOf(typeof(NodeBase)))
            {
                return null;
            }

            List<NodeOutputMethodInfo> nodeOutputMethodList = new List<NodeOutputMethodInfo>();

            MethodInfo[] methodInfos = nodeType.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < methodInfos.Length; i++)
            {
                MethodInfo methodInfo = methodInfos[i];

                object[] attributeObjects = methodInfo.GetCustomAttributes(typeof(NodeOutputPortAttribute), false);
                if (attributeObjects.Length > 0)
                {
                    nodeOutputMethodList.Add(new NodeOutputMethodInfo(methodInfo,
                        attributeObjects[0] as NodeOutputPortAttribute));
                }
            }

            nodeOutputMethodList.Sort((a, b) => a.attribute.portId - b.attribute.portId);
            return nodeOutputMethodList.ToArray();
        }

        private struct NodeOutputMethodInfo
        {
            public MethodInfo methodInfo;
            public NodeOutputPortAttribute attribute;

            public NodeOutputMethodInfo(MethodInfo methodInfo, NodeOutputPortAttribute attribute)
            {
                this.methodInfo = methodInfo;
                this.attribute = attribute;
            }
        }

        private static NodeInputPortFieldInfo[] GetNodeInputPortFieldInfo(Type nodeType)
        {
            List<NodeInputPortFieldInfo> resultList = new List<NodeInputPortFieldInfo>();

            FieldInfo[] fieldInfos = nodeType.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];

                object[] attributeObjects = fieldInfo.GetCustomAttributes(typeof(NodeInputPortAttribute), false);

                if (attributeObjects.Length == 0)
                {
                    continue;
                }

                Type valueType = fieldInfo.FieldType.GetGenericArguments()[0];

                resultList.Add(new NodeInputPortFieldInfo(fieldInfo.Name, valueType));
            }

            if (resultList.Count > 0)
            {
                return resultList.ToArray();
            }

            return null;
        }

        private struct NodeInputPortFieldInfo
        {
            public string fieldName;
            public Type valueType;

            public NodeInputPortFieldInfo(string fieldName, Type valueType)
            {
                this.fieldName = fieldName;
                this.valueType = valueType;
            }
        }
    }
}