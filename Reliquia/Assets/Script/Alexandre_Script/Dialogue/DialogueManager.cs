using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;

namespace AlexandreDialogues
{
	public class DialogueManager : MonoBehaviour
	{
		#region Singleton

		private static DialogueManager _instance;
		public static DialogueManager Instance
		{
			get
			{
				if (_instance == null)
				{
					Debug.LogError("DialogueManager > Instance null.");
				}
				return _instance;
			}

			// pas de setter
		}

		#endregion



		#region Fields

		[Header("Private")]
		[SerializeField] private Inventory _inventory;
		[SerializeField] private GameObject _defaultCamera;

		[Header("Public")]
		// pour la liste des objets à contrôler (Interfaces non sérialisables donc contourner)
		public List<MonoBehaviour> m_controledScripts = new List<MonoBehaviour>();
		public List<IEnableForDialogue> m_IEnableForDialogue;

		[SerializeField] public List<bool> m_conditionsBools = new List<bool>();
		
		[Header("Debug")]
		[SerializeField] private LevelProgress _levelProgress;
		[SerializeField] private GameObject _currentCamera;
		[SerializeField] private Dialogue _currentDialogue;

		#endregion



		#region Frame cycle

		void Awake()
		{
			_instance = this;

			// Contournement des Interfaces non sérialisées : remplir la liste des IEnableForDialogue à partir de la liste des MonoBehaviour
			m_IEnableForDialogue = new List<IEnableForDialogue>();
			foreach (MonoBehaviour item in m_controledScripts)
			{
				if (item is IEnableForDialogue)
				{
					m_IEnableForDialogue.Add((IEnableForDialogue)item);
				}
			}
		}

		#endregion



		#region Public methods

