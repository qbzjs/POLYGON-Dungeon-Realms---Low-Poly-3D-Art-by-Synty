using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMusicOn : MonoBehaviour
{
    public AudioSource _AudioSource;
    void OnTriggerEnter(Collider other)
    {
        if(!_AudioSource.isPlaying && other.name == "triggerPlayerZone")
        {
            _AudioSource.Play();
        }
    }
}
