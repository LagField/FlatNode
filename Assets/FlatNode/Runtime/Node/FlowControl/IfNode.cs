namespace FlatNode.Runtime
{
	[GraphNode("if","FlowControl/if")]
	[NodeFlowOutPortDefine(0,"true")]
	[NodeFlowOutPortDefine(1,"false")]
	public class IfNode : NodeBase
	{
		[NodeInputPort(0,"判断Bool")]
		public NodeInputVariable<bool> boolInput;

		public override void OnEnter()
		{
			base.OnEnter();

			bool boolValue = GetInputValue(boolInput);

			EventTiming(boolValue ? 0 : 1);
			Finish();
		}
	}

}

