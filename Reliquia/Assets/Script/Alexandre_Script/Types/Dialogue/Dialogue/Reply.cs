using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace AlexandreDialogues
{
	[System.Serializable]
	public class Reply
	{
		// Le rôle de cette réplique
		public Role m_role;

		// Les didascalies, séparées par des virgules
		public string m_stageDirections;

		// La couleur dans laquelle afficher les didascalies
		public int m_styleColorHash;

		// Temps d'affichage par lettre et par espace
		public float m_charSpeed;
		public float m_spaceSpeed;

		// Le texte
		public string m_text;

		// Un événement éventuel se déclenchant lors de cette réplique
		public UnityEvent m_replyEvent = new UnityEvent();
	}
}
