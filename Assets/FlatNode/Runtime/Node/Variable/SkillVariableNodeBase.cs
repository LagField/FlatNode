namespace FlatNode.Runtime
{
    /// <summary>
    /// 通过该节点子类，可以预先输入一些变量，或者在运行时将临时变量放到技能中。
    /// </summary>
    public abstract class SkillVariableNodeBase : NodeBase
    {
        
    }

    /// <summary>
    /// 主要是方便编辑器根据类型，打开对应的编辑窗口
    /// </summary>
    public class VariableWrapper
    {
        public int variableId = -1;

        public override string ToString()
        {
            return variableId.ToString();
        }
        
        public static implicit operator VariableWrapper(int value)
        {
            return new VariableWrapper
            {
                variableId = value
            };
        }
    }
    
    /// <summary>
    /// 从技能中获取临时变量
    /// </summary>
    [SkillNode("获取变量","Variable/GetVariable","获取技能的临时变量",hasFlowIn = false)]
    public class GetVariableNode : SkillVariableNodeBase
    {
        [NodeInputPort(0,"读变量")]
        public NodeInputVariable<VariableWrapper> getVariableWrapperInput;

        [NodeOutputPort(0,typeof(NodeVariable),"输出")]
        public NodeVariable GetValue()
        {
            int variableId = GetInputValue(getVariableWrapperInput).variableId;
            return graphBehaviourBase.GetGraphVariable(variableId);
        }
    }

    /// <summary>
    /// 将临时变量写入技能
    /// </summary>
    [SkillNode("写入变量","Variable/SetVariable","将内容写入临时变量")]
    [NodeFlowOutPortDefine(0,"完成")]
    public class SetVariableNode : SkillVariableNodeBase
    {
        [NodeInputPort(0,"写变量")]
        public NodeInputVariable<VariableWrapper> setVariableWrapperInput;

        [NodeInputPort(1,"要存储的变量")]
        public NodeInputVariable<NodeVariable> valueVariableInput;

        public override void OnEnter()
        {
            base.OnEnter();

            int variableId = GetInputValue(setVariableWrapperInput).variableId;
            NodeVariable saveValue = GetInputValueRaw(valueVariableInput);
            
            graphBehaviourBase.SetGraphVariable(variableId,saveValue);
            
            EventTiming(0);
            Finish();
        }
    }
}