using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	public class PlayerTriggering : MonoBehaviour
	{
		#region Private

		[SerializeField] private PlayerInteract _playerInteract = default;

		#endregion



		#region Frame cycle

		private void OnTriggerEnter(Collider other)
		{
			_playerInteract.IsTriggerEnter(other);
		}

		private void OnTriggerExit(Collider other)
		{
			_playerInteract.IsTriggerExit(other);
		}

		#endregion
	}
}