using System;
using System.Collections.Generic;
using UnityEngine;

namespace FlatNode.Editor
{
    public class CommentBoxView
    {
        public enum BoxEdge
        {
            None,
            Top,
            Bottom,
            Left,
            Right,
        }

        private GraphEditorWindow graph;
        public Rect rectInGraph;
        public Rect rectInWindow;
        private Rect headRectInWindow;

        private bool isEditingComment;
        public string comment;

        public CommentBoxView(GraphEditorWindow graph, Vector2 startPositionInGraph, Vector2 endPositionInGraph)
        {
            this.graph = graph;
            rectInGraph = new Rect(startPositionInGraph,
                new Vector2(endPositionInGraph.x - startPositionInGraph.x, endPositionInGraph.y - startPositionInGraph.y));

            rectInWindow.position = graph.GraphPositionToWindowPosition(rectInGraph.position);
            rectInWindow.max = graph.GraphPositionToWindowPosition(rectInGraph.max);
        }

        public CommentBoxView(GraphEditorWindow graph, Vector2 startPositionInGraph, Vector2 endPositionInGraph, string comment)
        {
            this.graph = graph;
            rectInGraph = new Rect(startPositionInGraph,
                new Vector2(endPositionInGraph.x - startPositionInGraph.x, endPositionInGraph.y - startPositionInGraph.y));

            rectInWindow.position = graph.GraphPositionToWindowPosition(rectInGraph.position);
            rectInWindow.max = graph.GraphPositionToWindowPosition(rectInGraph.max);
            UpdateHeadRect();

            this.comment = comment;
        }

        public static void DrawDraggingCommentBox(GraphEditorWindow graph, Vector2 startPositionInGraph,
            Vector2 endPositionInGraph)
        {
            Vector2 startPositionInWindow = graph.GraphPositionToWindowPosition(startPositionInGraph);
            Vector2 endPositionInWindow = graph.GraphPositionToWindowPosition(endPositionInGraph);

            Rect drawRect = new Rect(startPositionInWindow,
                new Vector2(endPositionInWindow.x - startPositionInWindow.x, endPositionInWindow.y - startPositionInWindow.y));

            GUI.DrawTexture(drawRect, Utility.GetCommentBoxTexture(), ScaleMode.StretchToFill, true);
        }

        public void Draw()
        {
            Vector2 startPositionInWindow = graph.GraphPositionToWindowPosition(rectInGraph.position);
            Vector2 endPositionInWindow = graph.GraphPositionToWindowPosition(rectInGraph.max);

            rectInWindow.position = startPositionInWindow;
            rectInWindow.max = endPositionInWindow;
            UpdateHeadRect();

            GUI.DrawTexture(rectInWindow, Utility.GetCommentBoxTexture(), ScaleMode.StretchToFill, true);
            GUI.DrawTexture(headRectInWindow, Utility.GetCommentBoxTexture(), ScaleMode.StretchToFill, true);

            if (comment == null)
            {
                comment = string.Empty;
            }

            if (isEditingComment)
            {
                if (GUI.GetNameOfFocusedControl() != "CommentEditArea")
                {
                    GUI.FocusControl("CommentEditArea");
                    GUI.SetNextControlName("CommentEditArea");
                }

                comment = GUI.TextArea(rectInWindow, comment, Utility.GetGuiStyle("CommentText"));
            }
            else
            {
                GUI.Label(rectInWindow, comment, Utility.GetGuiStyle("CommentText"));
            }
        }

        /// <summary>
        /// 更新头部的Rect
        /// todo 头部颜色要不一样
        /// </summary>
        private void UpdateHeadRect()
        {
            headRectInWindow = new Rect(rectInWindow);
            if (comment == null)
            {
                comment = string.Empty;
            }

            float commentTextHeight = Utility.GetGuiStyle("CommentText").CalcHeight(new GUIContent(comment), rectInWindow.width);
            headRectInWindow.height = Mathf.Max(commentTextHeight,80f);
        }

        public bool Contains(Vector2 positionInWindow)
        {
            //只有头部是操作区域
            return headRectInWindow.Contains(positionInWindow);
        }

        /// <summary>
        /// 确定该位置靠近注释框的哪一边
        /// </summary>
        /// <param name="positionInWindow"></param>
        /// <returns></returns>
        public BoxEdge AtEdge(Vector2 positionInWindow)
        {
            BoxEdge result = BoxEdge.None;

            const float distance = 10f;
            //check right
            if (Mathf.Abs(rectInWindow.xMax - positionInWindow.x) <= distance && positionInWindow.y <= rectInWindow.yMax &&
                positionInWindow.y >= rectInWindow.yMin)
            {
                result = BoxEdge.Right;
            }
            //check bottom
            else if (Mathf.Abs(rectInWindow.yMax - positionInWindow.y) <= distance && positionInWindow.x <= rectInWindow.xMax &&
                     positionInWindow.x >= rectInWindow.xMin)
            {
                result = BoxEdge.Bottom;
            }
            //check top
            else if (Mathf.Abs(rectInWindow.yMin - positionInWindow.y) <= distance && positionInWindow.x <= rectInWindow.xMax &&
                     positionInWindow.x >= rectInWindow.xMin)
            {
                result = BoxEdge.Top;
            }
            else if (Mathf.Abs(rectInWindow.xMin - positionInWindow.x) <= distance && positionInWindow.y <= rectInWindow.yMax &&
                     positionInWindow.y >= rectInWindow.yMin)
            {
                result = BoxEdge.Left;
            }

            return result;
        }

        public void Resizing(BoxEdge edge, Vector2 delta)
        {
            switch (edge)
            {
                case BoxEdge.None:
                    break;
                case BoxEdge.Top:
                    delta.x = 0f;
                    rectInGraph.position += delta;
                    rectInGraph.size = new Vector2(rectInGraph.size.x, rectInGraph.size.y - delta.y);
                    break;
                case BoxEdge.Bottom:
                    rectInGraph.yMax += delta.y;
                    break;
                case BoxEdge.Left:
                    delta.y = 0f;
                    rectInGraph.position += delta;
                    rectInGraph.size = new Vector2(rectInGraph.size.x - delta.x, rectInGraph.size.y);
                    break;
                case BoxEdge.Right:
                    rectInGraph.xMax += delta.x;
                    break;
            }
        }

        public void Drag(List<NodeEditorView> nodeViewList, Vector2 dragOffset)
        {
            rectInGraph.position += dragOffset;

            //同时，所有在注释框里面的节点要跟着被拖动
            if (nodeViewList != null && nodeViewList.Count > 0)
            {
                for (int i = 0; i < nodeViewList.Count; i++)
                {
                    NodeEditorView nodeEditorView = nodeViewList[i];
                    if (nodeEditorView.IsContainInRect(rectInWindow))
                    {
                        nodeEditorView.Drag(dragOffset);
                    }
                }
            }
        }

        public void EnableEditComment(bool isEnable)
        {
            isEditingComment = isEnable;
//            Debug.Log("enable edit comment: " + isEnable);
        }
    }
}