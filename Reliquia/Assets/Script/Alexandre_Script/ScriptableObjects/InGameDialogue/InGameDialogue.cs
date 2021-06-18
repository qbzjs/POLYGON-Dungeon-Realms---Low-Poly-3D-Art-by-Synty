using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	[CreateAssetMenu(fileName = "InGameDialogue", menuName = "ScriptableObjects/InGameDialogue/InGameDialogue")]
	public class InGameDialogue : ScriptableObject
	{
		// Tableau de répliques
		public InGameDialogueReply[] m_reply;
	}
}