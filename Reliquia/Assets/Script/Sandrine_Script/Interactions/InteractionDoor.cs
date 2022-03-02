using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using DG.Tweening;

public class InteractionDoor : MonoBehaviour
{
    private SoundManager _SoundManager;
    [Header("Objet à Animer")]
    public GameObject axisObject;
    public Axis axis;
    private Interactable _interactable;
    private Quaternion _angleOriginal;
    private Quaternion _angleCible;
    [Header("Pour l'axe Y uniquement")]
    public rotationDirection direction;
    [SerializeField]
    private float _angleRotation = 90.0f;
    [SerializeField]
    private float _vitesseRotation = 1.0f;
    private bool _estOuvert;

    

    
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
        _SoundManager = GameObject.FindObjectOfType<SoundManager>();
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
                _SoundManager.Play("wooden_door_open");
                _estOuvert = true;
                AnimerRotation();
                return;
            }
            if (_interactable.InteractOutline.enabled && _estOuvert) // ouvrir, jouer l'anim (E)
            {
                _SoundManager.Play("wooden_door_open");
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
        transform.DORotateQuaternion(_angleCible, _vitesseRotation);
    }
    public void FermerPorte()
    {
        transform.DORotateQuaternion(_angleOriginal, _vitesseRotation);
    }

}
