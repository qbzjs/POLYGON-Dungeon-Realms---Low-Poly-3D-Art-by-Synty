using DiasGames.ThirdPersonSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Mana : Modifier
{
    [SerializeField] private float m_MaxMana = 100f;


    //[Header("Audio Clips")]
    //[SerializeField] private AudioClip m_HurtClip = null;


    [Header("Events")]
    [SerializeField] private UnityEvent OnManaDamage;

    // Action Events
    public Action OnCharacterManaDamage, OnManaChanged;

    private float m_CurrentMana;


    public float ManaValue { get { return m_CurrentMana; } }
    public float MaximumMana { get { return m_MaxMana; } }

    //private CharacterAudioManager m_AudioManager;
    private float m_LastDamagedTime = 0;

    private void Awake()
    {

        //m_AudioManager = GetComponent<CharacterAudioManager>();

        m_CurrentMana = m_MaxMana;

        GlobalEvents.AddEvent("ManaDamage", ManaDamage);
        GlobalEvents.AddEvent("RestoreMana", RestoreMana);

    }

    private void RestoreMana(GameObject obj, object value)
    {
        m_CurrentMana += (float)value;
        m_CurrentMana = Mathf.Clamp(m_CurrentMana, 0, m_MaxMana);
        OnManaChanged?.Invoke();
    }

    private void ManaDamage(GameObject obj, object amount)
    {
        if (m_LastDamagedTime > Time.fixedTime)
            return;

        if (obj != gameObject)
        {
            return;
        }

        m_CurrentMana -= (float)amount;
        if (m_CurrentMana <= 0)
        {
            m_CurrentMana = 0;
            return;
        }
        else
        {
            OnManaDamage.Invoke();
            OnCharacterManaDamage?.Invoke();
            //if (m_HurtClip != null && m_AudioManager != null)
            //    m_AudioManager.PlayVoiceSound(m_HurtClip);
        }

        m_LastDamagedTime = Time.fixedTime + 0.1f;
        OnManaChanged?.Invoke();
    }

}
