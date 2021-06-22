using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AlexandreDialogues
{
	public class InGameDialogueTimeTrigger : MonoBehaviour
	{
		#region State Machine

		private enum States
		{
			Waiting,
			Dialoguing,
		}
		private States _currentState = default;

		#endregion



		#region Private

		[Tooltip("Un fichier ScriptableObject de dialogue in-game.")]
		[SerializeField] private InGameDialogue _inGameDialogue = default;
		[Tooltip("Le temps d'attente en secondes.")]
		[SerializeField] private float _duration = default;
		[Tooltip("Une fois le dialogue terminé : relancer le chronomètre et le dialogue ou bien désactiver le composant ?")]
		[SerializeField] private bool _resetAfterStart = default;

		private float _timer = default;
		private float _dialogueDuration = default;

		#endregion



		#region Editor

		void OnValidate()
		{
			if (_duration < 0)
			{
				_duration = 0;
			}
		}

		#endregion



		#region Frame cycle

		private void OnEnable()
		{
			_dialogueDuration = GetDialogueDuration();

			_currentState = States.Waiting;
		}

		private void Update()
		{
			OnStateUpdate(_currentState);
		}

		#endregion



		#region Methods

		private float ResetTimer()
		{
			return 0.0f;
		}

		private float GetDialogueDuration()
		{
			float totalTime = 0.0f;

			for (int i = 0; i < _inGameDialogue.m_reply.Length; i++)
			{
				totalTime += _inGameDialogue.m_reply[i].m_displayTime;
			}

			return totalTime;
		}

		#endregion



		#region State Machine

		private void OnStateEnter(States state)
		{
			switch (_currentState)
			{
				case States.Waiting:
					_timer = ResetTimer();
					break;
				case States.Dialoguing:
					InGameDialogueManager.Instance.StartDialogue(_inGameDialogue);
					_timer = ResetTimer();
					break;
				default:
					Debug.LogError($"OnStateEnter.default : {state}");
					break;
			}
		}

		private void OnStateExit(States state)
		{
			switch (_currentState)
			{
				case States.Waiting:
					break;
				case States.Dialoguing:
					break;
				default:
					Debug.LogError($"OnStateExit.default : {state}");
					break;
			}
		}

		private void OnStateUpdate(States state)
		{
			switch (_currentState)
			{
				case States.Waiting:
					_timer += Time.deltaTime;
					if (_timer >= _duration)
					{
						TransitionToState(_currentState, States.Dialoguing);
					}
					break;
				case States.Dialoguing:
					_timer += Time.deltaTime;
					if (_timer >= _dialogueDuration)
					{
						if (_resetAfterStart)
						{
							TransitionToState(_currentState, States.Waiting);
						}
						else
						{
							this.enabled = false;
						}
					}
					break;
				default:
					Debug.LogError($"OnStateUpdate.default : {state}");
					break;
			}
		}

		private void TransitionToState(States previousState, States nextState)
		{
			OnStateExit(previousState);
			_currentState = nextState;
			OnStateEnter(nextState);
		}

		#endregion
	}
}