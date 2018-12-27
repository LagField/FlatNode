using System;
using System.Collections.Generic;
using System.Linq;
using FlatNode.Runtime;
using UnityEngine;

namespace FlatNode.Editor
{
    /// <summary>
    /// 负责在编辑器界面绘制节点
    /// </summary>
    public class NodeEditorView
    {
        public GraphEditorWindow graph;

        public Vector2 PositionInWindow { get; private set; }
        public Vector2 PositionInGraph { get; set; }
        public Rect viewRect;

        public int NodeId { get; set; }
        public NodeReflectionInfo ReflectionInfo { get; private set; }

        public FlowInPortEditorView flowInPortView;
        public FlowOutPortEditorView[] flowOutPortViews;

        private PortLayoutHelper leftPortLayoutHelper;
        private PortLayoutHelper rightPortLayoutHelper;

        public List<InputPortEditorView> inputPortViewList;
        public List<OutputPortEditorView> outputPortViewList;
        public List<PortEditorView> allPortList;

        public bool isSelected;

        private const float PortAreaPadding = 20f;

        #region Rect Defines

        public Rect NodeNameRect
        {
            get { return new Rect(viewRect.x, viewRect.y + 5, viewRect.width, 20f); }
        }

        #endregion

        public NodeEditorView(Vector2 graphPosition, GraphEditorWindow graph, int nodeId, NodeReflectionInfo reflectionInfo)
        {
            this.graph = graph;
            this.NodeId = nodeId;
            this.ReflectionInfo = reflectionInfo;

            PositionInGraph = graphPosition;
            PositionInWindow = graph.GraphPositionToWindowPosition(graphPosition);

            viewRect = new Rect(Vector2.zero, new Vector2(200, 400));

            allPortList = new List<PortEditorView>();
            inputPortViewList = new List<InputPortEditorView>();
            outputPortViewList = new List<OutputPortEditorView>();

            leftPortLayoutHelper = new PortLayoutHelper();
            rightPortLayoutHelper = new PortLayoutHelper();

            if (reflectionInfo.HasFlowInPort)
            {
                flowInPortView = new FlowInPortEditorView(this);
                flowInPortView.portId = 0;
                allPortList.Add(flowInPortView);
            }

            if (reflectionInfo.flowOutPortDefineAttributes.Length > 0)
            {
                flowOutPortViews = new FlowOutPortEditorView[reflectionInfo.flowOutPortDefineAttributes.Length];
                for (int i = 0; i < flowOutPortViews.Length; i++)
                {
                    flowOutPortViews[i] = new FlowOutPortEditorView(this, reflectionInfo.flowOutPortDefineAttributes[i]);
                    flowOutPortViews[i].portId = i;
                    allPortList.Add(flowOutPortViews[i]);
                }
            }
            else
            {
                flowOutPortViews = new FlowOutPortEditorView[0];
            }

            List<InputPortReflectionInfo> inputPortReflectionInfos = reflectionInfo.inputPortInfoList;
            for (int i = 0; i < inputPortReflectionInfos.Count; i++)
            {
                InputPortReflectionInfo inputPortReflectionInfo = inputPortReflectionInfos[i];
                InputPortEditorView inputPortView = new InputPortEditorView(this, inputPortReflectionInfo);
                inputPortView.portId = i;

                inputPortViewList.Add(inputPortView);
                allPortList.Add(inputPortView);
            }

            List<OutputPortReflectionInfo> outputPortReflectionInfos = reflectionInfo.outputPortInfoList;
            for (int i = 0; i < outputPortReflectionInfos.Count; i++)
            {
                OutputPortReflectionInfo outputPortReflectionInfo = outputPortReflectionInfos[i];
                OutputPortEditorView outputPortView = new OutputPortEditorView(this, outputPortReflectionInfo, i);
                outputPortView.portId = i;

                outputPortViewList.Add(outputPortView);
                allPortList.Add(outputPortView);
            }

            CalculateNodeSize();
        }

