using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;
using System.Reflection;

namespace AlexandreDialogues
{
	public class ECSVDispatcher : EditorWindow
	{
		private static float _width, _height, _x, _y; 
		private static float _defaultWidth, _defaultHeight, _defaultX, _defaultY;

		private static bool _isFirstStarted;

		private float _startedWidth = 300f;
		private float _startedHeight = 500f;

		private bool IsDocked 
		{
			get
			{
				BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
				MethodInfo method = GetType().GetProperty("docked", flags).GetGetMethod(true);
				return (bool)method.Invoke(this, null);
			}
		}

		private string _baseText = "";

		private Vector2 _scrollOne = new Vector2();

		private GUIStyle _scrollStyle, _subLabelStyle, _textareaStyle;

		private char[] _arraySeparators = new char[] { '\n' };

		private bool _isEditing;

		private List<string> _castingNames = new List<string>();
		private string[][] _superArray;

		private float _charSpeedDefault = 0.1f;
		private float _spaceSpeedDefault = 0.5f;



		private void OnEnable()
		{
			if (_scrollStyle == null)
			{
				_scrollStyle = new GUIStyle();
				_scrollStyle.padding = new RectOffset(20, 20, 20, 20);

				_textareaStyle = new GUIStyle(EditorStyles.textArea);
				_textareaStyle.wordWrap = true;
				_textareaStyle.margin = new RectOffset(0, 0, 20, 0);

				_subLabelStyle = new GUIStyle(EditorStyles.label);
				_subLabelStyle.padding = new RectOffset(0, 0, 15, 10);
				_subLabelStyle.fontStyle = FontStyle.Bold;
			}
		}


		private void OnGUI()
		{
			_scrollOne = GUILayout.BeginScrollView(_scrollOne, _scrollStyle);
			{
				// ------------------------------------------------------------

				// en-tête et boutons

				GUILayout.BeginHorizontal();
				{
					if (!_isEditing)
					{
						GUILayout.Label(new GUIContent("Base text", "Le texte du dialogue comprenant nom, didascalies et réplique, séparées par des points-virgules."), EditorStyles.boldLabel);
					}
					else
					{
						GUILayout.Label(new GUIContent("Structure", "Le contenu structuré en Dialogue."), EditorStyles.boldLabel);
					}
					EditorGUI.BeginDisabledGroup(!_isEditing);
					{
						if (GUILayout.Button(new GUIContent("Main", "Retourner à l'écran précédent et annuler l'édition."), GUILayout.Width(50)))
						{
							// traitement --------------------------------------
							ResetContent();
							// -------------------------------------------------
							_isEditing = false;
							_scrollOne = Vector2.zero;
							Defocus();
						}
					}
					EditorGUI.EndDisabledGroup();
					EditorGUI.BeginDisabledGroup(_isEditing || (_baseText == null || _baseText.Length == 0));
					{
						if (GUILayout.Button(new GUIContent("Edit", "Editer les données."), GUILayout.Width(50)))
						{
							// traitement --------------------------------------
							if (_baseText != null && _baseText.Length > 0)
							{
								string[] arrayString = _baseText.Split(_arraySeparators, System.StringSplitOptions.RemoveEmptyEntries);

								_superArray = new string[arrayString.Length][];
								for (int i = 0; i < arrayString.Length; i++)
								{
									string[] lineArray =
									{
										string.Empty,
										string.Empty,
										string.Empty
									};

									string[] arrayContent = arrayString[i].Split(';');
									for (int j = 0; j < arrayContent.Length; j++)
									{
										lineArray[j] = arrayContent[j];
									}

									_superArray[i] = lineArray;

									if (!_castingNames.Contains(_superArray[i][0]))
									{
										_castingNames.Add(_superArray[i][0]);
									}
								}
							}
							// -------------------------------------------------
							_isEditing = true;
							_scrollOne = Vector2.zero;
							Defocus();
						}
					}
					EditorGUI.EndDisabledGroup();
				}
				GUILayout.EndHorizontal();

				// ------------------------------------------------------------

				if (!_isEditing)
				{
					GUILayout.Space(20);
					EditorGUILayout.HelpBox("Entrer un texte au format CSV. Chaque ligne représente une réplique. Chaque ligne doit contenir 3 blocs séparés par des points-virgules : nom, didascalies éventuelles, texte. Le nom dans le CSV doit correspondre au nom de fichier de Character existant dans le projet (sinon, renommer ou créer le fichier manquant).", MessageType.None);
					_baseText = EditorGUILayout.TextArea(_baseText, _textareaStyle);
				}
				else if (_isEditing)
				{

					bool[] allCharactersExist = new bool[_castingNames.Count]; 
					Character[] characters = new Character[_castingNames.Count]; 
					Role[] roles = new Role[_castingNames.Count];
					Reply[] replies = new Reply[_superArray.Length]; 

					GUILayout.Label(new GUIContent("Names", "Liste des noms de personnage."), _subLabelStyle);
					GUILayout.BeginVertical();
					for (int i = 0; i < _castingNames.Count; i++)
					{
						GUILayout.BeginHorizontal();
						{
							EditorGUI.BeginDisabledGroup(true);
							{
								_castingNames[i] = EditorGUILayout.TextField(_castingNames[i]);
							}
							EditorGUI.EndDisabledGroup();
							string[] files = AssetDatabase.FindAssets($"{_castingNames[i]}", new[] { "Assets/ScriptableObjects" }); // chercher par le type avec "t:..." ne fonctionne pas
							if (files == null || files.Length == 0)
							{
								allCharactersExist[i] = false;
								GUILayout.Label("Error", GUILayout.Width(50));
							}
							else
							{
								allCharactersExist[i] = true; 
								string path = AssetDatabase.GUIDToAssetPath(files[0]); 
								characters[i] = AssetDatabase.LoadAssetAtPath<Character>(path); 
								if (GUILayout.Button(new GUIContent("Select", "Séléctionner le fichier dans Project pour en afficher les propriétés en Inspector."), GUILayout.Width(50)))
								{
									SelectObject(characters[i]);
								}
							}
						}
						GUILayout.EndHorizontal();
					}
					GUILayout.EndVertical();

					if (Array.Exists(allCharactersExist, e => e == false))
					{
						GUILayout.Space(20);
						EditorGUILayout.HelpBox("Un nom fourni ne correspond à aucun fichier de Character existant. Corriger le nom dans le texte source ou bien créer un nouveau fichier Character.", MessageType.None);
					}
					else
					{
						for (int i = 0; i < roles.Length; i++)
						{
							roles[i] = new Role();
							roles[i].m_character = characters[i];

							if (i % 2 == 0) 
							{
								roles[i].m_characterView = RoleCharacterView.Left;
							}
							else
							{
								roles[i].m_characterView = RoleCharacterView.Right;
							}
						}

						for (int i = 0; i < _superArray.Length; i++)
						{
							replies[i] = new Reply(); 

							int indexInNames = _castingNames.IndexOf(_superArray[i][0], 0, _castingNames.Count);
							if (indexInNames != -1)
							{
								replies[i].m_role = roles[indexInNames];
							}

							replies[i].m_stageDirections = _superArray[i][1];
							replies[i].m_styleColorHash = -1139904493; // colorHash du style "normal"

							replies[i].m_charSpeed = _charSpeedDefault;
							replies[i].m_spaceSpeed = _spaceSpeedDefault;

							replies[i].m_text = _superArray[i][2];
						}

						GUILayout.Label(new GUIContent("Dialogue file name", "Entrer un nom pour générer le fichier ScriptableOject .asset du dialogue."), _subLabelStyle);
						if (GUILayout.Button(new GUIContent("Create Asset", "Générer le fichier.")))
						{
							string path = EditorUtility.SaveFilePanelInProject("Save dialogue", "", "asset", "Save file at path"); 
							if (path.Length != 0) 
							{
								Dialogue dialogue = CreateInstance<Dialogue>(); 
								dialogue.m_casting = roles;
								dialogue.m_replies = replies;
								AssetDatabase.CreateAsset(dialogue, path); 

								SelectObject(dialogue);

								ResetContent();
							}
						}
					}
				}
			}
			GUILayout.EndScrollView();
		}

