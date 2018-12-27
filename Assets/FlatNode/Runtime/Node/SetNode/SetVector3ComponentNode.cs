using UnityEngine;

namespace FlatNode.Runtime
{
    public enum VectorComponent
    {
        X = 0,
        Y = 1,
        Z = 2
    }
    [GraphNode("设置Vector3分量","Set/SetVector3Component",hasFlowIn = false)]
    public class SetVector3ComponentNode : NodeBase
    {
        [NodeInputPort(0,"v3")]
        public NodeInputVariable<Vector3> originV3Input;
        [NodeInputPort(1,"分量")]
        public NodeInputVariable<VectorComponent> targetComponentInput;
        [NodeInputPort(2, "修改值")] 
        public NodeInputVariable<float> valueInput;

        private NodeVariable<Vector3> resultV3Variable;
        
        [NodeOutputPort(0,typeof(Vector3),"result")]
        public NodeVariable GetVector3Result()
        {
            if (resultV3Variable == null)
            {
                resultV3Variable = new NodeVariable<Vector3>();
            }
            
            VectorComponent targetComponent = GetInputValue(targetComponentInput);

            Vector3 originVector3 = GetInputValue(originV3Input);
            float targetValue = GetInputValue(valueInput);
            
            switch (targetComponent)
            {
                case VectorComponent.X:
                    originVector3.x = targetValue;
                    break;
                case VectorComponent.Y:
                    originVector3.y = targetValue;
                    break;
                case VectorComponent.Z:
                    originVector3.z = targetValue;
                    break;
            }

            resultV3Variable.value = originVector3;
            return resultV3Variable;
        }
    }

}

