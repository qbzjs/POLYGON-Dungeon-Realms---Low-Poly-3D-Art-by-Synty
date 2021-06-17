using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	public class CustomConditions : MonoBehaviour
	{
		public void MyCustomMethod()
		{
			// du code ....
			// mes tests...
			// etc.

			// Renvoyer le résultat booléen à DialogueManager avec une méthode de ce dernier :
			DialogueManager.Instance.m_conditionsBools.Add(false);
		}
	}
}