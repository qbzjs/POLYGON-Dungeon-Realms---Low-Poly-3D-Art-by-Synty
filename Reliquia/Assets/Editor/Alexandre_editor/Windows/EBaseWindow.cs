using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using System;
using System.Reflection;

namespace AlexandreDialogues
{
	public abstract class EBaseWindow : EditorWindow
	{
		protected SerializedObject _targetSO; 

		protected Vector2[] _scrollEntries;

		protected Color _lineColor = new Color(0f, 0f, 0f, 0.3f);
		protected GUIStyle _myStyle;

		protected bool IsDocked
		{
			get
			{
				BindingFlags flags = BindingFlags.NonPublic | BindingFlags.Instance;
				MethodInfo method = GetType().GetProperty("docked", flags).GetGetMethod(true);
				return (bool)method.Invoke(this, null);
			}
		}

		private void OnEnable()
		{
			if (_myStyle == null)
			{
				_myStyle = new GUIStyle(EditorStyles.textArea);
				_myStyle.wordWrap = true;
			}
		}

		private void OnGUI()
		{
			if (EditorApplication.isPlayingOrWillChangePlaymode)
			{
				this.Close();
				return;
			}

			_targetSO.Update();

			EditorGUILayout.BeginHorizontal();

			//----------------------------------------
			DrawContent();
			//----------------------------------------

			EditorGUILayout.EndHorizontal();

			_targetSO.ApplyModifiedProperties();
		}

		protected abstract void DrawContent(); // les classes dérivées doivent implémenter cela

		protected void SetCoordinates(EditorWindow window, float w, float h, ref float _width, ref float _defaultWidth, ref float _height, ref float _defaultHeight, ref float _x, ref float _defaultX, ref float _y, ref float _defaultY)
		{
			if (_defaultWidth == default)
			{
				_defaultWidth = w;
				_defaultHeight = h;
				_defaultX = (Screen.currentResolution.width - _defaultWidth) / 2f;
				_defaultY = (Screen.currentResolution.height - _defaultHeight) / 2f;

				_width = _defaultWidth;
				_height = _defaultHeight;
				_x = _defaultX;
				_y = _defaultY;
			}
			else 
			{
				_width = window.position.width;
				_height = window.position.height;
				_x = window.position.x;
				_y = window.position.y;
			}

			if (!((EBaseWindow)window).IsDocked)
			{
				window.position = new Rect(_x, _y, _width, _height);
			}
		}

		protected void DrawLabel(string text, int spaceAbove)
		{
			GUILayout.Space(spaceAbove);

			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);
			GUILayout.Label(text, EditorStyles.boldLabel);
			EditorGUILayout.EndHorizontal();

			GUILayout.Space(10);
		}

		protected void Defocus()
		{
			GUI.SetNextControlName("");
			GUI.TextField(new Rect(0, 0, 0, 0), "");
			GUI.FocusControl("");
		}

		protected void HorizontalLine(float posY, Color color)
		{
			// Screen.width relatif au groupe conteneur
			Rect myRect = new Rect(0, posY, Screen.width, 1f);
			EditorGUI.DrawRect(myRect, color);
		}

		protected void VerticalLine(float posX, Color color)
		{
			Rect myRect = new Rect(posX, 0f, 1f, Screen.height);
			EditorGUI.DrawRect(myRect, color);
		}

		protected void DisplayElement(string labelName, string description, int labelWidth, Action myMethodName)
		{
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);
			EditorGUILayout.LabelField(new GUIContent(labelName, description), GUILayout.Width(labelWidth));
			myMethodName();
			EditorGUILayout.EndHorizontal();
		}
		// usage : DisplayElement("toto", "titi", 111, () => MyMethod("text"));

		protected void DisplayElementEvent(Action myMethodName)
		{
			GUILayout.Space(5);
			EditorGUILayout.BeginHorizontal();
			GUILayout.Space(20);
			myMethodName();
			EditorGUILayout.EndHorizontal();
		}
		// usage : DisplayElementEvent(() => MyMethod("text"));
	}
}