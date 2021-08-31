using AlexandreDialogues;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class DialogueAttached : MonoBehaviour
{
    public InGameDialogue inGameDialogue;
    public Dialogue dialogue;
    public GameObject virtualCamera;

    private bool alreadyRed;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && !alreadyRed)
        {
            // Lancer un dialogue Dialogue Box 
            if (dialogue != null)
            {
                DialogueManager.Instance.StartDialogueFromFile(dialogue, virtualCamera, true, true);
            }
            else
            {
                InGameDialogueManager.Instance.StartDialogue(inGameDialogue);
            }
            alreadyRed = true;
        }
        
    }
}
