using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.Events;
using System;

namespace DiasGames.ThirdPersonSystem
{
    public class Health : Modifier
    {
        [SerializeField] private float m_MaxHealth = 100f;
        [SerializeField] private bool m_RestartSceneAfterDie = true;
        [SerializeField] private float m_WaitToRestart = 3f;

        [Header("Audio Clips")]
        [SerializeField] private AudioClip m_HurtClip = null;
        [SerializeField] private AudioClip m_DeathClip = null;

        [Header("Events")]
        [SerializeField] private UnityEvent OnReceiveDamage;
        [SerializeField] private UnityEvent OnDie;

        // Action Events
        public Action OnCharacterDie, OnCharacterDamage, OnHealthChanged;

        private float m_CurrentHealth;
        private Rigidbody[] ragdollRigidbodies;
        private List<Collider> allColliders = new List<Collider>();

        public float HealthValue { get { return m_CurrentHealth; } }
        public float MaximumHealth { get { return m_MaxHealth; } }
        
        private CharacterAudioManager m_AudioManager;
        private float m_LastDamagedTime = 0;

        private void Awake()
        {
            ragdollRigidbodies = GetComponentsInChildren<Rigidbody>();
            allColliders.AddRange(GetComponentsInChildren<Collider>());

            m_AudioManager = GetComponent<CharacterAudioManager>();

            m_CurrentHealth = m_MaxHealth;

            GlobalEvents.AddEvent("Damage", Damage);
            GlobalEvents.AddEvent("Restart", RespawnCharacter);
            GlobalEvents.AddEvent("RestoreHealth", RestoreHealth);

            DisableRagdoll();
        }



        private void RestoreHealth(GameObject obj, object value)
        {
            m_CurrentHealth += (float)value;
            m_CurrentHealth = Mathf.Clamp(m_CurrentHealth, 0, m_MaxHealth); 
            OnHealthChanged?.Invoke();
        }

        public void Die()
        {
            if (m_System != null)
            {
                m_System.enabled = false;
                m_System.m_Animator.enabled = false;
                m_System.m_Capsule.sharedMaterial = null;
            }

            EnableRagdoll();
            OnDie.Invoke();
            OnCharacterDie?.Invoke();

            // Play sound
            if (m_DeathClip != null && m_AudioManager != null)
                m_AudioManager.PlayVoiceSound(m_DeathClip);

            if(m_RestartSceneAfterDie)
                StartCoroutine(RestartCharacter());
        }

        private IEnumerator RestartCharacter()
        {
            yield return new WaitForSeconds(m_WaitToRestart);

            GlobalEvents.ExecuteEvent("Restart", null, null);
        }

        private void RespawnCharacter(GameObject obj, object value)
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name);
        }

        private void Damage(GameObject obj, object amount)
        {
            if (m_LastDamagedTime > Time.fixedTime && m_RestartSceneAfterDie)
                return;

            if (obj != gameObject)
            {
                if (allColliders.Find(x => x.gameObject == obj) == null)
                    return;
            }

            m_CurrentHealth -= (float)amount;
            if (m_CurrentHealth <= 0)
            {
                m_CurrentHealth = 0;
                Die();
            }
            else
            {
                OnReceiveDamage.Invoke();
                OnCharacterDamage?.Invoke();
                if (m_HurtClip != null && m_AudioManager != null)
                    m_AudioManager.PlayVoiceSound(m_HurtClip);
            }

            m_LastDamagedTime = Time.fixedTime + 0.1f;
            OnHealthChanged?.Invoke();
        }


        private void EnableRagdoll()
        {
            if (ragdollRigidbodies.Length <= 0)
                return;

            Vector3 vel = ragdollRigidbodies[0].velocity;
            ragdollRigidbodies[0].isKinematic = true;

            for (int i = 1; i < ragdollRigidbodies.Length; i++)
            {
                ragdollRigidbodies[i].isKinematic = false;
                ragdollRigidbodies[i].useGravity = true;
                ragdollRigidbodies[i].velocity = vel;
            }
        }

        private void DisableRagdoll()
        {
            if (ragdollRigidbodies.Length <= 0)
                return;

            ragdollRigidbodies[0].isKinematic = false;

            for (int i = 1; i < ragdollRigidbodies.Length; i++)
                ragdollRigidbodies[i].isKinematic = true;
        }
    }
}