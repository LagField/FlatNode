using System.Collections.Generic;
using FlatNode.Runtime;
using UnityEngine;

namespace FlatNode.Editor
{
    public class FlowOutPortEditorView : PortEditorView
    {
        public NodeFlowOutPortDefineAttribute flowoutPortAttribute;

        public FlowOutPortEditorView(NodeEditorView nodeView, NodeFlowOutPortDefineAttribute flowoutPortAttribute) :
            base(nodeView)
        {
            FlowType = FlowType.Out;
            this.flowoutPortAttribute = flowoutPortAttribute;
        }

        public override void Draw()
        {
            //流出连接箭头小标
            connectionCircleRect = new Rect(portViewRect.xMax + ConnectionCirclePadding, portViewRect.center.y - ConnectionCircleSize / 2f,
                ConnectionCircleSize, ConnectionCircleSize);

            if (IsConnected)
            {
                GUI.Box(connectionCircleRect, string.Empty, Utility.GetGuiStyle("PortArrowFilled"));
            }
            else
            {
                GUI.Box(connectionCircleRect, string.Empty, Utility.GetGuiStyle("PortArrow"));
            }

            Rect nameRect = new Rect(portViewRect);
            nameRect.width = nameRect.width - 5f;

            GUI.Label(nameRect, string.Format("({0}){1}", flowoutPortAttribute.portId, flowoutPortAttribute.portName),
                Utility.GetGuiStyle("PortNameRight"));
        }

        public override float GetNameWidth()
        {
            float fontSize = Utility.GetGuiStyle("PortNameRight").fontSize;
            return Utility.GetStringGuiWidth(flowoutPortAttribute.portName, fontSize) + 30f; //30代表的是(n)前缀的长度
        }

        public override void DisconnectAllConnections()
        {
        }

        public override string PortDescription
        {
            get { return flowoutPortAttribute.description; }
        }
    }
}