using AlexandreDialogues;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class InteractionChest : MonoBehaviour
{
    private SoundManager _SoundManager;
    [Header("Objet à Animer")]
    public GameObject axisObject;
    public Axis axis;
    public GameObject inGameDialogueObject = null;
    private InGameDialogue inGameDialogue;
    private Interactable _interactable;
    private List<Interactable> _interactables;
    private Quaternion _angleOriginal;
    private Quaternion _angleCible;
    [Header("Pour l'axe Y uniquement")]
    public rotationDirection direction;
    [SerializeField]
    private float _angleRotation = 90.0f;
    [SerializeField]
    private float _vitesseRotation = 0.5f;
    private bool _estOuvert = false;
    

    

    private void OnEnable()
    {
        William_Script.INTERACT_ACTIONS += OpenChest;
    }

    private void OnDisable()
    {
        William_Script.INTERACT_ACTIONS -= OpenChest;
    }

    private void Awake()
    {
        _SoundManager = GameObject.FindObjectOfType<SoundManager>();
        _interactable = GetComponent<Interactable>();
        _interactables = new List<Interactable>();
        if (inGameDialogueObject != null)
        {
            inGameDialogue = inGameDialogueObject.GetComponent<DialogueAttached>().inGameDialogue;
        }
        if (axisObject != null)
        {
            _angleCible = axisObject.transform.rotation;
            if (axis == Axis.X)
            {
                _angleOriginal = axisObject.transform.rotation * Quaternion.Euler(-_angleRotation, 0, 0);
            }
            if (axis == Axis.Y && direction == rotationDirection.Forward)
            {
                _angleOriginal = axisObject.transform.rotation * Quaternion.Euler(0, 90, 0);
            }
            if (axis == Axis.Y && direction == rotationDirection.Backward)
            {
                _angleOriginal = axisObject.transform.rotation * Quaternion.Euler(0, -90, 0);
            }
        }

    }
    // Action d'ouverture du coffre.
    private void OpenChest()
    {
        if (inGameDialogue != null)
        {
            InGameDialogueManager.Instance.StartDialogue(inGameDialogue);
            inGameDialogue = null;
        }
        if (_interactable.InteractOutline.enabled && !_estOuvert) // ouvrir, jouer l'anim (E)
        {
            _SoundManager.Play("coffre_ouvert");
            _estOuvert = true;
            StartCoroutine(VerifierContenuCoffre());
            AnimerRotation();
        }
        if (_interactable.InteractOutline.enabled && _estOuvert) // ouvrir, jouer l'anim (E)
        {
            _SoundManager.Play("coffre_fermer");
            _estOuvert = false;
            //StartCoroutine(VerifierContenuCoffre());
            AnimerRotation();
        }


    }

    // Animation d'ouverture du coffre.
    private void AnimerRotation()
    {
        if (_estOuvert && axisObject.transform.rotation != _angleOriginal)
        {
            axisObject.transform.DORotateQuaternion(_angleOriginal, _vitesseRotation);
        }

        if (!_estOuvert && axisObject.transform.rotation != _angleCible)
        {
            axisObject.transform.DORotateQuaternion(_angleCible, _vitesseRotation);
        }
    }
    // Boucle qui continue tant que le coffre a des objets Interactable.
    private IEnumerator VerifierContenuCoffre()
    {
        RecupererObjetsInteractables();

        if (_interactables.Count > 0 && _estOuvert)
        {
            _interactable.ApplyOutline(false);
            for (int i = 0; i < _interactables.Count; i++)
            {
                if (_interactables[i].gameObject.CompareTag("ItemInteractable"))
                {
                    _interactables[i].gameObject.GetComponent<Collider>().enabled = true;
                }
            }
            yield return new WaitForSeconds(0.1f);
            StartCoroutine(VerifierContenuCoffre());
        }
        else
        {
            _interactable.ApplyOutline(true);
        }
    }
    // Récupération des objets Interactable contenu dans le coffre.
    private void RecupererObjetsInteractables()
    {
        _interactables.Clear();
        foreach (Transform child in transform)
        {
            Interactable localInteractble = child.GetComponent<Interactable>();
            if (localInteractble != null)
            {
                _interactables.Add(localInteractble);
            }
        }
    }




}
