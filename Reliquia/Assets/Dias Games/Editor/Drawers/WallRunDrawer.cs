using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace DiasGames.ThirdPersonSystem
{
    public class WallRunDrawer : AbilityDrawer
    {
        SerializedProperty wallRunRight;

        public override void GetProperties(SerializedObject targetObject)
        {
            base.GetProperties(targetObject);
        
        wallRunRight = serializedObject.FindProperty("m_WallRunRightState");
            customProperties.Add(wallRunRight.name);

            m_EnterStateLabel = "Wall Run Left State";
        }

        protected override void DrawAnimation(GUISkin contentSkin)
        {
            EditorGUILayout.PropertyField(wallRunRight);
            base.DrawAnimation(contentSkin);
        }
    }
}