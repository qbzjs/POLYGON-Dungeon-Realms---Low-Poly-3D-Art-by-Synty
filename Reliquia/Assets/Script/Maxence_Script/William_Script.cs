using clavier;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class William_Script : MonoBehaviour
{

    GameManager gameManager;
    RaccourciClavier_Script raccourciClavier;
    Compas_Script compas_Script;
    PhysicaltemInventaire physicaltemInventaire;

    public Inventaire_Script inventaire;

    private CharacterController characterController;

    [SerializeField] private GameObject ParentMenu;

    public static William_Script instance;
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
    }

    ItemInventaire item;
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Item"))
        {
            physicaltemInventaire = other.gameObject.GetComponent<PhysicaltemInventaire>();
            item = physicaltemInventaire.thisItem;
            gameManager.AfficherMessageInteraction("");
        }
        if (other.CompareTag("Interactable"))
        {
            Debug.Log("Interact");
            // PostProcessing solution
            // other.gameObject.layer = 27;
            other.gameObject.GetComponent<Interactable>().ExecuteActions();

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
            Debug.Log("Interact");
            // PostProcessing solution
            // other.gameObject.layer = 27;
            other.gameObject.GetComponent<Interactable>().ExecuteUndo();

        }

    }

    private void Update()
    {
        if(Input.GetKeyDown(raccourciClavier.toucheClavier["Action"]) && GameManager.instance.MessageInteraction.activeSelf == true)
        {
            physicaltemInventaire.AddItem(item);
            gameManager.FermerMessageInteraction();
        }
    }
}
