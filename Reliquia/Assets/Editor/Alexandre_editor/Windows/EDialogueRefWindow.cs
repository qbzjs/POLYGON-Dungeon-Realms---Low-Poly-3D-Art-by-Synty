using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;

namespace AlexandreDialogues
{
	public class EDialogueRefWindow : EBaseWindow
	{
		private DialogueEntry[] _entries;
		private DialogueEntry _currentEntry;
		private DialogueRef _target;

		private SerializedProperty _myDialogs;

		private SerializedProperty _eventAtStart, _eventAtEnd;
		private SerializedProperty _startConditions;

		private int _currentValue;

		private static float _width, _height, _x, _y; 
		private static float _defaultWidth, _defaultHeight, _defaultX, _defaultY;



		public static void Open(SerializedObject so, DialogueRef target)
		{
			EDialogueRefWindow window = (EDialogueRefWindow)EditorWindow.GetWindow(typeof(EDialogueRefWindow), false);
			window.Show();

			window.SetCoordinates(window, 960f, 570f, ref _width, ref _defaultWidth, ref _height, ref _defaultHeight, ref _x, ref _defaultX, ref _y, ref _defaultY);

			window.titleContent = new GUIContent("DialogueRef " + target.name);

			window._entries = target.m_myDialogs;
			window._targetSO = so;
			window._target = target;
			window._myDialogs = so.FindProperty("m_myDialogs");
			window._currentEntry = null;

			window._scrollEntries = new Vector2[3];
		}


		private void OnDestroy()
		{
			_defaultWidth = default;
		}


		protected override void DrawContent()
		{
			// entrées

			EditorGUILayout.BeginVertical(GUILayout.Width(260));

			_scrollEntries[0] = EditorGUILayout.BeginScrollView(_scrollEntries[0]);

			DrawLabel("Entries", 20);

			if (_entries != null && _entries.Length > 0)
			{
				for (int i = 0; i < _entries.Length; i++)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(20);
					GUILayout.Label(i.ToString(), GUILayout.Width(20));
					if (GUILayout.Button(_entries[i].m_name, GUILayout.Width(170)))
					{
						_currentEntry = _entries[i];

						GetDialogsProperties(i);

						Defocus();

						_currentValue = i;
					}
					if (GUILayout.Button("-", GUILayout.Width(20)))
					{
						_myDialogs.DeleteArrayElementAtIndex(i);
						_targetSO.ApplyModifiedProperties();

						int deletePos = i;
						List<DialogueEntry> saveList = new List<DialogueEntry>();
						for (int j = 0; j < _entries.Length; j++)
						{
							if (j != deletePos)
							{
								saveList.Add(_entries[j]);
							}
						}
						_entries = new DialogueEntry[saveList.Count];
						for (int j = 0; j < saveList.Count; j++)
						{
							_entries[j] = saveList[j];
						}
						_target.m_myDialogs = _entries;
						if (_entries.Length > 0)
						{
							_currentEntry = _entries[0];
							GetDialogsProperties(0);
							_currentValue = 0;
						}
					}
					EditorGUILayout.EndHorizontal();
				}
			}

			GUILayout.Space(20);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);
			if (GUILayout.Button("Add entry", GUILayout.Width(217)))
			{
				int currentValue = 0;
				if (_entries == null || _entries.Length == 0)
				{
					_entries = new DialogueEntry[1];
					_entries[0] = new DialogueEntry();
					_target.m_myDialogs = _entries;
					_currentEntry = _entries[currentValue];
				}
				else
				{
					DialogueEntry[] save = new DialogueEntry[_entries.Length];
					Array.Copy(_entries, save, _entries.Length);
					_entries = new DialogueEntry[save.Length + 1];
					for (int i = 0; i < save.Length; i++)
					{
						_entries[i] = save[i];
					}
					_entries[_entries.Length - 1] = new DialogueEntry();
					_target.m_myDialogs = _entries;
					currentValue = _entries.Length - 1;
					_currentEntry = _entries[currentValue];
				}

				_myDialogs.arraySize++;
				_targetSO.ApplyModifiedProperties();

				_currentEntry.m_dialogueAltSO = null;
				_currentEntry.m_dialogueSO = null;
				_currentEntry.m_eventAtEnd = null;
				_currentEntry.m_eventAtStart = null; 
				_currentEntry.m_levelChangesAtEnd = false;
				_currentEntry.m_name = "Entry #" + currentValue;
				_currentEntry.m_nextLevel = 0;
				_currentEntry.m_startConditions = null;
				_currentEntry.m_useControledScriptsAtEnd = false;
				_currentEntry.m_useControledScriptsAtStart = false;

				_currentValue = currentValue;

				GetDialogsProperties(currentValue);
			}

			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndScrollView();

