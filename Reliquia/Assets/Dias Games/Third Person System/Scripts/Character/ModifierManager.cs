using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class ModifierManager : MonoBehaviour
    {
        public List<Modifier> m_Modifiers;

        private ThirdPersonSystem m_System;

        private void Awake()
        {
            m_System = GetComponent<ThirdPersonSystem>();

            // Initialize each modifier attached to character
            foreach (Modifier mod in m_Modifiers)
                mod.Initialize(m_System);
        }

        private void Update()
        {
            foreach (Modifier mod in m_Modifiers)
                mod.UpdateModifier();
        }

        private void FixedUpdate()
        {
            foreach (Modifier mod in m_Modifiers)
                mod.FixedUpdateModifier();
        }

        public Modifier GetModifier<T>()
        {
            return m_Modifiers.Find(x => x is T);
        }
    }
}