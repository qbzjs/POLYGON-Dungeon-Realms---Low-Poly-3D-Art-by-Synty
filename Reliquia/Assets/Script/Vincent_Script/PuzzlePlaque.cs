using AlexandreDialogues;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PuzzlePlaque : MonoBehaviour
{
    public GameObject objetCible;
    public bool resolu = false;
    public PuzzlePlaque[] plaqueLies;
    [Header("Event")]
    public int EventNumber;
    public UnityEvent[] unityEvent;
    [Header("InGameDialogues")]
    public GameObject gameObjectReussiInDialogue;
    public GameObject gameObjectResolutionInDialogue;
    public GameObject[] gameObjectsErreurInDialogues;
    

    private void OnTriggerEnter(Collider other)
    {
        // Si l'objet est correct.
        if (other.gameObject == objetCible)
        {
            resolu = true;

            if (gameObjectReussiInDialogue != null)
            {
                JouerEtNettoyerInGameDialogue(gameObjectReussiInDialogue);
            }
            // Si le puzzle est résolu.
            if (plaqueLies.Length == Array.FindAll(plaqueLies, x => x.resolu == true).Length)
            {
                if (gameObjectResolutionInDialogue != null)
                {
                    JouerEtNettoyerInGameDialogue(gameObjectResolutionInDialogue);
                }
                unityEvent[EventNumber].Invoke();
                unityEvent[EventNumber].RemoveAllListeners();
            }
        }
        else
        {
            if (gameObjectsErreurInDialogues != null && other.gameObject.CompareTag("PuzzleTrigger"))
            {
                for (int nbDialogueErreur = 0; nbDialogueErreur < gameObjectsErreurInDialogues.Length; nbDialogueErreur++)
                {
                    if (gameObjectsErreurInDialogues[nbDialogueErreur].TryGetComponent<DialogueAttached>(out DialogueAttached dialogueAttached))
                    {
                        if (dialogueAttached.inGameDialogue != null)
                        {
                            InGameDialogueManager.Instance.StartDialogue(dialogueAttached.inGameDialogue);
                            dialogueAttached.inGameDialogue = null;
                            break;
                        }
                    }
                }
            }
        }
        
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject == objetCible)
        {
            resolu = false;
        }
    }
    // Recuperer InGameDialogue d'un GameObject et le vider.
    private void JouerEtNettoyerInGameDialogue(GameObject gameObject)
    {
        if (gameObject.TryGetComponent<DialogueAttached>(out DialogueAttached dialogueAttached))
        {
            if (dialogueAttached.inGameDialogue != null)
            {
                InGameDialogueManager.Instance.StartDialogue(dialogueAttached.inGameDialogue);
                dialogueAttached.inGameDialogue = null;
            }
        }
    }
}