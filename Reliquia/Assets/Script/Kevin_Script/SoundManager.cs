using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;
    //public Sound[] musique;
    public Sound[] sfx;

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

}
