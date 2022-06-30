using AlexandreDialogues;
using DiasGames.ThirdPersonSystem;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class William_Script : MonoBehaviour
{
    public static William_Script instance;
    [HideInInspector]
    public PlayerInput PlayerInput;
    [HideInInspector]
    public ThirdPersonSystem ThirdPersonSystem;
    [HideInInspector]
    public Mana Mana;
    public List<IPouvoir> ListePouvoirs = new List<IPouvoir>();
    [Header("Champ Vision Interactable")]
    public float radius;
    [Range(0, 1)]
    public float angle;
    public LayerMask targetMask;
    public LayerMask obstructionMask;
    [Header("Variables")]
    public GameObject InteractableObject;
    public GameObject ObjetPoussable;
    [Header("Inputs Bool")]
    public bool BoutonInteraction;
    public bool BoutonAccroupir;
    public bool BoutonRoulade;
    public bool BoutonSaut;
    public bool BoutonCourir;
    public bool BoutonAttaquer;
    public bool BoutonGarde;
    // Variable bruit de pas
    private float _derniereFoisBruitPas;
    private float _delaiBruitPas = 0.1f;


    public Inventory Inventory;

    private void Awake()
    {
        Inventory = GetComponent<Inventory>();
        if (Inventory == null)
            Debug.LogError("Missing Inventory Component on " + name);

        InitialiserVariables();
        ChargerBindingJoueur();
    }
    private void InitialiserVariables()
    {
        PlayerInput = GetComponent<PlayerInput>();
        ThirdPersonSystem = GetComponent<ThirdPersonSystem>();
        Mana = GetComponent<Mana>();
        ChargerListePouvoirs();
        //ListePouvoirs.AddRange(GetComponents<IPouvoir>());
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// Récupérer les binding du joueur stocké des PlayerPrefs en format string/Json.
    /// </summary>
    private void ChargerBindingJoueur()
    {
        string binds = PlayerInput.actions.ToJson();
        string rebinds = PlayerPrefs.GetString("rebinds", binds);
        PlayerInput.actions.LoadFromJson(rebinds);
    }

    /// <summary>
    /// Récupérer les pouvoirs avec un ordre précis.
    /// </summary>
    private void ChargerListePouvoirs()
    {
        ListePouvoirs.Add(GetComponent<PouvoirLighting>());
        ListePouvoirs.Add(GetComponent<PouvoirPulsate>());
        ListePouvoirs.Add(GetComponent<PouvoirAbsob>());
        ListePouvoirs.Add(GetComponent<PouvoirPraesidium>());

    }

    void OnEnable()
    {
        PlayerInput.actions.Enable();
    }

    void OnDisable()
    {
        PlayerInput.actions.Disable();
    }

    private void OnTriggerEnter(Collider other)
    {
        //Pour gérer les objets poussables.
        if (other.CompareTag("Poussable"))
        {
            if (ObjetPoussable == null)
            {
                ObjetPoussable = other.gameObject;
                ObjetPoussable.GetComponent<Outline>().enabled = true;
                GameManager.instance.AfficherMessageInteraction($"Maintenir {PlayerInput.actions["Interaction"].GetBindingDisplayString()} pour pousser.");
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        //Pour gérer les objets poussables.
        if (other.CompareTag("Poussable"))
        {
            if (ObjetPoussable != null)
            {
                ObjetPoussable.GetComponent<Outline>().enabled = false;
                GameManager.instance.FermerMessageInteraction();
            }
            ObjetPoussable = null;
        }
    }

    private void Update()
    {
        ChampVisionInteractableCheck();
        DebugTouches();

        Cursor.lockState = CursorLockMode.Confined;
    }

    /// <summary>
    /// A enlever une fois le debug terminer.
    /// </summary>
    private void DebugTouches()
    {
        if (Keyboard.current.oKey.isPressed)
        {
            GlobalEvents.ExecuteEvent("Damage", gameObject, 25.0f);
        }
        if (Keyboard.current.pKey.isPressed)
        {
            GlobalEvents.ExecuteEvent("RestoreHealth", gameObject, 25.0f);
        }
        if (Keyboard.current.lKey.isPressed)
        {
            GlobalEvents.ExecuteEvent("ManaDamage", gameObject, 25.0f);
        }
        if (Keyboard.current.semicolonKey.isPressed)
        {
            GlobalEvents.ExecuteEvent("RestoreMana", gameObject, 25.0f);
        }
    }

    /// <summary>
    /// Pour lancer les bruits de suivant avec les Animations Events.
    /// </summary>
    public void LancerBruitPas()
    {
        if (Time.time - _derniereFoisBruitPas >= _delaiBruitPas)
        {
            _derniereFoisBruitPas = Time.time;
            SoundManager.instance.JouerSfxPas();
        }
    }

    /// <summary>
    /// Détecter les objets IIinteractable dans le champs de vision.
    /// </summary>
    private void ChampVisionInteractableCheck()
    {
        // Rechercher les objets dans le rayon du joueur.
        Collider[] rangeChecks = Physics.OverlapSphere(transform.position, radius, targetMask);
        //Récupérer le plus proche.
        if (rangeChecks.Length != 0)
        {
            float plusProche = radius;
            int indexProche = 0;
            for (int i = 0; i < rangeChecks.Length; i++)
            {
                if (Vector3.Distance(rangeChecks[i].transform.position, transform.position) < plusProche)
                {
                    plusProche = Vector3.Distance(rangeChecks[i].transform.position, transform.position);
                    indexProche = i;
                }
            }
            Transform target = rangeChecks[indexProche].transform;
            Vector3 directionToTarget = (target.position - transform.position).normalized;
            // Vérifier si l'objet est dans l'angle de vision.
            if (Vector3.Dot(transform.forward, directionToTarget) > angle)
            {
                NettoyerInteractableObjet();
                InteractableObject = rangeChecks[indexProche].gameObject;
                InteractableObject.GetComponent<IInteractable>().MontrerOutline(true);
            }
            else
            {
                NettoyerInteractableObjet();
            }
        }
        else
        {
            NettoyerInteractableObjet();
        }
    }

    /// <summary>
    /// Nettoyer la variable et désactiver l'Outline de l'objet précédent.
    /// </summary>
    public void NettoyerInteractableObjet()
    {
        if (InteractableObject != null)
        {
            InteractableObject.GetComponent<IInteractable>().MontrerOutline(false);
        }

        InteractableObject = null;

        if (ObjetPoussable == null)
        {
            GameManager.instance.FermerMessageInteraction();
        }
    }
    #region UnityEvent_InputSystem
    public void OnMouvement(InputAction.CallbackContext context)
    {
        ThirdPersonSystem.InputManager.Move = context.ReadValue<Vector2>();
    }
    public void OnRegard(InputAction.CallbackContext context)
    {
        //ThirdPersonSystem.InputManager.ScrollView = context.ReadValue<Vector2>();
    }
    public void OnSaut(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            BoutonSaut = true;
        }
        else
        {
            BoutonSaut = false;
        }
    }
    public void OnAttaquer(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            BoutonAttaquer = true;
        }
        else
        {
            BoutonAttaquer = false;
        }
    }
    public void OnGarde(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() > 0)
        {
            BoutonGarde = true;
        }
        else
        {
            BoutonGarde = false;
        }
    }
    public void OnMenu(InputAction.CallbackContext context)
    {
        if (context.performed && !GameManager.instance.menuInventaireOuvert && !DialogueManager.Instance.IsDialogueStarted)
        {
            GameManager.instance.menuPause();
        }
    }
    public void OnInventaire(InputAction.CallbackContext context)
    {
        if (context.performed && !GameManager.instance.menuPauseOuvert && !DialogueManager.Instance.IsDialogueStarted)
        {
            GameManager.instance.menuInventaire();
        }
    }
    public void OnInteraction(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() > 0)
        {
            BoutonInteraction = true;
        }
        else
        {
            BoutonInteraction = false;
        }
        if (context.ReadValue<float>() > 0 && InteractableObject != null)
        {
            InteractableObject.GetComponent<IInteractable>().Interaction();
        }
    }
    public void OnPouvoir1(InputAction.CallbackContext context)
    {
        if (ListePouvoirs[0] != null)
        {
            if (context.performed)
            {
                ListePouvoirs[0].SetInputPouvoir(true);
            }
            else
            {
                ListePouvoirs[0].SetInputPouvoir(false);
            }
        }

    }
    public void OnPouvoir2(InputAction.CallbackContext context)
    {
        if (ListePouvoirs[1] != null)
        {
            if (context.performed)
            {
                ListePouvoirs[1].SetInputPouvoir(true);
            }
            else
            {
                ListePouvoirs[1].SetInputPouvoir(false);
            }
        }

    }
    public void OnPouvoir3(InputAction.CallbackContext context)
    {
        if (ListePouvoirs[2] != null)
        {
            if (context.performed)
            {
                ListePouvoirs[2].SetInputPouvoir(true);
            }
            else
            {
                ListePouvoirs[2].SetInputPouvoir(false);
            }
        }
    }
    public void OnPouvoir4(InputAction.CallbackContext context)
    {
        if (ListePouvoirs[3] != null)
        {
            if (context.performed)
            {
                ListePouvoirs[3].SetInputPouvoir(true);
            }
            else
            {
                ListePouvoirs[3].SetInputPouvoir(false);
            }
        }
    }
    public void OnCourir(InputAction.CallbackContext context)
    {
        if (context.ReadValue<float>() > 0)
        {
            BoutonCourir = true;
        }
        else
        {
            BoutonCourir = false;
        }
    }
    public void OnAccroupir(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            BoutonAccroupir = true;
        }
        else
        {
            BoutonAccroupir = false;
        }
    }
    public void OnRoulade(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            BoutonRoulade = true;
        }
        else
        {
            BoutonRoulade = false;
        }
    }
    #endregion
}
