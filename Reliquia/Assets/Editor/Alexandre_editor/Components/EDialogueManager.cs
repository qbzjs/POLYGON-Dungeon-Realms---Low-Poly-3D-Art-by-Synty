using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace AlexandreDialogues
{
	[CustomEditor(typeof(DialogueManager))]
	public class EDialogueManager : Editor
	{
		private SerializedObject _targetSO;
		private SerializedProperty _inventory, _defaultCamera, _canvas, _textLastCharIdentifier, _UICharacterLeft, _UICharacterRight, _replyText;
		private SerializedProperty m_controledScripts, m_conditionsBools;
		private SerializedProperty _levelProgress, _currentCamera, _currentDialogue, _currentUIObject, _currentReply, _isDialogueStarted, _clickState, _isCoroutineStarted, _hashPosition;

		private void OnEnable()
		{
			_targetSO = new SerializedObject(target);

			_inventory = _targetSO.FindProperty("_inventory");
			_defaultCamera = _targetSO.FindProperty("_defaultCamera");
			_canvas = _targetSO.FindProperty("_canvas");
			_textLastCharIdentifier = _targetSO.FindProperty("_textLastCharIdentifier");
			_UICharacterLeft = _targetSO.FindProperty("_UICharacterLeft");
			_UICharacterRight = _targetSO.FindProperty("_UICharacterRight");
			_replyText = _targetSO.FindProperty("_replyText");

			m_controledScripts = _targetSO.FindProperty("m_controledScripts");
			m_conditionsBools = _targetSO.FindProperty("m_conditionsBools");

			_levelProgress = _targetSO.FindProperty("_levelProgress");
			_currentCamera = _targetSO.FindProperty("_currentCamera");
			_currentDialogue = _targetSO.FindProperty("_currentDialogue");
			_currentUIObject = _targetSO.FindProperty("_currentUIObject");
			_currentReply = _targetSO.FindProperty("_currentReply");
			_isDialogueStarted = _targetSO.FindProperty("_isDialogueStarted");
			_clickState = _targetSO.FindProperty("_clickState");
			_isCoroutineStarted = _targetSO.FindProperty("_isCoroutineStarted");
			_hashPosition = _targetSO.FindProperty("_hashPosition");
		}

		public override void OnInspectorGUI()
		{
			_targetSO.Update();

			EditorGUILayout.HelpBox("Design pattern : Singleton.", MessageType.Info);

			EditorGUILayout.Space(10);
			GUILayout.Label("Private fields", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.ObjectField(_inventory, new GUIContent("Inventory", "La référence à l'inventaire."));
			EditorGUILayout.ObjectField(_defaultCamera, new GUIContent("Default camera", "La caméra par défaut qui est utilisée lors d'un dialogue."));
			EditorGUILayout.ObjectField(_canvas, new GUIContent("Canvas", "Le Canvas qui affiche les éléments d'UI relatifs au dialogue. Il est un objet enfant du DialogueManager."));
			EditorGUILayout.PropertyField(_textLastCharIdentifier, new GUIContent("Last char identifier", "Caractère utilisé pour déterminer la fin du texte de la réplique (non affiché)."));
			EditorGUILayout.ObjectField(_replyText, new GUIContent("Reply text", "L'élément d'UI texte pour la réplique."));
			EditorGUILayout.PropertyField(_UICharacterLeft, new GUIContent("UI Character left", "Références des éléments d'UI relatifs au cartouche de gauche."));
			EditorGUILayout.PropertyField(_UICharacterRight, new GUIContent("UI Character right", "Références des éléments d'UI relatifs au cartouche de droite."));
			EditorGUI.indentLevel--;

			EditorGUILayout.Space(10);
			GUILayout.Label("Public fields", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUILayout.PropertyField(m_controledScripts, new GUIContent("Controled scripts", "Liste des scripts implémentant l'interface IEnableForDialogue"));
			EditorGUI.indentLevel--;

			EditorGUILayout.Space(10);
			GUILayout.Label("Tracked fields", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			EditorGUI.BeginDisabledGroup(true);
			EditorGUILayout.ObjectField(_levelProgress, new GUIContent("Level progress", "Le ScriptableObject de progression utilisé pour telle entrée de dialogue."));
			EditorGUILayout.ObjectField(_currentCamera, new GUIContent("Current camera", "La caméra utilisée pour telle entrée de dialogue."));
			EditorGUILayout.ObjectField(_currentDialogue, new GUIContent("Current dialogue", "Le dialogue en cours."));
			EditorGUILayout.PropertyField(m_conditionsBools, new GUIContent("Conditions bools", "Liste de booléens, chacun d'eux renvoyés par une condition personnalisée."));
			EditorGUILayout.PropertyField(_currentUIObject, new GUIContent("Current UI Object", "Les éléments d'UI utilisés actuellement."));
			EditorGUILayout.PropertyField(_currentReply, new GUIContent("Current reply", "Le numéro de la réplique actuelle."));
			EditorGUILayout.PropertyField(_isDialogueStarted, new GUIContent("Is dialogue started", "Le dialogue est-il lancé ?"));
			EditorGUILayout.PropertyField(_clickState, new GUIContent("Click state", "L'état du clic (0 ou 1) déterminant le contrôle attendu : afficher le texte ou passer la réplique."));
			EditorGUILayout.PropertyField(_isCoroutineStarted, new GUIContent("Is coroutine started", "La coroutine d'affichage des lettres est-elle lancée ?"));
			EditorGUILayout.PropertyField(_hashPosition, new GUIContent("Hash position", "La position du caractère identifiant une fin de texte."));
			EditorGUI.indentLevel--;
			EditorGUI.EndDisabledGroup();

			_targetSO.ApplyModifiedProperties();
		}

	}
}