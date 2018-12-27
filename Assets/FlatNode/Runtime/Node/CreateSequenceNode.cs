using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
	/// <summary>
	/// 这个节点每次在执行子节点时，会创建一个新的skillsequence或者执行之前执行完成的skillsequence
	/// </summary>
	[SkillNode("CreateSequence","CreateSequence","这个节点每次在执行子节点时，会执行之前执行完成的skillsequence或者创建一个新的skillsequence")]
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
