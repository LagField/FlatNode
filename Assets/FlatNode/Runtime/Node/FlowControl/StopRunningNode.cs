using System.Collections.Generic;

namespace FlatNode.Runtime
{
    [SkillNode("停止运行节点","FlowControl/StopRunningNodes","停止整个技能当中特定的节点运行")]
    [NodeFlowOutPortDefine(0,"完成")]
    public class StopRunningNode : NodeBase
    {
        [NodeInputPort(0, "要停止节点的id")] public NodeInputVariable<List<int>> stopNodeIdsInput;

        public override void OnEnter()
        {
            base.OnEnter();

            List<int> nodeIdList = GetInputValue(stopNodeIdsInput);
            skillSequence.GraphBehaviourBase.StopNodes(nodeIdList);
            
            EventTiming(0);
            Finish();
        }
    }
}
