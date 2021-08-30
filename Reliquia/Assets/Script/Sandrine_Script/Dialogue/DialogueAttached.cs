using AlexandreDialogues;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DialogueAttached : MonoBehaviour
{
    public InGameDialogue inGameDialogue;

    private void OnTriggerEnter(Collider other)
    {
        InGameDialogueManager.Instance.StartDialogue(inGameDialogue);
    }
}
