using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    public class ClimbAbilityDrawer : AbilityDrawer
    {
        // Abstract climbing properties
        SerializedProperty m_ClimbableMask;
        SerializedProperty m_ObstacleMask;

        SerializedProperty m_CastCapsuleRadius;
        SerializedProperty m_Iterations;

        SerializedProperty m_VerticalLinecastStartPoint;
        SerializedProperty m_VerticalLinecastEndPoint;

        SerializedProperty m_UpdateCastByVerticalSpeed;
        SerializedProperty m_MaxDistanceToFindLedge;


        protected SerializedProperty m_CharacterOffset;
        protected SerializedProperty m_PositioningSmoothnessTime;

        SerializedProperty gizmoColor; // The color that editor must draw on Scene
        SerializedProperty drawGizmos;

        public override void GetProperties(SerializedObject targetObject)
        {
            base.GetProperties(targetObject);
            // FORWARD CAST PROPERTIES

            m_ClimbableMask = serializedObject.FindProperty("m_ClimbableMask");
            customProperties.Add(m_ClimbableMask.name);

            m_ObstacleMask = serializedObject.FindProperty("m_ObstacleMask");
            customProperties.Add(m_ObstacleMask.name);

            m_CastCapsuleRadius = serializedObject.FindProperty("m_CastCapsuleRadius");
            customProperties.Add(m_CastCapsuleRadius.name);

            m_Iterations = serializedObject.FindProperty("m_Iterations");
            customProperties.Add(m_Iterations.name);

            m_VerticalLinecastStartPoint = serializedObject.FindProperty("m_VerticalLinecastStartPoint");
            customProperties.Add(m_VerticalLinecastStartPoint.name);

            m_VerticalLinecastEndPoint = serializedObject.FindProperty("m_VerticalLinecastEndPoint");
            customProperties.Add(m_VerticalLinecastEndPoint.name);

            m_UpdateCastByVerticalSpeed = serializedObject.FindProperty("m_UpdateCastByVerticalSpeed");
            customProperties.Add(m_UpdateCastByVerticalSpeed.name);

            m_MaxDistanceToFindLedge = serializedObject.FindProperty("m_MaxDistanceToFindLedge");
            customProperties.Add(m_MaxDistanceToFindLedge.name);

            m_CharacterOffset = serializedObject.FindProperty("m_CharacterOffsetFromLedge");
            customProperties.Add(m_CharacterOffset.name);

            m_PositioningSmoothnessTime = serializedObject.FindProperty("m_PositioningSmoothnessTime");
            customProperties.Add(m_PositioningSmoothnessTime.name);

            gizmoColor = serializedObject.FindProperty("gizmoColor");
            customProperties.Add(gizmoColor.name);

            drawGizmos = serializedObject.FindProperty("drawGizmos");
            customProperties.Add(drawGizmos.name);

        }

        protected override void DrawUniqueProperties(GUISkin contentSkin)
        {
            base.DrawUniqueProperties(contentSkin);

            EditorGUILayout.Space();

            GUILayout.Label("Casting", contentSkin.label);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_ClimbableMask);
            EditorGUILayout.PropertyField(m_ObstacleMask);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_MaxDistanceToFindLedge);
            EditorGUILayout.PropertyField(m_CastCapsuleRadius);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(m_Iterations);
            EditorGUILayout.PropertyField(m_VerticalLinecastStartPoint);
            EditorGUILayout.PropertyField(m_VerticalLinecastEndPoint);
            EditorGUILayout.PropertyField(m_UpdateCastByVerticalSpeed);

            // --------------------------- POSITIONING PARAMETERS ---------------------------------- //

            EditorGUILayout.Space();

            GUILayout.Label("Positioning", contentSkin.label);

            EditorGUILayout.Space();
            DrawPositioningProperties();
            EditorGUILayout.Space();

            GUILayout.Label("Debug", contentSkin.label);

            EditorGUILayout.Space();

            EditorGUILayout.PropertyField(drawGizmos);

            if (drawGizmos.boolValue)
                EditorGUILayout.PropertyField(gizmoColor);
        }

        protected virtual void DrawPositioningProperties()
        {
            EditorGUILayout.PropertyField(m_CharacterOffset);
            EditorGUILayout.PropertyField(m_PositioningSmoothnessTime);
        }
    }
}