using System;
using UnityEditor;
using UnityEngine;

namespace FlatNode.Editor
{
	public class StringEditPopupWindow : PopupWindowContent
	{
		private string content;
		private Action<string> onEditFinish;

		public StringEditPopupWindow(string content, Action<string> onEditFinish)
		{
			this.content = content;
			this.onEditFinish = onEditFinish;
		}

		public override void OnGUI(Rect rect)
		{
			GUILayout.Label("编辑字符串", Utility.GetGuiStyle("Title"));
			GUILayout.Space(10);

			if (content == null)
			{
				content = string.Empty;
			}

			EditorStyles.textArea.wordWrap = true;
			GUI.SetNextControlName("StringEditArea");
			GUI.FocusControl("StringEditArea");
			content = EditorGUILayout.TextArea(content,EditorStyles.textArea, GUILayout.MinHeight(80));
			
			GUILayout.FlexibleSpace();

			if (GUILayout.Button("确认",GUILayout.MinHeight(30)))
			{
				if (onEditFinish != null)
				{
					onEditFinish(content);
					
					if (EditorWindow.focusedWindow != null)
					{
						editorWindow.Close();
					}
				}
			}
		}
		
		public override Vector2 GetWindowSize()
		{
			return new Vector2(400, 200);
		}
	}
}

