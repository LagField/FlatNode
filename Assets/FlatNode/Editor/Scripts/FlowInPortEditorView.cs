using UnityEngine;

namespace FlatNode.Editor
{
    public class FlowInPortEditorView : PortEditorView
    {
        public int flowInFromNodeId;

        public FlowInPortEditorView(NodeEditorView nodeView) : base(nodeView)
        {
            this.FlowType = FlowType.In;
            flowInFromNodeId = -1;
        }

        public override void Draw()
        {
            base.Draw();

            //流入箭头
            Rect flowArrowRect = new Rect(portViewRect);
            flowArrowRect.size = new Vector2(20f,20f);
            flowArrowRect.center = portViewRect.center;
            flowArrowRect.x = portViewRect.x + 10f;
            
            GUI.Box(flowArrowRect,string.Empty,Utility.GetGuiStyle("FlowArrow"));
        }

        public override float GetNameWidth()
        {
            return 20f;
        }

        public override void DisconnectAllConnections()
        {
            
        }

        public override string PortDescription
        {
            get { return "节点入口"; }
        }
    }
}