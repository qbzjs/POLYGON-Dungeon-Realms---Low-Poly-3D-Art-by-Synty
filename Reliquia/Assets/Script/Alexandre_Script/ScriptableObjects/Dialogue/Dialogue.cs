using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	[CreateAssetMenu(fileName = "Dialogue", menuName = "ScriptableObjects/Dialogue/Dialogue")]
	public class Dialogue : ScriptableObject
	{
		// Pour tester
		public string m_test;

		// Tableau des rôles
		public Role[] m_casting;

		// Tableau des répliques
		public Reply[] m_replies;
	}
}