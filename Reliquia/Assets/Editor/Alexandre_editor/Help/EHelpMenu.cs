using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace AlexandreDialogues
{
	public class EHelpMenu
	{
		[MenuItem("Tools/Reliquia/Dialogue/Help file")]
		private static void NestedOption1()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Editor/Alexandre_editor/Help/HelpFile.asset"); ;
		}

		[MenuItem("Tools/Reliquia/Dialogue/Notice")]
		private static void NestedOption2()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Editor/Alexandre_editor/Help/Dialogue system - notice.pdf"); ;
		}

		[MenuItem("Tools/Reliquia/Dialogue/Documentation")]
		private static void NestedOption3()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Editor/Alexandre_editor/Help/Dialogue system - documentation.pdf"); ;
		}
	}
}