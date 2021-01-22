using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpeakerUI : MonoBehaviour
{
    public Image portrait;
    public Text fullName;
    public Text dialogue;
    
    private Character speaker;
    public Character Speaker
    {
        get { return speaker; }
        set {
            speaker = value;
            portrait.sprite = value.portrait;
            fullName.text = value.fullName;
        }
    }
    public string Dialogue
    {
        set { dialogue.text = value; }
    }

    public void Show()
    {
        gameObject.SetActive(true);
    }
    public void Hide()
    {
        gameObject.SetActive(false);
    }

    public bool IsSpeaking(Character crt_speaker) {
        return speaker == crt_speaker;
    }
}
