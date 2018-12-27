using System;
using System.Collections.Generic;
using FlatNode.Runtime;
using UnityEngine;

namespace FlatNode.Editor
{
    public abstract class PortEditorView
    {
        public int portId;
        public FlowType FlowType { get; protected set; }
        public List<PortEditorView> connectedPortList;
        public Rect portViewRect;

        public NodeEditorView NodeView { get; protected set; }

        public Rect connectionCircleRect;
        protected const float ConnectionCircleSize = 20f;
        protected const float ConnectionCirclePadding = 3f;

        public virtual bool IsConnected
        {
            get { return connectedPortList.Count > 0 || isDraggingLine; }
        }

        /// <summary>
        /// 是否正在连线中
        /// </summary>
        public bool isDraggingLine;

        public abstract float GetNameWidth();
        public abstract void DisconnectAllConnections();
        public abstract string PortDescription { get; }

        protected PortEditorView(NodeEditorView nodeView)
        {
            NodeView = nodeView;
            connectedPortList = new List<PortEditorView>();
        }

        public virtual void Draw()
        {
            DrawConnectionPortCircle();
        }

        void DrawConnectionPortCircle()
        {
            if (FlowType == FlowType.In)
            {
                connectionCircleRect = new Rect(portViewRect.x - ConnectionCirclePadding - ConnectionCircleSize,
                    portViewRect.center.y - ConnectionCircleSize / 2f, ConnectionCircleSize, ConnectionCircleSize);
            }
            else
            {
                connectionCircleRect = new Rect(portViewRect.xMax + ConnectionCirclePadding,
                    portViewRect.center.y - ConnectionCircleSize / 2f, ConnectionCircleSize, ConnectionCircleSize);
            }

            if (IsConnected)
            {
                GUI.Box(connectionCircleRect, string.Empty, Utility.GetGuiStyle("FillCircle"));
            }
            else
            {
                GUI.Box(connectionCircleRect, string.Empty, Utility.GetGuiStyle("Circle"));
            }
        }
        
        /// <summary>
        /// 技能载入完成时调入，这个时候所有节点和所有连线都已经生成，这里处理一下其他需要初始化的逻辑
        /// </summary>
        public virtual void OnLoadFinish()
        {
            
        }
    }

    public enum FlowType
    {
        In = 0,
        Out = 1
    }
}