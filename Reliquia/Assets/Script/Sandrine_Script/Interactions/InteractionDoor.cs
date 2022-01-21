using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using DG.Tweening;

public class InteractionDoor : MonoBehaviour
{
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
        William_Script.INTERACT_ACTIONS += OuvrirPorte;
    }

    private void OnDisable()
    {
        William_Script.INTERACT_ACTIONS -= OuvrirPorte;
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
            _angleCible = axisObject.transform.rotation * Quaternion.Euler(0, _angleRotation, 0);
        }
        if (axis == Axis.Y && direction == rotationDirection.Backward)
        {
            _angleCible = axisObject.transform.rotation * Quaternion.Euler(0, -_angleRotation, 0);
        }
    }

    // Action d'ouverture.
    private void OuvrirPorte()
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
            transform.DORotate(_angleCible.eulerAngles, _vitesseRotation);
        }
        if (!_estOuvert && axisObject.transform.rotation != _angleOriginal)
        {
            transform.DORotate(_angleOriginal.eulerAngles, _vitesseRotation);
        }
    }

}
