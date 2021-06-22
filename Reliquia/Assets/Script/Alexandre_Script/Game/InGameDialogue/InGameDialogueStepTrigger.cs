﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	public class InGameDialogueStepTrigger : MonoBehaviour
	{
		#region Private

		[Tooltip("Un fichier ScriptableObject de dialogue in-game.")]
		[SerializeField] private InGameDialogue _inGameDialogue = default;
		[Tooltip("Un fichier ScriptableObject de progression.")]
		[SerializeField] private LevelProgress _levelProgress = default;
		[Tooltip("La valeur à laquelle lancer le dialogue.")]
		[SerializeField] private int _value = default;
		[Tooltip("Le tag d'objet à tester lors du OnTriggerEnter")]
		[SerializeField] private string _tag = default;

		#endregion



		#region Frame cycle

		private void OnTriggerEnter(Collider other)
		{
			if (_levelProgress.m_levelStep == _value)
			{
				if (other.CompareTag(_tag))
				{
					InGameDialogueManager.Instance.StartDialogue(_inGameDialogue);
					_levelProgress.m_levelStep++;
				}
			}
		}

		#endregion
	}
}