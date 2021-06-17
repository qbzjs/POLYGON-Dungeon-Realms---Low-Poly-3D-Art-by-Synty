using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	public class PlayerMove : MonoBehaviour, IEnableForDialogue
	{
		#region Private

		[SerializeField] private Transform _tr;
		[SerializeField] private float _moveSpeed;
		[SerializeField] private float _rotationSpeed;

		private Vector3 _inputAxis;

		[SerializeField] private bool _isActive;

		#endregion



		#region Frame cycle

		private void Update()
		{
			if (_isActive)
			{
				_inputAxis = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));
				_inputAxis.Normalize();

				Vector3 forward = Vector3.forward * _inputAxis.z;
				Vector3 right = Vector3.right * _inputAxis.x;
				Vector3 combined = forward + right;

				_tr.Translate(combined * _moveSpeed * Time.deltaTime);

				float mouseX = Input.GetAxis("Mouse X");
				_tr.Rotate(_tr.up, mouseX * _rotationSpeed * Time.deltaTime);
			}
		}

		#endregion



		#region IDisableForDialogue

		public void EnableMe(bool value)
		{
			_isActive = value;
		}

		#endregion
	}
}