using AlexandreDialogues;
//using Boo.Lang;
using clavier;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class William_Script : MonoBehaviour
{

    GameManager gameManager;
    
    RaccourciClavier_Script raccourciClavier;
    Compas_Script compas_Script;
    PhysicaltemInventaire physicaltemInventaire;
    Lighting_Gabriel lightGab;
    Pulsate_Vincent_Test pulsate;
    public PlayerInventory playerInventory;
    public Inventaire_Script inventaire;

    private CharacterController characterController;

    [SerializeField] private GameObject ParentMenu;

    public static William_Script instance;

    private bool flagBrasierAlreadyOn;
    private string Msg;

    // Delegate Interaction
    public delegate void Actions();
    static public event Actions INTERACT_ACTIONS;
    static public event Actions INTERACT_ACTIONS2;
    static public event Actions UNDO_ACTIONS;

    // Dialogue Interaction
    InGameDialogueManager inGameDialogueManager;
    GameObject dialogue;
    InGameDialogue inGameDialogueBandage;
    InGameDialogue inGameDialogueClef;
    // Variable bruit de pas
    private float _derniereFoisBruitPas;
    private float _delaiBruitPas = 0.1f;
    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

    }

    private void Start()
    {
        characterController = GetComponent<CharacterController>();
        raccourciClavier = FindObjectOfType<RaccourciClavier_Script>();
        compas_Script = FindObjectOfType<Compas_Script>();
        gameManager = FindObjectOfType<GameManager>();
        lightGab = GetComponent<Lighting_Gabriel>();
        pulsate = GetComponent<Pulsate_Vincent_Test>();
        inGameDialogueManager = FindObjectOfType<InGameDialogueManager>();
        dialogue = GameObject.FindGameObjectWithTag("DialogueBandage");
        inGameDialogueBandage = dialogue.GetComponent<DialogueAttached>().inGameDialogue;
        dialogue.SetActive(false);
        
        dialogue = GameObject.FindGameObjectWithTag("DialogueClef");
        inGameDialogueClef = dialogue.GetComponent<DialogueAttached>().inGameDialogue;
        dialogue.SetActive(false);
    }

    ItemInventaire item;
    Interactable interactableObject;
    Interactable interactableItem;
    private bool isLightPowerCreated;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            physicaltemInventaire = other.gameObject.GetComponent<PhysicaltemInventaire>();
            item = physicaltemInventaire.thisItem;

            gameManager.AfficherMessageInteraction("");
        }
        if (other.CompareTag("Interactable")) //&& interactableItem == null
        {

            interactableObject = other.gameObject.GetComponent<Interactable>();
            interactableObject.ApplyOutline(true);
            

            switch (interactableObject.type)
            {
                case Interactable.eInteractableType.Brasier:
                    if (!flagBrasierAlreadyOn) // le brasier n'est pas deja allumé.
                    {
                        //gameManager.AfficherMessageInteraction("Use Light");
                    }
                    break;
                default:
                    gameManager.AfficherMessageInteraction("");
                    break;
            }

        }
    }


    private void OnTriggerExit(Collider other)
    {
        if (gameManager != null) // work arround : error on play (sandrine)
        {
            gameManager.FermerMessageInteraction();
        }
        if (other.CompareTag("Interactable"))
        {
            interactableObject = null;
            other.gameObject.GetComponent<Interactable>().ApplyOutline(false);
        }

    }

    private void Update()
    {
        ManageItemInteractable();

        if (Input.GetKeyDown(KeyCode.Escape) && GameManager.instance.menuInventaireOuvert == false && GameManager.instance.menuOptionOuvert == false)
        {
            GameManager.instance.menuPause();
        }

        if (Input.GetKeyDown(KeyCode.I) && GameManager.instance.menuPauseOuvert == false)
        {
            GameManager.instance.menuInventaire();
        }
        // Si Touche Action && (Message Interaction || Dialogue)
        if (Input.GetKeyDown(raccourciClavier.toucheClavier["Action"]) &&
            ((GameManager.instance.MessageInteraction != null && GameManager.instance.MessageInteraction.activeSelf == true)
            || inGameDialogueManager.IsDialogueStarted)
            )
        {
            if (physicaltemInventaire != null && item != null)
            {
                physicaltemInventaire.AddItem(item);
                //gameManager.FermerMessageInteraction();
                interactableItem = null;
            }
            // 1. A sup. si inutile
            //gameManager.FermerMessageInteraction();
        }

        if (Input.GetKeyDown(raccourciClavier.toucheClavier["Action"]) && interactableObject != null)
        {
            if (interactableObject.type == Interactable.eInteractableType.Brasier)
            {
                gameManager.AfficherMessageInteraction("Use Light");
                return;
            }
            // Open Door and Chest
            INTERACT_ACTIONS?.Invoke();
        }

        if (Input.GetKeyDown(raccourciClavier.toucheClavier["Pouvoir1"]) && interactableObject != null)
        {
            lightGab.SwitchLight(true);
            isLightPowerCreated = lightGab.isCreated;
            if (isLightPowerCreated)
            {
                // Light Brasier 
                INTERACT_ACTIONS?.Invoke();
            }

        }
        if (Input.GetKeyDown(/*raccourciClavier.toucheClavier["Pouvoir 2"]*/KeyCode.Alpha2) && !pulsate.isCooldown)
        {


            
            INTERACT_ACTIONS2?.Invoke();


        }

        if (Input.GetKey(/*raccourciClavier.toucheClavier["Pouvoir 1"]*/KeyCode.R))
        {

            lightGab.targetImage.gameObject.SetActive(false);
            lightGab.SwitchLight(false);
        }
            
        }

    private void ManageItemInteractable()
    {

        RaycastHit hit;
        //Vector3 rayDirection = (Vector3.forward - Vector3.up) - (transform.position + Vector3.up);
        //Vector3 rayDirection = Vector3.forward - Vector3.right;

        var angle = transform.rotation; //* startingAngle
        var rayDirection = angle * Vector3.forward;

        rayDirection.Normalize();
        //Debug.Log("RayDirection : " + rayDirection);
        float rayDistance = 2f;

        if (Physics.Raycast(transform.position, rayDirection, out hit, rayDistance))
        {

            //Debug.DrawRay(transform.position, rayDirection, Color.red);
            var target = hit.transform;
            if (target != null && target.CompareTag("ItemInteractable"))
            {
                //Debug.DrawRay(transform.position, rayDirection, Color.cyan);
                // Set Inventaire
                physicaltemInventaire = target.gameObject.GetComponent<PhysicaltemInventaire>();
                item = physicaltemInventaire.thisItem;
                
                
                // Add Contour Blanc
                interactableItem = target.gameObject.GetComponent<Interactable>();
                interactableItem.ApplyOutline(true);

                if (target.GetComponent<Interactable>().type == Interactable.eInteractableType.Bandage)
                {
                    gameManager.FermerMessageInteraction();
                    inGameDialogueManager.StartDialogue(inGameDialogueBandage);
                }
                else if (target.GetComponent<Interactable>().type == Interactable.eInteractableType.Clef)
                {
                    gameManager.FermerMessageInteraction();
                    inGameDialogueManager.StartDialogue(inGameDialogueClef);
                }
                else
                {
                    gameManager.AfficherMessageInteraction("");
                }
            }
            else
            {
                // 2. Inutile et empêche d'afficher les autres messages
                //gameManager.FermerMessageInteraction();
            }
        }
        else if (interactableItem != null)
        {
            interactableItem.ApplyOutline(false);
            gameManager.FermerMessageInteraction();
            interactableItem = null;
        }
    }


    public bool HasKey(ItemInventaire key)
    {
        return playerInventory.GetItemFromSacoche(key);
    }

    public bool UseKey(ItemInventaire key)
    {
        bool useKey = playerInventory.UseItemFromSacoche(key);
        InventaireManager.instance.RemoveItemFromSacoche(key);
        return useKey;
    }

    public void DisableInteractableObject()
    {
        interactableObject.ApplyOutline(false);
        interactableObject = null;
    }
    public void LancerBruitPas()
    {
        if (Time.time - _derniereFoisBruitPas >= _delaiBruitPas)
        {
            _derniereFoisBruitPas = Time.time;
            SoundManager.instance.JouerSfxPas();
        }
    }
}
