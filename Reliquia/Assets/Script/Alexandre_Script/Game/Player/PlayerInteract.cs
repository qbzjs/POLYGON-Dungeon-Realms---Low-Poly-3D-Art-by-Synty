using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace AlexandreDialogues
{
	public class PlayerInteract : MonoBehaviour, IEnableForDialogue
	{
		// Un dialogue se lance sous différentes conditions 3D :
		//		1. si l'objet entre dans telle zone, alors démarrage automatique,
		//		2. si l'objet entre dans telle zone, et que l'utilisateur le décire, alors démarre manuel,
		//		3. on peut également vouloir déclencher un dialogue indépendamment de la situation 3D.

		#region Private

		[SerializeField] private bool _hasInteractiveZone, _hasTriggerZone;

		[SerializeField] private DialogueRef _dialogueRef;

		[SerializeField] private bool _isActive = true;

		#endregion



		#region Frame cycle

		private void Update()
		{
			if (_isActive)
			{
				if (Input.GetButtonDown("Fire1") && _hasInteractiveZone)
				{
					DialogueManagerStartDialogue();
				}
			}
		}

		#endregion



		#region Methods called by PlayerTriggering

		public void IsTriggerEnter(Collider other)
		{
			if (other.TryGetComponent(out _dialogueRef))
			{
				// Une action selon le type d'interaction
				if (_dialogueRef.m_dialogueRefType == DialogueRefType.InteractiveZone)
				{
					_hasInteractiveZone = true;
				}
				else if (_dialogueRef.m_dialogueRefType == DialogueRefType.TriggerZone)
				{
					_hasTriggerZone = true;
					DialogueManagerStartDialogue();
				}
			}

			// Sinon, rien.
		}

		public void IsTriggerExit(Collider other)
		{
			if (other.TryGetComponent(out _dialogueRef))
			{
				_dialogueRef = null;
				_hasInteractiveZone = false;
				_hasTriggerZone = false;
			}
		}

		#endregion



		#region Private methods

		// appeler la méthode de lancement de dialogue du DialogueManager
		private void DialogueManagerStartDialogue()
		{
			DialogueManager.Instance.DialogueCheckStart(_dialogueRef);
		}

		#endregion



		#region IEnableForDialogue

		public void EnableMe(bool value)
		{
			_isActive = value;
		}

		#endregion
	}
}