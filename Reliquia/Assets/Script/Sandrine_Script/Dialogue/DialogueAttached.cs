using AlexandreDialogues;
using Cinemachine;
using DiasGames.ThirdPersonSystem;
using System.Collections;
using System.Collections.Generic;
using System.Security.Permissions;
using UnityEngine;

public class DialogueAttached : MonoBehaviour
{
    public InGameDialogue inGameDialogue;
    public Dialogue dialogue;
    public GameObject virtualCamera;
    public CinemachineStateDrivenCamera cinemachineStateDriven;
    private ThirdPersonSystem thirdPersonSystem;

    private bool alreadyRed;
    public bool showOnSecondThrow;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player") && showOnSecondThrow)
        {
            return;
        }
        if (other.CompareTag("Player") && !alreadyRed)
        {
            // Lancer un dialogue Dialogue Box 
            if (dialogue != null)
            {
                if (other.TryGetComponent<ThirdPersonSystem>(out thirdPersonSystem))
                {
                    ForceIdleAnimation();
                    JoueurPeutBouger(false);
                    StartCoroutine(CheckDialogueFin());
                }

                DialogueManager.Instance.StartDialogueFromFile(dialogue, virtualCamera, true, true);
            }
            else
            {
                InGameDialogueManager.Instance.StartDialogue(inGameDialogue);
            }
            alreadyRed = true;
        }
        
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player") && showOnSecondThrow)
        {
            showOnSecondThrow = false;
        }
    }
    private IEnumerator CheckDialogueFin()
    {
        yield return new WaitForSeconds(0.1f);
        Debug.Log("Couroutine Runnig");
        if (!DialogueManager.Instance.IsDialogueStarted && alreadyRed)
        {
            if (thirdPersonSystem != null)
            {
                JoueurPeutBouger(true);
            }
        }
        else
        {
            thirdPersonSystem.m_Rigidbody.velocity = Vector3.zero;
            StartCoroutine(CheckDialogueFin());
        }
    }
    private void ForceIdleAnimation()
    {
        foreach (AnimatorControllerParameter parameter in thirdPersonSystem.m_Animator.parameters)
        {
            thirdPersonSystem.m_Animator.SetFloat(parameter.name, 0);
        }
    }
    private void JoueurPeutBouger(bool etat)
    {

        cinemachineStateDriven.enabled = etat;
        foreach (ThirdPersonAbility ability in thirdPersonSystem.CharacterAbilities)
        {
            ability.enabled = etat;
        }

        thirdPersonSystem.enabled = etat;
    }
}
