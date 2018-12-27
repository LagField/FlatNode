using System.Collections.Generic;
using FlatBuffers;
using FlatNode.Runtime.Flat;

namespace FlatNode.Runtime
{

    /// <summary>
    /// 图的基类
    /// 包含初始化的方法
    /// 自行扩展子类，实现图的不同运行方式 <see cref="LogicGraphBehaviour"/>
    /// </summary>
    public abstract class GraphBehaviourBase
    {
        public int graphId;
        
        //当前图的变量存储字典
        Dictionary<int,NodeVariable> variableDictionary;

        protected List<SkillSequence> runningSequenceList;

        /// <summary>
        /// 存储了技能的序列化信息，每一种技能只对应一个byteBuffer实例，实例在SkillManagerComponent里面
        /// </summary>
        protected ByteBuffer byteBuffer;

        /// <summary>
        /// 每一个节点都有一个实例在这里，做为技能序列树顶部公共使用的节点
        /// </summary>
        protected Dictionary<int, NodeBase> commonNodeDictionary;

        public GraphBehaviourBase(ByteBuffer byteBuffer)
        {
            this.byteBuffer = byteBuffer;
            GraphInfo graphInfo = GraphInfo.GetRootAsGraphInfo(byteBuffer);
            graphId = graphInfo.GraphId;
            InitCommonNodes(graphInfo);
            InitGraphVariables(graphInfo);
            runningSequenceList = new List<SkillSequence>();
        }

        public NodeBase DeserializeNode(int nodeId)
        {
            return FlatNodeUtility.DeserializeNode(byteBuffer, nodeId);
        }

        public abstract void Init();

        void InitCommonNodes(GraphInfo graphInfo)
        {
            commonNodeDictionary = new Dictionary<int, NodeBase>();

            int nodeCount = graphInfo.CommonNodeIdsLength;
            for (int i = 0; i < nodeCount; i++)
            {
                int nodeId = graphInfo.CommonNodeIds(i);
                
                NodeBase node = DeserializeNode(nodeId);
                node.isCommonNode = true;
                node.SetSkill(this);
                commonNodeDictionary.Add(nodeId, node);
            }
        }

        private void InitGraphVariables(GraphInfo graphInfo)
        {
            variableDictionary = FlatNodeUtility.DeserializeSkillVariableInfo(graphInfo);
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
                SkillSequence skillSequence = runningSequenceList[i];

                skillSequence.StopNodes(nodeIdList);
            }
        }

        /// <summary>
        /// 停止整个图，所有运行中的节点停掉并调用OnExit()
        /// </summary>
        public void StopGraph()
        {
            for (int i = 0; i < runningSequenceList.Count; i++)
            {
                SkillSequence skillSequence = runningSequenceList[i];

                skillSequence.StopAllRunningNodes();
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
        }

        public virtual void Dispose()
        {
            for (int i = 0; i < runningSequenceList.Count; i++)
            {
                runningSequenceList[i].Dispose();
            }
            
            runningSequenceList.Clear();
        }

        public NodeVariable GetGraphVariable(int variableId)
        {
            NodeVariable variable;
            if (variableDictionary.TryGetValue(variableId,out variable))
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
                variableDictionary.Add(variableId,value);
            }
        }
    }
}