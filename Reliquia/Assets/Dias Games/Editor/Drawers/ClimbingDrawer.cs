using UnityEngine;
using UnityEditor;


namespace DiasGames.ThirdPersonSystem
{
    public class ClimbingDrawer : ClimbAbilityDrawer
    {
        SerializedProperty m_CharacterOffsetOnHang;

        public override void GetProperties(SerializedObject targetObject)
        {
            base.GetProperties(targetObject);

            drawFinish = false;

            m_CharacterOffsetOnHang = serializedObject.FindProperty("m_CharacterOffsetOnHang");
            customProperties.Add(m_CharacterOffsetOnHang.name);

            // SIDE CAST PROPERTTIES
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name.Contains("State") || property.name.Contains("Side"))
                        customProperties.Add(property.name);

                } while (property.NextVisible(false));
            }

        }

        protected override void DrawAnimation(GUISkin contentSkin)
        {
            //base.DrawAnimation(contentSkin);
        
            EditorGUILayout.PropertyField(m_TransitionDuration);
            EditorGUILayout.Space();

            GUILayout.Label("States To Grab Ledge", contentSkin.label);
            EditorGUILayout.PropertyField(m_EnterState, new GUIContent("Brace Grab Down"));
            DrawStatesByType("Grab");

            EditorGUILayout.Space();

            GUILayout.Label("States For Idle", contentSkin.label);
            DrawStatesByType("Idle");

            EditorGUILayout.Space();

            GUILayout.Label("States For Up Actions", contentSkin.label);
            DrawStatesByType("Up");

            EditorGUILayout.Space();

            GUILayout.Label("States For Down Actions", contentSkin.label);
            DrawStatesByType("Down");
            EditorGUILayout.Space();
            DrawStatesByType("Back");

            EditorGUILayout.Space();

            GUILayout.Label("States For Right and Left Actions", contentSkin.label);
            EditorGUILayout.HelpBox("All states names bellow combine with sub state name. System will find a state in: SubStateName (can be Right or Left), and the desired state name: RightSubState.Hop Start", MessageType.Info);

            DrawStatesByType("Right");
            DrawStatesByType("Left");
            EditorGUI.indentLevel++;

            DrawStatesByType("Shimmy");
            EditorGUILayout.Space();
            DrawStatesByType("Corner");
            EditorGUILayout.Space();
            DrawStatesByType("LookSide");
            DrawStatesByType("HopSide");
            EditorGUILayout.Space();

            EditorGUI.indentLevel--;

            EditorGUILayout.Space();

            GUILayout.Label("Root Motion Parameters", contentSkin.label);
        }

        private void DrawStatesByType(string compare)
        {
            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name.Contains("State") && property.name.Contains(compare))
                        EditorGUILayout.PropertyField(property);

                } while (property.NextVisible(false));
            }
        }

        protected override void DrawUniqueProperties(GUISkin contentSkin)
        {
            base.DrawUniqueProperties(contentSkin);

            EditorGUILayout.Space();

            GUILayout.Label("Side Casting", contentSkin.label);

            EditorGUILayout.Space();

            SerializedProperty property = serializedObject.GetIterator();
            if (property.NextVisible(true))
            {
                do
                {
                    if (property.name.Contains("Side") && !property.name.Contains("State"))
                        EditorGUILayout.PropertyField(property);

                } while (property.NextVisible(false));
            }
        }

        protected override void DrawPositioningProperties()
        {
            EditorGUILayout.PropertyField(m_CharacterOffset);
            EditorGUILayout.PropertyField(m_CharacterOffsetOnHang);
            EditorGUILayout.PropertyField(m_PositioningSmoothnessTime);
        }
    }
}