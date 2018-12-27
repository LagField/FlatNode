using System;
using UnityEngine;

namespace FlatNode.Runtime
{
    public abstract class NodeBase
    {
        public int nodeId;
        public int[][] flowOutTargetNodeId;
        public Func<NodeVariable>[] outputPortFuncs;

        protected GraphBehaviourBase graphBehaviourBase;
        protected SkillSequence skillSequence;

        /// <summary>
        /// 看该节点是否是<see cref="CreateSequenceNode"/>类型，避免运行时频繁类型检查
        /// </summary>
        public bool isCreateSequenceNode;

        /// <summary>
        /// 是否是公共节点,公共节点不属于任何sequence，公共节点一般是只用于取值的节点，无法从流入接口进入到该节点的OnEnter
        /// </summary>
        public bool isCommonNode;

        /// <summary>
        /// 该节点是否调用了finish()方法结束
        /// </summary>
        public bool isFinish;

        /// <summary>
        /// 该节点是否update过，true时表示该节点在SkillSequence的runningNodeList里面
        /// </summary>
        public bool isUpdated;

        public virtual void OnEnter()
        {
            isFinish = false;
            isUpdated = false;
        }

        public virtual void OnExit()
        {
        }

        public virtual void OnUpdate(float deltaTime)
        {
            isUpdated = true;
        }

        protected void Finish()
        {
            isFinish = true;
            skillSequence.NodeExecuteFinish(this);
        }

        public virtual void OnReset()
        {

        }

        public virtual void OnDispose()
        {

        }

        public void SetSkillSequence(SkillSequence skillSequence)
        {
            this.skillSequence = skillSequence;
        }

        public void SetSkill(GraphBehaviourBase graphBehaviourBase)
        {
            this.graphBehaviourBase = graphBehaviourBase;
        }

        public NodeVariable GetOutputPortValue(int portIndex)
        {
            if (portIndex < 0 || portIndex >= outputPortFuncs.Length)
            {
                return null;
            }

            return outputPortFuncs[portIndex]();
        }

        protected void EventTiming(int eventTimingId)
        {
#if UNITY_EDITOR
            if (eventTimingId < 0 || eventTimingId >= flowOutTargetNodeId.Length)
            {
                Debug.LogErrorFormat("EventTimingId: {0} 超出了流出端口范围！", eventTimingId);
                return;
            }
#endif

            int[] targetNodeIds = flowOutTargetNodeId[eventTimingId];
            if (targetNodeIds != null)
            {
                for (int i = 0; i < targetNodeIds.Length; i++)
                {
                    skillSequence.FlowToNode(targetNodeIds[i]);
                }
            }
        }

        protected T GetInputValue<T>(NodeInputVariable<T> inputVariable)
        {
            if (inputVariable == null)
            {
                return default(T);
            }
            
            if (inputVariable.targetNodeId < 0 && inputVariable.targetPortId < 0)
            {
                return inputVariable.value;
            }

            NodeBase targetNode = null;
            //skillSequence为null 说明是公共节点
            if (skillSequence == null)
            {
                targetNode = graphBehaviourBase.GetCommonNode(inputVariable.targetNodeId);
            }
            else
            {
                targetNode = skillSequence.GetNodeAlongThisAndParentSequeces(inputVariable.targetNodeId);
            }

            if (targetNode == null)
            {
                return default(T);
            }

            NodeVariable nodeVariable = targetNode.GetOutputPortValue(inputVariable.targetPortId);
            NodeVariable<T> resultVariable = nodeVariable as NodeVariable<T>;
            if (resultVariable == null)
            {
                Debug.LogErrorFormat("获取节点 {0} 的输出端口 {1} 的值发生错误，类型不匹配", inputVariable.targetNodeId, inputVariable.targetPortId);
                return default(T);
            }

            return resultVariable.value;
        }

        /// <summary>
        /// 仅<see cref="SetVariableNode"/>使用
        /// </summary>
        /// <param name="inputVariable"></param>
        /// <returns></returns>
        protected NodeVariable GetInputValueRaw(NodeInputVariable<NodeVariable> inputVariable)
        {
            if (inputVariable == null)
            {
                return null;
            }

            if (inputVariable.targetNodeId < 0 || inputVariable.targetPortId < 0)
            {
                Debug.LogError("GetInputValueRaw 出错，该端口必须连接到另外一个output端口");
            }
            
            NodeBase targetNode = null;
            //skillSequence为null 说明是公共节点
            if (skillSequence == null)
            {
                targetNode = graphBehaviourBase.GetCommonNode(inputVariable.targetNodeId);
            }
            else
            {
                targetNode = skillSequence.GetNodeAlongThisAndParentSequeces(inputVariable.targetNodeId);
            }
            
            if (targetNode == null)
            {
                return null;
            }
            
            NodeVariable nodeVariable = targetNode.GetOutputPortValue(inputVariable.targetPortId);

            return nodeVariable;
        }
    }
}