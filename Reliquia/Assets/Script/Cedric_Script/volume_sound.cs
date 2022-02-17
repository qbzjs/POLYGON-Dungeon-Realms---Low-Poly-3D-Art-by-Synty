using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class volume_sound : MonoBehaviour
{

    public void ChangeVol(float newValue) {
        float newVol = AudioListener.volume;
        newVol = newValue;
        AudioListener.volume = newVol;
    }
}
