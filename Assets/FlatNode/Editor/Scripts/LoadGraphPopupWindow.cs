using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public class LoadGraphPopupWindow : PopupWindowContent
    {
        public delegate void OnConfirmLoad(int id);

        private OnConfirmLoad loadCallback;

        private Vector2 scrollPosition;

        public LoadGraphPopupWindow(OnConfirmLoad loadCallback)
        {
            this.loadCallback = loadCallback;
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("载入行为图", Utility.GetGuiStyle("Title"));
            GUILayout.Space(10);

            List<GraphBaseInfo> graphBaseInfoList = PersistenceTool.LoadGraphBaseInfoList();
            if (graphBaseInfoList == null)
            {
                GUILayout.Label("没有图可以载入", Utility.GetGuiStyle("LoadSkillHint"));
                return;
            }

            scrollPosition = GUILayout.BeginScrollView(scrollPosition);
            for (int i = 0; i < graphBaseInfoList.Count; i++)
            {
                GraphBaseInfo graphBaseInfo = graphBaseInfoList[i];
                if (GUILayout.Button(string.Format("{0}.{1}    {2}", graphBaseInfo.graphId, graphBaseInfo.graphName,
                    graphBaseInfo.graphDescription)))
                {
                    if (loadCallback != null)
                    {
                        loadCallback(graphBaseInfo.graphId);
                        
                        if (EditorWindow.focusedWindow != null)
                        {
                            editorWindow.Close();
                        }
                        break;
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