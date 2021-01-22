/*------------------------------------------------------------------------------*/

// Developed by Rodrigo Dias
// Contact: rodrigoaadias@hotmail.com

/*-----------------------------------------------------------------------------*/

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    public enum DrawSection { Properties, Animation, AbilitiesToIgnore, Events, Noone }

    [CustomEditor(typeof(ThirdPersonSystem))]
    [CanEditMultipleObjects]
    public class ThirdPersonSystemInspector : BaseInspector
    {
        SerializedProperty m_GroundMask;
        SerializedProperty m_GroundCheckDistance;
        SerializedProperty m_MaxAngleSlope;

        SerializedProperty gravityAcc;

        SerializedProperty alwaysZoomProperty;

        SerializedProperty OnGroundedEvent;
        SerializedProperty OnAnyAbilityEnters, OnAnyAbilityExits;
        
        // Internal
        private enum Selection { Controller, Abilities, Events }
        private Selection selection;
        private DrawSection m_AbilitySection = DrawSection.Noone;

        List<ThirdPersonAbility> m_Abilities = new List<ThirdPersonAbility>();
        int selectedAbility = -1;

        AbilityDrawer abilityDrawer = new AbilityDrawer();
        ThirdPersonSystem m_Controller;

        List<string> labels = new List<string>();
        int abilitySelected = 0;

        List<Type> allAvailablesAbilities = new List<Type>();

        protected override void OnEnable()
        {
            base.OnEnable();
            m_Controller = target as ThirdPersonSystem;

            m_GroundMask = serializedObject.FindProperty("m_GroundMask");
            customProperties.Add(m_GroundMask.name);

            m_GroundCheckDistance = serializedObject.FindProperty("m_GroundCheckDistance");
            customProperties.Add(m_GroundCheckDistance.name);

            m_MaxAngleSlope = serializedObject.FindProperty("m_MaxAngleSlope");
            customProperties.Add(m_MaxAngleSlope.name);

            gravityAcc = serializedObject.FindProperty("m_GravityAcceleration");
            customProperties.Add(gravityAcc.name);

            alwaysZoomProperty = serializedObject.FindProperty("m_AlwaysZoomCamera");
            customProperties.Add(alwaysZoomProperty.name);
            
            OnAnyAbilityEnters = serializedObject.FindProperty("OnAnyAbilityEnters");
            customProperties.Add(OnAnyAbilityEnters.name);

            OnAnyAbilityExits = serializedObject.FindProperty("OnAnyAbilityExits");
            customProperties.Add(OnAnyAbilityExits.name);

            OnGroundedEvent = serializedObject.FindProperty("OnGrounded");
            customProperties.Add(OnGroundedEvent.name);

            if (EditorPrefs.HasKey("Selection"))
                selection = (Selection)EditorPrefs.GetInt("Selection");

            if (EditorPrefs.HasKey("SelectedAbility"))
                selectedAbility = EditorPrefs.GetInt("SelectedAbility");

            if (EditorPrefs.HasKey("AbilitySelection"))
                m_AbilitySection = (DrawSection)EditorPrefs.GetInt("AbilitySelection");

            UpdateAbilitiesList();

            allAvailablesAbilities.Clear();
            var assemblies = AppDomain.CurrentDomain.GetAssemblies();
            for(int i=0; i< assemblies.Length; i++)
            {
                var types = assemblies[i].GetTypes();
                for (int j = 0; j < types.Length; ++j)
                {
                    // Must derive from Modifier.
                    if (!typeof(ThirdPersonAbility).IsAssignableFrom(types[j]))
                    {
                        continue;
                    }

                    // Ignore abstract classes.
                    if (types[j].IsAbstract)
                    {
                        continue;
                    }

                    allAvailablesAbilities.Add(types[j]);
                }
            }

            Undo.undoRedoPerformed += UpdateAbilitiesList;
        }

        private void UpdateAbilitiesList()
        {
            m_Abilities.Clear();
            labels.Clear();
            m_Abilities.AddRange((serializedObject.targetObject as MonoBehaviour).GetComponents<ThirdPersonAbility>());
            for (int i = 0; i < m_Abilities.Count; i++)
            {
                m_Abilities[i].hideFlags = HideFlags.HideInInspector;
                labels.Add(FormatLabel(m_Abilities[i].GetType().Name));
            }

            if (selectedAbility >= m_Abilities.Count)
                selectedAbility = m_Abilities.Count - 1;
        }

        public override void OnInspectorGUI()
        {
            DrawImageHeader();

            EditorGUILayout.PropertyField(serializedObject.FindProperty("m_Script"));
            serializedObject.Update();

            if (Application.isPlaying)
            {
                EditorGUILayout.BeginHorizontal(contentSkin.box);

                EditorGUILayout.LabelField("Current Active Ability");

                string activeAbility = (m_Controller.ActiveAbility == null) ? "Null" : m_Controller.ActiveAbility.GetType().Name;
                EditorGUILayout.LabelField(activeAbility, EditorStyles.boldLabel);

                EditorGUILayout.EndHorizontal();

                EditorGUILayout.Space();
            }

            EditorGUILayout.Space();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Controller Settings", (selection == Selection.Controller) ? active : normal, GUILayout.Height(25)))
                selection = Selection.Controller;
            if (GUILayout.Button("Character Abilities", (selection == Selection.Abilities) ? active : normal, GUILayout.Height(25)))
                selection = Selection.Abilities;
            if (GUILayout.Button("Events", (selection == Selection.Events) ? active : normal, GUILayout.Height(25)))
                selection = Selection.Events;

            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space();

            switch (selection)
            {
                case Selection.Controller:

                    EditorGUILayout.PropertyField(m_GroundMask);
                    EditorGUILayout.PropertyField(m_GroundCheckDistance);
                    EditorGUILayout.PropertyField(m_MaxAngleSlope);

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(gravityAcc);

                    EditorGUILayout.Space();

                    EditorGUILayout.PropertyField(alwaysZoomProperty);

                    EditorGUILayout.Space();

                    // Draw all other properties
                    DrawExtraParameters();
                    break;
                case Selection.Abilities:
                    EditorGUILayout.BeginHorizontal();
                    
                    selectedAbility = EditorGUILayout.Popup("Select ability to edit", selectedAbility, labels.ToArray());

                    if (GUILayout.Button("+", EditorStyles.miniButtonLeft, GUILayout.Width(25)))
                    {
                        TryAddAbility();
                    }
                    if (GUILayout.Button("-", EditorStyles.miniButtonRight, GUILayout.Width(25)))
                    {
                        TryRemoveAbility();
                    }

                    EditorGUILayout.EndHorizontal();

                    EditorGUILayout.Space();

                    
                    SetDrawer();

                    EditorGUILayout.BeginVertical(contentSkin.box);
                    EditorGUI.indentLevel++;

                    DrawAbilityInspector();

                    EditorGUI.indentLevel--;
                    EditorGUILayout.Space();

                    EditorGUILayout.EndVertical();

                    break;
                case Selection.Events:
                    DrawEventsRegion(normal, active);
                    break;
                default:
                    break;
            }

            EditorGUILayout.Space();
            
            serializedObject.ApplyModifiedProperties();
        }


        private void TryRemoveAbility()
        {
            GenericMenu menu = new GenericMenu();
            for (int i = 0; i < m_Abilities.Count; i++)
                menu.AddItem(new GUIContent(FormatLabel(m_Abilities[i].GetType().Name)), false, RemoveAbility, m_Abilities[i]);
            
            menu.ShowAsContext();
        }

        private void TryAddAbility()
        {
            GenericMenu menu = new GenericMenu();
            for(int i=0; i<allAvailablesAbilities.Count; i++)
            {
                if ((serializedObject.targetObject as MonoBehaviour).GetComponent(allAvailablesAbilities[i]) != null)
                    continue;

                menu.AddItem(new GUIContent(FormatLabel(allAvailablesAbilities[i].Name)), false, AddAbility, allAvailablesAbilities[i]);
            }

            menu.ShowAsContext();
        }

        private void AddAbility(object targetAbility)
        {
            var ability = Undo.AddComponent((serializedObject.targetObject as MonoBehaviour).gameObject, targetAbility as Type) as ThirdPersonAbility;
            ability.hideFlags = HideFlags.HideInInspector;
            UpdateAbilitiesList();

            selectedAbility = m_Abilities.Count - 1;

            serializedObject.ApplyModifiedProperties();
        }
        private void RemoveAbility(object targetAbility)
        {
            var ability = targetAbility as ThirdPersonAbility;
            Undo.DestroyObjectImmediate(ability);
            UpdateAbilitiesList();


            serializedObject.ApplyModifiedProperties();
        }

        private void SetDrawer()
        {
            if (m_Abilities[selectedAbility].GetType().FullName.Contains("ClimbingAbility"))
            {
                if (!(abilityDrawer is ClimbingDrawer))
                    abilityDrawer = new ClimbingDrawer();
            }
            else if (m_Abilities[selectedAbility].GetType().FullName.Contains("ClimbJump"))
            {
                if (!(abilityDrawer is ClimbJumpDrawer))
                    abilityDrawer = new ClimbJumpDrawer();
            }
            else if (m_Abilities[selectedAbility].GetType().FullName.Contains("Ladder"))
            {
                if (!(abilityDrawer is LadderDrawer))
                    abilityDrawer = new LadderDrawer();
            }
            else if (m_Abilities[selectedAbility].GetType().BaseType.FullName.Contains("AbstractClimbing"))
            {
                if (!(abilityDrawer is ClimbAbilityDrawer) || abilityDrawer is ClimbingDrawer)
                    abilityDrawer = new ClimbAbilityDrawer();
            }
            else if (m_Abilities[selectedAbility].GetType().FullName.Contains("Fall"))
            {
                if (!(abilityDrawer is FallDrawer))
                    abilityDrawer = new FallDrawer();
            }
            else if (m_Abilities[selectedAbility].GetType().FullName.Contains("Crawl"))
            {
                if (!(abilityDrawer is CrawlDrawer))
                    abilityDrawer = new CrawlDrawer();
            }
            else if (m_Abilities[selectedAbility].GetType().FullName.Contains("WallRun"))
            {
                if (!(abilityDrawer is WallRunDrawer))
                    abilityDrawer = new WallRunDrawer();
            }
            else if (m_Abilities[selectedAbility].GetType().FullName.Contains("WallClimb"))
            {
                if (!(abilityDrawer is WallClimbDrawer))
                    abilityDrawer = new WallClimbDrawer();
            }
            else if (m_Abilities[selectedAbility].GetType().FullName.Contains("Cover"))
            {
                if (!(abilityDrawer is CoverDrawer))
                    abilityDrawer = new CoverDrawer();
            }
            else
            {
                if (!abilityDrawer.GetType().Name.Equals("AbilityDrawer"))
                    abilityDrawer = new AbilityDrawer();
            }
        }

        private void DrawAbilityInspector()
        {
            EditorGUI.InspectorTitlebar(GUILayoutUtility.GetRect(0, 0), false, m_Abilities[selectedAbility], false);
            abilityDrawer.GetProperties(new SerializedObject(m_Abilities[selectedAbility]));
            abilityDrawer.DrawInspector(miniActive, miniNormal, contentSkin, headerSkin,
                SplitUpperCase(m_Abilities[selectedAbility].GetType().Name), ref m_AbilitySection);
        }

        private void DrawExtraParameters()
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

        private void DrawEventsRegion(GUIStyle normal, GUIStyle active)
        {
            EditorGUILayout.PropertyField(OnGroundedEvent);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(OnAnyAbilityEnters);
            EditorGUILayout.PropertyField(OnAnyAbilityExits);
        }

        private void OnDisable()
        {
            EditorPrefs.SetInt("SelectedAbility", selectedAbility);
            EditorPrefs.SetInt("Selection", (int)selection);
            EditorPrefs.SetInt("AbilitySelection", (int)m_AbilitySection);
        }
    }
}
