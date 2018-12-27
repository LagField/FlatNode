using UnityEngine;

namespace FlatNode.Runtime
{
    [GraphNode("检测按键按下","Input/CheckKeyPress")]
    [NodeFlowOutPortDefine(0,"检测到按键按下时")]
    public class CheckKeyPressNode : NodeBase 
    {
        [NodeInputPort(0,"要检测的按键")]
        public NodeInputVariable<KeyCode> checkKeyCodeInput;

        public override void OnEnter()
        {
            base.OnEnter();

            KeyCode keyCode = GetInputValue(checkKeyCodeInput);
            if (Input.GetKeyDown(keyCode))
            {
                EventTiming(0);
            }
            
            Finish();
        }
    }
}