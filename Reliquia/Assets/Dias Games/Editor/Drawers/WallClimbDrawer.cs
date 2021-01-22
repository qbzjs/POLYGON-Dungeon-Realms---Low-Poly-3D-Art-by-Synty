using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    public class WallClimbDrawer : AbilityDrawer
    {
        SerializedProperty climbUp, normalizedTime;

        public override void GetProperties(SerializedObject targetObject)
        {
            base.GetProperties(targetObject);
            climbUp = serializedObject.FindProperty("m_ClimbUpState");
            normalizedTime = serializedObject.FindProperty("m_NormalizedTimeToStop");

            customProperties.Add(climbUp.name);
            customProperties.Add(normalizedTime.name);

            m_EnterStateLabel = "Wall Climb State";
        }

        protected override void DrawAnimation(GUISkin contentSkin)
        {
            drawEnterState = false;

            EditorGUILayout.PropertyField(m_EnterState, new GUIContent(m_EnterStateLabel));
            EditorGUILayout.PropertyField(normalizedTime);
            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(climbUp);
            EditorGUILayout.Space();

            base.DrawAnimation(contentSkin);
        }
    }
}