using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;
using DiasGames.ThirdPersonSystem;

public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    //public Sound[] musique;
    public Sound[] sfx;
    public SfxPas[] sfxPasArray;
    private ThirdPersonSystem _thirdPersonSystem = null;
    void Awake()
    {
        instance = this;

        /*
        foreach(Sound x in musique)
        {
            x.source = gameObject.AddComponent<AudioSource>();
            x.source.clip = x.clip;
            x.source.volume = x.volume;
            x.source.loop = x.loop;
        }
        */

        foreach (Sound y in sfx)
        {
            y.source = gameObject.AddComponent<AudioSource>();
            y.source.clip = y.clip;
            y.source.volume = y.volume;
            y.source.loop = y.loop;
        }
        // Première boucle pour le type de sol.
        for (int sfxPasLoop = 0; sfxPasLoop < sfxPasArray.Length; sfxPasLoop++)
        {
            // Deuxième boucle pour les différents sons.
            for (int soundLoop = 0; soundLoop < sfxPasArray[sfxPasLoop].sounds.Length; soundLoop++)
            {
                sfxPasArray[sfxPasLoop].sounds[soundLoop].source = gameObject.AddComponent<AudioSource>();
                sfxPasArray[sfxPasLoop].sounds[soundLoop].source.clip = sfxPasArray[sfxPasLoop].sounds[soundLoop].clip;
                sfxPasArray[sfxPasLoop].sounds[soundLoop].source.volume = sfxPasArray[sfxPasLoop].sounds[soundLoop].volume;
                sfxPasArray[sfxPasLoop].sounds[soundLoop].source.loop = sfxPasArray[sfxPasLoop].sounds[soundLoop].loop;
            }
        }
    }

    public void Play(string name)
    {
        //Sound x = Array.Find(musique, sound => sound.name == name);
        Sound y = Array.Find(sfx, sound => sound.name == name);
        /*
        if (x != null)
        {
            x.source.Play();
        }
        */
        if(y != null)
        {
            y.source.Play();
        }
    }
/*
    public void PlaySfx(string name)
    {
        Sound y = Array.Find(sfx, sound => sound.name == name);
        if (y != null)
        {
            y.source.Play();
        }
    }

*/

    public void Stop(string name)
    {
        //Sound x = Array.Find(musique, sound => sound.name == name);
        Sound y = Array.Find(sfx, sound => sound.name == name);
        /*
        if (x != null)
        {
            x.source.Stop();
        }
        */
        if(y != null)
        {
            y.source.Stop();
        }
    }
    public void Pause(string name)
    {
        //Sound x = Array.Find(musique, sound => sound.name == name);
        Sound y = Array.Find(sfx, sound => sound.name == name);
        /*
        if (x != null)
        {
            x.source.Pause();
        }
        */
        if(y != null)
        {
            y.source.Pause();
        }
    }
    public void UnPause(string name)
    {
        //Sound x = Array.Find(musique, sound => sound.name == name);
        Sound y = Array.Find(sfx, sound => sound.name == name);
        /*
        if (x != null)
        {
            x.source.UnPause();
        }
        */
        if(y != null)
        {
            y.source.UnPause();
        }
    }
    // Lancer le bruit de pas suivant le type de sol grâce au tag.
    public void JouerSfxPas()
    {
        // Afin de récupérer le RaycastHit générer par ThirdPersonSystem.
        if (_thirdPersonSystem == null)
        {
            _thirdPersonSystem = GameObject.FindGameObjectWithTag("Player").GetComponent<ThirdPersonSystem>();
        }
        // Jouer un son aléatoire suivant le type de sol.
        if (_thirdPersonSystem.GroundHitInfo.collider.tag == "Sand")
        {
            SfxPas x = Array.Find(sfxPasArray, SoName => SoName.name == "pas_terre_sable");
            int rand = UnityEngine.Random.Range(0, x.sounds.Length);
            x.sounds[rand].source.Play();
        }
        if (_thirdPersonSystem.GroundHitInfo.collider.tag == "Stone")
        {
            SfxPas x = Array.Find(sfxPasArray, SoName => SoName.name == "pas_eglise");
            int rand = UnityEngine.Random.Range(0, x.sounds.Length);
            x.sounds[rand].source.Play();
        }
        //Debug.Log(_thirdPersonSystem.GroundHitInfo.collider.tag);
    }

}

[CreateAssetMenu(fileName = "pas_", menuName = "ScriptableObjects/SfxPas")]
public class SfxPas : ScriptableObject
{

    public Sound[] sounds;

}
