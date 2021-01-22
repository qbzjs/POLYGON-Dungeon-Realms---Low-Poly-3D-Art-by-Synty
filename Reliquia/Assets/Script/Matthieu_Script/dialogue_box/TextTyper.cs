using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TextTyper : MonoBehaviour
{
    public enum StateTextTyper {
        waiting,
        onLaunching,
        onGoing
    }

    private SpeakerUI activeSpeakerUI = null;
    private string text = "";
    private float delay = 0.025f;
    private StateTextTyper currentState = StateTextTyper.waiting;

    void Update()
    {
        if (Input.GetKeyDown("space") && IsOnGoing()) {
            CurrentState = StateTextTyper.waiting;
        }       
    }

    public void WriteText() {
        activeSpeakerUI.Dialogue = "";
        CurrentState = StateTextTyper.onLaunching;
        StartCoroutine(TypeText());
    }
    
    private IEnumerator TypeText() {
        string currentText = "";
        int i = 0;
        while(i < 5){
            currentText += text[i];
            activeSpeakerUI.Dialogue = currentText;
            yield return new WaitForSeconds(delay);
            i++;
        }
        CurrentState = StateTextTyper.onGoing;
        while(i < text.Length) {
            if (IsWaiting()) {
                activeSpeakerUI.Dialogue = text;
                break;
            } else {
                currentText += text[i];
                activeSpeakerUI.Dialogue = currentText;
                yield return new WaitForSeconds(delay);
            }
            i++;
        }
        yield return new WaitForSeconds(0.5f);
        CurrentState = StateTextTyper.waiting;
    }

    public void SetTextTyper(SpeakerUI activeSpeakerUI, string text, float delay) {
        ActiveSpeakerUI = activeSpeakerUI;
        Text = text;
        Delay = delay;
    }

    public SpeakerUI ActiveSpeakerUI
    {
        get { return activeSpeakerUI; }
        set { activeSpeakerUI = value; }
    }
    public string Text
    {
        get { return text; }
        set { text = value; }
    }
    public float Delay
    {
        get { return delay; }
        set { delay = value; }
    }

    public StateTextTyper CurrentState
    {
        get { return currentState; }
        set { currentState = value; }
    }

    public bool IsWaiting() {
        return CurrentState == StateTextTyper.waiting;
    }
    public bool IsOnGoing() {
        return CurrentState == StateTextTyper.onGoing;
    }
}
