using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System.IO;

namespace AlexandreTreasures
{
	public class TreasureManager : MonoBehaviour
	{
		#region Singleton

		private static TreasureManager _instance;
		public static TreasureManager Instance
		{
			get
			{
				if (_instance == null)
				{
					Debug.LogError($"<color=green>{nameof(TreasureManager)}</color> > Instance null.");
				}
				return _instance;
			}

			// pas de setter
		}

		#endregion



		#region Fields

		[SerializeField] private string _fileName;
		private string _filePath;

		[SerializeField] private TreasuresData _treasuresData;

		#endregion



		#region Frame cycle

		private void Awake()
		{
			// Singleton
			_instance = this;

			// Définir le chemin d'enregistrement au démarrage du jeu
			_filePath = Application.persistentDataPath + "/" + _fileName;
		}

		private void Start()
		{
			if (File.Exists(_filePath))
			{
				ReadBonusData();
			}
			else
			{
				SaveBonusData();
			}
		}

		#endregion



		#region Private methods

		private void ReadBonusData()
		{
			// Ouvrir le fichier et le lire
			using (StreamReader reader = new StreamReader(_filePath))
			{
				string json = reader.ReadToEnd();
				TreasureFileData bonusFileData = JsonUtility.FromJson<TreasureFileData>(json);

				// Distribuer les valeurs de découvertes du SO selon les valeurs du fichier
				for (int i = 0; i < bonusFileData.m_discoveries.Length; i++)
				{
					if (bonusFileData.m_discoveries[i].m_instanceId == _treasuresData.m_treasuresList[i].m_instanceId)
					{
						_treasuresData.m_treasuresList[i].m_isDiscovered = bonusFileData.m_discoveries[i].m_isDiscovered;
					}
				}
			}
		}

		private void SaveBonusData()
		{
			TreasureFileData bonusFileData = new TreasureFileData();

			bonusFileData.m_discoveries = new TreasureTypeData[_treasuresData.m_treasuresList.Count];

			for (int i = 0; i < _treasuresData.m_treasuresList.Count; i++)
			{
				bonusFileData.m_discoveries[i] = new TreasureTypeData(_treasuresData.m_treasuresList[i].m_instanceId, _treasuresData.m_treasuresList[i].m_isDiscovered);
			}

			string json = JsonUtility.ToJson(bonusFileData);

			using (StreamWriter file = File.CreateText(_filePath))
			{
				file.Write(json);
			}

		}

		#endregion



		#region Public methods

		public bool CheckIsDiscovered(int instanceId)
		{
			bool result = false;

			foreach (TreasureInfo bonus in _treasuresData.m_treasuresList)
			{
				if (bonus.m_instanceId == instanceId)
				{
					// Renvoyer l'état de la découverte
					result = bonus.m_isDiscovered;

					break;
				}
			}

			return result;
		}

		public void DiscoverThis(int instanceId)
		{
			foreach (TreasureInfo bonus in _treasuresData.m_treasuresList)
			{
				if (bonus.m_instanceId == instanceId)
				{
					// Découvrir le trésor
					bonus.m_isDiscovered = true;

					// Enregistrer les données dans le fichier
					SaveBonusData();

					break;
				}
			}
		}

		#endregion
	}
}