using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace AlexandreDialogues
{
	[CustomEditor(typeof(Dialogue))]
	public class EDialogue : Editor
	{
		private Dialogue _target;
		private SerializedObject _targetSO;

		private void OnEnable()
		{
			_target = (Dialogue)target;
			_targetSO = new SerializedObject(target);
		}

		public override void OnInspectorGUI()
		{
			if (GUILayout.Button("Open Dialogue"))
			{
				EDialogueWindow.Create(_targetSO, _target);
			}
		}
	}
}
