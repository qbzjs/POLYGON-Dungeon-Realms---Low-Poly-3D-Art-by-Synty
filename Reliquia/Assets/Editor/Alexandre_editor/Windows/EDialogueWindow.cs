using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;

namespace AlexandreDialogues
{
	public class EDialogueWindow : EBaseWindow
	{
		private Dialogue _target;

		private SerializedProperty _castingSO;

		private string[] _characterNames;
		private int[] _characterNamesIndex;

		private static float _width, _height, _x, _y; 
		private static float _defaultWidth, _defaultHeight, _defaultX, _defaultY;

		private DialogueColorsRef colorRefs = new DialogueColorsRef();
		private string[] _colorRefsString;



		public static void Create(SerializedObject so, Dialogue target)
		{
			EDialogueWindow window = (EDialogueWindow)EditorWindow.GetWindow(typeof(EDialogueWindow), false);
			window.Show();

			window.SetCoordinates(window, 680f, 570f, ref _width, ref _defaultWidth, ref _height, ref _defaultHeight, ref _x, ref _defaultX, ref _y, ref _defaultY);

			window.titleContent = new GUIContent("Dialogue " + target.name);

			window._target = target;
			window._targetSO = so;
			window._castingSO = so.FindProperty("m_casting");

			window._scrollEntries = new Vector2[2];

			window._colorRefsString = new string[window.colorRefs.ColorsHash.Count];
			int incrementor = 0;
			foreach (KeyValuePair<string, int> item in window.colorRefs.ColorsHash)
			{
				window._colorRefsString[incrementor] = item.Key;
				incrementor++;
			}
		}


		private void OnDestroy()
		{
			_defaultWidth = default;
		}


