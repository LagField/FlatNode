using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
	[SkillNode("Update","FlowControl/Update","该节点不会自动停止")]
	[NodeFlowOutPortDefine(0,"每一次update")]
	public class UpdateNode : NodeBase
	{
		[NodeInputPort(0,"间隔时间","如果小于等于0则每一帧执行一次")]
		public NodeInputVariable<float> intervalInput;

		private float interval;
		private float timer;

		public override void OnEnter()
		{
			base.OnEnter();

			timer = 0f;
			interval = GetInputValue(intervalInput);
			interval = Mathf.Max(0, interval);
		}

		public override void OnUpdate(float deltaTime)
		{
			base.OnUpdate(deltaTime);

			if (interval <= 0)
			{
				EventTiming(0);
			}
			else
			{
				timer += deltaTime;

				if (timer >= interval)
				{
					timer -= interval;
					EventTiming(0);
				}
			}
		}
	}
}

