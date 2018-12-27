using UnityEngine;

namespace FlatNode.Runtime
{
	[SkillNode("缩放Vector","Math/VectorScalar",hasFlowIn = false)]
	public class VectorScalarNode : NodeBase
	{
		[NodeInputPort(0,"输入Vector2")]
		public NodeInputVariable<Vector2> v2Input;
		[NodeInputPort(1,"输入Vector3")]
		public NodeInputVariable<Vector3> v3Input;

		[NodeInputPort(2, "缩放量")] public NodeInputVariable<float> scalarInput;

		private NodeVariable<Vector2> scalarVector2Variable;
		private NodeVariable<Vector3> scalarVector3Variable;

		[NodeOutputPort(0,typeof(Vector2),"缩放后Vector2")]
		public NodeVariable GetScalarVector2()
		{
			float scalar = GetInputValue(scalarInput);
			if (scalarVector2Variable == null)
			{
				scalarVector2Variable = new NodeVariable<Vector2>();
			}

			Vector2 v2 = GetInputValue(v2Input);
			scalarVector2Variable.value = v2 * scalar;
			return scalarVector2Variable;
		}

		[NodeOutputPort(1,typeof(Vector3),"缩放后Vector3")]
		public NodeVariable GetScalarVector3()
		{
			float scalar = GetInputValue(scalarInput);
			if (scalarVector3Variable == null)
			{
				scalarVector3Variable = new NodeVariable<Vector3>();
			}

			Vector3 v3 = GetInputValue(v3Input);
			scalarVector3Variable.value = v3 * scalar;
			return scalarVector3Variable;
		}
	}

}
