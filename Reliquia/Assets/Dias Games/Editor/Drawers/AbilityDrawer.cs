using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    public class AbilityDrawer
    {
        protected List<string> customProperties = new List<string>();
        protected SerializedObject serializedObject;

        // Serialized properties
        protected SerializedProperty m_EnterState;
        protected SerializedProperty m_TransitionDuration;
        SerializedProperty m_RootMotionMultiplier;
        protected SerializedProperty m_FinishOnAnimationEnd;
        SerializedProperty IgnoreAbilities;
        SerializedProperty m_UseRootMotion;
        SerializedProperty m_UseRotationRootMotion;
        SerializedProperty m_UseVerticalRootMotion;

        SerializedProperty allowZoomProperty;

        SerializedProperty abilityEnterType;
        SerializedProperty inputButton;
        SerializedProperty enabled;


        SerializedProperty OnEnterAbilityEvent, OnExitAbilityEvent;

        // Internal vars
        protected ThirdPersonAbility m_CharacterAbilityTarget;
        protected string m_EnterStateLabel = "Enter State";

        protected bool drawFinish = true;
        protected bool drawEnterState = true;

        protected virtual string FormatLabel(string label)
        {
            if (label.Contains("Ability"))
                label = label.Remove(label.IndexOf("Ability"), "Ability".Length);

            return label;
        }

        public virtual void GetProperties(SerializedObject targetObject)
        {
            serializedObject = targetObject;
            //LoadDefaultVars();

            m_EnterState = serializedObject.FindProperty("m_EnterState");
            customProperties.Add(m_EnterState.name);

            m_TransitionDuration = serializedObject.FindProperty("m_TransitionDuration");
            customProperties.Add(m_TransitionDuration.name);

            m_FinishOnAnimationEnd = serializedObject.FindProperty("m_FinishOnAnimationEnd");
            customProperties.Add(m_FinishOnAnimationEnd.name);

            m_UseRootMotion = serializedObject.FindProperty("m_UseRootMotion");
            customProperties.Add(m_UseRootMotion.name);

            m_RootMotionMultiplier = serializedObject.FindProperty("m_RootMotionMultiplier");
            customProperties.Add(m_RootMotionMultiplier.name);

            m_UseRotationRootMotion = serializedObject.FindProperty("m_UseRotationRootMotion");
            customProperties.Add(m_UseRotationRootMotion.name);

            m_UseVerticalRootMotion = serializedObject.FindProperty("m_UseVerticalRootMotion");
            customProperties.Add(m_UseVerticalRootMotion.name);

            IgnoreAbilities = serializedObject.FindProperty("IgnoreAbilities");
            customProperties.Add(IgnoreAbilities.name);

            OnEnterAbilityEvent = serializedObject.FindProperty("OnEnterAbilityEvent");
            customProperties.Add(OnEnterAbilityEvent.name);

            OnExitAbilityEvent = serializedObject.FindProperty("OnExitAbilityEvent");
            customProperties.Add(OnExitAbilityEvent.name);


            allowZoomProperty = serializedObject.FindProperty("m_AllowCameraZoom");
            customProperties.Add(allowZoomProperty.name);
            
            abilityEnterType = serializedObject.FindProperty("m_UseInputStateToEnter");
            customProperties.Add(abilityEnterType.name);

            inputButton = serializedObject.FindProperty("InputButton");
            customProperties.Add(inputButton.name);

            enabled = serializedObject.FindProperty("m_Enabled");

            m_CharacterAbilityTarget = serializedObject.targetObject as ThirdPersonAbility;
            if (m_CharacterAbilityTarget.IgnoreAbilities == null)
                m_CharacterAbilityTarget.IgnoreAbilities = new List<ThirdPersonAbility>();
            
        }

        public virtual void DrawInspector(GUIStyle active, GUIStyle normal, GUISkin contentSkin, GUISkin header, string abilityName, ref DrawSection drawSection)
        {
            serializedObject.Update();
            GUILayout.Space(20);
            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));

            GUILayout.Space(10);

            #region Buttons
            EditorGUILayout.BeginHorizontal();

            if (GUILayout.Button("Ability Settings", (drawSection == DrawSection.Properties) ? active : normal))
            {
                if (drawSection == DrawSection.Properties)
                    drawSection = DrawSection.Noone;
                else
                    drawSection = DrawSection.Properties;
            }

            if (GUILayout.Button("Animation", (drawSection == DrawSection.Animation) ? active : normal))
                drawSection = DrawSection.Animation;
            if (GUILayout.Button("Abilities To Ignore", (drawSection == DrawSection.AbilitiesToIgnore) ? active : normal))
                drawSection = DrawSection.AbilitiesToIgnore;
            if (GUILayout.Button("Events", (drawSection == DrawSection.Events) ? active : normal))
                drawSection = DrawSection.Events;

            EditorGUILayout.EndHorizontal();
            #endregion

            EditorGUILayout.BeginVertical();

            EditorGUILayout.Space();

            switch (drawSection)
            {
                case DrawSection.Properties:
                    DrawProperties(contentSkin);
                    break;
                case DrawSection.Animation:
                    DrawAnimation(contentSkin);
                    EditorGUILayout.Space();
                    DrawRootMotionProperties();
                    break;
                case DrawSection.AbilitiesToIgnore:
                    DrawIgnoreAbilities();
                    break;
                case DrawSection.Events:
                    DrawEvents();
                    break;
                default:
                    break;
            }

            EditorGUILayout.EndVertical();

            serializedObject.ApplyModifiedProperties();
        }

        protected virtual void DrawInput(GUISkin contentSkin)
        {
            EditorGUILayout.PropertyField(abilityEnterType);
            if (abilityEnterType.intValue != (int)InputEnterType.Noone)
            {
                EditorGUI.indentLevel++;
                EditorGUILayout.PropertyField(inputButton);
                EditorGUI.indentLevel--;
            }
        }

        protected virtual void DrawAnimation(GUISkin contentSkin)
        {
            if (drawEnterState)
            {
                EditorGUILayout.PropertyField(m_EnterState, new GUIContent(m_EnterStateLabel));
                EditorGUILayout.Space();
            }

            EditorGUILayout.PropertyField(m_TransitionDuration);
            EditorGUILayout.Space();

            if (drawFinish)
                EditorGUILayout.PropertyField(m_FinishOnAnimationEnd);
        }

        protected void DrawIgnoreAbilities()
        {
            EditorGUI.indentLevel++;

            // -------------------------------------- ADD AND REMOVE ABILITIES BUTTONS -------------------------------------- //

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.HelpBox("Add or remove abilities to be ignored when this ability request to enter. " +
                "All abilities that are inside ignore abilities will be stopped to allow this ability to enter. " +
                "If Ignore abilities were void, this ability will enter only if noone ability is active", MessageType.Info);

            EditorGUILayout.Space();

            // AddButton Style
            GUIStyle addButtonStyle = new GUIStyle(EditorStyles.miniButtonLeft);
            addButtonStyle.alignment = TextAnchor.MiddleRight;
            addButtonStyle.fontStyle = FontStyle.Bold;
            addButtonStyle.fontSize = 13;
            addButtonStyle.stretchWidth = false;

            if (GUILayout.Button("+", addButtonStyle))
            {
                GenericMenu m_Menu = new GenericMenu();
                ThirdPersonAbility[] allAbilities = m_CharacterAbilityTarget.GetComponents<ThirdPersonAbility>();
                foreach (ThirdPersonAbility ability in allAbilities)
                {
                    if (m_CharacterAbilityTarget.IgnoreAbilities.Contains(ability) || ability == m_CharacterAbilityTarget)
                        continue;

                    m_Menu.AddItem(new GUIContent(ability.GetType().Name), false, AddIgnoredAbility, ability);
                }

                m_Menu.ShowAsContext();
            }

            // RemoveButton Style
            GUIStyle removeButtonStyle = new GUIStyle(EditorStyles.miniButtonRight);
            removeButtonStyle.alignment = TextAnchor.MiddleCenter;
            removeButtonStyle.fontStyle = FontStyle.Bold;
            removeButtonStyle.fontSize = 13;
            removeButtonStyle.stretchWidth = false;

            if (GUILayout.Button("-", removeButtonStyle))
            {
                GenericMenu m_Menu = new GenericMenu();
                foreach (ThirdPersonAbility ability in m_CharacterAbilityTarget.IgnoreAbilities)
                {
                    m_Menu.AddItem(new GUIContent(ability.GetType().Name), false, RemoveIgnoredAbility, ability);
                }

                m_Menu.ShowAsContext();
            }

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            // ----------------------------------------------------------------------------------------------------- //



            // Draw ignored abilities
            for (int i = 0; i < IgnoreAbilities.arraySize; i++)
            {
                if (IgnoreAbilities.GetArrayElementAtIndex(i).objectReferenceValue == null)
                {
                    IgnoreAbilities.DeleteArrayElementAtIndex(i);
                    continue;
                }

                EditorGUILayout.PropertyField(IgnoreAbilities.GetArrayElementAtIndex(i),
                    new GUIContent(FormatLabel(IgnoreAbilities.GetArrayElementAtIndex(i).objectReferenceValue.GetType().Name)));
            }

            EditorGUI.indentLevel--;
        }

        protected virtual void DrawRootMotionProperties()
        {
            EditorGUILayout.PropertyField(m_UseRootMotion);
            if (m_CharacterAbilityTarget.UseRootMotion)
            {
                EditorGUI.indentLevel++;

                EditorGUILayout.PropertyField(m_UseVerticalRootMotion);
                EditorGUILayout.PropertyField(m_UseRotationRootMotion);

                EditorGUILayout.Space();

                DrawRootMotionMultiplier();

                EditorGUI.indentLevel--;
            }
        }

        protected virtual void DrawRootMotionMultiplier()
        {
            EditorGUILayout.PropertyField(m_RootMotionMultiplier);
        }

        protected virtual void DrawUniqueProperties(GUISkin contentSkin)
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

        protected virtual void DrawProperties(GUISkin contentSkin)
        {
            DrawInput(contentSkin);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(allowZoomProperty);

            EditorGUILayout.Space();

            DrawUniqueProperties(contentSkin);
        }


        protected void DrawEvents()
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.PropertyField(OnEnterAbilityEvent);
            EditorGUILayout.PropertyField(OnExitAbilityEvent);

            EditorGUI.indentLevel--;
        }

        /// <summary>
        /// Add a new ability to be ignored by a Context menu
        /// </summary>
        /// <param name="newAbility"></param>
        void AddIgnoredAbility(object newAbility)
        {
            int i = IgnoreAbilities.arraySize;
            IgnoreAbilities.arraySize++;
            IgnoreAbilities.GetArrayElementAtIndex(i).objectReferenceValue = newAbility as ThirdPersonAbility;

            serializedObject.ApplyModifiedProperties();
        }

        /// <summary>
        /// Remove an ignored ability from the list
        /// </summary>
        /// <param name="currentAbility"></param>
        void RemoveIgnoredAbility(object currentAbility)
        {
            int index = -1;
            ThirdPersonAbility abilityToRemove = currentAbility as ThirdPersonAbility;
            for (int i = 0; i < IgnoreAbilities.arraySize; i++)
            {
                if (IgnoreAbilities.GetArrayElementAtIndex(i).objectReferenceValue == abilityToRemove)
                {
                    index = i;
                    break;
                }
            }

            IgnoreAbilities.DeleteArrayElementAtIndex(index);
            if (IgnoreAbilities.GetArrayElementAtIndex(index).objectReferenceValue == null)
                IgnoreAbilities.DeleteArrayElementAtIndex(index);

            serializedObject.ApplyModifiedProperties();

        }
    }
}