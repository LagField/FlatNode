using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
    /// <summary>
    /// 节点序列
    /// </summary>
    public class NodeSequence
    {
        public NodeBase RootNode { get; private set; }
        protected Dictionary<int, NodeBase> sequenceNodeDictionary;

        public GraphBehaviour GraphBehaviour { get; private set; }
        private NodeSequence parentSequence;

        protected List<NodeBase> runningNodeList;
        protected List<NodeBase> executeFinishNodeList;

        protected List<NodeSequence> runningChildSequenceList;
        protected Dictionary<int, Queue<NodeSequence>> childSequencePool;

        public bool IsFinish
        {
            get
            {
                if (runningChildSequenceList != null)
                {
                    for (int i = 0; i < runningChildSequenceList.Count; i++)
                    {
                        if (!runningChildSequenceList[i].IsFinish)
                        {
                            return false;
                        }
                    }
                }

                return runningNodeList.Count == 0;
            }
        }

        /// <summary>
        /// 创建NodeSequence,会重新创建一个rootNode实例
        /// </summary>
        /// <param name="rootNodeId"></param>
        /// <param name="sequenceNodeIds"></param>
        /// <param name="graphBehaviour"></param>
        /// <param name="parentSequence"></param>
        public NodeSequence(int rootNodeId, int[] sequenceNodeIds, GraphBehaviour graphBehaviour, NodeSequence parentSequence)
        {
            GraphBehaviour = graphBehaviour;
            RootNode = graphBehaviour.DeserializeNode(rootNodeId);
            this.parentSequence = parentSequence;
            
            RootNode.SetNodeSequence(this);
            RootNode.SetGraphBehaviour(graphBehaviour);

            sequenceNodeDictionary = new Dictionary<int, NodeBase>();
            sequenceNodeDictionary.Add(RootNode.nodeId,RootNode);
            for (int i = 0; i < sequenceNodeIds.Length; i++)
            {
                NodeBase sequenceNode = graphBehaviour.DeserializeNode(sequenceNodeIds[i]);
                sequenceNode.SetNodeSequence(this);
                sequenceNode.SetGraphBehaviour(graphBehaviour);
                sequenceNodeDictionary.Add(sequenceNodeIds[i], sequenceNode);
            }

            runningNodeList = new List<NodeBase>();
            executeFinishNodeList = new List<NodeBase>();

            runningChildSequenceList = new List<NodeSequence>();
        }
        
        /// <summary>
        /// 创建NodeSequence,会共用传入的rootNode
        /// </summary>
        /// <param name="rootNode"></param>
        /// <param name="sequenceNodeIds"></param>
        /// <param name="graphBehaviour"></param>
        /// <param name="parentSequence"></param>
        public NodeSequence(NodeBase rootNode, int[] sequenceNodeIds, GraphBehaviour graphBehaviour, NodeSequence parentSequence)
        {
            GraphBehaviour = graphBehaviour;
            RootNode = rootNode;
            this.parentSequence = parentSequence;
            
            RootNode.SetGraphBehaviour(graphBehaviour);
            RootNode.SetNodeSequence(this);

            sequenceNodeDictionary = new Dictionary<int, NodeBase>();
            sequenceNodeDictionary.Add(RootNode.nodeId,RootNode);
            for (int i = 0; i < sequenceNodeIds.Length; i++)
            {
                NodeBase sequenceNode = graphBehaviour.DeserializeNode(sequenceNodeIds[i]);
                sequenceNode.SetNodeSequence(this);
                sequenceNode.SetGraphBehaviour(graphBehaviour);
                sequenceNodeDictionary.Add(sequenceNodeIds[i], sequenceNode);
            }

            runningNodeList = new List<NodeBase>();
            executeFinishNodeList = new List<NodeBase>();

            runningChildSequenceList = new List<NodeSequence>();
        }

        public void Start()
        {
            if (RootNode == null)
            {
                return;
            }

            runningNodeList.Clear();
            executeFinishNodeList.Clear();

            //root节点必须不能是走update的
            RootNode.OnEnter();
        }

        public void Update(float deltaTime)
        {
            if (executeFinishNodeList.Count > 0)
            {
                for (int i = 0; i < executeFinishNodeList.Count; i++)
                {
                    NodeBase executeFinishNode = executeFinishNodeList[i];
                    executeFinishNode.OnExit();

                    if (runningNodeList.Contains(executeFinishNode))
                    {
                        runningNodeList.Remove(executeFinishNode);
                    }
                }

                executeFinishNodeList.Clear();
            }

            for (int i = 0; i < runningNodeList.Count; i++)
            {
                runningNodeList[i].OnUpdate(deltaTime);
            }

            if (runningChildSequenceList != null && runningChildSequenceList.Count > 0)
            {
                for (int i = runningChildSequenceList.Count - 1; i >= 0; i--)
                {
                    NodeSequence nodeSequence = runningChildSequenceList[i];
                    nodeSequence.Update(deltaTime);

                    if (nodeSequence.IsFinish)
                    {
                        nodeSequence.Reset();

                        if (childSequencePool == null)
                        {
                            childSequencePool = new Dictionary<int, Queue<NodeSequence>>();
                        }
                        
                        //放回池中
                        if (childSequencePool.ContainsKey(nodeSequence.RootNode.nodeId))
                        {
                            Queue<NodeSequence> poolQueue = childSequencePool[nodeSequence.RootNode.nodeId];
                            if (poolQueue == null)
                            {
                                poolQueue = new Queue<NodeSequence>();
                            }

                            poolQueue.Enqueue(nodeSequence);
                        }
                        else
                        {
                            Queue<NodeSequence> poolQueue = new Queue<NodeSequence>();
                            poolQueue.Enqueue(nodeSequence);
                            childSequencePool.Add(nodeSequence.RootNode.nodeId, poolQueue);
                        }

                        runningChildSequenceList.RemoveAt(i);
                    }
                }
            }
        }

        public void Reset()
        {
            executeFinishNodeList.Clear();
            runningNodeList.Clear();
            using (var enumerator = sequenceNodeDictionary.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.OnReset();
                }
            }
        }

        public void Dispose()
        {
            executeFinishNodeList.Clear();
            runningNodeList.Clear();
            using (var enumerator = sequenceNodeDictionary.GetEnumerator())
            {
                while (enumerator.MoveNext())
                {
                    enumerator.Current.Value.OnDispose();
                }
            }
        }

        public void FlowToNode(int nodeId)
        {
            //流向下一个节点，下一个节点必须在这个技能序列内
            NodeBase targetNode = GetNode(nodeId);
            if (targetNode == null)
            {
                return;
            }

#if UNITY_EDITOR
            if (runningNodeList.Contains(targetNode))
            {
                Debug.LogErrorFormat("节点{0}已经在运行了，属于重复运行。多实例运行要使用CreateSequence节点", nodeId);
            }
#endif

            //进入创建序列流程
            if (targetNode.isCreateSequenceNode)
            {
                if (childSequencePool == null)
                {
                    childSequencePool = new Dictionary<int, Queue<NodeSequence>>();
                }

                bool needCreateNewSequence = true;
                //尝试从池中取
                if (childSequencePool.ContainsKey(nodeId))
                {
                    Queue<NodeSequence> poolSequence = childSequencePool[nodeId];
                    if (poolSequence != null && poolSequence.Count > 0)
                    {
//                        Debug.Log("cache hit");
                        NodeSequence nodeSequence = poolSequence.Dequeue();
                        nodeSequence.Reset();
                        runningChildSequenceList.Add(nodeSequence);
                        nodeSequence.Start();

                        needCreateNewSequence = false;
                    }
                }

                if (needCreateNewSequence)
                {
                    CreateSequenceNode createSequenceNode = targetNode as CreateSequenceNode;
                    NodeSequence newNodeSequence = new NodeSequence(nodeId, createSequenceNode.rightSideNodeIds, GraphBehaviour, this);

                    runningChildSequenceList.Add(newNodeSequence);
                    newNodeSequence.Start();
                }
            }
            else
            {
                targetNode.OnEnter();
                //该节点直接在OnEnter就结束了，不用加入runningNodeList
                if (targetNode.isFinish) 
                    return;
                
                if (!runningNodeList.Contains(targetNode))
                {
                    runningNodeList.Add(targetNode);
                }
            }
        }

        public NodeBase GetNode(int nodeId)
        {
            NodeBase result;
            if (sequenceNodeDictionary.TryGetValue(nodeId, out result))
            {
                return result;
            }

            return null;
        }

        public NodeBase GetNodeAlongThisAndParentSequeces(int nodeId)
        {
            NodeBase targetNode = null;
            //优先查找技能序列树中的节点
            NodeSequence sequence = this;
            while (sequence != null)
            {
                targetNode = sequence.GetNode(nodeId);

                if (targetNode != null)
                {
                    return targetNode;
                }

                sequence = sequence.parentSequence;
            }
            
            //查找公共节点
            targetNode = GraphBehaviour.GetCommonNode(nodeId);

            if (targetNode == null)
            {
                Debug.LogErrorFormat("整个技能中找不到id为 {0} 的节点", nodeId);
            }
            return targetNode;
        }

        public void NodeExecuteFinish(NodeBase node)
        {
#if UNITY_EDITOR
            if (!sequenceNodeDictionary.ContainsKey(node.nodeId))
            {
                Debug.LogErrorFormat("nodesequence中不包含nodeid {0} 的节点", node.nodeId);
                return;
            }

            if (executeFinishNodeList.Contains(node))
            {
                Debug.LogErrorFormat("nodeid {0} 节点单次循环中多次调用NodeExecuteFinish方法", node.nodeId);
                return;
            }
#endif
            //只有在运行过一次，才需要从运行列表中删除
            if (node.isUpdated)
            {
                executeFinishNodeList.Add(node);
            }
        }

        /// <summary>
        /// 停止整个技能中对应节点的运行
        /// </summary>
        /// <param name="nodeIdList"></param>
        public void StopNodes(List<int> nodeIdList)
        {
            if (nodeIdList == null || nodeIdList.Count == 0)
            {
                return;
            }

            if (runningNodeList.Count == 0)
            {
                return;
            }

            for (int i = runningNodeList.Count - 1; i >= 0; i--)
            {
                NodeBase node = runningNodeList[i];
                for (int j = 0; j < nodeIdList.Count; j++)
                {
                    int targetId = nodeIdList[j];
                    if (node.nodeId == targetId)
                    {
                        node.OnExit();
                        runningNodeList.RemoveAt(i);
                        break;
                    }
                }   
            }

            for (int i = 0; i < runningChildSequenceList.Count ; i++)
            {
                runningChildSequenceList[i].StopNodes(nodeIdList);
            }
        }

        public void StopAllRunningNodes()
        {
            if (runningNodeList.Count == 0)
            {
                return;
            }

            for (int i = 0; i < runningNodeList.Count; i++)
            {
                runningNodeList[i].OnExit();
            }
            
            executeFinishNodeList.Clear();
            
            runningNodeList.Clear();
        }
    }
}