using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
    public class SaveGraphPopupWindow : PopupWindowContent
    {
        public delegate void OnConfirmSave(int id,string skillName,string skillDescription);
        private OnConfirmSave saveCallback;

        private int skillId;
        private string skillName;
        private string skillDescription;
        
        public SaveGraphPopupWindow(int skillId,OnConfirmSave saveCallback)
        {
            this.skillId = skillId;
            this.saveCallback = saveCallback;

            skillName = string.Empty;
            skillDescription = string.Empty;
        }

        public override void OnGUI(Rect rect)
        {
            GUILayout.Label("保存行为图",Utility.GetGuiStyle("Title"));
            GUILayout.Space(10);
            
            GUILayout.Label(string.Format("行为图编号：{0}",skillId));
            
            GUILayout.Label("输入行为图名称(必填)",Utility.GetGuiStyle("TitleLittle"));
            GUILayout.BeginHorizontal();
            GUILayout.Label("行为图名称： ",GUILayout.MaxWidth(60));
            skillName = GUILayout.TextField(skillName);
            GUILayout.EndHorizontal();
            
            GUILayout.Space(10);

            GUILayout.Label("输入行为图说明",Utility.GetGuiStyle("TitleLittle"));
            
            GUILayout.BeginHorizontal();
            GUILayout.Label("行为图说明： ",GUILayout.MaxWidth(60));
            skillDescription = GUILayout.TextField(skillDescription,GUILayout.MinHeight(50));
            GUILayout.EndHorizontal();
            
            GUILayout.FlexibleSpace();
            
            if (GUILayout.Button("确认保存",GUILayout.MinHeight(40)))
            {
                if (!string.IsNullOrEmpty(skillName))
                {
                    if (saveCallback != null)
                    {
                        saveCallback(skillId,skillName,skillDescription);
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

