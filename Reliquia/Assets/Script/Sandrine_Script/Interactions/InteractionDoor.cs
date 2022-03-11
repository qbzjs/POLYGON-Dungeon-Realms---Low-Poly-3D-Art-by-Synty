using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using DG.Tweening;

public class InteractionDoor : MonoBehaviour
{
    public enum TypePorte
    {
        Bois,
        Metal,
        Bar
    }

    public TypePorte typePorte;
    private SoundManager _SoundManager;
    [Header("Objet à Animer")]
    public GameObject axisObject;
    public Axis axis;
    private Interactable _interactable;
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



    private void OnEnable()
    {
        William_Script.INTERACT_ACTIONS += InteractionPorte;
    }

    private void OnDisable()
    {
        William_Script.INTERACT_ACTIONS -= InteractionPorte;
    }

    private void Awake()
    {
        _interactable = GetComponent<Interactable>();

        if (axisObject != null)
        {
            InitialiserVariables();
        }

    }

    // Pour récupérer l'angle cible et orginal.
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
    }

    // Action d'ouverture.
    private void InteractionPorte()
    {
        if (axisObject != null)
        {
            if (_interactable.InteractOutline.enabled && !_estOuvert) // ouvrir, jouer l'anim (E)
            {
                _estOuvert = true;
                AnimerRotation();
                return;
            }
            if (_interactable.InteractOutline.enabled && _estOuvert) // ouvrir, jouer l'anim (E)
            {
                _estOuvert = false;
                AnimerRotation();
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
}
