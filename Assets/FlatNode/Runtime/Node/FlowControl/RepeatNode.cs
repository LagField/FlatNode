using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
    [GraphNode("重复执行","FlowControl/Repeat")]
    [NodeFlowOutPortDefine(0,"每一次执行时")]
    [NodeFlowOutPortDefine(1,"重复执行完成时")]
    public class RepeatNode : NodeBase
    {
        [NodeInputPort(0,"重复次数")]
        public NodeInputVariable<int> repeatCountInputVariable;
        [NodeInputPort(1,"每次重复间隔")]
        public NodeInputVariable<float> repeatIntervalSecondsInputVariable;
        [NodeInputPort(2,"是否立即执行第一次","进入节点后立即执行第一次")]
        public NodeInputVariable<bool> executeImmediatelyInputVariable;

        private int remaindRepeatCount;
        private float timer;

        public override void OnEnter()
        {
            base.OnEnter();

            timer = 0f;
            remaindRepeatCount = GetInputValue(repeatCountInputVariable);

            bool isExecuteImmediately = GetInputValue(executeImmediatelyInputVariable);
            if (isExecuteImmediately)
            {
                EventTiming(0);

                remaindRepeatCount--;
                
                if (remaindRepeatCount == 0)
                {
                    EventTiming(1);
                    Finish();
                }
            }
        }

        public override void OnUpdate(float deltaTime)
        {
            base.OnUpdate(deltaTime);

            timer += deltaTime;
            float repeatIntervalSeconds = GetInputValue(repeatIntervalSecondsInputVariable);
            
            if (timer >= repeatIntervalSeconds)
            {
                timer -= repeatIntervalSeconds;
                EventTiming(0);

                remaindRepeatCount--;
                if (remaindRepeatCount <= 0)
                {
                    EventTiming(1);
                    Finish();
                }
            }
        }
    }
}
