using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;

namespace DiasGames.ThirdPersonSystem
{
    [CustomEditor(typeof(ModifierManager))]
    [CanEditMultipleObjects]
    public class ModifierManagerInspector : BaseInspector
    {
        SerializedProperty modifiers;
        string[] displayedOptions;

        List<Type> m_Modifiers = new List<Type>();
        int selectedModifier = 0;

        CharacterShooterDrawer shooterDrawer = new CharacterShooterDrawer();

        protected override void OnEnable()
        {
            base.OnEnable();

            modifiers = serializedObject.FindProperty("m_Modifiers");
            
            // Search through all of the assemblies to find any types that derive from Ability.
            m_Modifiers.Clear();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();

            for (int i = 0; i < assemblies.Length; ++i)
            {
                var assemblyTypes = assemblies[i].GetTypes();
                for (int j = 0; j < assemblyTypes.Length; ++j)
                {
                    // Must derive from Modifier.
                    if (!typeof(Modifier).IsAssignableFrom(assemblyTypes[j]))
                    {
                        continue;
                    }

                    // Ignore abstract classes.
                    if (assemblyTypes[j].IsAbstract)
                    {
                        continue;
                    }
                    
                        m_Modifiers.Add(assemblyTypes[j]);
                }
            }

            if (EditorPrefs.HasKey("Selected"))
                selectedModifier = EditorPrefs.GetInt("Selected");

            Modifier[] allModifiers = (target as MonoBehaviour).GetComponents<Modifier>();
            foreach (Modifier mod in allModifiers)
                mod.hideFlags = HideFlags.HideInInspector;

            UpdateLabels();
        }

        public override void OnInspectorGUI()
        {
            DrawImageHeader();

            EditorGUILayout.BeginHorizontal();

            selectedModifier = EditorGUILayout.Popup("Select Modifier to edit", selectedModifier, displayedOptions);

            if(GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
            {
                TryAddModifier();
            }

            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20)))
            {
                TryRemoveModifier();
            }

            EditorGUILayout.EndHorizontal();
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(contentSkin.box);
            DrawModifiers();
            EditorGUILayout.EndVertical();
        }

        private void UpdateLabels()
        {
            displayedOptions = new string[modifiers.arraySize];
            for (int i = 0; i < modifiers.arraySize; i++)
                displayedOptions[i] = SplitUpperCase(modifiers.GetArrayElementAtIndex(i).objectReferenceValue.GetType().Name);
        }

        private void DrawModifiers()
        {
            if (modifiers.arraySize == 0)
                return;

            selectedModifier = Mathf.Clamp(selectedModifier, 0, modifiers.arraySize-1);

            if (modifiers.GetArrayElementAtIndex(selectedModifier) == null)
                return;

            EditorGUILayout.BeginVertical();

            EditorGUI.InspectorTitlebar(GUILayoutUtility.GetRect(0, 0), false, modifiers.GetArrayElementAtIndex(selectedModifier).objectReferenceValue, false);
            GUILayout.Space(20);
            if (modifiers.GetArrayElementAtIndex(selectedModifier).objectReferenceValue.GetType().Name.Contains("CharacterShooterController"))
            {
                shooterDrawer.GetProperties(new SerializedObject(modifiers.GetArrayElementAtIndex(selectedModifier).objectReferenceValue));
                shooterDrawer.DrawInspector();
            }
            else
                DrawModifierByIterator();

            EditorGUILayout.EndVertical();

            EditorGUILayout.Space();
        }

        private void DrawModifierByIterator()
        {
            SerializedObject modObject = new SerializedObject(modifiers.GetArrayElementAtIndex(selectedModifier).objectReferenceValue);

            modObject.Update();
            EditorGUILayout.PropertyField(modObject.FindProperty("m_Script"));
            SerializedProperty property = modObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (!property.name.Contains("Script"))
                        EditorGUILayout.PropertyField(property, true);

                } while (property.NextVisible(false));
            }
            modObject.ApplyModifiedProperties();
        }

        private void TryAddModifier()
        {
            GenericMenu menu = new GenericMenu();
            for(int i=0; i <m_Modifiers.Count; i++)
            {
                if ((serializedObject.targetObject as MonoBehaviour).GetComponent(m_Modifiers[i]) != null)
                    continue;

                menu.AddItem(new GUIContent(m_Modifiers[i].Name), false, AddModifier, m_Modifiers[i]);
            }

            menu.ShowAsContext();
        }

        private void TryRemoveModifier()
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < modifiers.arraySize; i++)
            {
                menu.AddItem(new GUIContent(modifiers.GetArrayElementAtIndex(i).objectReferenceValue.GetType().Name), 
                    false, RemoveModifier, modifiers.GetArrayElementAtIndex(i).objectReferenceValue);

            }

            menu.ShowAsContext();
        }

        private void AddModifier(object obj)
        {
            var modifier = Undo.AddComponent((serializedObject.targetObject as MonoBehaviour).gameObject, obj as Type) as Modifier;
            modifier.hideFlags = HideFlags.HideInInspector;

            modifiers.InsertArrayElementAtIndex(modifiers.arraySize);
            modifiers.GetArrayElementAtIndex(modifiers.arraySize - 1).objectReferenceValue = modifier;

            serializedObject.ApplyModifiedProperties();
            UpdateLabels();
        }

        private void RemoveModifier(object obj)
        {
            int index = 0;
            for(int i=0; i<modifiers.arraySize; i++)
            {
                if(modifiers.GetArrayElementAtIndex(i).objectReferenceValue == obj as Modifier)
                {
                    index = i;
                    break;
                }
            }

            modifiers.DeleteArrayElementAtIndex(index);
            if (modifiers.GetArrayElementAtIndex(index).objectReferenceValue == null)
                modifiers.DeleteArrayElementAtIndex(index);

            Undo.DestroyObjectImmediate(obj as Modifier);

            serializedObject.ApplyModifiedProperties();
            UpdateLabels();
        }

        private void OnDisable()
        {
            EditorPrefs.SetInt("Selected", selectedModifier);
        }
    }
}