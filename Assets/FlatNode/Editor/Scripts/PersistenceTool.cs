using System;
using System.Collections.Generic;
using System.IO;
using FlatBuffers;
using FlatNode.Runtime;
using FlatNode.Runtime.Flat;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    /// <summary>
    /// 持久化类，负责图的编辑器存储/加载
    /// </summary>
    public static class PersistenceTool
    {
        public static string GraphRecordFilePath
        {
            get { return Path.Combine(Application.dataPath, "FlatNode/Editor/GraphSavedConfig/GraphsRecord.json"); }
        }


        public static int GetNewGraphId()
        {
            if (File.Exists(GraphRecordFilePath))
            {
                string jsonString = File.ReadAllText(GraphRecordFilePath);
                GraphRecord graphRecord = new GraphRecord();
                EditorJsonUtility.FromJsonOverwrite(jsonString, graphRecord);

                return graphRecord.nextGraphId;
            }

            return 0;
        }

        public static List<GraphBaseInfo> LoadGraphBaseInfoList()
        {
            if (File.Exists(GraphRecordFilePath))
            {
                string jsonString = File.ReadAllText(GraphRecordFilePath);
                GraphRecord graphRecord = new GraphRecord();
                EditorJsonUtility.FromJsonOverwrite(jsonString, graphRecord);

                return graphRecord.graphBaseInfoList;
            }

            return null;
        }

        private static void AddOrUpdateGraphsRecord(GraphEditorData data)
        {
            if (File.Exists(GraphRecordFilePath))
            {
                string jsonString = File.ReadAllText(GraphRecordFilePath);
                GraphRecord graphRecord = new GraphRecord();
                EditorJsonUtility.FromJsonOverwrite(jsonString, graphRecord);

                bool hasFoundGraph = false;
                for (int i = 0; i < graphRecord.graphBaseInfoList.Count; i++)
                {
                    if (graphRecord.graphBaseInfoList[i].graphId == data.graphId)
                    {
                        hasFoundGraph = true;
                        graphRecord.graphBaseInfoList[i].graphName = data.graphName;
                        graphRecord.graphBaseInfoList[i].graphDescription = data.graphDescription;
                        break;
                    }
                }

                if (!hasFoundGraph)
                {
                    graphRecord.graphBaseInfoList.Add(new GraphBaseInfo
                    {
                        graphId = data.graphId, graphName = data.graphName, graphDescription = data.graphDescription
                    });
                    graphRecord.nextGraphId++;
                }

                jsonString = EditorJsonUtility.ToJson(graphRecord, true);
                File.WriteAllText(GraphRecordFilePath, jsonString);
            }
            else
            {
                GraphRecord graphRecord = new GraphRecord();

                graphRecord.graphBaseInfoList = new List<GraphBaseInfo>();
                graphRecord.graphBaseInfoList.Add(new GraphBaseInfo()
                {
                    graphId = data.graphId, graphName = data.graphName, graphDescription = data.graphDescription
                });
                graphRecord.nextGraphId++;

                string jsonString = EditorJsonUtility.ToJson(graphRecord, true);
                File.WriteAllText(GraphRecordFilePath, jsonString);
            }

            AssetDatabase.Refresh();
        }

        private static void RemoveGraphInfoInRecord(int graphId)
        {
            if (File.Exists(GraphRecordFilePath))
            {
                string jsonString = File.ReadAllText(GraphRecordFilePath);
                GraphRecord graphRecord = new GraphRecord();
                EditorJsonUtility.FromJsonOverwrite(jsonString, graphRecord);

                for (int i = 0; i < graphRecord.graphBaseInfoList.Count; i++)
                {
                    if (graphRecord.graphBaseInfoList[i].graphId == graphId)
                    {
                        graphRecord.graphBaseInfoList.RemoveAt(i);

                        jsonString = EditorJsonUtility.ToJson(graphRecord, true);
                        File.WriteAllText(GraphRecordFilePath, jsonString);
                        AssetDatabase.Refresh();

                        return;
                    }
                }
            }
        }

        #region 存储

        public static void SaveGraph(GraphEditorData data)
        {
            string jsonString = String.Empty;
            byte[] runtimeConfigBytes = null;

            bool isSuccess = true;
            try
            {
                //存储技能配置json文件,配置文件使用json是因为可读性好
                GraphConfigInfo graphConfigInfo = new GraphConfigInfo
                {
                    graphId = data.graphId,
                    graphName = data.graphName,
                    graphDescription = data.graphDescription,
                    nodesList = new List<NodeConfigInfo>(),
                    commentBoxInfoList = new List<CommentBoxInfo>(),
                    graphVariableInfoList = new List<GraphVariableInfo>(),
                };

                for (int i = 0; i < data.nodeList.Count; i++)
                {
                    NodeEditorView nodeView = data.nodeList[i];

                    graphConfigInfo.nodesList.Add(ConvertToNodeInfo(nodeView));
                }

                for (int i = 0; i < data.commentBoxViewList.Count; i++)
                {
                    CommentBoxView commentBoxView = data.commentBoxViewList[i];
                    graphConfigInfo.commentBoxInfoList.Add(ConvertToCommentInfo(commentBoxView));
                }

                for (int i = 0; i < data.graphVariableInfoList.Count; i++)
                {
                    graphConfigInfo.graphVariableInfoList.Add(data.graphVariableInfoList[i].OnSerialized());
                }

                jsonString = JsonUtility.ToJson(graphConfigInfo, true);
                runtimeConfigBytes =
                    ConvertToRuntimeInfo(data.nodeList, data.graphVariableInfoList, data.graphId);
            }
            catch (Exception e)
            {
                isSuccess = false;
                Debug.LogError(e.Message);
                throw;
            }
            finally
            {
                if (isSuccess)
                {
                    string graphEditorConfigFilePath = Path.Combine(Application.dataPath,
                        string.Format("FlatNode/Editor/GraphSavedConfig/{0}.json", data.graphId));
                    File.WriteAllText(graphEditorConfigFilePath, jsonString);

                    string graphRuntimeConfigFilePath = Path.Combine(Application.dataPath,
                        string.Format("Resources/GraphRuntime/{0}.bytes", data.graphId));
                    string parentDirctoryPath = Directory.GetParent(graphRuntimeConfigFilePath).FullName;
                    if (!Directory.Exists(parentDirctoryPath))
                    {
                        Directory.CreateDirectory(parentDirctoryPath);
                    }
                    if (runtimeConfigBytes != null)
                    {
                        File.WriteAllBytes(graphRuntimeConfigFilePath, runtimeConfigBytes);
                    }

                    //更新所有图的记录文件
                    AddOrUpdateGraphsRecord(data);
                    
                    AssetDatabase.Refresh();
                }
            }
        }

        private static CommentBoxInfo ConvertToCommentInfo(CommentBoxView commentBoxView)
        {
            CommentBoxInfo commentBoxInfo = new CommentBoxInfo
            {
                comment = commentBoxView.comment,
                positionInGraph = commentBoxView.rectInGraph.position,
                size = commentBoxView.rectInGraph.size
            };
            return commentBoxInfo;
        }

        private static NodeConfigInfo ConvertToNodeInfo(NodeEditorView nodeView)
        {
            NodeConfigInfo nodeConfigInfo = new NodeConfigInfo();
            nodeConfigInfo.nodeId = nodeView.NodeId;
            nodeConfigInfo.positionInGraph = nodeView.PositionInGraph;
            nodeConfigInfo.nodeClassTypeName = nodeView.ReflectionInfo.Type.FullName;

            nodeConfigInfo.flowOutPortInfoList = new List<FlowOutPortConfigInfo>();
            nodeConfigInfo.inputPortInfoList = new List<InputPortConfigInfo>();

            //flow out info
            for (int i = 0; i < nodeView.flowOutPortViews.Length; i++)
            {
                FlowOutPortEditorView flowOutPort = nodeView.flowOutPortViews[i];
                nodeConfigInfo.flowOutPortInfoList.Add(ConvertToFlowOutPortInfo(flowOutPort));
            }

            //input info
            for (int i = 0; i < nodeView.inputPortViewList.Count; i++)
            {
                InputPortEditorView inputPort = nodeView.inputPortViewList[i];
                nodeConfigInfo.inputPortInfoList.Add(ConvertToInputPortInfo(inputPort));
            }

            return nodeConfigInfo;
        }

        private static FlowOutPortConfigInfo ConvertToFlowOutPortInfo(FlowOutPortEditorView flowOutPort)
        {
            FlowOutPortConfigInfo flowOutPortConfigInfo = new FlowOutPortConfigInfo();
            flowOutPortConfigInfo.flowOutPortName = flowOutPort.flowoutPortAttribute.portName;

            flowOutPortConfigInfo.targetNodeList = new List<int>();
            for (int i = 0; i < flowOutPort.connectedPortList.Count; i++)
            {
                int connectedNodeId = flowOutPort.connectedPortList[i].NodeView.NodeId;
                flowOutPortConfigInfo.targetNodeList.Add(connectedNodeId);
            }

            return flowOutPortConfigInfo;
        }

        private static InputPortConfigInfo ConvertToInputPortInfo(InputPortEditorView inputPort)
        {
            InputPortConfigInfo inputPortConfigInfo = new InputPortConfigInfo();

            InputPortReflectionInfo inputPortReflectionInfo = inputPort.inputPortReflectionInfo;
            inputPortConfigInfo.portName = inputPortReflectionInfo.inputPortAttribute.portName;

            if (inputPort.connectedPortList.Count > 0)
            {
                inputPortConfigInfo.targetNodeId = inputPort.connectedPortList[0].NodeView.NodeId;
                OutputPortEditorView outputPortView = inputPort.connectedPortList[0] as OutputPortEditorView;
                if (outputPortView == null)
                {
                    Debug.LogErrorFormat("节点{0}的input端口{1} 连接的接口类型不是OutputPortEditorView", inputPort.NodeView.NodeId,
                        inputPort.portId);
                    return null;
                }

                inputPortConfigInfo.targetPortName = outputPortView.outputPortReflectionInfo.PortName;
            }
            else
            {
                inputPortConfigInfo.targetNodeId = -1;
                inputPortConfigInfo.targetPortName = string.Empty;

                inputPortConfigInfo.nodeVariableGenericTypeName = inputPortReflectionInfo.inputValueType.FullName;
                inputPortConfigInfo.nodeVariableValue = inputPortReflectionInfo.GetNodeVariableValueString();
            }

            return inputPortConfigInfo;
        }

        /// <summary>
        /// 图运行时节点信息
        /// </summary>
        /// <returns></returns>
        private static byte[] ConvertToRuntimeInfo(List<NodeEditorView> nodeViewList,
            List<GraphVariableInfo> graphVariableInfoList, int graphId)
        {
            FlatBufferBuilder fbb = new FlatBufferBuilder(1024);
            Offset<NodeInfo>[] nodeInfoOffsets = new Offset<NodeInfo>[nodeViewList.Count];
            List<int> commonNodeIdList = new List<int>();
            List<int> entranceNodeIdList = new List<int>();
            for (int i = 0; i < nodeViewList.Count; i++)
            {
                NodeEditorView nodeView = nodeViewList[i];
                Offset<NodeInfo> nodeInfoOffset = ConvertToRuntimeNodeInfo(fbb, nodeView);
                nodeInfoOffsets[i] = nodeInfoOffset;

                if (nodeView.CheckNodeIsCommonNode())
                {
                    commonNodeIdList.Add(nodeView.NodeId);
                }

                if (nodeView.ReflectionInfo.IsEntranceNode)
                {
                    entranceNodeIdList.Add(nodeView.NodeId);
                }
            }

            VectorOffset nodeVectorOffset = GraphInfo.CreateNodesVector(fbb, nodeInfoOffsets);

            //common node ids
            GraphInfo.StartCommonNodeIdsVector(fbb, commonNodeIdList.Count);
            for (int i = commonNodeIdList.Count - 1; i >= 0; i--)
            {
                fbb.AddInt(commonNodeIdList[i]);
            }

            VectorOffset commonNodeVectorOffset = fbb.EndVector();

            //entrance node ids
            GraphInfo.StartEntranceNodeIdsVector(fbb, entranceNodeIdList.Count);
            for (int i = entranceNodeIdList.Count - 1; i >= 0; i--)
            {
                fbb.AddInt(entranceNodeIdList[i]);
            }

            VectorOffset entranceNodeVectorOffset = fbb.EndVector();

            //skill variable infos
            int graphVariableCount = graphVariableInfoList.Count;
            Offset<FlatNode.Runtime.Flat.GraphVariableInfo>[] graphVariableInfoOffsets =
                new Offset<FlatNode.Runtime.Flat.GraphVariableInfo>[graphVariableCount];
            for (int i = 0; i < graphVariableCount; i++)
            {
                graphVariableInfoOffsets[i] = ConvertToRuntimeGraphVariableInfo(fbb, graphVariableInfoList[i]);
            }

            VectorOffset graphVariableInfoVectorOffset = GraphInfo.CreateGraphVariableInfosVector(fbb, graphVariableInfoOffsets);

            GraphInfo.StartGraphInfo(fbb);
            GraphInfo.AddGraphId(fbb, graphId);
            GraphInfo.AddCommonNodeIds(fbb, commonNodeVectorOffset);
            GraphInfo.AddEntranceNodeIds(fbb, entranceNodeVectorOffset);
            GraphInfo.AddNodes(fbb, nodeVectorOffset);
            GraphInfo.AddGraphVariableInfos(fbb, graphVariableInfoVectorOffset);
            Offset<GraphInfo> offset = GraphInfo.EndGraphInfo(fbb);
            fbb.Finish(offset.Value);
            return fbb.SizedByteArray();
        }

        private static Offset<NodeInfo> ConvertToRuntimeNodeInfo(FlatBufferBuilder fbb, NodeEditorView nodeView)
        {
            FlowOutPortEditorView[] flowOutPortViews = nodeView.flowOutPortViews;
            Offset<NodeFlowOutPortInfo>[] flowOutInfoOffsets = new Offset<NodeFlowOutPortInfo>[flowOutPortViews.Length];
            for (int i = 0; i < flowOutPortViews.Length; i++)
            {
                flowOutInfoOffsets[i] = ConvertToRuntimeFlowOutPortInfo(fbb, flowOutPortViews[i]);
            }

            List<InputPortEditorView> inputPortViewList = nodeView.inputPortViewList;
            Offset<NodeInputFieldInfo>[] inputPortInfoOffsets = new Offset<NodeInputFieldInfo>[inputPortViewList.Count];
            for (int i = 0; i < inputPortViewList.Count; i++)
            {
                inputPortInfoOffsets[i] = ConvertToRuntimeInputPortInfo(fbb, inputPortViewList[i]);
            }

            //这里处理rightSideNodeIds数组
            VectorOffset rightSideNodeIdsVectorOffset = new VectorOffset();
            if (nodeView.ReflectionInfo.IsEntranceNode || nodeView.ReflectionInfo.IsCreateSequenceNode)
            {
                List<int> rightSideNodeIdList = new List<int>();
                nodeView.GetSequenceNodesIdsRecursive(ref rightSideNodeIdList);

                if (rightSideNodeIdList.Count > 0)
                {
                    rightSideNodeIdsVectorOffset = NodeInfo.CreateRightSideNodeIdsVector(fbb, rightSideNodeIdList.ToArray());
                }
            }

            VectorOffset flowOutVectorOffset = NodeInfo.CreateFlowOutPortInfosVector(fbb, flowOutInfoOffsets);
            VectorOffset inputVectorOffset = NodeInfo.CreateInputPortInfosVector(fbb, inputPortInfoOffsets);
            StringOffset nodeTypeNameOffset = fbb.CreateString(nodeView.ReflectionInfo.Type.FullName);

            NodeInfo.StartNodeInfo(fbb);
            NodeInfo.AddNodeId(fbb, nodeView.NodeId);
            NodeInfo.AddNodeClassTypeName(fbb, nodeTypeNameOffset);
            NodeInfo.AddFlowOutPortInfos(fbb, flowOutVectorOffset);
            NodeInfo.AddInputPortInfos(fbb, inputVectorOffset);
            if (nodeView.ReflectionInfo.IsEntranceNode || nodeView.ReflectionInfo.IsCreateSequenceNode)
            {
                NodeInfo.AddRightSideNodeIds(fbb, rightSideNodeIdsVectorOffset);
            }

            return NodeInfo.EndNodeInfo(fbb);
        }

        private static Offset<NodeFlowOutPortInfo> ConvertToRuntimeFlowOutPortInfo(FlatBufferBuilder fbb,
            FlowOutPortEditorView flowOutPort)
        {
            List<PortEditorView> connectionPortList = flowOutPort.connectedPortList;
            int connectionPortCount = connectionPortList.Count;

            NodeFlowOutPortInfo.StartTargetNodeIdsVector(fbb, connectionPortCount);
            for (int i = connectionPortCount - 1; i >= 0; i--)
            {
                PortEditorView targetPortView = connectionPortList[i];
                fbb.AddInt(targetPortView.NodeView.NodeId);
            }

            VectorOffset offset = fbb.EndVector();

            NodeFlowOutPortInfo.StartNodeFlowOutPortInfo(fbb);
            NodeFlowOutPortInfo.AddTargetNodeIds(fbb, offset);
            return NodeFlowOutPortInfo.EndNodeFlowOutPortInfo(fbb);
        }

        private static Offset<NodeInputFieldInfo> ConvertToRuntimeInputPortInfo(FlatBufferBuilder fbb, InputPortEditorView inputPortView)
        {
            InputPortReflectionInfo inputPortReflectionInfo = inputPortView.inputPortReflectionInfo;

            StringOffset fieldNameOffset =
                fbb.CreateString(inputPortView.inputPortReflectionInfo.nodeInputVariableFieldInfo.Name);
            StringOffset inputValueTypeName = fbb.CreateString(inputPortReflectionInfo.inputValueType.FullName);

            if (inputPortView.connectedPortList.Count > 0)
            {
                NodeInputFieldInfo.StartNodeInputFieldInfo(fbb);
                NodeInputFieldInfo.AddTargetNodeId(fbb, inputPortView.connectedPortList[0].NodeView.NodeId);
                NodeInputFieldInfo.AddTargetPortId(fbb, inputPortView.connectedPortList[0].portId);
            }
            else
            {
                StringOffset inputValueString = fbb.CreateString(inputPortReflectionInfo.GetNodeVariableValueString());

                NodeInputFieldInfo.StartNodeInputFieldInfo(fbb);
                NodeInputFieldInfo.AddTargetNodeId(fbb, -1);
                NodeInputFieldInfo.AddTargetPortId(fbb, -1);
                NodeInputFieldInfo.AddValueString(fbb, inputValueString);
            }

            NodeInputFieldInfo.AddValueTypeName(fbb, inputValueTypeName);
            NodeInputFieldInfo.AddFieldName(fbb, fieldNameOffset);
            return NodeInputFieldInfo.EndNodeInputFieldInfo(fbb);
        }

        private static Offset<FlatNode.Runtime.Flat.GraphVariableInfo> ConvertToRuntimeGraphVariableInfo(FlatBufferBuilder fbb,
            GraphVariableInfo graphVariableInfo)
        {
            StringOffset typeNameStringOffset = fbb.CreateString(graphVariableInfo.typeName);
            StringOffset valueStringOffset = fbb.CreateString(graphVariableInfo.valueString);

            FlatNode.Runtime.Flat.GraphVariableInfo.StartGraphVariableInfo(fbb);

            FlatNode.Runtime.Flat.GraphVariableInfo.AddId(fbb, graphVariableInfo.id);
            FlatNode.Runtime.Flat.GraphVariableInfo.AddValueString(fbb, valueStringOffset);
            FlatNode.Runtime.Flat.GraphVariableInfo.AddTypeName(fbb, typeNameStringOffset);

            return FlatNode.Runtime.Flat.GraphVariableInfo.EndGraphVariableInfo(fbb);
        }

        #endregion

        #region 载入

        public static GraphEditorData LoadGraph(GraphEditorWindow graph, int graphId)
        {
            GraphEditorData resultData = new GraphEditorData();

            string graphEditorConfigFilePath = Path.Combine(Application.dataPath,
                string.Format("FlatNode/Editor/GraphSavedConfig/{0}.json", graphId));

            if (!File.Exists(graphEditorConfigFilePath))
            {
                Debug.LogErrorFormat("无法载入行为图配置文件： {0}", graphEditorConfigFilePath);
            }

            string jsonString = File.ReadAllText(graphEditorConfigFilePath);
            GraphConfigInfo graphConfigInfo = new GraphConfigInfo();
            EditorJsonUtility.FromJsonOverwrite(jsonString, graphConfigInfo);

            //处理注释框
            for (int i = 0; i < graphConfigInfo.commentBoxInfoList.Count; i++)
            {
                CommentBoxInfo commentBoxInfo = graphConfigInfo.commentBoxInfoList[i];
                CommentBoxView commentBoxView = ParseCommentBoxInfo(commentBoxInfo, graph);

                resultData.commentBoxViewList.Add(commentBoxView);
            }

            //变量
            for (int i = 0; i < graphConfigInfo.graphVariableInfoList.Count; i++)
            {
                if (!graphConfigInfo.graphVariableInfoList[i].Validate())
                {
                    continue;
                }

                resultData.graphVariableInfoList.Add(graphConfigInfo.graphVariableInfoList[i].OnDeserialized());
            }

            //如果有节点无法解析出来(可能是改了类名称之类的)，则需要跳过这些节点
            HashSet<int> errorNodeIndexSet = new HashSet<int>();
            //首先将所有的节点都生成
            for (int i = 0; i < graphConfigInfo.nodesList.Count; i++)
            {
                NodeEditorView nodeView = ParseNodeInfo(graphConfigInfo.nodesList[i], graph);
                if (nodeView == null)
                {
                    errorNodeIndexSet.Add(i);
                    continue;
                }

                resultData.nodeList.Add(nodeView);
            }

            //然后再将所有节点的内容写进去，将节点连起来
            int nodeIndex = 0;
            for (int i = 0; i < graphConfigInfo.nodesList.Count; i++)
            {
                if (errorNodeIndexSet.Contains(i))
                {
                    //skip
                    continue;
                }

                UpdateNodeViewData(graphConfigInfo.nodesList[i], resultData.nodeList[nodeIndex], resultData);
                nodeIndex++;
            }

            resultData.graphId = graphConfigInfo.graphId;
            resultData.graphName = graphConfigInfo.graphName;
            resultData.graphDescription = graphConfigInfo.graphDescription;

            return resultData;
        }

        private static CommentBoxView ParseCommentBoxInfo(CommentBoxInfo commentBoxInfo, GraphEditorWindow graph)
        {
            Vector2 startPositionInGraph = commentBoxInfo.positionInGraph;
            Vector2 boxSize = commentBoxInfo.size;
            Vector2 endPositionInGraph = new Vector2(startPositionInGraph.x + boxSize.x, startPositionInGraph.y + boxSize.y);

            CommentBoxView commentBoxView =
                new CommentBoxView(graph, startPositionInGraph, endPositionInGraph, commentBoxInfo.comment);
            return commentBoxView;
        }

        private static NodeEditorView ParseNodeInfo(NodeConfigInfo nodeConfigInfo, GraphEditorWindow graph)
        {
            string nodeTypeName = nodeConfigInfo.nodeClassTypeName;
            Type nodeType = Type.GetType(nodeTypeName + ",Assembly-CSharp");
            if (nodeType == null)
            {
                Debug.LogErrorFormat("无法载入类型{0} ,该节点被跳过", nodeTypeName);
                return null;
            }

            NodeReflectionInfo reflectionInfo = new NodeReflectionInfo(nodeType);
            NodeEditorView nodeView =
                new NodeEditorView(nodeConfigInfo.positionInGraph, graph, nodeConfigInfo.nodeId, reflectionInfo);

            return nodeView;
        }

        private static void UpdateNodeViewData(NodeConfigInfo nodeConfigInfo, NodeEditorView nodeView, GraphEditorData data)
        {
            //flow in port--处理流出节点的时候顺便就处理了

            //flow out port
            for (int i = 0; i < nodeConfigInfo.flowOutPortInfoList.Count; i++)
            {
                FlowOutPortConfigInfo flowOutPortConfigInfo = nodeConfigInfo.flowOutPortInfoList[i];
                FlowOutPortEditorView flowOutPortView =
                    GetFlowOutPortViewByPortName(nodeView.flowOutPortViews, flowOutPortConfigInfo.flowOutPortName);
                if (flowOutPortView == null)
                {
                    Debug.LogFormat("节点{0}中找不到流出端口 <{1}> 了,该端口的连接被忽略", nodeView.ReflectionInfo.Type,
                        flowOutPortConfigInfo.flowOutPortName);
                    continue;
                }

                for (int j = 0; j < flowOutPortConfigInfo.targetNodeList.Count; j++)
                {
                    int targetNodeId = flowOutPortConfigInfo.targetNodeList[j];
                    NodeEditorView targetNodeView = data.GetNode(targetNodeId);
                    if (targetNodeView == null)
                    {
                        Debug.LogErrorFormat("无法找到节点{0}，可能是配置损坏/更改了类名...", targetNodeId);
                        continue;
                    }

                    if (targetNodeView.flowInPortView == null)
                    {
                        Debug.LogErrorFormat("节点类型{0}没有流入节点，是否节点性质发生了改变?", nodeView.ReflectionInfo.Type.FullName);
                        continue;
                    }

                    ConnectionLineView connectionLineView =
                        new ConnectionLineView(flowOutPortView, targetNodeView.flowInPortView, data);
                    data.connectionLineList.Add(connectionLineView);
                }
            }

            //output port -- 不用配置

            //input port
            for (int i = 0; i < nodeConfigInfo.inputPortInfoList.Count; i++)
            {
                InputPortConfigInfo inputPortConfigInfo = nodeConfigInfo.inputPortInfoList[i];
                InputPortEditorView inputPortView =
                    GetInputPortViewByPortName(nodeView.inputPortViewList, inputPortConfigInfo.portName);

                if (inputPortView == null)
                {
                    Debug.LogFormat("节点{0}中无法找到接口名字为 <{1}> 的NodeInputVariable Field，该接口配置被跳过",
                        nodeView.ReflectionInfo.Type.FullName, inputPortConfigInfo.portName);
                    continue;
                }

                //没有连接到其他节点的情况
                if (string.IsNullOrEmpty(inputPortConfigInfo.targetPortName))
                {
                    //设置默认值
                    Type valueType = Type.GetType(inputPortConfigInfo.nodeVariableGenericTypeName);
                    if (valueType == null)
                    {
                        valueType = Type.GetType(inputPortConfigInfo.nodeVariableGenericTypeName + ",UnityEngine");
                        if (valueType == null)
                        {
                            valueType = Type.GetType(inputPortConfigInfo.nodeVariableGenericTypeName + ",Assembly-CSharp");
                        }
                    }

                    if (valueType == null)
                    {
                        Debug.LogErrorFormat("工程中找不到类型： {0}", inputPortConfigInfo.nodeVariableGenericTypeName);
                        continue;
                    }

                    SetNodeInputVariableValue(inputPortView.inputPortReflectionInfo, valueType,
                        inputPortConfigInfo.nodeVariableValue, nodeView);
                }
                //连接到其他节点的情况
                else
                {
                    NodeEditorView connectedToNodeView = data.GetNode(inputPortConfigInfo.targetNodeId);
                    if (connectedToNodeView == null)
                    {
                        Debug.LogErrorFormat("节点 {0} 的input接口 {1} 找不到连接的节点{2}", nodeView.NodeId, inputPortConfigInfo.portName,
                            inputPortConfigInfo.targetNodeId);
                        continue;
                    }

                    OutputPortEditorView connectedToOutputPortView =
                        GetOutputPortViewByPortName(connectedToNodeView.outputPortViewList, inputPortConfigInfo.targetPortName);
                    if (connectedToOutputPortView == null)
                    {
                        Debug.LogFormat("找不到节点{0}中 接口名字为 <{1}> 的output接口，该接口的连接被跳过", connectedToNodeView.NodeId,
                            inputPortConfigInfo.targetPortName);
                        continue;
                    }

                    ConnectionLineView connectionLineView =
                        new ConnectionLineView(inputPortView, connectedToOutputPortView, data);
                    data.connectionLineList.Add(connectionLineView);
                }
            }
        }

        private static InputPortEditorView GetInputPortViewByPortName(List<InputPortEditorView> inputPortViewList, string fieldName)
        {
            if (inputPortViewList == null)
            {
                return null;
            }

            for (int i = 0; i < inputPortViewList.Count; i++)
            {
                if (inputPortViewList[i].inputPortReflectionInfo.PortName == fieldName)
                {
                    return inputPortViewList[i];
                }
            }

            return null;
        }

        private static FlowOutPortEditorView GetFlowOutPortViewByPortName(FlowOutPortEditorView[] flowOutPortEditorViews, string portName)
        {
            if (flowOutPortEditorViews == null)
            {
                return null;
            }

            for (int i = 0; i < flowOutPortEditorViews.Length; i++)
            {
                if (flowOutPortEditorViews[i].flowoutPortAttribute.portName == portName)
                {
                    return flowOutPortEditorViews[i];
                }
            }

            return null;
        }

        private static OutputPortEditorView GetOutputPortViewByPortName(List<OutputPortEditorView> outputPortViewList, string portName)
        {
            if (outputPortViewList == null)
            {
                return null;
            }

            for (int i = 0; i < outputPortViewList.Count; i++)
            {
                if (outputPortViewList[i].outputPortReflectionInfo.PortName == portName)
                {
                    return outputPortViewList[i];
                }
            }

            return null;
        }

        /// <summary>
        /// 根据input port的类型，将对应数据初始化
        /// </summary>
        /// <param name="inputPortReflectionInfo"></param>
        /// <param name="type"></param>
        /// <param name="valueString"></param>
        /// <param name="nodeView"></param>
        private static void SetNodeInputVariableValue(InputPortReflectionInfo inputPortReflectionInfo, Type type, string valueString,
            NodeEditorView nodeView)
        {
            if (inputPortReflectionInfo.inputValueType != type)
            {
                Debug.LogErrorFormat("节点{0}中的Input Port {1} 的泛型改变了，之前是{2}，现在是{3}", nodeView.ReflectionInfo.Type,
                    inputPortReflectionInfo.PortName, type.FullName, inputPortReflectionInfo.inputValueType.FullName);
                return;
            }

            if (type == typeof(int))
            {
                int value = int.Parse(valueString);
                inputPortReflectionInfo.SetInputNodeVariableValue(value);
            }
            else if (type == typeof(float))
            {
                float value = float.Parse(valueString);
                inputPortReflectionInfo.SetInputNodeVariableValue(value);
            }
            else if (type == typeof(string))
            {
                inputPortReflectionInfo.SetInputNodeVariableValue(valueString);
            }
            else if (type == typeof(bool))
            {
                bool value = Boolean.Parse(valueString);
                inputPortReflectionInfo.SetInputNodeVariableValue(value);
            }
            else if (type.IsEnum)
            {
                object value = Enum.Parse(type, valueString);
                inputPortReflectionInfo.SetInputNodeVariableValue(value);
            }
            else if (type == typeof(List<int>))
            {
                string[] splitString = valueString.Split('|');
                List<int> value = new List<int>();
                for (int i = 0; i < splitString.Length; i++)
                {
                    value.Add(int.Parse(splitString[i]));
                }

                inputPortReflectionInfo.SetInputNodeVariableValue(value);
            }
            else if (type == typeof(LayerMaskWrapper))
            {
                LayerMaskWrapper value = valueString;
                inputPortReflectionInfo.SetInputNodeVariableValue(value);
            }
            else if (type == typeof(VariableWrapper))
            {
                int intValue;
                if (int.TryParse(valueString, out intValue))
                {
                    VariableWrapper variableWrapper = intValue;
                    inputPortReflectionInfo.SetInputNodeVariableValue(variableWrapper);
                }
                else
                {
                    Debug.LogErrorFormat("标记为VariableWrapper类型的input接口，但是它记录的值不是int类型的： {0}", valueString);
                }
            }
        }

        #endregion

        #region 删除

        public static void DeleteSkillFiles(int graphId)
        {
            //删掉editor配置文件
            string graphEditorConfigFilePath = Path.Combine(Application.dataPath,
                string.Format("FlatNode/Editor/GraphSavedConfig/{0}.json", graphId));
            //删掉运行时配置文件
            string graphRuntimeConfigFilePath =
                Path.Combine(Application.dataPath, string.Format("Resources/GraphRuntime/{0}.bytes", graphId));

            RemoveGraphInfoInRecord(graphId);

            if (File.Exists(graphEditorConfigFilePath))
            {
                File.Delete(graphEditorConfigFilePath);
            }

            if (File.Exists(graphRuntimeConfigFilePath))
            {
                File.Delete(graphRuntimeConfigFilePath);
            }
        }

        #endregion
    }

    /// <summary>
    /// 所有图信息的集合
    /// 这样就不用编辑器中打开图选择菜单时，要解包所有json文件只是为了获取所有图的id、名称、说明之类的了
    /// 感觉这种设计也不是很好
    /// </summary>
    [Serializable]
    public class GraphRecord
    {
        public int nextGraphId;
        public List<GraphBaseInfo> graphBaseInfoList;
    }

    /// <summary>
    /// 图的基本信息
    /// </summary>
    [Serializable]
    public class GraphBaseInfo
    {
        public int graphId;
        public string graphName;
        public string graphDescription;
    }

    [Serializable]
    public class GraphConfigInfo
    {
        public int graphId;
        public string graphName;
        public string graphDescription;

        public List<NodeConfigInfo> nodesList;
        public List<CommentBoxInfo> commentBoxInfoList;
        public List<GraphVariableInfo> graphVariableInfoList;
    }

    [Serializable]
    public class NodeConfigInfo
    {
        public int nodeId;
        public Vector2 positionInGraph;
        public string nodeClassTypeName;

        public List<InputPortConfigInfo> inputPortInfoList;
        public List<FlowOutPortConfigInfo> flowOutPortInfoList;
    }

    [Serializable]
    public class InputPortConfigInfo
    {
        public string portName;

        public string targetPortName;
        public int targetNodeId;

        public string nodeVariableGenericTypeName;
        public string nodeVariableValue;
    }

    [Serializable]
    public class FlowOutPortConfigInfo
    {
        public string flowOutPortName;
        public List<int> targetNodeList;
    }

    [Serializable]
    public class CommentBoxInfo
    {
        public Vector2 positionInGraph;
        public Vector2 size;
        public string comment;
    }

    /// <summary>
    /// 用于存储图变量在技能编辑器中编辑显示需要的信息
    /// </summary>
    [Serializable]
    public class GraphVariableInfo
    {
        public int id;
        public string name;
        public string valueString;
        [NonSerialized] public Type valueType;

        public string typeName;

        public GraphVariableInfo OnSerialized()
        {
            typeName = valueType.FullName;
            return this;
        }

        public GraphVariableInfo OnDeserialized()
        {
            valueType = Type.GetType(typeName);
            if (valueType == null)
            {
                valueType = Type.GetType(typeName + ",UnityEngine");
                if (valueType == null)
                {
                    valueType = Type.GetType(typeName + ",Assembly-CSharp");
                }
            }

            return this;
        }

        public bool Validate()
        {
            if (string.IsNullOrEmpty(typeName))
            {
                return false;
            }

            Type checkType = Type.GetType(typeName);
            if (checkType == null)
            {
                checkType = Type.GetType(typeName + ",UnityEngine");
                if (checkType == null)
                {
                    checkType = Type.GetType(typeName + ",Assembly-CSharp");
                }
            }

            return checkType != null;
        }
    }
}