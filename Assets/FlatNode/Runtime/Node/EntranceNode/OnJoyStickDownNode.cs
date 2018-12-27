using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
	[GraphNode("OnJoyStickDown","EntranceNodes/OnJoyStickDown","按下摇杆时",isEntranceNode = true)]
	[NodeFlowOutPortDefine(0, "按下摇杆时")]
	public class OnJoyStickDownNode : EntranceNodeBase
	{
		public Vector2 joyStickInput;
		private NodeVariable<Vector2> joyStickInputVariable;
		
		public override void OnEnter()
		{
			base.OnEnter();
            
			EventTiming(0);
			Finish();
		}
	}

}

