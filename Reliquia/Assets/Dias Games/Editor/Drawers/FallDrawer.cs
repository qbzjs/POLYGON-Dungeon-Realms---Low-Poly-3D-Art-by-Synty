using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    public class FallDrawer : AbilityDrawer
    {
        SerializedProperty hardLandState;

        public override void GetProperties(SerializedObject targetObject)
        {
            base.GetProperties(targetObject);

            hardLandState = serializedObject.FindProperty("m_HardLandState");
            customProperties.Add(hardLandState.name);

            m_EnterStateLabel = "Fall Loop State";
        }

        protected override void DrawAnimation(GUISkin contentSkin)
        {
            drawEnterState = false;

            EditorGUILayout.PropertyField(m_EnterState, new GUIContent(m_EnterStateLabel));
            EditorGUILayout.PropertyField(hardLandState);
            EditorGUILayout.Space();

            base.DrawAnimation(contentSkin);
        }
    }
}