using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueDisplay : MonoBehaviour
{
    public Conversation conversation;
    public GameObject speakerGlobal;
    private GameObject nextButton;
    public GameObject speakerLeft;
    public GameObject speakerRight;
    
    private SpeakerUI speakerUILeft;
    private SpeakerUI speakerUIRight;

    private int activeLineIndex = 0;

    private TextTyper textTyper = null;

    void Start()
    {
        nextButton = speakerGlobal.transform.Find("button").gameObject;
        speakerUILeft  = speakerLeft.GetComponent<SpeakerUI>();
        speakerUIRight = speakerRight.GetComponent<SpeakerUI>();

        speakerUILeft.Speaker  = conversation.speakerLeft;
        speakerUIRight.Speaker = conversation.speakerRight;

        textTyper = gameObject.GetComponent<TextTyper>();
    }

    void Update()
    {
        if (Input.GetKeyDown("space") && textTyper.IsWaiting()) {
            AdvanceConversation();
        }
        if (!nextButton.active && textTyper.IsWaiting()) {
            nextButton.SetActive(true);
        }
    }

    void AdvanceConversation() {
        if (activeLineIndex < conversation.lines.Length) {
            DisplayLine();
            activeLineIndex += 1;
        } else {
            speakerGlobal.SetActive(false);
            speakerUILeft.Hide();
            speakerUIRight.Hide();
            activeLineIndex = 0;
        }
    }

    void DisplayLine() {
        Line line = conversation.lines[activeLineIndex];
        Character speaker = line.speaker;

        if (speakerUILeft.IsSpeaking(speaker)) {
            SetDialogue(speakerUILeft, speakerUIRight, line.text);
        } else {
            SetDialogue(speakerUIRight, speakerUILeft, line.text);
        }
    }

    void SetDialogue(SpeakerUI activeSpeakerUI, SpeakerUI inactiveSpeakerUI, string text) {
        textTyper.SetTextTyper(activeSpeakerUI, text, 0.025f);
        textTyper.WriteText();
        activeSpeakerUI.Show();
        inactiveSpeakerUI.Hide();
        speakerGlobal.SetActive(true);
        nextButton.SetActive(false);
    }
}
