using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	public class UIFunctions : MonoBehaviour
	{
		[SerializeField] private Dialogue _dialogue;
		[SerializeField] private GameObject _virtualCamera;
		[SerializeField] private InGameDialogue _inGameDialogue;

		public void StartDialogueFromFile()
		{
			// Lancer un dialogue Dialogue Box 
			DialogueManager.Instance.StartDialogueFromFile(_dialogue, _virtualCamera, true, true);
		}

		public void StartInGameDialogue()
		{
			// Lancer un dialogue In Game
			InGameDialogueManager.Instance.StartDialogue(_inGameDialogue);
		}
	}
}