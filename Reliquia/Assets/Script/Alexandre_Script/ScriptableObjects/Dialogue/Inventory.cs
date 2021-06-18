using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	[CreateAssetMenu(fileName = "Inventory", menuName = "ScriptableObjects/Dialogue/Inventory")]
	public class Inventory : ScriptableObject
	{
		public string[] m_inventoryStrings;
	}
}