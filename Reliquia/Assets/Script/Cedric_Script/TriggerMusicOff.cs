using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerMusicOff : MonoBehaviour
{
    public AudioSource _AudioSource;
    void OnTriggerEnter(Collider other)
    {
        if(_AudioSource.isPlaying && other.name == "triggerPlayerZone")
        {
            StartCoroutine (AudioFadeOut.FadeOut (_AudioSource, 0.9f));
        }
    }
}