		private void SetCoordinates(EditorWindow window, float w, float h, ref float _width, ref float _defaultWidth, ref float _height, ref float _defaultHeight, ref float _x, ref float _defaultX, ref float _y, ref float _defaultY)
		{
			if (!_isFirstStarted)
			{
				_defaultWidth = w;
				_defaultHeight = h;
				_defaultX = (Screen.currentResolution.width - _defaultWidth) / 2f;
				_defaultY = (Screen.currentResolution.height - _defaultHeight) / 2f;

				_width = _defaultWidth;
				_height = _defaultHeight;
				_x = _defaultX;
				_y = _defaultY;

				_isFirstStarted = true;
			}
			else 
			{
				if (!((ECSVDispatcher)window).IsDocked)
				{
					_width = window.position.width;
					_height = window.position.height;
					_x = window.position.x;
					_y = window.position.y;
				}
			}
		}

		public static void ShowWindow()
		{
			ECSVDispatcher window = (ECSVDispatcher)EditorWindow.GetWindow(typeof(ECSVDispatcher), false);
			window.Show();

			window.SetCoordinates(window, window._startedWidth, window._startedHeight, ref _width, ref _defaultWidth, ref _height, ref _defaultHeight, ref _x, ref _defaultX, ref _y, ref _defaultY);

			window.titleContent = new GUIContent("CSV Dispatcher");
		}

		private void Defocus()
		{
			GUI.SetNextControlName("");
			GUI.TextField(new Rect(0, 0, 0, 0), "");
			GUI.FocusControl("");
		}

		private void SelectObject(UnityEngine.Object obj)
		{

			EditorGUIUtility.PingObject(obj); 
			Selection.activeObject = obj;
		}
		private void ResetContent()
		{
			_superArray = null;
			_castingNames.Clear();
			_isEditing = false;
			//_baseText = null;
			Defocus();
		}
	}
}