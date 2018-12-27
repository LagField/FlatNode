using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
    [GraphNode("RandomFloat", "Math/RandomFloat", "随机一个浮点数", hasFlowIn = false)]
    public class RandomFloatNode : NodeBase
    {
        [NodeInputPort(0, "最小", "包含")] public NodeInputVariable<float> minInputVariable;
        [NodeInputPort(1, "最大", "包含")] public NodeInputVariable<float> maxInputVariable;

        private NodeVariable<float> returnFloatVariable;

        [NodeOutputPort(0, typeof(float), "随机浮点数")]
        public NodeVariable GetRandomFloat()
        {
            if (returnFloatVariable == null)
            {
                returnFloatVariable = new NodeVariable<float>();
            }

            float min = GetInputValue(minInputVariable);
            float max = GetInputValue(maxInputVariable);

            returnFloatVariable.value = Random.Range(min, max);
            return returnFloatVariable;
        }
    }
}