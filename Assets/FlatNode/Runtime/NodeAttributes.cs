using System;

namespace FlatNode.Runtime
{
    [AttributeUsage(AttributeTargets.Class)]
    public class GraphNodeAttribute : Attribute
    {
        public string nodeName;
        public string nodeMenuPath;
        /// <summary>
        /// 该节点是否有流程进入接口
        /// </summary>
        public bool hasFlowIn;
        public string nodeDescription;

        /// <summary>
        /// 是否是进入节点，进入节点忽略流入接口
        /// </summary>
        public bool isEntranceNode;

        public GraphNodeAttribute(string nodeName, string nodeMenuPath, string nodeDescription = "", bool hasFlowIn = true,bool isEntranceNode = false)
        {
            this.nodeName = nodeName;
            this.nodeMenuPath = nodeMenuPath;
            this.nodeDescription = nodeDescription;
            this.hasFlowIn = hasFlowIn;

            if (isEntranceNode)
            {
                this.hasFlowIn = false;
            }
        }
    }

    [AttributeUsage(AttributeTargets.Class,AllowMultiple = true)]
    public class NodeFlowOutPortDefineAttribute : Attribute
    {
        public int portId;
        public string portName;
        public string description;

        public NodeFlowOutPortDefineAttribute(int portId, string portName, string description = "")
        {
            this.portId = portId;
            this.portName = portName;
            this.description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Field)]
    public class NodeInputPortAttribute : Attribute
    {
        public int priority;
        public string portName;
        public string description;

        public NodeInputPortAttribute(int priority, string portName,string description = "")
        {
            this.priority = priority;
            this.portName = portName;
            this.description = description;
        }
    }

    [AttributeUsage(AttributeTargets.Method)]
    public class NodeOutputPortAttribute : Attribute
    {
        public int portId;
        public Type outputType;
        public string portName;
        public string description;

        public NodeOutputPortAttribute(int portId,Type outputType,string portName, string description = "")
        {
            this.portId = portId;
            this.outputType = outputType;
            this.portName = portName;
            this.description = description;
        }
    }
}