		protected override void DrawContent()
		{
			// Debug, Casting

			EditorGUILayout.BeginVertical(GUILayout.Width(330f));

			_scrollEntries[0] = EditorGUILayout.BeginScrollView(_scrollEntries[0]);

			DrawLabel("Debug", 20);

			DisplayElement("Test", "Un texte de test pour tester du texte.", 50, () =>
			{
				_targetSO.FindProperty("m_test").stringValue = EditorGUILayout.TextField(_target.m_test, GUILayout.Width(230f));
			});

			HorizontalLine(80f, _lineColor);

			DrawLabel("Casting", 30);

			DisplayElement("Roles", "Les rôles pour ce dialogue.", 100, () => { });

			if (_target.m_casting != null)
			{
				for (int i = 0; i < _target.m_casting.Length; i++)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(20);
					Sprite characterSprite = null;
					if (_target.m_casting[i].m_character != null)
					{
						switch (_target.m_casting[i].m_characterView)
						{
							case RoleCharacterView.Left:
								characterSprite = _target.m_casting[i].m_character.m_spriteLeft;
								break;
							case RoleCharacterView.Right:
								characterSprite = _target.m_casting[i].m_character.m_spriteRight;
								break;
							default:
								break;
						}
					}
					var texture = AssetPreview.GetAssetPreview(characterSprite);
					GUILayout.Label(texture, GUILayout.Width(50), GUILayout.Height(70));

					EditorGUILayout.BeginVertical();

					EditorGUILayout.BeginHorizontal();
					GUILayout.Label(new GUIContent($"Character {i}", "Un personnage. Choisir son orientation : gauche ou droite."), GUILayout.Width(205f)); ;
					if (GUILayout.Button("-", GUILayout.Width(20)))
					{
						_castingSO.DeleteArrayElementAtIndex(i); 
						_targetSO.ApplyModifiedProperties();
						break;
					}
					EditorGUILayout.EndHorizontal();
					_targetSO.FindProperty("m_casting").GetArrayElementAtIndex(i).FindPropertyRelative("m_character").objectReferenceValue = (Character)EditorGUILayout.ObjectField(_targetSO.FindProperty("m_casting").GetArrayElementAtIndex(i).FindPropertyRelative("m_character").objectReferenceValue, typeof(Character), false, GUILayout.Width(230f));

					_target.m_casting[i].m_characterView = (RoleCharacterView)EditorGUILayout.EnumPopup(_target.m_casting[i].m_characterView, GUILayout.Width(230f));
					EditorGUILayout.EndVertical();

					EditorGUILayout.EndHorizontal();
				}
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(75);
			if (GUILayout.Button("Add character", GUILayout.Width(227f)))
			{
				_castingSO.arraySize++; 
				_targetSO.ApplyModifiedProperties();

				_target.m_casting[_target.m_casting.Length - 1].m_character = null;
				_target.m_casting[_target.m_casting.Length - 1].m_characterView = RoleCharacterView.Left;
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(20);

			EditorGUILayout.EndScrollView();

			VerticalLine(330f, _lineColor);

			EditorGUILayout.EndVertical();

			if (_target.m_casting == null || _target.m_casting.Length == 0)
			{
				if (_target.m_replies.Length > 0)
				{
					_target.m_replies = new Reply[0];
				}

				return;
			}

			_characterNames = new string[_target.m_casting.Length];
			for (int i = 0; i < _characterNames.Length; i++)
			{
				if (_target.m_casting[i].m_character == null)
				{
					return;
				}
				
				_characterNames[i] = _target.m_casting[i].m_character.m_name;

				for (int j = 0; j < _characterNames.Length; j++)
				{
					if (_target.m_casting[j].m_character != null && _target.m_casting[j].m_character.m_name == _target.m_casting[i].m_character.m_name && j != i)
					{
						_target.m_casting[j].m_character = null;
						break;
					}
				}
			}

			if (_target.m_replies != null)
			{
				_characterNamesIndex = new int[_target.m_replies.Length];
				for (int i = 0; i < _target.m_replies.Length; i++)
				{
					string name = string.Empty;
					int posInArray = 0;
					if (_target.m_replies[i].m_role.m_character != null)
					{
						name = _target.m_replies[i].m_role.m_character.m_name;
						posInArray = Array.IndexOf(_characterNames, name);
						if (posInArray == -1)
						{
							_characterNamesIndex[i] = 0;
						}
						else
						{
							_characterNamesIndex[i] = posInArray;
						}
					}
				}
			}

			//----------------------------------------

			// Répliques

			EditorGUILayout.BeginVertical(GUILayout.Width(350f));

			_scrollEntries[1] = EditorGUILayout.BeginScrollView(_scrollEntries[1]);

			DrawLabel("Replies", 20);

			if (_target.m_replies != null)
			{
				for (int i = 0; i < _target.m_replies.Length; i++)
				{
					EditorGUILayout.BeginHorizontal();
					GUILayout.Space(20);
					GUILayout.Label(new GUIContent($"Reply {i}", "Une réplique de dialogue."), GUILayout.Width(280));
					if (GUILayout.Button("-", GUILayout.Width(20)))
					{
						_targetSO.FindProperty("m_replies").DeleteArrayElementAtIndex(i);
						_targetSO.ApplyModifiedProperties();
						break;
					}
					EditorGUILayout.EndHorizontal();

					DisplayElement("Role", "Le personnage qui a la parole.", 100, () => {
						_characterNamesIndex[i] = EditorGUILayout.Popup(_characterNamesIndex[i], _characterNames, GUILayout.Width(200));
						_target.m_replies[i].m_role = _target.m_casting[_characterNamesIndex[i]];
					});

					DisplayElement("Stage direction", "Les didascalies pour ce personnage. Séparer les mots ou groupes de mots par des virgules.", 100, () =>
					{
						_targetSO.FindProperty("m_replies").GetArrayElementAtIndex(i).FindPropertyRelative("m_stageDirections").stringValue = EditorGUILayout.TextArea(_targetSO.FindProperty("m_replies").GetArrayElementAtIndex(i).FindPropertyRelative("m_stageDirections").stringValue, _myStyle, GUILayout.Width(200));
					});

					DisplayElement("Color", "La couleur choisie pour les didascalies.", 100, () =>
					{
						int index = 0;
						int incrementor = 0;
						foreach (KeyValuePair<string, int> item  in colorRefs.ColorsHash)
						{
							if (item.Value == _target.m_replies[i].m_styleColorHash)
							{
								index = incrementor;
								break;
							}
							incrementor++;
						}
						index = EditorGUILayout.Popup(index, _colorRefsString, GUILayout.Width(200));
						_target.m_replies[i].m_styleColorHash = colorRefs.ColorsHash[_colorRefsString[index]];
					});

					DisplayElement("Text", "Le texte de cette réplique", 100, () => {
						_targetSO.FindProperty("m_replies").GetArrayElementAtIndex(i).FindPropertyRelative("m_text").stringValue = EditorGUILayout.TextArea(_targetSO.FindProperty("m_replies").GetArrayElementAtIndex(i).FindPropertyRelative("m_text").stringValue, _myStyle, GUILayout.Width(200));
					});
					
					DisplayElement("Char Speed", "Vitesse à la quelle est affiché un caractère de texte qui n'est pas un espace.", 100, () =>
					{
						_targetSO.FindProperty("m_replies").GetArrayElementAtIndex(i).FindPropertyRelative("m_charSpeed").floatValue = EditorGUILayout.Slider(_targetSO.FindProperty("m_replies").GetArrayElementAtIndex(i).FindPropertyRelative("m_charSpeed").floatValue, 0f, 1f, GUILayout.Width(200));
					});

					DisplayElement("Space Speed", "Vitesse à la quelle est affiché un caractère de type espace.", 100, () =>
					{
						_targetSO.FindProperty("m_replies").GetArrayElementAtIndex(i).FindPropertyRelative("m_spaceSpeed").floatValue = EditorGUILayout.Slider(_targetSO.FindProperty("m_replies").GetArrayElementAtIndex(i).FindPropertyRelative("m_spaceSpeed").floatValue, 0f, 1f, GUILayout.Width(200));
					});

					DisplayElementEvent(() =>
					{
						EditorGUILayout.PropertyField(_targetSO.FindProperty("m_replies").GetArrayElementAtIndex(i).FindPropertyRelative("m_replyEvent"), true, GUILayout.Width(302)); // sérialisé (pas trouvé comment récupérer en non sérialisé)
					});

					GUILayout.Space(20);
				}
			}

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);
			if (GUILayout.Button("Add reply", GUILayout.Width(305f)))
			{
				_targetSO.FindProperty("m_replies").arraySize++; 
				_targetSO.ApplyModifiedProperties();

				Reply lastReply = _target.m_replies[_target.m_replies.Length - 1];
				lastReply.m_role = _target.m_casting[0];
				lastReply.m_stageDirections = string.Empty;
				lastReply.m_styleColorHash = colorRefs.ColorsHash["Normal"];
				lastReply.m_text = string.Empty;
				lastReply.m_charSpeed = 0.1f;
				lastReply.m_spaceSpeed = 0.5f;
				lastReply.m_replyEvent = null;
			}
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(20);

			EditorGUILayout.EndScrollView();

			EditorGUILayout.EndVertical();
		}
	}
}
