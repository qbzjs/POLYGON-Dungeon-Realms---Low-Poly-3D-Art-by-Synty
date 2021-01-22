using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DiasGames.ThirdPersonSystem
{
    public class CharacterAudioManager : MonoBehaviour
    {
        [SerializeField] private AudioSource m_VoiceSource;
        [SerializeField] private AudioSource m_EffectSource;

        [SerializeField] private float m_DelayBetweenVoices = 0.3f;

        private float lastTimeVoicePlayed = 0;

        private float initializedTime = 0;
        private void Start()
        {
            initializedTime = 1;// Time.fixedTime;
        }

        /// <summary>
        /// Play a sound with specific audio source
        /// </summary>
        /// <param name="clip"></param>
        /// <param name="soundType"></param>
        public void PlaySoundClip(AudioClip clip, AudioSource reference)
        {
            if (Time.fixedTime - initializedTime <= 1 || initializedTime == 0)
                return;
            
            reference.pitch = Random.Range(0.9f, 1.1f);
            reference.volume = Random.Range(0.9f, 1.1f);
            reference.clip = clip;
            reference.Play();

        }

        /// <summary>
        /// Plays a Sound Effect
        /// </summary>
        /// <param name="clip"></param>
        public void PlayEffectSound(AudioClip clip)
        {
            PlaySoundClip(clip, m_EffectSource);
        }

        /// <summary>
        /// Plays a voice
        /// </summary>
        /// <param name="clip"></param>
        public void PlayVoiceSound(AudioClip clip)
        {
            if (Time.fixedTime - lastTimeVoicePlayed <= m_DelayBetweenVoices)
                return;

            lastTimeVoicePlayed = Time.fixedTime;
            PlaySoundClip(clip, m_VoiceSource);
        }

    }
}