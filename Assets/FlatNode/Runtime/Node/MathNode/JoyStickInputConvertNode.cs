using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
	[GraphNode("转换摇杆输入","Math/JoyStickInputConvert","摇杆的输入是一个Vector2，将该v2的x和y分别放入vector3的x和z",hasFlowIn = false)]
	public class JoyStickInputConvertNode : NodeBase
	{
		[NodeInputPort(0,"Vector2")]
		public NodeInputVariable<Vector2> vector2Input;

		NodeVariable<Vector3> convertV3Variable = new NodeVariable<Vector3>();
		
		[NodeOutputPort(0,typeof(Vector3),"Vector3")]
		public NodeVariable GetConvertedVector3()
		{
			Vector2 v2 = GetInputValue(vector2Input);
			Vector3 v3 = new Vector3(v2.x,0,v2.y);
			convertV3Variable.value = v3;
			return convertV3Variable;
		}
	}
}