			VerticalLine(260f, _lineColor);

			EditorGUILayout.EndVertical();

			if (_entries == null || _entries.Length == 0)
			{
				return;
			}

			//----------------------------------------

			// paramètres

			EditorGUILayout.BeginVertical(GUILayout.Width(350));

			_scrollEntries[1] = EditorGUILayout.BeginScrollView(_scrollEntries[1]);

			DrawLabel("Global settings", 20);

			if (_currentEntry == null)
			{
				_currentEntry = _entries[0];

				GetDialogsProperties(0);
			}

			DisplayElement("Name", "Nom de l'entrée", 100, () =>
			{
				_currentEntry.m_name = EditorGUILayout.TextField(_currentEntry.m_name, GUILayout.Width(200));
			});

			DisplayElement("Dialogue SO", "Le ScriptableObject de ce dialogue.", 100, () =>
			{
				_currentEntry.m_dialogueSO = (Dialogue)EditorGUILayout.ObjectField(_currentEntry.m_dialogueSO, typeof(Dialogue), false, GUILayout.Width(200));
			});

			DisplayElement("Dialogue alt SO", "Le ScriptableObject du dialogue alternatif (si condition d'objet ou méthode personnalisée).", 100, () =>
			{
				_currentEntry.m_dialogueAltSO = (Dialogue)EditorGUILayout.ObjectField(_currentEntry.m_dialogueAltSO, typeof(Dialogue), false, GUILayout.Width(200)); 
			});

