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
    public bool gainRessources;
    public float montantRestaurer = 5.0f;
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
            // Sinon lancer un InGameDialogue
            else
            {
                InGameDialogueManager.Instance.StartDialogue(inGameDialogue);
            }
            if (gainRessources)
            {
                GlobalEvents.ExecuteEvent("RestoreHealth", other.gameObject, montantRestaurer);
                GlobalEvents.ExecuteEvent("RestoreMana", other.gameObject, montantRestaurer);
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
    /// <summary>
    /// Couroutine pour forcer le joueur à être immobile.
    /// </summary>
    /// <returns></returns>
    private IEnumerator CheckDialogueFin()
    {
        yield return new WaitForSeconds(0.1f);
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
    /// <summary>
    /// Forcer l'animation courante à être en position "Idle".
    /// </summary>
    private void ForceIdleAnimation()
    {
        foreach (AnimatorControllerParameter parameter in thirdPersonSystem.m_Animator.parameters)
        {
            thirdPersonSystem.m_Animator.SetFloat(parameter.name, 0);
        }
    }
    /// <summary>
    /// Pour activer ou désactiver toutes les abilité ainsi que "ThirdPersonSystem".
    /// </summary>
    /// <param name="etat">True = peut bouger, false = immobile</param>
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
