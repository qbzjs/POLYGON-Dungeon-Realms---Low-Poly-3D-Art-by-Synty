using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreTreasures
{
	public class UITreasuresFunctions : MonoBehaviour
	{
		[SerializeField] private Treasure _supercube, _capsuleMetal, _molecule = default;
		[SerializeField] private GameObject _treasuresPanel = default;
		[SerializeField] private GameObject _canvas = default;



		public void GrabSuperCube()
		{
			_supercube.GrabMe();
		}

		public void GrabMolecule()
		{
			_molecule.GrabMe();
		}

		public void GrabCapsuleMetal()
		{
			_capsuleMetal.GrabMe();

		}


		private void Update()
		{
			_canvas.SetActive(!_treasuresPanel.activeInHierarchy);
		}
	}
}