		public void DialogueCheckStart(DialogueRef dialogueRef)
		{
			// S'il y a au moins une entrée de dialogue.
			if (dialogueRef.m_myDialogs.Length > 0 && dialogueRef.m_levelProgress != null)
			{
				// Référencer de progression utilisée.
				_levelProgress = dialogueRef.m_levelProgress;

				// A-t-on une caméra virtuelle pour ce dialogue ?
				_currentCamera = dialogueRef.m_virtualCamera;
				if (_currentCamera == null)
				{
					_currentCamera = _defaultCamera;
				}

				// Valeur booléenne sur laquelle itérer les conditions.
				bool result = true;

				// une liste d'objets
				List<string> objects = new List<string>();

				// Pour chaque entrée de dialogue
				foreach (DialogueEntry entry in dialogueRef.m_myDialogs)
				{
					// L'entrée a-t-elle un nom ?
					if (entry.m_name == string.Empty)
					{
						Debug.LogError($"<color=magenta>DialogueManager</color> Dans {dialogueRef}, une entrée de dialogue n'a pas de nom.");
					}

					// A-t-on un dialogue ?
					if (entry.m_dialogueSO == null)
					{
						Debug.LogError($"<color=magenta>DialogueManager</color> Dans {dialogueRef}, cette entrée n'a pas de dialogue : {entry.m_name}.");
					}

					// pour chaque condition de démarrage de ce dialogue
					foreach (StartConditionsTest condition in entry.m_startConditions)
					{
						// si le résultat est vrai, on peut tester la suite ; sinon, c'est que la condition précédente n'est pas remplie et on annule
						if (result)
						{
							// selon le type de condition
							if (condition.m_type == StartConditionsTestType.Level)
							{
								// tester cette condition selon l'opérateur
								switch (condition.m_operator)
								{
									case StartConditionsOperator.EqualTo:
										result = _levelProgress.m_levelStep == condition.m_level;
										break;
									case StartConditionsOperator.NotEqualTo:
										result = _levelProgress.m_levelStep != condition.m_level;
										break;
									case StartConditionsOperator.LessThan:
										result = _levelProgress.m_levelStep < condition.m_level;
										break;
									case StartConditionsOperator.LessThanOrEqualTo:
										result = _levelProgress.m_levelStep <= condition.m_level;
										break;
									case StartConditionsOperator.GreaterThan:
										result = _levelProgress.m_levelStep > condition.m_level;
										break;
									case StartConditionsOperator.GreaterThanOrEqualTo:
										result = _levelProgress.m_levelStep >= condition.m_level;
										break;
									default:
										break;
								}
							}
							else if (condition.m_type == StartConditionsTestType.Object)
							{
								// ajouter l'objet à la liste
								objects.Add(condition.m_object);
							}
							else if (condition.m_type == StartConditionsTestType.Method)
							{
								// lancer l'événement
								condition.m_eventCondition.Invoke();
							}
						}
					}

					// Si les conditions de niveau pour cette entrée sont remplies.
					if (result)
					{
						// Pour lancer le dialogue principal.
						bool goMainDialogue = true;

						// Posons une variable qui sert de test pour l'inventaire.
						bool isInInventory = false;

						// A-t-on un test d'objets à effectuer ?
						// Pour le savoir, il suffit de tester la liste des objets : y a-t-il des objets renseignés ?
						if (objects.Count > 0)
						{
							// Maintenant, testons si nous avons les objets requis dans l'inventaire.
							foreach (string item in objects)
							{
								// Cet objet existe-t-il dans l'inventaire ?
								isInInventory = Array.Exists(_inventory.m_inventoryStrings, element => element == item);
								// S'il manque quelque chose, arrêter la recherche
								if (!isInInventory)
								{
									break;
								}
							}

							// S'il manque quelque chose.
							if (!isInInventory)
							{
								// Pas de dialogue principal.
								goMainDialogue = false;
							}
						}

						// Les conditions personnalisées sont-elles remplies ?
						foreach (bool boolValue in m_conditionsBools)
						{
							if (!boolValue)
							{
								goMainDialogue = false;
								break;
							}
						}

						// Si le dialogue principal se lance.
						if (goMainDialogue)
						{
							// PROVISOIRE -----------------------------
							Debug.Log("Dialogue principal = " + entry.m_dialogueSO.m_test);

							_currentCamera.SetActive(true);

							if (entry.m_useControledScriptsAtStart)
							{
								foreach (IEnableForDialogue controlledScript in m_IEnableForDialogue)
								{
									controlledScript.EnableMe(false);
								}
							}

							if (entry.m_eventAtStart != null)
							{
								entry.m_eventAtStart.Invoke();
							}

							if (entry.m_levelChangesAtEnd)
							{
								_levelProgress.m_levelStep = entry.m_nextLevel;
							}

							if (entry.m_useControledScriptsAtEnd)
							{
								foreach (IEnableForDialogue controlledScript in m_IEnableForDialogue)
								{
									controlledScript.EnableMe(true);
								}
							}

							if (entry.m_eventAtEnd != null)
							{
								entry.m_eventAtEnd.Invoke();
							}
							// --------------------------------------------
						}
						// Sinon, le dialogue principal ne se lance pas.
						else
						{
							
							// Y a-t-il un dialogue alternatif ? Si oui, le lancer.
							if (entry.m_dialogueAltSO != null)
							{
								// PROVISOIRE -----------------------------
								Debug.Log("Dialogue alternatif = " + entry.m_dialogueAltSO.m_test);

								_currentCamera.SetActive(true);

								if (entry.m_useControledScriptsAtStart)
								{
									foreach (IEnableForDialogue controlledScript in m_IEnableForDialogue)
									{
										controlledScript.EnableMe(false);
									}
								}

								if (entry.m_eventAtStart != null)
								{
									entry.m_eventAtStart.Invoke();
								}
								// --------------------------------------------
							}
						}

						// Vider les références.
						m_conditionsBools.Clear();
						_levelProgress = null;
						_currentCamera = null;

						// Stopper l'usine.
						break;
					}

					// Les conditions pour cette entrée ne sont pas remplies, donc on teste l'entrée suivante.
					// Remettre le résultat à true pour itérer.
					result = true;
				}
			}
		}

		#endregion
	}
}