        void CalculateNodeSize()
        {
            float nodeNameWidth =
                Utility.GetStringGuiWidth(ReflectionInfo.NodeName + "(0)   ", Utility.GetGuiStyle("NodeName").fontSize);
            //width
            float leftMaxWidth = 0f, rightMaxWidth = 0f;
            int leftPortCount = 0;
            int rightPortCount = 0;
            for (int i = 0; i < allPortList.Count(); i++)
            {
                PortEditorView portView = allPortList[i];
                float width = portView.GetNameWidth();

                if (portView.FlowType == FlowType.In)
                {
                    leftPortCount++;
                    if (width > leftMaxWidth)
                    {
                        leftMaxWidth = width;
                    }
                }
                else
                {
                    rightPortCount++;
                    if (width > rightMaxWidth)
                    {
                        rightMaxWidth = width;
                    }
                }
            }

            viewRect.width = Mathf.Max(leftMaxWidth + rightMaxWidth + PortAreaPadding, nodeNameWidth);
            viewRect.height = NodeNameRect.height + PortAreaPadding + Mathf.Max(
                                  PortLayoutHelper.CalculateHeightByPortCount(leftPortCount),
                                  PortLayoutHelper.CalculateHeightByPortCount(rightPortCount));


            leftPortLayoutHelper.SetOffset(0, leftMaxWidth);
            rightPortLayoutHelper.SetOffset(viewRect.width - rightMaxWidth, rightMaxWidth); //中间留点padding
        }

        /// <summary>
        /// 技能载入完成时调入，这个时候所有节点和所有连线都已经生成，这里处理一下其他需要初始化的逻辑
        /// </summary>
        public void OnLoadFinish()
        {
            for (int i = 0; i < inputPortViewList.Count; i++)
            {
                inputPortViewList[i].OnLoadFinish();
            }

            for (int i = 0; i < outputPortViewList.Count; i++)
            {
                outputPortViewList[i].OnLoadFinish();
            }
        }

        public void DrawNodeGUI()
        {
            if (graph == null)
            {
                return;
            }

            PositionInWindow = graph.GraphPositionToWindowPosition(PositionInGraph);
            viewRect.center = PositionInWindow;

            if (isSelected)
            {
                Rect highLightRect = new Rect(viewRect);
                highLightRect.position = highLightRect.position - Vector2.one * 2f;
                highLightRect.max = highLightRect.max + Vector2.one * 4f;
                GUI.Box(highLightRect, "", Utility.GetGuiStyle("Highlight"));
            }

            //draw back ground
            if (ReflectionInfo.IsEntranceNode)
            {
                GUI.Box(viewRect, "", Utility.GetGuiStyle("EntranceNode"));
            }

            if (ReflectionInfo.Type.IsSubclassOf(typeof(GraphVariableNodeBase)))
            {
                GUI.Box(viewRect, "", Utility.GetGuiStyle("NodeCyan"));
            }
            else
            {
                GUI.Box(viewRect, "", Utility.GetGuiStyle("NodeBg"));
            }

            //draw node name
            GUI.Label(NodeNameRect, string.Format("({0}){1}", NodeId, ReflectionInfo.NodeName), Utility.GetGuiStyle("NodeName"));

            leftPortLayoutHelper.SetPosition(new Vector2(viewRect.x, viewRect.y + NodeNameRect.height + PortAreaPadding));
            rightPortLayoutHelper.SetPosition(new Vector2(viewRect.x, viewRect.y + NodeNameRect.height + PortAreaPadding));

            if (flowInPortView != null)
            {
                flowInPortView.portViewRect = leftPortLayoutHelper.GetRect();
            }

            for (int i = 0; i < inputPortViewList.Count; i++)
            {
                InputPortEditorView inputPortView = inputPortViewList[i];
                inputPortView.portViewRect = leftPortLayoutHelper.GetRect();
            }

            if (flowOutPortViews.Length > 0)
            {
                for (int i = 0; i < flowOutPortViews.Length; i++)
                {
                    FlowOutPortEditorView flowoutPortView = flowOutPortViews[i];
                    flowoutPortView.portViewRect = rightPortLayoutHelper.GetRect();
                }
            }

            for (int i = 0; i < outputPortViewList.Count; i++)
            {
                OutputPortEditorView outputPortView = outputPortViewList[i];
                outputPortView.portViewRect = rightPortLayoutHelper.GetRect();
            }

            for (int i = 0; i < allPortList.Count; i++)
            {
                PortEditorView portView = allPortList[i];

                portView.Draw();
            }
        }

