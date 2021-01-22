using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class CrawlDrawer : AbilityDrawer
    {
        SerializedProperty crawling, finishCrawl;

        public override void GetProperties(SerializedObject targetObject)
        {
            base.GetProperties(targetObject);
            
            crawling = serializedObject.FindProperty("m_CrawlingState");
            finishCrawl = serializedObject.FindProperty("m_FinishCrawl");

            m_EnterStateLabel = "Start Crawl";

            customProperties.Add(crawling.name);
            customProperties.Add(finishCrawl.name);
        }

        protected override void DrawAnimation(GUISkin contentSkin)
        {
            drawEnterState = false;

            EditorGUILayout.PropertyField(m_EnterState, new GUIContent(m_EnterStateLabel));
            EditorGUILayout.PropertyField(crawling);
            EditorGUILayout.PropertyField(finishCrawl);
            EditorGUILayout.Space();

            base.DrawAnimation(contentSkin);
        }
    }
}