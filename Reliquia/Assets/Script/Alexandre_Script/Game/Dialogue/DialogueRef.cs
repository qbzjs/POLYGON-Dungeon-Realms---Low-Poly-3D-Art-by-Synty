using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	public class DialogueRef : MonoBehaviour
	{
		// Le type de ce DialogueRef (influence son déclenchement).
		public DialogueRefType m_dialogueRefType;

		// Le SO contenant la valeur sur laquelle effectuer des conditions de déclenchement.
		public LevelProgress m_levelProgress;

		// Une caméra virtuelle éventuelle (peut rester null).
		public GameObject m_virtualCamera;

		// La liste des entrées de dialogue pour cet objet.
		[SerializeField] public DialogueEntry[] m_myDialogs;
	}
}