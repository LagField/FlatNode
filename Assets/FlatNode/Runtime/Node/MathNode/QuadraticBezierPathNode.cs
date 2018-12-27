using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Runtime
{
	/// <summary>
	/// 计算贝塞尔曲线路径点
	/// </summary>
	[SkillNode("计算曲线路径点","Math/QuadraticBezierPath","计算贝塞尔曲线(Quadratic)的路径点")]
	[NodeFlowOutPortDefine(0,"计算完成")]
	public class QuadraticBezierPathNode : NodeBase
	{
		[NodeInputPort(0,"开始位置")]
		public NodeInputVariable<Vector3> startPosInput;
		[NodeInputPort(1,"结束位置")]
		public NodeInputVariable<Vector3> endPosInput;
		[NodeInputPort(2,"中间点高度偏移")]
		public NodeInputVariable<float> centerHeightOffsetInput;
		[NodeInputPort(3,"每一段路径的长度","是指结果路径点中，相邻两点的距离")]
		public NodeInputVariable<float> spanLengthInput;

		NodeVariable<List<Vector3>> pathResultVariable = new NodeVariable<List<Vector3>>();
		private const int samplePointCount = 20;
		private List<Vector3> samplePointList;

		public override void OnEnter()
		{
			base.OnEnter();
			
			CalculatePathPoint();
			
			EventTiming(0);
			Finish();
		}

		[NodeOutputPort(0,typeof(List<Vector3>),"路径点")]
		public NodeVariable GetPathResultPoints()
		{
			return pathResultVariable;
		}

		void CalculatePathPoint()
		{
			if (samplePointList == null)
			{
				samplePointList = new List<Vector3>();
			}
			samplePointList.Clear();
			
			if (pathResultVariable.value == null)
			{
				pathResultVariable.value = new List<Vector3>();
			}

			List<Vector3> resultList = pathResultVariable.value;
			resultList.Clear();
			
			Vector3 startPos = GetInputValue(startPosInput);
			Vector3 endPos = GetInputValue(endPosInput);

			float yOffset = GetInputValue(centerHeightOffsetInput);
			Vector3 controlPoint = (startPos + endPos) * 0.5f;
			float height = startPos.y > endPos.y ? startPos.y : endPos.y;
			controlPoint.y = yOffset + height;
			
			//包含起点和终点,总数量是samplePointCount+2
			for (int i = 0; i <= samplePointCount; i++)
			{
				float t = (float)i / samplePointCount;

				Vector3 samplePoint = (1 - t) * (1 - t) * startPos + 2 * (1 - t) * t * controlPoint + t * t * endPos;
				
				samplePointList.Add(samplePoint);
			}

			float spanLength = GetInputValue(spanLengthInput);
			float remainedSpanLength = spanLength;
			Vector3 previousPoint = samplePointList[0];
			//因为bezier曲线的点分布不是和t一样线性分布的，所以这里需要重新布置一下路径点位置
			for (int i = 0; i < samplePointList.Count; i++)
			{
				Vector3 nextPoint = samplePointList[i];

				float twoPointsSpan = (nextPoint - previousPoint).magnitude;
				if (remainedSpanLength - twoPointsSpan < 0)
				{
					Vector3 resultPoint = previousPoint + (nextPoint - previousPoint).normalized * remainedSpanLength;
					resultList.Add(resultPoint);
					
					previousPoint = resultPoint;
					remainedSpanLength = spanLength;
					i--;
				}
				else
				{
					remainedSpanLength -= twoPointsSpan;
					previousPoint = nextPoint;

					//如果是最后一个点
					if (i == samplePointList.Count - 1)
					{
						resultList.Add(nextPoint);
					}
				}
			}
		}
	}
}
