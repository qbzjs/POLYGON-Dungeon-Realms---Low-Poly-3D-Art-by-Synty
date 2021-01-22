using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    public class BaseInspector : Editor
    {
        protected Texture2D icon;
        protected GUISkin headerSkin;
        protected GUISkin contentSkin;
        protected GUISkin defaultSkin;

        protected string label = "";

        protected GUIStyle normal, miniNormal;
        protected GUIStyle active, miniActive;

        protected List<string> customProperties = new List<string>();

        protected virtual void OnEnable()
        {
            icon = (Texture2D)AssetDatabase.LoadAssetAtPath("Assets/Dias Games/Editor/GUI/tps_icon.psd", typeof(Texture2D));
            headerSkin = Resources.Load("HeaderSkin") as GUISkin;
            contentSkin = Resources.Load("ContentSkin") as GUISkin;
            defaultSkin = Resources.Load("DefaultSkin") as GUISkin;

            FormatLabel();

            normal = new GUIStyle(contentSkin.button);
            active = new GUIStyle(contentSkin.button);
            active.normal.background = active.active.background;
            active.normal.textColor = active.active.textColor;

            miniNormal = new GUIStyle(normal);
            miniActive = new GUIStyle(active);

            miniNormal.fontSize = 9;
            miniActive.fontSize = 9;
        }


        protected virtual void FormatLabel()
        {
            label = serializedObject.targetObject.GetType().Name;

            if (label.Contains("Ability"))
                label = label.Remove(label.IndexOf("Ability"), "Ability".Length);

            label = SplitUpperCase(label);
        }
        protected virtual string FormatLabel(string s)
        {
            if (s.Contains("Ability"))
                s = s.Remove(s.IndexOf("Ability"), "Ability".Length);

            s = SplitUpperCase(s);
            return s;
        }

        protected string SplitUpperCase(string s)
        {
            for (int i = 0; i < s.Length; i++)
            {
                char letter = s[i];
                if (char.IsUpper(letter) && i > 0)
                {
                    s = s.Insert(i, " ");
                    i++;
                }
            }

            return s;
        }

        protected void DrawImageHeader()
        {
            EditorGUILayout.BeginHorizontal(headerSkin.box);
            //GUILayout.Label(icon);
            GUILayout.FlexibleSpace();

            EditorGUILayout.BeginVertical();
            GUILayout.Label(label, headerSkin.label);
            EditorGUILayout.EndVertical();

            GUILayout.FlexibleSpace();
            EditorGUILayout.EndHorizontal();
        }


        public override void OnInspectorGUI()
        {
            DrawImageHeader();

            base.OnInspectorGUI();
        }

        public virtual void DrawByIterator()
        {
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (!customProperties.Contains(property.name) && !property.name.Contains("Script"))
                        EditorGUILayout.PropertyField(property, true);

                } while (property.NextVisible(false));
            }
        }

        public virtual void DrawByIterator(SerializedObject customObject)
        {
            customObject.Update();

            SerializedProperty property = customObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (!customProperties.Contains(property.name) && !property.name.Contains("Script"))
                        EditorGUILayout.PropertyField(property, true);

                } while (property.NextVisible(false));
            }

            customObject.ApplyModifiedProperties();
        }
    }
}