using System;
using System.Collections.Generic;
using System.Reflection;
using FlatNode.Runtime;
using UnityEngine;

namespace FlatNode.Editor
{
    public class InputPortReflectionInfo
    {
        public NodeInputPortAttribute inputPortAttribute;

        private FieldInfo targetNodeIdFieldInfo;
        private FieldInfo targetPortIdFieldInfo;
        public FieldInfo nodeInputVariableFieldInfo;
        private FieldInfo inputVariableValueFieldInfo;

        public Type inputValueType;
        public object nodeInstance;

        /// <summary>
        /// 在节点中标记有<see cref="inputPortAttribute"/>的field的类的实例，类型是<see cref="NodeInputVariable{T}"/>
        /// </summary>
        public object nodeInputVariableInstance;

        public object GetInputNodeVariableValue()
        {
            return inputVariableValueFieldInfo.GetValue(nodeInputVariableInstance);
        }

        public void SetInputNodeVariableValue(object o)
        {
            inputVariableValueFieldInfo.SetValue(nodeInputVariableInstance, o);
        }

        public string GetNodeVariableValueString()
        {
            object returnValue = inputVariableValueFieldInfo.GetValue(nodeInputVariableInstance);
            if (returnValue == null)
            {
                return "";
            }
            
            if (inputValueType == typeof(List<int>))
            {
                List<int> targetList = returnValue as List<int>;
                if (targetList.Count == 0)
                {
                    return "";
                }

                string returnListString = string.Empty;
                for (int i = 0; i < targetList.Count; i++)
                {
                    returnListString += targetList[i];
                    if (i != targetList.Count - 1)
                    {
                        returnListString += "|";
                    }
                }

                return returnListString;
            }
            
            return returnValue.ToString();
        }

        public InputPortReflectionInfo(NodeInputPortAttribute inputPortAttribute, FieldInfo nodeInputVariableFieldInfo, object nodeInstance)
        {
            this.inputPortAttribute = inputPortAttribute;
            this.nodeInputVariableFieldInfo = nodeInputVariableFieldInfo;
            this.nodeInstance = nodeInstance;

            InitInputValueType();
            InitInputVariable();
        }

        void InitInputValueType()
        {
            if (nodeInputVariableFieldInfo == null)
            {
                return;
            }

            inputValueType = nodeInputVariableFieldInfo.FieldType.GetGenericArguments()[0];
        }

        void InitInputVariable()
        {
            nodeInputVariableInstance = nodeInputVariableFieldInfo.GetValue(nodeInstance);
            if (nodeInputVariableInstance == null)
            {
                nodeInputVariableInstance = Activator.CreateInstance(nodeInputVariableFieldInfo.FieldType);
                nodeInputVariableFieldInfo.SetValue(nodeInstance, nodeInputVariableInstance);
            }

            targetNodeIdFieldInfo = nodeInputVariableFieldInfo.FieldType.GetField("targetNodeId",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (targetNodeIdFieldInfo == null)
            {
                Debug.LogErrorFormat("type {0} 没有targetNodeId field", nodeInputVariableFieldInfo.FieldType);
                return;
            }

            targetPortIdFieldInfo = nodeInputVariableFieldInfo.FieldType.GetField("targetPortId",
                BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            if (targetPortIdFieldInfo == null)
            {
                Debug.LogErrorFormat("type {0} targetPortId field", nodeInputVariableFieldInfo.FieldType);
                return;
            }

            //value
            inputVariableValueFieldInfo =
                nodeInputVariableFieldInfo.FieldType.GetField("value", BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        }

        public string PortName
        {
            get { return inputPortAttribute.portName; }
        }
    }
}