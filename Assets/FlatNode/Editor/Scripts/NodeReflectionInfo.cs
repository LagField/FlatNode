using System;
using System.Collections.Generic;
using System.Reflection;
using FlatNode.Runtime;
using UnityEngine;

namespace FlatNode.Editor
{
    public class NodeReflectionInfo
    {
        public Type Type { get; private set; }
        private object instance;

        public GraphNodeAttribute NodeAttribute { get; private set; }

        private FieldInfo nodeIdFieldInfo;
        
        public NodeFlowOutPortDefineAttribute[] flowOutPortDefineAttributes;
        private FieldInfo flowOutTargetNodeIdFieldInfo;

        public List<InputPortReflectionInfo> inputPortInfoList;
        public List<OutputPortReflectionInfo> outputPortInfoList;

        private FieldInfo outputPortFuncsFieldInfo;
        private Func<NodeVariable>[] outputPortFuncs;

        public NodeReflectionInfo(Type type)
        {
            Type = type;

            if (!Type.IsSubclassOf(typeof(NodeBase)))
            {
                Debug.LogErrorFormat("{0} 没有继承自NodeBase类", type.Name);
                return;
            }

            //SkillNodeAttribute
            object[] attributeObjects = type.GetCustomAttributes(typeof(GraphNodeAttribute), false);
            if (attributeObjects.Length == 0)
            {
                Debug.LogErrorFormat("class {0} 不包含SkillNodeAttribute", type.Name);
                return;
            }

            NodeAttribute = attributeObjects[0] as GraphNodeAttribute;

            nodeIdFieldInfo = type.GetField("nodeId", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);

            inputPortInfoList = new List<InputPortReflectionInfo>();
            outputPortInfoList = new List<OutputPortReflectionInfo>();

            //create instance
            instance = Activator.CreateInstance(type);
            
            //flowin port -- not need

            //flowout port reflection infos
            InitFlowoutPortReflectionInfos();

            //Input Port ReflectionInfos
            InitInputPortReflectionInfos();

            //Output Port ReflectionInfos
            InitOutputPortReflectionInfos();
        }

        public object GetClassInstance()
        {
            return instance;
        }

        void InitFlowoutPortReflectionInfos()
        {
            //FlowOutPortReflectionInfos
            object[] attributeObjects = Type.GetCustomAttributes(typeof(NodeFlowOutPortDefineAttribute), false);
            if (attributeObjects.Length > 0)
            {
                flowOutPortDefineAttributes = new NodeFlowOutPortDefineAttribute[attributeObjects.Length];
                for (int i = 0; i < flowOutPortDefineAttributes.Length; i++)
                {
                    flowOutPortDefineAttributes[i] = attributeObjects[i] as NodeFlowOutPortDefineAttribute;
                }
            }

            if (flowOutPortDefineAttributes == null)
            {
                flowOutPortDefineAttributes = new NodeFlowOutPortDefineAttribute[0];
            }
            Array.Sort(flowOutPortDefineAttributes, (a, b) => a.portId - b.portId);
            //检查Portid是否是从0开始连续排列的,
            for (int i = 0; i < flowOutPortDefineAttributes.Length; i++)
            {
                if (flowOutPortDefineAttributes[i].portId == i) continue;
                Debug.LogErrorFormat("类型 {0} 上使用的NodeFlowOutPortDefineAttribute的portId没有从0开始连续，请检查",Type.Name);
                return;
            }

            flowOutTargetNodeIdFieldInfo = Type.GetField("flowOutTargetNodeId",
                BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            if (flowOutTargetNodeIdFieldInfo == null)
            {
                Debug.LogErrorFormat("类型 {0} 不包含flowoutTargetNodeId 变量，请检查", Type.Name);
                return;
            }
        }

        void InitInputPortReflectionInfos()
        {
            FieldInfo[] fieldInfos = Type.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < fieldInfos.Length; i++)
            {
                FieldInfo fieldInfo = fieldInfos[i];

                object[] attributeObjects = fieldInfo.GetCustomAttributes(typeof(NodeInputPortAttribute), false);
                if (attributeObjects.Length > 0)
                {
                    InputPortReflectionInfo inputPortReflectionInfo =
                        new InputPortReflectionInfo(attributeObjects[0] as NodeInputPortAttribute, fieldInfo, instance);
                    inputPortInfoList.Add(inputPortReflectionInfo);
                }
            }

            inputPortInfoList.Sort((a, b) => a.inputPortAttribute.priority - b.inputPortAttribute.priority);
        }

        void InitOutputPortReflectionInfos()
        {
            MethodInfo[] methodInfos = Type.GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance);
            for (int i = 0; i < methodInfos.Length; i++)
            {
                MethodInfo methodInfo = methodInfos[i];

                object[] attributeObjects = methodInfo.GetCustomAttributes(typeof(NodeOutputPortAttribute), false);
                if (attributeObjects.Length > 0)
                {
                    OutputPortReflectionInfo outputPortReflectionInfo =
                        new OutputPortReflectionInfo(attributeObjects[0] as NodeOutputPortAttribute, methodInfo, instance);
                    outputPortInfoList.Add(outputPortReflectionInfo);
                }
            }

            outputPortInfoList.Sort((a, b) => a.outputPortAttribute.portId - b.outputPortAttribute.portId);
            for (int i = 0; i < outputPortInfoList.Count; i++)
            {
                if (outputPortInfoList[i].outputPortAttribute.portId == i) continue;
                Debug.LogErrorFormat("类型 {0} 上使用的NodeOutputPortAttribute的portId没有从0开始连续，请检查",Type.Name);
                return;
            }
        }
        #region Public Properties

        public string NodeName
        {
            get
            {
                if (NodeAttribute == null)
                {
                    return "错误节点";
                }

                return NodeAttribute.nodeName;
            }
        }

        /// <summary>
        /// 是否允许有流程进入入口（也就是允许有父节点）
        /// </summary>
        public bool HasFlowInPort
        {
            get
            {
                if (NodeAttribute == null)
                {
                    return false;
                }

                return NodeAttribute.hasFlowIn && !NodeAttribute.isEntranceNode;
            }
        }

        public bool IsEntranceNode
        {
            get
            {
                if (NodeAttribute == null)
                {
                    return false;
                }

                return NodeAttribute.isEntranceNode;
            }
        }


        public bool IsCreateSequenceNode
        {
            get { return Type == typeof(CreateSequenceNode); }
        }

        #endregion
    }
}