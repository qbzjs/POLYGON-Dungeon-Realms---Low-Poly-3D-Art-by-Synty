using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace DiasGames.ThirdPersonSystem.UI
{
    public class HealthBarUIManager : MonoBehaviour
    {
        [SerializeField] private Health m_Character = null;
        [SerializeField] private Image m_HealhtBarImage = null;

        private void Awake()
        {
            if (m_Character == null)
                m_Character = GameObject.FindGameObjectWithTag("Player").GetComponent<Health>();
        }

        private void OnEnable()
        {
            m_Character.OnHealthChanged += UpdateHealthBar;
        }
        private void OnDisable()
        {
            m_Character.OnHealthChanged -= UpdateHealthBar;
        }

        private void UpdateHealthBar()
        {
            m_HealhtBarImage.fillAmount = m_Character.HealthValue / m_Character.MaximumHealth;
        }
    }
}