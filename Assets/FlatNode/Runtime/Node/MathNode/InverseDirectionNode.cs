using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
	[SkillNode("反转方向","Math/InverseDirection",hasFlowIn = false)]
	public class InverseDirectionNode : NodeBase
	{
		[NodeInputPort(0,"输入方向")]
		public NodeInputVariable<Vector3> directionInput;

		private NodeVariable<Vector3> inverseDirectionVariable;

		[NodeOutputPort(0,typeof(Vector3),"反方向向量")]
		public NodeVariable GetInverseDirection()
		{
			Vector3 originDirection = GetInputValue(directionInput);

			if (inverseDirectionVariable == null)
			{
				inverseDirectionVariable = new NodeVariable<Vector3>();
			}

			inverseDirectionVariable.value = -originDirection;
			return inverseDirectionVariable;
		}
	}
}

