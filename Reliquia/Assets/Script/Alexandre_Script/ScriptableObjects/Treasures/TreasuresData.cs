using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreTreasures
{
	[CreateAssetMenu(fileName = "TreasuresData", menuName = "ScriptableObjects/Treasures/TreasuresData")]
	public class TreasuresData : ScriptableObject
	{
		// Ceci est le script pour générer un SO contenant la liste des trésors du jeu.

		#region Fields

		public List<TreasureInfo> m_treasuresList = new List<TreasureInfo>();

		#endregion



		#region Frame cycle

		private void OnEnable()
		{
			m_treasuresList.Sort();
		}

		#endregion



		#region Public methods

		// Un menu contextuel pour réinitialiser les découvertes (clic droit ou bouton rouage)
		[ContextMenu("Reset discoveries")]
		private void ResetDiscoveries()
		{
			foreach (TreasureInfo item in m_treasuresList)
			{
				item.m_isDiscovered = false;
			}
		}

		#endregion

	}
}