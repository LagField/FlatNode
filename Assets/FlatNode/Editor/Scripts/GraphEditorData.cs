using System;
using System.Collections.Generic;
using FlatNode.Runtime;
using UnityEngine;

namespace FlatNode.Editor
{
    public class GraphEditorData
    {
        const int MaxNodeLimit = 500;

        public Vector2 GraphOffset
        {
            get { return graphOffset; }
            set { graphOffset = value; }
        }
        private Vector2 graphOffset;

        public float GraphZoom
        {
            get { return graphZoom; }
            set { graphZoom = value; }
        }
        private float graphZoom;

        public const float MaxGraphZoom = 1f;
        public const float MinGraphZoom = .2f;
        public const float GraphZoomSpeed = 50f;

        public int graphId;
        public string graphName;
        public string graphDescription;

        public List<NodeEditorView> nodeList;

        public List<NodeEditorView> CurrentNodeList
        {
            get { return nodeList; }
        }

        public List<NodeEditorView> selectedNodeList; //选中的节点

        public List<ConnectionLineView> CurrentConnectionLineList
        {
            get { return connectionLineList; }
        }

        public List<ConnectionLineView> connectionLineList;

        public List<CommentBoxView> CurrentCommentBoxViewList
        {
            get { return commentBoxViewList; }
        }

        public List<CommentBoxView> commentBoxViewList;

        public List<GraphVariableInfo> CurrentGraphVariableInfoList
        {
            get { return graphVariableInfoList; }
        }

        public List<GraphVariableInfo> graphVariableInfoList;


        public GraphEditorData()
        {
            nodeList = new List<NodeEditorView>();
            connectionLineList = new List<ConnectionLineView>();
            commentBoxViewList = new List<CommentBoxView>();
            graphVariableInfoList = new List<GraphVariableInfo>();
            selectedNodeList = new List<NodeEditorView>();

            Clear();
        }

        public int GetNewNodeId()
        {
            for (int i = 0; i < MaxNodeLimit; i++)
            {
                bool hasAssigned = false;
                for (int j = 0; j < CurrentNodeList.Count; j++)
                {
                    if (CurrentNodeList[j].NodeId == i)
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

            return 0;
        }

        public void DeleteNode(NodeEditorView nodeView)
        {
            if (nodeView == null)
            {
                return;
            }

            if (!nodeList.Contains(nodeView))
            {
                return;
            }

            List<PortEditorView> allPortList = nodeView.allPortList;
            for (int i = 0; i < allPortList.Count; i++)
            {
                PortEditorView portView = allPortList[i];
                ClearPortAllConnections(portView);
            }

            nodeList.Remove(nodeView);
            if (CurrentNodeList.Contains(nodeView))
            {
                CurrentNodeList.Remove(nodeView);
            }
        }

        public void DeleteConnectionLine(ConnectionLineView connectionLineView)
        {
            if (connectionLineView == null)
            {
                return;
            }

            if (!connectionLineList.Contains(connectionLineView))
            {
                return;
            }

            PortEditorView flowOutPort = connectionLineView.FlowOutPortView;
            PortEditorView flowInport = connectionLineView.FlowInPortView;

            if (flowOutPort.connectedPortList.Contains(flowInport))
            {
                flowOutPort.connectedPortList.Remove(flowInport);
            }

            if (flowInport.connectedPortList.Contains(flowOutPort))
            {
                flowInport.connectedPortList.Remove(flowOutPort);
            }

            connectionLineList.Remove(connectionLineView);

            if (CurrentConnectionLineList.Contains(connectionLineView))
            {
                CurrentConnectionLineList.Remove(connectionLineView);
            }
        }

        public void DeleteCommentBox(CommentBoxView commentBoxView)
        {
            if (CurrentCommentBoxViewList.Contains(commentBoxView))
            {
                CurrentCommentBoxViewList.Remove(commentBoxView);
            }
        }

        public void ClearPortAllConnections(PortEditorView portView)
        {
            if (portView == null)
            {
                return;
            }

            List<PortEditorView> connectedPortList = portView.connectedPortList;
            for (int i = 0; i < connectedPortList.Count; i++)
            {
                PortEditorView connectedPort = connectedPortList[i];

                if (connectedPort.connectedPortList.Contains(portView))
                {
                    connectedPort.connectedPortList.Remove(portView);
                    FindConnectionByPortsAndRemoveIt(portView, connectedPort);
                }
            }

            portView.connectedPortList.Clear();
        }

        public void FindConnectionByPortsAndRemoveIt(PortEditorView portA, PortEditorView portB)
        {
            if (portA == null || portB == null)
            {
                return;
            }

            if (portA.FlowType == portB.FlowType)
            {
                Debug.LogError("RemoveConnectionByPort err: 两个接口类型相同");
                return;
            }

            PortEditorView flowInPort = portA.FlowType == FlowType.In ? portA : portB;
            PortEditorView flowOutPort = portA.FlowType == FlowType.Out ? portA : portB;

            ConnectionLineView needRemoveConnectionLineView = null;

            for (int i = 0; i < connectionLineList.Count; i++)
            {
                ConnectionLineView connectionLineView = connectionLineList[i];
                if (connectionLineView.FlowInPortView == flowInPort && connectionLineView.FlowOutPortView == flowOutPort)
                {
                    needRemoveConnectionLineView = connectionLineView;
                    break;
                }
            }

            if (needRemoveConnectionLineView != null)
            {
                connectionLineList.Remove(needRemoveConnectionLineView);
                if (CurrentConnectionLineList.Contains(needRemoveConnectionLineView))
                {
                    CurrentConnectionLineList.Remove(needRemoveConnectionLineView);
                }
            }
        }

        public NodeEditorView GetNode(int nodeId)
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                if (nodeList[i].NodeId == nodeId)
                {
                    return nodeList[i];
                }
            }

            return null;
        }

