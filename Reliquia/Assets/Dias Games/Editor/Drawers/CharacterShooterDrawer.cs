using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    public class CharacterShooterDrawer
    {
        SerializedObject serializedObject;

        SerializedProperty m_Inventory, script;
        SerializedProperty useGrounded, abilities;

        public void GetProperties(SerializedObject target)
        {
            serializedObject = target;

            m_Inventory = serializedObject.FindProperty("m_Inventory");
            script = serializedObject.FindProperty("m_Script");
            useGrounded = serializedObject.FindProperty("m_UseWeaponOnlyGrounded");
            abilities = serializedObject.FindProperty("m_AbilitiesAllowed");
        }

        public void DrawInspector()
        {
            EditorGUILayout.PropertyField(script);
            serializedObject.Update();

            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(m_Inventory);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(useGrounded);
            EditorGUILayout.Space();

            EditorGUILayout.BeginVertical(GUI.skin.box);
            GUILayout.Label("Allowed Abilities", EditorStyles.boldLabel);

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.HelpBox("Shooter Controller will only use weapons for abilities that are included in this list." +
                " Use + and - buttons to add or remove ability that allow shooter.", MessageType.Info);

            if(GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(20)))
            {
                GenericMenu m_Menu = new GenericMenu();
                List<ThirdPersonAbility> allAbilities = new List<ThirdPersonAbility>();
                allAbilities.AddRange((serializedObject.targetObject as MonoBehaviour).GetComponents<ThirdPersonAbility>());

                for(int i=0; i < abilities.arraySize; i++)
                {
                    if (allAbilities.Contains(abilities.GetArrayElementAtIndex(i).objectReferenceValue as ThirdPersonAbility))
                        allAbilities.Remove(abilities.GetArrayElementAtIndex(i).objectReferenceValue as ThirdPersonAbility);
                }

                foreach(ThirdPersonAbility ability in allAbilities)
                    m_Menu.AddItem(new GUIContent(ability.GetType().Name), false, AddAllowedAbility, ability);

                m_Menu.ShowAsContext();
            }

            if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(20)))
            {
                GenericMenu m_Menu = new GenericMenu();
                for (int i = 0; i < abilities.arraySize; i++)
                    m_Menu.AddItem(new GUIContent(abilities.GetArrayElementAtIndex(i).objectReferenceValue.GetType().Name), 
                        false, RemoveAllowedAbility, abilities.GetArrayElementAtIndex(i).objectReferenceValue);

                m_Menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            for (int i = 0; i < abilities.arraySize; i++)
                EditorGUILayout.PropertyField(abilities.GetArrayElementAtIndex(i), 
                    new GUIContent(abilities.GetArrayElementAtIndex(i).objectReferenceValue.GetType().Name));

            EditorGUILayout.Space();
            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        private void AddAllowedAbility(object ability)
        {
            var targetAbility = ability as ThirdPersonAbility;

            abilities.InsertArrayElementAtIndex(abilities.arraySize);
            abilities.GetArrayElementAtIndex(abilities.arraySize - 1).objectReferenceValue = targetAbility;

            serializedObject.ApplyModifiedProperties();
        }

        private void RemoveAllowedAbility(object ability)
        {
            int index = -1;
            ThirdPersonAbility abilityToRemove = ability as ThirdPersonAbility;
            for (int i = 0; i < abilities.arraySize; i++)
            {
                if (abilities.GetArrayElementAtIndex(i).objectReferenceValue == abilityToRemove)
                {
                    index = i;
                    break;
                }
            }

            abilities.DeleteArrayElementAtIndex(index);
            if (abilities.GetArrayElementAtIndex(index).objectReferenceValue == null)
                abilities.DeleteArrayElementAtIndex(index);

            serializedObject.ApplyModifiedProperties();
        }
    }
}