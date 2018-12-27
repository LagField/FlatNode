using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Editor
{
    public class OutputPortEditorView : PortEditorView
    {
        public OutputPortReflectionInfo outputPortReflectionInfo;

        private List<int> connectedNodeIdList;

        public Type PortValueType
        {
            get
            {
                if (overridePortType != null)
                {
                    return overridePortType;
                }
                return outputPortReflectionInfo.OutputType;
            }
        }
                
        /// <summary>
        /// 当不使用默认类型时，更改这个
        /// 目前主要针对读写技能变量的节点使用
        /// </summary>
        public Type overridePortType;

        public OutputPortEditorView(NodeEditorView nodeView, OutputPortReflectionInfo outputPortReflectionInfo, int portId) :
            base(nodeView)
        {
            FlowType = FlowType.Out;
            this.outputPortReflectionInfo = outputPortReflectionInfo;
            this.portId = portId;
        }

        public override void Draw()
        {
            base.Draw();

            DrawNameAndType();
        }

        private void DrawNameAndType()
        {
            Rect nameRect = new Rect(portViewRect);
            nameRect.width = nameRect.width - 5f;

            GUI.Label(nameRect,
                string.Format("{0}\n({1})", outputPortReflectionInfo.PortName, Utility.BeautifyTypeName(PortValueType)),
                Utility.GetGuiStyle("PortNameRight"));
        }

        public override float GetNameWidth()
        {
            GUIStyle guiStyle = Utility.GetGuiStyle("PortNameRight");
            float nameWidth = Utility.GetStringGuiWidth(outputPortReflectionInfo.PortName, guiStyle);
            float typeNameWidth =
                Utility.GetStringGuiWidth(Utility.BeautifyTypeName(PortValueType) + "()", guiStyle);
            return Mathf.Max(nameWidth, typeNameWidth);
        }

        public override void DisconnectAllConnections()
        {
        }

        public override string PortDescription
        {
            get { return outputPortReflectionInfo.outputPortAttribute.description; }
        }
    }
}