        public void Clear()
        {
            graphId = 0;
            graphName = string.Empty;
            graphDescription = string.Empty;

            nodeList.Clear();
            connectionLineList.Clear();
            commentBoxViewList.Clear();

            graphOffset = Vector2.zero;
            graphZoom = 1f;

            ClearSelectedNode();
        }

        /// <summary>
        /// 每一种入口节点只允许存在一个
        /// </summary>
        /// <param name="testType"></param>
        /// <returns></returns>
        public bool CheckDuplicateEntranceNode(Type testType)
        {
            if (!testType.IsSubclassOf(typeof(EntranceNodeBase)))
            {
                return false;
            }

            for (int i = 0; i < nodeList.Count; i++)
            {
                NodeEditorView nodeView = nodeList[i];
                if (nodeView.ReflectionInfo.Type == testType)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 检查当前技能是否有入口节点
        /// </summary>
        /// <returns></returns>
        public bool CheckHasEntranceNode()
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                NodeEditorView nodeView = nodeList[i];
                if (nodeView.ReflectionInfo.Type.IsSubclassOf(typeof(EntranceNodeBase)))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 将节点放到队尾，这样在渲染的时候就可以让这个节点在最上面
        /// </summary>
        public void PutNodeToListTail(int targetNodeIndex)
        {
            if (targetNodeIndex < 0 || targetNodeIndex >= CurrentNodeList.Count)
            {
                return;
            }

            if (targetNodeIndex == CurrentNodeList.Count - 1)
            {
                return;
            }

            NodeEditorView tailNode = CurrentNodeList[CurrentNodeList.Count - 1];
            CurrentNodeList[CurrentNodeList.Count - 1] = CurrentNodeList[targetNodeIndex];
            CurrentNodeList[targetNodeIndex] = tailNode;
        }

        public void UpdateSelectedNode(Rect selectionRect)
        {
            ClearSelectedNode();

            for (int i = 0; i < CurrentNodeList.Count; i++)
            {
                NodeEditorView nodeView = CurrentNodeList[i];
                if (nodeView.IsContainInRect(selectionRect))
                {
                    nodeView.isSelected = true;
                    selectedNodeList.Add(nodeView);
                }
            }

//            Debug.Log("select node count: " + selectedNodeList.Count);
        }

        public void ClearSelectedNode()
        {
            for (int i = 0; i < selectedNodeList.Count; i++)
            {
                selectedNodeList[i].isSelected = false;
            }

            selectedNodeList.Clear();
        }

        public GraphVariableInfo GetGraphVariableInfo(int varibleId)
        {
            if (graphVariableInfoList == null)
            {
                return null;
            }

            for (int i = 0; i < graphVariableInfoList.Count; i++)
            {
                if (graphVariableInfoList[i].id == varibleId)
                {
                    return graphVariableInfoList[i];
                }
            }

            return null;
        }

        public void OnGraphVariableListChange()
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].OnGraphVariableListChange();
            }
        }

        public void OnLoadFinish()
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].OnLoadFinish();
            }
        }

        /// <summary>
        /// 检查所有节点Input port和output port连接的合法性
        /// </summary>
        public void CheckAllNodeConnection()
        {
            for (int i = 0; i < nodeList.Count; i++)
            {
                nodeList[i].CheckIOPortConnectionValidate();
            }

        }
    }
}