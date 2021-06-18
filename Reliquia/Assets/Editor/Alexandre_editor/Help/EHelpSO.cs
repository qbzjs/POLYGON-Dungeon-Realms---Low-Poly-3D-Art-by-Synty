using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace AlexandreDialogues
{
	[CustomEditor(typeof(HelpSO))]
	public class EHelpSO : Editor
	{
		private HelpSO _target;

		[SerializeField] private GUIStyle _BodyStyle;
		[SerializeField] private GUIStyle _TitleStyle;
		[SerializeField] private GUIStyle _SubTitleStyle;
		[SerializeField] private GUIStyle _LinkStyle;
		[SerializeField] private GUIStyle _FoldOutStyle;

		private bool[] fold;

		private Texture2D _texture;

		private void OnEnable()
		{
			if(_BodyStyle == null)
			{
				_BodyStyle = new GUIStyle(EditorStyles.label);
				_BodyStyle.wordWrap = true;
				_BodyStyle.fontSize = 12;
				_BodyStyle.richText = true;

				_TitleStyle = new GUIStyle(_BodyStyle);
				_TitleStyle.fontSize = 20;

				_SubTitleStyle = new GUIStyle(_BodyStyle);
				_SubTitleStyle.fontSize = 14;

				_LinkStyle = new GUIStyle(_BodyStyle);
				_LinkStyle.wordWrap = false;
				_LinkStyle.fontSize = 14;
				_LinkStyle.normal.textColor = new Color(0x00 / 255f, 0x78 / 255f, 0xDA / 255f, 1f);
				_LinkStyle.stretchWidth = false;

				_texture = new Texture2D(Screen.width, 20);
				Color32 myColor = new Color(58f / 255f, 114f / 255f, 176f / 255f, 0.15f);
				Color32[] myColorArray = _texture.GetPixels32();
				for (int i = 0; i < myColorArray.Length; i++)
				{
					myColorArray[i] = myColor;
				}
				_texture.SetPixels32(myColorArray);
				_texture.Apply();

				_FoldOutStyle = new GUIStyle(_BodyStyle);
				_FoldOutStyle.wordWrap = true;
				_FoldOutStyle.fontSize = 14;
				_FoldOutStyle.padding = new RectOffset(20,20,5,5);
				_FoldOutStyle.margin = new RectOffset(0, 0, 0, 1);
				_FoldOutStyle.normal.background = _texture;
			}

			_target = (HelpSO)target;

			if (_target.m_helpTexts == null)
			{
				return;
			}

			fold = new bool[_target.m_helpTexts.Length];
		}

		public override void OnInspectorGUI()
		{
			if (_target.m_helpTexts == null)
			{
				return;
			}

			//--------------------------------------------------------------------------

			// En-tête

			GUILayout.Space(10);
			GUILayout.Label(_target.m_title, _TitleStyle);
			GUILayout.Space(10);
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Version", _SubTitleStyle, GUILayout.Width(70));
				GUILayout.Label(_target.m_date, _SubTitleStyle);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Auteur", _SubTitleStyle, GUILayout.Width(70));
				GUILayout.Label(_target.m_author, _SubTitleStyle);
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal();
			{
				GUILayout.Label("Contact", _SubTitleStyle, GUILayout.Width(70));
				if (LinkLabel(new GUIContent(_target.m_link)))
				{
					Application.OpenURL(_target.m_link);
				}
			}
			GUILayout.EndHorizontal();
			GUILayout.Space(15);

			//--------------------------------------------------------------------------

			// Corps du texte

			for (int i = 0; i < _target.m_helpTexts.Length; i++)
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Space(-5);
					fold[i] = EditorGUILayout.BeginFoldoutHeaderGroup(fold[i], _target.m_helpTexts[i].m_title, _FoldOutStyle);
				}
				GUILayout.EndHorizontal();
				if (fold[i])
				{
					GUILayout.Space(10);
					for (int j = 0; j < _target.m_helpTexts[i].m_text.Length; j++)
					{
						if(j > 0)
						{
							GUILayout.Space(10);
						}
						GUILayout.Label(_target.m_helpTexts[i].m_text[j], _BodyStyle, GUILayout.Width(Screen.width - 40));
					}
					GUILayout.Space(10);
				}
				EditorGUILayout.EndFoldoutHeaderGroup();
			}

			//--------------------------------------------------------------------------
		}

		private bool LinkLabel(GUIContent label)
		{
			var position = GUILayoutUtility.GetRect(label, _LinkStyle);
			EditorGUIUtility.AddCursorRect(position, MouseCursor.Link);
			return GUI.Button(position, label, _LinkStyle);
		}
	}
}