        public void Drag(Vector2 delta)
        {
            PositionInGraph += delta;
        }

        /// <summary>
        /// 递归获取该节点创建sequence时需要包含的所有节点id
        /// 遇到CreateSequenceNode则停止往右探索
        /// </summary>
        /// <returns></returns>
        public void GetSequenceNodesIdsRecursive(ref List<int> rightSideNodeIdList)
        {
            if (rightSideNodeIdList == null)
            {
                rightSideNodeIdList = new List<int>();
            }

            if (flowOutPortViews != null)
            {
                for (int i = 0; i < flowOutPortViews.Length; i++)
                {
                    List<PortEditorView> connectionPortList = flowOutPortViews[i].connectedPortList;
                    for (int j = 0; j < connectionPortList.Count; j++)
                    {
                        NodeEditorView targetNode = connectionPortList[j].NodeView;
                        int nodeId = targetNode.NodeId;
                        if (!rightSideNodeIdList.Contains(nodeId))
                        {
                            rightSideNodeIdList.Add(nodeId);

                            if (!targetNode.ReflectionInfo.IsCreateSequenceNode)
                            {
                                targetNode.GetSequenceNodesIdsRecursive(ref rightSideNodeIdList);
                            }
                        }
                    }
                }
            }

            for (int i = 0; i < outputPortViewList.Count; i++)
            {
                OutputPortEditorView outputPort = outputPortViewList[i];
                List<PortEditorView> connectionPortList = outputPort.connectedPortList;
                for (int j = 0; j < connectionPortList.Count; j++)
                {
                    NodeEditorView targetNode = connectionPortList[j].NodeView;
                    int nodeId = targetNode.NodeId;
                    if (!rightSideNodeIdList.Contains(nodeId))
                    {
                        rightSideNodeIdList.Add(nodeId);

                        if (!targetNode.ReflectionInfo.IsCreateSequenceNode)
                        {
                            targetNode.GetSequenceNodesIdsRecursive(ref rightSideNodeIdList);
                        }
                    }
                }
            }
        }

