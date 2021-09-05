using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace AlexandreDialogues
{
	[CustomEditor(typeof(InGameDialogue))]
	public class EInGameDialogue : Editor
	{
		private SerializedObject _targetSO;

		private SerializedProperty _replies;

		private float _displayTimeMin = 0f;
		private float _displayTimeMax = 10f;
		private float _displayTimeDefault = 1f;

		private GUIStyle _textareaStyle;



		private void OnEnable()
		{
			_targetSO = new SerializedObject(target);

			_replies = _targetSO.FindProperty("m_reply");

			if (_textareaStyle == null)
			{
				// retours ligne auto pour les TextAreas
				_textareaStyle = new GUIStyle(EditorStyles.textArea);
				_textareaStyle.wordWrap = true;
			}
		}

		public override void OnInspectorGUI()
		{
			_targetSO.Update(); 

			GUILayout.Space(10);
			GUILayout.Label("In game Dialogue", EditorStyles.boldLabel);
			GUILayout.Space(20);

			for (int i = 0; i < _replies.arraySize; i++)
			{
				GUILayout.BeginHorizontal();
				{
					GUILayout.Label($"Reply {i}", EditorStyles.boldLabel);
					if (GUILayout.Button(new GUIContent("-", "Supprimer l'entrée."), GUILayout.Width(20)))
					{
						_replies.DeleteArrayElementAtIndex(i); 
						_targetSO.ApplyModifiedProperties();
						Defocus();
						break;
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(10);
				EditorGUI.indentLevel++;
				GUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField(new GUIContent("Text", "Le texte de la réplique. Peut accueillir des styles de mise en forme, des retours à la ligne et tabulations."), GUILayout.Width(100));
					_replies.GetArrayElementAtIndex(i).FindPropertyRelative("m_text").stringValue = EditorGUILayout.TextArea(_replies.GetArrayElementAtIndex(i).FindPropertyRelative("m_text").stringValue, _textareaStyle);
				}
				GUILayout.EndHorizontal();
				GUILayout.BeginHorizontal();
				{
					EditorGUILayout.LabelField(new GUIContent("Display time", "Durée d'affichage du texte à l'écran mesurée en secondes."), GUILayout.Width(100));
					_replies.GetArrayElementAtIndex(i).FindPropertyRelative("m_displayTime").floatValue = EditorGUILayout.Slider(_replies.GetArrayElementAtIndex(i).FindPropertyRelative("m_displayTime").floatValue, _displayTimeMin, _displayTimeMax);
				}
				GUILayout.EndHorizontal();
				EditorGUI.indentLevel--;
				GUILayout.Space(15);
			}

			if (GUILayout.Button(new GUIContent("Add reply", "Ajouter une réplique.")))
			{
				_replies.arraySize++;
				_replies.GetArrayElementAtIndex(_replies.arraySize - 1).FindPropertyRelative("m_text").stringValue = string.Empty;
				_replies.GetArrayElementAtIndex(_replies.arraySize - 1).FindPropertyRelative("m_displayTime").floatValue = _displayTimeDefault;
				_targetSO.ApplyModifiedProperties();
			}

			_targetSO.ApplyModifiedProperties();
		}


		private void Defocus()
		{
			GUI.SetNextControlName("");
			GUI.TextField(new Rect(0, 0, 0, 0), "");
			GUI.FocusControl("");
		}
	}
}