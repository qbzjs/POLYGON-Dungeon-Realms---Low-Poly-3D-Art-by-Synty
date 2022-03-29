using AlexandreDialogues;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;

public class InteractionChest : MonoBehaviour,IInteractable
{
    public Outline _outline = null;
    [Header("Objet à Animer")]
    public GameObject axisObject;
    public Axis axis;
    private BoxCollider _boxCollider;
    private InGameDialogue inGameDialogue;
    private List<GameObject> _interactables;
    private Quaternion _angleOriginal;
    private Quaternion _angleCible;
    public rotationDirection direction;
    [SerializeField]
    private float _angleRotation = 90.0f;
    [SerializeField]
    private float _vitesseRotation = 0.5f;
    [Header("InGame Dialogue GO")]
    public GameObject inGameDialogueObject = null;
    
    private bool _estOuvert = false;
    

    private void Awake()
    {
        _boxCollider = GetComponent<BoxCollider>();
        _interactables = new List<GameObject>();
        if (_outline == null)
        {
            _outline = GetComponent<Outline>();
        }
        _outline.enabled = false;
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
        if (!_estOuvert) // ouvrir, jouer l'anim (E)
        {
            SoundManager.instance.Play("coffre_ouvert");
            _estOuvert = true;
            StartCoroutine(VerifierContenuCoffre());
            AnimerRotation();
        }
        else // ouvrir, jouer l'anim (E)
        {
            SoundManager.instance.Play("coffre_fermer");
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
            _boxCollider.enabled = false;
            for (int i = 0; i < _interactables.Count; i++)
            {
                _interactables[i].GetComponent<BoxCollider>().enabled = true;

            }
        }

        while (_interactables.Count > 0 && _estOuvert)
        {
            Debug.Log(_boxCollider);
            RecupererObjetsInteractables();
            yield return new WaitForSeconds(0.1f);
        }
        _boxCollider.enabled = true;
    }
    // Récupération des objets Interactable contenu dans le coffre.
    private void RecupererObjetsInteractables()
    {
        _interactables.Clear();
        foreach (Transform child in transform)
        {
            IInteractable localInteractble = child.GetComponent<IInteractable>();
            if (localInteractble != null)
            {
                _interactables.Add(child.gameObject);
            }
        }
    }

    public void Interaction()
    {
        OpenChest();
    }

    public void MontrerOutline(bool affichage)
    {
        _outline.enabled = affichage;
    }
}
