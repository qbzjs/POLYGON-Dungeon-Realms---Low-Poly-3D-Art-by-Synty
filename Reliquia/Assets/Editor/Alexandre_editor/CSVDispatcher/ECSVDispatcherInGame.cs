using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System.Reflection;

namespace AlexandreDialogues
{
	public class ECSVDispatcherInGame : EditorWindow
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

		private Vector2 _scrollOne = new Vector2();

		private GUIStyle _scrollStyle, _subLabelStyle, _textareaStyle;

		private bool _isEditing;

		private string _baseText = "";

		private string[][] _superArray;

		private char[] _arraySeparators = new char[] { '\n' };

		private float _defaultDisplayTime = 1f;


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

				GUILayout.BeginHorizontal();
				{
					if (!_isEditing)
					{
						GUILayout.Label(new GUIContent("Base text", "Le texte du dialogue comprenant réplique et durée, séparés par des points-virgules."), EditorStyles.boldLabel);
					}
					else
					{
						GUILayout.Label(new GUIContent("Structure", "Le contenu structuré en DialogueInGame."), EditorStyles.boldLabel);
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
					EditorGUILayout.HelpBox("Entrer un texte au format CSV. Chaque ligne représente une réplique. Chaque ligne doit contenir 3 blocs séparés par des points-virgules : nom, didascalies éventuelles, texte. Les deux premiers peuvent rester vides.", MessageType.None);
					_baseText = EditorGUILayout.TextArea(_baseText, _textareaStyle);
				}
				else if (_isEditing)
				{
					InGameDialogueReply[] replies = new InGameDialogueReply[_superArray.Length]; 

					for (int i = 0; i < _superArray.Length; i++)
					{
						replies[i] = new InGameDialogueReply(); 

						string name = _superArray[i][0];
						if (name != null && name.Length > 0)
						{
							name += " "; 
						}

						string stageDirections = _superArray[i][1];
						if (stageDirections != null && stageDirections.Length > 0)
						{
							stageDirections = $"({stageDirections}) ";
						}

						string twoPoints = null;
						if (name != null && name.Length > 0)
						{
							twoPoints = ": ";
						}

						replies[i].m_text = $"{name}{stageDirections}{twoPoints}{_superArray[i][2]}";

						replies[i].m_displayTime = _defaultDisplayTime;
					}

					GUILayout.Label(new GUIContent("Dialogue file", "Cliquer sur le bouton suivant pour créer un fichier de dialogue à l'emplacement de votre choix."), _subLabelStyle);
					if (GUILayout.Button(new GUIContent("Create Asset", "Générer le fichier.")))
					{
						string path = EditorUtility.SaveFilePanelInProject("Save dialogue", "", "asset", "Save file at path"); 
						if (path.Length != 0) 
						{
							InGameDialogue dialogue = CreateInstance<InGameDialogue>(); 
							dialogue.m_reply = replies;
							AssetDatabase.CreateAsset(dialogue, path); 

							SelectObject(dialogue);

							ResetContent();
						}
					}
				}
				// ------------------------------------------------------------
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
				if (!((ECSVDispatcherInGame)window).IsDocked)
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
			ECSVDispatcherInGame window = (ECSVDispatcherInGame)EditorWindow.GetWindow(typeof(ECSVDispatcherInGame), false);
			window.Show();

			window.SetCoordinates(window, window._startedWidth, window._startedHeight, ref _width, ref _defaultWidth, ref _height, ref _defaultHeight, ref _x, ref _defaultX, ref _y, ref _defaultY);

			window.titleContent = new GUIContent("CSV Dispatcher InGame");
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
			_isEditing = false;
			//_baseText = null;
			Defocus();
		}
	}
}