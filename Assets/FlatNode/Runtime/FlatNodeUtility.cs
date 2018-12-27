using System;
using System.Collections.Generic;
using System.Reflection;
using FlatBuffers;
using FlatNode.Runtime.Flat;

namespace FlatNode.Runtime
{
    public static partial class FlatNodeUtility
    {
        private static FieldInfo inputVariableFieldInfo;

        /// <summary>
        /// 初始化技能节点的初始化方法
        /// 基本都是自动生成的代码
        /// </summary>
        public static void Init()
        {
            InitNodeFactoryDictionary();
            InitInputPortParseFuncsDictionary();
            InitOutputFuncsDictionary();
        }
        
        public static NodeBase DeserializeNode(ByteBuffer bb, int nodeId)
        {
            NodeBase resultNode = null;
            GraphInfo graphInfo = GraphInfo.GetRootAsGraphInfo(bb);

            int nodeCount = graphInfo.NodesLength;
            for (int i = 0; i < nodeCount; i++)
            {
                NodeInfo? nodeInfo = graphInfo.Nodes(i);
                if (!nodeInfo.HasValue)
                {
                    continue;
                }

                if (nodeInfo.Value.NodeId != nodeId)
                {
                    continue;
                }

                string nodeClassName = nodeInfo.Value.NodeClassTypeName;
                Func<NodeBase> nodeFactory;
                if (nodeFactoryDictionary.TryGetValue(nodeClassName,out nodeFactory))
                {
                    resultNode = nodeFactory();
                }
                
                if (resultNode == null)
                {
                    UnityEngine.Debug.LogErrorFormat("无法实例化节点 {0},是否添加了新节点，但是没有生成静态代码?", nodeClassName);
                    return null;
                }
                
                Type nodeType = resultNode.GetType();

                resultNode.nodeId = nodeInfo.Value.NodeId;
                resultNode.isCreateSequenceNode = nodeType == typeof(CreateSequenceNode);

                //处理input
                //使用静态生成代码来处理
                int inputFieldCount = nodeInfo.Value.InputPortInfosLength;
                for (int j = 0; j < inputFieldCount; j++)
                {
                    NodeInputFieldInfo nodeInputFieldInfo = nodeInfo.Value.InputPortInfos(j).Value;
//                    WriteNodeInputVariableField(resultNode, nodeType, nodeInfo.Value.InputPortInfos(j).Value);
                    Action<NodeBase, NodeInputFieldInfo> parseFunction;
                    if(inputPortParseFuncDictionary.TryGetValue(nodeType,out parseFunction))
                    {
                        parseFunction(resultNode, nodeInputFieldInfo);
                    }
                    else
                    {
                        UnityEngine.Debug.LogErrorFormat("无法处理节点 {0} 的input port,是否添加了新节点，但是没有生成静态代码?", nodeClassName);
                    }
                }

                //处理flow out
                WriteFlowOutField(resultNode,nodeType,nodeInfo.Value);
                
                //处理outputPortFuncs
                //使用静态生成代码来处理
                Action<NodeBase> outputFuncsInitAction;
                if (nodeOutputInitFuncDictionary.TryGetValue(nodeType,out outputFuncsInitAction))
                {
                    outputFuncsInitAction(resultNode);
                }
                
                //还原EntranceNode或者CreateSequenceNode的右侧节点id
                EntranceNodeBase entranceNodeBase = resultNode as EntranceNodeBase;
                if (entranceNodeBase != null)
                {
                    int[] rightSideNodeIds = new int[nodeInfo.Value.RightSideNodeIdsLength];
                    for (int j = 0; j < rightSideNodeIds.Length; j++)
                    {
                        rightSideNodeIds[j] = nodeInfo.Value.RightSideNodeIds(j);
                    }
                
                    entranceNodeBase.rightSideNodeIds = rightSideNodeIds;
                }
                else
                {
                    CreateSequenceNode createSequenceNode = resultNode as CreateSequenceNode;
                    if (createSequenceNode != null)
                    {
                        int[] rightSideNodeIds = new int[nodeInfo.Value.RightSideNodeIdsLength];
                        for (int j = 0; j < rightSideNodeIds.Length; j++)
                        {
                            rightSideNodeIds[j] = nodeInfo.Value.RightSideNodeIds(j);
                        }

                        createSequenceNode.rightSideNodeIds = rightSideNodeIds;
                    }
                }
            }

            return resultNode;
        }

