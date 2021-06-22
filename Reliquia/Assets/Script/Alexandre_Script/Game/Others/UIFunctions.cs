using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	public class UIFunctions : MonoBehaviour
	{
		[SerializeField] private Dialogue _dialogue = default;
		[SerializeField] private GameObject _virtualCamera = default;
		[SerializeField] private InGameDialogue _inGameDialogue = default;

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