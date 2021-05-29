using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	public interface IEnableForDialogue
	{
		// Interface à implémenter pour en lancer la méthode au début ou à la fin d'un dialogue.

		void EnableMe(bool value);
	}
}