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
    }

    private void OnTriggerExit(Collider other)
    {
        gameManager.FermerMessageInteraction();
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
