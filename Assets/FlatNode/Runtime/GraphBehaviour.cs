using System.Collections.Generic;
using FlatBuffers;
using FlatNode.Runtime.Flat;
using UnityEngine;

namespace FlatNode.Runtime
{
    /// <summary>
    /// 行为图的运行类
    /// </summary>
    public class GraphBehaviour
    {
        public int graphId;

        /*
         * 当添加了新的入口节点时，在这里添加对应的实例供外界调用
         */
        private OnClickNode onClickNode;
        private OnJoyStickDragNode onJoyStickDragNode;
        private OnJoyStickDownNode onJoyStickDownNode;
        private OnJoyStickUpNode onJoyStickUpNode;

        private NodeSequence onClickSequence;
        private NodeSequence onJoyStickDragSequence;
        private NodeSequence onJoyStickDownSequence;
        private NodeSequence onJoyStickUpSequence;

        private List<NodeSequence> allEntranceSequenceList;

        //当前图的变量存储字典
        Dictionary<int, NodeVariable> variableDictionary;

        protected List<NodeSequence> runningSequenceList;

        /// <summary>
        /// 存储了行为图的序列化信息，每一种行为图只对应一个byteBuffer实例
        /// </summary>
        protected ByteBuffer byteBuffer;

        /// <summary>
        /// 每一个节点都有一个实例在这里，做为行为图序列树顶部公共使用的节点
        /// </summary>
        protected Dictionary<int, NodeBase> commonNodeDictionary;

        public GraphBehaviour(ByteBuffer byteBuffer)
        {
            this.byteBuffer = byteBuffer;
            GraphInfo graphInfo = GraphInfo.GetRootAsGraphInfo(byteBuffer);
            graphId = graphInfo.GraphId;
            InitCommonNodes(graphInfo);
            InitGraphVariables(graphInfo);
            runningSequenceList = new List<NodeSequence>();
            allEntranceSequenceList = new List<NodeSequence>();
        }

        public NodeBase DeserializeNode(int nodeId)
        {
            return FlatNodeUtility.DeserializeNode(byteBuffer, nodeId);
        }

        public void Init()
        {
            //初始化入口节点
            GraphInfo graphInfo = GraphInfo.GetRootAsGraphInfo(byteBuffer);
            int entranceNodeCount = graphInfo.EntranceNodeIdsLength;
            for (int i = 0; i < entranceNodeCount; i++)
            {
                int nodeId = graphInfo.EntranceNodeIds(i);
                EntranceNodeBase entranceNode = DeserializeNode(nodeId) as EntranceNodeBase;
                if (entranceNode == null)
                {
                    Debug.LogError(string.Format("节点 {0} 不是入口节点", nodeId));
                    continue;
                }

                if (onClickNode == null)
                {
                    onClickNode = entranceNode as OnClickNode;
                }

                if (onJoyStickDragNode == null)
                {
                    onJoyStickDragNode = entranceNode as OnJoyStickDragNode;
                }

                if (onJoyStickDownNode == null)
                {
                    onJoyStickDownNode = entranceNode as OnJoyStickDownNode;
                }

                if (onJoyStickUpNode == null)
                {
                    onJoyStickUpNode = entranceNode as OnJoyStickUpNode;
                }
            }
        }

        void InitCommonNodes(GraphInfo graphInfo)
        {
            commonNodeDictionary = new Dictionary<int, NodeBase>();

            int nodeCount = graphInfo.CommonNodeIdsLength;
            for (int i = 0; i < nodeCount; i++)
            {
                int nodeId = graphInfo.CommonNodeIds(i);

                NodeBase node = DeserializeNode(nodeId);
                node.isCommonNode = true;
                node.SetGraphBehaviour(this);
                commonNodeDictionary.Add(nodeId, node);
            }
        }

        private void InitGraphVariables(GraphInfo graphInfo)
        {
            variableDictionary = FlatNodeUtility.DeserializeGraphVariableInfo(graphInfo);
        }

        public void Update(float deltaTime)
        {
            for (int i = runningSequenceList.Count - 1; i >= 0; i--)
            {
                runningSequenceList[i].Update(deltaTime);

                if (runningSequenceList[i].IsFinish)
                {
                    runningSequenceList.RemoveAt(i);
                }
            }
        }

        public NodeBase GetCommonNode(int nodeId)
        {
            NodeBase node;
            if (commonNodeDictionary.TryGetValue(nodeId, out node))
            {
                return node;
            }

            return null;
        }

