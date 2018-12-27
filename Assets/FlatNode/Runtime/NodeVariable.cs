using System;

namespace FlatNode.Runtime
{
    [Serializable]
    public abstract class NodeVariable 
    {
        

    }

    [Serializable]
    public class NodeVariable<T> : NodeVariable
    {
        public T value;

        public NodeVariable()
        {
        }

        public NodeVariable(T value)
        {
            this.value = value;
        }
    }

    [Serializable]
    public class NodeInputVariable<T>
    {
        public int targetNodeId = -1;
        public int targetPortId = -1;

        public T value;
    }
}

