using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using TMPro;

namespace AlexandreDialogues
{
	public class InGameDialogueManager : MonoBehaviour
	{
		#region Singleton

		private static InGameDialogueManager _instance = default;
		public static InGameDialogueManager Instance
		{
			get
			{
				if (_instance == null)
				{
					Debug.LogError($"<color=yellow>{nameof(InGameDialogueManager)}</color> > Instance null.");
				}
				return _instance;
			}

			// pas de setter
		}

		#endregion



		#region Fields

		[SerializeField] private GameObject _canvas = default;
		[SerializeField] private TextMeshProUGUI _textTMPro = default;

		[SerializeField] private bool _isDialogueStarted = default;
		public bool IsDialogueStarted { get => _isDialogueStarted; private set => _isDialogueStarted = value; } // pour y accéder de l'extérieur
		private IEnumerator _dialogueCoroutine = default;

		[SerializeField] private int _dialogueReplyLength = default;
		[SerializeField] private int _dialogueCurrentReply = default;
		[SerializeField] private float _dialogueCurrentTime = default;

		#endregion



		#region Frame cycle

		void Awake()
		{
			// Singleton
			_instance = this;
		}

		#endregion



		#region Public methods

		public void StartDialogue(InGameDialogue dialogue)
		{
			if (dialogue != null && dialogue.m_reply.Length > 0)
			{
				// S'il y a un DialogueManager et si la boîte de dialogue y est lancée
				// alors arrêter car on ne veut pas de superposition des dialogues et le dialogue InGame est moins important que l'autre
				if (DialogueManager.Instance != null && DialogueManager.Instance.IsDialogueStarted)
				{
					return;
				}

				// Un dialogue InGame se lance à la place de celui en cours.
				// Donc, s'il y en a un déjà lancé, l'interrompre et lancer celui demandé.

				// Interrompre la coroutine si déjà lancée
				StopCoroutineIfStarted();

				// La longueur du dialogue
				_dialogueReplyLength = dialogue.m_reply.Length;

				// La réplique actuelle
				_dialogueCurrentReply = 0;

				// Afficher le canvas
				_canvas.SetActive(true);

				// Démarrer la coroutine
				_dialogueCoroutine = DialogueCoroutine(dialogue);
				StartCoroutine(_dialogueCoroutine);

				// Dire que le dialogue est démarré
				_isDialogueStarted = true;
			}
		}

		public void StopCurrentDialogue()
		{
			StopCoroutineIfStarted();
			ResetDialogue();
		}

		#endregion



		#region Private methods

		private IEnumerator DialogueCoroutine(InGameDialogue dialogue)
		{
			float delayBetween2Speach = 1f;
			while (_dialogueCurrentReply < _dialogueReplyLength)
			{
				_textTMPro.text = dialogue.m_reply[_dialogueCurrentReply].m_text;
				_dialogueCurrentTime = dialogue.m_reply[_dialogueCurrentReply].m_displayTime;

				yield return new WaitForSeconds(_dialogueCurrentTime);
				yield return new WaitForSeconds(delayBetween2Speach);

				_dialogueCurrentReply++;
			}

			// Finir 
			ResetDialogue();
			yield break;
		}

		private void ResetDialogue()
		{
			if (_isDialogueStarted)
			{
				_dialogueReplyLength = 0;
				_dialogueCurrentReply = 0;
				_dialogueCurrentTime = 0;
				_canvas.SetActive(false);
				_textTMPro.text = string.Empty;
				_isDialogueStarted = false;
			}
		}

		private void StopCoroutineIfStarted()
		{
			if (_isDialogueStarted)
			{
				StopCoroutine(_dialogueCoroutine);
			}
		}

		#endregion
	}
}