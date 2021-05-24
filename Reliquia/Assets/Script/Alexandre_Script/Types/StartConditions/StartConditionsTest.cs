using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEngine.Events;

namespace AlexandreDialogues
{
	//[System.Serializable]
	//public class ConditionEvent : UnityEvent<bool> { }

	[System.Serializable]
	public class StartConditionsTest
	{
		// Lancer un dialogue s'effectue sous des conditions (cumulées),
		//		- de niveau,
		//		- d'objet(s) dans l'inventaire,
		//		- de méthode(s) personnalisées.

		// Le type de la condition.
		public StartConditionsTestType m_type;

		// Si c'est une condition de niveau, alors on a besoin d'un opérateur et d'une valeur.
		public StartConditionsOperator m_operator;
		public int m_level;

		// Si c'est une condition d'objet, alors on a besoin d'un objet.
		public string m_object;

		// Si c'est une condition personnalisée, alors renseigner un événement avec des méthodes.
		// public UnityEvent m_eventCondition = new UnityEvent();
		public UnityEvent m_eventCondition = new UnityEvent();
	}
}