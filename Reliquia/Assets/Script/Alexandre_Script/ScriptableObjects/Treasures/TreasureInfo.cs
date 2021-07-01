using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace AlexandreTreasures
{
    [CreateAssetMenu(fileName = "TreasureInfo", menuName = "ScriptableObjects/Treasures/TreasureInfo")]
    public class TreasureInfo : ScriptableObject, IComparable
    {
		// Ceci est le script pour générer le SO d'un trésor en particulier.

		#region Fields

		public string m_name;

        public int m_instanceId;

        public Sprite m_sprite;

        public bool m_isDiscovered;

		#endregion



		#region Public methods

		public int CompareTo(object obj)
        {
            TreasureInfo t = (TreasureInfo)obj;
            return String.Compare(this.m_name, t.m_name); // tri par le m_name
        }

		#endregion
	}
}