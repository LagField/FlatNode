using System.Collections.Generic;
using FlatNode.Runtime;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    /// <summary>
    /// 端口之间的连线
    /// </summary>
    public class ConnectionLineView
    {
        public PortEditorView FlowOutPortView { get; private set; }
        public PortEditorView FlowInPortView { get; private set; }

        public PortEditorView draggingPort;

        private Vector2 startPos;
        private Vector2 endPos;
        private Vector2 startTangent;
        private Vector2 endTangent;

        private bool isHovering;

        private GraphEditorData graphEditorData;

        public ConnectionLineView(PortEditorView portViewA, PortEditorView portViewB,GraphEditorData graphEditorData)
        {
            if (portViewA.FlowType == portViewB.FlowType)
            {
                Debug.LogError("两个端口的FlowType相同,无法连接");
                return;
            }

            this.graphEditorData = graphEditorData;

            this.FlowOutPortView = portViewA.FlowType == FlowType.In ? portViewB : portViewA;
            this.FlowInPortView = portViewB.FlowType == FlowType.In ? portViewB : portViewA;

            //流入节点都只允许一个连接
            graphEditorData.ClearPortAllConnections(FlowInPortView);

            if (!FlowOutPortView.connectedPortList.Contains(FlowInPortView))
            {
                FlowOutPortView.connectedPortList.Add(FlowInPortView);
            }

            if (!FlowInPortView.connectedPortList.Contains(FlowOutPortView))
            {
                FlowInPortView.connectedPortList.Add(FlowOutPortView);
            }
        }

        /// <summary>
        /// 只有一个port的情况下，这条线认为是正在拖拽连接线
        /// </summary>
        /// <param name="draggingFromPort"></param>
        public ConnectionLineView(PortEditorView draggingFromPort)
        {
            this.draggingPort = draggingFromPort;
        }

        /// <summary>
        /// 检查两个端口是否可以连接
        /// </summary>
        /// <param name="portA"></param>
        /// <param name="portB"></param>
        /// <returns></returns>
        public static bool CheckPortsCanLine(PortEditorView portA, PortEditorView portB)
        {
            if (portA == null || portB == null)
            {
                return false;
            }

            if (portA.FlowType == portB.FlowType)
            {
                return false;
            }

            if (portA.NodeView == portB.NodeView)
            {
                return false;
            }

            PortEditorView flowOutPortView = portA.FlowType == FlowType.Out ? portA : portB;
            PortEditorView flowInPortView = portA.FlowType == FlowType.In ? portA : portB;

            //两边接口是流出和流入
            if (flowOutPortView is FlowOutPortEditorView && flowInPortView is FlowInPortEditorView)
            {
                return true;
            }
            
            //两边接口是Output和Input
            OutputPortEditorView outputPortView = flowOutPortView as OutputPortEditorView;
            InputPortEditorView inputPortView = flowInPortView as InputPortEditorView;
            if (outputPortView == null || inputPortView == null)
            {
                Debug.LogError("端口连接类型错误");
                return false;
            }

            if (outputPortView.PortValueType != inputPortView.PortValueType)
            {
                return false;
            }

            //存取技能变量的端口，都不允许连接
            if (inputPortView.inputPortReflectionInfo.inputValueType == typeof(VariableWrapper))
            {
                return false;
            }
            
            return true;
        }

        public void SetEndPos(Vector2 endPosition)
        {
            this.endPos = endPosition;
        }

        public void DrawDragLine()
        {
            if (draggingPort == null)
            {
                return;
            }

            draggingPort.isDraggingLine = true;
            startPos = draggingPort.connectionCircleRect.center;

            if (draggingPort.FlowType == FlowType.Out)
            {
                if (startPos.x >= endPos.x)
                {
                    Handles.BeginGUI();

                    float horizontalDistance = startPos.x - endPos.x;
                    startTangent = startPos + Vector2.right * 200f;
                    endTangent = endPos + Vector2.right * horizontalDistance / 2f;
                    Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Color.white, Utility.GetBezierLineTexture(), 3f);

                    Handles.EndGUI();
                }
                else
                {
                    Handles.BeginGUI();

                    float horizontalDistance = endPos.x - startPos.x;
                    startTangent = startPos + Vector2.right * horizontalDistance / 2f;
                    endTangent = endPos + Vector2.left * horizontalDistance / 2f;
                    Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Color.white, Utility.GetBezierLineTexture(), 3f);

                    Handles.EndGUI();
                }
            }
            else
            {
                if (startPos.x <= endPos.x)
                {
                    Handles.BeginGUI();

                    float horizontalDistance = endPos.x - startPos.x;
                    startTangent = startPos + Vector2.left * 200f;
                    endTangent = endPos + Vector2.left * horizontalDistance / 2f;
                    Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Color.white, Utility.GetBezierLineTexture(), 3f);

                    Handles.EndGUI();
                }
                else
                {
                    Handles.BeginGUI();

                    float horizontalDistance = startPos.x - endPos.x;
                    startTangent = startPos + Vector2.left * horizontalDistance / 2f;
                    endTangent = endPos + Vector2.right * horizontalDistance / 2f;
                    Handles.DrawBezier(startPos, endPos, startTangent, endTangent, Color.white, Utility.GetBezierLineTexture(), 3f);

                    Handles.EndGUI();
                }
            }
        }

        public void DrawConnectionLine()
        {
            if (FlowOutPortView == null || FlowInPortView == null)
            {
                return;
            }

            startPos = FlowOutPortView.connectionCircleRect.center;
            endPos = FlowInPortView.connectionCircleRect.center;

            float lineWidth;
            Color lineColor;

            if (FlowOutPortView is FlowOutPortEditorView)
            {
                lineWidth = 4f;

                lineColor = isHovering ? Color.red : Color.white;
            }
            else
            {
                lineWidth = 3f;
                lineColor = isHovering ? Color.red : Color.cyan;
            }

            if (startPos.x >= endPos.x)
            {
                Handles.BeginGUI();

                startTangent = startPos + Vector2.right * 200f;
                endTangent = endPos + Vector2.left * 200f;
                Handles.DrawBezier(startPos, endPos, startTangent, endTangent, lineColor, Utility.GetBezierLineTexture(), lineWidth);
                
                Handles.EndGUI();
            }
            else
            {
                Handles.BeginGUI();

                float horizontalDistance = endPos.x - startPos.x;
                startTangent = startPos + Vector2.right * horizontalDistance / 2f;
                endTangent = endPos + Vector2.left * horizontalDistance / 2f;
                Handles.DrawBezier(startPos, endPos, startTangent, endTangent, lineColor, Utility.GetBezierLineTexture(), lineWidth);

                Handles.EndGUI();
            }
        }

        public bool IsPositionCloseToLine(Vector2 positionInWindow)
        {
            return HandleUtility.DistancePointBezier(positionInWindow, startPos, endPos, startTangent, endTangent) < 10f;
        }

        /// <summary>
        /// 设置是否鼠标整悬浮在这个线上
        /// </summary>
        /// <param name="color"></param>
        public void SetHovering(bool isHovering)
        {
            this.isHovering = isHovering;
        }

        /// <summary>
        /// 该连接断开前调用该方法
        /// </summary>
        public void Dispose()
        {
            if (draggingPort != null)
            {
                draggingPort.isDraggingLine = false;
            }
        }
    }
}