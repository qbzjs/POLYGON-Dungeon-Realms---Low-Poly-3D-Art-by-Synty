using AlexandreDialogues;
using DiasGames.ThirdPersonSystem;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HabiltePousser : ThirdPersonAbility
{
    public GameObject[] gameObjectsInGameDialogues;
    private float _derniereFoisDialogue;
    private float _delaiDialogue = 10.0f;

    public override void Initialize(ThirdPersonSystem mainSystem, AnimatorManager animatorManager, UnityInputManager inputManager)
    {
        base.Initialize(mainSystem, animatorManager, inputManager);
    }
    public override bool TryEnterAbility()
    {
        return (m_System.IsGrounded && William_Script.instance.ObjetPoussable != null);
    }
    public override bool TryExitAbility()
    {
        bool inputToLeave = (m_UseInputStateToEnter == InputEnterType.ButtonPressing) ?
                !m_InputToEnter.IsPressed : m_InputStateSet;
        return inputToLeave | !m_System.IsGrounded | William_Script.instance.ObjetPoussable == null;
    }
    public override void FixedUpdateAbility()
    {
        m_AnimatorManager.SetForwardParameter(Input.GetAxis("Vertical"));
        if (Input.GetAxis("Vertical") > 0)
        {
            if (William_Script.instance.ObjetPoussable != null)
            {
                William_Script.instance.ObjetPoussable.transform.position += transform.forward / 2 * Time.deltaTime;
                LancerDialogue();
            }
        }
    }
    public override void OnEnterAbility()
    {
        base.OnEnterAbility();
    }
    public override void OnExitAbility()
    {
        base.OnExitAbility();
    }
    // Lancer un dialogue à partir d'un GameObject.
    private void LancerDialogue()
    {
        if (gameObjectsInGameDialogues != null && Time.time - _derniereFoisDialogue >= _delaiDialogue && !InGameDialogueManager.Instance.IsDialogueStarted)
        {
            for (int nbDialogueErreur = 0; nbDialogueErreur < gameObjectsInGameDialogues.Length; nbDialogueErreur++)
            {
                if (gameObjectsInGameDialogues[nbDialogueErreur].TryGetComponent<DialogueAttached>(out DialogueAttached dialogueAttached))
                {
                    if (dialogueAttached.inGameDialogue != null)
                    {
                        _derniereFoisDialogue = Time.time;
                        InGameDialogueManager.Instance.StartDialogue(dialogueAttached.inGameDialogue);
                        dialogueAttached.inGameDialogue = null;
                        break;
                    }
                }
            }
        }
    }
}