        /// <summary>
        /// 检查一个节点是否是通用节点。
        /// 通用节点是指该节点不会从技能入口流入到或者取值不会从能够从技能入口流入到的节点取值。
        /// 递归所有左Flow In节点和Input节点，如果能够访问到入口节点，则该节点不是通用节点。
        /// </summary>
        /// <returns></returns>
        public bool CheckNodeIsCommonNode(HashSet<NodeEditorView> checkedNodeSet = null)
        {
            if (ReflectionInfo.Type.IsSubclassOf(typeof(EntranceNodeBase)))
            {
                return false;
            }

            if (checkedNodeSet == null)
            {
                checkedNodeSet = new HashSet<NodeEditorView>();
            }

            if (flowInPortView != null && flowInPortView.connectedPortList.Count > 0)
            {
                List<PortEditorView> connectionPortList = flowInPortView.connectedPortList;
                for (int i = 0; i < connectionPortList.Count; i++)
                {
                    NodeEditorView nodeView = connectionPortList[i].NodeView;
                    if (checkedNodeSet.Contains(nodeView))
                    {
                        continue;
                    }

                    if (nodeView.ReflectionInfo.Type.IsSubclassOf(typeof(EntranceNodeBase)))
                    {
                        return false;
                    }

                    checkedNodeSet.Add(nodeView);
                    if (!nodeView.CheckNodeIsCommonNode(checkedNodeSet))
                    {
                        return false;
                    }
                }
            }

            if (inputPortViewList.Count > 0)
            {
                for (int i = 0; i < inputPortViewList.Count; i++)
                {
                    List<PortEditorView> connectionPortList = inputPortViewList[i].connectedPortList;
                    for (int j = 0; j < connectionPortList.Count; j++)
                    {
                        NodeEditorView nodeView = connectionPortList[j].NodeView;
                        if (checkedNodeSet.Contains(nodeView))
                        {
                            continue;
                        }

                        if (nodeView.ReflectionInfo.Type.IsSubclassOf(typeof(EntranceNodeBase)))
                        {
                            return false;
                        }

                        checkedNodeSet.Add(nodeView);
                        if (!nodeView.CheckNodeIsCommonNode(checkedNodeSet))
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public bool IsContainInRect(Rect rect)
        {
            return rect.Contains(viewRect.position, true) && rect.Contains(viewRect.max, true);
        }

        /// <summary>
        /// 检查input port和output port连线的合法性
        /// </summary>
        public void CheckIOPortConnectionValidate()
        {
            for (int i = 0; i < inputPortViewList.Count; i++)
            {
                InputPortEditorView inputPortView = inputPortViewList[i];
                if (inputPortView.connectedPortList.Count == 1)
                {
                    if (!ConnectionLineView.CheckPortsCanLine(inputPortView, inputPortView.connectedPortList[0]))
                    {
                        graph.data.FindConnectionByPortsAndRemoveIt(inputPortView, inputPortView.connectedPortList[0]);
                    }
                }
            }

            for (int i = 0; i < outputPortViewList.Count; i++)
            {
                OutputPortEditorView outputPortView = outputPortViewList[i];
                for (int j = 0; j < outputPortView.connectedPortList.Count; j++)
                {
                    InputPortEditorView connectedInputPortView = outputPortView.connectedPortList[j] as InputPortEditorView;
                    if (connectedInputPortView == null)
                    {
                        continue;
                    }

                    if (!ConnectionLineView.CheckPortsCanLine(outputPortView, connectedInputPortView))
                    {
                        graph.data.FindConnectionByPortsAndRemoveIt(outputPortView, connectedInputPortView);
                    }
                }
            }
        }

        /// <summary>
        /// 当节点对应的是<see cref="FlatNode.Runtime.GetVariableNode"/> 或者 <see cref="FlatNode.Runtime.SetVariableNode"/> 时
        /// 他们接口的类型需要显示为对应类型。
        /// 还要重新计算节点Rect的大小
        /// </summary>
        /// <param name="variableType"></param>
        /// <param name="needRecheckConnection"></param>
        public void UpdateGraphVariableNodeIOPortType(Type variableType,bool needRecheckConnection)
        {
            if (ReflectionInfo.Type == typeof(GetVariableNode))
            {
                //第一个input port是选择要读取哪一个变量
                InputPortEditorView inputPortView = inputPortViewList[0];
                inputPortView.overridePortType = variableType;

                //第一个output port是要输出选择的变量
                OutputPortEditorView outputPortView = outputPortViewList[0];
                outputPortView.overridePortType = variableType;

                CalculateNodeSize();

                if (needRecheckConnection)
                {
                    CheckIOPortConnectionValidate();
                }
            }
            else if (ReflectionInfo.Type == typeof(SetVariableNode))
            {
                //第一个input port是选择要存储到哪个变量
                InputPortEditorView setVariableInputPort = inputPortViewList[0];
                setVariableInputPort.overridePortType = variableType;

                //第二个input port是接收要存储的值
                InputPortEditorView valueVariableInputPort = inputPortViewList[1];
                valueVariableInputPort.overridePortType = variableType;

                //该节点没有output port

                CalculateNodeSize();
                
                if (needRecheckConnection)
                {
                    CheckIOPortConnectionValidate();
                }

            }
        }

        /// <summary>
        /// 当编辑了变量列表后，需要刷新一下所有接口，看是否还允许连接
        /// </summary>
        public void OnGraphVariableListChange()
        {
            for (int i = 0; i < inputPortViewList.Count; i++)
            {
                inputPortViewList[i].OnGraphVariableListChange();
            }
        }
    }
}