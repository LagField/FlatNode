using UnityEngine;

namespace FlatNode.Runtime
{
	[GraphNode("向量Vector3加减","Math/VectorAdd","a 加/减 b",hasFlowIn = false)]
	public class VectorAddNode : NodeBase
	{
		public enum CalculateType
		{
			Add = 0,
			Minus = 1,
		}

		private CalculateType calculateType;

		[NodeInputPort(0,"向量a")]
		public NodeInputVariable<Vector3> aVectorInput;
		[NodeInputPort(1,"向量b")]
		public NodeInputVariable<Vector3> bVectorInput;
		[NodeInputPort(2,"计算方式")]
		public NodeInputVariable<CalculateType> typeInput;

		private NodeVariable<Vector3> resultVectorVariab;

		[NodeOutputPort(0,typeof(Vector3),"计算结果")]
		public NodeVariable GetResult()
		{
			if (resultVectorVariab == null)
			{
				resultVectorVariab = new NodeVariable<Vector3>();
			}
			
			calculateType = GetInputValue(typeInput);
			Vector3 a = GetInputValue(aVectorInput);
			Vector3 b = GetInputValue(bVectorInput);
			
			switch (calculateType)
			{
				case CalculateType.Add:
					resultVectorVariab.value = a + b;
					break;
				case CalculateType.Minus:
					resultVectorVariab.value = a - b;
					break;
			}

			return resultVectorVariab;
		}
	}

}