        /// <summary>
        /// 停止整个图当中，对应id的节点运行，并调用OnExit()
        /// </summary>
        /// <param name="nodeIdList"></param>
        public void StopNodes(List<int> nodeIdList)
        {
            if (nodeIdList == null || nodeIdList.Count == 0)
            {
                return;
            }

            for (int i = 0; i < runningSequenceList.Count; i++)
            {
                NodeSequence nodeSequence = runningSequenceList[i];

                nodeSequence.StopNodes(nodeIdList);
            }
        }

        /// <summary>
        /// 停止整个图，所有运行中的节点停掉并调用OnExit()
        /// </summary>
        public void StopGraph()
        {
            for (int i = 0; i < runningSequenceList.Count; i++)
            {
                NodeSequence nodeSequence = runningSequenceList[i];

                nodeSequence.StopAllRunningNodes();
            }
        }


        public bool IsFinish
        {
            get { return runningSequenceList.Count == 0; }
        }

        public virtual void Reset()
        {
            for (int i = 0; i < runningSequenceList.Count; i++)
            {
                runningSequenceList[i].Reset();
            }

            runningSequenceList.Clear();
            
            for (int i = 0; i < allEntranceSequenceList.Count; i++)
            {
                allEntranceSequenceList[i].Reset();
            }
        }

        public virtual void Dispose()
        {
            for (int i = 0; i < runningSequenceList.Count; i++)
            {
                runningSequenceList[i].Dispose();
            }

            runningSequenceList.Clear();
            
            for (int i = 0; i < allEntranceSequenceList.Count; i++)
            {
                allEntranceSequenceList[i].Dispose();
            }
        }

        public NodeVariable GetGraphVariable(int variableId)
        {
            NodeVariable variable;
            if (variableDictionary.TryGetValue(variableId, out variable))
            {
                return variable;
            }

            return null;
        }

        public void SetGraphVariable(int variableId, NodeVariable value)
        {
            if (variableDictionary.ContainsKey(variableId))
            {
                variableDictionary[variableId] = value;
            }
            else
            {
                variableDictionary.Add(variableId, value);
            }
        }

        #region  所有入口节点API
        public void OnClick()
        {
            if (onClickNode == null)
            {
                return;
            }

            if (onClickSequence == null)
            {
                onClickSequence = new NodeSequence(onClickNode, onClickNode.rightSideNodeIds, this, null);
                allEntranceSequenceList.Add(onClickSequence);
            }

            if (onClickSequence.IsFinish)
            {
                runningSequenceList.Add(onClickSequence);
                onClickSequence.Start();
            }
        }

        public void OnJoyStickDown()
        {
            if (onJoyStickDownNode == null)
            {
                return;
            }

            if (onJoyStickDownSequence == null)
            {
                onJoyStickDownSequence = new NodeSequence(onJoyStickDownNode, onJoyStickDownNode.rightSideNodeIds, this, null);
                allEntranceSequenceList.Add(onJoyStickDownSequence);
            }

            if (onJoyStickDownSequence.IsFinish)
            {
                runningSequenceList.Add(onJoyStickDownSequence);

                onJoyStickDownSequence.Start();
            }
        }

        public void OnJoyStickDrag(Vector2 joyStickInput)
        {
            if (onJoyStickDragNode == null)
            {
                return;
            }

            if (onJoyStickDragSequence == null)
            {
                onJoyStickDragSequence = new NodeSequence(onJoyStickDragNode, onJoyStickDragNode.rightSideNodeIds, this, null);
                allEntranceSequenceList.Add(onJoyStickDragSequence);
            }

            if (onJoyStickDragSequence.IsFinish)
            {
                runningSequenceList.Add(onJoyStickDragSequence);

                onJoyStickDragNode.joyStickInput = joyStickInput;
                onJoyStickDragSequence.Start();
            }
            else
            {
                //只更新
                onJoyStickDragNode.joyStickInput = joyStickInput;
            }
        }

        public void OnJoyStickUp(Vector2 joyStickInput)
        {
            if (onJoyStickUpNode == null)
            {
                return;
            }

            if (onJoyStickUpSequence == null)
            {
                onJoyStickUpSequence = new NodeSequence(onJoyStickUpNode, onJoyStickUpNode.rightSideNodeIds, this, null);
                allEntranceSequenceList.Add(onJoyStickUpSequence);
            }

            if (onJoyStickUpSequence.IsFinish)
            {
                runningSequenceList.Add(onJoyStickUpSequence);

                onJoyStickUpNode.joyStickInput = joyStickInput;
                onJoyStickUpSequence.Start();
            }
        }
        #endregion

    }
}