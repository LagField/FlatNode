using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public class GraphEditorWindow : EditorWindow
    {
        [MenuItem("FlatNode/逻辑编辑器面板",priority = 0)]
        public static void ShowWindow()
        {
            GraphEditorWindow graphEditorWindow = GetWindow<GraphEditorWindow>();
            graphEditorWindow.minSize = new Vector2(800, 600);
            graphEditorWindow.titleContent = new GUIContent("FlatNode编辑器");
        }

        public static GraphEditorWindow instance;
        public GraphEditorData data;

        protected Rect guiRect;

        private ControlType controlType = ControlType.None;

        private NodeEditorView draggingNodeView;

        private PortEditorView currentHoverPortView;
        private NodeEditorView currentHoverNodeView;
        private ConnectionLineView currentHoverConnectionLineView;
        private double startHoverdTime;

        private ConnectionLineView draggingLineView;

        private bool isCtrlDown;
        private Vector2 startDraggingCommentBoxGraphPosition;
        private Vector2 endDraggingCommentBoxGraphPosition;
        private CommentBoxView lastClickCommentBox;
        private CommentBoxView editingCommentBox;
        private CommentBoxView draggingCommentBox;
        private CommentBoxView resizingCommentBox;
        private CommentBoxView.BoxEdge resizingCommentEdge;
        private double lastClickCommentTime;

        private Vector2 startMultiSelectionPos;

        private Matrix4x4 originMatrix;

        private double repaintTimer;

        private void OnEnable()
        {
            instance = this;
            data = new GraphEditorData();
            Reset();
        }

        private void OnDestroy()
        {
            instance = null;
        }

        void Reset()
        {
            if (data != null)
            {
                data.Clear();
            }

            draggingNodeView = null;
            currentHoverPortView = null;
            currentHoverNodeView = null;
            controlType = ControlType.None;

            repaintTimer = EditorApplication.timeSinceStartup;
        }

        private void Update()
        {
            if (EditorApplication.timeSinceStartup - repaintTimer > 0.03)
            {
                repaintTimer = EditorApplication.timeSinceStartup;
                Repaint();
            }

            //重置双击状态
            if (lastClickCommentBox != null)
            {
                double currentTime = EditorApplication.timeSinceStartup;
                if (currentTime - lastClickCommentTime > 0.3f)
                {
                    lastClickCommentBox = null;
                }
            }
        }

        private void OnGUI()
        {
            Event e = Event.current;

            originMatrix = GUI.matrix;
            guiRect = new Rect(0, 0, position.width, position.height);
            DrawHelper.DrawGrid(50, .5f, guiRect, data.GraphOffset, data.GraphZoom);
            DrawHelper.DrawGrid(25, .3f, guiRect, data.GraphOffset, data.GraphZoom);

            GUILayout.BeginHorizontal();
            //绘制技能名称
            if (string.IsNullOrEmpty(data.graphName))
            {
                GUILayout.Label("新建技能", Utility.GetGuiStyle("SkillGraphName"));
            }
            else
            {
                GUILayout.Label(string.Format("{0}.{1}", data.graphId, data.graphName), Utility.GetGuiStyle("SkillGraphName"));
            }

//
//            if (GUILayout.Button("Test", GUILayout.MinHeight(50), GUILayout.MinWidth(50)))
//            {
//                List<int> result = new List<int>();
//                for (int i = 0; i < data.logicNodeViewList.Count; i++)
//                {
//                    if (data.logicNodeViewList[i].CheckNodeIsCommonNode())
//                    {
//                        result.Add(data.logicNodeViewList[i].NodeId);
//                    }
//                }
//
//                for (int i = 0; i < result.Count; i++)
//                {
//                    Debug.Log(result[i]);
//                }
//            }

            GUILayout.EndHorizontal();

            ProcessEvent(e);

            BeginZoom();

            DrawCommentBox();
            DrawConnectionLine();
            DrawNodes();

            EndZoom();
            DrawMultiSelection(e);

            GUI.matrix = originMatrix;

            DrawTipBox();
        }

        void ProcessEvent(Event e)
        {
            Vector2 windowMousePosition = e.mousePosition;
            Vector2 zoomedMousePosition = NonZoomedWindowPositionToZoomedWindowPosition(windowMousePosition);

            List<NodeEditorView> nodeViewList = data.CurrentNodeList;
            List<ConnectionLineView> connectionLineViewList = data.CurrentConnectionLineList;

            bool hasHoveredNodeOrPort = false;
            //如果在输入标签上，要屏蔽与注释框的交互
            bool hasHoverdNodeInputLabel = false;
            for (int i = 0; i < nodeViewList.Count; i++)
            {
                NodeEditorView nodeView = nodeViewList[i];

                #region 提示框鼠标悬浮显示

                //在没有任何操作的时候，才显示tips框
                if (controlType == ControlType.None && !hasHoveredNodeOrPort)
                {
                    if (nodeView.NodeNameRect.Contains(zoomedMousePosition))
                    {
                        //reset time
                        if (currentHoverNodeView != nodeView)
                        {
                            startHoverdTime = EditorApplication.timeSinceStartup;
                        }

                        hasHoveredNodeOrPort = true;
                        currentHoverNodeView = nodeView;
                        currentHoverPortView = null;
                    }

                    if (!hasHoveredNodeOrPort)
                    {
                        List<PortEditorView> allPortList = nodeView.allPortList;
                        for (int j = 0; j < allPortList.Count; j++)
                        {
                            PortEditorView portView = allPortList[j];
                            if (portView.portViewRect.Contains(zoomedMousePosition))
                            {
                                //reset time
                                if (currentHoverPortView != portView)
                                {
                                    startHoverdTime = EditorApplication.timeSinceStartup;
                                }

                                hasHoveredNodeOrPort = true;
                                currentHoverPortView = portView;
                                currentHoverNodeView = null;

                                break;
                            }

                            if (portView is InputPortEditorView)
                            {
                                InputPortEditorView inputView = portView as InputPortEditorView;
                                if (inputView.IsInputLabelContains(zoomedMousePosition))
                                {
                                    hasHoverdNodeInputLabel = true;
                                }
                            }
                        }
                    }
                }

                #endregion

                if (controlType == ControlType.None && e.type == EventType.MouseDown && e.button == 0)
                {
                    //点击开始拖拽连线
                    List<PortEditorView> allPortList = nodeView.allPortList;
                    for (int j = 0; j < allPortList.Count; j++)
                    {
                        PortEditorView portView = allPortList[j];
                        if (portView.connectionCircleRect.Contains(zoomedMousePosition))
                        {
                            controlType = ControlType.DraggingConnection;
                            draggingLineView = new ConnectionLineView(portView);
                            draggingLineView.SetEndPos(zoomedMousePosition);
                            e.Use();
                            break;
                        }
                    }

                    //开始拖拽节点
                    if (nodeView.viewRect.Contains(zoomedMousePosition))
                    {
                        if (data.selectedNodeList.Contains(nodeView))
                        {
                            controlType = ControlType.DraggingMultiNodes;
                        }
                        else
                        {
                            controlType = ControlType.DraggingNode;
                            draggingNodeView = nodeView;
                            data.PutNodeToListTail(i);
                        }

                        e.Use();
                        break;
                    }
                }

                //节点右键菜单
                if (controlType == ControlType.None && e.type == EventType.MouseDown && e.button == 1)
                {
                    if (nodeView.viewRect.Contains(zoomedMousePosition))
                    {
                        OpenNodeGenericMenu(nodeView, e.mousePosition);
                        e.Use();
                        break;
                    }
                }
            }

            if (currentHoverConnectionLineView != null)
            {
                currentHoverConnectionLineView.SetHovering(false);
            }

            currentHoverConnectionLineView = null;
            if (controlType == ControlType.None)
            {
                //和连接线的交互
                for (int i = 0; i < connectionLineViewList.Count; i++)
                {
                    ConnectionLineView connectionLineView = connectionLineViewList[i];
                    if (connectionLineView.IsPositionCloseToLine(zoomedMousePosition))
                    {
                        currentHoverConnectionLineView = connectionLineView;
                        currentHoverConnectionLineView.SetHovering(true);

                        //右键点击连线
                        if (e.type == EventType.MouseDown && e.button == 1)
                        {
                            OpenConnectionLineGenericMenu(connectionLineView, e.mousePosition);
                            e.Use();
                        }

                        break;
                    }
                }
            }

            if (!hasHoveredNodeOrPort)
            {
                currentHoverPortView = null;
                currentHoverNodeView = null;
            }

            if (controlType == ControlType.DraggingNode && e.type == EventType.MouseDrag && e.button == 0)
            {
                draggingNodeView.Drag(e.delta / data.GraphZoom);
                e.Use();
            }

            if (controlType == ControlType.DraggingNode && e.type == EventType.MouseUp && e.button == 0)
            {
                controlType = ControlType.None;
                draggingNodeView = null;
                e.Use();
            }

            if (controlType == ControlType.DraggingMultiNodes && e.type == EventType.MouseDrag && e.button == 0)
            {
                for (int i = 0; i < data.selectedNodeList.Count; i++)
                {
                    data.selectedNodeList[i].Drag(e.delta / data.GraphZoom);
                }

                e.Use();
            }

            if (controlType == ControlType.DraggingMultiNodes && e.type == EventType.MouseUp && e.button == 0)
            {
                controlType = ControlType.None;

                e.Use();
            }

            if (controlType == ControlType.DraggingConnection && e.type == EventType.MouseDrag && e.button == 0)
            {
                if (draggingLineView != null)
                {
                    draggingLineView.SetEndPos(zoomedMousePosition);
                }
            }

            if (controlType == ControlType.DraggingConnection && e.type == EventType.MouseUp && e.button == 0)
            {
                if (draggingLineView != null)
                {
                    bool createNewConnection = false;
                    //检查是否有连接
                    for (int i = 0; i < nodeViewList.Count; i++)
                    {
                        NodeEditorView nodeView = nodeViewList[i];
                        List<PortEditorView> allPortList = nodeView.allPortList;
                        for (int j = 0; j < allPortList.Count; j++)
                        {
                            PortEditorView portView = allPortList[j];

                            if (portView.connectionCircleRect.Contains(zoomedMousePosition))
                            {
                                if (ConnectionLineView.CheckPortsCanLine(draggingLineView.draggingPort, portView))
                                {
                                    ConnectionLineView newConnectionLine =
                                        new ConnectionLineView(draggingLineView.draggingPort, portView, data);
                                    data.connectionLineList.Add(newConnectionLine);
                                    data.CurrentConnectionLineList.Add(newConnectionLine);

                                    createNewConnection = true;
                                    break;
                                }
                            }
                        }

                        if (createNewConnection)
                        {
                            break;
                        }
                    }

                    draggingLineView.Dispose();
                }

                draggingLineView = null;
                controlType = ControlType.None;
                e.Use();
            }

            //中键拖动面板
            if (e.type == EventType.MouseDrag && e.button == 2)
            {
                data.GraphOffset += e.delta / data.GraphZoom;
                e.Use();

                if (draggingLineView != null)
                {
                    draggingLineView.SetEndPos(zoomedMousePosition);
                }
            }

            //滚轮控制缩放
            if (e.type == EventType.ScrollWheel)
            {
                data.GraphZoom -= e.delta.y / GraphEditorData.GraphZoomSpeed;
                data.GraphZoom = Mathf.Clamp(data.GraphZoom, GraphEditorData.MinGraphZoom, GraphEditorData.MaxGraphZoom);
                e.Use();

                if (draggingLineView != null)
                {
                    draggingLineView.SetEndPos(zoomedMousePosition);
                }
            }


            //记录左ctrl按下状态
            if (e.type == EventType.KeyDown && e.keyCode == KeyCode.LeftControl)
            {
                isCtrlDown = true;
            }

            //记录左ctrl按下状态
            if (e.type == EventType.KeyUp && e.keyCode == KeyCode.LeftControl)
            {
                isCtrlDown = false;

                if (controlType == ControlType.DraggingNewCommentBox)
                {
                    controlType = ControlType.None;
                }
            }

            //开始拖拽新的注释框
            if (isCtrlDown && controlType == ControlType.None && e.type == EventType.MouseDown && e.button == 0)
            {
                controlType = ControlType.DraggingNewCommentBox;
                startDraggingCommentBoxGraphPosition = WindowPositionToGraphPosition(windowMousePosition);
                endDraggingCommentBoxGraphPosition = startDraggingCommentBoxGraphPosition;

                e.Use();
            }

            //更新新的注释框
            if (controlType == ControlType.DraggingNewCommentBox && e.type == EventType.MouseDrag && e.button == 0)
            {
                endDraggingCommentBoxGraphPosition = WindowPositionToGraphPosition(windowMousePosition);
                e.Use();
            }

            //结束新的注释框
            if (isCtrlDown && controlType == ControlType.DraggingNewCommentBox && e.type == EventType.MouseUp && e.button == 0)
            {
                controlType = ControlType.None;

                CommentBoxView newCommentBox = new CommentBoxView(this, startDraggingCommentBoxGraphPosition,
                    endDraggingCommentBoxGraphPosition);
                data.CurrentCommentBoxViewList.Add(newCommentBox);

                e.Use();
            }

            //注释框操作
            if (data.CurrentCommentBoxViewList.Count > 0 && !hasHoverdNodeInputLabel)
            {
                for (int i = 0; i < data.CurrentCommentBoxViewList.Count; i++)
                {
                    CommentBoxView commentBoxView = data.CurrentCommentBoxViewList[i];

                    //右键点击注释框
                    if (controlType == ControlType.None && e.type == EventType.MouseDown && e.button == 1)
                    {
                        if (commentBoxView.Contains(zoomedMousePosition))
                        {
                            OpenCommentBoxGenericMenu(commentBoxView, e.mousePosition);
                            e.Use();
                            break;
                        }
                    }

                    //拖拽编辑注释区域大小
                    if (controlType == ControlType.None && (e.type != EventType.Layout || e.type != EventType.Repaint))
                    {
                        CommentBoxView.BoxEdge boxEdge = commentBoxView.AtEdge(zoomedMousePosition);
                        if (boxEdge != CommentBoxView.BoxEdge.None)
                        {
                            MouseCursor cursorMode =
                                (boxEdge == CommentBoxView.BoxEdge.Left || boxEdge == CommentBoxView.BoxEdge.Right)
                                    ? MouseCursor.ResizeHorizontal
                                    : MouseCursor.ResizeVertical;
                            EditorGUIUtility.AddCursorRect(guiRect, cursorMode);

                            //开始拖拽扩大
                            if (e.type == EventType.MouseDown && e.button == 0)
                            {
                                resizingCommentEdge = boxEdge;
                                resizingCommentBox = commentBoxView;
                                controlType = ControlType.ResizingCommentBox;

                                e.Use();
                            }

                            break;
                        }
                    }

                    //双击编辑注释
                    if ((controlType == ControlType.None || controlType == ControlType.DraggingCommentBox) &&
                        e.type == EventType.MouseDown && e.button == 0)
                    {
                        if (commentBoxView.Contains(zoomedMousePosition))
                        {
                            if (lastClickCommentBox == null || lastClickCommentBox != commentBoxView)
                            {
//                                Debug.Log("click once");
                                //一次点击可能是要拖拽了
                                controlType = ControlType.DraggingCommentBox;
                                lastClickCommentBox = commentBoxView;
                                draggingCommentBox = commentBoxView;
                                lastClickCommentTime = EditorApplication.timeSinceStartup;
                                e.Use();
                                break;
                            }
                            else if (lastClickCommentBox == commentBoxView)
                            {
                                double currentTime = EditorApplication.timeSinceStartup;
                                if (currentTime - lastClickCommentTime <= 0.3f)
                                {
//                                    Debug.Log("click twice");
                                    controlType = ControlType.EditingComment;
                                    lastClickCommentBox.EnableEditComment(true);
                                    editingCommentBox = lastClickCommentBox;
                                    lastClickCommentBox = null;
                                    e.Use();
                                    break;
                                }
                                else
                                {
//                                    Debug.Log("click twice failed");
                                    lastClickCommentBox = null;
                                }
                            }
                        }
                    }
                }
            }

            //右键点击面板
            if (e.type == EventType.MouseDown && e.button == 1)
            {
                OpenGraphGenericMenu(e.mousePosition);
                e.Use();
            }

            //改变注释框大小的时候，改变鼠标图标
            if (controlType == ControlType.ResizingCommentBox)
            {
                MouseCursor cursorMode =
                    (resizingCommentEdge == CommentBoxView.BoxEdge.Left || resizingCommentEdge == CommentBoxView.BoxEdge.Right)
                        ? MouseCursor.ResizeHorizontal
                        : MouseCursor.ResizeVertical;
                EditorGUIUtility.AddCursorRect(guiRect, cursorMode);
            }

            //编辑注释框大小
            if (controlType == ControlType.ResizingCommentBox && e.type == EventType.MouseDrag && e.button == 0)
            {
                if (resizingCommentBox != null)
                {
                    if (resizingCommentEdge != CommentBoxView.BoxEdge.None)
                    {
                        resizingCommentBox.Resizing(resizingCommentEdge, e.delta / data.GraphZoom);
                        e.Use();
                    }
                }
            }

            //停止编辑注释框大小
            if (controlType == ControlType.ResizingCommentBox && e.type == EventType.MouseUp && e.button == 0)
            {
                controlType = ControlType.None;
                resizingCommentBox = null;
                resizingCommentEdge = CommentBoxView.BoxEdge.None;
                e.Use();
            }

            //拖拽注释框
            if (controlType == ControlType.DraggingCommentBox && draggingCommentBox != null && e.type == EventType.MouseDrag &&
                e.button == 0)
            {
                if (draggingCommentBox.Contains(zoomedMousePosition))
                {
                    draggingCommentBox.Drag(data.CurrentNodeList, e.delta / data.GraphZoom);
                    e.Use();
                }
            }

            //停止拖拽注释框
            if (controlType == ControlType.DraggingCommentBox && draggingCommentBox != null && e.type == EventType.MouseUp &&
                e.button == 0)
            {
                draggingCommentBox = null;
                controlType = ControlType.None;
                e.Use();
            }

            //停止编辑注释框
            if (e.type == EventType.MouseDown)
            {
                if (controlType == ControlType.EditingComment)
                {
                    if (!editingCommentBox.Contains(zoomedMousePosition))
                    {
                        controlType = ControlType.None;
                        editingCommentBox.EnableEditComment(false);
                        editingCommentBox = null;
                        GUI.FocusControl(null);
                        e.Use();
                    }
                }
                else
                {
                    data.ClearSelectedNode();
                    GUI.FocusControl(null);
                }
            }

            //开始多选框
            if (controlType == ControlType.None && e.type == EventType.MouseDrag && e.button == 0)
            {
                startMultiSelectionPos = e.mousePosition;
                controlType = ControlType.DraggingMultiSelection;
                e.Use();
            }

            //更新多选框
            if (controlType == ControlType.DraggingMultiSelection)
            {
                Rect multiSelectionRect = new Rect();
                multiSelectionRect.position = NonZoomedWindowPositionToZoomedWindowPosition(startMultiSelectionPos);
                multiSelectionRect.max = NonZoomedWindowPositionToZoomedWindowPosition(e.mousePosition);
                data.UpdateSelectedNode(multiSelectionRect);
            }

            //结束多选框
            if (controlType == ControlType.DraggingMultiSelection && e.type == EventType.MouseUp && e.button == 0)
            {
                controlType = ControlType.None;
                e.Use();
            }

            //排除掉鼠标移出去之后，多选框还会继续拖拽的问题
            if (!guiRect.Contains(e.mousePosition) && e.type != EventType.Layout && e.type != EventType.Repaint)
            {
                if (controlType == ControlType.DraggingMultiSelection)
                {
                    controlType = ControlType.None;
                    e.Use();
                }
            }
        }

        #region 绘制

        void DrawNodes()
        {
            if (data == null || data.CurrentNodeList == null)
            {
                return;
            }

            List<NodeEditorView> nodeViewList = data.CurrentNodeList;
            for (int i = 0; i < nodeViewList.Count; i++)
            {
                NodeEditorView nodeView = nodeViewList[i];

                nodeView.DrawNodeGUI();
            }
        }

        private void BeginZoom()
        {
            GUI.EndClip();

            float graphZoom = data.GraphZoom;
            GUIUtility.ScaleAroundPivot(Vector2.one * graphZoom, guiRect.size * 0.5f);

            GUI.BeginClip(new Rect(-(guiRect.width / graphZoom - guiRect.width) * 0.5f,
                -((guiRect.height / graphZoom - guiRect.height) * 0.5f) + 21 / graphZoom, guiRect.width / graphZoom,
                guiRect.height / graphZoom));
        }

        private void EndZoom()
        {
            float graphZoom = data.GraphZoom;
            GUIUtility.ScaleAroundPivot(Vector2.one / graphZoom, guiRect.size * 0.5f);

            Vector3 offset = new Vector3((guiRect.width / graphZoom - guiRect.width) * 0.5f,
                (guiRect.height / graphZoom - guiRect.height) * 0.5f - 21 / graphZoom + 21, 0);
            GUI.matrix = Matrix4x4.TRS(offset, Quaternion.identity, Vector3.one);
        }

        private bool isLayoutDone;

        void DrawTipBox()
        {
            if (currentHoverNodeView != null || currentHoverPortView != null)
            {
                if (EditorApplication.timeSinceStartup - startHoverdTime < 0.3f)
                {
                    return;
                }

                string description = string.Empty;


                if (currentHoverNodeView != null)
                {
                    string typeName = currentHoverNodeView.ReflectionInfo.Type.Name;
                    string nodeDescription = currentHoverNodeView.ReflectionInfo.NodeAttribute.nodeDescription;
                    description = string.Format("脚本: {0} \n {1}", typeName, nodeDescription);
                }
                else if (currentHoverPortView != null)
                {
                    description = currentHoverPortView.PortDescription;
                }

                if (string.IsNullOrEmpty(description))
                {
                    return;
                }

                Event e = Event.current;

                if (e.type == EventType.Layout)
                {
                    isLayoutDone = true;
                }

                if ((e.type == EventType.Layout || e.type == EventType.Repaint) && isLayoutDone)
                {
                    GUIStyle tipStyle = Utility.GetGuiStyle("TipLabel");
                    Rect tipRect = GUILayoutUtility.GetRect(new GUIContent(description), tipStyle, GUILayout.MinWidth(200),
                        GUILayout.MaxWidth(300));
                    Vector2 mousePosition = e.mousePosition;
                    GUI.Box(new Rect(mousePosition + new Vector2(10f, 10f), tipRect.size), description, tipStyle);
                }

                if (e.type != EventType.Layout)
                {
                    isLayoutDone = false;
                }
            }
        }

        private void DrawCommentBox()
        {
            if (controlType == ControlType.DraggingNewCommentBox)
            {
                CommentBoxView.DrawDraggingCommentBox(this, startDraggingCommentBoxGraphPosition,
                    endDraggingCommentBoxGraphPosition);
            }

            if (data.CurrentCommentBoxViewList.Count > 0)
            {
                for (int i = 0; i < data.CurrentCommentBoxViewList.Count; i++)
                {
                    CommentBoxView commentBoxView = data.CurrentCommentBoxViewList[i];
                    commentBoxView.Draw();
                }
            }
        }

        void DrawConnectionLine()
        {
            if (draggingLineView != null)
            {
                draggingLineView.DrawDragLine();
            }

            List<ConnectionLineView> connectionLineViewList = data.CurrentConnectionLineList;
            for (int i = 0; i < connectionLineViewList.Count; i++)
            {
                ConnectionLineView connectionLineView = connectionLineViewList[i];
                connectionLineView.DrawConnectionLine();
            }
        }

        /// <summary>
        /// 绘制多选框
        /// </summary>
        /// <param name="e"></param>
        void DrawMultiSelection(Event e)
        {
            if (controlType == ControlType.DraggingMultiSelection)
            {
                Vector2 endMultiSelectionPos = e.mousePosition;

                Rect selectionRect = new Rect();
                selectionRect.position = startMultiSelectionPos;
                selectionRect.max = endMultiSelectionPos;

                Handles.BeginGUI();
                Handles.DrawSolidRectangleWithOutline(selectionRect, Color.white * .1f, new Color(1, 1, 1, 1));
                Handles.EndGUI();
            }
        }

        #endregion

        #region 节点处理

        /// <summary>
        /// 编辑器右键菜单
        /// </summary>
        void OpenGraphGenericMenu(Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("增加新节点"), false, OpenAddNodeWindow, mousePosition);

            if (string.IsNullOrEmpty(data.graphName) && data.nodeList.Count > 0)
            {
                genericMenu.AddSeparator("");
                genericMenu.AddItem(new GUIContent("保存为新技能"), false, OpenSaveSkillWindow, mousePosition);
            }
            else if (!string.IsNullOrEmpty(data.graphName) && data.nodeList.Count > 0)
            {
                genericMenu.AddSeparator("");
                genericMenu.AddItem(new GUIContent("保存当前技能"), false, SaveCurrentSkill, mousePosition);
            }

            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("载入技能"), false, OpenSkillLoadWindow, mousePosition);

            genericMenu.AddSeparator("");
            genericMenu.AddItem(new GUIContent("删除技能"), false, OpenDeleteSkillWindow, mousePosition);

            genericMenu.ShowAsContext();
        }

        void OpenAddNodeWindow(object mousePositionObject)
        {
            Vector2 mousePosition = (Vector2) mousePositionObject;

            try
            {
                PopupWindow.Show(new Rect(mousePosition + position.position, new Vector2(200, 0)), new AddNodePopupWindow(type =>
                {
                    if (data.CheckDuplicateEntranceNode(type))
                    {
                        EditorUtility.DisplayDialog("重复入口类型", "每一种入口节点只允许存在一个", "ok");
                        return;
                    }

                    //重置tip显示
                    currentHoverNodeView = null;
                    currentHoverPortView = null;
                    startHoverdTime = EditorApplication.timeSinceStartup;

                    int newId = data.GetNewNodeId();
                    NodeReflectionInfo reflectionInfo = new NodeReflectionInfo(type);
                    NodeEditorView nodeEditorView = new NodeEditorView(WindowPositionToGraphPosition(mousePosition), this, newId,
                        reflectionInfo);
                    data.nodeList.Add(nodeEditorView);
                    data.CurrentNodeList.Add(nodeEditorView);
                }));
            }
            catch
            {
            }
        }

        void SaveCurrentSkill(object mousePositionObject)
        {
            if (data.CheckHasEntranceNode())
            {
                PersistenceTool.SaveGraph(data);
            }
            else
            {
                EditorUtility.DisplayDialog("没有入口节点", "技能当中没有入口节点", "ok");
            }
        }

        void OpenSaveSkillWindow(object mousePositionObject)
        {
            Vector2 mousePosition = (Vector2) mousePositionObject;

            if (!data.CheckHasEntranceNode())
            {
                EditorUtility.DisplayDialog("没有入口节点", "技能当中没有入口节点", "ok");
                return;
            }

            try
            {
                int newSkillId = PersistenceTool.GetNewGraphId();
                PopupWindow.Show(new Rect(mousePosition + position.position, new Vector2(500, 0)), new SaveGraphPopupWindow(
                    newSkillId, (id, skillName, description) =>
                    {
                        data.graphId = id;
                        data.graphName = skillName;
                        data.graphDescription = description;
                        PersistenceTool.SaveGraph(data);
                    }));
            }
            catch
            {
            }
        }

        void OpenSkillLoadWindow(object mousePositionObject)
        {
            Vector2 mousePosition = (Vector2) mousePositionObject;
            try
            {
                PopupWindow.Show(new Rect(mousePosition + position.position, new Vector2(500, 0)), new LoadGraphPopupWindow(
                    loadSkillId =>
                    {
                        Reset();
                        data = PersistenceTool.LoadGraph(this, loadSkillId);

                        //做一些载入完成的初始化工作
                        data.OnLoadFinish();

                        //检查连线的合法性
                        data.CheckAllNodeConnection();
                    }));
            }
            catch
            {
            }
        }

        void OpenDeleteSkillWindow(object mousePositionObject)
        {
            Vector2 mousePosition = (Vector2) mousePositionObject;
            try
            {
                PopupWindow.Show(new Rect(mousePosition + position.position, new Vector2(500, 0)),
                    new DeleteSkillPopupWindow(PersistenceTool.DeleteSkillFiles));
            }
            catch
            {
            }
        }

        void OpenNodeGenericMenu(NodeEditorView node, Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("删除该节点"), false, () =>
            {
                if (node == null)
                {
                    return;
                }

                data.DeleteNode(node);
            });

            genericMenu.ShowAsContext();
        }

        void OpenConnectionLineGenericMenu(ConnectionLineView connectionLineView, Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("删除连线"), false, () =>
            {
                if (connectionLineView == null)
                {
                    return;
                }

                data.DeleteConnectionLine(connectionLineView);
            });

            genericMenu.ShowAsContext();
        }

        void OpenCommentBoxGenericMenu(CommentBoxView commentBoxView, Vector2 mousePosition)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("删除注释框"), false, () =>
            {
                if (commentBoxView == null)
                {
                    return;
                }

                data.DeleteCommentBox(commentBoxView);
            });

            genericMenu.ShowAsContext();
        }

        #endregion

        #region 计算

        public Vector2 NonZoomedWindowPositionToZoomedWindowPosition(Vector2 nonZoomedPos)
        {
            return guiRect.position + (nonZoomedPos - guiRect.position) / data.GraphZoom;
        }

        public Vector2 ZoomedWindowPositionToNonZoomedWindowPosition(Vector2 nonZoomedPos)
        {
            return (nonZoomedPos - guiRect.position) * data.GraphZoom + guiRect.position;
        }

        public Vector2 GraphPositionToWindowPosition(Vector2 graphPosition)
        {
            return guiRect.center / data.GraphZoom + graphPosition + data.GraphOffset;
        }

        public Vector2 WindowPositionToGraphPosition(Vector2 windowPosition)
        {
            return (windowPosition - guiRect.center - data.GraphOffset * data.GraphZoom) / data.GraphZoom;
        }

        public float NonZoomedSizeToZoomedSize(float nonZoomedSize)
        {
            return nonZoomedSize / data.GraphZoom;
        }

        public Vector2 NonZoomedSizeToZoomedSize(Vector2 nonZoomedSize)
        {
            return new Vector2(NonZoomedSizeToZoomedSize(nonZoomedSize.x), NonZoomedSizeToZoomedSize(nonZoomedSize.y));
        }
        #endregion
    }
}