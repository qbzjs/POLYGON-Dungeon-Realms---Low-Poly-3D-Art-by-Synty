using System.Collections;
using System.Collections.Generic;
//using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class InteractionDestructible : MonoBehaviour, IInteractable
{
    #region Variables globales
    private Outline _outline;
    [SerializeField, Header("Objets")]
    private GameObject[] _gameObjects;

    [SerializeField, Header("Particules Prefabs")]
    private ParticleSystem[] _particles;

    private GameObject _objetActif;

    private bool _estDetruit = false;
    private bool _enTransition = false;
    private int _indexEtat = 0;

    [SerializeField, Header("Duree Transition")]
    private float _dureeTransition = 2;
    #endregion

    private void Awake()
    {
        InitialiserVariables();
    }

    // Initialisation des variables nécessaire.
    private void InitialiserVariables()
    {
        _outline = GetComponent<Outline>();
        _outline.enabled = false;
    }

    // Action avec le pouvoir Lighting.
    public void DetruireObjetLighting()
    {
        if (!_enTransition && !_estDetruit)
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
        AjouterOutline();
    }

    //Instancier les particules et jouer l'animation.
    private void InstancierParticules()
    {
        GameObject joueur = GameObject.FindGameObjectWithTag("Player");
        Vector3 directionJoueur = joueur.transform.position - transform.position;
        foreach (ParticleSystem particle in _particles)
        {
            // Instancier les particules un peu devant et en haut du joueur.
            var particleInst = Instantiate(particle,transform.position + directionJoueur.normalized + Vector3.up, Quaternion.identity);
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
            _objetActif.SetActive(false);
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
    private void AjouterOutline()
    {
        if (!_estDetruit)
        {
            _objetActif.AddComponent<Outline>().enabled = false;
        }
    }

    private void LancerSon()
    {
        SoundManager.instance.Play("explosion_barrel");
    }

    public void Interaction()
    {
        if (William_Script.instance.ThirdPersonSystem.ActiveAbility is PouvoirPulsate)
        {
            DetruireObjetPulsate();
        }
    }

    public void MontrerOutline(bool affichage)
    {
        if (!_enTransition)
        {
            if (_indexEtat == 0)
            {
                _outline.enabled = affichage;
            }
            else
            {
                _objetActif.GetComponent<Outline>().enabled = affichage;
            }

            AfficherMessageInteraction(affichage);
        }
    }

    //Afficher le message d'interaction.
    private void AfficherMessageInteraction(bool affichage)
    {
        if (affichage)
        {
            GameManager.instance.AfficherMessageInteraction("Utiliser Lighting ou Pulsate pour détruire.");
        }
        else
        {
            GameManager.instance.FermerMessageInteraction();
        }
            
    }
}
