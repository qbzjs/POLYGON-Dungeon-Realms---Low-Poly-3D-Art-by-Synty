using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace AlexandreDialogues
{
	public class EHelpMenu
	{
		[MenuItem("Tools/Reliquia/Dialogue/Help file", false, 0)]
		private static void NestedOption1()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Editor/Alexandre_editor/Help/HelpFile.asset"); ;
		}

		[MenuItem("Tools/Reliquia/Dialogue/Notice", false, 1)]
		private static void NestedOption2()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Editor/Alexandre_editor/Help/Dialogue system - notice.pdf"); ;
		}

		[MenuItem("Tools/Reliquia/Dialogue/Documentation", false, 2)]
		private static void NestedOption3()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath("Assets/Editor/Alexandre_editor/Help/Dialogue system - documentation.pdf"); ;
		}

		[MenuItem("Tools/Reliquia/Dialogue/CSV Dispatcher", false, 14)] 
		public static void CVSDispatcher()
		{
			ECSVDispatcher.ShowWindow();
		}
	}
}