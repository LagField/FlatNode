using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public class SaveGraphPopupWindow : PopupWindowContent
    {
        public delegate void OnConfirmSave(int id,string graphName,string graphDescription);
        private OnConfirmSave saveCallback;

        private int graphId;
        private string graphName;
        private string graphDescription;
        
        public SaveGraphPopupWindow(int graphId,OnConfirmSave saveCallback)
        {
            this.graphId = graphId;
            this.saveCallback = saveCallback;

            graphName = string.Empty;
            graphDescription = string.Empty;
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("保存行为图",Utility.GetGuiStyle("Title"));
            GUILayout.Space(10);
            
            GUILayout.Label(string.Format("行为图编号：{0}",graphId));
            
            GUILayout.Label("输入行为图名称(必填)",Utility.GetGuiStyle("TitleSmall"));
            GUILayout.BeginHorizontal();
            GUILayout.Label("行为图名称： ",GUILayout.MaxWidth(60));
            graphName = GUILayout.TextField(graphName);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);

            GUILayout.Label("输入行为图说明",Utility.GetGuiStyle("TitleSmall"));
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("行为图说明： ",GUILayout.MaxWidth(60));
            graphDescription = GUILayout.TextField(graphDescription,GUILayout.MinHeight(50));
            GUILayout.EndHorizontal();
            
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("确认保存",GUILayout.MinHeight(40)))
            {
                if (!string.IsNullOrEmpty(graphName))
                {
                    if (saveCallback != null)
                    {
                        saveCallback(graphId,graphName,graphDescription);
                    }

                    if (EditorWindow.focusedWindow != null)
                    {
                        editorWindow.Close();
                    }
                }
            }
        }

        public override Vector2 GetWindowSize()
        {
            return new Vector2(500,300);
        }
    }

}

