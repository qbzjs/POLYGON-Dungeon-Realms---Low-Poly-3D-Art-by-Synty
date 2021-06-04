using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace AlexandreDialogues
{
	[System.Serializable]
	public class DialogueEntry // on veut disposer d'un type nullable
	{
		// Nom du dialogue
		public string m_name;

		// Au début du dialogue, utilise-t-on la liste des ControledScript à désactiver ?
		// Si oui : la méthode correspondante est appelée. Si non : rien.
		public bool m_useControledScriptsAtStart;

		// Au début du dialogue, un événement est déclenché.
		[SerializeField] public UnityEvent m_eventAtStart = new UnityEvent();

		// Quel est le dialogue ?
		public Dialogue m_dialogueSO;

		// Les conditions de déclenchement (cumulées) quant au niveau actuel.
		public StartConditionsTest[] m_startConditions;

		// Si les conditions sur le niveau sont remplies, il reste des conditions d'objet ou des conditions personnalisées.
		// Si ces conditions ne sont pas remplies, on peut utiliser un dialogue alternatif optionnel. Lequel ?
		public Dialogue m_dialogueAltSO;

		// A la fin du dialogue, le niveau change-t-il ? Si oui, quel est le prochain niveau ?
		public bool m_levelChangesAtEnd;
		public int m_nextLevel;

		// A la fin du dialogue, utilise-t-on la liste des ControledScript à réactiver ?
		// Si oui : la méthode correspondante est appelée. Si non : rien.
		public bool m_useControledScriptsAtEnd;

		// A la fin du dialogue, un événement est déclenché.
		public UnityEvent m_eventAtEnd = new UnityEvent();
	}
}