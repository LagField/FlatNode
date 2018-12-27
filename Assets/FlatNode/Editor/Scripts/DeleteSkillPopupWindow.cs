using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public class DeleteSkillPopupWindow : PopupWindowContent
    {
        public delegate void OnConfirmDelete(int id);

        private OnConfirmDelete deleteCallback;

        private Vector2 scrollPosition;

        public DeleteSkillPopupWindow(OnConfirmDelete deleteCallback)
        {
            this.deleteCallback = deleteCallback;
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("删除技能", Utility.GetGuiStyle("DeleteFileTitle"));
            GUILayout.Space(10);

            List<GraphBaseInfo> skillBaseInfoList = PersistenceTool.LoadGraphBaseInfoList();
            if (skillBaseInfoList == null)
            {
                GUILayout.Label("没有技能可以删除", Utility.GetGuiStyle("LoadSkillHint"));
                return;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < skillBaseInfoList.Count; i++)
            {
                GraphBaseInfo graphBaseInfo = skillBaseInfoList[i];
                if (GUILayout.Button(string.Format("删除   {0}.{1}    {2}", graphBaseInfo.graphId, graphBaseInfo.graphName,
                    graphBaseInfo.graphDescription)))
                {
                    if (EditorUtility.DisplayDialog("删除技能",
                        string.Format("是否删除技能 {0}:{1} \n 描述:{2}", graphBaseInfo.graphId, graphBaseInfo.graphName,
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