using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
	/// <summary>
	/// 这个节点每次在执行子节点时，会创建一个新的Nodesequence或者执行之前执行完成的Nodesequence
	/// </summary>
	[GraphNode("CreateSequence","CreateSequence","这个节点每次在执行子节点时，会执行之前执行完成的NodeSequence或者创建一个新的NodeSequence")]
	[NodeFlowOutPortDefine(0,"创建序列")]
	public class CreateSequenceNode : NodeBase
	{
		[NodeInputPort(0, "初始化数量")] public NodeInputVariable<int> initCountInputVariable;
		
		public int[] rightSideNodeIds;

		public override void OnEnter()
		{
			base.OnEnter();
			
			EventTiming(0);
			Finish();
		}
	}
}
