using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace AlexandreTreasures
{
    [CustomEditor(typeof(TreasureManager))]
    public class ETreasureManager : Editor
    {
        private SerializedObject _targetSO;
        private SerializedProperty _fileName, _treasuresData;

		private void OnEnable()
		{
			_targetSO = new SerializedObject(target);

			_fileName = _targetSO.FindProperty("_fileName");
			_treasuresData = _targetSO.FindProperty("_treasuresData");
		}

		public override void OnInspectorGUI()
		{
			_targetSO.Update();

			EditorGUILayout.HelpBox("Design pattern : Singleton.", MessageType.Info);

			EditorGUILayout.Space(10);
			GUILayout.Label("Private fields", EditorStyles.boldLabel);
			EditorGUI.indentLevel++;
			{
				EditorGUILayout.TextField(new GUIContent("File name", "Chemin d'enregistrement, depuis Application.persistendDataPath."), _fileName.stringValue);
				EditorGUILayout.ObjectField(_treasuresData, new GUIContent("Treasures data", "Le ScriptableObject contenant la liste des trésors."));
			}
			EditorGUI.indentLevel--;

			_targetSO.ApplyModifiedProperties();
		}
	}
}