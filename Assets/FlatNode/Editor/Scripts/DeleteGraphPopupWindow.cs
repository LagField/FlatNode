using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public class DeleteGraphPopupWindow : PopupWindowContent
    {
        public delegate void OnConfirmDelete(int id);

        private OnConfirmDelete deleteCallback;

        private Vector2 scrollPosition;

        public DeleteGraphPopupWindow(OnConfirmDelete deleteCallback)
        {
            this.deleteCallback = deleteCallback;
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("删除行为图", Utility.GetGuiStyle("DeleteFileTitle"));
            GUILayout.Space(10);

            List<GraphBaseInfo> graphBaseInfoList = PersistenceTool.LoadGraphBaseInfoList();
            if (graphBaseInfoList == null)
            {
                GUILayout.Label("没有图可以删除", Utility.GetGuiStyle("LoadSkillHint"));
                return;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < graphBaseInfoList.Count; i++)
            {
                GraphBaseInfo graphBaseInfo = graphBaseInfoList[i];
                if (GUILayout.Button(string.Format("删除   {0}.{1}    {2}", graphBaseInfo.graphId, graphBaseInfo.graphName,
                    graphBaseInfo.graphDescription)))
                {
                    if (EditorUtility.DisplayDialog("删除行为图",
                        string.Format("是否删除行为图 {0}:{1} \n 描述:{2}", graphBaseInfo.graphId, graphBaseInfo.graphName,
                            graphBaseInfo.graphDescription), "ok"))
                    {
                        if (deleteCallback != null)
                        {
                            deleteCallback(graphBaseInfo.graphId);

                            if (EditorWindow.focusedWindow != null)
                            {
                                editorWindow.Close();
                            }

                            break;
                        }
                    }
                }
            }

            GUILayout.EndScrollView();
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(500, 300);
        }
    }
}