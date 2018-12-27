using System;
using UnityEngine;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

namespace FlatNode.Runtime
{
    [SkillNode("测试用节点","Test/TestNode","用于测试的节点")]
    [NodeFlowOutPortDefine(2,"击中敌人时")]
    [NodeFlowOutPortDefine(1,"击中墙壁时")]
    [NodeFlowOutPortDefine(0,"子弹消失时","没有击中任何东西，超出攻击范围消失时")]
    public class TestNode : NodeBase
    {
        [NodeInputPort(0,"伤害数值","这个值在节点内不会被改变")]
        public NodeInputVariable<float> damageInputVariable;
        [NodeInputPort(7,"位移距离")]
        public NodeInputVariable<float> displacementDistanceVariable;
        [NodeInputPort(2,"显示文字")] 
        public NodeInputVariable<bool> displayTextInputVariable;
        [NodeInputPort(3,"技能方向")]
        public NodeInputVariable<Vector3> skillDirection;
        [NodeInputPort(4, "技能方向坐标系")] 
        public NodeInputVariable<Space> directionSpace;
        [NodeInputPort(5, "碰撞参数")] 
        public NodeInputVariable<CollisionDetectionMode> collsiionMode;
        [NodeInputPort(6, "子弹GameObject", "必填项")]
        public NodeInputVariable<GameObject> bulletObject;
        [NodeInputPort(1, "子弹旋转")]
        public NodeInputVariable<Quaternion> bulletRotation;

        private NodeVariable<float> damagedVariable;
        private NodeVariable<float> neutralizedDamagedVariable;
        private NodeVariable<Space> skillNameVariable;

        [NodeOutputPort(1,typeof(float),"实际造成的伤害数值","计算了所有减伤/免伤效果之后的值")]
        public NodeVariable GetDamagedVariable()
        {
            if (damagedVariable == null)
            {
                damagedVariable = new NodeVariable<float>(0f);
            }

            damagedVariable.value = Random.Range(100f, 200f);
            return damagedVariable;
        }
        
        [NodeOutputPort(0,typeof(float),"被抵消掉的伤害数值","被减伤buff抵消掉的值")]
        public NodeVariable GetNeutralizedDamagedVariable()
        {
            if (neutralizedDamagedVariable == null)
            {
                neutralizedDamagedVariable = new NodeVariable<float>(0f);
            }

            neutralizedDamagedVariable.value = Random.Range(0f, 50f);
            return neutralizedDamagedVariable;
        }
        
        [NodeOutputPort(2,typeof(Space),"测试类型","各种类型测试")]
        public NodeVariable TypeTestOutput()
        {
            if (skillNameVariable == null)
            {
                skillNameVariable = new NodeVariable<Space>();
            }

            skillNameVariable.value = Space.World;
            return skillNameVariable;
        }

    }
}