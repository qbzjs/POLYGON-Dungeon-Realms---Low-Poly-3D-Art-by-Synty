using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using System;
using UnityEngine.Events;
using UnityEngine.UI;
using TMPro;

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
					Debug.LogError("<color=magenta>DialogueManager</color> > Instance null.");
				}
				return _instance;
			}

			// pas de setter
		}

		#endregion



		#region Fields

		// PRIVATE
		[SerializeField] private Inventory _inventory;
		[SerializeField] private GameObject _defaultCamera;
		private IEnumerator _coroutine;
		[SerializeField] private GameObject _canvas;
		[SerializeField] private char _textLastCharIdentifier;

		[System.Serializable]
		private class UIObjects
		{
			public GameObject m_character;
			public Image m_image;
			public TextMeshProUGUI m_name;
		}

		[SerializeField] private UIObjects _UICharacterLeft = new UIObjects();
		[SerializeField] private UIObjects _UICharacterRight = new UIObjects();
		[SerializeField] private TextMeshProUGUI _replyText;

		// PUBLIC
		// pour la liste des objets à contrôler (Interfaces non sérialisables donc contourner)
		public List<MonoBehaviour> m_controledScripts = new List<MonoBehaviour>();
		public List<IEnableForDialogue> m_IEnableForDialogue;

		[SerializeField] public List<bool> m_conditionsBools = new List<bool>();
		
		// TRACKED FIELDS
		[SerializeField] private LevelProgress _levelProgress;
		[SerializeField] private GameObject _currentCamera;
		[SerializeField] private Dialogue _currentDialogue;
		[SerializeField] private UIObjects _currentUIObject;
		[SerializeField] private int _currentReply;
		private DialogueEntry _currentDialogueEntry;
		[SerializeField] private bool _isDialogueStarted;
		[SerializeField] private int _clickState;
		[SerializeField] private bool _isCoroutineStarted;
		[SerializeField] private int _hashPosition;

		#endregion



		#region Frame cycle

		void Awake()
		{
			// Singleton
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

			// Désactiver le Canvas
			_canvas.SetActive(false);
		}

		private void Update()
		{
			if (_isDialogueStarted)
			{
				if (Input.GetButtonDown("Fire1"))
				{
					UserControl();
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
							StartDialogue(entry, entry.m_dialogueSO);
						}
						// Sinon, le dialogue principal ne se lance pas.
						else
						{
							
							// Y a-t-il un dialogue alternatif ? Si oui, le lancer.
							if (entry.m_dialogueAltSO != null)
							{
								StartDialogue(entry, entry.m_dialogueAltSO);
							}
						}

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



		#region Private methods

		private void StartDialogue(DialogueEntry entry, Dialogue dialogue)
		{
			// Conserver la référence du dialogue 
			_currentDialogue = dialogue;

			// Conserver la référence de l'entrée de dialogue
			_currentDialogueEntry = entry;

			if (dialogue.m_test != null && dialogue.m_test != string.Empty)
			{
				Debug.Log($"<color=magenta>DialogueManager</color> > Dialogue {dialogue.name} > {dialogue.m_test}");
			}

			// Activer la caméra de dialogue
			if (_currentCamera == null)
			{
				Debug.LogError($"<color=magenta>DialogueManager</color> > Le champ <b>Default camera</b> est null.");
				return;
			}
			_currentCamera.SetActive(true);

			// Si on utilise la liste des IEnableForDialogue, lancer la méthode afférente pour tous les objets renseignés
			if (entry.m_useControledScriptsAtStart)
			{
				foreach (IEnableForDialogue controlledScript in m_IEnableForDialogue)
				{
					controlledScript.EnableMe(false);
				}
			}

			// S'il y a des abonnés à l'UnityEvent de démarrage, lancer l'événement
			if (entry.m_eventAtStart != null)
			{
				entry.m_eventAtStart.Invoke();
			}

			// Lancer le Canvas
			_canvas.SetActive(true);

			// Lancer l'affichage de la première réplique
			PrepareReply();

			// Dire que le dialogue est commencé
			_isDialogueStarted = true;
		}

		private void PrepareReply()
		{
			// S'il n'y a pas de réplique, alors erreur
			if (_currentDialogue.m_replies == null || _currentDialogue.m_replies.Length == 0)
			{
				Debug.LogError($"<color=magenta>DialogueManager</color> Le dialogue {_currentDialogue.name} n'a pas de réplique.", _currentDialogue);
				return;
			}

			// La réplique actuelle
			Reply currentReply = _currentDialogue.m_replies[_currentReply];

			// Selon que le point de vue du personnage est à gauche
			if (currentReply.m_role.m_characterView == RoleCharacterView.Left)
			{
				// Sélectionner le GameObject correspondant à l'angle de vue gauche.
				_currentUIObject = _UICharacterLeft;
				// Renseigner le sprite dans l'image.
				_currentUIObject.m_image.sprite = currentReply.m_role.m_character.m_spriteLeft;

				// Les vignettes
				_UICharacterLeft.m_character.SetActive(true);
				_UICharacterRight.m_character.SetActive(false);
			}
			// ... ou à droite
			else
			{
				// Sélectionner le GameObject correspondant à l'angle de vue droite.
				_currentUIObject = _UICharacterRight;
				// Renseigner le sprite dans l'image.
				_currentUIObject.m_image.sprite = currentReply.m_role.m_character.m_spriteRight;

				// Les vignettes
				_UICharacterLeft.m_character.SetActive(false);
				_UICharacterRight.m_character.SetActive(true);
			}

			// Afficher le nom
			_currentUIObject.m_name.text = currentReply.m_role.m_character.m_name;

			// Didascalies : récupérer la couleur et paramétrer le texte avec les balises et des parenthèses.
			// Ajouter cela au texte de réplique.
			// Avec un caractère arbitraire de terminaison qui signifie la fin du texte.
			TMP_Style myStyle = TMP_StyleSheet.GetStyle(currentReply.m_styleColorHash);
			string textDirection = string.Empty;
			if (currentReply.m_stageDirections.Length > 0)
			{
				textDirection = $"({ currentReply.m_stageDirections}) ";
			}
			string finalText = $"{myStyle.styleOpeningDefinition}{textDirection}{myStyle.styleClosingDefinition}{currentReply.m_text}{_textLastCharIdentifier}";
			// Ce texte par défaut est mis en forme.

			// Lancer coroutine et le dire
			_coroutine = ReadTextCoroutine(finalText, currentReply.m_charSpeed, currentReply.m_spaceSpeed);
			StartCoroutine(_coroutine);
			_isCoroutineStarted = true;

			// Si abonnés, lancer l'événement associé à cette réplique
			if (currentReply.m_replyEvent != null)
			{
				currentReply.m_replyEvent.Invoke();
			}
		}

		private IEnumerator ReadTextCoroutine(string text, float charSpeed, float spaceSpeed)
		{
			// Un pas pour la boucle
			int currentChar = 0;

			// Renseigner le texte complet 
			_replyText.text = text;

			// Forcer le rafraîchissement du contenu
			_replyText.ForceMeshUpdate();

			// N'en afficher que 0 caractères au début
			_replyText.maxVisibleCharacters = 0;

			// Rrécupérer le texte actuel de l'objet
			string myText = _replyText.GetParsedText();

			// Récupérer la position de la terminaison dans le texte mis en forme
			_hashPosition = myText.LastIndexOf(_textLastCharIdentifier.ToString(), myText.Length);

			// La valeur de vitesse d'affichage à prendre par défaut
			float timeToWait = charSpeed;

			// Tant que le pas est inférieur à la longueur du texte
			while (currentChar < _replyText.text.Length)
			{
				// Ajuster le nombre de caractères visibles du champ au pas de boucle
				_replyText.maxVisibleCharacters = currentChar;

				// Récupérer le caractère final
				char lastChar = _replyText.textInfo.characterInfo[currentChar].character;

				// Tester si caractère de terminaison, alors terminer
				if (lastChar.Equals(_textLastCharIdentifier))
				{
					_isCoroutineStarted = false;
					yield break;
				}

				// Déterminer la vitesse d'affichage selon la nature du caractère
				timeToWait = charSpeed;
				if (Char.IsWhiteSpace(lastChar))
				{
					timeToWait = spaceSpeed;
				}

				// Incrémenter le pas pour la boucle
				currentChar++;

				// Attendre le temps défini précédemment
				yield return new WaitForSeconds(timeToWait);

			}

			// Hors ça, pour terminer
			_isCoroutineStarted = false;
			yield break;
		}

		private void UserControl()
		{
			// Si la coroutine est terminée, forcer l'état à 1
			if (!_isCoroutineStarted)
			{
				_clickState = 1;
			}

			// Au premier clic, afficher tout le texte de la réplique et changer l'état du clic
			if (_clickState == 0)
			{
				DisplayAllText();
				_clickState++;
			}
			// Sinon, à tout autre clic
			else
			{
				// Incrémenter la réplique actuelle
				_currentReply++;

				// Si on est à la fin du dialogue, finir le dialogue
				if (_currentReply > _currentDialogue.m_replies.Length - 1)
				{
					FinishDialogue();
				}
				// Sinon, c'est qu'il y a d'autres répliques, alors passer à la suivante
				else
				{
					PrepareReply();
				}

				_clickState = 0;
			}
		}

		private void DisplayAllText()
		{
			StopCoroutine(_coroutine);
			_isCoroutineStarted = false;
			_replyText.maxVisibleCharacters = _hashPosition;
		}

		private void FinishDialogue()
		{
			if (_currentDialogueEntry.m_levelChangesAtEnd)
			{
				_levelProgress.m_levelStep = _currentDialogueEntry.m_nextLevel;
			}

			if (_currentDialogueEntry.m_useControledScriptsAtEnd)
			{
				foreach (IEnableForDialogue controlledScript in m_IEnableForDialogue)
				{
					controlledScript.EnableMe(true);
				}
			}

			if (_currentDialogueEntry.m_eventAtEnd != null)
			{
				_currentDialogueEntry.m_eventAtEnd.Invoke();
			}

			_currentCamera.SetActive(false);

			_canvas.SetActive(false);

			// Vider les références pour accueillir le prochain dialogue
			m_conditionsBools.Clear();
			_levelProgress = null;
			_currentCamera = null;
			_currentDialogueEntry = null;
			_currentDialogue = null;
			_currentUIObject = null;
			_currentReply = 0;
			_hashPosition = 0;
			_isDialogueStarted = false;
		}

		#endregion
	}
}