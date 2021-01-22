using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.Animations;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class ClimbJumpDrawer : AbilityDrawer
    {
        SerializedProperty jumpToHangState;
        SerializedProperty jumpToHangDuration;

        SerializedProperty jumpToLowerState;
        SerializedProperty jumpToLowerDuration;

        SerializedProperty normalJumpParameters;

        SerializedProperty hopRightParameters;
        SerializedProperty hopLeftParameters;

        SerializedProperty hopUpParameters;
        SerializedProperty jumpBackParameters;

        SerializedProperty useLaunch;

        public override void GetProperties(SerializedObject targetObject)
        {
            base.GetProperties(targetObject);
        
            drawEnterState = false;
            drawFinish = false;

            jumpToHangState = serializedObject.FindProperty("m_JumpToHang");
            jumpToHangDuration = serializedObject.FindProperty("jumpHangClipDuration");
            jumpToLowerState = serializedObject.FindProperty("m_JumpToLowerClimb");
            jumpToLowerDuration = serializedObject.FindProperty("jumpLowerClipDuration");

            normalJumpParameters = serializedObject.FindProperty("NormalJumpParameters");
            hopRightParameters = serializedObject.FindProperty("HopRightParameters");
            hopLeftParameters = serializedObject.FindProperty("HopLeftParameters");

            hopUpParameters = serializedObject.FindProperty("HopUpParameters");
            jumpBackParameters = serializedObject.FindProperty("JumpBackParameters");

            useLaunch = serializedObject.FindProperty("m_UseLaunchMath");

            customProperties.Add(useLaunch.name);
            customProperties.Add(jumpToHangState.name);
            customProperties.Add(jumpToHangDuration.name);
            customProperties.Add(jumpToLowerState.name);
            customProperties.Add(jumpToLowerDuration.name);

            customProperties.Add(normalJumpParameters.name);
            customProperties.Add(hopRightParameters.name);
            customProperties.Add(hopLeftParameters.name);
            customProperties.Add(hopUpParameters.name);
            customProperties.Add(jumpBackParameters.name);

            UpdateDurationClips();
        }

        public void UpdateDurationClips()
        {
            UpdateJumpParameter(hopRightParameters);
            UpdateJumpParameter(hopLeftParameters);
            UpdateJumpParameter(hopUpParameters);
            UpdateJumpParameter(jumpBackParameters);

            jumpToHangDuration.floatValue = GetStateTimeLength(jumpToHangState.stringValue);
            jumpToLowerDuration.floatValue = GetStateTimeLength(jumpToLowerState.stringValue);
        }

        private void UpdateJumpParameter(SerializedProperty property)
        {
            property.FindPropertyRelative("clipDuration").floatValue = GetStateTimeLength(property.FindPropertyRelative("m_AnimationState").stringValue);
        }

        protected override void DrawUniqueProperties(GUISkin contentSkin)
        {
            EditorGUILayout.PropertyField(useLaunch);
            if (useLaunch.boolValue == false)
                EditorGUILayout.HelpBox("Disabling math calculation will make character jump with the maximum possible" +
                    " velocity set in the Jump Parameters.", MessageType.Warning);

            EditorGUILayout.Space();

            base.DrawUniqueProperties(contentSkin);

            EditorGUILayout.Space();

            GUILayout.Label("Jump Parameters", contentSkin.label);

            EditorGUILayout.PropertyField(jumpToHangState);
            EditorGUILayout.PropertyField(jumpToHangDuration, new GUIContent("Clip Duration"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(jumpToLowerState);
            EditorGUILayout.PropertyField(jumpToLowerDuration, new GUIContent("Clip Duration"));
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(normalJumpParameters.FindPropertyRelative("m_MinJumpHeight"));
            EditorGUILayout.PropertyField(normalJumpParameters.FindPropertyRelative("m_MaxJumpHeight"));
            EditorGUILayout.PropertyField(normalJumpParameters.FindPropertyRelative("HorizontalSpeed"));

            EditorGUILayout.Space();

            GUILayout.Label("Hop Up Parameters", contentSkin.label);
            DrawJumpParameter(hopUpParameters);

            EditorGUILayout.Space();

            GUILayout.Label("Hop Right Parameters", contentSkin.label);
            DrawJumpParameter(hopRightParameters);

            EditorGUILayout.Space();

            GUILayout.Label("Hop Left Parameters", contentSkin.label);
            DrawJumpParameter(hopLeftParameters);

            EditorGUILayout.Space();

            GUILayout.Label("Jump Back From Wall Parameters", contentSkin.label);
            DrawJumpParameter(jumpBackParameters);

            EditorGUILayout.Space();
            if (GUILayout.Button("Update Clips Duration"))
                UpdateDurationClips();
        }

        private void DrawJumpParameter(SerializedProperty jumpProperty)
        {
            if (jumpProperty.FindPropertyRelative("clipDuration").floatValue <= 0)
                UpdateDurationClips();

            EditorGUILayout.PropertyField(jumpProperty.FindPropertyRelative("m_AnimationState"));
            EditorGUILayout.PropertyField(jumpProperty.FindPropertyRelative("clipDuration"));

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(jumpProperty.FindPropertyRelative("m_MinJumpHeight"));
            EditorGUILayout.PropertyField(jumpProperty.FindPropertyRelative("m_MaxJumpHeight"));
            EditorGUILayout.PropertyField(jumpProperty.FindPropertyRelative("HorizontalSpeed"));
        }

        /// <summary>
        /// Search for the desired state and return its time length
        /// </summary>
        /// <param name="stateName">Desired state name</param>
        /// <returns>Duration of the state</returns>
        private float GetStateTimeLength(string stateName)
        {
            var controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(UnityEditor.AssetDatabase.GetAssetPath((serializedObject.targetObject as MonoBehaviour).GetComponent<Animator>().runtimeAnimatorController));

            //controller.layers[0].stateMachine.
            string[] statesMachinesNames = stateName.Split('.');
            AnimatorStateMachine stateMachine = controller.layers[0].stateMachine;
            for (int i = 0; i < statesMachinesNames.Length; i++)
            {
                if (i == statesMachinesNames.Length - 1)
                {
                    foreach (ChildAnimatorState parentState in stateMachine.states)
                    {
                        if (parentState.state.name == statesMachinesNames[i])
                            return parentState.state.motion.averageDuration;
                    }
                    break;
                }

                //string name = statesMachinesNames[i];
                foreach (ChildAnimatorStateMachine machine in stateMachine.stateMachines)
                {
                    if (machine.stateMachine.name == statesMachinesNames[i])
                    {
                        stateMachine = machine.stateMachine;
                        break;
                    }
                }

            }

            return -1f;
        }
    }
}