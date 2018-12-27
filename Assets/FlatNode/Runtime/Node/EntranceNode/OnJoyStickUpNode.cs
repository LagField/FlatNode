using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
	[GraphNode("OnJoyStickUp","EntranceNodes/OnJoyStickUp","抬起摇杆时",isEntranceNode = true)]
	[NodeFlowOutPortDefine(0, "摇杆抬起时")]
	public class OnJoyStickUpNode : EntranceNodeBase
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

