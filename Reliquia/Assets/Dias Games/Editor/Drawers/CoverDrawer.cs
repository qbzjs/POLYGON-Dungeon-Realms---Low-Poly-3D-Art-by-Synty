using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    public class CoverDrawer : AbilityDrawer
    {
        SerializedProperty m_Turn;

        public override void GetProperties(SerializedObject targetObject)
        {
            base.GetProperties(targetObject);

            m_Turn = targetObject.FindProperty("m_TurnState");
            customProperties.Add(m_Turn.name);

            m_EnterStateLabel = "Movement State";
                       



      }

        protected override void DrawAnimation(GUISkin contentSkin)
        {
            EditorGUILayout.PropertyField(m_Turn);
            base.DrawAnimation(contentSkin);
        }
    }
}