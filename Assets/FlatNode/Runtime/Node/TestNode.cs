using System;
using UnityEngine;
using UnityEngine.Scripting;
using Random = UnityEngine.Random;

namespace FlatNode.Runtime
{
    [GraphNode("测试用节点","Test/TestNode","用于测试的节点")]
    public class TestNode : NodeBase
    {
        [NodeInputPort(0,"类型测试")]
        public NodeInputVariable<LayerMaskWrapper> damageInputVariable;
    }
}