			GUILayout.Space(20);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);
			DialogueManager obj = GameObject.FindObjectOfType<DialogueManager>();
			if (obj == null)
			{
				EditorGUILayout.LabelField("Ajouter un dialogueManager à la scène.");
			}
			else
			{
				if (GUILayout.Button("Select DialogueManager", GUILayout.Width(300)))
				{
					EditorGUIUtility.PingObject(obj);
					Selection.activeGameObject = obj.gameObject;
				}
			}
			EditorGUILayout.EndHorizontal();

			HorizontalLine(160f, _lineColor);

			DrawLabel("Start settings", 30);

			DisplayElement("ControledScripts disabled", "Les ControledScripts disposant de l'interface IEnableForDialogue sont désactivés au démarrage du dialogue. Cette liste est dans DialogueManager.", 285, () =>
			{
				_currentEntry.m_useControledScriptsAtStart = EditorGUILayout.Toggle(_currentEntry.m_useControledScriptsAtStart);
			});

			DisplayElementEvent(() =>
			{
				EditorGUILayout.PropertyField(_eventAtStart, true, GUILayout.Width(302)); // sérialisé (pas trouvé comment récupérer en non sérialisé)
			});

			HorizontalLine(340f, _lineColor);

			DrawLabel("End settings", 30);

			DisplayElement("Level changes at end", "Le niveau de progression change-t-il en fin de dialogue ?", 285, () =>
			{
				_currentEntry.m_levelChangesAtEnd = EditorGUILayout.Toggle(_currentEntry.m_levelChangesAtEnd, GUILayout.Width(20));
			});

			DisplayElement("Next level", "Si le niveau de progression change en fin de dialogue, ceci en est la valeur.", 100, () =>
			{
				_currentEntry.m_nextLevel = EditorGUILayout.IntField(_currentEntry.m_nextLevel, GUILayout.Width(200));
			});

			DisplayElement("ControledScripts enabled", "Les ControledScripts disposant de l'interface IEnableForDialogue sont réactivés à la fin du dialogue. Cette liste est dans DialogueManager.", 285, () =>
			{
				_currentEntry.m_useControledScriptsAtEnd = EditorGUILayout.Toggle(_currentEntry.m_useControledScriptsAtEnd);
			});

			DisplayElementEvent(() =>
			{
				EditorGUILayout.PropertyField(_eventAtEnd, true, GUILayout.Width(302)); // sérialisé (pas trouvé comment récupérer en non sérialisé)
			});

			EditorGUILayout.Space(10);
			EditorGUILayout.EndScrollView();

			VerticalLine(610f, _lineColor);

			EditorGUILayout.EndVertical();

			//----------------------------------------

			// Conditions

			EditorGUILayout.BeginVertical(GUILayout.Width(350));

			_scrollEntries[2] = EditorGUILayout.BeginScrollView(_scrollEntries[2]);

			DrawLabel("Start conditions", 20);

			if (_currentEntry.m_startConditions != null)
			{
				for (int i = 0; i < _currentEntry.m_startConditions.Length; i++)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(20);
					GUILayout.Label($"Condition {i}", GUILayout.Width(279));
					if (GUILayout.Button("-", GUILayout.Width(20)))
					{
						_startConditions.DeleteArrayElementAtIndex(i); 
						_targetSO.ApplyModifiedProperties();
						break;
					}
					EditorGUILayout.EndHorizontal();

					DisplayElement("Type", "Le type de condition pour lancer ce de dialogue.", 100, () =>
					{
						_currentEntry.m_startConditions[i].m_type = (StartConditionsTestType)EditorGUILayout.EnumPopup(_currentEntry.m_startConditions[i].m_type, GUILayout.Width(200));
					});

					switch (_currentEntry.m_startConditions[i].m_type)
					{
						case StartConditionsTestType.Level:
							DisplayElement("Operator", "Dans le cas d'une condition de niveau progression, choisir l'opérateur de comparaison.", 100, () =>
							{
								_currentEntry.m_startConditions[i].m_operator = (StartConditionsOperator)EditorGUILayout.EnumPopup(_currentEntry.m_startConditions[i].m_operator, GUILayout.Width(200));
							});

							DisplayElement("Level", "Dans le cas d'une condition de niveau progression, entrer le niveau testé.", 100, () =>
							{
								_currentEntry.m_startConditions[i].m_level = EditorGUILayout.IntField(_currentEntry.m_startConditions[i].m_level, GUILayout.Width(200));
							});

							StartConditionReset("Object", i);
							StartConditionReset("Method", i);
							break;
						case StartConditionsTestType.Object:
							DisplayElement("Object", "Dans le cas d'une condition d'objet, renseigner l'objet à avoir en inventaire.", 100, () =>
							{
								_currentEntry.m_startConditions[i].m_object = EditorGUILayout.TextField(_currentEntry.m_startConditions[i].m_object, GUILayout.Width(200));
							});

							StartConditionReset("Level", i);
							StartConditionReset("Method", i);
							break;
						case StartConditionsTestType.Method:
							DisplayElementEvent(() =>
							{
								EditorGUILayout.PropertyField(_startConditions.GetArrayElementAtIndex(i).FindPropertyRelative("m_eventCondition"), true, GUILayout.Width(302)); // sérialisé (pas trouvé comment récupérer en non sérialisé)
							});

							StartConditionReset("Level", i);
							StartConditionReset("Object", i);
							break;
						default:
							break;
					}

					GUILayout.Space(20);
				}
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);
			if (GUILayout.Button("Add condition", GUILayout.Width(300)))
			{
				_startConditions.arraySize++;
				_targetSO.ApplyModifiedProperties();

				StartConditionsTest lastCondition = _currentEntry.m_startConditions[_startConditions.arraySize - 1];
				lastCondition.m_type = StartConditionsTestType.Level;
				lastCondition.m_operator = StartConditionsOperator.EqualTo;
				lastCondition.m_level = 0;
				lastCondition.m_object = string.Empty;
				lastCondition.m_eventCondition = null;
			}
			EditorGUILayout.EndHorizontal();

			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndVertical();
		}

		private void GetDialogsProperties(int index)
		{
			_eventAtStart = _myDialogs.GetArrayElementAtIndex(index).FindPropertyRelative("m_eventAtStart");
			_eventAtEnd = _myDialogs.GetArrayElementAtIndex(index).FindPropertyRelative("m_eventAtEnd");
			_startConditions = _myDialogs.GetArrayElementAtIndex(index).FindPropertyRelative("m_startConditions");
		}

		private void StartConditionReset(string choice, int index)
		{
			StartConditionsTest condition = _currentEntry.m_startConditions[index];

			switch (choice)
			{
				case "Level":
					condition.m_operator = StartConditionsOperator.EqualTo;
					condition.m_level = 0;
					break;
				case "Object":
					condition.m_object = string.Empty;
					break;
				case "Method":
					condition.m_eventCondition = null;
					break;
				default:
					break;
			}
		}
	}

}