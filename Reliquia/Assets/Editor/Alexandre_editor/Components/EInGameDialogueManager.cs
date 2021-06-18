using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace AlexandreDialogues
{
	[CustomEditor(typeof(InGameDialogueManager))]
	public class EInGameDialogueManager : Editor
	{
		private SerializedObject _targetSO;
		private SerializedProperty _canvas, _textTMPro;
		private SerializedProperty _isDialogueStarted, _dialogueReplyLength, _dialogueCurrentReply, _dialogueCurrentTime;

		private void OnEnable()
		{
			_targetSO = new SerializedObject(target);

			_canvas = _targetSO.FindProperty("_canvas");
			_textTMPro = _targetSO.FindProperty("_textTMPro");

			_isDialogueStarted = _targetSO.FindProperty("_isDialogueStarted");
			_dialogueReplyLength = _targetSO.FindProperty("_dialogueReplyLength");
			_dialogueCurrentReply = _targetSO.FindProperty("_dialogueCurrentReply");
			_dialogueCurrentTime = _targetSO.FindProperty("_dialogueCurrentTime");

		}

		public override void OnInspectorGUI()
		{
			_targetSO.Update();

			// ---------------------------------------------------------------------------

			EditorGUILayout.HelpBox("Design pattern : Singleton.", MessageType.Info);

			// ---------------------------------------------------------------------------

			// Private fields

			EditorGUILayout.Space(10);
			GUILayout.Label("Private fields", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			{
				EditorGUILayout.ObjectField(_canvas, new GUIContent("Canvas", "L'objet Canvas enfant par lequel le contenu s'affiche."));
				EditorGUILayout.ObjectField(_textTMPro, new GUIContent("Text TMPro", "L'objet TextMeshPro du texte à l'écran."));
			}
			EditorGUI.indentLevel--;

			// ---------------------------------------------------------------------------

			// Tracked fields

			EditorGUILayout.Space(10);
			GUILayout.Label("Tracked fields", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			{
				EditorGUI.BeginDisabledGroup(true);
				{
					EditorGUILayout.PropertyField(_isDialogueStarted, new GUIContent("Is dialogue started", "Le dialogue est-il en cours ?"));
					EditorGUILayout.PropertyField(_dialogueReplyLength, new GUIContent("Replies number", "Le nombre de réplique pour ce dialogue."));
					EditorGUILayout.PropertyField(_dialogueCurrentReply, new GUIContent("Current reply", "L'index de la réplique actuelle."));
					EditorGUILayout.PropertyField(_dialogueCurrentTime, new GUIContent("Current time", "La durée à l'écran de la réplique actuelle."));
				}
				EditorGUI.EndDisabledGroup();
			}
			EditorGUI.indentLevel--;

			// ---------------------------------------------------------------------------

			_targetSO.ApplyModifiedProperties();
		}
	}
}