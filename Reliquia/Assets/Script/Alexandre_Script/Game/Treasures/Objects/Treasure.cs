using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreTreasures
{
	public class Treasure : MonoBehaviour
	{
		// Ce script de composant fait d'un objet un trésor à découvrir. Requiert TreasureManager en scène.

		#region Fields

		private Transform _tr;
		private GameObject _parent;

		[Tooltip("Un fichier ScriptableObject de trésor.")]
		[SerializeField] private TreasureInfo _treasure;

		//[Tooltip("Le fichier ScriptableObject de la liste des trésors.")]
		//[SerializeField] private TreasuresData _treasuresData;

		#endregion



		#region Frame cycle

		private void Awake()
		{
			_tr = transform;
			_parent = _tr.parent.gameObject;
		}

		private void Start()
		{
			if (TreasureManager.Instance == null)
			{
				Debug.LogError("Le script Treasure requiert un TreasureManager dans la scène.");
				return;
			}

			// A-t-on découvert ce bonus ? Si oui, désactiver l'objet
			if (TreasureManager.Instance.CheckIsDiscovered(_treasure.m_instanceId))
			{
				_parent.SetActive(false);
			}
		}

		#endregion



		#region Public methods

		public void GrabMe()
		{
			if(_tr.gameObject.activeInHierarchy)
			{
				TreasureManager.Instance.DiscoverThis(_treasure.m_instanceId);
				_parent.SetActive(false);
			}
		}

		#endregion

	}
}