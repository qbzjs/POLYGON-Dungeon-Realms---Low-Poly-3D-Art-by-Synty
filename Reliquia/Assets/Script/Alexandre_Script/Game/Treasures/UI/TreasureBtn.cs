using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace AlexandreTreasures
{
	public class TreasureBtn : MonoBehaviour
	{
		[Tooltip("Champ renseigné au Runtime.")]
		public TreasureInfo m_treasureInfo;

		[Tooltip("Référence du texte d'UI enfant.")]
		[SerializeField] public TextMeshProUGUI m_text;

		[Tooltip("Champ renseigné au Runtime.")]
		public bool m_isDone;
	}
}