using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace DiasGames.ThirdPersonSystem
{
    public class LadderDrawer : AbilityDrawer
    {
        SerializedProperty ladderClimbUp;
        SerializedProperty ladderHopUp;
        SerializedProperty ladderHopRight;
        SerializedProperty ladderHopLeft;

        SerializedProperty useMath;

        public override void GetProperties(SerializedObject targetObject)
        {
            base.GetProperties(targetObject);

            m_EnterStateLabel = "Climb Ladder State";

            ladderClimbUp = serializedObject.FindProperty("m_ClimbUp");
            ladderHopUp = serializedObject.FindProperty("m_HopUpStart");
            ladderHopRight = serializedObject.FindProperty("m_HopRightStart");
            ladderHopLeft = serializedObject.FindProperty("m_HopLeftStart");

            useMath = serializedObject.FindProperty("m_UseLaunchMath");

            customProperties.Add(ladderClimbUp.name);
            customProperties.Add(ladderHopUp.name);
            customProperties.Add(ladderHopRight.name);
            customProperties.Add(ladderHopLeft.name);
            customProperties.Add(useMath.name);
        }

        protected override void DrawUniqueProperties(GUISkin contentSkin)
        {
            EditorGUILayout.PropertyField(useMath);

            if (useMath.boolValue == false)
                EditorGUILayout.HelpBox("Disabling math calculation will make character jump with the maximum possible" +
                    " velocity set in the Jump Parameters in ClimbJump ability.", MessageType.Warning);

            EditorGUILayout.Space();

            base.DrawUniqueProperties(contentSkin);
        }

        protected override void DrawAnimation(GUISkin contentSkin)
        {
            base.DrawAnimation(contentSkin);

            EditorGUILayout.Space();

            GUILayout.Label("Animation States for Hop and Climb", contentSkin.label);

            EditorGUILayout.PropertyField(ladderClimbUp);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(ladderHopUp);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(ladderHopRight);
            EditorGUILayout.Space();
            EditorGUILayout.PropertyField(ladderHopLeft);
            EditorGUILayout.Space();

            GUILayout.Label("Root Motion Parameters", contentSkin.label);
        }
    }
}