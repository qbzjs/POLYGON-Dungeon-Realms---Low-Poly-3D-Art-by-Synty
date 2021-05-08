using UnityEngine;

public class SoundManager_paul : MonoBehaviour
{
    public AudioClip[] playlist;
    public AudioSource audioSource;
    private int musicIndex = 0;


    // J'apppel mon Array
    void Start()
    {
        audioSource.clip = playlist[0];
        audioSource.Play();

    }

    void Update()
    {
        if (!audioSource.isPlaying){
            PlayNextSong();
        }
    }

    void PlayNextSong()
    {
        musicIndex = (musicIndex + 1) % playlist.Length;
        audioSource.clip = playlist[musicIndex];
        audioSource.Play();
    }
}
