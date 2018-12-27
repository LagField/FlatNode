using UnityEngine;

namespace FlatNode.Runtime
{
	[GraphNode("打印日志","Debug/Log","输出到Unity日志")]
	[NodeFlowOutPortDefine(0,"结束")]
	public class LogNode : NodeBase
	{
		public enum LogType
		{
			Log,
			Warnning,
			Error
		}

		[NodeInputPort(0,"输出日志等级")]
		public NodeInputVariable<LogType> logTypeInputVariable;
		[NodeInputPort(1,"输出日志内容")]
		public NodeInputVariable<string> logStringInputVariable;
		
		public override void OnEnter()
		{
			base.OnEnter();

			LogType logType = GetInputValue(logTypeInputVariable);
			string logString = GetInputValue(logStringInputVariable);

			switch (logType)
			{
				case LogType.Log:
					Debug.Log(logString);
					break;
				case LogType.Warnning:
					Debug.LogWarning(logString);
					break;
				case LogType.Error:
					Debug.LogError(logString);
					break;
			}
			
			Finish();
		}
	}

}

