using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	public class StrangeCube : MonoBehaviour
	{
		#region Private

		private Transform _tr;
		[SerializeField] private bool _isActivated = default;
		[SerializeField] private float _rotationSpeed = default;

		#endregion



		#region Frame cycle

		private void Awake()
		{
			_tr = transform;
		}

		private void Update()
		{
			if (_isActivated)
			{
				_tr.Rotate(_rotationSpeed * Time.deltaTime, _rotationSpeed * Time.deltaTime, _rotationSpeed * Time.deltaTime, Space.Self);
			}
		}
		#endregion



		#region Public methods

		public void ActivateMe()
		{
			_isActivated = true;
		}

		#endregion
	}
}
