using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	[CreateAssetMenu(fileName = "LevelProgress", menuName = "ScriptableObjects/Dialogue/LevelProgress")]
	public class LevelProgress : ScriptableObject
	{
		public int m_levelStep;

		[SerializeField] private int _initValue;

		private void OnEnable()
		{
			m_levelStep = _initValue;
		}
	}
}