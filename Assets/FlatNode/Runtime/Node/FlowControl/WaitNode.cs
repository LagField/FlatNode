using UnityEngine;

namespace FlatNode.Runtime
{
    [SkillNode("等待","FlowControl/Wait")]
    [NodeFlowOutPortDefine(0,"等待结束时")]
    public class WaitNode : NodeBase
    {
        [NodeInputPort(0,"等待时长")]
        public NodeInputVariable<float> waitTimeInputVariable;

        private float timer;

        public override void OnEnter()
        {
            base.OnEnter();

            timer = GetInputValue(waitTimeInputVariable);
//            Debug.Log("wait time: " + timer);
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);
            timer -= deltaTime;
//            Debug.Log("wait " + timer);
            if (timer <= 0)
            {
                EventTiming(0);
                Finish();
            }
        }
    }
}
