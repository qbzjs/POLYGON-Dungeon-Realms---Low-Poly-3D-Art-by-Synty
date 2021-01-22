using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    [CanEditMultipleObjects]
    [CustomEditor(typeof(UnityInputManager))]
    public class InputManagerInspector : BaseInspector
    {

        private enum InputSection { Ability, Shooter, Extra, Noone }
        private InputSection section = InputSection.Noone;

        private SerializedProperty camera;

        protected override void OnEnable()
        {
            base.OnEnable();

            camera = serializedObject.FindProperty("m_Camera");
        }

        protected override void FormatLabel()
        {
            label = "Input Manager";
        }

        public override void OnInspectorGUI()
        {
            DrawImageHeader();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            serializedObject.Update();

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(camera);

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Abilities Input", (section == InputSection.Ability) ? active : normal))
                section = (section == InputSection.Ability) ? InputSection.Noone : InputSection.Ability;

            if (GUILayout.Button("Extra Input", (section == InputSection.Extra) ? active : normal))
                section = (section == InputSection.Extra) ? InputSection.Noone : InputSection.Extra;

            if (GUILayout.Button("Shooter Input", (section == InputSection.Shooter) ? active : normal))
                section = (section == InputSection.Shooter) ? InputSection.Noone : InputSection.Shooter;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical();

            switch (section)
            {
                case InputSection.Ability:
                    DrawAbilitiesInput();
                    break;
                case InputSection.Shooter:
                    DrawShooterInput();
                    break;
                case InputSection.Extra:
                    DrawExtraInput();
                    break;
                case InputSection.Noone:
                    break;
                default:
                    break;
            }
            
            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }
        private void DrawAbilitiesInput()
        {
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name.Contains("m_Script") || property.name.Contains("m_Camera"))
                        continue;

                    if ((property.name.Contains("Weapon") || property.name.Contains("Fire") || property.name.Contains("Reload")))
                        continue;

                    if (property.name.Contains("Extra"))
                        continue;

                    EditorGUILayout.PropertyField(property, true);

                } while (property.NextVisible(false));
            }
        }

        private void DrawExtraInput()
        {
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name.Contains("Extra"))
                        EditorGUILayout.PropertyField(property, true);

                } while (property.NextVisible(false));
            }
        }

        private void DrawShooterInput()
        {
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if ((property.name.Contains("Weapon") || property.name.Contains("Fire") || property.name.Contains("Reload")))
                        EditorGUILayout.PropertyField(property, true);

                } while (property.NextVisible(false));
            }
        }
    }
}
