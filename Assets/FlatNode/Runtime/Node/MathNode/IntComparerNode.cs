namespace FlatNode.Runtime
{
	public enum  CompareType
	{
		Less = 0,// <
		LessEqual = 1,// <=
		Equal = 2, // ==
		GreaterEqual = 3,// >=
		Greater = 4,// >
	}
	
	[SkillNode("int比较","Math/IntComparer","a compare b",hasFlowIn = false)]
	public class IntComparerNode : NodeBase
	{
		[NodeInputPort(0,"a")]
		public NodeInputVariable<int> aInput;
		[NodeInputPort(1,"b")]
		public NodeInputVariable<int> bInput;
		[NodeInputPort(2,"比较方式","a compare b")]
		public NodeInputVariable<CompareType> compareTypeInput;

		private NodeVariable<bool> resultVariable;
		
		[NodeOutputPort(0,typeof(bool),"结果")]
		public NodeVariable GetCompareResult()
		{
			if (resultVariable == null)
			{
				resultVariable = new NodeVariable<bool>();
			}

			int a = GetInputValue(aInput);
			int b = GetInputValue(bInput);
			CompareType compareType = GetInputValue(compareTypeInput);
			switch (compareType)
			{
				case CompareType.Less:
					resultVariable.value = a < b;
					break;
				case CompareType.LessEqual:
					resultVariable.value = a <= b;
					break;
				case CompareType.Equal:
					resultVariable.value = a == b;
					break;
				case CompareType.GreaterEqual:
					resultVariable.value = a >= b;
					break;
				case CompareType.Greater:
					resultVariable.value = a > b;
					break;
			}

			return resultVariable;
		}
	}

}

