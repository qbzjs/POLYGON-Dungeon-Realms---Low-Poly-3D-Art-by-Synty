using AlexandreDialogues;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class InteractionLockDoor : MonoBehaviour,IInteractable
{
    public enum TypePorte
    {
        Bois,
        Metal,
        Bar
    }

    public TypePorte typePorte;
    [SerializeField]
    private Outline _outline = null;

    [Header("Objet à Animer")]
    public GameObject axisObject;
    public Axis axis;
    private Quaternion _angleOriginal;
    private Quaternion _angleCible;
    public rotationDirection direction;
    [SerializeField]
    private float _angleRotation = 90.0f;
    [SerializeField]
    private float _vitesseRotation = 1.0f;
    private bool _estOuvert;

    [Header("AudioSource")]
    public AudioSource audioSource;
    // Interaction Dialogue
    public GameObject dialogueVerrouilleGameObject;
    public GameObject dialogueOuvertGameObject;
    private InGameDialogue _inGameDialogueVerrouille;
    private InGameDialogue _inGameDialogueOuvert;
    public ItemInventaire item;
    private bool _estVerouille = true;

    private void Awake()
    {
        if (axisObject != null)
        {
            InitialiserVariables();
        }
        if (_outline == null)
        {
            _outline = GetComponent<Outline>();
        }
        _outline.enabled = false;
    }
    private void InitialiserVariables()
    {
        _angleOriginal = axisObject.transform.rotation;

        if (axis == Axis.Y && direction == rotationDirection.Forward)
        {
            _angleCible = Quaternion.AngleAxis(_angleRotation, Vector3.up) * axisObject.transform.rotation;
        }
        if (axis == Axis.Y && direction == rotationDirection.Backward)
        {
            _angleCible = Quaternion.AngleAxis(-_angleRotation, Vector3.up) * axisObject.transform.rotation;
        }
        InitialiserDialogue();


    }
    private void InitialiserDialogue()
    {
        if (dialogueVerrouilleGameObject != null)
        {
            _inGameDialogueVerrouille = dialogueVerrouilleGameObject.GetComponent<DialogueAttached>().inGameDialogue;
        }
        if (dialogueOuvertGameObject != null)
        {
            _inGameDialogueOuvert = dialogueOuvertGameObject.GetComponent<DialogueAttached>().inGameDialogue;
        }
    }
    // Action d'ouverture.
    private void InteractionPorte()
    {
        if (_estVerouille)
        {
            if (InventaireManager.instance.playerInventory.sacochesInventory.Contains(item))
            {
                InventaireManager.instance.playerInventory.sacochesInventory.Remove(item);
                InGameDialogueManager.Instance.StartDialogue(_inGameDialogueOuvert);
                _estVerouille = false;
            }
            else
            {
                InGameDialogueManager.Instance.StartDialogue(_inGameDialogueVerrouille);
            }
            
        }
        else
        {
            if (axisObject != null)
            {
                if (!_estOuvert) // ouvrir, jouer l'anim (E)
                {
                    _estOuvert = true;
                    AnimerRotation();
                    return;
                }
                if (_estOuvert) // ouvrir, jouer l'anim (E)
                {
                    _estOuvert = false;
                    AnimerRotation();
                }
            }
        }
        
    }

    // Animation d'ouverture.
    private void AnimerRotation()
    {
        if (_estOuvert && axisObject.transform.rotation != _angleCible)
        {
            OuvrirPorte();
        }
        if (!_estOuvert && axisObject.transform.rotation != _angleOriginal)
        {
            FermerPorte();
        }
    }
    public void OuvrirPorte()
    {
        LancerSon();
        transform.DORotateQuaternion(_angleCible, _vitesseRotation);
    }
    public void FermerPorte()
    {
        LancerSon();
        transform.DORotateQuaternion(_angleOriginal, _vitesseRotation);
    }
    private void LancerSon()
    {
        if (audioSource != null)
        {
            audioSource.Play();
        }
        else
        {
            switch (typePorte)
            {
                case TypePorte.Bois:
                    SoundManager.instance.Play("wooden_door_open");
                    break;
                case TypePorte.Metal:
                    SoundManager.instance.Play("door_metal_heavy");
                    break;
                case TypePorte.Bar:
                    SoundManager.instance.Play("door_metal_bar");
                    break;
                default:
                    SoundManager.instance.Play("wooden_door_open");
                    break;
            }
        }


    }

    public void Interaction()
    {
        InteractionPorte();
    }

    public void MontrerOutline(bool affichage)
    {
        _outline.enabled = affichage;
    }

}
