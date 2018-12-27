using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
    [SkillNode("OnJoyStickDrag", "EntranceNodes/OnJoyStickDrag", "摇杆拖拽", isEntranceNode = true)]
    [NodeFlowOutPortDefine(0, "拖拽摇杆时")]
    public class OnJoyStickDragNode : EntranceNodeBase
    {
        public Vector2 joyStickInput;

        private NodeVariable<Vector2> joyStickInputVariable;

        public override void OnEnter()
        {
            base.OnEnter();
            
            EventTiming(0);
            Finish();
        }

        [NodeOutputPort(0,typeof(Vector2),"摇杆输入")]
        public NodeVariable GetJoyStickInput()
        {
            if (joyStickInputVariable == null)
            {
                joyStickInputVariable = new NodeVariable<Vector2>();
            }

            joyStickInputVariable.value = joyStickInput;
            return joyStickInputVariable;
        }
    }
}