using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;

namespace AlexandreDialogues
{
	public class EHelpMenu
	{
		private static string _root = "Assets/Editor/Alexandre_editor/Help/";

		[MenuItem("Tools/Reliquia/Dialogue Box/Help file", false, 0)]
		private static void BoxHelp()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath($"{_root}DialogueBox/HelpFile.asset"); ;
		}

		[MenuItem("Tools/Reliquia/Dialogue Box/Notice", false, 1)]
		private static void BoxNotice()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath($"{_root}DialogueBox/Dialogue system - notice.pdf"); ;
		}

		[MenuItem("Tools/Reliquia/Dialogue Box/Documentation", false, 3)]
		private static void BoxDoc()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath($"{_root}DialogueBox/Dialogue system - documentation.pdf"); ;
		}

		[MenuItem("Tools/Reliquia/Dialogue Box/CSV Dispatcher", false, 14)]
		public static void CVSDispatcher()
		{
			ECSVDispatcher.ShowWindow();
		}



		[MenuItem("Tools/Reliquia/Dialogue InGame/Help file", false, 0)]
		private static void InGameHelp()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath($"{_root}InGameDialogue/HelpFile.asset"); ;
		}

		[MenuItem("Tools/Reliquia/Dialogue InGame/Notice", false, 1)]
		private static void InGameNotice()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath($"{_root}InGameDialogue/In Game Dialogue - notice.pdf"); ;
		}

		[MenuItem("Tools/Reliquia/Dialogue InGame/Documentation", false, 3)]
		private static void InGameDoc()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath($"{_root}InGameDialogue/In Game Dialogue - documentation.pdf"); ;
		}

		[MenuItem("Tools/Reliquia/Dialogue InGame/CSV Dispatcher InGame", false, 14)]
		public static void CVSDispatcherIngame()
		{
			ECSVDispatcherInGame.ShowWindow();
		}



		[MenuItem("Tools/Reliquia/Treasures/Help file", false, 0)]
		private static void TreasuresHelp()
		{
			Selection.activeObject = AssetDatabase.LoadMainAssetAtPath($"{_root}Treasures/HelpFile.asset"); ;
		}
	}
}