        /// <summary>
        /// 反射设置input 端口的参数
        /// 暂时不用
        /// </summary>
        /// <param name="nodeInstance"></param>
        /// <param name="nodeType"></param>
        /// <param name="inputFieldInfo"></param>
        public static void WriteNodeInputVariableField(NodeBase nodeInstance, Type nodeType, NodeInputFieldInfo inputFieldInfo)
        {
            string fieldName = inputFieldInfo.FieldName;
            inputVariableFieldInfo =
                nodeType.GetField(fieldName, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (inputVariableFieldInfo == null)
            {
                UnityEngine.Debug.LogErrorFormat("类 {0} 中没有找到 NodeInputVariable: {1}", nodeType.Namespace, fieldName);
                return;
            }

            string valueTypeName = inputFieldInfo.ValueTypeName;
            Type valueType = Type.GetType(valueTypeName);
            if (valueType == null)
            {
                valueType = Type.GetType(valueTypeName + ",UnityEngine");
                if (valueType == null)
                {
                    valueType = Type.GetType(valueTypeName + ",Assembly-CSharp");
                }
            }

            object nodeInputVariableInstance = Activator.CreateInstance(typeof(NodeInputVariable<>).MakeGenericType(valueType));
            inputVariableFieldInfo.SetValue(nodeInstance,nodeInputVariableInstance);

            if (inputFieldInfo.TargetNodeId >= 0 && inputFieldInfo.TargetPortId >= 0)
            {
                FieldInfo targetNodeIdFieldInfo =
                    nodeInputVariableInstance.GetType().GetField("targetNodeId", BindingFlags.Public | BindingFlags.Instance);
                targetNodeIdFieldInfo.SetValue(nodeInputVariableInstance,inputFieldInfo.TargetNodeId);
                
                FieldInfo targetPortIdFieldInfo =
                    nodeInputVariableInstance.GetType().GetField("targetPortId", BindingFlags.Public | BindingFlags.Instance);
                targetPortIdFieldInfo.SetValue(nodeInputVariableInstance,inputFieldInfo.TargetPortId);
            }
            else
            {
                FieldInfo targetNodeIdFieldInfo =
                    nodeInputVariableInstance.GetType().GetField("targetNodeId", BindingFlags.Public | BindingFlags.Instance);
                targetNodeIdFieldInfo.SetValue(nodeInputVariableInstance,-1);
                
                FieldInfo targetPortIdFieldInfo =
                    nodeInputVariableInstance.GetType().GetField("targetPortId", BindingFlags.Public | BindingFlags.Instance);
                targetPortIdFieldInfo.SetValue(nodeInputVariableInstance,-1);

                string valueString = inputFieldInfo.ValueString;

                //值类型
                if (valueType.IsValueType || valueType == typeof(string))
                {
                    FieldInfo valueFieldInfo =
                        nodeInputVariableInstance.GetType().GetField("value", BindingFlags.Public | BindingFlags.Instance);
                    
                    if (valueType == typeof(int))
                    {
                        int value = int.Parse(valueString);
                        valueFieldInfo.SetValue(nodeInputVariableInstance,value);
                    }
                    else if (valueType == typeof(float))
                    {
                        float value = float.Parse(valueString);
                        valueFieldInfo.SetValue(nodeInputVariableInstance,value);
                    }
                    else if (valueType == typeof(string))
                    {
                        valueFieldInfo.SetValue(nodeInputVariableInstance,valueString);
                    }
                    else if (valueType == typeof(bool))
                    {
                        bool value = Boolean.Parse(valueString);
                        valueFieldInfo.SetValue(nodeInputVariableInstance,value);
                    }
                    else if (valueType.IsEnum)
                    {
                        object value = Enum.Parse(valueType, valueString);
                        valueFieldInfo.SetValue(nodeInputVariableInstance,value);
                    }
                }
                //引用类型
                else
                {
                    FieldInfo valueFieldInfo =
                        nodeInputVariableInstance.GetType().GetField("value", BindingFlags.Public | BindingFlags.Instance);
                    if (valueType == typeof(List<int>))
                    {
                        List<int> list = ParseIntList(valueString);
                        valueFieldInfo.SetValue(nodeInputVariableInstance,list);
                    }
                    else if (valueType == typeof(LayerMaskWrapper))
                    {
                        LayerMaskWrapper layerMaskWrapper = valueString;
                        valueFieldInfo.SetValue(nodeInputVariableInstance,layerMaskWrapper);
                    }
                    else if (valueType == typeof(VariableWrapper))
                    {
                        int variableId;
                        if (int.TryParse(valueString,out variableId))
                        {
                            VariableWrapper variableWrapper = variableId;
                            valueFieldInfo.SetValue(nodeInputVariableInstance,variableWrapper);
                        }
                    }
                }
            }
        }

        public static void WriteFlowOutField(NodeBase nodeInstance, Type nodeType, NodeInfo nodeInfo)
        {
            int flowOutCount = nodeInfo.FlowOutPortInfosLength;
            int[][] flowOutTargetNodeId = new int[flowOutCount][];

            for (int i = 0; i < flowOutCount; i++)
            {
                NodeFlowOutPortInfo flowOutPortInfo = nodeInfo.FlowOutPortInfos(i).Value;
                int flowOutPortConnectedNodeCount = flowOutPortInfo.TargetNodeIdsLength;
                flowOutTargetNodeId[i] = new int[flowOutPortConnectedNodeCount];
                
                for (int j = 0; j < flowOutPortConnectedNodeCount; j++)
                {
                    int connectedNodeId = flowOutPortInfo.TargetNodeIds(j);
                    flowOutTargetNodeId[i][j] = connectedNodeId;
                }
            }

            nodeInstance.flowOutTargetNodeId = flowOutTargetNodeId;
        }

        /// <summary>
        /// 反序列化图的变量，并返回结果
        /// </summary>
        /// <param name="graphInfo"></param>
        /// <returns></returns>
        public static Dictionary<int, NodeVariable> DeserializeSkillVariableInfo(GraphInfo graphInfo)
        {
            Dictionary<int, NodeVariable> resultDictionary = new Dictionary<int, NodeVariable>();

            int skillVariableCount = graphInfo.GraphVariableInfosLength;
            for (int i = 0; i < skillVariableCount; i++)
            {
                GraphVariableInfo graphVariableInfo = graphInfo.GraphVariableInfos(i).Value;
                int id = graphVariableInfo.Id;
                string typeName = graphVariableInfo.TypeName;
                string valueString = graphVariableInfo.ValueString;

                if (resultDictionary.ContainsKey(id))
                {
                    continue;
                }
                
                Type variableType = Type.GetType(typeName);
                if (variableType == null)
                {
                    variableType = Type.GetType(typeName + ",UnityEngine");
                    if (variableType == null)
                    {
                        variableType = Type.GetType(typeName + ",Assembly-CSharp");
                    }
                }

                Type baseType = typeof(NodeVariable<>);
                Type genericType = baseType.MakeGenericType(variableType);
                
                NodeVariable instanceVariable = Activator.CreateInstance(genericType) as NodeVariable;

                //支持以下类型初始化值
                if (variableType == typeof(int))
                {
                    NodeVariable<int> variable = instanceVariable as NodeVariable<int>;
                    int intValue;
                    if (int.TryParse(valueString,out intValue))
                    {
                        variable.value = intValue;
                    }
                }
                else if (variableType == typeof(float))
                {
                    NodeVariable<float> variable = instanceVariable as NodeVariable<float>;

                    float floatValue;
                    if (float.TryParse(valueString,out floatValue))
                    {
                        variable.value = floatValue;
                    }
                }
                else if (variableType == typeof(bool))
                {
                    NodeVariable<bool> variable = instanceVariable as NodeVariable<bool>;

                    bool boolValue;
                    if (bool.TryParse(valueString,out boolValue))
                    {
                        variable.value = boolValue;
                    }
                }
                else if (variableType == typeof(string))
                {
                    NodeVariable<string> variable = instanceVariable as NodeVariable<string>;

                    variable.value = valueString;
                }
                
                //其他类型初始时均为null或默认值
                
                resultDictionary.Add(id,instanceVariable);
            }

            return resultDictionary;
        }

        private static List<int> ParseIntList(string valueString)
        {
            List<int> list = new List<int>();
            string[] splitString = valueString.Split('|');
            for (int i = 0; i < splitString.Length; i++)
            {
                list.Add(int.Parse(splitString[i]));
            }

            return list;
        }
    }
}