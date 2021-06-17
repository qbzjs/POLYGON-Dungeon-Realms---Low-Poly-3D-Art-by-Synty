using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	public class UIFunctions : MonoBehaviour
	{
		[SerializeField] private Dialogue _dialogue;
		[SerializeField] private GameObject _virtualCamera;

		public void StartDialogueFromFile()
		{
			// Lancer un dialogue directement 
			DialogueManager.Instance.StartDialogueFromFile(_dialogue, _virtualCamera, true, true);
		}
	}
}