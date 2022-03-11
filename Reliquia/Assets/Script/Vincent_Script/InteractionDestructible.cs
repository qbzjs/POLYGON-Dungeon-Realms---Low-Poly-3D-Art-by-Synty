using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;

public class InteractionDestructible : MonoBehaviour
{
    #region Variables globales
    private GameManager _gameManager;

    [SerializeField, Header("Objets")]
    private GameObject[] _gameObjects;

    [SerializeField, Header("Particules Prefabs")]
    private ParticleSystem[] _particles;

    private Interactable _interactableScript;
    private Lighting_Vincent_Test _lightingScript;
    private GameObject _objetActif;

    private bool _estDetruit = false;
    private bool _enTransition = false;
    private int _indexEtat = 0;

    [SerializeField, Header("Duree Transition")]
    private float _dureeTransition = 2;
    #endregion



    //Ajout des actions aux boutons actions.
    private void OnEnable()
    {
        William_Script.INTERACT_ACTIONS += DetruireObjetLighting;
        William_Script.INTERACT_ACTIONS2 += DetruireObjetPulsate;
    }

    private void OnDisable()
    {
        William_Script.INTERACT_ACTIONS -= DetruireObjetLighting;
        William_Script.INTERACT_ACTIONS2 -= DetruireObjetPulsate;
    }
    private void Awake()
    {
        InitialiserVariables();
    }
    // Initialisation des variables nécessaire.
    private void InitialiserVariables()
    {
        _gameManager = FindObjectOfType<GameManager>();
        _lightingScript = GameObject.FindGameObjectWithTag("Player").GetComponent<Lighting_Vincent_Test>();
        _interactableScript = GetComponent<Interactable>();
    }
    // Action avec le pouvoir Lighting.
    private void DetruireObjetLighting()
    {
        if (_lightingScript.isCreated && !_enTransition && !_estDetruit)
        {
            StartCoroutine(DetruireObjet(1));
        }

    }
    // Action avec le pouvoir Pulsate.
    private void DetruireObjetPulsate()
    {
        if (!_enTransition && !_estDetruit)
        {
            StartCoroutine(DetruireObjet(2));
        }

    }
    /// <summary>
    /// Couroutine pour la destruction des objets.
    /// </summary>
    /// <param name="index">La puissance du pouvoir pour la destruction</param>
    /// <returns></returns>
    private IEnumerator DetruireObjet(int index)
    {
        _enTransition = true;
        LancerSon();
        InstancierParticules();
        StartCoroutine(CacherObjetAcfif(index));
        yield return new WaitForSeconds(_dureeTransition);
        _enTransition = false;
        AfficherOutline();
    }
    //Instancier les particules et jouer l'animation.
    private void InstancierParticules()
    {
        GameObject joueur = GameObject.FindGameObjectWithTag("Player");
        foreach (ParticleSystem particle in _particles)
        {
                // Instancier les particules un peu devant et en haut du joueur.
                var particleInst = Instantiate(particle, joueur.transform.position + joueur.transform.forward + joueur.transform.up, joueur.transform.rotation);
                Destroy(particleInst, _dureeTransition);
                particleInst.Play();
        }
    }
    //Destruction de l'objet précédent ou cacher le 1er.
    private IEnumerator CacherObjetAcfif(int index)
    {
        // Attendre que l'animation des particules est commencé avant de cacher l'objet.
        yield return new WaitForSeconds(0.2f);
        if (_indexEtat == 0)
        {
            gameObject.GetComponent<MeshRenderer>().enabled = false;
        }
        else
        {
            Destroy(_objetActif);
        }
        _indexEtat += index;
        InstancierObjet();
    }
    // Instanciation de l'objet.
    private void InstancierObjet()
    {
        if (_indexEtat > _gameObjects.Length - 1)
        {
            // Dernier objet instancier et retirer l'interaction.
                _objetActif = _gameObjects[_gameObjects.Length - 1];
                _objetActif.SetActive(true);

            foreach (Collider collider in gameObject.GetComponents<Collider>())
            {
                collider.enabled = false;
            }
            GameManager.instance.FermerMessageInteraction();
            _estDetruit = true;
        }
        else
        {
                _objetActif = _gameObjects[_indexEtat - 1];
                _objetActif.SetActive(true);
        }
    }
    // Afficher l'outline tant que l'objet n'est pas completement été detruit.
    private void AfficherOutline()
    {
        if (!_estDetruit)
        {
            _objetActif.AddComponent<Outline>();
            _interactableScript.goContour = _objetActif;
            _interactableScript.ApplyOutline(true);
        }
    }
    private void LancerSon()
    {
        SoundManager.instance.Play("explosion_barrel");
    }
}
