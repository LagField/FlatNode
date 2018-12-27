using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
	[GraphNode("NormalizeVector","Math/NormalizeVector",hasFlowIn = false)]
	public class NormalizeVectorNode : NodeBase
	{
		[NodeInputPort(0,"Vector2")]
		public NodeInputVariable<Vector2> vector2Input;
		[NodeInputPort(1,"Vector3")]
		public NodeInputVariable<Vector3> vector3Input;

		private NodeVariable<Vector2> normalizedVector2Variable = new NodeVariable<Vector2>();
		private NodeVariable<Vector3> normalizedVector3Variable = new NodeVariable<Vector3>();

		[NodeOutputPort(0,typeof(Vector2),"NormalizedVector2")]
		public NodeVariable GetNormalizedVector2()
		{
			Vector2 vector2 = GetInputValue(vector2Input);
			normalizedVector2Variable.value = vector2.normalized;
			return normalizedVector2Variable;
		}
		
		[NodeOutputPort(1,typeof(Vector3),"NormalizedVector3")]
		public NodeVariable GetNormalizedVector3()
		{
			Vector3 vector3 = GetInputValue(vector3Input);
			normalizedVector3Variable.value = vector3.normalized;
			return normalizedVector3Variable;
		}
	}
}
