using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace AlexandreDialogues
{
	[CustomEditor(typeof(DialogueRef))]
	public class EDialogueRef : Editor
	{
		private DialogueRef _scriptContent;
		private SerializedObject _so;

		private void OnEnable()
		{
			_scriptContent = (DialogueRef)target;
			_so = new SerializedObject(target);
		}

		public override void OnInspectorGUI()
		{
			_scriptContent.m_dialogueRefType = (DialogueRefType)EditorGUILayout.EnumPopup(new GUIContent("Type", "Quel type d'interaction : choix de l'utilisateur ou déclenchement automatique ?"), _scriptContent.m_dialogueRefType);
			_scriptContent.m_levelProgress = (LevelProgress)EditorGUILayout.ObjectField(new GUIContent("LevelProgress", "ScriptableObject du niveau de progression."), _scriptContent.m_levelProgress, typeof(LevelProgress), false);
			_scriptContent.m_virtualCamera = (GameObject)EditorGUILayout.ObjectField(new GUIContent("Virtual camera", "(Option) Une caméra virtuelle Cinemachine."), _scriptContent.m_virtualCamera, typeof(GameObject), true);

			EditorGUILayout.BeginHorizontal();
			GUILayout.Label(new GUIContent("Dialogue entries", "Les entrées de dialogues."), GUILayout.Width(178));
			if (GUILayout.Button("Open Dialogue Entries"))
			{
				EDialogueRefWindow.Open(_so, _scriptContent);
			}
			EditorGUILayout.EndHorizontal();
		}
	}
}
