using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	[CreateAssetMenu(fileName = "DialogueCharacter", menuName = "ScriptableObjects/Dialogue/Character")]
	public class Character : ScriptableObject
	{
		// Les images gauche et droite
		public Sprite m_spriteLeft, m_spriteRight;

		// Le nom
		public string m_name;
	}
}
