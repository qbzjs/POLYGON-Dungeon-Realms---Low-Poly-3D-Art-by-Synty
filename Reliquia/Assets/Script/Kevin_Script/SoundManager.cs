using UnityEngine.Audio;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;


public class SoundManager : MonoBehaviour
{
    public static SoundManager instance;

    public Sound[] musique;
    public Sound[] sfx;

    void Start()
    {
        Play("Main_Theme");
    }

    void Awake()
    {
        instance = this;

        foreach(Sound x in musique)
        {
            x.source = gameObject.AddComponent<AudioSource>();
            x.source.clip = x.clip;
            x.source.volume = x.volume;
            x.source.loop = x.loop;
        }

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
        Sound x = Array.Find(musique, sound => sound.name == name);
        Sound y = Array.Find(sfx, sound => sound.name == name);
        if (x != null)
        {
            x.source.Play();
        }
        else if(y != null)
        {
            y.source.Play();
        }
    }

    public void PlaySfx(string name)
    {
        Sound y = Array.Find(sfx, sound => sound.name == name);
        if (y != null)
        {
            y.source.Play();
        }
    }

    //FindObjectOfType<SoundManager>().Play("");
}
